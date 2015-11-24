using System;

using Sq1.Core.Backtesting;
using Sq1.Core.Support;
using Sq1.Core.DataTypes;

namespace Sq1.Core.Livesim {
	public class LivesimLevelTwoGenerator {
		LivesimStreaming	livesimStreaming;
		SymbolInfo			symbolInfo;

		int					levelsToGenerate;
		double				stepSize;
		double				stepPrice;

		ConcurrentDictionaryGeneric<double, double> LevelTwoAsks { get { return this.livesimStreaming.StreamingDataSnapshot.LevelTwoAsks; } }
		ConcurrentDictionaryGeneric<double, double> LevelTwoBids { get { return this.livesimStreaming.StreamingDataSnapshot.LevelTwoBids; } }

		private LivesimLevelTwoGenerator() {
			levelsToGenerate = 5;
		}
		public LivesimLevelTwoGenerator(LivesimStreaming livesimStreaming) : this() {
			this.livesimStreaming = livesimStreaming;
		}
		public void Initialize(SymbolInfo symbolInfo, int levelsToGenerate, double stepPrice, double stepSize) {
			this.symbolInfo			= symbolInfo;
			this.levelsToGenerate	= levelsToGenerate;
			this.stepPrice			= stepPrice;
			this.stepSize			= stepSize;
		}
		public void GenerateAndStoreInStreamingSnap(QuoteGenerated quote) {
			this.LevelTwoBids.Clear(this, "GenerateAndStoreInStreamingSnap(" + quote + ")");
			this.LevelTwoAsks.Clear(this, "GenerateAndStoreInStreamingSnap(" + quote + ")");

			double sizeAsk = quote.Size;
			double sizeBid = quote.Size;

			double prevAsk = quote.Ask;
			double prevBid = quote.Bid;

			int randomHoleBid = (int)Math.Round((double)new Random().Next(this.levelsToGenerate));
			int randomHoleAsk = (int)Math.Round((double)new Random().Next(this.levelsToGenerate));

			for (int i = 0; i < this.levelsToGenerate; i++) {
				if (i != randomHoleAsk) this.LevelTwoAsks.Add(prevAsk, sizeAsk, this, "ask" + i + "above(" + quote + ")");
				if (i != randomHoleBid) this.LevelTwoBids.Add(prevBid, sizeBid, this, "bid" + i + "below(" + quote + ")");

				prevAsk += this.stepPrice;
				prevBid -= this.stepPrice;

				double	randomSizeIncrement = quote.Size * (new Random(6169916).Next(50, 99) / 100d);
				randomSizeIncrement = symbolInfo.AlignToVolumeStep(randomSizeIncrement);
				sizeAsk += randomSizeIncrement;

				randomSizeIncrement = quote.Size * (new Random(28).Next(5, 50) / 100d);
				randomSizeIncrement = symbolInfo.AlignToVolumeStep(randomSizeIncrement);
				sizeBid += randomSizeIncrement;
			}
		}
	}
}
