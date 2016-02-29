using System;

using Sq1.Core;
using Sq1.Core.DataFeed;
using Sq1.Core.DataTypes;

namespace Sq1.Gui.Forms {
	public partial class ChartForm {
		public event EventHandler<EventArgs>					OnStreamingButtonStateChanged;
		public event EventHandler<DataSourceSymbolEventArgs>	OnBarsEditorClicked;
		
		void raiseOnStreamingButtonStateChanged() {
			if (this.OnStreamingButtonStateChanged == null) return;
			string msg = "IF_IM_NOT_POPPED_UP_THEN_THERE_IS_NO_CONSUMERS_ANYMORE_IT_WAS_INTERNAL_LOOP_FROM_CHARTFROM_TO_CHARTFORM DELETEME_AFTER_JAN_2015";
			Assembler.PopupException(msg);
			this.OnStreamingButtonStateChanged(this, null);
		}
		
		void raiseOnBarsEditorClicked() {
			if (this.OnBarsEditorClicked == null) return;

			Bars bars = this.ChartControl.Bars;
			if (bars == null) {
				string msg = "MUST_NOT_BE_NULL_this.ChartControl.Bars //ChartForm.raiseOnBarsEditorClicked()";
				Assembler.PopupException(msg);
				return;
			}
			this.OnBarsEditorClicked(this, new DataSourceSymbolEventArgs(bars.DataSource, bars.Symbol));
		}
	}
}
