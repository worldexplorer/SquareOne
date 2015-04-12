using System;

using Sq1.Widgets;

namespace Sq1.Gui.Forms {
	partial class ChartForm : DockContentImproved {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.ChartControl = new Sq1.Charting.ChartControl();
			this.ctxBacktest = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.mniStrokes = new System.Windows.Forms.ToolStripMenuItem();
			this.ctxStrokesForQuoteGenerator = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
			this.mnitlbSpreadGeneratorPct = new Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox();
			this.mniFillOutsideQuoteSpreadParanoidCheckThrow = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			this.mniBacktestOnDataSourceSaved = new System.Windows.Forms.ToolStripMenuItem();
			this.mniBacktestOnRestart = new System.Windows.Forms.ToolStripMenuItem();
			this.mniBacktestOnSelectorsChange = new System.Windows.Forms.ToolStripMenuItem();
			this.mniBacktestNow = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.mnitlbPositionSizeSharesConstantEachTrade = new Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox();
			this.mnitlbPositionSizeDollarsEachTradeConstant = new Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox();
			this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
			this.MniShowLivesim = new System.Windows.Forms.ToolStripMenuItem();
			this.DdbBacktest = new System.Windows.Forms.ToolStripDropDownButton();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.mniStrategyRemove = new System.Windows.Forms.ToolStripMenuItem();
			this.MniShowSourceCodeEditor = new System.Windows.Forms.ToolStripMenuItem();
			this.MniShowSequencer = new System.Windows.Forms.ToolStripMenuItem();
			this.ctxStrategy = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.mniStrategyContextLoad = new System.Windows.Forms.ToolStripMenuItem();
			this.DdbStrategy = new System.Windows.Forms.ToolStripDropDownButton();
			this.TssReportersBelowMe = new System.Windows.Forms.ToolStripSeparator();
			this.statusStrip = new System.Windows.Forms.StatusStrip();
			this.DdbBars = new System.Windows.Forms.ToolStripDropDownButton();
			this.ctxBars = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.mniSubscribedToStreamingAdapterQuotesBars = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
			this.mnitlbYearly = new Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox();
			this.mnitlbMonthly = new Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox();
			this.mnitlbWeekly = new Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox();
			this.mnitlbDaily = new Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox();
			this.mnitlbHourly = new Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox();
			this.mnitlbMinutes = new Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox();
			this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
			this.mniBarsStoredScaleInterval = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.mnitlbShowLastBars = new Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox();
			this.mniShowBarRange = new System.Windows.Forms.ToolStripMenuItem();
			this.TsiProgressBarETA = new Sq1.Widgets.ProgressBacktestETA.ToolStripItemProgressBarETA();
			this.btnStreamingTriggersScript = new System.Windows.Forms.ToolStripButton();
			this.btnStrategyEmittingOrders = new System.Windows.Forms.ToolStripButton();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.ctxBacktest.SuspendLayout();
			this.ctxStrategy.SuspendLayout();
			this.statusStrip.SuspendLayout();
			this.ctxBars.SuspendLayout();
			this.SuspendLayout();
			// 
			// ChartControl
			// 
			this.ChartControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.ChartControl.Location = new System.Drawing.Point(0, 0);
			this.ChartControl.Name = "ChartControl";
			this.ChartControl.PaintAllowedDuringLivesimOrAfterBacktestFinished = true;
			this.ChartControl.RangeBarCollapsed = false;
			this.ChartControl.Size = new System.Drawing.Size(708, 287);
			this.ChartControl.TabIndex = 0;
			// 
			// ctxBacktest
			// 
			this.ctxBacktest.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mniStrokes,
            this.toolStripSeparator8,
            this.mnitlbSpreadGeneratorPct,
            this.mniFillOutsideQuoteSpreadParanoidCheckThrow,
            this.toolStripSeparator4,
            this.mniBacktestOnDataSourceSaved,
            this.mniBacktestOnRestart,
            this.mniBacktestOnSelectorsChange,
            this.mniBacktestNow,
            this.toolStripSeparator1,
            this.mnitlbPositionSizeSharesConstantEachTrade,
            this.mnitlbPositionSizeDollarsEachTradeConstant});
			this.ctxBacktest.Name = "ctxBacktest";
			this.ctxBacktest.Size = new System.Drawing.Size(308, 226);
			this.ctxBacktest.Opening += new System.ComponentModel.CancelEventHandler(this.ctxBacktest_Opening);
			// 
			// mniStrokes
			// 
			this.mniStrokes.DropDown = this.ctxStrokesForQuoteGenerator;
			this.mniStrokes.Name = "mniStrokes";
			this.mniStrokes.Size = new System.Drawing.Size(307, 22);
			this.mniStrokes.Text = "QuotesGenerator: [4strokesOHLC]";
			// 
			// ctxStrokesForQuoteGenerator
			// 
			this.ctxStrokesForQuoteGenerator.Name = "ctxStrokesForQuoteGenerator";
			this.ctxStrokesForQuoteGenerator.OwnerItem = this.mniStrokes;
			this.ctxStrokesForQuoteGenerator.Size = new System.Drawing.Size(61, 4);
			// 
			// toolStripSeparator8
			// 
			this.toolStripSeparator8.Name = "toolStripSeparator8";
			this.toolStripSeparator8.Size = new System.Drawing.Size(304, 6);
			// 
			// mnitlbSpreadGeneratorPct
			// 
			this.mnitlbSpreadGeneratorPct.BackColor = System.Drawing.Color.Transparent;
			this.mnitlbSpreadGeneratorPct.InputFieldAlignedRight = false;
			this.mnitlbSpreadGeneratorPct.InputFieldEditable = true;
			this.mnitlbSpreadGeneratorPct.InputFieldOffsetX = 100;
			this.mnitlbSpreadGeneratorPct.InputFieldValue = "0.005";
			this.mnitlbSpreadGeneratorPct.InputFieldWidth = 60;
			this.mnitlbSpreadGeneratorPct.Name = "mnitlbSpreadGeneratorPct";
			this.mnitlbSpreadGeneratorPct.Size = new System.Drawing.Size(163, 21);
			this.mnitlbSpreadGeneratorPct.TextLeft = "Spread %price";
			this.mnitlbSpreadGeneratorPct.TextLeftOffsetX = 0;
			this.mnitlbSpreadGeneratorPct.TextRed = false;
			this.mnitlbSpreadGeneratorPct.TextLeftWidth = 84;
			this.mnitlbSpreadGeneratorPct.UserTyped += new System.EventHandler<Sq1.Widgets.LabeledTextBox.LabeledTextBoxUserTypedArgs>(this.mnitlbSpreadGeneratorPct_UserTyped);
			// 
			// mniFillOutsideQuoteSpreadParanoidCheckThrow
			// 
			this.mniFillOutsideQuoteSpreadParanoidCheckThrow.CheckOnClick = true;
			this.mniFillOutsideQuoteSpreadParanoidCheckThrow.Name = "mniFillOutsideQuoteSpreadParanoidCheckThrow";
			this.mniFillOutsideQuoteSpreadParanoidCheckThrow.Size = new System.Drawing.Size(307, 22);
			this.mniFillOutsideQuoteSpreadParanoidCheckThrow.Text = "OutsiteQuote Fills Reported";
			this.mniFillOutsideQuoteSpreadParanoidCheckThrow.Click += new System.EventHandler(this.mniOutsideQuoteFillCheckThrow_Click);
			// 
			// toolStripSeparator4
			// 
			this.toolStripSeparator4.Name = "toolStripSeparator4";
			this.toolStripSeparator4.Size = new System.Drawing.Size(304, 6);
			// 
			// mniBacktestOnDataSourceSaved
			// 
			this.mniBacktestOnDataSourceSaved.Checked = true;
			this.mniBacktestOnDataSourceSaved.CheckOnClick = true;
			this.mniBacktestOnDataSourceSaved.CheckState = System.Windows.Forms.CheckState.Checked;
			this.mniBacktestOnDataSourceSaved.Name = "mniBacktestOnDataSourceSaved";
			this.mniBacktestOnDataSourceSaved.Size = new System.Drawing.Size(307, 22);
			this.mniBacktestOnDataSourceSaved.Text = "Backtest On DataSource Saved";
			this.mniBacktestOnDataSourceSaved.Click += new System.EventHandler(this.mniBacktestOnEveryChange_Click);
			// 
			// mniBacktestOnRestart
			// 
			this.mniBacktestOnRestart.Checked = true;
			this.mniBacktestOnRestart.CheckOnClick = true;
			this.mniBacktestOnRestart.CheckState = System.Windows.Forms.CheckState.Checked;
			this.mniBacktestOnRestart.Name = "mniBacktestOnRestart";
			this.mniBacktestOnRestart.Size = new System.Drawing.Size(307, 22);
			this.mniBacktestOnRestart.Text = "Backtest On Application Restart";
			this.mniBacktestOnRestart.Click += new System.EventHandler(this.mniBacktestOnRestart_Click);
			// 
			// mniBacktestOnSelectorsChange
			// 
			this.mniBacktestOnSelectorsChange.Checked = true;
			this.mniBacktestOnSelectorsChange.CheckOnClick = true;
			this.mniBacktestOnSelectorsChange.CheckState = System.Windows.Forms.CheckState.Checked;
			this.mniBacktestOnSelectorsChange.Name = "mniBacktestOnSelectorsChange";
			this.mniBacktestOnSelectorsChange.Size = new System.Drawing.Size(307, 22);
			this.mniBacktestOnSelectorsChange.Text = "Backtest On Bars/ScriptParameters Changed";
			this.mniBacktestOnSelectorsChange.Click += new System.EventHandler(this.mniBacktestOnEveryChange_Click);
			// 
			// mniBacktestNow
			// 
			this.mniBacktestNow.Name = "mniBacktestNow";
			this.mniBacktestNow.ShortcutKeys = System.Windows.Forms.Keys.F5;
			this.mniBacktestNow.Size = new System.Drawing.Size(307, 22);
			this.mniBacktestNow.Text = "Backtest Now";
			this.mniBacktestNow.Click += new System.EventHandler(this.mniBacktestNow_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(304, 6);
			// 
			// mnitlbPositionSizeSharesConstantEachTrade
			// 
			this.mnitlbPositionSizeSharesConstantEachTrade.BackColor = System.Drawing.Color.Transparent;
			this.mnitlbPositionSizeSharesConstantEachTrade.InputFieldAlignedRight = false;
			this.mnitlbPositionSizeSharesConstantEachTrade.InputFieldEditable = true;
			this.mnitlbPositionSizeSharesConstantEachTrade.InputFieldOffsetX = 80;
			this.mnitlbPositionSizeSharesConstantEachTrade.InputFieldValue = "";
			this.mnitlbPositionSizeSharesConstantEachTrade.InputFieldWidth = 85;
			this.mnitlbPositionSizeSharesConstantEachTrade.Name = "mnitlbPositionSizeSharesConstantEachTrade";
			this.mnitlbPositionSizeSharesConstantEachTrade.Size = new System.Drawing.Size(168, 21);
			this.mnitlbPositionSizeSharesConstantEachTrade.TextLeft = "Shares";
			this.mnitlbPositionSizeSharesConstantEachTrade.TextLeftOffsetX = 0;
			this.mnitlbPositionSizeSharesConstantEachTrade.TextRed = false;
			this.mnitlbPositionSizeSharesConstantEachTrade.TextLeftWidth = 43;
			this.mnitlbPositionSizeSharesConstantEachTrade.UserTyped += new System.EventHandler<Sq1.Widgets.LabeledTextBox.LabeledTextBoxUserTypedArgs>(this.mnitlbPositionSizeSharesConstantEachTrade_UserTyped);
			// 
			// mnitlbPositionSizeDollarsEachTradeConstant
			// 
			this.mnitlbPositionSizeDollarsEachTradeConstant.BackColor = System.Drawing.Color.Transparent;
			this.mnitlbPositionSizeDollarsEachTradeConstant.InputFieldAlignedRight = false;
			this.mnitlbPositionSizeDollarsEachTradeConstant.InputFieldEditable = true;
			this.mnitlbPositionSizeDollarsEachTradeConstant.InputFieldOffsetX = 80;
			this.mnitlbPositionSizeDollarsEachTradeConstant.InputFieldValue = "";
			this.mnitlbPositionSizeDollarsEachTradeConstant.InputFieldWidth = 85;
			this.mnitlbPositionSizeDollarsEachTradeConstant.Name = "mnitlbPositionSizeDollarsEachTradeConstant";
			this.mnitlbPositionSizeDollarsEachTradeConstant.Size = new System.Drawing.Size(168, 21);
			this.mnitlbPositionSizeDollarsEachTradeConstant.TextLeft = "$$ Each Trade";
			this.mnitlbPositionSizeDollarsEachTradeConstant.TextLeftOffsetX = 0;
			this.mnitlbPositionSizeDollarsEachTradeConstant.TextRed = false;
			this.mnitlbPositionSizeDollarsEachTradeConstant.TextLeftWidth = 82;
			this.mnitlbPositionSizeDollarsEachTradeConstant.UserTyped += new System.EventHandler<Sq1.Widgets.LabeledTextBox.LabeledTextBoxUserTypedArgs>(this.mnitlbPositionSizeDollarsConstantEachTrade_UserTyped);
			// 
			// toolStripSeparator7
			// 
			this.toolStripSeparator7.Name = "toolStripSeparator7";
			this.toolStripSeparator7.Size = new System.Drawing.Size(304, 6);
			// 
			// MniShowLivesim
			// 
			this.MniShowLivesim.CheckOnClick = true;
			this.MniShowLivesim.Name = "MniShowLivesim";
			this.MniShowLivesim.ShortcutKeys = System.Windows.Forms.Keys.F9;
			this.MniShowLivesim.Size = new System.Drawing.Size(248, 22);
			this.MniShowLivesim.Text = "Show Live Simulator";
			this.MniShowLivesim.Click += new System.EventHandler(this.mniShowLivesim_Click);
			// 
			// DdbBacktest
			// 
			this.DdbBacktest.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.DdbBacktest.DropDown = this.ctxBacktest;
			this.DdbBacktest.Name = "DdbBacktest";
			this.DdbBacktest.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
			this.DdbBacktest.Size = new System.Drawing.Size(64, 20);
			this.DdbBacktest.Text = "Backtest";
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(245, 6);
			// 
			// mniStrategyRemove
			// 
			this.mniStrategyRemove.Enabled = false;
			this.mniStrategyRemove.Name = "mniStrategyRemove";
			this.mniStrategyRemove.Size = new System.Drawing.Size(248, 22);
			this.mniStrategyRemove.Text = "NYI Remove Strategy From Chart";
			// 
			// MniShowSourceCodeEditor
			// 
			this.MniShowSourceCodeEditor.CheckOnClick = true;
			this.MniShowSourceCodeEditor.Name = "MniShowSourceCodeEditor";
			this.MniShowSourceCodeEditor.ShortcutKeys = System.Windows.Forms.Keys.F4;
			this.MniShowSourceCodeEditor.Size = new System.Drawing.Size(248, 22);
			this.MniShowSourceCodeEditor.Text = "Show Source Code Editor";
			this.MniShowSourceCodeEditor.Click += new System.EventHandler(this.mniShowSourceCodeEditor_Click);
			// 
			// MniShowSequencer
			// 
			this.MniShowSequencer.CheckOnClick = true;
			this.MniShowSequencer.Name = "MniShowSequencer";
			this.MniShowSequencer.ShortcutKeys = System.Windows.Forms.Keys.F8;
			this.MniShowSequencer.Size = new System.Drawing.Size(248, 22);
			this.MniShowSequencer.Text = "Show Sequencer";
			this.MniShowSequencer.Click += new System.EventHandler(this.mniShowSequencer_Click);
			// 
			// ctxStrategy
			// 
			this.ctxStrategy.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mniStrategyContextLoad,
            this.mniStrategyRemove,
            this.toolStripSeparator3,
            this.MniShowLivesim,
            this.MniShowSequencer,
            this.MniShowSourceCodeEditor});
			this.ctxStrategy.Name = "ctxPositionSize";
			this.ctxStrategy.Size = new System.Drawing.Size(249, 120);
			this.ctxStrategy.Opening += new System.ComponentModel.CancelEventHandler(this.ctxStrategy_Opening);
			// 
			// mniStrategyContextLoad
			// 
			this.mniStrategyContextLoad.Enabled = false;
			this.mniStrategyContextLoad.Name = "mniStrategyContextLoad";
			this.mniStrategyContextLoad.Size = new System.Drawing.Size(248, 22);
			this.mniStrategyContextLoad.Text = "NYI Load Script Context...";
			// 
			// DdbStrategy
			// 
			this.DdbStrategy.DropDown = this.ctxStrategy;
			this.DdbStrategy.Name = "DdbStrategy";
			this.DdbStrategy.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
			this.DdbStrategy.Size = new System.Drawing.Size(63, 20);
			this.DdbStrategy.Text = "Strategy";
			// 
			// TssReportersBelowMe
			// 
			this.TssReportersBelowMe.Name = "TssReportersBelowMe";
			this.TssReportersBelowMe.Size = new System.Drawing.Size(245, 6);
			// 
			// statusStrip
			// 
			this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.DdbBars,
            this.DdbStrategy,
            this.DdbBacktest,
            this.TsiProgressBarETA,
            this.btnStreamingTriggersScript,
            this.btnStrategyEmittingOrders});
			this.statusStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
			this.statusStrip.Location = new System.Drawing.Point(0, 286);
			this.statusStrip.Name = "statusStrip";
			this.statusStrip.Size = new System.Drawing.Size(708, 22);
			this.statusStrip.SizingGrip = false;
			this.statusStrip.TabIndex = 4;
			this.statusStrip.Text = "statusStrip1";
			// 
			// DdbBars
			// 
			this.DdbBars.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.DdbBars.DropDown = this.ctxBars;
			this.DdbBars.Name = "DdbBars";
			this.DdbBars.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
			this.DdbBars.Size = new System.Drawing.Size(42, 20);
			this.DdbBars.Text = "Bars";
			// 
			// ctxBars
			// 
			this.ctxBars.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mniSubscribedToStreamingAdapterQuotesBars,
            this.toolStripSeparator6,
            this.mnitlbYearly,
            this.mnitlbMonthly,
            this.mnitlbWeekly,
            this.mnitlbDaily,
            this.mnitlbHourly,
            this.mnitlbMinutes,
            this.toolStripSeparator5,
            this.mniBarsStoredScaleInterval,
            this.toolStripSeparator2,
            this.mnitlbShowLastBars,
            this.mniShowBarRange});
			this.ctxBars.Name = "ctxScaleInterval";
			this.ctxBars.Size = new System.Drawing.Size(252, 256);
			// 
			// mniSubscribedToStreamingAdapterQuotesBars
			// 
			this.mniSubscribedToStreamingAdapterQuotesBars.Checked = true;
			this.mniSubscribedToStreamingAdapterQuotesBars.CheckOnClick = true;
			this.mniSubscribedToStreamingAdapterQuotesBars.CheckState = System.Windows.Forms.CheckState.Checked;
			this.mniSubscribedToStreamingAdapterQuotesBars.Name = "mniSubscribedToStreamingAdapterQuotesBars";
			this.mniSubscribedToStreamingAdapterQuotesBars.Size = new System.Drawing.Size(251, 22);
			this.mniSubscribedToStreamingAdapterQuotesBars.Text = "Subscribed to [StreamingDerived]";
			this.mniSubscribedToStreamingAdapterQuotesBars.Click += new System.EventHandler(this.mniSubscribedToStreamingAdapterQuotesBars_Click);
			// 
			// toolStripSeparator6
			// 
			this.toolStripSeparator6.Name = "toolStripSeparator6";
			this.toolStripSeparator6.Size = new System.Drawing.Size(248, 6);
			// 
			// mnitlbYearly
			// 
			this.mnitlbYearly.BackColor = System.Drawing.Color.Transparent;
			this.mnitlbYearly.InputFieldAlignedRight = true;
			this.mnitlbYearly.InputFieldEditable = true;
			this.mnitlbYearly.InputFieldOffsetX = 4;
			this.mnitlbYearly.InputFieldValue = "";
			this.mnitlbYearly.InputFieldWidth = 40;
			this.mnitlbYearly.Name = "mnitlbYearly";
			this.mnitlbYearly.Size = new System.Drawing.Size(91, 21);
			this.mnitlbYearly.TextLeft = "Yearly";
			this.mnitlbYearly.TextLeftOffsetX = 47;
			this.mnitlbYearly.TextRed = false;
			this.mnitlbYearly.TextLeftWidth = 41;
			this.mnitlbYearly.UserTyped += new System.EventHandler<Sq1.Widgets.LabeledTextBox.LabeledTextBoxUserTypedArgs>(this.mnitlbAll_UserTyped);
			// 
			// mnitlbMonthly
			// 
			this.mnitlbMonthly.BackColor = System.Drawing.Color.Transparent;
			this.mnitlbMonthly.InputFieldAlignedRight = true;
			this.mnitlbMonthly.InputFieldEditable = true;
			this.mnitlbMonthly.InputFieldOffsetX = 4;
			this.mnitlbMonthly.InputFieldValue = "";
			this.mnitlbMonthly.InputFieldWidth = 40;
			this.mnitlbMonthly.Name = "mnitlbMonthly";
			this.mnitlbMonthly.Size = new System.Drawing.Size(104, 21);
			this.mnitlbMonthly.TextLeft = "Monthly";
			this.mnitlbMonthly.TextLeftOffsetX = 47;
			this.mnitlbMonthly.TextRed = false;
			this.mnitlbMonthly.TextLeftWidth = 54;
			this.mnitlbMonthly.UserTyped += new System.EventHandler<Sq1.Widgets.LabeledTextBox.LabeledTextBoxUserTypedArgs>(this.mnitlbAll_UserTyped);
			// 
			// mnitlbWeekly
			// 
			this.mnitlbWeekly.BackColor = System.Drawing.Color.Transparent;
			this.mnitlbWeekly.InputFieldAlignedRight = true;
			this.mnitlbWeekly.InputFieldEditable = true;
			this.mnitlbWeekly.InputFieldOffsetX = 4;
			this.mnitlbWeekly.InputFieldValue = "";
			this.mnitlbWeekly.InputFieldWidth = 40;
			this.mnitlbWeekly.Name = "mnitlbWeekly";
			this.mnitlbWeekly.Size = new System.Drawing.Size(97, 21);
			this.mnitlbWeekly.TextLeft = "Weekly";
			this.mnitlbWeekly.TextLeftOffsetX = 47;
			this.mnitlbWeekly.TextRed = false;
			this.mnitlbWeekly.TextLeftWidth = 47;
			this.mnitlbWeekly.UserTyped += new System.EventHandler<Sq1.Widgets.LabeledTextBox.LabeledTextBoxUserTypedArgs>(this.mnitlbAll_UserTyped);
			// 
			// mnitlbDaily
			// 
			this.mnitlbDaily.BackColor = System.Drawing.Color.Transparent;
			this.mnitlbDaily.InputFieldAlignedRight = true;
			this.mnitlbDaily.InputFieldEditable = true;
			this.mnitlbDaily.InputFieldOffsetX = 4;
			this.mnitlbDaily.InputFieldValue = "";
			this.mnitlbDaily.InputFieldWidth = 40;
			this.mnitlbDaily.Name = "mnitlbDaily";
			this.mnitlbDaily.Size = new System.Drawing.Size(85, 21);
			this.mnitlbDaily.TextLeft = "Daily";
			this.mnitlbDaily.TextLeftOffsetX = 47;
			this.mnitlbDaily.TextRed = false;
			this.mnitlbDaily.TextLeftWidth = 35;
			this.mnitlbDaily.UserTyped += new System.EventHandler<Sq1.Widgets.LabeledTextBox.LabeledTextBoxUserTypedArgs>(this.mnitlbAll_UserTyped);
			// 
			// mnitlbHourly
			// 
			this.mnitlbHourly.BackColor = System.Drawing.Color.Transparent;
			this.mnitlbHourly.InputFieldAlignedRight = true;
			this.mnitlbHourly.InputFieldEditable = true;
			this.mnitlbHourly.InputFieldOffsetX = 4;
			this.mnitlbHourly.InputFieldValue = "";
			this.mnitlbHourly.InputFieldWidth = 40;
			this.mnitlbHourly.Name = "mnitlbHourly";
			this.mnitlbHourly.Size = new System.Drawing.Size(95, 21);
			this.mnitlbHourly.TextLeft = "Hourly";
			this.mnitlbHourly.TextLeftOffsetX = 47;
			this.mnitlbHourly.TextRed = false;
			this.mnitlbHourly.TextLeftWidth = 45;
			this.mnitlbHourly.UserTyped += new System.EventHandler<Sq1.Widgets.LabeledTextBox.LabeledTextBoxUserTypedArgs>(this.mnitlbAll_UserTyped);
			// 
			// mnitlbMinutes
			// 
			this.mnitlbMinutes.BackColor = System.Drawing.Color.Transparent;
			this.mnitlbMinutes.InputFieldAlignedRight = true;
			this.mnitlbMinutes.InputFieldEditable = true;
			this.mnitlbMinutes.InputFieldOffsetX = 4;
			this.mnitlbMinutes.InputFieldValue = "";
			this.mnitlbMinutes.InputFieldWidth = 40;
			this.mnitlbMinutes.Name = "mnitlbMinutes";
			this.mnitlbMinutes.Size = new System.Drawing.Size(102, 21);
			this.mnitlbMinutes.TextLeft = "Minutes";
			this.mnitlbMinutes.TextLeftOffsetX = 47;
			this.mnitlbMinutes.TextRed = false;
			this.mnitlbMinutes.TextLeftWidth = 52;
			this.mnitlbMinutes.UserTyped += new System.EventHandler<Sq1.Widgets.LabeledTextBox.LabeledTextBoxUserTypedArgs>(this.mnitlbAll_UserTyped);
			// 
			// toolStripSeparator5
			// 
			this.toolStripSeparator5.Name = "toolStripSeparator5";
			this.toolStripSeparator5.Size = new System.Drawing.Size(248, 6);
			// 
			// mniBarsStoredScaleInterval
			// 
			this.mniBarsStoredScaleInterval.Name = "mniBarsStoredScaleInterval";
			this.mniBarsStoredScaleInterval.Size = new System.Drawing.Size(251, 22);
			this.mniBarsStoredScaleInterval.Text = "[5-Minutes] Minimum";
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(248, 6);
			// 
			// mnitlbShowLastBars
			// 
			this.mnitlbShowLastBars.BackColor = System.Drawing.Color.Transparent;
			this.mnitlbShowLastBars.InputFieldAlignedRight = true;
			this.mnitlbShowLastBars.InputFieldEditable = true;
			this.mnitlbShowLastBars.InputFieldOffsetX = 4;
			this.mnitlbShowLastBars.InputFieldValue = "";
			this.mnitlbShowLastBars.InputFieldWidth = 40;
			this.mnitlbShowLastBars.Name = "mnitlbShowLastBars";
			this.mnitlbShowLastBars.Size = new System.Drawing.Size(105, 21);
			this.mnitlbShowLastBars.TextLeft = "Last Bars";
			this.mnitlbShowLastBars.TextLeftOffsetX = 47;
			this.mnitlbShowLastBars.TextRed = false;
			this.mnitlbShowLastBars.TextLeftWidth = 55;
			this.mnitlbShowLastBars.UserTyped += new System.EventHandler<Sq1.Widgets.LabeledTextBox.LabeledTextBoxUserTypedArgs>(this.mnitlbShowLastBars_UserTyped);
			// 
			// mniShowBarRange
			// 
			this.mniShowBarRange.Checked = true;
			this.mniShowBarRange.CheckOnClick = true;
			this.mniShowBarRange.CheckState = System.Windows.Forms.CheckState.Checked;
			this.mniShowBarRange.Name = "mniShowBarRange";
			this.mniShowBarRange.Size = new System.Drawing.Size(251, 22);
			this.mniShowBarRange.Text = "Show Bars Range Selector";
			this.mniShowBarRange.Click += new System.EventHandler(this.mniShowBarRange_Click);
			// 
			// TsiProgressBarETA
			// 
			this.TsiProgressBarETA.AutoSize = false;
			this.TsiProgressBarETA.ETALabelText = "400/1600";
			this.TsiProgressBarETA.ETAProgressBarMaximum = 100;
			this.TsiProgressBarETA.ETAProgressBarMinimum = 0;
			this.TsiProgressBarETA.ETAProgressBarValue = 25;
			this.TsiProgressBarETA.Name = "TsiProgressBarETA";
			this.TsiProgressBarETA.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
			this.TsiProgressBarETA.Size = new System.Drawing.Size(200, 18);
			this.TsiProgressBarETA.Text = "TsiProgressBarETA";
			this.TsiProgressBarETA.Visible = false;
			this.TsiProgressBarETA.Click += new System.EventHandler(this.TsiProgressBarETA_Click);
			// 
			// btnStreamingTriggersScript
			// 
			this.btnStreamingTriggersScript.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			this.btnStreamingTriggersScript.CheckOnClick = true;
			this.btnStreamingTriggersScript.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnStreamingTriggersScript.Name = "btnStreamingTriggersScript";
			this.btnStreamingTriggersScript.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
			this.btnStreamingTriggersScript.Size = new System.Drawing.Size(65, 20);
			this.btnStreamingTriggersScript.Text = "Streaming";
			this.btnStreamingTriggersScript.ToolTipText = "ON=>Strategy will be invoked each Bar/Quote; OFF=>Strategy will never be invoked;" +
				" CHART draws streaming bars no matter what";
			this.btnStreamingTriggersScript.Click += new System.EventHandler(this.btnStreamingWillTriggerScript_Click);
			// 
			// btnStrategyEmittingOrders
			// 
			this.btnStrategyEmittingOrders.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			this.btnStrategyEmittingOrders.Checked = true;
			this.btnStrategyEmittingOrders.CheckOnClick = true;
			this.btnStrategyEmittingOrders.CheckState = System.Windows.Forms.CheckState.Checked;
			this.btnStrategyEmittingOrders.Name = "btnStrategyEmittingOrders";
			this.btnStrategyEmittingOrders.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
			this.btnStrategyEmittingOrders.Size = new System.Drawing.Size(70, 20);
			this.btnStrategyEmittingOrders.Text = "EmitOrders";
			this.btnStrategyEmittingOrders.ToolTipText = "ON=>Orders are submitted to BrokerAdapter you chose in your DataSource, OFF=>Orde" +
				"rs are routed to MarketSimStreaming through BacktestBrokerAdapter";
			this.btnStrategyEmittingOrders.Click += new System.EventHandler(this.btnStrategyEmittingOrders_Click);
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(307, 22);
			// 
			// ChartForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(708, 308);
			this.Controls.Add(this.statusStrip);
			this.Controls.Add(this.ChartControl);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.Name = "ChartForm";
			this.Text = "ChartForm";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.chartForm_Closed);
			this.ctxBacktest.ResumeLayout(false);
			this.ctxStrategy.ResumeLayout(false);
			this.statusStrip.ResumeLayout(false);
			this.statusStrip.PerformLayout();
			this.ctxBars.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		private Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox mnitlbShowLastBars;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripMenuItem mniStrategyContextLoad;
		public System.Windows.Forms.ToolStripMenuItem MniShowSourceCodeEditor;
		public System.Windows.Forms.ToolStripMenuItem MniShowSequencer;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		private System.Windows.Forms.ToolStripMenuItem mniStrategyRemove;
		private System.Windows.Forms.ToolStripMenuItem mniShowBarRange;
		private Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox mnitlbMinutes;
		private Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox mnitlbHourly;
		private Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox mnitlbDaily;
		private Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox mnitlbWeekly;
		private Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox mnitlbMonthly;
		private Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox mnitlbYearly;
		private System.Windows.Forms.ContextMenuStrip ctxBars;
		private Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox mnitlbPositionSizeDollarsEachTradeConstant;
		private Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox mnitlbPositionSizeSharesConstantEachTrade;
		private System.Windows.Forms.ContextMenuStrip ctxStrategy;
		public System.Windows.Forms.ToolStripButton btnStrategyEmittingOrders;
		public System.Windows.Forms.ToolStripButton btnStreamingTriggersScript;
		public Sq1.Widgets.ProgressBacktestETA.ToolStripItemProgressBarETA TsiProgressBarETA;
		private System.Windows.Forms.StatusStrip statusStrip;
		public System.Windows.Forms.ToolStripDropDownButton DdbBacktest;
		public System.Windows.Forms.ToolStripDropDownButton DdbStrategy;
		private System.Windows.Forms.ToolStripMenuItem mniBacktestNow;
		private System.Windows.Forms.ToolStripMenuItem mniBacktestOnSelectorsChange;
		private System.Windows.Forms.ToolStripMenuItem mniBacktestOnDataSourceSaved;
		private System.Windows.Forms.ToolStripMenuItem mniBacktestOnRestart;
		private System.Windows.Forms.ContextMenuStrip ctxBacktest;
		public Sq1.Charting.ChartControl ChartControl;
		private  System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		public System.Windows.Forms.ToolStripDropDownButton DdbBars;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
		private System.Windows.Forms.ToolStripMenuItem mniFillOutsideQuoteSpreadParanoidCheckThrow;
		private Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox mnitlbSpreadGeneratorPct;
		private System.Windows.Forms.ToolStripMenuItem mniSubscribedToStreamingAdapterQuotesBars;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
		private System.Windows.Forms.ToolStripMenuItem mniBarsStoredScaleInterval;
		public System.Windows.Forms.ToolStripSeparator TssReportersBelowMe;
		private  System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
		public System.Windows.Forms.ToolStripMenuItem MniShowLivesim;
		private System.Windows.Forms.ContextMenuStrip ctxStrokesForQuoteGenerator;
		private System.Windows.Forms.ToolStripMenuItem mniStrokes;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
	}
}
