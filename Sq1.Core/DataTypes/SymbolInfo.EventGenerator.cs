using System;

namespace Sq1.Core.DataTypes {
	public partial class SymbolInfo {
		public event EventHandler<EventArgs> PriceDecimalsChanged;
		void raisePriceDecimalsChanged() {
			if (this.PriceDecimalsChanged == null) return;
			try {
				this.PriceDecimalsChanged(this, null);
			} catch (Exception ex) {
				string msg = "ONE_OF_PriceDecimalsChanged_SUBSCRIBERS_THREW_DEPRIVING_OTHERS SymbolInfo[" + this.Symbol + "].PriceDecimals=>[" + this.PriceDecimals + "]";
				Assembler.PopupException(msg, ex);
			}
		}
	}
}
