using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

using Sq1.Core;
using WeifenLuo.WinFormsUI.Docking;

namespace Sq1.Widgets {
	public class DockContentImproved : DockContent {
		// Docked Form with MinimumSize will take all MinimumSize area and overlap to neighbours
		// MinimumSize should be 0 to avoid overlap confusion and let myself resize to 0
		// using FloatWindowRecommendedSize as inDesigner-user-defined Size to set initial Floating Window size
		public Size FloatWindowRecommendedSize;
		
		public DockContentImproved() : base() {
		}
//		protected override void OnLoad(EventArgs e) {
//			if (base.ShowHint == DockState.Float) {
//				//base.Size = this.FloatWindowRecommendedSize;
//				this.Width = this.FloatWindowRecommendedSize.Width;
//				this.Height = this.FloatWindowRecommendedSize.Height;
//				base.FloatPane.ClientSize = this.FloatWindowRecommendedSize; 
//				Size a = base.FloatPane.PreferredSize; 
//				base.FloatPane.Size = this.FloatWindowRecommendedSize;
//			}
//		}
//		protected override void OnActivated(EventArgs e) {
//			if (base.ShowHint == DockState.Float) {
//				//base.Size = this.FloatWindowRecommendedSize;
//				this.Width = this.FloatWindowRecommendedSize.Width;
//				this.Height = this.FloatWindowRecommendedSize.Height;
//				base.FloatPane.ClientSize = this.FloatWindowRecommendedSize; 
//				Size a = base.FloatPane.PreferredSize; 
//				base.FloatPane.Size = this.FloatWindowRecommendedSize; 
//			}
//		}
//		public new void Show(DockPanel dp) {
//			if (base.ShowHint == DockState.Float) {
//				//base.Size = this.FloatWindowRecommendedSize;
//				this.Width = this.FloatWindowRecommendedSize.Width;
//				this.Height = this.FloatWindowRecommendedSize.Height;
//				//base.FloatPane.ClientSize = this.FloatWindowRecommendedSize; 
//				//Size a = base.FloatPane.PreferredSize; 
//				//base.FloatPane.Size = this.FloatWindowRecommendedSize; 
//			}
//			base.Show(dp);
//		}
		//http://msdn.microsoft.com/en-us/library/86faxx0d(v=vs.110).aspx
//			Application.ApplicationExit += new EventHandler(Application_ApplicationExit);
//		public bool ApplicationExitOccured = false;
//		void Application_ApplicationExit(object sender, EventArgs e) {
//			this.ApplicationExitOccured = true;
//		}
//		
//		public bool IsClosing = false;
//		protected override void OnClosing(System.ComponentModel.CancelEventArgs e) {
//			this.IsClosing = true;
//			base.OnClosing(e);
//		}		
//
//		// too late for ExceptionsControl.Splitter*.Distance to serialize
//		public bool IsFormClosing = false;
//		protected override void OnFormClosing(System.Windows.Forms.FormClosingEventArgs e) {
//			this.IsFormClosing = true;
//			base.OnFormClosing(e);
//		}
//		
//		public bool IsFormClosed = false;
//		protected override void OnFormClosed(System.Windows.Forms.FormClosedEventArgs e) {
//			this.IsFormClosed = true;
//			base.OnFormClosed(e);
//		}
		
		public void ShowAsDocumentTabNotPane(DockPanel dockPanel) {
			var docs = dockPanel.DocumentsToArray();
			if (docs.Length == 0) {
				this.Show(dockPanel, DockState.Document);
				return;
			}
			// add new tab, not a new pane besides existing one
			foreach (IDockContent doc in docs) {
				var hopefullyDockContent = doc as DockContent;
				if (hopefullyDockContent == null) continue; 
				this.Show(hopefullyDockContent.Pane, null);
				return;
			}
			this.Show(dockPanel, DockState.Document);
		}

		
		public bool IsFloatingWindow { get { return this.DockState == DockState.Float; } }
		public bool IsInDocumentArea { get { return this.DockState == DockState.Document; } }
		public bool IsDocked { get { return DockHelper.IsDockWindowState(this.DockState); } }
		public bool IsDockedAutoHide { get { return DockHelper.IsDockStateAutoHide(this.DockState); } }
		public bool IsCoveredOrAutoHidden { get {
				if (this.IsDockedAutoHide) return true;
				if (this.IsDocked) {
					string msg = "go find out if I'm covered by other forms docked into the same area"
						+ " ; meanwhile I'll report I'm not covered so you can click ChartForm>HIDESourceCodeEditor";
					#if DEBUG
					//Debugger.Break();
					#endif
					return false;
				}
				if (this.IsFloatingWindow) {
					string msg = "go find out if I'm covered by other forms floating in the same window"
						+ " ; meanwhile I'll report I'm not covered so you can click ChartForm>HIDESourceCodeEditor";
					#if DEBUG
					Debugger.Break();
					#endif
					return false;
				}
				if (this.DockState == DockState.Unknown) {
					string msg = "EDITOR_WAS_CONDITIONALLY_INSTANTIATED_BUT_NOT_DOCKPANEL.SHOW()n";
					#if DEBUG
					//Debugger.Break();
					#endif
					return true;
				}
				if (this.DockState == DockState.Hidden) {
					string msg = "DESERIALIZED_AS_HIDDEN__NOT_REALLY_DOCKED_NOR_COVERED";
					#if DEBUG
					Debugger.Break();
					#endif
					return true;
				}
				
				string msg1 = "WHERE_AM_I,THEN???";
				#if DEBUG
				Debugger.Break();
				#endif
				return true;
			} }

		// moved from modified WelfenLuoBlaBlaBla.DockHandler to restore release-state of DockContent library (not fully restored, though)
		public void ActivateDockContentPopupAutoHidden(bool keepAutoHidden = true, bool activate = true) {
			if (this.IsDocked) {
				if (activate) this.Activate();
				//if (keepAutoHidden) this.ToggleAutoHide();
			} else if (this.IsDockedAutoHide) {
				if (keepAutoHidden == false) {
					this.ToggleAutoHide();
				}
				if (activate) this.Activate();
			}
		}

		public void ToggleAutoHide() {
			if (this.DockState == DockState.Unknown) return;
			if (this.DockState == DockState.Document) return;
			if (this.DockState == DockState.Float) return;
			if (this.DockState == DockState.Hidden) return;
			DockState newState = DockHelper.ToggleAutoHideState(this.Pane.DockState);
			this.Pane.SetDockState(newState);
		}
		protected override void OnResize(EventArgs e) {
			if (Assembler.InstanceInitialized.MainFormDockFormsFullyDeserializedLayoutComplete == false) {
				// it looks like ChartForm doesn't propagate its DockContent-set size to ChartControl =>
				// for wider than in Designer ChartConrtrol sizes I see gray horizontal lines and SliderOutOfBoundaries Exceptions for smaller than in Designer
				// (Disable Resize during DockContent XML deserialization and fire manually for each ChartForm (Document only?) )
				return;
			}
			if (Assembler.InstanceInitialized.MainFormClosingIgnoreReLayoutDockedForms) {
				// it looks like ChartForm doesn't propagate its DockContent-set size to ChartControl =>
				// for wider than in Designer ChartConrtrol sizes I see gray horizontal lines and SliderOutOfBoundaries Exceptions for smaller than in Designer
				// (Disable Resize during DockContent XML deserialization and fire manually for each ChartForm (Document only?) )
				// I want to avoid agonizing sizes to (ever) appear in ChartFormDataSnapshot 
				return;
			}
			base.OnResize(e);
		}
		public bool NullOrDisposed { get { 
				return DockContentImproved.IsNullOrDisposed(this);
			}}
		public static bool IsNullOrDisposed(Form form) {
			if (form == null) return true;
			if (form.IsDisposed) return true;
			return false;	//!this.chartForm.IsHidden;
		}
	}
}
