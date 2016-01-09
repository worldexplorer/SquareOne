using System;
using System.Collections.Generic;

namespace Sq1.Adapters.Quik.Dde.XlDde {
	public abstract partial class XlDdeTableMonitoreable<T> : XlDdeTable {
		List<T> dataStructuresParsed;

		public XlDdeTableMonitoreable(string topic, QuikStreaming quikStreaming, List<XlColumn> columns) : base(topic, quikStreaming, columns) {
			this.dataStructuresParsed = new List<T>();
		}

		protected override void IncomingTableRow_convertToDataStructure(XlRowParsed row) {
			T dataStructureParsed = this.IncomingTableRow_convertToDataStructure_monitoreable(row);
			this.dataStructuresParsed.Add(dataStructureParsed);
			this.raiseDataStructureParsed_One(dataStructureParsed);
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
