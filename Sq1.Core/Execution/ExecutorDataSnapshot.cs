using System;
using System.Collections.Generic;

using Sq1.Core.StrategyBase;
using Sq1.Core.Support;
using Sq1.Core.DataTypes;
using Sq1.Core.Backtesting;

namespace Sq1.Core.Execution {
	public partial class ExecutorDataSnapshot {
				ScriptExecutor					executor;
				object 							allOperations_withAlerts;
				object allOperations_withPositions;
		
		//NOT_USED public	AlertList						AlertsMaster					{ get; private set; }
		public	AlertList						AlertsNewAfterExec				{ get; private set; }
		public	AlertList						AlertsUnfilled					{ get; private set; }
		public	AlertList						AlertsDoomed					{ get; private set; }		 //will be Killed after Script.OnNewQuote()/OnNewBar() return
		//public Dictionary<int, List<Alert>>	AlertsPendingHistorySafeCopy { get { return this.AlertsPendingHistorySafeCopyForRenderer(0, -1); } }

		public	int								positionSerno_perStrategy		{ get; private set; }
		public	PositionList					Positions_AllBacktested			{ get; private set; }
		public	PositionList					Positions_toBeOpenedAfterExec	{ get; private set; }
		public	PositionList					Positions_toBeClosedAfterExec	{ get; private set; }
		public	PositionList					Positions_OpenNow				{ get; private set; }

		//public	List<Position>					Positions_OpenNow				{ get {
		//    List<Position> positions_withEntryAlert_filled = new List<Position>();
		//    foreach (Position position_Pending_orOpenNow in this.Positions_Pending_orOpenNow.SafeCopy(this, "Positions_OpenNow")) {
		//        if (position_Pending_orOpenNow.EntryAlert					== null)	continue;
		//        if (position_Pending_orOpenNow.EntryAlert.FilledBarIndex	== -1)		continue;	// not filled => skipping
		//        positions_withEntryAlert_filled.Add(position_Pending_orOpenNow);
		//    }
		//    return positions_withEntryAlert_filled;
		//} }

		//public	List<Position>					Positions_PendingNow				{ get {
		//    List<Position> positions_withEntryAlert_filled = new List<Position>();
		//    foreach (Position position_Pending_orOpenNow in this.Positions_Pending_orOpenNow.SafeCopy(this, "Positions_Pending")) {
		//        if (position_Pending_orOpenNow.EntryAlert					== null)	continue;
		//        if (position_Pending_orOpenNow.EntryAlert.FilledBarIndex	!= -1)		continue;	// filled => skipping
		//        positions_withEntryAlert_filled.Add(position_Pending_orOpenNow);
		//    }
		//    return positions_withEntryAlert_filled;
		//} }


		public ExecutorDataSnapshot(ScriptExecutor strategyExecutor) {
			this.executor						= strategyExecutor;
			allOperations_withAlerts					= new object();
			allOperations_withPositions					= new object();
			AlertsUnfilled						= new AlertList("AlertsPending"					, this);	// monitored
			AlertsDoomed						= new AlertList("AlertsDoomed"					, this);	// monitored
			//NOT_USED AlertsMaster						= new AlertList("AlertsMaster"					, this);	// monitored
			AlertsNewAfterExec					= new AlertList("AlertsNewAfterExec"			, this);	// monitored
			positionSerno_perStrategy			= 0;
			Positions_AllBacktested				= new PositionList("Positions_AllBacktested"		, this);	// monitored
			Positions_OpenNow					= new PositionList("Positions_OpenNow"				, this);	// monitored
			Positions_toBeOpenedAfterExec		= new PositionList("Positions_toBeOpenedAfterExec"	, this);	// monitored
			Positions_toBeClosedAfterExec		= new PositionList("Positions_toBeClosedAfterExec"	, this);	// monitored
			this.initializeScriptExecWatchdog();
		}

		public void Initialize() { lock (this.allOperations_withPositions) {
			string msig = " //Initialize(WAIT)";
			//NOT_USED this.AlertsMaster					.DisposeWaitHandles_andClearInnerList(this, msig);
			this.AlertsNewAfterExec				.DisposeWaitHandles_andClearInnerList(this, msig);
			this.AlertsUnfilled					.DisposeWaitHandles_andClearInnerList(this, msig);
			this.AlertsDoomed					.DisposeWaitHandles_andClearInnerList(this, msig);
			this.positionSerno_perStrategy	= 0;
			this.Positions_AllBacktested		.DisposeTwoRelatedAlerts_waitHandles_andClearInnerList(this, msig);
			this.Positions_toBeOpenedAfterExec	.DisposeTwoRelatedAlerts_waitHandles_andClearInnerList(this, msig);
			this.Positions_toBeClosedAfterExec	.DisposeTwoRelatedAlerts_waitHandles_andClearInnerList(this, msig);
			this.Positions_OpenNow				.DisposeTwoRelatedAlerts_waitHandles_andClearInnerList(this, msig);
		} }
		internal void Clear_priorTo_InvokeScript_onNewBar_onNewQuote() { lock (this.allOperations_withPositions) {
			string msig = " //Clear_priorTo_InvokeScript_onNewBar_onNewQuote(WAIT)";
			this.AlertsNewAfterExec				.Clear(this, msig);
			this.Positions_toBeOpenedAfterExec	.Clear(this, msig);
			this.Positions_toBeClosedAfterExec	.Clear(this, msig);
		} }
		internal int Positions_addNew_incrementPositionSerno(Position positionOpening_alertJustFilled) { lock (this.allOperations_withPositions) {
			int ret = 0;
			string msig = " //Positions_toBeOpenedAfterExec_addNew_incrementPositionsSerno(WAIT)";
			if (positionOpening_alertJustFilled.EntryFilledBarIndex == -1) {
				string msg = "ENTRY_BAR_NEGATIVE_CAN_NOT_STORE_POSITION_IN_PositionsMaster.ByEntryBarFilled"
					+ " Strategy[" + this.executor.Strategy.ToString() + "] EntryBar=-1 for position[" + positionOpening_alertJustFilled + "]";
				Assembler.PopupException(msg, null, false);
				return ret;
			}
			
			positionOpening_alertJustFilled.SernoPerStrategy = ++this.positionSerno_perStrategy;

			bool added1 = this.Positions_toBeOpenedAfterExec	.AddOpened_step1of2(positionOpening_alertJustFilled, this, msig);
			bool added2 = this.Positions_OpenNow				.AddOpened_step1of2(positionOpening_alertJustFilled, this, msig);
			if (added1) ret++;
			if (added2) ret++;
			return ret;
		} }
		public void AlertEnriched_register(Alert alert, bool registerIn_AlertsNewAfterExec = false) { lock (this.allOperations_withAlerts) {
			string msig = " //AlertEnriched_register(WAIT)";
			if (alert.Qty == 0.0) {
				string msg = "alert[" + alert + "].Qty==0; hopefully will be displayed but not executed...";
				throw new Exception(msg);
			}
			if (alert.Strategy.Script == null) {
				string msg = "TODO NYI alert submitted from mni / onChartTrading";
			}
			
			#if DEBUG
			// NEVER_HAPPENS_AND_TOO_EXPENSIVE Alert.IsIdentical_orderlessPriceless()
			//if (this.AlertsMaster.ContainsIdentical(alert, this, msig)) {
			//	string msg = "AlertsMasterContainsIdentical=>won't add NewPending;"
			//		+ " 1) broker's order status dupe? 2) are you using CoverAtStop() in your strategy?"
			//		+ " //" + alert;
			//	Assembler.PopupException(msg);
			//	return;
			//}
			#endif
			
			//NOT_USED this.AlertsMaster.AddNoDupe_byBarsPlaced(alert, this, msig);
			if (registerIn_AlertsNewAfterExec == true) this.AlertsNewAfterExec.AddNoDupe_byBarsPlaced(alert, this, msig);
			ByBarDumpStatus dumped = this.AlertsUnfilled.AddNoDupe_byBarsPlaced(alert, this, msig);
			switch (dumped) {
				case ByBarDumpStatus.BarAlreadyContained_alertYouAdd:
					string msg1 = "DUPE while adding JUST CREATED??? alert[" + alert + "]";
					throw new Exception(msg1);
					break;
				case ByBarDumpStatus.SequentialAlertAdded_forExistingBar:
					string msg2 = "Here is the case when PrototypeActivator changed alert[" + alert + "]";
					break;
			}
		} }
		public void MovePositionOpen_toClosed_backtestEnded(Position positionClosing, bool absenceThrows = true) { lock (this.allOperations_withPositions) {
			string msig = " //MovePositionOpen_toClosed(WAIT)";
			this.Positions_toBeClosedAfterExec.AddClosed(positionClosing, this, msig);
			this.Positions_OpenNow.Remove(positionClosing, this, msig, ConcurrentWatchdog.TIMEOUT_DEFAULT, absenceThrows);
		} }
		public AlertList AlertsUnfilled_thatQuoteWillFill(Quote quote) {		//QuoteGenerated quote
			string msig = " //AlertsUnfilled_thatQuoteWillFill(WAIT)";
			bool imRunningBacktest = this.executor.DataSource_fromBars.BrokerAsBacktest_nullUnsafe != null;
			BacktestMarketsim marketsim = imRunningBacktest
				? this.executor.DataSource_fromBars.BrokerAsBacktest_nullUnsafe.BacktestMarketsim
				: this.executor.DataSource_fromBars.BrokerAsLivesim_nullUnsafe.LivesimMarketsim;

			AlertList ret = new AlertList("THERE_WERE_NO_ALERTS_PENDING_TO_FILL_ON_EACH_QUOTE", null);
			if (this.AlertsUnfilled.Count == 0) return ret;

			ret = new AlertList("ALERTS_PENDING_MINUS_SCHEDULED_FOR_DELAYED_FILL", null);
			List<Alert> pendingSafe = this.AlertsUnfilled.SafeCopy(this, msig);
			foreach (Alert eachPending in pendingSafe) {
				double priceFill = -1;
				double slippageFill = -1;
				bool filled = marketsim.Check_alertWillBeFilled_byQuote(eachPending, quote, out priceFill, out slippageFill);
				if (filled == false) continue;

				ret.AddNoDupe_byBarsPlaced(eachPending, this, msig);
			}
			return ret;
		}
	}
}
