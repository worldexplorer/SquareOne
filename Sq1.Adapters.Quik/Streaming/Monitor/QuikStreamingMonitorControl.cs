using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;
using System.Collections.Generic;

using Sq1.Core;

using Sq1.Widgets;
using Sq1.Widgets.Level2;

using Sq1.Adapters.Quik.Streaming.Dde;

namespace Sq1.Adapters.Quik.Streaming.Monitor {
	public partial class QuikStreamingMonitorControl : UserControl {
		QuikStreaming quikStreaming;

		List<QuikLevel2Control> allQuikLevel2 { get {
			List<QuikLevel2Control> ret = new List<QuikLevel2Control>();
			foreach (UserControl mustBeQuikLevel2 in this.flpDoms.Controls) {
				QuikLevel2Control quikLevel2 = mustBeQuikLevel2 as QuikLevel2Control;
				if (quikLevel2 == null) {
					string msg = "MUST_CONTAIN_ONLY_QuikLevel2Controls_WHILE_ONE_OF_flpDoms.Controls_IS " + mustBeQuikLevel2 + "]";
					Assembler.PopupException(msg);
					continue;
				}
			}
			return ret;
		} }

		public QuikStreamingMonitorControl() {
			InitializeComponent();
			this.olvQuotesCustomize();
		}

		internal void Initialize(QuikStreaming quikStreamingPassed, Stopwatch stopwatchRarifyingUIupdates_passed) {
			this.quikStreaming = quikStreamingPassed;
			//this.flpDoms.Controls.Clear();
			this.domUserControls_deleteAll();
			this.domUserControls_createForEach_monitoreableLevelTwo(stopwatchRarifyingUIupdates_passed);
		}

		void domUserControls_createForEach_monitoreableLevelTwo(Stopwatch stopwatchRarifyingUIupdates_passed) {
			foreach (DdeTableDepth eachDom in this.quikStreaming.DdeBatchSubscriber.Level2BySymbol.Values) {
				QuikLevel2Control quikLevel2 = new QuikLevel2Control(eachDom, stopwatchRarifyingUIupdates_passed);
				quikLevel2.MyTableDepth_subscribe();
				this.flpDoms.Controls.Add(quikLevel2);
			}
		}
		void domUserControls_deleteAll() {
			if (base.InvokeRequired) {
				base.BeginInvoke((MethodInvoker)delegate() { this.domUserControls_deleteAll(); });
				return;
			}
			List<QuikLevel2Control> avoidingCollectionModifiedException = allQuikLevel2;
			foreach (QuikLevel2Control quikLevel2 in avoidingCollectionModifiedException) {
				quikLevel2.MyTableDepth_unsubscribe();
				this.flpDoms.Controls.Remove(quikLevel2);
			}
			if (this.flpDoms.Controls.Count > 0) {
				string msg = "MUST_NEVER_HAPPEN__I_JUST_REMOVED_QUIK_LEVEL2S_ONE_BY_ONE";
				Assembler.PopupException(msg);
				this.flpDoms.Controls.Clear();
			}
		}

		internal void Populate_grpStatuses() {
			//if (this.quikStreaming.ToString().Contains("")) {
			//	string msg = "TAKE_QUIK_STREAMING_FROM_DATASOURCE_NOT_A_DUMMY";
			//	//Assembler.PopupException(msg);
			//}
			this.grpQuotes.Text = this.quikStreaming.DdeBatchSubscriber.TableQuotes.ToString();
			this.grpDom.Text	= this.quikStreaming.DdeBatchSubscriber.DomGroupboxTitle;
			this.grpTrades.Text = this.quikStreaming.DdeBatchSubscriber.TableTrades.ToString();
		}
	}
}
