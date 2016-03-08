using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Core.DataFeed;
using Sq1.Core.Accounting;
using Sq1.Core.Broker;

namespace Sq1.Adapters.Quik.Broker {
	public partial class QuikBrokerEditorControl {
		QuikBroker	quikBrokerAdapter { get { return base.BrokerAdapter as QuikBroker; } }
		bool		dontStartStopDllConnection_imSyncingDdeStarted_intoTheBtnText_only;

		string	quikFolder {
			get { return this.txtQuikFolder.Text; }
			set { this.txtQuikFolder.Text = value; }
		}
		int		reconnectTimeoutMillis {
			get {
				int ret = 0;
				try {
					ret = Convert.ToInt32(this.txtReconnectTimeoutMillis.Text);
					this.txtReconnectTimeoutMillis.BackColor = Color.White;
				} catch (Exception e) {
					this.txtReconnectTimeoutMillis.BackColor = Color.LightCoral;
					this.txtReconnectTimeoutMillis.Text = "1000";	// induce one more event?...
				}
				return ret;
			}
			set { this.txtReconnectTimeoutMillis.Text = value.ToString(); }
		}
		Account account {
			get {
				Account ret;
				try {
					ret = new Account(this.txtQuikAccount.Text,
						Convert.ToDouble(this.txtCashAvailable.Text));
				} catch (Exception e) {
					ret = new Account();
					ret.AccountNumber = this.txtQuikAccount.Text;
				}
				return ret;
			}
			set {
				if (value == null) return;
				this.txtQuikAccount.Text = value.AccountNumber;
				this.txtCashAvailable.Text = value.CashAvailable.ToString();
			}
		}
		Account accountMicex {
			get {
				Account ret;
				try {
					ret = new Account(this.txtQuikAccountMicex.Text, Convert.ToDouble(this.txtCashAvailableMicex.Text));
				} catch (Exception e) {
					ret = new Account();
					ret.AccountNumber = this.txtQuikAccountMicex.Text;
				}
				return ret;
			}
			set {
				if (value == null) return;
				this.txtQuikAccountMicex.Text = value.AccountNumber;
				this.txtCashAvailableMicex.Text = value.CashAvailable.ToString();
			}
		}
		bool goRealDontUseOwnLivesim {
			get { return this.cbxGoRealWhenLivesim.Checked; }
			set { this.cbxGoRealWhenLivesim.Checked = value; }
		}

		bool		quikFolderExists		{ get { return Directory.Exists(this.quikFolder); } }
		//string		trans2QuikDllAbsPath	{ get { return Path.Combine(this.quikFolder, this.quikBrokerAdapter.Trans2QuikDllName); } }
		string		trans2QuikDllAbsPath	{ get { return Path.Combine(Assembler.InstanceInitialized.AppStartupPath, this.quikBrokerAdapter.Trans2QuikDllName); } }
		bool		trans2QuikDllFound		{ get { return File.Exists(this.trans2QuikDllAbsPath); } }
	
		LinkLabel.Link	trans2QuikDllLink	{ get {
			LinkLabel.Link ret = new LinkLabel.Link();
			ret.LinkData = this.quikBrokerAdapter.Trans2QuikDllUrl;
			return ret;
		} }

		// Designer will call this
		public QuikBrokerEditorControl() {
			this.InitializeComponent();
		}
		// NEVER_FORGET_":this()" DataSourceEditorControl.PopulateStreamingBrokerListViewsFromDataSource() => brokerAdapterInstance.BrokerEditorInitialize() will call this
		public QuikBrokerEditorControl(BrokerAdapter brokerQuik, IDataSourceEditor dataSourceEditor) : this() {
			base.Initialize(brokerQuik, dataSourceEditor);
			this.propagateBrokerConnected_intoBtnStateText();
			this.propagateDllFound();
		}
		public override void PushBrokerAdapterSettingsToEditor() {
			this.account				= this.quikBrokerAdapter.AccountAutoPropagate;
			// quik-specific
			this.accountMicex			= this.quikBrokerAdapter.AccountMicexAutoPopulated;
			this.quikFolder				= this.quikBrokerAdapter.QuikFolder;
			this.reconnectTimeoutMillis	= Convert.ToInt32(this.quikBrokerAdapter.ReconnectTimeoutMillis);
			//QuikClientCode = SettingsEditor.QuikClientCode;
		}
		public override void PushEditedSettingsToBrokerAdapter() {
			if (base.IgnoreEditorFieldChangesWhileInitializingEditor) return;
			this.quikBrokerAdapter.AccountAutoPropagate		= this.account;
			// quik-specific
			this.quikBrokerAdapter.AccountMicexAutoPopulated	= this.accountMicex;
			this.quikBrokerAdapter.QuikFolder					= this.quikFolder;
			this.quikBrokerAdapter.ReconnectTimeoutMillis		= this.reconnectTimeoutMillis;
			//this.editor.QuikClientCode = QuikClientCode;
			this.quikBrokerAdapter.GoRealDontUseOwnLivesim		= this.goRealDontUseOwnLivesim;
		}
		void propagateBrokerConnected_intoBtnStateText() {
			if (base.InvokeRequired) {
				base.BeginInvoke(new MethodInvoker(this.propagateBrokerConnected_intoBtnStateText));
				return;
			}
			//if (this.cbxConnectDLL.Checked == this.quikBrokerAdapter.DdeServerStarted) return;
			if (this.cbxConnectDLL.Checked != this.quikBrokerAdapter.UpstreamConnected) {
				try {
					this.dontStartStopDllConnection_imSyncingDdeStarted_intoTheBtnText_only = true;
					//this.cbxConnectDLL.Checked  = this.quikBrokerAdapter.DdeServerStarted;
					this.cbxConnectDLL.Checked = this.quikBrokerAdapter.UpstreamConnected;
				} catch (Exception ex) {
					string msg = "HOPEFULLY_NEVER_HAPPENS__YOU_CAUGHT_IT_EARLIER //QuikBrokerEditor(" + quikBrokerAdapter + ")";
					Assembler.PopupException(msg, ex);
				} finally {
					this.dontStartStopDllConnection_imSyncingDdeStarted_intoTheBtnText_only = false;
				}
			}
	
			string btnTxtMustBe = this.quikBrokerAdapter.DllConnectionStatus_oppositeAction;
			if (this.cbxConnectDLL.Text == btnTxtMustBe) return;
				this.cbxConnectDLL.Text  = btnTxtMustBe;
		}

		void propagateDllFound() {
			this.lnkTrans2quik.Text = this.quikBrokerAdapter.Trans2QuikDllName;

			this.lnkTrans2quik.Links.Clear();
			this.lnkTrans2quik.Links.Add(this.trans2QuikDllLink);

			if (this.trans2QuikDllFound) {
				this.lblTrans2quikFound.Text = "found in AppStartupPath";
				this.lblTrans2quikFound.ForeColor = Color.Green;
			} else {
				this.lblTrans2quikFound.Text = "not found in AppStartupPath";
				this.lblTrans2quikFound.ForeColor = Color.Red;
			}
		}
	}
}