using Sq1.Core.Backtesting;
using Sq1.Core.DataTypes;
using Sq1.Core.DataFeed;

namespace Sq1.Core.Livesim {
	public class LivesimDataSource : DataSource {
		public StreamingLivesim		StreamingLivesim	{ get { return base.StreamingProvider as StreamingLivesim; } }
		public BrokerLivesim	 	BrokerLivesim		{ get { return base.BrokerProvider as BrokerLivesim; } }

		public LivesimDataSource() {
			base.Name = "BacktestDataSource";
			base.StreamingProvider = new StreamingLivesim();
			base.BrokerProvider = new BrokerLivesim();
		}
		public void Initialize(Bars bars, BacktestSpreadModeler spreadModeler) {
			base.MarketInfo = bars.MarketInfo;
			base.ScaleInterval = bars.ScaleInterval;
			base.Symbols.Clear();
			base.Symbols.Add(bars.Symbol);
			base.StreamingProvider.InitializeFromDataSource(this);
			this.StreamingLivesim.SpreadModeler = spreadModeler;
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