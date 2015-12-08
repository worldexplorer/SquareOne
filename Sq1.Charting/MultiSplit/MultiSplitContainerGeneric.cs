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
		public		int		SplitterHeight;
		protected	int		GrabHandleWidth;
		protected	int		MinimumPanelHeight;
		protected	Color	ColorGrabHandle;
		protected	Color	ColorBackgroundSliderDroppingTarget;
		protected	Color	ColorBackgroundSliderRegular;
		protected	Color	ColorBackgroundSliderMouseOver;
		public		bool	DebugSplitter;
		public		bool	VerticalizeAllLogic;	// ideally I'd split to MultiSplitHorizontal and MultiSplitVertical; but I might offer the orientation swap in runtime
		
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

		MultiSplitContainerGeneric(bool verticalizeAllLogic = false, bool debugSplitter = false) {
			VerticalizeAllLogic = verticalizeAllLogic;
			DebugSplitter = debugSplitter;
			panels = new ObservableCollection<PANEL_BASE>();
			splitters = new ObservableCollection<MultiSplitter>();
			SplitterHeight = 25;
			GrabHandleWidth = 30;
			MinimumPanelHeight = 5;
			ColorGrabHandle = Color.Orange;
			ColorBackgroundSliderDroppingTarget = Color.Red;
			ColorBackgroundSliderRegular = Color.DarkGray;
			ColorBackgroundSliderMouseOver = Color.SlateGray;
		}
		public MultiSplitContainerGeneric() : this(false, false) {
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
			base.SuspendLayout();	// forces this.PanelAddSplitterCreateAdd not to invoke my.OnResize() <=PerformLayout() 
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
			this.AssignPanelBelowAbove_fromPanelsList();
			this.DistributePanelsAndSplitters();
			base.ResumeLayout(true);
		}
		public void AssignPanelBelowAbove_fromPanelsList() {
			for (int i=0; i<this.panels.Count; i++) {
				if (i >= this.splitters.Count) {
					string msg = "YOU_GOT_MORE_PANELS_(DESERIALIZED)_THAN_SPLITTERS MUST_BE_EQUAL";
					Assembler.PopupException(msg);
					break;
				}
				PANEL_BASE panel = this.panels[i];
				MultiSplitter splitter = this.splitters[i];

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
				if (i > 0) {
					splitter.PanelAbove = this.panels[i - 1] as Control;
				} else {
					splitter.PanelAbove = null;
				}
			}
		}
		public void DistributePanelsAndSplitters() {		//Dictionary<int, int> splitterPositionsByManorder = null) {
			if (Assembler.InstanceInitialized.MainFormDockFormsFullyDeserializedLayoutComplete == false) {
				string msg = "ONCE_PER_PANEL_LET_IT_DISTRIBUTE_COMMENTED WHAT_IF_YOU_FIND_ALL_AND_LINK_PANEL_BELOWs_HERE?...";
			//	return;
			}

			SortedDictionary<string, MultiSplitterProperties> splittersDistributed =
				this.VerticalizeAllLogic
					? distributeRows()
					: distributeColumns();
			this.SplitterPropertiesByPanelNameSet(splittersDistributed);

			//if (this.VerticalizeAllLogic == false) {
			//    try {
			//        this.distributePanelsAndSplittersHorizontally_setHeightAndY();
			//    } catch (Exception ex) {
			//        Assembler.PopupException("//distributePanelsAndSplittersHorizontally_setHeightAndY()", ex);
			//    }
			//} else {
			//    try {
			//        this.distributePanelsAndSplittersVertically_setWidthAndX();
			//    } catch (Exception ex) {
			//        Assembler.PopupException("//distributePanelsAndSplittersVertically_setWidthAndX()", ex);
			//    }
			//}
		}
		//void distributePanelsAndSplittersHorizontally_setHeightAndY() {		//Dictionary<int, int> splitterPositionsByManorder = null) {
		//    if (this.DesignMode) return;
		//    //if (Assembler.InstanceInitialized.MainFormDockFormsFullyDeserializedLayoutComplete == false) return;

		//    string whoAmI = this.ToString();
		//    string whoAmIatBase = base.ToString();
		//    if (whoAmI != "NO_PARENT_INFO for ChartControl") {
		//        string msg = "DID_I_GET_MAIN_FORM_INSTEAD_OF_CHART_FORM???";
		//    }
		//    if (this.Height != base.Height) {
		//        string msg = "EHM______";
		//    }
		//    int baseHeight = base.Height;
		//    //baseHeight -= 4;	// LOWER_PANEL_GETS_CUT_BY_HSCROLLBAR diagnose by swapping with upper panel
			
		//    // DO_I_NEED_IT? base.SuspendLayout();

		//    int thingsIchanged = 0;
		//    // FIRST_LOOP_DISTRIBUTES_VERTICALLY_KEEP_ORIGINAL_PANELS_HEIGHTS
		//    int y = 0;
		//    for (int i=0; i<this.panels.Count; i++) {
		//        PANEL_BASE panel = this.panels[i];
		//        MultiSplitter splitter = this.splitters[i];

		//        if (splitter.Location.Y != y) {
		//            //splitter.Location = new Point(splitter.Location.X, y);
		//            splitter.Location = new Point(0, y);
		//            thingsIchanged++;
		//        }
		//        y += splitter.Height;

		//        if (panel.Location.Y != y) {
		//            //panel.Location = new Point(panel.Location.X, y);
		//            panel.Location = new Point(0, y);
		//            thingsIchanged++;
		//        }
		//        y += panel.Height;
		//    }
			
		//    // MOVED_20_LINES_UP if (Assembler.InstanceInitialized.MainFormDockFormsFullyDeserializedLayoutComplete == false) return;
			
		//    int deserializationError = Math.Abs(y - baseHeight);
		//    if (deserializationError <= 3) {
		//        string msg = "we don't need proportional vertical fill when 1) splitterMoved, 2) splitterDragged"
		//            + " , 3) splitterPositionsByManorder.Count==this.panels.Count";
		//        //Assembler.PopupException(msg);
		//        return;
		//    }
			
		//    int panelHeight = baseHeight - this.MinimumPanelHeight;
		//    if (panelHeight < 0) {
		//        string msg = "I_SHOULD_NEVER_BE_HERE__WTF";
		//        Assembler.PopupException(msg, null, false);
		//        return;
		//    }

		//    // we need proportional vertical fill when 1) a new panel was added, 2) an old panel was removed, 3) Initialize(List<Panel>), 4) OnResize
		//    // SECOND_LOOP_RESIZES_EACH_HEIGHT_PROPORTIONALLY_TO_FILL_WHOLE_CONTAINER_SURFACE_VERTICALLY
		//    int totalFixedHeight = this.SplitterHeight * this.panels.Count;
		//    double fillVerticalK = (double) (baseHeight - totalFixedHeight) / (double) (y - totalFixedHeight);
			
		//    y = 0;
		//    for (int i=0; i<this.panels.Count; i++) {
		//        PANEL_BASE panel = this.panels[i];
		//        PanelBase panelBase = panel as PanelBase;
		//        if (panelBase == null) {
		//            string msg = "I_THOUGH_MULTISPLITTER_IS_ALSO_A_PANEL_BASE";
		//            Assembler.PopupException(msg);
		//        }
		//        MultiSplitter splitter = this.splitters[i];
				
		//        int minimumPanelHeight = this.MinimumPanelHeight;
		//        Control panelControl = panelBase as Control;
		//        Size defaultSize = default(Size);
		//        if (panelControl != null && panelControl.MinimumSize != null && panelControl.MinimumSize != defaultSize) {
		//            minimumPanelHeight = panelControl.MinimumSize.Height;
		//        }

		//        if (i == this.panels.Count - 1) {
		//            int lastSplitterMaxY = baseHeight - (splitter.Height + minimumPanelHeight);
		//            if (y > lastSplitterMaxY) {
		//                y = lastSplitterMaxY; 
		//            }
		//        }

		//        if (splitter.Location.Y != y) {
		//            splitter.Location = new Point(0, y);
		//            thingsIchanged++;
		//        }
		//        y += splitter.Height;
				
		//        if (panelBase.Location.Y != y) {
		//            panelBase.Location = new Point(0, y);
		//            thingsIchanged++;
		//        }

		//        int newHeight = (int)(Math.Round(panelBase.Height * fillVerticalK, 0));
		//        if (panelBase.Height != newHeight) {
		//            //panel.Height  = newHeight;
		//            panelBase.SetHeightIgnoreResize(newHeight);
		//            thingsIchanged++;
		//        }
		//        if (i == this.panels.Count - 1) {
		//            if (panelBase.Height < minimumPanelHeight) {
		//                //panel.Height = minimumPanelHeight;
		//                panelBase.SetHeightIgnoreResize(minimumPanelHeight);
		//            }

		//            if (y + panelBase.Height > baseHeight) {
		//                //panel.Height = baseHeight - y;
		//                panelBase.SetHeightIgnoreResize(baseHeight - y);
		//                thingsIchanged++;
		//                if (panelBase.Height < 0) {
		//                    string msg = "STILL_RESIZING_IN_GUI_THREAD???  panel.Height[" + panelBase.Height + "] < 0";
		//                    Assembler.PopupException(msg);
		//                    return;
		//                }
		//            }
		//        }
		//        y += panelBase.Height;
		//    }
			
		//    #if DEBUG		// TESTS_EMBEDDED
		//    //if (baseHeight != base.Height) {
		//    //    Debugger.Break();
		//    //    baseHeight  = base.Height;
		//    //}
		//    int roundingError = Math.Abs(y - baseHeight);
		//    if (roundingError > 1) {
		//        Debugger.Break();	// LOWER_PANEL_GETS_CUT_BY_HSCROLLBAR
		//    }
		//    #endif
			
		//    if (thingsIchanged == 0) {
		//        string msg = "I_WAS_USELESS__REMOVE_MY_INVOCATION_FROM_UPSTACK //distributePanelsAndSplittersHorizontally_setHeightAndY()";
		//        Assembler.PopupException(msg, null, false);
		//    }

		//    // DO_I_NEED_IT? base.ResumeLayout();
		//    //base.Invalidate();
		//}
		//void distributePanelsAndSplittersVertically_setWidthAndX() {		//Dictionary<int, int> splitterPositionsByManorder = null) {
		//    if (this.DesignMode) return;
		//    //if (Assembler.InstanceInitialized.MainFormDockFormsFullyDeserializedLayoutComplete == false) return;

		//    string whoAmI = this.ToString();
		//    string whoAmIatBase = base.ToString();
		//    if (whoAmI != "NO_PARENT_INFO for ChartControl") {
		//        string msg = "DID_I_GET_MAIN_FORM_INSTEAD_OF_CHART_FORM???";
		//    }
		//    if (base.ClientRectangle.Width != base.Width) {
		//        string msg = "EHM______";
		//    }
		//    int baseWidth = base.Width;
		//    //baseHeight -= 4;	// LOWER_PANEL_GETS_CUT_BY_HSCROLLBAR diagnose by swapping with upper panel
			
		//    // DO_I_NEED_IT? base.SuspendLayout();
		//    int thingsIchanged = 0;
			
		//    // FIRST_LOOP_DISTRIBUTES_VERTICALLY_KEEP_ORIGINAL_PANELS_HEIGHTS
		//    int x = 0;
		//    for (int i=0; i<this.panels.Count; i++) {
		//        if (i >= this.splitters.Count) {
		//            string msg = "YOU_GOT_MORE_PANELS_(DESERIALIZED)_THAN_SPLITTERS MUST_BE_EQUAL";
		//            Assembler.PopupException(msg);
		//            break;
		//        }
				
		//        PANEL_BASE panel = this.panels[i];
		//        MultiSplitter splitter = this.splitters[i];

		//        if (splitter.Location.X != x) {
		//            //splitter.Location = new Point(splitter.Location.X, y);
		//            splitter.Location = new Point(x, 0);
		//            thingsIchanged++;
		//        }
		//        x += splitter.Width;
				
		//        if (panel.Location.X != x) {
		//            //panel.Location = new Point(panel.Location.X, y);
		//            //I_SWAPPED_VERTICAL_PANELS_WDYM??? YOU_MAY_NEED_X_BUT_WHY_DO_YOU_SET_IT_NOW???
		//            panel.Location = new Point(x, 0);
		//            thingsIchanged++;
		//        }
		//        x += panel.Width;
		//    }
			
		//    //MOVED_20_LINES_UP if (Assembler.InstanceInitialized.MainFormDockFormsFullyDeserializedLayoutComplete == false) return;
			
		//    int deserializationError = Math.Abs(x - baseWidth);
		//    if (deserializationError <= 3) {
		//        string msg = "we don't need proportional horizontal fill when 1) splitterMoved, 2) splitterDragged"
		//            + " , 3) splitterPositionsByManorder.Count==this.panels.Count";
		//        //Assembler.PopupException(msg, null, false);
		//        return;
		//    }
			
		//    int panelWidth = baseWidth - this.MinimumPanelHeight;
		//    if (panelWidth < 0) {
		//        string msg = "I_SHOULD_NEVER_BE_HERE__WTF";
		//        Assembler.PopupException(msg, null, false);
		//        return;
		//    }

		//    // we need proportional vertical fill when 1) a new panel was added, 2) an old panel was removed, 3) Initialize(List<Panel>), 4) OnResize
		//    // SECOND_LOOP_RESIZES_EACH_HEIGHT_PROPORTIONALLY_TO_FILL_WHOLE_CONTAINER_SURFACE_VERTICALLY
		//    int totalFixedWidth = this.SplitterHeight * this.panels.Count;
		//    double fillVerticalK = (double) (baseWidth - totalFixedWidth) / (double) (x - totalFixedWidth);
			
		//    x = 0;
		//    for (int i=0; i<this.panels.Count; i++) {
		//        PANEL_BASE panel = this.panels[i];
		//        PanelBase panelBase = panels as PanelBase;
		//        if (panelBase == null) {
		//            Debugger.Break();
		//        }
		//        MultiSplitter splitter = this.splitters[i];
				
		//        int minimumPanelWidth = this.MinimumPanelHeight;
		//        var panelControl = panelBase as Control;
		//        Size defaultSize = default(Size);
		//        if (panelControl != null && panelControl.MinimumSize != null && panelControl.MinimumSize != defaultSize) {
		//            minimumPanelWidth = panelControl.MinimumSize.Width;
		//        }

		//        if (i == this.panels.Count - 1) {
		//            int lastSplitterMaxY = baseWidth - (splitter.Width + minimumPanelWidth);
		//            if (x > lastSplitterMaxY) {
		//                x = lastSplitterMaxY; 
		//            }
		//        }

		//        if (splitter.Location.X != x) {
		//            splitter.Location = new Point(x, 0);
		//            thingsIchanged++;
		//        }
		//        x += splitter.Width;
				
		//        if (panelBase.Location.X != x) {
		//            panelBase.Location = new Point(x, 0);
		//            thingsIchanged++;
		//        }

		//        // NB WILL_INVOKE_SiblingPanels.OnResize() => DistributePanelsAndSplittersVertically
		//        int newWidth = (int)(Math.Round(panelBase.Width * fillVerticalK, 0));
		//        Form parentAsForm = base.Parent as Form;
		//        if (parentAsForm != null) {
		//            string msg = "SUBTRACT_FORMS_PADDING_OR_WHATEVER";
		//            newWidth = parentAsForm.DisplayRectangle.Width;
		//            //newWidth = parentAsForm.PreferredSize.Width;
					
		//            //if (parentAsForm.HorizontalScroll != null && parentAsForm.HorizontalScroll.Visible == true) {
		//            //    newWidth -= parentAsForm.AutoScrollMargin.Width;
		//            //} else {
		//            //    string msg = "PARENT_FORM'S_SCROLLBAR_NULL__OR__IM_AN_INNER_MULTISPLITTER";
		//            //    Assembler.PopupException(msg, null, false);
		//            //}
		//        }

		//        if (panelBase.Width != newWidth) {
		//            //panel.Width = newWidth;
		//            panelBase.SetWidthIgnoreResize(newWidth);
		//            thingsIchanged++;
		//        }
		//        if (i == this.panels.Count - 1) {
		//            if (panelBase.Width < minimumPanelWidth) {
		//                //panel.Width = minimumPanelWidth;
		//                panelBase.SetWidthIgnoreResize(minimumPanelWidth);
		//            }

		//            if (x + panelBase.Width > baseWidth) {
		//                //panel.Width = baseWidth - x;
		//                panelBase.SetWidthIgnoreResize(baseWidth - x);
		//                thingsIchanged++;
		//                if (panelBase.Height < 0) {
		//                    string msg = "STILL_RESIZING_IN_GUI_THREAD???  panel.Width[" + panelBase.Width + "] < 0";
		//                    Assembler.PopupException(msg);
		//                    return;
		//                }
		//            }
		//        }
		//        x += panelBase.Width;
		//    }
			
		//    #if DEBUG		// TESTS_EMBEDDED
		//    //if (base.Width != baseWidth) {
		//    //    Debugger.Break();
		//    //}
		//    //baseWidth = base.Width;
		//    int roundingError = Math.Abs(x - baseWidth);
		//    if (roundingError > 1) {
		//        //Debugger.Break();	// LOWER_PANEL_GETS_CUT_BY_HSCROLLBAR
		//        string msg = "roundingError[" + roundingError + "] //distributePanelsAndSplittersHorizontally_setHeightAndY()";
		//        Assembler.PopupException(msg, null, false);
		//    }
		//    #endif
			
		//    if (thingsIchanged == 0) {
		//        string msg = "I_WAS_USELESS__REMOVE_MY_INVOCATION_FROM_UPSTACK //distributePanelsAndSplittersVertically_setWidthAndX()";
		//        Assembler.PopupException(msg, null, false);
		//    }

		//    // DO_I_NEED_IT? base.ResumeLayout();
		//    //base.Invalidate();
		//}
		

		SortedDictionary<string, MultiSplitterProperties> distributeColumns() {
		    SortedDictionary<string, MultiSplitterProperties> ret = new SortedDictionary<string, MultiSplitterProperties>();

		    // FIRST_LOOP_DISTRIBUTES_VERTICALLY_KEEP_ORIGINAL_PANELS_HEIGHTS
		    int x = 0;
		    for (int i=0; i<this.splitters.Count; i++) {
		        if (i >= this.panels.Count) {
		            string msg = "YOU_GOT_MORE_PANELS_(DESERIALIZED)_THAN_SPLITTERS MUST_BE_EQUAL";
		            Assembler.PopupException(msg);
		            break;
		        }

				MultiSplitter splitter = this.splitters[i];
				PANEL_BASE panel = this.panels[i];

				int minimumPanelWidth = this.MinimumPanelHeight;
				Control panelControl = panel as Control;
				if (panelControl != null && panelControl.MinimumSize != null && panelControl.MinimumSize.Width > 0) {
					minimumPanelWidth = panelControl.MinimumSize.Width;
				}

				MultiSplitterProperties splitterDistance = new MultiSplitterProperties(i, x, minimumPanelWidth);

			    ret.Add(panel.Name, splitterDistance);

		        x += splitter.Width;	// visually, splitter comes first, even for very first row (makes dragging handle visible)
		        x += panel.Width;		// may go beoynd base.Width but I will handle that on SetProperties()
		    }

			int baseWidth = base.Width;
			// we need proportional vertical fill when 1) a new panel was added, 2) an old panel was removed, 3) Initialize(List<Panel>), 4) OnResize
			// SECOND_LOOP_RESIZES_EACH_HEIGHT_PROPORTIONALLY_TO_FILL_WHOLE_CONTAINER_SURFACE_VERTICALLY
			int totalFixedWidth = this.SplitterHeight * ret.Count;
			double stretchHorizontalK = (double) (baseWidth - totalFixedWidth) / (double) (x - totalFixedWidth);
			
			x = 0;
			foreach (MultiSplitterProperties splitterDistance in ret.Values) {
				MultiSplitter splitter = this.splitters[splitterDistance.ManualOrder];
				bool atLastSplitter = splitterDistance.ManualOrder == ret.Count - 1;
				if (atLastSplitter) {
					int lastSplitterX = baseWidth - splitterDistance.DistanceMinimal - splitter.Width;	// subtracting from right boundary to where minimal should lie
					if (x > lastSplitterX) {
						x = lastSplitterX;		// yes I bite pre-last panel to let last Panel to take its minimal width
					}
				}
				int panelWidthStretched = (int)(Math.Round(splitterDistance.Distance * stretchHorizontalK, 0));
				x += splitter.Width;
				x += panelWidthStretched;
			}
			
			#if DEBUG		// TESTS_EMBEDDED
			int roundingError = Math.Abs(x - baseWidth);
			if (roundingError > 1) {
				string msg = "roundingError[" + roundingError + "] //distributeColumns()";
				Assembler.PopupException(msg, null, false);
			}
			#endif
			return ret;
		}

		SortedDictionary<string, MultiSplitterProperties> distributeRows() {
		    SortedDictionary<string, MultiSplitterProperties> ret = new SortedDictionary<string, MultiSplitterProperties>();

		    // FIRST_LOOP_DISTRIBUTES_VERTICALLY_KEEP_ORIGINAL_PANELS_HEIGHTS
		    int y = 0;
		    for (int i=0; i<this.splitters.Count; i++) {
		        if (i >= this.panels.Count) {
		            string msg = "YOU_GOT_MORE_PANELS_(DESERIALIZED)_THAN_SPLITTERS MUST_BE_EQUAL";
		            Assembler.PopupException(msg);
		            break;
		        }

				MultiSplitter splitter = this.splitters[i];
				PANEL_BASE panel = this.panels[i];

				int minimumPanelHeight = this.MinimumPanelHeight;
				Control panelControl = panel as Control;
				if (panelControl != null && panelControl.MinimumSize != null && panelControl.MinimumSize.Height > 0) {
					minimumPanelHeight = panelControl.MinimumSize.Height;
				}

				MultiSplitterProperties splitterDistance = new MultiSplitterProperties(i, y, minimumPanelHeight);

			    ret.Add(panel.Name, splitterDistance);

		        y += splitter.Height;	// visually, splitter comes first, even for very first row (makes dragging handle visible)
		        y += panel.Height;		// may go beoynd base.Width but I will handle that on SetProperties()
		    }

			int baseHeight = base.Height;
			// we need proportional vertical fill when 1) a new panel was added, 2) an old panel was removed, 3) Initialize(List<Panel>), 4) OnResize
			// SECOND_LOOP_RESIZES_EACH_HEIGHT_PROPORTIONALLY_TO_FILL_WHOLE_CONTAINER_SURFACE_VERTICALLY
			int totalFixedHeight = this.SplitterHeight * ret.Count;
			double stretchVerticalK = (double) (baseHeight - totalFixedHeight) / (double) (y - totalFixedHeight);
			
			y = 0;
			foreach (MultiSplitterProperties splitterDistance in ret.Values) {
				MultiSplitter splitter = this.splitters[splitterDistance.ManualOrder];
				bool atLastSplitter = splitterDistance.ManualOrder == ret.Count - 1;
				if (atLastSplitter) {
					int lastSplitterX = baseHeight - splitterDistance.DistanceMinimal - splitter.Height;	// subtracting from right boundary to where minimal should lie
					if (y > lastSplitterX) {
						y = lastSplitterX;		// yes I bite pre-last panel to let last Panel to take its minimal width
					}
				}
				int panelWidthStretched = (int)(Math.Round(splitterDistance.Distance * stretchVerticalK, 0));
				y += splitter.Height;
				y += panelWidthStretched;
			}
			
			#if DEBUG		// TESTS_EMBEDDED
			int roundingError = Math.Abs(y - baseHeight);
			if (roundingError > 1) {
				string msg = "roundingError[" + roundingError + "] //distributeRows()";
				Assembler.PopupException(msg, null, false);
			}
			#endif
			return ret;
		}


		public void PanelAddSplitterCreateAdd(PANEL_BASE panel, bool redistributeAfterAddingOneNotManyResistributeManual = true) {
//											  , Dictionary<string, MultiSplitterProperties> splitterPositionsByManorder = null) {
//			panel.Capture = true;	// NO_YOU_WONT will I have MouseEnter during dragging the splitter? I_HATE_HACKING_WINDOWS_FORMS
//		   	panel.MouseEnter += new EventHandler(panel_MouseEnter);
//		   	panel.MouseLeave += new EventHandler(panel_MouseLeave);
			//if (base.Controls.Contains(panel)) base.Controls.Remove(panel);
			// NO_ATTACHMENT_TO_ANY_REAL_SIZE_AT_THE_CONSTRUCTION_STAGE__MOVED_TO_distributeRows/distributeColumns
			//if (this.VerticalizeAllLogic == false) {
			//    panel.Width = base.Width;
			//} else {
			//    panel.Height = base.Height;
			//}

			//v1
			//if (panel.Parent != null && panel.Parent is Control) {
			//	Control parentControl = panel.Parent as Control;
			//	if (parentControl.Controls.Contains(panel)) parentControl.Controls.Remove(panel);
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
			// WILL_DO_LATER_ONCE_FOR_ALL__PropagateSplitterManorderDistanceIfFullyDeserialized() this.AssignPanelBelowAbove_fromPanelsList();
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
			this.AssignPanelBelowAbove_fromPanelsList();
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

		
		bool ignoreResizeImSettingWidthOrHeight;
		internal bool SetWidthIgnoreResize(int panelWidth) {
			if (base.Width == panelWidth) return false;
			if (this.ignoreResizeImSettingWidthOrHeight) return false;
			try {
				this.ignoreResizeImSettingWidthOrHeight = true;
				if (base.Parent != null) {
					if (panelWidth >= base.Parent.Width) {
						string msg = "BEOYND_PARENTS_WIDTH__NOT_TALKING_ABOUT_BORDERS panelWidth[" + panelWidth + "] >= base.Parent.Width[" + base.Parent.Width + "]";
						Assembler.PopupException(msg, null, false);
					}
				}
				base.Width  = panelWidth;
			} finally {
				this.ignoreResizeImSettingWidthOrHeight = false;
			}
			return true;
		}
		internal bool SetHeightIgnoreResize(int panelHeight) {
			if (base.Height == panelHeight) return false;
			if (this.ignoreResizeImSettingWidthOrHeight) return false;
			try {
				this.ignoreResizeImSettingWidthOrHeight = true;
				if (base.Parent != null) {
					if (panelHeight >= base.Parent.Height) {
						string msg = "BEOYND_PARENTS_HEIGHT__NOT_TALKING_ABOUT_BORDERS panelWidth[" + panelHeight + "] >= base.Parent.Width[" + base.Parent.Height + "]";
						Assembler.PopupException(msg, null, false);
					}
				}
				base.Height  = panelHeight;
			} finally {
				this.ignoreResizeImSettingWidthOrHeight = false;
			}
			return true;
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
