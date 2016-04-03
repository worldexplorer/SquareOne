using System;
using System.Windows.Forms;

using Sq1.Widgets;
using Sq1.Core;

namespace Sq1.Gui.Singletons {
	public class DockContentSingleton<T> : DockContentImproved where T : DockContentImproved, new() {
		#region v1 DockContentSingleton-derived forms had to implement Instance when DockContentSingleton wasn't generic
		//static ExceptionsForm instance = null;
		//public new static ExceptionsForm Instance {
		//	get {
		//		if (ExceptionsForm.instance == null) ExceptionsForm.instance = new ExceptionsForm();
		//		return ExceptionsForm.instance;
		//	}
		//}
		#endregion
		#region v2 moving Instance to the parent: FAILED without generics; 
		//protected static DockContentSingleton instance;
		//public static DockContentSingleton Instance {
		//	get {
				//DockContentSingleton parentInstance = DockContentSingleton.instance;
				//DataSourcesForm myInstance = parentInstance as DataSourcesForm;
				//if (myInstance == null) parentInstance = new DataSourcesForm();
				//return myInstance;
		//		throw new Exception("You forgot to define [public static override derived(DockContentSingleton) Instance]");
		//	}
		//}
		#endregion

		protected static bool instanceBeingConstructedUseForDesignMode { get; private set; }
		protected static T instance;
		public static T Instance { get {
				// without "where T : new()" in class declaration above, "new T()" below can not be compiled
				bool IamNotYetCreated = DockContentSingleton<T>.instance == null;
				bool IamDisposed = false;
				if (IamNotYetCreated == false) IamDisposed = DockContentSingleton<T>.instance.IsDisposed;
				if (IamNotYetCreated || IamDisposed) {
					if (IamNotYetCreated == false && IamDisposed) {
						//string ofWhatStatic = DockContentSingleton<T>.instance.GetType().GetGenericTypeDefinition().GetType().FullName;
						string ofWhatStatic = "OF_WHAT_THREW";
						string msig = " //DockContentSingleton<" + ofWhatStatic + ">[" + DockContentSingleton<T>.instance.Name + "].Instance";
						string msg = "IF_YOU_CLOSED_A_FORM__PLEASE_NULLIFY_THE_POINTER";
						Assembler.PopupException(msg + msig);
					}
					DockContentSingleton<T>.instanceBeingConstructedUseForDesignMode = true;
					DockContentSingleton<T>.instance = new T();
					DockContentSingleton<T>.instanceBeingConstructedUseForDesignMode = false;
				}
				return DockContentSingleton<T>.instance;
			} }
//		public string OfWhat { get {
//				string ret = "UNKNOWN";
//				var args = this.GetType().GetGenericArguments();
//				if (args.Length > 0) ret = args[0].Name;
//				return ret;
//			} }
		public string OfWhat { get { return typeof(T).Name; } }
		
		public DockContentSingleton() : base() {
			base.HideOnClose = true;
			if (DockContentSingleton<T>.instance == null) return;
			if (DockContentSingleton<T>.instanceBeingConstructedUseForDesignMode == true) return;
			string msg = "Don't invoke ctor(), use " + this.OfWhat + ".Instance instead" +
				"; " + this.OfWhat + " is a singleton, constructed ONLY_ONCE during the application's lifetime";
			throw new Exception(msg);
		}

//		protected DockPanel mainFormDockPanel;
//		protected IStatusReporter statusReporter;
//
//		public virtual void Initialize(IStatusReporter statusReporter, DockPanel mainFormDockPanel) {
//			this.mainFormDockPanel = mainFormDockPanel;
//			this.statusReporter = statusReporter;
//		}

		// 1) you specify HideOnClose=true  and do not override OnFormClosing(); OnClose: <Content IsHidden="True"> will	 be added into Layout.xml
		// 2) you specify HideOnClose=false and  you   override OnFormClosing(); OnClose: <Content IsHidden="True"> will NOT be added into Layout.xml
		// by HideOnClose=false and overriding with hide-cancelTrue-OnFormClosing-cancelFalse, I achieved:
		// onDockedMinimize - stay minimized after restart, onClose - removed from Layout.xml
		protected override void OnFormClosing(FormClosingEventArgs e) {
			//v1
//			base.Hide();
//			e.Cancel = true;
//			base.OnFormClosing(e);
//			e.Cancel = false;	// without it, <Content IsHidden="True"> will be added
			//v2 ctor() has base.HideOnClose = true; whatever we close we DONT close!!
			e.Cancel = false;	// without it, <Content IsHidden="True"> will be added
			return;
		}
		//protected override void OnFormClosing(FormClosingEventArgs e) {
		//	// let MainForm Receive the event
		//	base.OnFormClosing(e);
		//	base.Hide();
		//	e.Cancel = true;
		//}

//		public new bool Enabled {
//			get { return base.Enabled; }
//			set {
//				base.Enabled = value;
//				//this.dataSourceTreeView.Enabled = base.Enabled;
//			}
//		}

	}
}
