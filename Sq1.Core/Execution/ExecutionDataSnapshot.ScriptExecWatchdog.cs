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

		public	bool	AnyScriptOverridenMethodIsRunningNow { get {
			bool ret =	IsScriptRunningOnBarStaticLast;
			ret		|=	IsScriptRunningOnNewQuote;
			ret		|=	IsScriptRunningOnAlertFilled;
			ret		|=	IsScriptRunningOnAlertFilled;
			ret		|=	IsScriptRunningOnAlertNotSubmitted;
			ret		|=	IsScriptRunningOnPositionOpened;
			ret		|=	IsScriptRunningOnPositionOpenedPrototypeSlTpPlaced;
			ret		|=	IsScriptRunningOnPositionClosed;
			ret		|=	IsScriptRunningOnStreamingTriggeringScriptTurnedOn;
			ret		|=	IsScriptRunningOnStreamingTriggeringScriptTurnedOff;
			ret		|=	IsScriptRunningOnStrategyEmittingOrdersTurnedOn;
			ret		|=	IsScriptRunningOnStrategyEmittingOrdersTurnedOff;
			return ret;
		} }

		public bool IsScriptRunningOnBarStaticLast						{
			get { return this.ScriptRunningOnBarStaticLast.WaitOne(0); }
			set { if (value == true) this.ScriptRunningOnBarStaticLast.Set();
								else this.ScriptRunningOnBarStaticLast.Reset(); } }
		public bool IsScriptRunningOnNewQuote							{
			get { return this.ScriptRunningOnNewQuote.WaitOne(0); }
			set { if (value == true) this.ScriptRunningOnNewQuote.Set();
								else this.ScriptRunningOnNewQuote.Reset(); } }
		public bool IsScriptRunningOnAlertFilled						{
			get { return this.ScriptRunningOnAlertFilled.WaitOne(0); }
			set { if (value == true) this.ScriptRunningOnAlertFilled.Set();
								else this.ScriptRunningOnAlertFilled.Reset(); } }
		public bool IsScriptRunningOnAlertKilled						{
			get { return this.ScriptRunningOnAlertKilled.WaitOne(0); }
			set { if (value == true) this.ScriptRunningOnAlertKilled.Set();
								else this.ScriptRunningOnAlertKilled.Reset(); } }
		public bool IsScriptRunningOnAlertNotSubmitted					{
			get { return this.ScriptRunningOnAlertNotSubmitted.WaitOne(0); }
			set { if (value == true) this.ScriptRunningOnAlertNotSubmitted.Set();
								else this.ScriptRunningOnAlertNotSubmitted.Reset(); } }
		public bool IsScriptRunningOnPositionOpened						{
			get { return this.ScriptRunningOnPositionOpened.WaitOne(0); }
			set { if (value == true) this.ScriptRunningOnPositionOpened.Set();
								else this.ScriptRunningOnPositionOpened.Reset(); } }
		public bool IsScriptRunningOnPositionOpenedPrototypeSlTpPlaced	{
			get { return this.ScriptRunningOnPositionOpenedPrototypeSlTpPlaced.WaitOne(0); }
			set { if (value == true) this.ScriptRunningOnPositionOpenedPrototypeSlTpPlaced.Set();
								else this.ScriptRunningOnPositionOpenedPrototypeSlTpPlaced.Reset(); } }
		public bool IsScriptRunningOnPositionClosed						{
			get { return this.ScriptRunningOnPositionClosed.WaitOne(0); }
			set { if (value == true) this.ScriptRunningOnPositionClosed.Set();
								else this.ScriptRunningOnPositionClosed.Reset(); } }
		public bool IsScriptRunningOnStreamingTriggeringScriptTurnedOn	{
			get { return this.ScriptRunningOnStreamingTriggeringScriptTurnedOn.WaitOne(0); }
			set { if (value == true) this.ScriptRunningOnStreamingTriggeringScriptTurnedOn.Set();
								else this.ScriptRunningOnStreamingTriggeringScriptTurnedOn.Reset(); } }
		public bool IsScriptRunningOnStreamingTriggeringScriptTurnedOff	{
			get { return this.ScriptRunningOnStreamingTriggeringScriptTurnedOff.WaitOne(0); }
			set { if (value == true) this.ScriptRunningOnStreamingTriggeringScriptTurnedOff.Set();
								else this.ScriptRunningOnStreamingTriggeringScriptTurnedOff.Reset(); } }
		public bool IsScriptRunningOnStrategyEmittingOrdersTurnedOn		{
			get { return this.ScriptRunningOnStrategyEmittingOrdersTurnedOn.WaitOne(0); }
			set { if (value == true) this.ScriptRunningOnStrategyEmittingOrdersTurnedOn.Set();
								else this.ScriptRunningOnStrategyEmittingOrdersTurnedOn.Reset(); } }
		public bool IsScriptRunningOnStrategyEmittingOrdersTurnedOff {
			get { return this.ScriptRunningOnStrategyEmittingOrdersTurnedOff.WaitOne(0); }
			set { if (value == true) this.ScriptRunningOnStrategyEmittingOrdersTurnedOff.Set();
								else this.ScriptRunningOnStrategyEmittingOrdersTurnedOff.Reset(); } }

		public string WhatScriptOverridenMethodsAreRunningNow { get {
			string ret = "";
			if (this.IsScriptRunningOnBarStaticLast)						ret += "OnBarStaticLast,";
			if (this.IsScriptRunningOnNewQuote)								ret += "OnNewQuote,";
			if (this.IsScriptRunningOnAlertFilled)							ret += "OnAlertFilled,";
			if (this.IsScriptRunningOnAlertNotSubmitted)					ret += "OnAlertNotSubmitted,";
			if (this.IsScriptRunningOnPositionOpened)						ret += "OnPositionOpened,";
			if (this.IsScriptRunningOnPositionOpenedPrototypeSlTpPlaced)	ret += "OnPositionOpenedPrototypeSlTpPlaced,";
			if (this.IsScriptRunningOnPositionClosed)						ret += "PositionClosed,";
			if (this.IsScriptRunningOnStreamingTriggeringScriptTurnedOn)	ret += "OnStreamingTriggeringScriptTurnedOn,";
			if (this.IsScriptRunningOnStreamingTriggeringScriptTurnedOff)	ret += "OnStreamingTriggeringScriptTurnedOff,";
			if (this.IsScriptRunningOnStrategyEmittingOrdersTurnedOn)		ret += "OnStrategyEmittingOrdersTurnedOn,";
			if (this.IsScriptRunningOnStrategyEmittingOrdersTurnedOff)		ret += "OnStrategyEmittingOrdersTurnedOff,";
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
		public void PopupIfRunning(string msig) {
			if (this.AnyScriptOverridenMethodIsRunningNow == false) return;
			string msg = this.WhatScriptOverridenMethodsAreRunningNow;
			Assembler.PopupException(msg + msig, null, false);
		}
	}
}
