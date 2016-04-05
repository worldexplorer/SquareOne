using System;
using System.Collections.Generic;

using Sq1.Core.DataTypes;

namespace Sq1.Core.Streaming {
	public partial class SymbolScaleStreamSolidifier : SymbolScaleStream<StreamingConsumerSolidifier> {
		//public SymbolScaleStreamSolidifier() : base() {}

		public SymbolScaleStreamSolidifier(
			SymbolChannel<StreamingConsumerSolidifier> symbolChannel, string symbol, BarScaleInterval scaleInterval, string reasonIwasCreated = "REASON_UNKNOWN")
												: base(symbolChannel, symbol, scaleInterval, reasonIwasCreated) {
			this.barsEmulator_forSolidifier = new BarsEmulator_forSolidifier(symbol, scaleInterval);
		}
	}
}
