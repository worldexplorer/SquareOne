using System;
using System.Collections.Generic;

using Sq1.Adapters.Quik.Dde.XlDde;
using Sq1.Adapters.QuikLivesim;
using Sq1.Adapters.QuikLivesim.Dde.XlDde;
using Sq1.Core;

namespace Sq1.Adapters.QuikLiveism.Dde {
	public abstract class XlDdeTableGenerator  {
		protected abstract	string			DdeGeneratorClassName	{ get; }
		public				DateTime		LastDataReceived		{ get; protected set; }

		public		string					Topic					{ get; private set; }
		protected	QuikLivesimStreaming	QuikLivesimStreaming	{ get; private set; }

		public		List<XlColumn>					Columns				{ get; private set; }
		public		Dictionary<string, XlColumn>	XlColumnsLookup		{ get; private set; }

		protected	XlWriter				XlWriter				{ get; private set; }

		protected XlDdeTableGenerator(string topic, QuikLivesimStreaming quikLivesimStreaming) {
			this.Topic = topic;
			this.QuikLivesimStreaming = quikLivesimStreaming;
		}
		protected void Initialize(List<XlColumn> columns) {
			if (columns == null || columns.Count == 0) {
				string msg = "DONT_FEED_ME_WITH_EMPTY_COLUMNS //XlWriter(" + columns + ")";
				Assembler.PopupException(msg);
				return;
			}

			this.Columns = columns;
			this.XlColumnsLookup = new Dictionary<string, XlColumn>();
			foreach (XlColumn col in this.Columns) {
				this.XlColumnsLookup.Add(col.Name, col);
			}

			this.XlWriter = new XlWriter(this);
		}

		public virtual void OutgoingTableBegin() {
			this.XlWriter.StartNewTable();
		}
		public virtual void OutgoingTableTerminate() {
		}

		public abstract void OutgoingObjectBufferize_eachRow(object objectToSendAsDdeMessage);

		public override string ToString() {
			string ret = "";
			if (string.IsNullOrEmpty(this.Topic) == false) ret += "Topic[" + this.Topic + "] ";
			ret = this.DdeGeneratorClassName + "{Symbols[" + this.QuikLivesimStreaming.StreamingDataSnapshot.SymbolsSubscribedAndReceiving + "] " + ret + "}";
			return ret;
		}
	}
}