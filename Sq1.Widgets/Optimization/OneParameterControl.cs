using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

using BrightIdeasSoftware;

namespace Sq1.Widgets.Optimization {
	public partial class OneParameterControl : UserControl {
		private OneParameterAllValuesAveraged oneParamForOneOLV;

		public OneParameterControl() {
			InitializeComponent();
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
		}

		public OneParameterControl(OneParameterAllValuesAveraged oneParamForOneOLV) : this() {
			this.oneParamForOneOLV = oneParamForOneOLV;
			this.olvcParamValues.Text = this.oneParamForOneOLV.ParameterName;
			this.olvCustomize();
			this.olv.SetObjects(this.oneParamForOneOLV.AllValuesForOneParameterWithAverages);
		}

		internal void KPIsLocalRecalculateDone_refreshOLV() {
			this.olv.SetObjects(this.oneParamForOneOLV.AllValuesForOneParameterWithAverages);
		}
	}
}
