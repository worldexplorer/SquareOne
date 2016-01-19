using System;
using System.Collections.Generic;

using Sq1.Core.Support;

namespace Sq1.Adapters.Quik.Streaming {
	public class Level2 {
		public ConcurrentDictionaryGeneric<double, double> Asks { get; private set; }
		public ConcurrentDictionaryGeneric<double, double> Bids { get; private set; }

		public Level2(ConcurrentDictionaryGeneric<double, double> levelTwoBids, ConcurrentDictionaryGeneric<double, double> levelTwoAsks) {
			this.Bids = levelTwoBids;
			this.Asks = levelTwoAsks;
		}
	}
}
