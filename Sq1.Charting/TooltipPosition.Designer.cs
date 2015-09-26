using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;

namespace Sq1.Charting {
	[Designer("System.Windows.Forms.Design.ParentControlDesigner, System.Design", typeof(IDesigner))]
	public partial class TooltipPosition : UserControl {
		private IContainer components;
		private Label lblEntry;
		private Label lblExit;
		private Label lblDistancePoints;
		private Label lblShares;
		private Label lblNetProfitLoss;
		private Label lblCommission;
		private Label lblDateVal;
		private Label lblEntryVal;
		private Label lblExitVal;
		private Label lblDistancePointsVal;
		private Label lblSharesVal;
		private Label lblNetProfitLossValue;
		private Label lblCommissionVal;
		private Label lblGrossProfitLossVal;
		private Label lblGrossProfitLoss;
		private Label lblPoint2DollarVal;
		private Label lblPoint2Dollar;
		private Label lblPriceLevelSizeVal;
		private Label lblPriceLevelSize;
		private Label lblBasisPriceVal;
		private Label lblBasisPrice;
		private Label lblSlippagesVal;
		private Label lblSlippages;
		private Label lblEntrySignalVal;
		private Label lblExitSignalVal;
		protected override void Dispose(bool disposing) {
			if (disposing && this.components != null) {
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.lblEntry = new System.Windows.Forms.Label();
			this.lblExit = new System.Windows.Forms.Label();
			this.lblDistancePoints = new System.Windows.Forms.Label();
			this.lblShares = new System.Windows.Forms.Label();
			this.lblNetProfitLoss = new System.Windows.Forms.Label();
			this.lblCommission = new System.Windows.Forms.Label();
			this.lblDateVal = new System.Windows.Forms.Label();
			this.lblEntryVal = new System.Windows.Forms.Label();
			this.lblExitVal = new System.Windows.Forms.Label();
			this.lblDistancePointsVal = new System.Windows.Forms.Label();
			this.lblSharesVal = new System.Windows.Forms.Label();
			this.lblNetProfitLossValue = new System.Windows.Forms.Label();
			this.lblCommissionVal = new System.Windows.Forms.Label();
			this.lblGrossProfitLossVal = new System.Windows.Forms.Label();
			this.lblGrossProfitLoss = new System.Windows.Forms.Label();
			this.lblPoint2DollarVal = new System.Windows.Forms.Label();
			this.lblPoint2Dollar = new System.Windows.Forms.Label();
			this.lblPriceLevelSizeVal = new System.Windows.Forms.Label();
			this.lblPriceLevelSize = new System.Windows.Forms.Label();
			this.lblBasisPriceVal = new System.Windows.Forms.Label();
			this.lblBasisPrice = new System.Windows.Forms.Label();
			this.lblSlippagesVal = new System.Windows.Forms.Label();
			this.lblSlippages = new System.Windows.Forms.Label();
			this.lblEntrySignalVal = new System.Windows.Forms.Label();
			this.lblExitSignalVal = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// lblEntry
			// 
			this.lblEntry.AutoSize = true;
			this.lblEntry.Location = new System.Drawing.Point(2, 17);
			this.lblEntry.Name = "lblEntry";
			this.lblEntry.Size = new System.Drawing.Size(61, 13);
			this.lblEntry.TabIndex = 1;
			this.lblEntry.Text = "Long/Short";
			// 
			// lblExit
			// 
			this.lblExit.AutoSize = true;
			this.lblExit.Location = new System.Drawing.Point(2, 46);
			this.lblExit.Name = "lblExit";
			this.lblExit.Size = new System.Drawing.Size(73, 13);
			this.lblExit.TabIndex = 2;
			this.lblExit.Text = "Sold/Covered";
			// 
			// lblDistancePoints
			// 
			this.lblDistancePoints.AutoSize = true;
			this.lblDistancePoints.Location = new System.Drawing.Point(2, 78);
			this.lblDistancePoints.Name = "lblDistancePoints";
			this.lblDistancePoints.Size = new System.Drawing.Size(49, 13);
			this.lblDistancePoints.TabIndex = 3;
			this.lblDistancePoints.Text = "Distance";
			// 
			// lblShares
			// 
			this.lblShares.AutoSize = true;
			this.lblShares.Location = new System.Drawing.Point(2, 91);
			this.lblShares.Name = "lblShares";
			this.lblShares.Size = new System.Drawing.Size(40, 13);
			this.lblShares.TabIndex = 4;
			this.lblShares.Text = "Shares";
			// 
			// lblNetProfitLoss
			// 
			this.lblNetProfitLoss.AutoSize = true;
			this.lblNetProfitLoss.Location = new System.Drawing.Point(2, 131);
			this.lblNetProfitLoss.Name = "lblNetProfitLoss";
			this.lblNetProfitLoss.Size = new System.Drawing.Size(78, 13);
			this.lblNetProfitLoss.TabIndex = 5;
			this.lblNetProfitLoss.Text = "Net Profit/Loss";
			// 
			// lblCommission
			// 
			this.lblCommission.AutoSize = true;
			this.lblCommission.Location = new System.Drawing.Point(2, 148);
			this.lblCommission.Name = "lblCommission";
			this.lblCommission.Size = new System.Drawing.Size(66, 13);
			this.lblCommission.TabIndex = 6;
			this.lblCommission.Text = "Comisn incld";
			// 
			// lblDateVal
			// 
			this.lblDateVal.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblDateVal.Location = new System.Drawing.Point(1, 1);
			this.lblDateVal.Name = "lblDateVal";
			this.lblDateVal.Size = new System.Drawing.Size(162, 15);
			this.lblDateVal.TabIndex = 7;
			this.lblDateVal.Text = "1/1/2001";
			this.lblDateVal.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblEntryVal
			// 
			this.lblEntryVal.Location = new System.Drawing.Point(85, 16);
			this.lblEntryVal.Name = "lblEntryVal";
			this.lblEntryVal.Size = new System.Drawing.Size(50, 14);
			this.lblEntryVal.TabIndex = 8;
			this.lblEntryVal.Text = "190,660";
			this.lblEntryVal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lblExitVal
			// 
			this.lblExitVal.Location = new System.Drawing.Point(85, 46);
			this.lblExitVal.Name = "lblExitVal";
			this.lblExitVal.Size = new System.Drawing.Size(50, 14);
			this.lblExitVal.TabIndex = 9;
			this.lblExitVal.Text = "190,745";
			this.lblExitVal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lblDistancePointsVal
			// 
			this.lblDistancePointsVal.Location = new System.Drawing.Point(85, 77);
			this.lblDistancePointsVal.Name = "lblDistancePointsVal";
			this.lblDistancePointsVal.Size = new System.Drawing.Size(50, 14);
			this.lblDistancePointsVal.TabIndex = 10;
			this.lblDistancePointsVal.Text = "125";
			this.lblDistancePointsVal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lblSharesVal
			// 
			this.lblSharesVal.Location = new System.Drawing.Point(85, 90);
			this.lblSharesVal.Name = "lblSharesVal";
			this.lblSharesVal.Size = new System.Drawing.Size(50, 14);
			this.lblSharesVal.TabIndex = 11;
			this.lblSharesVal.Text = "1";
			this.lblSharesVal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lblNetProfitLossValue
			// 
			this.lblNetProfitLossValue.ForeColor = System.Drawing.Color.Black;
			this.lblNetProfitLossValue.Location = new System.Drawing.Point(85, 130);
			this.lblNetProfitLossValue.Name = "lblNetProfitLossValue";
			this.lblNetProfitLossValue.Size = new System.Drawing.Size(50, 14);
			this.lblNetProfitLossValue.TabIndex = 12;
			this.lblNetProfitLossValue.Text = "125";
			this.lblNetProfitLossValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lblCommissionVal
			// 
			this.lblCommissionVal.ForeColor = System.Drawing.Color.Black;
			this.lblCommissionVal.Location = new System.Drawing.Point(88, 144);
			this.lblCommissionVal.Name = "lblCommissionVal";
			this.lblCommissionVal.Size = new System.Drawing.Size(47, 14);
			this.lblCommissionVal.TabIndex = 13;
			this.lblCommissionVal.Text = "2.40";
			this.lblCommissionVal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lblGrossProfitLossVal
			// 
			this.lblGrossProfitLossVal.Location = new System.Drawing.Point(85, 103);
			this.lblGrossProfitLossVal.Name = "lblGrossProfitLossVal";
			this.lblGrossProfitLossVal.Size = new System.Drawing.Size(50, 14);
			this.lblGrossProfitLossVal.TabIndex = 17;
			this.lblGrossProfitLossVal.Text = "120";
			this.lblGrossProfitLossVal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lblGrossProfitLoss
			// 
			this.lblGrossProfitLoss.AutoSize = true;
			this.lblGrossProfitLoss.Location = new System.Drawing.Point(2, 104);
			this.lblGrossProfitLoss.Name = "lblGrossProfitLoss";
			this.lblGrossProfitLoss.Size = new System.Drawing.Size(77, 13);
			this.lblGrossProfitLoss.TabIndex = 16;
			this.lblGrossProfitLoss.Text = "Grs Profit/Loss";
			// 
			// lblPoint2DollarVal
			// 
			this.lblPoint2DollarVal.Location = new System.Drawing.Point(85, 116);
			this.lblPoint2DollarVal.Name = "lblPoint2DollarVal";
			this.lblPoint2DollarVal.Size = new System.Drawing.Size(50, 14);
			this.lblPoint2DollarVal.TabIndex = 31;
			this.lblPoint2DollarVal.Text = "0.66152";
			this.lblPoint2DollarVal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lblPoint2Dollar
			// 
			this.lblPoint2Dollar.AutoSize = true;
			this.lblPoint2Dollar.Location = new System.Drawing.Point(2, 117);
			this.lblPoint2Dollar.Name = "lblPoint2Dollar";
			this.lblPoint2Dollar.Size = new System.Drawing.Size(64, 13);
			this.lblPoint2Dollar.TabIndex = 30;
			this.lblPoint2Dollar.Text = "Point2Dollar";
			// 
			// lblPriceLevelSizeVal
			// 
			this.lblPriceLevelSizeVal.ForeColor = System.Drawing.Color.Black;
			this.lblPriceLevelSizeVal.Location = new System.Drawing.Point(88, 160);
			this.lblPriceLevelSizeVal.Name = "lblPriceLevelSizeVal";
			this.lblPriceLevelSizeVal.Size = new System.Drawing.Size(47, 14);
			this.lblPriceLevelSizeVal.TabIndex = 33;
			this.lblPriceLevelSizeVal.Text = "10";
			this.lblPriceLevelSizeVal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lblPriceLevelSize
			// 
			this.lblPriceLevelSize.AutoSize = true;
			this.lblPriceLevelSize.Location = new System.Drawing.Point(2, 161);
			this.lblPriceLevelSize.Name = "lblPriceLevelSize";
			this.lblPriceLevelSize.Size = new System.Drawing.Size(77, 13);
			this.lblPriceLevelSize.TabIndex = 32;
			this.lblPriceLevelSize.Text = "PriceMinimalStep";
			// 
			// lblBasisPriceVal
			// 
			this.lblBasisPriceVal.Location = new System.Drawing.Point(85, 173);
			this.lblBasisPriceVal.Name = "lblBasisPriceVal";
			this.lblBasisPriceVal.Size = new System.Drawing.Size(50, 14);
			this.lblBasisPriceVal.TabIndex = 35;
			this.lblBasisPriceVal.Text = "190,660";
			this.lblBasisPriceVal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lblBasisPrice
			// 
			this.lblBasisPrice.AutoSize = true;
			this.lblBasisPrice.Location = new System.Drawing.Point(2, 174);
			this.lblBasisPrice.Name = "lblBasisPrice";
			this.lblBasisPrice.Size = new System.Drawing.Size(56, 13);
			this.lblBasisPrice.TabIndex = 34;
			this.lblBasisPrice.Text = "BasisPrice";
			// 
			// lblSlippagesVal
			// 
			this.lblSlippagesVal.Location = new System.Drawing.Point(85, 190);
			this.lblSlippagesVal.Name = "lblSlippagesVal";
			this.lblSlippagesVal.Size = new System.Drawing.Size(50, 14);
			this.lblSlippagesVal.TabIndex = 37;
			this.lblSlippagesVal.Text = "50 / -50";
			this.lblSlippagesVal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lblSlippages
			// 
			this.lblSlippages.AutoSize = true;
			this.lblSlippages.Location = new System.Drawing.Point(2, 191);
			this.lblSlippages.Name = "lblSlippages";
			this.lblSlippages.Size = new System.Drawing.Size(53, 13);
			this.lblSlippages.TabIndex = 36;
			this.lblSlippages.Text = "Slippages";
			// 
			// lblEntrySignalVal
			// 
			this.lblEntrySignalVal.AutoSize = true;
			this.lblEntrySignalVal.Location = new System.Drawing.Point(2, 31);
			this.lblEntrySignalVal.Name = "lblEntrySignalVal";
			this.lblEntrySignalVal.Size = new System.Drawing.Size(131, 13);
			this.lblEntrySignalVal.TabIndex = 38;
			this.lblEntrySignalVal.Text = "BuyAtMarket(bar+1)=5333";
			// 
			// lblExitSignalVal
			// 
			this.lblExitSignalVal.AutoSize = true;
			this.lblExitSignalVal.Location = new System.Drawing.Point(2, 60);
			this.lblExitSignalVal.Name = "lblExitSignalVal";
			this.lblExitSignalVal.Size = new System.Drawing.Size(129, 13);
			this.lblExitSignalVal.TabIndex = 39;
			this.lblExitSignalVal.Text = "CoverAtLimit(bar+1)=5333";
			// 
			// TooltipPosition
			// 
			this.BackColor = System.Drawing.SystemColors.Info;
			this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.Controls.Add(this.lblExitSignalVal);
			this.Controls.Add(this.lblEntrySignalVal);
			this.Controls.Add(this.lblSlippagesVal);
			this.Controls.Add(this.lblSlippages);
			this.Controls.Add(this.lblBasisPriceVal);
			this.Controls.Add(this.lblBasisPrice);
			this.Controls.Add(this.lblPriceLevelSizeVal);
			this.Controls.Add(this.lblPriceLevelSize);
			this.Controls.Add(this.lblPoint2DollarVal);
			this.Controls.Add(this.lblPoint2Dollar);
			this.Controls.Add(this.lblGrossProfitLossVal);
			this.Controls.Add(this.lblGrossProfitLoss);
			this.Controls.Add(this.lblCommissionVal);
			this.Controls.Add(this.lblNetProfitLossValue);
			this.Controls.Add(this.lblSharesVal);
			this.Controls.Add(this.lblDistancePointsVal);
			this.Controls.Add(this.lblExitVal);
			this.Controls.Add(this.lblEntryVal);
			this.Controls.Add(this.lblDateVal);
			this.Controls.Add(this.lblCommission);
			this.Controls.Add(this.lblNetProfitLoss);
			this.Controls.Add(this.lblShares);
			this.Controls.Add(this.lblDistancePoints);
			this.Controls.Add(this.lblExit);
			this.Controls.Add(this.lblEntry);
			this.ForeColor = System.Drawing.SystemColors.InfoText;
			this.Name = "TooltipPosition";
			this.Size = new System.Drawing.Size(139, 215);
			this.ResumeLayout(false);
			this.PerformLayout();
		}
	}
}
