using System;
using System.Collections.Generic;

using Sq1.Core.Charting;
using Sq1.Core.DataTypes;

namespace Sq1.Core.Streaming {
	public partial class SymbolScaleStreamChart : SymbolScaleStream<StreamingConsumerChart> {
		Dictionary<StreamingConsumerChart, BinderAttacher_perStreamingChart> binderPerConsumer;

		SymbolScaleStreamChart() : base() {
			binderPerConsumer	= new Dictionary<StreamingConsumerChart, BinderAttacher_perStreamingChart>();
		}

		public SymbolScaleStreamChart(SymbolChannel<StreamingConsumerChart> symbolChannel, string symbol, BarScaleInterval scaleInterval
						, string reasonIwasCreated = "REASON_UNKNOWN") : this() {
			throw new Exception("YOU_DIDNT_IMPLEMENT_ME");
		}

		public Bars GetBarsOfChart(StreamingConsumerChart consumer) {
			return this.binderPerConsumer[consumer].Consumer.ChartShadow.Bars;
		}
	}
}
