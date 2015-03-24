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

namespace Sq1.Reporters {
	public partial class Positions : Reporter {
		PositionsDataSnapshot snap;
		
		IList<Position> positionsAllReversedCached;

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
			this.rebuildOLVproperly();
		}
		void rebuildOLVproperly() {
			try {
				this.olvPositions.SetObjects(this.positionsAllReversedCached);
				this.olvPositions.RebuildColumns();
				this.olvPositions.BuildList();
				base.TabText = "Positions (" + this.positionsAllReversedCached.Count + ")";
			} catch (Exception ex) {
				string msg = "I_KNEW_OLV_WILL_REFUSE_TO_REFRESH_POSITIONS_IT_DOESNT_HAVE";
				Assembler.PopupException(msg);
			}
		}
		public override void BuildIncrementalOnBrokerFilledAlertsOpeningForPositions_step1of3(ReporterPokeUnit pokeUnit) {
			List<Position> safeCopy = pokeUnit.PositionsOpened.SafeCopy(this, "BuildIncrementalOnBrokerFilledAlertsOpeningForPositions_step1of3(WAIT)");
			foreach (Position pos in safeCopy) {
				if (this.positionsAllReversedCached.Contains(pos)) {
					string msg3 = "REPORTERS.POSITIONS_ALREADY_ADDED_BuildIncrementalOnBrokerFilledAlertsOpeningForPositions_step1of3()";
					Assembler.PopupException(msg3);
					continue;
				}
				this.positionsAllReversedCached.Insert(0, pos);
			}
			this.rebuildOLVproperly();
		}
		public override void BuildIncrementalOnPositionsOpenedClosed_step3of3(ReporterPokeUnit pokeUnit) {
			List<Position> positionsUpdatedDueToStreamingNewQuote = pokeUnit.PositionsClosed.SafeCopy(this, "BuildIncrementalOnPositionsOpenedClosed_step3of3(WAIT)");
			// NOPE_SOMETIMES_CLOSING_PRICE_NOT_SHOWN ALREADY_REFRESHED_DURING_STEP2
			this.olvPositions.RefreshObjects(positionsUpdatedDueToStreamingNewQuote);
		}
		public override void BuildIncrementalUpdateOpenPositionsDueToStreamingNewQuote_step2of3(ReporterPokeUnit pokeUnit) {
			List<Position> positionsUpdatedDueToStreamingNewQuote = pokeUnit.PositionsOpenNow.SafeCopy(this, "BuildIncrementalUpdateOpenPositionsDueToStreamingNewQuote_step2of3(WAIT)");
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
					sb.Append(position.EntryDate.ToShortDateString() + " " + position.EntryDate.ToShortTimeString());
					sb.Append("\t");
				} else {
					sb.Append(position.EntryDate.ToShortDateString());
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
						sb.Append(position.ExitDate.ToShortDateString() + " " + position.ExitDate.ToShortTimeString());
						sb.Append("\t");
					} else {
						sb.Append(position.ExitDate.ToShortDateString());
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
	}
}