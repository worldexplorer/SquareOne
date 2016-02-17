using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Sq1.Core;

namespace Sq1.Adapters.Quik.Streaming.Dde.XlDde {
	public abstract partial class XlDdeTableMonitoreable<T> : XlDdeTable {
				List<T>			dataStructuresParsed;
		public	UserControl		UserControlMonitoringMe;

		public override bool TopicConnected {
			get { return base.TopicConnected; }
			set {
				base.TopicConnected = value;
				if (value == false) {
					this.ColumnsIdentified = false;
					this.raiseDataStructuresParsed_Table(new List<T>());	// clean the monitor on topic disconnected from Quik side (user clicked "stop sending by DDE")
				}
			}
		}

		public XlDdeTableMonitoreable(string topic, QuikStreaming quikStreaming, List<XlColumn> columns, bool oneRowUpdates = false)
			: base(topic, quikStreaming, columns, oneRowUpdates) {
			this.dataStructuresParsed = new List<T>();
		}

		protected override void IncomingTableRow_convertToDataStructure(XlRowParsed row) {
			try {
				T oneParsed = this.IncomingTableRow_convertToDataStructure_monitoreable(row);
				if (base.DdeWillDeliver_updatesForEachRow_inSeparateMessages) {
					bool updatedByPrimaryKey = false;
					int indexToReplace = -1;

					//foreach (T eachParsed in this.dataStructuresParsed) {
					for (int i=0; i<this.dataStructuresParsed.Count; i++) {
						T eachParsed = this.dataStructuresParsed[i];
						string primaryKeyNotFound = "primaryKeyNotFound[" + base.PrimaryKey_forEachRowUpdate + "]";
						string primaryValue = row.GetString(base.PrimaryKey_forEachRowUpdate, primaryKeyNotFound);
						if (primaryValue == primaryKeyNotFound) {
							string msg = "PRIMARY_KEY_MUST_BE_PARSED_FOR_EACH_MESSAGE";
							Assembler.PopupException(msg);
							continue;
						}
						string parsedAsString = eachParsed.ToString();
						if (parsedAsString.Contains(primaryValue) == false) continue;
						updatedByPrimaryKey = true;
						indexToReplace = i;
					}
					if (updatedByPrimaryKey == false) {
						this.dataStructuresParsed.Add(oneParsed);
					} else {
						this.dataStructuresParsed.RemoveAt(indexToReplace);
						this.dataStructuresParsed.Insert(indexToReplace, oneParsed);
					}
				} else {
					if (this.dataStructuresParsed.Contains(oneParsed) == false) {
						string msg = "RIM3,LKOH,SBER EACH_SYMBOL_PARSED__WILL_BE_ADDED_INTO_THE_LIST";
						this.dataStructuresParsed.Add(oneParsed);
					} else {
						string msg = "FOR_DOM_EACH_ROW_PARSED_RETURNS_SAME_LEVEL2 LIST_WILL_CONTAIN_ONE_ELEMENT A_LEVEL2_FOR_A_SYMBOL ";
					}
				}
				this.raiseDataStructureParsed_One(oneParsed);
			} catch (Exception ex) {
				string msg = "THROWN_IN_IncomingTableRow_convertToDataStructure(" + row + ")";
				Assembler.PopupException(msg, ex, false);
			}
		}
		protected abstract T IncomingTableRow_convertToDataStructure_monitoreable(XlRowParsed row);


		protected override void IncomingTableBegun() {
			if (base.DdeWillDeliver_updatesForEachRow_inSeparateMessages) return;		// don't clean
			this.dataStructuresParsed.Clear();
		}

		protected override void IncomingTableTerminated() {
			this.raiseDataStructuresParsed_Table(this.dataStructuresParsed);
		}

	}
}
