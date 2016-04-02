using Newtonsoft.Json;
using Sq1.Core.Repositories;
using Sq1.Core.StrategyBase;

namespace Sq1.Core.Livesim {
	public class LivesimStreamingSettings : StrategySelfSaver {
		[JsonProperty]	public	int		DelayBetweenSerialQuotesMin;
		[JsonProperty]	public	int		DelayBetweenSerialQuotesMax;
		[JsonProperty]	public	bool	DelayBetweenSerialQuotesEnabled;

		[JsonProperty]	public	int		OutOfOrderQuoteGenerationHappensOncePerQuoteMin;
		[JsonProperty]	public	int		OutOfOrderQuoteGenerationHappensOncePerQuoteMax;
		[JsonProperty]	public	int		OutOfOrderQuoteGenerationDelayMillisMin;
		[JsonProperty]	public	int		OutOfOrderQuoteGenerationDelayMillisMax;
		[JsonProperty]	public	bool	OutOfOrderQuoteDeliveryEnabled;

		[JsonProperty]	public	int		QuoteGenerationFreezeHappensOncePerQuoteMin;
		[JsonProperty]	public	int		QuoteGenerationFreezeHappensOncePerQuoteMax;
		[JsonProperty]	public	int		QuoteGenerationFreezeMillisMin;
		[JsonProperty]	public	int		QuoteGenerationFreezeMillisMax;
		[JsonProperty]	public	bool	QuoteGenerationFreezeEnabled;

		[JsonProperty]	public	int		AdaperDisconnectHappensOncePerQuoteMin;
		[JsonProperty]	public	int		AdaperDisconnectHappensOncePerQuoteMax;
		[JsonProperty]	public	int		AdaperDisconnectReconnectsAfterMillisMin;
		[JsonProperty]	public	int		AdaperDisconnectReconnectsAfterMillisMax;
		[JsonProperty]	public	bool	AdaperDisconnectEnabled;

		[JsonProperty]	public	bool	GenerateWideSpreadWithZeroSize;
		[JsonProperty]	public	int		LevelTwoLevelsToGenerate;

		public LivesimStreamingSettings(Strategy strategy) : base() {
			base.Initialize(strategy);
			DelayBetweenSerialQuotesEnabled = true;
			OutOfOrderQuoteDeliveryEnabled = true;
			QuoteGenerationFreezeEnabled = true;
			AdaperDisconnectEnabled = true;
			LevelTwoLevelsToGenerate = 10;
			GenerateWideSpreadWithZeroSize = false;
		}
	}
}
