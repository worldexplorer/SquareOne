using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

using Sq1.Core.DataTypes;
using Sq1.Core.Execution;

namespace Sq1.Core.StrategyBase {
	public class SystemPerformanceSlice {
		private SystemPerformance parentPerformance;
		public string ReasonToExist;
		public PositionLongShort PositionLongShortImTracking { get; private set; }
		public DataSeriesTimeBased EquityCurve { get; private set; }
		public DataSeriesTimeBased CashCurve { get; private set; }
		public List<Position> PositionsImTracking { get; private set; }
		public IList<Position> PositionsImTrackingReadOnly { get { return this.PositionsImTracking.AsReadOnly(); } }
		public List<Position> PositionsImTrackingReversed { get { 
				List<Position> ret = new List<Position>();
				foreach (Position pos in this.PositionsImTrackingReadOnly) ret.Insert(0, pos);
				return ret;
			} }
		

		public double CashAvailable;

		public DateTime NetProfitPeakDate;
		public double NetProfitPeak;
		public DateTime MaxDrawDownLastLossDate;
		public double MaxDrawDown;

		double curConsecWinners;
		double curConsecLosers;
		public double MaxConsecWinners;
		public double MaxConsecLosers;

		public double ProfitFactor { get {
				if (this.NetLossLosers == 0) return double.PositiveInfinity;
				return this.NetProfitWinners / Math.Abs(this.NetLossLosers);
			} }
		public double RecoveryFactor { get {
				if (this.MaxDrawDown == 0) return double.PositiveInfinity;
				return Math.Abs(this.NetProfitForClosedPositionsBoth / this.MaxDrawDown);
			} }
		public double WinRatePct { get {
				if (this.PositionsCountBoth == 0) return double.PositiveInfinity;
				return 100 * this.PositionsCountWinners / this.PositionsCountBoth;
			} }
		public double LossRatePct { get {
				if (this.PositionsCountBoth == 0) return double.PositiveInfinity;
				return 100 * this.PositionsCountLosers / this.PositionsCountBoth;
			} }
		public double WinLossRatio  { get {
				double ret = double.NaN;
				if (this.PositionsCountLosers == 0) return ret;
				// I_HATE_IT 296/452 = 0 !!!! if you don't convert divider to (double)
				return this.PositionsCountWinners / (double) this.PositionsCountLosers;
			} }
		public double PayoffRatio { get {
			if (this.AvgLossPctLosers == 0) return 0;
			return Math.Abs(this.AvgProfitPctBoth / this.AvgLossPctLosers);
			} }

		#region AllTrades
		public double CommissionBoth;
		public double NetProfitPctForClosedPositionsBoth;
		public double NetProfitForClosedPositionsBoth;
		public int BarsHeldTotalForClosedPositionsBoth;
		public double ProfitPerBarBoth { get {
			if (this.BarsHeldTotalForClosedPositionsBoth == 0) return 0;
				return this.NetProfitForClosedPositionsBoth / (double)this.BarsHeldTotalForClosedPositionsBoth;
			} }
		public int PositionsCountBoth;
		public double AvgProfitBoth { get {
			if (this.PositionsCountBoth == 0) return 0;
			return this.NetProfitForClosedPositionsBoth / this.PositionsCountBoth;
		} }
		public double AvgProfitPctBoth { get {
			if (this.PositionsCountBoth == 0) return 0;
			return this.NetProfitPctForClosedPositionsBoth / this.PositionsCountBoth;
		} }
		public double AvgBarsHeldBoth { get {
			if (this.PositionsCountBoth == 0) return 0;
			return this.BarsHeldTotalForClosedPositionsBoth / this.PositionsCountBoth;
		} }
		#endregion

		#region Winners
		public double CommissionWinners;
		public double NetProfitPctForClosedPositionsLong;
		public double NetProfitWinners;
		public int BarsHeldTotalForClosedPositionsWinners;
		public double ProfitPerBarLong { get {
				if (this.BarsHeldTotalForClosedPositionsWinners == 0) return 0;
				return this.NetProfitWinners / (double)this.BarsHeldTotalForClosedPositionsWinners;
			} }
		public int PositionsCountWinners;
		public double AvgProfitWinners { get {
				if (this.PositionsCountWinners == 0) return 0;
				return this.NetProfitWinners / this.PositionsCountWinners;
			} }
		public double AvgProfitPctWinners { get {
				if (this.PositionsCountWinners == 0) return 0;
				return this.NetProfitPctForClosedPositionsLong / this.PositionsCountWinners;
			} }
		public double AvgBarsHeldWinners { get {
				if (this.PositionsCountWinners == 0) return 0;
				return this.BarsHeldTotalForClosedPositionsWinners / this.PositionsCountWinners;
			} }
		#endregion

		#region Losers
		public double CommissionLosers;
		public double NetProfitPctForClosedPositionsLosers;
		public double NetLossLosers;
		public int BarsHeldTotalForClosedPositionsLosers;
		public double ProfitPerBarLosers { get {
				if (this.BarsHeldTotalForClosedPositionsLosers == 0) return 0;
				return this.NetLossLosers / (double)this.BarsHeldTotalForClosedPositionsLosers;
			} }
		public int PositionsCountLosers;
		public double AvgLossLosers { get {
				if (this.PositionsCountLosers == 0) return 0;
				return this.NetLossLosers / this.PositionsCountLosers;
			} }
		public double AvgLossPctLosers { get {
				if (this.PositionsCountLosers == 0) return 0;
				return this.NetProfitPctForClosedPositionsLosers / this.PositionsCountLosers;
			} }
		public double AvgBarsHeldLosers { get {
				if (this.PositionsCountLosers == 0) return 0;
				return this.BarsHeldTotalForClosedPositionsLosers / this.PositionsCountLosers;
			} }
		#endregion

		protected SystemPerformanceSlice() {
			ReasonToExist = "NO_REASON_TO_EXIST";
			// LOOKS_STUPID_BUT_DONT_SUGGEST_BRINGING_EXECUTOR_HERE: new BarScaleInterval(BarScale.Unknown, 0)
			EquityCurve = new DataSeriesTimeBased(new BarScaleInterval(BarScale.Unknown, 0), "Equity");
			CashCurve = new DataSeriesTimeBased(new BarScaleInterval(BarScale.Unknown, 0), "Cash");
			PositionsImTracking = new List<Position>();
			PositionLongShortImTracking = PositionLongShort.Unknown;	// direction not specified => it means "both short and long" here
		}
		public SystemPerformanceSlice(SystemPerformance performance, PositionLongShort positionLongShortImTracking, string reasonToExist) : this() {
			parentPerformance = performance;
			PositionLongShortImTracking = positionLongShortImTracking;
			ReasonToExist = reasonToExist;
		}
		internal void Initialize(double startingCapital = -1) {
			this.PositionsImTracking.Clear();
			this.EquityCurve.Clear();
			this.CashCurve.Clear();
			this.CashAvailable = 0;

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
		public int BuildStatsIncrementallyOnEachBarExecFinished (Dictionary<int, List<Position>> positionsOpenedAfterExec, Dictionary<int, List<Position>> positionsClosedAfterExec) {
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
			return positionsOpenAbsorbed;
		}
		int updateStatsForBar(int barIndex, List<Position> positionsOpenedAtBar, List<Position> positionsClosedAtBar) {
			DateTime barDateTime = DateTime.MinValue;

			double cashBalanceAtBar = 0;

			double commissionBoth = 0;
			double netProfitAtBarBoth = 0;
			double netProfitPctAtBarBoth = 0;
			int barsHeldAtBarBoth = 0;
			int positionsOpenAbsorbedBoth = 0;
			int positionsClosedAbsorbedBoth = 0;

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

			foreach (Position entryPosition in positionsOpenedAtBar) {
				if (this.PositionLongShortImTracking != PositionLongShort.Unknown
						&& entryPosition.PositionLongShort != this.PositionLongShortImTracking) continue;
				if (barDateTime == DateTime.MinValue) barDateTime = entryPosition.EntryBar.DateTimeOpen;
				this.checkThrowEntry(entryPosition, barDateTime);
				double priceSpent = //(entryPosition.EntryAlert.PriceDeposited != -1) ? entryPosition.EntryAlert.PriceDeposited :
					entryPosition.EntryFilledPrice;
				//if (entryPosition.IsShort) priceSpent *= -1;
				cashBalanceAtBar -= (priceSpent + entryPosition.EntryFilledCommission);
				if (entryPosition.IsExitFilled == false) continue;

				commissionBoth += entryPosition.EntryFilledCommission;
				int absorbed = this.absorbPositionsImTracking(entryPosition);
				positionsOpenAbsorbedBoth += absorbed;
				if (entryPosition.NetProfit > 0) {
					commissionWinners += entryPosition.EntryFilledCommission;
					positionsOpenAbsorbedWinners += absorbed;
				} else {
					commissionLosers += entryPosition.EntryFilledCommission;
					positionsOpenAbsorbedLosers += absorbed;
				}
			}
			foreach (Position exitPosition in positionsClosedAtBar) {
				if (this.PositionLongShortImTracking != PositionLongShort.Unknown
						&& exitPosition.PositionLongShort != this.PositionLongShortImTracking) continue;
				if (barDateTime == DateTime.MinValue) barDateTime = exitPosition.ExitBar.DateTimeOpen;
				this.checkThrowExit(exitPosition, barDateTime);

				double priceReceived = //(exitPosition.ExitAlert.PriceDeposited != -1) ? exitPosition.ExitAlert.PriceDeposited : 
				exitPosition.ExitFilledPrice;
				//if (exitPosition.IsShort) priceReceived *= -1;
				cashBalanceAtBar += (priceReceived - exitPosition.ExitFilledCommission);
				
				int absorbed = this.absorbPositionsImTracking(exitPosition);
				positionsClosedAbsorbedBoth += absorbed;
				netProfitAtBarBoth += exitPosition.NetProfit;
				netProfitPctAtBarBoth += exitPosition.NetProfitPercent;
				barsHeldAtBarBoth += exitPosition.BarsHeld;
				if (exitPosition.NetProfit > 0) {
					commissionWinners += exitPosition.ExitFilledCommission;
					netProfitAtBarWinners += exitPosition.NetProfit;
					netProfitPctAtBarWinners += exitPosition.NetProfitPercent;
					barsHeldAtBarWinners += exitPosition.BarsHeld;
					positionsOpenAbsorbedWinners += absorbed;
					this.curConsecLosers = 0;
					this.curConsecWinners++;
					if (this.MaxConsecWinners < this.curConsecWinners) this.MaxConsecWinners = this.curConsecWinners;
				} else {
					commissionLosers += exitPosition.ExitFilledCommission;
					netProfitAtBarLosers += exitPosition.NetProfit;
					netProfitPctAtBarLosers += exitPosition.NetProfitPercent;
					barsHeldAtBarLosers += exitPosition.BarsHeld;
					positionsOpenAbsorbedLosers += absorbed;
					this.curConsecWinners = 0;
					this.curConsecLosers++;
					if (this.MaxConsecLosers < this.curConsecLosers) this.MaxConsecLosers = this.curConsecLosers;

				}
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
		public void BuildStatsOnBacktestFinished(Dictionary<int, List<Position>> posByEntry, Dictionary<int, List<Position>> posByExit) {
			this.BuildStatsIncrementallyOnEachBarExecFinished(posByEntry, posByExit);
		}
		int absorbPositionsImTracking(Position position) {
			int absorbed = 0;
			if (position.Shares == 0.0) {
				string msg = "PERFORMANCE_SLICE_GOT_ZERO_SIZE_POSITION position[" + position + "]";
				Assembler.PopupException(msg);
				return absorbed;
			}
			if (this.ContainsIdenticalPosition(position)) {
				string msg = "PERFORMANCE_SLICE_GOT_POSITION_DUPE position[" + position + "]";
				Assembler.PopupException(msg);
				return absorbed;
			}
			if (this.PositionLongShortImTracking == PositionLongShort.Unknown) {
				this.PositionsImTracking.Add(position);
				absorbed++;
				return absorbed;
			}
			if (position.PositionLongShort != this.PositionLongShortImTracking) return absorbed;
			this.PositionsImTracking.Add(position);
			absorbed++;
			return absorbed;
		}
		public bool ContainsIdenticalPosition(Position maybeAlready) {
			return this.PositionsImTracking.Contains(maybeAlready);
		}
		public override string ToString() {
			return "Slice[" + this.PositionLongShortImTracking + "] NetProfit[" + this.NetProfitForClosedPositionsBoth + "]"
				+ "[" + this.ReasonToExist + "]"
				;
		}
	}
}