using Sq1.Core.Backtesting;
using Sq1.Core.DataTypes;
using Sq1.Core.DataFeed;
using Sq1.Core.StrategyBase;

namespace Sq1.Core.Livesim {
	public class LivesimDataSource : BacktestDataSource {
		public ScriptExecutor Executor;

		public LivesimStreaming		StreamingAsLivesimNullUnsafe	{ get { return base.StreamingProvider	as LivesimStreaming; } }
		public LivesimBroker	 	   BrokerAsLivesimNullUnsafe	{ get { return base.BrokerProvider		as LivesimBroker; } }

		public LivesimDataSource() {
			base.Name				= "LivesimDataSource";
			base.StreamingProvider	= new LivesimStreaming(this);
			base.BrokerProvider		= new LivesimBroker(this);
		}

		public LivesimDataSource(ScriptExecutor executor) : this() {
			this.Executor = executor;
		}
	}
}