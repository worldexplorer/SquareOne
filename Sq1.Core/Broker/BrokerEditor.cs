using System;
using System.Windows.Forms;

using Sq1.Core.DataFeed;

namespace Sq1.Core.Broker {
	// this class should be abstract; it's not abstract because VS2012 opens MockBrokerEditor and complains
	// "I need to instantiate base class but base class is abstract"
	public class BrokerEditor : UserControl {
		protected BrokerAdapter BrokerAdapter;
		protected IDataSourceEditor DataSourceEditor;
		protected bool IgnoreEditorFieldChangesWhileInitializingEditor;

		public BrokerEditor() { /*used in Design Mode for the descendands*/ }

		public virtual void Initialize(BrokerAdapter brokerAdapterPassed, IDataSourceEditor dataSourceEditor) {
			this.BrokerAdapter = brokerAdapterPassed;
			this.DataSourceEditor = dataSourceEditor;
			this.initializeEditorFields();
		}
		// was intended to be abstract but has implementation for Designer to be able to instantiate BrokerEditor
		public virtual void PushBrokerAdapterSettingsToEditor() {
			throw new Exception("please override BrokerAdapter::PushBrokerAdapterSettingsToEditor() for brokerAdapter=[" + BrokerAdapter + "]");
		}
		// was intended to be abstract but has implementation for Designer to be able to instantiate BrokerEditor
		public virtual void PushEditedSettingsToBrokerAdapter() {
			throw new Exception("please override BrokerAdapter::PushEditedSettingsToBrokerAdapter() for brokerAdapter=[" + BrokerAdapter + "]");
		}

		void initializeEditorFields() {
			this.IgnoreEditorFieldChangesWhileInitializingEditor = true;
			this.PushBrokerAdapterSettingsToEditor();
			this.IgnoreEditorFieldChangesWhileInitializingEditor = false;
		}
	}
}