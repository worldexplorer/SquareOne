using System;

using Sq1.Core.DataFeed;

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
			this.cbx_PartialFillEnabled						.Checked = this.livesimBrokerSettings.PartialFillEnabled;
			this.cbx_OutOfOrderFillEnabled					.Checked = this.livesimBrokerSettings.OutOfOrderFillEnabled;
			this.cbx_PriceDeviationForMarketOrdersEnabled	.Checked = this.livesimBrokerSettings.PriceDeviationForMarketOrdersEnabled;
			this.cbx_AdaperDisconnectEnabled				.Checked = this.livesimBrokerSettings.AdaperDisconnectEnabled;

			this.txt_DelayBeforeFillMillisMin								.Text = this.livesimBrokerSettings.DelayBeforeFillMillisMin									.ToString();
			this.txt_DelayBeforeFillMillisMax								.Text = this.livesimBrokerSettings.DelayBeforeFillMillisMax									.ToString();
			this.txt_OrderRejectionHappensOncePerXordersMin					.Text = this.livesimBrokerSettings.OrderRejectionHappensOncePerXordersMin					.ToString();
			this.txt_OrderRejectionHappensOncePerXordersMax					.Text = this.livesimBrokerSettings.OrderRejectionHappensOncePerXordersMax					.ToString();
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
			this.txt_AdaperDisconnectHappensOncePerQuoteMin					.Text = this.livesimBrokerSettings.AdaperDisconnectHappensOncePerQuoteMin					.ToString();
			this.txt_AdaperDisconnectHappensOncePerQuoteMax					.Text = this.livesimBrokerSettings.AdaperDisconnectHappensOncePerQuoteMax					.ToString();
			this.txt_AdaperDisconnectReconnectsAfterMillisMin				.Text = this.livesimBrokerSettings.AdaperDisconnectReconnectsAfterMillisMin					.ToString();
			this.txt_AdaperDisconnectReconnectsAfterMillisMax				.Text = this.livesimBrokerSettings.AdaperDisconnectReconnectsAfterMillisMax					.ToString();
		}
	}
}