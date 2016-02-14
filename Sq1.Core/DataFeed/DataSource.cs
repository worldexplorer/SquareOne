using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

using Newtonsoft.Json;

using Sq1.Core.DataTypes;
using Sq1.Core.Repositories;
using Sq1.Core.Charting;
using Sq1.Core.Broker;
using Sq1.Core.Backtesting;
using Sq1.Core.Streaming;
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
		[JsonIgnore]	public BacktestStreaming	StreamingAsBacktest_nullUnsafe	{ get { return this.StreamingAdapter as BacktestStreaming; } }
		[JsonIgnore]	public LivesimStreaming		StreamingAsLivesim_nullUnsafe	{ get { return this.StreamingAdapter as LivesimStreaming; } }

		[JsonProperty]	public BrokerAdapter		BrokerAdapter;
		[JsonIgnore]	public BacktestBroker		BrokerAsBacktest_nullUnsafe		{ get { return this.BrokerAdapter	as BacktestBroker; } }
		[JsonIgnore]	public LivesimBroker		BrokerAsLivesim_nullUnsafe		{ get { return this.BrokerAdapter	as LivesimBroker; } }

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
				// FAILED_FIXING_IN_DataDistributor BarStaticLast_nullUnsafe=null for freshly added Symbol
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
			barsOriginal = barsFile.BarsLoadAll_nullUnsafeThreadSafe();
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
	}
}
