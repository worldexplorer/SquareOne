using System;
using System.Threading;

namespace Sq1.Core.Execution {
	public partial class ExecutionDataSnapshot {
			ManualResetEvent	ScriptRunningOnBarStaticLast					;//	{ get; private set; }
			ManualResetEvent	ScriptRunningOnNewQuote							;//	{ get; private set; }
			ManualResetEvent	ScriptRunningOnAlertFilled						;//	{ get; private set; }
			ManualResetEvent	ScriptRunningOnAlertKilled						;//	{ get; private set; }
			ManualResetEvent	ScriptRunningOnAlertNotSubmitted				;//	{ get; private set; }
			ManualResetEvent	ScriptRunningOnPositionOpened					;//	{ get; private set; }
			ManualResetEvent	ScriptRunningOnPositionOpenedPrototypeSlTpPlaced;//	{ get; private set; }
			ManualResetEvent	ScriptRunningOnPositionClosed					;//	{ get; private set; }
			ManualResetEvent	ScriptRunningOnStreamingTriggeringScriptTurnedOn;//	{ get; private set; }
			ManualResetEvent	ScriptRunningOnStreamingTriggeringScriptTurnedOff;//	{ get; private set; }
			ManualResetEvent	ScriptRunningOnStrategyEmittingOrdersTurnedOn	;//	{ get; private set; }
			ManualResetEvent	ScriptRunningOnStrategyEmittingOrdersTurnedOff	;//	{ get; private set; }

		public	bool	AnyScriptOverridenMethodIsRunningNowBlockingRead { get {
			bool ret =	IsScriptRunningOnBarStaticLastNonBlockingRead;
			ret		|=	IsScriptRunningOnNewQuoteNonBlockingRead;
			ret		|=	IsScriptRunningOnAlertFilledNonBlockingRead;
			ret		|=	IsScriptRunningOnAlertKilledNonBlockingRead;
			ret		|=	IsScriptRunningOnAlertNotSubmittedNonBlockingRead;
			ret		|=	IsScriptRunningOnPositionOpenedNonBlockingRead;
			ret		|=	IsScriptRunningOnPositionOpenedPrototypeSlTpPlacedNonBlockingRead;
			ret		|=	IsScriptRunningOnPositionClosedNonBlockingRead;
			ret		|=	IsScriptRunningOnStreamingTriggeringScriptTurnedOnNonBlockingRead;
			ret		|=	IsScriptRunningOnStreamingTriggeringScriptTurnedOffNonBlockingRead;
			ret		|=	IsScriptRunningOnStrategyEmittingOrdersTurnedOnNonBlockingRead;
			ret		|=	IsScriptRunningOnStrategyEmittingOrdersTurnedOffNonBlockingRead;
			return ret;
		} }

		public bool IsScriptRunningOnBarStaticLastNonBlockingRead						{
			get { return this.ScriptRunningOnBarStaticLast.WaitOne(0); }
			set { if (value == true) this.ScriptRunningOnBarStaticLast.Set();
								else this.ScriptRunningOnBarStaticLast.Reset(); } }
		public bool IsScriptRunningOnNewQuoteNonBlockingRead							{
			get { return this.ScriptRunningOnNewQuote.WaitOne(0); }
			set { if (value == true) this.ScriptRunningOnNewQuote.Set();
								else this.ScriptRunningOnNewQuote.Reset(); } }
		public bool IsScriptRunningOnAlertFilledNonBlockingRead						{
			get { return this.ScriptRunningOnAlertFilled.WaitOne(0); }
			set { if (value == true) this.ScriptRunningOnAlertFilled.Set();
								else this.ScriptRunningOnAlertFilled.Reset(); } }
		public bool IsScriptRunningOnAlertKilledNonBlockingRead						{
			get { return this.ScriptRunningOnAlertKilled.WaitOne(0); }
			set { if (value == true) this.ScriptRunningOnAlertKilled.Set();
								else this.ScriptRunningOnAlertKilled.Reset(); } }
		public bool IsScriptRunningOnAlertNotSubmittedNonBlockingRead					{
			get { return this.ScriptRunningOnAlertNotSubmitted.WaitOne(0); }
			set { if (value == true) this.ScriptRunningOnAlertNotSubmitted.Set();
								else this.ScriptRunningOnAlertNotSubmitted.Reset(); } }
		public bool IsScriptRunningOnPositionOpenedNonBlockingRead						{
			get { return this.ScriptRunningOnPositionOpened.WaitOne(0); }
			set { if (value == true) this.ScriptRunningOnPositionOpened.Set();
								else this.ScriptRunningOnPositionOpened.Reset(); } }
		public bool IsScriptRunningOnPositionOpenedPrototypeSlTpPlacedNonBlockingRead	{
			get { return this.ScriptRunningOnPositionOpenedPrototypeSlTpPlaced.WaitOne(0); }
			set { if (value == true) this.ScriptRunningOnPositionOpenedPrototypeSlTpPlaced.Set();
								else this.ScriptRunningOnPositionOpenedPrototypeSlTpPlaced.Reset(); } }
		public bool IsScriptRunningOnPositionClosedNonBlockingRead						{
			get { return this.ScriptRunningOnPositionClosed.WaitOne(0); }
			set { if (value == true) this.ScriptRunningOnPositionClosed.Set();
								else this.ScriptRunningOnPositionClosed.Reset(); } }
		public bool IsScriptRunningOnStreamingTriggeringScriptTurnedOnNonBlockingRead	{
			get { return this.ScriptRunningOnStreamingTriggeringScriptTurnedOn.WaitOne(0); }
			set { if (value == true) this.ScriptRunningOnStreamingTriggeringScriptTurnedOn.Set();
								else this.ScriptRunningOnStreamingTriggeringScriptTurnedOn.Reset(); } }
		public bool IsScriptRunningOnStreamingTriggeringScriptTurnedOffNonBlockingRead	{
			get { return this.ScriptRunningOnStreamingTriggeringScriptTurnedOff.WaitOne(0); }
			set { if (value == true) this.ScriptRunningOnStreamingTriggeringScriptTurnedOff.Set();
								else this.ScriptRunningOnStreamingTriggeringScriptTurnedOff.Reset(); } }
		public bool IsScriptRunningOnStrategyEmittingOrdersTurnedOnNonBlockingRead		{
			get { return this.ScriptRunningOnStrategyEmittingOrdersTurnedOn.WaitOne(0); }
			set { if (value == true) this.ScriptRunningOnStrategyEmittingOrdersTurnedOn.Set();
								else this.ScriptRunningOnStrategyEmittingOrdersTurnedOn.Reset(); } }
		public bool IsScriptRunningOnStrategyEmittingOrdersTurnedOffNonBlockingRead {
			get { return this.ScriptRunningOnStrategyEmittingOrdersTurnedOff.WaitOne(0); }
			set { if (value == true) this.ScriptRunningOnStrategyEmittingOrdersTurnedOff.Set();
								else this.ScriptRunningOnStrategyEmittingOrdersTurnedOff.Reset(); } }

				string whatScriptOverridenMethodsAreRunningNow { get {
			string ret = "";
			if (this.IsScriptRunningOnBarStaticLastNonBlockingRead)							ret += "OnBarStaticLast,";
			if (this.IsScriptRunningOnNewQuoteNonBlockingRead)								ret += "OnNewQuote,";
			if (this.IsScriptRunningOnAlertFilledNonBlockingRead)							ret += "OnAlertFilled,";
			if (this.IsScriptRunningOnAlertNotSubmittedNonBlockingRead)						ret += "OnAlertNotSubmitted,";
			if (this.IsScriptRunningOnPositionOpenedNonBlockingRead)						ret += "OnPositionOpened,";
			if (this.IsScriptRunningOnPositionOpenedPrototypeSlTpPlacedNonBlockingRead)		ret += "OnPositionOpenedPrototypeSlTpPlaced,";
			if (this.IsScriptRunningOnPositionClosedNonBlockingRead)						ret += "PositionClosed,";
			if (this.IsScriptRunningOnStreamingTriggeringScriptTurnedOnNonBlockingRead)		ret += "OnStreamingTriggeringScriptTurnedOn,";
			if (this.IsScriptRunningOnStreamingTriggeringScriptTurnedOffNonBlockingRead)	ret += "OnStreamingTriggeringScriptTurnedOff,";
			if (this.IsScriptRunningOnStrategyEmittingOrdersTurnedOnNonBlockingRead)		ret += "OnStrategyEmittingOrdersTurnedOn,";
			if (this.IsScriptRunningOnStrategyEmittingOrdersTurnedOffNonBlockingRead)		ret += "OnStrategyEmittingOrdersTurnedOff,";
			ret = ret.TrimEnd(",".ToCharArray());
			return ret;
		} }

		void initializeScriptExecWatchdog() {
			ScriptRunningOnBarStaticLast						= new ManualResetEvent(false);
			ScriptRunningOnNewQuote								= new ManualResetEvent(false);
			ScriptRunningOnAlertFilled							= new ManualResetEvent(false);
			ScriptRunningOnAlertKilled							= new ManualResetEvent(false);
			ScriptRunningOnAlertNotSubmitted					= new ManualResetEvent(false);
			ScriptRunningOnPositionOpened						= new ManualResetEvent(false);
			ScriptRunningOnPositionOpenedPrototypeSlTpPlaced	= new ManualResetEvent(false);
			ScriptRunningOnPositionClosed						= new ManualResetEvent(false);
			ScriptRunningOnStreamingTriggeringScriptTurnedOn	= new ManualResetEvent(false);
			ScriptRunningOnStreamingTriggeringScriptTurnedOff	= new ManualResetEvent(false);
			ScriptRunningOnStrategyEmittingOrdersTurnedOn		= new ManualResetEvent(false);
			ScriptRunningOnStrategyEmittingOrdersTurnedOff		= new ManualResetEvent(false);
		}
		public void BarkIfAnyScriptOverrideIsRunning(string msig) {
			return;	//just disabled it here; POTENTIAL_RACE_CONDITIONs are all followed by lock(){} upstack, right?


			if (this.AnyScriptOverridenMethodIsRunningNowBlockingRead == false) return;
			if (this.executor.Backtester.IsBacktestingNoLivesimNow) {
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
	}
}
