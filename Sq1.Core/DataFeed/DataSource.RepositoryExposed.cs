using System;
using System.IO;
using System.Collections.Generic;

using Sq1.Core.Repositories;
using Sq1.Core.Charting;
using Sq1.Core.DataTypes;

namespace Sq1.Core.DataFeed {
	public partial class DataSource {

		// internal => use only RepositoryJsonDataSource.SymbolAdd() which will notify subscribers about add operation
		internal void SymbolAdd(string symbolToAdd) {
			if (this.Symbols.Contains(symbolToAdd)) {
				throw new Exception("ALREADY_EXISTS[" + symbolToAdd + "]");
			}
			this.BarsRepository.SymbolDataFileAdd(symbolToAdd);
			this.Symbols.Add(symbolToAdd);
			//NOPE_POSTPONED_AS_ATOMIC_KEY+FIRST_CONTENT this.ChartsOpenForSymbol.Add(symbolToAdd, new List<ChartShadow>());
			// RepositoryJsonDataSource.RaiseOnSymbolAdded()_WILL_NOTIFY_DATASOURCE_TREE_UPSTACK this.DataSourceEdited_treeShouldRebuild(this);
		}
		internal void SymbolCopyOrCompressFrom(DataSource dataSourceFrom, string symbolToCopy, DataSource dataSourceTo) {
			string msig = " // DataSource[" + this.Name + "].SymbolCopyOrCompressFrom(" + dataSourceFrom.Name + ", " + symbolToCopy + ") ";
			if (this.Symbols.Contains(symbolToCopy)) {
				throw new Exception("ALREADY_EXISTS[" + symbolToCopy + "]" + msig);
			}
			if (dataSourceFrom.ScaleInterval.CanConvertTo(dataSourceTo.ScaleInterval) == false) {
				throw new Exception("CANT_CONVERT_TIMEFRAMES_TO_MORE_GRANULAR " + dataSourceFrom.Name + "[" + dataSourceFrom.ScaleInterval + "]=> " + dataSourceTo.Name + "[" + dataSourceTo.ScaleInterval + "]" + msig);
			}
			if (dataSourceFrom.ScaleInterval.AsTimeSpanInSeconds == dataSourceTo.ScaleInterval.AsTimeSpanInSeconds) {
				string abspathSource = dataSourceFrom.BarsRepository.AbspathForSymbol(symbolToCopy);
				this.BarsRepository.SymbolDataFileCopy(symbolToCopy, abspathSource);
				RepositoryBarsFile filePickedUp = this.BarsRepository.DataFileForSymbol(symbolToCopy);
				Assembler.PopupException("BARS_SAVED_UNCOMPRESSED: " + filePickedUp.BarsLoadAllNullUnsafeThreadSafe().Count + msig, null, false);
			} else {
				string millisElapsedLoadCompress;
				Bars barsCompressed = dataSourceFrom.BarsLoadAndCompress(symbolToCopy, dataSourceTo.ScaleInterval, out millisElapsedLoadCompress);
				this.BarsRepository.SymbolDataFileAdd(symbolToCopy, true);
				RepositoryBarsFile fileToSaveTo = this.BarsRepository.DataFileForSymbol(symbolToCopy);
				int barsSaved = fileToSaveTo.BarsSaveThreadSafe(barsCompressed);
				Assembler.PopupException("BARS_SAVED_COMPRESSED: " + barsSaved + msig + millisElapsedLoadCompress, null, false);
			}
			this.Symbols.Add(symbolToCopy);
		}
		// internal => use only RepositoryJsonDataSource.SymbolRename() which will notify subscribers about rename operation
		internal void SymbolRename(string oldSymbolName, string newSymbolName) {
			if (this.Symbols.Contains(oldSymbolName) == false) {
				throw new Exception("OLD_SYMBOL_DOESNT_EXIST[" + oldSymbolName + "] in [" + this.Name + "]");
			}
			if (this.Symbols.Contains(newSymbolName)) {
				throw new Exception("NEW_SYMBOL_ALREADY_EXISTS[" + newSymbolName + "] in [" + this.Name + "]");
			}

			try {
				bool executorProhibitedRenaming = this.RaiseOnSymbolRenamed_eachExecutorShouldRenameItsBars_saveStrategyIfNotNull(oldSymbolName, newSymbolName);
				if (executorProhibitedRenaming) return;	// event handlers are responsible to Assembler.PopupException(), I reported MY errors above
	
				this.BarsRepository.SymbolDataFileRename(oldSymbolName, newSymbolName);

				// DUMB_AND_ERROR_PRONE
				//List<ChartShadow> chartsForOldSymbol = this.ChartsOpenForSymbol[oldSymbolName];
				//this.ChartsOpenForSymbol.Add(newSymbolName, chartsForOldSymbol);
				//this.ChartsOpenForSymbol.Remove(oldSymbolName);
				SymbolOfDataSource oldSymbolOfDataSource = this.ChartsOpenForSymbol.FindSimilarKey(new SymbolOfDataSource(oldSymbolName, this));
				if (oldSymbolOfDataSource != null) {
					this.ChartsOpenForSymbol.RenameKey(oldSymbolOfDataSource, new SymbolOfDataSource(newSymbolName, this));
				}
				// RepositoryJsonDataSource.RaiseOnSymbolRenamed()_WILL_NOTIFY_DATASOURCE_TREE_UPSTACK this.DataSourceEdited_treeShouldRebuild(this);
			} catch (Exception ex) {
				Assembler.PopupException("DataSource.SymbolRename(" + oldSymbolName + "=>" + newSymbolName + ")", ex);
			}

			// outside the try{} block to keep UI with latest changes
			this.Symbols = this.BarsRepository.SymbolsInScaleIntervalSubFolder;
		}
		public void SymbolsRebuildReadDataSourceSubFolderAfterDeserialization() {
			this.Symbols = this.BarsRepository.SymbolsInScaleIntervalSubFolder;
		}
		// internal => use only RepositoryJsonDataSource.SymbolRemove() which will notify subscribers about remove operation
		internal void SymbolRemove(string symbolToDelete) {
			if (this.Symbols.Contains(symbolToDelete) == false) {
				throw new Exception("ALREADY_DELETED[" + symbolToDelete + "] in [" + this.Name + "]");
			}
			this.Symbols.Remove(symbolToDelete);
			this.BarsRepository.SymbolDataFileDelete(symbolToDelete);

			List<ChartShadow> chartsForOldSymbol = this.ChartsOpenForSymbol.FindContentsForSimilarKey_NullUnsafe(new SymbolOfDataSource(symbolToDelete, this));
			if (chartsForOldSymbol != null) {
				string msg = "SHOULD_I_CLOSE_THE_CHARTS_OPEN_WITH_SYMBOL? symbolToDelete[" + symbolToDelete + "]";
				Assembler.PopupException(msg);
			} else {
				//this.ChartsOpenForSymbol.Remove(symbolToDelete);
				this.ChartsOpenForSymbol.UnRegisterSimilar(new SymbolOfDataSource(symbolToDelete, this));
				// RepositoryJsonDataSource.RaiseOnSymbolRemovedDone()_WILL_NOTIFY_DATASOURCE_TREE_UPSTACK this.DataSourceEdited_treeShouldRebuild(this);
			}
		}


		// Initialize() creates the folder, now create empty files for non-file-existing-symbols
		internal int CreateDeleteBarFilesToSymbolsDeserialized() {
			foreach (string symbolToAdd in this.Symbols) {
				if (this.BarsRepository.DataFileExistsForSymbol(symbolToAdd)) continue;
				this.BarsRepository.SymbolDataFileAdd(symbolToAdd);
			}
			List<string> symbolsToDelete = new List<string>();
			foreach (string symbolWhateverCase in this.BarsRepository.SymbolsInScaleIntervalSubFolder) {
				string symbol = symbolWhateverCase.ToUpper();
				if (this.Symbols.Contains(symbol)) continue;
				symbolsToDelete.Add(symbol);
			}
			foreach (string symbolToDelete in symbolsToDelete) {
				this.BarsRepository.SymbolDataFileDelete(symbolToDelete);
			}
			return symbolsToDelete.Count;
		}
		internal void DataSourceFolderDeleteWithSymbols() {
			if (this.BarsRepository == null) {
				string msg = "DATASOURCE_INITIALIZE_NOT_INVOKED_YET //DataSourceFolderDeleteWithSymbols()";
				Assembler.PopupException(msg);
			}
			this.BarsRepository.DeleteAllDataFilesAllSymbols();
			Directory.Delete(this.DataSourceAbspath);
		}
		internal bool DataSourceFolderRename(string newName) {
			bool ret = false;
			string msig = " DataSourceFolderRename(" + this.Name + "=>" + newName + ")";
			if (Directory.Exists(this.DataSourceAbspath) == false) {
				throw new Exception("DATASOURCE_OLD_FOLDER_DOESNT_EXIST this.FolderForBarDataStore[" + this.DataSourceAbspath + "]" + msig);
			}
			string abspathNewFolderName = Path.Combine(this.DataSourcesAbspath, newName);
			if (Directory.Exists(abspathNewFolderName)) {
				string abspathNewRandomFolderName = Path.Combine(this.DataSourcesAbspath, newName + "-OutOfMyWay-" + new Random().Next(1000000, 9999999));
				int newNameGenTrialsDone = 0;
				int newNameGenTrialsLimit = 1000;
				while (Directory.Exists(abspathNewRandomFolderName)) {
					abspathNewRandomFolderName = Path.Combine(this.DataSourcesAbspath, newName + "-OutOfMyWay-" + new Random().Next(1000000, 9999999));
					newNameGenTrialsDone++;
					if (newNameGenTrialsDone >= newNameGenTrialsLimit) {
						string fatal = "CHECK_YOUR_FOLDER_IT_HAS_WAY_TOO_MANY_-OutOfMyWay-_FOLDERS LAST_EXISTING[" + abspathNewRandomFolderName + "]"
							+ " newNameGenTrialsDone[" + newNameGenTrialsDone + "] >= newNameGenTrialsLimit[" +  newNameGenTrialsLimit + "]";
						//throw new Exception(fatal + msig);
						Assembler.PopupException(fatal + msig);
						return ret;
					}
				}
				string msg = "DATASOURCE_NEW_FOLDER_ALREADY_EXISTS abspathNewFolderName[" + abspathNewFolderName + "]=>renamingToRandom[" + abspathNewRandomFolderName + "] TO_GET_CLEAR_WAY";
				try {
					Directory.Move(abspathNewFolderName, abspathNewRandomFolderName);
					Assembler.PopupException(msg + msig);
				} catch (Exception ex) {
					msg = "RENAME_FAILED__GRANT_YOURSELF_FULL_CONTROL_TO_FOLDER this.DataSourcesAbspath[" + this.DataSourcesAbspath + "] " + msg;
					Assembler.PopupException(msg + msig, ex);
					return ret;
				}
			}
			try {
				Directory.Move(this.DataSourceAbspath, abspathNewFolderName);
			} catch (Exception ex) {
				string msg = "RENAME_FAILED__GRANT_YOURSELF_FULL_CONTROL_TO_FOLDER this.DataSourcesAbspath[" + this.DataSourcesAbspath + "] "
					+ " Directory.Move(" + this.DataSourceAbspath + "=>" + abspathNewFolderName + ")";
				Assembler.PopupException(msg + msig, ex);
				return ret;
			}
			this.Name = newName;
			this.DataSourceAbspath = abspathNewFolderName;
			this.BarsRepository = new RepositoryBarsSameScaleInterval(this.DataSourceAbspath, this.ScaleInterval, true);
			ret = true;
			return ret;
		}

	}
}
