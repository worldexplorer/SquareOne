namespace Sq1.Core.Execution {
	public enum MarketOrderAs {
		ERROR						= 0,
		Unknown						= 1,
		MarketZeroSentToBroker		= 2,	// looks like default, must be crossmarket to fill it right now
		MarketMinMaxSentToBroker	= 3,
		LimitTidal					= 4,
		LimitCrossMarket			= 5,
	}
}
