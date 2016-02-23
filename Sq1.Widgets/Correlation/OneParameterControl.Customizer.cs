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
									: (rowModel.KPIsDelta.PositionsCount < 0) ? this.colorBackgroundGreen : this.colorBackgroundRed;
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
				e.SubItem.BackColor = (rowModel.KPIsMomentumAverage.PositionsCount == 0) ? Color.White
									: (rowModel.KPIsMomentumAverage.PositionsCount > 0) ? this.colorBackgroundGreen : this.colorBackgroundRed;
			} else if (e.Column == this.olvcMomentumsAverageNetProfit) {
				e.SubItem.BackColor = (rowModel.KPIsMomentumAverage.NetProfit == 0) ? Color.White
									: (rowModel.KPIsMomentumAverage.NetProfit > 0) ? this.colorBackgroundGreen : this.colorBackgroundRed;
			} else if (e.Column == this.olvcMomentumsAverageProfitPerPosition) {
				e.SubItem.BackColor = (rowModel.KPIsMomentumAverage.PositionAvgProfit == 0) ? Color.White
									: (rowModel.KPIsMomentumAverage.PositionAvgProfit > 0) ? this.colorBackgroundGreen : this.colorBackgroundRed;
			} else if (e.Column == this.olvcMomentumsAverageProfitFactor) {
				e.SubItem.BackColor = (rowModel.KPIsMomentumAverage.ProfitFactor == 0) ? Color.White
									: (rowModel.KPIsMomentumAverage.ProfitFactor > 0) ? this.colorBackgroundGreen : this.colorBackgroundRed;
			} else if (e.Column == this.olvcMomentumsAverageWinLoss) {
				e.SubItem.BackColor = (rowModel.KPIsMomentumAverage.WinLossRatio == 0) ? Color.White
									: (rowModel.KPIsMomentumAverage.WinLossRatio > 0) ? this.colorBackgroundGreen : this.colorBackgroundRed;
			} else if (e.Column == this.olvcMomentumsAverageRecoveryFactor) {
				e.SubItem.BackColor = (rowModel.KPIsMomentumAverage.RecoveryFactor == 0) ? Color.White
									: (rowModel.KPIsMomentumAverage.RecoveryFactor > 0) ? this.colorBackgroundGreen : this.colorBackgroundRed;
			} else if (e.Column == this.olvcMomentumsAverageMaxDrawdown) {
				e.SubItem.BackColor = (rowModel.KPIsMomentumAverage.MaxDrawDown == 0) ? Color.White
									: (rowModel.KPIsMomentumAverage.MaxDrawDown > 0) ? this.colorBackgroundRed : this.colorBackgroundGreen;
			} else if (e.Column == this.olvcMomentumsAverageMaxConsecutiveWinners) {
				e.SubItem.BackColor = (rowModel.KPIsMomentumAverage.MaxConsecWinners == 0) ? Color.White
									: (rowModel.KPIsMomentumAverage.MaxConsecWinners > 0) ? this.colorBackgroundGreen : this.colorBackgroundRed;
			} else if (e.Column == this.olvcMomentumsAverageMaxConsecutiveLosers) {
				e.SubItem.BackColor = (rowModel.KPIsMomentumAverage.MaxConsecLosers == 0) ? Color.White
									: (rowModel.KPIsMomentumAverage.MaxConsecLosers > 0) ? this.colorBackgroundGreen : this.colorBackgroundRed;
			}
			#endregion

			#region KPIsMomentumsDispersionGlobal
			if (e.Column == this.olvcMomentumsDispersionGlobalTotalPositions) {
				e.SubItem.BackColor = (rowModel.KPIsMomentumDispersionGlobal.PositionsCount == 0) ? Color.White
									: (rowModel.KPIsMomentumDispersionGlobal.PositionsCount > 0) ? this.colorBackgroundGreen : this.colorBackgroundRed;
			} else if (e.Column == this.olvcMomentumsDispersionGlobalNetProfit) {
				e.SubItem.BackColor = (rowModel.KPIsMomentumDispersionGlobal.NetProfit == 0) ? Color.White
									: (rowModel.KPIsMomentumDispersionGlobal.NetProfit > 0) ? this.colorBackgroundGreen : this.colorBackgroundRed;
			} else if (e.Column == this.olvcMomentumsDispersionGlobalProfitPerPosition) {
				e.SubItem.BackColor = (rowModel.KPIsMomentumDispersionGlobal.PositionAvgProfit == 0) ? Color.White
									: (rowModel.KPIsMomentumDispersionGlobal.PositionAvgProfit > 0) ? this.colorBackgroundGreen : this.colorBackgroundRed;
			} else if (e.Column == this.olvcMomentumsDispersionGlobalProfitFactor) {
				e.SubItem.BackColor = (rowModel.KPIsMomentumDispersionGlobal.ProfitFactor == 0) ? Color.White
									: (rowModel.KPIsMomentumDispersionGlobal.ProfitFactor > 0) ? this.colorBackgroundGreen : this.colorBackgroundRed;
			} else if (e.Column == this.olvcMomentumsDispersionGlobalWinLoss) {
				e.SubItem.BackColor = (rowModel.KPIsMomentumDispersionGlobal.WinLossRatio == 0) ? Color.White
									: (rowModel.KPIsMomentumDispersionGlobal.WinLossRatio > 0) ? this.colorBackgroundGreen : this.colorBackgroundRed;
			} else if (e.Column == this.olvcMomentumsDispersionGlobalRecoveryFactor) {
				e.SubItem.BackColor = (rowModel.KPIsMomentumDispersionGlobal.RecoveryFactor == 0) ? Color.White
									: (rowModel.KPIsMomentumDispersionGlobal.RecoveryFactor > 0) ? this.colorBackgroundGreen : this.colorBackgroundRed;
			} else if (e.Column == this.olvcMomentumsDispersionGlobalMaxDrawdown) {
				e.SubItem.BackColor = (rowModel.KPIsMomentumDispersionGlobal.MaxDrawDown == 0) ? Color.White
									: (rowModel.KPIsMomentumDispersionGlobal.MaxDrawDown > 0) ? this.colorBackgroundRed : this.colorBackgroundGreen;
			} else if (e.Column == this.olvcMomentumsDispersionGlobalMaxConsecutiveWinners) {
				e.SubItem.BackColor = (rowModel.KPIsMomentumDispersionGlobal.MaxConsecWinners == 0) ? Color.White
									: (rowModel.KPIsMomentumDispersionGlobal.MaxConsecWinners > 0) ? this.colorBackgroundGreen : this.colorBackgroundRed;
			} else if (e.Column == this.olvcMomentumsDispersionGlobalMaxConsecutiveLosers) {
				e.SubItem.BackColor = (rowModel.KPIsMomentumDispersionGlobal.MaxConsecLosers == 0) ? Color.White
									: (rowModel.KPIsMomentumDispersionGlobal.MaxConsecLosers > 0) ? this.colorBackgroundGreen : this.colorBackgroundRed;
			}
			#endregion

			#region KPIsMomentumsDispersionLocal
			if (e.Column == this.olvcMomentumsDispersionLocalTotalPositions) {
				e.SubItem.BackColor = (rowModel.KPIsMomentumDispersionLocal.PositionsCount == 0) ? Color.White
									: (rowModel.KPIsMomentumDispersionLocal.PositionsCount > 0) ? this.colorBackgroundGreen : this.colorBackgroundRed;
			} else if (e.Column == this.olvcMomentumsDispersionLocalNetProfit) {
				e.SubItem.BackColor = (rowModel.KPIsMomentumDispersionLocal.NetProfit == 0) ? Color.White
									: (rowModel.KPIsMomentumDispersionLocal.NetProfit > 0) ? this.colorBackgroundGreen : this.colorBackgroundRed;
			} else if (e.Column == this.olvcMomentumsDispersionLocalProfitPerPosition) {
				e.SubItem.BackColor = (rowModel.KPIsMomentumDispersionLocal.PositionAvgProfit == 0) ? Color.White
									: (rowModel.KPIsMomentumDispersionLocal.PositionAvgProfit > 0) ? this.colorBackgroundGreen : this.colorBackgroundRed;
			} else if (e.Column == this.olvcMomentumsDispersionLocalProfitFactor) {
				e.SubItem.BackColor = (rowModel.KPIsMomentumDispersionLocal.ProfitFactor == 0) ? Color.White
									: (rowModel.KPIsMomentumDispersionLocal.ProfitFactor > 0) ? this.colorBackgroundGreen : this.colorBackgroundRed;
			} else if (e.Column == this.olvcMomentumsDispersionLocalWinLoss) {
				e.SubItem.BackColor = (rowModel.KPIsMomentumDispersionLocal.WinLossRatio == 0) ? Color.White
									: (rowModel.KPIsMomentumDispersionLocal.WinLossRatio > 0) ? this.colorBackgroundGreen : this.colorBackgroundRed;
			} else if (e.Column == this.olvcMomentumsDispersionLocalRecoveryFactor) {
				e.SubItem.BackColor = (rowModel.KPIsMomentumDispersionLocal.RecoveryFactor == 0) ? Color.White
									: (rowModel.KPIsMomentumDispersionLocal.RecoveryFactor > 0) ? this.colorBackgroundGreen : this.colorBackgroundRed;
			} else if (e.Column == this.olvcMomentumsDispersionLocalMaxDrawdown) {
				e.SubItem.BackColor = (rowModel.KPIsMomentumDispersionLocal.MaxDrawDown == 0) ? Color.White
									: (rowModel.KPIsMomentumDispersionLocal.MaxDrawDown > 0) ? this.colorBackgroundRed : this.colorBackgroundGreen;
			} else if (e.Column == this.olvcMomentumsDispersionLocalMaxConsecutiveWinners) {
				e.SubItem.BackColor = (rowModel.KPIsMomentumDispersionLocal.MaxConsecWinners == 0) ? Color.White
									: (rowModel.KPIsMomentumDispersionLocal.MaxConsecWinners > 0) ? this.colorBackgroundGreen : this.colorBackgroundRed;
			} else if (e.Column == this.olvcMomentumsDispersionLocalMaxConsecutiveLosers) {
				e.SubItem.BackColor = (rowModel.KPIsMomentumDispersionLocal.MaxConsecLosers == 0) ? Color.White
									: (rowModel.KPIsMomentumDispersionLocal.MaxConsecLosers > 0) ? this.colorBackgroundGreen : this.colorBackgroundRed;
			}
			#endregion

			#region KPIsMomentumsDispersionDelta
			if (e.Column == this.olvcMomentumsDispersionDeltaTotalPositions) {
				e.SubItem.BackColor = (rowModel.KPIsMomentumDispersionDelta.PositionsCount == 0) ? Color.White
									: (rowModel.KPIsMomentumDispersionDelta.PositionsCount < 0) ? this.colorBackgroundGreen : this.colorBackgroundRed;
			} else if (e.Column == this.olvcMomentumsDispersionDeltaNetProfit) {
				e.SubItem.BackColor = (rowModel.KPIsMomentumDispersionDelta.NetProfit == 0) ? Color.White
									: (rowModel.KPIsMomentumDispersionDelta.NetProfit > 0) ? this.colorBackgroundGreen : this.colorBackgroundRed;
			} else if (e.Column == this.olvcMomentumsDispersionDeltaProfitPerPosition) {
				e.SubItem.BackColor = (rowModel.KPIsMomentumDispersionDelta.PositionAvgProfit == 0) ? Color.White
									: (rowModel.KPIsMomentumDispersionDelta.PositionAvgProfit > 0) ? this.colorBackgroundGreen : this.colorBackgroundRed;
			} else if (e.Column == this.olvcMomentumsDispersionDeltaProfitFactor) {
				e.SubItem.BackColor = (rowModel.KPIsMomentumDispersionDelta.ProfitFactor == 0) ? Color.White
									: (rowModel.KPIsMomentumDispersionDelta.ProfitFactor > 0) ? this.colorBackgroundGreen : this.colorBackgroundRed;
			} else if (e.Column == this.olvcMomentumsDispersionDeltaWinLoss) {
				e.SubItem.BackColor = (rowModel.KPIsMomentumDispersionDelta.WinLossRatio == 0) ? Color.White
									: (rowModel.KPIsMomentumDispersionDelta.WinLossRatio > 0) ? this.colorBackgroundGreen : this.colorBackgroundRed;
			} else if (e.Column == this.olvcMomentumsDispersionDeltaRecoveryFactor) {
				e.SubItem.BackColor = (rowModel.KPIsMomentumDispersionDelta.RecoveryFactor == 0) ? Color.White
									: (rowModel.KPIsMomentumDispersionDelta.RecoveryFactor > 0) ? this.colorBackgroundGreen : this.colorBackgroundRed;
			} else if (e.Column == this.olvcMomentumsDispersionDeltaMaxDrawdown) {
				e.SubItem.BackColor = (rowModel.KPIsMomentumDispersionDelta.MaxDrawDown == 0) ? Color.White
									: (rowModel.KPIsMomentumDispersionDelta.MaxDrawDown > 0) ? this.colorBackgroundRed : this.colorBackgroundGreen;
			} else if (e.Column == this.olvcMomentumsDispersionDeltaMaxConsecutiveWinners) {
				e.SubItem.BackColor = (rowModel.KPIsMomentumDispersionDelta.MaxConsecWinners == 0) ? Color.White
									: (rowModel.KPIsMomentumDispersionDelta.MaxConsecWinners > 0) ? this.colorBackgroundGreen : this.colorBackgroundRed;
			} else if (e.Column == this.olvcMomentumsDispersionDeltaMaxConsecutiveLosers) {
				e.SubItem.BackColor = (rowModel.KPIsMomentumDispersionDelta.MaxConsecLosers == 0) ? Color.White
									: (rowModel.KPIsMomentumDispersionDelta.MaxConsecLosers > 0) ? this.colorBackgroundGreen : this.colorBackgroundRed;
			}
			#endregion

			#region KPIsMomentumsVariance
			//if (e.Column == this.olvcMomentumsVarianceTotalPositions) {
			//	e.SubItem.BackColor = (rowModel.KPIsMomentumVariance.PositionsCount == 0) ? Color.White
			//						: (rowModel.KPIsMomentumVariance.PositionsCount > 0) ? this.colorBackgroundGreen : this.colorBackgroundRed;
			//} else if (e.Column == this.olvcMomentumsVarianceNetProfit) {
			//	e.SubItem.BackColor = (rowModel.KPIsMomentumVariance.NetProfit == 0) ? Color.White
			//						: (rowModel.KPIsMomentumVariance.NetProfit > 0) ? this.colorBackgroundGreen : this.colorBackgroundRed;
			//} else if (e.Column == this.olvcMomentumsVarianceProfitPerPosition) {
			//	e.SubItem.BackColor = (rowModel.KPIsMomentumVariance.PositionAvgProfit == 0) ? Color.White
			//						: (rowModel.KPIsMomentumVariance.PositionAvgProfit > 0) ? this.colorBackgroundGreen : this.colorBackgroundRed;
			//} else if (e.Column == this.olvcMomentumsVarianceProfitFactor) {
			//	e.SubItem.BackColor = (rowModel.KPIsMomentumVariance.ProfitFactor == 0) ? Color.White
			//						: (rowModel.KPIsMomentumVariance.ProfitFactor > 0) ? this.colorBackgroundGreen : this.colorBackgroundRed;
			//} else if (e.Column == this.olvcMomentumsVarianceWinLoss) {
			//	e.SubItem.BackColor = (rowModel.KPIsMomentumVariance.WinLossRatio == 0) ? Color.White
			//						: (rowModel.KPIsMomentumVariance.WinLossRatio > 0) ? this.colorBackgroundGreen : this.colorBackgroundRed;
			//} else if (e.Column == this.olvcMomentumsVarianceRecoveryFactor) {
			//	e.SubItem.BackColor = (rowModel.KPIsMomentumVariance.RecoveryFactor == 0) ? Color.White
			//						: (rowModel.KPIsMomentumVariance.RecoveryFactor > 0) ? this.colorBackgroundGreen : this.colorBackgroundRed;
			//} else if (e.Column == this.olvcMomentumsVarianceMaxDrawdown) {
			//	e.SubItem.BackColor = (rowModel.KPIsMomentumVariance.MaxDrawDown == 0) ? Color.White
			//						: (rowModel.KPIsMomentumVariance.MaxDrawDown > 0) ? this.colorBackgroundRed : this.colorBackgroundGreen;
			//} else if (e.Column == this.olvcMomentumsVarianceMaxConsecutiveWinners) {
			//	e.SubItem.BackColor = (rowModel.KPIsMomentumVariance.MaxConsecWinners == 0) ? Color.White
			//						: (rowModel.KPIsMomentumVariance.MaxConsecWinners > 0) ? this.colorBackgroundGreen : this.colorBackgroundRed;
			//} else if (e.Column == this.olvcMomentumsVarianceMaxConsecutiveLosers) {
			//	e.SubItem.BackColor = (rowModel.KPIsMomentumVariance.MaxConsecLosers == 0) ? Color.White
			//						: (rowModel.KPIsMomentumVariance.MaxConsecLosers > 0) ? this.colorBackgroundGreen : this.colorBackgroundRed;
			//}
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
			//if (this.olv.UseWaitCursor == true) {	// CANT_FIX_FIRST_CLICK_RESETTING_ALL_CHECKBOXES
			//	return;	// FIRST_CLICK_ON_CHECKBOX_DESELECTS_OTHERS__SYNCING_VIA_WAIT_CURSOR DO_NOTHING_SINCE_PUTTER_ARRIVES_FIRST
			//}

			//string msg = "olvAllValuesForOneParam_CellClick() WHEN_BOTH_PUTTER_AND_CELLCLICK_REGISTERED_BOTH_ARE_INVOKED_ON_PUTTER_FIRST_TIME_CLICKED_PUTTER_THEN_CLICK";
			//Assembler.PopupException(msg, null, false);
			if (this.ignoreCellClickDupe_HappensOnFirstCheckboxPutter == -1) {
				this.ignoreCellClickDupe_HappensOnFirstCheckboxPutter = 0;	// user clicked the label first (CellClick), not the checkbox (putter) => the bug will never arise again (observation)
			}
			if (this.ignoreCellClickDupe_HappensOnFirstCheckboxPutter == 1) {
				this.ignoreCellClickDupe_HappensOnFirstCheckboxPutter = 0;
				return;		// user clicked the checkbox first (putter invoked) => CellClick handler gets a buggy duplicated => drop this event, reset and forget forever koz happens once / lifetime
			}
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
			this.olvStateBinaryRestoreAllValuesForOneParam();

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
			this.olvcTotalPositionsGlobal.AspectToStringFormat = "{0:N1}";

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
			this.olvcTotalPositionsLocal.AspectToStringFormat = "{0:N1}";

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
				double positionsCount = oneParameterOneValue.KPIsMomentumAverage.PositionsCount;
				if (positionsCount == 0) {
					string msg = "TAKE_DELTA_FOR_CELL_WITH_AVG_OR_STDEV_OR_VAR";
					if (oneParameterOneValue.ArtificialName == OneParameterAllValuesAveraged.ARTIFICIAL_ROW_MEAN) {
						positionsCount = oneParameterOneValue.KPIsDelta.PositionsCount;
					} else if (oneParameterOneValue.ArtificialName == OneParameterAllValuesAveraged.ARTIFICIAL_ROW_DISPERSION) {
						positionsCount = oneParameterOneValue.KPIsDelta.PositionsCount;
					} else {
						positionsCount = -999;
					}
				}
				return positionsCount;
			};
			this.olvcMomentumsAverageTotalPositions.AspectToStringFormat = this.formatMomentums;

			this.olvcMomentumsAverageProfitPerPosition.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcMomentumsAverageProfitPerPosition.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsMomentumAverage.PositionAvgProfit;
			};
			this.olvcMomentumsAverageProfitPerPosition.AspectToStringFormat = this.formatMomentums;

			this.olvcMomentumsAverageNetProfit.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcMomentumsAverageNetProfit.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsMomentumAverage.NetProfit;
			};
			this.olvcMomentumsAverageNetProfit.AspectToStringFormat = this.formatMomentums;

			this.olvcMomentumsAverageWinLoss.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcMomentumsAverageWinLoss.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsMomentumAverage.WinLossRatio;
			};
			this.olvcMomentumsAverageWinLoss.AspectToStringFormat = this.formatMomentums;

			this.olvcMomentumsAverageProfitFactor.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcMomentumsAverageProfitFactor.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsMomentumAverage.ProfitFactor;
			};
			this.olvcMomentumsAverageProfitFactor.AspectToStringFormat = this.formatMomentums;

			this.olvcMomentumsAverageRecoveryFactor.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcMomentumsAverageRecoveryFactor.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsMomentumAverage.RecoveryFactor;
			};
			this.olvcMomentumsAverageRecoveryFactor.AspectToStringFormat = this.formatMomentums;

			this.olvcMomentumsAverageMaxDrawdown.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcMomentumsAverageMaxDrawdown.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsMomentumAverage.MaxDrawDown;
			};
			this.olvcMomentumsAverageMaxDrawdown.AspectToStringFormat = this.formatMomentums;

			this.olvcMomentumsAverageMaxConsecutiveWinners.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcMomentumsAverageMaxConsecutiveWinners.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsMomentumAverage.MaxConsecWinners;
			};
			this.olvcMomentumsAverageMaxConsecutiveWinners.AspectToStringFormat = this.formatMomentums;

			this.olvcMomentumsAverageMaxConsecutiveLosers.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcMomentumsAverageMaxConsecutiveLosers.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsMomentumAverage.MaxConsecLosers;
			};
			this.olvcMomentumsAverageMaxConsecutiveLosers.AspectToStringFormat = this.formatMomentums;
			#endregion

			#region Momentums: Dispersion Global
			this.olvcMomentumsDispersionGlobalTotalPositions.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcMomentumsDispersionGlobalTotalPositions.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsMomentumDispersionGlobal.PositionsCount;
			};
			this.olvcMomentumsDispersionGlobalTotalPositions.AspectToStringFormat = this.formatMomentums;

			this.olvcMomentumsDispersionGlobalProfitPerPosition.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcMomentumsDispersionGlobalProfitPerPosition.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsMomentumDispersionGlobal.PositionAvgProfit;
			};
			this.olvcMomentumsDispersionGlobalProfitPerPosition.AspectToStringFormat = this.formatMomentums;

			this.olvcMomentumsDispersionGlobalNetProfit.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcMomentumsDispersionGlobalNetProfit.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsMomentumDispersionGlobal.NetProfit;
			};
			this.olvcMomentumsDispersionGlobalNetProfit.AspectToStringFormat = this.formatMomentums;

			this.olvcMomentumsDispersionGlobalWinLoss.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcMomentumsDispersionGlobalWinLoss.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsMomentumDispersionGlobal.WinLossRatio;
			};
			this.olvcMomentumsDispersionGlobalWinLoss.AspectToStringFormat = this.formatMomentums;

			this.olvcMomentumsDispersionGlobalProfitFactor.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcMomentumsDispersionGlobalProfitFactor.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsMomentumDispersionGlobal.ProfitFactor;
			};
			this.olvcMomentumsDispersionGlobalProfitFactor.AspectToStringFormat = this.formatMomentums;

			this.olvcMomentumsDispersionGlobalRecoveryFactor.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcMomentumsDispersionGlobalRecoveryFactor.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsMomentumDispersionGlobal.RecoveryFactor;
			};
			this.olvcMomentumsDispersionGlobalRecoveryFactor.AspectToStringFormat = this.formatMomentums;

			this.olvcMomentumsDispersionGlobalMaxDrawdown.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcMomentumsDispersionGlobalMaxDrawdown.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsMomentumDispersionGlobal.MaxDrawDown;
			};
			this.olvcMomentumsDispersionGlobalMaxDrawdown.AspectToStringFormat = this.formatMomentums;

			this.olvcMomentumsDispersionGlobalMaxConsecutiveWinners.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcMomentumsDispersionGlobalMaxConsecutiveWinners.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsMomentumDispersionGlobal.MaxConsecWinners;
			};
			this.olvcMomentumsDispersionGlobalMaxConsecutiveWinners.AspectToStringFormat = this.formatMomentums;

			this.olvcMomentumsDispersionGlobalMaxConsecutiveLosers.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcMomentumsDispersionGlobalMaxConsecutiveLosers.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsMomentumDispersionGlobal.MaxConsecLosers;
			};
			this.olvcMomentumsDispersionGlobalMaxConsecutiveLosers.AspectToStringFormat = this.formatMomentums;
			#endregion

			#region Momentums: Dispersion Local
			this.olvcMomentumsDispersionLocalTotalPositions.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcMomentumsDispersionLocalTotalPositions.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsMomentumDispersionLocal.PositionsCount;
			};
			this.olvcMomentumsDispersionLocalTotalPositions.AspectToStringFormat = this.formatMomentums;

			this.olvcMomentumsDispersionLocalProfitPerPosition.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcMomentumsDispersionLocalProfitPerPosition.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsMomentumDispersionLocal.PositionAvgProfit;
			};
			this.olvcMomentumsDispersionLocalProfitPerPosition.AspectToStringFormat = this.formatMomentums;

			this.olvcMomentumsDispersionLocalNetProfit.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcMomentumsDispersionLocalNetProfit.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsMomentumDispersionLocal.NetProfit;
			};
			this.olvcMomentumsDispersionLocalNetProfit.AspectToStringFormat = this.formatMomentums;

			this.olvcMomentumsDispersionLocalWinLoss.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcMomentumsDispersionLocalWinLoss.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsMomentumDispersionLocal.WinLossRatio;
			};
			this.olvcMomentumsDispersionLocalWinLoss.AspectToStringFormat = this.formatMomentums;

			this.olvcMomentumsDispersionLocalProfitFactor.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcMomentumsDispersionLocalProfitFactor.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsMomentumDispersionLocal.ProfitFactor;
			};
			this.olvcMomentumsDispersionLocalProfitFactor.AspectToStringFormat = this.formatMomentums;

			this.olvcMomentumsDispersionLocalRecoveryFactor.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcMomentumsDispersionLocalRecoveryFactor.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsMomentumDispersionLocal.RecoveryFactor;
			};
			this.olvcMomentumsDispersionLocalRecoveryFactor.AspectToStringFormat = this.formatMomentums;

			this.olvcMomentumsDispersionLocalMaxDrawdown.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcMomentumsDispersionLocalMaxDrawdown.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsMomentumDispersionLocal.MaxDrawDown;
			};
			this.olvcMomentumsDispersionLocalMaxDrawdown.AspectToStringFormat = this.formatMomentums;

			this.olvcMomentumsDispersionLocalMaxConsecutiveWinners.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcMomentumsDispersionLocalMaxConsecutiveWinners.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsMomentumDispersionLocal.MaxConsecWinners;
			};
			this.olvcMomentumsDispersionLocalMaxConsecutiveWinners.AspectToStringFormat = this.formatMomentums;

			this.olvcMomentumsDispersionLocalMaxConsecutiveLosers.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcMomentumsDispersionLocalMaxConsecutiveLosers.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsMomentumDispersionLocal.MaxConsecLosers;
			};
			this.olvcMomentumsDispersionLocalMaxConsecutiveLosers.AspectToStringFormat = this.formatMomentums;
			#endregion

			#region Momentums: Dispersion Delta
			this.olvcMomentumsDispersionDeltaTotalPositions.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcMomentumsDispersionDeltaTotalPositions.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsMomentumDispersionDelta.PositionsCount;
			};
			this.olvcMomentumsDispersionDeltaTotalPositions.AspectToStringFormat = this.formatMomentums;

			this.olvcMomentumsDispersionDeltaProfitPerPosition.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcMomentumsDispersionDeltaProfitPerPosition.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsMomentumDispersionDelta.PositionAvgProfit;
			};
			this.olvcMomentumsDispersionDeltaProfitPerPosition.AspectToStringFormat = this.formatMomentums;

			this.olvcMomentumsDispersionDeltaNetProfit.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcMomentumsDispersionDeltaNetProfit.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsMomentumDispersionDelta.NetProfit;
			};
			this.olvcMomentumsDispersionDeltaNetProfit.AspectToStringFormat = this.formatMomentums;

			this.olvcMomentumsDispersionDeltaWinLoss.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcMomentumsDispersionDeltaWinLoss.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsMomentumDispersionDelta.WinLossRatio;
			};
			this.olvcMomentumsDispersionDeltaWinLoss.AspectToStringFormat = this.formatMomentums;

			this.olvcMomentumsDispersionDeltaProfitFactor.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcMomentumsDispersionDeltaProfitFactor.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsMomentumDispersionDelta.ProfitFactor;
			};
			this.olvcMomentumsDispersionDeltaProfitFactor.AspectToStringFormat = this.formatMomentums;

			this.olvcMomentumsDispersionDeltaRecoveryFactor.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcMomentumsDispersionDeltaRecoveryFactor.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsMomentumDispersionDelta.RecoveryFactor;
			};
			this.olvcMomentumsDispersionDeltaRecoveryFactor.AspectToStringFormat = this.formatMomentums;

			this.olvcMomentumsDispersionDeltaMaxDrawdown.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcMomentumsDispersionDeltaMaxDrawdown.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsMomentumDispersionDelta.MaxDrawDown;
			};
			this.olvcMomentumsDispersionDeltaMaxDrawdown.AspectToStringFormat = this.formatMomentums;

			this.olvcMomentumsDispersionDeltaMaxConsecutiveWinners.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcMomentumsDispersionDeltaMaxConsecutiveWinners.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsMomentumDispersionDelta.MaxConsecWinners;
			};
			this.olvcMomentumsDispersionDeltaMaxConsecutiveWinners.AspectToStringFormat = this.formatMomentums;

			this.olvcMomentumsDispersionDeltaMaxConsecutiveLosers.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcMomentumsDispersionDeltaMaxConsecutiveLosers.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsMomentumDispersionDelta.MaxConsecLosers;
			};
			this.olvcMomentumsDispersionDeltaMaxConsecutiveLosers.AspectToStringFormat = this.formatMomentums;
			#endregion


			#region Momentums: Variance
			//this.olvcMomentumsVarianceTotalPositions.AspectGetter = delegate(object o) {
			//	OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
			//	if (oneParameterOneValue == null) return "olvcMomentumsVarianceMomentumsVarianceTotalPositionsDelta.AspectGetter: OneParameterOneValue=null";
			//	//return oneParameterOneValue.KPIsMomentumsVariance.PositionsCountFormatted;
			//	return oneParameterOneValue.KPIsMomentumVariance.PositionsCount;
			//};
			//this.olvcMomentumsVarianceTotalPositions.AspectToStringFormat = this.formatMomentums;

			//this.olvcMomentumsVarianceProfitPerPosition.AspectGetter = delegate(object o) {
			//	OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
			//	if (oneParameterOneValue == null) return "olvcMomentumsVarianceMomentumsVarianceProfitPerPositionDelta.AspectGetter: OneParameterOneValue=null";
			//	//return oneParameterOneValue.KPIsMomentumsVariance.PositionAvgProfitFormatted;
			//	return oneParameterOneValue.KPIsMomentumVariance.PositionAvgProfit;
			//};
			//// PRINTED_N1 this.olvcMomentumsVarianceProfitPerPosition.AspectToStringFormat = "N1";
			//this.olvcMomentumsVarianceProfitPerPosition.AspectToStringFormat = this.formatMomentums;

			//this.olvcMomentumsVarianceNetProfit.AspectGetter = delegate(object o) {
			//	OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
			//	if (oneParameterOneValue == null) return "olvcMomentumsVarianceMomentumsVarianceNetProfitDelta.AspectGetter: OneParameterOneValue=null";
			//	return oneParameterOneValue.KPIsMomentumVariance.NetProfit;
			//};
			//this.olvcMomentumsVarianceNetProfit.AspectToStringFormat = this.formatMomentums;
			//// DIDNT_MAKE_ANY_CHANGE this.olvcMomentumsVarianceNetProfit.DataType = typeof(int);

			//this.olvcMomentumsVarianceWinLoss.AspectGetter = delegate(object o) {
			//	OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
			//	if (oneParameterOneValue == null) return "olvcMomentumsVarianceMomentumsVarianceWinLossDelta.AspectGetter: OneParameterOneValue=null";
			//	return oneParameterOneValue.KPIsMomentumVariance.WinLossRatio;
			//};
			//this.olvcMomentumsVarianceWinLoss.AspectToStringFormat = this.formatMomentums;

			//this.olvcMomentumsVarianceProfitFactor.AspectGetter = delegate(object o) {
			//	OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
			//	if (oneParameterOneValue == null) return "olvcMomentumsVarianceMomentumsVarianceProfitFactorDelta.AspectGetter: OneParameterOneValue=null";
			//	return oneParameterOneValue.KPIsMomentumVariance.ProfitFactor;
			//};
			//this.olvcMomentumsVarianceProfitFactor.AspectToStringFormat = this.formatMomentums;

			//this.olvcMomentumsVarianceRecoveryFactor.AspectGetter = delegate(object o) {
			//	OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
			//	if (oneParameterOneValue == null) return "olvcMomentumsVarianceMomentumsVarianceRecoveryFactorDelta.AspectGetter: OneParameterOneValue=null";
			//	return oneParameterOneValue.KPIsMomentumVariance.RecoveryFactor;
			//};
			//this.olvcMomentumsVarianceRecoveryFactor.AspectToStringFormat = this.formatMomentums;

			//this.olvcMomentumsVarianceMaxDrawdown.AspectGetter = delegate(object o) {
			//	OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
			//	if (oneParameterOneValue == null) return "olvcMomentumsVarianceMomentumsVarianceMaxDrawdownDelta.AspectGetter: OneParameterOneValue=null";
			//	return oneParameterOneValue.KPIsMomentumVariance.MaxDrawDown;
			//};
			//this.olvcMomentumsVarianceMaxDrawdown.AspectToStringFormat = this.formatMomentums;

			//this.olvcMomentumsVarianceMaxConsecutiveWinners.AspectGetter = delegate(object o) {
			//	OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
			//	if (oneParameterOneValue == null) return "olvcMomentumsVarianceMomentumsVarianceMaxConsecutiveWinnersDelta.AspectGetter: OneParameterOneValue=null";
			//	return oneParameterOneValue.KPIsMomentumVariance.MaxConsecWinners;
			//};
			//this.olvcMomentumsVarianceMaxConsecutiveWinners.AspectToStringFormat = this.formatMomentums;

			//this.olvcMomentumsVarianceMaxConsecutiveLosers.AspectGetter = delegate(object o) {
			//	OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
			//	if (oneParameterOneValue == null) return "olvcMomentumsVarianceMomentumsVarianceMaxConsecutiveLosersDelta.AspectGetter: OneParameterOneValue=null";
			//	return oneParameterOneValue.KPIsMomentumVariance.MaxConsecLosers;
			//};
			//this.olvcMomentumsVarianceMaxConsecutiveLosers.AspectToStringFormat = this.formatMomentums;
			#endregion


			
			// USED_AspectToStringConverter_INSTEAD_OF_CUSTOM_SORTING
			//this.olvAllValuesForOneParam.CustomSorter = delegate(OLVColumn column, SortOrder order) {
			//	if (column == this.olvcMaxDrawdownDelta) {
			//		this.olvAllValuesForOneParam.ListViewItemSorter = new NumberComparer(column, order);
			//	}
			//};

			this.olv.CheckStateGetter = delegate(object o) {
				//if (this.ignoreCheckStateWillRefreshLater_fixFirstClickResettingAllCheckboxes) return CheckState.Indeterminate;
				//if (this.olv.UseWaitCursor == true) return CheckState.Indeterminate;
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return CheckState.Indeterminate;
				return oneParameterOneValue.Chosen ? CheckState.Checked : CheckState.Unchecked;
			};
			this.olv.CheckStatePutter = delegate(object o, CheckState newState) {
				//if (this.IgnoreCheboxStatePutterImanuallyResetThem) return;
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return CheckState.Indeterminate;

				oneParameterOneValue.Chosen = newState.CompareTo(CheckState.Checked) == 0;

				//string msg = "oneParameterOneValue[" + oneParameterOneValue + "].Chosen[" + oneParameterOneValue.Chosen + "] WHEN_BOTH_PUTTER_AND_CELLCLICK_REGISTERED_BOTH_ARE_INVOKED_ON_PUTTER_FIRST_TIME_CLICKED_PUTTER_THEN_CLICK";
				//Assembler.PopupException(msg, null, false);
				if (this.ignoreCellClickDupe_HappensOnFirstCheckboxPutter == -1) {
					this.ignoreCellClickDupe_HappensOnFirstCheckboxPutter = 1;	// user clicked checkbox first (putter) => inform CellClick handler to drop its buggy event
				}

				this.olv.UseWaitCursor = true;	// CANT_FIX_FIRST_CLICK_RESETTING_ALL_CHECKBOXES FIRST_CLICK_ON_CHECKBOX_DESELECTS_OTHERS__SYNCING_VIA_WAIT_CURSOR
				this.correlator.OneParameterOneValueUserSelectedChanged_recalculateLocalKPIsMomentums(oneParameterOneValue);
				// NO_NEED__ALL_THREE
				//this.olv.RefreshObject(oneParameterOneValue);
				//this.olv.RebuildColumns();
				//this.olv.Refresh();
				return newState;
			};

			// TOO_MANY_STRATEGY_SERIALIZATIONS__INTRODUCED_dontSerializeStrategy_ImAligingInCtor
			this.olv.ColumnWidthChanged += new ColumnWidthChangedEventHandler(olv_ColumnWidthChanged);
			this.olv.ColumnWidthChanging += new ColumnWidthChangingEventHandler(olv_ColumnWidthChanging);
		}

		int ignoreCellClickDupe_HappensOnFirstCheckboxPutter = -1;

		void olv_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e) {
			if (this.dontSerializeStrategy_ImAligingInCtor) return;
			if (this.dontRaiseContainerShouldSerializedForEachColumnVisibilityChanged_alreadyRaised) return;
			if (this.dontSerializeStrategy_ImAligingInCtor) return;
			try {
				this.AlignBaseSizeToDisplayedCells();
			} catch (Exception ex) {
				string msg = "olv_ColumnWidthChanging()";
				Assembler.PopupException(msg, ex, false);
			}
		}
		void olv_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e) {
			if (this.dontSerializeStrategy_ImAligingInCtor) return;
			if (this.dontRaiseContainerShouldSerializedForEachColumnVisibilityChanged_alreadyRaised) return;
			if (this.dontSerializeStrategy_ImAligingInCtor) return;
			try {
				//MOVED_TO_olv_ColumnWidthChanging() this.AlignBaseSizeToDisplayedCells();
				this.olvSaveBinaryState_SerializeSnapshot();
			} catch (Exception ex) {
				string msg = "NYI: NEED_TO_IGNORE MULTIPLE_COLUMNS_WIDTHS_CHANGED INDUCED_BY_REBUILD_COLUMNS after RIGHT_CLICK_INDIVIDUAL_COLUMN_CHECKED";
				Assembler.PopupException(msg, ex, false);
			}
		}
		void oLVColumn_VisibilityChanged(object sender, EventArgs e) {
			if (this.indicatorParameter_nullUnsafe == null) return;
			OLVColumn oLVColumn = sender as OLVColumn;
			if (oLVColumn == null) return;
			this.olvSaveBinaryState_SerializeSnapshot();
		}
	}
	// USED_AspectToStringConverter_INSTEAD_OF_CUSTOM_SORTING
	//class NumberComparer : IComparer {
	//	SortOrder order;
	//	OLVColumn column;
	//	public NumberComparer(OLVColumn column, SortOrder order) {
	//		this.column = column;
	//		this.order = order;
	//	}
	//	public int Compare(object x, object y) {
	//		OLVListItem row1 = x as OLVListItem;
	//		OLVListItem row2 = y as OLVListItem;

	//		OneParameterOneValue cell1 = row1.RowObject as OneParameterOneValue;
	//		OneParameterOneValue cell2 = row2.RowObject as OneParameterOneValue;

	//		int forAscending = cell1.KPIsDelta.MaxDrawDown.CompareTo(cell2.KPIsDelta.MaxDrawDown);
	//		if (this.order == SortOrder.Descending) return forAscending *= -1;
	//		return forAscending;
	//	}
	//}
}
