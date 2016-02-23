using System;
using System.Drawing;

using Sq1.Core.DataFeed;
using Sq1.Core.Accounting;
using Sq1.Core.Broker;

namespace Sq1.Adapters.Quik.Broker {
	public partial class BrokerQuikEditor {
		public string QuikFolder {
			get { return this.txtQuikFolder.Text; }
			set { this.txtQuikFolder.Text = value; }
		}
		public int ReconnectTimeoutMillis {
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
		public Account Account {
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
		public Account AccountMicex {
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
		QuikBroker BrokerQuik { get { return base.BrokerAdapter as QuikBroker; } }

		// Designer will call this
		public BrokerQuikEditor() {
			this.InitializeComponent();
		}

		// NEVER_FORGET_":this()" DataSourceEditorControl.PopulateStreamingBrokerListViewsFromDataSource() => brokerAdapterInstance.BrokerEditorInitialize() will call this
		public BrokerQuikEditor(BrokerAdapter brokerQuik, IDataSourceEditor dataSourceEditor) : this() {
			base.Initialize(brokerQuik, dataSourceEditor);
		}
		public override void PushBrokerAdapterSettingsToEditor() {
			this.Account = this.BrokerQuik.AccountAutoPropagate;
			// quik-specific
			this.AccountMicex = this.BrokerQuik.AccountMicexAutoPopulated;
			this.QuikFolder = this.BrokerQuik.QuikFolder;
			this.ReconnectTimeoutMillis = Convert.ToInt32(this.BrokerQuik.ReconnectTimeoutMillis);
			//QuikClientCode = SettingsEditor.QuikClientCode;
		}
		public override void PushEditedSettingsToBrokerAdapter() {
			if (base.IgnoreEditorFieldChangesWhileInitializingEditor) return;
			this.BrokerQuik.AccountAutoPropagate = this.Account;
			// quik-specific
			this.BrokerQuik.AccountMicexAutoPopulated = this.AccountMicex;
			this.BrokerQuik.QuikFolder = QuikFolder;
			this.BrokerQuik.ReconnectTimeoutMillis = ReconnectTimeoutMillis;
			//this.editor.QuikClientCode = QuikClientCode;
		}
	}
}