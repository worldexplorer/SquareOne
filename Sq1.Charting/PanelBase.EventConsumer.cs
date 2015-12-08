using System;
using Sq1.Core;

namespace Sq1.Charting {
	public partial class PanelBase {
		protected override void OnResize(EventArgs e) {	//PanelDoubleBuffered does this already to DisposeAndNullify managed Graphics
#if NON_DOUBLE_BUFFERED	//SAFE_TO_UNCOMMENT_COMMENTED_OUT_TO_MAKE_C#DEVELOPER_EXTRACT_METHOD
			if (base.DesignMode) return;
			base.OnResize(e);	// empty inside but who knows what useful job it does?
			base.Invalidate();	// SplitterMoved => repaint; Panel and UserControl don't have that (why?)
#else
			string msg = "WHO_INVOKES_ME? SetHeightIgnoreResize()";
			if (Assembler.InstanceInitialized.MainFormDockFormsFullyDeserializedLayoutComplete == false) return;
			if (this.ignoreResizeImSettingWidthOrHeight) return;
			if (this.DisplayRectangle.Width != base.ClientRectangle.Width) {
				int a = 0;
			}
			// this.SetWidthIgnoreResize(base.ClientSize.Width - 8);
			//this.SetHeightIgnoreResize(base.ClientSize.Height - 8);

			//BOTH_WORK_PERFECT!!! VERTICAL_NON_FULLY_REPAINT_EXTENDED_LEFT_TO_SOLVE NO_EFFECT
			base.OnResize(e);
			//BOTH_WORK_PERFECT!!! VERTICAL_NON_FULLY_REPAINT_EXTENDED_SOLVED_AS_WELL GOES_VERY_BAD
			base.Invalidate();	// VERTICAL_NON_FULLY_REPAINT_EXTENDED_LEFT_TO_SOLVE
#endif
		}
		protected override void OnLayout(System.Windows.Forms.LayoutEventArgs levent) {
			 base.OnLayout(levent);
		}
		protected override void SetClientSizeCore(int x, int y) {
			base.SetClientSizeCore(x, y);
		}
		protected override void OnClientSizeChanged(EventArgs e) {
			base.OnClientSizeChanged(e);
		}
	}
}