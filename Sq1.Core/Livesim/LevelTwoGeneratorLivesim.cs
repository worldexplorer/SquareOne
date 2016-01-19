using System;

namespace Sq1.Core.Livesim {
	public class LevelTwoGeneratorLivesim : LevelTwoGenerator {
		LivesimStreaming	livesimStreaming;

		public LevelTwoGeneratorLivesim(LivesimStreaming livesimStreaming) : base() {
			this.livesimStreaming = livesimStreaming;	// just to have its Name
		}
		public void InitializeLevelTwo(string symbolLivesimming) {
			base.LevelTwoAsks = this.livesimStreaming.StreamingDataSnapshot.LevelTwoAsks_getForSymbol(symbolLivesimming);
			base.LevelTwoBids = this.livesimStreaming.StreamingDataSnapshot.LevelTwoBids_getForSymbol(symbolLivesimming);
		}
	}
}
