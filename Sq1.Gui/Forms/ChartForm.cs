using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Diagnostics;		//Stopwatch
using System.Threading;
using System.Threading.Tasks;

using Sq1.Core;
using Sq1.Core.DataTypes;
using Sq1.Core.Execution;
using Sq1.Core.StrategyBase;
using Sq1.Core.Streaming;
using Sq1.Core.DataFeed;

using Sq1.Widgets;
using Sq1.Widgets.RangeBar;
using Sq1.Widgets.LabeledTextBox;

using Sq1.Gui.Singletons;

namespace Sq1.Gui.Forms {
	public partial class ChartForm : DockContentImproved {
		public	ChartFormManager	ChartFormManager;

				List<string>		GroupScaleLabeledTextboxes;
				List<string>		GroupPositionSizeLabeledTextboxes;

				ManualResetEvent	waitForChartFormIsLoaded;
		public	bool				ChartFormIsLoaded_NonBlocking { get { return this.waitForChartFormIsLoaded.WaitOne(0); } }
		public	bool				ChartFormIsLoaded_Blocking { get {
			string ident = " [" + this.ChartFormManager.Executor.ToString() + "]";	// base.Text throws cross-thread exception, of course on Workspace reload
			// POTENTIALLY_CAN_BE_INVOKED_AFTER_BEED_TRIGGERED_AS_LOADED, ExceptionsControl:UserControlImproved is a more obvious case
			//if (this.waitForChartFormIsLoaded.WaitOne(0) == true) {
			//	string msg1 = "MUST_BE_INSTANTIATED_AS_NON_SIGNALLED_IN_CTOR()_#2 waitForChartFormIsLoaded.WaitOne(0)=[true]";
			//	Assembler.PopupException(msg1);
			//	return false;
			//}
			Stopwatch formLoaded = new Stopwatch();
			formLoaded.Start();
			string msg = "CHART_FORM_IS_LOADED__WAITING..." + ident;
			//Assembler.PopupException(msg, null, false);
			bool loaded = this.waitForChartFormIsLoaded.WaitOne(-1);
			long waitedForMillis = formLoaded.ElapsedMilliseconds;
			formLoaded.Stop();
			if (this.waitForChartFormIsLoaded.WaitOne(0) == false) {
				msg = "CHART_PARANOID FORM_IS_LOADED__FALSE_AFTER_WAITING" + ident;
				Assembler.PopupException(msg);
			}
			msg = "CHART_FORM_IS_LOADED[" + loaded + "] waited[" + waitedForMillis + "]ms" + ident;
			Assembler.PopupException(msg, null, false);
			return loaded;
		} }

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

			this.waitForChartFormIsLoaded = new ManualResetEvent(false);

			this.ChartControl.OnBarStreamingUpdatedMerged_chartFormPrintsQuoteTimestamp += new EventHandler<BarEventArgs>(this.chartControl_BarStreamingUpdatedMerged_chartFormPrintsQuoteTimestamp);
		}
		//programmer's constructor
		public ChartForm(ChartFormManager chartFormManager) : this() {
			this.ChartFormManager = chartFormManager;
			// right now this.ChartFormsManager.Executor IS NULL, will create and Chart.Initialize() upstack :((
			//this.Chart.Initialize(this.ChartFormsManager.Executor);
			// TOO_EARLY_NO_BARS_SET_WILL_BE_THROWN this.PopulateBtnStreamingText();

			this.ctxStrokesForQuoteGenerator.Opening += new CancelEventHandler(ctxStrokesForQuoteGenerator_Opening_SelectCurrent);

			if (this.ChartControl.ClientRectangle.Width != base.ClientRectangle.Width) {
				string msg = "CANT_CATCH_BEOYND_BOUNDARIES_BUG_HERE_DOCKCONTENT_DIDNT_RESIZE_ME_YET MUST_BE_EQUAL_SINCE_CHART_FORM_ISNT_RESIZED_BY_DOCK_CONTENT_BY_ASSIGNING_TO_DOCUMENT_AREA";
				Assembler.PopupException(msg);
			}
		}
		public void ChartFormEventsToChartFormManagerAttach() {
			this.ChartControl.RangeBar.OnValueMinChanged						+= new EventHandler<RangeArgs<DateTime>>(this.ChartFormManager.InterformEventsConsumer.ChartRangeBar_OnAnyValueChanged);
			this.ChartControl.RangeBar.OnValueMaxChanged						+= new EventHandler<RangeArgs<DateTime>>(this.ChartFormManager.InterformEventsConsumer.ChartRangeBar_OnAnyValueChanged);
			this.ChartControl.RangeBar.OnValuesMinAndMaxChanged					+= new EventHandler<RangeArgs<DateTime>>(this.ChartFormManager.InterformEventsConsumer.ChartRangeBar_OnAnyValueChanged);
			this.ChartControl.OnChartSettingsChanged_containerShouldSerialize_ChartFormDataSnapshot	+= new EventHandler<EventArgs>(this.chartControl_OnChartSettingsChanged_containerShouldSerialize_ChartFormDataSnapshot_copyMultiSplitterDictionaries);
			this.ChartControl.OnContextScriptChanged_containerShouldSerialize	+= new EventHandler<EventArgs>(this.chartControl_OnContextScriptChanged_containerShouldSerialize);
			this.ChartControl.OnPumpPaused										+= new EventHandler<EventArgs>(this.chartControl_OnPumpPaused);
			this.ChartControl.OnPumpUnPaused									+= new EventHandler<EventArgs>(this.chartControl_OnPumpUnPaused);
		}

		public void ChartFormEventsToChartFormManagerDetach() {
			this.ChartControl.RangeBar.OnValueMinChanged						-= new EventHandler<RangeArgs<DateTime>>(this.ChartFormManager.InterformEventsConsumer.ChartRangeBar_OnAnyValueChanged);
			this.ChartControl.RangeBar.OnValueMaxChanged						-= new EventHandler<RangeArgs<DateTime>>(this.ChartFormManager.InterformEventsConsumer.ChartRangeBar_OnAnyValueChanged);
			this.ChartControl.RangeBar.OnValuesMinAndMaxChanged					-= new EventHandler<RangeArgs<DateTime>>(this.ChartFormManager.InterformEventsConsumer.ChartRangeBar_OnAnyValueChanged);
			this.ChartControl.OnChartSettingsChanged_containerShouldSerialize_ChartFormDataSnapshot	-= new EventHandler<EventArgs>(this.chartControl_OnChartSettingsChanged_containerShouldSerialize_ChartFormDataSnapshot_copyMultiSplitterDictionaries);
			this.ChartControl.OnContextScriptChanged_containerShouldSerialize	-= new EventHandler<EventArgs>(this.chartControl_OnContextScriptChanged_containerShouldSerialize);
			this.ChartControl.OnPumpPaused										-= new EventHandler<EventArgs>(this.chartControl_OnPumpPaused);
			this.ChartControl.OnPumpUnPaused									-= new EventHandler<EventArgs>(this.chartControl_OnPumpUnPaused);
		}

		void chartControl_OnContextScriptChanged_containerShouldSerialize(object sender, EventArgs e) {
			if (this.ChartFormManager.Strategy == null) {
				string msg = "I_INVOKED_YOU_FROM_REPORTER_NOT_POSSIBLE_STRATEGY_DISAPPEARED_NOW";
				Assembler.PopupException(msg);
			}
			this.ChartFormManager.Strategy.Serialize();
		}
		void chartControl_OnChartSettingsChanged_containerShouldSerialize_ChartFormDataSnapshot_copyMultiSplitterDictionaries(object sender, EventArgs e) {
			if (this.ChartFormManager.ContextCurrentChartOrStrategy.Symbol != this.ChartFormManager.Executor.Bars.Symbol) {
				string msg = "YOU_ARE_COMING_FROM_SyncBarsIdentDueToSymbolRename__ChartShadow_COULDNT_HANDLE_ITS_OWN_CONTEXT__DO_YOU_NEED/KNOW_HOW_TO_REFACTOR?";
				this.ChartFormManager.ContextCurrentChartOrStrategy.Symbol  = this.ChartFormManager.Executor.Bars.Symbol;
			}
			this.ChartFormManager.DataSnapshot.ChartSettingsName = this.ChartControl.ChartSettings.Name;

			this.ChartFormManager.DataSnapshot.MultiSplitterRowsPropertiesByPanelName = this.ChartControl.ChartSettings.MultiSplitterRowsPropertiesByPanelName_tunnelled;
			this.ChartFormManager.DataSnapshot.MultiSplitterColumnsPropertiesByPanelName = this.ChartControl.ChartSettings.MultiSplitterColumnsPropertiesByPanelName_tunnelled;

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
		public void PrintQuoteTimestamp_onStrategyTriggeringButton_beforeExecution_switchToGuiThread(Quote quote) {
			if (base.InvokeRequired) {
				//DEADLOCK#1 - happens when DdeMessagePump thread wants to switch to GUI thread; switching to GUI thread via trampoline Task releases this method from held in GuiMessageQueue
				//Task deadlockOtherwize = new Task(delegate {
					base.BeginInvoke((MethodInvoker)delegate { this.PrintQuoteTimestamp_onStrategyTriggeringButton_beforeExecution_switchToGuiThread(quote); });
				//});
				//deadlockOtherwize.Start();
				return;
			}
			//v1 I_NEED_MY_CONTROLS_VISIBLE_AT_ALL_TIMES button gets too wide and gets hidden when ChartForm is narrow
			string btnText = this.ChartFormManager.StreamingButtonIdent;
			//if (quote != null) btnText += quote.StreamingButtonIdent;
			this.BtnStreamingTriggersScript.Text = btnText;

			//v2 separate label that gets squeezed
			//if (this.btnQuoteTimingRealtime.Visible == false) return;
			if (quote == null) {
				if (this.ChartFormManager.Executor.DataSource_fromBars.StreamingAdapter == null) {
					string msg = "I_REFUSE_TO_PRINT_QUOTE_TIMESTAMP this.ChartFormManager.Executor.DataSource_fromBars.StreamingAdapter==null";
					Assembler.PopupException(msg);
					return;
				}
				if (this.ChartFormManager.Executor.Bars == null) {
					string msg = "I_REFUSE_TO_PRINT_QUOTE_TIMESTAMP this.ChartFormManager.Executor.Bars==null";
					Assembler.PopupException(msg);
					return;
				}
				quote = this.ChartFormManager.Executor.DataSource_fromBars.StreamingAdapter.StreamingDataSnapshot.GetQuoteLast_forSymbol_nullUnsafe(this.ChartFormManager.Executor.Bars.Symbol);
			}
			if (quote == null) return;
			this.btnQuoteTimingRealtime.Text = quote.QuoteTiming_localRemoteLeft;
		}
		public void PopulateBtnStreamingTriggersScript_afterBarsLoaded() {
			DataSource ds = this.ChartFormManager.Executor.DataSource_fromBars;
			if (ds.StreamingAdapter == null) {
				//TOO_LONG_TITLE_MAKES_BUTTON_DISAPPEAR this.BtnStreamingTriggersScript.Text = "DataSource[" + ds + "]:Streaming[" + StreamingAdapter.NO_STREAMING_ADAPTER + "]";
				this.BtnStreamingTriggersScript.Text = this.ChartFormManager.StreamingButtonIdent;
				this.mniSubscribedToStreamingAdapterQuotesBars.Text = "NOT Subscribed: edit DataSource > attach StreamingAdapter";
			} else {
				this.PrintQuoteTimestamp_onStrategyTriggeringButton_beforeExecution_switchToGuiThread(null);
			}
			// "AfterBarsLoaded" implies Executor.SetBars() has already initialized this.ChartFormManager.Executor.DataSource
			this.populateCtxMniBars_streamingConnectionState_orange();
			this.mnitlbSpreadGeneratorPct.TextRight = this.ChartFormManager.Executor.SpreadPips + " pips";
		}
		void populateCtxMniBars_streamingConnectionState_orange() {
			if (this.ChartFormManager.ContextCurrentChartOrStrategy.DownstreamSubscribed) {
				this.mniSubscribedToStreamingAdapterQuotesBars.Checked = true;
				this.mniSubscribedToStreamingAdapterQuotesBars.BackColor = SystemColors.Control;
				this.DdbBars.BackColor = SystemColors.Control;

				DataSource dataSource = this.ChartFormManager.Executor.DataSource_fromBars;
				string mniSubscribedText = "Subscribed to [" + dataSource.StreamingAdapterName + "]";
				mniSubscribedText += dataSource.StreamingAdapter != null ? "[" + dataSource.StreamingAdapter.UpstreamConnectionState + "]" : "[StreamingAdapter_NULL]";
				this.mniSubscribedToStreamingAdapterQuotesBars.Text = mniSubscribedText;
			} else {
				this.mniSubscribedToStreamingAdapterQuotesBars.Checked = false;
				this.mniSubscribedToStreamingAdapterQuotesBars.BackColor = Color.LightSalmon;
				this.DdbBars.BackColor = Color.LightSalmon;

				DataSource dataSource = this.ChartFormManager.Executor.DataSource_fromBars;
				string mniSubscribedText = "NOT Subscribed to [" + dataSource.StreamingAdapterName + "]";
				mniSubscribedText += dataSource.StreamingAdapter != null ? "[" + dataSource.StreamingAdapter.UpstreamConnectionState + "]" : "[StreamingAdapter_NULL]";
				this.mniSubscribedToStreamingAdapterQuotesBars.Text = mniSubscribedText;
			}
		}

		public void AbsorbContextBarsToGui() {
			if (base.InvokeRequired) {
				base.BeginInvoke((MethodInvoker)delegate { this.AbsorbContextBarsToGui(); });
				return;
			}

			StreamingAdapter streaming = this.ChartFormManager.Executor.DataSource_fromBars.StreamingAdapter;
			Bitmap iconCanBeNull = streaming != null ? streaming.Icon : null;
			this.BtnStreamingTriggersScript.Image = iconCanBeNull; 			// NO_I_WANT_ABSENCE_OF_STREAMING_TO_CLEAR_PREVIOUS_BARS_IN_CHART_AFTER_CHANGING_SYMBOL_FOR_CHART if (iconCanBeNull != null) {

			Bars barsClickedUpstack = this.ChartFormManager.Executor.Bars;
			this.mniBarsStoredScaleInterval.Text = barsClickedUpstack != null
				? barsClickedUpstack.ScaleInterval.ToString() + " Minimun"
				: "[MIN_SCALEINTERVAL_UNKNOWN]";
			this.mniBarsSymbolDataSource.Text = barsClickedUpstack.SymbolAndDataSource;

			// WAS_METHOD_PARAMETER_BUT_ACCESSIBLE_LIKE_THIS__NULL_CHECK_DONE_UPSTACK
			ContextChart ctxChartOrScript = this.ChartFormManager.ContextCurrentChartOrStrategy;
			this.ChartControl.CtxChart = ctxChartOrScript;
			
			if (ctxChartOrScript.ShowRangeBar) {
				this.ChartControl.RangeBarCollapsed = false; 
				this.mniShowBarRange.Checked = true;
			} else {
				this.ChartControl.RangeBarCollapsed = true; 
				this.mniShowBarRange.Checked = false;
			}

			this.PopulateBtnStreamingTriggersScript_afterBarsLoaded();

			this.BtnStreamingTriggersScript.Checked = ctxChartOrScript.StreamingIsTriggeringScript;

			ContextScript ctxScript = ctxChartOrScript as ContextScript;
			if (ctxScript != null) {
				this.mniBacktestOnTriggeringYesWhenNotSubscribed				.Checked = ctxScript.BacktestOnTriggeringYesWhenNotSubscribed;
				this.mniBacktestOnDataSourceSaved								.Checked = ctxScript.BacktestOnDataSourceSaved;	// looks redundant here
				this.mniBacktestOnRestart										.Checked = ctxScript.BacktestOnRestart;
				this.mniBacktestOnSelectorsChange								.Checked = ctxScript.BacktestOnSelectorsChange;
				this.mniBacktestAfterSubscribed									.Checked = ctxScript.BacktestAfterSubscribed;
				this.BtnStrategyEmittingOrders									.Checked = ctxScript.StrategyEmittingOrders;
				this.mniMinimizeAllReportersGuiExtensiveForTheDurationOfLiveSim .Checked = ctxScript.MinimizeGuiExtensiveExecutionAllReportersForTheDurationOfLiveSim;

				this.mniBacktestOnTriggeringYesWhenNotSubscribed				.Enabled = true;
				this.mniBacktestOnDataSourceSaved								.Enabled = true;
				this.mniBacktestOnRestart										.Enabled = true;
				this.mniBacktestOnSelectorsChange								.Enabled = true;
				this.mniBacktestAfterSubscribed									.Enabled = true;
				this.BtnStrategyEmittingOrders									.Enabled = true;
				this.mniMinimizeAllReportersGuiExtensiveForTheDurationOfLiveSim .Enabled = true;

				this.BtnStreamingTriggersScript									.Enabled = true;
			} else {
				this.mniBacktestOnTriggeringYesWhenNotSubscribed				.Checked = false;
				this.mniBacktestOnDataSourceSaved								.Checked = false;
				this.mniBacktestOnRestart										.Checked = false;
				this.mniBacktestOnSelectorsChange								.Checked = false;
				this.mniBacktestAfterSubscribed									.Checked = false;
				this.BtnStrategyEmittingOrders									.Checked = false;
				this.mniMinimizeAllReportersGuiExtensiveForTheDurationOfLiveSim .Checked = false;

				this.mniBacktestOnTriggeringYesWhenNotSubscribed				.Enabled = false;
				this.mniBacktestOnDataSourceSaved								.Enabled = false;
				this.mniBacktestOnRestart										.Enabled = false;
				this.mniBacktestOnSelectorsChange								.Enabled = false;
				this.mniBacktestAfterSubscribed									.Enabled = false;
				this.BtnStrategyEmittingOrders									.Enabled = false;
				this.mniMinimizeAllReportersGuiExtensiveForTheDurationOfLiveSim .Enabled = false;
				
				this.BtnStreamingTriggersScript									.Enabled = false;
			}

			this.Propagate_contextChartOrScript_toLTB(ctxChartOrScript);
			this.PopulateBtnStreamingTriggersScript_afterBarsLoaded();
		}
		
		public void Propagate_contextChartOrScript_toLTB(ContextChart ctxChartOrScript) {
			this.mnitlbMinutes	.InputFieldValue = "";	// otherwize it holds "0.0005" initialized in MenuItemLabeledTextBox.ctor()
			this.mnitlbDaily	.InputFieldValue = "";	// otherwize it holds "0.0005" initialized in MenuItemLabeledTextBox.ctor()
			this.mnitlbHourly	.InputFieldValue = "";	// otherwize it holds "0.0005" initialized in MenuItemLabeledTextBox.ctor()
			this.mnitlbWeekly	.InputFieldValue = "";	// otherwize it holds "0.0005" initialized in MenuItemLabeledTextBox.ctor()
			this.mnitlbMonthly	.InputFieldValue = "";	// otherwize it holds "0.0005" initialized in MenuItemLabeledTextBox.ctor()
			//this.mnitlbQuarterly	.InputFieldValue = "";	// otherwize it holds "0.0005" initialized in MenuItemLabeledTextBox.ctor()
			this.mnitlbYearly	.InputFieldValue = "";	// otherwize it holds "0.0005" initialized in MenuItemLabeledTextBox.ctor()

			MenuItemLabeledTextBox mnitlbForScale = null;
			switch (ctxChartOrScript.ScaleInterval.Scale) {
				case BarScale.Minute:		mnitlbForScale = this.mnitlbMinutes; break; 
				case BarScale.Hour:			mnitlbForScale = this.mnitlbDaily; break; 
				case BarScale.Daily:		mnitlbForScale = this.mnitlbHourly; break; 
				case BarScale.Weekly:		mnitlbForScale = this.mnitlbWeekly; break; 
				case BarScale.Monthly:		mnitlbForScale = this.mnitlbMonthly; break; 
				//case BarScale.Quarterly		mnitlbForScale = this.mnitlbQuarterly; break;
				case BarScale.Yearly:		mnitlbForScale = this.mnitlbYearly; break;
				case BarScale.Unknown: 
					string msg = "TODO: figure out why deserialized / userSelected strategyClicked[" + this.ChartFormManager.Executor.Strategy
						+ "].ScriptContextCurrent.ScaleInterval[" + ctxChartOrScript.ScaleInterval + "] has BarScale.Unknown #4";
					Assembler.PopupException(msg);
					break;
				default:
					string msg2 = "SCALE_UNHANDLED_NO_TEXTBOX_TO_POPULATE " + ctxChartOrScript.ScaleInterval.Scale;
					Assembler.PopupException(msg2);
					break;
			}
				
			if (mnitlbForScale != null) {
				mnitlbForScale.InputFieldValue = ctxChartOrScript.ScaleInterval.Interval.ToString();
				mnitlbForScale.BackColor = Color.Gainsboro;
			}

			this.mniShowBarRange.Checked = ctxChartOrScript.ShowRangeBar;
			switch (ctxChartOrScript.DataRange.Range) {
				case BarRange.AllData:
					this.mnitlbShowLastBars.InputFieldValue = "";
					this.mnitlbShowLastBars.BackColor = Color.White;
					break;
				case BarRange.DateRange:
					this.ChartControl.RangeBar.ValueMin = ctxChartOrScript.DataRange.DateFrom; 
					this.ChartControl.RangeBar.ValueMax = ctxChartOrScript.DataRange.DateTill; 
					this.mnitlbShowLastBars.InputFieldValue = "";
					this.mnitlbShowLastBars.BackColor = Color.White;
					break;
				case BarRange.RecentBars:
					this.mnitlbShowLastBars.InputFieldValue = ctxChartOrScript.DataRange.RecentBars.ToString();
					this.mnitlbShowLastBars.BackColor = Color.Gainsboro;
					//this.mniShowBarRange.Checked = false;
					break;
				default:
					string msg = "DATE_RANGE_UNHANDLED_RECENT_TIMEUNITS_NYI " + ctxChartOrScript.DataRange;
					Assembler.PopupException(msg);
					break;
			}
			this.ChartControl.RangeBarCollapsed = !this.mniShowBarRange.Checked;

			ContextScript ctxScript = ctxChartOrScript as ContextScript;
			if (ctxScript != null) {
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
				this.mnitlbSpreadGeneratorPct.TextRight = this.ChartFormManager.Executor.SpreadPips + " pips";

				if (this.ChartFormManager.MainForm.DockPanel.ActiveDocument == null) {
					string msg = "IM_LOADING_WORKSPACE_WITHOUT_STRATEGY_LOADED_YET";
					#if DEBUG_HEAVY
					Assembler.PopupException(msg, null, false);
					#endif
					return;
				}
			}

			if (Assembler.InstanceInitialized.MainForm_dockFormsFullyDeserialized_layoutComplete) {
				ChartForm chartForm_nullUnsafe = this.ChartFormManager.MainForm.ChartFormActive_nullUnsafe;
				if (chartForm_nullUnsafe == null) {
					string msg2 = "IM_LOADING_WORKSPACE_WITHOUT_STRATEGY_LOADED_YET WE_ARE_HERE_WHEN_I_SWITCH_ACTIVE_DOCUMENT_TAB_FROM_DataSourceEditor_TO_ChartForm";
					//#if DEBUG_HEAVY
					Assembler.PopupException(msg2, null, false);
					//#endif
				} else {
					#if DEBUG	// PARANOID TEST
					if (chartForm_nullUnsafe != this) {
						string msg = "LEFT_FOR_QUIK_LIVESIM_COMPAT_TEST WHY___WE_ARE_HERE_WHEN_WE_CHANGE_TIMEFRAME_OF_CHART I_STARTED_LIVESIM_FOR_NON_ACTIVE_CHART";
						Assembler.PopupException(msg, null, false);
					}
					#endif
				}
			}
			this.ChartFormManager.PopulateThroughMainForm_symbolStrategyTree_andSliders();
			this.PropagateSelectors_disabledIfStreaming_forCurrentChart();
		}
		public void PropagateSelectors_disabledIfStreaming_forCurrentChart() {
			Strategy strategyClicked = this.ChartFormManager.Strategy;
			if (strategyClicked == null) return;

			//do not disturb a streaming chart with selector's changes (disable selectors if streaming; for script-free charts strategy=null)
			//v1 WASNT_ENABLED_FOR_NULL_STREAMING_PROVIDER bool enableForNonStreaming = !strategyClicked.ScriptContextCurrent.IsStreamingTriggeringScript;
			bool enableForNonStreaming = true;
			
			//DataSourcesForm.Instance.DataSourcesTree.Enabled = enableForNonStreaming;
			
			this.mnitlbMinutes								.Enabled = enableForNonStreaming;
			this.mnitlbDaily								.Enabled = enableForNonStreaming;
			this.mnitlbHourly								.Enabled = enableForNonStreaming;
			this.mnitlbWeekly								.Enabled = enableForNonStreaming;
			this.mnitlbMonthly								.Enabled = enableForNonStreaming;
			//this.mnitlbQuarterly							.Enabled = enableForNonStreaming;
			this.mnitlbYearly								.Enabled = enableForNonStreaming;
			this.mnitlbShowLastBars							.Enabled = enableForNonStreaming;
			this.mnitlbPositionSizeDollarsEachTradeConstant	.Enabled = enableForNonStreaming;
			this.mnitlbPositionSizeSharesConstantEachTrade	.Enabled = enableForNonStreaming;

			this.ctxStrokesPopulate_orSelectCurrent();
		}

		public void Initialize(Strategy strategy = null) {
			this.mniStrategyName.Text = strategy == null ? "[NO_STRATEGY_LOADED]" : strategy.NameAndDll;

			bool containsStrategy = strategy != null;
			bool strategyActivatedFromDll = (strategy != null && strategy.ActivatedFromDll) ? true : false;

			this.DdbStrategy.Enabled = containsStrategy;
			this.DdbBacktest.Enabled = containsStrategy;
			//this.btnStrategyEmittingOrders.Enabled = containsStrategy;

			this.MniShowSourceCodeEditor.Enabled = containsStrategy;
			if (containsStrategy) this.MniShowSourceCodeEditor.Enabled = !strategyActivatedFromDll;

			this.mniStrategyRemove	.Enabled = containsStrategy;
			this.MniShowLivesim		.Enabled = containsStrategy;	// disables F-key from being clicked despite whole "Strategy" parent ctx is disabled
			this.MniShowCorrelator	.Enabled = containsStrategy;	// disables F-key from being clicked despite whole "Strategy" parent ctx is disabled
			this.MniShowSequencer	.Enabled = containsStrategy;	// disables F-key from being clicked despite whole "Strategy" parent ctx is disabled
		}
		public void AppendReportersMenuItems(ToolStripItem[] toolStripItems) {
			this.ctxStrategy.Items.Add(this.TssReportersBelowMe);	// if not added then we didn't initialize!
			this.ctxStrategy.Items.AddRange(toolStripItems);
		}

		private void lblEmptySpring_catchingClickEvents_Click(object sender, EventArgs e) {

		}
	}
}