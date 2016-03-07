using System;
using System.Drawing;

using Sq1.Core;

namespace Sq1.Adapters.Quik.Broker {
	public partial class QuikBrokerEditorControl {
		void txtQuikFolder_TextChanged(object sender, EventArgs e) {
			if (this.dllFoundInFolder) {
				this.txtQuikFolder.BackColor = Color.White;
				this.lblQuikPathContainsDllStatus.Text = "found";		// + this.quikBrokerAdapter.QuikDllName;
				this.lblQuikPathContainsDllStatus.ForeColor = Color.Green;
			} else {
				this.txtQuikFolder.BackColor = Color.LightCoral;
				this.lblQuikPathContainsDllStatus.Text = "folder not found";	// + this.quikBrokerAdapter.QuikDllName;
				this.lblQuikPathContainsDllStatus.ForeColor = Color.Red;
			}
		}

		void cbxConnectDLL_CheckedChanged(object sender, EventArgs e) {
			try {
				if (this.dontStartStopDllConnection_imSyncingDdeStarted_intoTheBtnText_only) {
					this.propagateBrokerConnected_intoBtnStateText();
					return;
				}
				if (this.cbxConnectDLL.Checked) {
					//this.quikBrokerAdapter.ConnectToDll_subscribe();
					this.quikBrokerAdapter.Connect();
				} else {
					//this.quikBrokerAdapter.DisconnectFromDll_unsubscribe();
					this.quikBrokerAdapter.Disconnect();
				}
			} catch (Exception ex) {
				string msg = "(DIS)CONNECT_THREW";
				Assembler.PopupException(msg, ex);
			}
		}
	}
}