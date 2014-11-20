using Sq1.Core.DataTypes;
using Sq1.Core.DataFeed;

namespace Sq1.Core.Backtesting {
	public class BacktestDataSource : DataSource {
		public BacktestStreamingProvider BacktestStreamingProvider	{ get { return base.StreamingProvider as BacktestStreamingProvider; } }
		public BacktestBrokerProvider	 BacktestBrokerProvider		{ get { return base.BrokerProvider as BacktestBrokerProvider; } }

		public BacktestDataSource() {
			base.Name = "BacktestDataSource";
			base.StreamingProvider = new BacktestStreamingProvider();
			base.BrokerProvider = new BacktestBrokerProvider();
		}
		public void Initialize(Bars bars, BacktestSpreadModeler spreadModeler) {
			base.MarketInfo = bars.MarketInfo;
			base.ScaleInterval = bars.ScaleInterval;
			base.Symbols.Clear();
			base.Symbols.Add(bars.Symbol);
			base.StreamingProvider.InitializeFromDataSource(this);
			this.BacktestStreamingProvider.SpreadModeler = spreadModeler;
			//base.BrokerProvider.Initialize(this, base.StreamingProvider, null, base.StatusReporter);
		}

		public override string ToString() {
			return this.Name
				+ "(" + this.ScaleInterval.ToString() + ")"
				+ this.SymbolsCSV
				+ " {" + this.StreamingProviderName + ":" + this.BrokerProviderName + "}";
		}
	}
}