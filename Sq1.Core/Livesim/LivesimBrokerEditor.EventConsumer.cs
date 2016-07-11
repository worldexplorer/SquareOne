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

			} else if (whatIchecked == this.cbx_TransactionStatusAfterOrderStatusEnabled) {
				this.livesimBrokerSettings.TransactionStatusAfterOrderStatusEnabled = whatIchecked.Checked;

			} else if (whatIchecked == this.cbx_KillerTransactionCallbackAfterVictimFilled_enabled) {
				this.livesimBrokerSettings.KillerTransactionCallbackAfterVictimFilledEnabled = whatIchecked.Checked;

			} else if (whatIchecked == this.cbx_PartialFillEnabled) {
				this.livesimBrokerSettings.PartialFillEnabled = whatIchecked.Checked;

			} else if (whatIchecked == this.cbx_OutOfOrderFillEnabled) {
				this.livesimBrokerSettings.OutOfOrderFillEnabled = whatIchecked.Checked;

			} else if (whatIchecked == this.cbx_PriceDeviationForMarketOrdersEnabled) {
				this.livesimBrokerSettings.PriceDeviationForMarketOrdersEnabled = whatIchecked.Checked;

			} else if (whatIchecked == this.cbx_KillPendingDelayEnabled) {
				this.livesimBrokerSettings.KillPendingDelayEnabled = whatIchecked.Checked;

			} else if (whatIchecked == this.cbx_AdaperDisconnectEnabled) {
				this.livesimBrokerSettings.AdapterDisconnectEnabled = whatIchecked.Checked;

			} else if (whatIchecked == this.cbx_ClearExecutionExceptions) {
				this.livesimBrokerSettings.ClearExecutionExceptions = whatIchecked.Checked;

			} else if (whatIchecked == this.cbx_OrderSickEnabled) {
				this.livesimBrokerSettings.OrderSickEnabled = whatIchecked.Checked;

			} else {
				Assembler.PopupException("ADD_TARGET_FOR_CLICK_PROPAGATION_FOR_whatIchecked[" + whatIchecked.Name + "]");
			}
			if (Assembler.InstanceInitialized.MainForm_dockFormsFullyDeserialized_layoutComplete == false) return;
			this.livesimBrokerSettings.SaveStrategy();
		}

		//void anyTextBox_Enter(object sender, EventArgs e) {
		//    TextBox whereIfocused = sender as TextBox;
		//    if (whereIfocused == null) {
		//        string msg = "HANDLER_WRONGLY_ATTACHED_SENDER_MUST_BE_A_TextBox LivesimBrokerEditor.anyTextBox_Enter(sender[" + sender.ToString() + "])";
		//        Assembler.PopupException(msg);
		//        return;
		//    }
		//    whereIfocused.SelectAll();		//WHY_SELECTALL_DOESNT_SELECT_ANYTHING???
		//}
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


			// QUIK sending "OrderFilled Transaction SUCCESS" after "OrderStatus filled" - simulating this async
			} else if (whereItyped == this.txt_TransactionStatusAfterOrderStatusHappensOncePerOrdersMin) {
				this.livesimBrokerSettings.TransactionStatusAfterOrderStatusHappensOncePerOrdersMin = parsedInt;

			} else if (whereItyped == this.txt_TransactionStatusAfterOrderStatusHappensOncePerOrdersMax) {
				this.livesimBrokerSettings.TransactionStatusAfterOrderStatusHappensOncePerOrdersMax = parsedInt;
			
			} else if (whereItyped == this.txt_TransactionStatusAfterOrderStatusDelayAfterFillMin) {
				this.livesimBrokerSettings.TransactionStatusAfterOrderStatusDelayAfterFillMin = parsedInt;

			} else if (whereItyped == this.txt_TransactionStatusAfterOrderStatusDelayAfterFillMax) {
				this.livesimBrokerSettings.TransactionStatusAfterOrderStatusDelayAfterFillMax = parsedInt;

			
			// QUIK sending "Killer Transaction SUCCESS" after "OrderStatus filled" - simulating this async
			} else if (whereItyped == this.txt_KillerTransactionCallbackAfterVictimFilled_happensMin) {
				this.livesimBrokerSettings.KillerTransactionCallbackAfterVictimFilledHappensOncePerKillersMin = parsedInt;

			} else if (whereItyped == this.txt_KillerTransactionCallbackAfterVictimFilled_happensMax) {
				this.livesimBrokerSettings.KillerTransactionCallbackAfterVictimFilledHappensOncePerKillersMax = parsedInt;
			
			} else if (whereItyped == this.txt_KillerTransactionCallbackAfterVictimFilled_delayMin) {
				this.livesimBrokerSettings.KillerTransactionCallbackAfterVictimFilledDelayMin = parsedInt;

			} else if (whereItyped == this.txt_KillerTransactionCallbackAfterVictimFilled_delayMax) {
				this.livesimBrokerSettings.KillerTransactionCallbackAfterVictimFilledDelayMax = parsedInt;


			
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


			} else if (whereItyped == this.txt_KillPendingDelay_min) {
				this.livesimBrokerSettings.KillPendingDelayMillisMin = parsedInt;

			} else if (whereItyped == this.txt_KillPendingDelay_max) {
				this.livesimBrokerSettings.KillPendingDelayMillisMax = parsedInt;


			} else if (whereItyped == this.txt_AdapterDisconnectHappensOncePerOrderMin) {
				this.livesimBrokerSettings.AdapterDisconnectHappensOncePerOrderMin = parsedInt;

			} else if (whereItyped == this.txt_AdapterDisconnectHappensOncePerOrderMax) {
				this.livesimBrokerSettings.AdapterDisconnectHappensOncePerOrderMax = parsedInt;

			} else if (whereItyped == this.txt_AdapterDisconnectReconnectsAfterMillisMin) {
				this.livesimBrokerSettings.AdapterDisconnectReconnectsAfterMillisMin = parsedInt;

			} else if (whereItyped == this.txt_AdapterDisconnectReconnectsAfterMillisMax) {
				this.livesimBrokerSettings.AdapterDisconnectReconnectsAfterMillisMax = parsedInt;


			} else if (whereItyped == this.txt_OrderSickHappensOncePerXordersMin) {
				this.livesimBrokerSettings.OrderSickHappensOncePerXordersMin = parsedInt;

			} else if (whereItyped == this.txt_OrderSickHappensOncePerXordersMax) {
				this.livesimBrokerSettings.OrderSickHappensOncePerXordersMax = parsedInt;


			} else {
				Assembler.PopupException("ADD_TARGET_FOR_TYPING_PROPAGATION_FOR whereItyped[" + whereItyped.Text + "]/[" + whereItyped.Name + "]");
				return;
			}
			this.livesimBrokerSettings.SaveStrategy();
		}
	}
}