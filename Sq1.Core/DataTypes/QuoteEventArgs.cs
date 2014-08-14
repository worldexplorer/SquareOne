using System;

namespace Sq1.Core.DataTypes {
	public class QuoteEventArgs : EventArgs {
		public Quote Quote;
		public QuoteEventArgs(Quote quote) {
			this.Quote = quote;
		}
	}
}
