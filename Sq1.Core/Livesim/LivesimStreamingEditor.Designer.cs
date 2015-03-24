using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;

using Sq1.Core.Streaming;

namespace Sq1.Core.Livesim {
	[ToolboxBitmap(typeof(LivesimStreamingEditor), "StreamingLivesimEditor")]
	[Designer("System.Windows.Forms.Design.ParentControlDesigner, System.Design", typeof(IDesigner))]
	public partial class LivesimStreamingEditor : StreamingEditor {
		#region Component Designer generated code
		private System.ComponentModel.IContainer components = null;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox txt_QuoteGenerationFreezeMillisMax;
		private System.Windows.Forms.TextBox txt_QuoteGenerationFreezeMillisMin;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.CheckBox cbx_QuoteGenerationFreezeEnabled;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.CheckBox cbx_DelayBetweenSerialQuotesEnabled;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.TextBox txt_DelayBetweenSerialQuotesMax;
		private System.Windows.Forms.TextBox txt_DelayBetweenSerialQuotesMin;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox txt_AdaperDisconnectReconnectsAfterMillisMax;
		private System.Windows.Forms.TextBox txt_AdaperDisconnectReconnectsAfterMillisMin;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.CheckBox cbx_AdaperDisconnectEnabled;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.TextBox txt_AdaperDisconnectHappensOncePerQuoteMax;
		private System.Windows.Forms.TextBox txt_AdaperDisconnectHappensOncePerQuoteMin;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.Label label16;
		private System.Windows.Forms.Label label17;
		private System.Windows.Forms.TextBox txt_OutOfOrderQuoteGenerationDelayMillisMax;
		private System.Windows.Forms.TextBox txt_OutOfOrderQuoteGenerationDelayMillisMin;
		private System.Windows.Forms.Label label18;
		private System.Windows.Forms.Label label19;
		private System.Windows.Forms.CheckBox cbx_OutOfOrderQuoteGenerationEnabled;
		private System.Windows.Forms.Label label20;
		private System.Windows.Forms.TextBox txt_OutOfOrderQuoteGenerationHappensOncePerQuoteMax;
		private System.Windows.Forms.TextBox txt_OutOfOrderQuoteGenerationHappensOncePerQuoteMin;
		private System.Windows.Forms.Label label21;
		private void InitializeComponent() {
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.txt_QuoteGenerationFreezeHappensOncePerQuoteMax = new System.Windows.Forms.TextBox();
			this.txt_QuoteGenerationFreezeHappensOncePerQuoteMin = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.txt_QuoteGenerationFreezeMillisMax = new System.Windows.Forms.TextBox();
			this.txt_QuoteGenerationFreezeMillisMin = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.cbx_QuoteGenerationFreezeEnabled = new System.Windows.Forms.CheckBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.label9 = new System.Windows.Forms.Label();
			this.cbx_DelayBetweenSerialQuotesEnabled = new System.Windows.Forms.CheckBox();
			this.label10 = new System.Windows.Forms.Label();
			this.txt_DelayBetweenSerialQuotesMax = new System.Windows.Forms.TextBox();
			this.txt_DelayBetweenSerialQuotesMin = new System.Windows.Forms.TextBox();
			this.label11 = new System.Windows.Forms.Label();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.txt_AdaperDisconnectReconnectsAfterMillisMax = new System.Windows.Forms.TextBox();
			this.txt_AdaperDisconnectReconnectsAfterMillisMin = new System.Windows.Forms.TextBox();
			this.label12 = new System.Windows.Forms.Label();
			this.label13 = new System.Windows.Forms.Label();
			this.cbx_AdaperDisconnectEnabled = new System.Windows.Forms.CheckBox();
			this.label14 = new System.Windows.Forms.Label();
			this.txt_AdaperDisconnectHappensOncePerQuoteMax = new System.Windows.Forms.TextBox();
			this.txt_AdaperDisconnectHappensOncePerQuoteMin = new System.Windows.Forms.TextBox();
			this.label15 = new System.Windows.Forms.Label();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.label16 = new System.Windows.Forms.Label();
			this.label17 = new System.Windows.Forms.Label();
			this.txt_OutOfOrderQuoteGenerationDelayMillisMax = new System.Windows.Forms.TextBox();
			this.txt_OutOfOrderQuoteGenerationDelayMillisMin = new System.Windows.Forms.TextBox();
			this.label18 = new System.Windows.Forms.Label();
			this.label19 = new System.Windows.Forms.Label();
			this.cbx_OutOfOrderQuoteGenerationEnabled = new System.Windows.Forms.CheckBox();
			this.label20 = new System.Windows.Forms.Label();
			this.txt_OutOfOrderQuoteGenerationHappensOncePerQuoteMax = new System.Windows.Forms.TextBox();
			this.txt_OutOfOrderQuoteGenerationHappensOncePerQuoteMin = new System.Windows.Forms.TextBox();
			this.label21 = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.txt_QuoteGenerationFreezeHappensOncePerQuoteMax);
			this.groupBox1.Controls.Add(this.txt_QuoteGenerationFreezeHappensOncePerQuoteMin);
			this.groupBox1.Controls.Add(this.label8);
			this.groupBox1.Controls.Add(this.label6);
			this.groupBox1.Controls.Add(this.txt_QuoteGenerationFreezeMillisMax);
			this.groupBox1.Controls.Add(this.txt_QuoteGenerationFreezeMillisMin);
			this.groupBox1.Controls.Add(this.label7);
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Controls.Add(this.cbx_QuoteGenerationFreezeEnabled);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Location = new System.Drawing.Point(4, 196);
			this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.groupBox1.Size = new System.Drawing.Size(460, 112);
			this.groupBox1.TabIndex = 27;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Quote Generation Suspension (DataFeed freeze)";
			// 
			// txt_QuoteGenerationFreezeHappensOncePerQuoteMax
			// 
			this.txt_QuoteGenerationFreezeHappensOncePerQuoteMax.Location = new System.Drawing.Point(247, 23);
			this.txt_QuoteGenerationFreezeHappensOncePerQuoteMax.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.txt_QuoteGenerationFreezeHappensOncePerQuoteMax.Name = "txt_QuoteGenerationFreezeHappensOncePerQuoteMax";
			this.txt_QuoteGenerationFreezeHappensOncePerQuoteMax.Size = new System.Drawing.Size(59, 22);
			this.txt_QuoteGenerationFreezeHappensOncePerQuoteMax.TabIndex = 10;
			this.txt_QuoteGenerationFreezeHappensOncePerQuoteMax.Enter += new System.EventHandler(this.anyTextBox_Enter);
			this.txt_QuoteGenerationFreezeHappensOncePerQuoteMax.KeyUp += new System.Windows.Forms.KeyEventHandler(this.anyTextBox_KeyUp);
			// 
			// txt_QuoteGenerationFreezeHappensOncePerQuoteMin
			// 
			this.txt_QuoteGenerationFreezeHappensOncePerQuoteMin.Location = new System.Drawing.Point(159, 23);
			this.txt_QuoteGenerationFreezeHappensOncePerQuoteMin.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.txt_QuoteGenerationFreezeHappensOncePerQuoteMin.Name = "txt_QuoteGenerationFreezeHappensOncePerQuoteMin";
			this.txt_QuoteGenerationFreezeHappensOncePerQuoteMin.Size = new System.Drawing.Size(59, 22);
			this.txt_QuoteGenerationFreezeHappensOncePerQuoteMin.TabIndex = 9;
			this.txt_QuoteGenerationFreezeHappensOncePerQuoteMin.Enter += new System.EventHandler(this.anyTextBox_Enter);
			this.txt_QuoteGenerationFreezeHappensOncePerQuoteMin.KeyUp += new System.Windows.Forms.KeyEventHandler(this.anyTextBox_KeyUp);
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(8, 27);
			this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(128, 21);
			this.label8.TabIndex = 28;
			this.label8.Text = "Happens once per";
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(223, 59);
			this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(21, 18);
			this.label6.TabIndex = 27;
			this.label6.Text = "to";
			// 
			// txt_QuoteGenerationFreezeMillisMax
			// 
			this.txt_QuoteGenerationFreezeMillisMax.Location = new System.Drawing.Point(247, 55);
			this.txt_QuoteGenerationFreezeMillisMax.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.txt_QuoteGenerationFreezeMillisMax.Name = "txt_QuoteGenerationFreezeMillisMax";
			this.txt_QuoteGenerationFreezeMillisMax.Size = new System.Drawing.Size(59, 22);
			this.txt_QuoteGenerationFreezeMillisMax.TabIndex = 12;
			this.txt_QuoteGenerationFreezeMillisMax.Enter += new System.EventHandler(this.anyTextBox_Enter);
			this.txt_QuoteGenerationFreezeMillisMax.KeyUp += new System.Windows.Forms.KeyEventHandler(this.anyTextBox_KeyUp);
			// 
			// txt_QuoteGenerationFreezeMillisMin
			// 
			this.txt_QuoteGenerationFreezeMillisMin.Location = new System.Drawing.Point(159, 55);
			this.txt_QuoteGenerationFreezeMillisMin.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.txt_QuoteGenerationFreezeMillisMin.Name = "txt_QuoteGenerationFreezeMillisMin";
			this.txt_QuoteGenerationFreezeMillisMin.Size = new System.Drawing.Size(59, 22);
			this.txt_QuoteGenerationFreezeMillisMin.TabIndex = 11;
			this.txt_QuoteGenerationFreezeMillisMin.Enter += new System.EventHandler(this.anyTextBox_Enter);
			this.txt_QuoteGenerationFreezeMillisMin.KeyUp += new System.Windows.Forms.KeyEventHandler(this.anyTextBox_KeyUp);
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(315, 59);
			this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(124, 21);
			this.label7.TabIndex = 26;
			this.label7.Text = "milliSeconds";
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(8, 59);
			this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(143, 21);
			this.label5.TabIndex = 23;
			this.label5.Text = "Generator stops for";
			// 
			// cbx_QuoteGenerationFreezeEnabled
			// 
			this.cbx_QuoteGenerationFreezeEnabled.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.cbx_QuoteGenerationFreezeEnabled.Checked = true;
			this.cbx_QuoteGenerationFreezeEnabled.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbx_QuoteGenerationFreezeEnabled.Location = new System.Drawing.Point(8, 85);
			this.cbx_QuoteGenerationFreezeEnabled.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.cbx_QuoteGenerationFreezeEnabled.Name = "cbx_QuoteGenerationFreezeEnabled";
			this.cbx_QuoteGenerationFreezeEnabled.Size = new System.Drawing.Size(444, 21);
			this.cbx_QuoteGenerationFreezeEnabled.TabIndex = 13;
			this.cbx_QuoteGenerationFreezeEnabled.Text = "Enable Random DataFeed Freeze";
			this.cbx_QuoteGenerationFreezeEnabled.UseVisualStyleBackColor = true;
			this.cbx_QuoteGenerationFreezeEnabled.CheckedChanged += new System.EventHandler(this.anyCheckBox_CheckedChanged);
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(223, 27);
			this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(21, 18);
			this.label4.TabIndex = 22;
			this.label4.Text = "to";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(315, 27);
			this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(124, 21);
			this.label3.TabIndex = 20;
			this.label3.Text = "quotes generated";
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox2.Controls.Add(this.label9);
			this.groupBox2.Controls.Add(this.cbx_DelayBetweenSerialQuotesEnabled);
			this.groupBox2.Controls.Add(this.label10);
			this.groupBox2.Controls.Add(this.txt_DelayBetweenSerialQuotesMax);
			this.groupBox2.Controls.Add(this.txt_DelayBetweenSerialQuotesMin);
			this.groupBox2.Controls.Add(this.label11);
			this.groupBox2.Location = new System.Drawing.Point(4, 4);
			this.groupBox2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.groupBox2.Size = new System.Drawing.Size(460, 80);
			this.groupBox2.TabIndex = 28;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Quote Generation Speed";
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(8, 25);
			this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(157, 21);
			this.label9.TabIndex = 34;
			this.label9.Text = "Delay between quotes";
			// 
			// cbx_DelayBetweenSerialQuotesEnabled
			// 
			this.cbx_DelayBetweenSerialQuotesEnabled.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.cbx_DelayBetweenSerialQuotesEnabled.Checked = true;
			this.cbx_DelayBetweenSerialQuotesEnabled.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbx_DelayBetweenSerialQuotesEnabled.Location = new System.Drawing.Point(8, 52);
			this.cbx_DelayBetweenSerialQuotesEnabled.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.cbx_DelayBetweenSerialQuotesEnabled.Name = "cbx_DelayBetweenSerialQuotesEnabled";
			this.cbx_DelayBetweenSerialQuotesEnabled.Size = new System.Drawing.Size(444, 21);
			this.cbx_DelayBetweenSerialQuotesEnabled.TabIndex = 3;
			this.cbx_DelayBetweenSerialQuotesEnabled.Text = "Enable Random Delay between generated quotes";
			this.cbx_DelayBetweenSerialQuotesEnabled.UseVisualStyleBackColor = true;
			this.cbx_DelayBetweenSerialQuotesEnabled.CheckedChanged += new System.EventHandler(this.anyCheckBox_CheckedChanged);
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(237, 25);
			this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(21, 18);
			this.label10.TabIndex = 33;
			this.label10.Text = "to";
			// 
			// txt_DelayBetweenSerialQuotesMax
			// 
			this.txt_DelayBetweenSerialQuotesMax.Location = new System.Drawing.Point(261, 21);
			this.txt_DelayBetweenSerialQuotesMax.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.txt_DelayBetweenSerialQuotesMax.Name = "txt_DelayBetweenSerialQuotesMax";
			this.txt_DelayBetweenSerialQuotesMax.Size = new System.Drawing.Size(59, 22);
			this.txt_DelayBetweenSerialQuotesMax.TabIndex = 2;
			this.txt_DelayBetweenSerialQuotesMax.Enter += new System.EventHandler(this.anyTextBox_Enter);
			this.txt_DelayBetweenSerialQuotesMax.KeyUp += new System.Windows.Forms.KeyEventHandler(this.anyTextBox_KeyUp);
			// 
			// txt_DelayBetweenSerialQuotesMin
			// 
			this.txt_DelayBetweenSerialQuotesMin.Location = new System.Drawing.Point(173, 21);
			this.txt_DelayBetweenSerialQuotesMin.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.txt_DelayBetweenSerialQuotesMin.Name = "txt_DelayBetweenSerialQuotesMin";
			this.txt_DelayBetweenSerialQuotesMin.Size = new System.Drawing.Size(59, 22);
			this.txt_DelayBetweenSerialQuotesMin.TabIndex = 1;
			this.txt_DelayBetweenSerialQuotesMin.Enter += new System.EventHandler(this.anyTextBox_Enter);
			this.txt_DelayBetweenSerialQuotesMin.KeyUp += new System.Windows.Forms.KeyEventHandler(this.anyTextBox_KeyUp);
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(329, 25);
			this.label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(91, 21);
			this.label11.TabIndex = 31;
			this.label11.Text = "milliSeconds";
			// 
			// groupBox3
			// 
			this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox3.Controls.Add(this.label1);
			this.groupBox3.Controls.Add(this.label2);
			this.groupBox3.Controls.Add(this.txt_AdaperDisconnectReconnectsAfterMillisMax);
			this.groupBox3.Controls.Add(this.txt_AdaperDisconnectReconnectsAfterMillisMin);
			this.groupBox3.Controls.Add(this.label12);
			this.groupBox3.Controls.Add(this.label13);
			this.groupBox3.Controls.Add(this.cbx_AdaperDisconnectEnabled);
			this.groupBox3.Controls.Add(this.label14);
			this.groupBox3.Controls.Add(this.txt_AdaperDisconnectHappensOncePerQuoteMax);
			this.groupBox3.Controls.Add(this.txt_AdaperDisconnectHappensOncePerQuoteMin);
			this.groupBox3.Controls.Add(this.label15);
			this.groupBox3.Location = new System.Drawing.Point(4, 308);
			this.groupBox3.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.groupBox3.Size = new System.Drawing.Size(460, 111);
			this.groupBox3.TabIndex = 29;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Streaming Adapter Disconnect";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 27);
			this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(128, 21);
			this.label1.TabIndex = 28;
			this.label1.Text = "Happens once per";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(223, 59);
			this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(21, 18);
			this.label2.TabIndex = 27;
			this.label2.Text = "to";
			// 
			// txt_AdaperDisconnectReconnectsAfterMillisMax
			// 
			this.txt_AdaperDisconnectReconnectsAfterMillisMax.Location = new System.Drawing.Point(247, 55);
			this.txt_AdaperDisconnectReconnectsAfterMillisMax.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.txt_AdaperDisconnectReconnectsAfterMillisMax.Name = "txt_AdaperDisconnectReconnectsAfterMillisMax";
			this.txt_AdaperDisconnectReconnectsAfterMillisMax.Size = new System.Drawing.Size(59, 22);
			this.txt_AdaperDisconnectReconnectsAfterMillisMax.TabIndex = 17;
			this.txt_AdaperDisconnectReconnectsAfterMillisMax.Enter += new System.EventHandler(this.anyTextBox_Enter);
			this.txt_AdaperDisconnectReconnectsAfterMillisMax.KeyUp += new System.Windows.Forms.KeyEventHandler(this.anyTextBox_KeyUp);
			// 
			// txt_AdaperDisconnectReconnectsAfterMillisMin
			// 
			this.txt_AdaperDisconnectReconnectsAfterMillisMin.Location = new System.Drawing.Point(159, 55);
			this.txt_AdaperDisconnectReconnectsAfterMillisMin.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.txt_AdaperDisconnectReconnectsAfterMillisMin.Name = "txt_AdaperDisconnectReconnectsAfterMillisMin";
			this.txt_AdaperDisconnectReconnectsAfterMillisMin.Size = new System.Drawing.Size(59, 22);
			this.txt_AdaperDisconnectReconnectsAfterMillisMin.TabIndex = 16;
			this.txt_AdaperDisconnectReconnectsAfterMillisMin.Enter += new System.EventHandler(this.anyTextBox_Enter);
			this.txt_AdaperDisconnectReconnectsAfterMillisMin.KeyUp += new System.Windows.Forms.KeyEventHandler(this.anyTextBox_KeyUp);
			// 
			// label12
			// 
			this.label12.Location = new System.Drawing.Point(315, 59);
			this.label12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(124, 21);
			this.label12.TabIndex = 26;
			this.label12.Text = "milliSeconds";
			// 
			// label13
			// 
			this.label13.Location = new System.Drawing.Point(8, 59);
			this.label13.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(143, 21);
			this.label13.TabIndex = 23;
			this.label13.Text = "ReConnect after";
			// 
			// cbx_AdaperDisconnectEnabled
			// 
			this.cbx_AdaperDisconnectEnabled.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.cbx_AdaperDisconnectEnabled.Checked = true;
			this.cbx_AdaperDisconnectEnabled.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbx_AdaperDisconnectEnabled.Location = new System.Drawing.Point(8, 82);
			this.cbx_AdaperDisconnectEnabled.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.cbx_AdaperDisconnectEnabled.Name = "cbx_AdaperDisconnectEnabled";
			this.cbx_AdaperDisconnectEnabled.Size = new System.Drawing.Size(444, 21);
			this.cbx_AdaperDisconnectEnabled.TabIndex = 18;
			this.cbx_AdaperDisconnectEnabled.Text = "Enable Random Streaming Adapter Disconnect";
			this.cbx_AdaperDisconnectEnabled.UseVisualStyleBackColor = true;
			this.cbx_AdaperDisconnectEnabled.CheckedChanged += new System.EventHandler(this.anyCheckBox_CheckedChanged);
			// 
			// label14
			// 
			this.label14.Location = new System.Drawing.Point(223, 27);
			this.label14.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(21, 18);
			this.label14.TabIndex = 22;
			this.label14.Text = "to";
			// 
			// txt_AdaperDisconnectHappensOncePerQuoteMax
			// 
			this.txt_AdaperDisconnectHappensOncePerQuoteMax.Location = new System.Drawing.Point(247, 23);
			this.txt_AdaperDisconnectHappensOncePerQuoteMax.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.txt_AdaperDisconnectHappensOncePerQuoteMax.Name = "txt_AdaperDisconnectHappensOncePerQuoteMax";
			this.txt_AdaperDisconnectHappensOncePerQuoteMax.Size = new System.Drawing.Size(59, 22);
			this.txt_AdaperDisconnectHappensOncePerQuoteMax.TabIndex = 15;
			this.txt_AdaperDisconnectHappensOncePerQuoteMax.Enter += new System.EventHandler(this.anyTextBox_Enter);
			this.txt_AdaperDisconnectHappensOncePerQuoteMax.KeyUp += new System.Windows.Forms.KeyEventHandler(this.anyTextBox_KeyUp);
			// 
			// txt_AdaperDisconnectHappensOncePerQuoteMin
			// 
			this.txt_AdaperDisconnectHappensOncePerQuoteMin.Location = new System.Drawing.Point(159, 23);
			this.txt_AdaperDisconnectHappensOncePerQuoteMin.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.txt_AdaperDisconnectHappensOncePerQuoteMin.Name = "txt_AdaperDisconnectHappensOncePerQuoteMin";
			this.txt_AdaperDisconnectHappensOncePerQuoteMin.Size = new System.Drawing.Size(59, 22);
			this.txt_AdaperDisconnectHappensOncePerQuoteMin.TabIndex = 14;
			this.txt_AdaperDisconnectHappensOncePerQuoteMin.Enter += new System.EventHandler(this.anyTextBox_Enter);
			this.txt_AdaperDisconnectHappensOncePerQuoteMin.KeyUp += new System.Windows.Forms.KeyEventHandler(this.anyTextBox_KeyUp);
			// 
			// label15
			// 
			this.label15.Location = new System.Drawing.Point(315, 27);
			this.label15.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(124, 21);
			this.label15.TabIndex = 20;
			this.label15.Text = "quotes generated";
			// 
			// groupBox4
			// 
			this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox4.Controls.Add(this.label16);
			this.groupBox4.Controls.Add(this.label17);
			this.groupBox4.Controls.Add(this.txt_OutOfOrderQuoteGenerationDelayMillisMax);
			this.groupBox4.Controls.Add(this.txt_OutOfOrderQuoteGenerationDelayMillisMin);
			this.groupBox4.Controls.Add(this.label18);
			this.groupBox4.Controls.Add(this.label19);
			this.groupBox4.Controls.Add(this.cbx_OutOfOrderQuoteGenerationEnabled);
			this.groupBox4.Controls.Add(this.label20);
			this.groupBox4.Controls.Add(this.txt_OutOfOrderQuoteGenerationHappensOncePerQuoteMax);
			this.groupBox4.Controls.Add(this.txt_OutOfOrderQuoteGenerationHappensOncePerQuoteMin);
			this.groupBox4.Controls.Add(this.label21);
			this.groupBox4.Location = new System.Drawing.Point(4, 84);
			this.groupBox4.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.groupBox4.Size = new System.Drawing.Size(460, 112);
			this.groupBox4.TabIndex = 30;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Out-of-order Quote Generation";
			// 
			// label16
			// 
			this.label16.Location = new System.Drawing.Point(8, 27);
			this.label16.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(128, 21);
			this.label16.TabIndex = 28;
			this.label16.Text = "Happens once per";
			// 
			// label17
			// 
			this.label17.Location = new System.Drawing.Point(223, 59);
			this.label17.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label17.Name = "label17";
			this.label17.Size = new System.Drawing.Size(21, 18);
			this.label17.TabIndex = 27;
			this.label17.Text = "to";
			// 
			// txt_OutOfOrderQuoteGenerationDelayMillisMax
			// 
			this.txt_OutOfOrderQuoteGenerationDelayMillisMax.Location = new System.Drawing.Point(247, 55);
			this.txt_OutOfOrderQuoteGenerationDelayMillisMax.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.txt_OutOfOrderQuoteGenerationDelayMillisMax.Name = "txt_OutOfOrderQuoteGenerationDelayMillisMax";
			this.txt_OutOfOrderQuoteGenerationDelayMillisMax.Size = new System.Drawing.Size(59, 22);
			this.txt_OutOfOrderQuoteGenerationDelayMillisMax.TabIndex = 7;
			this.txt_OutOfOrderQuoteGenerationDelayMillisMax.Enter += new System.EventHandler(this.anyTextBox_Enter);
			this.txt_OutOfOrderQuoteGenerationDelayMillisMax.KeyUp += new System.Windows.Forms.KeyEventHandler(this.anyTextBox_KeyUp);
			// 
			// txt_OutOfOrderQuoteGenerationDelayMillisMin
			// 
			this.txt_OutOfOrderQuoteGenerationDelayMillisMin.Location = new System.Drawing.Point(159, 55);
			this.txt_OutOfOrderQuoteGenerationDelayMillisMin.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.txt_OutOfOrderQuoteGenerationDelayMillisMin.Name = "txt_OutOfOrderQuoteGenerationDelayMillisMin";
			this.txt_OutOfOrderQuoteGenerationDelayMillisMin.Size = new System.Drawing.Size(59, 22);
			this.txt_OutOfOrderQuoteGenerationDelayMillisMin.TabIndex = 6;
			this.txt_OutOfOrderQuoteGenerationDelayMillisMin.Enter += new System.EventHandler(this.anyTextBox_Enter);
			this.txt_OutOfOrderQuoteGenerationDelayMillisMin.KeyUp += new System.Windows.Forms.KeyEventHandler(this.anyTextBox_KeyUp);
			// 
			// label18
			// 
			this.label18.Location = new System.Drawing.Point(315, 59);
			this.label18.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label18.Name = "label18";
			this.label18.Size = new System.Drawing.Size(124, 21);
			this.label18.TabIndex = 26;
			this.label18.Text = "milliSeconds later";
			// 
			// label19
			// 
			this.label19.Location = new System.Drawing.Point(8, 59);
			this.label19.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label19.Name = "label19";
			this.label19.Size = new System.Drawing.Size(143, 21);
			this.label19.TabIndex = 23;
			this.label19.Text = "Quote Arrives";
			// 
			// cbx_OutOfOrderQuoteGenerationEnabled
			// 
			this.cbx_OutOfOrderQuoteGenerationEnabled.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.cbx_OutOfOrderQuoteGenerationEnabled.Checked = true;
			this.cbx_OutOfOrderQuoteGenerationEnabled.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbx_OutOfOrderQuoteGenerationEnabled.Location = new System.Drawing.Point(8, 86);
			this.cbx_OutOfOrderQuoteGenerationEnabled.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.cbx_OutOfOrderQuoteGenerationEnabled.Name = "cbx_OutOfOrderQuoteGenerationEnabled";
			this.cbx_OutOfOrderQuoteGenerationEnabled.Size = new System.Drawing.Size(444, 21);
			this.cbx_OutOfOrderQuoteGenerationEnabled.TabIndex = 8;
			this.cbx_OutOfOrderQuoteGenerationEnabled.Text = "Enable Random Out-of-order Quote Generation";
			this.cbx_OutOfOrderQuoteGenerationEnabled.UseVisualStyleBackColor = true;
			this.cbx_OutOfOrderQuoteGenerationEnabled.CheckedChanged += new System.EventHandler(this.anyCheckBox_CheckedChanged);
			// 
			// label20
			// 
			this.label20.Location = new System.Drawing.Point(223, 27);
			this.label20.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label20.Name = "label20";
			this.label20.Size = new System.Drawing.Size(21, 18);
			this.label20.TabIndex = 22;
			this.label20.Text = "to";
			// 
			// txt_OutOfOrderQuoteGenerationHappensOncePerQuoteMax
			// 
			this.txt_OutOfOrderQuoteGenerationHappensOncePerQuoteMax.Location = new System.Drawing.Point(247, 23);
			this.txt_OutOfOrderQuoteGenerationHappensOncePerQuoteMax.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.txt_OutOfOrderQuoteGenerationHappensOncePerQuoteMax.Name = "txt_OutOfOrderQuoteGenerationHappensOncePerQuoteMax";
			this.txt_OutOfOrderQuoteGenerationHappensOncePerQuoteMax.Size = new System.Drawing.Size(59, 22);
			this.txt_OutOfOrderQuoteGenerationHappensOncePerQuoteMax.TabIndex = 5;
			this.txt_OutOfOrderQuoteGenerationHappensOncePerQuoteMax.Enter += new System.EventHandler(this.anyTextBox_Enter);
			this.txt_OutOfOrderQuoteGenerationHappensOncePerQuoteMax.KeyUp += new System.Windows.Forms.KeyEventHandler(this.anyTextBox_KeyUp);
			// 
			// txt_OutOfOrderQuoteGenerationHappensOncePerQuoteMin
			// 
			this.txt_OutOfOrderQuoteGenerationHappensOncePerQuoteMin.Location = new System.Drawing.Point(159, 23);
			this.txt_OutOfOrderQuoteGenerationHappensOncePerQuoteMin.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.txt_OutOfOrderQuoteGenerationHappensOncePerQuoteMin.Name = "txt_OutOfOrderQuoteGenerationHappensOncePerQuoteMin";
			this.txt_OutOfOrderQuoteGenerationHappensOncePerQuoteMin.Size = new System.Drawing.Size(59, 22);
			this.txt_OutOfOrderQuoteGenerationHappensOncePerQuoteMin.TabIndex = 4;
			this.txt_OutOfOrderQuoteGenerationHappensOncePerQuoteMin.Enter += new System.EventHandler(this.anyTextBox_Enter);
			this.txt_OutOfOrderQuoteGenerationHappensOncePerQuoteMin.KeyUp += new System.Windows.Forms.KeyEventHandler(this.anyTextBox_KeyUp);
			// 
			// label21
			// 
			this.label21.Location = new System.Drawing.Point(315, 27);
			this.label21.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label21.Name = "label21";
			this.label21.Size = new System.Drawing.Size(124, 21);
			this.label21.TabIndex = 20;
			this.label21.Text = "quotes generated";
			// 
			// LivesimStreamingEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoScroll = true;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.Controls.Add(this.groupBox4);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.Name = "LivesimStreamingEditor";
			this.Size = new System.Drawing.Size(468, 425);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.groupBox4.ResumeLayout(false);
			this.groupBox4.PerformLayout();
			this.ResumeLayout(false);

		}
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#endregion

		private System.Windows.Forms.TextBox txt_QuoteGenerationFreezeHappensOncePerQuoteMax;
		private System.Windows.Forms.TextBox txt_QuoteGenerationFreezeHappensOncePerQuoteMin;
	}
}