using System;
using System.Windows.Forms;

using Sq1.Core.Streaming;

using Sq1.Adapters.Quik.Streaming.Dde.XlDde;

namespace Sq1.Adapters.Quik.Streaming.Dde {
	public partial class DdeBatchSubscriber {

		internal void RaiseOnDdeMonitorClosing(object sender, FormClosingEventArgs e) {
			foreach (XlDdeTableMonitoreable<LevelTwo> eachLevel2 in this.Level2BySymbol.Values) {
				//TODO eachLevel2.Form(sender, e);
			}
		}
	}
}
