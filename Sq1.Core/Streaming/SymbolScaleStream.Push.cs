using System;
using System.Collections.Generic;

using Sq1.Core.DataTypes;
using Sq1.Core.Backtesting;
using Sq1.Core.Livesim;
using Sq1.Core.Charting;

namespace Sq1.Core.Streaming {
	public partial class SymbolScaleStream<STREAMING_CONSUMER_CHILD> {
		public		abstract void PushQuote_toConsumers					(Quote quoteDequeued_singleInstance);
		//protected	abstract void QuoteClonedBound_attachToStreamingBar_ofCosumer	(Quote quoteClone_sernoEnriched_withStreamingBarUnattachedToParents,	STREAMING_CONSUMER_CHILD barConsumer);
		//protected	abstract Quote  Quote_cloneBind						(Quote quoteCloned_intrabarSernoEnriched_unbound,						STREAMING_CONSUMER_CHILD quoteConsumer);
	}
}
