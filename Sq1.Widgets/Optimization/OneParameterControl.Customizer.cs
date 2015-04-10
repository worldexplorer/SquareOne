using System;
using System.Drawing;
using System.Windows.Forms;

using BrightIdeasSoftware;
using Sq1.Core;
using Sq1.Core.Optimization;

namespace Sq1.Widgets.Optimization {
	public partial class OneParameterControl {
		Color colorBackgroundRed;
		Color colorBackgroundGreen;
		
		void olv_FormatRow(object sender, FormatRowEventArgs e) {
			OneParameterOneValue oneParameterOneValue = e.Model as OneParameterOneValue;
			if (oneParameterOneValue == null) return;
			if (oneParameterOneValue.IsArtificialRow) {
				e.Item.BackColor = Color.Gainsboro;
				return;
			}
			return;
			//e.Item.BackColor = (oneParameterOneValue.KPIsGlobal.NetProfit > 0.0) ? this.colorBackgroundGreen : this.colorBackgroundRed;
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
		}
		void olvCustomizeColors() {
			//if (this.snap.Colorify) {
				this.colorBackgroundRed = Color.FromArgb(255, 230, 230);
				this.colorBackgroundGreen = Color.FromArgb(230, 255, 230);
				this.olv.UseCellFormatEvents = true;
				this.olv.FormatRow += new EventHandler<FormatRowEventArgs>(olv_FormatRow);
				this.olv.FormatCell += new EventHandler<FormatCellEventArgs>(olv_FormatCell);
			//} else {
			//	this.olvPositions.UseCellFormatEvents = false;
			//	this.olvPositions.FormatRow -= new EventHandler<FormatRowEventArgs>(olvPositions_FormatRow);
			//}
		}
		void olvCustomize() {
			this.olvCustomizeColors();
			this.olvcParamValues.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcParamValues.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.ParameterNameValue;
			};

			#region Global
			this.olvcNetProfitGlobal.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcNetProfitGlobal.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsGlobal.NetProfitFormatted;
			};
			this.olvcTotalPositionsGlobal.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcTotalPositionsGlobal.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsGlobal.PositionsCountFormatted;
			};
			this.olvcProfitPerPositionGlobal.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcProfitPerPositionGlobal.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsGlobal.PositionAvgProfitFormatted;
			};
			this.olvcWinLossGlobal.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcWinLossGlobal.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsGlobal.WinLossRatioFormatted;
			};
			this.olvcProfitFactorGlobal.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcProfitFactorGlobal.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsGlobal.ProfitFactorFormatted;
			};
			this.olvcRecoveryFactorGlobal.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcRecoveryFactorGlobal.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsGlobal.RecoveryFactorFormatted;
			};
			this.olvcMaxDrawdownGlobal.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcMaxDrawdownGlobal.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsGlobal.MaxDrawDownFormatted;
			};
			this.olvcMaxConsecutiveWinnersGlobal.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcMaxConsecutiveWinnersGlobal.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsGlobal.MaxConsecWinnersFormatted;
			};
			this.olvcMaxConsecutiveLosersGlobal.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcMaxConsecutiveLosersGlobal.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsGlobal.MaxConsecLosersFormatted;
			};
			#endregion

			#region Local
			this.olvcNetProfitLocal.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcNetProfitLocal.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsLocal.NetProfitFormatted;
			};
			this.olvcTotalPositionsLocal.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcTotalPositionsLocal.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsLocal.PositionsCountFormatted;
			};
			this.olvcProfitPerPositionLocal.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcProfitPerPositionLocal.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsLocal.PositionAvgProfitFormatted;
			};
			this.olvcWinLossLocal.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcWinLossLocal.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsLocal.WinLossRatioFormatted;
			};
			this.olvcProfitFactorLocal.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcProfitFactorLocal.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsLocal.ProfitFactorFormatted;
			};
			this.olvcRecoveryFactorLocal.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcRecoveryFactorLocal.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsLocal.RecoveryFactorFormatted;
			};
			this.olvcMaxDrawdownLocal.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcMaxDrawdownLocal.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsLocal.MaxDrawDownFormatted;
			};
			this.olvcMaxConsecutiveWinnersLocal.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcMaxConsecutiveWinnersLocal.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsLocal.MaxConsecWinnersFormatted;
			};
			this.olvcMaxConsecutiveLosersLocal.AspectGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return "olvcMaxConsecutiveLosersLocal.AspectGetter: OneParameterOneValue=null";
				return oneParameterOneValue.KPIsLocal.MaxConsecLosersFormatted;
			};
			#endregion



			this.olv.CheckStateGetter = delegate(object o) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return CheckState.Indeterminate;
				return oneParameterOneValue.UserSelected ? CheckState.Checked : CheckState.Unchecked;
			};
			this.olv.CheckStatePutter = delegate(object o, CheckState newState) {
				OneParameterOneValue oneParameterOneValue = o as OneParameterOneValue;
				if (oneParameterOneValue == null) return CheckState.Indeterminate;
				oneParameterOneValue.UserSelected = newState.CompareTo(CheckState.Checked) == 0;
				this.olv.RefreshObject(oneParameterOneValue);
				this.RaiseOnKPIsLocalRecalculate();
				return newState;
			};
			
		}
	}
}
