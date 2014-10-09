using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Core.Charting;
using Sq1.Core.Execution;
using Sq1.Core.Repositories;
using Sq1.Core.StrategyBase;
using Sq1.Gui.Forms;
using Sq1.Widgets;
using WeifenLuo.WinFormsUI.Docking;

namespace Sq1.Gui.ReportersSupport {
	public class ReportersFormsManager {
		public readonly ChartFormManager ChartFormManager;
		private readonly RepositoryDllReporters repository;
		// doesn't allow multiple instances of the same reporter invoked for one chart
		public Dictionary<string, Reporter> ReporterShortNamesUserInvoked;
		public MenuItemsProvider MenuItemsProvider;
		public int deserializeIndex = 0;
		
		public Dictionary<string, DockContent> FormsAllRelated {
			get {
				var ret = new Dictionary<string, DockContent>();
				foreach (string reporterName in this.ReporterShortNamesUserInvoked.Keys) {
					Reporter reporter = this.ReporterShortNamesUserInvoked[reporterName];
					var parentForm = reporter.Parent as DockContent;
					if (parentForm == null) {
						string msg = "Reporter[" + reporter + "].Parent[" + reporter.Parent + "] is not a DockContent";
						Assembler.PopupException(msg);
						continue;
					}
					ret.Add(reporterName, parentForm);
				}
				return ret;
			}
		}



		public ReportersFormsManager(ChartFormManager chartFormManager, RepositoryDllReporters repository) {
			this.ChartFormManager = chartFormManager;
			this.repository = repository;
			this.ReporterShortNamesUserInvoked = new Dictionary<string, Reporter>();
			this.MenuItemsProvider = new MenuItemsProvider(this, this.repository.TypesFound);
		}
		public void BuildOnceAllReports(SystemPerformance performance) {
			if (this.ChartFormManager.ChartForm.InvokeRequired) {
				this.ChartFormManager.ChartForm.BeginInvoke((MethodInvoker)delegate { this.BuildOnceAllReports(performance); });
				return;
			}
			foreach (Reporter rep in this.ReporterShortNamesUserInvoked.Values) {
				rep.BuildOnceAfterFullBlindBacktestFinished(performance);
				
				// Reporters.Position should display "Positions (276)"
				ReporterFormWrapper parent = rep.Parent as ReporterFormWrapper;
				if (parent == null) continue;
				parent.Text = rep.TabText + " :: " + this.ChartFormManager.ChartForm.Text;
			}
		}
		public void BuildIncrementalAllReports(ReporterPokeUnit pokeUnit) {
			if (this.ChartFormManager.ChartForm.InvokeRequired) {
				this.ChartFormManager.ChartForm.BeginInvoke((MethodInvoker)delegate { this.BuildIncrementalAllReports(pokeUnit); });
				return;
			}
			foreach (Reporter rep in this.ReporterShortNamesUserInvoked.Values) {
				rep.BuildIncrementalAfterPositionsChangedInRealTime(pokeUnit);
			}
		}
		public void ChartForm_OnReporterMniClicked(object sender, EventArgs e) {
			var mniClicked = sender as ToolStripMenuItem;
			if (mniClicked == null) {
				string msg = "ChartForm_OnReporterMniClicked() should receive a click on ToolStripMenuItem, received from [" + sender.GetType() + "]";
				throw new Exception(msg);
			}
			string reporterShortNameClicked = mniClicked.Text;
			bool beforeCheckPropagatedInverted = mniClicked.Checked;
			try {
				if (beforeCheckPropagatedInverted == true) {
					Reporter reporterToBeClosed = this.ReporterShortNamesUserInvoked[reporterShortNameClicked];
					this.ReporterClosingUnregisterMniUntick(reporterToBeClosed.GetType().Name);
					//reporterToBeClosed.ParentForm.Close();
					DockContent form = reporterToBeClosed.Parent as DockContent;
					form.Close();
				} else {
					this.ReporterActivateShowRegisterMniTick(reporterShortNameClicked);
				}
				this.ChartFormManager.MainForm.MainFormSerialize();
			} catch (Exception ex) {
				this.ChartFormManager.MainForm.PopupException(ex);
				return;
			}
			//nope, doing that in ReporterActivateShowRegisterMniTick() ReporterClosingUnregisterMniUntick()
			//mniClicked.Checked = !mniClicked.Checked;
		}

		public ReporterFormWrapper ReporterActivateShowRegisterMniTick(string typeNameShortOrFullAutodetect, bool show=true) {
			string typeNameShort = this.repository.ShrinkTypeName(typeNameShortOrFullAutodetect);
			Reporter reporterActivated = this.repository.ActivateFromTypeName(typeNameShortOrFullAutodetect);
			object reportersSnapshot = this.FindOrCreateReportersSnapshot(reporterActivated);
			reporterActivated.Initialize(this.ChartFormManager.ChartForm.ChartControl as ChartShadow, reportersSnapshot);
			var ret = new ReporterFormWrapper(this, reporterActivated);
			//ret.Text = reporterActivated.TabText + " :: " + this.ChartFormsManager.Strategy.Name;
			ret.Text = reporterActivated.TabText + " :: " + this.ChartFormManager.ChartForm.Text;
			if (show) ret.Show(this.ChartFormManager.MainForm.DockPanel);
			this.ReporterShortNamesUserInvoked.Add(typeNameShort, reporterActivated);
			this.ChartFormManager.ReportersDumpCurrentForSerialization();
			this.MenuItemsProvider.FindMniByShortNameAndTick(typeNameShort);
			reporterActivated.BuildOnceAfterFullBlindBacktestFinished(this.ChartFormManager.Executor.Performance);
			return ret;
		}
		object FindOrCreateReportersSnapshot(Reporter reporterActivated) {
			Strategy strategy = this.ChartFormManager.Executor.Strategy;
			if (strategy == null) {
				string msg = "STRATEGY_MUST_NOT_BE_NULL ChartFormManager.Executor.Strategy";
				Debugger.Break();
			}
			ContextScript ctx = strategy.ScriptContextCurrent;
			if (ctx == null) {
				string msg = "CONTEXT_MUST_NOT_BE_NULL ChartFormManager.Executor.Strategy.ScriptContextCurrent";
				Debugger.Break();
			}
			Dictionary<string, object> snapshots = ctx.ReportersSnapshots;
			if (snapshots == null) {
				string msg = "REPORTERS_SNAPSHOTS_MUST_NOT_BE_NULL ChartFormManager.Executor.Strategy.ScriptContextCurrent.ReporterSnapshots";
				Debugger.Break();
			}
			return ctx.FindOrCreateReportersSnapshot(reporterActivated);
		}

		public void ReporterClosingUnregisterMniUntick(string reporterShortName) {
			this.ReporterShortNamesUserInvoked.Remove(reporterShortName);
			this.ChartFormManager.ReportersDumpCurrentForSerialization();
			this.MenuItemsProvider.FindMniByShortNameAndTick(reporterShortName, false);
		}

		public void PopupReporters_OnParentChartActivated(object sender, EventArgs e) {
			foreach (Reporter reporterToPopup in this.ReporterShortNamesUserInvoked.Values) {
				DockContentImproved dockContentImproved = reporterToPopup.Parent as DockContentImproved;
				if (dockContentImproved == null) {
					string msg = "reporterToPopup.Parent IS_NOT DockContentImproved";
					#if DEBUG
					Debugger.Break();
					#endif
					Assembler.PopupException(msg + " //PopupReporters_OnParentChartActivated()");
					return;
				}
				// INFINITE_LOOP_HANGAR_NINE_DOOMED_TO_COLLAPSE form.Activate();
				dockContentImproved.ActivateDockContentPopupAutoHidden(false, true);
			}
		}
	}
}