using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;

using Sq1.Core;
using Sq1.Core.DataTypes;
using Sq1.Core.Execution;
using Sq1.Core.Indicators;
using Sq1.Core.StrategyBase;
using Sq1.Core.Streaming;

namespace Sq1.Strategies.Demo {
	public class EnterEveryBarCompiled : Script {
		// if an indicator is NULL (isn't initialized in this.ctor()) you'll see INDICATOR_DECLARED_BUT_NOT_CREATED+ASSIGNED_IN_CONSTRUCTOR in ExceptionsForm 
		IndicatorMovingAverageSimple MAfast;
		ScriptParameter test;
		ScriptParameter verbose;

		public EnterEveryBarCompiled() {
			MAfast = new IndicatorMovingAverageSimple();
			MAfast.ParamPeriod = new IndicatorParameter("Period", 15, 10, 20, 1);
			MAfast.LineWidth = 2;
			MAfast.LineColor = Color.LightSeaGreen;
			
			//base.ScriptParameterCreateRegister(1, "test", 0, 0, 10, 1);
			test = new ScriptParameter(1, "test", 0, 0, 10, 1);

			//base.ScriptParameterCreateRegister(2, "verbose", 0, 0, 1, 1, "set to 0 if you don't want log() to spam your Exceptions window");
			verbose = new ScriptParameter(2, "verbose", 0, 0, 10, 1, "set to 0 if you don't want log() to spam your Exceptions window");
		}
		
		protected void log(string msg) {
			if (this.verbose.ValueCurrent == 0) {
				return;
			}
			string whereIam = "\n\r\n\rEnterEveryBar.cs now=[" + DateTime.Now.ToString("ddd dd-MMM-yyyy HH:mm:ss.fff") + "]";
			this.Executor.PopupException(msg + whereIam);
		}
		public override void InitializeBacktest() {
			//Debugger.Break();
			//this.PadBars(0);
			if (base.Strategy == null) {
				log("CANT_SET_EXCEPTIONS_LIMIT: base.Strategy == null");
				#if DEBUG
				Debugger.Break();
				#endif
				return;
			}
			base.Strategy.ExceptionsLimitToAbortBacktest = 10;
			//this.MAslow.NotOnChartSymbol = "SANDP-FUT";
			//this.MAslow.NotOnChartBarScaleInterval = new BarScaleInterval(BarScale.Hour, 1);
			//this.MAslow.NotOnChartBarScaleInterval = new BarScaleInterval(BarScale.Minute, 15);
			//this.MAslow.LineWidth = 2;
			//this.MAslow.LineColor = System.Drawing.Color.LightCoral;
			
			testChartLabelDrawOnNextLineModify();
		}
		void testChartLabelDrawOnNextLineModify() {
			//Font font = new Font(FontFamily.GenericMonospace, 8, FontStyle.Bold);
			//base.Executor.ChartConditionalChartLabelDrawOnNextLineModify("labelTest", "test[" + test+ "]", font, Color.Brown, Color.Empty);
			Font font = new Font("Consolas", 8, FontStyle.Bold);
			base.Executor.ChartConditionalChartLabelDrawOnNextLineModify("labelTest", "test["
				+ this.test.ValueCurrent + "]", font, Color.Brown, Color.Beige);
		}
		public override void OnNewQuoteOfStreamingBarCallback(Quote quote) {
			//double slowStreaming = this.MAslow.BarClosesProxied.StreamingValue;
			//double slowStatic = this.MAslow.ClosesProxyEffective.LastStaticValue;
			//DateTime slowStaticDate = this.MAslow.ClosesProxyEffective.LastStaticDate;


			if (this.Executor.Backtester.IsBacktestingNoLivesimNow == false) {
				Bar bar = quote.ParentBarStreaming;
				int barNo = bar.ParentBarsIndex;
				if (barNo <= 0) return;
				DateTime lastStaticBarDateTime = bar.ParentBars.BarStaticLastNullUnsafe.DateTimeOpen;
				DateTime streamingBarDateTime = bar.DateTimeOpen;
				Bar barNormalizedDateTimes = new Bar(bar.Symbol, bar.ScaleInterval, quote.ServerTime);
				DateTime thisBarDateTimeOpen = barNormalizedDateTimes.DateTimeOpen;
				int a = 1;
			}
			//log("OnNewQuoteCallback(): [" + quote.ToString() + "]"); 
			string msg = "OnNewQuoteCallback(): [" + quote.ToString() + "]";
			log("EnterEveryBar.cs now=[" + DateTime.Now.ToString("ddd dd-MMM-yyyy HH:mm:ss.fff" + "]: " + msg));

			if (quote.IntraBarSerno == 0) {
				return;
			}
		}
		public override void OnBarStaticLastFormedWhileStreamingBarWithOneQuoteAlreadyAppendedCallback(Bar barStaticFormed) {
			//this.testBarAnnotations(barStaticFormed);
			
			//Thread.Sleep(500);

			Bar barStreaming = base.Bars.BarStreaming;
			if (this.Executor.Backtester.IsBacktestingNoLivesimNow == false) {
				//Debugger.Break();
			}
			if (barStaticFormed.ParentBarsIndex <= 2) return;
			if (barStaticFormed.IsBarStreaming) {
				string msg = "SHOULD_NEVER_HAPPEN triggered@barStaticFormed.IsBarStreaming[" + barStaticFormed + "] while Streaming[" + barStreaming + "]";
				#if DEBUG
				Debugger.Break();
				#endif
			}

			Position lastPos = base.LastPosition;
			bool isLastPositionNotClosedYet = base.IsLastPositionNotClosedYet;
			if (isLastPositionNotClosedYet) {
				if (lastPos.EntryFilledBarIndex > barStaticFormed.ParentBarsIndex) {
					string msg1 = "NOTIFIED_ABOUT_LAST_FORMED_WHILE_LAST_POST_FILLED_AT_STREAMING__LOOKS_OK";
					//Debugger.Break();
				}

				if (lastPos.ExitAlert != null) {
					string msg1 = "you want to avoid POSITION_ALREADY_HAS_AN_EXIT_ALERT_REPLACE_INSTEAD_OF_ADDING_SECOND"
						+ " ExitAtMarket by throwing [can't have two closing alerts for one positionExit] Strategy[" + this.Strategy.ToString() + "]";
					#if DEBUG
					Debugger.Break();
					#endif
					return;
				}

				//if (barStaticFormed.ParentBarsIndex == 163) {
				//	#if DEBUG
				//	Debugger.Break();
				//	#endif
				//	StreamingDataSnapshot streaming = this.Executor.DataSource.StreamingAdapter.StreamingDataSnapshot;
				//	Quote lastQuote = streaming.LastQuoteCloneGetForSymbol(barStaticFormed.Symbol);
				//	double priceForMarketOrder = streaming.LastQuoteGetPriceForMarketOrder(barStaticFormed.Symbol);
				//}

				string msg = "ExitAtMarket@" + barStaticFormed.ParentBarsIdent;
				this.Executor.ExecutionDataSnapshot.IsScriptRunningOnBarStaticLastNonBlockingRead = false;
				Alert exitPlaced = ExitAtMarket(barStreaming, lastPos, msg);
				this.Executor.ExecutionDataSnapshot.IsScriptRunningOnBarStaticLastNonBlockingRead = true;
				log("Execute(): " + msg);
			}

			ExecutionDataSnapshot snap = base.Executor.ExecutionDataSnapshot;

			if (base.HasAlertsPendingOrPositionsOpenNow) {
			//if (base.HasAlertsPendingAndPositionsOpenNow) {
				if (snap.AlertsPending.Count > 0) {
					//GOT_OUT_OF_BOUNDADRY_EXCEPTION_ONCE Alert firstPendingAlert = snap.AlertsPending.InnerList[0];
					Alert firstPendingAlert = snap.AlertsPending.LastNullUnsafe;
					Alert lastPosEntryAlert = lastPos != null ? lastPos.EntryAlert : null;
					Alert lastPosExitAlert  = lastPos != null ? lastPos.ExitAlert : null;
					if (firstPendingAlert == lastPosEntryAlert) {
						string msg = "EXPECTED: I don't have open positions but I have an unfilled firstPendingAlert from lastPosition.EntryAlert=alertsPending[0]";
						this.log(msg);
					} else if (firstPendingAlert == lastPosExitAlert) {
						string msg = "EXPECTED: I have and open lastPosition with .ExitAlert=alertsPending[0]";
						this.log(msg);
					} else {
						string msg = "UNEXPECTED: firstPendingAlert alert doesn't relate to lastPosition; who is here?";
						this.log(msg);
					}
				}
				if (snap.PositionsOpenNow.Count > 1) {
					string msg = "EXPECTED: I got multiple positions[" + snap.PositionsOpenNow.Count + "]";
					if (snap.PositionsOpenNow.InnerList[0] == lastPos) {
						msg += "50/50: positionsMaster.Last = positionsOpenNow.First";
					}
					this.log(msg);
				}
				return;
			}

			if (barStaticFormed.Close > barStaticFormed.Open) {
				string msg = "BuyAtMarket@" + barStaticFormed.ParentBarsIdent;
				this.Executor.ExecutionDataSnapshot.IsScriptRunningOnBarStaticLastNonBlockingRead = false;
				Position buyPlaced = BuyAtMarket(barStreaming, msg);
				this.Executor.ExecutionDataSnapshot.IsScriptRunningOnBarStaticLastNonBlockingRead = true;
				//Debugger.Break();
				this.log(msg);
			} else {
				string msg = "ShortAtMarket@" + barStaticFormed.ParentBarsIdent;
				this.Executor.ExecutionDataSnapshot.IsScriptRunningOnBarStaticLastNonBlockingRead = false;
				Position shortPlaced = ShortAtMarket(barStreaming, msg);
				this.Executor.ExecutionDataSnapshot.IsScriptRunningOnBarStaticLastNonBlockingRead = true;
				//Debugger.Break();
				this.log(msg);
			}
		}
		
		public override void OnStreamingTriggeringScriptTurnedOnCallback() {
			string msg = "SCRIPT_IS_NOW_AWARE_THAT_STREAMING_ADAPDER_WILL_TRIGGER_SCRIPT_METHODS"
				+ " ScriptContextCurrent.IsStreamingTriggeringScript[" + this.Strategy.ScriptContextCurrent.IsStreamingTriggeringScript+ "]";
			Assembler.PopupException(msg, null, false);
			
			if (base.HasAlertsPendingOrPositionsOpenNow == false) return;

			string msg2 = "here you can probably sync your actual open positions on the broker side with backtest-opened ghosts";
			Assembler.PopupException(msg2, null, false);
		}
		public override void OnStreamingTriggeringScriptTurnedOffCallback() {
			string msg = "SCRIPT_IS_NOW_AWARE_THAT_STREAMING_ADAPDER_WILL_NOT_TRIGGER_SCRIPT_METHODS"
				+ " ScriptContextCurrent.IsStreamingTriggeringScript[" + this.Strategy.ScriptContextCurrent.IsStreamingTriggeringScript+ "]";
			Assembler.PopupException(msg, null, false);
		}
		
		public override void OnStrategyEmittingOrdersTurnedOnCallback() {
			string msg = "SCRIPT_IS_NOW_AWARE_THAT_ORDERS_WILL_START_SHOOTING_THROUGH_BROKER_ADAPDER"
				+ " ScriptContextCurrent.StrategyEmittingOrders[" + this.Strategy.ScriptContextCurrent.StrategyEmittingOrders+ "]";
			Assembler.PopupException(msg, null, false);
		}
		public override void OnStrategyEmittingOrdersTurnedOffCallback() {
			string msg = "SCRIPT_IS_NOW_AWARE_THAT_ORDERS_WILL_STOP_SHOOTING_THROUGH_BROKER_ADAPDER"
				+ " ScriptContextCurrent.StrategyEmittingOrders[" + this.Strategy.ScriptContextCurrent.StrategyEmittingOrders+ "]";
			Assembler.PopupException(msg, null, false);
		}

		
		public override void OnAlertFilledCallback(Alert alertFilled) {
			if (alertFilled.FilledBarIndex == 12) {
				//Debugger.Break();
			}
		}
		public override void OnAlertKilledCallback(Alert alertKilled) {
			#if DEBUG
			Debugger.Break();
			#endif
		}
		public override void OnAlertNotSubmittedCallback(Alert alertNotSubmitted, int barNotSubmittedRelno) {
			#if DEBUG
			Debugger.Break();
			#endif
		}
		public override void OnPositionOpenedCallback(Position positionOpened) {
			//if (positionOpened.EntryFilledBarIndex == 37) {
			//	#if DEBUG
			//	Debugger.Break();
			//	#endif
			//}
		}
		public override void OnPositionOpenedPrototypeSlTpPlacedCallback(Position positionOpenedByPrototype) {
			#if DEBUG
			Debugger.Break();
			#endif
		}
		public override void OnPositionClosedCallback(Position positionClosed) {
			//if (positionClosed.EntryFilledBarIndex == 37) {
			//	Debugger.Break();
			//}
		}
		void testBarAnnotations(Bar barStaticFormed) {
			int barIndex = barStaticFormed.ParentBarsIndex;
			string labelText = barStaticFormed.DateTimeOpen.ToString("HH:mm");
			labelText += " " + barStaticFormed.BarIndexAfterMidnightReceived + "/";
			labelText += barStaticFormed.BarIndexExpectedSinceTodayMarketOpen + ":" + barStaticFormed.BarIndexExpectedMarketClosesTodaySinceMarketOpen;
			Font font = new Font("Consolas", 7);
			//bool evenAboveOddBelow = true;
			bool evenAboveOddBelow = (barStaticFormed.ParentBarsIndex % 2) == 0;
			base.Executor.ChartConditionalBarAnnotationDrawModify(
				barIndex, "ann" + barIndex, labelText, font, Color.ForestGreen, Color.Empty, evenAboveOddBelow);
			// checking labels stacking next upon (underneath) the previous
			base.Executor.ChartConditionalBarAnnotationDrawModify(
				barIndex, "ann2" + barIndex, labelText, font, Color.ForestGreen, Color.LightGray, evenAboveOddBelow);
		}
	}
}