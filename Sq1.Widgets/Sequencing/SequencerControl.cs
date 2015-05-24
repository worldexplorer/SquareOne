using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

using BrightIdeasSoftware;
using Sq1.Core;
using Sq1.Core.Sequencing;
using Sq1.Core.Repositories;
using Sq1.Core.StrategyBase;
using Sq1.Core.Indicators;
using Sq1.Core.DataTypes;

namespace Sq1.Widgets.Sequencing {
	public partial class SequencerControl : UserControl {
				Sequencer					sequencer;
				List<string>				colMetricsShouldStay;
				SequencedBacktests			backtestsLocalEasierToSync;
				List<OLVColumn>				columnsDynParams;
		public	RepositoryJsonsInFolderSimpleDictionarySequencer		RepositoryJsonSequencer					{ get; private set; }
				List<IndicatorParameter>	scriptAndIndicatorParametersMergedCloned;
		public	SequencedBacktestsEventArgs	PushToCorrelator { get { return new SequencedBacktestsEventArgs(this.backtestsLocalEasierToSync); } } 

		public SequencerControl() {
			InitializeComponent();
			this.colMetricsShouldStay = new List<string>() {
				this.olvcSerno.Name,
				this.olvcTotalPositions.Name,
				this.olvcProfitPerPosition.Name,
				this.olvcNetProfit.Name,
				this.olvcWinLoss.Name,
				this.olvcProfitFactor.Name,
				this.olvcRecoveryFactor.Name,
				this.olvcMaxDrawdown.Name,
				this.olvcMaxConsecutiveWinners.Name,
				this.olvcMaxConsecutiveLosers.Name
			};
			
			// in case if Designer clears out all the columns all of a sudden
			// IF_NOT_ADDED_CLICKING_PARAMETER_WILL_REMOVE_THESE_ADDED_FOREVER_OLV_GLITCH this will enable show/hide columns by right click on header
			//this.olvBacktests.AllColumns.AddRange(new BrightIdeasSoftware.OLVColumn[] {
			//	this.olvcSerno,
			//	this.olvcTotalPositions,
			//	this.olvcProfitPerPosition,
			//	this.olvcNetProfit,
			//	this.olvcWinLoss,
			//	this.olvcProfitFactor,
			//	this.olvcRecoveryFactor,
			//	this.olvcMaxDrawdown,
			//	this.olvcMaxConsecutiveWinners,
			//	this.olvcMaxConsecutiveLosers});

			//this.fastOLVparametersYesNoMinMaxStep.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.fastOLVparametersYesNoMinMaxStep_ItemCheck);

			backtestsLocalEasierToSync	= new SequencedBacktests();
			columnsDynParams			= new List<OLVColumn>();
			RepositoryJsonSequencer		= new RepositoryJsonsInFolderSimpleDictionarySequencer();
		}
		public void Initialize(Sequencer sequencer) {
			this.sequencer = sequencer;
			if (this.sequencer == null) {
				this.olvBacktests.EmptyListMsg = "this.sequencer == null";
				return;
			}
			if (this.sequencer.InitializedProperly_executorHasScript_readyToOptimize == false) {
				this.olvBacktests.EmptyListMsg = "this.sequencerInitializedProperly == false";
				return;
			}
			this.backtestsLocalEasierToSync = new SequencedBacktests(this.sequencer.Executor, this.sequencer.Executor.Strategy.ScriptContextCurrentName);

			// removing first to avoid reception of same SystemResults reception due to multiple Initializations by SequencerControl.Initialize() 
			//SETTING_COLLAPSED_FROM_BTN_RUN_CLICK this.sequencer.OnBacktestStarted -= new EventHandler<EventArgs>(sequencer_OnBacktestStarted);
			//SETTING_COLLAPSED_FROM_BTN_RUN_CLICK this.sequencer.OnBacktestStarted += new EventHandler<EventArgs>(sequencer_OnBacktestStarted);
			
			this.sequencer.OnOneBacktestFinished -= new EventHandler<SystemPerformanceRestoreAbleEventArgs>(this.sequencer_OnOneBacktestFinished);
			// since Sequencer.backtests is multithreaded list, I keep own copy here SequencerControl.backtests for ObjectListView to freely crawl over it without interference (instead of providing Sequencer.BacktestsThreadSafeCopy)  
			this.sequencer.OnOneBacktestFinished += new EventHandler<SystemPerformanceRestoreAbleEventArgs>(this.sequencer_OnOneBacktestFinished);
			
			this.sequencer.OnAllBacktestsFinished -= new EventHandler<EventArgs>(this.sequencer_OnAllBacktestsFinished);
			this.sequencer.OnAllBacktestsFinished += new EventHandler<EventArgs>(this.sequencer_OnAllBacktestsFinished);
			
			this.sequencer.OnSequencerAborted -= new EventHandler<EventArgs>(this.sequencer_OnSequencerAborted);
			this.sequencer.OnSequencerAborted += new EventHandler<EventArgs>(this.sequencer_OnSequencerAborted);

			this.sequencer.OnScriptRecompiledUpdateHeaderPostponeColumnsRebuild -= new EventHandler<EventArgs>(this.sequencer_OnScriptRecompiledUpdateHeaderPostponeColumnsRebuild);
			this.sequencer.OnScriptRecompiledUpdateHeaderPostponeColumnsRebuild += new EventHandler<EventArgs>(this.sequencer_OnScriptRecompiledUpdateHeaderPostponeColumnsRebuild);
			
			this.populateTextboxesFromExecutorsState();

			this.olvParametersCustomize();
			this.olvParameterPopulate();

			this.populateColumns();
			this.olvBacktestsCustomize();
		
			this.olvHistoryCustomize();
			this.RepositoryJsonSequencer.Initialize(Assembler.InstanceInitialized.AppDataPath
				, Path.Combine("Sequencer", this.sequencer.Executor.Strategy.RelPathAndNameForSequencerResults));
			
			//string symbolScaleRange = this.sequencer.Executor.Strategy.ScriptContextCurrent.ToStringSymbolScaleIntervalDataRangeForScriptContextNewName();
			//this.olvHistoryRescanRefillSelect(symbolScaleRange);

			this.symbolScaleRangeSelected = "";
			this.SelectHistoryPopulateBacktestsAndPushToCorellatorWithSequencedResultsBySymbolScaleRange();
		}
		void olvParameterPopulate() {
			this.scriptAndIndicatorParametersMergedCloned = this.sequencer.Executor.Strategy.ScriptContextCurrent.ScriptAndIndicatorParametersMergedUnclonedForSequencerAndSliders;
			this.olvParameters.SetObjects(this.scriptAndIndicatorParametersMergedCloned);
		}
		void olvHistoryRescanRefillSelect(string symbolScaleRange) {
			this.RepositoryJsonSequencer.RescanFolderStoreNamesFound();
			this.olvHistoryComputeAverage();
			this.olvHistory.SetObjects(this.RepositoryJsonSequencer.ItemsFound);
			FnameDateSizeColorPFavg found = null;
			foreach (FnameDateSizeColorPFavg each in this.RepositoryJsonSequencer.ItemsFound) {
				if (each.SymbolScaleRange != symbolScaleRange) continue;
				found = each;
				break;
			}
			if (found == null) {
				this.olvHistory.SelectedIndex = -1;
			} else {
				this.olvHistory.SelectObject(found, true);
				this.olvHistory.RefreshSelectedObjects();
			}
		}
		void olvHistoryComputeAverage() {
			return;

			foreach (FnameDateSizeColorPFavg each in this.RepositoryJsonSequencer.ItemsFound) {
				SequencedBacktests eachSequence = this.RepositoryJsonSequencer.DeserializeSingle(each.NameWithMarker);
				if (eachSequence == null || this.backtestsLocalEasierToSync.Count == 0) {
					string msg = "NO_BACKTESTS_FOUND_INSIDE_FILE " + each.NameWithMarker;
					Assembler.PopupException(msg);
					continue;
				}
			}
		}
		public void SelectHistoryPopulateBacktestsAndPushToCorellatorWithSequencedResultsBySymbolScaleRange(string symbolScaleRange = null) {
			if (base.InvokeRequired) {
				base.BeginInvoke((MethodInvoker)delegate { this.SelectHistoryPopulateBacktestsAndPushToCorellatorWithSequencedResultsBySymbolScaleRange(symbolScaleRange); });
				return;
			}

			if (string.IsNullOrEmpty(symbolScaleRange)) {
				Strategy strategy = this.sequencer.Executor.Strategy;
				symbolScaleRange = strategy.ScriptContextCurrent.ToStringSymbolScaleIntervalDataRangeForScriptContextNewName();
			}

			this.olvHistory.UseWaitCursor = true;
			this.olvBacktests.UseWaitCursor = true;

			if (this.symbolScaleRangeSelected != symbolScaleRange) {
				FnameDateSizeColorPFavg foundBySymbolScaleRange = this.RepositoryJsonSequencer.ItemsFoundContainsSymbolScaleRange_NullUnsafe(symbolScaleRange);
				if (foundBySymbolScaleRange != null) {
					this.backtestsLocalEasierToSync = this.RepositoryJsonSequencer.DeserializeSingle(foundBySymbolScaleRange.NameWithMarker);
					if (this.backtestsLocalEasierToSync == null || this.backtestsLocalEasierToSync.Count == 0) {
						string msg = "NO_BACKTESTS_FOUND_INSIDE_FILE " + symbolScaleRange;
						Assembler.PopupException(msg);
						//this.olvHistory.UseWaitCursor = false;
						//this.olvBacktests.UseWaitCursor = false;
						// 1) POPULATE_CHANGED_BARS_500=>500_INTO_SEQUENCER 2) WAIT_CURSOR_REMOVE DONT_return;
					}
					this.backtestsLocalEasierToSync.FileName = symbolScaleRange;
					this.backtestsLocalEasierToSync.CheckPositionsCountMustIncreaseOnly();

					RepositorySerializerSymbolInfo symbolInfoRep = Assembler.InstanceInitialized.RepositorySymbolInfo;
					SymbolInfo reloadNetWhenSymbolInfoChanged = symbolInfoRep.FindSymbolInfoNullUnsafe(this.backtestsLocalEasierToSync.Symbol);
					if (reloadNetWhenSymbolInfoChanged != null) {
						reloadNetWhenSymbolInfoChanged.PriceDecimalsChanged -= new EventHandler<EventArgs>(reloadNetWhenSymbolInfoChanged_PriceDecimalsChanged);
						reloadNetWhenSymbolInfoChanged.PriceDecimalsChanged += new EventHandler<EventArgs>(reloadNetWhenSymbolInfoChanged_PriceDecimalsChanged);
					} else {
						string msg = "SYMBOL_WAS_NOT_SERIALIZED_IN_foundBySymbolScaleRange[" + foundBySymbolScaleRange + "]";
						Assembler.PopupException(msg);
					}

					this.symbolScaleRangeSelected = symbolScaleRange;
				} else {
					string msg = "NOT_FOUND_WITH_MARKER_FOR_symbolScaleRange[" + symbolScaleRange + "]";
					Assembler.PopupException(msg, null, false);
				}
			}

			//preserveState=true will help NOT having SelectedObject=null between (rightClickCtx and Copy)clicks (while optimization is still running)
			this.olvBacktests.SetObjects(this.backtestsLocalEasierToSync.BacktestsReadonly, true);

			this.olvHistoryRescanRefillSelect(symbolScaleRange);
			this.populateTextboxesFromExecutorsState();

			if (backtestsLocalEasierToSync.Count > 0) {
				//my future Correlator wasn't initialized with me and no subscribers for this event so far => duplicate 20 lines above
				this.RaiseOnCorrelatorShouldPopulate(this.backtestsLocalEasierToSync);
				//v1 this.statsAndHistoryCollapse();
				//ANNOYING this.cbxExpanded.Checked = false;
			}

			this.olvHistory.UseWaitCursor = false;
			this.olvBacktests.UseWaitCursor = false;
		}

		void reloadNetWhenSymbolInfoChanged_PriceDecimalsChanged(object sender, EventArgs e) {
			if (base.InvokeRequired) {
				string msg = "NYI__SYMBOL_INFO.PRICE_DECIMALS__CHANGED_IN_A_NON_GUI_THREAD //SequencerControl.reloadNetWhenSymbolInfoChanged_PriceDecimalsChanged()";
				Assembler.PopupException(msg);
				return;
			}
			SymbolInfo iUpdatedPriceFormat = sender as SymbolInfo;
			if (iUpdatedPriceFormat == null) {
				string msg = "MUST_BE_SymbolInfo_sender[" + sender + "] //reloadNetWhenSymbolInfoChanged_PriceDecimalsChanged()";
				Assembler.PopupException(msg);
				return;
			}
			this.olvBacktestsReCustomize_OnPriceDecimalsChanged();
			this.olvBacktests.RebuildColumns();
		}

		void populateTextboxesFromExecutorsState() {
			if (this.splitContainer1.SplitterDistance != this.heightExpanded) {
				this.splitContainer1.SplitterDistance  = this.heightExpanded;
			}
			
			this.cbxRunCancel.Enabled				= true;
			this.cbxPauseResume.Enabled				= false;

			//string staleReason = this.sequencer.StaleReason;
			//if (string.IsNullOrEmpty(staleReason) == false) {
			//	return staleReason;
			//}
			//this.lblStats.Text				= staleReason;
			this.txtDataRange.Text			= this.sequencer.DataRangeAsString;
			this.txtPositionSize.Text		= this.sequencer.PositionSizeAsString;
			this.txtStrategy.Text			= this.sequencer.StrategyAsString;
			this.txtSymbol.Text				= this.sequencer.SymbolScaleIntervalAsString;
			this.txtSpread.Text				= this.sequencer.SpreadPips;
			this.txtQuotesGenerator.Text	= this.sequencer.BacktestStrokesPerBar;
			this.totalsPropagateAdjustSplitterDistance();
		}

		void totalsPropagateAdjustSplitterDistance() {
			this.txtScriptParameterTotalNr.Text = this.sequencer.ScriptParametersTotalNr.ToString();
			this.txtIndicatorParameterTotalNr.Text = this.sequencer.IndicatorParameterTotalNr.ToString();

			int backtestsTotal = this.sequencer.BacktestsTotal;
			this.cbxRunCancel.Text = "Run " + backtestsTotal + " backtests";
			this.cbxRunCancel.Enabled = backtestsTotal > 0 ? true : false;
			this.lblStats.Text = "0% complete   0/" + backtestsTotal;
			this.progressBar1.Value = 0;
			this.progressBar1.Maximum = (backtestsTotal != -1) ? backtestsTotal : 0;

			this.nudThreadsToRun.Value = this.sequencer.ThreadsToUse;

			if (backtestsTotal == -1) {
				this.olvBacktests.EmptyListMsg = "RUN_-1_BACKTESTS_IS_DUE_TO MULTIPLICATION_OF_POSSIBLE_PARAMETERS"
					+ " IS_MORE_THAN_2,14_BLN_COMBINATIONS WENT_OUT_OF_INT32_CAPACITY"
					+ " SEE_EXCEPTIONS_FORM_FOR_OFFENSIVE_PARAMETERS DECREASE_RANGE_OR_INCREASE_STEP_FOR_xxx RECOMPILE";
			}

			this.adjustSplitterDistanceToNumberOfParameters_invokeMeAfterRecompiled();
		}

		void adjustSplitterDistanceToNumberOfParameters_invokeMeAfterRecompiled() {
			//I_NEED_NEW_HEIGHT_ANYWAY if (this.cbxExpanded.Checked == false) return;

			//int rowsShown = this.fastOLVparametersYesNoMinMaxStep.RowsPerPage;
			int splitterDistanceForTwoLines = 196;
			int allParameterLinesToDraw = this.sequencer.AllParameterLinesToDraw;
			int heightEachNewLine = this.olvParameters.RowHeightEffective;
			if (this.olvParameters.GridLines) heightEachNewLine++;
			int inAdditionToTwo = (allParameterLinesToDraw - 2) * heightEachNewLine;
			this.heightExpanded = splitterDistanceForTwoLines + inAdditionToTwo;
			
			//if (allParameterLinesToDraw <= 3) {
			//    this.splitContainer1.SplitterDistance = splitterDistanceForTwoLines;
			//    return;
			//}
			//v1 this.statsAndHistoryExpand();
			this.cbxExpanded.Checked = true;
		}
		void populateColumns() {
			//DONT_CLEAR_RESULTS_AFTER_TAB_SWITCHING_ONLY_RUN_WILL_CLEAR_OLD_TABLE this.olvBacktests.Items.Clear();
			this.columnsDynParams.Clear();
			
			List<OLVColumn> colParametersToClear = new List<OLVColumn>();
			foreach (OLVColumn col in this.olvBacktests.Columns) {
				if (this.colMetricsShouldStay.Contains(col.Name)) continue;
				colParametersToClear.Add(col);
			}
			foreach (OLVColumn col in colParametersToClear) {
				this.olvBacktests.Columns.Remove(col);
				this.olvBacktests.AllColumns.Remove(col);
			}
			if (this.sequencer == null) {
				this.olvBacktests.EmptyListMsg = "this.sequencer == null";
				return;
			}
			SortedDictionary<int, ScriptParameter> sparams = this.sequencer.Executor.Strategy.Script.ScriptParametersById_ReflectedCached;
			if (sparams == null) {
				this.olvBacktests.EmptyListMsg = "this.sequencer.ExecutorCloneToBeSpawned.Strategy.Script.ScriptParametersById_ReflectedCached == null";
				return;
			}
			Dictionary<string, IndicatorParameter> iparams = this.sequencer.Executor.Strategy.Script.IndicatorsParameters_ReflectedCached;
			if (iparams == null) {
				this.olvBacktests.EmptyListMsg = "this.sequencer.ExecutorCloneToBeSpawned.Strategy.Script.IndicatorsByName_ReflectedCached == null";
				return;
			}

			foreach (ScriptParameter sp in sparams.Values) {
				//CHANGING_COLUMN_VISIBILITY_INSTEAD if (this.showAllScriptIndicatorParametersInOptimizationResults == false) {
				//CHANGING_COLUMN_VISIBILITY_INSTEAD 	if (sp.WillBeSequencedDuringOptimization == false) continue;
				//CHANGING_COLUMN_VISIBILITY_INSTEAD }
				OLVColumn olvcSP = new OLVColumn();
				olvcSP.Name = sp.Name;
				olvcSP.Text = sp.Name;
				olvcSP.Width = 85;
				olvcSP.TextAlign = HorizontalAlignment.Right;
				olvcSP.IsVisible = sp.WillBeSequenced;
				this.olvBacktests.Columns.Add(olvcSP);
				this.olvBacktests.AllColumns.Add(olvcSP);
				this.columnsDynParams.Add(olvcSP);
			}
			
			foreach (string indicatorDotParameter in iparams.Keys) {
				//CHANGING_COLUMN_VISIBILITY_INSTEAD if (this.showAllScriptIndicatorParametersInOptimizationResults == false) {
				//CHANGING_COLUMN_VISIBILITY_INSTEAD 	IndicatorParameter ip = iparams[indicatorDotParameter];
				//CHANGING_COLUMN_VISIBILITY_INSTEAD 	if (ip.WillBeSequencedDuringOptimization == false) continue;
				//CHANGING_COLUMN_VISIBILITY_INSTEAD }
				OLVColumn olvcIP = new OLVColumn();
				olvcIP.Name = indicatorDotParameter;
				olvcIP.Text = indicatorDotParameter;
				olvcIP.Width = 85;
				olvcIP.TextAlign = HorizontalAlignment.Right;
				IndicatorParameter ip = iparams[indicatorDotParameter];
				olvcIP.IsVisible = ip.WillBeSequenced;
				this.olvBacktests.Columns.Add(olvcIP);
				this.olvBacktests.AllColumns.Add(olvcIP);
				this.columnsDynParams.Add(olvcIP);
			}
			this.olvBacktests.RebuildColumns();	// OTHERWIZE_FIRST_TIME_SHOWN_INVISIBLES_ARE_VISIBLE__TIRED_OF_OLV_ILLOGICALITIES_RRRR
		}
		
		int heightExpanded;	//REPLACED_BY_this.adjustSplitterDistanceToNumberOfParameters_invokeMeAfterRecompiled() { get { return this.splitContainer1.Panel1MinSize * 8; } }
		string symbolScaleRangeSelected;
		int heightCollapsed { get { return this.splitContainer1.Panel1MinSize; } }
		public void NormalizeBackgroundOrMarkIfBacktestResultsAreForDifferentSymbolScaleIntervalRangePositionSize() {
			Strategy strategy = this.sequencer.Executor.Strategy;
			string symbolScaleRange = strategy.ScriptContextCurrent.ToStringSymbolScaleIntervalDataRangeForScriptContextNewName();
			FnameDateSizeColorPFavg foundBySymbolScaleRange = this.RepositoryJsonSequencer.ItemsFoundContainsSymbolScaleRange_NullUnsafe(symbolScaleRange);
			if (foundBySymbolScaleRange == null) return;

			//string staleReason = this.sequencer.StaleReason;
			//this.lblStats.Text = staleReason; // TextBox doesn't display "null" for null-string
			
			//bool userClickedAnotherSymbolScaleIntervalRangePositionSize = string.IsNullOrEmpty(staleReason) == false;
			//this.splitContainer1.Panel1.BackColor = userClickedAnotherSymbolScaleIntervalRangePositionSize
			//	? Color.LightSalmon : SystemColors.Control;
			//this.splitContainer1.SplitterDistance = userClickedAnotherSymbolScaleIntervalRangePositionSize || this.backtestsLocalEasierToSync.Count == 0
			//	? this.heightExpanded : this.heightCollapsed;
			//this.cbxRunCancel.Text = userClickedAnotherSymbolScaleIntervalRangePositionSize
			//	? "Clear to Optimize" : "Run " + this.sequencer.BacktestsTotal + " backtests";
		}

		void statsAndHistoryExpand(bool changeCheckboxState = true) {
			try {
				this.splitContainer1.SplitterDistance = this.heightExpanded;
				this.cbxExpanded.Text = "-";
				if (changeCheckboxState == true && this.cbxExpanded.Checked != true) this.cbxExpanded.Checked = true;
			} catch (Exception ex) {
				Assembler.PopupException("RESIZE_DIDNT_SYNC_SPLITTER_MIN_MAX???", ex);
			}
		}
		void statsAndHistoryCollapse(bool changeCheckboxState = true) {
			try {
				this.splitContainer1.SplitterDistance = this.heightCollapsed;
				this.cbxExpanded.Text = "+";
				if (changeCheckboxState == true && this.cbxExpanded.Checked != false) this.cbxExpanded.Checked = false;
			} catch (Exception ex) {
				Assembler.PopupException("RESIZE_DIDNT_SYNC_SPLITTER_MIN_MAX???", ex);
			}
		}

		public void BacktestsReplaceWithCorrelated(IEnumerable<SystemPerformanceRestoreAble> list) {
			this.olvBacktests.SetObjects(list, true);
			this.mni_showAllScriptIndicatorParametersInSequencedBacktest.Checked = false;
		}

		public void BacktestsRestoreCorrelatedClosed() {
			this.olvBacktests.SetObjects(this.backtestsLocalEasierToSync.BacktestsReadonly, true);
		}
		public override string ToString() {
			string ret = "UNINITIALIZED";
			if (this.sequencer != null) ret = this.sequencer.Executor.Strategy.WindowTitle;
			return "Sequencer :: " + ret;
		}
	}
}
