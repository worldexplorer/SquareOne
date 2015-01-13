namespace Sq1.Core.StrategyBase {
	public enum SystemPerformancePositionsTracking {
		Unknown			= 0,	// should be never used; invalid instantiation => throw immediately
		LongOnly		= 1,
		ShortOnly		= 2,
		LongAndShort	= 3,	// used for StatsForShortAndLongPositions
		BuyAndHold		= 4		// used only for StatsForBuyHold
	}
}
