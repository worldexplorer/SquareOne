using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Sq1.Core.Livesim {
	public class LivesimSpoiler {
		int delayForCurrentQuotePush = 0;
		LivesimStreaming livesimStreaming;

		public LivesimSpoiler(LivesimStreaming livesimStreaming) {
			this.livesimStreaming = livesimStreaming;
		}

		public void Spoil_priorTo_PushQuoteGenerated() {
			this.delayForCurrentQuotePush = 0;
			if (this.livesimStreaming.LivesimSettings.DelayBetweenSerialQuotesEnabled) {
				delayForCurrentQuotePush = this.livesimStreaming.LivesimSettings.DelayBetweenSerialQuotesMin;
				if (this.livesimStreaming.LivesimSettings.DelayBetweenSerialQuotesMax > 0) {
					int range = Math.Abs(this.livesimStreaming.LivesimSettings.DelayBetweenSerialQuotesMax -
										 this.livesimStreaming.LivesimSettings.DelayBetweenSerialQuotesMin);
					double rnd0to1 = new Random().NextDouble();
					int rangePart = (int)Math.Round(range * rnd0to1);
					delayForCurrentQuotePush += rangePart;
				}
			}
			if (delayForCurrentQuotePush > 0) {
				this.livesimStreaming.Livesimulator.LivesimStreamingIsSleepingNow_ReportersAndExecutionHaveTimeToRebuild = true;
			}
		}

		public void Spoil_after_PushQuoteGenerated() {
			if (this.delayForCurrentQuotePush > 0) {
				//Application.DoEvents();
				Thread.Sleep(this.delayForCurrentQuotePush);
			}
			this.livesimStreaming.Livesimulator.LivesimStreamingIsSleepingNow_ReportersAndExecutionHaveTimeToRebuild = false;
		}
	}
}
