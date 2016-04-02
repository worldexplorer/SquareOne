using System;

using Sq1.Core.DataTypes;
using Sq1.Core.Charting;
using Sq1.Core.DataFeed;
using Sq1.Core.StrategyBase;
using Sq1.Core.Livesim;

namespace Sq1.Core.Streaming {
	public abstract class StreamingConsumer {
		protected 		string				MsigForNpExceptions			= "Failed to StreamingSubscribe(): ";
		public			string				ReasonToExist				{ get; protected set; }

		public virtual	Bars				ConsumerBars_toAppendInto	{ get { return this.Bars_nullReported; } }
		public virtual	BarScaleInterval	ScaleInterval				{ get { return this.ConsumerBars_toAppendInto.ScaleInterval; } }
		public virtual	string				Symbol						{ get { return this.ConsumerBars_toAppendInto.Symbol; } }

		public abstract	void	UpstreamSubscribed_toSymbol_streamNotifiedMe(Quote quoteFirstAfterStart);
		public abstract	void	UpstreamUnsubscribed_fromSymbol_streamNotifiedMe(Quote quoteLastBeforeStop);
		public abstract	void	ConsumeQuoteOfStreamingBar(Quote quote);
		public abstract	void	ConsumeBarLastStatic_justFormed_whileStreamingBarWithOneQuote_alreadyAppended(Bar barLastFormed, Quote quoteForAlertsCreated);


		public abstract	ScriptExecutor	Executor			{ get; }
		
		#region CASCADED_INITIALIZATION_ALL_CHECKING_CONSISTENCY_FROM_ONE_METHOD begin
		protected 		ScriptExecutor Executor_nullReported { get {
				var ret = this.Executor;
				this.ActionForNullPointer(ret, "this.Executor=null");
				return ret;
			} }
		protected 		Strategy Strategy_nullReported { get {
				var ret = this.Executor_nullReported.Strategy;
				this.ActionForNullPointer(ret, "this.Executor.Strategy=null");
				return ret;
			} }
		protected 		ContextChart ContextCurrentChartOrStrategy_nullReported { get {
				string msg = "";
				Strategy strategy = this.Executor.Strategy;
				if (strategy != null) {
					return strategy.ScriptContextCurrent;
				}
				msg += "NOT_STRATEGY";
				ChartShadow chartShadow_nullUnsafe = this.ChartShadow_nullReported;
				if (chartShadow_nullUnsafe != null) {
					return chartShadow_nullUnsafe.CtxChart;
				}
				msg += " AND_ChartShadow.CtxChart_WAS_NOT_INITIALIZED_YET";
				Assembler.PopupException(msg);
				return null;
			} }
		protected 		string Symbol_nullReported { get {
				ScriptExecutor executor_nullUnsafe = this.Executor_nullReported;
				string symbol = (executor_nullUnsafe.Strategy == null) ? executor_nullUnsafe.Bars.Symbol : this.ContextCurrentChartOrStrategy_nullReported.Symbol;
				if (String.IsNullOrEmpty(symbol)) {
					this.action("this.Executor.Strategy.ScriptContextCurrent.Symbol IsNullOrEmpty");
				}
				return symbol;
			} }
		protected 		BarScaleInterval ScaleInterval_nullReported { get {
				ScriptExecutor executor_nullUnsafe = this.Executor_nullReported;
				var ret = (executor_nullUnsafe.Strategy == null) ? executor_nullUnsafe.Bars.ScaleInterval : this.ContextCurrentChartOrStrategy_nullReported.ScaleInterval;
				this.ActionForNullPointer(ret, "this.Executor.Strategy.ScriptContextCurrent.ScaleInterval=null");
				return ret;
			} }
		protected 		BarScale Scale_nullReported { get {
				var ret = this.ScaleInterval_nullReported.Scale;
				this.ActionForNullPointer(ret, "this.Executor.Strategy.ScriptContextCurrent.ScaleInterval.Scale=null");
				if (ret == BarScale.Unknown) {
					this.action("this.Executor.Strategy.ScriptContextCurrent.ScaleInterval.Scale=Unknown");
				}
				return ret;
			} }
//		BarDataRange DataRange { get {
//				var ret = this.ScriptContextCurrent.DataRange;
//				this.actionForNullPointer(ret, "this.Executor.Strategy.ScriptContextCurrent.DataRange=null");
//				return ret;
//			} }
		protected 		DataSource DataSource_nullReported { get {
				var ret = this.Executor_nullReported.DataSource_fromBars;
				this.ActionForNullPointer(ret, "this.Executor.DataSource=null");
				return ret;
			} }
		protected 		StreamingAdapter StreamingAdapter_nullReported { get {
				StreamingAdapter ret = this.DataSource_nullReported.StreamingAdapter;
				this.ActionForNullPointer(ret, "STREAMING_ADAPDER_NOT_ASSIGNED_IN_DATASOURCE this.Executor.DataSource[" + this.DataSource_nullReported + "].StreamingAdapter=null");
				return ret;
			} }
		protected 		StreamingSolidifier StreamingSolidifierDeep { get {
				if (this.StreamingAdapter_nullReported is LivesimStreamingDefault) {
					return null;
				}
				var ret = this.StreamingAdapter_nullReported.StreamingSolidifier;
				this.ActionForNullPointer(ret, "SOLIDIFIER_NULL_IN_STREAMING this.Executor.DataSource[" + this.DataSource_nullReported.Name + "].StreamingAdapter[" + this.DataSource_nullReported.StreamingAdapterName + "].StreamingSolidifier=null");
				return ret;
			} }

		protected 		ChartShadow ChartShadow_nullReported { get {
				ChartStreamingConsumer thisAsChartConsumer = this as ChartStreamingConsumer;
				if (thisAsChartConsumer != null) return thisAsChartConsumer.ChartShadow;
				var ret = this.Executor_nullReported.ChartShadow;
				this.ActionForNullPointer(ret, "this.Executor.ChartShadow=null");
				return ret;
			} }
		protected 		Bars Bars_nullReported { get {
				var ret = this.Executor_nullReported.Bars;
				this.ActionForNullPointer(ret, "this.Executor.Bars=null");
				return ret;
			} }
		protected 		Bar StreamingBarSafeClone_nullReported { get {
				var ret = this.Bars_nullReported.BarStreaming_nullUnsafeCloneReadonly;
				//this.actionForNullPointer(ret, "this.Executor.Bars.StreamingBarSafeClone=null");
				if (ret == null) ret = new Bar();
				return ret;
			} }
		protected 		Bar LastStaticBar_nullReported { get {
				var ret = this.Bars_nullReported.BarStaticLast_nullUnsafe;
				this.ActionForNullPointer(ret, "this.Executor.Bars.LastStaticBar=null");
				return ret;
			} }
		public bool DownstreamSubscribed { get {
				if (this.NPEs_handled() == false) return false;	// NULL_POINTERS_ARE_ALREADY_REPORTED_TO_EXCEPTIONS_FORM

				var streamingSafe		= this.StreamingAdapter_nullReported;
				var symbolSafe			= this.Symbol_nullReported;
				var scaleIntervalSafe	= this.ScaleInterval_nullReported;

				if (streamingSafe.Distributor_substitutedDuringLivesim == null) {
					// quick hack; make QuikStreaming use base(reasonIwasCreated)
					this.StreamingAdapter_nullReported.CreateDistributors_onlyWhenNecessary(this.ReasonToExist);
				}
				bool quote	= streamingSafe.Distributor_substitutedDuringLivesim.ConsumerQuoteIsSubscribed(this);
				bool bar	= streamingSafe.Distributor_substitutedDuringLivesim.ConsumerBarIsSubscribed(this);
				bool ret = quote & bar;
				return ret;
			}}

	
		protected bool NPEs_handled() {
			try {
				var symbolSafe		= this.Symbol_nullReported;
				var scaleSafe		= this.Scale_nullReported;
				var streamingSafe	= this.StreamingAdapter_nullReported;
				var staticDeepSafe	= this.StreamingSolidifierDeep;
			} catch (Exception ex) {
				// already reported
				return false;
			}
			return true;
		}
		#endregion

		protected void ActionForNullPointer(object mustBeInstance, string msgIfNull) {
			if (mustBeInstance != null) return;
			this.action(msgIfNull);
		}
		void action(string msgIfNull) {
			Assembler.PopupException(msgIfNull + this.MsigForNpExceptions, null, false);
			//throw new Exception(msg);
		}

		public virtual void PumpPaused_notification_overrideMe_switchLivesimmingThreadToGui() {
		}
		public virtual void PumpUnPaused_notification_overrideMe_switchLivesimmingThreadToGui() {
		}

		public override string ToString() {
			return this.ReasonToExist;
		}
	}
}
