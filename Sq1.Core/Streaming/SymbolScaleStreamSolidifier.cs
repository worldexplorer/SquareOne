using System;
using System.Collections.Generic;

using Sq1.Core.DataTypes;

namespace Sq1.Core.Streaming {
	public partial class SymbolScaleStreamSolidifier : SymbolScaleStream<StreamingConsumerSolidifier> {
		public	SolidifierBarsEmulator								UnattachedStreamingBar_factoryPerSymbolScale	{ get; protected set; }

		public SymbolScaleStreamSolidifier() : base() {
		}


		public SymbolScaleStreamSolidifier(SymbolChannel<StreamingConsumerSolidifier> symbolChannel, string symbol, BarScaleInterval scaleInterval
						, string reasonIwasCreated = "REASON_UNKNOWN") : this() {
			this.UnattachedStreamingBar_factoryPerSymbolScale = new SolidifierBarsEmulator(symbol, ScaleInterval);
			throw new Exception("YOU_DIDNT_IMPLEMENT_ME");
		}
	}
}
