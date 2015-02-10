using System;
using System.Windows.Forms;
using System.Drawing;

namespace Sq1.Core.Livesim {
	public partial class LivesimBrokerEditor {

		void anyCheckBox_CheckedChanged(object sender, EventArgs e) {
			CheckBox whatIchecked = sender as CheckBox;
			if (whatIchecked == null) {
				string msg = "HANDLER_WRONGLY_ATTACHED_SENDER_MUST_BE_A_CHECKBOX LivesimBrokerEditor.checkBox_CheckedChanged(sender[" + sender.ToString() + "])";
				Assembler.PopupException(msg);
				return;
			}
			if (whatIchecked == this.cbx_DelayBeforeFillEnabled) {
				this.livesimBrokerSettings.DelayBeforeFillEnabled = whatIchecked.Checked;

			} else if (whatIchecked == this.cbx_OrderRejectionEnabled) {
				this.livesimBrokerSettings.OrderRejectionEnabled = whatIchecked.Checked;

			} else if (whatIchecked == this.cbx_PartialFillEnabled) {
				this.livesimBrokerSettings.PartialFillEnabled = whatIchecked.Checked;

			} else if (whatIchecked == this.cbx_OutOfOrderFillEnabled) {
				this.livesimBrokerSettings.OutOfOrderFillEnabled = whatIchecked.Checked;

			} else if (whatIchecked == this.cbx_PriceDeviationForMarketOrdersEnabled) {
				this.livesimBrokerSettings.PriceDeviationForMarketOrdersEnabled = whatIchecked.Checked;

			} else if (whatIchecked == this.cbx_AdaperDisconnectEnabled) {
				this.livesimBrokerSettings.AdaperDisconnectEnabled = whatIchecked.Checked;

			} else {
				Assembler.PopupException("ADD_TARGET_FOR_CLICK_PROPAGATION_FOR_whatIchecked[" + whatIchecked.Name + "]");
			}
			this.livesimBrokerSettings.SaveStrategy();
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

			if (whereItyped == this.txt_DelayBeforeFillMillisMin) {
				this.livesimBrokerSettings.DelayBeforeFillMillisMin = parsedInt;

			} else if (whereItyped == this.txt_DelayBeforeFillMillisMax) {
				this.livesimBrokerSettings.DelayBeforeFillMillisMax = parsedInt;

			} else if (whereItyped == this.txt_OrderRejectionHappensOncePerXordersMin) {
				this.livesimBrokerSettings.OrderRejectionHappensOncePerXordersMin = parsedInt;

			} else if (whereItyped == this.txt_OrderRejectionHappensOncePerXordersMax) {
				this.livesimBrokerSettings.OrderRejectionHappensOncePerXordersMax = parsedInt;

			} else if (whereItyped == this.txt_PartialFillHappensOncePerQuoteMin) {
				this.livesimBrokerSettings.PartialFillHappensOncePerQuoteMin = parsedInt;

			} else if (whereItyped == this.txt_PartialFillHappensOncePerQuoteMax) {
				this.livesimBrokerSettings.PartialFillHappensOncePerQuoteMax = parsedInt;

			} else if (whereItyped == this.txt_PartialFillPercentageFilledMin) {
				this.livesimBrokerSettings.PartialFillPercentageFilledMin = parsedInt;

			} else if (whereItyped == this.txt_PartialFillPercentageFilledMax) {
				this.livesimBrokerSettings.PartialFillPercentageFilledMax = parsedInt;

			} else if (whereItyped == this.txt_OutOfOrderFillHappensOncePerQuoteMin) {
				this.livesimBrokerSettings.OutOfOrderFillHappensOncePerQuoteMin = parsedInt;

			} else if (whereItyped == this.txt_OutOfOrderFillHappensOncePerQuoteMax) {
				this.livesimBrokerSettings.OutOfOrderFillHappensOncePerQuoteMax = parsedInt;

			} else if (whereItyped == this.txt_OutOfOrderFillDeliveredXordersLaterMin) {
				this.livesimBrokerSettings.OutOfOrderFillDeliveredXordersLaterMin = parsedInt;

			} else if (whereItyped == this.txt_OutOfOrderFillDeliveredXordersLaterMax) {
				this.livesimBrokerSettings.OutOfOrderFillDeliveredXordersLaterMax = parsedInt;

			} else if (whereItyped == this.txt_PriceDeviationForMarketOrdersHappensOncePerXordersMin) {
				this.livesimBrokerSettings.PriceDeviationForMarketOrdersHappensOncePerXordersMin = parsedInt;

			} else if (whereItyped == this.txt_PriceDeviationForMarketOrdersHappensOncePerXordersMax) {
				this.livesimBrokerSettings.PriceDeviationForMarketOrdersHappensOncePerXordersMax = parsedInt;

			} else if (whereItyped == this.txt_PriceDeviationForMarketOrdersPercentageOfBestPriceMin) {
				this.livesimBrokerSettings.PriceDeviationForMarketOrdersPercentageOfBestPriceMin = parsedInt;

			} else if (whereItyped == this.txt_PriceDeviationForMarketOrdersPercentageOfBestPriceMax) {
				this.livesimBrokerSettings.PriceDeviationForMarketOrdersPercentageOfBestPriceMax = parsedInt;

			} else if (whereItyped == this.txt_AdaperDisconnectHappensOncePerQuoteMin) {
				this.livesimBrokerSettings.AdaperDisconnectHappensOncePerQuoteMin = parsedInt;

			} else if (whereItyped == this.txt_AdaperDisconnectHappensOncePerQuoteMax) {
				this.livesimBrokerSettings.AdaperDisconnectHappensOncePerQuoteMax = parsedInt;

			} else if (whereItyped == this.txt_AdaperDisconnectReconnectsAfterMillisMin) {
				this.livesimBrokerSettings.AdaperDisconnectReconnectsAfterMillisMin = parsedInt;

			} else if (whereItyped == this.txt_AdaperDisconnectReconnectsAfterMillisMax) {
				this.livesimBrokerSettings.AdaperDisconnectReconnectsAfterMillisMax = parsedInt;

			} else {
				Assembler.PopupException("ADD_TARGET_FOR_TYPING_PROPAGATION_FOR whereItyped[" + whereItyped.Text + "]/[" + whereItyped.Name + "]");
				return;
			}
			this.livesimBrokerSettings.SaveStrategy();
		}
	}
}