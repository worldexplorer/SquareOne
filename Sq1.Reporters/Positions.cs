using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

		//cumulativeProfitDollar,Percent+CalcEachStreamingUpdate should be moved into SystemPerformanceSlice for other Reporters to be used in their drawings
		Dictionary<Position, double> cumulativeProfitDollar;
		Dictionary<Position, double> cumulativeProfitPercent;

		public Positions() : base() {
			this.positionsAllReversedCached = new List<Position>();
			this.cumulativeProfitDollar		= new Dictionary<Position, double>();
			this.cumulativeProfitPercent	= new Dictionary<Position, double>();

			base.TabText = "Positions";
			this.InitializeComponent();

			//LEFT_NULL_INTENTIONALLY_SINCE_ReportersFormManager_IS_RESPONSIBLE_TO_CALL_OUR_CreateSnapshotToStoreInScriptContext() snap = new PositionsDataSnapshot();
			WindowsFormsUtils.SetDoubleBuffered(this.olvPositions);
			WindowsFormsUtils.SetDoubleBuffered(this);
		}
		public override object CreateSnapshotToStoreInScriptContext() {
			return new PositionsDataSnapshot();
		}
		public override void Initialize(ChartShadow chart, object reportersOwnDataSnapshotInOut) {
			base.Initialize(chart, reportersOwnDataSnapshotInOut);
			PositionsDataSnapshot snapCasted = reportersOwnDataSnapshotInOut as PositionsDataSnapshot;
			if (snapCasted == null) {
				string msg = "INVOKER_MUST_HAVE_CALLED_MY_CREATE_SNAPSHOT_AND_REGISTERED_IT_IN_SCRIPT_CONTEXT invokerAssumed=ReportersFormManager.ReporterActivateShowRegisterMniTick()<FindOrCreateReportersSnapshot()";
				Assembler.PopupException(msg);
				Debugger.Break();
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
		public override void BuildFullOnBacktestFinished(SystemPerformance performance) {
			base.SystemPerformance = performance;
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

			double cumProfitDollar = 0;			//void reversePositionsCalcCumulativesDumpToTitleAndOLV() {
			double cumProfitPercent = 0;
			this.cumulativeProfitDollar.Clear();
			this.cumulativeProfitPercent.Clear();
			this.positionsAllReversedCached.Clear();
			SystemPerformanceSlice both = base.SystemPerformance.SlicesShortAndLong;
			foreach (Position pos in both.PositionsImTrackingReadonly) {
				if (this.positionsAllReversedCached.Contains(pos)) {
					string msg = "CUMULATIVES_ALREADY_CALCULATED_FOR_THIS_POSITION SystemPerformanceSlice["
						+ both + "].PositionsImTracking contains duplicate " + pos;
					Assembler.PopupException(msg);
					continue;
				}
				cumProfitDollar += pos.NetProfit;
				cumProfitPercent += pos.NetProfitPercent;
				this.cumulativeProfitDollar.Add(pos, cumProfitDollar);
				this.cumulativeProfitPercent.Add(pos, cumProfitPercent);
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
			foreach (Position pos in pokeUnit.PositionsOpened) {
				if (this.positionsAllReversedCached.Contains(pos)) {
					string msg3 = "REPORTERS.POSITIONS_ALREADY_ADDED_BuildIncrementalOnBrokerFilledAlertsOpeningForPositions_step1of3()";
					Assembler.PopupException(msg3);
					continue;
				}
				double cumProfitDollar1 = 0;
				double cumProfitPercent1 = 0;
				this.cumulativeProfitDollar.Add(pos, cumProfitDollar1);
				this.cumulativeProfitPercent.Add(pos, cumProfitPercent1);
				this.positionsAllReversedCached.Insert(0, pos);
			}
			this.rebuildOLVproperly();
		}
		public override void BuildIncrementalOnPositionsOpenedClosed_step3of3(ReporterPokeUnit pokeUnit) {
			List<Position> positionsUpdatedDueToStreamingNewQuote = pokeUnit.PositionsClosed;
		}
		public override void BuildIncrementalUpdateOpenPositionsDueToStreamingNewQuote_step2of3(List<Position> positionsUpdatedDueToStreamingNewQuote) {
			int earliestIndexUpdated = -1;
			foreach (Position pos in positionsUpdatedDueToStreamingNewQuote) {
				int posIndex = this.positionsAllReversedCached.IndexOf(pos);
				if (posIndex == -1) {
					string msg3 = "YOU_DIDNT_INVOKE_BuildIncrementalOnBrokerFilledAlertsOpeningForPositions_step1of3_WITH_THIS_POSITION???";
					Assembler.PopupException(msg3);
				}
				if (earliestIndexUpdated == -1) {
					earliestIndexUpdated = posIndex;
					continue;
				}
				if (earliestIndexUpdated >= posIndex) continue;
				earliestIndexUpdated = posIndex;
			}
			if (earliestIndexUpdated == -1) {
				string msg2 = "NO_POSITIONS_UPDATED_NO_NEED_TO_RECALCULATE_ANYTHING";
				if (positionsUpdatedDueToStreamingNewQuote.Count > 0) {
					this.rebuildOLVproperly();
				}
				return;
			}
			string msg = "POSITIONS_UPDATED_SO_I_RECALCULATE_FROM_EARLIEST_INDEX_UP_TO_ZERO";
			double cumProfitDollar = 0;
			double cumProfitPercent = 0;

			for (int i = earliestIndexUpdated; i >= 0; i--) {
				Position pos = this.positionsAllReversedCached[i];
				if (i == earliestIndexUpdated && i < this.positionsAllReversedCached.Count) {
					Position posPrev = this.positionsAllReversedCached[i+1];
					if (this.cumulativeProfitDollar.ContainsKey(posPrev)) {
						cumProfitDollar = this.cumulativeProfitDollar[posPrev];
					} else {
						string msg1 = "REPORTERS.POSITIONS_NONSENSE#1";
						Assembler.PopupException(msg1);
					}
					if (this.cumulativeProfitPercent.ContainsKey(posPrev)) {
						cumProfitPercent = this.cumulativeProfitPercent[posPrev];
					} else {
						string msg1 = "REPORTERS.POSITIONS_NONSENSE#2";
						Assembler.PopupException(msg1);
					}
				}
				cumProfitDollar += pos.NetProfit;
				cumProfitPercent += pos.NetProfitPercent;

				double oldValue = this.cumulativeProfitDollar[pos];
				this.cumulativeProfitDollar[pos] = cumProfitDollar;
				this.cumulativeProfitPercent[pos] = cumProfitPercent;

				double newValue = this.cumulativeProfitDollar[pos];
				double difference = newValue - oldValue;
				if (difference == 0) {
					string msg2 = "DID_YOU_GET_QUOTE_WITH_SAME_BID_ASK????__YOU_SHOULDVE_IGNORED_IT_IN_STREAMING_PROVIDER";
					Assembler.PopupException(msg2, null, false);
				}
			}
			//v1 SLOW? this.rebuildOLVproperly();
			//v2 hoping ObjectListView uses Dictionary to locate changed positions
			this.olvPositions.RefreshObjects(positionsUpdatedDueToStreamingNewQuote);
		}
		string generateTextScreenshot() {
			if (this.positionsAllReversedCached.Count == 0) return "NO_POSITIONS";
			StringBuilder sb = new StringBuilder();
			sb.Append("Position\tSymbol\tShares\tEntry Date\tEntry Price\tExit Date\tExit Price\tProfit %\tProfit $\tBars Held\tProfit per bar\tEntry Name\tExit Name\tMAE %\tMFE %\n");
			foreach (Position position in this.positionsAllReversedCached) {
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
				sb.Append(position.EntryFilledPrice.ToString("C"));
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
					sb.Append(position.ExitFilledPrice.ToString("C"));
					sb.Append("\t");
				}
				sb.Append(position.NetProfitPercent.ToString("F2"));
				sb.Append("\t");
				sb.Append(position.NetProfit.ToString("C"));
				sb.Append("\t");
				sb.Append(position.BarsHeld.ToString("N0"));
				sb.Append("\t");
				sb.Append(position.ProfitPerBar.ToString("C"));
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
				sb.Append("\n");
			}
			return sb.ToString();
		}
	}
}