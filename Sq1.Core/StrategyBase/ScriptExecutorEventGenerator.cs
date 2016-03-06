using System;
using System.Collections.Generic;

using Sq1.Core.Execution;
using Sq1.Core.DataTypes;

namespace Sq1.Core.StrategyBase {
	public class ScriptExecutorEventGenerator {
		public event EventHandler<EventArgs>		OnBacktesterBarsIdenticalButEmptySubstitutedToGrow_step1of4;
		public event EventHandler<EventArgs>		OnBacktesterContextInitialized_step2of4;
		public event EventHandler<EventArgs>		OnBacktesterSimulatedChunk_step3of4;
		public event EventHandler<EventArgs>		OnBacktesterContextRestoredAfterExecutingAllBars_step4of4;

		public event EventHandler<QuoteEventArgs>	OnStrategyPreExecuteOneQuote;
		public event EventHandler<QuoteEventArgs>	OnStrategyExecuted_oneQuote;
		public event EventHandler<BarEventArgs>		OnStrategyExecuted_oneBar;
		public event EventHandler<EventArgs>		OnStrategyExecuted_oneQuoteOrBar_ordersEmitted;

		public event EventHandler<ReporterPokeUnitEventArgs>	OnBrokerFilledAlertsOpeningForPositions_step1of3;
		//YOU_KNOW_I_HATE_UNNECESSARY_EVENTS!!!__INVOKING_DIRECTLY_UpdateOpenPositionsDueToStreamingNewQuote()
		public event EventHandler<ReporterPokeUnitEventArgs>	OnOpenPositionsUpdatedDueToStreamingNewQuote_step2of3;
		public event EventHandler<ReporterPokeUnitEventArgs>	OnBrokerFilledAlertsClosingForPositions_step3of3;

		public event EventHandler<QuoteEventArgs>	OnQuoteReceived_butWasntPushedAnywhere_dueToZeroSubscribers_tunnelToInterChartForm;

		private ScriptExecutor scriptExecutor;

		public ScriptExecutorEventGenerator(ScriptExecutor scriptExecutor) {
			this.scriptExecutor = scriptExecutor;
		}

		public void RaiseOnBacktesterBarsIdenticalButEmptySubstitutedToGrow_step1of4() {
			if (this.OnBacktesterBarsIdenticalButEmptySubstitutedToGrow_step1of4 == null) return;
			this.OnBacktesterBarsIdenticalButEmptySubstitutedToGrow_step1of4(this, null);
		}
		public void RaiseOnBacktesterSimulationContextInitialized_step2of4() {
			if (this.OnBacktesterContextInitialized_step2of4 == null) return;
			this.OnBacktesterContextInitialized_step2of4(this, null);
		}
		public void RaiseOnBacktesterSimulatedChunk_step3of4() {
			if (this.OnBacktesterSimulatedChunk_step3of4 == null) return;
			this.OnBacktesterSimulatedChunk_step3of4(this, null);
		}
		public void RaiseOnBacktesterContextRestoredAfterExecutingAllBars_step4of4(Quote quote) {
			if (this.OnBacktesterContextRestoredAfterExecutingAllBars_step4of4 == null) return;
			try {
				this.OnBacktesterContextRestoredAfterExecutingAllBars_step4of4(this, new QuoteEventArgs(quote));
			} catch (Exception e) {
				string msg = "EVENT_CONSUMER(USED_ONLY_FOR_LIVE_SIMULATOR)_THROWN //DataDistributor.RaiseQuotePushedToDistributor(" + quote + ")";
				Assembler.PopupException(msg, e);
			}
		}

		public void RaiseOnBrokerFilledAlertsOpeningForPositions_step1of3(ReporterPokeUnit pokeUnit) {
			try {
				if (this.OnBrokerFilledAlertsOpeningForPositions_step1of3 == null) return;
				this.OnBrokerFilledAlertsOpeningForPositions_step1of3(this, new ReporterPokeUnitEventArgs(pokeUnit));
			} catch (Exception ex) {
				string msg = "EVENT_SUBSCRIBER_THREW_WHILE_BeginInvoke__BrokerFilledAlertsOpeningForPositions_step1of3()";
				Assembler.PopupException(msg, ex);
			}
		}
		public void RaiseOpenPositionsUpdatedDueToStreamingNewQuote_step2of3(ReporterPokeUnit pokeUnit) {
			try {
				if (this.OnOpenPositionsUpdatedDueToStreamingNewQuote_step2of3 == null) return;
				this.OnOpenPositionsUpdatedDueToStreamingNewQuote_step2of3(this, new ReporterPokeUnitEventArgs(pokeUnit));
			} catch (Exception ex) {
				string msg = "EVENT_SUBSCRIBER_THREW_WHILE_BeginInvoke__OpenPositionsUpdatedDueToStreamingNewQuote_step2of3()";
				Assembler.PopupException(msg, ex);
			}
		}
		public void RaiseOnBrokerFilledAlertsClosingForPositions_step3of3(ReporterPokeUnit pokeUnit) {
			try {
				if (this.OnBrokerFilledAlertsClosingForPositions_step3of3 == null) return;
				this.OnBrokerFilledAlertsClosingForPositions_step3of3(this, new ReporterPokeUnitEventArgs(pokeUnit));
			} catch (Exception ex) {
				string msg = "EVENT_SUBSCRIBER_THREW_WHILE_BeginInvoke__BrokerFilledAlertsClosingForPositions_step3of3()";
				Assembler.PopupException(msg, ex);
			}
		}
		
		public void RaiseOnStrategyPreExecute_oneQuote(Quote quoteForAlertsCreated) {
			if (this.OnStrategyPreExecuteOneQuote == null) return;
			try {
				this.OnStrategyPreExecuteOneQuote(this, new QuoteEventArgs(quoteForAlertsCreated));
			} catch (Exception ex) {
				string msg = "TUNNELLING_QUOTE_ARRIVED_TO_UPDATE_BTN_STREAMING_TEXT MOVED_FROM_ChartFromStreamingConsumer";
				Assembler.PopupException(msg, ex);
			}
		}
		public void RaiseOnStrategyExecuted_oneQuote(Quote quoteForAlertsCreated) {
			if (this.OnStrategyExecuted_oneQuote == null) return;
			try {
				this.OnStrategyExecuted_oneQuote(this, new QuoteEventArgs(quoteForAlertsCreated));
			} catch (Exception ex) {
				string msg = "//OnStrategyExecuted_oneQuote()";
				Assembler.PopupException(msg, ex);
			}
		}
		public void RaiseOnStrategyExecuted_oneBar(Bar bar) {
			if (this.OnStrategyExecuted_oneBar == null) return;
			try {
				this.OnStrategyExecuted_oneBar(this, new BarEventArgs(bar));
			} catch (Exception ex) {
				string msg = "//OnStrategyExecuted_oneBar()";
				Assembler.PopupException(msg, ex);
			}
		}
		public void RaiseOnStrategyExecuted_oneQuoteOrBar_ordersEmitted(List<Order> ordersEmitted) {
			if (this.OnStrategyExecuted_oneQuoteOrBar_ordersEmitted == null) return;
			try {
				this.OnStrategyExecuted_oneQuoteOrBar_ordersEmitted(this, new OrdersListEventArgs(ordersEmitted));
			} catch (Exception ex) {
				string msg = "//RaiseOnStrategyExecuted_oneQuoteOrBar_ordersEmitted()";
				Assembler.PopupException(msg, ex);
			}
		}

		public void RaiseOnQuoteReceived_butWasntPushedAnywhere_dueToZeroSubscribers_tunnelToInterChartForm(Quote quote) {
			if (this.OnQuoteReceived_butWasntPushedAnywhere_dueToZeroSubscribers_tunnelToInterChartForm == null) return;
			try {
				this.OnQuoteReceived_butWasntPushedAnywhere_dueToZeroSubscribers_tunnelToInterChartForm(this, new QuoteEventArgs(quote));
			} catch (Exception e) {
				string msg = "EVENT_CONSUMER(USED_ONLY_FOR_LIVE_SIMULATOR)_THROWN //ScriptExecutorEventGenerator.OnQuoteReceived_butWasntPushedAnywhere_dueToZeroSubscribers(" + quote + ")";
				Assembler.PopupException(msg, e);
			}
		}

	}
}
