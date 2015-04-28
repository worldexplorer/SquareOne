using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using BrightIdeasSoftware;
using Sq1.Core;
using Sq1.Core.Charting;
using Sq1.Core.Execution;
using Sq1.Core.StrategyBase;
using Sq1.Support;
using Sq1.Core.Support;

namespace Sq1.Reporters {
	public partial class Positions : Reporter {
		PositionsDataSnapshot	snap;
		IList<Position>			positionsAllReversedCached;

		public Positions() : base() {
			this.positionsAllReversedCached = new List<Position>();

			base.TabText = "Positions";
			this.InitializeComponent();

			//LEFT_NULL_INTENTIONALLY_SINCE_ReportersFormManager_IS_RESPONSIBLE_TO_CALL_OUR_CreateSnapshotToStoreInScriptContext() snap = new PositionsDataSnapshot();
			WindowsFormsUtils.SetDoubleBuffered(this.olvPositions);
			WindowsFormsUtils.SetDoubleBuffered(this);
		}
		public override object CreateSnapshotToStoreInScriptContext() {
			return new PositionsDataSnapshot();
		}
		public override void Initialize(ChartShadow chart, object reportersOwnDataSnapshotInOut, SystemPerformance performance) {
			base.Initialize(chart, reportersOwnDataSnapshotInOut, performance);
			PositionsDataSnapshot snapCasted = reportersOwnDataSnapshotInOut as PositionsDataSnapshot;
			if (snapCasted == null) {
				string msg = "INVOKER_MUST_HAVE_CALLED_MY_CREATE_SNAPSHOT_AND_REGISTERED_IT_IN_SCRIPT_CONTEXT invokerAssumed=ReportersFormManager.ReporterActivateShowRegisterMniTick()<FindOrCreateReportersSnapshot()";
				Assembler.PopupException(msg);
				return;	// this.snap already initialized in ctor();
			}
			this.snap = snapCasted;
			this.mniColorify.Checked = this.snap.Colorify;
			this.objectListViewCustomize();
		}
		void activateMniFilter(Dictionary<ToolStripMenuItem, List<OLVColumn>> columnsByFilters, ToolStripMenuItem mni) {
			if (columnsByFilters.ContainsKey(mni) == false) {
				string msg = "Add ToolStripMenuItem[" + mni.Name + "] into columnsByFilters";
				throw new Exception(msg);
				//log.Fatal(msg);
				//return;
			}
			bool newCheckedState = mni.Checked;
			List<OLVColumn> columns = columnsByFilters[mni];
			if (columns.Count == 0) return;
			foreach (OLVColumn column in columns) {
				if (column.IsVisible == newCheckedState) continue;
				column.IsVisible = newCheckedState;
			}
			this.olvPositions.RebuildColumns();
		}
		public override void BuildFullOnBacktestFinished() {
			if (base.SystemPerformance.Bars == null) {
				string msg = "REPORTERS.POSITIONS_CANT_PROCESS_SYSTEM_PERFORMANCE=NULL";
				Assembler.PopupException(msg);
				return;
			}
			if (base.SystemPerformance.Bars.IsIntraday) {
				this.olvcEntryDate.Width = 120;
				this.olvcExitDate.Width = 120;
			} else {
				this.olvcEntryDate.Width = 80;
				this.olvcExitDate.Width = 80;
			}

			this.positionsAllReversedCached.Clear();
			SystemPerformanceSlice both = base.SystemPerformance.SlicesShortAndLong;
			foreach (Position pos in both.PositionsImTrackingReadonly) {
				if (this.positionsAllReversedCached.Contains(pos)) {
					string msg = "CUMULATIVES_ALREADY_CALCULATED_FOR_THIS_POSITION SystemPerformanceSlice["
						+ both + "].PositionsImTracking contains duplicate " + pos;
					Assembler.PopupException(msg);
					continue;
				}
				this.positionsAllReversedCached.Insert(0, pos);
			}
			this.RebuildingFullReportForced_onLivesimPaused();
		}
		public override void RebuildingFullReportForced_onLivesimPaused() {
			try {
				//DOESNT_MAKE_SENSE  this.olvPositions.SuspendLayout();
				this.olvPositions.SetObjects(this.positionsAllReversedCached, false);
				//TOO_MUCH__CLEARS_ITEMS_COLUMNS__ADDS_RANGE__SORTS___UPDATES_FILTERING this.olvPositions.RebuildColumns();
				//CLEARS_ADDS_RANGE_SORTS__ALREADY_INVOKED_FROM_SetObjects() this.olvPositions.BuildList(false);
				//DOESNT_MAKE_SENSE this.olvPositions.Invalidate();

				//v1 base.TabText = "Positions (" + this.positionsAllReversedCached.Count + ")";
				//v2 this.StashWindowTextSuffix();
				this.StashWindowTextSuffixInBaseTabText_usefulToUpdateAutohiddenStatsWithoutRebuildingFullReport_OLVisSlow();
			} catch (Exception ex) {
				string msg = "I_KNEW_OLV_WILL_REFUSE_TO_REFRESH_POSITIONS_IT_DOESNT_HAVE";
				Assembler.PopupException(msg);
				//DOESNT_MAKE_SENSE } finally {
				//DOESNT_MAKE_SENSE  	this.olvPositions.ResumeLayout();
			}
		}
		public override void StashWindowTextSuffixInBaseTabText_usefulToUpdateAutohiddenStatsWithoutRebuildingFullReport_OLVisSlow() {
			base.TabText = "Positions (" + this.positionsAllReversedCached.Count + ")";
		}
		public override void BuildIncrementalOnBrokerFilledAlertsOpeningForPositions_step1of3(ReporterPokeUnit pokeUnit) {
			//v1
			List<Position> safeCopy = pokeUnit.PositionsOpened.SafeCopy(this, "BuildIncrementalOnBrokerFilledAlertsOpeningForPositions_step1of3(WAIT)");
			//v2
			#if DEBUG
			PositionList positionsOpenedSafeCopy = pokeUnit.PositionsOpened.Clone(this, "BuildIncrementalOnBrokerFilledAlertsOpeningForPositions_step1of3(WAIT)");
			#endif
			//List<Position> safeCopy = positionsOpenedSafeCopy.SafeCopy(this, "BuildIncrementalOnBrokerFilledAlertsOpeningForPositions_step1of3(WAIT)");
			foreach (Position pos in safeCopy) {
				if (this.positionsAllReversedCached.Contains(pos)) {
					string msg3 = "REPORTERS.POSITIONS_ALREADY_ADDED_BuildIncrementalOnBrokerFilledAlertsOpeningForPositions_step1of3()";
					Assembler.PopupException(msg3);
					continue;
				}
				this.positionsAllReversedCached.Insert(0, pos);
				
				//v2 ACCELERATING_ON_POSITION_FILLED copypaste from BuildList()
				//this.olvPositions.BeginUpdate();
				//try {
				//	OLVListItem lvi = new OLVListItem(pos);
				//	this.olvPositions.Items.Insert(0, lvi);
				//} finally {
				//	this.olvPositions.EndUpdate();
				//}
			}
			this.StashWindowTextSuffixInBaseTabText_usefulToUpdateAutohiddenStatsWithoutRebuildingFullReport_OLVisSlow();
			// MINIMIZED_IF_STREAMING_QUOTE_DELAY<100ms_UNMINIMIZE_AND_WATCH__REALTIME_UNAFFECTED
			////v1
			//bool livesimStreamingIsSleeping = pokeUnit.PositionsOpened.AlertsEntry.GuiHasTimeToRebuild(this, "Reporters.Positions.BuildIncrementalOnBrokerFilledAlertsOpeningForPositions_step1of3(WAIT)");
			//#if DEBUG
			////v2
			//bool livesimStreamingIsSleeping2 = positionsOpenedSafeCopy.AlertsEntry.GuiHasTimeToRebuild(this, "Reporters.Positions.BuildIncrementalOnBrokerFilledAlertsOpeningForPositions_step1of3(WAIT)");
			//if (livesimStreamingIsSleeping != livesimStreamingIsSleeping2) {
			//    string msg = "NONSENSE";
			//    //Assembler.PopupException(msg);
			//}
			//#endif
			//if (livesimStreamingIsSleeping == false) {
			//    return;
			//}
			if (base.Visible == false) return;		//DockContent is minimized / "autohidden"
			//v1 this.rebuildOLVproperly();
			//DOESNT_INSERT__REPLACES_EXISTING_LISTVIEW_ITEMS_IF_FOUND_IN_MODEL this.olvPositions.RefreshObjects(safeCopy);
			//v3
            this.olvPositions.InsertObjects(0, safeCopy);
		}
		public override void BuildIncrementalOnPositionsOpenedClosed_step3of3(ReporterPokeUnit pokeUnit) {
			List<Position> positionsUpdatedDueToStreamingNewQuote = pokeUnit.PositionsClosed.SafeCopy(this, "BuildIncrementalOnPositionsOpenedClosed_step3of3(WAIT)");

			//v1 NO_NEED_TO_UPDATE_STASHED_POSITION_COUNTER_IT_DIDNT_CHANGE_ON_step2of3
			//this.StashWindowTextSuffixInBaseTabText();
 			//bool livesimStreamingIsSleeping = pokeUnit.PositionsOpenNow.AlertsOpenNow.GuiHasTimeToRebuild(this, "Reporters.Positions.BuildIncrementalOnPositionsOpenedClosed_step3of3(WAIT)");
 			//if (livesimStreamingIsSleeping == false) return;

			if (base.Visible == false) return;		//DockContent is minimized / "autohidden"
			// CORE_HAS_NO_IDEA_ABOUT_WIDGETS__ASK_WINDOWS_FORMS_IF_IM_VISIBLE if (base..... ReporterFormWrapper.ParentDockContentIsCoveredOrAutoHidden)
			// NOPE_SOMETIMES_CLOSING_PRICE_NOT_SHOWN ALREADY_REFRESHED_DURING_STEP2
			this.olvPositions.RefreshObjects(positionsUpdatedDueToStreamingNewQuote);
		}
		public override void BuildIncrementalUpdateOpenPositionsDueToStreamingNewQuote_step2of3(ReporterPokeUnit pokeUnit) {
			List<Position> positionsUpdatedDueToStreamingNewQuote = pokeUnit.PositionsOpenNow.SafeCopy(this, "BuildIncrementalUpdateOpenPositionsDueToStreamingNewQuote_step2of3(WAIT)");

			//v1 NO_NEED_TO_UPDATE_STASHED_POSITION_COUNTER_IT_DIDNT_CHANGE_ON_step2of3
			//this.StashWindowTextSuffixInBaseTabText();
			//bool livesimStreamingIsSleeping = pokeUnit.PositionsOpenNow.AlertsOpenNow.GuiHasTimeToRebuild(this, "Reporters.Positions.BuildIncrementalUpdateOpenPositionsDueToStreamingNewQuote_step2of3(WAIT)");
 			//if (livesimStreamingIsSleeping == false) return;

			if (base.Visible == false) return;		//DockContent is minimized / "autohidden"
			//v1 SLOW? this.rebuildOLVproperly();
			//v2 hoping ObjectListView uses Dictionary to locate changed positions
			this.olvPositions.RefreshObjects(positionsUpdatedDueToStreamingNewQuote);
		}
		string generateTextScreenshot() {
			if (this.positionsAllReversedCached.Count == 0) return "NO_POSITIONS";
			StringBuilder sb = new StringBuilder();
			sb.Append("#\tPosition\tSymbol\tShares\tEntry Date\tEntry Price\tExit Date\tExit Price\tProfit %\tProfit $\tBars Held\tProfit per bar\tEntry Name\tExit Name\tMAE %\tMFE %\tCumulProfit$\tCumulProfit%\n");
			foreach (Position position in this.positionsAllReversedCached) {
				sb.Append(position.SernoAbs);
				sb.Append("\t");
				sb.Append(position.PositionLongShort.ToString());
				sb.Append("\t");
				sb.Append(position.Bars.Symbol);
				sb.Append("\t");
				sb.Append(position.Shares.ToString("N0"));
				sb.Append("\t");
				if (base.SystemPerformance.Bars.IsIntraday) {
					sb.Append(position.EntryDateBarTimeOpen.ToShortDateString() + " " + position.EntryDateBarTimeOpen.ToShortTimeString());
					sb.Append("\t");
				} else {
					sb.Append(position.EntryDateBarTimeOpen.ToShortDateString());
					sb.Append("\t");
				}
				sb.Append(position.EntryFilledPrice.ToString(base.FormatPrice));
				sb.Append("\t");
				if (position.ExitNotFilledOrStreaming) {
					sb.Append("Open");
					sb.Append("\t");
					sb.Append("Open");
					sb.Append("\t");
				} else {
					if (base.SystemPerformance.Bars.IsIntraday) {
						sb.Append(position.ExitDateBarTimeOpen.ToShortDateString() + " " + position.ExitDateBarTimeOpen.ToShortTimeString());
						sb.Append("\t");
					} else {
						sb.Append(position.ExitDateBarTimeOpen.ToShortDateString());
						sb.Append("\t");
					}
					sb.Append(position.ExitFilledPrice.ToString(base.FormatPrice));
					sb.Append("\t");
				}
				sb.Append(position.NetProfitPercent.ToString("F2"));
				sb.Append("\t");
				sb.Append(position.NetProfit.ToString());
				sb.Append("\t");
				sb.Append(position.BarsHeld.ToString("N0"));
				sb.Append("\t");
				sb.Append(position.ProfitPerBar.ToString("F2"));
				sb.Append("\t");
				sb.Append(position.EntrySignal.ToString());
				sb.Append("\t");
				if (position.ExitNotFilledOrStreaming) {
					sb.Append("Open");
					sb.Append("\t");
				} else {
					sb.Append(position.ExitSignal.ToString());
					sb.Append("\t");
				}
				sb.Append(position.MAEPercent.ToString("F2"));
				sb.Append("\t");
				sb.Append(position.MFEPercent.ToString("F2"));
				sb.Append("\t");
				sb.Append(this.SystemPerformance.SlicesShortAndLong.CumulativeNetProfitForPosition(position).ToString(base.FormatPrice));
				sb.Append("\t");
				sb.Append(this.SystemPerformance.SlicesShortAndLong.CumulativeNetProfitPercentForPosition(position).ToString("F2"));
				sb.Append("\n");
			}
			return sb.ToString();
		}
		void olvBinaryStateRestore() {
			try {
				// #1/2 OBJECTLISTVIEW_HACK__SEQUENCE_MATTERS!!!! otherwize RestoreState() doesn't restore after restart
				// adds columns to filter in the header (right click - unselect garbage columns); there might be some BrightIdeasSoftware.SyncColumnsToAllColumns()?...
				List<OLVColumn> allColumnsOtherwizeEmptyListAndRowFormatException = new List<OLVColumn>();
				foreach (ColumnHeader columnHeader in this.olvPositions.Columns) {
					OLVColumn oLVColumn = columnHeader as OLVColumn;
					if (oLVColumn == null) continue;
					oLVColumn.VisibilityChanged += oLVColumn_VisibilityChanged;
					//THROWS_ADDING_ALL_REGARDLESS_AFTER_OrdersTreeOLV.RestoreState(base64Decoded)_ADDED_FILTER_IN_OUTER_LOOP 
					if (this.olvPositions.AllColumns.Contains(oLVColumn)) continue;
					allColumnsOtherwizeEmptyListAndRowFormatException.Add(oLVColumn);
				}
				if (allColumnsOtherwizeEmptyListAndRowFormatException.Count > 0) {
					//THROWS_ADDING_ALL_REGARDLESS_AFTER_OrdersTreeOLV.RestoreState(base64Decoded)_ADDED_FILTER_IN_OUTER_LOOP 
					this.olvPositions.AllColumns.AddRange(allColumnsOtherwizeEmptyListAndRowFormatException);
				}
				// #2/2 OBJECTLISTVIEW_HACK__SEQUENCE_MATTERS!!!! otherwize RestoreState() doesn't restore after restart
				if (this.snap.PositionsOlvStateBase64.Length > 0) {
					byte[] olvStateBinary = ObjectListViewStateSerializer.Base64Decode(this.snap.PositionsOlvStateBase64);
					this.olvPositions.RestoreState(olvStateBinary);
				}
			} catch (Exception ex) {
				string msg = "this.olvPositions.RestoreState(olvStateBinary)";
				Assembler.PopupException(msg, ex);
			}
		}
		void olvBinaryStateSaveRaiseStrategySerialize() {
			byte[] olvStateBinary = this.olvPositions.SaveState();
			this.snap.PositionsOlvStateBase64 = ObjectListViewStateSerializer.Base64Encode(olvStateBinary);
			if (Assembler.InstanceInitialized.MainFormDockFormsFullyDeserializedLayoutComplete == false) return;
			base.RaiseContextScriptChangedContainerShouldSerialize();
		}
	}
}