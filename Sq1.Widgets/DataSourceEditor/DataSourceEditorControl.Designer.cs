namespace Sq1.Widgets.DataSourceEditor {
	partial class DataSourceEditorControl {
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.ListView lvBrokerAdapters;
		private System.Windows.Forms.ListView lvStreamingAdapters;
		private System.Windows.Forms.ImageList imglStreamingAdapters;
		private System.Windows.Forms.ImageList imglBrokerAdapters;
		private System.Windows.Forms.Label lblStreaming;
		private System.Windows.Forms.Label lblExecution;
		private System.Windows.Forms.Panel pnlStreamingEditor;
		private System.Windows.Forms.Panel pnlBrokerEditor;
		private System.Windows.Forms.SplitContainer splitContainer3;
		private System.Windows.Forms.SplitContainer splitContainer2;
		private Sq1.Widgets.DataSourceEditor.MarketInfoEditor marketInfoEditor;
		private System.Windows.Forms.SplitContainer splitContainer4;
		private System.Windows.Forms.SplitContainer splitContainer5;
		private System.Windows.Forms.GroupBox grpStreaming;
		private System.Windows.Forms.GroupBox grpBroker;
		
		protected override void Dispose(bool disposing) {
			if (disposing && this.components != null) {
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.splitContainer3 = new System.Windows.Forms.SplitContainer();
			this.splitContainer2 = new System.Windows.Forms.SplitContainer();
			this.splitContainer4 = new System.Windows.Forms.SplitContainer();
			this.lblStreaming = new System.Windows.Forms.Label();
			this.lvStreamingAdapters = new System.Windows.Forms.ListView();
			this.imglStreamingAdapters = new System.Windows.Forms.ImageList(this.components);
			this.grpStreaming = new System.Windows.Forms.GroupBox();
			this.pnlStreamingEditor = new System.Windows.Forms.Panel();
			this.splitContainer5 = new System.Windows.Forms.SplitContainer();
			this.lblExecution = new System.Windows.Forms.Label();
			this.lvBrokerAdapters = new System.Windows.Forms.ListView();
			this.imglBrokerAdapters = new System.Windows.Forms.ImageList(this.components);
			this.grpBroker = new System.Windows.Forms.GroupBox();
			this.pnlBrokerEditor = new System.Windows.Forms.Panel();
			this.marketInfoEditor = new Sq1.Widgets.DataSourceEditor.MarketInfoEditor();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.tsiBtnSave = new System.Windows.Forms.ToolStripButton();
			this.tsiLtbDataSourceName = new Sq1.Widgets.LabeledTextBox.ToolStripItemLabeledTextBox();
			this.tsiNudInterval = new Sq1.Widgets.ToolStripImproved.ToolStripItemNumericUpDownWithMouseEvents();
			this.tsiCbxScale = new Sq1.Widgets.ToolStripImproved.ToolStripItemComboBox();
			this.tsiLtbSymbols = new Sq1.Widgets.LabeledTextBox.ToolStripItemLabeledTextBox();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
			this.splitContainer3.Panel1.SuspendLayout();
			this.splitContainer3.Panel2.SuspendLayout();
			this.splitContainer3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
			this.splitContainer2.Panel1.SuspendLayout();
			this.splitContainer2.Panel2.SuspendLayout();
			this.splitContainer2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).BeginInit();
			this.splitContainer4.Panel1.SuspendLayout();
			this.splitContainer4.Panel2.SuspendLayout();
			this.splitContainer4.SuspendLayout();
			this.grpStreaming.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer5)).BeginInit();
			this.splitContainer5.Panel1.SuspendLayout();
			this.splitContainer5.Panel2.SuspendLayout();
			this.splitContainer5.SuspendLayout();
			this.grpBroker.SuspendLayout();
			this.statusStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer3
			// 
			this.splitContainer3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.splitContainer3.BackColor = System.Drawing.SystemColors.ControlDark;
			this.splitContainer3.Location = new System.Drawing.Point(0, 0);
			this.splitContainer3.Name = "splitContainer3";
			this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer3.Panel1
			// 
			this.splitContainer3.Panel1.BackColor = System.Drawing.SystemColors.Control;
			this.splitContainer3.Panel1.Controls.Add(this.splitContainer2);
			// 
			// splitContainer3.Panel2
			// 
			this.splitContainer3.Panel2.BackColor = System.Drawing.SystemColors.Control;
			this.splitContainer3.Panel2.Controls.Add(this.marketInfoEditor);
			this.splitContainer3.Size = new System.Drawing.Size(652, 398);
			this.splitContainer3.SplitterDistance = 250;
			this.splitContainer3.TabIndex = 12;
			// 
			// splitContainer2
			// 
			this.splitContainer2.BackColor = System.Drawing.SystemColors.ControlDark;
			this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer2.Location = new System.Drawing.Point(0, 0);
			this.splitContainer2.Name = "splitContainer2";
			// 
			// splitContainer2.Panel1
			// 
			this.splitContainer2.Panel1.BackColor = System.Drawing.SystemColors.Control;
			this.splitContainer2.Panel1.Controls.Add(this.splitContainer4);
			// 
			// splitContainer2.Panel2
			// 
			this.splitContainer2.Panel2.BackColor = System.Drawing.SystemColors.Control;
			this.splitContainer2.Panel2.Controls.Add(this.splitContainer5);
			this.splitContainer2.Size = new System.Drawing.Size(652, 250);
			this.splitContainer2.SplitterDistance = 324;
			this.splitContainer2.TabIndex = 11;
			// 
			// splitContainer4
			// 
			this.splitContainer4.BackColor = System.Drawing.SystemColors.ControlDark;
			this.splitContainer4.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer4.Location = new System.Drawing.Point(0, 0);
			this.splitContainer4.Name = "splitContainer4";
			this.splitContainer4.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer4.Panel1
			// 
			this.splitContainer4.Panel1.BackColor = System.Drawing.SystemColors.Control;
			this.splitContainer4.Panel1.Controls.Add(this.lblStreaming);
			this.splitContainer4.Panel1.Controls.Add(this.lvStreamingAdapters);
			// 
			// splitContainer4.Panel2
			// 
			this.splitContainer4.Panel2.BackColor = System.Drawing.SystemColors.Control;
			this.splitContainer4.Panel2.Controls.Add(this.grpStreaming);
			this.splitContainer4.Size = new System.Drawing.Size(324, 250);
			this.splitContainer4.SplitterDistance = 81;
			this.splitContainer4.TabIndex = 7;
			// 
			// lblStreaming
			// 
			this.lblStreaming.AutoSize = true;
			this.lblStreaming.Location = new System.Drawing.Point(3, 5);
			this.lblStreaming.Name = "lblStreaming";
			this.lblStreaming.Size = new System.Drawing.Size(185, 13);
			this.lblStreaming.TabIndex = 5;
			this.lblStreaming.Text = "DataFeed (StreamingAdapter-derived)";
			// 
			// lvStreamingAdapters
			// 
			this.lvStreamingAdapters.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.lvStreamingAdapters.HideSelection = false;
			this.lvStreamingAdapters.Location = new System.Drawing.Point(3, 21);
			this.lvStreamingAdapters.MultiSelect = false;
			this.lvStreamingAdapters.Name = "lvStreamingAdapters";
			this.lvStreamingAdapters.Size = new System.Drawing.Size(318, 57);
			this.lvStreamingAdapters.SmallImageList = this.imglStreamingAdapters;
			this.lvStreamingAdapters.TabIndex = 5;
			this.lvStreamingAdapters.UseCompatibleStateImageBehavior = false;
			this.lvStreamingAdapters.View = System.Windows.Forms.View.List;
			this.lvStreamingAdapters.SelectedIndexChanged += new System.EventHandler(this.lvStreamingAdapters_SelectedIndexChanged);
			// 
			// imglStreamingAdapters
			// 
			this.imglStreamingAdapters.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
			this.imglStreamingAdapters.ImageSize = new System.Drawing.Size(16, 16);
			this.imglStreamingAdapters.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// grpStreaming
			// 
			this.grpStreaming.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.grpStreaming.Controls.Add(this.pnlStreamingEditor);
			this.grpStreaming.Location = new System.Drawing.Point(3, 3);
			this.grpStreaming.Name = "grpStreaming";
			this.grpStreaming.Size = new System.Drawing.Size(318, 162);
			this.grpStreaming.TabIndex = 8;
			this.grpStreaming.TabStop = false;
			this.grpStreaming.Text = "Streaming Settings";
			// 
			// pnlStreamingEditor
			// 
			this.pnlStreamingEditor.AutoScroll = true;
			this.pnlStreamingEditor.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.pnlStreamingEditor.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlStreamingEditor.Location = new System.Drawing.Point(3, 16);
			this.pnlStreamingEditor.Name = "pnlStreamingEditor";
			this.pnlStreamingEditor.Size = new System.Drawing.Size(312, 143);
			this.pnlStreamingEditor.TabIndex = 7;
			// 
			// splitContainer5
			// 
			this.splitContainer5.BackColor = System.Drawing.SystemColors.ControlDark;
			this.splitContainer5.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer5.Location = new System.Drawing.Point(0, 0);
			this.splitContainer5.Name = "splitContainer5";
			this.splitContainer5.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer5.Panel1
			// 
			this.splitContainer5.Panel1.BackColor = System.Drawing.SystemColors.Control;
			this.splitContainer5.Panel1.Controls.Add(this.lblExecution);
			this.splitContainer5.Panel1.Controls.Add(this.lvBrokerAdapters);
			// 
			// splitContainer5.Panel2
			// 
			this.splitContainer5.Panel2.BackColor = System.Drawing.SystemColors.Control;
			this.splitContainer5.Panel2.Controls.Add(this.grpBroker);
			this.splitContainer5.Size = new System.Drawing.Size(324, 250);
			this.splitContainer5.SplitterDistance = 81;
			this.splitContainer5.TabIndex = 11;
			// 
			// lblExecution
			// 
			this.lblExecution.AutoSize = true;
			this.lblExecution.Location = new System.Drawing.Point(0, 5);
			this.lblExecution.Name = "lblExecution";
			this.lblExecution.Size = new System.Drawing.Size(198, 13);
			this.lblExecution.TabIndex = 8;
			this.lblExecution.Text = "Order Execution (BrokerAdapter-derived)";
			// 
			// lvBrokerAdapters
			// 
			this.lvBrokerAdapters.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.lvBrokerAdapters.HideSelection = false;
			this.lvBrokerAdapters.Location = new System.Drawing.Point(3, 21);
			this.lvBrokerAdapters.MultiSelect = false;
			this.lvBrokerAdapters.Name = "lvBrokerAdapters";
			this.lvBrokerAdapters.Size = new System.Drawing.Size(318, 57);
			this.lvBrokerAdapters.SmallImageList = this.imglBrokerAdapters;
			this.lvBrokerAdapters.TabIndex = 10;
			this.lvBrokerAdapters.UseCompatibleStateImageBehavior = false;
			this.lvBrokerAdapters.View = System.Windows.Forms.View.List;
			this.lvBrokerAdapters.SelectedIndexChanged += new System.EventHandler(this.lvBrokerAdapters_SelectedIndexChanged);
			// 
			// imglBrokerAdapters
			// 
			this.imglBrokerAdapters.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
			this.imglBrokerAdapters.ImageSize = new System.Drawing.Size(16, 16);
			this.imglBrokerAdapters.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// grpBroker
			// 
			this.grpBroker.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.grpBroker.Controls.Add(this.pnlBrokerEditor);
			this.grpBroker.Location = new System.Drawing.Point(3, 3);
			this.grpBroker.Name = "grpBroker";
			this.grpBroker.Size = new System.Drawing.Size(318, 162);
			this.grpBroker.TabIndex = 10;
			this.grpBroker.TabStop = false;
			this.grpBroker.Text = "Broker Settings";
			// 
			// pnlBrokerEditor
			// 
			this.pnlBrokerEditor.AutoScroll = true;
			this.pnlBrokerEditor.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.pnlBrokerEditor.BackColor = System.Drawing.SystemColors.Control;
			this.pnlBrokerEditor.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlBrokerEditor.Location = new System.Drawing.Point(3, 16);
			this.pnlBrokerEditor.Name = "pnlBrokerEditor";
			this.pnlBrokerEditor.Size = new System.Drawing.Size(312, 143);
			this.pnlBrokerEditor.TabIndex = 9;
			// 
			// marketInfoEditor
			// 
			this.marketInfoEditor.AutoSize = true;
			this.marketInfoEditor.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.marketInfoEditor.Dock = System.Windows.Forms.DockStyle.Fill;
			this.marketInfoEditor.Location = new System.Drawing.Point(0, 0);
			this.marketInfoEditor.Name = "marketInfoEditor";
			this.marketInfoEditor.Size = new System.Drawing.Size(652, 144);
			this.marketInfoEditor.TabIndex = 0;
			// 
			// statusStrip1
			// 
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsiBtnSave,
            this.tsiLtbDataSourceName,
            this.tsiNudInterval,
            this.tsiCbxScale,
            this.tsiLtbSymbols});
			this.statusStrip1.Location = new System.Drawing.Point(0, 401);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(655, 25);
			this.statusStrip1.SizingGrip = false;
			this.statusStrip1.TabIndex = 53;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// tsiBtnSave
			// 
			this.tsiBtnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsiBtnSave.Name = "tsiBtnSave";
			this.tsiBtnSave.Size = new System.Drawing.Size(35, 23);
			this.tsiBtnSave.Text = "Save";
			this.tsiBtnSave.Click += new System.EventHandler(this.btnSave_Click);
			// 
			// tsiLtbDataSourceName
			// 
			this.tsiLtbDataSourceName.BackColor = System.Drawing.Color.Transparent;
			this.tsiLtbDataSourceName.InputFieldAlignedRight = false;
			this.tsiLtbDataSourceName.InputFieldBackColor = System.Drawing.SystemColors.Window;
			this.tsiLtbDataSourceName.InputFieldEditable = true;
			this.tsiLtbDataSourceName.InputFieldMultiline = false;
			this.tsiLtbDataSourceName.InputFieldOffsetX = 80;
			this.tsiLtbDataSourceName.InputFieldValue = "QMOCK";
			this.tsiLtbDataSourceName.InputFieldWidth = 60;
			this.tsiLtbDataSourceName.Name = "tsiLtbDataSourceName";
			this.tsiLtbDataSourceName.OffsetTop = 1;
			this.tsiLtbDataSourceName.Size = new System.Drawing.Size(147, 23);
			this.tsiLtbDataSourceName.Text = "tsiLtbDataSourceName";
			this.tsiLtbDataSourceName.TextLeft = "DS Name:";
			this.tsiLtbDataSourceName.TextLeftOffsetX = 21;
			this.tsiLtbDataSourceName.TextLeftWidth = 61;
			this.tsiLtbDataSourceName.TextRed = false;
			this.tsiLtbDataSourceName.TextRight = "";
			this.tsiLtbDataSourceName.TextRightOffsetX = 140;
			this.tsiLtbDataSourceName.TextRightWidth = 4;
			// 
			// tsiNudInterval
			// 
			this.tsiNudInterval.DomainUpDownMinimumSize = new System.Drawing.Size(40, 0);
			this.tsiNudInterval.Name = "tsiNudInterval";
			this.tsiNudInterval.Size = new System.Drawing.Size(41, 23);
			this.tsiNudInterval.Text = "15";
			this.tsiNudInterval.DomainUpDownArrowUpStepAdd += new System.EventHandler(this.tsiNudInterval_ValueChanged);
			this.tsiNudInterval.DomainUpDownArrowDownStepSubstract += new System.EventHandler(this.tsiNudInterval_ValueChanged);
			// 
			// tsiCbxScale
			// 
			this.tsiCbxScale.ComboBoxDropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.tsiCbxScale.ComboBoxFormattingEnabled = false;
			this.tsiCbxScale.ComboBoxItems.AddRange(new object[] {
            "Unknown",
            "Tick",
            "Second",
            "Minute",
            "Daily",
            "Weekly",
            "Monthly",
            "Quarterly",
            "Yearly"});
			this.tsiCbxScale.ComboBoxLocation = new System.Drawing.Point(224, 3);
			this.tsiCbxScale.ComboBoxSelectedIndex = -1;
			this.tsiCbxScale.ComboBoxSize = new System.Drawing.Size(70, 21);
			this.tsiCbxScale.ComboBoxSorted = false;
			this.tsiCbxScale.Name = "tsiCbxScale";
			this.tsiCbxScale.Size = new System.Drawing.Size(70, 23);
			this.tsiCbxScale.ComboBoxSelectedIndexChanged += new System.EventHandler<System.EventArgs>(this.tsiCmbScale_SelectedIndexChanged);
			// 
			// tsiLtbSymbols
			// 
			this.tsiLtbSymbols.BackColor = System.Drawing.Color.Transparent;
			this.tsiLtbSymbols.InputFieldAlignedRight = false;
			this.tsiLtbSymbols.InputFieldBackColor = System.Drawing.SystemColors.Window;
			this.tsiLtbSymbols.InputFieldEditable = true;
			this.tsiLtbSymbols.InputFieldMultiline = false;
			this.tsiLtbSymbols.InputFieldOffsetX = 60;
			this.tsiLtbSymbols.InputFieldValue = "LKOH,RIM3,autogen";
			this.tsiLtbSymbols.InputFieldWidth = 190;
			this.tsiLtbSymbols.Margin = new System.Windows.Forms.Padding(5, 2, 0, 0);
			this.tsiLtbSymbols.Name = "tsiLtbSymbols";
			this.tsiLtbSymbols.OffsetTop = 1;
			this.tsiLtbSymbols.Size = new System.Drawing.Size(260, 23);
			this.tsiLtbSymbols.Text = "tsmniSymbols";
			this.tsiLtbSymbols.TextLeft = "Symbols:";
			this.tsiLtbSymbols.TextLeftOffsetX = 0;
			this.tsiLtbSymbols.TextLeftWidth = 57;
			this.tsiLtbSymbols.TextRed = false;
			this.tsiLtbSymbols.TextRight = "";
			this.tsiLtbSymbols.TextRightOffsetX = 253;
			this.tsiLtbSymbols.TextRightWidth = 4;
			this.tsiLtbSymbols.UserTyped += new System.EventHandler<Sq1.Widgets.LabeledTextBox.LabeledTextBoxUserTypedArgs>(this.tsiLtbSymbols_UserTyped);
			// 
			// DataSourceEditorControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.splitContainer3);
			this.Controls.Add(this.statusStrip1);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.Name = "DataSourceEditorControl";
			this.Size = new System.Drawing.Size(655, 426);
			this.splitContainer3.Panel1.ResumeLayout(false);
			this.splitContainer3.Panel2.ResumeLayout(false);
			this.splitContainer3.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
			this.splitContainer3.ResumeLayout(false);
			this.splitContainer2.Panel1.ResumeLayout(false);
			this.splitContainer2.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
			this.splitContainer2.ResumeLayout(false);
			this.splitContainer4.Panel1.ResumeLayout(false);
			this.splitContainer4.Panel1.PerformLayout();
			this.splitContainer4.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).EndInit();
			this.splitContainer4.ResumeLayout(false);
			this.grpStreaming.ResumeLayout(false);
			this.splitContainer5.Panel1.ResumeLayout(false);
			this.splitContainer5.Panel1.PerformLayout();
			this.splitContainer5.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer5)).EndInit();
			this.splitContainer5.ResumeLayout(false);
			this.grpBroker.ResumeLayout(false);
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		private System.Windows.Forms.StatusStrip statusStrip1;
		private ToolStripImproved.ToolStripItemComboBox tsiCbxScale;
		public System.Windows.Forms.ToolStripButton tsiBtnSave;
		private LabeledTextBox.ToolStripItemLabeledTextBox tsiLtbDataSourceName;
		private LabeledTextBox.ToolStripItemLabeledTextBox tsiLtbSymbols;
		private ToolStripImproved.ToolStripItemNumericUpDownWithMouseEvents tsiNudInterval;
	}
}
