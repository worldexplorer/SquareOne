using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using BrightIdeasSoftware;
using Sq1.Core;
using Sq1.Core.Sequencing;
using Sq1.Core.Correlation;
using Sq1.Core.Indicators;
using Sq1.Core.Support;

namespace Sq1.Widgets.Correlation {
	public partial class OneParameterControl : UserControlResizeable {
		public OneParameterAllValuesAveraged Parameter { get; private set; }

		Dictionary<ToolStripMenuItem, List<OLVColumn>>			columnsByFilter;
		Dictionary<ToolStripMenuItem, OLVColumn>				columnToSortDescendingByMaximizationMni;
		Dictionary<MaximizationCriterion, ToolStripMenuItem>	mniToCheckForMaximizationCriterion;
		MaximizationCriterion sortBy;

		CorrelatorControl	allParametersControl;
		Correlator			correlator						{ get {
			if (this.allParametersControl == null) {
				string msg = "YOU_WANTED_TO_CATCH_IT";
				Assembler.PopupException(msg);
			}
			return this.allParametersControl.Correlator; } }

		IndicatorParameter	indicatorParameterNullUnsafe	{ get { 
			IndicatorParameter ret = null;
			if (this.correlator.Executor == null) {
				string msg = "OneParameterControl.correlator.Executor == null";
				Assembler.PopupException(msg);
				return ret;
			}
			if (this.correlator.Executor.Strategy == null) {
				string msg = "OneParameterControl.correlator.Executor.Strategy == null";
				Assembler.PopupException(msg);
				return ret;
			}
			if (this.correlator.Executor.Strategy.ScriptContextCurrent == null) {
				string msg = "OneParameterControl.correlator.Executor.Strategy.ScriptContextCurrent == null";
				Assembler.PopupException(msg);
				return ret;
			}
			SortedDictionary<string, IndicatorParameter> parametersByFullName =
				this.correlator.Executor.Strategy.ScriptContextCurrent
					.ScriptAndIndicatorParametersMergedUnclonedForSequencerByName;
			if (parametersByFullName.ContainsKey(this.Parameter.ParameterName) == false) {
				string msg = "OneParameterControl.correlator.Executor.Strategy.ScriptContextCurrent"
					+ ".ScriptAndIndicatorParametersMergedUnclonedForSequencerByName.ContainsKey(" + this.Parameter.ParameterName + ") == false";
				Assembler.PopupException(msg);
				return ret;
			}
			ret = parametersByFullName[this.Parameter.ParameterName];
			return ret;
		} }

		public OneParameterControl() : base() {
			InitializeComponent();
			//	this.olvcParamValues.HeaderTriStateCheckBox = true;

			// in case Designer drops them and I won't have any column selector by colheader rightclick anymore
			//this.olv.AllColumns.Add(this.olvcParamValues);
			//this.olv.AllColumns.Add(this.olvcTotalPositions);
			//this.olv.AllColumns.Add(this.olvcTotalPositionsLocal);
			//this.olv.AllColumns.Add(this.olvcProfitPerPosition);
			//this.olv.AllColumns.Add(this.olvcProfitPerPositionLocal);
			//this.olv.AllColumns.Add(this.olvcNetProfit);
			//this.olv.AllColumns.Add(this.olvcNetProfitLocal);
			//this.olv.AllColumns.Add(this.olvcWinLoss);
			//this.olv.AllColumns.Add(this.olvcWinLossLocal);
			//this.olv.AllColumns.Add(this.olvcProfitFactor);
			//this.olv.AllColumns.Add(this.olvcProfitFactorLocal);
			//this.olv.AllColumns.Add(this.olvcRecoveryFactor);
			//this.olv.AllColumns.Add(this.olvcRecoveryFactorLocal);
			//this.olv.AllColumns.Add(this.olvcMaxDrawdown);
			//this.olv.AllColumns.Add(this.olvcMaxDrawdownLocal);
			//this.olv.AllColumns.Add(this.olvcMaxConsecutiveWinners);
			//this.olv.AllColumns.Add(this.olvcMaxConsecutiveWinnersLocal);
			//this.olv.AllColumns.Add(this.olvcMaxConsecutiveLosers);
			//this.olv.AllColumns.Add(this.olvcMaxConsecutiveLosersLocal);

			//this.olv.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
			//this.olvcParamValues,

			//this.olvcTotalPositionsGlobal,
			//this.olvcTotalPositionsLocal,
			//this.olvcTotalPositionsDelta,
			//this.olvcMomentumsAverageTotalPositions,
			//this.olvcMomentumsDispersionTotalPositions,
			//this.olvcMomentumsVarianceTotalPositions,

			//this.olvcProfitPerPositionGlobal,
			//this.olvcProfitPerPositionLocal,
			//this.olvcProfitPerPositionDelta,
			//this.olvcMomentumsAverageProfitPerPosition,
			//this.olvcMomentumsDispersionProfitPerPosition,
			//this.olvcMomentumsDispersionProfitPerPosition,

			//this.olvcNetProfitGlobal,
			//this.olvcNetProfitLocal,
			//this.olvcNetProfitDelta,
			//this.olvcMomentumsAverageNetProfit,
			//this.olvcMomentumsDispersionNetProfit,
			//this.olvcMomentumsVarianceNetProfit,

			//this.olvcWinLossGlobal,
			//this.olvcWinLossLocal,
			//this.olvcWinLossDelta,
			//this.olvcMomentumsAverageWinLoss,
			//this.olvcMomentumsDispersionWinLoss,
			//this.olvcMomentumsVarianceWinLoss,

			//this.olvcProfitFactorGlobal,
			//this.olvcProfitFactorLocal,
			//this.olvcProfitFactorDelta,
			//this.olvcMomentumsAverageProfitFactor,
			//this.olvcMomentumsDispersionProfitFactor,
			//this.olvcMomentumsVarianceProfitFactor,

			//this.olvcRecoveryFactorGlobal,
			//this.olvcRecoveryFactorLocal,
			//this.olvcRecoveryFactorDelta,
			//this.olvcMomentumsAverageRecoveryFactor,
			//this.olvcMomentumsDispersionRecoveryFactor,
			//this.olvcMomentumsVarianceRecoveryFactor,

			//this.olvcMaxDrawdownGlobal,
			//this.olvcMaxDrawdownLocal,
			//this.olvcMaxDrawdownDelta,
			//this.olvcMomentumsAverageMaxDrawdown,
			//this.olvcMomentumsDispersionMaxDrawdown,
			//this.olvcMomentumsVarianceMaxDrawdown,

			//this.olvcMaxConsecutiveWinnersGlobal,
			//this.olvcMaxConsecutiveWinnersLocal,
			//this.olvcMaxConsecutiveWinnersDelta,
			//this.olvcMomentumsAverageMaxConsecutiveWinners,
			//this.olvcMomentumsDispersionMaxConsecutiveWinners,
			//this.olvcMomentumsVarianceMaxConsecutiveWinners,

			//this.olvcMaxConsecutiveLosersGlobal,
			//this.olvcMaxConsecutiveLosersLocal,
			//this.olvcMaxConsecutiveLosersDelta,
			//this.olvcMomentumsAverageMaxConsecutiveLosers,
			//this.olvcMomentumsDispersionMaxConsecutiveLosers,
			//this.olvcMomentumsVarianceMaxConsecutiveLosers
			//});

			//this.olv.AllColumns.Add(this.olvcParamValues);
			//this.olv.AllColumns.Add(this.olvcTotalPositionsGlobal);
			//this.olv.AllColumns.Add(this.olvcTotalPositionsLocal);
			//this.olv.AllColumns.Add(this.olvcTotalPositionsDelta);
			//this.olv.AllColumns.Add(this.olvcTotalPositionsGlobal);
			//this.olv.AllColumns.Add(this.olvcTotalPositionsLocal);
			//this.olv.AllColumns.Add(this.olvcTotalPositionsDelta);
			//this.olv.AllColumns.Add(this.olvcProfitPerPositionGlobal);
			//this.olv.AllColumns.Add(this.olvcProfitPerPositionLocal);
			//this.olv.AllColumns.Add(this.olvcProfitPerPositionDelta);
			//this.olv.AllColumns.Add(this.olvcNetProfitGlobal);
			//this.olv.AllColumns.Add(this.olvcNetProfitLocal);
			//this.olv.AllColumns.Add(this.olvcNetProfitDelta);
			//this.olv.AllColumns.Add(this.olvcWinLossGlobal);
			//this.olv.AllColumns.Add(this.olvcWinLossLocal);
			//this.olv.AllColumns.Add(this.olvcWinLossDelta);
			//this.olv.AllColumns.Add(this.olvcProfitFactorGlobal);
			//this.olv.AllColumns.Add(this.olvcProfitFactorLocal);
			//this.olv.AllColumns.Add(this.olvcProfitFactorDelta);
			//this.olv.AllColumns.Add(this.olvcRecoveryFactorGlobal);
			//this.olv.AllColumns.Add(this.olvcRecoveryFactorLocal);
			//this.olv.AllColumns.Add(this.olvcRecoveryFactorDelta);
			//this.olv.AllColumns.Add(this.olvcMaxDrawdownGlobal);
			//this.olv.AllColumns.Add(this.olvcMaxDrawdownLocal);
			//this.olv.AllColumns.Add(this.olvcMaxDrawdownDelta);
			//this.olv.AllColumns.Add(this.olvcMaxConsecutiveWinnersGlobal);
			//this.olv.AllColumns.Add(this.olvcMaxConsecutiveWinnersLocal);
			//this.olv.AllColumns.Add(this.olvcMaxConsecutiveWinnersDelta);
			//this.olv.AllColumns.Add(this.olvcMaxConsecutiveLosersGlobal);
			//this.olv.AllColumns.Add(this.olvcMaxConsecutiveLosersLocal);
			//this.olv.AllColumns.Add(this.olvcMaxConsecutiveLosersDelta);


			this.mniToCheckForMaximizationCriterion = new Dictionary<MaximizationCriterion,		ToolStripMenuItem>();
			this.mniToCheckForMaximizationCriterion.Add(MaximizationCriterion.PositionsCount,		this.mniMaximiseDeltaTotalPositions);
			this.mniToCheckForMaximizationCriterion.Add(MaximizationCriterion.PositionAvgProfit,	this.mniMaximiseDeltaProfitPerPosition);
			this.mniToCheckForMaximizationCriterion.Add(MaximizationCriterion.NetProfit,			this.mniMaximiseDeltaNet);
			this.mniToCheckForMaximizationCriterion.Add(MaximizationCriterion.WinLossRatio,			this.mniMaximiseDeltaWinLoss);
			this.mniToCheckForMaximizationCriterion.Add(MaximizationCriterion.ProfitFactor,			this.mniMaximiseDeltaProfitFactor);
			this.mniToCheckForMaximizationCriterion.Add(MaximizationCriterion.RecoveryFactor,		this.mniMaximiseDeltaRecoveryFactor);
			this.mniToCheckForMaximizationCriterion.Add(MaximizationCriterion.MaxDrawDown,			this.mniMaximiseDeltaMaxDrawdown);
			this.mniToCheckForMaximizationCriterion.Add(MaximizationCriterion.MaxConsecWinners,		this.mniMaximiseDeltaMaxConsecutiveWinners);
			this.mniToCheckForMaximizationCriterion.Add(MaximizationCriterion.MaxConsecLosers,		this.mniMaximiseDeltaMaxConsecutiveLosers);

			this.buildMniFilteringAfterInitializeComponent();
			this.buildMniSortingAfterInitializeComponent();

			string msg = "should see parametrized ctor() upstack";
			//WILL_THROW_EXCEPTIOINS_FOR_IS_DISPOSED__FATAL Assembler.PopupException(msg);

		}
		void buildMniSortingAfterInitializeComponent() {
			this.columnToSortDescendingByMaximizationMni = new Dictionary<ToolStripMenuItem, OLVColumn>();

			this.columnToSortDescendingByMaximizationMni.Add(this.mniMaximiseDeltaTotalPositions,			this.olvcTotalPositionsDelta);
			this.columnToSortDescendingByMaximizationMni.Add(this.mniMaximiseDeltaProfitPerPosition,		this.olvcProfitPerPositionDelta);
			this.columnToSortDescendingByMaximizationMni.Add(this.mniMaximiseDeltaNet,						this.olvcNetProfitDelta);
			this.columnToSortDescendingByMaximizationMni.Add(this.mniMaximiseDeltaWinLoss,					this.olvcWinLossDelta);
			this.columnToSortDescendingByMaximizationMni.Add(this.mniMaximiseDeltaProfitFactor,				this.olvcProfitFactorDelta);
			this.columnToSortDescendingByMaximizationMni.Add(this.mniMaximiseDeltaRecoveryFactor,			this.olvcRecoveryFactorDelta);
			this.columnToSortDescendingByMaximizationMni.Add(this.mniMaximiseDeltaMaxDrawdown,				this.olvcMaxDrawdownDelta);
			this.columnToSortDescendingByMaximizationMni.Add(this.mniMaximiseDeltaMaxConsecutiveWinners,	this.olvcMaxConsecutiveWinnersDelta);
			this.columnToSortDescendingByMaximizationMni.Add(this.mniMaximiseDeltaMaxConsecutiveLosers,		this.olvcMaxConsecutiveLosersDelta);

			// sort by column that is Selected in Designer
			foreach (ToolStripMenuItem mni in this.columnsByFilter.Keys) {
				List<OLVColumn> columns = this.columnsByFilter[mni];
				foreach (OLVColumn column in columns) column.IsVisible = mni.Checked;
			}
			this.olv.RebuildColumns();

			this.mniMaximiseDeltaTotalPositions			.Click += new EventHandler(mniMaximizationKPISortDescendingBy_Click);
			this.mniMaximiseDeltaProfitPerPosition		.Click += new EventHandler(mniMaximizationKPISortDescendingBy_Click);
			this.mniMaximiseDeltaNet					.Click += new EventHandler(mniMaximizationKPISortDescendingBy_Click);
			this.mniMaximiseDeltaWinLoss				.Click += new EventHandler(mniMaximizationKPISortDescendingBy_Click);
			this.mniMaximiseDeltaProfitFactor			.Click += new EventHandler(mniMaximizationKPISortDescendingBy_Click);
			this.mniMaximiseDeltaRecoveryFactor			.Click += new EventHandler(mniMaximizationKPISortDescendingBy_Click);
			this.mniMaximiseDeltaMaxDrawdown			.Click += new EventHandler(mniMaximizationKPISortDescendingBy_Click);
			this.mniMaximiseDeltaMaxConsecutiveWinners	.Click += new EventHandler(mniMaximizationKPISortDescendingBy_Click);
			this.mniMaximiseDeltaMaxConsecutiveLosers	.Click += new EventHandler(mniMaximizationKPISortDescendingBy_Click);
		}

		void checkOneMniForMaximizationCriterionUncheckOthers(ToolStripMenuItem mniToCheck) {
			mniToCheck.Checked = true;
			foreach (ToolStripMenuItem mniEachDisableCheckedUncheckOthers in this.columnToSortDescendingByMaximizationMni.Keys) {
				if (mniEachDisableCheckedUncheckOthers == mniToCheck) {
					//DISABLED_HAS_NO_TICK_WEIRD mniEachDisableCheckedUncheckOthers.Enabled = false;
					continue;
				}
				//DISABLED_HAS_NO_TICK_WEIRD mniEachDisableCheckedUncheckOthers.Enabled = true;
				mniEachDisableCheckedUncheckOthers.Checked = false;
			}
		}

		void buildMniFilteringAfterInitializeComponent() {
			this.columnsByFilter = new Dictionary<ToolStripMenuItem, List<OLVColumn>>();

			this.mniShowAllBacktestedParams.Click += new EventHandler(this.mniShowColumnByFilter_Click);
			columnsByFilter.Add(this.mniShowAllBacktestedParams, new List<OLVColumn>() {
				this.olvcTotalPositionsGlobal,
				this.olvcProfitPerPositionGlobal,
				this.olvcNetProfitGlobal,
				this.olvcWinLossGlobal,
				this.olvcProfitFactorGlobal,
				this.olvcRecoveryFactorGlobal,
				this.olvcMaxDrawdownGlobal,
				this.olvcMaxConsecutiveWinnersGlobal,
				this.olvcMaxConsecutiveLosersGlobal
				});

			this.mniShowChosenParams.Click += new EventHandler(this.mniShowColumnByFilter_Click);
			columnsByFilter.Add(this.mniShowChosenParams, new List<OLVColumn>() {
				this.olvcTotalPositionsLocal,
				this.olvcProfitPerPositionLocal,
				this.olvcNetProfitLocal,
				this.olvcWinLossLocal,
				this.olvcProfitFactorLocal,
				this.olvcRecoveryFactorLocal,
				this.olvcMaxDrawdownLocal,
				this.olvcMaxConsecutiveWinnersLocal,
				this.olvcMaxConsecutiveLosersLocal
				});

			this.mniShowDeltasBtwAllAndChosenParams.Click += new EventHandler(this.mniShowColumnByFilter_Click);
			columnsByFilter.Add(this.mniShowDeltasBtwAllAndChosenParams, new List<OLVColumn>() {
				this.olvcTotalPositionsDelta,
				this.olvcProfitPerPositionDelta,
				this.olvcNetProfitDelta,
				this.olvcWinLossDelta,
				this.olvcProfitFactorDelta,
				this.olvcRecoveryFactorDelta,
				this.olvcMaxDrawdownDelta,
				this.olvcMaxConsecutiveWinnersDelta,
				this.olvcMaxConsecutiveLosersDelta
				});


			this.mniShowMomentumsAverage.Click += new EventHandler(this.mniShowColumnByFilter_Click);
			columnsByFilter.Add(this.mniShowMomentumsAverage, new List<OLVColumn>() {
				this.olvcMomentumsAverageTotalPositions,
				this.olvcMomentumsAverageProfitPerPosition,
				this.olvcMomentumsAverageNetProfit,
				this.olvcMomentumsAverageWinLoss,
				this.olvcMomentumsAverageProfitFactor,
				this.olvcMomentumsAverageRecoveryFactor,
				this.olvcMomentumsAverageMaxDrawdown,
				this.olvcMomentumsAverageMaxConsecutiveWinners,
				this.olvcMomentumsAverageMaxConsecutiveLosers
				});

			this.mniShowMomentumsDispersionGlobal.Click += new EventHandler(this.mniShowColumnByFilter_Click);
			columnsByFilter.Add(this.mniShowMomentumsDispersionGlobal, new List<OLVColumn>() {
				this.olvcMomentumsDispersionGlobalTotalPositions,
				this.olvcMomentumsDispersionGlobalProfitPerPosition,
				this.olvcMomentumsDispersionGlobalNetProfit,
				this.olvcMomentumsDispersionGlobalWinLoss,
				this.olvcMomentumsDispersionGlobalProfitFactor,
				this.olvcMomentumsDispersionGlobalRecoveryFactor,
				this.olvcMomentumsDispersionGlobalMaxDrawdown,
				this.olvcMomentumsDispersionGlobalMaxConsecutiveWinners,
				this.olvcMomentumsDispersionGlobalMaxConsecutiveLosers
				});

			this.mniShowMomentumsDispersionLocal.Click += new EventHandler(this.mniShowColumnByFilter_Click);
			columnsByFilter.Add(this.mniShowMomentumsDispersionLocal, new List<OLVColumn>() {
				this.olvcMomentumsDispersionLocalTotalPositions,
				this.olvcMomentumsDispersionLocalProfitPerPosition,
				this.olvcMomentumsDispersionLocalNetProfit,
				this.olvcMomentumsDispersionLocalWinLoss,
				this.olvcMomentumsDispersionLocalProfitFactor,
				this.olvcMomentumsDispersionLocalRecoveryFactor,
				this.olvcMomentumsDispersionLocalMaxDrawdown,
				this.olvcMomentumsDispersionLocalMaxConsecutiveWinners,
				this.olvcMomentumsDispersionLocalMaxConsecutiveLosers
				});

			this.mniShowMomentumsDispersionDelta.Click += new EventHandler(this.mniShowColumnByFilter_Click);
			columnsByFilter.Add(this.mniShowMomentumsDispersionDelta, new List<OLVColumn>() {
				this.olvcMomentumsDispersionDeltaTotalPositions,
				this.olvcMomentumsDispersionDeltaProfitPerPosition,
				this.olvcMomentumsDispersionDeltaNetProfit,
				this.olvcMomentumsDispersionDeltaWinLoss,
				this.olvcMomentumsDispersionDeltaProfitFactor,
				this.olvcMomentumsDispersionDeltaRecoveryFactor,
				this.olvcMomentumsDispersionDeltaMaxDrawdown,
				this.olvcMomentumsDispersionDeltaMaxConsecutiveWinners,
				this.olvcMomentumsDispersionDeltaMaxConsecutiveLosers
				});
	
			this.mniShowMomentumsVariance.Click += new EventHandler(this.mniShowColumnByFilter_Click);
			columnsByFilter.Add(this.mniShowMomentumsVariance, new List<OLVColumn>() {
				this.olvcMomentumsVarianceTotalPositions,
				this.olvcMomentumsVarianceProfitPerPosition,
				this.olvcMomentumsVarianceNetProfit,
				this.olvcMomentumsVarianceWinLoss,
				this.olvcMomentumsVarianceProfitFactor,
				this.olvcMomentumsVarianceRecoveryFactor,
				this.olvcMomentumsVarianceMaxDrawdown,
				this.olvcMomentumsVarianceMaxConsecutiveWinners,
				this.olvcMomentumsVarianceMaxConsecutiveLosers
				});

			//this.propagateColumnVisibilityFromMni();
			// correlator will be NPE this.olvStateBinaryRestoreAllValuesForOneParam();
		}
		void propagateColumnVisibilityFromMni() {
			// hide columns that aren't Checked in Designer
			foreach (ToolStripMenuItem mni in this.columnsByFilter.Keys) {
				List<OLVColumn> columns = this.columnsByFilter[mni];
				foreach (OLVColumn column in columns) {
					if (column.IsVisible == mni.Checked) continue;
					column.IsVisible = mni.Checked;
				}
			}
			this.olv.RebuildColumns();
		}

		bool dontSerializeStrategy_ImAligingInCtor = false;
		public OneParameterControl(CorrelatorControl allParametersControl, OneParameterAllValuesAveraged parameter) : this() {
			this.allParametersControl = allParametersControl;
			this.Parameter = parameter;
			//MOVED_DOWN_populateKPIsToParamColumnHeader() this.olvcParamValues.Text = this.Parameter.ParameterName;
			this.olvAllValuesForOneParamCustomize();

			this.dontSerializeStrategy_ImAligingInCtor = true;
			this.olv.SetObjects(this.Parameter.AllValuesWithArtificials);
			parameter.OnParameterRecalculatedLocalsAndDeltas += new EventHandler<OneParameterAllValuesAveragedEventArgs>(parameter_ParameterRecalculatedLocalsAndDeltas);
			base.Initialize_byMovingControlsToInner();
			this.AlignBaseSizeToDisplayedCells();
			this.populateKPIsToParamColumnHeader();
			this.dontSerializeStrategy_ImAligingInCtor = false;
		}

		void Initialize() {
			if (this.mniToCheckForMaximizationCriterion.ContainsKey(this.Parameter.MaximizationCriterion)) {
				ToolStripMenuItem mniToCheck = this.mniToCheckForMaximizationCriterion[this.Parameter.MaximizationCriterion];
				this.checkOneMniForMaximizationCriterionUncheckOthers(mniToCheck);
				OLVColumn sortBy = this.columnToSortDescendingByMaximizationMni[mniToCheck];
				this.olv.Sort(sortBy, SortOrder.Descending);
			}

			this.KPIsLocalRecalculateDone_refreshOLV();
		}

		internal void KPIsLocalRecalculateDone_refreshOLV() {
			if (this.Parameter == null) {
				string msg = "DONT_USE_DESIGNER_PURPOSED_DEFAULT_CONTROL "
					+ " YOU_SHOULD_CALL_PROPER_CTOR public OneParameterControl(CorrelatorControl allParametersControl, OneParameterAllValuesAveraged parameter)"
					+ " //KPIsLocalRecalculateDone_refreshOLV()";
				Assembler.PopupException(msg, null, false);
				return;
			}
			if (this.olv.IsDisposed == true) {
				string msg = "CREATE_OLV_AFTER_YOU_CLOSED_CORELLATOR_FORM"
					+ " //KPIsLocalRecalculateDone_refreshOLV()";
				Assembler.PopupException(msg, null, false);
				return;
			}
			this.olv.SetObjects(this.Parameter.AllValuesWithArtificials, true);
			this.olv.UseWaitCursor = false;
		}
		public void AlignBaseSizeToDisplayedCells() {
			int oneRowFullHeight = this.olv.RowHeightEffective;
			oneRowFullHeight += (this.olv.CellPadding != null)
						? this.olv.CellPadding.Value.Top + this.olv.CellPadding.Value.Bottom
						: 2;
			if (this.Parameter == null) {
				string msg = "DONT_USE_DESIGNER_PURPOSED_DEFAULT_CONTROL "
					+ " YOU_SHOULD_CALL_PROPER_CTOR public OneParameterControl(CorrelatorControl allParametersControl, OneParameterAllValuesAveraged parameter)"
					+ " //AlignBaseSizeToDisplayedCells()";
				Assembler.PopupException(msg, null, false);
				return;
			}
			//int rowsPlusArtificials = this.Parameter.OneParamOneValueByValues.Count + 2;	// 3		//REPLACE_WITH_max(avg(Net)),min(StDev(Net))_ALIGNED_WITH_MaximizationCriterion 
			int rowsPlusArtificials = this.Parameter.AllValuesWithArtificials.Count;
			int parentResizeableBordersTopBottom = base.PaddingMouseReceiving.Top + base.PaddingMouseReceiving.Bottom;
			int headerAssumedHeight = oneRowFullHeight - 4;		//28;	// enough to fit hscrollbar...
				headerAssumedHeight += 6;		//12;	// enough to fit hscrollbar...
			int visibleRowsHeight = oneRowFullHeight * rowsPlusArtificials + parentResizeableBordersTopBottom + headerAssumedHeight;
			base.Height = visibleRowsHeight;

			//int visibleRowsWidth	= this.olv.RowHeightEffective * this.parameter.ValuesByParam.Count + 20;
			if (this.olv.ColumnsInDisplayOrder.Count == 0) {
				string msg = "AVOIDING_NPE_DUE_TO_ZERO_COLUMNS_SHOWN";
				Assembler.PopupException(msg);
			}
			OLVColumn olvcLastVisible = this.olv.ColumnsInDisplayOrder[this.olv.ColumnsInDisplayOrder.Count - 1];
			Rectangle lastVisibleRectangle = this.olv.CalculateColumnVisibleBounds(base.ClientRectangle, olvcLastVisible);
			int parentResizeableBordersLeftRight = base.PaddingMouseReceiving.Left + base.PaddingMouseReceiving.Right;
			base.Width = lastVisibleRectangle.Right + parentResizeableBordersLeftRight;
		}
		void olvStateBinaryRestoreAllValuesForOneParam() {
			if (this.indicatorParameterNullUnsafe == null) {
				this.propagateColumnVisibilityFromMni();
				return;
			}

			try {
				if (this.correlator.CorrelatorDataSnapshot.ContainsKey(this.Parameter.ParameterName) == false) {
					this.correlator.CorrelatorDataSnapshot.Add(this.Parameter.ParameterName, new CorrelatorOneParameterSnapshot(this.Parameter.ParameterName));
				}
				CorrelatorOneParameterSnapshot snap = this.correlator.CorrelatorDataSnapshot[this.Parameter.ParameterName];

				this.mniShowAllBacktestedParams			.Checked = snap.MniShowAllBacktestsChecked;
				this.mniShowChosenParams				.Checked = snap.MniShowChosenChecked;
				this.mniShowDeltasBtwAllAndChosenParams	.Checked = snap.MniShowDeltaChecked;

				this.mniShowMomentumsAverage			.Checked = snap.MniShowMomentumsAverageChecked;
				this.mniShowMomentumsDispersionGlobal	.Checked = snap.MniShowMomentumsDispersionGlobalChecked;
				this.mniShowMomentumsDispersionLocal	.Checked = snap.MniShowMomentumsDispersionLocalChecked;
				this.mniShowMomentumsDispersionDelta	.Checked = snap.MniShowMomentumsDispersionDeltaChecked;
				this.mniShowMomentumsVariance			.Checked = snap.MniShowMomentumsVarianceChecked;

				this.propagateColumnVisibilityFromMni();

				// #1/2 OBJECTLISTVIEW_HACK__SEQUENCE_MATTERS!!!! otherwize RestoreState() doesn't restore after restart
				// adds columns to filter in the header (right click - unselect garbage columns); there might be some BrightIdeasSoftware.SyncColumnsToAllColumns()?...
				foreach (ColumnHeader columnHeader in this.olv.AllColumns) {
					OLVColumn oLVColumn = columnHeader as OLVColumn;
					if (oLVColumn == null) continue;
					oLVColumn.VisibilityChanged += oLVColumn_VisibilityChanged;
				}
				// #2/2 OBJECTLISTVIEW_HACK__SEQUENCE_MATTERS!!!! otherwize RestoreState() doesn't restore after restart
				if (snap.OlvStateBase64.Length > 0) {
					byte[] olvStateBinary = ObjectListViewStateSerializer.Base64Decode(snap.OlvStateBase64);
					this.olv.RestoreState(olvStateBinary);
				}
			} catch (Exception ex) {
				string msg = "CorrelatorDataSnapshot[" + this.Parameter.ParameterName + "] //olvStateBinaryRestoreAllValuesForOneParam()";
				Assembler.PopupException(msg, ex);
			}
		}
		void olvSaveBinaryState_SerializeSnapshot() {
			if (Assembler.InstanceInitialized.MainFormDockFormsFullyDeserializedLayoutComplete == false) return;
			if (this.dontRaiseContainerShouldSerializedForEachColumnVisibilityChanged_alreadyRaised) return;
			try {
				byte[] olvStateBinary = this.olv.SaveState();
				CorrelatorOneParameterSnapshot snap = this.correlator.CorrelatorDataSnapshot[this.Parameter.ParameterName];
				snap.OlvStateBase64 = ObjectListViewStateSerializer.Base64Encode(olvStateBinary);
				this.correlator.CorrelatorDataSnapshotSerializer.Serialize();
				//I_REMOVED_CORRELATOR_SNAP_FROM_PARAMETERS__NO_NEED_TO_SAVE_ANYMORE this.allParametersControl.Correlator.Executor.ChartShadow.RaiseContextScriptChangedContainerShouldSerialize();
			} catch (Exception ex) {
				string msg = "CorrelatorDataSnapshot[" + this.Parameter.ParameterName + "] //olvSaveBinaryState_SerializeSnapshot()";
				Assembler.PopupException(msg, ex);
			}
		}

		internal void OlvRebuildColumns() {
			this.olv.RebuildColumns();
		}
	}
}
