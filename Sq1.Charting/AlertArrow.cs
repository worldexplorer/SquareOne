using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using Sq1.Core.Execution;

namespace Sq1.Charting {
	public class AlertArrow {	// TODO : Rectangle
		public Position Position;
		public int Ytransient;
		public int XBarMiddle;
		public int Xtransient { get {
				float arrowTipOffsetFromBitmapLeftEdge = 0.9f;
				int offsetForOpeningArrows = (int)Math.Round(this.Width * arrowTipOffsetFromBitmapLeftEdge);
				int offsetForClosingArrows = (int)Math.Round(this.Width * (1 - arrowTipOffsetFromBitmapLeftEdge));
				int leftEdgeDisplacedForTipToPointOnBarShadow = (this.ArrowIsForPositionEntry)
					? this.XBarMiddle - offsetForOpeningArrows
					: this.XBarMiddle - offsetForClosingArrows;
				return leftEdgeDisplacedForTipToPointOnBarShadow; 
			} }
		
		public bool ArrowIsForPositionEntry { get; private set; }
		public double PriceAt { get {
				double ret = -1;
				if (this.ArrowIsForPositionEntry) {
					ret = this.Position.IsEntryFilled ? this.Position.EntryPriceScript : this.Position.EntryFilledPrice;
				} else {
					ret = this.Position.IsExitFilled ? this.Position.ExitPriceScript : this.Position.ExitFilledPrice;
				}
				return ret;
			} }
		public double PriceAtBeyondBarRectangle { get {
				double ret;
				if (this.AboveBar) {
					ret = this.ArrowIsForPositionEntry ? this.Position.EntryBar.High : this.Position.ExitBar.High;
				} else {
					ret = this.ArrowIsForPositionEntry ? this.Position.EntryBar.Low : this.Position.ExitBar.Low;
				}
				return ret;
			} }
		
		public int BarIndexFilled { get {
				return this.ArrowIsForPositionEntry
					? this.Position.EntryFilledBarIndex
					: this.Position.ExitFilledBarIndex; } }
		public bool AboveBar {
			get {
				return
					//this.AlertArrow.IsShort ? true : false;
					this.Position.IsShort
						? this.ArrowIsForPositionEntry ? true : false
						: this.ArrowIsForPositionEntry ? false : true;
			} }
		public Bitmap bitmap;
		public Bitmap Bitmap { get {
				if (this.bitmap == null) {
					// OPENING_ARROW_WILL_ALWAYS_BE_GRAY
					if (this.ArrowIsForPositionEntry == false) {
						// v2 caching AlertArrow.Bitmap: ExitAlert always
						this.bitmap = this.AboveBar
							? (this.Position.NetProfit > 0.0) ? Sq1.Charting.Properties.Resources.LongExitProfit : Sq1.Charting.Properties.Resources.LongExitLoss
							: (this.Position.NetProfit > 0.0) ? Sq1.Charting.Properties.Resources.ShortExitProfit : Sq1.Charting.Properties.Resources.ShortExitLoss;
					} else {
						// v1 NON_DYNAMIC return this.AboveBar ? Sq1.Charting.Properties.Resources.ShortEntryUnknown : Sq1.Charting.Properties.Resources.LongEntryUnknown;
						Bitmap ret = this.AboveBar
								? (this.Position.NetProfit > 0.0) ? Sq1.Charting.Properties.Resources.ShortEntryProfit : Sq1.Charting.Properties.Resources.ShortEntryLoss
								: (this.Position.NetProfit > 0.0) ? Sq1.Charting.Properties.Resources.LongEntryProfit : Sq1.Charting.Properties.Resources.LongEntryLoss;
						if (this.Position.ExitFilledPrice == -1) {
							// v2 don't cache bitmaps for opening Alerts, calculate their color on-the-fly for every incoming quote
							return ret;							
						}
						// v2 caching AlertArrow.Bitmap: EntryAlert ExitAlert arrived, cache red/green into this.bitmap   
						this.bitmap = ret; 
					}
				}
				return this.bitmap;
			} }
		
		private TextureBrush bitmapTextureBrush_cached;
		public TextureBrush BitmapTextureBrush { get {
				if (bitmapTextureBrush_cached == null && this.Bitmap != null) {
					bitmapTextureBrush_cached = new TextureBrush(this.Bitmap, WrapMode.TileFlipY);	//, this.ClientRectangle
				}
				return bitmapTextureBrush_cached;
			} }
		public Rectangle ClientRectangle { get { return new Rectangle(this.Xtransient, this.Ytransient,
																		(this.bitmap == null) ? 0: this.Width,
																		(this.bitmap == null) ? 0: this.Height); } }
		//public Rectangle ClientRectangle { get { return new Rectangle(this.Xtransient, this.Ytransient, 12, 12); } }
		public Point Location { get { return new Point(this.Xtransient, this.Ytransient); } }

		public int Width { get { return this.Bitmap.Width; } }
		public int Height { get { return this.Bitmap.Height; } }
		
		public AlertArrow(Position position, bool arrowIsForPositionEntry) {
			this.Position = position;
			this.ArrowIsForPositionEntry = arrowIsForPositionEntry;
			this.Ytransient = 0;
			this.XBarMiddle = 99;
			//this.Location = new Point(0, 0);
		}
		public override string ToString() {
			string ret = "ArrowFor" + ((this.ArrowIsForPositionEntry) ? "Entry" : "Exit");
			ret += this.Position.ToString();
			return ret;
		}
	}
}