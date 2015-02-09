using System;
using System.Windows.Forms;

namespace Sq1.Adapters.QuikMock {
	public partial class StreamingMockEditor {
		void txtQuoteDelay_KeyDown(object sender, KeyEventArgs e) {
			if (e.KeyData == Keys.Enter) {
				this.dataSourceEditor.ApplyEditorsToDataSourceAndClose();
			} 
		}
		void txtQuoteDelay_TextChanged(object sender, EventArgs e) {
			this.PushEditedSettingsToStreamingAdapter();
		}
		void txtGenerateOnlySymbols_KeyDown(object sender, KeyEventArgs e) {
			if (e.KeyData == Keys.Enter) {
				this.dataSourceEditor.ApplyEditorsToDataSourceAndClose();
			}
		}
		void txtGenerateOnlySymbols_TextChanged(object sender, EventArgs e) {
			this.PushEditedSettingsToStreamingAdapter();
		}

		void cbxGeneratingNow_CheckedChanged(object sender, EventArgs e) {
			this.PushEditedSettingsToStreamingAdapter();
		}
	}
}