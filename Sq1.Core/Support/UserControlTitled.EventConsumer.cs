using System;
using System.ComponentModel;
using System.Windows.Forms;

using System.Drawing;

namespace Sq1.Core.Support {
	public partial class UserControlTitled : UserControlResizeable {
		protected override void OnResize(EventArgs e) {
			base.OnResize(e);
			if (this.pnlWindowTitle == null) return;

			base.UserControlInner.Location = new Point(
			    base.UserControlInner.Location.X,
			    base.UserControlInner.Location.Y + this.pnlWindowTitle.Height - 1);
			base.UserControlInner.Size = new Size(
			    base.UserControlInner.Width,
			    base.UserControlInner.Height - this.pnlWindowTitle.Height + 1);
		}


		void lblTitle_MouseEnter(object sender, EventArgs e) {	base.Cursor = Cursors.SizeAll; }
		void lblTitle_MouseLeave(object sender, EventArgs e) {	base.Cursor = Cursors.Arrow; }


		bool mouseHeld;
		void lblTitle_MouseDown(object sender, MouseEventArgs e) {	this.mouseHeld = true;	}
		void lblTitle_MouseUp(object sender, MouseEventArgs e) {	this.mouseHeld = false;	}

		Point mouseLocation_lastUpdated;
		void lblWindowTitle_MouseMove(object sender, MouseEventArgs e) {
			if (this.mouseHeld == false) return;

			int moveWindowToRight	= e.Location.X - this.mouseLocation_lastUpdated.X;
			int moveWindowToDown	= e.Location.Y - this.mouseLocation_lastUpdated.Y;

			Point newLocation = new Point(
				base.Location.X + moveWindowToRight,
				base.Location.Y + moveWindowToDown);
			base.Location = newLocation;

			this.mouseLocation_lastUpdated = e.Location;
		}
	}
}
