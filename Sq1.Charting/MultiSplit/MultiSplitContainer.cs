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
	// PanelNamedFolding can be replaced by Control if you want; it's an overkill to make it MultiSplitContainer<PANEL_BASE> : UserControl where T : Control 
	// did overkill for MultiSplitTest.cs ....  public partial class MultiSplitContainer {
	 public partial class MultiSplitContainer<PANEL_BASE> {
		public int SplitterHeight;
		public int GrabHandleWidth;
		public int MinimumPanelSize;
		public Color ColorGrabHandle;
		public Color ColorBackgroundSliderDroppingTarget;
		public Color ColorBackgroundSliderRegular;
		public Color ColorBackgroundSliderMouseOver;
        public bool DebugSplitter;
		//public Dictionary<Control, int> initialHeights;
		
		ObservableCollection<PANEL_BASE> panels;
		ObservableCollection<MultiSplitter> splitters;
		
		public MultiSplitContainer() : this(false) {
		}
        public MultiSplitContainer(bool debugSplitter = false) {
			panels = new ObservableCollection<PANEL_BASE>();
			splitters = new ObservableCollection<MultiSplitter>();
			SplitterHeight = 5;
			GrabHandleWidth = 30;
			MinimumPanelSize = 5;
			ColorGrabHandle = Color.Orange;
			ColorBackgroundSliderDroppingTarget = Color.Red;
			ColorBackgroundSliderRegular = Color.DarkGray;
			ColorBackgroundSliderMouseOver = Color.SlateGray;
            DebugSplitter = debugSplitter;
            //initialHeights = new Dictionary<Control, int>();
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
			//Debugger.Break();
			this.panels.Clear();
			this.splitters.Clear();
			foreach (PANEL_BASE c in whatIadd) {
				PANEL_BASE panel = c as PANEL_BASE;
				if (panel == null) continue;
				this.PanelAddSplitterCreateAdd(panel, false);
			}
			this.DistributePanelsAndSplittersVertically();
		}
		public Dictionary<string, MultiSplitterProperties> SplitterPropertiesByPanelNameGet() {
			Dictionary<string, MultiSplitterProperties> ret = new Dictionary<string, MultiSplitterProperties>();
			foreach (MultiSplitter s in this.splitters) {
				#region CANT_ACCESS_PANELS_FROM_SlidersAutoGrow_SliderValueChanged()__NEED_MultiSplitterPropertiesByPanelName_SYNCED_TO_RESTORE_AFTER_DESERIALIZATION
				PanelIndicator panelIndicator = s.PanelBelow as PanelIndicator;  
				if (panelIndicator != null) {
					string candidateIndicatorClicked = panelIndicator.Indicator.ToString();
					if (panelIndicator.PanelName != candidateIndicatorClicked) {
						panelIndicator.PanelName  = candidateIndicatorClicked;
					}
				}
				#endregion
				int sernoFromObservable = this.splitters.IndexOf(s);
				ret.Add(s.PanelBelow.PanelName, new MultiSplitterProperties(sernoFromObservable, s.Location.Y));
			}
			return ret;
		}
		public void SplitterPropertiesByPanelNameSet(Dictionary<string, MultiSplitterProperties> splitterPropertiesByPanelName) {
			if (splitterPropertiesByPanelName == null) return;
			if (splitterPropertiesByPanelName.Count == 0) return;
			if (splitterPropertiesByPanelName.Count != this.panels.Count) {
				#if DEBUG
				Debugger.Break();
				#endif
			}
       		int baseHeight = base.Height;
try {
			foreach (string panelName in splitterPropertiesByPanelName.Keys) {
				MultiSplitterProperties prop = splitterPropertiesByPanelName[panelName];
				if (prop.ManualOrder < 0 && prop.ManualOrder >= this.splitters.Count) {
					string msg = "SPLITTER_SKIPPED_CANT_MOVE_NO_DESTINATION_WHERE_TO panelName[" + panelName + "]";
					#if DEBUG
					Debugger.Break();
					#endif
					Assembler.PopupException(msg);
					continue;
				}
				if (prop.ManualOrder < 0 && prop.ManualOrder >= this.panels.Count) {
					string msg = "SPLITTER_SKIPPED_CANT_MOVE_PANEL_NO_DESTINATION_WHERE_TO panelName[" + panelName + "]";
					#if DEBUG
					Debugger.Break();
					#endif
					Assembler.PopupException(msg);
					continue;
				}

				MultiSplitter splitterFound = null;
				foreach (MultiSplitter each in this.splitters) {
					if (each.PanelBelow.PanelName != panelName) continue;
					splitterFound = each;
					break;
				}
				if (splitterFound == null) {
					string msg = "SPLITTER_NOT_FOUND_WHILE_SETTING_MANORDER_DISTANCE panelName[" + panelName + "]";
					#if DEBUG
					Debugger.Break();
					#endif
					Assembler.PopupException(msg);
					continue;
				}
				splitterFound.Location = new Point(splitterFound.Location.X, prop.Distance);
				int splitterFoundIndex = this.splitters.IndexOf(splitterFound);
				if (splitterFoundIndex == prop.ManualOrder) {
					continue;
				}
				// very illogical way to sync-up; splitters may have holes and implies MultiSplitterPropertiesByPanelName.*.ManualOrder must have no holes/duplicates
				this.panels.Move(splitterFoundIndex, prop.ManualOrder);
				this.splitters.Move(splitterFoundIndex, prop.ManualOrder);
       		}
			
			// align panels to splitters; I need to know the prevSplitterLocationY to set panelHeight
			int y = 0;
			for (int i=this.splitters.Count-1; i>=0; i--) {
	        	PANEL_BASE panel = this.panels[i];
	        	MultiSplitter splitter = this.splitters[i];
	        	
				int panelY = splitter.Location.Y + splitter.Height;
	        	if (panel.Location.Y != panelY) {
	        		panel.Location = new Point(panel.Location.X, panelY);
	        	}
	        	
	        	int panelHeight = -1;
	        	if (i == this.splitters.Count-1) {
	        		panelHeight = baseHeight - panelY;
	        		if (panelHeight < 0) {
						#if DEBUG
		    			//POSTPONE_WINDOWS_MAGIC Debugger.Break();	// AFTERDRAG_MANUAL_ORDER_WASNT_SYNCED YOUVE_HAD_MAXIMIZED_OK_BUT_NORMAL_WINDOW_SIZE_MADE_THE_PANEL_TOO_SMALL MAYBE_YOU_CAN_COLLAPSE_EXCEPTIONS_PANEL_TO_SEE_VOLUME?
		    			#endif
		        		return;
	        		}
		        	if (panelHeight < MinimumPanelSize) {
		        		panelHeight = MinimumPanelSize;
		        	}
	        	} else {
	        		MultiSplitter lowerSplitter = this.splitters[i+1];		//prevSplitterLocationY
	        		panelHeight = lowerSplitter.Location.Y - panelY;  
		        	if (panelHeight < MinimumPanelSize) {
		        		panelHeight = MinimumPanelSize;
		        	}
	        	}

	        	//panelHeight -= splitter.Height + 40;	// LOWER_PANEL_GETS_CUT_BY_HSCROLLBAR
	        	if (panel.Height != panelHeight) {
		        	panel.Height = panelHeight;
	        	}
	        	y += splitter.Height;
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
			
} catch (Exception ex) {
	#if DEBUG
	Debugger.Break();
	#endif
	Assembler.PopupException(null, ex);
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
					Debugger.Break();
					break;
				}
	        	
	        	PANEL_BASE panel = this.panels[i];
	        	MultiSplitter splitter = this.splitters[i];

	        	//splitter.Location = new Point(splitter.Location.X, y);
	        	splitter.Location = new Point(0, y);
	        	y += splitter.Height;
	        	
				// SpliiterMovingEnded will need to know splitter.PanelBelow.Location to save it
				PanelBase panelNamedFolding = panel as PanelBase;	// back from generics to real world
				if (panelNamedFolding != null) {
		        	splitter.PanelBelow = panelNamedFolding;
		        	if (i > 0) {
		        		PANEL_BASE prevPanel = this.panels[i - 1];
						PanelBase prevPanelNamedFolding = prevPanel as PanelBase;	// back from generics to real world
						if (panelNamedFolding != null) {
			        		splitter.PanelAbove = prevPanelNamedFolding;
						}
		        	}
				}

	        	//panel.Location = new Point(panel.Location.X, y);
	        	panel.Location = new Point(0, y);
	        	y += panel.Height;
			}
			
			if (Assembler.InstanceInitialized.MainFormDockFormsFullyDeserializedLayoutComplete == false) return;
			
			int deserializationError = Math.Abs(y - baseHeight);
			if (deserializationError <= 3) {
				#if DEBUG
				//Debugger.Break();
				#endif
				return;			// we don't need proportional vertical fill when 1) splitterMoved, 2) splitterDragged, 3) splitterPositionsByManorder.Count==this.panels.Count
			}
			
    		int panelHeight = baseHeight - MinimumPanelSize;
    		if (panelHeight < 0) {
    			#if DEBUG
    			Debugger.Break();		// WTF
    			#endif
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

	        	if (i == this.panels.Count - 1) {
	        		int lastSplitterMaxY = baseHeight - (splitter.Height + MinimumPanelSize);
	        		if (y > lastSplitterMaxY) {
		        		y = lastSplitterMaxY; 
	        		}
	        	}

	        	splitter.Location = new Point(0, y);
	        	y += splitter.Height;
	        	
	        	panel.Location = new Point(0, y);
	        	panel.Height = (int)(Math.Round(panel.Height * fillVerticalK, 0));
	        	if (i == this.panels.Count - 1) {
		        	if (panel.Height < MinimumPanelSize) {
		        		panel.Height = MinimumPanelSize;
		        	}

	        		if (y + panel.Height > baseHeight) {
		        		panel.Height = baseHeight - y;
		        		if (panel.Height < 0) {
			    			Debugger.Break();
			        		return;		// WTF
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

		public void PanelAddSplitterCreateAdd(PANEL_BASE panel, bool redistributeAfterAddingOneNotManyResistributeManual = true) {
//		                                      , Dictionary<string, MultiSplitterProperties> splitterPositionsByManorder = null) {
//			panel.Capture = true;	// NO_YOU_WONT will I have MouseEnter during dragging the splitter? I_HATE_HACKING_WINDOWS_FORMS
//	       	panel.MouseEnter += new EventHandler(panel_MouseEnter);
//	       	panel.MouseLeave += new EventHandler(panel_MouseLeave);
			//if (base.Controls.Contains(panel)) base.Controls.Remove(panel);
        	panel.Width = base.Width;
			base.Controls.Add(panel);
			#if DEBUG
			if (base.Parent.Controls.Contains(panel)) {
				string msg = "I expect panels be removed from miltisplitContainer.Parent"
					+ " so that only miltisplitContainer.Size will trigger miltisplitContainer.Controls.*.Resize()";
				Debugger.Break();
			}
			#endif
			//this.initialHeights.Add(panel, panel.Height);
			this.panels.Add(panel);
			this.splitterCreateAdd(panel);
			if (redistributeAfterAddingOneNotManyResistributeManual == false) return;
			this.DistributePanelsAndSplittersVertically();
		}
		public void PanelRemove(PANEL_BASE panel) {
			int panelIndex = this.panels.IndexOf(panel);
			if (panelIndex == 0) return; 
			MultiSplitter splitter = this.splitters[panelIndex];
			this.panels.Remove(panel);
			this.splitters.Remove(splitter);
			base.Controls.Remove(panel);
			base.Controls.Remove(splitter);
			//initialHeights.Remove(panel);
			this.DistributePanelsAndSplittersVertically();
		}
		public void splitterCreateAdd(PANEL_BASE panel) {
			MultiSplitter splitter = new MultiSplitter(GrabHandleWidth, ColorGrabHandle);
        	splitter.Width = base.Width;
			splitter.Height = SplitterHeight;
			//ROLLEDBACK panel.Height -= SplitterHeight;	// LOWER_PANEL_GETS_CUT_BY_HSCROLLBAR FIX
        	splitter.MouseMove += new MouseEventHandler(splitter_MouseMove);
        	splitter.MouseEnter += new EventHandler(splitter_MouseEnter);
        	splitter.MouseLeave += new EventHandler(splitter_MouseLeave);
        	splitter.MouseUp += new MouseEventHandler(splitter_MouseUp);
        	splitter.MouseDown += new MouseEventHandler(splitter_MouseDown);
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
