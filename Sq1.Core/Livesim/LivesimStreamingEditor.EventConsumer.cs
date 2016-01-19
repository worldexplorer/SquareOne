using System;
using System.Windows.Forms;
using System.Drawing;

namespace Sq1.Core.Livesim {
	public partial class LivesimStreamingEditor  {
		void anyCheckBox_CheckedChanged(object sender, EventArgs e) {
			CheckBox whatIchecked = sender as CheckBox;
			if (whatIchecked == null) {
				string msg = "HANDLER_WRONGLY_ATTACHED_SENDER_MUST_BE_A_CheckBox LivesimBrokerEditor.anyCheckBox_CheckedChanged(sender[" + sender.ToString() + "])";
				Assembler.PopupException(msg);
				return;
			}
			if (whatIchecked == this.cbx_DelayBetweenSerialQuotesEnabled) {
				this.livesimStreamingSettings.DelayBetweenSerialQuotesEnabled = whatIchecked.Checked;

			} else if (whatIchecked == this.cbx_OutOfOrderQuoteGenerationEnabled) {
				this.livesimStreamingSettings.OutOfOrderQuoteDeliveryEnabled = whatIchecked.Checked;

			} else if (whatIchecked == this.cbx_QuoteGenerationFreezeEnabled) {
				this.livesimStreamingSettings.QuoteGenerationFreezeEnabled = whatIchecked.Checked;

			} else if (whatIchecked == this.cbx_AdaperDisconnectEnabled) {
				this.livesimStreamingSettings.AdaperDisconnectEnabled = whatIchecked.Checked;

			} else {
				Assembler.PopupException("ADD_TARGET_FOR_CLICK_PROPAGATION_FOR_whatIchecked[" + whatIchecked.Text + "]/[" + whatIchecked.Name + "]");
				return;
			}
			this.livesimStreamingSettings.SaveStrategy();
		}

		void anyTextBox_Enter(object sender, EventArgs e) {
			TextBox whereIfocused = sender as TextBox;
			if (whereIfocused == null) {
				string msg = "HANDLER_WRONGLY_ATTACHED_SENDER_MUST_BE_A_TextBox LivesimBrokerEditor.anyTextBox_Enter(sender[" + sender.ToString() + "])";
				Assembler.PopupException(msg);
				return;
			}
			whereIfocused.SelectAll();		//WHY_SELECTALL_DOESNT_SELECT_ANYTHING???
		}
		void anyTextBox_KeyUp(object sender, KeyEventArgs e) {
			TextBox whereItyped = sender as TextBox;
			if (whereItyped == null) {
				string msg = "HANDLER_WRONGLY_ATTACHED_SENDER_MUST_BE_A_TextBox LivesimBrokerEditor.anyTextBox_KeyPress(sender[" + sender.ToString() + "])";
				Assembler.PopupException(msg);
				return;
			}

			decimal parsedDecimal;
			bool parsed = Decimal.TryParse(whereItyped.Text, out parsedDecimal);
			if (parsed == false && whereItyped.Text.Length > 0) {
				whereItyped.BackColor = Color.FromArgb(255, 150, 150);
				return;
			}
			whereItyped.BackColor = Color.White;
			int parsedInt = (int)Math.Abs(parsedDecimal);

			if (whereItyped == this.txt_DelayBetweenSerialQuotesMin) {
				this.livesimStreamingSettings.DelayBetweenSerialQuotesMin = parsedInt;

			} else if (whereItyped == this.txt_DelayBetweenSerialQuotesMax) {
				if (parsedInt <= 0) {
					parsedInt = 3;		// otherwize GUI becomes completely irresponsive; Application.DoEvents will unleash guiHasTime while it doesn't
					whereItyped.Text = parsedInt.ToString();
				}
				this.livesimStreamingSettings.DelayBetweenSerialQuotesMax = parsedInt;

			} else if (whereItyped == this.txt_OutOfOrderQuoteGenerationHappensOncePerQuoteMin) {
				this.livesimStreamingSettings.OutOfOrderQuoteGenerationHappensOncePerQuoteMin = parsedInt;

			} else if (whereItyped == this.txt_OutOfOrderQuoteGenerationHappensOncePerQuoteMax) {
				this.livesimStreamingSettings.OutOfOrderQuoteGenerationHappensOncePerQuoteMax = parsedInt;

			} else if (whereItyped == this.txt_OutOfOrderQuoteGenerationDelayMillisMin) {
				this.livesimStreamingSettings.OutOfOrderQuoteGenerationDelayMillisMin = parsedInt;

			} else if (whereItyped == this.txt_OutOfOrderQuoteGenerationDelayMillisMax) {
				this.livesimStreamingSettings.OutOfOrderQuoteGenerationDelayMillisMax = parsedInt;

			} else if (whereItyped == this.txt_QuoteGenerationFreezeHappensOncePerQuoteMin) {
				this.livesimStreamingSettings.QuoteGenerationFreezeHappensOncePerQuoteMin = parsedInt;

			} else if (whereItyped == this.txt_QuoteGenerationFreezeHappensOncePerQuoteMax) {
				this.livesimStreamingSettings.QuoteGenerationFreezeHappensOncePerQuoteMax = parsedInt;

			} else if (whereItyped == this.txt_QuoteGenerationFreezeMillisMin) {
				this.livesimStreamingSettings.QuoteGenerationFreezeMillisMin = parsedInt;

			} else if (whereItyped == this.txt_QuoteGenerationFreezeMillisMax) {
				this.livesimStreamingSettings.QuoteGenerationFreezeMillisMax = parsedInt;

			} else if (whereItyped == this.txt_AdaperDisconnectHappensOncePerQuoteMin) {
				this.livesimStreamingSettings.AdaperDisconnectHappensOncePerQuoteMin = parsedInt;

			} else if (whereItyped == this.txt_AdaperDisconnectHappensOncePerQuoteMax) {
				this.livesimStreamingSettings.AdaperDisconnectHappensOncePerQuoteMax = parsedInt;

			} else if (whereItyped == this.txt_AdaperDisconnectReconnectsAfterMillisMin) {
				this.livesimStreamingSettings.AdaperDisconnectReconnectsAfterMillisMin = parsedInt;

			} else if (whereItyped == this.txt_AdaperDisconnectReconnectsAfterMillisMax) {
				this.livesimStreamingSettings.AdaperDisconnectReconnectsAfterMillisMax = parsedInt;

			} else {
				Assembler.PopupException("ADD_TARGET_FOR_TYPING_PROPAGATION_FOR whereItyped[" + whereItyped.Text + "]/[" + whereItyped.Name + "]");
				return;
			}
			this.livesimStreamingSettings.SaveStrategy();
		}
	}
}
