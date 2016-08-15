using System;

using Sq1.Core;

namespace Sq1.Widgets.Exceptions {
	public partial class ExceptionsControl {
		void olvTreeExceptions_customize() {
			//v2	http://stackoverflow.com/questions/9802724/how-to-create-a-multicolumn-treeview-like-this-in-c-sharp-winforms-app/9802753#9802753
			this.olvTreeExceptions.CanExpandGetter = delegate(object o) {
				var ex = o as Exception;
				if (ex == null) return false;
				return ex.InnerException != null;
			};
			this.olvTreeExceptions.ChildrenGetter = delegate(object o) {
				var ex = o as Exception;
				if (ex == null) return null;
				return new Exception[] { ex.InnerException };
			};
			this.olvcTimestamp.AspectGetter = delegate(object o) {
				var ex = o as Exception;
				if (ex == null) return o.ToString();
				string timeOrName = "";
				if (this.exceptionTimes.ContainsKey(ex, this, "olvcTime.AspectGetter")) {
					DateTime exTime = this.exceptionTimes.GetAtKey_nullUnsafe(ex, this, "olvcTime.AspectGetter");
					timeOrName = exTime.ToString(Assembler.DateTimeFormat_toMillis);
				} else {
					timeOrName = ex.GetType().Name;
				}
				return timeOrName;
			};
			this.olvcException.AspectGetter = delegate(object o) {
				var ex = o as Exception;
				if (ex == null) return o.ToString();
				return ex.Message;
			};
		}
	}
}

			//this.olvcTime.AspectGetter = delegate(object o) {
			//	var ex = o as Exception;
			//	if (ex == null) return o.ToString();
			//	string messageOrNameOrTime = "";
			//	// this.DataSnapshot is null when ExceptionsForm is used outside Sq1.Gui / MainForm / Assembler lifecycle
			//	bool showTime = this.DataSnapshot != null ? this.DataSnapshot.TreeShowExceptionTime : true;
			//	if (showTime) {
			//		if (this.ExceptionTimes.ContainsKey(ex)) {
			//			DateTime exTime = this.ExceptionTimes[ex];
			//			messageOrNameOrTime = exTime.ToString(Assembler.DateTimeFormatLong);
			//		} else {
			//			messageOrNameOrTime = ex.GetType().Name;
			//		}
			//		messageOrNameOrTime += " | ";
			//	}
			//	messageOrNameOrTime += ex.Message;
			//	return messageOrNameOrTime;
			//};
