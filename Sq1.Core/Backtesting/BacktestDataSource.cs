using Sq1.Core.DataTypes;
using Sq1.Core.DataFeed;

namespace Sq1.Core.Backtesting {
	public class BacktestDataSource : DataSource {
		public BacktestStreamingProvider BacktestStreamingProvider	{ get { return base.StreamingProvider as BacktestStreamingProvider; } }
		public BacktestBrokerProvider	 BacktestBrokerProvider		{ get { return base.BrokerProvider as BacktestBrokerProvider; } }

		public BacktestDataSource(Bars bars) {
			base.Name = "BacktestDataSource";
//			base.DataSourceManager = bars.DataSource.DataSourceManager;
			base.MarketInfo = bars.MarketInfo;
			base.ScaleInterval = bars.ScaleInterval;
			base.Symbols.Add(bars.Symbol);
			base.StreamingProvider = new BacktestStreamingProvider(bars.Symbol);
			base.StreamingProvider.InitializeFromDataSource(this);
			base.BrokerProvider = new BacktestBrokerProvider();
			//base.BrokerProvider.Initialize(this, base.StreamingProvider, null, base.StatusReporter);
		}

		public override string ToString() {
			return Name + "(" + this.ScaleInterval.ToString() + ")" + SymbolsCSV
				+ " {" + StreamingProviderName + ":" + BrokerProviderName + "}";
		}
	}
}