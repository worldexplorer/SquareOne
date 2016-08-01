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
				this.livesimBrokerSettings.ClearExecutionExceptions_beforeLivesim = whatIchecked.Checked;

			} else if (whatIchecked == this.cbx_BrokerDeniedSubmission_Enabled) {
				this.livesimBrokerSettings.BrokerDeniedSubmission_injectionEnabled = whatIchecked.Checked;

			} else if (whatIchecked == this.cbx_NoOrderStateCallbackAfterSubmitted_HappensOncePer_Enabled) {
				this.livesimBrokerSettings.NoOrderStateCallbackAfterSubmitted_Enabled = whatIchecked.Checked;

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
			} else if (whereItyped == this.txt_TransactionStatusAfterOrderStatus_HappensOncePerOrders_Min) {
				this.livesimBrokerSettings.TransactionStatusAfterOrderStatusHappensOncePerOrdersMin = parsedInt;

			} else if (whereItyped == this.txt_TransactionStatusAfterOrderStatus_HappensOncePerOrders_Max) {
				this.livesimBrokerSettings.TransactionStatusAfterOrderStatusHappensOncePerOrdersMax = parsedInt;
			
			} else if (whereItyped == this.txt_TransactionStatusAfterOrderStatus_DelayAfterFill_Min) {
				this.livesimBrokerSettings.TransactionStatusAfterOrderStatusDelayAfterFillMin = parsedInt;

			} else if (whereItyped == this.txt_TransactionStatusAfterOrderStatus_DelayAfterFill_Max) {
				this.livesimBrokerSettings.TransactionStatusAfterOrderStatusDelayAfterFillMax = parsedInt;

			
			// QUIK sending "Killer Transaction SUCCESS" after "OrderStatus filled" - simulating this async
			} else if (whereItyped == this.txt_KillerTransactionCallbackAfterVictimFilled_HappensOncePerOrders_Min) {
				this.livesimBrokerSettings.KillerTransactionCallbackAfterVictimFilledHappensOncePerKillersMin = parsedInt;

			} else if (whereItyped == this.txt_KillerTransactionCallbackAfterVictimFilled_HappensOncePerOrders_Max) {
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


			} else if (whereItyped == this.txt_PriceDeviationForMarketOrders_HappensOncePerXorders_Min) {
				this.livesimBrokerSettings.PriceDeviationForMarketOrdersHappensOncePerXordersMin = parsedInt;

			} else if (whereItyped == this.txt_PriceDeviationForMarketOrders_HappensOncePerXorders_Max) {
				this.livesimBrokerSettings.PriceDeviationForMarketOrdersHappensOncePerXordersMax = parsedInt;

			} else if (whereItyped == this.txt_PriceDeviationForMarketOrdersPercentageOfBestPrice_Min) {
				this.livesimBrokerSettings.PriceDeviationForMarketOrdersPercentageOfBestPriceMin = parsedInt;

			} else if (whereItyped == this.txt_PriceDeviationForMarketOrdersPercentageOfBestPrice_Max) {
				this.livesimBrokerSettings.PriceDeviationForMarketOrdersPercentageOfBestPriceMax = parsedInt;


			} else if (whereItyped == this.txt_KillPendingDelay_Min) {
				this.livesimBrokerSettings.KillPendingDelayMillisMin = parsedInt;

			} else if (whereItyped == this.txt_KillPendingDelay_Max) {
				this.livesimBrokerSettings.KillPendingDelayMillisMax = parsedInt;


			} else if (whereItyped == this.txt_AdapterDisconnect_HappensOncePerOrder_Min) {
				this.livesimBrokerSettings.AdapterDisconnectHappensOncePerOrderMin = parsedInt;

			} else if (whereItyped == this.txt_AdapterDisconnect_HappensOncePerOrder_Max) {
				this.livesimBrokerSettings.AdapterDisconnectHappensOncePerOrderMax = parsedInt;

			} else if (whereItyped == this.txt_AdapterDisconnectReconnectsAfterMillis_Min) {
				this.livesimBrokerSettings.AdapterDisconnectReconnectsAfterMillisMin = parsedInt;

			} else if (whereItyped == this.txt_AdapterDisconnectReconnectsAfterMillis_Max) {
				this.livesimBrokerSettings.AdapterDisconnectReconnectsAfterMillisMax = parsedInt;


			} else if (whereItyped == this.txt_BrokerDeniedSubmission_HappensOncePerXorders_Min) {
				this.livesimBrokerSettings.BrokerDeniedSubmission_HappensOncePerXorders_Min = parsedInt;

			} else if (whereItyped == this.txt_BrokerDeniedSubmission_HappensOncePerXorders_Max) {
				this.livesimBrokerSettings.BrokerDeniedSubmission_HappensOncePerXorders_Max = parsedInt;


			} else if (whereItyped == this.txt_NoOrderStateCallbackAfterSubmitted_HappensOncePer_Min) {
				this.livesimBrokerSettings.NoOrderStateCallbackAfterSubmitted_HappensOncePerXorders_Min = parsedInt;

			} else if (whereItyped == this.txt_NoOrderStateCallbackAfterSubmitted_HappensOncePer_Max) {
				this.livesimBrokerSettings.NoOrderStateCallbackAfterSubmitted_HappensOncePerXorders_Max = parsedInt;


			} else {
				Assembler.PopupException("ADD_TARGET_FOR_TYPING_PROPAGATION_FOR whereItyped[" + whereItyped.Text + "]/[" + whereItyped.Name + "]");
				return;
			}
			this.livesimBrokerSettings.SaveStrategy();
		}
	}
}