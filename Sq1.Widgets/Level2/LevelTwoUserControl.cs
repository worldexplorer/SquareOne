using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;
using System.Collections.Generic;

using Sq1.Core;
using Sq1.Core.Support;
using Sq1.Core.Streaming;
using Sq1.Core.DataTypes;

namespace Sq1.Widgets.Level2 {
	public partial class LevelTwoUserControl : UserControlTitled {
				StreamingAdapter		streamingAdapter;
				SymbolInfo				symbolInfo;
				string					windowTitle;

		//public	DockContentImproved		DdeMonitorForm_nullUnsafe		{ get {
		//	DockContentImproved ddeMonitorForm = this.Parent.Parent.Parent as DockContentImproved;
		//	if (ddeMonitorForm == null) {
		//		string msg = "I_NEED_THE_UPPER_LEVEL_FORM_VISIBILITY_TO_NOT_TO_REPAINT_IF_FORM_IS_MINIMIZED_OR_NOT_SHOWN";
		//		Assembler.PopupException(msg + " //LevelTwoUserControl.DdeMonitorForm", null, false);
		//	}
		//	return ddeMonitorForm;
		//} }

		//Stopwatch			stopwatchRarifyingUIupdates;

		public LevelTwoUserControl() : base() {
			InitializeComponent();
			//this.OlvLevelTwo.EmptyListMsg = "No Level2 Received Yet";
			this.olvDomCustomize();
			//this.stopwatchRarifyingUIupdates = new Stopwatch();
		}
		void layoutUserControlResizeable() {
			base.UserControlInner.Controls.Add(this.OlvLevelTwo);
			this.OlvLevelTwo.Dock = DockStyle.Fill;
		}

		public void Initialize(StreamingAdapter quikStreaming_passed, SymbolInfo symbolInfo_passed, string windowTitle_passed) {
			this.streamingAdapter	= quikStreaming_passed;
			this.symbolInfo			= symbolInfo_passed;
			this.windowTitle		= windowTitle_passed;

			//this.stopwatchRarifyingUIupdates.Start();

			if (this.symbolInfo.Level2ShowCumulativesInsteadOfLots) {
				this.olvAskCumulative	.IsVisible = true;
				this.olvAsk				.IsVisible = false;
				this.olvBid				.IsVisible = false;
				this.olvBidCumulative	.IsVisible = true;
			} else {
				this.olvAskCumulative	.IsVisible = false;
				this.olvAsk				.IsVisible = true;
				this.olvBid				.IsVisible = true;
				this.olvBidCumulative	.IsVisible = false;
			}
			this.OlvLevelTwo.RebuildColumns();

			//this.layoutUserControlResizeable();
			this.PopulateLevel2ToTitle(this.windowTitle, true);
		}
		public void PopulateLevel2ToTitle(string windowTitle_passed = null, bool ignoreTimer = false) {
			if (this.OlvLevelTwo.IsDisposed) return;
			if (base.IsDisposed) return;

			if (windowTitle_passed != null) {
				this.windowTitle = windowTitle_passed;
			}

			//if (ignoreTimer == false) {
				// WHAT_IF_BEFORE_SWITCHING_TO_GUI_THREAD?
				//if (this.stopwatchRarifyingUIupdates.ElapsedMilliseconds < this.streamingAdapter.Level2RefreshRateMs) return;
			//}

			if (base.InvokeRequired) {
				base.BeginInvoke((MethodInvoker)delegate { this.PopulateLevel2ToTitle(windowTitle_passed, ignoreTimer); });
				return;
			}
			// I paid the price of switching to GuiThread, but I don' have to worry if I already stopwatch.Restart()ed
			//if (ignoreTimer == false) {
			//	if (this.stopwatchRarifyingUIupdates.ElapsedMilliseconds < this.quikStreaming.DdeMonitorRefreshRate) return;
			//RESTART_ONLY_IN_ONE_PLACE_OTHERWIZE_THE_OTHER_NEVER_DOES_ANYTHING this.stopwatchRarifyingUIupdates.Restart();
			//}

			base.WindowTitle = this.windowTitle;
		}

		public void PopulateLevel2_asFlattenedRows_toOLV(LevelTwo level2fromDde_freezeSortFlatten_insertPriceLevels_pushToOLV_nullToClearDom) {
			if (level2fromDde_freezeSortFlatten_insertPriceLevels_pushToOLV_nullToClearDom == null) {
				string msg = "DdeTableDepth_MANUALLY_RAISED_EVENT_WITH_EMPTY_LIST_TO_CLEAR_EVERYTHING_IN_DDE_MONITOR_(QUOTES/LEVEL2/TRADES)_RIGHT_AFTER_USER_STOPPED_DDE_FEED"
					//v1 + "LEVEL2_PROXY_WASNT_CREATED_DUE_TO_A_TYPO...."
					;
				Assembler.PopupException(msg, null, false);
				//v1 return;
			}
			if (this.OlvLevelTwo.IsDisposed) return;
			if (base.IsDisposed) return;

			// WHAT_IF_BEFORE_SWITCHING_TO_GUI_THREAD?
			//if (this.stopwatchRarifyingUIupdates.ElapsedMilliseconds < this.streamingAdapter.Level2RefreshRateMs) return;
			//this.stopwatchRarifyingUIupdates.Restart();

			if (base.InvokeRequired) {
				base.BeginInvoke((MethodInvoker)delegate { this.PopulateLevel2_asFlattenedRows_toOLV(level2fromDde_freezeSortFlatten_insertPriceLevels_pushToOLV_nullToClearDom); });
				return;
			}
			// I paid the price of switching to GuiThread, but I don' have to worry if I already stopwatch.Restart()ed
			//if (this.stopwatchRarifyingUIupdates.ElapsedMilliseconds < this.quikStreaming.DdeMonitorRefreshRate) return;
			//this.stopwatchRarifyingUIupdates.Restart();

			if (level2fromDde_freezeSortFlatten_insertPriceLevels_pushToOLV_nullToClearDom == null) {
				string msg = "DdeTableDepth_MANUALLY_RAISED_EVENT_WITH_EMPTY_LIST_TO_CLEAR_EVERYTHING_IN_DDE_MONITOR_(QUOTES/LEVEL2/TRADES)_RIGHT_AFTER_USER_STOPPED_DDE_FEED";
				this.OlvLevelTwo.SetObjects(new List<LevelTwoEachLine>(), true);
				return;
			}

			List<LevelTwoEachLine> level2rows = level2fromDde_freezeSortFlatten_insertPriceLevels_pushToOLV_nullToClearDom.FrozenSortedFlattened_priceLevelsInserted_forOlv;
			if (level2rows.Count == 0) {
				string msg = "NO_LEVEL2_ROWS_TO_DISPLAY";
				Assembler.PopupException(msg, null, false);
				return;
			}
			this.OlvLevelTwo.SetObjects(level2rows, true);
		}
	}
}
