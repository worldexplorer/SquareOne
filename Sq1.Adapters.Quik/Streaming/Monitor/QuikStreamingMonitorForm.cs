using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Forms;

using Sq1.Widgets;
using Sq1.Adapters.Quik.Dde.XlDde;

namespace Sq1.Adapters.Quik {
	public partial class QuikStreamingMonitorForm : DockContentImproved {
		QuikStreaming quikStreaming;

		// I_DONT_WANT_MONITOR_TO_STAY_AFTER_APPRESTART__HOPING_NEVER_INVOKED_BY_DESERIALIZER
		public QuikStreamingMonitorForm() {
			InitializeComponent();
		}

		// HUMAN_INVOKED_CONSTRUCTOR
		public QuikStreamingMonitorForm(QuikStreaming quikStreamingInstantiatedForDataSource) : this() {
			this.quikStreaming = quikStreamingInstantiatedForDataSource;
			this.quikStreaming.DdeBatchSubscriber.TableQuotes.DataStructureParsed_One		+= new EventHandler<XlDdeTableMonitoringEventArg<QuoteQuik>>(		tableQuotes_DataStructureParsed_One);
			this.quikStreaming.DdeBatchSubscriber.TableQuotes.DataStructuresParsed_Table	+= new EventHandler<XlDdeTableMonitoringEventArg<List<QuoteQuik>>>(	tableQuotes_DataStructuresParsed_Table);
		}

		void quikStreamingMonitorForm_FormClosing(object sender, FormClosingEventArgs e) {
			this.quikStreaming.DdeBatchSubscriber.TableQuotes.DataStructureParsed_One		-= new EventHandler<XlDdeTableMonitoringEventArg<QuoteQuik>>(		tableQuotes_DataStructureParsed_One);
			this.quikStreaming.DdeBatchSubscriber.TableQuotes.DataStructuresParsed_Table	-= new EventHandler<XlDdeTableMonitoringEventArg<List<QuoteQuik>>>(	tableQuotes_DataStructuresParsed_Table);
		}

		void tableQuotes_DataStructuresParsed_Table(object sender, XlDdeTableMonitoringEventArg<List<QuoteQuik>> e) {
			if (this.InvokeRequired) {
				base.BeginInvoke((MethodInvoker)delegate { this.tableQuotes_DataStructuresParsed_Table(sender, e); });
				return;
			}
			this.quikStreamingMonitorControl.OlvQuotes.SetObjects(e.DataStructureParsed);
		}
		void tableQuotes_DataStructureParsed_One(object sender, XlDdeTableMonitoringEventArg<QuoteQuik> e) {
		}

		internal void PopulateWindowTitle_dataSourceName_market_quotesTopic() {
			base.Text = this.ToString();
		}
		public override string ToString() {
			return this.quikStreaming.IdentForMonitorWindowTitle;
		}

	}
}
