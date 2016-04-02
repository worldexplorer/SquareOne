using System;

namespace Sq1.Core.DataTypes {
	public partial class Bars {
		#region OBSOLETE_NOW__USE_STREAMING_CONSUMERS_INSTEAD
		//// IObservable emulated?...
		//public event EventHandler<BarEventArgs> OnBarStaticAdded;
		//public event EventHandler<BarEventArgs> OnBarStreamingAdded;
		//public event EventHandler<BarEventArgs> OnBarStreamingUpdatedMerged;
		
		//void raiseOnBarStaticAdded(Bar barAdding) {
		//    if (this.OnBarStaticAdded == null) return;
		//    try {
		//        this.OnBarStaticAdded(this, new BarEventArgs(barAdding));
		//    } catch (Exception ex) {
		//        string msg = "Bars.BarStaticAdded(bar[" + barAdding + "])";
		//        Assembler.PopupException(msg, ex, false);
		//    }
		//}
		//void raiseOnBarStreamingAdded(Bar barAdding) {
		//    if (this.OnBarStreamingAdded == null) return;
		//    try {
		//        this.OnBarStreamingAdded(this, new BarEventArgs(barAdding));
		//    } catch (Exception ex) {
		//        string msg = "Bars.BarStreamingAdded(bar[" + barAdding + "])";
		//        Assembler.PopupException(msg, ex, false);
		//    }
		//}
		//void raiseOnBarStreamingUpdated(Bar barUpdated) {
		//    if (this.OnBarStreamingUpdatedMerged == null) return;
		//    try {
		//        this.OnBarStreamingUpdatedMerged(this, new BarEventArgs(barUpdated));
		//    } catch (Exception ex) {
		//        string msg = "Bars.BarStreamingUpdated(bar[" + barUpdated + "])";
		//        Assembler.PopupException(msg, ex, false);
		//    }
		//}
		#endregion
	}
}
