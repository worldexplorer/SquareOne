using System;

using Sq1.Core.DataFeed;

namespace Sq1.Core.Broker {
	public partial class BrokerAdapter {
		public event EventHandler<EventArgs>		OnBrokerConnectionStateChanged;
		
		public void RaiseOnBrokerConnectionStateChanged() {
			if (this.OnBrokerConnectionStateChanged == null) return;
			try {
				this.OnBrokerConnectionStateChanged(this, null);
			} catch (Exception e) {
				string msg = "EVENT_CONSUMER_THROWN //BrokerAdapter.RaiseOnBrokerConnectionStateChanged()";
				Assembler.PopupException(msg, e);
			}
		}
	}
}