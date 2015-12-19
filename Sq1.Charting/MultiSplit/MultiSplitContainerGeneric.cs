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

		//public int MinimalPanelWidths_SumIfVertical_MaxIfHorizontal { get {
		//    int ret = 0;
		//    foreach (PANEL_BASE panel in this.panels) {
		//        if (panel.ToString().Contains("UNINITIALIZED")) {
		//            string msg = "YOU_DIDNT_INITIALIZE_THE_PANEL_NAME";
		//            Assembler.PopupException(msg);
		//        }
		//        if (this.VerticalizeAllLogic) {
		//            ret += panel.MinimumSize.Width;
		//        } else {
		//            ret = Math.Max(ret, panel.MinimumSize.Width);
		//        }
		//    }
		//    return ret;
		//} }
		//public int MinimalPanelHeights_SumIfHorizontal_MaxIfVertical { get {
		//    int ret = 0;
		//    foreach (PANEL_BASE panel in this.panels) {
		//        if (panel.ToString().Contains("UNINITIALIZED")) {
		//            string msg = "YOU_DIDNT_INITIALIZE_THE_PANEL_NAME";
		//            Assembler.PopupException(msg);
		//        }
		//        if (this.VerticalizeAllLogic == false) {
		//            ret += panel.MinimumSize.Height;
		//        } else {
		//            ret = Math.Max(ret, panel.MinimumSize.Height);
		//        }
		//    }
		//    return ret;
		//} }

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

		public void InitializeCreateSplittersDistributeFor(List<PANEL_BASE> whatIadd) {
			// DOENST_HELP_OPENING_IN_DESIGNER STILL_THROWS if (base.DesignMode) return;
			//Debugger.Break();
			this.panels.Clear();
			this.splitters.Clear();
			//base.SuspendLayout();	// forces this.PanelAddSplitterCreateAdd not to invoke my.OnResize() <=PerformLayout() 
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
			if (Assembler.InstanceInitialized.MainFormDockFormsFullyDeserializedLayoutComplete == false) {
				string msg = "ONCE_PER_PANEL_LET_IT_DISTRIBUTE_COMMENTED WHAT_IF_YOU_FIND_ALL_AND_LINK_PANEL_BELOWs_HERE?...";
		        Assembler.PopupException(msg);
		        return;
			}
		    if (this.splitters.Count < 1) {
		        string msg = "YOU_NEED_TO_ADD_PANELS_BEFORE_HEIGHT_DISTRIBUTION";
		        Assembler.PopupException(msg);
		        return;
		    }
		    if (this.splitters.Count != this.panels.Count) {
		        string msg = "YOU_GOT_MORE_PANELS_(DESERIALIZED)_THAN_SPLITTERS MUST_BE_EQUAL";
		        Assembler.PopupException(msg);
		        return;
		    }
			for (int i=0; i<this.panels.Count; i++) {
				PANEL_BASE panel = this.panels[i];
				if (panel == null) {
					string msg = "PANELS_MUST_NOT_BE_NULL";
					Assembler.PopupException(msg);
					return;
				}
				if (string.IsNullOrEmpty(panel.Name)) {
					string msg = "PANELS_MUST_HAVE_NAME";
					Assembler.PopupException(msg);
					return;
				}
			}

			this.AssignPanelBelowAbove_setMinimalSize_fromPanelsList();
			this.DistributePanelsAndSplitters();	// this must be single panel distribution after MainFormDockFormsFullyDeserializedLayoutComplete=true
			//base.ResumeLayout(true);
		}
		public void AssignPanelBelowAbove_setMinimalSize_fromPanelsList() {
			for (int i=0; i<this.panels.Count; i++) {
				if (i >= this.splitters.Count) {
					string msg = "YOU_GOT_MORE_PANELS_(DESERIALIZED)_THAN_SPLITTERS MUST_BE_EQUAL";
					Assembler.PopupException(msg);
					break;
				}
				PANEL_BASE panel = this.panels[i];
				MultiSplitter splitter = this.splitters[i];

				splitter.PanelBelow = panel as Control;				// back to real world from generics
				if (i > 0) {
					splitter.PanelAbove = this.panels[i - 1] as Control;
				} else {
					splitter.PanelAbove = null;
				}

				if (panel.MinimumSize.Width > base.MinimumSize.Width) {
					base.MinimumSize = new Size(panel.MinimumSize.Width, base.MinimumSize.Height);
				}
				if (panel.MinimumSize.Height > base.MinimumSize.Height) {
					base.MinimumSize = new Size(base.MinimumSize.Height, panel.MinimumSize.Width);
				}
			}
		}
		public void DistributePanelsAndSplitters() {		//Dictionary<int, int> splitterPositionsByManorder = null) {
			Dictionary<string, MultiSplitterProperties> splittersDistributed =
				this.VerticalizeAllLogic
					? this.distributeColumns()
					: this.distributeRows();
			this.SplitterPropertiesByPanelNameSet(splittersDistributed);
		}

		Dictionary<string, MultiSplitterProperties> distributeColumns() {
		    int panelsWidthOriginal= 0;
		    for (int i=0; i<this.splitters.Count; i++) {
				PANEL_BASE panel = this.panels[i];
		        panelsWidthOriginal += panel.Width;		// mapanelsWidthOriginalgo beoynd base.Width but I will handle that on SetProperties()
		    }

			int baseWidth = base.Width;
			// we need proportional vertical fill when 1) a new panel was added, 2) an old panel was removed, 3) Initialize(List<Panel>), 4) OnResize
			int totalFixedWidth = this.SplitterHeight * this.splitters.Count;
			int panelsWidthEffective = baseWidth - totalFixedWidth;
			//panelsWidthEffective -= this.splitters.Count * 2;	// panel start NEXT pixel after splitter's boundary => each panel has 1px less space
			double stretchVerticalK = (double) panelsWidthEffective / (double) panelsWidthOriginal;
			
		    int x = 0;
			int stealFromNextPanel = 0;
			string lastPanelName = null;
		    Dictionary<string, MultiSplitterProperties> ret = new Dictionary<string, MultiSplitterProperties>();

			for (int i=0; i<this.splitters.Count; i++) {
				PANEL_BASE panel = this.panels[i];
				lastPanelName = panel.Name;
				int panelWidthStretched = (int)(Math.Round(panel.Width * stretchVerticalK, 0));

				if (stealFromNextPanel > 0) {
					x += stealFromNextPanel;
					stealFromNextPanel = 0;
				}

				int panelWidthMinimal = this.MinimumPanelHeight;
				if (panel.MinimumSize != null && panel.MinimumSize.Width > 0) {
					panelWidthMinimal = panel.MinimumSize.Width;
				}

				if (panelWidthMinimal > panelWidthStretched) {
					stealFromNextPanel = panelWidthMinimal - panelWidthStretched;
					panelWidthStretched = panelWidthMinimal;
				}

				MultiSplitterProperties splitterDistance = new MultiSplitterProperties(i, x, this.SplitterHeight, panelWidthStretched);
				ret.Add(panel.Name, splitterDistance);

				x += this.SplitterHeight;	// separate heights for splitters, different for each splitter, aren't supported
				x += panelWidthStretched;
			}

		    int roundingError = Math.Abs(x - baseWidth);
		    if (roundingError != 0) {
				string msg = "YOUR_DISTRIBUTION_COEFFICIENT_WASNT_PRECISE LAST_PANEL_WIDTH_SUBTRACTED[" + roundingError + "]";	// LOWER_PANEL_GETS_CUT_BY_HSCROLLBAR
				Assembler.PopupException(msg, null, false);

				MultiSplitterProperties lastSplitterProp = ret[lastPanelName];
				lastSplitterProp.PanelHeight -= roundingError;
		    }
			return ret;
		}

		Dictionary<string, MultiSplitterProperties> distributeRows() {
		    int panelsHeightOriginal= 0;
		    for (int i=0; i<this.splitters.Count; i++) {
				PANEL_BASE panel = this.panels[i];
		        panelsHeightOriginal += panel.Height;		// mapanelsHeightOriginalgo beoynd base.Width but I will handle that on SetProperties()
		    }

			int baseHeight = base.Height;
			// we need proportional vertical fill when 1) a new panel was added, 2) an old panel was removed, 3) Initialize(List<Panel>), 4) OnResize
			int totalFixedHeight = this.SplitterHeight * this.splitters.Count;
			int panelsHeightEffective = baseHeight - totalFixedHeight;
			//BOTTOM_4PX_GRAY panelsHeightEffective -= this.splitters.Count * 2;	// panel start NEXT pixel after splitter's boundary => each panel has 1px less space
			double stretchVerticalK = (double) panelsHeightEffective / (double) panelsHeightOriginal;
			
		    int y = 0;
			int stealFromNextPanel = 0;
			string lastPanelName = null;
		    Dictionary<string, MultiSplitterProperties> ret = new Dictionary<string, MultiSplitterProperties>();

			for (int i=0; i<this.splitters.Count; i++) {
				PANEL_BASE panel = this.panels[i];
				lastPanelName = panel.Name;
				int panelHeightStretched = (int)(Math.Round(panel.Height * stretchVerticalK, 0));

				if (stealFromNextPanel > 0) {
					panelHeightStretched -= stealFromNextPanel;
					stealFromNextPanel = 0;
				}

				int panelHeightMinimal = this.MinimumPanelHeight;
				if (panel.MinimumSize != null && panel.MinimumSize.Height > 0) {
					panelHeightMinimal = panel.MinimumSize.Height;
				}

				if (panelHeightMinimal > panelHeightStretched) {
					stealFromNextPanel = panelHeightMinimal - panelHeightStretched;
					panelHeightStretched = panelHeightMinimal;
				}

				MultiSplitterProperties splitterDistance = new MultiSplitterProperties(i, y, this.SplitterHeight, panelHeightStretched);
				ret.Add(panel.Name, splitterDistance);

				y += this.SplitterHeight;	// separate heights for splitters, different for each splitter, aren't supported
				y += panelHeightStretched;
			}

		    int roundingError = Math.Abs(y - baseHeight);
		    if (roundingError != 0) {
				string msg = "YOUR_DISTRIBUTION_COEFFICIENT_WASNT_PRECISE LAST_PANEL_HEIGHT_SUBTRACTED[" + roundingError + "]";	// LOWER_PANEL_GETS_CUT_BY_HSCROLLBAR
				Assembler.PopupException(msg, null, false);

				MultiSplitterProperties lastSplitterProp = ret[lastPanelName];
				lastSplitterProp.PanelHeight -= roundingError;
		    }
			return ret;
		}


		public void PanelAddSplitterCreateAdd(PANEL_BASE panel, bool redistributeAfterAddingOneNotManyResistributeManual = true) {
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
			this.AssignPanelBelowAbove_setMinimalSize_fromPanelsList();
			this.DistributePanelsAndSplitters();
		}
		void splitterCreateAdd(PANEL_BASE panel) {
			MultiSplitter splitter = new MultiSplitter(GrabHandleWidth, ColorGrabHandle, this.VerticalizeAllLogic, this.DebugSplitter);
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
					if (panelWidth > base.Parent.Width) {
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
					if (panelHeight > base.Parent.Height) {
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
