using System.Diagnostics;

using Sq1.Widgets.Level2;

using Sq1.Adapters.Quik.Streaming.Dde;

namespace Sq1.Adapters.Quik.Streaming.Monitor {
	public partial class QuikLevel2Control : LevelTwoUserControl {
		DdeTableDepth		ddeTableDepth;
		Stopwatch			stopwatchRarifyingUIupdates;

		public QuikLevel2Control(DdeTableDepth ddeTableDepth_passed, Stopwatch stopwatchRarifyingUIupdates_passed) : base() {
			this.ddeTableDepth = ddeTableDepth_passed;
			this.stopwatchRarifyingUIupdates = stopwatchRarifyingUIupdates_passed;

			base.Initialize(ddeTableDepth.QuikStreaming, ddeTableDepth.SymbolInfo, ddeTableDepth.ToString());
		}

	}
}
