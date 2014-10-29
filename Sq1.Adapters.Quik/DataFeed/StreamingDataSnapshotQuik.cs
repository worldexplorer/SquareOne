using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Sq1.Core.DataTypes;
using Sq1.Core.Streaming;

namespace Sq1.Adapters.Quik {
	[DataContract]
	public class StreamingDataSnapshotQuik : StreamingDataSnapshot {
		[DataMember]
		protected Dictionary<string, double> FortsDepositSell { get; private set; }
		[DataMember]
		protected Dictionary<string, double> FortsDepositBuy { get; private set; }
		[DataMember]
		protected Dictionary<string, double> FortsPriceMin { get; private set; }
		[DataMember]
		protected Dictionary<string, double> FortsPriceMax { get; private set; }

		public StreamingDataSnapshotQuik(StreamingProvider streamingProvider) : base(streamingProvider) {
			this.FortsDepositBuy = new Dictionary<string, double>();
			this.FortsDepositSell = new Dictionary<string, double>();
			this.FortsPriceMin = new Dictionary<string, double>();
			this.FortsPriceMax = new Dictionary<string, double>();
		}
		public override void UpdateLastBidAskSnapFromQuote(Quote quote) {
			base.UpdateLastBidAskSnapFromQuote(quote);
			QuoteQuik quikQuote = QuoteQuik.SafeUpcast(quote);
			if (quikQuote.FortsDepositBuy != 0 || quikQuote.FortsDepositSell != 0) {
				this.FortsPutPriceMinMaxForSymbol(quikQuote.Symbol, quikQuote.FortsDepositBuy, quikQuote.FortsDepositSell);
			}
			if (quikQuote.FortsPriceMin != 0 || quikQuote.FortsPriceMax != 0) {
				this.FortsPutDepositForSymbol(quikQuote.Symbol, quikQuote.FortsPriceMin, quikQuote.FortsPriceMax);
			}
		}
		private void FortsPutDepositForSymbol(string Symbol, double FortsDepositBuy, double FortsDepositSell) {
			if (this.FortsDepositBuy.ContainsKey(Symbol)) {
				this.FortsDepositBuy[Symbol] = FortsDepositBuy;
			} else {
				this.FortsDepositBuy.Add(Symbol, FortsDepositBuy);
			}
			if (this.FortsDepositSell.ContainsKey(Symbol)) {
				this.FortsDepositSell[Symbol] = FortsDepositSell;
			} else {
				this.FortsDepositSell.Add(Symbol, FortsDepositSell);
			}
		}
		private void FortsPutPriceMinMaxForSymbol(string Symbol, double FortsPriceMin, double FortsPriceMax) {
			if (this.FortsPriceMin.ContainsKey(Symbol)) {
				this.FortsPriceMin[Symbol] = FortsPriceMin;
			} else {
				this.FortsPriceMin.Add(Symbol, FortsPriceMin);
			}
			if (this.FortsPriceMax.ContainsKey(Symbol)) {
				this.FortsPriceMax[Symbol] = FortsPriceMax;
			} else {
				this.FortsPriceMax.Add(Symbol, FortsPriceMax);
			}
		}

		public double FortsGetDepositBuyForSymbol(string Symbol) {
			double ret = 0;
			if (this.FortsDepositBuy.ContainsKey(Symbol)) {
				ret = this.FortsDepositBuy[Symbol];
			}
			return ret;
		}
		public double FortsGetDepositSellForSymbol(string Symbol) {
			double ret = 0;
			if (this.FortsDepositSell.ContainsKey(Symbol)) {
				ret = this.FortsDepositSell[Symbol];
			}
			return ret;
		}
		public double FortsGetPriceMinForSymbol(string Symbol) {
			double ret = 0;
			if (this.FortsPriceMin.ContainsKey(Symbol)) {
				ret = this.FortsPriceMin[Symbol];
			}
			return ret;
		}
		public double FortsGetPriceMaxForSymbol(string Symbol) {
			double ret = 0;
			if (this.FortsPriceMax.ContainsKey(Symbol)) {
				ret = this.FortsPriceMax[Symbol];
			}
			return ret;
		}

	}
}
