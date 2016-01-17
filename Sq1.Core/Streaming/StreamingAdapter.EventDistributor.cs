using System;

namespace Sq1.Core.Streaming {
	public partial class StreamingAdapter {
		public event EventHandler<EventArgs> OnConnectionStateChanged;
		
		public void RaiseOnConnectionStateChanged() {
			if (this.OnConnectionStateChanged == null) return;
			try {
				this.OnConnectionStateChanged(this, null);
			} catch (Exception e) {
				string msg = "EVENT_CONSUMER_THROWN //StreamingAdapter.RaiseOnConnectionStateChanged()";
				Assembler.PopupException(msg, e);
			}
		}

	}
}
