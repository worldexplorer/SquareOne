using Sq1.Core.Backtesting;
using Sq1.Core.StrategyBase;

namespace Sq1.Core.Livesim {
	public class LivesimDataSource : BacktestDataSource {
		public ScriptExecutor Executor;

		public LivesimStreaming		StreamingAsLivesimNullUnsafe	{ get { return base.StreamingAdapter	as LivesimStreaming; } }
		public LivesimBroker	 	   BrokerAsLivesimNullUnsafe	{ get { return base.BrokerAdapter		as LivesimBroker; } }

		public LivesimDataSource() {
			base.Name				= "LivesimDataSource";
			base.StreamingAdapter	= new LivesimStreaming(this);
			base.BrokerAdapter		= new LivesimBroker(this);
		}

		public LivesimDataSource(ScriptExecutor executor) : this() {
			this.Executor = executor;
		}
	}
}