using System;
using System.ComponentModel;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Widgets;
using Sq1.Widgets.Level2;

using Sq1.Adapters.Quik.Streaming.Dde;

namespace Sq1.Adapters.Quik.Streaming.Monitor {
	public partial class QuikStreamingMonitorControl : UserControl {
		QuikStreaming quikStreaming;

		public QuikStreamingMonitorControl() {
			InitializeComponent();
			this.olvQuotesCustomize();
		}
		internal void Initialize(QuikStreaming quikStreamingPassed) {
			this.quikStreaming = quikStreamingPassed;
		}

		internal void DomUserControl_createAddFor(DdeTableDepth tableLevel2) {
			string msig = " //DomUserControl_createAddFor(" + tableLevel2.ToString() + ")";
			LevelTwoUserControl level2userControl = new LevelTwoUserControl();
			DockContentImproved ddeMonitorForm = this.Parent as DockContentImproved;
			if (ddeMonitorForm == null) {
				string msg = "I_NEED_THE_UPPER_LEVEL_FORM_VISIBILITY_TO_NOT_TO_REPAINT_IF_FORM_IS_MINIMIZED_OR_NOT_SHOWN";
				Assembler.PopupException(msg + msig);
			}
			level2userControl.Initialize(this.quikStreaming, tableLevel2.SymbolInfo, tableLevel2.ToString(), ddeMonitorForm);
			tableLevel2.UserControlMonitoringMe = level2userControl;
			this.flpDoms.Controls.Add(level2userControl);
		}
		internal void DomUserControl_deleteFor(DdeTableDepth tableLevel2) {
			string msig = " //DomUserControl_deleteFor(" + tableLevel2 + ")";
			LevelTwoUserControl domResizeable = tableLevel2.UserControlMonitoringMe as LevelTwoUserControl;
			if (domResizeable == null) {
				string msg = "I_MUST_HAVE_BEEN_LevelTwoUserControl_tableLevel2.WhereIamMonitored[" + tableLevel2.UserControlMonitoringMe + "]";
				Assembler.PopupException(msg + msig);
				return;
			}
			this.flpDoms.Controls.Remove(domResizeable);
		}

		internal void Populate_grpStatuses() {
			this.grpQuotes.Text = this.quikStreaming.DdeBatchSubscriber.TableQuotes.ToString();
			this.grpDom.Text	= this.quikStreaming.DdeBatchSubscriber.DomGroupboxTitle;
			this.grpTrades.Text = this.quikStreaming.DdeBatchSubscriber.TableTrades.ToString();
		}
	}
}
