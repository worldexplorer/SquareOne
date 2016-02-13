using System;
using System.Threading;
using System.Collections.Generic;
using System.Windows.Forms;

using Sq1.Core.Execution;
using Sq1.Core.DataTypes;

namespace Sq1.Core.Livesim {
	public partial class Livesimulator {
		ManualResetEvent barsAreSetInGuiThread = new ManualResetEvent(false);	// SOLVES__BAR_STATIC_LAST_IS_NULL__DURING_SECOND_LIVESIM
		void executor_OnBacktesterContextInitialized_step2of4(object sender, EventArgs e) {
			barsAreSetInGuiThread.Reset();	// SOLVES__BAR_STATIC_LAST_IS_NULL__DURING_SECOND_LIVESIM
			if (this.chartShadow.InvokeRequired) {
				// will always InvokeRequied since we RaiseOnBacktesterSimulationContextInitialized_step2of4
				// from a just started thread with a new Backtest BacktesterRunSimulation_threadEntry_exceptionCatcher() SEE_CALL_STACK_NOW
				// too late to do it in GUI thread; switch takes a tons of time; do gui-unrelated preparations NOW
				List<Order> ordersStale = this.DataSourceAsLivesim_nullUnsafe.BrokerAsLivesim_nullUnsafe.OrdersSubmittedForOneLivesimBacktest;
				if (ordersStale.Count > 0) {
					int beforeCleanup = this.Executor.OrderProcessor.DataSnapshot.OrdersAll.Count;
					this.Executor.OrderProcessor.DataSnapshot.OrdersRemove(ordersStale);
					int afterCleanup = this.Executor.OrderProcessor.DataSnapshot.OrdersAll.Count;
					if (beforeCleanup > 0 && beforeCleanup <= afterCleanup)  {
						string msg = "STALE_ORDER_CLEANUP_DOESNT_WORK__LIVESIM";
						Assembler.PopupException(msg);
					}
					ordersStale.Clear();
				}
				//base.Stopwatch.Restart();
				var alreadyStartedUpstack = base.Stopwatch;

				this.chartShadow.BeginInvoke((MethodInvoker)delegate { this.executor_OnBacktesterContextInitialized_step2of4(sender, e); });
				return;
			}

			if (this.chartShadow.Bars != null) {
				this.DataSourceAsLivesim_nullUnsafe.StreamingAsLivesim_nullUnsafe.OnQuoteReceived_butWasntPushedAnywhere_dueToZeroSubscribers_blinkDataSourceTreeWithOrange -= new EventHandler<QuoteEventArgs>(streamingAsLivesim_nullUnsafe_OnQuoteReceived_butWasntPushedAnywhere_dueToZeroSubscribers_blinkDataSourceTreeWithOrange);
			}


			this.chartShadow.Initialize(base.Executor.Bars, base.Executor.StrategyName, false, true);

			this.DataSourceAsLivesim_nullUnsafe.StreamingAsLivesim_nullUnsafe.OnQuoteReceived_butWasntPushedAnywhere_dueToZeroSubscribers_blinkDataSourceTreeWithOrange += new EventHandler<QuoteEventArgs>(streamingAsLivesim_nullUnsafe_OnQuoteReceived_butWasntPushedAnywhere_dueToZeroSubscribers_blinkDataSourceTreeWithOrange);

			barsAreSetInGuiThread.Set();		// SOLVES__BAR_STATIC_LAST_IS_NULL__DURING_SECOND_LIVESIM release the thread that waits to start livesim (second livesim throws LastBar_nullUnsafe == null)
			barsAreSetInGuiThread.Reset();		// SOLVES__BAR_STATIC_LAST_IS_NULL__DURING_SECOND_LIVESIM and close the gate again - you should not wait!!! I don't trust AutoResetEvent

			this.btnPauseResume.Enabled = true;
			this.btnPauseResume.Text = "Pause";
		}

		// mess here, but but there is a dead end anyway ("if there is no ChartControl I have no way to transfer it")
		void livesimulator_OnQuoteReceived_zeroSubscribers_blinkDataSourceTreeWithOrange(object sender, QuoteEventArgs e) {
			// INFINITE_LOOP this.raiseOnQuoteReceived_zeroSubscribers_blinkDataSourceTreeWithOrange(e);
		}
		void streamingAsLivesim_nullUnsafe_OnQuoteReceived_butWasntPushedAnywhere_dueToZeroSubscribers_blinkDataSourceTreeWithOrange(object sender, QuoteEventArgs e) {
			this.raiseOnQuoteReceived_zeroSubscribers_blinkDataSourceTreeWithOrange(e);
		}
	}
}
