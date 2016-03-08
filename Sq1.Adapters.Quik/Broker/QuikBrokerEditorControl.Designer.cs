using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.ComponentModel.Design;

using Sq1.Core.Broker;

namespace Sq1.Adapters.Quik.Broker {
	[ToolboxBitmap(typeof(QuikBrokerEditorControl), "BrokerQuik")]
	[Designer("System.Windows.Forms.Design.ParentControlDesigner, System.Design", typeof(IDesigner))]
	public partial class QuikBrokerEditorControl : BrokerEditor {
        #region Component Designer generated code
		private System.ComponentModel.IContainer components = null;
		private Label lblQuikPath;
		private TextBox txtQuikFolder;
		private Label lblQuikAccount;
		private TextBox txtQuikAccount;
		private TextBox txtReconnectTimeoutMillis;
		private Label lblReconnectTimeoutMillis;
		private TextBox txtCashAvailable;
		private Label lblCashAvailable;
		private TextBox txtQuikAccountMicex;
		private TextBox txtCashAvailableMicex;
		private Label lblCashAvailableMicex;
		private Label lblQuikAccountMicex;
		void InitializeComponent() {
			this.lblQuikPath = new System.Windows.Forms.Label();
			this.lblQuikAccount = new System.Windows.Forms.Label();
			this.txtQuikFolder = new System.Windows.Forms.TextBox();
			this.txtQuikAccount = new System.Windows.Forms.TextBox();
			this.txtReconnectTimeoutMillis = new System.Windows.Forms.TextBox();
			this.lblReconnectTimeoutMillis = new System.Windows.Forms.Label();
			this.txtCashAvailable = new System.Windows.Forms.TextBox();
			this.lblCashAvailable = new System.Windows.Forms.Label();
			this.txtQuikAccountMicex = new System.Windows.Forms.TextBox();
			this.lblQuikAccountMicex = new System.Windows.Forms.Label();
			this.txtCashAvailableMicex = new System.Windows.Forms.TextBox();
			this.lblCashAvailableMicex = new System.Windows.Forms.Label();
			this.cbxGoRealWhenLivesim = new System.Windows.Forms.CheckBox();
			this.lblQuikPathFound = new System.Windows.Forms.Label();
			this.cbxConnectDLL = new System.Windows.Forms.CheckBox();
			this.lblTrans2quikFound = new System.Windows.Forms.Label();
			this.lnkTrans2quik = new System.Windows.Forms.LinkLabel();
			this.SuspendLayout();
			// 
			// lblQuikPath
			// 
			this.lblQuikPath.AutoSize = true;
			this.lblQuikPath.Location = new System.Drawing.Point(3, 6);
			this.lblQuikPath.Name = "lblQuikPath";
			this.lblQuikPath.Size = new System.Drawing.Size(54, 13);
			this.lblQuikPath.TabIndex = 0;
			this.lblQuikPath.Text = "Quik Path";
			// 
			// lblQuikAccount
			// 
			this.lblQuikAccount.AutoSize = true;
			this.lblQuikAccount.Location = new System.Drawing.Point(3, 162);
			this.lblQuikAccount.Name = "lblQuikAccount";
			this.lblQuikAccount.Size = new System.Drawing.Size(92, 13);
			this.lblQuikAccount.TabIndex = 1;
			this.lblQuikAccount.Text = "Account SPBFUT";
			// 
			// txtQuikFolder
			// 
			this.txtQuikFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtQuikFolder.Location = new System.Drawing.Point(3, 22);
			this.txtQuikFolder.Name = "txtQuikFolder";
			this.txtQuikFolder.Size = new System.Drawing.Size(247, 20);
			this.txtQuikFolder.TabIndex = 6;
			this.txtQuikFolder.TextChanged += new System.EventHandler(this.txtQuikFolder_TextChanged);
			// 
			// txtQuikAccount
			// 
			this.txtQuikAccount.Enabled = false;
			this.txtQuikAccount.Location = new System.Drawing.Point(3, 178);
			this.txtQuikAccount.Name = "txtQuikAccount";
			this.txtQuikAccount.Size = new System.Drawing.Size(108, 20);
			this.txtQuikAccount.TabIndex = 7;
			// 
			// txtReconnectTimeoutMillis
			// 
			this.txtReconnectTimeoutMillis.Location = new System.Drawing.Point(3, 65);
			this.txtReconnectTimeoutMillis.Name = "txtReconnectTimeoutMillis";
			this.txtReconnectTimeoutMillis.Size = new System.Drawing.Size(108, 20);
			this.txtReconnectTimeoutMillis.TabIndex = 19;
			// 
			// lblReconnectTimeoutMillis
			// 
			this.lblReconnectTimeoutMillis.AutoSize = true;
			this.lblReconnectTimeoutMillis.Location = new System.Drawing.Point(127, 68);
			this.lblReconnectTimeoutMillis.Name = "lblReconnectTimeoutMillis";
			this.lblReconnectTimeoutMillis.Size = new System.Drawing.Size(105, 13);
			this.lblReconnectTimeoutMillis.TabIndex = 18;
			this.lblReconnectTimeoutMillis.Text = "DLL ReconnectMillis";
			// 
			// txtCashAvailable
			// 
			this.txtCashAvailable.Enabled = false;
			this.txtCashAvailable.Location = new System.Drawing.Point(3, 217);
			this.txtCashAvailable.Name = "txtCashAvailable";
			this.txtCashAvailable.Size = new System.Drawing.Size(108, 20);
			this.txtCashAvailable.TabIndex = 21;
			// 
			// lblCashAvailable
			// 
			this.lblCashAvailable.AutoSize = true;
			this.lblCashAvailable.Location = new System.Drawing.Point(3, 201);
			this.lblCashAvailable.Name = "lblCashAvailable";
			this.lblCashAvailable.Size = new System.Drawing.Size(116, 13);
			this.lblCashAvailable.TabIndex = 20;
			this.lblCashAvailable.Text = "Cash Available FORTS";
			// 
			// txtQuikAccountMicex
			// 
			this.txtQuikAccountMicex.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtQuikAccountMicex.Enabled = false;
			this.txtQuikAccountMicex.Location = new System.Drawing.Point(130, 178);
			this.txtQuikAccountMicex.Name = "txtQuikAccountMicex";
			this.txtQuikAccountMicex.Size = new System.Drawing.Size(120, 20);
			this.txtQuikAccountMicex.TabIndex = 23;
			// 
			// lblQuikAccountMicex
			// 
			this.lblQuikAccountMicex.AutoSize = true;
			this.lblQuikAccountMicex.Location = new System.Drawing.Point(131, 162);
			this.lblQuikAccountMicex.Name = "lblQuikAccountMicex";
			this.lblQuikAccountMicex.Size = new System.Drawing.Size(102, 13);
			this.lblQuikAccountMicex.TabIndex = 22;
			this.lblQuikAccountMicex.Text = "Account * (EQBR...)";
			// 
			// txtCashAvailableMicex
			// 
			this.txtCashAvailableMicex.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtCashAvailableMicex.Enabled = false;
			this.txtCashAvailableMicex.Location = new System.Drawing.Point(130, 217);
			this.txtCashAvailableMicex.Name = "txtCashAvailableMicex";
			this.txtCashAvailableMicex.Size = new System.Drawing.Size(120, 20);
			this.txtCashAvailableMicex.TabIndex = 25;
			// 
			// lblCashAvailableMicex
			// 
			this.lblCashAvailableMicex.AutoSize = true;
			this.lblCashAvailableMicex.Location = new System.Drawing.Point(131, 201);
			this.lblCashAvailableMicex.Name = "lblCashAvailableMicex";
			this.lblCashAvailableMicex.Size = new System.Drawing.Size(113, 13);
			this.lblCashAvailableMicex.TabIndex = 24;
			this.lblCashAvailableMicex.Text = "Cash Available MICEX";
			// 
			// cbxGoRealWhenLivesim
			// 
			this.cbxGoRealWhenLivesim.AutoSize = true;
			this.cbxGoRealWhenLivesim.Location = new System.Drawing.Point(6, 119);
			this.cbxGoRealWhenLivesim.Name = "cbxGoRealWhenLivesim";
			this.cbxGoRealWhenLivesim.Size = new System.Drawing.Size(207, 17);
			this.cbxGoRealWhenLivesim.TabIndex = 26;
			this.cbxGoRealWhenLivesim.Text = "Go Real, route Livesim Orders via DLL";
			this.cbxGoRealWhenLivesim.UseVisualStyleBackColor = true;
			// 
			// lblQuikPathFound
			// 
			this.lblQuikPathFound.AutoSize = true;
			this.lblQuikPathFound.ForeColor = System.Drawing.Color.Red;
			this.lblQuikPathFound.Location = new System.Drawing.Point(63, 6);
			this.lblQuikPathFound.Name = "lblQuikPathFound";
			this.lblQuikPathFound.Size = new System.Drawing.Size(65, 13);
			this.lblQuikPathFound.TabIndex = 27;
			this.lblQuikPathFound.Text = "doesn\'t exist";
			// 
			// cbxConnectDLL
			// 
			this.cbxConnectDLL.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.cbxConnectDLL.Appearance = System.Windows.Forms.Appearance.Button;
			this.cbxConnectDLL.AutoCheck = false;
			this.cbxConnectDLL.Location = new System.Drawing.Point(3, 89);
			this.cbxConnectDLL.Name = "cbxConnectDLL";
			this.cbxConnectDLL.Size = new System.Drawing.Size(247, 24);
			this.cbxConnectDLL.TabIndex = 28;
			this.cbxConnectDLL.Text = "Connect to QUIK";
			this.cbxConnectDLL.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.cbxConnectDLL.UseVisualStyleBackColor = true;
			this.cbxConnectDLL.CheckedChanged += new System.EventHandler(this.cbxConnectDLL_CheckedChanged);
			// 
			// lblTrans2quikFound
			// 
			this.lblTrans2quikFound.AutoSize = true;
			this.lblTrans2quikFound.ForeColor = System.Drawing.Color.Red;
			this.lblTrans2quikFound.Location = new System.Drawing.Point(127, 46);
			this.lblTrans2quikFound.Name = "lblTrans2quikFound";
			this.lblTrans2quikFound.Size = new System.Drawing.Size(106, 13);
			this.lblTrans2quikFound.TabIndex = 29;
			this.lblTrans2quikFound.Text = "Not found /appfolder";
			// 
			// lnkTrans2quik
			// 
			this.lnkTrans2quik.AutoSize = true;
			this.lnkTrans2quik.Location = new System.Drawing.Point(22, 46);
			this.lnkTrans2quik.Name = "lnkTrans2quik";
			this.lnkTrans2quik.Size = new System.Drawing.Size(89, 13);
			this.lnkTrans2quik.TabIndex = 30;
			this.lnkTrans2quik.TabStop = true;
			this.lnkTrans2quik.Text = "TRANS2QUIK.dll";
			this.lnkTrans2quik.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lnkTrans2quik.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkTrans2quik_LinkClicked);
			// 
			// QuikBrokerEditorControl
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			this.AutoScroll = true;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.Controls.Add(this.lnkTrans2quik);
			this.Controls.Add(this.lblTrans2quikFound);
			this.Controls.Add(this.cbxConnectDLL);
			this.Controls.Add(this.lblQuikPathFound);
			this.Controls.Add(this.cbxGoRealWhenLivesim);
			this.Controls.Add(this.txtCashAvailableMicex);
			this.Controls.Add(this.lblCashAvailableMicex);
			this.Controls.Add(this.txtQuikAccountMicex);
			this.Controls.Add(this.lblQuikAccountMicex);
			this.Controls.Add(this.txtCashAvailable);
			this.Controls.Add(this.lblCashAvailable);
			this.Controls.Add(this.txtReconnectTimeoutMillis);
			this.Controls.Add(this.lblReconnectTimeoutMillis);
			this.Controls.Add(this.txtQuikAccount);
			this.Controls.Add(this.txtQuikFolder);
			this.Controls.Add(this.lblQuikAccount);
			this.Controls.Add(this.lblQuikPath);
			this.Name = "QuikBrokerEditorControl";
			this.Size = new System.Drawing.Size(253, 247);
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

		private CheckBox cbxGoRealWhenLivesim;
		private Label lblQuikPathFound;
		private CheckBox cbxConnectDLL;
		private Label lblTrans2quikFound;
		private LinkLabel lnkTrans2quik;
    }
}