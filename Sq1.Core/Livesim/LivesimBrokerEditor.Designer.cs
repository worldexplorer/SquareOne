using System;
using System.Drawing;
using System.ComponentModel;
using System.ComponentModel.Design;

using Sq1.Core.Broker;

namespace Sq1.Core.Livesim {
	[ToolboxBitmap(typeof(LivesimBrokerEditor), "BrokerLivesimEditor")]
	[Designer("System.Windows.Forms.Design.ParentControlDesigner, System.Design", typeof(IDesigner))]
	public partial class LivesimBrokerEditor : BrokerEditor {
        #region Component Designer generated code
		private System.ComponentModel.IContainer components = null;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textBox3;
		private System.Windows.Forms.TextBox textBox4;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.CheckBox checkBox1;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.TextBox textBox5;
		private System.Windows.Forms.TextBox textBox6;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.TextBox textBox2;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.CheckBox checkBox2;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox textBox7;
		private System.Windows.Forms.TextBox textBox8;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.CheckBox checkBox3;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.TextBox textBox9;
		private System.Windows.Forms.TextBox textBox10;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.Label label16;
		private System.Windows.Forms.Label label17;
		private System.Windows.Forms.TextBox textBox11;
		private System.Windows.Forms.TextBox textBox12;
		private System.Windows.Forms.Label label18;
		private System.Windows.Forms.Label label19;
		private System.Windows.Forms.CheckBox checkBox4;
		private System.Windows.Forms.Label label20;
		private System.Windows.Forms.TextBox textBox13;
		private System.Windows.Forms.TextBox textBox14;
		private System.Windows.Forms.Label label21;
		private System.Windows.Forms.GroupBox groupBox5;
		private System.Windows.Forms.Label label22;
		private System.Windows.Forms.Label label23;
		private System.Windows.Forms.TextBox textBox15;
		private System.Windows.Forms.TextBox textBox16;
		private System.Windows.Forms.Label label24;
		private System.Windows.Forms.Label label25;
		private System.Windows.Forms.CheckBox checkBox5;
		private System.Windows.Forms.Label label26;
		private System.Windows.Forms.TextBox textBox17;
		private System.Windows.Forms.TextBox textBox18;
		private System.Windows.Forms.Label label27;
		private System.Windows.Forms.GroupBox groupBox6;
		private System.Windows.Forms.Label label28;
		private System.Windows.Forms.CheckBox checkBox6;
		private System.Windows.Forms.Label label29;
		private System.Windows.Forms.TextBox textBox19;
		private System.Windows.Forms.TextBox textBox20;
		private System.Windows.Forms.Label label30;
		private void InitializeComponent() {
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.textBox3 = new System.Windows.Forms.TextBox();
			this.textBox4 = new System.Windows.Forms.TextBox();
			this.label9 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.checkBox1 = new System.Windows.Forms.CheckBox();
			this.label11 = new System.Windows.Forms.Label();
			this.textBox5 = new System.Windows.Forms.TextBox();
			this.textBox6 = new System.Windows.Forms.TextBox();
			this.label12 = new System.Windows.Forms.Label();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.label13 = new System.Windows.Forms.Label();
			this.checkBox3 = new System.Windows.Forms.CheckBox();
			this.label14 = new System.Windows.Forms.Label();
			this.textBox9 = new System.Windows.Forms.TextBox();
			this.textBox10 = new System.Windows.Forms.TextBox();
			this.label15 = new System.Windows.Forms.Label();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.label16 = new System.Windows.Forms.Label();
			this.label17 = new System.Windows.Forms.Label();
			this.textBox11 = new System.Windows.Forms.TextBox();
			this.textBox12 = new System.Windows.Forms.TextBox();
			this.label18 = new System.Windows.Forms.Label();
			this.label19 = new System.Windows.Forms.Label();
			this.checkBox4 = new System.Windows.Forms.CheckBox();
			this.label20 = new System.Windows.Forms.Label();
			this.textBox13 = new System.Windows.Forms.TextBox();
			this.textBox14 = new System.Windows.Forms.TextBox();
			this.label21 = new System.Windows.Forms.Label();
			this.groupBox5 = new System.Windows.Forms.GroupBox();
			this.label22 = new System.Windows.Forms.Label();
			this.label23 = new System.Windows.Forms.Label();
			this.textBox15 = new System.Windows.Forms.TextBox();
			this.textBox16 = new System.Windows.Forms.TextBox();
			this.label24 = new System.Windows.Forms.Label();
			this.label25 = new System.Windows.Forms.Label();
			this.checkBox5 = new System.Windows.Forms.CheckBox();
			this.label26 = new System.Windows.Forms.Label();
			this.textBox17 = new System.Windows.Forms.TextBox();
			this.textBox18 = new System.Windows.Forms.TextBox();
			this.label27 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.textBox2 = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.checkBox2 = new System.Windows.Forms.CheckBox();
			this.label7 = new System.Windows.Forms.Label();
			this.textBox7 = new System.Windows.Forms.TextBox();
			this.textBox8 = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.groupBox6 = new System.Windows.Forms.GroupBox();
			this.label28 = new System.Windows.Forms.Label();
			this.checkBox6 = new System.Windows.Forms.CheckBox();
			this.label29 = new System.Windows.Forms.Label();
			this.textBox19 = new System.Windows.Forms.TextBox();
			this.textBox20 = new System.Windows.Forms.TextBox();
			this.label30 = new System.Windows.Forms.Label();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.groupBox5.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.groupBox6.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox2.Controls.Add(this.label1);
			this.groupBox2.Controls.Add(this.label2);
			this.groupBox2.Controls.Add(this.textBox3);
			this.groupBox2.Controls.Add(this.textBox4);
			this.groupBox2.Controls.Add(this.label9);
			this.groupBox2.Controls.Add(this.label10);
			this.groupBox2.Controls.Add(this.checkBox1);
			this.groupBox2.Controls.Add(this.label11);
			this.groupBox2.Controls.Add(this.textBox5);
			this.groupBox2.Controls.Add(this.textBox6);
			this.groupBox2.Controls.Add(this.label12);
			this.groupBox2.Location = new System.Drawing.Point(3, 161);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(345, 108);
			this.groupBox2.TabIndex = 28;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Partial Fills";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(6, 22);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(107, 17);
			this.label1.TabIndex = 28;
			this.label1.Text = "Happens once per";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(167, 48);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(16, 15);
			this.label2.TabIndex = 27;
			this.label2.Text = "to";
			// 
			// textBox3
			// 
			this.textBox3.Enabled = false;
			this.textBox3.Location = new System.Drawing.Point(185, 45);
			this.textBox3.Name = "textBox3";
			this.textBox3.Size = new System.Drawing.Size(45, 20);
			this.textBox3.TabIndex = 24;
			// 
			// textBox4
			// 
			this.textBox4.Enabled = false;
			this.textBox4.Location = new System.Drawing.Point(119, 45);
			this.textBox4.Name = "textBox4";
			this.textBox4.Size = new System.Drawing.Size(45, 20);
			this.textBox4.TabIndex = 25;
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(236, 48);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(93, 17);
			this.label9.TabIndex = 26;
			this.label9.Text = "% order size";
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(6, 48);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(107, 17);
			this.label10.TabIndex = 23;
			this.label10.Text = "How much gets filled";
			// 
			// checkBox1
			// 
			this.checkBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.checkBox1.Appearance = System.Windows.Forms.Appearance.Button;
			this.checkBox1.Checked = true;
			this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBox1.Location = new System.Drawing.Point(6, 78);
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.Size = new System.Drawing.Size(333, 24);
			this.checkBox1.TabIndex = 21;
			this.checkBox1.Text = "Enable Random Partial Fills";
			this.checkBox1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.checkBox1.UseVisualStyleBackColor = true;
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(167, 22);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(16, 15);
			this.label11.TabIndex = 22;
			this.label11.Text = "to";
			// 
			// textBox5
			// 
			this.textBox5.Enabled = false;
			this.textBox5.Location = new System.Drawing.Point(185, 19);
			this.textBox5.Name = "textBox5";
			this.textBox5.Size = new System.Drawing.Size(45, 20);
			this.textBox5.TabIndex = 13;
			// 
			// textBox6
			// 
			this.textBox6.Enabled = false;
			this.textBox6.Location = new System.Drawing.Point(119, 19);
			this.textBox6.Name = "textBox6";
			this.textBox6.Size = new System.Drawing.Size(45, 20);
			this.textBox6.TabIndex = 16;
			// 
			// label12
			// 
			this.label12.Location = new System.Drawing.Point(236, 22);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(93, 17);
			this.label12.TabIndex = 20;
			this.label12.Text = "orders processed";
			// 
			// groupBox3
			// 
			this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox3.Controls.Add(this.label13);
			this.groupBox3.Controls.Add(this.checkBox3);
			this.groupBox3.Controls.Add(this.label14);
			this.groupBox3.Controls.Add(this.textBox9);
			this.groupBox3.Controls.Add(this.textBox10);
			this.groupBox3.Controls.Add(this.label15);
			this.groupBox3.Location = new System.Drawing.Point(3, 82);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(345, 73);
			this.groupBox3.TabIndex = 30;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Order Rejection Rate";
			// 
			// label13
			// 
			this.label13.Location = new System.Drawing.Point(6, 20);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(118, 17);
			this.label13.TabIndex = 34;
			this.label13.Text = "Happens once per";
			// 
			// checkBox3
			// 
			this.checkBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.checkBox3.Appearance = System.Windows.Forms.Appearance.Button;
			this.checkBox3.Checked = true;
			this.checkBox3.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBox3.Location = new System.Drawing.Point(6, 43);
			this.checkBox3.Name = "checkBox3";
			this.checkBox3.Size = new System.Drawing.Size(333, 24);
			this.checkBox3.TabIndex = 32;
			this.checkBox3.Text = "Enable Random Order Rejection";
			this.checkBox3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.checkBox3.UseVisualStyleBackColor = true;
			// 
			// label14
			// 
			this.label14.Location = new System.Drawing.Point(167, 20);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(16, 15);
			this.label14.TabIndex = 33;
			this.label14.Text = "to";
			// 
			// textBox9
			// 
			this.textBox9.Enabled = false;
			this.textBox9.Location = new System.Drawing.Point(185, 17);
			this.textBox9.Name = "textBox9";
			this.textBox9.Size = new System.Drawing.Size(45, 20);
			this.textBox9.TabIndex = 29;
			// 
			// textBox10
			// 
			this.textBox10.Enabled = false;
			this.textBox10.Location = new System.Drawing.Point(119, 17);
			this.textBox10.Name = "textBox10";
			this.textBox10.Size = new System.Drawing.Size(45, 20);
			this.textBox10.TabIndex = 30;
			// 
			// label15
			// 
			this.label15.Location = new System.Drawing.Point(236, 20);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(89, 17);
			this.label15.TabIndex = 31;
			this.label15.Text = "orders processed";
			// 
			// groupBox4
			// 
			this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox4.Controls.Add(this.label16);
			this.groupBox4.Controls.Add(this.label17);
			this.groupBox4.Controls.Add(this.textBox11);
			this.groupBox4.Controls.Add(this.textBox12);
			this.groupBox4.Controls.Add(this.label18);
			this.groupBox4.Controls.Add(this.label19);
			this.groupBox4.Controls.Add(this.checkBox4);
			this.groupBox4.Controls.Add(this.label20);
			this.groupBox4.Controls.Add(this.textBox13);
			this.groupBox4.Controls.Add(this.textBox14);
			this.groupBox4.Controls.Add(this.label21);
			this.groupBox4.Location = new System.Drawing.Point(3, 389);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(345, 108);
			this.groupBox4.TabIndex = 29;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Market Order Fill Price <=> Best Bid/Ask Deviation";
			// 
			// label16
			// 
			this.label16.Location = new System.Drawing.Point(6, 22);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(107, 17);
			this.label16.TabIndex = 28;
			this.label16.Text = "Happens once per";
			// 
			// label17
			// 
			this.label17.Location = new System.Drawing.Point(167, 48);
			this.label17.Name = "label17";
			this.label17.Size = new System.Drawing.Size(16, 15);
			this.label17.TabIndex = 27;
			this.label17.Text = "to";
			// 
			// textBox11
			// 
			this.textBox11.Enabled = false;
			this.textBox11.Location = new System.Drawing.Point(185, 45);
			this.textBox11.Name = "textBox11";
			this.textBox11.Size = new System.Drawing.Size(45, 20);
			this.textBox11.TabIndex = 24;
			// 
			// textBox12
			// 
			this.textBox12.Enabled = false;
			this.textBox12.Location = new System.Drawing.Point(119, 45);
			this.textBox12.Name = "textBox12";
			this.textBox12.Size = new System.Drawing.Size(45, 20);
			this.textBox12.TabIndex = 25;
			// 
			// label18
			// 
			this.label18.Location = new System.Drawing.Point(236, 48);
			this.label18.Name = "label18";
			this.label18.Size = new System.Drawing.Size(93, 17);
			this.label18.TabIndex = 26;
			this.label18.Text = "% best price";
			// 
			// label19
			// 
			this.label19.Location = new System.Drawing.Point(6, 48);
			this.label19.Name = "label19";
			this.label19.Size = new System.Drawing.Size(107, 17);
			this.label19.TabIndex = 23;
			this.label19.Text = "Price Deviation +/-";
			// 
			// checkBox4
			// 
			this.checkBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.checkBox4.Appearance = System.Windows.Forms.Appearance.Button;
			this.checkBox4.Checked = true;
			this.checkBox4.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBox4.Location = new System.Drawing.Point(6, 78);
			this.checkBox4.Name = "checkBox4";
			this.checkBox4.Size = new System.Drawing.Size(333, 24);
			this.checkBox4.TabIndex = 21;
			this.checkBox4.Text = "Enable Random Market Order Fill Price <=> Best Bid/Ask Deviation";
			this.checkBox4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.checkBox4.UseVisualStyleBackColor = true;
			// 
			// label20
			// 
			this.label20.Location = new System.Drawing.Point(167, 22);
			this.label20.Name = "label20";
			this.label20.Size = new System.Drawing.Size(16, 15);
			this.label20.TabIndex = 22;
			this.label20.Text = "to";
			// 
			// textBox13
			// 
			this.textBox13.Enabled = false;
			this.textBox13.Location = new System.Drawing.Point(185, 19);
			this.textBox13.Name = "textBox13";
			this.textBox13.Size = new System.Drawing.Size(45, 20);
			this.textBox13.TabIndex = 13;
			// 
			// textBox14
			// 
			this.textBox14.Enabled = false;
			this.textBox14.Location = new System.Drawing.Point(119, 19);
			this.textBox14.Name = "textBox14";
			this.textBox14.Size = new System.Drawing.Size(45, 20);
			this.textBox14.TabIndex = 16;
			// 
			// label21
			// 
			this.label21.Location = new System.Drawing.Point(236, 22);
			this.label21.Name = "label21";
			this.label21.Size = new System.Drawing.Size(93, 17);
			this.label21.TabIndex = 20;
			this.label21.Text = "market orders";
			// 
			// groupBox5
			// 
			this.groupBox5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox5.Controls.Add(this.label22);
			this.groupBox5.Controls.Add(this.label23);
			this.groupBox5.Controls.Add(this.textBox15);
			this.groupBox5.Controls.Add(this.textBox16);
			this.groupBox5.Controls.Add(this.label24);
			this.groupBox5.Controls.Add(this.label25);
			this.groupBox5.Controls.Add(this.checkBox5);
			this.groupBox5.Controls.Add(this.label26);
			this.groupBox5.Controls.Add(this.textBox17);
			this.groupBox5.Controls.Add(this.textBox18);
			this.groupBox5.Controls.Add(this.label27);
			this.groupBox5.Location = new System.Drawing.Point(3, 503);
			this.groupBox5.Name = "groupBox5";
			this.groupBox5.Size = new System.Drawing.Size(345, 108);
			this.groupBox5.TabIndex = 31;
			this.groupBox5.TabStop = false;
			this.groupBox5.Text = "Broker Provider Disconnect";
			// 
			// label22
			// 
			this.label22.Location = new System.Drawing.Point(6, 22);
			this.label22.Name = "label22";
			this.label22.Size = new System.Drawing.Size(107, 17);
			this.label22.TabIndex = 28;
			this.label22.Text = "Happens once per";
			// 
			// label23
			// 
			this.label23.Location = new System.Drawing.Point(167, 48);
			this.label23.Name = "label23";
			this.label23.Size = new System.Drawing.Size(16, 15);
			this.label23.TabIndex = 27;
			this.label23.Text = "to";
			// 
			// textBox15
			// 
			this.textBox15.Enabled = false;
			this.textBox15.Location = new System.Drawing.Point(185, 45);
			this.textBox15.Name = "textBox15";
			this.textBox15.Size = new System.Drawing.Size(45, 20);
			this.textBox15.TabIndex = 24;
			// 
			// textBox16
			// 
			this.textBox16.Enabled = false;
			this.textBox16.Location = new System.Drawing.Point(119, 45);
			this.textBox16.Name = "textBox16";
			this.textBox16.Size = new System.Drawing.Size(45, 20);
			this.textBox16.TabIndex = 25;
			// 
			// label24
			// 
			this.label24.Location = new System.Drawing.Point(236, 48);
			this.label24.Name = "label24";
			this.label24.Size = new System.Drawing.Size(93, 17);
			this.label24.TabIndex = 26;
			this.label24.Text = "milliSeconds";
			// 
			// label25
			// 
			this.label25.Location = new System.Drawing.Point(6, 48);
			this.label25.Name = "label25";
			this.label25.Size = new System.Drawing.Size(107, 17);
			this.label25.TabIndex = 23;
			this.label25.Text = "ReConnect after";
			// 
			// checkBox5
			// 
			this.checkBox5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.checkBox5.Appearance = System.Windows.Forms.Appearance.Button;
			this.checkBox5.Checked = true;
			this.checkBox5.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBox5.Location = new System.Drawing.Point(6, 78);
			this.checkBox5.Name = "checkBox5";
			this.checkBox5.Size = new System.Drawing.Size(333, 24);
			this.checkBox5.TabIndex = 21;
			this.checkBox5.Text = "Enable Random Broker Provider Disconnect";
			this.checkBox5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.checkBox5.UseVisualStyleBackColor = true;
			// 
			// label26
			// 
			this.label26.Location = new System.Drawing.Point(167, 22);
			this.label26.Name = "label26";
			this.label26.Size = new System.Drawing.Size(16, 15);
			this.label26.TabIndex = 22;
			this.label26.Text = "to";
			// 
			// textBox17
			// 
			this.textBox17.Enabled = false;
			this.textBox17.Location = new System.Drawing.Point(185, 19);
			this.textBox17.Name = "textBox17";
			this.textBox17.Size = new System.Drawing.Size(45, 20);
			this.textBox17.TabIndex = 13;
			// 
			// textBox18
			// 
			this.textBox18.Enabled = false;
			this.textBox18.Location = new System.Drawing.Point(119, 19);
			this.textBox18.Name = "textBox18";
			this.textBox18.Size = new System.Drawing.Size(45, 20);
			this.textBox18.TabIndex = 16;
			// 
			// label27
			// 
			this.label27.Location = new System.Drawing.Point(236, 22);
			this.label27.Name = "label27";
			this.label27.Size = new System.Drawing.Size(93, 17);
			this.label27.TabIndex = 20;
			this.label27.Text = "quotes generated";
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.textBox1);
			this.groupBox1.Controls.Add(this.textBox2);
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Controls.Add(this.label6);
			this.groupBox1.Controls.Add(this.checkBox2);
			this.groupBox1.Controls.Add(this.label7);
			this.groupBox1.Controls.Add(this.textBox7);
			this.groupBox1.Controls.Add(this.textBox8);
			this.groupBox1.Controls.Add(this.label8);
			this.groupBox1.Location = new System.Drawing.Point(3, 275);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(345, 108);
			this.groupBox1.TabIndex = 30;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Out-of-order Fill Delivery (Callback Coming After Next Fill)";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(6, 22);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(107, 17);
			this.label3.TabIndex = 28;
			this.label3.Text = "Happens once per";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(167, 48);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(16, 15);
			this.label4.TabIndex = 27;
			this.label4.Text = "to";
			// 
			// textBox1
			// 
			this.textBox1.Enabled = false;
			this.textBox1.Location = new System.Drawing.Point(185, 45);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(45, 20);
			this.textBox1.TabIndex = 24;
			// 
			// textBox2
			// 
			this.textBox2.Enabled = false;
			this.textBox2.Location = new System.Drawing.Point(119, 45);
			this.textBox2.Name = "textBox2";
			this.textBox2.Size = new System.Drawing.Size(45, 20);
			this.textBox2.TabIndex = 25;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(236, 48);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(93, 17);
			this.label5.TabIndex = 26;
			this.label5.Text = "orders later";
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(6, 48);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(107, 17);
			this.label6.TabIndex = 23;
			this.label6.Text = "Fill Msg Arrives";
			// 
			// checkBox2
			// 
			this.checkBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.checkBox2.Appearance = System.Windows.Forms.Appearance.Button;
			this.checkBox2.Checked = true;
			this.checkBox2.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBox2.Location = new System.Drawing.Point(6, 78);
			this.checkBox2.Name = "checkBox2";
			this.checkBox2.Size = new System.Drawing.Size(333, 24);
			this.checkBox2.TabIndex = 21;
			this.checkBox2.Text = "Enable Random Out-of-order Fill Delivery";
			this.checkBox2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.checkBox2.UseVisualStyleBackColor = true;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(167, 22);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(16, 15);
			this.label7.TabIndex = 22;
			this.label7.Text = "to";
			// 
			// textBox7
			// 
			this.textBox7.Enabled = false;
			this.textBox7.Location = new System.Drawing.Point(185, 19);
			this.textBox7.Name = "textBox7";
			this.textBox7.Size = new System.Drawing.Size(45, 20);
			this.textBox7.TabIndex = 13;
			// 
			// textBox8
			// 
			this.textBox8.Enabled = false;
			this.textBox8.Location = new System.Drawing.Point(119, 19);
			this.textBox8.Name = "textBox8";
			this.textBox8.Size = new System.Drawing.Size(45, 20);
			this.textBox8.TabIndex = 16;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(236, 22);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(93, 17);
			this.label8.TabIndex = 20;
			this.label8.Text = "orders processed";
			// 
			// groupBox6
			// 
			this.groupBox6.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox6.Controls.Add(this.label28);
			this.groupBox6.Controls.Add(this.checkBox6);
			this.groupBox6.Controls.Add(this.label29);
			this.groupBox6.Controls.Add(this.textBox19);
			this.groupBox6.Controls.Add(this.textBox20);
			this.groupBox6.Controls.Add(this.label30);
			this.groupBox6.Location = new System.Drawing.Point(3, 3);
			this.groupBox6.Name = "groupBox6";
			this.groupBox6.Size = new System.Drawing.Size(345, 73);
			this.groupBox6.TabIndex = 32;
			this.groupBox6.TabStop = false;
			this.groupBox6.Text = "Order Execution Delay";
			// 
			// label28
			// 
			this.label28.Location = new System.Drawing.Point(6, 20);
			this.label28.Name = "label28";
			this.label28.Size = new System.Drawing.Size(118, 17);
			this.label28.TabIndex = 34;
			this.label28.Text = "Delay between quotes";
			// 
			// checkBox6
			// 
			this.checkBox6.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.checkBox6.Appearance = System.Windows.Forms.Appearance.Button;
			this.checkBox6.Checked = true;
			this.checkBox6.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBox6.Location = new System.Drawing.Point(6, 43);
			this.checkBox6.Name = "checkBox6";
			this.checkBox6.Size = new System.Drawing.Size(333, 24);
			this.checkBox6.TabIndex = 32;
			this.checkBox6.Text = "Enable Random Order Generation Delay";
			this.checkBox6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.checkBox6.UseVisualStyleBackColor = true;
			// 
			// label29
			// 
			this.label29.Location = new System.Drawing.Point(178, 20);
			this.label29.Name = "label29";
			this.label29.Size = new System.Drawing.Size(16, 15);
			this.label29.TabIndex = 33;
			this.label29.Text = "to";
			// 
			// textBox19
			// 
			this.textBox19.Enabled = false;
			this.textBox19.Location = new System.Drawing.Point(196, 17);
			this.textBox19.Name = "textBox19";
			this.textBox19.Size = new System.Drawing.Size(45, 20);
			this.textBox19.TabIndex = 29;
			// 
			// textBox20
			// 
			this.textBox20.Enabled = false;
			this.textBox20.Location = new System.Drawing.Point(130, 17);
			this.textBox20.Name = "textBox20";
			this.textBox20.Size = new System.Drawing.Size(45, 20);
			this.textBox20.TabIndex = 30;
			// 
			// label30
			// 
			this.label30.Location = new System.Drawing.Point(247, 20);
			this.label30.Name = "label30";
			this.label30.Size = new System.Drawing.Size(68, 17);
			this.label30.TabIndex = 31;
			this.label30.Text = "milliSeconds";
			// 
			// LivesimBrokerEditor
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			this.AutoScroll = true;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.Controls.Add(this.groupBox6);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.groupBox5);
			this.Controls.Add(this.groupBox4);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.groupBox2);
			this.Name = "LivesimBrokerEditor";
			this.Size = new System.Drawing.Size(351, 624);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.groupBox4.ResumeLayout(false);
			this.groupBox4.PerformLayout();
			this.groupBox5.ResumeLayout(false);
			this.groupBox5.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox6.ResumeLayout(false);
			this.groupBox6.PerformLayout();
			this.ResumeLayout(false);

		}
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}