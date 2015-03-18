using System;
using System.Diagnostics;
using System.Windows.Forms;

using Sq1.Core;

namespace Sq1.Widgets.SteppingSlider {
	public partial class PanelFillSlider {
		
//	param:	[0...P..........500]units 
//	width:	[0...X..........250]px
//	ratio:	500/250 = 2 px/unit								//PixelsForOnePercentage
//	mouse:	X = 60px						
//	%%:		60px / 250px = 0.24; 0.24 * 100 = 24%			//OnMouseMove
//	%%:		60px * 2px/unit = 120units; 120units / 500units = 0.24 * 100 = 24%
//	filled:	24% / 100 = 0.24; 250px * 0.24 = 60px
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			if (e.X > base.Width) return;

			float range = (float) Math.Abs(this.ValueMax - this.ValueMin);
			decimal mouseRange;
			if (this.LeftToRight) {
				float partFilled = e.X / (float)base.Width;	//without (float) division of two ints is an int !!! (zero)
				if (partFilled > 1) {
					string msg = "PART_FILLED_MUST_BE_LESS_THAN_1=e.X[" + e.X + "]/base.Width[" + base.Width + "] MOUSE_MOVE_FROM_ANOTHER_CONTROL?";
					#if DEBUG
					Debugger.Break();
					#endif
					Assembler.PopupException(msg, null, false);
					return;
				}
				//this.ValueMouseOver = this.ValueMin + new decimal(partFilled * this.PixelsForOneValueUnit);
				this.ValueMouseOver = this.ValueMin + new decimal(partFilled * range);
				this.ValueMouseOver = this.RoundToClosestStep(this.ValueMouseOver);
				mouseRange = this.ValueMouseOver - this.ValueMin;
			} else {
				//float partFilled = (base.Width - e.X) / (float) base.Width;	//without (float) division of two ints is an int !!! (zero)
				float partFilled = (base.Width - e.X) / (float) base.Width;	//without (float) division of two ints is an int !!! (zero)
				//this.ValueMouseOver = this.ValueMax + new decimal(partFilled * this.PixelsForOneValueUnit);
				this.ValueMouseOver = this.ValueMax + new decimal(partFilled * range);
				this.ValueMouseOver = this.RoundToClosestStep(this.ValueMouseOver);
				mouseRange = this.ValueMouseOver - this.ValueMax;
			}
			
			//COMMENTED_TO_FIX_BEGIN crash on slider change while "Parameter Bags" CTX is open
			//if (leftMouseButtonHeldDown) {	// I_HATE_HACKING_F_WINDOWS_FORMS
			//	string msg = "DRAG_SIMULATION_AND_ON_DRAG_OVER_BOTH_DONT_WORK IF_YOU_SEE_THIS_SEND_A_SCREENSHOT_TO_DEVELOPER";
			//	//Debugger.Break();
			//	Assembler.PopupException(msg);
			//	if (this.ValueCurrent != this.ValueMouseOver) {
			//		//Debugger.Break();
			//		this.ValueCurrent = this.ValueMouseOver;
			//	}
			//}
			//COMMENTED_TO_FIX_END
			
			this.FilledPercentageMouseOver = 100 * ((float)mouseRange / range);
			base.Invalidate();
		}
		protected override void OnDragOver(DragEventArgs e) {
			// I_HATE_HACKING_F_WINDOWS_FORMS
			base.OnDragOver(e);
			this.OnMouseMove(new MouseEventArgs(MouseButtons.Left, 1, e.X, e.Y, 0));
			if (this.ValueCurrent != this.ValueMouseOver) {
				string msg = "this.ValueCurrent[" + this.ValueCurrent + "] != this.ValueMouseOver[" + this.ValueMouseOver + "]";
				Assembler.PopupException(msg);
				this.ValueCurrent = this.ValueMouseOver;
			}
		}
		
		bool mouseOver = false;
		protected override void OnMouseEnter(EventArgs e) {
			base.OnMouseEnter(e);
			this.mouseOver = true;
			base.Invalidate();
		}		
		protected override void OnMouseLeave(EventArgs e) {
			base.OnMouseLeave(e);
			if (rightClickShouldKeepMouseOver == true) {
				rightClickShouldKeepMouseOver = false;
				return;
			}
			this.mouseOver = false;
			this.leftMouseButtonHeldDown = false; 
			base.Invalidate();
		}
		bool leftMouseButtonHeldDown = false;
		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			if (e.Button == MouseButtons.Left) {
				leftMouseButtonHeldDown = true;	// I_HATE_HACKING_F_WINDOWS_FORMS
			}
			if (this.ValueCurrent != this.ValueMouseOver && leftMouseButtonHeldDown) {
				this.ValueCurrent = this.ValueMouseOver;
			}
		}
		bool rightClickShouldKeepMouseOver = false;
		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			if (e.Button == MouseButtons.Left) {
				leftMouseButtonHeldDown = false; 
			}
			if (e.Button != MouseButtons.Left) {
				rightClickShouldKeepMouseOver = true;
				return;
			}
			//this.FilledPercentageCurrentValue = this.FilledPercentageMouseOver;
			// we Raise on MouseDown and MouseDrag now
//			if (this.ValueCurrent != this.ValueMouseOver && this.mouseOver) {
//				this.ValueCurrent = this.ValueMouseOver;	// we Raise on MouseDown and MouseDrag now
//			}
		}

	}
}