using System;
using System.Collections.Generic;

using Sq1.Core;

using Sq1.Adapters.Quik.Streaming.Dde.XlDde;
using Sq1.Adapters.Quik.Streaming.Monitor;

namespace Sq1.Adapters.Quik.Streaming.Dde {
	public partial class DdeBatchSubscriber {

		void level2_OnDataStructuresParsed_Table_butAlwaysOneElementInList(
							object sender, XlDdeTableMonitoringEventArg<List<Level2>> alwaysJustOneDom) {
			string msig = " //level2_OnDataStructuresParsed_Table_butAlwaysOneElementInList(" + sender + ")";
			XlDdeTableMonitoreable<Level2> tableLevel2 = sender as XlDdeTableMonitoreable<Level2>;
			if (tableLevel2 == null) {
				string msg = "I_MUST_HAVE_BEEN_INVOKED_WITH_XlDdeTableMonitoreable<Level2>";
				Assembler.PopupException(msg + msig);
				return;
			}
			QuikStreamingMonitorDomUserControl domResizeable = tableLevel2.WhereIamMonitored as QuikStreamingMonitorDomUserControl;
			if (domResizeable == null) {
				string msg = "I_MUST_HAVE_BEEN_QuikStreamingMonitorDomUserControl_tableLevel2.WhereIamMonitored[" + tableLevel2.WhereIamMonitored + "]";
				Assembler.PopupException(msg + msig);
				return;
			}
			domResizeable.PopulateLevel2ToTitle();
		}
	}
}
