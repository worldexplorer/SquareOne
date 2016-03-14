﻿using System;
using System.Windows.Forms;

using WeifenLuo.WinFormsUI.Docking;

using Sq1.Core;
using Sq1.Core.Streaming;
using Sq1.Core.DataFeed;

namespace Sq1.Adapters.Quik.Streaming {
	public partial class QuikStreamingEditorControl {
		QuikStreaming	quikStreamingAdapter { get { return base.StreamingAdapter as QuikStreaming; } }
		bool			dontStartStopDdeServer_imSyncingDdeStarted_intoTheBtnText_only;

		public QuikStreamingEditorControl() {
			this.InitializeComponent();
		}
		// NEVER_FORGET_":this()" DataSourceEditorControl.PopulateStreamingBrokerListViewsFromDataSource() => streamingAdapterInstance.StreamingEditorInitialize() will call this
		public QuikStreamingEditorControl(StreamingAdapter quikStreamingAdapter, IDataSourceEditor dataSourceEditor) : this() {
			base.Initialize(quikStreamingAdapter, dataSourceEditor);
			this.propagateStreamingConnected_intoBtnStateText();
		}

		public override void PopulateStreamingAdapterSettingsToEditor() {
			this.txtDdeServerPrefix.Text		= this.quikStreamingAdapter.DdeServiceName;
			this.txtDdeTopicQuotes.Text			= this.quikStreamingAdapter.DdeTopicQuotes;
			this.txtDdeTopicTrades.Text			= this.quikStreamingAdapter.DdeTopicTrades;
			this.txtDdeTopicPrefixDom.Text		= this.quikStreamingAdapter.DdeTopicSuffixDom;
			this.txtDdeMonitorRefreshRate.Text	= this.quikStreamingAdapter.DdeMonitorRefreshRateMs.ToString();

			if (Assembler.InstanceInitialized.MainForm_dockFormsFullyDeserialized_layoutComplete) return;
			if (this.quikStreamingAdapter.DdeMonitorPopupOnRestart == false) return;
			// CONTINUE_WITH_MONTIOR_APPRESTART induces {if (existingFormThatIWillCover.DockPanel == null) return;} lnkDdeMonitor_LinkClicked(this, null);
		}
		public override void PushEditedSettingsToStreamingAdapter() {
			if (base.IgnoreEditorFieldChangesWhileInitializingEditor) return;
			this.quikStreamingAdapter.DdeServiceName	= this.txtDdeServerPrefix.Text;
			this.quikStreamingAdapter.DdeTopicQuotes	= this.txtDdeTopicQuotes.Text;
			this.quikStreamingAdapter.DdeTopicTrades	= this.txtDdeTopicTrades.Text;
			this.quikStreamingAdapter.DdeTopicSuffixDom	= this.txtDdeTopicPrefixDom.Text;

			int refreshRateParsed = 200;
			Int32.TryParse(this.txtDdeMonitorRefreshRate.Text, out refreshRateParsed);
			this.quikStreamingAdapter.DdeMonitorRefreshRateMs	= refreshRateParsed;
			this.quikStreamingAdapter.Level2RefreshRateMs		= this.quikStreamingAdapter.DdeMonitorRefreshRateMs;
		}

		void propagateStreamingConnected_intoBtnStateText() {
			if (base.InvokeRequired) {
				base.BeginInvoke(new MethodInvoker(this.propagateStreamingConnected_intoBtnStateText));
				return;
			}
			//if (this.cbxStartDde.Checked == this.quikStreamingAdapter.DdeServerStarted) return;
			if (this.cbxStartDde.Checked != this.quikStreamingAdapter.UpstreamConnected) {
				try {
					this.dontStartStopDdeServer_imSyncingDdeStarted_intoTheBtnText_only = true;
					//this.cbxStartDde.Checked  = this.quikStreamingAdapter.DdeServerStarted;
					this.cbxStartDde.Checked = this.quikStreamingAdapter.UpstreamConnected;
				} catch (Exception ex) {
					string msg = "HOPEFULLY_NEVER_HAPPENS__YOU_CAUGHT_IT_EARLIER //QuikStreamingEditor(" + quikStreamingAdapter + ")";
					Assembler.PopupException(msg, ex);
				} finally {
					this.dontStartStopDdeServer_imSyncingDdeStarted_intoTheBtnText_only = false;
				}
			}
	
			string btnTxtMustBe = this.quikStreamingAdapter.DdeServerStartStop_oppositeAction;
			if (this.cbxStartDde.Text == btnTxtMustBe) return;
				this.cbxStartDde.Text  = btnTxtMustBe;
		}
	}
}