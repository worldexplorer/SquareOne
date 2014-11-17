using System;
using System.Windows.Forms;

using Sq1.Core;

namespace Sq1.Adapters.QuikMock {
	public partial class StreamingMockEditor {
		void txtQuoteDelay_KeyDown(object sender, KeyEventArgs e) {
			if (e.KeyData == Keys.Enter) {
				this.dataSourceEditor.ApplyEditorsToDataSourceAndClose();
			} 
		}
		void txtQuoteDelay_TextChanged(object sender, EventArgs e) {
			this.PushEditedSettingsToStreamingProvider();
		}
		void txtGenerateOnlySymbols_KeyDown(object sender, KeyEventArgs e) {
			if (e.KeyData == Keys.Enter) {
				this.dataSourceEditor.ApplyEditorsToDataSourceAndClose();
			}
		}
		void txtGenerateOnlySymbols_TextChanged(object sender, EventArgs e) {
			this.PushEditedSettingsToStreamingProvider();
		}

		void cbxGeneratingNow_CheckedChanged(object sender, EventArgs e) {
			if (this.cbxGeneratingNow.Checked) {
				mockStreamingProvider.AllSymbolsGenerateStart();
			} else {
				mockStreamingProvider.AllSymbolsGenerateStop();
			}
			this.PushEditedSettingsToStreamingProvider();
		}
	}
}