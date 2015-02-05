using Sq1.Core.Backtesting;
using Sq1.Core.DataTypes;
using Sq1.Core.DataFeed;

namespace Sq1.Core.Livesim {
	public class LivesimDataSource : BacktestDataSource {
		public LivesimStreaming		StreamingLivesim	{ get { return base.StreamingProvider as LivesimStreaming; } }
		public LivesimBroker	 	BrokerLivesim		{ get { return base.BrokerProvider as LivesimBroker; } }

		public LivesimDataSource() {
			base.Name				= "BacktestDataSource";
			base.StreamingProvider	= new LivesimStreaming();
			base.BrokerProvider		= new LivesimBroker();
		}
	}
}