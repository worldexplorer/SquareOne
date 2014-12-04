using System;
using System.Drawing;

using Sq1.Core.Accounting;
using Sq1.Core.DataFeed;

namespace Sq1.Adapters.QuikMock {
	public partial class BrokerMockEditor {
		public Account Account { get {
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
			} }
		public int RejectFirstNOrders { get {
				int ret = 0;
				try {
					ret = Convert.ToInt32(this.txtRejectFirstNOrders.Text);
					this.txtRejectFirstNOrders.BackColor = Color.White;
				} catch (Exception e) {
					this.txtRejectFirstNOrders.BackColor = Color.LightCoral;
				}
				return ret;
			}
			set { this.txtRejectFirstNOrders.Text = value.ToString(); }
		}
		public bool RejectRandomly {
			get { return this.cbxRejectRandomly.Checked; }
			set { this.cbxRejectRandomly.Checked = value; }
		}
		public bool RejectAllUpcoming {
			get { return this.cbxRejectAllUpcoming.Checked; }
			set { this.cbxRejectAllUpcoming.Checked = value; }
		}
		public int FillFellow1CrossMarket2 {
			get {
				int ret = 0;
				if (this.rbtnFellow.Checked) ret = 1;
				if (this.rbtnCrossMarket.Checked) ret = 2;
				return ret;
			}
			set {
				if (value == 1) this.rbtnFellow.Checked = true;
				if (value == 2) this.rbtnCrossMarket.Checked = true;
			}
		}
		public int ExecutionDelayMillis { get {
				int ret = 0;
				try {
					ret = Convert.ToInt32(this.txtExecutionDelayMillis.Text);
					this.txtExecutionDelayMillis.BackColor = Color.White;
				} catch (Exception e) {
					this.txtExecutionDelayMillis.BackColor = Color.LightCoral;
					this.txtExecutionDelayMillis.Text = "1000";	// induce one more event?...
				}
				return ret;
			}
			set { this.txtExecutionDelayMillis.Text = value.ToString(); }
		}
		BrokerMock mockBrokerProvider { get { return base.brokerProvider as BrokerMock; } }

		// Designer will call this
		public BrokerMockEditor() {
			this.InitializeComponent();
		}

		// NEVER_FORGET_":this()" DataSourceEditorControl.PopulateStreamingBrokerListViewsFromDataSource() => brokerProviderInstance.BrokerEditorInitialize() will call this
		public BrokerMockEditor(BrokerMock mockBrokerProvider, IDataSourceEditor dataSourceEditor) : this() {
			base.Initialize(mockBrokerProvider, dataSourceEditor);
		}

		public override void PushBrokerProviderSettingsToEditor() {
			this.Account = this.mockBrokerProvider.AccountAutoPropagate;
			// mock-specific
			this.ExecutionDelayMillis = this.mockBrokerProvider.ExecutionDelayMillis;
			this.RejectFirstNOrders = this.mockBrokerProvider.RejectFirstNOrders;
			this.RejectRandomly = this.mockBrokerProvider.RejectRandomly;
			this.RejectAllUpcoming = this.mockBrokerProvider.RejectAllUpcoming;
			//this.QuikClientCode = this.editor.QuikClientCode;
			//if (this.DataSource == null) return;	//changing market for ASCII DataProvider
			//this.DataSource.DataSourceManager.DataSourceSaveTreeviewRefill(this.DataSource);
		}
		public override void PushEditedSettingsToBrokerProvider() {
			if (base.ignoreEditorFieldChangesWhileInitializingEditor) return;
			this.mockBrokerProvider.AccountAutoPropagate = this.Account;
			// mock-specific
			this.mockBrokerProvider.ExecutionDelayMillis = this.ExecutionDelayMillis;
			this.mockBrokerProvider.RejectFirstNOrders = this.RejectFirstNOrders;
			this.mockBrokerProvider.RejectRandomly = this.RejectRandomly;
			this.mockBrokerProvider.RejectAllUpcoming = this.RejectAllUpcoming;
			//this.editor.QuikClientCode = this.QuikClientCode;
		}
	}
}