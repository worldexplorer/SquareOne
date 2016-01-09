using System;

using WeifenLuo.WinFormsUI.Docking;

using Sq1.Core;
using Sq1.Core.Streaming;
using Sq1.Core.DataFeed;

namespace Sq1.Adapters.Quik.Streaming {
	public partial class QuikStreamingEditor {
		public String DdeServerPrefix {
			get { return this.txtDdeServerPrefix.Text; }
			set { this.txtDdeServerPrefix.Text = value; }
		}
		public String DdeTopicQuotes {
			get { return this.txtTopicQuotes.Text; }
			set { this.txtTopicQuotes.Text = value; }
		}
		public String DdeTopicTrades {
			get { return this.txtTopicTrades.Text; }
			set { this.txtTopicTrades.Text = value; }
		}
		public String DdeTopicPrefixDom {
			get { return this.txtTopicPrefixDOM.Text; }
			set { this.txtTopicPrefixDOM.Text = value; }
		}

		QuikStreaming	quikStreamingAdapter { get { return base.StreamingAdapter as QuikStreaming; } }
		bool			dontStartStopDdeServer_imSyncingDdeStarted_intoTheBtnText_only;

		public QuikStreamingEditor() {
			this.InitializeComponent();
		}
		// NEVER_FORGET_":this()" DataSourceEditorControl.PopulateStreamingBrokerListViewsFromDataSource() => streamingAdapterInstance.StreamingEditorInitialize() will call this
		public QuikStreamingEditor(StreamingAdapter quikStreamingAdapter, IDataSourceEditor dataSourceEditor) : this() {
			base.Initialize(quikStreamingAdapter, dataSourceEditor);
			if (this.cbxStartDde.Checked == this.quikStreamingAdapter.DdeServerStarted) return;
			try {
				this.dontStartStopDdeServer_imSyncingDdeStarted_intoTheBtnText_only = true;
				this.cbxStartDde.Checked  = this.quikStreamingAdapter.DdeServerStarted;
			} catch (Exception ex) {
				string msg = "HOPEFULLY_NEVER_HAPPENS__YOU_CAUGHT_IT_EARLIER //QuikStreamingEditor(" + quikStreamingAdapter + ")";
				Assembler.PopupException(msg, ex);
			} finally {
				this.dontStartStopDdeServer_imSyncingDdeStarted_intoTheBtnText_only = false;
				this.propagateDdeState_intoBtnText_threadUnsafe();
			}
		}
		public override void PushStreamingAdapterSettingsToEditor() {
			this.DdeServerPrefix	= this.quikStreamingAdapter.DdeServiceName;
			this.DdeTopicQuotes		= this.quikStreamingAdapter.DdeTopicQuotes;
			this.DdeTopicTrades		= this.quikStreamingAdapter.DdeTopicTrades;
			this.DdeTopicPrefixDom	= this.quikStreamingAdapter.DdeTopicPrefixDom;
		}
		public override void PushEditedSettingsToStreamingAdapter() {
			if (base.IgnoreEditorFieldChangesWhileInitializingEditor) return;
			this.quikStreamingAdapter.DdeServiceName	= this.DdeServerPrefix;
			this.quikStreamingAdapter.DdeTopicQuotes	= this.DdeTopicQuotes;
			this.quikStreamingAdapter.DdeTopicTrades	= this.DdeTopicTrades;
			this.quikStreamingAdapter.DdeTopicPrefixDom	= this.DdeTopicPrefixDom;
		}

		void propagateDdeState_intoBtnText_threadUnsafe() {
			string btnTxtMustBe = this.quikStreamingAdapter.DdeServerStartStopOppositeAction;
			if (this.cbxStartDde.Text == btnTxtMustBe) return;
				this.cbxStartDde.Text  = btnTxtMustBe;
		}
    }
}