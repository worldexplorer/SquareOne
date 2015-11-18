using Sq1.Core;

namespace Sq1.Core.Livesim {
	public partial class LivesimStreaming {
		public override void UpstreamConnect() {
			string msig = " //UpstreamConnect(" + this.ToString() + ")";
			string msg = "LIVESIM_CHILDREN_SHOULD_NEVER_RECEIVE_UpstreamConnect()";
			Assembler.PopupException(msg + msig, null, false);
		}
		public override void UpstreamDisconnect() {
			string msig = " //UpstreamDisconnect(" + this.ToString() + ")";
			string msg = "LIVESIM_CHILDREN_SHOULD_NEVER_RECEIVE_UpstreamDisonnect()";
			Assembler.PopupException(msg + msig, null, false);
		}
	}
}
