using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Core.Charting;

namespace Sq1.Charting.MultiSplit {
	//LAZY_TO_MAKE_IT_ACCEPT_NESTED_PANELS_DROPPED_FOR_DESIGNER THIS_SOLVES_ANOTHER_PROBLEM http://stackoverflow.com/questions/2785376/how-to-enable-design-support-in-a-custom-control/2863807#2863807
	public partial class MultiSplitContainerGeneric<PANEL_BASE> {
		public	int		SplitterHeight;
		public	int		GrabHandleWidth;
		public	int		MinimumPanelHeight;
		public	Color	ColorGrabHandle;
		public	Color	ColorBackgroundSliderDroppingTarget;
		public	Color	ColorBackgroundSliderRegular;
		public	Color	ColorBackgroundSliderMouseOver;
		public	bool	DebugSplitter;
		public	bool	VerticalizeAllLogic;	// ideally I'd split to MultiSplitHorizontal and MultiSplitVertical; but I might offer the orientation swap in runtime
		
				// not observing (no events attached to add/move/remove)
				// didn't find a better implementation for swap() or .Move() - that's all I needed
				ObservableCollection<PANEL_BASE>	panels;
				ObservableCollection<MultiSplitter> splitters;
		public	List<Control>	ControlsContained { get {
			List<Control> ret = new List<Control>();
			foreach (PANEL_BASE inner in this.panels) {
				Control panelOrMultiSplitContainer = inner as Control;
				if (panelOrMultiSplitContainer == null) {
					string msg = "MUST_BE_CONTROLS_ONLY inner=[" + inner + "] AM_I_Controls.Items_CLONE?";
					Assembler.PopupException(msg);
				}
				ret.Add(panelOrMultiSplitContainer);
			}
			return ret;
		} }

		
		public MultiSplitContainerGeneric() : this(false, false) {
			panels = new ObservableCollection<PANEL_BASE>();
			splitters = new ObservableCollection<MultiSplitter>();
			SplitterHeight = 5;
			GrabHandleWidth = 30;
			MinimumPanelHeight = 5;
			ColorGrabHandle = Color.Orange;
			ColorBackgroundSliderDroppingTarget = Color.Red;
			ColorBackgroundSliderRegular = Color.DarkGray;
			ColorBackgroundSliderMouseOver = Color.SlateGray;
		}
		public MultiSplitContainerGeneric(bool verticalizeAllLogic = false, bool debugSplitter = false) {
			VerticalizeAllLogic = verticalizeAllLogic;
			DebugSplitter = debugSplitter;
		}
//		[Obsolete("pass panels implicitly to InitializeCreateSplittersDistributeFor() since you may have other controls in base.Controls, such as buttons with fixed position etc")]
//		public void InitializeExtractPanelsFromBaseControlsCreateSplittersDistribute() {
//			var list = new List<PANEL_BASE>();
//			foreach (PANEL_BASE c in base.Controls) {
//				c.Text = c.Name;
//				list.Add(c);
//			}
//			this.InitializeCreateSplittersDistributeFor(list);
//		}
		public void InitializeCreateSplittersDistributeFor(List<PANEL_BASE> whatIadd) {
			// DOENST_HELP_OPENING_IN_DESIGNER STILL_THROWS if (base.DesignMode) return;
			//Debugger.Break();
			this.panels.Clear();
			this.splitters.Clear();
			foreach (PANEL_BASE c in whatIadd) {
				PANEL_BASE panel = c as PANEL_BASE;
				if (panel == null) continue;
				try {
					this.PanelAddSplitterCreateAdd(panel, false);
				} catch (Exception ex) {
					string msg = "DONT_PanelAddSplitterCreateAdd(" + panel + ", false)";
					Assembler.PopupException(msg, ex);
				}
			}
			this.DistributePanelsAndSplitters();
		}
		public void DistributePanelsAndSplitters() {		//Dictionary<int, int> splitterPositionsByManorder = null) {
			if (this.VerticalizeAllLogic == false) {
				try {
					this.DistributePanelsAndSplittersVertically();
				} catch (Exception ex) {
					Assembler.PopupException("//DistributePanelsAndSplittersVertically()", ex);
				}
			} else {
				try {
					this.DistributePanelsAndSplittersHorizontally();
				} catch (Exception ex) {
					Assembler.PopupException("//DistributePanelsAndSplittersHorizontally()", ex);
				}
			}
		}
		public void DistributePanelsAndSplittersVertically() {		//Dictionary<int, int> splitterPositionsByManorder = null) {
			if (this.DesignMode) return;
			int baseHeight = base.Height;
			//baseHeight -= 4;	// LOWER_PANEL_GETS_CUT_BY_HSCROLLBAR diagnose by swapping with upper panel
			
			// DO_I_NEED_IT? base.SuspendLayout();
			
			// FIRST_LOOP_DISTRIBUTES_VERTICALLY_KEEP_ORIGINAL_PANELS_HEIGHTS
			int y = 0;
			for (int i=0; i<this.panels.Count; i++) {
				if (i >= this.splitters.Count) {
					string msg = "YOU_GOT_MORE_PANELS_(DESERIALIZED)_THAN_SPLITTERS MUST_BE_EQUAL";
					Assembler.PopupException(msg);
					break;
				}
				
				PANEL_BASE panel = this.panels[i];
				MultiSplitter splitter = this.splitters[i];

				//splitter.Location = new Point(splitter.Location.X, y);
				splitter.Location = new Point(0, y);
				y += splitter.Height;
				
				// SplitterMovingEnded will need to know splitter.PanelBelow.Location to save it
				//v1
				//PanelBase panelNamedFolding = panel as PanelBase;	// back from generics to real world
				//if (panelNamedFolding != null) {
				//	splitter.PanelBelow = panelNamedFolding;
				//	if (i > 0) {
				//		PANEL_BASE prevPanel = this.panels[i - 1];
				//		PanelBase prevPanelNamedFolding = prevPanel as PanelBase;	// back from generics to real world
				//		if (panelNamedFolding != null) splitter.PanelAbove = prevPanelNamedFolding;
				//	}
				//}
				//v2 it could be another MultiSplitContainer and I want its name for de/serialization
				splitter.PanelBelow = panel as Control;				// back to real world from generics
				if (i > 0) splitter.PanelAbove = this.panels[i - 1] as Control;

				//panel.Location = new Point(panel.Location.X, y);
				panel.Location = new Point(0, y);
				y += panel.Height;
			}
			
			if (Assembler.InstanceInitialized.MainFormDockFormsFullyDeserializedLayoutComplete == false) return;
			
			int deserializationError = Math.Abs(y - baseHeight);
			if (deserializationError <= 3) {
				string msg = "we don't need proportional vertical fill when 1) splitterMoved, 2) splitterDragged"
					+ " , 3) splitterPositionsByManorder.Count==this.panels.Count";
				//Assembler.PopupException(msg);
				return;
			}
			
			int panelHeight = baseHeight - this.MinimumPanelHeight;
			if (panelHeight < 0) {
				string msg = "I_SHOULD_NEVER_BE_HERE__WTF";
				Assembler.PopupException(msg);
				return;
			}

			// we need proportional vertical fill when 1) a new panel was added, 2) an old panel was removed, 3) Initialize(List<Panel>), 4) OnResize
			// SECOND_LOOP_RESIZES_EACH_HEIGHT_PROPORTIONALLY_TO_FILL_WHOLE_CONTAINER_SURFACE_VERTICALLY
			int totalFixedHeight = this.SplitterHeight * this.panels.Count;
			double fillVerticalK = (double) (baseHeight - totalFixedHeight) / (double) (y - totalFixedHeight);
			
			y = 0;
			for (int i=0; i<this.panels.Count; i++) {
				PANEL_BASE panel = this.panels[i];
				MultiSplitter splitter = this.splitters[i];
				
				int minimumPanelHeight = this.MinimumPanelHeight;
				var panelControl = panel as Control;
				Size defaultSize = default(Size);
				if (panelControl != null && panelControl.MinimumSize != null && panelControl.MinimumSize != defaultSize) {
					minimumPanelHeight = panelControl.MinimumSize.Height;
				}

				if (i == this.panels.Count - 1) {
					int lastSplitterMaxY = baseHeight - (splitter.Height + minimumPanelHeight);
					if (y > lastSplitterMaxY) {
						y = lastSplitterMaxY; 
					}
				}

				splitter.Location = new Point(0, y);
				y += splitter.Height;
				
				panel.Location = new Point(0, y);
				panel.Height = (int)(Math.Round(panel.Height * fillVerticalK, 0));
				if (i == this.panels.Count - 1) {
					if (panel.Height < minimumPanelHeight) {
						panel.Height = minimumPanelHeight;
					}

					if (y + panel.Height > baseHeight) {
						panel.Height = baseHeight - y;
						if (panel.Height < 0) {
							string msg = "STILL_RESIZING_IN_GUI_THREAD???  panel.Height[" + panel.Height + "] < 0";
							Assembler.PopupException(msg);
							return;
						}
					}
				}
				y += panel.Height;
			}
			
			#if DEBUG		// TESTS_EMBEDDED
			if (base.Height != baseHeight) {
				Debugger.Break();
			}
			baseHeight = base.Height;
			int roundingError = Math.Abs(y - baseHeight);
			if (roundingError > 1) {
				Debugger.Break();	// LOWER_PANEL_GETS_CUT_BY_HSCROLLBAR
			}
			#endif
			
			// DO_I_NEED_IT? base.ResumeLayout();
			//base.Invalidate();
		}
		public void DistributePanelsAndSplittersHorizontally() {		//Dictionary<int, int> splitterPositionsByManorder = null) {
			if (this.DesignMode) return;
			int baseWidth = base.Width;
			//baseHeight -= 4;	// LOWER_PANEL_GETS_CUT_BY_HSCROLLBAR diagnose by swapping with upper panel
			
			// DO_I_NEED_IT? base.SuspendLayout();
			
			// FIRST_LOOP_DISTRIBUTES_VERTICALLY_KEEP_ORIGINAL_PANELS_HEIGHTS
			int x = 0;
			for (int i=0; i<this.panels.Count; i++) {
				if (i >= this.splitters.Count) {
					string msg = "YOU_GOT_MORE_PANELS_(DESERIALIZED)_THAN_SPLITTERS MUST_BE_EQUAL";
					Assembler.PopupException(msg);
					break;
				}
				
				PANEL_BASE panel = this.panels[i];
				MultiSplitter splitter = this.splitters[i];

				//splitter.Location = new Point(splitter.Location.X, y);
				splitter.Location = new Point(x, 0);
				x += splitter.Width;
				
				// SplitterMovingEnded will need to know splitter.PanelBelow.Location to save it
				//v1
				//PanelBase panelNamedFolding = panel as PanelBase;	// back from generics to real world
				//if (panelNamedFolding != null) {
				//	splitter.PanelBelow = panelNamedFolding;
				//	if (i > 0) {
				//		PANEL_BASE prevPanel = this.panels[i - 1];
				//		PanelBase prevPanelNamedFolding = prevPanel as PanelBase;	// back from generics to real world
				//		if (panelNamedFolding != null) splitter.PanelAbove = prevPanelNamedFolding;
				//	}
				//}
				//v2 it could be another MultiSplitContainer and I want its name for de/serialization
				splitter.PanelBelow = panel as Control;				// back to real world from generics
				if (i > 0) splitter.PanelAbove = this.panels[i - 1] as Control;

				//panel.Location = new Point(panel.Location.X, y);
				panel.Location = new Point(x, 0);
				x += panel.Width;
			}
			
			if (Assembler.InstanceInitialized.MainFormDockFormsFullyDeserializedLayoutComplete == false) return;
			
			int deserializationError = Math.Abs(x - baseWidth);
			if (deserializationError <= 3) {
				string msg = "we don't need proportional horizontal fill when 1) splitterMoved, 2) splitterDragged"
					+ " , 3) splitterPositionsByManorder.Count==this.panels.Count";
				//Assembler.PopupException(msg, null, false);
				return;
			}
			
			int panelWidth = baseWidth - this.MinimumPanelHeight;
			if (panelWidth < 0) {
				string msg = "I_SHOULD_NEVER_BE_HERE__WTF";
				Assembler.PopupException(msg);
				return;
			}

			// we need proportional vertical fill when 1) a new panel was added, 2) an old panel was removed, 3) Initialize(List<Panel>), 4) OnResize
			// SECOND_LOOP_RESIZES_EACH_HEIGHT_PROPORTIONALLY_TO_FILL_WHOLE_CONTAINER_SURFACE_VERTICALLY
			int totalFixedWidth = this.SplitterHeight * this.panels.Count;
			double fillVerticalK = (double) (baseWidth - totalFixedWidth) / (double) (x - totalFixedWidth);
			
			x = 0;
			for (int i=0; i<this.panels.Count; i++) {
				PANEL_BASE panel = this.panels[i];
				MultiSplitter splitter = this.splitters[i];
				
				int minimumPanelWidth = this.MinimumPanelHeight;
				var panelControl = panel as Control;
				Size defaultSize = default(Size);
				if (panelControl != null && panelControl.MinimumSize != null && panelControl.MinimumSize != defaultSize) {
					minimumPanelWidth = panelControl.MinimumSize.Width;
				}

				if (i == this.panels.Count - 1) {
					int lastSplitterMaxY = baseWidth - (splitter.Width + minimumPanelWidth);
					if (x > lastSplitterMaxY) {
						x = lastSplitterMaxY; 
					}
				}

				splitter.Location = new Point(x, 0);
				x += splitter.Width;
				
				panel.Location = new Point(x, 0);
				panel.Width = (int)(Math.Round(panel.Width * fillVerticalK, 0));
				if (i == this.panels.Count - 1) {
					if (panel.Width < minimumPanelWidth) {
						panel.Width = minimumPanelWidth;
					}

					if (x + panel.Width > baseWidth) {
						panel.Width = baseWidth - x;
						if (panel.Height < 0) {
							string msg = "STILL_RESIZING_IN_GUI_THREAD???  panel.Width[" + panel.Width + "] < 0";
							Assembler.PopupException(msg);
							return;
						}
					}
				}
				x += panel.Width;
			}
			
			#if DEBUG		// TESTS_EMBEDDED
			if (base.Width != baseWidth) {
				Debugger.Break();
			}
			baseWidth = base.Width;
			int roundingError = Math.Abs(x - baseWidth);
			if (roundingError > 1) {
				Debugger.Break();	// LOWER_PANEL_GETS_CUT_BY_HSCROLLBAR
			}
			#endif
			
			// DO_I_NEED_IT? base.ResumeLayout();
			//base.Invalidate();
		}
		
		public void PanelAddSplitterCreateAdd(PANEL_BASE panel, bool redistributeAfterAddingOneNotManyResistributeManual = true) {
//											  , Dictionary<string, MultiSplitterProperties> splitterPositionsByManorder = null) {
//			panel.Capture = true;	// NO_YOU_WONT will I have MouseEnter during dragging the splitter? I_HATE_HACKING_WINDOWS_FORMS
//		   	panel.MouseEnter += new EventHandler(panel_MouseEnter);
//		   	panel.MouseLeave += new EventHandler(panel_MouseLeave);
			//if (base.Controls.Contains(panel)) base.Controls.Remove(panel);
			if (this.VerticalizeAllLogic == false) {
				panel.Width = base.Width;
			} else {
				panel.Height = base.Height;
			}

			//v1
			//if (panel.Parent != null && panel.Parent is Control) {
			//    Control parentControl = panel.Parent as Control;
			//    if (parentControl.Controls.Contains(panel)) parentControl.Controls.Remove(panel);
			//}
			//v2
			if (panel == this) {
				string msg = "CHECK_CAREFULLY_YOU_ARE_ADDING_ME_TO_MY_OWN_CONTROLS__BEEN_THERE__WINFORMS_EXCEPTION_WILL_FOLLOW_NEXT_LINE";
				Assembler.PopupException(msg);
			}
			base.Controls.Add(panel);
			#if DEBUG
			if (base.Parent.Controls.Contains(panel)) {
				string msg = "I expect panels be removed from miltisplitContainer.Parent"
					+ " so that only miltisplitContainer.Size will trigger miltisplitContainer.Controls.*.Resize()";
				Debugger.Break();
			}
			#endif
			this.panels.Add(panel);
			this.splitterCreateAdd(panel);
			if (redistributeAfterAddingOneNotManyResistributeManual == false) return;
			this.DistributePanelsAndSplitters();
		}
		public void PanelRemove(PANEL_BASE panel) {
			int panelIndex = this.panels.IndexOf(panel);
			if (panelIndex == 0) return; 
			MultiSplitter splitter = this.splitters[panelIndex];
			this.panels.Remove(panel);
			this.splitters.Remove(splitter);
			base.Controls.Remove(panel);
			base.Controls.Remove(splitter);
			this.DistributePanelsAndSplitters();
		}
		void splitterCreateAdd(PANEL_BASE panel) {
			MultiSplitter splitter = new MultiSplitter(GrabHandleWidth, ColorGrabHandle, this.VerticalizeAllLogic, false);
			if (this.VerticalizeAllLogic == false) {
				splitter.Width = base.Width;
				splitter.Height = this.SplitterHeight;
			} else {
				splitter.Height = base.Height;
				splitter.Width = this.SplitterHeight;
			}
			//ROLLEDBACK panel.Height -= SplitterHeight;	// LOWER_PANEL_GETS_CUT_BY_HSCROLLBAR FIX
			splitter.MouseMove	+= new MouseEventHandler(	splitter_MouseMove);
			splitter.MouseEnter += new EventHandler(		splitter_MouseEnter);
			splitter.MouseLeave += new EventHandler(		splitter_MouseLeave);
			splitter.MouseUp	+= new MouseEventHandler(	splitter_MouseUp);
			splitter.MouseDown	+= new MouseEventHandler(	splitter_MouseDown);
			this.splitters.Add(splitter);
			base.Controls.Add(splitter);	//make splitter receive OnPaint()
		}
		public override string ToString() {
			string ret = "NO_PARENT_INFO";
			SplitterPanel splitterBarRangeEtMoi = this.Parent as SplitterPanel;
			if (splitterBarRangeEtMoi != null) {
				SplitContainer splitContainer = splitterBarRangeEtMoi.Parent as SplitContainer;
				if (splitContainer != null) {
					ChartControl chartControl = splitContainer.Parent as ChartControl;
					if (chartControl != null) {
						ret = chartControl.ToString();
					}
				}
			} else {
				if (this.Parent != null) ret = "Parent[" + this.Parent.ToString() + "]";
			}
			return ret;
		}
	}
}
