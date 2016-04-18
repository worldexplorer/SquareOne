using System;

using Sq1.Core.DataTypes;

namespace Sq1.Core.Streaming {
	public partial class SymbolScaleStream<STREAMING_CONSUMER_CHILD> {
		public	abstract void PushQuote_toConsumers					(Quote quoteDequeued_singleInstance);
		public	abstract void PushLevelTwoFrozen_toConsumers(LevelTwoFrozen levelTwo_frozenSorted_immutableNoWatchdog_peeledOffStreamingSnap_singleInstance_sameForAllChartsTimeframes);
	}
}
