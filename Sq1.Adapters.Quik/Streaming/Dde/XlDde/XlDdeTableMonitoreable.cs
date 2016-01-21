using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Sq1.Adapters.Quik.Streaming.Dde.XlDde {
	public abstract partial class XlDdeTableMonitoreable<T> : XlDdeTable {
				List<T>			dataStructuresParsed;
		public	UserControl		UserControlMonitoringMe;

		public XlDdeTableMonitoreable(string topic, QuikStreaming quikStreaming, List<XlColumn> columns) : base(topic, quikStreaming, columns) {
			this.dataStructuresParsed = new List<T>();
		}

		protected override void IncomingTableRow_convertToDataStructure(XlRowParsed row) {
			T oneParsed = this.IncomingTableRow_convertToDataStructure_monitoreable(row);
			if (this.dataStructuresParsed.Contains(oneParsed) == false) {
				string msg = "RIM3,LKOH,SBER EACH_SYMBOL_PARSED__WILL_BE_ADDED_INTO_THE_LIST";
				this.dataStructuresParsed.Add(oneParsed);
			} else {
				string msg = "FOR_DOM_EACH_ROW_PARSED_RETURNS_SAME_LEVEL2 LIST_WILL_CONTAIN_ONE_ELEMENT A_LEVEL2_FOR_A_SYMBOL ";
			}
			this.raiseDataStructureParsed_One(oneParsed);
		}
		protected abstract T IncomingTableRow_convertToDataStructure_monitoreable(XlRowParsed row);


		protected override void IncomingTableBegun() {
			this.dataStructuresParsed.Clear();
		}

		protected override void IncomingTableTerminated() {
			this.raiseDataStructuresParsed_Table(this.dataStructuresParsed);
		}

	}
}
