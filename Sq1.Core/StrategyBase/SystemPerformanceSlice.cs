using System;
using System.Collections.Generic;

using Sq1.Core.DataTypes;
using Sq1.Core.Execution;
using Sq1.Core.Support;

namespace Sq1.Core.StrategyBase {
	public class SystemPerformanceSlice {
		public	string				ReasonToExist;
		public	SystemPerformancePositionsTracking	PositionLongShortImTracking		{ get; private set; }
		
		public	DataSeriesTimeBased	CashCurve						{ get; private set; }
		
		// cumulativeProfitDollar,Percent+CalcEachStreamingUpdate should be moved from Reporters.Positions to SystemPerformanceSlice for other Reporters to be used in their drawings
		// per-Position is used by Reporters.Positions to re-draw only particular changed rows in the list of Positions
		// (DataSeriesTimeBased isn't good enough for this due to too many lookups to match datetime to a position)
		public	Dictionary<Position, double> CumulativeNetProfitDollar;
		public	Dictionary<Position, double> CumulativeNetProfitPercent;
		
		public	PositionList		PositionsImTracking				{ get; private set; }
		public	List<Position>		PositionsImTrackingReadonly		{ get { return this.PositionsImTracking.SafeCopy(this, "//PositionsImTrackingReadonly(WAIT)"); } }

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
				if (this.PositionsCount == 0) return double.PositiveInfinity;
				return Math.Round(100 * this.PositionsCountWinners / (double)this.PositionsCount, 1);
			} }
		public double LossRatePct	{ get {
				if (this.PositionsCount == 0) return double.PositiveInfinity;
				return Math.Round(100 * this.PositionsCountLosers / (double)this.PositionsCount, 2);
			} }
		public double WinLossRatio  { get {
				double ret = double.NaN;
				if (this.PositionsCountLosers == 0) return ret;
				// I_HATE_IT 296/452 = 0 !!!! if you don't convert divider to (double)
				return Math.Round(this.PositionsCountWinners / (double)this.PositionsCountLosers, 2);
			} }
		public double PayoffRatio { get {
				if (this.AvgLossPctLosers == 0) return 0;
				return Math.Round(Math.Abs(this.PositionAvgProfitPctBoth / (double)this.AvgLossPctLosers), 2);
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
		public int PositionsCount;
		public double PositionAvgProfitBoth { get {
				if (this.PositionsCount == 0) return 0;
				return Math.Round(this.NetProfitForClosedPositionsBoth / (double)this.PositionsCount, 2);
			} }
		public double PositionAvgProfitPctBoth { get {
				if (this.PositionsCount == 0) return 0;
				return Math.Round(this.NetProfitPctForClosedPositionsBoth / (double)this.PositionsCount, 2);
			} }
		public double PositionAvgBarsHeldBoth { get {
				if (this.PositionsCount == 0) return 0;
				return Math.Round(this.BarsHeldTotalForClosedPositionsBoth / (double)this.PositionsCount, 1);
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

		public SortedDictionary<DateTime, KPIs> KPIsCumulativeByDate { get; private set; }
		int positionsCountPrevStep_MustIncreaseOnly;
	
		protected SystemPerformanceSlice() {
			ReasonToExist = "NO_REASON_TO_EXIST";
			// LOOKS_STUPID_BUT_DONT_SUGGEST_BRINGING_EXECUTOR_HERE: new BarScaleInterval(BarScale.Unknown, 0)
			CashCurve	= new DataSeriesTimeBased(new BarScaleInterval(BarScale.Unknown, 0), "Cash");
			
			CumulativeNetProfitDollar		= new Dictionary<Position, double>();
			CumulativeNetProfitPercent		= new Dictionary<Position, double>();

			PositionsImTracking = new PositionList("PositionsImTracking", null);
			//v1 PositionLongShortImTracking = PositionLongShort.Unknown;	// direction not specified => it means "both short and long" here
			//v2
			PositionLongShortImTracking		= SystemPerformancePositionsTracking.LongAndShort;
			KPIsCumulativeByDate			= new SortedDictionary<DateTime, KPIs>();
		}
		public SystemPerformanceSlice(SystemPerformancePositionsTracking positionLongShortImTracking, string reasonToExist) : this() {
			PositionLongShortImTracking = positionLongShortImTracking;
			ReasonToExist = reasonToExist;
		}
		internal void Initialize(double startingCapital = -1) {
			this.PositionsImTracking.Clear(this, "Clear");
			this.CashCurve.Clear();
			this.CashAvailable = 0;

			this.CumulativeNetProfitDollar.Clear();
			this.CumulativeNetProfitPercent.Clear();

			this.MaxDrawDownLastLossDate				= DateTime.MinValue;
			this.MaxDrawDown							= 0;
			this.NetProfitPeakDate						= DateTime.MinValue;
			this.NetProfitPeak							= 0;

			this.curConsecWinners						= 0;
			this.curConsecLosers						= 0;
			this.MaxConsecWinners						= 0;
			this.MaxConsecLosers						= 0;

			this.NetProfitForClosedPositionsBoth		= 0;
			this.BarsHeldTotalForClosedPositionsBoth	= 0;
			this.PositionsCount						= 0;

			this.NetProfitWinners						= 0;
			this.BarsHeldTotalForClosedPositionsWinners	= 0;
			this.PositionsCountWinners					= 0;

			this.NetLossLosers							= 0;
			this.BarsHeldTotalForClosedPositionsLosers	= 0;
			this.PositionsCountLosers					= 0;

			this.KPIsCumulativeByDate.Clear();
			this.positionsCountPrevStep_MustIncreaseOnly = 0;
		}
		[Obsolete("GET_RID_OF_ME__IM_TOO_EXPENSIVE__REPLACE_WITH_RANGED_updateStatsForBar+CumulativeAppendForPositionClosed")]
		public void BuildStatsCumulativeOnBacktestFinished() {
			double cumProfitDollar = 0;
			double cumProfitPercent = 0;
			this.CumulativeNetProfitDollar.Clear();
			this.CumulativeNetProfitPercent.Clear();
			foreach (Position pos in this.PositionsImTrackingReadonly) {
				if (this.CumulativeNetProfitDollar.ContainsKey(pos)) {
					string msg = "CUMULATIVES_ALREADY_CALCULATED_FOR_THIS_POSITION SystemPerformanceSlice["
						+ this.ToString() + "].PositionsImTracking contains duplicate " + pos;
					Assembler.PopupException(msg);
					continue;
				}
				cumProfitDollar += pos.NetProfit;
				cumProfitPercent += pos.NetProfitPercent;
				this.CumulativeNetProfitDollar.Add(pos, cumProfitDollar);
				this.CumulativeNetProfitPercent.Add(pos, cumProfitPercent);
			}
		}
		public void CumulativeAppendOrReplaceForPositionClosedOrOpenNow(Position positionClosedOrUpdating, bool throwIfWasntAddedBeforeReplaceMode = true) {
			if (this.CumulativeNetProfitDollar.ContainsKey(positionClosedOrUpdating)) {
				string msg = "DOING_REPLACE_INSTEAD_OF_ADDING!!! CUMULATIVES_ALREADY_CALCULATED_FOR_THIS_POSITION SystemPerformanceSlice["
					+ this.ToString() + "].PositionsImTracking[" + this.PositionsImTracking.ToString() + "] already contains " + positionClosedOrUpdating;
				//Assembler.PopupException(msg);
				//return;
			}

			double prevCumProfitDollar = 0;
			double prevCumProfitPercent = 0;

			Position preLastPositionTracked = this.PositionsImTracking.PreLast_nullUnsafe(this, "CumulativeAppendOrReplaceForPositionClosedOrOpenNow(WAIT)");
			if (preLastPositionTracked != null) {
				if (this.CumulativeNetProfitDollar.ContainsKey(preLastPositionTracked) == false) {
					if (this.CumulativeNetProfitDollar.Count > this.PositionsImTracking.Count - 2) {
						string msg2 = "CumulativeProfitDollar_SHOULD_ALREADY_CONTAIN_preLastPositionTracked " + preLastPositionTracked;
						Assembler.PopupException(msg2);
					}
				} else {
					prevCumProfitDollar = this.CumulativeNetProfitDollar[preLastPositionTracked];
				}
				if (this.CumulativeNetProfitPercent.ContainsKey(preLastPositionTracked) == false) {
					if (this.CumulativeNetProfitPercent.Count > this.PositionsImTracking.Count - 2) {
						string msg2 = "CumulativeProfitPercent_SHOULD_ALREADY_CONTAIN_preLastPositionTracked " + preLastPositionTracked;
						Assembler.PopupException(msg2);
					}
				} else {
					prevCumProfitPercent = this.CumulativeNetProfitPercent[preLastPositionTracked];
				}
			}

			double cumProfitDollar	= prevCumProfitDollar	+ positionClosedOrUpdating.NetProfit;
			double cumProfitPercent	= prevCumProfitPercent	+ positionClosedOrUpdating.NetProfitPercent;

			if (this.CumulativeNetProfitDollar.ContainsKey(positionClosedOrUpdating) == false) {
				if (throwIfWasntAddedBeforeReplaceMode) {
					string msg = "I_WAS_EXPECTING_REPLACEMENT_BUT_IT_WASNT_ADDED__YOU_SHOULD_INVOKE_STEP1_BEFORE_INVOKING_STEP2 CumulativeProfitDollar";
					Assembler.PopupException(msg);
					return;
				}
				this.CumulativeNetProfitDollar.Add(positionClosedOrUpdating, cumProfitDollar);
			} else {
				this.CumulativeNetProfitDollar[positionClosedOrUpdating] = cumProfitDollar;
			}
			if (this.CumulativeNetProfitPercent.ContainsKey(positionClosedOrUpdating) == false) {
				if (throwIfWasntAddedBeforeReplaceMode) {
					string msg = "I_WAS_EXPECTING_REPLACEMENT_BUT_IT_WASNT_ADDED__YOU_SHOULD_INVOKE_STEP1_BEFORE_INVOKING_STEP2 CumulativeProfitPercent";
					Assembler.PopupException(msg);
					return;
				}
				this.CumulativeNetProfitPercent.Add(positionClosedOrUpdating, cumProfitPercent);
			} else {
				this.CumulativeNetProfitPercent[positionClosedOrUpdating] = cumProfitPercent;
			}
		}
		internal int BuildIncrementalBrokerFilledAlertsOpeningForPositions_step1of3(Position positionOpened) {
			int positionsOpenAbsorbedBoth = 0;
			if (this.positionIsMineShouldAppendAndUpdate(positionOpened) == false) return positionsOpenAbsorbedBoth;
			bool added = this.PositionsImTracking.AddOpened_step1of2(positionOpened, this, "BuildIncrementalBrokerFilledAlertsOpeningForPositions_step1of3(WAIT)", ConcurrentWatchdog.TIMEOUT_DEFAULT, true);
			if (added == false) {
				string msg = "IS_THIS_WHY_I_GET_EMPTY_INNER_LIST_FOR_SLICE_BOTH?";
				Assembler.PopupException(msg);
				return positionsOpenAbsorbedBoth;
			}

			positionsOpenAbsorbedBoth += this.updateAtomicStatsForPositionsClosedAtBar(positionOpened.EntryFilledBarIndex, new List<Position>() { positionOpened }, new List<Position>());
			if (positionOpened.EntryFilledBarIndex < this.PositionsImTracking.LastBarIndexEntry) {
				string msg = "recalculate CashCurve from now (positionOpened.EntryFilledBarIndex) till the end (this.PositionsImTracking.LastEntryFilledBarIndex)";
				Assembler.PopupException(msg);
			}
			this.CumulativeAppendOrReplaceForPositionClosedOrOpenNow(positionOpened, false);

			return positionsOpenAbsorbedBoth;
		}
		internal int BuildReportIncrementalBrokerFilledAlertsClosingForPositions_step3of3(Position positionClosed) {
			int positionsClosedAbsorbedBoth = 0;
			if (this.positionIsMineShouldAppendAndUpdate(positionClosed) == false) return positionsClosedAbsorbedBoth; 
			bool added = this.PositionsImTracking.AddToClosedDictionary_step2of2(positionClosed, this, "BuildIncrementalBrokerFilledAlertsOpeningForPositions_step1of3(WAIT)", ConcurrentWatchdog.TIMEOUT_DEFAULT, true);
			if (added == false) {
				return positionsClosedAbsorbedBoth;
			}

			positionsClosedAbsorbedBoth += this.updateAtomicStatsForPositionsClosedAtBar(positionClosed.ExitFilledBarIndex, new List<Position>(), new List<Position>() { positionClosed });
			if (positionClosed.ExitFilledBarIndex < this.PositionsImTracking.LastBarIndexExit) {
				string msg = "recalculate CashCurve from now (positionOpened.ExitFilledBarIndex) till the end (this.PositionsImTracking.LastExitFilledBarIndex)";
				Assembler.PopupException(msg);
			}
			return positionsClosedAbsorbedBoth;
		}
		internal int BuildIncrementalOpenPositionsUpdatedDueToStreamingNewQuote_step2of3(PositionList positionsOpenNow) {
			int positionsUpdatedAbsorbed = 0;

			var byEntryBarFilledSafeCopySafeCopy = positionsOpenNow.ByEntryBarFilledSafeCopy(
				this, "//BuildIncrementalOpenPositionsUpdatedDueToStreamingNewQuote_step2of3(WAIT)");
			List<int> barIndexesAsc = new List<int>(byEntryBarFilledSafeCopySafeCopy.Keys);
			barIndexesAsc.Sort();

			List<Position> positionsClosedAtBar = new List<Position>();
			foreach (int barIndex in barIndexesAsc) {
				List<Position> positionsOpenedAtBar = byEntryBarFilledSafeCopySafeCopy[barIndex];
				foreach (Position positionOpenedAtBar in positionsOpenedAtBar) {
					if (positionsOpenedAtBar.Count != 1) {
						string msg = "MULTIPLE_POSITIONS_OPEN_FOR_ONE_BAR[" + barIndex + "] positionOpenedAtBar[" + positionOpenedAtBar + "]";
						Assembler.PopupException(msg, null, false);
					}
					this.CumulativeAppendOrReplaceForPositionClosedOrOpenNow(positionOpenedAtBar, false);
					positionsUpdatedAbsorbed++;
				}
			}

			if (positionsUpdatedAbsorbed != positionsOpenNow.Count) {
				string msg = " SOME_POSITIONS_NOT_ABSORBED_BY " + this.ToString()
					+ " positionsUpdatedAbsorbed[" + positionsUpdatedAbsorbed + "]"
					+ " positionsOpenNow[" + positionsOpenNow.Count + "]";
				Assembler.PopupException(msg, null, false);
			}
			return positionsUpdatedAbsorbed;
		}
		public int BuildStatsOnBacktestFinished(ReporterPokeUnit pokeUnit) {
			int positionsOpenedAbsorbedToInternal = 0;
			List<Position> openedSafe = pokeUnit.PositionsOpened.SafeCopy(this, "//BuildStatsOnBacktestFinished(WAIT)");
			foreach (Position positionOpened in openedSafe) {
				if (this.positionIsMineShouldAppendAndUpdate(positionOpened) == false) continue;
				bool added = this.PositionsImTracking.AddOpened_step1of2(positionOpened, this, "BuildIncrementalBrokerFilledAlertsOpeningForPositions_step1of3(WAIT)", ConcurrentWatchdog.TIMEOUT_DEFAULT, true);
				if (added == false) {
					string msg = "IS_THIS_WHY_I_GET_EMPTY_INNER_LIST_FOR_SLICE_BOTH? SHOULD_I_CLEAR_BEFORE_BuildStatsOnBacktestFinished()?";
					Assembler.PopupException(msg);
					continue;
				}
				positionsOpenedAbsorbedToInternal++;
			}
			if (positionsOpenedAbsorbedToInternal != openedSafe.Count) {
				string msg = "NO_ITS_FINE IS_THERE_ANY_PROBLEM? positionsOpenedAbsorbedToInternal[" + positionsOpenedAbsorbedToInternal + "] != openedSafe.Count[" + openedSafe.Count + "]";
				//Assembler.PopupException(msg, null, false);
			}
			
			int positionsClosedAbsorbedToInternal = 0;
			List<Position> closedSafe = pokeUnit.PositionsClosed.SafeCopy(this, "//BuildStatsOnBacktestFinished(WAIT)");
			foreach (Position positionClosed in closedSafe) {
				if (this.positionIsMineShouldAppendAndUpdate(positionClosed) == false) continue; 
				bool added = this.PositionsImTracking.AddToClosedDictionary_step2of2(positionClosed, this, "BuildIncrementalBrokerFilledAlertsOpeningForPositions_step1of3(WAIT)", ConcurrentWatchdog.TIMEOUT_DEFAULT, true);
				if (added == false) {
					string msg = "IS_THIS_WHY_I_GET_EMPTY_INNER_LIST_FOR_SLICE_BOTH? SHOULD_I_CLEAR_BEFORE_BuildStatsOnBacktestFinished()?";
					Assembler.PopupException(msg);
					continue;
				}
				positionsClosedAbsorbedToInternal++;
			}
			if (positionsClosedAbsorbedToInternal != closedSafe.Count) {
					string msg = "IS_THERE_ANY_PROBLEM?";
					//Assembler.PopupException(msg);
			}


			Dictionary<int, List<Position>> positionsOpenedAfterExec	= pokeUnit.PositionsOpened.ByEntryBarFilledSafeCopy(this, "BuildStatsOnBacktestFinished(WAIT)");
			Dictionary<int, List<Position>> positionsClosedAfterExec	= pokeUnit.PositionsClosed.ByExitBarFilledSafeCopy(this, "BuildStatsOnBacktestFinished(WAIT)");
			
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

				positionsOpenAbsorbed += this.updateAtomicStatsForPositionsClosedAtBar(barIndex, positionsOpenedAtBar, positionsClosedAtBar);
			}
			this.BuildStatsCumulativeOnBacktestFinished();
			return positionsOpenAbsorbed;
		}

		List<Position> positionsOpenedAccounted = new List<Position>();
		List<Position> positionsClosedAccounted = new List<Position>();
		int updateAtomicStatsForPositionsClosedAtBar(int barIndex, List<Position> positionsOpenedAtBar, List<Position> positionsClosedAtBar) {
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
				if (positionsOpenedAccounted.Contains(positionOpened)) {
					string msg = "LIVESIM_ABORTED_WITH_POSITION_OPEN SHOULD_BE_INVOKED_FOR_LIVE_OPENED_POSITION_ONCE_ON_STEP1_OR_STEP3__DONT_USE_THIS_METHOD_FOR_STEP2";
					Assembler.PopupException(msg);
				}
				positionsOpenedAccounted.Add(positionOpened);

				//v1 if (this.PositionLongShortImTracking != PositionLongShort.Unknown && entryPosition.PositionLongShort != this.PositionLongShortImTracking) continue;
				if (this.positionIsMineShouldAppendAndUpdate(positionOpened) == false) continue;
				if (barDateTime == DateTime.MinValue) barDateTime = positionOpened.EntryBar.DateTimeOpen;
				if (positionOpened.EntryDateBarTimeOpen != barDateTime) {
					string msg = "YOU_GOT_POSITIONS_WITH_ALL_DIFFERENT_DATES_WHILE_MUST_BE_SAME_FOR_positionsOpenedAtBar";
					Assembler.PopupException(msg);
				}
				
				positionsOpenAbsorbedBoth++;

				double priceSpent = //(entryPosition.EntryAlert.PriceDeposited != -1) ? entryPosition.EntryAlert.PriceDeposited :
					positionOpened.EntryFilled_price;
				//if (entryPosition.IsShort) priceSpent *= -1;
				cashBalanceAtBar -= (priceSpent + positionOpened.EntryFilled_commission);
				//if (entryPosition.IsExitFilled == false) continue;
			}

			double netProfitAtBarBoth = 0;
			double netProfitPctAtBarBoth = 0;
			int barsHeldAtBarBoth = 0;
			int positionsClosedAbsorbedBoth = 0;

			foreach (Position positionClosed in positionsClosedAtBar) {
				if (positionClosed.ExitAlert == null) {
					string msg = "QUE_BOI_HACER_HERMA? positionClosed.ExitAlert=null";
					Assembler.PopupException(msg);
					continue;
				}
				if (positionClosed.ExitAlert.ForcefullyClosedBacktestLastPosition) continue;
				if (positionsClosedAccounted.Contains(positionClosed)) {
					string msg = "LIVESIM_ABORTED_WITH_POSITION_OPEN SHOULD_BE_INVOKED_FOR_LIVE_CLOSED_POSITION_ONCE";
					Assembler.PopupException(msg);
				}
				positionsClosedAccounted.Add(positionClosed);

				//v1 if (this.PositionLongShortImTracking != PositionLongShort.Unknown && exitPosition.PositionLongShort != this.PositionLongShortImTracking) continue;
				if (this.positionIsMineShouldAppendAndUpdate(positionClosed) == false) continue;
				if (barDateTime == DateTime.MinValue) barDateTime = positionClosed.ExitBar.DateTimeOpen;
				if (positionClosed.ExitDateBarTimeOpen != barDateTime) {
					string msg = "YOU_GOT_POSITIONS_WITH_ALL_DIFFERENT_DATES_WHILE_MUST_BE_SAME_FOR_positionsClosedAtBar barDateTime[" + barDateTime + "] " + positionClosed;
					Assembler.PopupException(msg);
				}

				positionsClosedAbsorbedBoth++;

				double priceReceived = //(exitPosition.ExitAlert.PriceDeposited != -1) ? exitPosition.ExitAlert.PriceDeposited :
					positionClosed.ExitFilled_price;
				//if (exitPosition.IsShort) priceReceived *= -1;
				cashBalanceAtBar		+= (priceReceived - positionClosed.ExitFilled_commission);
				netProfitAtBarBoth		+= positionClosed.NetProfit;
				netProfitPctAtBarBoth	+= positionClosed.NetProfitPercent;
				barsHeldAtBarBoth		+= positionClosed.BarsHeld;
				if (positionClosed.NetProfit > 0) {
					commissionWinners			+= positionClosed.ExitFilled_commission + positionClosed.EntryFilled_commission;
					netProfitAtBarWinners		+= positionClosed.NetProfit;
					netProfitPctAtBarWinners	+= positionClosed.NetProfitPercent;
					barsHeldAtBarWinners		+= positionClosed.BarsHeld;
					positionsOpenAbsorbedWinners++;
					this.curConsecLosers = 0;
					this.curConsecWinners++;
					if (this.MaxConsecWinners < this.curConsecWinners) this.MaxConsecWinners = this.curConsecWinners;
				} else {
					commissionLosers			+= positionClosed.ExitFilled_commission + positionClosed.EntryFilled_commission;
					netProfitAtBarLosers		+= positionClosed.NetProfit;
					netProfitPctAtBarLosers		+= positionClosed.NetProfitPercent;
					barsHeldAtBarLosers			+= positionClosed.BarsHeld;
					positionsOpenAbsorbedLosers++;
					this.curConsecWinners = 0;
					this.curConsecLosers++;
					if (this.MaxConsecLosers < this.curConsecLosers) this.MaxConsecLosers = this.curConsecLosers;
				}
				this.CumulativeAppendOrReplaceForPositionClosedOrOpenNow(positionClosed, false);
			}

			if (barDateTime == DateTime.MinValue) {
				string msg2 = "NO_POSITIONS_AFTER_FILTERING_IS_MINE " + this.PositionLongShortImTracking;
				if (positionsOpenAbsorbedBoth != 0) {
					string msg = "THIS_MUST_BE_ZERO " + positionsOpenAbsorbedBoth;
					Assembler.PopupException(msg);
				}
				return positionsOpenAbsorbedBoth;
			}
			
			this.CashAvailable							+= cashBalanceAtBar;
			this.CashCurve	.SumupOrAppend(barDateTime, cashBalanceAtBar);

			this.CommissionBoth							+= commissionWinners + commissionLosers;
			//this.PositionsCountBoth						+= positionsOpenAbsorbedBoth + positionsClosedAbsorbedBoth;
			this.PositionsCount							+= positionsClosedAbsorbedBoth;
			this.BarsHeldTotalForClosedPositionsBoth	+= barsHeldAtBarBoth;
			this.NetProfitForClosedPositionsBoth		+= netProfitAtBarBoth;
			this.NetProfitPctForClosedPositionsBoth		+= netProfitPctAtBarBoth;


			this.CommissionWinners						+= commissionWinners;
			this.PositionsCountWinners					+= positionsOpenAbsorbedWinners;
			this.BarsHeldTotalForClosedPositionsWinners += barsHeldAtBarWinners;
			this.NetProfitWinners						+= netProfitAtBarWinners;
			this.NetProfitPctForClosedPositionsLong		+= netProfitPctAtBarWinners;

			this.CommissionLosers						+= commissionLosers;
			this.PositionsCountLosers					+= positionsOpenAbsorbedLosers;
			this.BarsHeldTotalForClosedPositionsLosers	+= barsHeldAtBarLosers;
			this.NetLossLosers							+= netProfitAtBarLosers;
			this.NetProfitPctForClosedPositionsLosers	+= netProfitPctAtBarLosers;

			if (this.NetProfitPeak < this.NetProfitForClosedPositionsBoth) {
				this.NetProfitPeak = this.NetProfitForClosedPositionsBoth;
				this.NetProfitPeakDate = barDateTime;
			}
			double drawDown = this.NetProfitForClosedPositionsBoth - this.NetProfitPeak;
			if (this.MaxDrawDown > drawDown) {
				this.MaxDrawDown = drawDown;
				this.MaxDrawDownLastLossDate = barDateTime;
			}

			if (this.PositionLongShortImTracking != SystemPerformancePositionsTracking.LongAndShort) return positionsOpenAbsorbedBoth;
			if (positionsClosedAbsorbedBoth == 0) return positionsOpenAbsorbedBoth;

			KPIs kpisForBar = new KPIs(this.ReasonToExist, 
				this.PositionsCount, this.NetProfitForClosedPositionsBoth, this.PositionAvgProfitBoth,
				this.WinLossRatio, this.ProfitFactor, this.RecoveryFactor,
				this.MaxDrawDown, this.MaxConsecWinners, this.MaxConsecLosers);

			if (this.positionsCountPrevStep_MustIncreaseOnly >= this.PositionsCount && this.PositionsCount > 0) {
				string msg = "POSITIONS_COUNT_CANT_DECREASE ["
					+ this.positionsCountPrevStep_MustIncreaseOnly + "]MUST_BE_LESS_THAN[" + kpisForBar.PositionsCount + "]"
					+ " //" + this.ReasonToExist;
				Assembler.PopupException(msg);
			}
			this.positionsCountPrevStep_MustIncreaseOnly = this.PositionsCount;

			if (this.KPIsCumulativeByDate.ContainsKey(barDateTime) == false) {
				this.KPIsCumulativeByDate.Add(barDateTime, kpisForBar);
			} else {
				string msg = "NONSENSE_BUT_ADDING__MUST_NOT_BE_DUPLICATES_KOZ_INVOKING_ONCE_PER_BAR barDateTime[" + barDateTime + "]";
				//Assembler.PopupException(msg, null, false);

				KPIs existing = this.KPIsCumulativeByDate[barDateTime];
				existing.AddKPIs(kpisForBar);
			}

			return positionsOpenAbsorbedBoth;
		}
		bool positionIsMineShouldAppendAndUpdate(Position position) {
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
		public double CumulativeNetProfitForPosition(Position position) {
			return this.CumulativeNetProfitDollar.ContainsKey(position) ? this.CumulativeNetProfitDollar[position] : -1; }
		public double CumulativeNetProfitPercentForPosition(Position position) {
			return this.CumulativeNetProfitPercent.ContainsKey(position) ? this.CumulativeNetProfitPercent[position] : -1; }

	}
}