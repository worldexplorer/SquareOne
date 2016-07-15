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

		[JsonProperty]	public	int		TransactionStatusAfterOrderStatusDelayAfterFillMin;
		[JsonProperty]	public	int		TransactionStatusAfterOrderStatusDelayAfterFillMax;
		[JsonProperty]	public	int		TransactionStatusAfterOrderStatusHappensOncePerOrdersMin;
		[JsonProperty]	public	int		TransactionStatusAfterOrderStatusHappensOncePerOrdersMax;
		[JsonProperty]	public	bool	TransactionStatusAfterOrderStatusEnabled;
		
		[JsonProperty]	public	int		KillerTransactionCallbackAfterVictimFilledHappensOncePerKillersMin;
		[JsonProperty]	public	int		KillerTransactionCallbackAfterVictimFilledHappensOncePerKillersMax;
		[JsonProperty]	public	int		KillerTransactionCallbackAfterVictimFilledDelayMax;
		[JsonProperty]	public	int		KillerTransactionCallbackAfterVictimFilledDelayMin;
		[JsonProperty]	public	bool	KillerTransactionCallbackAfterVictimFilledEnabled;

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

		[JsonProperty]	public	int		AdapterDisconnectHappensOncePerOrderMin;
		[JsonProperty]	public	int		AdapterDisconnectHappensOncePerOrderMax;
		[JsonProperty]	public	int		AdapterDisconnectReconnectsAfterMillisMin;
		[JsonProperty]	public	int		AdapterDisconnectReconnectsAfterMillisMax;
		[JsonProperty]	public	bool	AdapterDisconnectEnabled;

		[JsonProperty]	public	int		BrokerDeniedSubmission_HappensOncePerXorders_Min;
		[JsonProperty]	public	int		BrokerDeniedSubmission_HappensOncePerXorders_Max;
		[JsonProperty]	public	bool	BrokerDeniedSubmission_injectionEnabled;

		[JsonProperty]	public	bool	ClearExecutionExceptions_beforeLivesim;

		public LivesimBrokerSettings(Strategy strategy) : base() {
			base.Initialize(strategy);
			DelayBeforeFillEnabled						= true;
			KillPendingDelayEnabled						= true;
			OrderRejectionEnabled						= true;
			TransactionStatusAfterOrderStatusEnabled	= true;
			KillerTransactionCallbackAfterVictimFilledEnabled	= true;
			PartialFillEnabled							= true;
			OutOfOrderFillEnabled						= true;
			PriceDeviationForMarketOrdersEnabled		= true;
			AdapterDisconnectEnabled					= true;
			BrokerDeniedSubmission_injectionEnabled		= true;
			ClearExecutionExceptions_beforeLivesim		= false;
		}
	}
}
