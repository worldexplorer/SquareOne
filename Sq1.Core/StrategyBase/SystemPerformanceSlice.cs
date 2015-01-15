using System;
using System.Collections.Generic;

using Sq1.Core.DataTypes;
using Sq1.Core.Execution;

namespace Sq1.Core.StrategyBase {
	public class SystemPerformanceSlice {
		public	string				ReasonToExist;
		public	SystemPerformancePositionsTracking	PositionLongShortImTracking		{ get; private set; }
		
		public	DataSeriesTimeBased	EquityCurve						{ get; private set; }
		public	DataSeriesTimeBased	CashCurve						{ get; private set; }
		//cumulativeProfitDollar,Percent+CalcEachStreamingUpdate should be moved into SystemPerformanceSlice for other Reporters to be used in their drawings
		public	Dictionary<Position, double> CumulativeProfitDollar;
		public	Dictionary<Position, double> CumulativeProfitPercent;
		
		public	PositionList		PositionsImTracking				{ get; private set; }
		public	IList<Position>		PositionsImTrackingReadonly		{ get { return this.PositionsImTracking.InnerList.AsReadOnly(); } }

		public	double				CashAvailable;

		public	DateTime			NetProfitPeakDate;
		public	double				NetProfitPeak;
		public	DateTime			MaxDrawDownLastLossDate;
		public	double				MaxDrawDown;

				double				curConsecWinners;
				double				curConsecLosers;
		public	double				MaxConsecWinners;
		public	double				MaxConsecLosers;

		public double ProfitFactor	{ get {
				if (this.NetLossLosers == 0) return double.PositiveInfinity;
				return Math.Round(this.NetProfitWinners / Math.Abs(this.NetLossLosers), 2);
			} }
		public double RecoveryFactor	{ get {
				if (this.MaxDrawDown == 0) return double.PositiveInfinity;
				return Math.Round(Math.Abs(this.NetProfitForClosedPositionsBoth / this.MaxDrawDown), 2);
			} }
		public double WinRatePct	{ get {
				if (this.PositionsCountBoth == 0) return double.PositiveInfinity;
				return Math.Round(100 * this.PositionsCountWinners / (double)this.PositionsCountBoth, 1);
			} }
		public double LossRatePct	{ get {
				if (this.PositionsCountBoth == 0) return double.PositiveInfinity;
				return Math.Round(100 * this.PositionsCountLosers / (double)this.PositionsCountBoth, 2);
			} }
		public double WinLossRatio  { get {
				double ret = double.NaN;
				if (this.PositionsCountLosers == 0) return ret;
				// I_HATE_IT 296/452 = 0 !!!! if you don't convert divider to (double)
				return Math.Round(this.PositionsCountWinners / (double)this.PositionsCountLosers, 2);
			} }
		public double PayoffRatio { get {
				if (this.AvgLossPctLosers == 0) return 0;
				return Math.Round(Math.Abs(this.AvgProfitPctBoth / (double)this.AvgLossPctLosers), 2);
			} }

		#region AllTrades
		public double CommissionBoth;
		public double NetProfitPctForClosedPositionsBoth;
		public double NetProfitForClosedPositionsBoth;
		public int BarsHeldTotalForClosedPositionsBoth;
		public double ProfitPerBarBoth { get {
				if (this.BarsHeldTotalForClosedPositionsBoth == 0) return 0;
				return Math.Round(this.NetProfitForClosedPositionsBoth / (double)this.BarsHeldTotalForClosedPositionsBoth, 2);
			} }
		public int PositionsCountBoth;
		public double AvgProfitBoth { get {
				if (this.PositionsCountBoth == 0) return 0;
				return Math.Round(this.NetProfitForClosedPositionsBoth / (double)this.PositionsCountBoth, 2);
			} }
		public double AvgProfitPctBoth { get {
				if (this.PositionsCountBoth == 0) return 0;
				return Math.Round(this.NetProfitPctForClosedPositionsBoth / (double)this.PositionsCountBoth, 2);
			} }
		public double AvgBarsHeldBoth { get {
				if (this.PositionsCountBoth == 0) return 0;
				return Math.Round(this.BarsHeldTotalForClosedPositionsBoth / (double)this.PositionsCountBoth, 1);
			} }
		#endregion

		#region Winners
		public double CommissionWinners;
		public double NetProfitPctForClosedPositionsLong;
		public double NetProfitWinners;
		public int BarsHeldTotalForClosedPositionsWinners;
		public double ProfitPerBarLong { get {
				if (this.BarsHeldTotalForClosedPositionsWinners == 0) return 0;
				return Math.Round(this.NetProfitWinners / (double)this.BarsHeldTotalForClosedPositionsWinners, 2);
			} }
		public int PositionsCountWinners;
		public double AvgProfitWinners { get {
				if (this.PositionsCountWinners == 0) return 0;
				return Math.Round(this.NetProfitWinners / (double)this.PositionsCountWinners, 2);
			} }
		public double AvgProfitPctWinners { get {
				if (this.PositionsCountWinners == 0) return 0;
				return Math.Round(this.NetProfitPctForClosedPositionsLong / (double)this.PositionsCountWinners, 2);
			} }
		public double AvgBarsHeldWinners { get {
				if (this.PositionsCountWinners == 0) return 0;
				return Math.Round(this.BarsHeldTotalForClosedPositionsWinners / (double)this.PositionsCountWinners, 1);
			} }
		#endregion

		#region Losers
		public double CommissionLosers;
		public double NetProfitPctForClosedPositionsLosers;
		public double NetLossLosers;
		public int BarsHeldTotalForClosedPositionsLosers;
		public double ProfitPerBarLosers { get {
				if (this.BarsHeldTotalForClosedPositionsLosers == 0) return 0;
				return Math.Round(this.NetLossLosers / (double)this.BarsHeldTotalForClosedPositionsLosers, 2);
			} }
		public int PositionsCountLosers;
		public double AvgLossLosers { get {
				if (this.PositionsCountLosers == 0) return 0;
				return Math.Round(this.NetLossLosers / (double)this.PositionsCountLosers, 2);
			} }
		public double AvgLossPctLosers { get {
				if (this.PositionsCountLosers == 0) return 0;
				return Math.Round(this.NetProfitPctForClosedPositionsLosers / (double)this.PositionsCountLosers, 2);
			} }
		public double AvgBarsHeldLosers { get {
				if (this.PositionsCountLosers == 0) return 0;
				return Math.Round(this.BarsHeldTotalForClosedPositionsLosers / (double)this.PositionsCountLosers, 1);
			} }
		#endregion

		protected SystemPerformanceSlice() {
			ReasonToExist = "NO_REASON_TO_EXIST";
			// LOOKS_STUPID_BUT_DONT_SUGGEST_BRINGING_EXECUTOR_HERE: new BarScaleInterval(BarScale.Unknown, 0)
			EquityCurve = new DataSeriesTimeBased(new BarScaleInterval(BarScale.Unknown, 0), "Equity");
			CashCurve = new DataSeriesTimeBased(new BarScaleInterval(BarScale.Unknown, 0), "Cash");
			CumulativeProfitDollar = new Dictionary<Position, double>();
			CumulativeProfitPercent = new Dictionary<Position, double>();
			
			PositionsImTracking = new PositionList("PositionsImTracking");
			//v1 PositionLongShortImTracking = PositionLongShort.Unknown;	// direction not specified => it means "both short and long" here
			//v2
			PositionLongShortImTracking = SystemPerformancePositionsTracking.LongAndShort;
		}
		public SystemPerformanceSlice(SystemPerformancePositionsTracking positionLongShortImTracking, string reasonToExist) : this() {
			PositionLongShortImTracking = positionLongShortImTracking;
			ReasonToExist = reasonToExist;
		}
		internal void Initialize(double startingCapital = -1) {
			this.PositionsImTracking.Clear();
			this.EquityCurve.Clear();
			this.CashCurve.Clear();
			this.CashAvailable = 0;

			this.CumulativeProfitDollar.Clear();
			this.CumulativeProfitPercent.Clear();

			this.MaxDrawDownLastLossDate = DateTime.MinValue;
			this.MaxDrawDown = 0;
			this.NetProfitPeakDate = DateTime.MinValue;
			this.NetProfitPeak = 0;

			this.curConsecWinners = 0;
			this.curConsecLosers = 0;
			this.MaxConsecWinners = 0;
			this.MaxConsecLosers = 0;

			this.NetProfitForClosedPositionsBoth = 0;
			this.BarsHeldTotalForClosedPositionsBoth = 0;
			this.PositionsCountBoth = 0;

			this.NetProfitWinners = 0;
			this.BarsHeldTotalForClosedPositionsWinners = 0;
			this.PositionsCountWinners = 0;

			this.NetLossLosers = 0;
			this.BarsHeldTotalForClosedPositionsLosers = 0;
			this.PositionsCountLosers = 0;
		}
		[Obsolete("GET_RID_OF_ME__IM_TOO_EXPENSIVE__REPLACE_WITH_RANGED_updateStatsForBar+CumulativeAppendForPositionClosed")]
		public void BuildStatsCumulative() {
			double cumProfitDollar = 0;
			double cumProfitPercent = 0;
			this.CumulativeProfitDollar.Clear();
			this.CumulativeProfitPercent.Clear();
			foreach (Position pos in this.PositionsImTrackingReadonly) {
				if (this.CumulativeProfitDollar.ContainsKey(pos)) {
					string msg = "CUMULATIVES_ALREADY_CALCULATED_FOR_THIS_POSITION SystemPerformanceSlice["
						+ this.ToString() + "].PositionsImTracking contains duplicate " + pos;
					Assembler.PopupException(msg);
					continue;
				}
				cumProfitDollar += pos.NetProfit;
				cumProfitPercent += pos.NetProfitPercent;
				this.CumulativeProfitDollar.Add(pos, cumProfitDollar);
				this.CumulativeProfitPercent.Add(pos, cumProfitPercent);
			}
		}
		public void CumulativeAppendForPositionClosed(Position positionClosed) {
			if (this.CumulativeProfitDollar.ContainsKey(positionClosed)) {
				string msg = "CUMULATIVES_ALREADY_CALCULATED_FOR_THIS_POSITION SystemPerformanceSlice["
					+ this.ToString() + "].PositionsImTracking[" + this.PositionsImTracking.ToString() + "] already contains " + positionClosed;
				Assembler.PopupException(msg);
				return;
			}

			double prefCumProfitDollar = 0;
			double prevCumProfitPercent = 0;

			Position preLastPositionTracked = this.PositionsImTracking.PreLastNullUnsafe;
			if (preLastPositionTracked != null) {
				if (this.CumulativeProfitDollar.ContainsKey(preLastPositionTracked) == false) {
					if (this.CumulativeProfitDollar.Count > this.PositionsImTracking.Count - 2) {
						string msg2 = "CumulativeProfitDollar_SHOULD_ALREADY_CONTAIN_preLastPositionTracked " + preLastPositionTracked;
						Assembler.PopupException(msg2);
					}
				} else {
					prefCumProfitDollar = this.CumulativeProfitDollar[preLastPositionTracked];
				}
				if (this.CumulativeProfitPercent.ContainsKey(preLastPositionTracked) == false) {
					if (this.CumulativeProfitPercent.Count > this.PositionsImTracking.Count - 2) {
						string msg2 = "CumulativeProfitPercent_SHOULD_ALREADY_CONTAIN_preLastPositionTracked " + preLastPositionTracked;
						Assembler.PopupException(msg2);
					}
				} else {
					prevCumProfitPercent = this.CumulativeProfitPercent[preLastPositionTracked];
				}
			}
			this.CumulativeProfitDollar.Add(positionClosed, prefCumProfitDollar + positionClosed.NetProfit);
			this.CumulativeProfitPercent.Add(positionClosed, prevCumProfitPercent + positionClosed.NetProfitPercent);
		}
		public double CumulativeNetProfitForPosition(Position position) {
			double ret = this.CumulativeProfitDollar.ContainsKey(position) ? this.CumulativeProfitDollar[position] : -1;
			return ret;
		}
		public double CumulativeNetProfitPercentForPosition(Position position) {
			double ret = this.CumulativeProfitPercent.ContainsKey(position) ? this.CumulativeProfitPercent[position] : -1;
			return ret;
		}
		internal int BuildIncrementalBrokerFilledAlertsOpeningForPositions_step1of3(Position positionOpened) {
			int positionsOpenAbsorbedBoth = 0;
			if (this.shouldAddupPosition(positionOpened) == false) return positionsOpenAbsorbedBoth;
			this.checkThrowEntry(positionOpened, positionOpened.EntryBar.DateTimeOpen);

			bool added = this.PositionsImTracking.AddOpening_step1of2(positionOpened, true);
			if (added == false) {
				return positionsOpenAbsorbedBoth;
			}

			positionsOpenAbsorbedBoth += this.updateStatsForBar(positionOpened.EntryFilledBarIndex, new List<Position>() { positionOpened }, new List<Position>());
			if (positionOpened.EntryFilledBarIndex < this.PositionsImTracking.LastEntryFilledBarIndex) {
				string msg = "recalculate CashCurve from now (positionOpened.EntryFilledBarIndex) till the end (this.PositionsImTracking.LastEntryFilledBarIndex)";
				Assembler.PopupException(msg);
			}
			return positionsOpenAbsorbedBoth;
		}
		internal int BuildReportIncrementalBrokerFilledAlertsClosingForPositions_step3of3(Position positionClosed) {
			int positionsClosedAbsorbedBoth = 0;
			if (this.shouldAddupPosition(positionClosed) == false) return positionsClosedAbsorbedBoth; 
			this.checkThrowExit(positionClosed, positionClosed.ExitBar.DateTimeOpen);

			bool added = this.PositionsImTracking.AddToClosedDictionary_step2of2(positionClosed, true);
			if (added == false) {
				return positionsClosedAbsorbedBoth;
			}

			positionsClosedAbsorbedBoth += this.updateStatsForBar(positionClosed.ExitFilledBarIndex, new List<Position>(), new List<Position>() { positionClosed });
			if (positionClosed.ExitFilledBarIndex < this.PositionsImTracking.LastExitFilledBarIndex) {
				string msg = "recalculate CashCurve from now (positionOpened.ExitFilledBarIndex) till the end (this.PositionsImTracking.LastExitFilledBarIndex)";
				Assembler.PopupException(msg);
			}
			return positionsClosedAbsorbedBoth;
		}
		internal int BuildIncrementalOpenPositionsUpdatedDueToStreamingNewQuote_step2of3(PositionList positionsOpenNow) {
			int positionsUpdatedAbsorbed = 0;

			int earliestIndexUpdated = -1;
			foreach (Position pos in positionsOpenNow.InnerList) {
				if (this.shouldAddupPosition(pos) == false) continue;

				int posIndex = this.PositionsImTrackingReadonly.IndexOf(pos);
				if (posIndex == -1) {
					string msg3 = "IS_IT_STILL_RUNNING_IN_ANOTHER_THREAD??___YOU_DIDNT_INVOKE_BuildIncrementalOnBrokerFilledAlertsOpeningForPositions_step1of3_WITH_THIS_POSITION???";
					Assembler.PopupException(msg3, null, false);
				}
				if (earliestIndexUpdated == -1) {
					earliestIndexUpdated = posIndex;
					continue;
				}
				if (earliestIndexUpdated >= posIndex) continue;
				earliestIndexUpdated = posIndex;
			}
			if (earliestIndexUpdated == -1) {
				string msg2 = "NO_POSITIONS_UPDATED_NO_NEED_TO_RECALCULATE_ANYTHING";
				if (positionsOpenNow.Count > 0) {
					//this.rebuildOLVproperly();
				}
				return positionsUpdatedAbsorbed;
			}
			string msg = "POSITIONS_UPDATED_SO_I_RECALCULATE_FROM_EARLIEST_INDEX_UP_TO_ZERO";
			double cumProfitDollar = 0;
			double cumProfitPercent = 0;

			//for (int i = earliestIndexUpdated; i >= 0; i--) {
			//    Position pos = this.positionsAllReversedCached[i];
			//    if (i == earliestIndexUpdated && i < this.positionsAllReversedCached.Count) {
			//        Position posPrev = this.positionsAllReversedCached[i + 1];
			//        if (this.cumulativeProfitDollar.ContainsKey(posPrev)) {
			//            cumProfitDollar = this.cumulativeProfitDollar[posPrev];
			//        } else {
			//            string msg1 = "REPORTERS.POSITIONS_NONSENSE#1";
			//            Assembler.PopupException(msg1);
			//        }
			//        if (this.cumulativeProfitPercent.ContainsKey(posPrev)) {
			//            cumProfitPercent = this.cumulativeProfitPercent[posPrev];
			//        } else {
			//            string msg1 = "REPORTERS.POSITIONS_NONSENSE#2";
			//            Assembler.PopupException(msg1);
			//        }
			//    }
			//    cumProfitDollar += pos.NetProfit;
			//    cumProfitPercent += pos.NetProfitPercent;

			//    double oldValue = this.cumulativeProfitDollar[pos];
			//    this.cumulativeProfitDollar[pos] = cumProfitDollar;
			//    this.cumulativeProfitPercent[pos] = cumProfitPercent;

			//    double newValue = this.cumulativeProfitDollar[pos];
			//    double difference = newValue - oldValue;
			//    if (difference == 0) {
			//        string msg2 = "DID_YOU_GET_QUOTE_WITH_SAME_BID_ASK????__YOU_SHOULDVE_IGNORED_IT_IN_STREAMING_PROVIDER";
			//        Assembler.PopupException(msg2, null, false);
			//    }
			//}

			//			if (absorbedOpenBoth != pokeUnit.PositionsOpened.Count) {
			//				string msg = pokeUnit.PositionsOpened.Count + " POSITIONS_NOT_ABSORBED_BY " + this.SlicesShortAndLong.ToString() + " HOPE_SLICE_BOTH_WILL_RECEIVE_IT";
			//				Assembler.PopupException(msg);
			//			}
			return positionsUpdatedAbsorbed;
		}
		public int BuildStatsOnBacktestFinished(ReporterPokeUnit pokeUnit) {
			Dictionary<int, List<Position>> positionsOpenedAfterExec	= pokeUnit.PositionsOpened.ByEntryBarFilled;
			Dictionary<int, List<Position>> positionsClosedAfterExec	= pokeUnit.PositionsClosed.ByExitBarFilled;
			
			int positionsOpenAbsorbed = 0;

			int minBar = Int32.MaxValue;
			int maxBar = Int32.MinValue;

			foreach (int bar in positionsOpenedAfterExec.Keys) {
				if (bar < minBar) minBar = bar;
				if (bar > maxBar) maxBar = bar;
			}
			foreach (int bar in positionsClosedAfterExec.Keys) {
				if (bar < minBar) minBar = bar;
				if (bar > maxBar) maxBar = bar;
			}

			if (minBar == Int32.MaxValue) return 0;				// no entries => clean EquityCurve
			if (maxBar == Int32.MinValue) maxBar = minBar;		// some entries & no exits => only CurrentCash drops when we buy

			for (int barIndex = minBar; barIndex <= maxBar; barIndex++) {
				List<Position> positionsOpenedAtBar = positionsOpenedAfterExec.ContainsKey(barIndex)
					? positionsOpenedAfterExec[barIndex] : new List<Position>();
				List<Position> positionsClosedAtBar = positionsClosedAfterExec.ContainsKey(barIndex)
					? positionsClosedAfterExec[barIndex] : new List<Position>();
				if (positionsOpenedAtBar.Count == 0 && positionsClosedAtBar.Count == 0) continue;
				positionsOpenAbsorbed += this.updateStatsForBar(barIndex, positionsOpenedAtBar, positionsClosedAtBar);
			}
			this.BuildStatsCumulative();
			return positionsOpenAbsorbed;
		}
		int updateStatsForBar(int barIndex, List<Position> positionsOpenedAtBar, List<Position> positionsClosedAtBar) {
			DateTime barDateTime = DateTime.MinValue;

			double cashBalanceAtBar = 0;

			double commissionBoth = 0;
			int positionsOpenAbsorbedBoth = 0;

			double commissionWinners = 0;
			double netProfitAtBarWinners = 0;
			double netProfitPctAtBarWinners = 0;
			int barsHeldAtBarWinners = 0;
			int positionsOpenAbsorbedWinners = 0;
			int positionsClosedAbsorbedLong = 0;

			double commissionLosers = 0;
			double netProfitAtBarLosers = 0;
			double netProfitPctAtBarLosers = 0;
			int barsHeldAtBarLosers = 0;
			int positionsOpenAbsorbedLosers = 0;
			int positionsClosedAbsorbedShort = 0;

			foreach (Position positionOpened in positionsOpenedAtBar) {
				//v1 if (this.PositionLongShortImTracking != PositionLongShort.Unknown && entryPosition.PositionLongShort != this.PositionLongShortImTracking) continue;
				if (this.shouldAddupPosition(positionOpened) == false) continue; 
				if (barDateTime == DateTime.MinValue) barDateTime = positionOpened.EntryBar.DateTimeOpen;
				this.checkThrowEntry(positionOpened, barDateTime);
				//bool added = this.PositionsImTracking.AddOpening_step1of2(positionOpened, true);
				//if (added == false) continue;
				positionsOpenAbsorbedBoth++;

				double priceSpent = //(entryPosition.EntryAlert.PriceDeposited != -1) ? entryPosition.EntryAlert.PriceDeposited :
					positionOpened.EntryFilledPrice;
				//if (entryPosition.IsShort) priceSpent *= -1;
				cashBalanceAtBar -= (priceSpent + positionOpened.EntryFilledCommission);
				//if (entryPosition.IsExitFilled == false) continue;
			}

			double netProfitAtBarBoth = 0;
			double netProfitPctAtBarBoth = 0;
			int barsHeldAtBarBoth = 0;
			int positionsClosedAbsorbedBoth = 0;

			foreach (Position positionClosed in positionsClosedAtBar) {
				//v1 if (this.PositionLongShortImTracking != PositionLongShort.Unknown && exitPosition.PositionLongShort != this.PositionLongShortImTracking) continue;
				if (this.shouldAddupPosition(positionClosed) == false) continue;
				if (barDateTime == DateTime.MinValue) barDateTime = positionClosed.ExitBar.DateTimeOpen;
				this.checkThrowExit(positionClosed, barDateTime);

				//bool added = this.PositionsImTracking.AddToClosedDictionary_step2of2(positionClosed, true);
				//if (added == false) continue;
				positionsClosedAbsorbedBoth++;

				double priceReceived = //(exitPosition.ExitAlert.PriceDeposited != -1) ? exitPosition.ExitAlert.PriceDeposited :
					positionClosed.ExitFilledPrice;
				//if (exitPosition.IsShort) priceReceived *= -1;
				cashBalanceAtBar += (priceReceived - positionClosed.ExitFilledCommission);
				netProfitAtBarBoth += positionClosed.NetProfit;
				netProfitPctAtBarBoth += positionClosed.NetProfitPercent;
				barsHeldAtBarBoth += positionClosed.BarsHeld;
				if (positionClosed.NetProfit > 0) {
					commissionWinners += positionClosed.ExitFilledCommission + positionClosed.EntryFilledCommission;
					netProfitAtBarWinners += positionClosed.NetProfit;
					netProfitPctAtBarWinners += positionClosed.NetProfitPercent;
					barsHeldAtBarWinners += positionClosed.BarsHeld;
					positionsOpenAbsorbedWinners++;
					this.curConsecLosers = 0;
					this.curConsecWinners++;
					if (this.MaxConsecWinners < this.curConsecWinners) this.MaxConsecWinners = this.curConsecWinners;
				} else {
					commissionLosers += positionClosed.ExitFilledCommission + positionClosed.EntryFilledCommission;
					netProfitAtBarLosers += positionClosed.NetProfit;
					netProfitPctAtBarLosers += positionClosed.NetProfitPercent;
					barsHeldAtBarLosers += positionClosed.BarsHeld;
					positionsOpenAbsorbedLosers++;
					this.curConsecWinners = 0;
					this.curConsecLosers++;
					if (this.MaxConsecLosers < this.curConsecLosers) this.MaxConsecLosers = this.curConsecLosers;
				}
				this.CumulativeAppendForPositionClosed(positionClosed);
			}

			if (barDateTime == DateTime.MinValue) {
				string msg = "NO_POSITIONS_AFTER_FILTERING_WHAT_I_TRACK_LONG_OR_SHORT";
				return positionsOpenAbsorbedBoth;
			}
			
			this.CashAvailable += cashBalanceAtBar;
			this.EquityCurve.SumupOrAppend(barDateTime, netProfitAtBarBoth);
			this.CashCurve.SumupOrAppend(barDateTime, cashBalanceAtBar);

			this.CommissionBoth += commissionWinners + commissionLosers;
			this.PositionsCountBoth += positionsOpenAbsorbedBoth + positionsClosedAbsorbedBoth;
			this.BarsHeldTotalForClosedPositionsBoth += barsHeldAtBarBoth;
			this.NetProfitForClosedPositionsBoth += netProfitAtBarBoth;
			this.NetProfitPctForClosedPositionsBoth += netProfitPctAtBarBoth;


			this.CommissionWinners += commissionWinners;
			this.PositionsCountWinners += positionsOpenAbsorbedWinners;
			this.BarsHeldTotalForClosedPositionsWinners += barsHeldAtBarWinners;
			this.NetProfitWinners += netProfitAtBarWinners;
			this.NetProfitPctForClosedPositionsLong += netProfitPctAtBarWinners;

			this.CommissionLosers += commissionLosers;
			this.PositionsCountLosers += positionsOpenAbsorbedLosers;
			this.BarsHeldTotalForClosedPositionsLosers += barsHeldAtBarLosers;
			this.NetLossLosers += netProfitAtBarLosers;
			this.NetProfitPctForClosedPositionsLosers += netProfitPctAtBarLosers;

			if (this.NetProfitPeak < this.NetProfitForClosedPositionsBoth) {
				this.NetProfitPeak = this.NetProfitForClosedPositionsBoth;
				this.NetProfitPeakDate = barDateTime;
			}
			double drawDown = this.NetProfitForClosedPositionsBoth - this.NetProfitPeak;
			if (this.MaxDrawDown > drawDown) {
				this.MaxDrawDown = drawDown;
				this.MaxDrawDownLastLossDate = barDateTime;
			}

			return positionsOpenAbsorbedBoth;
		}
		void checkThrowEntry(Position entryPosition, DateTime barDateTime) {
			if (entryPosition.EntryAlert == null) {
				throw new Exception("POSITION_ATBAR_HAS_NO_ENTRY_ALERT");
			}
			if (entryPosition.EntryDate == DateTime.MinValue) {
				throw new Exception("POSITION_ATBAR_HAS_NO_ENTRY_DATE"
					+ " while EntryAlert.FilledBar.DateTimeOpen[" + entryPosition.EntryAlert.FilledBar.DateTimeOpen + "]");
			}
			if (barDateTime != entryPosition.EntryBar.DateTimeOpen) {
				throw new Exception("POSITION_ATBAR_HAS_ENTRY_DATE_DIFFERENT_FROM_OTHER_POSITIONS"
					+ " otherAlerts[" + barDateTime + "] entryPosition.EntryBar.DateTimeOpen[" + entryPosition.EntryBar.DateTimeOpen + "]");
			}
		}
		void checkThrowExit(Position exitPosition, DateTime barDateTime) {
			if (exitPosition.ExitAlert == null) {
				throw new Exception("POSITION_ATBAR_HAS_NO_EXIT_ALERT");
			}
			if (exitPosition.ExitDate == DateTime.MinValue) {
				throw new Exception("POSITION_ATBAR_HAS_NO_EXIT_DATE"
					+ " while ExitAlert.FilledBar.DateTimeOpen[" + exitPosition.ExitAlert.FilledBar.DateTimeOpen + "]");
			}
			if (barDateTime != exitPosition.ExitBar.DateTimeOpen) {
				throw new Exception("POSITION_ATBAR_HAS_EXIT_DATE_DIFFERENT_FROM_OTHER_POSITIONS"
					+ " otherAlerts[" + barDateTime + "] exitPosition.ExitBar.DateTimeOpen[" + exitPosition.ExitBar.DateTimeOpen + "]");
			}
		}
		bool shouldAddupPosition(Position position) {
			bool ret = false;
			if (position.PositionLongShort == PositionLongShort.Unknown) {
				string msg = "POSITION_MUST_BE_LONG_OR_SHORT__UNKNOWN_CANNOT_BE_ACCEPTED_TO_ADDUP_INTO_PERFORMANCE_SLICE " + this.ToString();
				Assembler.PopupException(msg);
				return ret;
			}
			switch (this.PositionLongShortImTracking) {
				case SystemPerformancePositionsTracking.Unknown:
					string msg = "SHOULD_NEVER_BE_USED_FOR_INSTANTIATED_SLICES SystemPerformancePositionsTracking." + this.PositionsImTracking;
					Assembler.PopupException(msg);
					break;
				case SystemPerformancePositionsTracking.LongOnly:
					ret = (position.PositionLongShort == PositionLongShort.Long) ? true : false;
					break;
				case SystemPerformancePositionsTracking.ShortOnly:
					ret = (position.PositionLongShort == PositionLongShort.Short) ? true : false;
					break;
				case SystemPerformancePositionsTracking.LongAndShort:
					ret = true;
					break;
				case SystemPerformancePositionsTracking.BuyAndHold:
					ret = false;
					break;
				default:
					string msg2 = "ADD_CASE_IF_YOU_EXTENDED_SystemPerformancePositionsTracking";
					Assembler.PopupException(msg2);
					break;
			}
			return ret;
		}
		public override string ToString() {
			return "Slice[" + this.PositionLongShortImTracking + "] NetProfit[" + this.NetProfitForClosedPositionsBoth + "]"
				+ "[" + this.ReasonToExist + "]"
				;
		}
	}
}