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
		SaveFileDialog sfd;
		Dictionary<ToolStripMenuItem, List<OLVColumn>> columnsByFilters;
		PositionsDataSnapshot snap;
		
		IList<Position> positionsAllReversedCached;
		Dictionary<Position, double> cumulativeProfitDollar;
		Dictionary<Position, double> cumulativeProfitPercent;

		public Positions() : base() {
			this.positionsAllReversedCached = new List<Position>();
			this.cumulativeProfitDollar = new Dictionary<Position, double>();
			this.cumulativeProfitPercent = new Dictionary<Position, double>();

			base.TabText = "Positions";
			this.InitializeComponent();
			
			//VS2010 way to manage resources, autogen inside InitializeComponent()
			//System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Positions));
			//private ImageList tradeTypes;
			//this.tradeTypes = new System.Windows.Forms.ImageList(this.components);
			//this.tradeTypes.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("tradeTypes.ImageStream")));
			//this.tradeTypes.TransparentColor = System.Drawing.Color.Transparent;
			//this.tradeTypes.Images.SetKeyName(0, "45degrees3-LongEntryUnknown.png");
			//this.tradeTypes.Images.SetKeyName(1, "45degrees3-LongEntryProfit.png");
			//this.tradeTypes.Images.SetKeyName(2, "45degrees3-LongEntryLoss.png");
			//this.tradeTypes.Images.SetKeyName(3, "45degrees3-ShortEntryUnknown.png");
			//this.tradeTypes.Images.SetKeyName(4, "45degrees3-ShortEntryProfit.png");
			//this.tradeTypes.Images.SetKeyName(5, "45degrees3-ShortEntryLoss.png");
			
//			this.olvPositions.AllColumns.Add(this.olvcPosition);
//			this.olvPositions.AllColumns.Add(this.olvcSerno);
//			this.olvPositions.AllColumns.Add(this.olvcSymbol);
//			this.olvPositions.AllColumns.Add(this.olvcQuantity);
//			this.olvPositions.AllColumns.Add(this.olvcEntryDate);
//			this.olvPositions.AllColumns.Add(this.olvcEntryPrice);
//			this.olvPositions.AllColumns.Add(this.olvcEntryOrder);
//			this.olvPositions.AllColumns.Add(this.olvcExitDate);
//			this.olvPositions.AllColumns.Add(this.olvcExitPrice);
//			this.olvPositions.AllColumns.Add(this.olvcExitOrder);
//			this.olvPositions.AllColumns.Add(this.olvcProfitPct);
//			this.olvPositions.AllColumns.Add(this.olvcProfitDollar);
//			this.olvPositions.AllColumns.Add(this.olvcBarsHeld);
//			this.olvPositions.AllColumns.Add(this.olvcProfitPerBar);
//			this.olvPositions.AllColumns.Add(this.olvcEntrySignalName);
//			this.olvPositions.AllColumns.Add(this.olvcExitSignalName);
//			this.olvPositions.AllColumns.Add(this.olvcMaePct);
//			this.olvPositions.AllColumns.Add(this.olvcMfePct);
//			this.olvPositions.AllColumns.Add(this.olvcCumProfitPct);
//			this.olvPositions.AllColumns.Add(this.olvcCumProfitDollar);
//			this.olvPositions.AllColumns.Add(this.olvcComission);

			WindowsFormsUtils.SetDoubleBuffered(this.olvPositions);
			WindowsFormsUtils.SetDoubleBuffered(this);
			columnsByFilters = new Dictionary<ToolStripMenuItem, List<OLVColumn>>();
			columnsByFilters.Add(this.mniShowEntriesExits, new List<OLVColumn>() {
				this.olvcEntryDate,
				this.olvcEntryPrice,
				this.olvcEntryOrder,
				this.olvcExitDate,
				this.olvcExitPrice,
				this.olvcExitOrder
				});
			columnsByFilters.Add(this.mniShowPercentage, new List<OLVColumn>() {
				this.olvcProfitPct,
				this.olvcCumProfitPct
				});
			columnsByFilters.Add(this.mniShowBarsHeld, new List<OLVColumn>() {
				this.olvcBarsHeld,
				this.olvcProfitPerBar
				});
			columnsByFilters.Add(this.mniShowMaeMfe, new List<OLVColumn>() {
				this.olvcMaePct,
				this.olvcMfePct
				});
			columnsByFilters.Add(this.mniShowSignals, new List<OLVColumn>() {
				this.olvcEntrySignalName,
				this.olvcExitSignalName
				});
			columnsByFilters.Add(this.mniShowCommission, new List<OLVColumn>() {
				this.olvcComission
				});
			//LEFT_NULL_INTENTIONALLY_SINCE_ReportersFormManager_IS_RESPONSIBLE_TO_CALL_OUR_CreateSnapshotToStoreInScriptContext() snap = new PositionsDataSnapshot();
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
			// this.olvPositions is empty, so excessive RebuildColumns() doens't harm
			this.propagatePositionsDataSnapshotToCtxMenuItemsAndHideColumns();
			this.objectListViewCustomize();
		}
		void propagatePositionsDataSnapshotToCtxMenuItemsAndHideColumns() {
			this.mniShowEntriesExits.Checked = this.snap.ShowEntriesExits;
			this.activateMniFilter(this.columnsByFilters, this.mniShowEntriesExits);

			this.mniShowPercentage.Checked = this.snap.ShowPercentage;
			this.activateMniFilter(this.columnsByFilters, this.mniShowPercentage);

			this.mniShowBarsHeld.Checked = this.snap.ShowBarsHeld;
			this.activateMniFilter(this.columnsByFilters, this.mniShowBarsHeld);
			
			this.mniShowMaeMfe.Checked = this.snap.ShowMaeMfe;
			this.activateMniFilter(this.columnsByFilters, this.mniShowMaeMfe);

			this.mniShowSignals.Checked = this.snap.ShowSignals;
			this.activateMniFilter(this.columnsByFilters, this.mniShowSignals);

			this.mniShowCommission.Checked = this.snap.ShowCommission;
			this.activateMniFilter(this.columnsByFilters, this.mniShowCommission);

			this.mniColorify.Checked = this.snap.Colorify;
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
		
		public override void BuildOnceAfterFullBlindBacktestFinished(SystemPerformance performance) {
			base.SystemPerformance = performance;
			if (base.SystemPerformance.Bars == null) {
				Debugger.Break();
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
			foreach (Position pos in base.SystemPerformance.SlicesShortAndLong.PositionsImTrackingReadonly) {
				if (this.positionsAllReversedCached.Contains(pos) == false) {
					cumProfitDollar += pos.NetProfit;
					cumProfitPercent += pos.NetProfitPercent;
					this.cumulativeProfitDollar.Add(pos, cumProfitDollar);
					this.cumulativeProfitPercent.Add(pos, cumProfitPercent);
					this.positionsAllReversedCached.Insert(0, pos);
					continue;
				}
			}
			base.TabText = "Positions (" + this.positionsAllReversedCached.Count + ")";
			this.olvPositions.SetObjects(this.positionsAllReversedCached);
		}
		public override void BuildIncrementalAfterPositionsChangedInRealTime(ReporterPokeUnit pokeUnit) {
			int earliestIndexUpdated = -1;
			List<Position> merged = pokeUnit.PositionsOpenedClosedMergedTogether;
			foreach (Position pos in merged) {
				if (this.positionsAllReversedCached.Contains(pos) == false) {
					Position posRecent = this.positionsAllReversedCached[0];
					double cumProfitDollar1 = this.cumulativeProfitDollar[posRecent] + pos.NetProfit;
					double cumProfitPercent1 = this.cumulativeProfitPercent[posRecent] + pos.NetProfitPercent;
					this.cumulativeProfitDollar.Add(pos, cumProfitDollar1);
					this.cumulativeProfitPercent.Add(pos, cumProfitPercent1);
					this.positionsAllReversedCached.Insert(0, pos);
					continue;
				}
				int posIndex = this.positionsAllReversedCached.IndexOf(pos);
				if (posIndex == -1) {
					string msg3 = "YOU_JUST_SAID_YOU_CONTAINED_THIS_POSITION???";
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
				if (merged.Count > 0) {
					base.TabText = "Positions (" + this.positionsAllReversedCached.Count + ")";
					this.olvPositions.SetObjects(this.positionsAllReversedCached);
				}
				return;
			}
			string msg = "POSITIONS_UPDATED_SO_I_RECALCULATE_FROM_EARLIEST_INDEX_UP_TO_ZERO";
			double cumProfitDollar = 0;
			double cumProfitPercent = 0;
			for(int i = earliestIndexUpdated; i>=0; i--) {
				Position pos = this.positionsAllReversedCached[i];
				if (i == earliestIndexUpdated) {
					if (this.cumulativeProfitDollar.ContainsKey(pos)) {
						cumProfitDollar = this.cumulativeProfitDollar[pos] + pos.NetProfit;
					} else {
						string msg1 = "NONSENSE#1";
					}
					if (this.cumulativeProfitPercent.ContainsKey(pos)) {
						cumProfitPercent = this.cumulativeProfitPercent[pos] + pos.NetProfitPercent;
					} else {
						string msg2 = "NONSENSE#2";
					}
					continue;
				}
				cumProfitDollar += pos.NetProfit;
				cumProfitPercent += pos.NetProfitPercent;

				this.cumulativeProfitDollar[pos] = cumProfitDollar;
				this.cumulativeProfitPercent[pos] = cumProfitPercent;
			}

			base.TabText = "Positions (" + this.positionsAllReversedCached.Count + ")";
			this.olvPositions.SetObjects(this.positionsAllReversedCached);
		}
		string generateTextScreenshot() {
			if (this.positionsAllReversedCached.Count == 0) return "NO_POSITIONS";
			StringBuilder buffer = new StringBuilder();
			buffer.Append("Position\tSymbol\tShares\tEntry Date\tEntry Price\tExit Date\tExit Price\tProfit %\tProfit $\tBars Held\tProfit per bar\tEntry Name\tExit Name\tPriority\tMAE %\tMFE %\n");
			foreach (Position position in this.positionsAllReversedCached) {
				buffer.Append(position.PositionLongShort.ToString());
				buffer.Append("\t");
				buffer.Append(position.Bars.Symbol);
				buffer.Append("\t");
				buffer.Append(position.Shares.ToString("N0"));
				buffer.Append("\t");
				if (base.SystemPerformance.Bars.IsIntraday) {
					buffer.Append(position.EntryDate.ToShortDateString() + " " + position.EntryDate.ToShortTimeString());
					buffer.Append("\t");
				} else {
					buffer.Append(position.EntryDate.ToShortDateString());
					buffer.Append("\t");
				}
				buffer.Append(position.EntryFilledPrice.ToString("C"));
				buffer.Append("\t");
				if (position.ExitNotFilledOrStreaming) {
					buffer.Append("Open");
					buffer.Append("\t");
					buffer.Append("Open");
					buffer.Append("\t");
				} else {
					if (base.SystemPerformance.Bars.IsIntraday) {
						buffer.Append(position.ExitDate.ToShortDateString() + " " + position.ExitDate.ToShortTimeString());
						buffer.Append("\t");
					} else {
						buffer.Append(position.ExitDate.ToShortDateString());
						buffer.Append("\t");
					}
					buffer.Append(position.ExitFilledPrice.ToString("C"));
					buffer.Append("\t");
				}
				buffer.Append(position.NetProfitPercent.ToString("F2"));
				buffer.Append("\t");
				buffer.Append(position.NetProfit.ToString("C"));
				buffer.Append("\t");
				buffer.Append(position.BarsHeld.ToString("N0"));
				buffer.Append("\t");
				buffer.Append(position.ProfitPerBar.ToString("C"));
				buffer.Append("\t");
				buffer.Append(position.EntrySignal.ToString());
				buffer.Append("\t");
				if (position.ExitNotFilledOrStreaming) {
					buffer.Append("Open");
					buffer.Append("\t");
				} else {
					buffer.Append(position.ExitSignal.ToString());
					buffer.Append("\t");
				}
				//buffer.Append(position.Priority.ToString());
				//buffer.Append("\t");
				buffer.Append(position.MAEPercent.ToString("F2"));
				buffer.Append("\t");
				buffer.Append(position.MFEPercent.ToString("F2"));
				buffer.Append("\n");
			}
			return buffer.ToString();
		}
		public void SaveToFile() {
			this.sfd = new SaveFileDialog();
			this.sfd.Filter = "Text Files (*.txt)|*.txt";
			string text = this.generateTextScreenshot();
			if (this.sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
				using (StreamWriter writer = File.CreateText(this.sfd.FileName)) {
					writer.Write(text);
				}
			}
		}
	}
}