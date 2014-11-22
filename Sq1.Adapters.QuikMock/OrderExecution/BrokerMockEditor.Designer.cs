using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;

using Sq1.Core.Broker;

namespace Sq1.Adapters.QuikMock {
	[ToolboxBitmap(typeof(BrokerMockEditor), "BrokerMock")]
	[Designer("System.Windows.Forms.Design.ParentControlDesigner, System.Design", typeof(IDesigner))]
	public partial class BrokerMockEditor : BrokerEditor {
		#region Component Designer generated code
		private System.ComponentModel.IContainer components = null;
		private TextBox txtCashAvailable;
		private Label lblFirstOrdersFail;
		private TextBox txtQuikAccount;
		private Label lblQuikAccount;
		private TextBox txtExecutionDelayMillis;
		private RadioButton rbtnCrossMarket;
		private RadioButton rbtnFellow;
		private TextBox txtRejectFirstNOrders;
		private Label lblRejectFirstNOrders;
		private CheckBox cbxRejectRandomly;
		private CheckBox cbxRejectAllUpcoming;
		private Label lblExecutionDelay;

		void InitializeComponent() {
			this.txtCashAvailable = new System.Windows.Forms.TextBox();
			this.lblFirstOrdersFail = new System.Windows.Forms.Label();
			this.txtQuikAccount = new System.Windows.Forms.TextBox();
			this.lblQuikAccount = new System.Windows.Forms.Label();
			this.txtExecutionDelayMillis = new System.Windows.Forms.TextBox();
			this.lblExecutionDelay = new System.Windows.Forms.Label();
			this.rbtnCrossMarket = new System.Windows.Forms.RadioButton();
			this.rbtnFellow = new System.Windows.Forms.RadioButton();
			this.txtRejectFirstNOrders = new System.Windows.Forms.TextBox();
			this.lblRejectFirstNOrders = new System.Windows.Forms.Label();
			this.cbxRejectRandomly = new System.Windows.Forms.CheckBox();
			this.cbxRejectAllUpcoming = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// txtCashAvailable
			// 
			this.txtCashAvailable.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.txtCashAvailable.Location = new System.Drawing.Point(124, 58);
			this.txtCashAvailable.Name = "txtCashAvailable";
			this.txtCashAvailable.Size = new System.Drawing.Size(153, 20);
			this.txtCashAvailable.TabIndex = 25;
			// 
			// lblFirstOrdersFail
			// 
			this.lblFirstOrdersFail.AutoSize = true;
			this.lblFirstOrdersFail.Location = new System.Drawing.Point(124, 42);
			this.lblFirstOrdersFail.Name = "lblFirstOrdersFail";
			this.lblFirstOrdersFail.Size = new System.Drawing.Size(107, 13);
			this.lblFirstOrdersFail.TabIndex = 24;
			this.lblFirstOrdersFail.Text = "Mock Cash Available";
			// 
			// txtQuikAccount
			// 
			this.txtQuikAccount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.txtQuikAccount.Location = new System.Drawing.Point(124, 16);
			this.txtQuikAccount.Name = "txtQuikAccount";
			this.txtQuikAccount.Size = new System.Drawing.Size(153, 20);
			this.txtQuikAccount.TabIndex = 23;
			// 
			// lblQuikAccount
			// 
			this.lblQuikAccount.AutoSize = true;
			this.lblQuikAccount.Location = new System.Drawing.Point(124, 0);
			this.lblQuikAccount.Name = "lblQuikAccount";
			this.lblQuikAccount.Size = new System.Drawing.Size(114, 13);
			this.lblQuikAccount.TabIndex = 22;
			this.lblQuikAccount.Text = "Mock AccountNumber";
			// 
			// txtExecutionDelayMillis
			// 
			this.txtExecutionDelayMillis.Location = new System.Drawing.Point(3, 16);
			this.txtExecutionDelayMillis.Name = "txtExecutionDelayMillis";
			this.txtExecutionDelayMillis.Size = new System.Drawing.Size(115, 20);
			this.txtExecutionDelayMillis.TabIndex = 27;
			this.txtExecutionDelayMillis.TextChanged += new System.EventHandler(this.txtExecutionDelayMillis_TextChanged);
			// 
			// lblExecutionDelay
			// 
			this.lblExecutionDelay.AutoSize = true;
			this.lblExecutionDelay.Location = new System.Drawing.Point(3, 0);
			this.lblExecutionDelay.Name = "lblExecutionDelay";
			this.lblExecutionDelay.Size = new System.Drawing.Size(109, 13);
			this.lblExecutionDelay.TabIndex = 26;
			this.lblExecutionDelay.Text = "Execution Delay Millis";
			// 
			// rbtnCrossMarket
			// 
			this.rbtnCrossMarket.AutoSize = true;
			this.rbtnCrossMarket.Checked = true;
			this.rbtnCrossMarket.Enabled = false;
			this.rbtnCrossMarket.Location = new System.Drawing.Point(124, 102);
			this.rbtnCrossMarket.Name = "rbtnCrossMarket";
			this.rbtnCrossMarket.Size = new System.Drawing.Size(153, 17);
			this.rbtnCrossMarket.TabIndex = 17;
			this.rbtnCrossMarket.TabStop = true;
			this.rbtnCrossMarket.Text = "Fill with CrossMarket Quote";
			this.rbtnCrossMarket.UseVisualStyleBackColor = true;
			this.rbtnCrossMarket.Visible = false;
			// 
			// rbtnFellow
			// 
			this.rbtnFellow.AutoSize = true;
			this.rbtnFellow.Enabled = false;
			this.rbtnFellow.Location = new System.Drawing.Point(124, 84);
			this.rbtnFellow.Name = "rbtnFellow";
			this.rbtnFellow.Size = new System.Drawing.Size(124, 17);
			this.rbtnFellow.TabIndex = 16;
			this.rbtnFellow.Text = "Fill with Fellow Quote";
			this.rbtnFellow.UseVisualStyleBackColor = true;
			this.rbtnFellow.Visible = false;
			// 
			// txtRejectFirstNOrders
			// 
			this.txtRejectFirstNOrders.Location = new System.Drawing.Point(3, 58);
			this.txtRejectFirstNOrders.Name = "txtRejectFirstNOrders";
			this.txtRejectFirstNOrders.Size = new System.Drawing.Size(115, 20);
			this.txtRejectFirstNOrders.TabIndex = 5;
			// 
			// lblRejectFirstNOrders
			// 
			this.lblRejectFirstNOrders.AutoSize = true;
			this.lblRejectFirstNOrders.Location = new System.Drawing.Point(3, 42);
			this.lblRejectFirstNOrders.Name = "lblRejectFirstNOrders";
			this.lblRejectFirstNOrders.Size = new System.Drawing.Size(105, 13);
			this.lblRejectFirstNOrders.TabIndex = 4;
			this.lblRejectFirstNOrders.Text = "Reject First N Orders";
			// 
			// cbxRejectRandomly
			// 
			this.cbxRejectRandomly.AutoSize = true;
			this.cbxRejectRandomly.Location = new System.Drawing.Point(3, 84);
			this.cbxRejectRandomly.Name = "cbxRejectRandomly";
			this.cbxRejectRandomly.Size = new System.Drawing.Size(107, 17);
			this.cbxRejectRandomly.TabIndex = 28;
			this.cbxRejectRandomly.Text = "Reject Randomly";
			this.cbxRejectRandomly.UseVisualStyleBackColor = true;
			// 
			// cbxRejectAllUpcoming
			// 
			this.cbxRejectAllUpcoming.Appearance = System.Windows.Forms.Appearance.Button;
			this.cbxRejectAllUpcoming.AutoSize = true;
			this.cbxRejectAllUpcoming.Location = new System.Drawing.Point(3, 107);
			this.cbxRejectAllUpcoming.Name = "cbxRejectAllUpcoming";
			this.cbxRejectAllUpcoming.Size = new System.Drawing.Size(113, 23);
			this.cbxRejectAllUpcoming.TabIndex = 29;
			this.cbxRejectAllUpcoming.Text = "Reject All Upcoming";
			this.cbxRejectAllUpcoming.UseVisualStyleBackColor = true;
			this.cbxRejectAllUpcoming.CheckedChanged += new System.EventHandler(this.cbxRejectAllUpcoming_CheckedChanged);
			// 
			// BrokerMockEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoScroll = true;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.Controls.Add(this.cbxRejectAllUpcoming);
			this.Controls.Add(this.cbxRejectRandomly);
			this.Controls.Add(this.txtExecutionDelayMillis);
			this.Controls.Add(this.lblExecutionDelay);
			this.Controls.Add(this.txtCashAvailable);
			this.Controls.Add(this.lblFirstOrdersFail);
			this.Controls.Add(this.txtQuikAccount);
			this.Controls.Add(this.lblQuikAccount);
			this.Controls.Add(this.rbtnCrossMarket);
			this.Controls.Add(this.rbtnFellow);
			this.Controls.Add(this.txtRejectFirstNOrders);
			this.Controls.Add(this.lblRejectFirstNOrders);
			this.Location = new System.Drawing.Point(5, 5);
			this.Name = "BrokerMockEditor";
			this.Size = new System.Drawing.Size(280, 138);
			this.ResumeLayout(false);
			this.PerformLayout();

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