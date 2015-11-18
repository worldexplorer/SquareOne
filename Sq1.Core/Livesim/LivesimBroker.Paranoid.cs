using Sq1.Core;

namespace Sq1.Core.Livesim {
	public partial class LivesimBroker {
		public override void Connect() {
			string msig = " //UpstreamConnect(" + this.ToString() + ")";
			string msg = "LIVESIM_CHILDREN_SHOULD_NEVER_RECEIVE_UpstreamConnect()";
			Assembler.PopupException(msg + msig, null, false);
		}
		public override void Disconnect() {
			string msig = " //UpstreamDisconnect(" + this.ToString() + ")";
			string msg = "LIVESIM_CHILDREN_SHOULD_NEVER_RECEIVE_UpstreamDisonnect()";
			Assembler.PopupException(msg + msig, null, false);
		}
	}
}