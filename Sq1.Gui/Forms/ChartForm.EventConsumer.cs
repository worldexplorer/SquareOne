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
using WeifenLuo.WinFormsUI.Docking;

namespace Sq1.Gui.Forms {
	public partial class ChartForm {
		void ctxChart_Opening(object sender, CancelEventArgs e) {
			this.MniShowSourceCodeEditor.Checked = (this.ChartFormManager.EditorFormIsNotDisposed);
			//this.editBarToolStripMenuItem.Enabled = this.AllowEditBarData;
			//_preCalcPricesHandleTradeMenuItemsGuiThread();
		}
		void MniShowSourceCodeEditor_Click(object sender, EventArgs e) {
			if (this.MniShowSourceCodeEditor.Checked) {
				// if autohidden => popup and keepAutoHidden=false
				this.ChartFormManager.EditorFormShow(false);
			} else {
				// I assume btn.Checked => EditorForm.NotAutoHidden synchronization
				DockHelper.ToggleAutoHide(this.ChartFormManager.ScriptEditorFormConditionalInstance);
				//public static void ToggleAutoHide(DockContent form) {
				//	if (form.DockState == DockState.Unknown) return;
				//	if (form.DockState == DockState.Document) return;
				//	if (form.DockState == DockState.Float) return;
				//	if (form.DockState == DockState.Hidden) return;
				//	DockState newState = DockHelper.ToggleAutoHideState(form.Pane.DockState);
				//	form.Pane.SetDockState(newState);
				//}
			}
			this.ChartFormManager.MainForm.MainFormSerialize();
		}
		void mniShowSourceCodeEditor_Click(object sender, System.EventArgs e) {
			if (this.MniShowSourceCodeEditor.Checked) {
				// if autohidden => popup and keepAutoHidden=false
				this.ChartFormManager.EditorFormShow(false);
			} else {
				DockHelper.ToggleAutoHide(this.ChartFormManager.ScriptEditorFormConditionalInstance);
			}
			this.ChartFormManager.MainForm.MainFormSerialize();
		}

		void btnStreaming_Click(object sender, EventArgs e) {
			// ToolStripButton pre-toggles itself when ChartForm{Properties}.BtnStreaming.CheckOnClick=True this.BtnStreaming.Checked = !this.BtnStreaming.Checked;
			try {
				if (this.btnStreaming.Checked) {
					this.ChartFormManager.ChartStreamingConsumer.StartStreaming();
				} else {
					this.ChartFormManager.ChartStreamingConsumer.StopStreaming();
				}
				this.PopulateBtnStreamingClickedAndText();
				this.RaiseStreamingButtonStateChanged();
			} catch (Exception ex) {
				Assembler.PopupException(ex.Message);
			}
		}
		void btnAutoSubmit_Click(object sender, EventArgs e) {
			// ToolStripButton pre-toggles itself when ChartForm{Properties}.BtnAutoSubmit.CheckOnClick=True this.BtnAutoSubmit.Checked = !this.BtnAutoSubmit.Checked;;
			this.ChartFormManager.Executor.IsAutoSubmitting = this.btnAutoSubmit.Checked;
		}
		void mniBacktestOnEveryChange_Click(object sender, System.EventArgs e) {
			try {
				Strategy strategy = this.ChartFormManager.Executor.Strategy;
				if (strategy == null) return;
				strategy.ScriptContextCurrent.BacktestOnSelectorsChange = this.mniBacktestOnSelectorsChange.Checked;
				strategy.ScriptContextCurrent.BacktestOnDataSourceSaved = this.mniBacktestOnDataSourceSaved.Checked;
				Assembler.InstanceInitialized.RepositoryDllJsonStrategy.StrategySave(strategy);
			} catch (Exception ex) {
				Assembler.PopupException("mniBacktestOnEveryChange_Click()", ex);
			}
		}
		void mniBacktestOnRestart_Click(object sender, System.EventArgs e) {
			try {
				Strategy strategy = this.ChartFormManager.Executor.Strategy;
				if (strategy == null) return;
				strategy.ScriptContextCurrent.BacktestOnRestart = this.mniBacktestOnRestart.Checked;
				Assembler.InstanceInitialized.RepositoryDllJsonStrategy.StrategySave(strategy);
			} catch (Exception ex) {
				Assembler.PopupException("mniBacktestOnRestart_Click()", ex);
			}
		}
		void mniBacktestNow_Click(object sender, System.EventArgs e) {
			try {
				this.ChartFormManager.BacktesterRunSimulationRegular();
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
			} catch (Exception ex) {
				Assembler.PopupException("mnitlbShowLastBars_UserTyped()", ex);
			}
		}
		void mniShowBarRange_Click(object sender, EventArgs e) {
			try {
				if (e != null) {
					this.mnitlbShowLastBars.BackColor = Color.White;
					this.mnitlbShowLastBars.InputFieldValue = "";
				}
				
				this.ChartControl.RangeBarCollapsed = !this.mniShowBarRange.Checked; 
				if (this.ChartFormManager.Strategy != null) {
					this.ChartFormManager.Strategy.ScriptContextCurrent.ShowRangeBar = this.mniShowBarRange.Checked;
					Assembler.InstanceInitialized.RepositoryDllJsonStrategy.StrategySave(this.ChartFormManager.Strategy);
				} else {
					this.ChartFormManager.DataSnapshot.ContextChart.ShowRangeBar = this.mniShowBarRange.Checked;
					this.ChartFormManager.DataSnapshotSerializer.Serialize();
				}
			} catch (Exception ex) {
				Assembler.PopupException("mniShowBarRange_Click()", ex);
			}
		}
		void mnitlbPositionSizeSharesConstantEachTrade_UserTyped(object sender, LabeledTextBoxUserTypedArgs e) {
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
			} catch (Exception ex) {
				Assembler.PopupException("mnitlbPositionSizeDollarsEachTradeConstant_UserTyped()", ex);
			}
		}
	}
}