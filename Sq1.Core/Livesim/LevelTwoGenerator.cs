using System;

using Sq1.Core.Backtesting;
using Sq1.Core.Support;
using Sq1.Core.DataTypes;
using Sq1.Core.Streaming;

namespace Sq1.Core.Livesim {
	public class LevelTwoGenerator {
		string				reasonToExist_whoseLevelTwoImFilling;
		SymbolInfo			symbolInfo;
		double				stepSize;
		double				stepPrice;
		int					levelsToGenerate;

		public	LevelTwo	LevelTwo_fromStreaming		{ get; protected set; }

		public LevelTwoGenerator() {
			levelsToGenerate = 5;
		}
		public void Initialize(LevelTwo levelTwo_streamingsOwn_notQuikStreamings, SymbolInfo symbolInfo, int levelsToGenerate, string streamingAdapterInitialized_asString) {
			if (levelTwo_streamingsOwn_notQuikStreamings == null) {
				string msg = "DONT_INVOKE_ME_WITH_NULL_levelTwo //LevelTwoGenerator.Initialize(" + symbolInfo + ", " + levelsToGenerate + ")";
				Assembler.PopupException(msg);
				return;
			}
			this.LevelTwo_fromStreaming = levelTwo_streamingsOwn_notQuikStreamings;

			if (symbolInfo == null) {
				string msg = "DONT_INVOKE_ME_WITH_NULL_SYMBOL_INFO //LevelTwoGenerator.Initialize(" + symbolInfo + ", " + levelsToGenerate + ")";
				Assembler.PopupException(msg);
				return;
			}
			this.symbolInfo			= symbolInfo;
			this.stepPrice			= this.symbolInfo.PriceStep;
			this.stepSize			= this.symbolInfo.VolumeStepFromDecimal;
			this.levelsToGenerate	= levelsToGenerate;
			this.reasonToExist_whoseLevelTwoImFilling	= streamingAdapterInitialized_asString;
		}
		public void GenerateForQuote(QuoteGenerated quote, int levelsToGenerate_fromLivesimSettings, int skipPriceLevels_fromSpread = 0) {
			if (this.symbolInfo == null) {
				string msg = "DONT_INVOKE_ME_WITH_NULL_SYMBOL_INFO I_WOULD_THROW_ANYWAY_MAKING_THE_REASON_CLEAR //LevelTwoGenerator.GenerateForQuote(" + quote + ")";
				throw new Exception(msg);
			}

			string lockReason = "GenerateForQuote(" + quote + ")";
			// IM_NOT_FREEZING_LEVEL_TWO_HERE string recipient = "LIVESIM_OR_STREAMINGLIVESIMS_OWN_IMPLEMENTATION";

			try {
				this.LevelTwo_fromStreaming.WaitAndLockFor(this, lockReason);
				this.LevelTwo_fromStreaming.Clear_LevelTwo(this, lockReason);

				double sizeAsk = quote.Size;
				double sizeBid = quote.Size;

				double prevAsk = quote.Ask;
				double prevBid = quote.Bid;

				int randomHoleBid = (int)Math.Round((double)new Random().Next(levelsToGenerate_fromLivesimSettings));		//this.levelsToGenerate));
				int randomHoleAsk = (int)Math.Round((double)new Random().Next(levelsToGenerate_fromLivesimSettings));		//this.levelsToGenerate));

				//for (int i = 0; i < this.levelsToGenerate; i++) {
				for (int i = 0; i < levelsToGenerate_fromLivesimSettings; i++) {
					if (i >= skipPriceLevels_fromSpread) {
						if (i != randomHoleAsk) this.LevelTwo_fromStreaming.AddAsk(prevAsk, sizeAsk, this, "ask" + i + "above(" + quote + ")");
						if (i != randomHoleBid) this.LevelTwo_fromStreaming.AddBid(prevBid, sizeBid, this, "bid" + i + "below(" + quote + ")");
					}

					prevAsk += this.stepPrice;
					prevBid -= this.stepPrice;

					double	randomSizeIncrement = quote.Size * (new Random(6169916).Next(50, 99) / 100d);
					randomSizeIncrement = symbolInfo.AlignToVolumeStep(randomSizeIncrement);
					sizeAsk += randomSizeIncrement;

					randomSizeIncrement = quote.Size * (new Random(28).Next(5, 50) / 100d);
					randomSizeIncrement = symbolInfo.AlignToVolumeStep(randomSizeIncrement);
					sizeBid += randomSizeIncrement;
				}
			} catch (Exception ex) {
				string msg = "MUST_NEVER_HAPPEN_BUT_IM_HERE_TO_UNLOCK_IN_FINALLY";
				Assembler.PopupException(msg, ex);
			} finally {
				this.LevelTwo_fromStreaming.UnLockFor(this, lockReason);
			}
		}
	}
}
