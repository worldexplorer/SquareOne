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
		public	ChartFormManager 				ChartFormManager 				{ get; private set; }
				RepositoryDllReporters 			reportersRepo;
		public	Dictionary<string, Reporter>	ReporterShortNamesUserInvoked	{ get; private set; }	// multiple instances of the same reporter invoked for one chart <= are not allowed
		public	MenuItemsProvider				MenuItemsProvider				{ get; private set; }
		
		public Dictionary<string, ReporterFormWrapper> FormsAllRelated { get {
				var ret = new Dictionary<string, ReporterFormWrapper>();
				foreach (string reporterName in this.ReporterShortNamesUserInvoked.Keys) {
					Reporter reporter = this.ReporterShortNamesUserInvoked[reporterName];
					ReporterFormWrapper reporterContainerForm = reporter.Parent as ReporterFormWrapper;
					if (reporterContainerForm == null) {
						string msg = "Reporter[" + reporter + "].Parent[" + reporter.Parent + "] is not a ReporterFormWrapper";
						Assembler.PopupException(msg);
						continue;
					}
					ret.Add(reporterName, reporterContainerForm);
				}
				return ret;
			} }

		private ReportersFormsManager() {
			// ALREADY_THERE deserializeIndex = 0;
			reportersRepo = Assembler.InstanceInitialized.RepositoryDllReporters;
			ReporterShortNamesUserInvoked = new Dictionary<string, Reporter>();
			MenuItemsProvider = new MenuItemsProvider(this, this.reportersRepo.TypesFound);
		}
		public ReportersFormsManager(ChartFormManager chartFormManager) : this() {
			this.ChartFormManager = chartFormManager;

			this.ChartFormManager.Executor.EventGenerator.BrokerFilledAlertsOpeningForPositions_step1of3 += new EventHandler<ReporterPokeUnitEventArgs>(
				this.EventGenerator_BrokerFilledAlertsOpeningForPositions_step1of3);

			this.ChartFormManager.Executor.EventGenerator.OpenPositionsUpdatedDueToStreamingNewQuote_step2of3 += new EventHandler<PositionListEventArgs>(
				this.EventGenerator_OpenPositionsUpdatedDueToStreamingNewQuote_step2of3);

			this.ChartFormManager.Executor.EventGenerator.BrokerFilledAlertsClosingForPositions_step3of3 += new EventHandler<ReporterPokeUnitEventArgs>(
				this.EventGenerator_BrokerFilledAlertsClosingForPositions_step3of3);
		}

		void EventGenerator_BrokerFilledAlertsOpeningForPositions_step1of3(object sender, ReporterPokeUnitEventArgs e) {
			this.BuildIncrementalOnPositionsOpenedAllReports_step1of3(e.PokeUnit);
		}
		void EventGenerator_OpenPositionsUpdatedDueToStreamingNewQuote_step2of3(object sender, PositionListEventArgs e) {
		    this.UpdateOpenPositionsDueToStreamingNewQuote_step2of3(e.PositionsOpenedNow);
		}
		void EventGenerator_BrokerFilledAlertsClosingForPositions_step3of3(object sender, ReporterPokeUnitEventArgs e) {
			this.BuildIncrementalOnPositionsClosedAllReports_step3of3(e.PokeUnit);
		}
		public void BuildReportFullOnBacktestFinishedAllReporters(SystemPerformance performance) {
			if (this.ChartFormManager.ChartForm.InvokeRequired) {
				this.ChartFormManager.ChartForm.BeginInvoke((MethodInvoker)delegate { this.BuildReportFullOnBacktestFinishedAllReporters(performance); });
				return;
			}
			foreach (Reporter rep in this.ReporterShortNamesUserInvoked.Values) {
				rep.BuildFullOnBacktestFinished(performance);
				
				// Reporters.Position should display "Positions (276)"
				ReporterFormWrapper parent = rep.Parent as ReporterFormWrapper;
				if (parent == null) continue;
				parent.Text = rep.TabText + " :: " + this.ChartFormManager.ChartForm.Text;
			}
		}
		public void BuildIncrementalOnPositionsClosedAllReports_step3of3(ReporterPokeUnit pokeUnit) {
			if (this.ChartFormManager.ChartForm.InvokeRequired) {
				this.ChartFormManager.ChartForm.BeginInvoke((MethodInvoker)delegate { this.BuildIncrementalOnPositionsClosedAllReports_step3of3(pokeUnit); });
				return;
			}
			foreach (Reporter rep in this.ReporterShortNamesUserInvoked.Values) {
				rep.BuildIncrementalOnPositionsOpenedClosed_step3of3(pokeUnit);
			}
		}
		public void UpdateOpenPositionsDueToStreamingNewQuote_step2of3(List<Position> positionsUpdatedDueToStreamingNewQuote) {
			if (this.ChartFormManager.ChartForm.InvokeRequired) {
				this.ChartFormManager.ChartForm.BeginInvoke((MethodInvoker)delegate { this.UpdateOpenPositionsDueToStreamingNewQuote_step2of3(positionsUpdatedDueToStreamingNewQuote); });
				return;
			}
			foreach (Reporter rep in this.ReporterShortNamesUserInvoked.Values) {
				rep.BuildIncrementalUpdateOpenPositionsDueToStreamingNewQuote_step2of3(positionsUpdatedDueToStreamingNewQuote);
			}
		}
		public void BuildIncrementalOnPositionsOpenedAllReports_step1of3(ReporterPokeUnit pokeUnit) {
			if (this.ChartFormManager.ChartForm.InvokeRequired) {
				this.ChartFormManager.ChartForm.BeginInvoke((MethodInvoker)delegate { this.BuildIncrementalOnPositionsOpenedAllReports_step1of3(pokeUnit); });
				return;
			}
			foreach (Reporter rep in this.ReporterShortNamesUserInvoked.Values) {
				rep.BuildIncrementalOnBrokerFilledAlertsOpeningForPositions_step1of3(pokeUnit);
			}
		}
		public void ChartForm_OnReporterMniClicked(object sender, EventArgs e) {
			var mniClicked = sender as ToolStripMenuItem;
			if (mniClicked == null) {
				string msg = "ChartForm_OnReporterMniClicked() should receive a click on ToolStripMenuItem, received from [" + sender.GetType() + "]";
				throw new Exception(msg);
			}
			string reporterShortNameClicked = this.MenuItemsProvider.StripPrefixFromMniName(mniClicked);
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
				Assembler.PopupException("ChartForm_OnReporterMniClicked()", ex);
				return;
			}
			//nope, doing that in ReporterActivateShowRegisterMniTick() ReporterClosingUnregisterMniUntick()
			//mniClicked.Checked = !mniClicked.Checked;
		}

		public ReporterFormWrapper ReporterActivateShowRegisterMniTick(string typeNameShortOrFullAutodetect, bool show=true) {
			string typeNameShort = this.reportersRepo.ShrinkTypeName(typeNameShortOrFullAutodetect);
			Reporter reporterActivated = this.reportersRepo.ActivateFromTypeName(typeNameShortOrFullAutodetect);
			object reportersSnapshot = this.findOrCreateReportersSnapshot(reporterActivated);
			reporterActivated.Initialize(this.ChartFormManager.ChartForm.ChartControl as ChartShadow, reportersSnapshot);
			var ret = new ReporterFormWrapper(this, reporterActivated);
			//ret.Text = reporterActivated.TabText + " :: " + this.ChartFormsManager.Strategy.Name;
			ret.Text = reporterActivated.TabText + " :: " + this.ChartFormManager.ChartForm.Text;
			if (show) ret.Show(this.ChartFormManager.MainForm.DockPanel);
			this.ReporterShortNamesUserInvoked.Add(typeNameShort, reporterActivated);
			this.ChartFormManager.ReportersDumpCurrentForSerialization();
			this.MenuItemsProvider.FindMniByShortNameAndTick(typeNameShort);
			if (this.ChartFormManager.Executor.Performance != null) {
				reporterActivated.BuildFullOnBacktestFinished(this.ChartFormManager.Executor.Performance);
			}
			return ret;
		}
		object findOrCreateReportersSnapshot(Reporter reporterActivated) {
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