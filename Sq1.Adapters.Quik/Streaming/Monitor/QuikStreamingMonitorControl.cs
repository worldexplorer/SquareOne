using System;
using System.ComponentModel;
using System.Windows.Forms;

using Sq1.Core;

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
			QuikStreamingMonitorDomUserControl controlForLevel2 = new QuikStreamingMonitorDomUserControl();
			controlForLevel2.Initialize(tableLevel2);
			this.flpDoms.Controls.Add(controlForLevel2);
		}
		internal void DomUserControl_deleteFor(DdeTableDepth tableLevel2) {
			string msig = " //DomUserControl_deleteFor(" + tableLevel2 + ")";
			QuikStreamingMonitorDomUserControl domResizeable = tableLevel2.WhereIamMonitored as QuikStreamingMonitorDomUserControl;
			if (domResizeable == null) {
				string msg = "I_MUST_HAVE_BEEN_QuikStreamingMonitorDomUserControl_tableLevel2.WhereIamMonitored[" + tableLevel2.WhereIamMonitored + "]";
				Assembler.PopupException(msg + msig);
				return;
			}
			this.flpDoms.Controls.Remove(domResizeable);
		}

		internal void Populate_grpStatuses() {
			this.grpQuotes.Text = this.quikStreaming.DdeBatchSubscriber.TableQuotes.ToString();
			this.grpTrades.Text = this.quikStreaming.DdeBatchSubscriber.TableTrades.ToString();
			this.grpDom.Text	= this.quikStreaming.DdeBatchSubscriber.DomGroupboxTitle;
		}
	}
}
