using System;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;	//Process.Start(target);

using Sq1.Core;

namespace Sq1.Adapters.Quik.Broker {
	public partial class QuikBrokerEditorControl {
		void txtQuikFolder_TextChanged(object sender, EventArgs e) {
			if (this.quikFolderExists) {
				this.txtQuikFolder.BackColor = Color.White;
				this.lblQuikPathFound.Text = "folder found";
				this.lblQuikPathFound.ForeColor = Color.Green;
			} else {
				this.txtQuikFolder.BackColor = Color.LightCoral;
				this.lblQuikPathFound.Text = "folder does not exist";
				this.lblQuikPathFound.ForeColor = Color.Red;
			}
			this.propagateDllFound();	// each keypress you have a chance to see if I found the DLL; appRestart may be neededt to use it
		}

		void cbxConnectDLL_Click(object sender, EventArgs e) {
			try {
			    if (this.cbxConnectDLL.Checked) {
			        this.quikBrokerAdapter.Broker_disconnect();
			    } else {
			        this.quikBrokerAdapter.Broker_connect();
			    }
				//this.quikBrokerAdapter.UpstreamConnectedOnAppRestart = this.quikBrokerAdapter.UpstreamConnected;

				this.propagateBrokerConnected_intoBtnStateText();
			} catch (Exception ex) {
			    string msg = "(DIS)CONNECT_THREW";
			    Assembler.PopupException(msg, ex);
			}
		}

		void cbxConnectDLL_CheckedChanged(object sender, EventArgs e) {
			return;		// 
			//try {
			//    if (this.dontStartStopDllConnection_imSyncingDdeStarted_intoTheBtnText_only) {
			//        this.propagateBrokerConnected_intoBtnStateText();
			//        return;
			//    }
			//    if (this.cbxConnectDLL.Checked) {
			//        //this.quikBrokerAdapter.ConnectToDll_subscribe();
			//        this.quikBrokerAdapter.Broker_connect();
			//    } else {
			//        //this.quikBrokerAdapter.DisconnectFromDll_unsubscribe();
			//        this.quikBrokerAdapter.Broker_disconnect();
			//    }
			//} catch (Exception ex) {
			//    string msg = "(DIS)CONNECT_THREW";
			//    Assembler.PopupException(msg, ex);
			//}
		}

		void lnkTrans2quik_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
			this.lnkTrans2quik.LinkVisited = true;
			string target = e.Link.LinkData as string;
			Process.Start(target);
		}
		protected override void BrokerAdapter_OnBrokerConnectionStateChanged(object sender, EventArgs e) {
			this.propagateBrokerConnected_intoBtnStateText();
		}
	}
}