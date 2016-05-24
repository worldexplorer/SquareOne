using System;
using System.Collections.Generic;

using Sq1.Core.StrategyBase;
using Sq1.Core.Support;
using Sq1.Core.DataTypes;
using Sq1.Core.Backtesting;

namespace Sq1.Core.Execution {
	public partial class ExecutorDataSnapshot {
				ScriptExecutor					executor;
				object 							addingSynchronouslyToAlertsMasterPendingNew;
				object positionsMasterLock;
		
		public	AlertList						AlertsMaster					{ get; private set; }
		public	AlertList						AlertsNewAfterExec				{ get; private set; }
		public	AlertList						AlertsPending_havingOrderFollowed_notYetFilled				{ get; private set; }
		public	AlertList						AlertsDoomed					{ get; private set; }
		//public Dictionary<int, List<Alert>>	AlertsPendingHistorySafeCopy { get { return this.AlertsPendingHistorySafeCopyForRenderer(0, -1); } }

		public	int								positionSerno_perStrategy		{ get; private set; }
		public	PositionList					PositionsMaster					{ get; private set; }
		public	PositionList					Positions_toBeOpenedAfterExec	{ get; private set; }
		public	PositionList					Positions_toBeClosedAfterExec	{ get; private set; }
		public	PositionList					Positions_Pending_orOpenNow		{ get; private set; }

		public	List<Position>					Positions_OpenNow				{ get {
			List<Position> positions_withEntryAlert_filled = new List<Position>();
			foreach (Position position_Pending_orOpenNow in this.Positions_Pending_orOpenNow.SafeCopy(this, "Positions_OpenNow")) {
				if (position_Pending_orOpenNow.EntryAlert					== null)	continue;
				if (position_Pending_orOpenNow.EntryAlert.FilledBarIndex	== -1)		continue;	// not filled => skipping
				positions_withEntryAlert_filled.Add(position_Pending_orOpenNow);
			}
			return positions_withEntryAlert_filled;
		} }

		public	List<Position>					Positions_PendingNow				{ get {
			List<Position> positions_withEntryAlert_filled = new List<Position>();
			foreach (Position position_Pending_orOpenNow in this.Positions_Pending_orOpenNow.SafeCopy(this, "Positions_Pending")) {
				if (position_Pending_orOpenNow.EntryAlert					== null)	continue;
				if (position_Pending_orOpenNow.EntryAlert.FilledBarIndex	!= -1)		continue;	// filled => skipping
				positions_withEntryAlert_filled.Add(position_Pending_orOpenNow);
			}
			return positions_withEntryAlert_filled;
		} }


		public ExecutorDataSnapshot(ScriptExecutor strategyExecutor) {
			this.executor						= strategyExecutor;
			addingSynchronouslyToAlertsMasterPendingNew					= new object();
			positionsMasterLock					= new object();
			AlertsPending_havingOrderFollowed_notYetFilled						= new AlertList("AlertsPending"					, this);	// monitored
			AlertsDoomed						= new AlertList("AlertsDoomed"					, this);	// monitored
			AlertsMaster						= new AlertList("AlertsMaster"					, this);	// monitored
			AlertsNewAfterExec					= new AlertList("AlertsNewAfterExec"			, this);	// monitored
			positionSerno_perStrategy			= 0;
			PositionsMaster						= new PositionList("PositionsMaster"			, this);	// monitored
			Positions_Pending_orOpenNow			= new PositionList("Positions_Pending_orOpenNow"			, this);	// monitored
			Positions_toBeOpenedAfterExec		= new PositionList("Positions_toBeOpenedAfterExec"	, this);	// monitored
			Positions_toBeClosedAfterExec		= new PositionList("Positions_toBeClosedAfterExec"	, this);	// monitored
			this.initializeScriptExecWatchdog();
		}

		public void Initialize() { lock (this.positionsMasterLock) {
			string msig = " //Initialize(WAIT)";
			this.AlertsMaster				.DisposeWaitHandlesAndClear(this, msig);
			this.AlertsNewAfterExec			.DisposeWaitHandlesAndClear(this, msig);
			this.AlertsPending_havingOrderFollowed_notYetFilled				.DisposeWaitHandlesAndClear(this, msig);
			this.AlertsDoomed				.DisposeWaitHandlesAndClear(this, msig);
			this.positionSerno_perStrategy	= 0;
			this.PositionsMaster			.DisposeTwoRelatedAlertsWaitHandlesAndClear(this, msig);
			this.Positions_toBeOpenedAfterExec	.DisposeTwoRelatedAlertsWaitHandlesAndClear(this, msig);
			this.Positions_toBeClosedAfterExec	.DisposeTwoRelatedAlertsWaitHandlesAndClear(this, msig);
			this.Positions_Pending_orOpenNow			.DisposeTwoRelatedAlertsWaitHandlesAndClear(this, msig);
		} }
		internal void Clear_priorTo_InvokeScript_onNewBar_onNewQuote() { lock (this.positionsMasterLock) {
			string msig = " //Clear_priorTo_InvokeScript_onNewBar_onNewQuote(WAIT)";
			this.AlertsNewAfterExec			.Clear(this, msig);
			this.Positions_toBeOpenedAfterExec	.Clear(this, msig);
			this.Positions_toBeClosedAfterExec	.Clear(this, msig);
		} }
		internal void PositionsMasterOpen_addNew(Position positionOpening) { lock (this.positionsMasterLock) {
			string msig = " //PositionsMasterOpen_addNew(WAIT)";
			if (positionOpening.EntryFilledBarIndex == -1) {
				string msg = "ENTRY_BAR_NEGATIVE_CAN_NOT_STORE_POSITION_IN_PositionsMaster.ByEntryBarFilled"
					+ " Strategy[" + this.executor.Strategy.ToString() + "] EntryBar=-1 for position[" + positionOpening + "]";
				Assembler.PopupException(msg);
				return;
			}
			
			positionOpening.SernoPerStrategy = ++this.positionSerno_perStrategy;
			this.PositionsMaster				.AddOpened_step1of2(positionOpening, this, msig);
			this.Positions_toBeOpenedAfterExec	.AddOpened_step1of2(positionOpening, this, msig);
			if (this.Positions_Pending_orOpenNow.Contains(positionOpening, this, msig) == false) {
				this.Positions_Pending_orOpenNow.AddPending(positionOpening, this, msig);
			}
		} }
		public void AlertEnriched_register(Alert alert, bool registerInNewAfterExec = false) { lock (this.addingSynchronouslyToAlertsMasterPendingNew) {
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
			
			this.AlertsMaster.AddNoDupe(alert, this, "AlertEnrichedRegister(WAIT)");
			if (registerInNewAfterExec == true) this.AlertsNewAfterExec.AddNoDupe(alert, this, msig);
			ByBarDumpStatus dumped = this.AlertsPending_havingOrderFollowed_notYetFilled.AddNoDupe(alert, this, msig);
			switch (dumped) {
				case ByBarDumpStatus.BarAlreadyContainedTheAlertToAdd:
					string msg1 = "DUPE while adding JUST CREATED??? alert[" + alert + "]";
					throw new Exception(msg1);
					break;
				case ByBarDumpStatus.SequentialAlertAddedForExistingBarInHistory:
					string msg2 = "Here is the case when PrototypeActivator changed alert[" + alert + "]";
					break;
			}
		} }
		public void MovePositionOpen_toClosed(Position positionClosing, bool absenseInPositionsOpenNowIsAnError = true) { lock (this.positionsMasterLock) {
			string msig = " //MovePositionOpen_toClosed(WAIT)";
			bool added = this.PositionsMaster.AddToClosedDictionary_step2of2(positionClosing, this, msig, ConcurrentWatchdog.TIMEOUT_DEFAULT, absenseInPositionsOpenNowIsAnError);
			this.Positions_toBeClosedAfterExec.AddClosed(positionClosing, this, msig);
			this.Positions_Pending_orOpenNow.Remove(positionClosing, this, msig);
		} }
		public AlertList AlertsPending_thatQuoteWillFill(Quote quote) {		//QuoteGenerated quote
			string msig = " //AlertsPending_thatQuoteWillFill(WAIT)";
			bool imRunningBacktest = this.executor.DataSource_fromBars.BrokerAsBacktest_nullUnsafe != null;
			BacktestMarketsim marketsim = imRunningBacktest
				? this.executor.DataSource_fromBars.BrokerAsBacktest_nullUnsafe.BacktestMarketsim
				: this.executor.DataSource_fromBars.BrokerAsLivesim_nullUnsafe.LivesimMarketsim;

			AlertList ret = new AlertList("THERE_WERE_NO_ALERTS_PENDING_TO_FILL_ON_EACH_QUOTE", null);
			if (this.AlertsPending_havingOrderFollowed_notYetFilled.Count == 0) return ret;

			ret = new AlertList("ALERTS_PENDING_MINUS_SCHEDULED_FOR_DELAYED_FILL", null);
			List<Alert> pendingSafe = this.AlertsPending_havingOrderFollowed_notYetFilled.SafeCopy(this, msig);
			foreach (Alert eachPending in pendingSafe) {
				double priceFill = -1;
				double slippageFill = -1;
				bool filled = marketsim.Check_alertWillBeFilled_byQuote(eachPending, quote, out priceFill, out slippageFill);
				if (filled == false) continue;

				ret.AddNoDupe(eachPending, this, msig);
			}
			return ret;
		}
	}
}
