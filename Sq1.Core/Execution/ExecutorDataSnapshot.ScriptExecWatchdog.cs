using System;
using System.Threading;

namespace Sq1.Core.Execution {
	public partial class ExecutorDataSnapshot : IDisposable {
			ManualResetEvent	ScriptRunning_OnBarStaticLast					;//	{ get; private set; }
			ManualResetEvent	ScriptRunning_OnNewQuote						;//	{ get; private set; }
			ManualResetEvent	ScriptRunning_OnAlertFilled						;//	{ get; private set; }
			ManualResetEvent	ScriptRunning_OnAlertKilled						;//	{ get; private set; }
			ManualResetEvent	ScriptRunning_OnAlertNotSubmitted				;//	{ get; private set; }
			ManualResetEvent	ScriptRunning_OnPositionOpened					;//	{ get; private set; }
			ManualResetEvent	ScriptRunning_onPositionOpened_entryPrototypedFilled;//	{ get; private set; }
			ManualResetEvent	ScriptRunning_onPositionClosed_exitPrototypedFilled;
			ManualResetEvent	ScriptRunning_OnPositionClosed					;//	{ get; private set; }
			ManualResetEvent	ScriptRunning_OnStreamingTriggeringScriptTurnedOn;//	{ get; private set; }
			ManualResetEvent	ScriptRunning_OnStreamingTriggeringScriptTurnedOff;//	{ get; private set; }
			ManualResetEvent	ScriptRunning_OnStrategyEmittingOrdersTurnedOn	;//	{ get; private set; }
			ManualResetEvent	ScriptRunning_OnStrategyEmittingOrdersTurnedOff	;//	{ get; private set; }

		public	bool	AnyScriptOverridenMethod_isRunningNow_nonBlocking { get {
			bool ret =	IsScriptRunning_OnBarStaticLast_nonBlocking;
			ret		|=	IsScriptRunning_OnNewQuote_nonBlocking;
			ret		|=	IsScriptRunning_OnAlertFilled_nonBlocking;
			ret		|=	IsScriptRunning_OnAlertKilled_nonBlocking;
			ret		|=	IsScriptRunning_OnAlertNotSubmitted_nonBlocking;
			ret		|=	IsScriptRunning_OnPositionOpened_nonBlocking;
			ret		|=	IsScriptRunning_OnPositionClosed_nonBlocking;
			ret		|=	IsScriptRunning_onPositionOpened_entryPrototypedFilled_nonBlocking;
			ret		|=	IsScriptRunning_onPositionClosed_exitPrototypedFilled_nonBlocking;
			ret		|=	IsScriptRunning_OnStreamingTriggeringScriptTurnedOn_nonBlocking;
			ret		|=	IsScriptRunning_OnStreamingTriggeringScriptTurnedOff_nonBlocking;
			ret		|=	IsScriptRunning_OnStrategyEmittingOrdersTurnedOn_nonBlocking;
			ret		|=	IsScriptRunning_OnStrategyEmittingOrdersTurnedOff_nonBlocking;
			return ret;
		} }

		public bool IsScriptRunning_OnBarStaticLast_nonBlocking						{
			get { return this.ScriptRunning_OnBarStaticLast.WaitOne(0); }
			set { if (value == true) this.ScriptRunning_OnBarStaticLast.Set();
								else this.ScriptRunning_OnBarStaticLast.Reset(); } }
		public bool IsScriptRunning_OnNewQuote_nonBlocking							{
			get { return this.ScriptRunning_OnNewQuote.WaitOne(0); }
			set { if (value == true) this.ScriptRunning_OnNewQuote.Set();
								else this.ScriptRunning_OnNewQuote.Reset(); } }
		public bool IsScriptRunning_OnAlertFilled_nonBlocking						{
			get { return this.ScriptRunning_OnAlertFilled.WaitOne(0); }
			set { if (value == true) this.ScriptRunning_OnAlertFilled.Set();
								else this.ScriptRunning_OnAlertFilled.Reset(); } }
		public bool IsScriptRunning_OnAlertKilled_nonBlocking						{
			get { return this.ScriptRunning_OnAlertKilled.WaitOne(0); }
			set { if (value == true) this.ScriptRunning_OnAlertKilled.Set();
								else this.ScriptRunning_OnAlertKilled.Reset(); } }
		public bool IsScriptRunning_OnAlertNotSubmitted_nonBlocking					{
			get { return this.ScriptRunning_OnAlertNotSubmitted.WaitOne(0); }
			set { if (value == true) this.ScriptRunning_OnAlertNotSubmitted.Set();
								else this.ScriptRunning_OnAlertNotSubmitted.Reset(); } }
		public bool IsScriptRunning_OnPositionOpened_nonBlocking						{
			get { return this.ScriptRunning_OnPositionOpened.WaitOne(0); }
			set { if (value == true) this.ScriptRunning_OnPositionOpened.Set();
								else this.ScriptRunning_OnPositionOpened.Reset(); } }

		public bool IsScriptRunning_onPositionOpened_entryPrototypedFilled_nonBlocking	{
			get { return this.ScriptRunning_onPositionOpened_entryPrototypedFilled.WaitOne(0); }
			set { if (value == true) this.ScriptRunning_onPositionOpened_entryPrototypedFilled.Set();
								else this.ScriptRunning_onPositionOpened_entryPrototypedFilled.Reset(); } }

		public bool IsScriptRunning_onPositionClosed_exitPrototypedFilled_nonBlocking	{
			get { return this.ScriptRunning_onPositionClosed_exitPrototypedFilled.WaitOne(0); }
			set { if (value == true) this.ScriptRunning_onPositionClosed_exitPrototypedFilled.Set();
								else this.ScriptRunning_onPositionClosed_exitPrototypedFilled.Reset(); } }

		public bool IsScriptRunning_OnPositionClosed_nonBlocking						{
			get { return this.ScriptRunning_OnPositionClosed.WaitOne(0); }
			set { if (value == true) this.ScriptRunning_OnPositionClosed.Set();
								else this.ScriptRunning_OnPositionClosed.Reset(); } }
		public bool IsScriptRunning_OnStreamingTriggeringScriptTurnedOn_nonBlocking	{
			get { return this.ScriptRunning_OnStreamingTriggeringScriptTurnedOn.WaitOne(0); }
			set { if (value == true) this.ScriptRunning_OnStreamingTriggeringScriptTurnedOn.Set();
								else this.ScriptRunning_OnStreamingTriggeringScriptTurnedOn.Reset(); } }
		public bool IsScriptRunning_OnStreamingTriggeringScriptTurnedOff_nonBlocking	{
			get { return this.ScriptRunning_OnStreamingTriggeringScriptTurnedOff.WaitOne(0); }
			set { if (value == true) this.ScriptRunning_OnStreamingTriggeringScriptTurnedOff.Set();
								else this.ScriptRunning_OnStreamingTriggeringScriptTurnedOff.Reset(); } }
		public bool IsScriptRunning_OnStrategyEmittingOrdersTurnedOn_nonBlocking		{
			get { return this.ScriptRunning_OnStrategyEmittingOrdersTurnedOn.WaitOne(0); }
			set { if (value == true) this.ScriptRunning_OnStrategyEmittingOrdersTurnedOn.Set();
								else this.ScriptRunning_OnStrategyEmittingOrdersTurnedOn.Reset(); } }
		public bool IsScriptRunning_OnStrategyEmittingOrdersTurnedOff_nonBlocking {
			get { return this.ScriptRunning_OnStrategyEmittingOrdersTurnedOff.WaitOne(0); }
			set { if (value == true) this.ScriptRunning_OnStrategyEmittingOrdersTurnedOff.Set();
								else this.ScriptRunning_OnStrategyEmittingOrdersTurnedOff.Reset(); } }

				string whatScriptOverridenMethodsAreRunningNow { get {
			string ret = "";
			if (this.IsScriptRunning_OnBarStaticLast_nonBlocking)							ret += "OnBarStaticLast,";
			if (this.IsScriptRunning_OnNewQuote_nonBlocking)								ret += "OnNewQuote,";
			if (this.IsScriptRunning_OnAlertFilled_nonBlocking)								ret += "OnAlertFilled,";
			if (this.IsScriptRunning_OnAlertNotSubmitted_nonBlocking)						ret += "OnAlertNotSubmitted,";
			if (this.IsScriptRunning_OnPositionOpened_nonBlocking)							ret += "OnPositionOpened,";
			if (this.IsScriptRunning_onPositionOpened_entryPrototypedFilled_nonBlocking)	ret += "OnPositionOpenedPrototypeSlTpPlaced,";
			if (this.IsScriptRunning_OnPositionClosed_nonBlocking)							ret += "PositionClosed,";
			if (this.IsScriptRunning_OnStreamingTriggeringScriptTurnedOn_nonBlocking)		ret += "OnStreamingTriggeringScriptTurnedOn,";
			if (this.IsScriptRunning_OnStreamingTriggeringScriptTurnedOff_nonBlocking)		ret += "OnStreamingTriggeringScriptTurnedOff,";
			if (this.IsScriptRunning_OnStrategyEmittingOrdersTurnedOn_nonBlocking)			ret += "OnStrategyEmittingOrdersTurnedOn,";
			if (this.IsScriptRunning_OnStrategyEmittingOrdersTurnedOff_nonBlocking)			ret += "OnStrategyEmittingOrdersTurnedOff,";
			ret = ret.TrimEnd(",".ToCharArray());
			return ret;
		} }

		void initializeScriptExecWatchdog() {
			ScriptRunning_OnBarStaticLast						= new ManualResetEvent(false);
			ScriptRunning_OnNewQuote							= new ManualResetEvent(false);
			ScriptRunning_OnAlertFilled							= new ManualResetEvent(false);
			ScriptRunning_OnAlertKilled							= new ManualResetEvent(false);
			ScriptRunning_OnAlertNotSubmitted					= new ManualResetEvent(false);
			ScriptRunning_OnPositionOpened						= new ManualResetEvent(false);
			ScriptRunning_onPositionOpened_entryPrototypedFilled	= new ManualResetEvent(false);
			ScriptRunning_onPositionClosed_exitPrototypedFilled		= new ManualResetEvent(false);
			ScriptRunning_OnPositionClosed						= new ManualResetEvent(false);
			ScriptRunning_OnStreamingTriggeringScriptTurnedOn	= new ManualResetEvent(false);
			ScriptRunning_OnStreamingTriggeringScriptTurnedOff	= new ManualResetEvent(false);
			ScriptRunning_OnStrategyEmittingOrdersTurnedOn		= new ManualResetEvent(false);
			ScriptRunning_OnStrategyEmittingOrdersTurnedOff		= new ManualResetEvent(false);
		}
		public void BarkIfAnyScriptOverrideIsRunning(string msig) {
			//WHATS_THE_POINT_THEN return;	//EXPLAIN_BETTER  POTENTIAL_RACE_CONDITIONs are all followed by lock(){} upstack, right?

			if (this.AnyScriptOverridenMethod_isRunningNow_nonBlocking == false) return;
			if (this.executor.BacktesterOrLivesimulator.ImRunningChartless_backtestOrSequencing) {
				string msg1 = "SKIPPING_CHECKS_FOR_BACKTESTER_SINCE_ITS_SINGLE_THREADED";
				return;
			}
			string msg = this.whatScriptOverridenMethodsAreRunningNow;
			if (string.IsNullOrEmpty(msg)) {
				string funny = "NO_DANGER__SCRIPT_OVERRIDE_ALREADY_TERMINATED__WHILE_YOU_WERE_CONCATENATING_YOUR_SLOW_STRINGS__JAJA ";
				Assembler.PopupException(funny + msig, null, false);
				return;
			}
			// I want to provide script overridden methods collections-not-modified guarantee;
			// collision might happen when 2 event happen simultaneously:
			// 1) Streaming delivered a new quote => Script.OnNewQuote invokees Buy/Sell which modifies AlertsPending
			// 2) Broker delivered an OrderFill => Core adds a PositionsOpenedNow and invokes Script.OnAlertFilled

			// current implementation is rather verbose and "preventive" for easy lifecycle debugging, than sober and consise:
			// 1) Core, before modifying AlertsPending / PositionsOpenNow (as a result of BrokerAdapter.AlertFilledNotifyCore)
			// 2) checks if any Script's *Callback is executed now (during which I want to avoid CollectionModified Exception)
			// 3) if there Script's override was indeed running - adds a detailed message into ExceptionForm
			// 4) and locks on ConcurrentList.LockObject for the ConcurrentList it's trying to modify

			// deadlocks still have chance to evolve:
			// 1) Broker=>OrderFilledCallback locks on AlertsPending, Streaming=>Strategy.OnNewQuote=>Buy has queued on AlertPending, while Performance in GUI reads AlertsPending.Count and throws
			// 2) all three require Core's smart decision simultaneously: script.OnPrototypeSLfilled, streaming.OnNewQuote, broker.OnAnotherAlertFilled
			// 3) strategy accessing two symbols is doomed (multi-strike options strategies tam biem)
			Assembler.PopupException("POTENTIAL_RACE_CONDITION " + msg + msig, null, false);
		}

		public void Dispose() {
			if (this.IsDisposed) {
				string msg = "ALREADY_DISPOSED__DONT_INVOKE_ME_TWICE  " + this.ToString();
				Assembler.PopupException(msg);
				return;
			}
			this.ScriptRunning_OnBarStaticLast.Dispose();
			this.ScriptRunning_OnNewQuote							.Dispose();
			this.ScriptRunning_OnAlertFilled						.Dispose();
			this.ScriptRunning_OnAlertKilled						.Dispose();
			this.ScriptRunning_OnAlertNotSubmitted					.Dispose();
			this.ScriptRunning_OnPositionOpened						.Dispose();
			this.ScriptRunning_onPositionOpened_entryPrototypedFilled	.Dispose();
			this.ScriptRunning_OnPositionClosed						.Dispose();
			this.ScriptRunning_OnStreamingTriggeringScriptTurnedOn	.Dispose();
			this.ScriptRunning_OnStreamingTriggeringScriptTurnedOff	.Dispose();
			this.ScriptRunning_OnStrategyEmittingOrdersTurnedOn		.Dispose();
			this.ScriptRunning_OnStrategyEmittingOrdersTurnedOff	.Dispose();

			this.AlertsUnfilled										.Dispose();
			//NOT_USED this.AlertsMaster										.Dispose();
			this.AlertsNewAfterExec									.Dispose();
			this.Positions_AllBacktested							.Dispose();
			this.Positions_OpenNow									.Dispose();
			this.Positions_toBeOpenedAfterExec						.Dispose();
			this.Positions_toBeClosedAfterExec						.Dispose();
			this.IsDisposed = true;
		}
		public bool IsDisposed { get; private set; }
	}
}
