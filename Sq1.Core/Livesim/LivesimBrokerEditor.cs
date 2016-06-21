using System;

namespace Sq1.Core.Livesim {
	public partial class LivesimBrokerEditor {
		LivesimBrokerSettings livesimBrokerSettings;

		public LivesimBrokerEditor() {
			this.InitializeComponent();
		}

		public void Initialize(LivesimBrokerSettings livesimBrokerSettings) {
			this.livesimBrokerSettings = livesimBrokerSettings;

			this.cbx_DelayBeforeFillEnabled					.Checked = this.livesimBrokerSettings.DelayBeforeFillEnabled;
			this.cbx_OrderRejectionEnabled					.Checked = this.livesimBrokerSettings.OrderRejectionEnabled;

			this.cbx_TransactionStatusAfterOrderStatusEnabled.Checked = this.livesimBrokerSettings.TransactionStatusAfterOrderStatusEnabled;
			this.cbx_KillPendingDelayEnabled				.Checked = this.livesimBrokerSettings.KillerTransactionCallbackAfterVictimFilledEnabled;

			this.cbx_PartialFillEnabled						.Checked = this.livesimBrokerSettings.PartialFillEnabled;
			this.cbx_OutOfOrderFillEnabled					.Checked = this.livesimBrokerSettings.OutOfOrderFillEnabled;
			this.cbx_PriceDeviationForMarketOrdersEnabled	.Checked = this.livesimBrokerSettings.PriceDeviationForMarketOrdersEnabled;
			this.cbx_KillPendingDelayEnabled				.Checked = this.livesimBrokerSettings.KillPendingDelayEnabled;
			this.cbx_AdaperDisconnectEnabled				.Checked = this.livesimBrokerSettings.AdapterDisconnectEnabled;
			this.cbx_ClearExecutionExceptions				.Checked = this.livesimBrokerSettings.ClearExecutionExceptions;



			this.txt_DelayBeforeFillMillisMin								.Text = this.livesimBrokerSettings.DelayBeforeFillMillisMin									.ToString();
			this.txt_DelayBeforeFillMillisMax								.Text = this.livesimBrokerSettings.DelayBeforeFillMillisMax									.ToString();
			this.txt_OrderRejectionHappensOncePerXordersMin					.Text = this.livesimBrokerSettings.OrderRejectionHappensOncePerXordersMin					.ToString();
			this.txt_OrderRejectionHappensOncePerXordersMax					.Text = this.livesimBrokerSettings.OrderRejectionHappensOncePerXordersMax					.ToString();

			this.txt_TransactionStatusAfterOrderStatusHappensOncePerOrdersMin	.Text = this.livesimBrokerSettings.TransactionStatusAfterOrderStatusHappensOncePerOrdersMin	.ToString();
			this.txt_TransactionStatusAfterOrderStatusHappensOncePerOrdersMax	.Text = this.livesimBrokerSettings.TransactionStatusAfterOrderStatusHappensOncePerOrdersMax	.ToString();
			this.txt_TransactionStatusAfterOrderStatusDelayAfterFillMin			.Text = this.livesimBrokerSettings.TransactionStatusAfterOrderStatusDelayAfterFillMin		.ToString();
			this.txt_TransactionStatusAfterOrderStatusDelayAfterFillMax			.Text = this.livesimBrokerSettings.TransactionStatusAfterOrderStatusDelayAfterFillMax		.ToString();

			this.txt_KillerTransactionCallbackAfterVictimFilled_happensMin		.Text = this.livesimBrokerSettings.KillerTransactionCallbackAfterVictimFilledHappensOncePerKillersMin	.ToString();
			this.txt_KillerTransactionCallbackAfterVictimFilled_happensMax		.Text = this.livesimBrokerSettings.KillerTransactionCallbackAfterVictimFilledHappensOncePerKillersMax	.ToString();
			this.txt_KillerTransactionCallbackAfterVictimFilled_delayMin		.Text = this.livesimBrokerSettings.KillerTransactionCallbackAfterVictimFilledDelayMin					.ToString();
			this.txt_KillerTransactionCallbackAfterVictimFilled_delayMax		.Text = this.livesimBrokerSettings.KillerTransactionCallbackAfterVictimFilledDelayMax					.ToString();


			this.txt_PartialFillHappensOncePerQuoteMin						.Text = this.livesimBrokerSettings.PartialFillHappensOncePerQuoteMin						.ToString();
			this.txt_PartialFillHappensOncePerQuoteMax						.Text = this.livesimBrokerSettings.PartialFillHappensOncePerQuoteMax						.ToString();
			this.txt_PartialFillPercentageFilledMin							.Text = this.livesimBrokerSettings.PartialFillPercentageFilledMin							.ToString();
			this.txt_PartialFillPercentageFilledMax							.Text = this.livesimBrokerSettings.PartialFillPercentageFilledMax							.ToString();

			this.txt_OutOfOrderFillHappensOncePerQuoteMin					.Text = this.livesimBrokerSettings.OutOfOrderFillHappensOncePerQuoteMin						.ToString();
			this.txt_OutOfOrderFillHappensOncePerQuoteMax					.Text = this.livesimBrokerSettings.OutOfOrderFillHappensOncePerQuoteMax						.ToString();
			this.txt_OutOfOrderFillDeliveredXordersLaterMin					.Text = this.livesimBrokerSettings.OutOfOrderFillDeliveredXordersLaterMin					.ToString();
			this.txt_OutOfOrderFillDeliveredXordersLaterMax					.Text = this.livesimBrokerSettings.OutOfOrderFillDeliveredXordersLaterMax					.ToString();

			this.txt_PriceDeviationForMarketOrdersHappensOncePerXordersMin	.Text = this.livesimBrokerSettings.PriceDeviationForMarketOrdersHappensOncePerXordersMin	.ToString();
			this.txt_PriceDeviationForMarketOrdersHappensOncePerXordersMax	.Text = this.livesimBrokerSettings.PriceDeviationForMarketOrdersHappensOncePerXordersMax	.ToString();
			this.txt_PriceDeviationForMarketOrdersPercentageOfBestPriceMin	.Text = this.livesimBrokerSettings.PriceDeviationForMarketOrdersPercentageOfBestPriceMin	.ToString();
			this.txt_PriceDeviationForMarketOrdersPercentageOfBestPriceMax	.Text = this.livesimBrokerSettings.PriceDeviationForMarketOrdersPercentageOfBestPriceMax	.ToString();

			this.txt_KillPendingDelay_min									.Text = this.livesimBrokerSettings.KillPendingDelayMillisMin								.ToString();
			this.txt_KillPendingDelay_max									.Text = this.livesimBrokerSettings.KillPendingDelayMillisMax								.ToString();

			this.txt_AdapterDisconnectHappensOncePerOrderMin				.Text = this.livesimBrokerSettings.AdapterDisconnectHappensOncePerOrderMin					.ToString();
			this.txt_AdapterDisconnectHappensOncePerOrderMax				.Text = this.livesimBrokerSettings.AdapterDisconnectHappensOncePerOrderMax					.ToString();
			this.txt_AdapterDisconnectReconnectsAfterMillisMin				.Text = this.livesimBrokerSettings.AdapterDisconnectReconnectsAfterMillisMin				.ToString();
			this.txt_AdapterDisconnectReconnectsAfterMillisMax				.Text = this.livesimBrokerSettings.AdapterDisconnectReconnectsAfterMillisMax				.ToString();
		}
	}
}