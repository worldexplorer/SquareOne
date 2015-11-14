using System;

using Sq1.Core;
using Sq1.Core.Backtesting;
using Sq1.Core.StrategyBase;
using Sq1.Core.Livesim;

namespace Sq1.Adapters.QuikLivesim {
	public class QuikLivesimDataSource : LivesimDataSource {
		public QuikLivesimStreaming		StreamingAsQuikLivesimNullUnsafe	{ get { return base.StreamingAdapter	as QuikLivesimStreaming; } }
		public QuikLivesimBroker		BrokerAsQuikLivesimNullUnsafe		{ get { return base.BrokerAdapter		as QuikLivesimBroker; } }

		QuikLivesimDataSource() {
			base.Name				= "QuikLivesimDataSource";
			base.StreamingAdapter	= new QuikLivesimStreaming(this);
			base.BrokerAdapter		= new QuikLivesimBroker(this);
		}

		public QuikLivesimDataSource(ScriptExecutor executor) : this() {
			this.Executor = executor;
		}
	}
}