using System.Diagnostics;

using Sq1.Widgets.Level2;

using Sq1.Adapters.Quik.Streaming.Dde;

namespace Sq1.Adapters.Quik.Streaming.Monitor {
	public partial class QuikLevel2Control : LevelTwoUserControl {
		DdeTableDepth		tableLevel2;
		Stopwatch			stopwatchRarifyingUIupdates;

		public QuikLevel2Control(DdeTableDepth tableLevel2_passed, Stopwatch stopwatchRarifyingUIupdates_passed) : base() {
			this.tableLevel2 = tableLevel2_passed;
			this.stopwatchRarifyingUIupdates = stopwatchRarifyingUIupdates_passed;

			base.Initialize(tableLevel2.QuikStreaming, tableLevel2.SymbolInfo, tableLevel2.ToString());
		}

	}
}
