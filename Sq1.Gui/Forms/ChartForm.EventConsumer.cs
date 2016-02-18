using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Core.DataTypes;
using Sq1.Core.Execution;
using Sq1.Core.StrategyBase;
using Sq1.Core.Charting;

using Sq1.Widgets;
using Sq1.Widgets.LabeledTextBox;

namespace Sq1.Gui.Forms {
	public partial class ChartForm {
		void ctxBars_Opening(object sender, CancelEventArgs e) {
			this.populateCtxMniBars_streamingConnectionState_orange();
		}
		void ctxChart_Opening(object sender, CancelEventArgs e) {
			if (this.MniShowSourceCodeEditor.Enabled == false) return;	// don't show ScriptEditor for Strategy.ActivatedFromDll
			this.MniShowSourceCodeEditor.Checked = this.ChartFormManager.ScriptEditorIsOnSurface; 
			//this.editBarToolStripMenuItem.Enabled = this.AllowEditBarData;
			//_preCalcPricesHandleTradeMenuItemsGuiThread();
		}
		void ctxStrategy_Opening(object sender, CancelEventArgs e) {
			this.MniShowLivesim		.Checked = this.ChartFormManager.LivesimFormIsOnSurface;
			this.MniShowSequencer	.Checked = this.ChartFormManager.SequencerIsOnSurface;
			this.MniShowCorrelator	.Checked = this.ChartFormManager.CorrelatorFormIsOnSurface;
			if (this.MniShowSourceCodeEditor.Enabled == false) return;	// don't show ScriptEditor for Strategy.ActivatedFromDll
			this.MniShowSourceCodeEditor.Checked = this.ChartFormManager.ScriptEditorIsOnSurface; 
		}
		void ctxBacktest_Opening(object sender, CancelEventArgs e) {
		}
		void mniStrategyRemove_Click(object sender, System.EventArgs e) {
			this.ChartFormManager.Strategy = null;
			this.ChartFormManager.Executor.Initialize(null, this.ChartControl);
			this.ChartFormManager.InitializeWithoutStrategy(this.ChartFormManager.ContextCurrentChartOrStrategy, true);
			this.ChartControl.ClearAllScriptObjectsBeforeBacktest();
			
			if (DockContentImproved.IsNullOrDisposed(this.ChartFormManager.ScriptEditorForm) == false) {
				this.ChartFormManager.ScriptEditorForm.Close();
			}
			if (DockContentImproved.IsNullOrDisposed(this.ChartFormManager.LivesimForm) == false) {
				this.ChartFormManager.LivesimForm.Close();
			}
			if (DockContentImproved.IsNullOrDisposed(this.ChartFormManager.CorrelatorForm) == false) {
				this.ChartFormManager.CorrelatorForm.Close();
			}
			if (DockContentImproved.IsNullOrDisposed(this.ChartFormManager.SequencerForm) == false) {
				this.ChartFormManager.SequencerForm.Close();
			}
			this.mniStrategyRemove.Enabled = false;
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
		void mniShowSequencer_Click(object sender, System.EventArgs e) {
			this.ctxStrategy.Visible = true;
			if (this.MniShowSequencer.Checked == false) {
				this.MniShowSequencer.Checked = true;
				// if autohidden => popup and keepAutoHidden=false
				this.ChartFormManager.SequencerFormShow(false);
				this.ChartFormManager.MainForm.MainFormSerialize();
			} else {
				this.MniShowSequencer.Checked = false;
				//v1 this.ChartFormManager.SequencerFormConditionalInstance.ToggleAutoHide();
				if (DockContentImproved.IsNullOrDisposed(this.ChartFormManager.SequencerForm)) {
					string msg = "CHECK_ON_CLICK_WILL_SET_CHECKED_AFTER_THIS_HANDLER_EXITS YOU_DIDNT_SYNC_MNI_TICK=OFF_WHEN_SEQUENCER_FORM_WAS_CLOSED_BY_X";
					//Assembler.PopupException(msg);
				} else {
					this.ChartFormManager.SequencerForm.Close();
				}
			}
			// DUPLICATE_XML_SERIALIZATION_AFTER SequencerForm.OnFormClosed()
			//this.ChartFormManager.MainForm.MainFormSerialize();
		}
		void mniShowCorrelator_Click(object sender, System.EventArgs e) {
			this.ctxStrategy.Visible = true;
			if (this.MniShowCorrelator.Checked == false) {
				this.MniShowCorrelator.Checked = true;
				if (this.ChartFormManager.CorrelatorForm == null) {
					this.ChartFormManager.CorrelatorFormConditionalInstance.Initialize(this.ChartFormManager);
					//this.ChartFormManager.CorrelatorFormConditionalInstance.PopulateSequencedHistory(this.ChartFormManager.SequencerFormConditionalInstance.SequencerControl.Seq.SequencedBacktests);
					this.ChartFormManager.SequencerFormConditionalInstance.SequencerControl.RaiseOnCorrelatorShouldPopulateBacktestsIhave();
				}
				this.ChartFormManager.CorrelatorFormShow(false);		// if autohidden => popup and keepAutoHidden=false
				this.ChartFormManager.MainForm.MainFormSerialize();
			} else {
				this.MniShowCorrelator.Checked = false;
				//v1 this.ChartFormManager.CorrelatorFormConditionalInstance.ToggleAutoHide();
				if (DockContentImproved.IsNullOrDisposed(this.ChartFormManager.CorrelatorForm)) {
					string msg = "CHECK_ON_CLICK_WILL_SET_CHECKED_AFTER_THIS_HANDLER_EXITS YOU_DIDNT_SYNC_MNI_TICK=OFF_WHEN_Correlator_FORM_WAS_CLOSED_BY_X";
					//Assembler.PopupException(msg);
				} else {
					this.ChartFormManager.CorrelatorForm.Close();
				}
			}
			this.ctxStrategy.Visible = false;	// only mniCorrelator.Checked=true;
			this.ctxStrategy.Visible = true;	// reopen will show mniSequencer.Checked=true koz we just opened both

			// DUPLICATE_XML_SERIALIZATION_AFTER CorrelatorForm.OnFormClosed()
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
				if (this.BtnStreamingTriggersScript.Checked) {
					this.ChartControl.ChartStreamingConsumer.StreamingTriggeringScriptStart();
					// same idea as in mniSubscribedToStreamingAdapterQuotesBars_Click();
					ContextChart ctxChart = this.ChartFormManager.ContextCurrentChartOrStrategy;
					if (	this.ChartFormManager.Executor.Strategy != null
						&&	this.ChartFormManager.Executor.Strategy.Script != null
						&&	ctxChart.StreamingIsTriggeringScript
						&& this.ChartFormManager.Strategy.ScriptContextCurrent.BacktestOnTriggeringYesWhenNotSubscribed
						) {
						this.ChartFormManager.BacktesterRunSimulation();
					}
				} else {
					this.ChartControl.ChartStreamingConsumer.StreamingTriggeringScriptStop();
				}
				this.PopulateBtnStreamingTriggersScript_afterBarsLoaded();
				if (this.ChartFormManager.Strategy != null) {
					this.ChartFormManager.Strategy.Serialize();
				} else {
					string msg = "CHART_WITHOUT_STRATEGY_IS_ALWAYS_STREAMING_I_JUST_IGNORED??_BUTTON_UNCLICK";
				}
				//WHO_ELSE_NEEDS_IT? this.RaiseStreamingButtonStateChanged();
				this.PropagateSelectorsDisabledIfStreaming_forCurrentChart();
			} catch (Exception ex) {
				Assembler.PopupException(ex.Message);
			}
		}
		void btnStrategyEmittingOrders_Click(object sender, EventArgs e) {
			// ToolStripButton pre-toggles itself when ChartForm{Properties}.BtnAutoSubmit.CheckOnClick=True this.BtnAutoSubmit.Checked = !this.BtnAutoSubmit.Checked;;
			this.ChartFormManager.Executor.IsStrategyEmittingOrders = this.BtnStrategyEmittingOrders.Checked;
			this.ChartFormManager.Strategy.Serialize();
		}

		void mniBacktestOnAnyChange_Click(object sender, System.EventArgs e) {
			try {
				Strategy strategy = this.ChartFormManager.Executor.Strategy;
				if (strategy == null) return;
				strategy.ScriptContextCurrent.BacktestOnTriggeringYesWhenNotSubscribed	= this.mniBacktestOnTriggeringYesWhenNotSubscribed.Checked;
				strategy.ScriptContextCurrent.BacktestOnSelectorsChange					= this.mniBacktestOnSelectorsChange.Checked;
				strategy.ScriptContextCurrent.BacktestOnDataSourceSaved					= this.mniBacktestOnDataSourceSaved.Checked;
				strategy.ScriptContextCurrent.BacktestOnRestart							= this.mniBacktestOnRestart.Checked;
				strategy.ScriptContextCurrent.BacktestAfterSubscribed					= this.mniBacktestAfterSubscribed.Checked;
				strategy.Serialize();

				if (sender == this.mniBacktestAfterSubscribed) {
					this.ctxBars.Visible = true;
				} else {
					this.ctxBacktest.Visible = true;
				}
			} catch (Exception ex) {
				Assembler.PopupException("mniBacktestOnEveryChange_Click()", ex);
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
				
				this.ChartFormManager.PopulateSelectors_fromCurrentChartOrScriptContext_loadBars_saveStrategyOrCtx_backtestIfStrategy("mniltbAll_UserTyped");
				this.ChartFormManager.SequencerFormIfOpenPropagateTextboxesOrMarkStaleResultsAndDeleteHistory();
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

				this.ChartFormManager.PopulateSelectors_fromCurrentChartOrScriptContext_loadBars_saveStrategyOrCtx_backtestIfStrategy("mnitlbShowLastBars_UserTyped");
				this.ChartFormManager.SequencerFormIfOpenPropagateTextboxesOrMarkStaleResultsAndDeleteHistory();
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

				this.ChartFormManager.PopulateSelectors_fromCurrentChartOrScriptContext_loadBars_saveStrategyOrCtx_backtestIfStrategy("mnitlbPositionSizeSharesConstant_UserTyped");
				this.ChartFormManager.SequencerFormIfOpenPropagateTextboxesOrMarkStaleResultsAndDeleteHistory();
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

				this.ChartFormManager.PopulateSelectors_fromCurrentChartOrScriptContext_loadBars_saveStrategyOrCtx_backtestIfStrategy("mnitlbPositionSizeDollarsEachTradeConstant_UserTyped");
				this.ChartFormManager.SequencerFormIfOpenPropagateTextboxesOrMarkStaleResultsAndDeleteHistory();
			} catch (Exception ex) {
				Assembler.PopupException("mnitlbPositionSizeDollarsEachTradeConstant_UserTyped()", ex);
			}
		}
		void mniOutsideQuoteFillCheckThrow_Click(object sender, EventArgs e) {
			this.ctxBacktest.Visible = true;
			ContextScript context = this.ChartFormManager.Strategy.ScriptContextCurrent;
			context.FillOutsideQuoteSpreadParanoidCheckThrow = this.mniFillOutsideQuoteSpreadParanoidCheckThrow.Checked;
			this.ChartFormManager.Strategy.Serialize();
			this.ChartFormManager.PopulateSelectors_fromCurrentChartOrScriptContext_loadBars_saveStrategyOrCtx_backtestIfStrategy("mniOutOfQuoteFillThrow_Click");
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
				this.mnitlbSpreadGeneratorPct.TextRight = this.ChartFormManager.Executor.SpreadPips + " pips";
				
				if (this.ChartFormManager.Executor.BacktesterOrLivesimulator.BacktestDataSource == null) {
					this.ChartFormManager.Executor.BacktesterOrLivesimulator.InitializeQuoteGenerator();
				}

				this.ChartFormManager.PopulateSelectors_fromCurrentChartOrScriptContext_loadBars_saveStrategyOrCtx_backtestIfStrategy("mnitlbSpreadGeneratorPct_UserTyped");
				this.ChartFormManager.SequencerFormIfOpenPropagateTextboxesOrMarkStaleResultsAndDeleteHistory();
			} catch (Exception ex) {
				Assembler.PopupException("mnitlbSpreadGeneratorPct_UserTyped()", ex);
			}
		}
		void mniSubscribedToStreamingAdapterQuotesBars_Click(object sender, EventArgs e) {
			try {
				ContextChart ctxChart = this.ChartFormManager.ContextCurrentChartOrStrategy;
				bool prevStreaming = ctxChart.DownstreamSubscribed;

				string reason = "mniSubscribedToStreamingAdapterQuotesBars.Checked[" + this.mniSubscribedToStreamingAdapterQuotesBars.Checked + "]";
				if (this.mniSubscribedToStreamingAdapterQuotesBars.Checked) {
					this.ChartControl.ChartStreamingConsumer.StreamingSubscribe(reason);
					if (this.ChartFormManager.Strategy != null
							// GET_IT_FROM_SCRIPT_NOT_CHART_ALTHOUGH_SAME_POINTER && ctxChart.IsStreamingTriggeringScript
							//&& this.ChartFormManager.Strategy.ScriptContextCurrent.StreamingIsTriggeringScript
							&& this.ChartFormManager.Strategy.ScriptContextCurrent.BacktestAfterSubscribed
						) {
						// without backtest here, Indicators aren't calculated if there was no "Backtest Now" or "Backtest on App Restart"
						// better duplicated backtest but synced, than streaming starts without prior bars are processed by the strategy
						// TODO few quotes might get pushed into the indicators/strategy before backtest pauses QuotePump in new thread
						this.ChartFormManager.BacktesterRunSimulation();
					}
				} else {
					this.ChartControl.ChartStreamingConsumer.StreamingUnsubscribe(reason);
					this.ChartControl.ScriptExecutorObjects.QuoteLast = null;
				}

				bool nowStreaming = ctxChart.DownstreamSubscribed;
				if (nowStreaming == prevStreaming) {
					string msg = "SHOULD_HAVE_CHANGED_BUT_STAYS_THE_SAME nowStreaming[" + nowStreaming + "] == prevStreaming[" + prevStreaming + "] "  + reason;
					Assembler.PopupException(msg);
				}
				this.populateCtxMniBars_streamingConnectionState_orange();
				this.PrintQuoteTimestampOnStrategyTriggeringButton_beforeExecution_switchToGuiThread(null);
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

				this.ctxBars.Visible = true;
			} catch (Exception ex) {
				Assembler.PopupException("mniSubscribedToStreamingAdapterQuotesBars_Click()", ex);
			}
		}

		void TsiProgressBarETA_Click(object sender, EventArgs e) {
			this.ChartFormManager.Executor.BacktesterOrLivesimulator.AbortRunningBacktestWaitAborted("Backtest Aborted by clicking on progress bar");
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			if (base.DesignMode) return;
			base.OnMouseUp(e);
			//NO_VISIBLE_IMPAIRMENT_IF_COMMENTED_OUT_RIGHT? ChartSettingsEditorForm.Instance.ChartSettingsEditorControl.PopulateWithChartSettings();
		}
		void chartForm_Load(object sender, EventArgs e) {
			if (this.waitForChartFormIsLoaded.WaitOne(0) == true) {
				string msg = "MUST_BE_INSTANTIATED_AS_NON_SIGNALLED_IN_CTOR()_#1 waitForChartFormIsLoaded.WaitOne(0)=[true]";
				Assembler.PopupException(msg);
				return;	// why signal on already-signalled?
			}
			this.waitForChartFormIsLoaded.Set();
			this.ChartControl.ChartShadow_AddToDataSource();
		}
		void chartForm_Closed(object sender, FormClosedEventArgs e) {
			if (Assembler.InstanceInitialized.MainFormClosingIgnoreReLayoutDockedForms) {
				return;
			}
			this.ChartFormManager.DataSnapshotSerializer.DeleteJsonFile();
			this.ChartControl.ChartShadow_RemoveFromDataSource();
		}

		void mniMinimizeAllReportersGuiExtensiveForTheDurationOfLiveSim_Clicked(object sender, EventArgs e) {
			string msig = "mniMinimizeAllReportersGuiExtensiveForTheDurationOfLiveSim_Clicked()";
			try {
				this.ctxStrategy.Visible = true;
				//if (this.ChartFormManager.Strategy == null) return;
				Strategy strategy = this.ChartFormManager.Executor.Strategy;
				if (strategy == null) {
					string msg = "CAN_NOT_SAVE_CHECKBOX_YOU_CLICKED__INACCESSIBLE_ScriptContextCurrent this.ChartFormManager.Executor.Strategy=null";
					Assembler.PopupException(msg + msig);
					return;
				}
				strategy.ScriptContextCurrent.MinimizeGuiExtensiveExecutionAllReportersForTheDurationOfLiveSim
									= this.mniMinimizeAllReportersGuiExtensiveForTheDurationOfLiveSim.Checked;
				strategy.Serialize();
				this.MniShowLivesim.ShowDropDown();

				//dont sync me now, I'm having just a good time
				//if (strategy.ScriptContextCurrent.MinimizeAllReportersGuiExtensiveForTheDurationOfLiveSim) {	// sync after click (paused or running)
				//    this.ChartFormManager.LivesimStartedOrUnpaused_AutoHiddeExecutionAndReporters();
				//} else {
				//    this.ChartFormManager.LivesimEndedOrStoppedOrPaused_RestoreAutoHiddenExecutionAndReporters();
				//}
			} catch (Exception ex) {
				Assembler.PopupException(msig, ex);
			}
		}

		void chartControl_BarStreamingUpdatedMerged(object sender, BarEventArgs e) {
			if (this.ChartFormManager.Executor.BacktesterOrLivesimulator.ImRunningLivesim == false) {
				string msg = "NON_LIVESIM_STREAMING_SEEMS_TO_HAVE_ChartFormStreamingConsumer_HANDLING_QUOTE_TIMESTAMP_ON_BTN";
				//Assembler.PopupException(msg, null, false);
				//return;
			}
			bool guiHasTime = this.ChartFormManager.Executor.Livesimulator.LivesimStreamingIsSleepingNow_ReportersAndExecutionHaveTimeToRebuild;
			if (guiHasTime == false) return;
			this.PrintQuoteTimestampOnStrategyTriggeringButton_beforeExecution_switchToGuiThread(null);
		}

		void ChartControl_OnPumpPaused(object sender, EventArgs e) {
			this.mniSubscribedToStreamingAdapterQuotesBars.Enabled = false;
		}
		void ChartControl_OnPumpUnPaused(object sender, EventArgs e) {
			this.mniSubscribedToStreamingAdapterQuotesBars.Enabled = true;
		}
	}
}