using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Core.DataTypes;
using Sq1.Core.Execution;
using Sq1.Core.Indicators;
using Sq1.Core.StrategyBase;

namespace Sq1.Charting {
	public partial class TooltipPrice {
		public TooltipPrice() {
			this.InitializeComponent();
			//base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			//base.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			//this.SetStyle(ControlStyles.Selectable, false);
		}
		public void PopulateTooltip(Bar barToPopulate, Dictionary<string, Indicator> indicators, List<Alert> alersForBar) {
			if (barToPopulate.IsBarStreaming) {
				this.BackColor = SystemColors.GradientActiveCaption;
			} else if (barToPopulate.IsBarStaticLast) {
				this.BackColor = SystemColors.ControlLight;
			} else {
				this.BackColor = SystemColors.Info;
			}

			string titleBlue = barToPopulate.DateTimeOpen.ToString("HH:mm ") + " #" + barToPopulate.ParentBarsIndex;
			this.lblHeaderVal.Text = titleBlue;
			this.lblDateValue.Text = barToPopulate.DateTimeOpen.ToString("ddd dd-MMM-yyyy");
			string formatPrice = "N" + barToPopulate.ParentBars.SymbolInfo.DecimalsPrice;
			this.lblOpenVal.Text = barToPopulate.Open.ToString(formatPrice);
			this.lblHighVal.Text = barToPopulate.High.ToString(formatPrice);
			this.lblLowVal.Text = barToPopulate.Low.ToString(formatPrice);
			this.lblCloseVal.Text = barToPopulate.Close.ToString(formatPrice);

			string formatVolume = "N" + barToPopulate.ParentBars.SymbolInfo.DecimalsPrice;
			this.lblVolumeVal.Text = barToPopulate.Volume.ToString(formatVolume);

			string alertsAsString = "";
			foreach (var alert in alersForBar) {
				if (alertsAsString != "") alertsAsString += "\r\n";
				alertsAsString += alert.ToStringForTooltip();
			}
			//this.toolTipAlerts.SetToolTip(this.lnkAlertsVal, alertsAsString);
			this.lnkAlertsVal.Text = alersForBar.Count + "";

			this.IndicatorLabelsBuild(barToPopulate, indicators);
		}
		
		
		int initialStaticHeight = 0;
		int spacingVerticalBeforeAnyDynamicIndicatorLabels = 5;
		int lineIncrement = 12;
		string dynamicItemPrefix = "lblIndicator_";
		public void IndicatorLabelsBuild(Bar barToPopulate, Dictionary<string, Indicator> indicators) {
			if (this.initialStaticHeight == 0) this.initialStaticHeight = this.Size.Height;
			this.SuspendLayout();
			this.IndicatorLabelsRemoveDecreaseTooltipHeight();
			this.IndicatorLabelsBuildIncreaseTooltipHeight(barToPopulate, indicators);
			this.ResumeLayout(true);
		}
		public void IndicatorLabelsBuildIncreaseTooltipHeight(Bar barToPopulate, Dictionary<string, Indicator> indicators) {
			Label ethalonName = this.lblVolume;
			Label ethalonValue = this.lblVolumeVal;
			
			int ethalonNameX = ethalonName.Location.X;
			int ethalonValueX = ethalonValue.Location.X;
			Size ethalonNameSize = ethalonName.Size;
			Size ethalonValueSize = ethalonValue.Size;
			
			int lastStaticTooltipLabelY = ethalonName.Location.Y + this.spacingVerticalBeforeAnyDynamicIndicatorLabels;
			int dynamicLabelY = lastStaticTooltipLabelY + this.lineIncrement;
			int expandTooltipHeightBy = 0;

			foreach (Indicator indicator in indicators.Values) {
				Label lblIndicatorName = new Label();
				lblIndicatorName.Name = dynamicItemPrefix + "_Name_" + indicator.NameWithParameters;
				lblIndicatorName.Text = indicator.Name;	//indicator.NameWithParameters;
				lblIndicatorName.Location = new Point(ethalonNameX, dynamicLabelY);
				lblIndicatorName.Size = ethalonNameSize;
				lblIndicatorName.AutoSize = true;
				lblIndicatorName.ForeColor = indicator.LineColor;
				
				Label lblIndicatorValue = new Label();
				lblIndicatorValue.Name = dynamicItemPrefix + "_Value_" + indicator.NameWithParameters;
				lblIndicatorValue.Text = indicator.FormatValueForBar(barToPopulate);
				lblIndicatorValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
				lblIndicatorValue.Anchor = ((AnchorStyles)((AnchorStyles.Top | AnchorStyles.Right)));
				lblIndicatorValue.Location = new System.Drawing.Point(ethalonValueX, dynamicLabelY);
				lblIndicatorValue.Size = ethalonValueSize;

				this.Controls.Add(lblIndicatorName);
				this.Controls.Add(lblIndicatorValue);

				dynamicLabelY += this.lineIncrement;
				expandTooltipHeightBy += this.lineIncrement; 
			}
			
			if (expandTooltipHeightBy > 0) {
				expandTooltipHeightBy += spacingVerticalBeforeAnyDynamicIndicatorLabels;
				this.Size = new Size(this.Size.Width, this.initialStaticHeight + expandTooltipHeightBy);
			}
		}
		
		public void IndicatorLabelsRemoveDecreaseTooltipHeight() {
			foreach (Control item in this.Controls) {
				//Label label = item as Label;
				//if (label == null) continue;
				if (item.Name.Contains(dynamicItemPrefix) == false) continue;
				this.Controls.Remove(item);
			}
			this.Size = new Size(this.Size.Width, this.initialStaticHeight);
		}
	}
}
