using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Sq1.Adapters.Quik.Streaming.Dde;
using Sq1.Adapters.Quik.Streaming.Dde.XlDde;

namespace Sq1.Adapters.Quik.Streaming.Monitor {
	public partial class QuikStreamingMonitorForm {
		protected override void OnLoad(EventArgs e) {
			this.quikStreaming.DdeBatchSubscriber.TableQuotes.OnDataStructureParsed_One		+= new EventHandler<XlDdeTableMonitoringEventArg<QuoteQuik>>(		tableQuotes_DataStructureParsed_One);
			this.quikStreaming.DdeBatchSubscriber.TableQuotes.OnDataStructuresParsed_Table	+= new EventHandler<XlDdeTableMonitoringEventArg<List<QuoteQuik>>>(	tableQuotes_DataStructuresParsed_Table);
			this.quikStreaming.OnConnectionStateChanged += new EventHandler<EventArgs>(quikStreaming_OnConnectionStateChanged);
			this.populateWindowTitle_grpStatuses();
			base.OnLoad(e);
		}
		protected override void OnFormClosing(FormClosingEventArgs e) {
			base.OnFormClosing(e);
			this.quikStreaming.OnConnectionStateChanged -= new EventHandler<EventArgs>(quikStreaming_OnConnectionStateChanged);
			this.quikStreaming.DdeBatchSubscriber.TableQuotes.OnDataStructureParsed_One		-= new EventHandler<XlDdeTableMonitoringEventArg<QuoteQuik>>(		tableQuotes_DataStructureParsed_One);
			this.quikStreaming.DdeBatchSubscriber.TableQuotes.OnDataStructuresParsed_Table	-= new EventHandler<XlDdeTableMonitoringEventArg<List<QuoteQuik>>>(	tableQuotes_DataStructuresParsed_Table);
		}
	}
}
