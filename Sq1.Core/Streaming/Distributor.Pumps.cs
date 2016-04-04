using System;
using System.Collections.Generic;

using Sq1.Core.DataTypes;
using Sq1.Core.Charting;
using Sq1.Core.Backtesting;
using Sq1.Core.Livesim;

namespace Sq1.Core.Streaming {
	public partial class Distributor<STREAMING_CONSUMER_CHILD> {

		internal void PumpPause_forSymbolLivesimming(string symbolLivesimming, string reasonForNewDistributor) {
			string msig = " //PumpPause_forSymbolLivesimming(" + symbolLivesimming + "," + reasonForNewDistributor + ") DISTRIBUTOR[" + this.ReasonIwasCreated + "]";
			string msg = "I_REFUSE_TO_PAUSE_PUMP ";
			if (this.ChannelsBySymbol.ContainsKey(symbolLivesimming) == false) {
				msg += "CHANNEL_DOESNT_EXIST_FOR_SYMBOL";
				Assembler.PopupException(msg + msig);
				return;
			}
			SymbolChannel<STREAMING_CONSUMER_CHILD> channel = this.ChannelsBySymbol[symbolLivesimming];
			if (channel.QuotePump_nullUnsafe == null) {
				msg += "PUMP_NULL_ONLY_WHEN_BACKTESTING";
				Assembler.PopupException(msg + msig);
				return;
			}
			if (channel.QuotePump_nullUnsafe.Paused) {
				msg += "PUSHING_THREAD_ALREADY_PAUSED";
				Assembler.PopupException(msg + msig);
				return;
			}
			channel.QuotePump_nullUnsafe.PusherPause_waitUntilPaused();
			//channel.QuotePump_nullUnsafe.WaitUntilPaused();
			msg = "PUMP_UNPAUSED_CONFIRMED";
			Assembler.PopupException(msg + msig, null, false);
		}
		internal void PumpUnpause_forSymbolLivesimming(string symbolLivesimming, string reasonForStoppingReplacedDistributor) {
			string msig = " //PumpUnpause_forSymbolLivesimming(" + symbolLivesimming + "," + reasonForStoppingReplacedDistributor + ") DISTRIBUTOR[" + this.ReasonIwasCreated + "]";
			string msg = "I_REFUSE_TO_UNPAUSE_PUMP ";
			if (this.ChannelsBySymbol.ContainsKey(symbolLivesimming) == false) {
				msg += "CHANNEL_DOESNT_EXIST_FOR_SYMBOL";
				Assembler.PopupException(msg + msig);
				return;
			}
			SymbolChannel<STREAMING_CONSUMER_CHILD> channel = this.ChannelsBySymbol[symbolLivesimming];
			if (channel.QuotePump_nullUnsafe == null) {
				msg += "PUMP_NULL_ONLY_WHEN_BACKTESTING";
				Assembler.PopupException(msg + msig);
				return;
			}
			if (channel.QuotePump_nullUnsafe.Paused == false) {
				msg += "PUSHING_THREAD_NOT_PAUSED";
				Assembler.PopupException(msg + msig);
				return;
			}
			channel.QuotePump_nullUnsafe.PusherUnpause_waitUntilUnpaused();
			//channel.QuotePump_nullUnsafe.WaitUntilUnpaused();
			msg = "PUMP_UNPAUSED_CONFIRMED";
			Assembler.PopupException(msg + msig, null, false);
		}
		internal void PumpStop_forSymbolLivesimming(string symbolLivesimming, string reasonForStoppingReplacedDistributor) {
			string msig = " //PumpStop_forSymbolLivesimming(" + symbolLivesimming + "," + reasonForStoppingReplacedDistributor + ") DISTRIBUTOR[" + this.ReasonIwasCreated + "]";
			string msg = "I_REFUSE_TO_STOP_PUMP ";
			if (this.ChannelsBySymbol.ContainsKey(symbolLivesimming) == false) {
				msg += "CHANNEL_DOESNT_EXIST_FOR_SYMBOL";
				Assembler.PopupException(msg + msig);
				return;
			}
			SymbolChannel<STREAMING_CONSUMER_CHILD> channel = this.ChannelsBySymbol[symbolLivesimming];
			if (channel.QuotePump_nullUnsafe == null) {
				msg += "PUMP_NULL_ONLY_WHEN_BACKTESTING";
				Assembler.PopupException(msg + msig);
				return;
			}
			if (channel.QuotePump_nullUnsafe.IsPushingThreadStarted == false) {
				msg += "PUSHING_THREAD_NOT_STARTED";
				Assembler.PopupException(msg + msig);
				return;
			}
			channel.QuotePump_nullUnsafe.PushingThreadStop_waitConfirmed();
			msg = "PUMP_STOPPED_CONFIRMED";
			Assembler.PopupException(msg + msig, null, false);
		}

		#region v1
		//internal void AllQuotePumps_Pause(string livesimCausingPauseName) {
		//	string msig = " //AllQuotePumps_Pause(" + livesimCausingPauseName + ") DISTRIBUTOR[" + this.ReasonIwasCreated + "]";
		//	string msg = "";
		//	foreach(SymbolChannel<STREAMING_CONSUMER_CHILD> eachChannel in this.ChannelsBySymbol.Values) {
		//		if (eachChannel.QuotePump_nullUnsafe == null) {
		//			string msg1 = "CHANNEL_IS_A_QUEUE_NOT_PUMP__CAN_NOT_PAUSE eachChannel=[" + eachChannel + "]";
		//			Assembler.PopupException(msg1 + msig);
		//		}
		//		if (eachChannel.QuotePump_nullUnsafe.Paused == true) {
		//			string msg1 = "PUMP_ALREADY_PAUSED livesimCausingPauseName=[" + livesimCausingPauseName + "]";
		//			Assembler.PopupException(msg1 + msig, null, false);
		//		} else {
		//			eachChannel.QuotePump_nullUnsafe.PusherPause();
		//		}
		//		if (msg != "") msg += ",";
		//		msg += "[" + eachChannel.ToString() + "]";
		//	}
		//	if (string.IsNullOrEmpty(msg)) {
		//		msg = "NO_QUOTE_PUMPS_PAUSED";
		//	} else {
		//		msg = "QUOTE_PUMPS_PAUSED_INSIDE_REPLACED_DISTRIBUTOR " + msg;
		//	}
		//	Assembler.PopupException(msg + msig, null, false);
		//}
		//internal void AllQuotePumps_Unpause(string livesimCausedPauseName) {
		//	string msig = " //AllQuotePumps_Unpause(" + livesimCausedPauseName + ") DISTRIBUTOR[" + this.ReasonIwasCreated + "]";
		//	string msg = "";
		//	foreach(SymbolChannel<STREAMING_CONSUMER_CHILD> eachChannel in this.ChannelsBySymbol.Values) {
		//		if (eachChannel.QuotePump_nullUnsafe == null) {
		//			string msg1 = "CHANNEL_IS_A_QUEUE_NOT_PUMP__CAN_NOT_PAUSE eachChannel=[" + eachChannel + "]";
		//			Assembler.PopupException(msg1 + msig);
		//		}
		//		if (eachChannel.QuotePump_nullUnsafe.Paused == false) {
		//			string msg1 = "PUMP_ALREADY_UNPAUSED livesimCausedPauseName=[" + livesimCausedPauseName + "]";
		//			Assembler.PopupException(msg1 + msig);
		//		} else {
		//			eachChannel.QuotePump_nullUnsafe.PusherUnpause();
		//		}
		//		if (msg != "") msg += ",";
		//		msg += "[" + eachChannel.ToString() + "]";
		//	}
		//	if (string.IsNullOrEmpty(msg)) {
		//		msg = "NO_QUOTE_PUMPS_UNPAUSED";
		//	} else {
		//		msg = "QUOTE_PUMPS_UNPAUSED_INSIDE_RESTORED_DISTRIBUTOR " + msg;
		//	}
		//	Assembler.PopupException(msg + msig, null, false);
		//}
		//internal void AllQuotePumps_Stop(string livesimCausedPauseName) {
		//	string msig = " //AllQuotePumps_Stop(" + livesimCausedPauseName + ") DISTRIBUTOR[" + this.ReasonIwasCreated + "]";
		//	string msg = "";
		//	foreach(SymbolChannel<STREAMING_CONSUMER_CHILD> eachChannel in this.ChannelsBySymbol.Values) {
		//		//v1 if (eachChannel.QuoteQueue.HasSeparatePushingThread == false) {
		//		if (eachChannel.QuotePump_nullUnsafe == null) {
		//			string msg1 = "QUOTE_PUMP_IS_A_QUEUE_MUST_BE_PUMP livesimCausedPauseName=[" + livesimCausedPauseName + "]";
		//			Assembler.PopupException(msg1 + msig);
		//			continue;
		//		}
		//		eachChannel.QuotePump_nullUnsafe.PushingThreadStop_waitConfirmed();
		//		if (msg != "") msg += ",";
		//		msg += "[" + eachChannel.ToString() + "]";
		//	}
		//	if (string.IsNullOrEmpty(msg)) {
		//		msg = "NO_QUOTE_PUMPS_STOPPED_THEIR_THREADS";
		//	} else {
		//		msg = "LIVESIM_RECEIVING_PUMPS_STOPPED_INSIDE_REPLACED " + msg;
		//	}
		//	Assembler.PopupException(msg + msig, null, false);
		//}
		#endregion

		//List<SymbolScaleStream> flattenDistributionChannels() { lock (this.lockConsumersBySymbol) {
		//	List<SymbolScaleStream> ret = new List<SymbolScaleStream>();
		//	foreach(Dictionary<BarScaleInterval, SymbolScaleStream> channelsForEachSymbol in this.ChannelsBySymbol.Values) {
		//		foreach(SymbolScaleStream eachChannel in channelsForEachSymbol.Values) {
		//			if (ret.Contains(eachChannel)) continue;
		//			ret.Add(eachChannel);
		//		}
		//	}
		//	return ret;
		//} }

	}
}