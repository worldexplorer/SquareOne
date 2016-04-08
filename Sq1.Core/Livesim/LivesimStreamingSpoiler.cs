using System;
using System.Threading;

namespace Sq1.Core.Livesim {
	public class LivesimStreamingSpoiler {
		int delayForCurrentQuotePush = 0;
		LivesimStreaming livesimStreaming;

		public LivesimStreamingSpoiler(LivesimStreaming livesimStreaming) {
			this.livesimStreaming = livesimStreaming;
		}

		public void Spoil_priorTo_PushQuoteGenerated() {
			this.delayForCurrentQuotePush = 0;
			if (this.livesimStreaming.LivesimStreamingSettings.DelayBetweenSerialQuotesEnabled) {
				this.delayForCurrentQuotePush = this.livesimStreaming.LivesimStreamingSettings.DelayBetweenSerialQuotesMin;

				if (this.livesimStreaming.LivesimStreamingSettings.DelayBetweenSerialQuotesMax > 0) {
					int range = Math.Abs(this.livesimStreaming.LivesimStreamingSettings.DelayBetweenSerialQuotesMax -
										 this.livesimStreaming.LivesimStreamingSettings.DelayBetweenSerialQuotesMin);
					double rnd0to1 = new Random().NextDouble();
					int rangePart = (int)Math.Round(range * rnd0to1);
					this.delayForCurrentQuotePush += rangePart;
				}
			}
			if (this.delayForCurrentQuotePush > 0) {
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
