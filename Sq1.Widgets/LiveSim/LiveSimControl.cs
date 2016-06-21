using System;
using System.Windows.Forms;

namespace Sq1.Widgets.Livesim {
	public partial class LivesimControl : UserControl {
		public LivesimControl() {
			InitializeComponent();
		}

		void cbxClearExcepExec_CheckBoxCheckedChanged(object sender, EventArgs e) {
			this.BrokerLivesimEditor.PushEditedSettings_toBrokerAdapter_serializeDataSource();
		}
	}
}
