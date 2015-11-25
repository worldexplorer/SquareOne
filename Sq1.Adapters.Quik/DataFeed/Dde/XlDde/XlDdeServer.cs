using System;
using System.Collections.Generic;

using NDde.Server;

using Sq1.Core;

namespace Sq1.Adapters.Quik.Dde.XlDde {
	public class XlDdeServer : DdeServer {
		Dictionary<string, XlDdeTable> tablesByTopic;
		object lockSynchronousPoke;	// NOT_NEEDED_BUT_I_WANT_TO_MAKE_SURE__REMOVE_IF_TOO_SLOW

		public XlDdeServer(string service) : base(service) {
			this.tablesByTopic = new Dictionary<string, XlDdeTable>();
			this.lockSynchronousPoke = new object();
		}
		public void TableAdd(string topic, XlDdeTable channel) {
			if (this.tablesByTopic.ContainsKey(topic)) return;
			this.tablesByTopic.Add(topic, channel);
		}
		public void TableRemove(string topic) {
			if (this.tablesByTopic.ContainsKey(topic) == false) return;
			this.tablesByTopic.Remove(topic);
		}
		protected override bool OnBeforeConnect(string topic) {
			string msig = " //OnBeforeConnect(" + topic + ")";
			bool readyToAccept = this.tablesByTopic.ContainsKey(topic);
			if (readyToAccept == false) {
				string msg = "QUIK_REQUESTS_TO_ACCEPT_TOPIC_IM_NOT_SUBSCRIBED_TO";
				Assembler.PopupException(msg + msig, null, false);
			}
			return readyToAccept;
		}
		protected override void OnAfterConnect(DdeConversation c) {
			string msig = " //OnAfterConnect(" + c.Topic + ")";
			if (this.tablesByTopic.ContainsKey(c.Topic) == false) {
				string msg = "TABLE_DISAPPEARED_FOR_TOPIC";
				Assembler.PopupException(msg + msig, null, false);
				return;
			}
			XlDdeTable table = this.tablesByTopic[c.Topic];
			c.Tag = table;
			table.ReceivingDataDde = true;
		}
		protected override void OnDisconnect(DdeConversation c) {
			string msig = " //OnDisconnect(" + c.Topic + ")";
			XlDdeTable tableRecipient = (XlDdeTable)c.Tag;
			tableRecipient.ReceivingDataDde = false;
			string msg = "TABLE_MAGICALLY_REMOVED_FOR_TOPIC";
			Assembler.PopupException(msg + msig, null, false);
		}
		protected override PokeResult OnPoke(DdeConversation c, string item, byte[] data, int format) { lock(this.lockSynchronousPoke) {	// NOT_NEEDED_BUT_I_WANT_TO_MAKE_SURE__REMOVE_IF_TOO_SLOW
			string msig = " //OnPoke(" + c.Topic + "," + item + ")";
			//if(format != xlTableFormat) return PokeResult.NotProcessed;
			XlDdeTable tableRecipient = (XlDdeTable)c.Tag;
			try {
				// SET_QUOTE_PUMP_STRAIGHT_NO_SEPARATE_THREAD
				tableRecipient.ParseDeliveredDdeData_pushToStreaming(data);
			} catch (Exception ex) {
				string msg = "DDE_DATA_PARSING_FAILED tableRecipient[" + tableRecipient.Topic + "]";
				Assembler.PopupException(msg + msig, ex);
				return PokeResult.NotProcessed;
			}
			return PokeResult.Processed;
		} }
	}
}
