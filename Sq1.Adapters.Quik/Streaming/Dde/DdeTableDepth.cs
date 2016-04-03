using System;
using System.Collections.Generic;

using Sq1.Core;
using Sq1.Core.DataTypes;
using Sq1.Core.Support;
using Sq1.Core.Streaming;

using Sq1.Adapters.Quik.Streaming.Dde.XlDde;

namespace Sq1.Adapters.Quik.Streaming.Dde {
	public class DdeTableDepth : XlDdeTableMonitoreable<LevelTwo> {
		protected override	string DdeConsumerClassName { get { return "DdeTableDepth"; } }
		static				string lockReason			{ get { return "DdeTableDepth_LOCKED_WHILE_PARSING__IncomingTable"; } }

		string			symbol;
		LevelTwo		levelTwo_fromStreamingSnap;

		public	SymbolInfo		SymbolInfo		{ get; private set; }

		public DdeTableDepth(string topic, QuikStreaming quikStreaming, List<XlColumn> columns, string symbol_passed)
							: base(topic, quikStreaming, columns) {
			this.symbol = symbol_passed;
			this.SymbolInfo = Assembler.InstanceInitialized.RepositorySymbolInfos.FindSymbolInfoOrNew(symbol_passed);
			this.levelTwo_fromStreamingSnap = base.QuikStreaming.StreamingDataSnapshot.GetLevelTwo_forSymbol_nullUnsafe(this.symbol);
			if (this.levelTwo_fromStreamingSnap == null) {
				string msg = "YOU_DIDNT_INITIALIZE_LevelTwo_FOR symbol[" + this.symbol + "] base.QuikStreaming[" + base.QuikStreaming + "]";
				throw new Exception(msg);
			}
		}
		protected override void IncomingTableBegun() {
			this.levelTwo_fromStreamingSnap.Clear_LevelTwo			(this, DdeTableDepth.lockReason);
			this.levelTwo_fromStreamingSnap.WaitAndLockFor	(this, DdeTableDepth.lockReason);
		}
		protected override void IncomingTableTerminated() {
			string msig = " //DdeTableDepth.IncomingTableTerminated()";
			this.levelTwo_fromStreamingSnap.UnLockFor		(this, DdeTableDepth.lockReason);
			string msg = "send a notify event or Chart is already Wait(indefinitely)'ing to get a FrozenHalfs  and go draw them?...";

#region moved to Level2 => never reverted anymore
			//if (this.levelTwo_pointerToStreamingSnap.Count > 0 && this.levelTwoBids_pointerToStreamingSnap.Count > 0) {
			//    List<double> asksPriceLevels_ASC = new List<double>(levelTwoAsks_pointerToStreamingSnap.SafeCopy(this, msig).Keys);
			//    double askBest_lowest =  asksPriceLevels_ASC[0];
			//    List<double> bidsPriceLevels_DESC = new List<double>(levelTwoBids_pointerToStreamingSnap.SafeCopy(this, msig).Keys);
			//    double bidBest_highest =  bidsPriceLevels_DESC[bidsPriceLevels_DESC.Count-1];
			//    if (askBest_lowest < bidBest_highest) {
			//        string msg1 = "YOUR_MOUSTACHES_GOT_REVERTED";
			//        Assembler.PopupException(msg1, null, false);
			//    }

			//    this.reconstructSpreadQuote_pushToStreaming_ifChanged(bidBest_highest, askBest_lowest);
			//} else {
			//    string msg1 = "FIRST_LEVEL2_AFTER_CTRL+L MOUSTASHES_ARE_CUT__HALF_OR_BOTH__YOU_WILL_GET_SOMETHING_UNPREDICTABLE_NOW";
			//    Assembler.PopupException(msg1 + msig, null, false);
			//}
#endregion

			msg = "DomResizeableUserControl will update by event, now";
			base.IncomingTableTerminated();
		}

		QuoteQuik quoteSpread_triggeredScript_last;
		void reconstructSpreadQuote_pushToStreaming_ifChanged(double bidBest_highest, double askBest_lowest) {
			string reason2exist = "Quik.Quotes table delivers only TRADED quotes, with .Size>0; level2 changes more frequently than trades occur"
				+ "; for the strategy to decide on quotes I deliver SPREAD"
				+ "; ideally if I receive Dde.Level2 my Quote.ServerTime will be 100% without the holes"
				+ " but and I can not skip delivering Dde.Quotes since Dde.Quote.Size adds up into Bar.Volume - in each Channel(ScaleInterval)";

			string msig = " //reconstructSpreadQuote_pushToStreaming_ifChanged(bidBest_highest[" + bidBest_highest + "], askBest_lowest[" + askBest_lowest + "])";
			// is ChartControl.chartControl_BarStreamingUpdatedMerged_ShouldTriggerRepaint_WontUpdateBtnTriggeringScriptTimeline() invoked?

			if (this.quoteSpread_triggeredScript_last != null) {
				bool ddeDelivered_level2_withoutChange_inSpread =
					this.quoteSpread_triggeredScript_last.Ask == askBest_lowest &&
					this.quoteSpread_triggeredScript_last.Bid == bidBest_highest;
				if (ddeDelivered_level2_withoutChange_inSpread) {
					string msg = "NOT_GENERATING_QUOTE_FROM_LEVEL2 ddeDelivered_level2_withoutChange_inSpread[" + ddeDelivered_level2_withoutChange_inSpread + "]";
					//Assembler.PopupException(msg + msig, null, false);
					return;
				}
			}

			string msg1 = this.symbol + " [" + bidBest_highest + "]...[" + askBest_lowest + "] GENERATING_QUOTE_FROM_LEVEL2";
			//Assembler.PopupException(msg1 + msig, null, false);

			DateTime now_usefulForPausedDebugger = DateTime.Now;
			string errorServerDateTime_alwaysEmpty = "";
			DateTime guessing_serverTime = base.QuikStreaming.ReconstructServerTime_useNowAndTimezoneFromMarketInfo(out errorServerDateTime_alwaysEmpty);

			Quote quoteLast_withServerTime_absnoPerSymbol = base.QuikStreaming.StreamingDataSnapshot.GetQuoteCurrent_forSymbol_nullUnsafe(this.symbol);
			if (quoteLast_withServerTime_absnoPerSymbol == null) {
				string msg = "FIRST_QUOTE_OF_LIVESIM_WILL_STILL_HAVE_TO_FIGURE_OUT_ITS_SERVER_TIME";
				guessing_serverTime = DateTime.MinValue;		//testing StreamingAdapter.Quote_fixServerTime_absnoPerSymbol()
			} else {
				//guessing_serverTime = quoteLast_withServerTime_absnoPerSymbol.ServerTime.AddMilliseconds(911);	// binder still can't guess time
				guessing_serverTime = DateTime.MinValue;		//YES_IT_WORKS!!! testing StreamingAdapter.Quote_fixServerTime_absnoPerSymbol()
			}

			long absnoPerSymbol = quoteLast_withServerTime_absnoPerSymbol != null ? quoteLast_withServerTime_absnoPerSymbol.AbsnoPerSymbol : 0;
			absnoPerSymbol++;
			QuoteQuik spreadChanged = new QuoteQuik(now_usefulForPausedDebugger, guessing_serverTime, this.symbol, absnoPerSymbol,
													bidBest_highest, askBest_lowest, 0, BidOrAsk.UNKNOWN);
			//spreadChanged.Bid = bidBest_highest;
			//spreadChanged.Ask = askBest_lowest;
			//spreadChanged.TradedAt = BidOrAsk.UNKNOWN;	//this quote wans't traded - it's an "artificial" I reconstructed from SpreadChanged
			//spreadChanged.Size = 0;					// Bar.Volume will decrease if you leave it as -1 (Quote.ctor() sets it to -1 intentionally)
			spreadChanged.Source = "Quik.Level2_spreadChanged";

			this.quoteSpread_triggeredScript_last = spreadChanged;
			base.QuikStreaming.PushQuoteReceived(spreadChanged);
		}

		//protected override void IncomingTableRow_convertToDataStructure(XlRowParsed row) {
		protected override LevelTwo IncomingTableRow_convertToDataStructure_monitoreable(XlRowParsed row) {
			double bidVolume	= row.GetDouble("BUY_VOLUME"	, double.NaN);
			double price		= row.GetDouble("PRICE"			, double.NaN);
			double askVolume	= row.GetDouble("SELL_VOLUME"	, double.NaN);

			if (double.IsNaN(bidVolume) == false) {		// where Blank become NaN?
				//base.QuikStreaming.StreamingDataSnapshot.LevelTwoBids_refactorBySymbol.Add(price, bidVolume, this, "IncomingRowParsedPush");
				levelTwo_fromStreamingSnap.AddBid(price, bidVolume, this, DdeTableDepth.lockReason);
			} else {
				//base.QuikStreaming.StreamingDataSnapshot.LevelTwoAsks_refactorBySymbol.Add(price, askVolume, this, "IncomingRowParsedPush");
				levelTwo_fromStreamingSnap.AddAsk(price, askVolume, this, DdeTableDepth.lockReason);
			}
			return this.levelTwo_fromStreamingSnap;		// growing each time
		}
		public override string ToString() {
			string ret = "";

			if (string.IsNullOrEmpty(this.Topic) == false) ret += this.Topic;

			ret += " "	+ this.DdeMessagesReceived;
			ret += ":"	+ this.DdeRowsReceived;

			string connectionStatus = "NEVER_CONNECTED_DDE";
			if (this.DdeConnectedTimes > 0) {
				if (this.TopicConnected) {
					connectionStatus = "LISTENING_DDE#";
					if (this.DdeMessagesReceived > 0) connectionStatus = "RECEIVING_DDE#";
				} else {
					connectionStatus = "DISCONNECTED_DDE#";
				}
				connectionStatus += this.DdeConnectedTimes;
			}

			ret += " "	+ connectionStatus;
			ret += " " + (this.ColumnsIdentified ? "columnsIdentified" : "columnsNotIdentified");
			return ret;
		}
	}
}