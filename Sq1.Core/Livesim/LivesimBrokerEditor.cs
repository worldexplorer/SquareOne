using System;

using Sq1.Core.DataFeed;

namespace Sq1.Core.Livesim {
	public partial class LivesimBrokerEditor {
//		public string QuikFolder {
//			get { return this.txtQuikFolder.Text; }
//			set { this.txtQuikFolder.Text = value; }
//		}
//		public int ReconnectTimeoutMillis {
//			get {
//				int ret = 0;
//				try {
//					ret = Convert.ToInt32(this.txtReconnectTimeoutMillis.Text);
//					this.txtReconnectTimeoutMillis.BackColor = Color.White;
//				} catch (Exception e) {
//					this.txtReconnectTimeoutMillis.BackColor = Color.LightCoral;
//					this.txtReconnectTimeoutMillis.Text = "1000";	// induce one more event?...
//				}
//				return ret;
//			}
//			set { this.txtReconnectTimeoutMillis.Text = value.ToString(); }
//		}
//		public Account Account {
//			get {
//				Account ret;
//				try {
//					ret = new Account(this.txtQuikAccount.Text,
//						Convert.ToDouble(this.txtCashAvailable.Text));
//				} catch (Exception e) {
//					ret = new Account();
//					ret.AccountNumber = this.txtQuikAccount.Text;
//				}
//				return ret;
//			}
//			set {
//				if (value == null) return;
//				this.txtQuikAccount.Text = value.AccountNumber;
//				this.txtCashAvailable.Text = value.CashAvailable.ToString();
//			}
//		}
//		public Account AccountMicex {
//			get {
//				Account ret;
//				try {
//					ret = new Account(this.txtQuikAccountMicex.Text, Convert.ToDouble(this.txtCashAvailableMicex.Text));
//				} catch (Exception e) {
//					ret = new Account();
//					ret.AccountNumber = this.txtQuikAccountMicex.Text;
//				}
//				return ret;
//			}
//			set {
//				if (value == null) return;
//				this.txtQuikAccountMicex.Text = value.AccountNumber;
//				this.txtCashAvailableMicex.Text = value.CashAvailable.ToString();
//			}
//		}
		LivesimBroker brokerLivesim {
			get { return base.brokerAdapter as LivesimBroker; }
		}

		public LivesimBrokerEditor() {
			this.InitializeComponent();
		}
		//public override void Initialize(BrokerAdapter BrokerQuik, IDataSourceEditor dataSourceEditor) {
		//    base.Initialize(BrokerQuik, dataSourceEditor);
		//    base.InitializeEditorFields();
		//}
		public override void PushBrokerAdapterSettingsToEditor() {
//			this.Account = this.brokerLivesim.AccountAutoPropagate;
			// quik-specific
//			this.AccountMicex = this.brokerLivesim.AccountMicexAutoPopulated;
//			this.QuikFolder = this.brokerLivesim.QuikFolder;
//			this.ReconnectTimeoutMillis = Convert.ToInt32(this.brokerLivesim.ReconnectTimeoutMillis);
			//QuikClientCode = SettingsEditor.QuikClientCode;
		}
		public override void PushEditedSettingsToBrokerAdapter() {
			if (base.ignoreEditorFieldChangesWhileInitializingEditor) return;
//			this.brokerLivesim.AccountAutoPropagate = this.Account;
			// quik-specific
//			this.brokerLivesim.AccountMicexAutoPopulated = this.AccountMicex;
//			this.brokerLivesim.QuikFolder = QuikFolder;
//			this.brokerLivesim.ReconnectTimeoutMillis = ReconnectTimeoutMillis;
			//this.editor.QuikClientCode = QuikClientCode;
		}
    }
}