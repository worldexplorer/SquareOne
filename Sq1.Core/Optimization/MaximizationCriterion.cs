namespace Sq1.Core.Optimization {
	public enum MaximizationCriterion {
		Unknown				= 0,

		PositionsCount		= 1,
		PositionAvgProfit	= 2,
		NetProfit			= 3,
		WinLossRatio		= 4,
		ProfitFactor		= 5,
		RecoveryFactor		= 6,
		MaxDrawDown			= 7,
		MaxConsecWinners	= 8,
		MaxConsecLosers		= 9
	}
}
