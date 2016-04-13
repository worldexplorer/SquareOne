using System;

using Sq1.Core.DataTypes;

namespace Sq1.Core.Streaming {
	public partial class SymbolScaleStream<STREAMING_CONSUMER_CHILD> {
		public		abstract void PushQuote_toConsumers					(Quote quoteDequeued_singleInstance);
		//protected	abstract void QuoteClonedBound_attachToStreamingBar_ofCosumer	(Quote quoteClone_sernoEnriched_withStreamingBarUnattachedToParents,	STREAMING_CONSUMER_CHILD barConsumer);
		//protected	abstract Quote  Quote_cloneBind						(Quote quoteCloned_intrabarSernoEnriched_unbound,						STREAMING_CONSUMER_CHILD quoteConsumer);
		public	abstract void PushLevelTwoFrozen_toConsumers(LevelTwoFrozen levelTwo_frozenSorted_immutableNoWatchdog_peeledOffStreamingSnap_singleInstance_sameForAllChartsTimeframes);
	}
}
