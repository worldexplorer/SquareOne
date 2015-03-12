using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Core.DataTypes;
using Sq1.Core.Execution;
using Sq1.Core.StrategyBase;
using Sq1.Widgets.LabeledTextBox;
using Sq1.Widgets;

namespace Sq1.Gui.Forms {
	public partial class ChartForm {
		void ctxChart_Opening(object sender, CancelEventArgs e) {
			if (this.MniShowSourceCodeEditor.Enabled == false) return;	// don't show ScriptEditor for Strategy.ActivatedFromDll
			this.MniShowSourceCodeEditor.Checked = this.ChartFormManager.ScriptEditorIsOnSurface; 
			//this.editBarToolStripMenuItem.Enabled = this.AllowEditBarData;
			//_preCalcPricesHandleTradeMenuItemsGuiThread();
		}
		void ctxStrategy_Opening(object sender, CancelEventArgs e) {
			this.MniShowLivesim		.Checked = this.ChartFormManager.LivesimFormIsOnSurface;
			this.MniShowOptimizer	.Checked = this.ChartFormManager.OptimizerIsOnSurface;
			if (this.MniShowSourceCodeEditor.Enabled == false) return;	// don't show ScriptEditor for Strategy.ActivatedFromDll
			this.MniShowSourceCodeEditor.Checked = this.ChartFormManager.ScriptEditorIsOnSurface; 
		}
		void ctxBacktest_Opening(object sender, CancelEventArgs e) {
		}

		void mniShowSourceCodeEditor_Click(object sender, System.EventArgs e) {
			this.ctxStrategy.Visible = true;
			if (this.MniShowSourceCodeEditor.Checked) {
				// if autohidden => popup and keepAutoHidden=false
				this.ChartFormManager.EditorFormShow(false);
				this.ChartFormManager.MainForm.MainFormSerialize();
			} else {
				//v1 this.ChartFormManager.ScriptEditorFormConditionalInstance.ToggleAutoHide();
				if (DockContentImproved.IsNullOrDisposed(this.ChartFormManager.ScriptEditorForm)) {
					string msg = "YOU_DIDNT_SYNC_MNI_TICK=OFF_WHEN_SCRIPT_EDITOR_FORM_WAS_CLOSED_BY_X";
					Assembler.PopupException(msg);
				} else {
					this.ChartFormManager.ScriptEditorForm.Close();
				}
			}
			// DUPLICATE_XML_SERIALIZATION_AFTER ScriptEditorForm.OnFormClosed()
			//this.ChartFormManager.MainForm.MainFormSerialize();
		}
		void mniShowOptimizer_Click(object sender, System.EventArgs e) {
			this.ctxStrategy.Visible = true;
			if (this.MniShowOptimizer.Checked == false) {
				this.MniShowOptimizer.Checked = true;
				// if autohidden => popup and keepAutoHidden=false
				this.ChartFormManager.OptimizerFormShow(false);
				this.ChartFormManager.MainForm.MainFormSerialize();
			} else {
				this.MniShowOptimizer.Checked = false;
				//v1 this.ChartFormManager.OptimizerFormConditionalInstance.ToggleAutoHide();
				if (DockContentImproved.IsNullOrDisposed(this.ChartFormManager.OptimizerForm)) {
					string msg = "CHECK_ON_CLICK_WILL_SET_CHECKED_AFTER_THIS_HANDLER_EXITS YOU_DIDNT_SYNC_MNI_TICK=OFF_WHEN_OPTIMIZER_FORM_WAS_CLOSED_BY_X";
					//Assembler.PopupException(msg);
				} else {
					this.ChartFormManager.OptimizerForm.Close();
				}
			}
			// DUPLICATE_XML_SERIALIZATION_AFTER OptimizerForm.OnFormClosed()
			//this.ChartFormManager.MainForm.MainFormSerialize();
		}
		void mniShowLivesim_Click(object sender, EventArgs e) {
			this.ctxStrategy.Visible = true;
			if (this.MniShowLivesim.Checked == false) {
				this.MniShowLivesim.Checked = true;
				// if autohidden => popup and keepAutoHidden=false
				this.ChartFormManager.LivesimFormShow(false);
				this.ChartFormManager.MainForm.MainFormSerialize();
			} else {
				this.MniShowLivesim.Checked = false;
				//this.ChartFormManager.LivesimFormConditionalInstance.Visible = true;
				//this.ChartFormManager.LivesimFormShow(false);
				//v1 this.ChartFormManager.LivesimFormConditionalInstance.ToggleAutoHide();
				if (DockContentImproved.IsNullOrDisposed(this.ChartFormManager.LivesimForm)) {
					string msg = "YOU_DIDNT_SYNC_MNI_TICK=OFF_WHEN_LIVESIM_FORM_WAS_CLOSED_BY_X";
					Assembler.PopupException(msg);
				} else {
					this.ChartFormManager.LivesimForm.Close();
				}
			}
			// DUPLICATE_XML_SERIALIZATION_AFTER LivesimForm.OnFormClosed()
			//this.ChartFormManager.MainForm.MainFormSerialize();
		}
		void btnStreamingWillTriggerScript_Click(object sender, EventArgs e) {
			// ToolStripButton pre-toggles itself when ChartForm{Properties}.BtnStreaming.CheckOnClick=True this.BtnStreaming.Checked = !this.BtnStreaming.Checked;
			try {
				if (this.btnStreamingTriggersScript.Checked) {
					this.ChartFormManager.ChartStreamingConsumer.StreamingTriggeringScriptStart();
					// same idea as in mniSubscribedToStreamingAdapterQuotesBars_Click();
					ContextChart ctxChart = this.ChartFormManager.ContextCurrentChartOrStrategy;
					if (this.ChartFormManager.Executor.Strategy.Script != null && ctxChart.IsStreamingTriggeringScript) {
						this.ChartFormManager.BacktesterRunSimulation();
					}
				} else {
					this.ChartFormManager.ChartStreamingConsumer.StreamingTriggeringScriptStop();
				}
				this.PopulateBtnStreamingTriggersScriptAfterBarsLoaded();
				this.ChartFormManager.Strategy.Serialize();
				//WHO_ELSE_NEEDS_IT? this.RaiseStreamingButtonStateChanged();
				this.PropagateSelectorsDisabledIfStreamingForCurrentChart();
			} catch (Exception ex) {
				Assembler.PopupException(ex.Message);
			}
		}
		void btnStrategyEmittingOrders_Click(object sender, EventArgs e) {
			// ToolStripButton pre-toggles itself when ChartForm{Properties}.BtnAutoSubmit.CheckOnClick=True this.BtnAutoSubmit.Checked = !this.BtnAutoSubmit.Checked;;
			this.ChartFormManager.Executor.IsStrategyEmittingOrders = this.btnStrategyEmittingOrders.Checked;
			this.ChartFormManager.Strategy.Serialize();
		}
		void mniBacktestOnEveryChange_Click(object sender, System.EventArgs e) {
			try {
				this.ctxBacktest.Visible = true;
				Strategy strategy = this.ChartFormManager.Executor.Strategy;
				if (strategy == null) return;
				strategy.ScriptContextCurrent.BacktestOnSelectorsChange = this.mniBacktestOnSelectorsChange.Checked;
				strategy.ScriptContextCurrent.BacktestOnDataSourceSaved = this.mniBacktestOnDataSourceSaved.Checked;
				strategy.Serialize();
			} catch (Exception ex) {
				Assembler.PopupException("mniBacktestOnEveryChange_Click()", ex);
			}
		}
		void mniBacktestOnRestart_Click(object sender, System.EventArgs e) {
			try {
				this.ctxBacktest.Visible = true;
				Strategy strategy = this.ChartFormManager.Executor.Strategy;
				if (strategy == null) return;
				strategy.ScriptContextCurrent.BacktestOnRestart = this.mniBacktestOnRestart.Checked;
				strategy.Serialize();
			} catch (Exception ex) {
				Assembler.PopupException("mniBacktestOnRestart_Click()", ex);
			}
		}
		void mniBacktestNow_Click(object sender, System.EventArgs e) {
			try {
				// AFTER_F5_PANEL_SHOWS_UP_OUTSIDE_APP_WINDOW this.ctxBacktest.Visible = true;
				this.ChartFormManager.BacktesterRunSimulation();
			} catch (Exception ex) {
				Assembler.PopupException("mniBacktestNow_Click()", ex);
			}
		}
		void chartForm_Closed(object sender, FormClosedEventArgs e) {
			if (Assembler.InstanceInitialized.MainFormClosingIgnoreReLayoutDockedForms) {
				return;
			}
			this.ChartFormManager.DataSnapshotSerializer.DeleteJsonFile();
		}
		void mnitlbAll_UserTyped(object sender, LabeledTextBoxUserTypedArgs e) {
			this.ctxBars.Visible = true;
			try {
				string userTyped = e.StringUserTyped;
				int userTypedInteger;
				bool validInteger = Int32.TryParse(userTyped, out userTypedInteger);
				if (validInteger == false) {
					e.HighlightTextWithRed = true;
					return;
				}
				if (userTypedInteger <= 0) {
					e.HighlightTextWithRed = true;
					return;
				}
				MenuItemLabeledTextBox mniTypedAsLTB = sender as MenuItemLabeledTextBox;
				if (mniTypedAsLTB == null) {
					string msg = "SENDER_MUSTBE_LabeledTextBoxControl_GOT " + mniTypedAsLTB.GetType();
					Assembler.PopupException(msg);
					return;
				}

				BarScale barScaleTyped;
				switch (mniTypedAsLTB.Name) {
					case "mnitlbMinutes":		barScaleTyped = BarScale.Minute;		break;
					case "mnitlbHourly":		barScaleTyped = BarScale.Hour;			break;
					case "mnitlbDaily":			barScaleTyped = BarScale.Daily;			break;
					case "mnitlbWeekly":		barScaleTyped = BarScale.Weekly;		break;
					case "mnitlbMonthly":		barScaleTyped = BarScale.Monthly;		break;
					//case "mnitlbQuarterly":		barScaleTyped = BarScale.Quarterly;		break;
					case "mnitlbYearly":		barScaleTyped = BarScale.Yearly;		break;
					default:
						string msg = "SENDER_UNEXPECTED_NAME " + mniTypedAsLTB.Name;
						Assembler.PopupException(msg);
						return;
				}
				
				this.selectOneDeselectResetOthers(this.DdbBars.DropDownItems, mniTypedAsLTB, this.GroupScaleLabeledTextboxes);
				
				BarScaleInterval scaleIntervalUserEntered = new BarScaleInterval(barScaleTyped, userTypedInteger);
				ContextChart context = this.ChartFormManager.ContextCurrentChartOrStrategy;
				context.ScaleInterval = scaleIntervalUserEntered;
				
				this.ChartFormManager.PopulateSelectorsFromCurrentChartOrScriptContextLoadBarsSaveBacktestIfStrategy("mniltbAll_UserTyped");
				this.ChartFormManager.OptimizerFormIfOpenPropagateTextboxesOrMarkStaleResultsAndDeleteHistory();
			} catch (Exception ex) {
				Assembler.PopupException("mniltbMinutes_UserTyped()", ex);
			}
		}
		void selectOneDeselectResetOthers(ToolStripItemCollection allDropDownItems, object sender, List<string> groupLabeledTextboxes) {
			MenuItemLabeledTextBox mniltbToSelect = sender as MenuItemLabeledTextBox;
			if (mniltbToSelect == null) {
				string msg = "SENDER_MUSTBE_LabeledTextBoxControl_GOT " + mniltbToSelect.GetType();
				Assembler.PopupException(msg);
				return;
			}
			
			foreach (ToolStripItem eachMni in allDropDownItems) {
				MenuItemLabeledTextBox eachMniAsLTB = eachMni as MenuItemLabeledTextBox;
				if (eachMniAsLTB == null) continue;
				if (eachMniAsLTB == mniltbToSelect) continue;
				if (groupLabeledTextboxes.Contains(eachMniAsLTB.Name) == false) continue;
				eachMniAsLTB.InputFieldValue = "";
				//eachMniAsLTB.Selected = false;
				eachMniAsLTB.BackColor = Color.White;
				//eachMniAsLTB.ForeColor = Color.Black;
			}
			//mniltbNameToSelect.Selected = true;
			mniltbToSelect.BackColor = Color.Gainsboro;
			//mniltbNameToSelect.ForeColor = Color.White;
		}
		
		void mnitlbShowLastBars_UserTyped(object sender, LabeledTextBoxUserTypedArgs e) {
			this.ctxBars.Visible = true;
			try {
				string userTyped = e.StringUserTyped;
				int userTypedInteger;
				bool validInteger = Int32.TryParse(userTyped, out userTypedInteger);
				if (validInteger == false) {
					e.HighlightTextWithRed = true;
					return;
				}
				if (userTypedInteger < 0) {
					e.HighlightTextWithRed = true;
					return;
				}
				
				ContextChart context = this.ChartFormManager.ContextCurrentChartOrStrategy;
				context.DataRange = (userTypedInteger == 0) ? new BarDataRange() : new BarDataRange(userTypedInteger);

				this.mnitlbShowLastBars.BackColor = Color.Gainsboro;
				this.mniShowBarRange.Checked = false;
				this.mniShowBarRange_Click(sender, null);

				this.ChartFormManager.PopulateSelectorsFromCurrentChartOrScriptContextLoadBarsSaveBacktestIfStrategy("mnitlbShowLastBars_UserTyped");
				this.ChartFormManager.OptimizerFormIfOpenPropagateTextboxesOrMarkStaleResultsAndDeleteHistory();
			} catch (Exception ex) {
				Assembler.PopupException("mnitlbShowLastBars_UserTyped()", ex);
			}
		}
		void mniShowBarRange_Click(object sender, EventArgs e) {
			this.ctxBars.Visible = true;
			try {
				if (e != null) {
					this.mnitlbShowLastBars.BackColor = Color.White;
					this.mnitlbShowLastBars.InputFieldValue = "";
				}
				
				this.ChartControl.RangeBarCollapsed = !this.mniShowBarRange.Checked; 
				if (this.ChartFormManager.Strategy != null) {
					this.ChartFormManager.Strategy.ScriptContextCurrent.ShowRangeBar = this.mniShowBarRange.Checked;
					this.ChartFormManager.Strategy.Serialize();
				} else {
					this.ChartFormManager.DataSnapshot.ContextChart.ShowRangeBar = this.mniShowBarRange.Checked;
					this.ChartFormManager.DataSnapshotSerializer.Serialize();
				}
			} catch (Exception ex) {
				Assembler.PopupException("mniShowBarRange_Click()", ex);
			}
		}
		void mnitlbPositionSizeSharesConstantEachTrade_UserTyped(object sender, LabeledTextBoxUserTypedArgs e) {
			this.ctxBacktest.Visible = true;
			try {
				string userTyped = e.StringUserTyped;
				double userTypedDouble;
				bool validInteger = Double.TryParse(userTyped, out userTypedDouble);
				if (validInteger == false) {
					e.HighlightTextWithRed = true;
					return;
				}
				if (userTypedDouble <= 0) {
					e.HighlightTextWithRed = true;
					return;
				}
				
				ContextScript context = this.ChartFormManager.Strategy.ScriptContextCurrent;
				context.PositionSize = new PositionSize(PositionSizeMode.SharesConstantEachTrade, userTypedDouble);

				this.selectOneDeselectResetOthers(this.DdbBacktest.DropDownItems, sender, this.GroupPositionSizeLabeledTextboxes);

				this.ChartFormManager.PopulateSelectorsFromCurrentChartOrScriptContextLoadBarsSaveBacktestIfStrategy("mnitlbPositionSizeSharesConstant_UserTyped");
				this.ChartFormManager.OptimizerFormIfOpenPropagateTextboxesOrMarkStaleResultsAndDeleteHistory();
			} catch (Exception ex) {
				Assembler.PopupException("mnitlbPositionSizeSharesConstant_UserTyped()", ex);
			}
		}
		void mnitlbPositionSizeDollarsConstantEachTrade_UserTyped(object sender, LabeledTextBoxUserTypedArgs e) {
			try {
				string userTyped = e.StringUserTyped;
				double userTypedDouble;
				bool validInteger = Double.TryParse(userTyped, out userTypedDouble);
				if (validInteger == false) {
					e.HighlightTextWithRed = true;
					return;
				}
				if (userTypedDouble <= 0) {
					e.HighlightTextWithRed = true;
					return;
				}
				
				ContextScript context = this.ChartFormManager.Strategy.ScriptContextCurrent;
				context.PositionSize = new PositionSize(PositionSizeMode.DollarsConstantForEachTrade, userTypedDouble);

				this.selectOneDeselectResetOthers(this.DdbBacktest.DropDownItems, sender, this.GroupPositionSizeLabeledTextboxes);

				this.ChartFormManager.PopulateSelectorsFromCurrentChartOrScriptContextLoadBarsSaveBacktestIfStrategy("mnitlbPositionSizeDollarsEachTradeConstant_UserTyped");
				this.ChartFormManager.OptimizerFormIfOpenPropagateTextboxesOrMarkStaleResultsAndDeleteHistory();
			} catch (Exception ex) {
				Assembler.PopupException("mnitlbPositionSizeDollarsEachTradeConstant_UserTyped()", ex);
			}
		}
		void mniOutsideQuoteFillCheckThrow_Click(object sender, EventArgs e) {
			this.ctxBacktest.Visible = true;
			ContextScript context = this.ChartFormManager.Strategy.ScriptContextCurrent;
			context.FillOutsideQuoteSpreadParanoidCheckThrow = this.mniFillOutsideQuoteSpreadParanoidCheckThrow.Checked;
			this.ChartFormManager.Strategy.Serialize();
			this.ChartFormManager.PopulateSelectorsFromCurrentChartOrScriptContextLoadBarsSaveBacktestIfStrategy("mniOutOfQuoteFillThrow_Click");
		}
		void mnitlbSpreadGeneratorPct_UserTyped(object sender, LabeledTextBoxUserTypedArgs e) {
			this.ctxBacktest.Visible = true;
			try {
				string userTyped = e.StringUserTyped;
				double userTypedDouble;
				bool validInteger = Double.TryParse(userTyped, out userTypedDouble);
				if (validInteger == false) {
					e.HighlightTextWithRed = true;
					return;
				}
				if (userTypedDouble <= 0) {
					e.HighlightTextWithRed = true;
					return;
				}
				
				ContextScript context = this.ChartFormManager.Strategy.ScriptContextCurrent;
				context.SpreadModelerPercent = userTypedDouble;
				
				if (this.ChartFormManager.Executor.Backtester.BacktestDataSource == null) {
					this.ChartFormManager.Executor.Backtester.InitializeQuoteGenerator();
				}

				this.ChartFormManager.PopulateSelectorsFromCurrentChartOrScriptContextLoadBarsSaveBacktestIfStrategy("mnitlbSpreadGeneratorPct_UserTyped");
				this.ChartFormManager.OptimizerFormIfOpenPropagateTextboxesOrMarkStaleResultsAndDeleteHistory();
			} catch (Exception ex) {
				Assembler.PopupException("mnitlbSpreadGeneratorPct_UserTyped()", ex);
			}
		}
		void mniSubscribedToStreamingAdapterQuotesBars_Click(object sender, EventArgs e) {
			this.ctxBars.Visible = true;
			try {
				if (this.mniSubscribedToStreamingAdapterQuotesBars.Checked == false) {
					this.mniSubscribedToStreamingAdapterQuotesBars.BackColor = Color.LightSalmon;
					this.DdbBars.BackColor = Color.LightSalmon;
					this.mniSubscribedToStreamingAdapterQuotesBars.Text = "NOT Subscribed to [" + this.ChartFormManager.Executor.DataSource.StreamingAdapterName + "]";
				} else {
					this.mniSubscribedToStreamingAdapterQuotesBars.BackColor = SystemColors.Control;
					this.DdbBars.BackColor = SystemColors.Control;
					this.mniSubscribedToStreamingAdapterQuotesBars.Text = "Subscribed to [" + this.ChartFormManager.Executor.DataSource.StreamingAdapterName + "]";
				}

				ContextChart ctxChart = this.ChartFormManager.ContextCurrentChartOrStrategy;
				bool prevStreaming = ctxChart.IsStreaming;

				string reason = "mniSubscribedToStreamingAdapterQuotesBars.Checked[" + this.mniSubscribedToStreamingAdapterQuotesBars.Checked + "]";
				if (this.mniSubscribedToStreamingAdapterQuotesBars.Checked) {
					this.ChartFormManager.ChartStreamingConsumer.StreamingSubscribe(reason);
					if (this.ChartFormManager.Strategy != null && ctxChart.IsStreamingTriggeringScript) {
						// without backtest here, Indicators aren't calculated if there was no "Backtest Now" or "Backtest on App Restart"
						// better duplicated backtest but synced, than streaming starts without prior bars are processed by the strategy
						// TODO few quotes might get pushed into the indicators/strategy before backtest pauses QuotePump in new thread
						this.ChartFormManager.BacktesterRunSimulation();
					}
				} else {
					this.ChartFormManager.ChartStreamingConsumer.StreamingUnsubscribe(reason);
				}

				bool nowStreaming = ctxChart.IsStreaming;
				if (nowStreaming == prevStreaming) {
					string msg = "SHOULD_HAVE_CHANGED_BUT_STAYS_THE_SAME nowStreaming[" + nowStreaming + "] == prevStreaming[" + prevStreaming + "]";
					Assembler.PopupException(msg);
				}
				if (nowStreaming != this.mniSubscribedToStreamingAdapterQuotesBars.Checked) {
					string msg = "MUST_BE_SYNCHRONIZED_BUT_STAYS_UNSYNC nowStreaming[" + nowStreaming
						+ "] != this.mniSubscribedToStreamingAdapterQuotesBars.Checked[" + this.mniSubscribedToStreamingAdapterQuotesBars.Checked + "]";
					Assembler.PopupException(msg);
				}

				if (this.ChartFormManager.Strategy == null) {
					// .IsStreaming {that we just changed by StreamingSubscribe()/StreamingUnSubscribe()} is in ContextChart => saving DataSnapshot
					this.ChartFormManager.DataSnapshotSerializer.Serialize();
				} else {
					// .IsStreaming {that we just changed by StreamingSubscribe()/StreamingUnSubscribe()} is in ContextScript => saving Strategy
					this.ChartFormManager.Strategy.Serialize();
				}

			} catch (Exception ex) {
				Assembler.PopupException("mniRedrawChartOnEachQuote_Click()", ex);
			}
		}
		void TsiProgressBarETA_Click(object sender, EventArgs e) {
			this.ChartFormManager.Executor.Backtester.AbortRunningBacktestWaitAborted("Backtest Aborted by clicking on progress bar");
		}
	}
}