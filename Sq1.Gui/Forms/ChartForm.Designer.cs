using System;
using Sq1.Widgets.LabeledTextBox;

namespace Sq1.Gui.Forms {
	partial class ChartForm {
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
			this.mniBacktestOnRestart = new System.Windows.Forms.ToolStripMenuItem();
			this.mniBacktestOnSelectorsChange = new System.Windows.Forms.ToolStripMenuItem();
			this.mniBacktestOnDataSourceSaved = new System.Windows.Forms.ToolStripMenuItem();
			this.mniBacktestNow = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.mnitlbPositionSizeSharesConstantEachTrade = new Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox();
			this.mnitlbPositionSizeDollarsEachTradeConstant = new Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox();
			this.DdbBacktest = new System.Windows.Forms.ToolStripDropDownButton();
			this.mniStrategyRemove = new System.Windows.Forms.ToolStripMenuItem();
			this.MniShowSourceCodeEditor = new System.Windows.Forms.ToolStripMenuItem();
			this.MniShowOptimizer = new System.Windows.Forms.ToolStripMenuItem();
			this.ctxStrategy = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.mniStrategyContextLoad = new System.Windows.Forms.ToolStripMenuItem();
			this.DdbStrategy = new System.Windows.Forms.ToolStripDropDownButton();
			this.CtxReporters = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.DdbReporters = new System.Windows.Forms.ToolStripDropDownButton();
			this.statusStrip = new System.Windows.Forms.StatusStrip();
			this.DdbBars = new System.Windows.Forms.ToolStripDropDownButton();
			this.ctxBars = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.mnitlbYearly = new Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox();
			this.mnitlbMonthly = new Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox();
			this.mnitlbWeekly = new Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox();
			this.mnitlbDaily = new Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox();
			this.mnitlbHourly = new Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox();
			this.mnitlbMinutes = new Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.mnitlbShowLastBars = new Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox();
			this.mniShowBarRange = new System.Windows.Forms.ToolStripMenuItem();
			this.TsiProgressBarETA = new Sq1.Widgets.ProgressBacktestETA.ToolStripItemProgressBarETA();
			this.btnStreaming = new System.Windows.Forms.ToolStripButton();
			this.btnAutoSubmit = new System.Windows.Forms.ToolStripButton();
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
			this.ChartControl.RangeBarCollapsed = false;
			this.ChartControl.Size = new System.Drawing.Size(708, 287);
			this.ChartControl.TabIndex = 0;
			// 
			// ctxBacktest
			// 
			this.ctxBacktest.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.mniBacktestOnDataSourceSaved,
									this.mniBacktestOnRestart,
									this.mniBacktestOnSelectorsChange,
									this.mniBacktestNow,
									this.toolStripSeparator1,
									this.mnitlbPositionSizeSharesConstantEachTrade,
									this.mnitlbPositionSizeDollarsEachTradeConstant});
			this.ctxBacktest.Name = "ctxBacktest";
			this.ctxBacktest.Size = new System.Drawing.Size(232, 124);
			this.ctxBacktest.Opening += new System.ComponentModel.CancelEventHandler(ctxBacktest_Opening);
			// 
			// mniBacktestOnRestart
			// 
			this.mniBacktestOnRestart.Checked = true;
			this.mniBacktestOnRestart.CheckOnClick = true;
			this.mniBacktestOnRestart.CheckState = System.Windows.Forms.CheckState.Checked;
			this.mniBacktestOnRestart.Name = "mniBacktestOnRestart";
			this.mniBacktestOnRestart.Size = new System.Drawing.Size(231, 22);
			this.mniBacktestOnRestart.Text = "Backtest On Application Restart";
			this.mniBacktestOnRestart.Click += new System.EventHandler(this.mniBacktestOnRestart_Click);
			// 
			// mniBacktestOnSelectorsChange
			// 
			this.mniBacktestOnSelectorsChange.Checked = true;
			this.mniBacktestOnSelectorsChange.CheckOnClick = true;
			this.mniBacktestOnSelectorsChange.CheckState = System.Windows.Forms.CheckState.Checked;
			this.mniBacktestOnSelectorsChange.Name = "mniBacktestOnSelectorsChange";
			this.mniBacktestOnSelectorsChange.Size = new System.Drawing.Size(231, 22);
			this.mniBacktestOnSelectorsChange.Text = "Backtest On Bars/ScriptParameters Changed";
			this.mniBacktestOnSelectorsChange.Click += new System.EventHandler(this.mniBacktestOnEveryChange_Click);
			
			// 
			// mniBacktestOnDataSourceEdited
			// 
			this.mniBacktestOnDataSourceSaved.Checked = true;
			this.mniBacktestOnDataSourceSaved.CheckOnClick = true;
			this.mniBacktestOnDataSourceSaved.CheckState = System.Windows.Forms.CheckState.Checked;
			this.mniBacktestOnDataSourceSaved.Name = "mniBacktestOnDataSourceSaved";
			this.mniBacktestOnDataSourceSaved.Size = new System.Drawing.Size(231, 22);
			this.mniBacktestOnDataSourceSaved.Text = "Backtest On DataSource Saved";
			this.mniBacktestOnDataSourceSaved.Click += new System.EventHandler(this.mniBacktestOnEveryChange_Click);
				
			// 
			// mniBacktestNow
			// 
			this.mniBacktestNow.Name = "mniBacktestNow";
			this.mniBacktestNow.Size = new System.Drawing.Size(231, 22);
			this.mniBacktestNow.Text = "Backtest Now";
			this.mniBacktestNow.Click += new System.EventHandler(this.mniBacktestNow_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(228, 6);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator1";
			this.toolStripSeparator3.Size = new System.Drawing.Size(228, 6);
			// 
			// mniSharesConstant
			// 
			this.mnitlbPositionSizeSharesConstantEachTrade.BackColor = System.Drawing.Color.Transparent;
			this.mnitlbPositionSizeSharesConstantEachTrade.InputFieldOffsetX = 80;
			this.mnitlbPositionSizeSharesConstantEachTrade.InputFieldValue = "";
			this.mnitlbPositionSizeSharesConstantEachTrade.InputFieldWidth = 85;
			this.mnitlbPositionSizeSharesConstantEachTrade.Name = "mniSharesConstant";
			this.mnitlbPositionSizeSharesConstantEachTrade.Size = new System.Drawing.Size(165, 21);
			this.mnitlbPositionSizeSharesConstantEachTrade.Text = "Shares";
			this.mnitlbPositionSizeSharesConstantEachTrade.TextRed = false;
			this.mnitlbPositionSizeSharesConstantEachTrade.UserTyped += new System.EventHandler<Sq1.Widgets.LabeledTextBox.LabeledTextBoxUserTypedArgs>(this.mnitlbPositionSizeSharesConstantEachTrade_UserTyped);
			// 
			// mniPositionSizeSimpleDollar
			// 
			this.mnitlbPositionSizeDollarsEachTradeConstant.BackColor = System.Drawing.Color.Transparent;
			this.mnitlbPositionSizeDollarsEachTradeConstant.InputFieldOffsetX = 80;
			this.mnitlbPositionSizeDollarsEachTradeConstant.InputFieldValue = "";
			this.mnitlbPositionSizeDollarsEachTradeConstant.InputFieldWidth = 85;
			this.mnitlbPositionSizeDollarsEachTradeConstant.Name = "mniPositionSizeSimpleDollar";
			this.mnitlbPositionSizeDollarsEachTradeConstant.Size = new System.Drawing.Size(165, 21);
			this.mnitlbPositionSizeDollarsEachTradeConstant.Text = "$$ Each Trade";
			this.mnitlbPositionSizeDollarsEachTradeConstant.TextRed = false;
			this.mnitlbPositionSizeDollarsEachTradeConstant.UserTyped += new System.EventHandler<Sq1.Widgets.LabeledTextBox.LabeledTextBoxUserTypedArgs>(this.mnitlbPositionSizeDollarsConstantEachTrade_UserTyped);
			// 
			// DdbBacktest
			// 
			this.DdbBacktest.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.DdbBacktest.DropDown = this.ctxBacktest;
			this.DdbBacktest.Name = "DdbBacktest";
			this.DdbBacktest.Size = new System.Drawing.Size(64, 20);
			this.DdbBacktest.Text = "Backtest";
			// 
			// mniStrategyRemove
			// 
			this.mniStrategyRemove.Name = "mniStrategyRemove";
			this.mniStrategyRemove.Size = new System.Drawing.Size(226, 22);
			this.mniStrategyRemove.Text = "NYI Remove Strategy From Chart";
			this.mniStrategyRemove.Enabled = false;
			// 
			// MniShowSourceCodeEditor
			// 
			this.MniShowSourceCodeEditor.CheckOnClick = true;
			this.MniShowSourceCodeEditor.Name = "MniShowSourceCodeEditor";
			this.MniShowSourceCodeEditor.Size = new System.Drawing.Size(226, 22);
			this.MniShowSourceCodeEditor.Text = "Show Source Code Editor";
			this.MniShowSourceCodeEditor.Click += new System.EventHandler(this.MniShowSourceCodeEditor_Click);
			// 
			// MniOptimizer
			// 
			this.MniShowOptimizer.CheckOnClick = true;
			this.MniShowOptimizer.Name = "MniOptimizer";
			this.MniShowOptimizer.Size = new System.Drawing.Size(226, 22);
			this.MniShowOptimizer.Text = "Show Optimizer";
			this.MniShowOptimizer.Click += new System.EventHandler(this.MniShowOptimizer_Click);
			// 
			// ctxStrategy
			// 
			this.ctxStrategy.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.mniStrategyContextLoad,
									this.mniStrategyRemove,
									this.toolStripSeparator3,
									this.MniShowOptimizer,
									this.MniShowSourceCodeEditor});
			this.ctxStrategy.Name = "ctxPositionSize";
			this.ctxStrategy.Size = new System.Drawing.Size(227, 92);
			this.ctxStrategy.Opening += new System.ComponentModel.CancelEventHandler(ctxStrategy_Opening);
			// 
			// mniStrategyContextLoad
			// 
			this.mniStrategyContextLoad.Enabled = false;
			this.mniStrategyContextLoad.Name = "mniStrategyContextLoad";
			this.mniStrategyContextLoad.Size = new System.Drawing.Size(226, 22);
			this.mniStrategyContextLoad.Text = "NYI Load Script Context...";
			// 
			// DdbStrategy
			// 
			this.DdbStrategy.DropDown = this.ctxStrategy;
			this.DdbStrategy.Name = "DdbStrategy";
			this.DdbStrategy.Size = new System.Drawing.Size(63, 20);
			this.DdbStrategy.Text = "Strategy";
			// 
			// CtxReporters
			// 
			this.CtxReporters.Name = "ctxChart";
			this.CtxReporters.Size = new System.Drawing.Size(61, 4);
			this.CtxReporters.Opening += new System.ComponentModel.CancelEventHandler(this.ctxChart_Opening);
			// 
			// DdbReporters
			// 
			this.DdbReporters.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.DdbReporters.DropDown = this.CtxReporters;
			this.DdbReporters.Name = "DdbReporters";
			this.DdbReporters.Size = new System.Drawing.Size(70, 20);
			this.DdbReporters.Text = "Reporters";
			// 
			// statusStrip
			// 
			this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.DdbBars,
									this.DdbStrategy,
									this.DdbBacktest,
									this.DdbReporters,
									this.TsiProgressBarETA,
									this.btnStreaming,
									this.btnAutoSubmit});
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
			this.DdbBars.Size = new System.Drawing.Size(42, 20);
			this.DdbBars.Text = "Bars";
			// 
			// ctxBars
			// 
			this.ctxBars.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.mnitlbYearly,
									this.mnitlbMonthly,
									this.mnitlbWeekly,
									this.mnitlbDaily,
									this.mnitlbHourly,
									this.mnitlbMinutes,
									this.toolStripSeparator2,
									this.mnitlbShowLastBars,
									this.mniShowBarRange});
			this.ctxBars.Name = "ctxScaleInterval";
			this.ctxBars.OwnerItem = this.DdbBars;
			this.ctxBars.Size = new System.Drawing.Size(226, 200);
			// 
			// mnitlbYearly
			// 
			this.mnitlbYearly.BackColor = System.Drawing.Color.Transparent;
			this.mnitlbYearly.InputFieldOffsetX = 80;
			this.mnitlbYearly.InputFieldValue = "";
			this.mnitlbYearly.InputFieldWidth = 85;
			this.mnitlbYearly.Name = "mnitlbYearly";
			this.mnitlbYearly.Size = new System.Drawing.Size(165, 21);
			this.mnitlbYearly.Text = "Yearly";
			this.mnitlbYearly.TextRed = false;
			this.mnitlbYearly.UserTyped += new System.EventHandler<Sq1.Widgets.LabeledTextBox.LabeledTextBoxUserTypedArgs>(this.mnitlbAll_UserTyped);
			// 
			// mnitlbMonthly
			// 
			this.mnitlbMonthly.BackColor = System.Drawing.Color.Transparent;
			this.mnitlbMonthly.InputFieldOffsetX = 80;
			this.mnitlbMonthly.InputFieldValue = "";
			this.mnitlbMonthly.InputFieldWidth = 85;
			this.mnitlbMonthly.Name = "mnitlbMonthly";
			this.mnitlbMonthly.Size = new System.Drawing.Size(165, 21);
			this.mnitlbMonthly.Text = "Monthly";
			this.mnitlbMonthly.TextRed = false;
			this.mnitlbMonthly.UserTyped += new System.EventHandler<Sq1.Widgets.LabeledTextBox.LabeledTextBoxUserTypedArgs>(this.mnitlbAll_UserTyped);
			// 
			// mnitlbWeekly
			// 
			this.mnitlbWeekly.BackColor = System.Drawing.Color.Transparent;
			this.mnitlbWeekly.InputFieldOffsetX = 80;
			this.mnitlbWeekly.InputFieldValue = "";
			this.mnitlbWeekly.InputFieldWidth = 85;
			this.mnitlbWeekly.Name = "mnitlbWeekly";
			this.mnitlbWeekly.Size = new System.Drawing.Size(165, 21);
			this.mnitlbWeekly.Text = "Weekly";
			this.mnitlbWeekly.TextRed = false;
			this.mnitlbWeekly.UserTyped += new System.EventHandler<Sq1.Widgets.LabeledTextBox.LabeledTextBoxUserTypedArgs>(this.mnitlbAll_UserTyped);
			// 
			// mnitlbDaily
			// 
			this.mnitlbDaily.BackColor = System.Drawing.Color.Transparent;
			this.mnitlbDaily.InputFieldOffsetX = 80;
			this.mnitlbDaily.InputFieldValue = "";
			this.mnitlbDaily.InputFieldWidth = 85;
			this.mnitlbDaily.Name = "mnitlbDaily";
			this.mnitlbDaily.Size = new System.Drawing.Size(165, 21);
			this.mnitlbDaily.Text = "Daily";
			this.mnitlbDaily.TextRed = false;
			this.mnitlbDaily.UserTyped += new System.EventHandler<Sq1.Widgets.LabeledTextBox.LabeledTextBoxUserTypedArgs>(this.mnitlbAll_UserTyped);
			// 
			// mnitlbHourly
			// 
			this.mnitlbHourly.BackColor = System.Drawing.Color.Transparent;
			this.mnitlbHourly.InputFieldOffsetX = 80;
			this.mnitlbHourly.InputFieldValue = "";
			this.mnitlbHourly.InputFieldWidth = 85;
			this.mnitlbHourly.Name = "mnitlbHourly";
			this.mnitlbHourly.Size = new System.Drawing.Size(165, 21);
			this.mnitlbHourly.Text = "Hourly";
			this.mnitlbHourly.TextRed = false;
			this.mnitlbHourly.UserTyped += new System.EventHandler<Sq1.Widgets.LabeledTextBox.LabeledTextBoxUserTypedArgs>(this.mnitlbAll_UserTyped);
			// 
			// mnitlbMinutes
			// 
			this.mnitlbMinutes.BackColor = System.Drawing.Color.Transparent;
			this.mnitlbMinutes.InputFieldOffsetX = 80;
			this.mnitlbMinutes.InputFieldValue = "";
			this.mnitlbMinutes.InputFieldWidth = 85;
			this.mnitlbMinutes.Name = "mnitlbMinutes";
			this.mnitlbMinutes.Size = new System.Drawing.Size(165, 21);
			this.mnitlbMinutes.Text = "Minutes";
			this.mnitlbMinutes.TextRed = false;
			this.mnitlbMinutes.UserTyped += new System.EventHandler<Sq1.Widgets.LabeledTextBox.LabeledTextBoxUserTypedArgs>(this.mnitlbAll_UserTyped);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(222, 6);
			// 
			// mnitlbShowLastBars
			// 
			this.mnitlbShowLastBars.BackColor = System.Drawing.Color.Transparent;
			this.mnitlbShowLastBars.InputFieldOffsetX = 80;
			this.mnitlbShowLastBars.InputFieldValue = "";
			this.mnitlbShowLastBars.InputFieldWidth = 85;
			this.mnitlbShowLastBars.Name = "mnitlbShowLastBars";
			this.mnitlbShowLastBars.Size = new System.Drawing.Size(165, 21);
			this.mnitlbShowLastBars.Text = "Last Bars";
			this.mnitlbShowLastBars.TextRed = false;
			this.mnitlbShowLastBars.UserTyped += new System.EventHandler<Sq1.Widgets.LabeledTextBox.LabeledTextBoxUserTypedArgs>(this.mnitlbShowLastBars_UserTyped);
			// 
			// mniShowBarRange
			// 
			this.mniShowBarRange.Checked = true;
			this.mniShowBarRange.CheckOnClick = true;
			this.mniShowBarRange.CheckState = System.Windows.Forms.CheckState.Checked;
			this.mniShowBarRange.Name = "mniShowBarRange";
			this.mniShowBarRange.Size = new System.Drawing.Size(225, 22);
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
			this.TsiProgressBarETA.Size = new System.Drawing.Size(200, 18);
			this.TsiProgressBarETA.Text = "TsiProgressBarETA";
			this.TsiProgressBarETA.Visible = false;
			this.TsiProgressBarETA.Click += new System.EventHandler(this.TsiProgressBarETAClick);
			// 
			// btnStreaming
			// 
			this.btnStreaming.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			this.btnStreaming.CheckOnClick = true;
			this.btnStreaming.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnStreaming.Name = "btnStreaming";
			this.btnStreaming.Size = new System.Drawing.Size(65, 20);
			this.btnStreaming.Text = "Streaming";
			this.btnStreaming.ToolTipText = "Enable/Disable Streaming";
			this.btnStreaming.Click += new System.EventHandler(this.btnStreaming_Click);
			// 
			// btnAutoSubmit
			// 
			this.btnAutoSubmit.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			this.btnAutoSubmit.Checked = false;
			this.btnAutoSubmit.CheckOnClick = true;
			this.btnAutoSubmit.CheckState = System.Windows.Forms.CheckState.Checked;
			this.btnAutoSubmit.Enabled = false;
			this.btnAutoSubmit.Name = "btnAutoSubmit";
			this.btnAutoSubmit.Size = new System.Drawing.Size(80, 20);
			this.btnAutoSubmit.Text = "Auto-Submit";
			this.btnAutoSubmit.ToolTipText = "ON=>Orders are submitted to BrokerProvider you chose in your DataSource, OFF=>Ord" +
			"ers are routed to DEPRECATEDSimulateBacktestPendingAlertsFill()";
			this.btnAutoSubmit.Click += new System.EventHandler(this.btnAutoSubmit_Click);
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
		private System.Windows.Forms.ToolStripMenuItem mniStrategyOptimize;
		private System.Windows.Forms.ToolStripMenuItem mniStrategyContextLoad;
		public System.Windows.Forms.ToolStripMenuItem MniShowSourceCodeEditor;
		public System.Windows.Forms.ToolStripMenuItem MniShowOptimizer;
		private  System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
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
		public System.Windows.Forms.ToolStripButton btnAutoSubmit;
		public System.Windows.Forms.ToolStripButton btnStreaming;
		public Sq1.Widgets.ProgressBacktestETA.ToolStripItemProgressBarETA TsiProgressBarETA;
		private System.Windows.Forms.StatusStrip statusStrip;
		public System.Windows.Forms.ToolStripDropDownButton DdbReporters;
		public System.Windows.Forms.ContextMenuStrip CtxReporters;
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
	}
}
