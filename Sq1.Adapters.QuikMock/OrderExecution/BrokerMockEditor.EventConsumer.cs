using System;

namespace Sq1.Adapters.QuikMock {
	public partial class BrokerMockEditor {
		void rbtnCrossMarket_CheckedChanged(object sender, EventArgs e) {
		}
		void rbtnFellow_CheckedChanged(object sender, EventArgs e) {
		}
		void cbxRejectAllUpcoming_CheckedChanged(object sender, EventArgs e) {
			this.PushEditedSettingsToBrokerAdapter();
		}
		void txtExecutionDelayMillis_TextChanged(object sender, EventArgs e) {
			this.PushEditedSettingsToBrokerAdapter();
		}
	}
}