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

		//bool		quikFolderExists	{ get { return Directory.Exists(this.quikFolder); } }
		string		quikDllAbsPath		{ get { return Path.Combine(this.quikFolder, this.quikBrokerAdapter.QuikDllName); } }
		bool		dllFoundInFolder	{ get { return File.Exists(quikDllAbsPath); } }
	

		// Designer will call this
		public QuikBrokerEditorControl() {
			this.InitializeComponent();
		}
		// NEVER_FORGET_":this()" DataSourceEditorControl.PopulateStreamingBrokerListViewsFromDataSource() => brokerAdapterInstance.BrokerEditorInitialize() will call this
		public QuikBrokerEditorControl(BrokerAdapter brokerQuik, IDataSourceEditor dataSourceEditor) : this() {
			base.Initialize(brokerQuik, dataSourceEditor);
			this.propagateBrokerConnected_intoBtnStateText();
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
	}
}