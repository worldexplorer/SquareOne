using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

using WeifenLuo.WinFormsUI.Docking;

using Sq1.Core;
using Sq1.Core.Charting;
using Sq1.Core.Execution;
using Sq1.Core.Repositories;
using Sq1.Core.StrategyBase;
using Sq1.Widgets;
using Sq1.Gui.Forms;

namespace Sq1.Gui.ReportersSupport {
	public class ReportersFormsManager {
		public	ChartFormManager 				ChartFormsManager 				{ get; private set; }
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

		ReportersFormsManager() {
			// ALREADY_THERE deserializeIndex = 0;
			reportersRepo = Assembler.InstanceInitialized.RepositoryDllReporters;
			ReporterShortNamesUserInvoked = new Dictionary<string, Reporter>();
			MenuItemsProvider = new MenuItemsProvider(this, this.reportersRepo.TypesFound_inScannedDLLs);
		}
		public ReportersFormsManager(ChartFormManager chartFormManager) : this() {
			this.ChartFormsManager = chartFormManager;

			this.ChartFormsManager.Executor.EventGenerator.OnBacktesterContextInitialized_step2of4 += new EventHandler<EventArgs>(
				this.eventGenerator_BacktesterContextInitialized_step2of4);

			this.ChartFormsManager.Executor.EventGenerator.OnBrokerFilledAlertsOpeningForPositions_step1of3 += new EventHandler<ReporterPokeUnitEventArgs>(
				this.eventGenerator_BrokerFilledAlertsOpeningForPositions_step1of3);

			this.ChartFormsManager.Executor.EventGenerator.OnOpenPositionsUpdatedDueToStreamingNewQuote_step2of3 += new EventHandler<ReporterPokeUnitEventArgs>(
				this.eventGenerator_OpenPositionsUpdatedDueToStreamingNewQuote_step2of3);

			this.ChartFormsManager.Executor.EventGenerator.OnBrokerFilledAlertsClosingForPositions_step3of3 += new EventHandler<ReporterPokeUnitEventArgs>(
				this.eventGenerator_BrokerFilledAlertsClosingForPositions_step3of3);
		}

		void eventGenerator_BacktesterContextInitialized_step2of4(object sender, EventArgs e) {
			this.ClearAllReportsSincePerformanceGotCleared_step0of3();
		}

		void eventGenerator_BrokerFilledAlertsOpeningForPositions_step1of3(object sender, ReporterPokeUnitEventArgs e) {
			//DELEGATED_TO_REPORTER_WRAPPER if (e.PokeUnit.PositionsOpened.AlertsEntry.GuiHasTimeToRebuild(this, "eventGenerator_BrokerFilledAlertsOpeningForPositions_step1of3(WAIT)") == false) return;
			this.BuildIncrementalOnPositionsOpenedAllReports_step1of3(e.PokeUnit);
		}
		void eventGenerator_OpenPositionsUpdatedDueToStreamingNewQuote_step2of3(object sender, ReporterPokeUnitEventArgs e) {
			//I_WANT_REPORTERS_TO_REBUILD!!OPTIMIZED_THEM
			//DELEGATED_TO_REPORTER_WRAPPER if (e.PokeUnit.PositionsOpenNow.AlertsOpenNow.GuiHasTimeToRebuild(this, "eventGenerator_OpenPositionsUpdatedDueToStreamingNewQuote_step2of3(WAIT)") == false) return;
			this.UpdateOpenPositionsDueToStreamingNewQuote_step2of3(e.PokeUnit);
		}
		void eventGenerator_BrokerFilledAlertsClosingForPositions_step3of3(object sender, ReporterPokeUnitEventArgs e) {
			//DELEGATED_TO_REPORTER_WRAPPER if (e.PokeUnit.PositionsClosed.AlertsExit.GuiHasTimeToRebuild(this, "eventGenerator_BrokerFilledAlertsClosingForPositions_step3of3(WAIT)") == false) return;
			this.BuildIncrementalOnPositionsClosedAllReports_step3of3(e.PokeUnit);
		}
		public void ClearAllReportsSincePerformanceGotCleared_step0of3() {
			SystemPerformanceSlice both = this.ChartFormsManager.Executor.PerformanceAfterBacktest.SlicesShortAndLong;
			//bool amIlaunchingLivesim = this.ChartFormManager.Executor.Backtester.IsLivesimRunning;
			//if (amIlaunchingLivesim) {
			//	if (both.PositionsImTracking.Count > 0 || both.NetProfitForClosedPositionsBoth > 0) {
			//		string msg = "I_REFUSE_CLEAR_ALL_REPORTS__SYSTEM_PERFORMANCE_MUST_BE_CLEAN_AFTER_USER_CLICKED_START_LIVESIM_DURING_REAL_LIVE";
			//		Assembler.PopupException(msg, null, false);
			//		return;
			//	}
			//}

			if (this.ChartFormsManager.ChartForm.InvokeRequired) {
				//if (amIlaunchingLivesim == false) {
					if (both.PositionsImTracking.Count > 0 || both.NetProfitForClosedPositionsBoth > 0) {
						string msg2 = "ERROR__SYSTEM_PERFORMANCE_MUST_BE_CLEAN__RUNNING_BACKTEST_ON_APP_RESTART_OR_F8";
						Assembler.PopupException(msg2, null, false);
					} else {
						string msg = "OK_HERE__OFFLINE_BACKTEST_RESETTING_REPORTERS_TO_ZERO__AFTER_BACKTEST_CONTEXT_INITIALIZE";
						//Assembler.PopupException(msg, null, false);
					}
				//}

				this.ChartFormsManager.ChartForm.BeginInvoke((MethodInvoker)delegate { this.ClearAllReportsSincePerformanceGotCleared_step0of3(); });
				return;
			}

			foreach (Reporter rep in this.ReporterShortNamesUserInvoked.Values) {
				rep.BuildFullOnBacktestFinished();
			}
			this.WindowTitlePullFromStrategy_allReporterWrappers();
		}

		public void WindowTitlePullFromStrategy_allReporterWrappers() {
			foreach (Reporter rep in this.ReporterShortNamesUserInvoked.Values) {
				// Reporters.Position should display "Positions (276)"
				ReporterFormWrapper parent = rep.Parent as ReporterFormWrapper;
				if (parent == null) continue;

				string windowTitle = rep.TabText + " :: " + this.ChartFormsManager.ChartForm.Text;
				//ALREADY_APPENDED if (this.ChartFormManager.Strategy.ActivatedFromDll == true) windowTitle += "-DLL";
				//if (this.ChartFormsManager.ScriptEditedNeedsSaving) {
				//	windowTitle = ChartFormsManager.PREFIX_FOR_UNSAVED_STRATEGY_SOURCE_CODE + windowTitle;
				//}
				parent.Text = windowTitle;
			}
		}
		public void BuildReportFullOnBacktestFinishedAllReporters(SystemPerformance performance = null) {
			if (performance == null) {
				performance = this.ChartFormsManager.Executor.PerformanceAfterBacktest;
			}
			if (this.ChartFormsManager.ChartForm.InvokeRequired) {
				this.ChartFormsManager.ChartForm.BeginInvoke((MethodInvoker)delegate { this.BuildReportFullOnBacktestFinishedAllReporters(performance); });
				return;
			}
			if (performance.SlicesShortAndLong.PositionsImTracking.Count == 0 && performance.SlicesShortAndLong.NetProfitForClosedPositionsBoth != 0) {
				string msg = "REPORTERS.POSITIONS_WILL_BE_EMPTY__WHILE_REPORTERS.PERFORMACE_WILL_DISPLAY_BACKTESTED_NUMBERS";
				Assembler.PopupException(msg);
			}
			foreach (Reporter rep in this.ReporterShortNamesUserInvoked.Values) {
				rep.BuildFullOnBacktestFinished();
				this.populateWindowTextFromTabTextStashed(rep);
			}
		}

		void populateWindowTextFromTabTextStashed(Reporter rep) {
				// Reporters.Position should display "Positions (276)"
				ReporterFormWrapper parent = rep.Parent as ReporterFormWrapper;
				if (parent == null) return;
				parent.Text = rep.TabText + " :: " + this.ChartFormsManager.ChartForm.Text;
		}
		public void BuildIncrementalOnPositionsClosedAllReports_step3of3(ReporterPokeUnit pokeUnit) {
			if (this.ChartFormsManager.ChartForm.InvokeRequired) {
				this.ChartFormsManager.ChartForm.BeginInvoke((MethodInvoker)delegate { this.BuildIncrementalOnPositionsClosedAllReports_step3of3(pokeUnit); });
				return;
			}
			foreach (Reporter rep in this.ReporterShortNamesUserInvoked.Values) {
				rep.BuildIncrementalOnPositionsOpenedClosed_step3of3(pokeUnit);
				this.populateWindowTextFromTabTextStashed(rep);
			}
		}
		public void UpdateOpenPositionsDueToStreamingNewQuote_step2of3(ReporterPokeUnit pokeUnit) {
//			foreach (Reporter rep in this.ReporterShortNamesUserInvoked.Values) {
//				if (rep.SystemPerformance != null) continue;
//				string msg = "AVOIDING_EXCEPTION_DOWNSTACK YOU_JUST_RESTARTED_APP_AND_DIDNT_EXECUTE_BACKTEST_PRIOR_TO_CONSUMING_STREAMING_QUOTES__QUOTES_PUMP_SHOULD_BE_UNPAUSED_AFTER_BACKTEST_COMPLETES";
//				Assembler.PopupException(msg);
//				return;
//			}
	
			if (this.ChartFormsManager.ChartForm.InvokeRequired) {
				this.ChartFormsManager.ChartForm.BeginInvoke((MethodInvoker)delegate { this.UpdateOpenPositionsDueToStreamingNewQuote_step2of3(pokeUnit); });
				return;
			}
			foreach (Reporter rep in this.ReporterShortNamesUserInvoked.Values) {
				rep.BuildIncrementalUpdateOpenPositionsDueToStreamingNewQuote_step2of3(pokeUnit);
				this.populateWindowTextFromTabTextStashed(rep);
			}
		}
		public void BuildIncrementalOnPositionsOpenedAllReports_step1of3(ReporterPokeUnit pokeUnit) {
			if (this.ChartFormsManager.ChartForm.InvokeRequired) {
				this.ChartFormsManager.ChartForm.BeginInvoke((MethodInvoker)delegate { this.BuildIncrementalOnPositionsOpenedAllReports_step1of3(pokeUnit); });
				return;
			}
			foreach (Reporter rep in this.ReporterShortNamesUserInvoked.Values) {
				rep.BuildIncrementalOnBrokerFilledAlertsOpeningForPositions_step1of3(pokeUnit);
				this.populateWindowTextFromTabTextStashed(rep);
			}
		}

		public void RebuildingFullReportForced_onLivesimPaused() {
			if (this.ChartFormsManager.ChartForm.InvokeRequired) {
				this.ChartFormsManager.ChartForm.BeginInvoke((MethodInvoker)delegate { this.RebuildingFullReportForced_onLivesimPaused(); });
				return;
			}
			foreach (Reporter rep in this.ReporterShortNamesUserInvoked.Values) {
				rep.StashWindowTextSuffixInBaseTabText_usefulToUpdateAutohiddenStatsWithoutRebuildingFullReport_OLVisSlow();
				this.populateWindowTextFromTabTextStashed(rep);
				rep.RebuildingFullReportForced_onLivesimPaused();
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
				this.ChartFormsManager.MainForm.MainFormSerialize();
			} catch (Exception ex) {
				Assembler.PopupException("ChartForm_OnReporterMniClicked()", ex);
				return;
			}
			//nope, doing that in ReporterActivateShowRegisterMniTick() ReporterClosingUnregisterMniUntick()
			//mniClicked.Checked = !mniClicked.Checked;
		}

		public ReporterFormWrapper ReporterActivateShowRegisterMniTick(string typeNameShortOrFullAutodetect, bool show=true) {
			string typeNameShort = this.reportersRepo.ShrinkTypeName(typeNameShortOrFullAutodetect);
			Reporter reporterActivated = this.reportersRepo.Activate_fromTypeName(typeNameShortOrFullAutodetect);
			object reportersSnapshot = this.findOrCreateReportersSnapshot_nullUnsafe(reporterActivated);
			reporterActivated.Initialize(this.ChartFormsManager.ChartForm.ChartControl as ChartShadow, reportersSnapshot, this.ChartFormsManager.Executor.PerformanceAfterBacktest);

			var ret = new ReporterFormWrapper(this, reporterActivated);
			//ret.Text = reporterActivated.TabText + " :: " + this.ChartFormsManager.Strategy.Name;
			ret.Text = reporterActivated.TabText + " :: " + this.ChartFormsManager.ChartForm.Text;
			if (show) ret.Show(this.ChartFormsManager.MainForm.DockPanel);
			this.ReporterShortNamesUserInvoked.Add(typeNameShort, reporterActivated);
			this.ChartFormsManager.ReportersDumpCurrentForSerialization();
			this.MenuItemsProvider.FindMniByShortNameAndTick(typeNameShort);
			
			// avoiding unnesessary Reporters' calculation when there was no backtest invoked yet; I don't mind absolutely blank Performance Report without headers
			if (Assembler.InstanceInitialized.MainForm_dockFormsFullyDeserialized_layoutComplete == false) return ret;
			if (this.ChartFormsManager.Executor.PerformanceAfterBacktest == null) {
				string msg = "SO_WHEN_IT_HAPPENS_IF_EVER?..";
				Assembler.PopupException(msg);
				return ret;
			}
			reporterActivated.BuildFullOnBacktestFinished();
			return ret;
		}
		object findOrCreateReportersSnapshot_nullUnsafe(Reporter reporterActivated) {
			Strategy strategy = this.ChartFormsManager.Executor.Strategy;
			if (strategy == null) {
				string msg = "STRATEGY_MUST_NOT_BE_NULL ChartFormManager.Executor.Strategy";
				Assembler.PopupException(msg);
				return null;
			}
			ContextScript ctx = strategy.ScriptContextCurrent;
			if (ctx == null) {
				string msg = "CONTEXT_MUST_NOT_BE_NULL ChartFormManager.Executor.Strategy.ScriptContextCurrent";
				Assembler.PopupException(msg);
				return null;
			}
			Dictionary<string, object> snapshots = ctx.ReportersSnapshots;
			if (snapshots == null) {
				string msg = "REPORTERS_SNAPSHOTS_MUST_NOT_BE_NULL ChartFormManager.Executor.Strategy.ScriptContextCurrent.ReporterSnapshots";
				Assembler.PopupException(msg);
				return null;
			}
			return ctx.FindOrCreateReportersSnapshot(reporterActivated);
		}

		public void ReporterClosingUnregisterMniUntick(string reporterShortName) {
			this.ReporterShortNamesUserInvoked.Remove(reporterShortName);
			this.ChartFormsManager.ReportersDumpCurrentForSerialization();
			this.MenuItemsProvider.FindMniByShortNameAndTick(reporterShortName, false);
			this.ChartFormsManager.MainForm.MainFormSerialize();
		}

		public void PopupReporters_OnParentChartActivated(object sender, EventArgs e) {
			foreach (Reporter reporterToPopup in this.ReporterShortNamesUserInvoked.Values) {
				DockContentImproved dockContentImproved = reporterToPopup.Parent as DockContentImproved;
				if (dockContentImproved == null) {
					string msg = "MUST_BE_DockContentImproved_reporterToPopup.Parent[" + reporterToPopup.Parent + "]";
					Assembler.PopupException(msg + " //PopupReporters_OnParentChartActivated()", null, false);
					return;
				}
				// INFINITE_LOOP_HANGAR_NINE_DOOMED_TO_COLLAPSE form.Activate();
				dockContentImproved.ActivateDockContentPopupAutoHidden(false, true);
			}
		}

		public void LivesimStartedOrUnpaused_HideReporters() {
			foreach (Reporter rep in this.ReporterShortNamesUserInvoked.Values) {
				ReporterFormWrapper wrapper = rep.Parent as ReporterFormWrapper;
				if (wrapper == null) continue;
				if (wrapper.IsCoveredOrAutoHidden == false) wrapper.ToggleAutoHide();
			}
		}
		public void LivesimEndedOrStoppedOrPaused_RestoreHiddenReporters() {
			foreach (Reporter rep in this.ReporterShortNamesUserInvoked.Values) {
				ReporterFormWrapper wrapper = rep.Parent as ReporterFormWrapper;
				if (wrapper == null) continue;
				if (wrapper.IsCoveredOrAutoHidden == true) wrapper.ToggleAutoHide();
			}
		}

		internal void Dispose_workspaceReloading() {
			foreach (ReporterFormWrapper each in this.FormsAllRelated.Values) {
				if (each.IsDisposed || each.Disposing) continue;
				each.Dispose();
			}
			// each Reporter was added into ReporterFormWrapper, so none of my Reporters should have left undisposed by now
			foreach (Reporter each in this.ReporterShortNamesUserInvoked.Values) {
				if (each.IsDisposed || each.Disposing) continue;
				each.Dispose();
			}
		}
	}
}