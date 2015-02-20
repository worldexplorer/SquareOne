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

		public event EventHandler<QuoteEventArgs>	OnStrategyExecutedOneQuote;
		public event EventHandler<BarEventArgs>		OnStrategyExecutedOneBar;
		public event EventHandler<EventArgs>		OnStrategyExecutedOneQuoteOrBarOrdersEmitted;

		public event EventHandler<ReporterPokeUnitEventArgs>	OnBrokerFilledAlertsOpeningForPositions_step1of3;
		//YOU_KNOW_I_HATE_UNNECESSARY_EVENTS!!!__INVOKING_DIRECTLY_UpdateOpenPositionsDueToStreamingNewQuote()
		public event EventHandler<ReporterPokeUnitEventArgs>	OnOpenPositionsUpdatedDueToStreamingNewQuote_step2of3;
		public event EventHandler<ReporterPokeUnitEventArgs>	OnBrokerFilledAlertsClosingForPositions_step3of3;

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

		public void RaiseOnStrategyExecutedOneQuote(Quote quoteForAlertsCreated) {
			if (this.OnStrategyExecutedOneQuote == null) return;
			this.OnStrategyExecutedOneQuote(this, new QuoteEventArgs(quoteForAlertsCreated));
		}
		public void RaiseOnStrategyExecutedOneBar(Bar bar) {
			if (this.OnStrategyExecutedOneBar == null) return;
			this.OnStrategyExecutedOneBar(this, new BarEventArgs(bar));
		}
		public void RaiseOnStrategyExecutedOneQuoteOrBarOrdersEmitted(List<Order> ordersEmitted) {
			if (this.OnStrategyExecutedOneQuoteOrBarOrdersEmitted == null) return;
			this.OnStrategyExecutedOneQuoteOrBarOrdersEmitted(this, new OrdersListEventArgs(ordersEmitted));
		}
	}
}
