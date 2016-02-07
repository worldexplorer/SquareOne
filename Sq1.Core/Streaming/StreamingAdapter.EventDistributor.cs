using System;
using Sq1.Core.DataTypes;

namespace Sq1.Core.Streaming {
	public partial class StreamingAdapter {
		public event EventHandler<EventArgs>		OnConnectionStateChanged;
		public event EventHandler<QuoteEventArgs>	OnQuoteReceived_butWasntPushedAnywhere_dueToZeroSubscribers_blinkDataSourceTreeWithOrange;
		
		public void RaiseOnConnectionStateChanged() {
			if (this.OnConnectionStateChanged == null) return;
			try {
				this.OnConnectionStateChanged(this, null);
			} catch (Exception e) {
				string msg = "EVENT_CONSUMER_THROWN //StreamingAdapter.RaiseOnConnectionStateChanged()";
				Assembler.PopupException(msg, e);
			}
		}

		public void RaiseOnQuoteReceived_butWasntPushedAnywhere_dueToZeroSubscribers_blinkDataSourceTreeWithOrange(Quote quoteWithoutSubscribers) {
			if (this.OnQuoteReceived_butWasntPushedAnywhere_dueToZeroSubscribers_blinkDataSourceTreeWithOrange == null) return;
			try {
				this.OnQuoteReceived_butWasntPushedAnywhere_dueToZeroSubscribers_blinkDataSourceTreeWithOrange(this, new QuoteEventArgs(quoteWithoutSubscribers));
			} catch (Exception e) {
				string msg = "EVENT_CONSUMER_THROWN //StreamingAdapter.RaiseOnQuoteReceived_butWasntPushedAnywhere_dueToZeroSubscribers_blinkDataSourceTreeWithOrange()";
				Assembler.PopupException(msg, e);
			}
		}
	}
}
