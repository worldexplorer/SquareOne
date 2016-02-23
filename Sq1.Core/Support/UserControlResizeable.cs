using System;
using System.Drawing;
using System.Windows.Forms;

namespace Sq1.Core.Support {
	public partial class UserControlResizeable : UserControl {
		public Padding PaddingMouseReceiving { get; private set; }
		bool mouseLeftButtonPressedNow;
		int borderThickness;

		public UserControlResizeable() {
			PaddingMouseReceiving = new Padding(4, 4, 4, 4);
			InitializeComponent();
			//DO_IT_AFTER_ADDING_OneParameter_IN_ME: A circular control reference has been made. A control cannot be owned by or parented to itself
			//Initialize_byMovingControlsToInner();
			this.borderThickness = 0;
			if (this.BorderStyle == BorderStyle.FixedSingle) this.borderThickness = 1;
		}

		public void Initialize_byMovingControlsToInner() {
			foreach (Control ctrl in base.Controls) {
				if (ctrl == this.UserControlInner) continue;
				this.UserControlInner.Controls.Add(ctrl);
			}
		}

		// WinForms' runtime $exception	{"A circular control reference has been made. A control cannot be owned by or parented to itself."}
		//public new ControlCollection Controls { get {
		//	return this.UserControlInner.Controls;
		//} }

		//private void oneParameterControl1_Paint(object sender, PaintEventArgs e) {
		////protected void Paint(PaintEventArgs e) {
		////	base.Paint(e);
		//	this.UserControlInner.Paint(this, e);
		//}

		protected override void OnResize(EventArgs e) {
			this.UserControlInner.Location = new Point(this.PaddingMouseReceiving.Left + this.borderThickness, this.PaddingMouseReceiving.Top + this.borderThickness);
			this.UserControlInner.Size = new Size(
				base.Width	- this.PaddingMouseReceiving.Right	- this.UserControlInner.Location.X - this.borderThickness*2,
				base.Height - this.PaddingMouseReceiving.Bottom - this.UserControlInner.Location.Y - this.borderThickness*2);
			base.OnResize(e);
		}

		bool resizingTopCorner_invertMouseDeltaY;
		bool resizingLeftCorner_invertMouseDeltaX;
		Point mouseXYprev;
		//void UserControlResizeable_MouseMove(object sender, MouseEventArgs e) {
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			if (this.mouseLeftButtonPressedNow) {
				int mouseXdelta = e.X - this.mouseXYprev.X;
				int mouseYdelta = e.Y - this.mouseXYprev.Y;

				if (resizingLeftCorner_invertMouseDeltaX) mouseXdelta *= -1;
				if (resizingTopCorner_invertMouseDeltaY)  mouseYdelta *= -1;

				if (base.Cursor == Cursors.SizeWE) {
					base.Width	+= mouseXdelta;
				}
				if (base.Cursor == Cursors.SizeNS) {
					base.Height += mouseYdelta;
				}
				if (base.Cursor == Cursors.SizeNWSE) {
					if (resizingTopCorner_invertMouseDeltaY) mouseXdelta *= -1;		// looks illogical but works
					base.Width  += mouseXdelta;
					base.Height += mouseYdelta;
				}
				if (base.Cursor == Cursors.SizeNESW) {
					base.Width += mouseXdelta;
					base.Height += mouseYdelta;
				}
				mouseXYprev = e.Location;
				return;
			}

			bool overBorderLeft		= e.X <= this.PaddingMouseReceiving.Left;
			bool overBorderTop		= e.Y <= this.PaddingMouseReceiving.Top;
			bool overBorderRight	= e.X >= this.Width	 - this.PaddingMouseReceiving.Right;
			bool overBorderBottom	= e.Y >= this.Height - this.PaddingMouseReceiving.Bottom;

			this.resizingTopCorner_invertMouseDeltaY  = overBorderTop;
			this.resizingLeftCorner_invertMouseDeltaX = overBorderLeft && overBorderTop == false;

			if (overBorderLeft && overBorderTop || overBorderRight && overBorderBottom) {
				if (base.Cursor != Cursors.SizeNWSE) {
					base.Cursor = Cursors.SizeNWSE;
				}
				return;
			}
			if (overBorderLeft && overBorderBottom || overBorderRight && overBorderTop) {
				if (base.Cursor != Cursors.SizeNESW) {
					base.Cursor = Cursors.SizeNESW;
				}
				return;
			}

			if (overBorderRight || overBorderLeft) {
				if (base.Cursor != Cursors.SizeWE) {
					base.Cursor  = Cursors.SizeWE;
				}
				return;
			}
			if (overBorderBottom || overBorderTop) {
				if (base.Cursor != Cursors.SizeNS) {
					base.Cursor  = Cursors.SizeNS;
				}
				return;
			}
			base.Cursor = Cursors.Default;
		}

		//this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.UserControlResizeable_MouseDown);
		//this.MouseLeave += new System.EventHandler(this.UserControlResizeable_MouseLeave);
		//this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.UserControlResizeable_MouseMove);
		//this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.UserControlResizeable_MouseUp);

		//private void UserControlResizeable_MouseDown(object sender, MouseEventArgs e) {
		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			this.mouseLeftButtonPressedNow = e.Button == MouseButtons.Left;
			this.mouseXYprev = e.Location;
		}

		//private void UserControlResizeable_MouseUp(object sender, MouseEventArgs e) {
		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			this.mouseLeftButtonPressedNow = false;
		}
	}
}
