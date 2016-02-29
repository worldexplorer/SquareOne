using System;
using System.Windows.Forms;

using Sq1.Core;

namespace Sq1.Widgets.RangeBar {
	public abstract partial class RangeBar<T> {
		protected override void OnMouseEnter(EventArgs e) {
			this.mouseOver = true;
			this.dragging = false;
			base.Invalidate();
			base.OnMouseEnter(e);
		}
		protected override void OnMouseLeave(EventArgs e) {
			this.mouseOver = false;
			this.dragging = false;
			base.Invalidate();	// visuals first!!!
			this.ValueMouseOverRangePercentage = -1;
			this.RaiseOnValueMouseOverChanged();
			base.OnMouseLeave(e);
		}		
		protected override void OnMouseDown(MouseEventArgs e) {
			if (e.Button != MouseButtons.Left) return;
			this.dragButtonPressed = true;
			this.dragging = false;
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			string msig = " //OnMouseUp() " + this.ToString();

			if (e.Button == MouseButtons.Right) {
				string msg = "QUIK_HACK_TO_RESET_SELECTED_RANGE_TO_FULL_RANGE";
				try {
					this.ValueMin = this.RangeMin;
					this.ValueMax = this.RangeMax;
					this.dragging = false;
					this.dragButtonPressed = false;
					base.Invalidate();	// visuals first!!!
					this.RaiseOnValuesMinAndMaxChanged();
				} catch (Exception ex) {
					string msg1 = "WindProc won't catch your exceptions; keep a breakpoint here";
					Assembler.PopupException(msg1 + msig, ex);
				}
				base.OnMouseUp(e);
				return;
			}

			if (e.Button != MouseButtons.Left) return;	// dont react on middle button and other gaming function keys
			try {
				if (this.ValueMouseOverRangePercentage > 1) {
					string msg = "NEVER_OBSERVED_BEFORE Houston we have a problem";
					Assembler.PopupException(msg + msig);
				}
				if (this.dragging == false) {
					if (this.ValueMouseOverRangePercentage < this.ValueMinMaxMedianPercentage) {
						this.ValueMin = this.ValueMouseOverFromRangePercentage;
						base.Invalidate();	// visuals first!!!
						this.RaiseOnValueMinChanged();
					} else {
						this.ValueMax = this.ValueMouseOverFromRangePercentage;
						base.Invalidate();	// visuals first!!!
						this.RaiseOnValueMaxChanged();
					}
				} else {
					// handling dragEnd here
					float dragSmaller = this.dragStartedPercentage;
					float dragGreater = this.ValueMouseOverRangePercentage;
					if (dragSmaller > dragGreater) {
						dragSmaller = this.ValueMouseOverRangePercentage;
						dragGreater = this.dragStartedPercentage;
					}
					// fixed using XbecomesNegativeWhileDragging
					//if (dragSmaller < 0) dragSmaller = 0;		// when we released Left mouse button beyond  left edge of the RangeBar control
					//if (dragGreater > 1) dragGreater = 1; 		// when we released Left mouse button beyond right edge of the RangeBar control
					this.ValueMin = this.ValueFromPercentage(dragSmaller);
					this.ValueMax = this.ValueFromPercentage(dragGreater);
					this.dragStartedPercentage = -1;
					this.dragging = false;
					base.Invalidate();	// visuals first!!!
					this.RaiseOnValuesMinAndMaxChanged();
				}
				this.dragButtonPressed = false;
			} catch (Exception ex) {
				string msg = "WindProc won't catch your exceptions; keep a breakpoint here";
				Assembler.PopupException(msg + msig, ex);
				//throw ex;
			}
			base.OnMouseUp(e);
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			try {
				if (this.dragging) {
					// should drag a little for me to consider the user is really dragging anything
					if (Math.Abs(dragStartedX - e.X) < this.dragStartSensitivity) return;
					int XnotBeyond0width = e.X;
					if (e.X < 0) XnotBeyond0width = 0;
					if (e.X > base.Width) XnotBeyond0width = base.Width;
					this.ValueMouseOverRangePercentage = this.XonGraphicsToPercentage(XnotBeyond0width);
				} else {
					this.ValueMouseOverRangePercentage = this.XonGraphicsToPercentage(e.X);
				}
				
				if (this.dragButtonPressed == false) {
					base.Invalidate();	// visuals first!!!
					this.RaiseOnValueMouseOverChanged();
					base.OnMouseMove(e);
					return;
				}
				if (this.dragging == false) {
					// first move after this.mousePressed means DRAG!! wasn't in UserControl due to ambiguity between OnMouseMove and OnMouseDrag, probably 
					this.dragStartedX = e.X;
					this.dragStartedPercentage = this.XonGraphicsToPercentage(e.X);
					this.dragging = true;
				}
				base.Invalidate();	// visuals first!!!
				base.OnMouseMove(e);
			} catch (Exception ex) {
				string msg = "WindProc won't catch your exceptions; keep a breakpoint here";
				throw ex;
			}
		}
	}
}