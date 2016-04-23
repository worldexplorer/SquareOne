using System;
using System.Collections.Generic;
using System.Drawing;

using Newtonsoft.Json;

using Sq1.Core.DataTypes;
using Sq1.Core.DataFeed;
using Sq1.Core.Livesim;

namespace Sq1.Core.Streaming {
	public abstract partial class StreamingAdapter : IDisposable {
		[JsonIgnore]	public		string					Name								{ get; protected set; }
		[JsonIgnore]	public		string					ReasonToExist						{ get; protected set; }
		[JsonIgnore]	public		Bitmap					Icon								{ get; protected set; }
		[JsonIgnore]	public		DataSource				DataSource							{ get; protected set; }
		[JsonIgnore]	public		string					marketName							{ get { return this.DataSource.MarketInfo.Name; } }
		[JsonIgnore]	public		StreamingDataSnapshot	StreamingDataSnapshot				{ get; protected set; }

		[JsonProperty]	public		int						Level2RefreshRateMs;
		[JsonIgnore]	public		bool					QuotePumpSeparatePushingThreadEnabled	{ get; protected set; }
		[JsonIgnore]	public		LivesimStreaming		LivesimStreaming_ownImplementation		{ get; protected set; }

		// public for assemblyLoader: Streaming-derived.CreateInstance();
		public StreamingAdapter() {
			ReasonToExist							= "DUMMY_FOR_LIST_OF_STREAMING_PROVIDERS_IN_DATASOURCE_EDITOR";
			SymbolsSubscribedLock					= new object();
			SymbolsUpstreamSubscribed				= new List<string>();

			StreamingDataSnapshot					= new StreamingDataSnapshot(this);			// not in DataSource because YOU may want to implement your own Level2, e.g. for Options (3D, containing multiple Strikes for one Symbol, to let the strategy choose "the best" strike for emiting Orders)
			StreamingSolidifier_oneForAllSymbols	= new StreamingConsumerSolidifier();		// not in DataSource because YOU may want to create a StreamingAdapter that doean't save incoming quotes directly to file, but YOU will save some crazy composite index / Synthetic-you-own Symbol (collected across multiple StreamingAdapters) separately

			Level2RefreshRateMs						= 200;
			QuotePumpSeparatePushingThreadEnabled	= true;		// set FALSE for Queue-based BacktestStreamingAdapter(s)
			LivesimStreaming_ownImplementation		= null;		// be careful addressing it! valid only for StreamingAdapter-derived!!!
			//if (this is LivesimStreaming) return;
			//NULL_UNTIL_QUIK_PROVIDES_OWN_DDE_REDIRECTOR LivesimStreamingImplementation					= new LivesimStreamingDefault(true, "USED_FOR_LIVESIM_ON_DATASOURCES_WITHOUT_ASSIGNED_STREAMING");	// QuikStreaming replaces it to DdeGenerator + QuikPuppet
		}

		public StreamingAdapter(string reasonToExist) : this() {
			ReasonToExist	= reasonToExist;
			this.CreateDistributors_onlyWhenNecessary(reasonToExist);
		}

		public virtual void InitializeFromDataSource(DataSource dataSource) {
			this.DataSource = dataSource;
			this.StreamingDataSnapshot.Initialize_levelTwo_lastPrevQuotes_forAllSymbols_inDataSource(this.DataSource.Symbols);
			this.UpstreamConnectionState = ConnectionState.Streaming_JustInitialized_solidifiersUnsubscribed;
		}

		public override string ToString() {
			string dataSourceAsString = this.DataSource != null ? this.DataSource.ToString() : "NOT_INITIALIZED_YET";
			string ret = this.Name + "/[" + this.UpstreamConnectionState + "]"
				+ ": UpstreamSymbols[" + this.SymbolsUpstreamSubscribedAsString + "]"
				//+ "DataSource[" + dataSourceAsString + "]"
				+ " (" + this.ReasonToExist + ")"
				;
			return ret;
		}

		//internal void FactoryStreamingBar_absorbFromStream_onBacktestComplete(StreamingAdapter streamingBacktest, string symbol, BarScaleInterval barScaleInterval) {
		//    SymbolScaleStream<StreamingConsumerChart> streamBacktest = streamingBacktest.DistributorCharts_substitutedDuringLivesim.GetStreamFor_nullUnsafe(symbol, barScaleInterval);
		//    if (streamBacktest == null) return;
		//    Bar barLastFormedBacktest = streamBacktest.UnattachedStreamingBar_factoryPerSymbolScale.BarLastFormedUnattached_nullUnsafe;
		//    if (barLastFormedBacktest == null) return;

		//    Bar barStreamingBacktest = streamBacktest.UnattachedStreamingBar_factoryPerSymbolScale.BarStreaming_unattached;

		//    SymbolScaleStream<StreamingConsumerChart> streamOriginal = this.DistributorCharts_substitutedDuringLivesim.GetStreamFor_nullUnsafe(symbol, barScaleInterval);
		//    if (streamOriginal == null) return;
		//    Bar barLastFormedOriginal = streamOriginal.UnattachedStreamingBar_factoryPerSymbolScale.BarLastFormedUnattached_nullUnsafe;
		//    //if (barLastFormedOriginal == null) return;

		//    streamOriginal.UnattachedStreamingBar_factoryPerSymbolScale.BarLastStatic_absorbFromStream_onBacktestComplete(streamBacktest);
		//    Bar barLastFormedAbsorbed = streamOriginal.UnattachedStreamingBar_factoryPerSymbolScale.BarLastFormedUnattached_nullUnsafe;
		//    if (barLastFormedOriginal == null || barLastFormedAbsorbed.DateTimeOpen != barLastFormedOriginal.DateTimeOpen) {
		//        string msg = "GUT";
		//    }

		//    Bar barStreamingOriginal = streamOriginal.UnattachedStreamingBar_factoryPerSymbolScale.BarStreaming_unattached;
		//    streamOriginal.UnattachedStreamingBar_factoryPerSymbolScale.BarStreaming_absorbFromStream_onBacktestComplete(streamBacktest);
		//    Bar barStreamingAbsorbed = streamOriginal.UnattachedStreamingBar_factoryPerSymbolScale.BarStreaming_unattached;
		//    if (barStreamingAbsorbed == null || barStreamingAbsorbed.DateTimeOpen != barStreamingOriginal.DateTimeOpen) {
		//        string msg = "GUT";
		//    }
		//    return;
		//}
		

		public bool IsDisposed { get; private set; }
		public virtual void Dispose() {
			if (this.IsDisposed) {
				string msg = "ALREADY_DISPOSED__DONT_INVOKE_ME_TWICE  " + this.ToString();
				Assembler.PopupException(msg);
				return;
			}
		}
	}
} 