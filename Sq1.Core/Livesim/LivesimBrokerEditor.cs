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
			this.cbx_BrokerDeniedSubmission_Enabled			.Checked = this.livesimBrokerSettings.BrokerDeniedSubmission_injectionEnabled;
			this.cbx_ClearExecutionExceptions				.Checked = this.livesimBrokerSettings.ClearExecutionExceptions_beforeLivesim;
			this.cbx_NoOrderStateCallbackAfterSubmitted_HappensOncePer_Enabled.Checked = this.livesimBrokerSettings.NoOrderStateCallbackAfterSubmitted_Enabled;



			this.txt_DelayBeforeFillMillisMin								.Text = this.livesimBrokerSettings.DelayBeforeFillMillisMin									.ToString();
			this.txt_DelayBeforeFillMillisMax								.Text = this.livesimBrokerSettings.DelayBeforeFillMillisMax									.ToString();

			this.txt_OrderRejectionHappensOncePerXordersMin					.Text = this.livesimBrokerSettings.OrderRejectionHappensOncePerXordersMin					.ToString();
			this.txt_OrderRejectionHappensOncePerXordersMax					.Text = this.livesimBrokerSettings.OrderRejectionHappensOncePerXordersMax					.ToString();

			this.txt_TransactionStatusAfterOrderStatus_HappensOncePerOrders_Min	.Text = this.livesimBrokerSettings.TransactionStatusAfterOrderStatusHappensOncePerOrdersMin	.ToString();
			this.txt_TransactionStatusAfterOrderStatus_HappensOncePerOrders_Max	.Text = this.livesimBrokerSettings.TransactionStatusAfterOrderStatusHappensOncePerOrdersMax	.ToString();
			this.txt_TransactionStatusAfterOrderStatus_DelayAfterFill_Min			.Text = this.livesimBrokerSettings.TransactionStatusAfterOrderStatusDelayAfterFillMin		.ToString();
			this.txt_TransactionStatusAfterOrderStatus_DelayAfterFill_Max			.Text = this.livesimBrokerSettings.TransactionStatusAfterOrderStatusDelayAfterFillMax		.ToString();

			this.txt_KillerTransactionCallbackAfterVictimFilled_HappensOncePerOrders_Min		.Text = this.livesimBrokerSettings.KillerTransactionCallbackAfterVictimFilledHappensOncePerKillersMin	.ToString();
			this.txt_KillerTransactionCallbackAfterVictimFilled_HappensOncePerOrders_Max		.Text = this.livesimBrokerSettings.KillerTransactionCallbackAfterVictimFilledHappensOncePerKillersMax	.ToString();
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

			this.txt_PriceDeviationForMarketOrders_HappensOncePerXorders_Min	.Text = this.livesimBrokerSettings.PriceDeviationForMarketOrdersHappensOncePerXordersMin	.ToString();
			this.txt_PriceDeviationForMarketOrders_HappensOncePerXorders_Max	.Text = this.livesimBrokerSettings.PriceDeviationForMarketOrdersHappensOncePerXordersMax	.ToString();
			this.txt_PriceDeviationForMarketOrdersPercentageOfBestPrice_Min	.Text = this.livesimBrokerSettings.PriceDeviationForMarketOrdersPercentageOfBestPriceMin	.ToString();
			this.txt_PriceDeviationForMarketOrdersPercentageOfBestPrice_Max	.Text = this.livesimBrokerSettings.PriceDeviationForMarketOrdersPercentageOfBestPriceMax	.ToString();

			this.txt_KillPendingDelay_Min									.Text = this.livesimBrokerSettings.KillPendingDelayMillisMin								.ToString();
			this.txt_KillPendingDelay_Max									.Text = this.livesimBrokerSettings.KillPendingDelayMillisMax								.ToString();

			this.txt_AdapterDisconnect_HappensOncePerOrder_Min				.Text = this.livesimBrokerSettings.AdapterDisconnectHappensOncePerOrderMin					.ToString();
			this.txt_AdapterDisconnect_HappensOncePerOrder_Max				.Text = this.livesimBrokerSettings.AdapterDisconnectHappensOncePerOrderMax					.ToString();
			this.txt_AdapterDisconnectReconnectsAfterMillis_Min				.Text = this.livesimBrokerSettings.AdapterDisconnectReconnectsAfterMillisMin				.ToString();
			this.txt_AdapterDisconnectReconnectsAfterMillis_Max				.Text = this.livesimBrokerSettings.AdapterDisconnectReconnectsAfterMillisMax				.ToString();

			this.txt_BrokerDeniedSubmission_HappensOncePerXorders_Min		.Text = this.livesimBrokerSettings.BrokerDeniedSubmission_HappensOncePerXorders_Min			.ToString();
			this.txt_BrokerDeniedSubmission_HappensOncePerXorders_Max		.Text = this.livesimBrokerSettings.BrokerDeniedSubmission_HappensOncePerXorders_Max			.ToString();

			this.txt_NoOrderStateCallbackAfterSubmitted_HappensOncePer_Min	.Text = this.livesimBrokerSettings.NoOrderStateCallbackAfterSubmitted_HappensOncePerXorders_Min.ToString();
			this.txt_NoOrderStateCallbackAfterSubmitted_HappensOncePer_Max	.Text = this.livesimBrokerSettings.NoOrderStateCallbackAfterSubmitted_HappensOncePerXorders_Max.ToString();

		}
	}
}