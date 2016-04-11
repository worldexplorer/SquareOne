using System;

namespace Sq1.Core.Streaming {
	public partial class Distributor<STREAMING_CONSUMER_CHILD> {

		internal int PumpPause_forSymbolLivesimming(string symbolLivesimming, string reasonForNewDistributor) {
			string msig = " //PumpPause_forSymbolLivesimming(" + symbolLivesimming + "," + reasonForNewDistributor + ") DISTRIBUTOR[" + this.ReasonIwasCreated + "]";
			string msg = "I_REFUSE_TO_PAUSE_PUMP ";
			int paused = 0;
			if (this.ChannelsBySymbol.ContainsKey(symbolLivesimming) == false) {
				msg += "CHANNEL_DOESNT_EXIST_FOR_SYMBOL";
				Assembler.PopupException(msg + msig);
				return paused;
			}
			SymbolChannel<STREAMING_CONSUMER_CHILD> channel = this.ChannelsBySymbol[symbolLivesimming];
			if (channel.QuotePump_nullUnsafe == null) {
				msg += "PUMP_NULL_ONLY_WHEN_BACKTESTING";
				Assembler.PopupException(msg + msig);
				return paused;
			}
			if (channel.QuotePump_nullUnsafe.Paused) {
				msg += "PUSHING_THREAD_ALREADY_PAUSED";
				Assembler.PopupException(msg + msig);
				return paused;
			}
			channel.QuotePump_nullUnsafe.PusherPause_waitUntilPaused();
			//channel.QuotePump_nullUnsafe.WaitUntilPaused();
			msg = "PUMP_UNPAUSED_CONFIRMED";
			Assembler.PopupException(msg + msig, null, false);
			paused++;
			return paused;
		}
		internal int PumpUnpause_forSymbolLivesimming(string symbolLivesimming, string reasonForStoppingReplacedDistributor) {
			string msig = " //PumpUnpause_forSymbolLivesimming(" + symbolLivesimming + "," + reasonForStoppingReplacedDistributor + ") DISTRIBUTOR[" + this.ReasonIwasCreated + "]";
			string msg = "I_REFUSE_TO_UNPAUSE_PUMP ";
			int unpaused = 0;
			if (this.ChannelsBySymbol.ContainsKey(symbolLivesimming) == false) {
				msg += "CHANNEL_DOESNT_EXIST_FOR_SYMBOL";
				Assembler.PopupException(msg + msig);
				return unpaused;
			}
			SymbolChannel<STREAMING_CONSUMER_CHILD> channel = this.ChannelsBySymbol[symbolLivesimming];
			if (channel.QuotePump_nullUnsafe == null) {
				msg += "PUMP_NULL_ONLY_WHEN_BACKTESTING";
				Assembler.PopupException(msg + msig);
				return unpaused;
			}
			if (channel.QuotePump_nullUnsafe.Paused == false) {
				msg += "PUSHING_THREAD_NOT_PAUSED";
				Assembler.PopupException(msg + msig);
				return unpaused;
			}
			channel.QuotePump_nullUnsafe.PusherUnpause_waitUntilUnpaused();
			//channel.QuotePump_nullUnsafe.WaitUntilUnpaused();
			msg = "PUMP_UNPAUSED_CONFIRMED";
			Assembler.PopupException(msg + msig, null, false);
			unpaused++;
			return unpaused;
		}
		internal int PumpStop_forSymbolLivesimming(string symbolLivesimming, string reasonForStoppingReplacedDistributor) {
			string msig = " //PumpStop_forSymbolLivesimming(" + symbolLivesimming + "," + reasonForStoppingReplacedDistributor + ") DISTRIBUTOR[" + this.ReasonIwasCreated + "]";
			string msg = "I_REFUSE_TO_STOP_PUMP ";
			int stopped = 0;
			if (this.ChannelsBySymbol.ContainsKey(symbolLivesimming) == false) {
				msg += "CHANNEL_DOESNT_EXIST_FOR_SYMBOL";
				Assembler.PopupException(msg + msig);
				return stopped;
			}
			SymbolChannel<STREAMING_CONSUMER_CHILD> channel = this.ChannelsBySymbol[symbolLivesimming];
			if (channel.QuotePump_nullUnsafe == null) {
				msg += "PUMP_NULL_ONLY_WHEN_BACKTESTING";
				Assembler.PopupException(msg + msig);
				return stopped;
			}
			if (channel.QuotePump_nullUnsafe.IsPushingThreadStarted == false) {
				msg += "PUSHING_THREAD_NOT_STARTED";
				Assembler.PopupException(msg + msig);
				return stopped;
			}
			channel.QuotePump_nullUnsafe.PushingThreadStop_waitConfirmed();
			msg = "PUMP_STOPPED_CONFIRMED";
			Assembler.PopupException(msg + msig, null, false);
			stopped++;
			return stopped;
		}
		
		internal int AllPumpsPause_forAllSymbol_duringLivesimmingOne(string reasonForNewDistributor) {
			int paused = 0;
			foreach (string symbol in this.ChannelsBySymbol.Keys) {
				paused += this.PumpPause_forSymbolLivesimming(symbol, reasonForNewDistributor);
			}
			return paused;
		}

		internal int AllPumpsUnpause_forAllSymbol_afterLivesimmingOne(string reasonForNewDistributor) {
			int unpaused = 0;
			foreach (string symbol in this.ChannelsBySymbol.Keys) {
				unpaused += this.PumpUnpause_forSymbolLivesimming(symbol, reasonForNewDistributor);
			}
			return unpaused;
		}

	}
}