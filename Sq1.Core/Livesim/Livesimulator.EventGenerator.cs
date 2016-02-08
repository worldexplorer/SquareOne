using System;
using Sq1.Core.DataTypes;

namespace Sq1.Core.Livesim {
	public partial class Livesimulator {
		public event EventHandler<QuoteEventArgs>	OnQuoteReceived_zeroSubscribers_blinkDataSourceTreeWithOrange;

		void raiseOnQuoteReceived_zeroSubscribers_blinkDataSourceTreeWithOrange(QuoteEventArgs e) {
			if (this.OnQuoteReceived_zeroSubscribers_blinkDataSourceTreeWithOrange == null) return;
			try {
				this.OnQuoteReceived_zeroSubscribers_blinkDataSourceTreeWithOrange(this, e);
			} catch (Exception ex) {
				string msg = "EVENT_CONSUMER(USED_ONLY_FOR_LIVE_SIMULATOR)_THROWN //Livesimulator.OnQuoteReceived_zeroSubscribers_blinkDataSourceTreeWithOrange(" + e.Quote + ")";
				Assembler.PopupException(msg, ex);
			}
		}
	}
}
