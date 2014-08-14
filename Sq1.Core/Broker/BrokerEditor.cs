using System;
using System.Windows.Forms;
using Sq1.Core.DataFeed;

namespace Sq1.Core.Broker {
	// this class should be abstract; it's not abstract because VS2012 opens MockBrokerEditor and complains
	// "I need to instantiate base class but base class is abstract"
	public class BrokerEditor : UserControl {
		protected BrokerProvider brokerProvider;
		protected IDataSourceEditor dataSourceEditor;
		protected bool ignoreEditorFieldChangesWhileInitializingEditor;

		public BrokerEditor() { /*used in Design Mode for the descendands*/ }
		public BrokerEditor(BrokerProvider BrokerProvider, IDataSourceEditor dataSourceEditor) {
			this.brokerProvider = BrokerProvider;
			this.dataSourceEditor = dataSourceEditor;
		}
		public void InitializeEditorFields() {
			this.ignoreEditorFieldChangesWhileInitializingEditor = true;
			this.PushBrokerProviderSettingsToEditor();
			this.ignoreEditorFieldChangesWhileInitializingEditor = false;
		}

		// was intended to be abstract but has implementation for Designer to be able to instantiate BrokerEditor
		public virtual void PushBrokerProviderSettingsToEditor() {
			throw new Exception("please override BrokerProvider::PushBrokerProviderSettingsToEditor() for brokerProvider=[" + brokerProvider + "]");
		}
		// was intended to be abstract but has implementation for Designer to be able to instantiate BrokerEditor
		public virtual void PushEditedSettingsToBrokerProvider() {
			throw new Exception("please override BrokerProvider::PushEditedSettingsToBrokerProvider() for brokerProvider=[" + brokerProvider + "]");
		}
	}
}