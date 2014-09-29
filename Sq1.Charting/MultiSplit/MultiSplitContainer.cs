using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace Sq1.Charting.MultiSplit {
	//LAZY_TO_MAKE_IT_ACCEPT_NESTED_PANELS_DROPPED_FOR_DESIGNER THIS_SOLVES_ANOTHER_PROBLEM http://stackoverflow.com/questions/2785376/how-to-enable-design-support-in-a-custom-control/2863807#2863807
	// PanelNamedFolding can be replaced by Control if you want; it's an overkill to make it MultiSplitContainer<T> : UserControl where T : Control 
	public partial class MultiSplitContainer {
		public int SplitterHeight;
		public int GrabHandleWidth = 30;
		public Color ColorGrabHandle;
		public Color ColorBackgroundSliderDroppingTarget;
		public Color ColorBackgroundSliderRegular;
		public Color ColorBackgroundSliderMouseOver;
        public bool DebugSplitter;

		ObservableCollection<PanelNamedFolding> panels;
		ObservableCollection<MultiSplitter> splitters;
		public Dictionary<Control, int> initialHeights;
        
		public MultiSplitContainer() : this(false) {
		}
        public MultiSplitContainer(bool debugSplitter = false) {
			panels = new ObservableCollection<PanelNamedFolding>();
			splitters = new ObservableCollection<MultiSplitter>();
			SplitterHeight = 5;
			ColorGrabHandle = Color.Orange;
			ColorBackgroundSliderDroppingTarget = Color.Red;
			ColorBackgroundSliderRegular = Color.DarkGray;
			ColorBackgroundSliderMouseOver = Color.SlateGray;
            DebugSplitter = debugSplitter;
            initialHeights = new Dictionary<Control, int>();
		}
//		[Obsolete("")]
		public void InitializeExtractPanelsFromBaseControlsCreateSplittersDistribute() {
			var list = new List<PanelNamedFolding>();
			foreach (PanelNamedFolding c in base.Controls) {
				c.Text = c.Name;
				list.Add(c);
			}
			this.InitializeCreateSplittersDistributeFor(list);
		}
		public void InitializeCreateSplittersDistributeFor(List<PanelNamedFolding> whatIadd) {
			//Debugger.Break();
			this.panels.Clear();
			this.splitters.Clear();
			foreach (PanelNamedFolding c in whatIadd) {
				PanelNamedFolding panel = c as PanelNamedFolding;
				if (panel == null) continue;
				this.PanelAddSplitterCreateAdd(panel, true);
			}
			this.DistributePanelsAndSplittersVerticallyByPanelsOrder();
		}
		public void DistributePanelsAndSplittersVerticallyByPanelsOrder() {
			int baseHeight = base.Height;
			
			// DO_I_NEED_IT? base.SuspendLayout();
			int y = 0;
	        for (int i=0; i<this.panels.Count; i++) {
				if (i >= this.splitters.Count) {
					Debugger.Break();
					break;
				}
	        	
	        	PanelNamedFolding panel = this.panels[i];
	        	MultiSplitter splitter = this.splitters[i];
	        	if (splitter.Tag != panel) {
	        		Debugger.Break();
	        	}

	        	//splitter.Location = new Point(splitter.Location.X, y);
	        	splitter.Location = new Point(0, y);
	        	y += splitter.Height;
	        	splitter.Width = base.Width;

	        	//panel.Location = new Point(panel.Location.X, y);
	        	panel.Location = new Point(0, y);
	        	panel.Width = base.Width;
	        	y += panel.Height;
			}
			
			int totalFixedHeight = this.SplitterHeight * this.panels.Count;
			double fillVerticalK = (double) (base.Height) / (double) (y - totalFixedHeight);
			
			y = 0;
			for (int i=0; i<this.panels.Count; i++) {
	        	PanelNamedFolding panel = this.panels[i];
	        	MultiSplitter splitter = this.splitters[i];

	        	if (i == this.panels.Count - 1) {
	        		int lastSplitterMaxY = base.Height - (splitter.Height + 5);
	        		if (y > lastSplitterMaxY) {
		        		y = lastSplitterMaxY; 
	        		}
	        	}

	        	splitter.Location = new Point(0, y);
	        	y += splitter.Height;
	        	
	        	panel.Location = new Point(0, y);
	        	panel.Height = (int)(panel.Height * fillVerticalK);
	        	if (i == this.panels.Count - 1) {
		        	if (panel.Height < 5) {
		        		panel.Height = 5;
		        	}
					if (y + panel.Height > base.Height - 5) {
		        		//panel.Height = 5;
		        	}
	        	}
	        	y += panel.Height;
			}
			
			// DO_I_NEED_IT? base.ResumeLayout();
			//Debugger.Break();
			//base.Invalidate();
		}

		public void PanelAddSplitterCreateAdd(PanelNamedFolding panel, bool dontRedistributeAddingMany = false) {
//			panel.Capture = true;	// NO_YOU_WONT will I have MouseEnter during dragging the splitter? I_HATE_HACKING_WINDOWS_FORMS
//	       	panel.MouseEnter += new EventHandler(panel_MouseEnter);
//	       	panel.MouseLeave += new EventHandler(panel_MouseLeave);
			//if (base.Controls.Contains(panel)) base.Controls.Remove(panel);
			base.Controls.Add(panel);
			this.initialHeights.Add(panel, panel.Height);
			this.panels.Add(panel);
			this.splitterCreateAdd(panel);
			if (dontRedistributeAddingMany) return;
			this.DistributePanelsAndSplittersVerticallyByPanelsOrder();
		}
		public void PanelRemove(PanelNamedFolding panel) {
			int panelIndex = this.panels.IndexOf(panel);
			if (panelIndex == 0) return; 
			MultiSplitter splitter = this.splitters[panelIndex];
			this.panels.Remove(panel);
			this.splitters.Remove(splitter);
			base.Controls.Remove(panel);
			base.Controls.Remove(splitter);
			initialHeights.Remove(panel);
			this.DistributePanelsAndSplittersVerticallyByPanelsOrder();
		}
		public void splitterCreateAdd(PanelNamedFolding panel) {
			MultiSplitter splitter = new MultiSplitter(GrabHandleWidth, ColorGrabHandle);
			splitter.Height = SplitterHeight;
			splitter.Tag = panel;
        	splitter.MouseMove += new MouseEventHandler(splitter_MouseMove);
        	splitter.MouseEnter += new EventHandler(splitter_MouseEnter);
        	splitter.MouseLeave += new EventHandler(splitter_MouseLeave);
        	splitter.MouseUp += new MouseEventHandler(splitter_MouseUp);
        	splitter.MouseDown += new MouseEventHandler(splitter_MouseDown);
			this.splitters.Add(splitter);
			base.Controls.Add(splitter);	//make splitter receive OnPaint()
		}
		
	}
}
