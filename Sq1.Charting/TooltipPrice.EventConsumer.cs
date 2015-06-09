using System.Drawing;
using System.Windows.Forms;

using Sq1.Core;

namespace Sq1.Charting {
	public partial class TooltipPrice {
		ChartControl chartControl;

		internal void Initialize(ChartControl chartControlPassed) {
			this.chartControl = chartControlPassed;
		}

		protected override void OnMouseMove(MouseEventArgs e) {
			//TRANSPARENT_MOUSEMOVE_FORWARDING__WHEN_ONMOUSEOVER_TOOLTIP_I_GET_MOUSELEAVE_HERE__FOLLOWING_INVALIDATE_WILL_HIDE
			base.OnMouseMove(e);
			//return;		//TURNED_OFF_KOZ__INVALIDATING_ALL_STILL_LETS_SOMETIME__MOUSE_OVER_TOOLTIP__IT_HAPPENS_TO_BE_INITIALIZED_WITH_NULL_CREISI

			if (this.chartControl == null) {
				string msg = "I_REFUSE_TRANSPARENT_MOUSEMOVE_FORWARDING__WHEN_ONMOUSEOVER_TOOLTIP_I_GET_MOUSELEAVE_HERE__FOLLOWING_INVALIDATE_WILL_HIDE";
				Assembler.PopupException(msg, null, false);
				return;
			}
			this.chartControl.InvalidateAllPanels();
		}
	}
}
