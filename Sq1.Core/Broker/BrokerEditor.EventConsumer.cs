using System;

namespace Sq1.Core.Broker {
	public partial class BrokerEditor {

		protected virtual void BrokerAdapter_OnBrokerConnectionStateChanged(object sender, EventArgs e) {
			string msg = "OVERRIDE_ME_IN_DERIVED UPDATE_YOUR_CONNECT_BUTTON_STATE_BECAUSE_IT_WAS_CHANGED_SOMEWHERE_ELSE";
			Assembler.PopupException(msg);
		}

	}
}