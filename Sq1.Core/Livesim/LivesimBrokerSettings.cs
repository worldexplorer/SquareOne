using Newtonsoft.Json;
using Sq1.Core.Repositories;
using Sq1.Core.StrategyBase;

namespace Sq1.Core.Livesim {
	public class LivesimBrokerSettings : StrategySelfSaver {
		[JsonProperty]	public	int		DelayBeforeFillMillisMin;
		[JsonProperty]	public	int		DelayBeforeFillMillisMax;
		[JsonProperty]	public	bool	DelayBeforeFillEnabled;

		[JsonProperty]	public	int		KillPendingDelayMillisMin;
		[JsonProperty]	public	int		KillPendingDelayMillisMax;
		[JsonProperty]	public	bool	KillPendingDelayEnabled;

		[JsonProperty]	public	int		OrderRejectionHappensOncePerXordersMin;
		[JsonProperty]	public	int		OrderRejectionHappensOncePerXordersMax;
		[JsonProperty]	public	bool	OrderRejectionEnabled;

		[JsonProperty]	public	int		PartialFillHappensOncePerQuoteMin;
		[JsonProperty]	public	int		PartialFillHappensOncePerQuoteMax;
		[JsonProperty]	public	int		PartialFillPercentageFilledMin;
		[JsonProperty]	public	int		PartialFillPercentageFilledMax;
		[JsonProperty]	public	bool	PartialFillEnabled;

		[JsonProperty]	public	int		OutOfOrderFillHappensOncePerQuoteMin;
		[JsonProperty]	public	int		OutOfOrderFillHappensOncePerQuoteMax;
		[JsonProperty]	public	int		OutOfOrderFillDeliveredXordersLaterMin;
		[JsonProperty]	public	int		OutOfOrderFillDeliveredXordersLaterMax;
		[JsonProperty]	public	bool	OutOfOrderFillEnabled;

		[JsonProperty]	public	int		PriceDeviationForMarketOrdersHappensOncePerXordersMin;
		[JsonProperty]	public	int		PriceDeviationForMarketOrdersHappensOncePerXordersMax;
		[JsonProperty]	public	int		PriceDeviationForMarketOrdersPercentageOfBestPriceMin;
		[JsonProperty]	public	int		PriceDeviationForMarketOrdersPercentageOfBestPriceMax;
		[JsonProperty]	public	bool	PriceDeviationForMarketOrdersEnabled;

		[JsonProperty]	public	int		AdaperDisconnectHappensOncePerQuoteMin;
		[JsonProperty]	public	int		AdaperDisconnectHappensOncePerQuoteMax;
		[JsonProperty]	public	int		AdaperDisconnectReconnectsAfterMillisMin;
		[JsonProperty]	public	int		AdaperDisconnectReconnectsAfterMillisMax;
		[JsonProperty]	public	bool	AdaperDisconnectEnabled;

		public LivesimBrokerSettings(Strategy strategy) : base() {
			base.Initialize(strategy);
			DelayBeforeFillEnabled					= true;
			KillPendingDelayEnabled					= true;
			OrderRejectionEnabled					= true;
			PartialFillEnabled						= true;
			OutOfOrderFillEnabled					= true;
			PriceDeviationForMarketOrdersEnabled	= true;
			AdaperDisconnectEnabled					= true;
		}
	}
}
