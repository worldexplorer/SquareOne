using System;
using System.Collections.Generic;

namespace Sq1.Adapters.Quik.Streaming.Dde.XlDde {
	public class XlDdeTableMonitoringEventArg<T> : EventArgs {
		public		T	DataStructureParsed		{ get; private set; }
		public XlDdeTableMonitoringEventArg(T dataStructureParsed) {
			this.DataStructureParsed = dataStructureParsed;
		}
	}
}
