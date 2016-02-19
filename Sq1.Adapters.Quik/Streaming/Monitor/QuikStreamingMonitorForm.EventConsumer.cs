using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Sq1.Core;

using Sq1.Adapters.Quik.Streaming.Dde;
using Sq1.Adapters.Quik.Streaming.Dde.XlDde;

namespace Sq1.Adapters.Quik.Streaming.Monitor {
	public partial class QuikStreamingMonitorForm {
		protected override void OnLoad(EventArgs e) {
			try {
				this.quikStreaming.DdeBatchSubscriber.TableQuotes.OnDataStructureParsed_One		+= new EventHandler<XlDdeTableMonitoringEventArg<QuoteQuik>>(		tableQuotes_DataStructureParsed_One);
				this.quikStreaming.DdeBatchSubscriber.TableQuotes.OnDataStructuresParsed_Table	+= new EventHandler<XlDdeTableMonitoringEventArg<List<QuoteQuik>>>(	tableQuotes_DataStructuresParsed_Table);
				this.quikStreaming.OnConnectionStateChanged += new EventHandler<EventArgs>(this.quikStreaming_OnConnectionStateChanged);
				base.OnLoad(e);

				this.populateWindowTitle_grpStatuses();

				if (this.quikStreaming.DdeMonitorPopupOnRestart) return;	// we are deserializing => popping up => no need to set the flag I'm to complying to
				this.quikStreaming.DdeMonitorPopupOnRestart = true;
				this.quikStreaming.DataSourceEditor.SerializeDataSource_saveAdapters();
			} catch (Exception ex) {
				string msg = "IS_DATASOURCE_EDITOR_NULL? this.quikStreaming.DataSourceEditor[" + this.quikStreaming.DataSourceEditor + "] //QuikStreamingMonitorForm.OnLoad()";
				Assembler.PopupException(msg, ex);
			}
		}
		protected override void OnFormClosing(FormClosingEventArgs e) {
			try {
				base.OnFormClosing(e);
				this.quikStreaming.OnConnectionStateChanged -= new EventHandler<EventArgs>(this.quikStreaming_OnConnectionStateChanged);
				this.quikStreaming.DdeBatchSubscriber.TableQuotes.OnDataStructureParsed_One		-= new EventHandler<XlDdeTableMonitoringEventArg<QuoteQuik>>(		tableQuotes_DataStructureParsed_One);
				this.quikStreaming.DdeBatchSubscriber.TableQuotes.OnDataStructuresParsed_Table	-= new EventHandler<XlDdeTableMonitoringEventArg<List<QuoteQuik>>>(	tableQuotes_DataStructuresParsed_Table);

				if (Assembler.InstanceInitialized.MainFormClosingIgnoreReLayoutDockedForms) return;	// I closed the app, but user didn't click the Monitor and wants it be restored on appRestart
				this.quikStreaming.DdeMonitorPopupOnRestart = false;
				this.quikStreaming.DataSourceEditor.SerializeDataSource_saveAdapters();
			} catch (Exception ex) {
				string msg = "IS_DATASOURCE_EDITOR_NULL? this.quikStreaming.DataSourceEditor[" + this.quikStreaming.DataSourceEditor + "] //QuikStreamingMonitorForm.OnFormClosing()";
				Assembler.PopupException(msg, ex);
			}
		}
	}
}
