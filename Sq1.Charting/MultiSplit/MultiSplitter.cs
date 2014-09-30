using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace Sq1.Charting.MultiSplit {
	public class MultiSplitter : UserControl {
		public PanelNamedFolding PanelAbove;
		public PanelNamedFolding PanelBelow;
		public int ManualOrder;

		int GrabHandleWidth;
		Color ColorGrabHandle;
		bool DebugSplitter;
		
		public MultiSplitter(int grabHandleWidth, Color colorGrabHandle, bool debugSplitter = false) {
			GrabHandleWidth = grabHandleWidth;
			ColorGrabHandle = colorGrabHandle;
			DebugSplitter = debugSplitter;
		}

		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);
			if (base.DesignMode) return;
			try {
				Graphics g = e.Graphics;
				Rectangle grabRect = new Rectangle(0, 0, this.GrabHandleWidth, base.Height);
				using (SolidBrush grabBrush = new SolidBrush(this.ColorGrabHandle)) {
					g.FillRectangle(grabBrush, grabRect);
				}
				//this.DrawGripForSplitter(g);
				if (this.DebugSplitter) {
					if (string.IsNullOrEmpty(base.Text)) return;
					using (SolidBrush textBrush = new SolidBrush(this.ForeColor)) {
						g.DrawString(base.Text, base.Font, textBrush, 0, 0);
					}
				}
			} catch (Exception ex) {
				Debugger.Break();
			}
		}
		
		protected override void OnPaintBackground(PaintEventArgs e) {
			base.OnPaint(e);
			if (base.DesignMode) return;
			try {
				e.Graphics.Clear(this.BackColor);
			} catch (Exception ex) {
				Debugger.Break();
			}
		}

		public void DrawGripForSplitter(Graphics g) {
			Rectangle splitterRectangle = base.ClientRectangle;
		    Point centerPoint = new Point(splitterRectangle.Left - 1 + splitterRectangle.Width / 2, splitterRectangle.Top - 1 + splitterRectangle.Height / 2);
		    int dotSize = 2;
		    //Rectangle dotRect = new Rectangle(dotSize, dotSize);
		    using (Brush myFore = new SolidBrush(this.ForeColor)) {
			    g.FillEllipse(myFore, centerPoint.X, centerPoint.Y, dotSize, dotSize);
		        g.FillEllipse(myFore, centerPoint.X - 10, centerPoint.Y, dotSize, dotSize);
		        g.FillEllipse(myFore, centerPoint.X + 10, centerPoint.Y, dotSize, dotSize);
		    }
	    }
		public override string ToString() {
			string ret = "PANEL_BELOW_SPLITTER_IS_NULL";
			if (this.PanelBelow == null) return ret;
			ret = this.PanelBelow.PanelName;
			ret += ":" + this.Location.Y + "+" + this.Height + "=" + (this.Location.Y + this.Height);
			return ret;
		}
	}
}
