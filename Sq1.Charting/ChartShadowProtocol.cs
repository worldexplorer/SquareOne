using System.Diagnostics;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Core.Charting;
using Sq1.Core.Execution;

namespace Sq1.Charting {
	public partial class ChartControl {
		public void ActivateParentForm() {
			Form parent = this.Parent as Form;
			if (parent != null) {
				parent.Activate();
			} else {
				string msg = "Chart::ActivateParentForm() chart[" + this + "].Parent is not a Form, can not Activate()";
				Assembler.PopupException(msg);
			}
		}
		public override void SelectAlert(Alert alert) {
			this.ActivateParentForm();
			if (alert == null) {
				string msg = "DONT_PASS_ALERT=NULL_TO_CHART_SHADOW ChartControl.SelectAlert()";
				Assembler.PopupException(msg);
				#if DEBUG
				Debugger.Break();
				#endif
				return;
			}
			if (alert.PlacedBar == null) {
				string msg = "DONT_PASS_ALERT.PLACEDBAR=NULL_TO_CHART_SHADOW ChartControl.SelectAlert()";
				Assembler.PopupException(msg);
				#if DEBUG
				Debugger.Break();
				#endif
				return;
			}
			if (alert.PlacedBar.HasParentBars == false) {
				string msg = "DONT_PASS_ALERT.PLACEDBAR.HASPARENTBARS=FALSE_TO_CHART_SHADOW ChartControl.SelectAlert()";
				Assembler.PopupException(msg);
				#if DEBUG
				Debugger.Break();
				#endif
				return;
			}
			int bar = alert.PlacedBar.ParentBarsIndex;		
			this.scrollToBarSafely(bar);
		}		
		public override void SelectPosition(Position position) {
			this.ActivateParentForm();
			if (position == null) {
				string msg = "DONT_PASS_NULL_POSITION_TO_CHART_SHADOW ChartControl.SelectPosition()";
				Assembler.PopupException(msg);
				#if DEBUG
				Debugger.Break();
				#endif
				return;
			}
			int bar = (position.ExitAlert != null)
				? position.ExitAlert.PlacedBar.ParentBarsIndex
				: position.EntryAlert.PlacedBar.ParentBarsIndex;
			this.scrollToBarSafely(bar);
		}
	}
}