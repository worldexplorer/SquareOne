using System;
using Newtonsoft.Json;
using Sq1.Core.DataTypes;

namespace Sq1.Core.Execution {
	public partial class Position {
		public int SernoAbs;
		
		public bool IsLong { get { return this.PositionLongShort == PositionLongShort.Long; } }
		public bool IsShort { get { return this.PositionLongShort == PositionLongShort.Short; } }
		
		public PositionLongShort PositionLongShort { get; protected set; }
		public Bars Bars { get; protected set; }
		public string Symbol { get { return this.Bars.Symbol; } }
		public double Shares { get; protected set; }
		public double LastQuoteForMarketOrStopLimitImplicitPrice;// { get; protected set; }

		public PositionPrototype Prototype;

		public Alert EntryAlert;		// { get; protected set; }
		public MarketLimitStop EntryMarketLimitStop { get; protected set; }
		public int EntryFilledBarIndex { get; protected set; }
		public Bar EntryBar { get { return this.Bars[this.EntryFilledBarIndex]; } }
		public double EntryFilledPrice { get; protected set; }
		public double EntryPriceScript;	// { get; protected set; }
		private double EntryFilledQty;	// { get; protected set; }
		public double EntryFilledSlippage { get; protected set; }
		public string EntrySignal { get; protected set; }
		public double EntryFilledCommission { get; protected set; }

		public Alert ExitAlert;			// { get; protected set; }
		public MarketLimitStop ExitMarketLimitStop { get; protected set; }
		public int ExitFilledBarIndex { get; protected set; }
		public Bar ExitBar { get { return this.Bars[this.ExitFilledBarIndex]; } }
		public double ExitFilledPrice { get; protected set; }
		public double ExitPriceScript;	// { get; protected set; }
		private double ExitFilledQty;	// { get; protected set; }
		public double ExitFilledSlippage { get; protected set; }
		public string ExitSignal { get; protected set; }
		public double ExitFilledCommission { get; protected set; }

		public string StrategyID;
		//public bool NoExitBarOrStreaming { get { return (this.ExitBarIndex == -1 || this.ExitBarIndex == this.Bars.Count); } }
		public bool ExitNotFilledOrStreaming { get {
				if (this.ExitFilledBarIndex == -1) return true;
				if (this.ExitBar == null) return true;
				if (this.ExitBar.IsBarStreaming) return true;
				return false;
			} }
		//public bool EntrySafeToPaint { get { return (this.EntryBar > -1 && this.EntryBar < this.Bars.Count); } }
		//public bool ExitSafeToPaint { get { return (this.ExitBar > -1 && this.ExitBar < this.Bars.Count); } }
		public DateTime EntryDate { get {
				//if (this.EntryBarIndex < 0 || this.EntryBarIndex > this.Bars.Count) return DateTime.MinValue;
				Bar barEntry = this.EntryBar;		// don't take it from Alert! dateFilled depends on the market, not on your strategy
				if (barEntry == null) return DateTime.MinValue; 
				return this.EntryBar.DateTimeOpen;
			} }
		public DateTime ExitDate { get {
//				if (this.ExitBarIndex == -1 || this.ExitBarIndex > this.Bars.Count) {
//					if (this.ExitAlert != null) return this.ExitAlert.DateTime;
//					return DateTime.MinValue;
//				}
//				if (this.ExitBarIndex == this.Bars.Count) {
//					return this.Bars.StreamingBarCloneReadonly.DateTimeOpen;
//				}
//				Bar exitBar = this.Bars[this.ExitBarIndex];
//				if (exitBar == null) return DateTime.MinValue;
//				return exitBar.DateTimeOpen;
				Bar barExit = this.ExitBar;		// don't take it from this.ExitAlert! dateFilled depends on the market, not on your strategy
				if (barExit == null) return DateTime.MinValue; 
				return this.EntryBar.DateTimeOpen;
			} }
		public double Size { get {
				if (this.Bars.SymbolInfo.SecurityType == SecurityType.Future) {
					return this.Bars.SymbolInfo.LeverageForFutures * this.Shares;
				}
				return this.EntryFilledPrice * this.Shares;
			} }

		public double ExitOrStreamingPrice { get {
				double ret = -1;
//				if (this.ExitBarIndex == -1 || this.ExitBarIndex > this.Bars.Count) {
//					Bar partial = this.Bars.StreamingBarCloneReadonly;
//					ret = (double.IsNaN(partial.Close) == false) ? partial.Close : this.Bars.BarStaticLast.Close;
//				} else {
//					if (this.ExitPrice != 0 && this.ExitPrice != -1) {	//-1 is a standard for justInitialized nonFilled position's Entry/Exit Prices and Bars;
//						ret = this.ExitPrice;
//					} else {
//						//if (this.ExitBar == this.Bars.Count - 1) {
//						//	return this.Bars.LastBar.Close;
//						//}
//						if (this.ExitBarIndex < this.Bars.Count) {
//							ret = this.Bars[this.ExitBarIndex].Close;
//						}
//						if (this.ExitBarIndex == this.Bars.Count) {
//							ret = this.Bars.StreamingBarCloneReadonly.Close;
//						}
//						// here is the reason for an exception!! TODO: refactor in more concise words
//					}
//				}
				if (this.ExitFilledPrice != 0 && this.ExitFilledPrice != -1) {	//-1 is a standard for justInitialized nonFilled position's Entry/Exit Prices and Bars;
					ret = this.ExitFilledPrice;
				} else {
					if (this.Bars.BarStreaming == null) {
						throw new Exception("Position.ExitOrStreamingPrice: this.Bars.StreamingBar=null; @ExitBar[" + this.ExitFilledBarIndex + "] position=[" + this + "]; ");
					}
					ret = this.Bars.BarStreaming.Close;
				}
				if (this.ExitFilledSlippage != -1) ret += this.ExitFilledSlippage;
				return ret;
			} }
		public double EntryPriceNoSlippage { get {
				double ret = 0;
				if (this.EntryFilledPrice == -1) return ret;
				ret = this.EntryFilledPrice - this.EntryFilledSlippage;
				return ret;
			} }
		public double ExitOrCurrentPriceNoSlippage { get {
				double ret = this.ExitOrStreamingPrice;
				if (this.ExitFilledSlippage != -1) ret -= this.ExitFilledSlippage;
				return ret;
			} }
		public bool IsEntryFilled { get {
				if (this.EntryFilledPrice == -1) return false;
				if (this.EntryFilledQty == -1) return false;
				if (this.EntryFilledCommission == -1) return false;
				if (this.EntryFilledSlippage == -1) return false;
				return true;
			} }
		public bool IsExitFilled { get {
				if (this.ExitFilledPrice == -1) return false;
				if (this.ExitFilledQty == -1) return false;
				if (this.ExitFilledCommission == -1) return false;
				if (this.ExitFilledSlippage == -1) return false;
				return true;
			} }
		public bool ClosedByTakeProfitLogically { get {
				if (this.EntryFilledPrice == -1) {
					throw new Exception("position.EntryPrice=-1, make sure you called EntryFilledWith()");
				}
				if (this.ExitFilledPrice == -1) {
					throw new Exception("position.ExitPrice=-1, make sure you called ExitFilledWith()");
				}
				if (this.IsExitFilled == false) {
					throw new Exception("position isn't closed yet, ExitFilled=false");
				}
				bool exitAboveEntry = this.ExitFilledPrice > this.EntryFilledPrice;

				if (this.PositionLongShort == PositionLongShort.Long) return exitAboveEntry;
				else return !exitAboveEntry;
			} }
		// prototype-related methods
		public bool IsExitFilledWithPrototypedAlert { get {
				this.checkThrowPrototypeNotNullAndIsExitFilled();
				bool oneSideFilled =
					this.ExitAlert == this.Prototype.StopLossAlertForAnnihilation ||
					this.ExitAlert == this.Prototype.TakeProfitAlertForAnnihilation;
				return oneSideFilled;
			} }
		protected void checkThrowPrototypeNotNullAndIsExitFilled() {
			if (this.Prototype == null) {
				throw new Exception("this.Prototype=null, check IsPrototypeNull first");
			}
			if (this.IsExitFilled == false) {
				throw new Exception("position isn't closed yet, ExitFilled=false");
			}
		}
		public bool IsExitFilledByPrototypedStopLoss { get {
				this.checkThrowPrototypeNotNullAndIsExitFilled();
				if (this.ExitAlert == this.Prototype.StopLossAlertForAnnihilation) return true;
				return false;
			} }
		public bool IsExitFilledByPrototypedTakeProfit { get {
				this.checkThrowPrototypeNotNullAndIsExitFilled();
				if (this.ExitAlert == this.Prototype.TakeProfitAlertForAnnihilation) return true;
				return false;
			} }
		public Alert PrototypedExitCounterpartyAlert { get {
				if (this.IsExitFilledByPrototypedTakeProfit) return this.Prototype.StopLossAlertForAnnihilation;
				if (this.IsExitFilledByPrototypedStopLoss) return this.Prototype.TakeProfitAlertForAnnihilation;
				string msg = "Prototyped position closed by some prototype-unrelated alert[" + this.ExitAlert + "]";
				throw new Exception(msg);
			} }
		public Position() {
			PositionLongShort = PositionLongShort.Unknown;
			StrategyID = "STRATEGY_ID_NOT_INITIALIZED";

			EntryMarketLimitStop = MarketLimitStop.Unknown;
			EntryFilledBarIndex = -1;
			EntryFilledPrice = -1;
			EntryFilledQty = -1;
			EntryFilledSlippage = -1;
			EntryFilledCommission = -1;

			ExitMarketLimitStop = MarketLimitStop.Unknown;
			ExitFilledBarIndex = -1;
			ExitFilledPrice = -1;
			ExitFilledQty = -1;
			ExitFilledSlippage = -1;
			ExitFilledCommission = -1;
		}
		protected Position(Bars bars, PositionLongShort positionLongShort, string strategyID, double basisPrice,
				double shares) : this() {
			this.Bars = bars;
			this.PositionLongShort = positionLongShort;
			this.StrategyID = strategyID;
			this.LastQuoteForMarketOrStopLimitImplicitPrice = basisPrice;
			this.Shares = shares;
		}
		public Position(Alert alertEntry, double basisPrice) : this(alertEntry.Bars
				, alertEntry.PositionLongShortFromDirection, alertEntry.StrategyID.ToString()
				, basisPrice, alertEntry.Qty) {
			this.EntryAlert = alertEntry;
			this.EntryMarketLimitStop = alertEntry.MarketLimitStop;
			this.EntryPriceScript = alertEntry.PriceScript;
			this.EntrySignal = alertEntry.SignalName;
		}
		public void ExitAlertAttach(Alert alertExit) {
			if (this.Prototype == null) {
				if (this.ExitAlert != null) {
					string msg = "POSITION_WAS_ALREADY_ATTCHED_TO_EXIT_ALERT ExitAlert[" + this.ExitAlert + "] ";
					throw new Exception(msg);
				}
				if (this.ExitMarketLimitStop != MarketLimitStop.Unknown) {
					string msg = "POSITION_WAS_ALREADY_SYNCHED_WITH_FILLED_ALERT_ON_ALERT_FILLED_CALLBACK: ExitPriceScript[" + this.ExitPriceScript + "]";
					throw new Exception(msg);
				}
			} else {
				if (this.ExitAlert != null) {
					if (alertExit.IsFilled == false) {
						string msg = "REPLACING_FIRST_CREATED_STOPLOSS_WITH_SECOND_CREATED_TAKEPROFIT ExitAlert[" + this.ExitAlert.SignalName + "] shouldBeTakeprofit[" + alertExit.SignalName + "]";
					} else {
						string msg = "REPLACING_EXIT_ALERT_WITH_PROTOTYPED_TP_OR_SL_WHICHEVER_FILLED_FIRST ExitAlert[" + this.ExitAlert + "] filledFirst[" + alertExit + "]";
					}
				}
			}
			this.ExitAlert = alertExit;
			this.ExitMarketLimitStop = alertExit.MarketLimitStop;
			this.ExitPriceScript = alertExit.PriceScript;
			this.ExitSignal = alertExit.SignalName;
		}
		public void FillEntryWith(Bar entryBar, double entryFillPrice, double entryFillQty, double entrySlippage, double entryCommission) {
			string msig = " FillEntryWith(" + entryBar + ", " + entryFillPrice + ", " + entryFillQty + ", " + entrySlippage + ", " + entryCommission + ")";
			string alertOpenedThisPosition = (this.EntryAlert == null) ? "NO_ENTRY_ALERT" : this.EntryAlert.ToString();
			// 1) absolutely acceptable to have a limit order beoynd the bar;
			// 2) Market order must be filled now at SpreadGenerator-generated ANY price while StreamingBar may contain only 1 quote (height=0)
			//if (entryBar.ContainsPrice(entryFillPrice) == false) {
			//	string msg = "PRICE_FILLED_POSITION_ENTRY_DOESNT_EXIST_IN_ENTRYBAR entryFilledPrice[" + entryFillPrice + "] entryBar[" + entryBar + "]";
			//	throw new Exception(msg + msig);
			//}
			if (entryBar.Volume < entryFillQty) {
				string msg = "VOLUME_FILLED_POSITION_ENTRY_NEVER_TRADED_DURING_THE_ENTRYBAR entryFilledQty[" + entryFillQty + "] entryBar.Volume[" + entryBar.Volume + "]";
				throw new Exception(msg + msig);
			}
			if (this.EntryFilledBarIndex != -1) {
				string msg = "PositionEntry was already filled earlier @EntryBar[" + this.EntryFilledBarIndex + "]"
						+ ", you can't override it with [" + entryBar + "]; alertOpenedThisPosition[" + alertOpenedThisPosition + "]";
				throw new Exception(msg + msig);
			}
			this.EntryFilledBarIndex = entryBar.ParentBarsIndex;
			if (this.EntryBar == null) {
				string msg = "BARINDEX_FILLED_POSITION_ENTRY_DOESNT_BELONG_TO_ITS_OWN_PARENT_CHECK_Position.EntryBar_PROPERTY entryBar[" + entryBar + "] this.Bars[" + this.Bars + "]";
				throw new Exception(msg + msig);
			}
			if (entryBar.Open != this.EntryBar.Open) {
				string msg = "BAR_FILLED_POSITION_ENTRY_DOESNT_HAVE_SAME_OPEN_CHECK_Position.EntryBar_PROPERTY entryBar.Open[" + entryBar.Open + "] this.Bars[" + this.EntryFilledBarIndex + "].Open[" + entryBar.Open + "]";
				throw new Exception(msg + msig);
			}
			this.EntryFilledPrice = entryFillPrice;
			this.EntryFilledQty = entryFillQty;
			this.EntryFilledSlippage = entrySlippage;
			this.EntryFilledCommission = entryCommission;
		}
		public void FillExitWith(Bar exitBar, double exitFillPrice, double exitFillQty, double exitSlippage, double exitCommission) {
			string msig = " FillExitWith(" + exitBar + ", " + exitFillPrice + ", " + exitFillQty + ", " + exitSlippage + ", " + exitCommission + ")";
			// 1) absolutely acceptable to have a limit order beoynd the bar;
			// 2) Market order must be filled now at SpreadGenerator-generated ANY price while StreamingBar may contain only 1 quote (height=0)
			//if (exitBar.ContainsPrice(exitFillPrice) == false) {
			//	string msg = "PRICE_FILLED_POSITION_EXIT_DOESNT_EXIST_IN_EXITBAR exitFilledPrice[" + exitFillPrice + "] exitBar[" + exitBar + "]";
			//	throw new Exception(msg + msig);
			//}
			if (exitBar.Volume < exitFillQty) {
				string msg = "VOLUME_FILLED_POSITION_EXIT_NEVER_TRADED_DURING_THE_EXITBAR exitFilledQty[" + exitFillQty + "] exitBar.Volume[" + exitBar.Volume + "]";
				throw new Exception(msg + msig);
			}
			if (this.EntryFilledBarIndex == -1) {
				string msg = "ATTEMPT_TO_CLOSE_NON_OPENED_POSITION this.EntryBarIndex=-1 " + this;
				throw new Exception(msg + msig);
			}
			if (this.EntryFilledBarIndex > exitBar.ParentBarsIndex) {
				string msg = "ATTEMPT_TO_CLOSE_POSITION_AT_BAR_EARLIER_THAN_POSITION_WAS_OPENED this.EntryBarIndex[" + this.EntryFilledBarIndex + "] > exitBar.ParentBarsIndex[" + exitBar.ParentBarsIndex + "]";
				throw new Exception(msg + msig);
			}
			if (this.ExitFilledBarIndex != -1) {
				string alertClosedThisPosition = (this.ExitAlert == null) ? "NO_EXIT_ALERT" : this.ExitAlert.ToString();
				string msg = "PositionExit was already filled earlier @ExitBar[" + this.ExitFilledBarIndex + "]"
						+ ", you can't override it with [" + exitBar + "]; alertClosedThisPosition[" + alertClosedThisPosition + "]";
				throw new Exception(msg);
			}
			this.ExitFilledBarIndex = exitBar.ParentBarsIndex;
			if (this.ExitBar == null) {
				string msg = "BARINDEX_FILLED_POSITION_EXIT_DOESNT_BELONG_TO_ITS_OWN_PARENT_BARS_CHECK_Position.EntryBar_PROPERTY exitBar[" + exitBar + "] this.Bars[" + this.Bars + "]";
				throw new Exception(msg + msig);
			}
			if (exitBar.Open != this.ExitBar.Open) {
				string msg = "BAR_FILLED_POSITION_EXIT_DOESNT_HAVE_SAME_OPEN_CHECK_Position.EntryBar_PROPERTY exitBar.Open[" + exitBar.Open + "] this.Bars[" + this.ExitFilledBarIndex + "].Open[" + exitBar.Open + "]";
				throw new Exception(msg + msig);
			}
			if (exitBar.ParentBars != this.EntryBar.ParentBars) {
				string msg = "PARENTS_OF_BAR_FILLED_POSITION_EXIT_MUST_BE_SAME_AS_ENTRY_BAR_PARENTS exitBar.ParentBars[" + exitBar.ParentBars + "] != this.EntryBar.ParentBars[" + this.EntryBar.ParentBars + "]";
				throw new Exception(msg + msig);
			}
			this.ExitFilledPrice = exitFillPrice;
			this.ExitFilledQty = exitFillQty;
			this.ExitFilledSlippage = exitSlippage;
			this.ExitFilledCommission = exitCommission;
		}
		public override int GetHashCode() {
			unchecked {
				int hash = 17;
				hash = hash * 23 + PositionLongShort.GetHashCode();
				hash = hash * 23 + Symbol.GetHashCode();
				hash = hash * 23 + Shares.GetHashCode();
				if (this.EntryFilledBarIndex == -1) return hash;

				hash = hash * 23 + EntryMarketLimitStop.GetHashCode();
				hash = hash * 23 + EntryFilledBarIndex.GetHashCode();
				hash = hash * 23 + EntryFilledPrice.GetHashCode();
				hash = hash * 23 + EntrySignal.GetHashCode();
				if (this.ExitFilledBarIndex == -1) return hash;

				hash = hash * 23 + ExitMarketLimitStop.GetHashCode();
				hash = hash * 23 + ExitFilledBarIndex.GetHashCode();
				hash = hash * 23 + ExitFilledPrice.GetHashCode();
				hash = hash * 23 + ExitSignal.GetHashCode();
				return hash;
			}
		}
		public override bool Equals(object obj) {
			if ((obj is Position) == false) return false;
			return Equals((Position)obj);
		}
		public bool Equals(Position otherPosition) {
			bool headerIsTheSame = this.PositionLongShort == otherPosition.PositionLongShort
				&& this.Symbol == otherPosition.Symbol
				&& this.Shares == otherPosition.Shares;
			if (headerIsTheSame == false) return false;

			bool entryIsTheSame = this.EntryMarketLimitStop == otherPosition.EntryMarketLimitStop
				&& this.EntryFilledBarIndex == otherPosition.EntryFilledBarIndex
				&& this.EntryFilledPrice == otherPosition.EntryFilledPrice
				&& this.EntrySignal == otherPosition.EntrySignal;
			if (entryIsTheSame == false) return false;

			// what did I want to say by this??
			//if (this.ExitBar == -1) return true;
			//if (otherPosition.ExitBar == -1) return true;

			bool bothNaNs = double.IsNaN(this.ExitFilledPrice) && double.IsNaN(otherPosition.ExitFilledPrice);
			if (bothNaNs) {
				bool exitIsTheSameNaNs = this.EntryMarketLimitStop == otherPosition.EntryMarketLimitStop
					&& this.ExitFilledBarIndex == otherPosition.ExitFilledBarIndex
					&& this.ExitSignal == otherPosition.ExitSignal;
				return exitIsTheSameNaNs;
			}
			bool exitIsTheSame = this.EntryMarketLimitStop == otherPosition.EntryMarketLimitStop
				&& this.ExitFilledBarIndex == otherPosition.ExitFilledBarIndex
				&& this.ExitFilledPrice == otherPosition.ExitFilledPrice
				&& this.ExitSignal == otherPosition.ExitSignal;
			//bool alwaysDifferent = this.SernoInMasterList == position.SernoInMasterList;
			return exitIsTheSame;
		}
		public override string ToString() {
			string ret = "";
			if (this.SernoAbs > 0) ret += "#" + this.SernoAbs;
			ret += this.PositionLongShort + " " + Shares + "*" + this.Symbol + " Entry=[";
			if (this.EntryFilledBarIndex != -1) {
				ret += EntryMarketLimitStop + "@" + EntryFilledPrice + "/bar" + EntryFilledBarIndex + ":";
				if (this.EntryAlert != null) {
					if (this.EntryAlert.OrderFollowed != null) {
						ret += this.EntryAlert.OrderFollowed.State;
					} else {
						ret += "NO_ENTRY_ORDER";
					}
				} else {
					ret += "NO_ENTRY_ALERT";
				}
			} else {
				ret += "ENTRY_BAR-1";
			}
			ret += "] Exit=[";
			if (this.ExitFilledBarIndex != -1) {
				ret += ExitMarketLimitStop + "@" + ExitFilledPrice + "/bar" + ExitFilledBarIndex + ":";
				if (this.ExitAlert != null) {
					if (this.ExitAlert.OrderFollowed != null) {
						ret += this.ExitAlert.OrderFollowed.State;
					} else {
						ret += "NO_EXIT_ORDER";
					}
				} else {
					ret += "NO_EXIT_ALERT";
				}
			} else {
				ret += "EXIT_BAR-1";
			}
			ret += "]";
			if (this.LastQuoteForMarketOrStopLimitImplicitPrice != 0) ret += " BasisPrice[" + this.LastQuoteForMarketOrStopLimitImplicitPrice + "]";
			if (this.Prototype != null) ret += " Proto" + this.Prototype;
			return ret;
		}
	}
}