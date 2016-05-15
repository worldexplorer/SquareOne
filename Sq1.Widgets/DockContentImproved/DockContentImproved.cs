using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

using WeifenLuo.WinFormsUI.Docking;
using Sq1.Core;
using System.Collections.Generic;

namespace Sq1.Widgets {
	public class DockContentImproved : DockContent {
		// Docked Form with MinimumSize will take all MinimumSize area and overlap to neighbours
		// MinimumSize should be 0 to avoid overlap confusion and let myself resize to 0
		// using FloatWindowRecommendedSize as inDesigner-user-defined Size to set initial Floating Window size
		public	Size		FloatWindowRecommendedSize;
		
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

		//public	DockPanel	DockPanel { get; private set; }
		//public new void Show(DockPanel dockPanelPassed) {
		//	this.DockPanel = dockPanelPassed;
		//	base.Show(this.DockPanel);
		//}

		public void ShowAsDocumentTabNotPane(DockPanel dockPanel) {
			//v1 NOT_MOVING_TO_DOCUMENT_ON_RESTART
			//if (this.IsShown) {
			//	//this.ActivateDockContentPopupAutoHidden(false, true);
			//	return;
			//}
			if (this.IsShown && this.IsInDocumentArea) {
				this.Activate();
				return;
			}

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


		Dictionary <DockState, List<DockPane>> panesByDockState(DockPanel dockPanel) {
			Dictionary <DockState, List<DockPane>> panesByDockState = new Dictionary<DockState, List<DockPane>>();
			foreach (DockPane eachPane in dockPanel.Panes) {
				if (panesByDockState.ContainsKey(eachPane.DockState) == false) {
					panesByDockState.Add(eachPane.DockState, new List<DockPane>());
				}
				List<DockPane> panesForMyDockState = panesByDockState[eachPane.DockState];
				panesForMyDockState.Add(eachPane);
			}
			return panesByDockState;
		}
		public void ShowStackedHinted(DockPanel dockPanel) {
			DockPane targetPane_firstContent = null;

			Dictionary <DockState, List<DockPane>> panesByDockState = this.panesByDockState(dockPanel);
			if (panesByDockState.ContainsKey(base.ShowHint) == false) {
				base.Show(dockPanel);
				return;
			}
			List<DockPane> panesForMyDockState = panesByDockState[base.ShowHint];
			targetPane_firstContent = panesForMyDockState[0];

			base.Show(dockPanel);

			int tabToTheLeft = 0;
			int tabToTheRight = -1;
			base.DockTo(targetPane_firstContent, DockStyle.Fill, tabToTheLeft);
	
			base.Activate();
		}
		public void ShowOnTopOf(DockContent existingFormThatIWillCover) {
			//existingFormThatIWillCover.Show(this);
			//base.Show(this.DockHandler.Pane, existingFormThatIWillCover);
			if (existingFormThatIWillCover.DockPanel == null) return;

			base.Show(existingFormThatIWillCover.DockPanel, existingFormThatIWillCover.DockState);
			try {
				//base.DockTo(existingFormThatIWillCover.DockPanel.Panes[0], DockStyle.Fill, 0);
				//var notParent1 = base.Pane;
				//var notParent2 = base.PanelPane;
				//var notParent3 = this.DockHandler.PreviousActive;
				DockPane parent = this.DockHandler.PreviousActive.DockHandler.Pane;
				//var dockedRight = base.DockPanel.Panes[4]; 
				//var whichPaneIsDockedRight = base.DockPanel;	// 14 panes "under" 14 forms open and docked everywhere
				int tabToTheLeft = 0;
				int tabToTheRight = -1;
				base.DockTo(parent, DockStyle.Fill, tabToTheLeft);		// my own page = pane of existingFormThatIWillCover, Fill = on top
			} catch (Exception ex) {
				string msg = "IS_IT_NULL? this.DockHandler.PreviousActive[" + this.DockHandler.PreviousActive + "]";
				Assembler.PopupException(msg);
			} finally {
				base.Activate();
			}
		}
		//public void ShowOnTopOfMe(DockContentImproved willCoverMe) {
		//    //base.Show(willCoverMe.DockHandler.Pane, this);
		//    willCoverMe.Show(this.DockPanel, this.DockState);
		//    willCoverMe.DockTo(base.DockPanel.Panes[0], DockStyle.Fill, 0);
		//    //willCoverMe.Activate();
		//}
		
		#region taken from Exceptions, TODO use variables from this class (replace first steps of explorations with DockContentImproved's methods)
		public void ShowPopupSwitchToGuiThreadRunDelegateInIt(Delegate runInGuiThread = null) {
			if (base.IsDisposed) return;
			if (base.InvokeRequired == true) {
				//base.BeginInvoke((MethodInvoker)delegate { this.ShowPopupSwitchToGuiThread(runInGuiThread); });
				//return;
				base.Invoke((MethodInvoker)delegate { this.ShowPopupSwitchToGuiThreadRunDelegateInIt(runInGuiThread); });
				return;
			}
			// doesn't help to show up an Auto-Hidden-after-creation THIS
			//base.Show();
			//base.BringToFront();

			//base.Pane=null after RestoreXmlLayout()
			if (base.Pane == null) return;
			if (base.Pane.IsAutoHide) {
				// why DockHelper.ToggleAutoHideState() and DockHandler.SetDockState() are internal???...
				DockState newState = DockHelper.ToggleAutoHideState(base.Pane.DockState);
				base.Pane.SetDockState(newState);
			}
			// added if(base.IsHidden) to stop Pane.Activate() steal focus during keyUp/keyDown in ExecutionTree when this generates Exceptions
			//if (base.IsHidden) base.Pane.Activate();
			if (base.Visible == false) {
				string msg = "I'm here when ExceptionsForm is covered by another docked pane";
				//DOESNT_BRING_ME_ON_TOP_OF_PANES_COVERING_ME base.Pane.Activate();
				this.Activate();	// NOW_ITS_A_POPUP_INDEED !!! YAHOO_WASNT_POPPING_UP_FOR_TWO_YEARS__SPENT_HOURS_TO_FIGURE_OUT
			}
			// removes focus from other forms; makes ExecutionForm.SelectedRow blue=>gray
			// base.Activate();

			if (runInGuiThread != null) {
				runInGuiThread.DynamicInvoke();
			}
		}
//		public override void VisibleChanged(object sender, EventArgs e) {
//			int a = 1;
//		}
		#endregion

		public bool IsShown				{ get { return base.Visible && base.DockState != DockState.Unknown; } }
		public bool IsFloatingWindow	{ get { return base.Visible && base.DockState == DockState.Float; } }
		public bool IsInDocumentArea	{ get { return base.Visible && base.DockState == DockState.Document; } }
		public bool IsDocked			{ get { return DockHelper.IsDockWindowState(base.DockState); } }
		public bool IsDockedAutoHide	{ get { return DockHelper.IsDockStateAutoHide(base.DockState); } }
		public bool IsCoveredOrAutoHidden { get {
				//if (base.Visible == false) return false;
				//if (this.IsDockedAutoHide) return base.IsHidden;	// returns false, like it's not hidden; while it IS autoHidden
				if (this.IsDockedAutoHide) return true;
				if (this.IsDocked) {
					string msg = "go find out if I'm covered by other forms docked into the same area"
						+ " ; meanwhile I'll report I'm not covered so you can click ChartForm>HIDESourceCodeEditor";
					//Assembler.PopupException(msg, null, false);
					//v1 return false;
					bool isCovered = base.Pane.ActiveContent != this;
					return isCovered;
				}
				if (this.IsFloatingWindow) {
					string msg = "go find out if I'm covered by other forms floating in the same window"
						+ " ; meanwhile I'll report I'm not covered so you can click ChartForm>HIDESourceCodeEditor";
					Assembler.PopupException(msg, null, false);
					return true;
				}
				if (base.DockState == DockState.Unknown) {
					string msg = "EDITOR_WAS_CONDITIONALLY_INSTANTIATED_BUT_NOT_DOCKPANEL.SHOW()n";
					Assembler.PopupException(msg, null, false);
					return true;
				}
				if (base.DockState == DockState.Hidden) {
					string msg = "DESERIALIZED_AS_HIDDEN__NOT_REALLY_DOCKED_NOR_COVERED";
					Assembler.PopupException(msg, null, false);
					return true;
				}
				
				if (base.Visible == false && this.IsDocked) {
					string msg = "OPEN_IN_ONE_OF_THE_TABS__BUT_IS_NOT_ACTIVE";
					return true;
				}
				
				string msg1 = "WHERE_AM_I,THEN???";
				Assembler.PopupException(msg1, null, false);
				return true;
			} }
		public bool MustBeActivated		{ get { return this.IsShown ? this.IsCoveredOrAutoHidden : false; } }

		// moved from modified WelfenLuoBlaBlaBla.DockHandler to restore release-state of DockContent library (not fully restored, though)
		public void ActivateDockContentPopupAutoHidden(bool keepAutoHidden = true, bool activate = true) {
			if (this.IsDocked) {
				//if (keepAutoHidden) this.ToggleAutoHide();
			} else if (this.IsDockedAutoHide || this.DockState == DockState.Hidden) {
				if (keepAutoHidden == false) {
					this.ToggleAutoHide();
				}
			}
			if (activate) this.Activate();
		}

		public void ToggleAutoHide() {
			if (this.DockState == DockState.Unknown)	return;
			if (this.DockState == DockState.Document)	return;
			if (this.DockState == DockState.Float)		return;
			if (this.DockState == DockState.Hidden)		return;
			//DockState newState = // BROKEN_CONTAINS_DUPLICATED_LOGIC_NONSENSE DockHelper.ToggleAutoHideState(this.Pane.DockState);
			DockState newState = this.ToggleAutoHideState(this.Pane.DockState);
			this.Pane.SetDockState(newState);
		}

		public DockState ToggleAutoHideState(DockState state) {
			// unminimize if minimized
			if (state ==	DockState.DockLeftAutoHide)
					return	DockState.DockLeft;
			if (state ==	DockState.DockRightAutoHide)
					return	DockState.DockRight;
			if (state ==	DockState.DockTopAutoHide)
					return	DockState.DockTop;
			if (state ==	DockState.DockBottomAutoHide)
					return	DockState.DockBottom;
			
			// minimize if not minimized
			if (state ==	DockState.DockLeft)
					return	DockState.DockLeftAutoHide;
			if (state ==	DockState.DockRight)
					return	DockState.DockRightAutoHide;
			if (state ==	DockState.DockTop)
					return	DockState.DockTopAutoHide;
			if (state ==	DockState.DockBottom)
					return	DockState.DockBottomAutoHide;
			
			// these are non-Minimize-able states DockState.Unknown,Document,Float,Hidden
			return state;
		}

		protected override void OnResize(EventArgs e) {
			if (base.DesignMode) {
				base.OnResize(e);
				return;
			}
			if (Assembler.InstanceInitialized.MainForm_dockFormsFullyDeserialized_layoutComplete == false) {
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
			#if DEBUG
			string thisName = this.GetType().Name;
			if (thisName == "ExceptionsForm" && base.Parent != null) {
				if (base.Width != base.Parent.Width) {
					string msg = "PLACED_EXCEPTION_TEXTAREA_TO_HORIZONTAL_SPLITTER_TOP_PANEL reason for unsused space on the right before the form edge?";
				}
			}
			#endif
			base.OnResize(e);
		}
		public bool NullOrDisposed { get { return DockContentImproved.IsNullOrDisposed(this); } }
		public static bool IsNullOrDisposed(Form form) {
			if (form == null) return true;
			if (form.IsDisposed) return true;
			return false;	//!this.chartForm.IsHidden;
		}
	}
}
