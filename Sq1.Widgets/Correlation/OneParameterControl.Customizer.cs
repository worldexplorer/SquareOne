using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

using BrightIdeasSoftware;
using Sq1.Core;
using Sq1.Core.Sequencing;
using Sq1.Core.Correlation;

namespace Sq1.Widgets.Correlation {
	public partial class OneParameterControl {
		Color colorBackgroundRed;
		Color colorBackgroundGreen;
		bool dontRaiseContainerShouldSerializedForEachColumnVisibilityChanged_alreadyRaised;
		
		void olv_FormatRow(object sender, FormatRowEventArgs e) {
			OneParameterOneValue oneParameterOneValue = e.Model as OneParameterOneValue;
			if (oneParameterOneValue == null) return;
			if (oneParameterOneValue.IsArtificialRow) {
				e.Item.BackColor = Color.Gainsboro;
				return;
			}
			//return;
			//e.Item.BackColor = (oneParameterOneValue.KPIsGlobal.NetProfit > 0.0) ? this.colorBackgroundGreen : this.colorBackgroundRed;
			e.Item.ForeColor = oneParameterOneValue.Chosen ? Color.Black : Color.DarkGray;
		}
		void olv_FormatCell(object sender, FormatCellEventArgs e) {
			OneParameterOneValue rowModel = e.Model as OneParameterOneValue;
			if (e.Column == this.olvcParamValues) return;

			if (e.Column == this.olvcNetProfitGlobal) {
				e.SubItem.BackColor = (rowModel.KPIsGlobal.NetProfit > 0.0) ? this.colorBackgroundGreen : this.colorBackgroundRed;
			} else if (e.Column == this.olvcProfitFactorGlobal) {
				e.SubItem.BackColor = (rowModel.KPIsGlobal.ProfitFactor > 1) ? this.colorBackgroundGreen : this.colorBackgroundRed;
			} else if (e.Column == this.olvcWinLossGlobal) {
				e.SubItem.BackColor = (rowModel.KPIsGlobal.WinLossRatio > 1) ? this.colorBackgroundGreen : this.colorBackgroundRed;
			} else if (e.Column == this.olvcProfitPerPositionGlobal) {
				e.SubItem.BackColor = (rowModel.KPIsGlobal.PositionAvgProfit > 0.0) ? this.colorBackgroundGreen : this.colorBackgroundRed;
			} else if (e.Column == this.olvcRecoveryFactorGlobal) {
				e.SubItem.BackColor = (rowModel.KPIsGlobal.RecoveryFactor > 1) ? this.colorBackgroundGreen : this.colorBackgroundRed;
			}

			if (e.Column == this.olvcNetProfitLocal) {
				e.SubItem.BackColor = (rowModel.KPIsLocal.NetProfit > 0.0) ? this.colorBackgroundGreen : this.colorBackgroundRed;
			} else if (e.Column == this.olvcProfitFactorLocal) {
				e.SubItem.BackColor = (rowModel.KPIsLocal.ProfitFactor > 1) ? this.colorBackgroundGreen : this.colorBackgroundRed;
			} else if (e.Column == this.olvcWinLossLocal) {
				e.SubItem.BackColor = (rowModel.KPIsLocal.WinLossRatio > 1) ? this.colorBackgroundGreen : this.colorBackgroundRed;
			} else if (e.Column == this.olvcProfitPerPositionLocal) {
				e.SubItem.BackColor = (rowModel.KPIsLocal.PositionAvgProfit > 0.0) ? this.colorBackgroundGreen : this.colorBackgroundRed;
			} else if (e.Column == this.olvcRecoveryFactorLocal) {
				e.SubItem.BackColor = (rowModel.KPIsLocal.RecoveryFactor > 1) ? this.colorBackgroundGreen : this.colorBackgroundRed;
			}

			if (e.Column == this.olvcTotalPositionsDelta) {
				e.SubItem.BackColor = (rowModel.KPIsDelta.PositionsCount == 0) ? Color.White
									: (rowModel.KPIsDelta.PositionsCount > 0) ? this.colorBackgroundGreen : this.colorBackgroundRed;
			} else if (e.Column == this.olvcNetProfitDelta) {
				e.SubItem.BackColor = (rowModel.KPIsDelta.NetProfit == 0) ? Color.White
									: (rowModel.KPIsDelta.NetProfit > 0) ? this.colorBackgroundGreen : this.colorBackgroundRed;
			} else  if (e.Column == this.olvcProfitPerPositionDelta) {
				e.SubItem.BackColor = (rowModel.KPIsDelta.PositionAvgProfit == 0) ? Color.White
									: (rowModel.KPIsDelta.PositionAvgProfit > 0) ? this.colorBackgroundGreen : this.colorBackgroundRed;
			} else if (e.Column == this.olvcProfitFactorDelta) {
				e.SubItem.BackColor = (rowModel.KPIsDelta.ProfitFactor == 0) ? Color.White
									: (rowModel.KPIsDelta.ProfitFactor > 0) ? this.colorBackgroundGreen : this.colorBackgroundRed;
			} else if (e.Column == this.olvcWinLossDelta) {
				e.SubItem.BackColor = (rowModel.KPIsDelta.WinLossRatio == 0) ? Color.White
									: (rowModel.KPIsDelta.WinLossRatio > 0) ? this.colorBackgroundGreen : this.colorBackgroundRed;
			} else if (e.Column == this.olvcRecoveryFactorDelta) {
				e.SubItem.BackColor = (rowModel.KPIsDelta.RecoveryFactor == 0) ? Color.White
									: (rowModel.KPIsDelta.RecoveryFactor > 0) ? this.colorBackgroundGreen : this.colorBackgroundRed;
			} else if (e.Column == this.olvcMaxDrawdownDelta) {
				e.SubItem.BackColor = (rowModel.KPIsDelta.MaxDrawDown == 0) ? Color.White
									: (rowModel.KPIsDelta.MaxDrawDown > 0) ? this.colorBackgroundRed : this.colorBackgroundGreen;
			} else if (e.Column == this.olvcMaxConsecutiveWinnersDelta) {
				e.SubItem.BackColor = (rowModel.KPIsDelta.MaxConsecWinners == 0) ? Color.White
									: (rowModel.KPIsDelta.MaxConsecWinners > 0) ? this.colorBackgroundGreen : this.colorBackgroundRed;
			} else if (e.Column == this.olvcMaxConsecutiveLosersDelta) {
				e.SubItem.BackColor = (rowModel.KPIsDelta.MaxConsecLosers == 0) ? Color.White
									: (rowModel.KPIsDelta.MaxConsecLosers > 0) ? this.colorBackgroundGreen : this.colorBackgroundRed;
			}

			#region KPIsMomentumsAverage
			if (e.Column == this.olvcMomentumsAverageTotalPositions) {
				e.SubItem.BackColor = (rowModel.KPIsMomentumsAverage.PositionsCount == 0) ? Color.White
									: (rowModel.KPIsMomentumsAverage.PositionsCount > 0) ? this.colorBackgroundGreen : this.colorBackgroundRed;
			} else if (e.Column == this.olvcMomentumsAverageNetProfit) {
				e.SubItem.BackColor = (rowModel.KPIsMomentumsAverage.NetProfit == 0) ? Color.White
									: (rowModel.KPIsMomentumsAverage.NetProfit > 0) ? this.colorBackgroundGreen : this.colorBackgroundRed;
			} else if (e.Column == this.olvcMomentumsAverageProfitPerPosition) {
				e.SubItem.BackColor = (rowModel.KPIsMomentumsAverage.PositionAvgProfit == 0) ? Color.White
									: (rowModel.KPIsMomentumsAverage.PositionAvgProfit > 0) ? this.colorBackgroundGreen : this.colorBackgroundRed;
			} else if (e.Column == this.olvcMomentumsAverageProfitFactor) {
				e.SubItem.BackColor = (rowModel.KPIsMomentumsAverage.ProfitFactor == 0) ? Color.White
									: (rowModel.KPIsMomentumsAverage.ProfitFactor > 0) ? this.colorBackgroundGreen : this.colorBackgroundRed;
			} else if (e.Column == this.olvcMomentumsAverageWinLoss) {
				e.SubItem.BackColor = (rowModel.KPIsMomentumsAverage.WinLossRatio == 0) ? Color.White
									: (rowModel.KPIsMomentumsAverage.WinLossRatio > 0) ? this.colorBackgroundGreen : this.colorBackgroundRed;
			} else if (e.Column == this.olvcMomentumsAverageRecoveryFactor) {
				e.SubItem.BackColor = (rowModel.KPIsMomentumsAverage.RecoveryFactor == 0) ? Color.White
									: (rowModel.KPIsMomentumsAverage.RecoveryFactor > 0) ? this.colorBackgroundGreen : this.colorBackgroundRed;
			} else if (e.Column == this.olvcMomentumsAverageMaxDrawdown) {
				e.SubItem.BackColor = (rowModel.KPIsMomentumsAverage.MaxDrawDown == 0) ? Color.White
									: (rowModel.KPIsMomentumsAverage.MaxDrawDown > 0) ? this.colorBackgroundRed : this.colorBackgroundGreen;
			} else if (e.Column == this.olvcMomentumsAverageMaxConsecutiveWinners) {
				e.SubItem.BackColor = (rowModel.KPIsMomentumsAverage.MaxConsecWinners == 0) ? Color.White
									: (rowModel.KPIsMomentumsAverage.MaxConsecWinners > 0) ? this.colorBackgroundGreen : this.colorBackgroundRed;
			} else if (e.Column == this.olvcMomentumsAverageMaxConsecutiveLosers) {
				e.SubItem.BackColor = (rowModel.KPIsMomentumsAverage.MaxConsecLosers == 0) ? Color.White
									: (rowModel.KPIsMomentumsAverage.MaxConsecLosers > 0) ? this.colorBackgroundGreen : this.colorBackgroundRed;
			}
			#endregion

			#region KPIsMomentumsDispersion
			if (e.Column == this.olvcMomentumsDispersionTotalPositions) {
				e.SubItem.BackColor = (rowModel.KPIsMomentumsDispersion.PositionsCount == 0) ? Color.White
									: (rowModel.KPIsMomentumsDispersion.PositionsCount > 0) ? this.colorBackgroundGreen : this.colorBackgroundRed;
			} else if (e.Column == this.olvcMomentumsDispersionNetProfit) {
				e.SubItem.BackColor = (rowModel.KPIsMomentumsDispersion.NetProfit == 0) ? Color.White
									: (rowModel.KPIsMomentumsDispersion.NetProfit > 0) ? this.colorBackgroundGreen : this.colorBackgroundRed;
			} else if (e.Column == this.olvcMomentumsDispersionProfitPerPosition) {
				e.SubItem.BackColor = (rowModel.KPIsMomentumsDispersion.PositionAvgProfit == 0) ? Color.White
									: (rowModel.KPIsMomentumsDispersion.PositionAvgProfit > 0) ? this.colorBackgroundGreen : this.colorBackgroundRed;
			} else if (e.Column == this.olvcMomentumsDispersionProfitFactor) {
				e.SubItem.BackColor = (rowModel.KPIsMomentumsDispersion.ProfitFactor == 0) ? Color.White
									: (rowModel.KPIsMomentumsDispersion.ProfitFactor > 0) ? this.colorBackgroundGreen : this.colorBackgroundRed;
			} else if (e.Column == this.olvcMomentumsDispersionWinLoss) {
				e.SubItem.BackColor = (rowModel.KPIsMomentumsDispersion.WinLossRatio == 0) ? Color.White
									: (rowModel.KPIsMomentumsDispersion.WinLossRatio > 0) ? this.colorBackgroundGreen : this.colorBackgroundRed;
			} else if (e.Column == this.olvcMomentumsDispersionRecoveryFactor) {
				e.SubItem.BackColor = (rowModel.KPIsMomentumsDispersion.RecoveryFactor == 0) ? Color.White
									: (rowModel.KPIsMomentumsDispersion.RecoveryFactor > 0) ? this.colorBackgroundGreen : this.colorBackgroundRed;
			} else if (e.Column == this.olvcMomentumsDispersionMaxDrawdown) {
				e.SubItem.BackColor = (rowModel.KPIsMomentumsDispersion.MaxDrawDown == 0) ? Color.White
									: (rowModel.KPIsMomentumsDispersion.MaxDrawDown > 0) ? this.colorBackgroundRed : this.colorBackgroundGreen;
			} else if (e.Column == this.olvcMomentumsDispersionMaxConsecutiveWinners) {
				e.SubItem.BackColor = (rowModel.KPIsMomentumsDispersion.MaxConsecWinners == 0) ? Color.White
									: (rowModel.KPIsMomentumsDispersion.MaxConsecWinners > 0) ? this.colorBackgroundGreen : this.colorBackgroundRed;
			} else if (e.Column == this.olvcMomentumsDispersionMaxConsecutiveLosers) {
				e.SubItem.BackColor = (rowModel.KPIsMomentumsDispersion.MaxConsecLosers == 0) ? Color.White
									: (rowModel.KPIsMomentumsDispersion.MaxConsecLosers > 0) ? this.colorBackgroundGreen : this.colorBackgroundRed;
			}
			#endregion

			#region KPIsMomentumsVariance
			if (e.Column == this.olvcMomentumsVarianceTotalPositions) {
				e.SubItem.BackColor = (rowModel.KPIsMomentumsVariance.PositionsCount == 0) ? Color.White
									: (rowModel.KPIsMomentumsVariance.PositionsCount > 0) ? this.colorBackgroundGreen : this.colorBackgroundRed;
			} else if (e.Column == this.olvcMomentumsVarianceNetProfit) {
				e.SubItem.BackColor = (rowModel.KPIsMomentumsVariance.NetProfit == 0) ? Color.White
									: (rowModel.KPIsMomentumsVariance.NetProfit > 0) ? this.colorBackgroundGreen : this.colorBackgroundRed;
			} else if (e.Column == this.olvcMomentumsVarianceProfitPerPosition) {
				e.SubItem.BackColor = (rowModel.KPIsMomentumsVariance.PositionAvgProfit == 0) ? Color.White
									: (rowModel.KPIsMomentumsVariance.PositionAvgProfit > 0) ? this.colorBackgroundGreen : this.colorBackgroundRed;
			} else if (e.Column == this.olvcMomentumsVarianceProfitFactor) {
				e.SubItem.BackColor = (rowModel.KPIsMomentumsVariance.ProfitFactor == 0) ? Color.White
									: (rowModel.KPIsMomentumsVariance.ProfitFactor > 0) ? this.colorBackgroundGreen : this.colorBackgroundRed;
			} else if (e.Column == this.olvcMomentumsVarianceWinLoss) {
				e.SubItem.BackColor = (rowModel.KPIsMomentumsVariance.WinLossRatio == 0) ? Color.White
									: (rowModel.KPIsMomentumsVariance.WinLossRatio > 0) ? this.colorBackgroundGreen : this.colorBackgroundRed;
			} else if (e.Column == this.olvcMomentumsVarianceRecoveryFactor) {
				e.SubItem.BackColor = (rowModel.KPIsMomentumsVariance.RecoveryFactor == 0) ? Color.White
									: (rowModel.KPIsMomentumsVariance.RecoveryFactor > 0) ? this.colorBackgroundGreen : this.colorBackgroundRed;
			} else if (e.Column == this.olvcMomentumsVarianceMaxDrawdown) {
				e.SubItem.BackColor = (rowModel.KPIsMomentumsVariance.MaxDrawDown == 0) ? Color.White
									: (rowModel.KPIsMomentumsVariance.MaxDrawDown > 0) ? this.colorBackgroundRed : this.colorBackgroundGreen;
			} else if (e.Column == this.olvcMomentumsVarianceMaxConsecutiveWinners) {
				e.SubItem.BackColor = (rowModel.KPIsMomentumsVariance.MaxConsecWinners == 0) ? Color.White
									: (rowModel.KPIsMomentumsVariance.MaxConsecWinners > 0) ? this.colorBackgroundGreen : this.colorBackgroundRed;
			} else if (e.Column == this.olvcMomentumsVarianceMaxConsecutiveLosers) {
				e.SubItem.BackColor = (rowModel.KPIsMomentumsVariance.MaxConsecLosers == 0) ? Color.White
									: (rowModel.KPIsMomentumsVariance.MaxConsecLosers > 0) ? this.colorBackgroundGreen : this.colorBackgroundRed;
			}
			#endregion
		}
		void olvAllValuesForOneParamCustomizeColors() {
			//if (this.snap.Colorify) {
				this.colorBackgroundRed = Color.FromArgb(255, 230, 230);
				this.colorBackgroundGreen = Color.FromArgb(230, 255, 230);
				this.olv.UseCellFormatEvents = true;
				this.olv.FormatRow += new EventHandler<FormatRowEventArgs>(olv_FormatRow);
				this.olv.FormatCell += new EventHandler<FormatCellEventArgs>(olv_FormatCell);
				this.olv.CellClick += new EventHandler<CellClickEventArgs>(olvAllValuesForOneParam_CellClick);
			//} else {
			//	this.olvPositions.UseCellFormatEvents = false;
			//	this.olvPositions.FormatRow -= new EventHandler<FormatRowEventArgs>(olvPositions_FormatRow);
			//}
		}

		void olvAllValuesForOneParam_CellClick(object sender, CellClickEventArgs e) {
			if (e.Column != this.olvcParamValues) return;

			OneParameterOneValue paramValueClicked = e.Model as OneParameterOneValue;
			this.olv.UseWaitCursor = true;
			this.correlator.ChooseThisOneResetOthers_RecalculateAllKPIsLocalAndDelta(paramValueClicked);
		}

		string formatterPriceFormatter(object cellValue) {
			if (cellValue is double == false) return cellValue.ToString();
			double asDouble = (double) cellValue;
			string doubleFormatted = asDouble.ToString(this.allParametersControl.PriceFormat);		// format is "0,000.0"
			return doubleFormatted;
		}
		string formatterKpiFormatter(object cellValue) {
			if (cellValue is double == false) return cellValue.ToString();
			double asDouble = (double)cellValue;
			string doubleFormatted = asDouble.ToString("N1");
			return doubleFormatted;
		}
		string formatterPriceFormatDelta_addPlusSign(object cellValue) {
			if (cellValue is double == false) return cellValue.ToString();
			double asDouble = (double)cellValue;
			string doubleFormatted = this.formatterPriceFormatter(asDouble);

			if (asDouble > 0			// can be negative for KPIsDelta
					&& double.IsInfinity(asDouble) == false
					&& double.IsNaN(asDouble) == false) doubleFormatted = "+" + doubleFormatted;

			return doubleFormatted;
		}
		string formatterKpiFormatDelta_addPlusSign(object cellValue) {
			if (cellValue is double == false) return cellValue.ToString();
			double asDouble = (double)cellValue;
			string doubleFormatted = this.formatterKpiFormatter(asDouble);

			if (asDouble > 0			// can be negative for KPIsDelta
					&& double.IsInfinity(asDouble) == false
					&& double.IsNaN(asDouble) == false) doubleFormatted = "+" + doubleFormatted;

			return doubleFormatted;
		}
		string formatMomentums = "{0:N1}";

		void olvAllValuesForOneParamCustomize() {
			this.olvAllValuesForOneParamCustomizeColors();
			//this.olvStateBinaryRestoreAllValuesForOneParam();

			this.olvcParamValues.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcParamValues.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.ParameterNameValue;
			};

			#region Global
			this.olvcTotalPositionsGlobal.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcTotalPositionsGlobal.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsGlobal.PositionsCount;
			};
			//this.olvcTotalPositionsGlobal.AspectToStringConverter = this.formatterPriceFormat;
			this.olvcTotalPositionsGlobal.AspectToStringFormat = "{0:N0}";

			this.olvcProfitPerPositionGlobal.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcProfitPerPositionGlobal.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsGlobal.PositionAvgProfit;
			};
			this.olvcProfitPerPositionGlobal.AspectToStringConverter = this.formatterPriceFormatter;

			this.olvcNetProfitGlobal.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcNetProfitGlobal.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsGlobal.NetProfit;
			};
			this.olvcNetProfitGlobal.AspectToStringConverter = this.formatterPriceFormatter;

			this.olvcWinLossGlobal.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcWinLossGlobal.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsGlobal.WinLossRatio;
			};
			this.olvcWinLossGlobal.AspectToStringConverter = this.formatterKpiFormatter;

			this.olvcProfitFactorGlobal.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcProfitFactorGlobal.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsGlobal.ProfitFactor;
			};
			this.olvcProfitFactorGlobal.AspectToStringConverter = this.formatterKpiFormatter;

			this.olvcRecoveryFactorGlobal.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcRecoveryFactorGlobal.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsGlobal.RecoveryFactor;
			};
			this.olvcRecoveryFactorGlobal.AspectToStringConverter = this.formatterKpiFormatter;

			this.olvcMaxDrawdownGlobal.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcMaxDrawdownGlobal.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsGlobal.MaxDrawDown;
			};
			this.olvcMaxDrawdownGlobal.AspectToStringConverter = this.formatterPriceFormatter;

			this.olvcMaxConsecutiveWinnersGlobal.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcMaxConsecutiveWinnersGlobal.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsGlobal.MaxConsecWinners;
			};
			//this.olvcMaxConsecutiveWinnersGlobal.AspectToStringConverter = this.formatterKpiFormat;
			this.olvcMaxConsecutiveWinnersGlobal.AspectToStringFormat = "{0:N0}";

			this.olvcMaxConsecutiveLosersGlobal.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcMaxConsecutiveLosersGlobal.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsGlobal.MaxConsecLosers;
			};
			//this.olvcMaxConsecutiveLosersGlobal.AspectToStringConverter = this.formatterKpiFormat;
			this.olvcMaxConsecutiveLosersGlobal.AspectToStringFormat = "{0:N0}";
			#endregion

			#region Local
			this.olvcTotalPositionsLocal.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcTotalPositionsLocal.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsLocal.PositionsCount;
			};
			//this.olvcTotalPositionsLocal.AspectToStringConverter = this.formatterPriceFormat;
			this.olvcTotalPositionsLocal.AspectToStringFormat = "{0:N0}";

			this.olvcProfitPerPositionLocal.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcProfitPerPositionLocal.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsLocal.PositionAvgProfit;
			};
			this.olvcProfitPerPositionLocal.AspectToStringConverter = this.formatterPriceFormatter;

			this.olvcNetProfitLocal.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcNetProfitLocal.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsLocal.NetProfit;
			};
			this.olvcNetProfitLocal.AspectToStringConverter = this.formatterPriceFormatter;

			this.olvcWinLossLocal.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcWinLossLocal.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsLocal.WinLossRatio;
			};
			this.olvcWinLossLocal.AspectToStringConverter = this.formatterKpiFormatter;

			this.olvcProfitFactorLocal.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcProfitFactorLocal.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsLocal.ProfitFactor;
			};
			this.olvcProfitFactorLocal.AspectToStringConverter = this.formatterKpiFormatter;

			this.olvcRecoveryFactorLocal.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcRecoveryFactorLocal.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsLocal.RecoveryFactor;
			};
			this.olvcRecoveryFactorLocal.AspectToStringConverter = this.formatterKpiFormatter;

			this.olvcMaxDrawdownLocal.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcMaxDrawdownLocal.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsLocal.MaxDrawDown;
			};
			this.olvcMaxDrawdownLocal.AspectToStringConverter = this.formatterPriceFormatter;

			this.olvcMaxConsecutiveWinnersLocal.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcMaxConsecutiveWinnersLocal.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsLocal.MaxConsecWinners;
			};
			//this.olvcMaxConsecutiveWinnersLocal.AspectToStringConverter = this.formatterPriceFormat;
			this.olvcMaxConsecutiveWinnersLocal.AspectToStringFormat = "{0:N0}";

			this.olvcMaxConsecutiveLosersLocal.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcMaxConsecutiveLosersLocal.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsLocal.MaxConsecLosers;
			};
			//this.olvcMaxConsecutiveLosersLocal.AspectToStringConverter = this.formatterPriceFormat;
			this.olvcMaxConsecutiveLosersLocal.AspectToStringFormat = "{0:N0}";
			#endregion

			#region Delta
			this.olvcTotalPositionsDelta.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcTotalPositionsDelta.AspectGetter: OneParameterOneValue=null";
				//return oneParameterOneValue.KPIsDelta.PositionsCountFormatted;
				return oneParameterOneValue.KPIsDelta.PositionsCount;
			};
			this.olvcTotalPositionsDelta.AspectToStringConverter = this.formatterKpiFormatDelta_addPlusSign;

			this.olvcProfitPerPositionDelta.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcProfitPerPositionDelta.AspectGetter: OneParameterOneValue=null";
				//return oneParameterOneValue.KPIsDelta.PositionAvgProfitFormatted;
				return oneParameterOneValue.KPIsDelta.PositionAvgProfit;
			};
			// PRINTED_N1 this.olvcProfitPerPositionDelta.AspectToStringFormat = "N1";
			this.olvcProfitPerPositionDelta.AspectToStringConverter = this.formatterPriceFormatDelta_addPlusSign;

			this.olvcNetProfitDelta.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcNetProfitDelta.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsDelta.NetProfit;
			};
			this.olvcNetProfitDelta.AspectToStringConverter = this.formatterPriceFormatDelta_addPlusSign;
			// DIDNT_MAKE_ANY_CHANGE this.olvcNetProfitDelta.DataType = typeof(int);

			this.olvcWinLossDelta.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcWinLossDelta.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsDelta.WinLossRatio;
			};
			this.olvcWinLossDelta.AspectToStringConverter = this.formatterKpiFormatDelta_addPlusSign;

			this.olvcProfitFactorDelta.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcProfitFactorDelta.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsDelta.ProfitFactor;
			};
			this.olvcProfitFactorDelta.AspectToStringConverter = this.formatterKpiFormatDelta_addPlusSign;

			this.olvcRecoveryFactorDelta.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcRecoveryFactorDelta.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsDelta.RecoveryFactor;
			};
			this.olvcRecoveryFactorDelta.AspectToStringConverter = this.formatterKpiFormatDelta_addPlusSign;

			this.olvcMaxDrawdownDelta.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcMaxDrawdownDelta.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsDelta.MaxDrawDown;
			};
			this.olvcMaxDrawdownDelta.AspectToStringConverter = this.formatterKpiFormatDelta_addPlusSign;

			this.olvcMaxConsecutiveWinnersDelta.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcMaxConsecutiveWinnersDelta.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsDelta.MaxConsecWinners;
			};
			this.olvcMaxConsecutiveWinnersDelta.AspectToStringConverter = this.formatterKpiFormatDelta_addPlusSign;

			this.olvcMaxConsecutiveLosersDelta.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcMaxConsecutiveLosersDelta.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsDelta.MaxConsecLosers;
			};
			this.olvcMaxConsecutiveLosersDelta.AspectToStringConverter = this.formatterKpiFormatDelta_addPlusSign;
			#endregion



			#region Momentums: Average
			this.olvcMomentumsAverageTotalPositions.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcMomentumsAverageTotalPositions.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsMomentumsAverage.PositionsCount;
			};
			this.olvcMomentumsAverageTotalPositions.AspectToStringFormat = this.formatMomentums;

			this.olvcMomentumsAverageProfitPerPosition.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcMomentumsAverageProfitPerPosition.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsMomentumsAverage.PositionAvgProfit;
			};
			this.olvcMomentumsAverageProfitPerPosition.AspectToStringFormat = this.formatMomentums;

			this.olvcMomentumsAverageNetProfit.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcMomentumsAverageNetProfit.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsMomentumsAverage.NetProfit;
			};
			this.olvcMomentumsAverageNetProfit.AspectToStringFormat = this.formatMomentums;

			this.olvcMomentumsAverageWinLoss.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcMomentumsAverageWinLoss.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsMomentumsAverage.WinLossRatio;
			};
			this.olvcMomentumsAverageWinLoss.AspectToStringFormat = this.formatMomentums;

			this.olvcMomentumsAverageProfitFactor.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcMomentumsAverageProfitFactor.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsMomentumsAverage.ProfitFactor;
			};
			this.olvcMomentumsAverageProfitFactor.AspectToStringFormat = this.formatMomentums;

			this.olvcMomentumsAverageRecoveryFactor.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcMomentumsAverageRecoveryFactor.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsMomentumsAverage.RecoveryFactor;
			};
			this.olvcMomentumsAverageRecoveryFactor.AspectToStringFormat = this.formatMomentums;

			this.olvcMomentumsAverageMaxDrawdown.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcMomentumsAverageMaxDrawdown.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsMomentumsAverage.MaxDrawDown;
			};
			this.olvcMomentumsAverageMaxDrawdown.AspectToStringFormat = this.formatMomentums;

			this.olvcMomentumsAverageMaxConsecutiveWinners.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcMomentumsAverageMaxConsecutiveWinners.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsMomentumsAverage.MaxConsecWinners;
			};
			this.olvcMomentumsAverageMaxConsecutiveWinners.AspectToStringFormat = this.formatMomentums;

			this.olvcMomentumsAverageMaxConsecutiveLosers.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcMomentumsAverageMaxConsecutiveLosers.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsMomentumsAverage.MaxConsecLosers;
			};
			this.olvcMomentumsAverageMaxConsecutiveLosers.AspectToStringFormat = this.formatMomentums;
			#endregion

			#region Momentums: Dispersion
			this.olvcMomentumsDispersionTotalPositions.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcMomentumsDispersionTotalPositions.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsMomentumsDispersion.PositionsCount;
			};
			this.olvcMomentumsDispersionTotalPositions.AspectToStringFormat = this.formatMomentums;

			this.olvcMomentumsDispersionProfitPerPosition.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcMomentumsDispersionProfitPerPosition.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsMomentumsDispersion.PositionAvgProfit;
			};
			this.olvcMomentumsDispersionProfitPerPosition.AspectToStringFormat = this.formatMomentums;

			this.olvcMomentumsDispersionNetProfit.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcMomentumsDispersionNetProfit.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsMomentumsDispersion.NetProfit;
			};
			this.olvcMomentumsDispersionNetProfit.AspectToStringFormat = this.formatMomentums;

			this.olvcMomentumsDispersionWinLoss.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcMomentumsDispersionWinLoss.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsMomentumsDispersion.WinLossRatio;
			};
			this.olvcMomentumsDispersionWinLoss.AspectToStringFormat = this.formatMomentums;

			this.olvcMomentumsDispersionProfitFactor.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcMomentumsDispersionProfitFactor.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsMomentumsDispersion.ProfitFactor;
			};
			this.olvcMomentumsDispersionProfitFactor.AspectToStringFormat = this.formatMomentums;

			this.olvcMomentumsDispersionRecoveryFactor.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcMomentumsDispersionRecoveryFactor.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsMomentumsDispersion.RecoveryFactor;
			};
			this.olvcMomentumsDispersionRecoveryFactor.AspectToStringFormat = this.formatMomentums;

			this.olvcMomentumsDispersionMaxDrawdown.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcMomentumsDispersionMaxDrawdown.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsMomentumsDispersion.MaxDrawDown;
			};
			this.olvcMomentumsDispersionMaxDrawdown.AspectToStringFormat = this.formatMomentums;

			this.olvcMomentumsDispersionMaxConsecutiveWinners.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcMomentumsDispersionMaxConsecutiveWinners.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsMomentumsDispersion.MaxConsecWinners;
			};
			this.olvcMomentumsDispersionMaxConsecutiveWinners.AspectToStringFormat = this.formatMomentums;

			this.olvcMomentumsDispersionMaxConsecutiveLosers.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcMomentumsDispersionMaxConsecutiveLosers.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsMomentumsDispersion.MaxConsecLosers;
			};
			this.olvcMomentumsDispersionMaxConsecutiveLosers.AspectToStringFormat = this.formatMomentums;
			#endregion

			#region Momentums: Variance
			this.olvcMomentumsVarianceTotalPositions.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcMomentumsVarianceMomentumsVarianceTotalPositionsDelta.AspectGetter: OneParameterOneValue=null";
				//return oneParameterOneValue.KPIsMomentumsVariance.PositionsCountFormatted;
				return oneParameterOneValue.KPIsMomentumsVariance.PositionsCount;
			};
			this.olvcMomentumsVarianceTotalPositions.AspectToStringFormat = this.formatMomentums;

			this.olvcMomentumsVarianceProfitPerPosition.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcMomentumsVarianceMomentumsVarianceProfitPerPositionDelta.AspectGetter: OneParameterOneValue=null";
				//return oneParameterOneValue.KPIsMomentumsVariance.PositionAvgProfitFormatted;
				return oneParameterOneValue.KPIsMomentumsVariance.PositionAvgProfit;
			};
			// PRINTED_N1 this.olvcMomentumsVarianceProfitPerPosition.AspectToStringFormat = "N1";
			this.olvcMomentumsVarianceProfitPerPosition.AspectToStringFormat = this.formatMomentums;

			this.olvcMomentumsVarianceNetProfit.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcMomentumsVarianceMomentumsVarianceNetProfitDelta.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsMomentumsVariance.NetProfit;
			};
			this.olvcMomentumsVarianceNetProfit.AspectToStringFormat = this.formatMomentums;
			// DIDNT_MAKE_ANY_CHANGE this.olvcMomentumsVarianceNetProfit.DataType = typeof(int);

			this.olvcMomentumsVarianceWinLoss.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcMomentumsVarianceMomentumsVarianceWinLossDelta.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsMomentumsVariance.WinLossRatio;
			};
			this.olvcMomentumsVarianceWinLoss.AspectToStringFormat = this.formatMomentums;

			this.olvcMomentumsVarianceProfitFactor.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcMomentumsVarianceMomentumsVarianceProfitFactorDelta.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsMomentumsVariance.ProfitFactor;
			};
			this.olvcMomentumsVarianceProfitFactor.AspectToStringFormat = this.formatMomentums;

			this.olvcMomentumsVarianceRecoveryFactor.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcMomentumsVarianceMomentumsVarianceRecoveryFactorDelta.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsMomentumsVariance.RecoveryFactor;
			};
			this.olvcMomentumsVarianceRecoveryFactor.AspectToStringFormat = this.formatMomentums;

			this.olvcMomentumsVarianceMaxDrawdown.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcMomentumsVarianceMomentumsVarianceMaxDrawdownDelta.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsMomentumsVariance.MaxDrawDown;
			};
			this.olvcMomentumsVarianceMaxDrawdown.AspectToStringFormat = this.formatMomentums;

			this.olvcMomentumsVarianceMaxConsecutiveWinners.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcMomentumsVarianceMomentumsVarianceMaxConsecutiveWinnersDelta.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsMomentumsVariance.MaxConsecWinners;
			};
			this.olvcMomentumsVarianceMaxConsecutiveWinners.AspectToStringFormat = this.formatMomentums;

			this.olvcMomentumsVarianceMaxConsecutiveLosers.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcMomentumsVarianceMomentumsVarianceMaxConsecutiveLosersDelta.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsMomentumsVariance.MaxConsecLosers;
			};
			this.olvcMomentumsVarianceMaxConsecutiveLosers.AspectToStringFormat = this.formatMomentums;
			#endregion


			
			// USED_AspectToStringConverter_INSTEAD_OF_CUSTOM_SORTING
			//this.olvAllValuesForOneParam.CustomSorter = delegate(OLVColumn column, SortOrder order) {
			//	if (column == this.olvcMaxDrawdownDelta) {
			//		this.olvAllValuesForOneParam.ListViewItemSorter = new NumberComparer(column, order);
			//	}
			//};

			this.olv.CheckStateGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return CheckState.Indeterminate;
				return oneParameterOneValue.Chosen ? CheckState.Checked : CheckState.Unchecked;
			};
			this.olv.CheckStatePutter = delegate(object o, CheckState newState) {
				//if (this.IgnoreCheboxStatePutterImanuallyResetThem) return;
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return CheckState.Indeterminate;
				//bool clickWillCheckOnlyClickedCheckboxAndUncheckAllOthers
				//	= this.olvAllValuesForOneParam.CheckIndeterminateHeaderCheckBox(this.olvcParamValues);
				// GUI thread will invoke Putters asynchronously, you can't control them in a linear manner :(((
				//if (clickWillCheckOnlyClickedCheckboxAndUncheckAllOthers == true) {
				//    this.IgnoreCheboxStatePutterImanuallyResetThem = true;
				//    this.IgnoreCheboxStatePutterImanuallyResetThem = true;
				//}
				oneParameterOneValue.Chosen = newState.CompareTo(CheckState.Checked) == 0;
				this.olv.RefreshObject(oneParameterOneValue);
				this.olv.UseWaitCursor = true;
				this.correlator.OneParameterOneValueUserSelectedChanged_recalculateAllKPIsLocal(oneParameterOneValue);
				return newState;
			};

			// TOO_MANY_STRATEGY_SERIALIZATIONS__INTRODUCED_dontSerializeStrategy_ImAligingInCtor
			this.olv.ColumnWidthChanged += new ColumnWidthChangedEventHandler(olv_ColumnWidthChanged);
		}
		void olv_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e) {
			if (this.dontSerializeStrategy_ImAligingInCtor) return;
			this.olvStateBinaryStateSaveAndRaiseStrategySerialize();
			this.AlignBaseSizeToDisplayedCells();
		}
		void oLVColumn_VisibilityChanged(object sender, EventArgs e) {
			if (this.indicatorParameterNullUnsafe == null) return;
			OLVColumn oLVColumn = sender as OLVColumn;
			if (oLVColumn == null) return;
			this.olvStateBinaryStateSaveAndRaiseStrategySerialize();
		}
	}
	// USED_AspectToStringConverter_INSTEAD_OF_CUSTOM_SORTING
	//class NumberComparer : IComparer {
	//    SortOrder order;
	//    OLVColumn column;
	//    public NumberComparer(OLVColumn column, SortOrder order) {
	//        this.column = column;
	//        this.order = order;
	//    }
	//    public int Compare(object x, object y) {
	//        OLVListItem row1 = x as OLVListItem;
	//        OLVListItem row2 = y as OLVListItem;

	//        OneParameterOneValue cell1 = row1.RowObject as OneParameterOneValue;
	//        OneParameterOneValue cell2 = row2.RowObject as OneParameterOneValue;

	//        int forAscending = cell1.KPIsDelta.MaxDrawDown.CompareTo(cell2.KPIsDelta.MaxDrawDown);
	//        if (this.order == SortOrder.Descending) return forAscending *= -1;
	//        return forAscending;
	//    }
	//}
}
