using System;

namespace Sq1.Core.Livesim {
	public partial class LivesimStreamingEditor {
		LivesimStreamingSettings livesimStreamingSettings;
		public LivesimStreamingEditor() {
			this.InitializeComponent();
		}
		public void Initialize(LivesimStreamingSettings livesimStreamingSettings) {
			this.livesimStreamingSettings = livesimStreamingSettings;

			this.cbx_DelayBetweenSerialQuotesEnabled					.Checked = this.livesimStreamingSettings.DelayBetweenSerialQuotesEnabled;
			this.cbx_OutOfOrderQuoteGenerationEnabled					.Checked = this.livesimStreamingSettings.OutOfOrderQuoteDeliveryEnabled;
			this.cbx_QuoteGenerationFreezeEnabled						.Checked = this.livesimStreamingSettings.QuoteGenerationFreezeEnabled;
			this.cbx_AdaperDisconnectEnabled							.Checked = this.livesimStreamingSettings.AdaperDisconnectEnabled;

			this.txt_DelayBetweenSerialQuotesMin						.Text = this.livesimStreamingSettings.DelayBetweenSerialQuotesMin					.ToString();
			this.txt_DelayBetweenSerialQuotesMax						.Text = this.livesimStreamingSettings.DelayBetweenSerialQuotesMax					.ToString();
			this.txt_OutOfOrderQuoteGenerationHappensOncePerQuoteMin	.Text = this.livesimStreamingSettings.OutOfOrderQuoteGenerationHappensOncePerQuoteMin	.ToString();
			this.txt_OutOfOrderQuoteGenerationHappensOncePerQuoteMax	.Text = this.livesimStreamingSettings.OutOfOrderQuoteGenerationHappensOncePerQuoteMax	.ToString();
			this.txt_OutOfOrderQuoteGenerationDelayMillisMin			.Text = this.livesimStreamingSettings.OutOfOrderQuoteGenerationDelayMillisMin			.ToString();
			this.txt_OutOfOrderQuoteGenerationDelayMillisMax			.Text = this.livesimStreamingSettings.OutOfOrderQuoteGenerationDelayMillisMax			.ToString();
			this.txt_QuoteGenerationFreezeHappensOncePerQuoteMin		.Text = this.livesimStreamingSettings.QuoteGenerationFreezeHappensOncePerQuoteMin	.ToString();
			this.txt_QuoteGenerationFreezeHappensOncePerQuoteMax		.Text = this.livesimStreamingSettings.QuoteGenerationFreezeHappensOncePerQuoteMax	.ToString();
			this.txt_QuoteGenerationFreezeMillisMin						.Text = this.livesimStreamingSettings.QuoteGenerationFreezeMillisMin				.ToString();
			this.txt_QuoteGenerationFreezeMillisMax						.Text = this.livesimStreamingSettings.QuoteGenerationFreezeMillisMax				.ToString();
			this.txt_AdaperDisconnectHappensOncePerQuoteMin				.Text = this.livesimStreamingSettings.AdaperDisconnectHappensOncePerQuoteMin		.ToString();
			this.txt_AdaperDisconnectHappensOncePerQuoteMax				.Text = this.livesimStreamingSettings.AdaperDisconnectHappensOncePerQuoteMax		.ToString();
			this.txt_AdaperDisconnectReconnectsAfterMillisMin			.Text = this.livesimStreamingSettings.AdaperDisconnectReconnectsAfterMillisMin		.ToString();
			this.txt_AdaperDisconnectReconnectsAfterMillisMax			.Text = this.livesimStreamingSettings.AdaperDisconnectReconnectsAfterMillisMax		.ToString();
		}
	}
}
