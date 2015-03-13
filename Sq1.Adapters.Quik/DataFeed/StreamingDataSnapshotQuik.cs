using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using Sq1.Core.Streaming;

namespace Sq1.Adapters.Quik {
	public class StreamingDataSnapshotQuik : StreamingDataSnapshot {
		[JsonProperty]	protected Dictionary<string, double> FortsDepositSell { get; private set; }
		[JsonProperty]	protected Dictionary<string, double> FortsDepositBuy { get; private set; }
		[JsonProperty]	protected Dictionary<string, double> FortsPriceMin { get; private set; }
		[JsonProperty]	protected Dictionary<string, double> FortsPriceMax { get; private set; }

		public StreamingDataSnapshotQuik(StreamingAdapter streamingAdapter) : base(streamingAdapter) {
			this.FortsDepositBuy	= new Dictionary<string, double>();
			this.FortsDepositSell	= new Dictionary<string, double>();
			this.FortsPriceMin		= new Dictionary<string, double>();
			this.FortsPriceMax		= new Dictionary<string, double>();
		}
		public void StoreFortsSpecifics(QuoteQuik quikQuote) {
			string msg = "I_POSTPONED_REFACTORING_STREAMING_DATA_SNAPSHOT_AND_ITS_LIFECYCLE";
			//Assembler.PopupException(msg);
			if (quikQuote.FortsDepositBuy != 0 || quikQuote.FortsDepositSell != 0) {
				this.FortsPutPriceMinMaxForSymbol(quikQuote.Symbol, quikQuote.FortsPriceMin, quikQuote.FortsPriceMax);
			}
			if (quikQuote.FortsPriceMin != 0 || quikQuote.FortsPriceMax != 0) {
				this.FortsPutDepositForSymbol(quikQuote.Symbol, quikQuote.FortsDepositBuy, quikQuote.FortsDepositSell);
			}
		}
		void FortsPutDepositForSymbol(string Symbol, double FortsDepositBuy, double FortsDepositSell) {
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
		void FortsPutPriceMinMaxForSymbol(string Symbol, double FortsPriceMin, double FortsPriceMax) {
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
