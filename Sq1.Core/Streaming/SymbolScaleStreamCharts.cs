using System;

using Sq1.Core.Charting;
using Sq1.Core.DataTypes;

namespace Sq1.Core.Streaming {
	public partial class SymbolScaleStreamCharts : SymbolScaleStream<StreamingConsumerChart> {
		//SymbolScaleStreamChart() : base() { }
		public SymbolScaleStreamCharts(
			SymbolChannel<StreamingConsumerChart> symbolChannel, string symbol, BarScaleInterval scaleInterval, string reasonIwasCreated = "REASON_UNKNOWN")
			: base(symbolChannel, symbol, scaleInterval, reasonIwasCreated) {
		}
	}
}
