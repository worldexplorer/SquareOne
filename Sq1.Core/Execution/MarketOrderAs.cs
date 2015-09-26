namespace Sq1.Core.Execution {
	public enum MarketOrderAs {
		ERROR						= 0,
		Unknown						= 1,
		MarketUnchanged				= 2,
		MarketZeroSentToBroker		= 3,	// looks like default, must be crossmarket to fill it right now
		MarketMinMaxSentToBroker	= 4,
		LimitTidal					= 5,
		LimitCrossMarket			= 6,
	}
}
