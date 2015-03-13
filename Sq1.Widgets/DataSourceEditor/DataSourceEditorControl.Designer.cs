namespace Sq1.Widgets.DataSourceEditor {
	partial class DataSourceEditorControl {
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.Panel pnlIntro;
		private System.Windows.Forms.Panel pnlContent;
		private System.Windows.Forms.Button btnSave;
		private System.Windows.Forms.ListView lvBrokerAdapters;
		private System.Windows.Forms.ListView lvStreamingAdapters;
		private System.Windows.Forms.ImageList imglStreamingAdapters;
		private System.Windows.Forms.ImageList imglBrokerAdapters;
		private System.Windows.Forms.TextBox txtDataSourceName;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.Label lblDataSourceName;
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
		private System.Windows.Forms.GroupBox grpExecution;
		private System.Windows.Forms.Label lblSymbols;
		private System.Windows.Forms.TextBox txtSymbols;
		private System.Windows.Forms.NumericUpDown nmrInterval;
		private System.Windows.Forms.Label lblInterval;
		private System.Windows.Forms.ComboBox cmbScale;
		private System.Windows.Forms.Label lblScale;
		
		protected override void Dispose(bool disposing) {
			if (disposing && this.components != null) {
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.btnSave = new System.Windows.Forms.Button();
			this.pnlIntro = new System.Windows.Forms.Panel();
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
			this.grpExecution = new System.Windows.Forms.GroupBox();
			this.pnlBrokerEditor = new System.Windows.Forms.Panel();
			this.marketInfoEditor = new Sq1.Widgets.DataSourceEditor.MarketInfoEditor();
			this.txtDataSourceName = new System.Windows.Forms.TextBox();
			this.pnlContent = new System.Windows.Forms.Panel();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.lblScale = new System.Windows.Forms.Label();
			this.nmrInterval = new System.Windows.Forms.NumericUpDown();
			this.txtSymbols = new System.Windows.Forms.TextBox();
			this.cmbScale = new System.Windows.Forms.ComboBox();
			this.lblDataSourceName = new System.Windows.Forms.Label();
			this.lblSymbols = new System.Windows.Forms.Label();
			this.lblInterval = new System.Windows.Forms.Label();
			this.pnlIntro.SuspendLayout();
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
			this.grpExecution.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nmrInterval)).BeginInit();
			this.SuspendLayout();
			// 
			// btnSave
			// 
			this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.btnSave.Location = new System.Drawing.Point(6, 399);
			this.btnSave.Name = "btnSave";
			this.btnSave.Size = new System.Drawing.Size(128, 23);
			this.btnSave.TabIndex = 0;
			this.btnSave.Text = "Save";
			this.btnSave.UseVisualStyleBackColor = true;
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
			// 
			// pnlIntro
			// 
			this.pnlIntro.Controls.Add(this.splitContainer3);
			this.pnlIntro.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlIntro.Location = new System.Drawing.Point(0, 0);
			this.pnlIntro.Name = "pnlIntro";
			this.pnlIntro.Size = new System.Drawing.Size(525, 425);
			this.pnlIntro.TabIndex = 0;
			// 
			// splitContainer3
			// 
			this.splitContainer3.BackColor = System.Drawing.SystemColors.ControlDark;
			this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
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
			this.splitContainer3.Size = new System.Drawing.Size(525, 425);
			this.splitContainer3.SplitterDistance = 255;
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
			this.splitContainer2.Size = new System.Drawing.Size(525, 255);
			this.splitContainer2.SplitterDistance = 251;
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
			this.splitContainer4.Size = new System.Drawing.Size(251, 255);
			this.splitContainer4.SplitterDistance = 133;
			this.splitContainer4.TabIndex = 7;
			// 
			// lblStreaming
			// 
			this.lblStreaming.AutoSize = true;
			this.lblStreaming.Location = new System.Drawing.Point(5, 0);
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
			this.lvStreamingAdapters.Location = new System.Drawing.Point(5, 17);
			this.lvStreamingAdapters.MultiSelect = false;
			this.lvStreamingAdapters.Name = "lvStreamingAdapters";
			this.lvStreamingAdapters.Size = new System.Drawing.Size(243, 113);
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
			this.grpStreaming.Location = new System.Drawing.Point(5, 3);
			this.grpStreaming.Name = "grpStreaming";
			this.grpStreaming.Size = new System.Drawing.Size(243, 115);
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
			this.pnlStreamingEditor.Size = new System.Drawing.Size(237, 96);
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
			this.splitContainer5.Panel2.Controls.Add(this.grpExecution);
			this.splitContainer5.Size = new System.Drawing.Size(270, 255);
			this.splitContainer5.SplitterDistance = 133;
			this.splitContainer5.TabIndex = 11;
			// 
			// lblExecution
			// 
			this.lblExecution.AutoSize = true;
			this.lblExecution.Location = new System.Drawing.Point(3, 0);
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
			this.lvBrokerAdapters.Location = new System.Drawing.Point(3, 19);
			this.lvBrokerAdapters.MultiSelect = false;
			this.lvBrokerAdapters.Name = "lvBrokerAdapters";
			this.lvBrokerAdapters.Size = new System.Drawing.Size(261, 111);
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
			// grpExecution
			// 
			this.grpExecution.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.grpExecution.Controls.Add(this.pnlBrokerEditor);
			this.grpExecution.Location = new System.Drawing.Point(3, 3);
			this.grpExecution.Name = "grpExecution";
			this.grpExecution.Size = new System.Drawing.Size(264, 115);
			this.grpExecution.TabIndex = 10;
			this.grpExecution.TabStop = false;
			this.grpExecution.Text = "Broker Settings";
			// 
			// pnlBrokerEditor
			// 
			this.pnlBrokerEditor.AutoScroll = true;
			this.pnlBrokerEditor.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.pnlBrokerEditor.BackColor = System.Drawing.SystemColors.Control;
			this.pnlBrokerEditor.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlBrokerEditor.Location = new System.Drawing.Point(3, 16);
			this.pnlBrokerEditor.Name = "pnlBrokerEditor";
			this.pnlBrokerEditor.Size = new System.Drawing.Size(258, 96);
			this.pnlBrokerEditor.TabIndex = 9;
			// 
			// marketInfoEditor
			// 
			this.marketInfoEditor.AutoSize = true;
			this.marketInfoEditor.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.marketInfoEditor.Dock = System.Windows.Forms.DockStyle.Fill;
			this.marketInfoEditor.Location = new System.Drawing.Point(0, 0);
			this.marketInfoEditor.Name = "marketInfoEditor";
			this.marketInfoEditor.Size = new System.Drawing.Size(525, 166);
			this.marketInfoEditor.TabIndex = 0;
			// 
			// txtDataSourceName
			// 
			this.txtDataSourceName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtDataSourceName.Location = new System.Drawing.Point(6, 25);
			this.txtDataSourceName.Name = "txtDataSourceName";
			this.txtDataSourceName.Size = new System.Drawing.Size(128, 20);
			this.txtDataSourceName.TabIndex = 1;
			// 
			// pnlContent
			// 
			this.pnlContent.AutoScroll = true;
			this.pnlContent.AutoSize = true;
			this.pnlContent.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.pnlContent.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlContent.Location = new System.Drawing.Point(0, 0);
			this.pnlContent.Name = "pnlContent";
			this.pnlContent.Size = new System.Drawing.Size(525, 425);
			this.pnlContent.TabIndex = 3;
			// 
			// splitContainer1
			// 
			this.splitContainer1.BackColor = System.Drawing.SystemColors.ControlDark;
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.BackColor = System.Drawing.SystemColors.Control;
			this.splitContainer1.Panel1.Controls.Add(this.lblScale);
			this.splitContainer1.Panel1.Controls.Add(this.btnSave);
			this.splitContainer1.Panel1.Controls.Add(this.nmrInterval);
			this.splitContainer1.Panel1.Controls.Add(this.txtSymbols);
			this.splitContainer1.Panel1.Controls.Add(this.cmbScale);
			this.splitContainer1.Panel1.Controls.Add(this.lblDataSourceName);
			this.splitContainer1.Panel1.Controls.Add(this.lblSymbols);
			this.splitContainer1.Panel1.Controls.Add(this.txtDataSourceName);
			this.splitContainer1.Panel1.Controls.Add(this.lblInterval);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.BackColor = System.Drawing.SystemColors.Control;
			this.splitContainer1.Panel2.Controls.Add(this.pnlIntro);
			this.splitContainer1.Panel2.Controls.Add(this.pnlContent);
			this.splitContainer1.Size = new System.Drawing.Size(669, 425);
			this.splitContainer1.SplitterDistance = 140;
			this.splitContainer1.TabIndex = 4;
			// 
			// lblScale
			// 
			this.lblScale.AutoSize = true;
			this.lblScale.Location = new System.Drawing.Point(6, 51);
			this.lblScale.Name = "lblScale";
			this.lblScale.Size = new System.Drawing.Size(51, 13);
			this.lblScale.TabIndex = 1;
			this.lblScale.Text = "Bar scale";
			// 
			// nmrInterval
			// 
			this.nmrInterval.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.nmrInterval.Location = new System.Drawing.Point(72, 75);
			this.nmrInterval.Maximum = new decimal(new int[] {
			9999,
			0,
			0,
			0});
			this.nmrInterval.Name = "nmrInterval";
			this.nmrInterval.Size = new System.Drawing.Size(62, 20);
			this.nmrInterval.TabIndex = 2;
			this.nmrInterval.Value = new decimal(new int[] {
			5,
			0,
			0,
			0});
			this.nmrInterval.ValueChanged += new System.EventHandler(this.nmrInterval_ValueChanged);
			// 
			// txtSymbols
			// 
			this.txtSymbols.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtSymbols.Location = new System.Drawing.Point(6, 116);
			this.txtSymbols.Multiline = true;
			this.txtSymbols.Name = "txtSymbols";
			this.txtSymbols.Size = new System.Drawing.Size(128, 277);
			this.txtSymbols.TabIndex = 6;
			this.txtSymbols.Text = "RIZ2,RIH3";
			// 
			// cmbScale
			// 
			this.cmbScale.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.cmbScale.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbScale.FormattingEnabled = true;
			this.cmbScale.Items.AddRange(new object[] {
			"Unknown",
			"Tick",
			"Second",
			"Minute",
			"Daily",
			"Weekly",
			"Monthly",
			"Quarterly",
			"Yearly"});
			this.cmbScale.Location = new System.Drawing.Point(72, 48);
			this.cmbScale.Name = "cmbScale";
			this.cmbScale.Size = new System.Drawing.Size(62, 21);
			this.cmbScale.TabIndex = 1;
			this.cmbScale.SelectedIndexChanged += new System.EventHandler(this.cmbScale_SelectedIndexChanged);
			// 
			// lblDataSourceName
			// 
			this.lblDataSourceName.AutoSize = true;
			this.lblDataSourceName.Location = new System.Drawing.Point(6, 8);
			this.lblDataSourceName.Name = "lblDataSourceName";
			this.lblDataSourceName.Size = new System.Drawing.Size(98, 13);
			this.lblDataSourceName.TabIndex = 4;
			this.lblDataSourceName.Text = "Data Source Name";
			// 
			// lblSymbols
			// 
			this.lblSymbols.AutoSize = true;
			this.lblSymbols.Location = new System.Drawing.Point(6, 100);
			this.lblSymbols.Name = "lblSymbols";
			this.lblSymbols.Size = new System.Drawing.Size(46, 13);
			this.lblSymbols.TabIndex = 7;
			this.lblSymbols.Text = "Symbols";
			// 
			// lblInterval
			// 
			this.lblInterval.AutoSize = true;
			this.lblInterval.Location = new System.Drawing.Point(6, 77);
			this.lblInterval.Name = "lblInterval";
			this.lblInterval.Size = new System.Drawing.Size(60, 13);
			this.lblInterval.TabIndex = 2;
			this.lblInterval.Text = "Bar interval";
			// 
			// DataSourceEditorControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.splitContainer1);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.Name = "DataSourceEditorControl";
			this.Size = new System.Drawing.Size(669, 425);
			this.pnlIntro.ResumeLayout(false);
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
			this.grpExecution.ResumeLayout(false);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel1.PerformLayout();
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.nmrInterval)).EndInit();
			this.ResumeLayout(false);

		}
	}
}
