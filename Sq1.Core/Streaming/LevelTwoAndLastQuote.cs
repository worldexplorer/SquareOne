using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using Sq1.Core.DataTypes;

namespace Sq1.Core.Streaming {
	public class LevelTwoAndLastQuote {
								string			symbol;
		[JsonIgnore]	public	Quote			LastQuote;
		[JsonIgnore]	public	LevelTwoHalf	Asks;
		[JsonIgnore]	public	LevelTwoHalf	Bids;

		public LevelTwoAndLastQuote(string symbolPassed) {
			this.symbol = symbolPassed;
			this.Asks = new LevelTwoHalf("LevelTwoAsks[" + this.symbol + "]");
			this.Bids = new LevelTwoHalf("LevelTwoBids[" + this.symbol + "]");
		}

		internal Quote Initialize() {
			Quote ret = this.LastQuote;
			this.LastQuote = null;
			this.Asks.Clear(this, "livesimEnded");
			this.Bids.Clear(this, "livesimEnded");
			return ret;
		}
	}
}
