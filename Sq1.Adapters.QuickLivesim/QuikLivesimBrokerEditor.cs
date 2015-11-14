using System;

namespace Sq1.Adapters.QuikLivesim {
	public partial class QuikLivesimBrokerEditor {
		QuikLivesimBrokerSettings	livesimBrokerSettings;
		QuikLivesimBroker			quikLivesimBroker;

		QuikLivesimBrokerEditor() {
			this.InitializeComponent();
		}

		public QuikLivesimBrokerEditor(QuikLivesimBroker quikLivesimBroker, Core.DataFeed.IDataSourceEditor dataSourceEditor) : this() {
			this.quikLivesimBroker = quikLivesimBroker;
			this.dataSourceEditor = dataSourceEditor;
		}

		public void Initialize(QuikLivesimBrokerSettings livesimBrokerSettings) {
			this.livesimBrokerSettings = livesimBrokerSettings;
		}

		public override void PushBrokerAdapterSettingsToEditor() {
		}
		public override void PushEditedSettingsToBrokerAdapter() {
		}
	}
}