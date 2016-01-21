using Sq1.Core.DataTypes;
using Sq1.Core.Charting;
using Sq1.Core.DataFeed;
using System;
using Sq1.Core.StrategyBase;

namespace Sq1.Core.Streaming {
	public abstract class StreamingConsumer {
		protected 		string	MsigForNpExceptions	= "Failed to StreamingSubscribe(): ";
		public			string	ReasonToExist				{ get; protected set; }
		public virtual	Bars	ConsumerBarsToAppendInto	{ get { return this.Bars_nullReported; } }

		public abstract	void	UpstreamSubscribedToSymbolNotification(Quote quoteFirstAfterStart);
		public abstract	void	UpstreamUnSubscribedFromSymbolNotification(Quote quoteLastBeforeStop);
		public abstract	void	ConsumeQuoteOfStreamingBar(Quote quote);
		public abstract	void	ConsumeBarLastStaticJustFormedWhileStreamingBarWithOneQuoteAlreadyAppended(Bar barLastFormed, Quote quoteForAlertsCreated);



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
				var ret = this.Strategy_nullReported.ScriptContextCurrent;
				this.ActionForNullPointer(ret, "this.Executor.Strategy.ScriptContextCurrent=null");
				return ret;
			} }
		protected 		string Symbol_nullReported { get {
				string symbol = (this.Executor_nullReported.Strategy == null) ? this.Executor_nullReported.Bars.Symbol : this.ContextCurrentChartOrStrategy_nullReported.Symbol;
				if (String.IsNullOrEmpty(symbol)) {
					this.Action("this.Executor.Strategy.ScriptContextCurrent.Symbol IsNullOrEmpty");
				}
				return symbol;
			} }
		protected 		BarScaleInterval ScaleInterval_nullReported { get {
				var ret = (this.Executor_nullReported.Strategy == null) ? this.Executor_nullReported.Bars.ScaleInterval : this.ContextCurrentChartOrStrategy_nullReported.ScaleInterval;
				this.ActionForNullPointer(ret, "this.Executor.Strategy.ScriptContextCurrent.ScaleInterval=null");
				return ret;
			} }
		protected 		BarScale Scale_nullReported { get {
				var ret = this.ScaleInterval_nullReported.Scale;
				this.ActionForNullPointer(ret, "this.Executor.Strategy.ScriptContextCurrent.ScaleInterval.Scale=null");
				if (ret == BarScale.Unknown) {
					this.Action("this.Executor.Strategy.ScriptContextCurrent.ScaleInterval.Scale=Unknown");
				}
				return ret;
			} }
//		BarDataRange DataRange { get {
//				var ret = this.ScriptContextCurrent.DataRange;
//				this.actionForNullPointer(ret, "this.Executor.Strategy.ScriptContextCurrent.DataRange=null");
//				return ret;
//			} }
		protected 		DataSource DataSource_nullReported { get {
				var ret = this.Executor_nullReported.DataSource;
				this.ActionForNullPointer(ret, "this.Executor.DataSource=null");
				return ret;
			} }
		protected 		StreamingAdapter StreamingAdapter_nullReported { get {
				StreamingAdapter ret = this.DataSource_nullReported.StreamingAdapter;
				this.ActionForNullPointer(ret, "this.Executor.DataSource[" + this.DataSource_nullReported + "].StreamingAdapter=null STREAMING_ADAPDER_NOT_ASSIGNED_IN_DATASOURCE");
				return ret;
			} }
		protected 		StreamingSolidifier StreamingSolidifierDeep { get {
				var ret = this.StreamingAdapter_nullReported.StreamingSolidifier;
				this.ActionForNullPointer(ret, "this.Executor.DataSource[" + this.DataSource_nullReported + "].StreamingAdapter.StreamingSolidifier=null");
				return ret;
			} }

		protected 		ChartShadow ChartShadow_nullReported { get {
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
				var ret = this.Bars_nullReported.BarStreamingNullUnsafeCloneReadonly;
				//this.actionForNullPointer(ret, "this.Executor.Bars.StreamingBarSafeClone=null");
				if (ret == null) ret = new Bar();
				return ret;
			} }
		protected 		Bar LastStaticBar_nullReported { get {
				var ret = this.Bars_nullReported.BarStaticLastNullUnsafe;
				this.ActionForNullPointer(ret, "this.Executor.Bars.LastStaticBar=null");
				return ret;
			} }
		public bool DownstreamSubscribed { get {
				if (this.CanSubscribeToStreamingAdapter() == false) return false;	// NULL_POINTERS_ARE_ALREADY_REPORTED_TO_EXCEPTIONS_FORM

				var streamingSafe		= this.StreamingAdapter_nullReported;
				var symbolSafe			= this.Symbol_nullReported;
				var scaleIntervalSafe	= this.ScaleInterval_nullReported;

				bool quote	= streamingSafe.DataDistributor_replacedForLivesim.ConsumerQuoteIsSubscribed(	symbolSafe, scaleIntervalSafe, this);
				bool bar	= streamingSafe.DataDistributor_replacedForLivesim.ConsumerBarIsSubscribed(	symbolSafe, scaleIntervalSafe, this);
				bool ret = quote & bar;
				return ret;
			}}

	
		protected bool CanSubscribeToStreamingAdapter() {
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

		public void ActionForNullPointer(object mustBeInstance, string msgIfNull) {
			if (mustBeInstance != null) return;
			this.Action(msgIfNull);
		}
		public void Action(string msgIfNull) {
			string msg = MsigForNpExceptions + msgIfNull;
			Assembler.PopupException(msg, null, false);
			//throw new Exception(msg);
		}

	}
}
