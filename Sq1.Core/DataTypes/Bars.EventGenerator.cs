using System;

namespace Sq1.Core.DataTypes {
	public partial class Bars {
		// IObservable emulated?...
		public event EventHandler<BarEventArgs> BarStaticAdded;
		public event EventHandler<BarEventArgs> BarStreamingAdded;
		public event EventHandler<BarEventArgs> BarStreamingUpdatedMerged;
		
		public void RaiseBarStaticAdded(Bar barAdding) {
			if (this.BarStaticAdded == null) return;
			try {
				this.BarStaticAdded(this, new BarEventArgs(barAdding));
			} catch (Exception ex) {
				string msg = "Bars.BarStaticAdded(bar[" + barAdding + "])";
				Assembler.PopupException(msg, ex, false);
			}
		}
		public void RaiseBarStreamingAdded(Bar barAdding) {
			if (this.BarStreamingAdded == null) return;
			try {
				this.BarStreamingAdded(this, new BarEventArgs(barAdding));
			} catch (Exception ex) {
				string msg = "Bars.BarStreamingAdded(bar[" + barAdding + "])";
				Assembler.PopupException(msg, ex, false);
			}
		}
		public void RaiseBarStreamingUpdated(Bar barUpdated) {
			if (this.BarStreamingUpdatedMerged == null) return;
			try {
				this.BarStreamingUpdatedMerged(this, new BarEventArgs(barUpdated));
			} catch (Exception ex) {
				string msg = "Bars.BarStreamingUpdated(bar[" + barUpdated + "])";
				Assembler.PopupException(msg, ex, false);
			}
		}
	}
}
