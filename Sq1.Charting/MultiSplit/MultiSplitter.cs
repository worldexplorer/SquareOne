using System;
using System.Drawing;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Core.DoubleBuffered;

namespace Sq1.Charting.MultiSplit {
	public partial class MultiSplitter
#if NON_DOUBLE_BUFFERED
//#Dev doesn't F12 on PanelAbove 
			: UserControl
#else
			: UserControlDoubleBuffered
#endif
			/* where PANEL_BASE : Control */ {
		
		
		public	Control	PanelAbove;
		public	Control	PanelBelow;

				int		grabHandleWidth;
				Color	grabHandleColor;
				bool	debugSplitter;
				bool	verticalizeAllLogic;
		
		public MultiSplitter(int grabHandleWidth, Color grabHandleColor, bool verticalizeAllLogic = false, bool debugSplitter = false) {
			this.grabHandleWidth = grabHandleWidth;
			this.grabHandleColor = grabHandleColor;
			this.verticalizeAllLogic = verticalizeAllLogic;
			this.debugSplitter = debugSplitter;
		}

#if NON_DOUBLE_BUFFERED		//WHEN_INHERITED_FROM_REGULAR_USERCONTROL
		protected override void OnPaintBackground(PaintEventArgs e) {
			base.OnPaintBackground(e);
			// before_went_doublebuffered??? base.OnPaint(e);
#else						//WHEN_INHERITED_FROM_USERCONTROL_DOUBLEBUFFERED
		protected override void OnPaintBackgroundDoubleBuffered(PaintEventArgs e) {
#endif
			if (base.DesignMode) return;
			try {
				e.Graphics.Clear(this.BackColor);
				// MAKES_SENSE_TO_REPAINT_EACH_ONRESIZE()_HERE_TO_AVOID_BLACK_UNTIL_MOUSEOVER AND BASE_DOESNT_PAINT_POSPONING_FLUSHING_BACKGROUNDS_ON_FINAL_FOREGROUND
				// YES_SOLVES_THE_PROBLEM_BUT_VERY_SLOW base.BufferedGraphics.Render(e.Graphics);
			} catch (Exception ex) {
				string msig = " //MultiSplitter.OnPaintBackground()";
				string msg = "SHOULD_NEVER_HAPPEN I_DONT_THINK_GRAPHICS.CLEAR_WOULD_EVER_THROW";
				Assembler.PopupException(msg + msig, ex, false);
			}
		}

#if NON_DOUBLE_BUFFERED		//WHEN_INHERITED_FROM_REGULAR_USERCONTROL
		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);
#else						//WHEN_INHERITED_FROM_USERCONTROL_DOUBLEBUFFERED
		protected override void OnPaintDoubleBuffered(PaintEventArgs e) {
#endif
			if (base.DesignMode) return;
			try {
				Graphics g = e.Graphics;
				Rectangle grabRect = this.verticalizeAllLogic == false
					? new Rectangle(0, 0, this.grabHandleWidth, base.Height)
					: new Rectangle(0, 0, base.Width, this.grabHandleWidth);
				using (SolidBrush grabBrush = new SolidBrush(this.grabHandleColor)) {
					g.FillRectangle(grabBrush, grabRect);
				}
				//this.DrawGripForSplitter(g);
				if (this.debugSplitter) {
					if (string.IsNullOrEmpty(base.Text)) return;
					using (SolidBrush textBrush = new SolidBrush(this.ForeColor)) {
						g.DrawString(base.Text, base.Font, textBrush, 0, 0);
					}
				}
				// NO_NEED_BASE_WILL_SPIT_MY_GRAPHICS this.BufferedGraphics.Render(pe.Graphics);
			} catch (Exception ex) {
				string msg = "I_ONLY_DID_g.DrawString()_AND_g.FillRectangle() //MultiSplitter.OnPaint()";
				Assembler.PopupException(msg, ex);
			}
		}
		//public void DrawGripForSplitter(Graphics g) {
		//	Rectangle splitterRectangle = base.ClientRectangle;
		//	Point centerPoint = new Point(splitterRectangle.Left - 1 + splitterRectangle.Width / 2, splitterRectangle.Top - 1 + splitterRectangle.Height / 2);
		//	int dotSize = 2;
		//	//Rectangle dotRect = new Rectangle(dotSize, dotSize);
		//	using (Brush myFore = new SolidBrush(this.ForeColor)) {
		//		g.FillEllipse(myFore, centerPoint.X, centerPoint.Y, dotSize, dotSize);
		//		g.FillEllipse(myFore, centerPoint.X - 10, centerPoint.Y, dotSize, dotSize);
		//		g.FillEllipse(myFore, centerPoint.X + 10, centerPoint.Y, dotSize, dotSize);
		//	}
		//}
		public override string ToString() {
			string ret = "PANEL_BELOW_SPLITTER_IS_NULL";
			if (this.PanelBelow == null) return ret;
			ret = this.PanelBelow.Name;
			ret += ":" + this.Location.Y + "+" + this.Height + "=" + (this.Location.Y + this.Height);
			return ret;
		}
		private void InitializeComponent() {
			this.SuspendLayout();
			this.Name = "MultiSplitter";
			this.Size = new System.Drawing.Size(783, 10);
			this.ResumeLayout(false);
		}
	}
}
