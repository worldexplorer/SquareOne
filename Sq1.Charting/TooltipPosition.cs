using System.Drawing;
using System.Windows.Forms;

using Sq1.Core.DataTypes;
using Sq1.Core.Execution;

namespace Sq1.Charting {
	public partial class TooltipPosition {
		public AlertArrow AlertArrow;

		public TooltipPosition() {
			this.InitializeComponent();
			//base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			//base.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			//this.SetStyle(ControlStyles.Selectable, false);
		}
		public void PopulateTooltip(AlertArrow arrow) {
			this.AlertArrow = arrow;
			Position position = arrow.Position;
			Bars bars = position.Bars;
			string priceFormat = bars.SymbolInfo.PriceFormat;

			if (position.PositionLongShort == PositionLongShort.Long) {
				lblEntry.Text = "Long" + position.EntryMarketLimitStop.ToString().Substring(0, 1);
				lblExit.Text = "Sold" + position.EntryMarketLimitStop.ToString().Substring(0, 1);
			} else {
				lblEntry.Text = "Short" + position.ExitMarketLimitStop.ToString().Substring(0, 1);
				lblExit.Text = "Covered" + position.ExitMarketLimitStop.ToString().Substring(0, 1);
			}
			lblEntry.Text += " (" + position.EntryPriceScript.ToString(priceFormat) + ")";
			lblExit.Text += " (" + position.ExitPriceScript.ToString(priceFormat) + ")";

			this.lblEntrySignalVal.Text = position.EntrySignal;
			this.lblExitSignalVal.Text = position.ExitSignal;

			string text = "";
			if (bars.IsIntraday) {
				//text = text + " " + bars.Date[barNumber].ToShortTimeString();
				//text = text + bars[bar].DateTimeOpen.ToString("HH:mm ");
				text = text + position.EntryDate.ToString("HH:mm ");
			}
			//text = text + bars[bar].DateTimeOpen.ToString("ddd dd-MMM-yyyy");
			text = text + position.EntryDate.ToString("ddd dd-MMM-yyyy");
			this.lblDateVal.Text = text;

			this.lblEntryVal.Text = position.EntryFilledPrice.ToString(priceFormat);
			this.lblExitVal.Text = position.ExitFilledPrice.ToString(priceFormat);

			double distancePoints = position.DistanceInPoints;
			this.lblDistancePointsVal.Text = distancePoints.ToString(priceFormat);

			this.lblSharesVal.Text = position.Shares.ToString();

			double grossProfit = distancePoints * position.Shares;
			this.lblGrossProfitLossVal.Text = grossProfit.ToString(priceFormat);
			this.lblPoint2DollarVal.Text = position.Bars.SymbolInfo.Point2Dollar.ToString();
			this.lblNetProfitLossValue.Text = position.NetProfit.ToString(priceFormat);
			
			double commissions = position.EntryFilledCommission + position.ExitFilledCommission;
			this.lblCommissionVal.Text = commissions.ToString();
			this.lblPriceLevelSizeVal.Text = position.Bars.SymbolInfo.PriceStep.ToString(priceFormat);
			this.lblBasisPriceVal.Text = position.EntryFilledPrice.ToString(priceFormat);
	
			Color color = (position.NetProfit > 0.0) ? Color.Green : Color.Red;
			this.lblDistancePoints.ForeColor = color;
			this.lblDistancePointsVal.ForeColor = color;
			this.lblNetProfitLoss.ForeColor = color;
			this.lblNetProfitLossValue.ForeColor = color;
			this.lblGrossProfitLoss.ForeColor = color;
			this.lblGrossProfitLossVal.ForeColor = color;

			if (position.NetProfit > 0.0) {
				this.lblNetProfitLoss.Text = "Net Profit";
				this.lblGrossProfitLoss.Text = "Grs Profit";
			} else {
				this.lblNetProfitLoss.Text = "Net Loss";
				this.lblGrossProfitLoss.Text = "Grs Loss";
			}

			this.lblSlippagesVal.Text = position.EntryFilledSlippage.ToString() + " / " + position.ExitFilledSlippage.ToString();
		}
		void TooltipPosition_MouseMove(object sender, MouseEventArgs e) {
			this.Visible = false;
		}
	}
}