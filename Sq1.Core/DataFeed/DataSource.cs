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

namespace Sq1.Core.DataFeed {
	public partial class DataSource : NamedObjectJsonSerializable {
		// MOVED_TO_PARENT_NamedObjectJsonSerializable [DataMember] public new string Name;
		[JsonProperty]	public string				SymbolSelected;
		[JsonProperty]	public List<string>			Symbols;
		[JsonIgnore]	public string				SymbolsCSV			{ get {
				StringBuilder stringBuilder = new StringBuilder();
				foreach (string current in Symbols) {
					if (stringBuilder.Length > 0) stringBuilder.Append(",");
					stringBuilder.Append(current);
				}
				return stringBuilder.ToString();
			} }
		[JsonProperty]	public BarScaleInterval		ScaleInterval;
		[JsonProperty]	public StreamingProvider	StreamingProvider;
		[JsonProperty]	public BrokerProvider		BrokerProvider;
		[JsonProperty]	public string				MarketName;
		[JsonIgnore]	public MarketInfo			marketInfo;
		[JsonIgnore]	public MarketInfo			MarketInfo {
			get { return this.marketInfo; }
			set {
				this.marketInfo = value;
				MarketName = value.Name;
			} }
		[JsonProperty]	public string				StreamingProviderName	{ get {
				if (StreamingProvider == null) return "PLEASE_ATTACH_AND_CONFIGURE_STREAMING_PROVIDER_IN_DATA_SOURCE_RIGHT_CLICK_EDIT";
				//return staticProvider.GetType().Name;
				return StreamingProvider.Name;
			} }
		[JsonProperty]	public string				BrokerProviderName		{ get {
				if (BrokerProvider == null) return "PLEASE_ATTACH_AND_CONFIGURE_BROKER_PROVIDER_IN_DATA_SOURCE_RIGHT_CLICK_EDIT";
				//return staticProvider.GetType().Name;
				return BrokerProvider.Name;
			} }
		[JsonIgnore]	public bool					IsIntraday				{ get { return this.ScaleInterval.IsIntraday; } }
		[JsonIgnore]	public RepositoryBarsSameScaleInterval	BarsRepository	{ get; protected set; }
		//[JsonIgnore]	public BarsFolder			BarsFolderPerst			{ get; protected set; }
		[JsonProperty]	public string				DataSourceAbspath		{ get; protected set; }
		[JsonIgnore]	public string				DataSourcesAbspath;

		// used only by JsonDeserialize()
		public DataSource() {
			Name = "";
			MarketName = "";
			Symbols = new List<string>();
			SymbolSelected = "";
			DataSourceAbspath = "DATASOURCE_INITIALIZE_NOT_INVOKED_YET";
		}
		
		// should be used by a programmer
		public DataSource(string name, BarScaleInterval scaleInterval = null, MarketInfo marketInfo = null) : this() {
			this.Name = name;
			if (scaleInterval == null) {
				scaleInterval = new BarScaleInterval(BarScale.Minute, 5);
			}
			this.ScaleInterval = scaleInterval; 
			if (marketInfo == null) {
				marketInfo = Assembler.InstanceInitialized.RepositoryMarketInfo.FindMarketInfoOrNew("MOCK"); 
			}
			this.MarketInfo = marketInfo; 
		}
		public void Initialize(string dataSourcesAbspath, OrderProcessor orderProcessor) {
			this.DataSourcesAbspath = dataSourcesAbspath;
			this.DataSourceAbspath = Path.Combine(this.DataSourcesAbspath, this.Name);
			this.BarsRepository = new RepositoryBarsSameScaleInterval(this.DataSourceAbspath, this.ScaleInterval, true);

			foreach (string symbol in this.Symbols) {
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

			// works only for deserialized providers; for a newDataSource they are NULLs to be assigned in DataSourceEditor 
			if (this.StreamingProvider != null) {
				this.StreamingProvider.Initialize(this);
				if (this.BrokerProvider != null) {
					this.BrokerProvider.Initialize(this, this.StreamingProvider, orderProcessor);
				}
			}
		}
		public override string ToString() {
			return Name + "(" + this.ScaleInterval.ToString() + ")" + SymbolsCSV
				+ " {" + StreamingProviderName + ":" + BrokerProviderName + "}";
		}

		// internal => use only RepositoryJsonDataSource.SymbolAdd() which will notify subscribers about add operation
		internal void SymbolAdd(string symbolToAdd) {
			if (this.Symbols.Contains(symbolToAdd)) {
				throw new Exception("ALREADY_EXISTS[" + symbolToAdd + "]");
			}
			this.BarsRepository.SymbolDataFileAdd(symbolToAdd);
			this.Symbols.Add(symbolToAdd);
		}
		// internal => use only RepositoryJsonDataSource.SymbolRename() which will notify subscribers about rename operation
		internal void SymbolRename(string oldSymbolName, string newSymbolName) {
			// nope StaticProvider can subscribe to dataSourceRepository_OnSymbolRenamed() as well and do 
			//if (this.StaticProvider != null) {
			//	this.StaticProvider.SymbolRename(oldSymbolName, newSymbolName);
			//	this.Symbols = this.StaticProvider.SymbolsStored;
			//	return;
			//}

			if (this.Symbols.Contains(oldSymbolName) == false) {
				throw new Exception("OLD_SYMBOL_DOESNT_EXIST[" + oldSymbolName + "] in [" + this.Name + "]");
			}
			if (this.Symbols.Contains(newSymbolName)) {
				throw new Exception("NEW_SYMBOL_ALREADY_EXISTS[" + newSymbolName + "] in [" + this.Name + "]");
			}

			//v1
			//var replacement = new List<string>();
			//foreach (var symbol in this.Symbols) {
			//	var symbolCopy = symbol;
			//	if (symbolCopy == oldSymbolName) {
			//		symbolCopy = newSymbolName;
			//	}
			//	replacement.Add(symbolCopy);
			//}
			//this.Symbols = replacement;
			
			try {
				//v2
				bool executorProhibitedRenaming = this.RaiseSymbolRenamedExecutorShouldRenameEachBarSaveStrategyNotBars(oldSymbolName, newSymbolName);
				if (executorProhibitedRenaming) return;	// event handlers are responsible to Assembler.PopupException(), I reported MY errors above
	
				#if DEBUG
				//TESTED Debugger.Break();
				#endif
				//v3 SYMBOL_NOT_STORED_ANYMORE optimize file write time by seek to Bars.Symbol position &write FIXED-LENGTH string in header only => don't have to flush out 3Mb with bars' OHLCV;
				//Bars bars = this.RequestDataFromRepository(oldSymbolName);
				this.BarsRepository.SymbolDataFileRename(oldSymbolName, newSymbolName);
				//bars.RenameSymbol(newSymbolName);
				//this.BarsSave(bars);
				//v3 SYMBOL_NOT_STORED_ANYMORE optimize end
			} catch (Exception ex) {
				Assembler.PopupException("DataSource.SymbolRename(" + oldSymbolName + "=>" + newSymbolName + ")", ex);
			}

			//re-read for DataSourceTree use (?)
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
			this.BarsRepository.DeleteAllDataFilesAllSymbols();
			Directory.Delete(this.DataSourceAbspath);
		}
		internal void DataSourceFolderRename(string newName) {
			string msig = " DataSourceFolderRename(" + this.Name + "=>" + newName + ")";
			if (File.Exists(this.DataSourceAbspath) == false) {
				throw new Exception("DATASOURCE_OLD_FOLDER_DOESNT_EXIST this.FolderForBarDataStore[" + this.DataSourceAbspath + "]" + msig);
			}
			string abspathNewFolderName = Path.Combine(this.BarsRepository.DataSourceAbspath, newName);
			if (File.Exists(abspathNewFolderName)) {
				throw new Exception("DATASOURCE_NEW_FOLDER_ALREADY_EXISTS abspathNewFolderName[" + abspathNewFolderName + "]" + msig);
			}
			Directory.Move(this.DataSourceAbspath, abspathNewFolderName);
			this.Name = newName;
			this.DataSourceAbspath = Path.Combine(this.DataSourcesAbspath, this.Name);
			this.BarsRepository = new RepositoryBarsSameScaleInterval(this.DataSourceAbspath, this.ScaleInterval, true);
		}
		public Bars BarsLoadAndCompress(string symbolRq, BarScaleInterval scaleIntervalRq, out string millisElapsed) {
			millisElapsed = "WASNT_LOADED";

			Stopwatch readAllTimer = new Stopwatch();
			readAllTimer.Start();
	
			Bars ret;
			//BarsFolder perstFolder = new BarsFolder(this.BarsFolder.RootFolder, this.DataSource.ScaleInterval, true, "dts");
			//RepositoryBarsPerst barsPerst = new RepositoryBarsPerst(perstFolder, symbol, false);
			//ret = barsPerst.BarsRead();
			//if (ret == null) {
			RepositoryBarsFile barsFile = this.BarsRepository.DataFileForSymbol(symbolRq);
			ret = barsFile.BarsLoadAllNullUnsafeThreadSafe();
			//}

			readAllTimer.Stop();
			millisElapsed = "BarsLoadAndCompress[" + ret.Symbol + "][" + ret.Count + "](" + readAllTimer.ElapsedMilliseconds + ")ms";

			if (ret == null) ret = new Bars(symbolRq, this.ScaleInterval, "FILE_NOT_FOUND_OR_EMPTY " + this.GetType().Name);
			ret.DataSource = this;
			ret.MarketInfo = this.MarketInfo;
			ret.SymbolInfo = Assembler.InstanceInitialized.RepositorySymbolInfo.FindSymbolInfoOrNew(ret.Symbol);
			if (ret.Count == 0) return ret;
			if (scaleIntervalRq == ret.ScaleInterval) return ret;
			
			bool canConvert = ret.CanConvertTo(scaleIntervalRq);
			if (canConvert == false) {
				string msg = "CANNOT_COMPRESS_BARS " + symbolRq + "[" + ret.ScaleInterval + "]=>[" + scaleIntervalRq + "]";
				Assembler.PopupException(msg);
				return ret;
			}

			Stopwatch compressTimer = new Stopwatch();
			compressTimer.Start();

			try {
				ret = ret.ToLargerScaleInterval(scaleIntervalRq);
			} catch (Exception e) {
				Assembler.PopupException("BARS_COMPRESSION_FAILED (ret, scaleIntervalRq)", e);
				throw e;
			}

			compressTimer.Stop();
			millisElapsed += " ToLargerScaleInterval(" + scaleIntervalRq + ")[" + compressTimer.ElapsedMilliseconds + "]ms";

			return ret;
		}
		public void PausePumpingFor(Bars bars, bool wrongUsagePopup = true) {
			SymbolScaleDistributionChannel channel = this.StreamingProvider.DataDistributor.GetDistributionChannelFor(bars.Symbol, bars.ScaleInterval);
			if (channel.QuotePump.SeparatePushingThreadEnabled == false) {
				if (wrongUsagePopup == true) {
					string msg = "WILL_PAUSE_DANGEROUS_DROPPING_INCOMING_QUOTES__PUSHING_THREAD_HAVENT_STARTED (review how you use QuotePump)";
					Assembler.PopupException(msg, null, false);
				}
				channel.QuotePump.PushConsumersPaused = true;
				return;
			}
			if (channel.QuotePump.PushConsumersPaused == true) {
				if (wrongUsagePopup == true) {
					string msg = "PUSHING_THREAD_ALREADY_PAUSED (review how you use QuotePump)";
					Assembler.PopupException(msg, null, true);
				}
				return;
			}
			channel.QuotePump.PushConsumersPaused = true;
		}
		public void UnPausePumpingFor(Bars bars, bool wrongUsagePopup = true) {
			SymbolScaleDistributionChannel channel = this.StreamingProvider.DataDistributor.GetDistributionChannelFor(bars.Symbol, bars.ScaleInterval);
			if (channel.QuotePump.SeparatePushingThreadEnabled == false) {
				if (wrongUsagePopup == true) {
					string msg = "WILL_UNPAUSE_DANGEROUS_I_MIGHT_HAVE_DROPPED_ALREADY_A_FEW_QUOTES__PUSHING_THREAD_HAVENT_STARTED (review how you use QuotePump)";
					Assembler.PopupException(msg, null, false);
				}
				channel.QuotePump.PushConsumersPaused = false;
				return;
			}
			if (channel.QuotePump.PushConsumersPaused == false) {
				if (wrongUsagePopup == true) {
					string msg = "PUSHING_THREAD_ALREADY_UNPAUSED (review how you use QuotePump)";
					Assembler.PopupException(msg, null, true);
				}
				return;
			}
			channel.QuotePump.PushConsumersPaused = false;
		}

		public bool WaitUntilPumpUnpaused(Bars bars, int maxWaitingMillis = 1000) {
			DataDistributor distr = this.StreamingProvider.DataDistributor;
			SymbolScaleDistributionChannel channel = distr.GetDistributionChannelFor(bars.Symbol, bars.ScaleInterval);
			bool unpaused = channel.QuotePump.WaitUntilUnpaused(maxWaitingMillis);
			return unpaused;
		}
		public bool WaitUntilPumpPaused(Bars bars, int maxWaitingMillis = 1000) {
			DataDistributor distr = this.StreamingProvider.DataDistributor;
			SymbolScaleDistributionChannel channel = distr.GetDistributionChannelFor(bars.Symbol, bars.ScaleInterval);
			bool paused = channel.QuotePump.WaitUntilPaused(maxWaitingMillis);
			return paused;
		}
	}
}
