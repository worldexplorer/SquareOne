using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Core.DataTypes;
using Sq1.Core.Execution;
using Sq1.Core.StrategyBase;
using Sq1.Core.Streaming;
using Sq1.Widgets.LabeledTextBox;

namespace Sq1.Gui.Forms {
	public partial class ChartForm {
		public ChartFormManager ChartFormManager;

		List<string> GroupScaleLabeledTextboxes;
		List<string> GroupPositionSizeLabeledTextboxes;

		// SharpDevelop/VisualStudio Designer's constructor
		public ChartForm() {
			InitializeComponent();
			this.ChartControl.RangeBarCollapsed = !this.mniShowBarRange.Checked;
			
			// in case if Designer removes these from ChartForm.Designer.cs 
//			this.mnitlbYearly.UserTyped += new EventHandler<LabeledTextBoxUserTypedArgs>(this.mniltbAll_UserTyped);
//			this.mnitlbMonthly.UserTyped += new EventHandler<LabeledTextBoxUserTypedArgs>(this.mniltbAll_UserTyped);
//			this.mnitlbWeekly.UserTyped += new EventHandler<LabeledTextBoxUserTypedArgs>(this.mniltbAll_UserTyped);
//			this.mnitlbDaily.UserTyped += new EventHandler<LabeledTextBoxUserTypedArgs>(this.mniltbAll_UserTyped);
//			this.mnitlbHourly.UserTyped += new EventHandler<LabeledTextBoxUserTypedArgs>(this.mniltbAll_UserTyped);
//			this.mnitlbMinutes.UserTyped += new EventHandler<LabeledTextBoxUserTypedArgs>(this.mniltbAll_UserTyped);
//			this.mnitlbShowLastBars.UserTyped += new EventHandler<LabeledTextBoxUserTypedArgs>(this.mnitlbShowLastBars_UserTyped);

			this.GroupScaleLabeledTextboxes = new List<string>() {
				this.mnitlbMinutes.Name, this.mnitlbHourly.Name, this.mnitlbDaily.Name, this.mnitlbWeekly.Name, this.mnitlbMonthly.Name,
				//this.mnitlbQuarterly.Name,
				this.mnitlbYearly.Name};
			this.GroupPositionSizeLabeledTextboxes = new List<string>() {
				this.mnitlbPositionSizeDollarsEachTradeConstant.Name, this.mnitlbPositionSizeSharesConstantEachTrade.Name};
			//OVERRODE_IN_CHART_CONTROL_DONT_CARE_HERE_NOW: override bool ProcessCmdKey //base.KeyPreview = true;
		}
		//programmer's constructor
		public ChartForm(ChartFormManager chartFormManager) : this() {
			this.ChartFormManager = chartFormManager;
			// right now this.ChartFormsManager.Executor IS NULL, will create and Chart.Initialize() upstack :((
			//this.Chart.Initialize(this.ChartFormsManager.Executor);
			// TOO_EARLY_NO_BARS_SET_WILL_BE_THROWN this.PopulateBtnStreamingText();
		}
		public void ChartFormEventsToChartFormManagerAttach() {
			this.ChartControl.RangeBar.ValueMinChanged += this.ChartFormManager.InterformEventsConsumer.ChartRangeBar_AnyValueChanged;
			this.ChartControl.RangeBar.ValueMaxChanged += this.ChartFormManager.InterformEventsConsumer.ChartRangeBar_AnyValueChanged;
			this.ChartControl.RangeBar.ValuesMinAndMaxChanged += this.ChartFormManager.InterformEventsConsumer.ChartRangeBar_AnyValueChanged;
			
			this.ChartControl.ChartSettingsChangedContainerShouldSerialize += new EventHandler<EventArgs>(ChartControl_ChartSettingsChangedContainerShouldSerialize);
			this.ChartControl.ContextScriptChangedContainerShouldSerialize += new EventHandler<EventArgs>(ChartControl_ContextScriptChangedContainerShouldSerialize);
		}
		public void ChartFormEventsToChartFormManagerDetach() {
			this.ChartControl.RangeBar.ValueMinChanged -= this.ChartFormManager.InterformEventsConsumer.ChartRangeBar_AnyValueChanged;
			this.ChartControl.RangeBar.ValueMaxChanged -= this.ChartFormManager.InterformEventsConsumer.ChartRangeBar_AnyValueChanged;
			this.ChartControl.RangeBar.ValuesMinAndMaxChanged -= this.ChartFormManager.InterformEventsConsumer.ChartRangeBar_AnyValueChanged;

			this.ChartControl.ChartSettingsChangedContainerShouldSerialize -= new EventHandler<EventArgs>(ChartControl_ChartSettingsChangedContainerShouldSerialize);
			this.ChartControl.ContextScriptChangedContainerShouldSerialize -= new EventHandler<EventArgs>(ChartControl_ContextScriptChangedContainerShouldSerialize);
		}

		void ChartControl_ContextScriptChangedContainerShouldSerialize(object sender, EventArgs e) {
			if (this.ChartFormManager.Strategy == null) {
				string msg = "I_INVOKED_YOU_FROM_REPORTER_NOT_POSSIBLE_STRATEGY_DISAPPEARED_NOW";
				Debugger.Break();
			}
			Assembler.InstanceInitialized.RepositoryDllJsonStrategy.StrategySave(this.ChartFormManager.Strategy);
		}
		void ChartControl_ChartSettingsChangedContainerShouldSerialize(object sender, EventArgs e) {
			this.ChartFormManager.DataSnapshotSerializer.Serialize();
		}
		// http://www.codeproject.com/Articles/525541/Decoupling-Content-From-Container-in-Weifen-Luos
		// using ":" since "=" leads to an exception in DockPanelPersistor.cs
		protected override string GetPersistString() {
			string ret = "Chart:" + this.GetType().FullName + ",ChartSerno:" + this.ChartFormManager.DataSnapshot.ChartSerno;
			if (this.ChartFormManager.Strategy != null) {
				string strategyName = this.ChartFormManager.Strategy.Name;
				strategyName.Replace(',', '_');
				strategyName.Replace(':', '-');
				ret += ",StrategyName:" + strategyName;
				ret += ",StrategyGuid:" + this.ChartFormManager.Strategy.Guid;
				if (this.ChartFormManager.Strategy.ScriptContextCurrent != null) {
					ret += ",StrategyScriptContextName:" + this.ChartFormManager.Strategy.ScriptContextCurrent.Name;
				}
			}
			return ret;
		}
		public void PrintQuoteTimestampOnStrategyTriggeringButtonBeforeExecution(Quote quote) {
			if (quote == null) return;
			if (InvokeRequired) {
				base.BeginInvoke((MethodInvoker)delegate { this.PrintQuoteTimestampOnStrategyTriggeringButtonBeforeExecution(quote); });
				return;
			}
			StringBuilder sb = new StringBuilder(this.ChartFormManager.StreamingButtonIdent);
			sb.Append(" #");
			sb.Append(quote.IntraBarSerno.ToString("000"));
			sb.Append(" ");
			sb.Append(quote.ServerTime.ToString("HH:mm:ss.fff"));
			bool quoteTimesDifferMoreThanOneMicroSecond = quote.ServerTime.ToString("HH:mm:ss.f") != quote.LocalTimeCreated.ToString("HH:mm:ss.f");
			if (quoteTimesDifferMoreThanOneMicroSecond) {
				sb.Append(" :: ");
				sb.Append(quote.LocalTimeCreated.ToString("HH:mm:ss.fff"));
			}
			if (quote.HasParentBar) {
				TimeSpan timeLeft = (quote.ParentBarStreaming.DateTimeNextBarOpenUnconditional > quote.ServerTime)
					? quote.ParentBarStreaming.DateTimeNextBarOpenUnconditional.Subtract(quote.ServerTime)
					: quote.ServerTime.Subtract(quote.ParentBarStreaming.DateTimeNextBarOpenUnconditional);
				string format = ":ss";
				if (timeLeft.Minutes > 0) format = "mm:ss";
				if (timeLeft.Hours > 0) format = "HH:mm:ss";
				sb.Append(" ");
				sb.Append(new DateTime(timeLeft.Ticks).ToString(format));
			}
			this.btnStreamingTriggersScript.Text = sb.ToString();
		}
		public void PopulateBtnStreamingTriggersScriptAfterBarsLoaded() {
			//v1: IDEALLY_BACKTESTS_ARE_POSSIBLE_EVEN_DURING_STREAMING_USING_PUT_ON_HOLD
//			bool streamingNow = this.ChartFormManager.ContextCurrentChartOrStrategy.IsStreaming;
//			if (streamingNow) {
//				this.mniBacktestOnSelectorsChange.Enabled = false;
//				this.mniBacktestOnDataSourceSaved.Enabled = false;
//				this.mniBacktestNow.Enabled = false;
//				this.btnStrategyEmittingOrders.Enabled = true;
//			} else {
//				this.mniBacktestOnSelectorsChange.Enabled = true;
//				this.mniBacktestOnDataSourceSaved.Enabled = true;
//				this.mniBacktestNow.Enabled = true;
//				this.btnStrategyEmittingOrders.Enabled = false;
//			}
			
			if (this.ChartFormManager.Executor.DataSource.StreamingProvider == null) {
				this.btnStreamingTriggersScript.Text = "DataSource: [" + StreamingProvider.NO_STREAMING_PROVIDER + "]";
				this.btnStreamingTriggersScript.Enabled = false;
				this.mniSubscribedToStreamingProviderQuotesBars.Text = "Not Subscribed: edit DataSource > attach StreamingProvider";
			} else {
				this.btnStreamingTriggersScript.Text = this.ChartFormManager.StreamingButtonIdent + " 00:00:00.000"; //+:: 00:00:00.000";
				this.btnStreamingTriggersScript.Enabled = true;

				// "AfterBarsLoaded" implies Executor.SetBars() has already initialized this.ChartFormManager.Executor.DataSource
				this.mniSubscribedToStreamingProviderQuotesBars.Text = "Subscribed to [" + this.ChartFormManager.Executor.DataSource.StreamingProvider.Name + "]";
			}
		}
		public void AbsorbContextBarsToGui() {
			if (base.InvokeRequired) {
				base.BeginInvoke((MethodInvoker)delegate { this.AbsorbContextBarsToGui(); });
				return;
			}

			StreamingProvider streaming = this.ChartFormManager.Executor.DataSource.StreamingProvider;
			Bitmap iconCanBeNull = streaming != null ? streaming.Icon : null;

			if (iconCanBeNull != null) {
				this.btnStreamingTriggersScript.Image = iconCanBeNull;
			}
			// from btnStreaming_Click(); not related but visualises the last clicked state
			if (this.btnStreamingTriggersScript.Checked) {
				this.mniBacktestOnSelectorsChange.Enabled = false;
				this.mniBacktestOnDataSourceSaved.Enabled = false;
				this.mniBacktestNow.Enabled = false;
				//this.btnStrategyEmittingOrders.Enabled = true;
			} else {
				this.mniBacktestOnSelectorsChange.Enabled = true;
				this.mniBacktestOnDataSourceSaved.Enabled = true;
				this.mniBacktestNow.Enabled = true;
				//this.btnStrategyEmittingOrders.Enabled = false;
			}

			Bars barsClickedUpstack = this.ChartFormManager.Executor.Bars;
			this.mniBarsStoredScaleInterval.Text = barsClickedUpstack != null
				? barsClickedUpstack.ScaleInterval.ToString() + " Minimun"
				: "[MIN_SCALEINTERVAL_UNKNOWN]";

			// WAS_METHOD_PARAMETER_BUT_ACCESSIBLE_LIKE_THIS__NULL_CHECK_DONE_UPSTACK
			ContextChart ctxChart = this.ChartFormManager.ContextCurrentChartOrStrategy;

			this.mniSubscribedToStreamingProviderQuotesBars.Checked = ctxChart.IsStreaming;
			if (this.mniSubscribedToStreamingProviderQuotesBars.Checked == false) {
				this.mniSubscribedToStreamingProviderQuotesBars.BackColor = Color.LightSalmon;
				this.DdbBars.BackColor = Color.LightSalmon;
			} else {
				this.mniSubscribedToStreamingProviderQuotesBars.BackColor = SystemColors.Control;
				this.DdbBars.BackColor = SystemColors.Control;
			}
			
			if (ctxChart.ShowRangeBar) {
				this.ChartControl.RangeBarCollapsed = false; 
				this.mniShowBarRange.Checked = true;
			} else {
				this.ChartControl.RangeBarCollapsed = true; 
				this.mniShowBarRange.Checked = false;
			}

			this.btnStreamingTriggersScript.Checked = ctxChart.IsStreamingTriggeringScript;
			ContextScript ctxScript = ctxChart as ContextScript;
			if (ctxScript == null) return;
			
			this.mniBacktestOnRestart.Checked = ctxScript.BacktestOnRestart;
			this.mniBacktestOnSelectorsChange.Checked = ctxScript.BacktestOnSelectorsChange;
			this.btnStrategyEmittingOrders.Checked = ctxScript.StrategyEmittingOrders;
			this.PropagateContextScriptToLTB(ctxScript);
		}
		
		public void PropagateContextScriptToLTB(ContextScript ctxScript) {
			MenuItemLabeledTextBox mnitlbForScale = null;
			switch (ctxScript.ScaleInterval.Scale) {
				case BarScale.Minute:		mnitlbForScale = this.mnitlbMinutes; break; 
				case BarScale.Hour:			mnitlbForScale = this.mnitlbDaily; break; 
				case BarScale.Daily:		mnitlbForScale = this.mnitlbHourly; break; 
				case BarScale.Weekly:		mnitlbForScale = this.mnitlbWeekly; break; 
				case BarScale.Monthly:		mnitlbForScale = this.mnitlbMonthly; break; 
				//case BarScale.Quarterly		mnitlbForScale = this.mnitlbQuarterly; break;
				case BarScale.Yearly:		mnitlbForScale = this.mnitlbYearly; break;
				case BarScale.Unknown: 
					string msg = "TODO: figure out why deserialized / userSelected strategyClicked[" + this.ChartFormManager.Executor.Strategy
						+ "].ScriptContextCurrent.ScaleInterval[" + ctxScript.ScaleInterval + "] has BarScale.Unknown #4";
					Assembler.PopupException(msg);
					break;
				default:
					string msg2 = "SCALE_UNHANDLED_NO_TEXTBOX_TO_POPULATE " + ctxScript.ScaleInterval.Scale;
					Assembler.PopupException(msg2);
					break;
			}
				
			if (mnitlbForScale != null) {
				mnitlbForScale.InputFieldValue = ctxScript.ScaleInterval.Interval.ToString();
				mnitlbForScale.BackColor = Color.Gainsboro;
			}

			this.mniShowBarRange.Checked = ctxScript.ShowRangeBar;
			switch (ctxScript.DataRange.Range) {
				case BarRange.AllData:
					this.mnitlbShowLastBars.InputFieldValue = "";
					this.mnitlbShowLastBars.BackColor = Color.White;
					break;
				case BarRange.DateRange:
					this.ChartControl.RangeBar.ValueMin = ctxScript.DataRange.DateFrom; 
					this.ChartControl.RangeBar.ValueMax = ctxScript.DataRange.DateTill; 
					this.mnitlbShowLastBars.InputFieldValue = "";
					this.mnitlbShowLastBars.BackColor = Color.White;
					break;
				case BarRange.RecentBars:
					this.mnitlbShowLastBars.InputFieldValue = ctxScript.DataRange.RecentBars.ToString();
					this.mnitlbShowLastBars.BackColor = Color.Gainsboro;
					//this.mniShowBarRange.Checked = false;
					break;
				default:
					string msg = "DATE_RANGE_UNHANDLED_RECENT_TIMEUNITS_NYI " + ctxScript.DataRange;
					Assembler.PopupException(msg);
					break;
			}
			this.ChartControl.RangeBarCollapsed = !this.mniShowBarRange.Checked; 


			if (ctxScript.PositionSize.Mode == PositionSizeMode.Unknown) {
				ctxScript.PositionSize = new PositionSize(PositionSizeMode.SharesConstantEachTrade, 1);
				string msg = "FIXED_POSITIONSIZE_TO_SHARE_1 strategy[" + this.ChartFormManager.Executor.Strategy
					+ "].ScriptContextsByName[" + ctxScript.Name + "] had PositionSize.Mode=Unknown";
				Assembler.PopupException(msg);
			}

			switch (ctxScript.PositionSize.Mode) {
				case PositionSizeMode.SharesConstantEachTrade:
					this.mnitlbPositionSizeSharesConstantEachTrade.InputFieldValue = ctxScript.PositionSize.SharesConstantEachTrade.ToString();
					this.mnitlbPositionSizeSharesConstantEachTrade.BackColor = Color.Gainsboro;
					this.mnitlbPositionSizeDollarsEachTradeConstant.BackColor = Color.White;
					break;
				case PositionSizeMode.DollarsConstantForEachTrade:
					this.mnitlbPositionSizeDollarsEachTradeConstant.InputFieldValue = ctxScript.PositionSize.DollarsConstantEachTrade.ToString();
					this.mnitlbPositionSizeDollarsEachTradeConstant.BackColor = Color.Gainsboro;
					this.mnitlbPositionSizeSharesConstantEachTrade.BackColor = Color.White;
					break;
				default:
					string msg = "POSITION_SIZE_UNHANDLED_NYI " + ctxScript.PositionSize.Mode;
					Assembler.PopupException(msg);
					break;
			}
			
			this.mniFillOutsideQuoteSpreadParanoidCheckThrow.Checked = ctxScript.FillOutsideQuoteSpreadParanoidCheckThrow;
			this.mnitlbSpreadGeneratorPct.InputFieldValue = ctxScript.SpreadModelerPercent.ToString();

			if (this.ChartFormManager.MainForm.ChartFormActiveNullUnsafe == this) {
				string msg = "WE_ARE_HERE_WHEN_WE_SWITCH_STRATEGY_FOR_CHART";
				//Debugger.Break();
				
				//v1 DataSourcesForm.Instance.DataSourcesTreeControl.SelectSymbol(ctxScript.DataSourceName, ctxScript.Symbol);
				//v1 StrategiesForm.Instance.StrategiesTreeControl.SelectStrategy(this.ChartFormManager.Executor.Strategy);
				//v2
				this.ChartFormManager.PopulateMainFormSymbolStrategyTreesScriptParameters();
			}
			this.PropagateSelectorsDisabledIfStreamingForCurrentChart();
		}
		public void PropagateSelectorsDisabledIfStreamingForCurrentChart() {
			Strategy strategyClicked = this.ChartFormManager.Strategy;
			if (strategyClicked == null) return;

			//do not disturb a streaming chart with selector's changes (disable selectors if streaming; for script-free charts strategy=null)
			bool enableForNonStreaming = !strategyClicked.ScriptContextCurrent.IsStreamingTriggeringScript;
			
			//DataSourcesForm.Instance.DataSourcesTree.Enabled = enableForNonStreaming;
			
			this.mnitlbMinutes.Enabled = enableForNonStreaming;
			this.mnitlbDaily.Enabled = enableForNonStreaming;
			this.mnitlbHourly.Enabled = enableForNonStreaming;
			this.mnitlbWeekly.Enabled = enableForNonStreaming;
			this.mnitlbMonthly.Enabled = enableForNonStreaming;
			//this.mnitlbQuarterly.Enabled = enableForNonStreaming;
			this.mnitlbYearly.Enabled = enableForNonStreaming;
			this.mnitlbShowLastBars.Enabled = enableForNonStreaming;
			this.mnitlbPositionSizeDollarsEachTradeConstant.Enabled = enableForNonStreaming;
			this.mnitlbPositionSizeSharesConstantEachTrade.Enabled = enableForNonStreaming;
		}

		public void Initialize(bool containsStrategy, bool strategyActivatedFromDll = false) {
			this.DdbStrategy.Enabled = containsStrategy;
			this.DdbBacktest.Enabled = containsStrategy;
			//this.btnStrategyEmittingOrders.Enabled = containsStrategy;

			this.MniShowSourceCodeEditor.Enabled = containsStrategy;
			if (containsStrategy) this.MniShowSourceCodeEditor.Enabled = !strategyActivatedFromDll;
		}
		public void AppendReportersMenuItems(ToolStripItem[] toolStripItems) {
			this.ctxStrategy.Items.Add(this.TssReportersBelowMe);	// if not added then we didn't initialize!
			this.ctxStrategy.Items.AddRange(toolStripItems);
		}
	}
}