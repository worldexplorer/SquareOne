using System;
using System.Drawing;
using System.Windows.Forms;

namespace Sq1.Widgets.SteppingSlider {
	public partial class PanelFillSlider {

		//WHEN_INHERITED_FROM_REGULAR_USERCONTROL protected override void OnPaint(PaintEventArgs e) {
		protected override void OnPaintDoubleBuffered(PaintEventArgs pe) {
			int paddingLeft = 3;
			int paddingTop = 0;
			int paddingRight = 3;
			// base.ForeColor initialized after ctor(), in parent's InitializeComponents
			if (this.brushFgText == null) this.brushFgText = new SolidBrush(base.ForeColor);
			try {
				Graphics g = pe.Graphics;
				int valueMinWidthMeasured = (int)Math.Round(g.MeasureString(this.ValueMin.ToString(this.ValueFormat), this.Font).Width);
				int valueCurrentWidthMeasured = (int)Math.Round(g.MeasureString(this.ValueCurrent.ToString(this.ValueFormat), this.Font).Width);
				int valueMouseOverWidthMeasured = (int)Math.Round(g.MeasureString(this.ValueMouseOver.ToString(this.ValueFormat), this.Font).Width);
				int valueMaxWidthMeasured = (int)Math.Round(g.MeasureString(this.ValueMax.ToString(this.ValueFormat), this.Font).Width);
				int labelTextWidthMeasured = (int)Math.Round(g.MeasureString(this.LabelText + "", this.Font).Width);
				int labelYposition = base.Padding.Top + paddingTop;

				decimal range = Math.Abs(this.ValueMax - this.ValueMin);
				if (range == 0) {
					string msg = "ValueMax[" + this.ValueMax + "] - ValueMin[" + this.ValueMin + "] = 0";
					g.DrawString(msg, this.Font, this.brushFgText, base.Padding.Left, base.Padding.Top);
					return;
				}
				int widthMinusBorder = base.Width + base.Padding.Left + base.Padding.Right;
				
				if (this.LeftToRight) {		// INVERTED_SLIDER_ORIGINAL TODO: DO_PIXEL_PERFECT_REFACTORING
					float partOfRange = (float)(this.ValueCurrent - this.ValueMin) / (float)range;
					this.FilledPercentageCurrentValue = 100 * partOfRange;
					int widthToFillCurrentValue = (int)Math.Round(widthMinusBorder * partOfRange);
					
					g.FillRectangle(brushBgValueCurrent, 0, 0, widthToFillCurrentValue, base.Height);
					using (Pen curPurple = new Pen(base.ForeColor)) {
						g.DrawLine(curPurple, widthToFillCurrentValue, 0, widthToFillCurrentValue, base.Height);
					}

					int widthToFillMouseOver = 0;
					if (this.mouseOver) {
						widthToFillMouseOver = (int)Math.Round((widthMinusBorder * this.FilledPercentageMouseOver / 100));
						g.FillRectangle(brushBgMouseOver, 0, 0, widthToFillMouseOver, base.Height);
					}
	
					int valueCurrentXpos = widthToFillCurrentValue - valueCurrentWidthMeasured - base.Padding.Right - paddingRight;
					int valueMinEndsAt = valueMinWidthMeasured + base.Padding.Left;
					bool currentOverlapsMin = valueCurrentXpos < valueMinEndsAt;
					if (currentOverlapsMin == false) {
						g.DrawString(this.ValueMin.ToString(this.ValueFormat), this.Font, this.brushFgText, base.Padding.Left, labelYposition);
					}
	
					int labelTextXposition = base.Padding.Left + valueMinWidthMeasured + paddingLeft;
					int valueMaxXpos = widthMinusBorder - valueMaxWidthMeasured - base.Padding.Right - paddingRight;
					bool currentOverlapsMax = widthToFillCurrentValue > valueMaxXpos;
					if (currentOverlapsMax == false || this.mouseOver == true) {
						g.DrawString(this.ValueMax.ToString(this.ValueFormat), this.Font, this.brushFgText, valueMaxXpos, labelYposition);
// LOOKS LIKE NONSENSE here in ORIGINAL LeftToRight version (TODO: comment out and observe the "missing")
						if (labelTextXposition + labelTextWidthMeasured > valueMaxXpos) {
							// paint bg under MAX label if it overlaps long labelText
							if (brushBgControl == null) brushBgControl = new SolidBrush(base.BackColor);
							g.FillRectangle(brushBgControl, valueMaxXpos - base.Padding.Left, 0, valueMaxXpos + base.Padding.Right, base.Height);
						}
					}
	
					if (this.mouseOver == true && this.ValueMouseOver > this.ValueMin) {	// && this.ValueMouseOver < this.ValueMax) {
						g.DrawString(this.LabelText, this.Font, this.brushFgParameterLabel, labelTextXposition, labelYposition);
	
						int valueMouseOverXpos = widthToFillMouseOver - valueMouseOverWidthMeasured - base.Padding.Right - paddingRight;
						if (valueMouseOverXpos < base.Padding.Right + paddingRight) valueMouseOverXpos = base.Padding.Right + paddingRight;
						g.FillRectangle(brushBgMouseOver, valueMouseOverXpos, 0, valueMouseOverWidthMeasured, base.Height);
	
						g.DrawString(this.ValueMouseOver.ToString(this.ValueFormat), this.Font, this.brushFgText, valueMouseOverXpos, labelYposition);
					} else {
						g.DrawString(this.LabelText, this.Font, this.brushFgParameterLabel, labelTextXposition, labelYposition);
						g.DrawString(this.ValueCurrent.ToString(this.ValueFormat), this.Font, this.brushFgText, valueCurrentXpos, labelYposition);
					}
				} else {		// INVERTED_SLIDER_ORIGINAL_PORTED_BY_MIRRORRING; MAX and MIN INVERTED BELOW
					float partOfRange = (float)(this.ValueCurrent - this.ValueMax) / (float)range;
					this.FilledPercentageCurrentValue = 100 * partOfRange;
					int widthToFillCurrentValue = (int)Math.Round(widthMinusBorder * partOfRange);

					g.FillRectangle(brushBgValueCurrent, base.Width - widthToFillCurrentValue, 0, widthToFillCurrentValue, base.Height);
					using (Pen curPurple = new Pen(base.ForeColor)) {
						g.DrawLine(curPurple, widthToFillCurrentValue, 0, widthToFillCurrentValue, base.Height);
					}
	
					int widthToFillMouseOver = 0;
					if (this.mouseOver) {
						widthToFillMouseOver = (int)Math.Round((widthMinusBorder * this.FilledPercentageMouseOver / 100));
						g.FillRectangle(brushBgMouseOver, base.Width - this.Padding.Right - widthToFillMouseOver, 0, widthToFillMouseOver, base.Height);
					}
	
					int valueLeftXpos = widthMinusBorder - valueMinWidthMeasured - base.Padding.Right - paddingRight;
					bool currentOverlapsRight = widthToFillCurrentValue > valueLeftXpos;
					if (currentOverlapsRight == false) {
						g.DrawString(this.ValueMax.ToString(this.ValueFormat), this.Font, this.brushFgText, valueLeftXpos, labelYposition);
					}

					int labelTextXposition = base.Padding.Left + valueMaxWidthMeasured + paddingLeft;
					int valueCurrentXpos = base.Width - widthToFillCurrentValue + base.Padding.Left + paddingLeft;
					if (this.mouseOver == true) {		// && this.ValueMouseOver > this.ValueMax
						g.DrawString(this.LabelText, this.Font, this.brushFgParameterLabel, labelTextXposition, labelYposition);
	
						int valueMouseOverXpos = base.Width - widthToFillMouseOver + paddingLeft;
						int valueMouseOverXposStillVisible = base.Width - valueMouseOverWidthMeasured + base.Padding.Left - paddingLeft;
						if (valueMouseOverXpos > valueMouseOverXposStillVisible) valueMouseOverXpos = valueMouseOverXposStillVisible;
						g.FillRectangle(brushBgMouseOver, valueMouseOverXpos, 0, valueMouseOverWidthMeasured, base.Height);
	
						g.DrawString(this.ValueMouseOver.ToString(this.ValueFormat), this.Font, this.brushFgText, valueMouseOverXpos, labelYposition);
					} else {
						g.DrawString(this.LabelText, this.Font, this.brushFgParameterLabel, labelTextXposition, labelYposition);
						g.DrawString(this.ValueCurrent.ToString(this.ValueFormat), this.Font, this.brushFgText, valueCurrentXpos, labelYposition);
					}

					int valueLeftEndsAt = valueMaxWidthMeasured + base.Padding.Left + base.Padding.Right;
					bool currentOverlapsLeft = valueCurrentXpos < valueLeftEndsAt;
					if (currentOverlapsLeft == false || this.mouseOver == true) {
						g.DrawString(this.ValueMin.ToString(this.ValueFormat), this.Font, this.brushFgText, base.Padding.Left, labelYposition);
					}
				}
				//DOUBLEBUFFERED_PARENT_DID_THAT base.OnPaint(pe);
			} catch (Exception e) {
				string msg = "WindProc won't catch your exceptions; keep a breakpoint here";
			}
		}
	}
}