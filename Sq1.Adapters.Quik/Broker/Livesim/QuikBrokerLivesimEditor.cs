using System;

using Sq1.Core.DataFeed;

namespace Sq1.Adapters.Quik.Broker.Livesim {
	public partial class QuikBrokerLivesimEditor {
		QuikBrokerLivesimSettings	livesimBrokerSettings;
		QuikBrokerLivesim			quikLivesimBroker;

		QuikBrokerLivesimEditor() {
			this.InitializeComponent();
		}

		public QuikBrokerLivesimEditor(QuikBrokerLivesim quikLivesimBroker, IDataSourceEditor dataSourceEditor) : this() {
			this.quikLivesimBroker = quikLivesimBroker;
			base.DataSourceEditor = dataSourceEditor;
		}

		public void Initialize(QuikBrokerLivesimSettings livesimBrokerSettings) {
			this.livesimBrokerSettings = livesimBrokerSettings;
		}

		public override void PushBrokerAdapterSettingsToEditor() {
		}
		public override void PushEditedSettingsToBrokerAdapter() {
		}
	}
}