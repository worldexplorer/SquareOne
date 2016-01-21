using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;

using Sq1.Core;
using Sq1.Core.DataTypes;
using Sq1.Core.Support;

using Sq1.Adapters.Quik.Streaming.Dde;

namespace Sq1.Adapters.Quik.Streaming.Monitor {
	public partial class QuikStreamingMonitorDomUserControl {	
		void olvDomCustomize() {
			this.olvDomCustomize_cellBackgound();

			this.olvAsk.AspectGetter = delegate(object o) {
				LevelTwoOlvEachLine askPriceBid = o as LevelTwoOlvEachLine;
				if (askPriceBid == null) return "olvAsk.AspectGetter: askPriceBid=null";
				if (double.IsNaN(askPriceBid.Ask)) return null;
				string formatVolume = this.tableLevel2.FormatVolume;
				return string.Format(formatVolume, askPriceBid.Ask);
			};
			this.olvPrice.AspectGetter = delegate(object o) {
				LevelTwoOlvEachLine askPriceBid = o as LevelTwoOlvEachLine;
				if (askPriceBid == null) return "olvPrice.AspectGetter: askPriceBid=null";
				string formatPrice = this.tableLevel2.FormatPrice;
				return string.Format(formatPrice, askPriceBid.Price);
			};
			this.olvBid.AspectGetter = delegate(object o) {
				LevelTwoOlvEachLine askPriceBid = o as LevelTwoOlvEachLine;
				if (askPriceBid == null) return "olvBid.AspectGetter: askPriceBid=null";
				if (double.IsNaN(askPriceBid.Bid)) return null;
				string formatVolume = this.tableLevel2.FormatVolume;
				return string.Format(formatVolume, askPriceBid.Bid);
			};
		}
		
		//Color	LevelTwoLotsColorForeground;
		Color	LevelTwoAskColorBackground;
		//Color	LevelTwoAskColorContour;
		Color	LevelTwoBidColorBackground;
		//Color	LevelTwoBidColorContour;

		void olvDomCustomize_cellBackgound() {
			// colors copypasted from ChartSettings.cs
			//this.LevelTwoLotsColorForeground	= Color.Black;
			this.LevelTwoAskColorBackground		= Color.FromArgb(255, 230, 230);
			//this.LevelTwoAskColorContour		= Color.FromArgb(this.LevelTwoAskColorBackground.R - 50, this.LevelTwoAskColorBackground.G - 50, this.LevelTwoAskColorBackground.B - 50);
			this.LevelTwoBidColorBackground		= Color.FromArgb(230, 255, 230);
			//this.LevelTwoBidColorContour		= Color.FromArgb(this.LevelTwoBidColorBackground.R - 50, this.LevelTwoBidColorBackground.G - 50, this.LevelTwoBidColorBackground.B - 50);

			this.olvcLevelTwo.FormatCell += new EventHandler<BrightIdeasSoftware.FormatCellEventArgs>(olvcLevelTwo_FormatCell);
			this.olvcLevelTwo.UseCellFormatEvents = true;
		}

		void olvcLevelTwo_FormatCell(object sender, BrightIdeasSoftware.FormatCellEventArgs e) {			
			LevelTwoOlvEachLine askPriceBid = e.Model as LevelTwoOlvEachLine;
			if (askPriceBid == null) {
				string msg = "MUST_BE_LevelTwoOlvEachLine e.Model[" + e.Model + "] //olvcLevelTwo_FormatCell()";
				Assembler.PopupException(msg, null, false);
			}
			if (e.Column == this.olvAsk) {
				if (askPriceBid != null && askPriceBid.BidOrAsk != BidOrAsk.Ask) return;
				e.SubItem.BackColor = this.LevelTwoAskColorBackground;
				return;
			}
			if (e.Column == this.olvBid) {
				if (askPriceBid != null && askPriceBid.BidOrAsk != BidOrAsk.Bid) return;
				e.SubItem.BackColor = this.LevelTwoBidColorBackground;
				return;
			}
		}
	}
}
