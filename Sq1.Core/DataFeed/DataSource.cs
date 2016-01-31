using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;

using Newtonsoft.Json;

using Sq1.Core.Broker;
using Sq1.Core.DataTypes;
using Sq1.Core.Repositories;
using Sq1.Core.Streaming;
using Sq1.Core.StrategyBase;
using Sq1.Core.Charting;
using Sq1.Core.Livesim;

namespace Sq1.Core.DataFeed {
	public partial class DataSource : NamedObjectJsonSerializable {
		// MOVED_TO_PARENT_NamedObjectJsonSerializable [DataMember] public new string Name;
		[JsonProperty]	public string				SymbolSelected;
		[JsonProperty]	public List<string>			Symbols;
		[JsonIgnore]	public string				SymbolsCSV			{ get {
				StringBuilder stringBuilder = new StringBuilder();
				foreach (string current in this.Symbols) {
					if (stringBuilder.Length > 0) stringBuilder.Append(",");
					stringBuilder.Append(current);
				}
				return stringBuilder.ToString();
			} }
		[JsonProperty]	public BarScaleInterval		ScaleInterval;
		[JsonProperty]	public StreamingAdapter		StreamingAdapter;
		[JsonProperty]	public BrokerAdapter		BrokerAdapter;
		[JsonProperty]	public string				MarketName;
		[JsonIgnore]	public MarketInfo			marketInfo;
		[JsonIgnore]	public MarketInfo			MarketInfo {
			get { return this.marketInfo; }
			set {
				this.marketInfo = value;
				MarketName = value.Name;
			} }
		[JsonProperty]	public string				StreamingAdapterName	{ get {
				//if (this.StreamingAdapter == null) return "PLEASE_ATTACH_AND_CONFIGURE_STREAMING_ADAPDER_IN_DATA_SOURCE_RIGHT_CLICK_EDIT";
				if (this.StreamingAdapter == null) return "NO_STREAMING";
				return this.StreamingAdapter.Name;
			} }
		[JsonProperty]	public string				BrokerAdapterName		{ get {
				//if (this.BrokerAdapter == null) return "PLEASE_ATTACH_AND_CONFIGURE_BROKER_ADAPDER_IN_DATA_SOURCE_RIGHT_CLICK_EDIT";
				if (this.BrokerAdapter == null) return "NO_BROKER";
				return this.BrokerAdapter.Name;
			} }
		[JsonIgnore]	public bool					IsIntraday				{ get { return this.ScaleInterval.IsIntraday; } }
		[JsonIgnore]	public RepositoryBarsSameScaleInterval	BarsRepository	{ get; protected set; }
		//[JsonIgnore]	public BarsFolder			BarsFolderPerst			{ get; protected set; }
		[JsonProperty]	public string				DataSourceAbspath		{ get; protected set; }
		[JsonIgnore]	public string				DataSourcesAbspath;

		//[JsonIgnore]	public Dictionary<string, List<ChartShadow>>		ChartsOpenForSymbol { get; private set; }
		//[JsonIgnore]	public List<SymbolOfDataSource>	SymbolsWithBackRef	{ get {
		//    List<SymbolOfDataSource> ret = new List<SymbolOfDataSource>();
		//    foreach (string symbol in this.Symbols) {
		//        ret.Add(new SymbolOfDataSource(symbol, this));
		//    }
		//    return ret;
		//} }
		[JsonIgnore]	public DictionaryManyToOne<SymbolOfDataSource, ChartShadow>		ChartsOpenForSymbol { get; private set; }

		// used only by JsonDeserialize()
		public DataSource() {
			Name = "";
			MarketName = "";
			Symbols = new List<string>();
			SymbolSelected = "";
			DataSourceAbspath = "DATASOURCE_INITIALIZE_NOT_INVOKED_YET";
			//ChartsOpenForSymbol = new Dictionary<string, List<ChartShadow>>();
			ChartsOpenForSymbol = new DictionaryManyToOne<SymbolOfDataSource, ChartShadow>();
		}
		
		// should be used by a programmer
		public DataSource(string name, BarScaleInterval scaleInterval = null, MarketInfo marketInfo = null) : this() {
			this.Name = name;
			if (scaleInterval == null) {
				scaleInterval = new BarScaleInterval(BarScale.Minute, 5);
			}
			this.ScaleInterval = scaleInterval; 
			if (marketInfo == null) {
				marketInfo = Assembler.InstanceInitialized.RepositoryMarketInfos.FindMarketInfoOrNew("MOCK"); 
			}
			this.MarketInfo = marketInfo; 
		}
		public void Initialize(string dataSourcesAbspath, OrderProcessor orderProcessor) {
			this.DataSourcesAbspath = dataSourcesAbspath;
			this.DataSourceAbspath = Path.Combine(this.DataSourcesAbspath, this.Name);
			this.BarsRepository = new RepositoryBarsSameScaleInterval(this.DataSourceAbspath, this.ScaleInterval, true);

			foreach (string symbol in this.Symbols) {
				//this.ChartsOpenForSymbol.Add(symbol, new List<ChartShadow>());
				this.ChartsOpenForSymbol.Register(new SymbolOfDataSource(symbol, this));

				if (this.BarsRepository.DataFileExistsForSymbol(symbol)) continue;
				Bars barsEmpty = new Bars(symbol, this.ScaleInterval, "DISCOVERED_NON_EXISTING");
				// FAILED_FIXING_IN_DataDistributor BarStaticLastNullUnsafe=null for freshly added Symbol
				//barsEmpty.BarAppendBindStatic(new Bar(symbol, this.ScaleInterval, DateTime.Now));
				//barsEmpty.BarCreateAppendBindStatic(DateTime.Now, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN);

				string millisElapsed;
				int mustBeZero = this.BarsSave(barsEmpty, out millisElapsed, true);
				Assembler.PopupException("BARS_INITIALIZED_EMPTY[" + mustBeZero + "] " + millisElapsed + " " + barsEmpty.ToString(), null, false);
			}

			//this.BarsFolderPerst = new BarsFolder(this.FolderForBarDataStore, this.ScaleInterval, true, "dts");
			this.Initialize(orderProcessor);
		}
		public void Initialize(OrderProcessor orderProcessor) {
			// works only for deserialized adapters; for a newDataSource they are NULLs to be assigned in DataSourceEditor 
			if (this.StreamingAdapter == null) {
				//v1 return;
				string reasonToBeCreated = "NEW_DATASOURCE_IS_AUTOMATICALLY_SERVED_BY_LivesimStreamingDefault";
				this.StreamingAdapter = new LivesimStreamingDefault(reasonToBeCreated);
				Assembler.PopupException(reasonToBeCreated, null, false);
			}
			this.StreamingAdapter.InitializeDataSource_inverse(this);
			if (this.BrokerAdapter == null) {
				//v1 return;
				string reasonToBeCreated = "NEW_DATASOURCE_IS_AUTOMATICALLY_SERVED_BY_LivesimBrokerDefault";
				this.BrokerAdapter = new LivesimBrokerDefault(reasonToBeCreated);
				Assembler.PopupException(reasonToBeCreated, null, false);
			}
			this.BrokerAdapter.InitializeDataSource_inverse(this, this.StreamingAdapter, orderProcessor);
		}
		public override string ToString() {
			return this.Name + "(" + this.ScaleInterval.ToString() + ")" + this.SymbolsCSV
				+ " {" + this.StreamingAdapterName + ":" + this.BrokerAdapterName + "}";
		}

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
				bool executorProhibitedRenaming = this.RaiseSymbolRenamedExecutorShouldRenameEachBarSaveStrategyNotBars(oldSymbolName, newSymbolName);
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
		internal int BarAppendOrReplaceLast(Bar barLastFormed, out string millisElapsed) {
			int ret = -1;
			millisElapsed = "BAR_WASNT_SAVED";
			if (barLastFormed == null) {
				millisElapsed += " barLastFormed=null"; 
				return ret;
			}

			if (this.ScaleInterval != barLastFormed.ScaleInterval) return ret;
			if (this.Symbols.Contains(barLastFormed.Symbol) == false) return ret;
			if (this.BarsRepository == null) return ret;
			try {
				barLastFormed.CheckOHLCVthrow();
			} catch (Exception ex) {
				Assembler.PopupException("WONT_ADD_TO_BAR_FILE DataSource.BarAppend(" + barLastFormed + ")", ex, false);
				return ret;
			}

			RepositoryBarsFile file = this.BarsRepository.DataFileForSymbol(barLastFormed.Symbol);
			//v1 TESTED LIMITED_TO_APPEND_ONLY__SUITS_FOR_APPENDING_NOT_REPLACING_LAST_STORED_BAR ret = file.BarAppendStaticUnconditionalThreadSafe(barLastFormed);
			//v2 experimental
			Stopwatch replaceLastBarTimer = new Stopwatch();
			replaceLastBarTimer.Start();
			ret = file.BarAppendStaticOrReplaceStreamingThreadSafe(barLastFormed);
			replaceLastBarTimer.Stop();
			millisElapsed = "BarAppendOrReplaceLast[" + barLastFormed.Symbol + "][" + ret + "](" + replaceLastBarTimer.ElapsedMilliseconds + ")ms";

			return ret;
		}
		public int BarsSave(Bars bars, out string millisElapsed, bool createIfDoesntExist = false) {
			millisElapsed = "BARS_WERENT_SAVED";
		
			RepositoryBarsFile barsFile = this.BarsRepository.DataFileForSymbol(bars.Symbol, false, createIfDoesntExist);
			Stopwatch replaceLastBarTimer = new Stopwatch();
			replaceLastBarTimer.Start();

			int barsSaved = barsFile.BarsSaveThreadSafe(bars);

			replaceLastBarTimer.Stop();
			millisElapsed = "BarsSave[" + bars.Count + "][" + barsSaved + "](" + replaceLastBarTimer.ElapsedMilliseconds + ")ms";

			string msg = "Saved [ " + barsSaved + "]" + millisElapsed + "; static[" + this.Name + "]";

			//BarsFolder perstFolder = new BarsFolder(this.BarsFolder.RootFolder, bars.ScaleInterval, true, "dts");
			//RepositoryBarsPerst barsPerst = new RepositoryBarsPerst(perstFolder, bars.Symbol, false);
			//int barsSavedPerst = barsPerst.BarsSave(bars);
			//string msgPerst = "Saved [ " + barsSavedPerst + "] bars; static[" + this.Name + "]";
			return barsSaved;
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
		public Bars BarsLoadAndCompress(string symbolRq, BarScaleInterval scaleIntervalRq, out string millisElapsed) {
			millisElapsed = "WASNT_LOADED";

			Stopwatch readAllTimer = new Stopwatch();
			readAllTimer.Start();
	
			Bars barsOriginal;
			//BarsFolder perstFolder = new BarsFolder(this.BarsFolder.RootFolder, this.DataSource.ScaleInterval, true, "dts");
			//RepositoryBarsPerst barsPerst = new RepositoryBarsPerst(perstFolder, symbol, false);
			//ret = barsPerst.BarsRead();
			//if (ret == null) {
			RepositoryBarsFile barsFile = this.BarsRepository.DataFileForSymbol(symbolRq);
			barsOriginal = barsFile.BarsLoadAllNullUnsafeThreadSafe();
			//}

			readAllTimer.Stop();
			if (barsOriginal == null) {
				barsOriginal = new Bars(symbolRq, this.ScaleInterval, "FILE_NOT_FOUND_OR_EMPTY " + this.GetType().Name);
				string msg = "BARS_NULL " + barsFile.Abspath + " //BarsLoadAndCompress(" + symbolRq + ":" + this.BarsRepository.ScaleInterval + "][NULL]bars[" + readAllTimer.ElapsedMilliseconds + "]msRead";
				Assembler.PopupException(msg);
			}
			barsOriginal.DataSource = this;
			barsOriginal.MarketInfo = this.MarketInfo;
			barsOriginal.SymbolInfo = Assembler.InstanceInitialized.RepositorySymbolInfos.FindSymbolInfoOrNew(barsOriginal.Symbol);

			millisElapsed = "BarsLoadAndCompress[" + barsOriginal.Symbol + ":" + barsOriginal.ScaleInterval + "]["
				+ barsOriginal.Count + "]bars[" + readAllTimer.ElapsedMilliseconds + "]msRead";
			if (barsOriginal.Count == 0) return barsOriginal;
			if (scaleIntervalRq == barsOriginal.ScaleInterval) return barsOriginal;

			bool canConvert = barsOriginal.ScaleInterval.CanConvertTo(scaleIntervalRq);
			if (canConvert == false) {
				string msg = "CANNOT_INCREASE_GRANULARITY " + symbolRq + "[" + barsOriginal.ScaleInterval + "]=>[" + scaleIntervalRq + "]";
				Assembler.PopupException(msg);
				return barsOriginal;
			}

			Stopwatch compressTimer = new Stopwatch();
			compressTimer.Start();

			Bars barsCompressed;
			try {
				barsCompressed = barsOriginal.ToLargerScaleInterval(scaleIntervalRq);
			} catch (Exception e) {
				Assembler.PopupException("BARS_COMPRESSION_FAILED (ret, scaleIntervalRq)", e);
				throw e;
			}

			compressTimer.Stop();
			millisElapsed += " ToLargerScaleInterval[" + barsCompressed.Symbol + ":" + barsCompressed.ScaleInterval + "][" + barsCompressed.Count + "]bars[" + compressTimer.ElapsedMilliseconds + "]msCompressed";

			return barsCompressed;
		}
		public bool PumpPause_freezeOtherLiveChartsExecutors_toLetMyOrderExecutionCallbacksGoFirst(ScriptExecutor executor, bool wrongUsagePopup = true) {
			SymbolScaleDistributionChannel channel = this.StreamingAdapter.DataDistributor_replacedForLivesim
				.GetDistributionChannelFor_nullUnsafe(executor.Bars.Symbol, executor.Bars.ScaleInterval);
			string msig = " //PumpPause_freezeOtherLiveChartsExecutors_toLetMyOrderExecutionCallbacksGoFirst(" + executor + ")";
			if (channel == null) {
				string msg = "NOT_AN_ERROR__BACKTESTER_EMPLOYS_OWN_QUEUE__NO_CHART_NOR_SOLIDIFIER_AT_TIMEFRAME_DIFFERENT_TO_DS'S_CAN_BE_POSSIBLE"
					//+ " THERE_MUSTBE_AT_LEAST_ONE_EXECUTOR_THAT_INVOKED_ME_UPSTACK"
					;
				//Assembler.PopupException(msg + msig);
				return false;
			}
			if (channel.QuoteQueue_onlyWhenBacktesting == null) {
				return false;
			}
			if (channel.QuoteQueue_onlyWhenBacktesting.HasSeparatePushingThread == false) {
				if (wrongUsagePopup == true) {
					string msg = "WILL_PAUSE_DANGEROUS_DROPPING_INCOMING_QUOTES__PUSHING_THREAD_HAVENT_STARTED (review how you use QuotePump)";
					Assembler.PopupException(msg + msig);
				}
				return false;
			}
			channel.PumpPause_addBacktesterLaunchingScript_eachQuote(executor.BacktesterOrLivesimulator);
			return true;
		}
		public bool PumpResume_unfreezeOtherLiveChartsExecutors_toLetMyOrderExecutionCallbacksGoFirst(ScriptExecutor executor, bool wrongUsagePopup = true) {
			string msig = " //PumpResume_unfreezeOtherLiveChartsExecutors_toLetMyOrderExecutionCallbacksGoFirst(" + executor + ")";
			if (Assembler.InstanceInitialized.MainFormClosingIgnoreReLayoutDockedForms) {
				string msg = "I_REFUSE_TO_RESUME_PUMP_BECAUSE_IT_LEADS_TO_DEADLOCK IM_CLOSING_MAINFORM_WHILE_LIVESIM_IS_RUNNING";
				Assembler.PopupException(msg + msig, null, false);
				return false;
			}

			SymbolScaleDistributionChannel channel = this.StreamingAdapter.DataDistributor_replacedForLivesim
				.GetDistributionChannelFor_nullUnsafe(executor.Bars.Symbol, executor.Bars.ScaleInterval);
			if (channel == null) {
				string msg = "NOT_AN_ERROR__BACKTESTER_EMPLOYS_OWN_QUEUE__NO_CHART_NOR_SOLIDIFIER_AT_TIMEFRAME_DIFFERENT_TO_DS'S_CAN_BE_POSSIBLE"
					//+ " THERE_MUSTBE_AT_LEAST_ONE_EXECUTOR_THAT_INVOKED_ME_UPSTACK"
					;
				//Assembler.PopupException(msg + msig);
				return false;
			}

			if (channel.QuoteQueue_onlyWhenBacktesting.HasSeparatePushingThread == false) {
				if (wrongUsagePopup == true) {
					string msg = "WILL_UNPAUSE_DANGEROUS_I_MIGHT_HAVE_DROPPED_ALREADY_A_FEW_QUOTES__PUSHING_THREAD_HAVENT_STARTED (review how you use QuotePump)";
					Assembler.PopupException(msg + msig, null, false);
				}
				return false;
			}
			channel.PumpResume_removeBacktesterFinishedScript_eachQuote(executor.BacktesterOrLivesimulator);
			return true;
		}

		//public bool PumpingPausedGet(Bars bars) {
		//    DataDistributor distr = this.StreamingAdapter.DataDistributor;
		//    SymbolScaleDistributionChannel channel = distr.GetDistributionChannelFor_nullUnsafe(bars.Symbol, bars.ScaleInterval);
		//    bool paused = channel.QuotePump.Paused;
		//    return paused;
		//}
		//public bool PumpingWaitUntilUnpaused(Bars bars, int maxWaitingMillis = 1000) {
		//    DataDistributor distr = this.StreamingAdapter.DataDistributor;
		//    SymbolScaleDistributionChannel channel = distr.GetDistributionChannelFor_nullUnsafe(bars.Symbol, bars.ScaleInterval);
		//    bool unpaused = channel.QuotePump.WaitUntilUnpaused(maxWaitingMillis);
		//    return unpaused;
		//}
		public bool PumpingWaitUntilPaused(Bars bars, int maxWaitingMillis = 1000) {
			DataDistributor distr = this.StreamingAdapter.DataDistributor_replacedForLivesim;
			SymbolScaleDistributionChannel channel = distr.GetDistributionChannelFor_nullUnsafe(bars.Symbol, bars.ScaleInterval);
			bool paused = channel.QuoteQueue_onlyWhenBacktesting.WaitUntilPaused(maxWaitingMillis);
			return paused;
		}
	}
}
