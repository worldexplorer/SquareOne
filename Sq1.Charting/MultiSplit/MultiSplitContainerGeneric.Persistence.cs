using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Core.Charting;

namespace Sq1.Charting.MultiSplit {
	 public partial class MultiSplitContainerGeneric<PANEL_BASE> {
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
				int manualOrder = this.splitters.IndexOf(s);
				if (s.PanelBelow == null) {
					string msg = "AVOIDING_NPE //SplitterPropertiesByPanelNameGet()";
					Assembler.PopupException(msg, null, false);
					continue;
				}
				int distance	= this.VerticalizeAllLogic == false ? s.Location.Y			: s.Location.X;
				int panelHeight	= this.VerticalizeAllLogic == false ? s.PanelBelow.Height	: s.PanelBelow.Width;
				ret.Add(s.PanelBelow.Name, new MultiSplitterProperties(manualOrder, distance, this.SplitterHeight, panelHeight));
			}
			return ret;
		}
		public void SplitterPropertiesByPanelNameSet(Dictionary<string, MultiSplitterProperties> splitterPropertiesByPanelName) {
			if (splitterPropertiesByPanelName == null) return;
			if (splitterPropertiesByPanelName.Count == 0) return;
			if (splitterPropertiesByPanelName.Count != this.panels.Count) {
				string msg = "multisplit container doesn't get indicators panels added when WorkspaceLoad()"
					+ "; if I return here to skip PropagateSplitterManorderDistance, will I be re-invoked after indicators are added?";
				Assembler.PopupException(msg, null, false);
				// NO_BETTER_LET_IT_SPARSELY_ASSIGN_PANEL_HEIGHTS__INDICATORS_WILL_PERFECTLY_FIT_IN_LATER return;
			}

			for (int i=this.splitters.Count-1; i>=0; i--) {
				PANEL_BASE panel = this.panels[i];
				MultiSplitter splitter = this.splitters[i];

				string panelName = panel.Name;
				if (splitterPropertiesByPanelName.ContainsKey(panelName) == false) {
					string msg = "CRITICAL YOU_ADDED_RENAMED_A_PANEL_AFTER_BUILDING_PROPERTIES or DESERIALIZED_DICTIONARY_CONTAINS_OLD_CONTENT_OR_NAMES_OF_PANELS";
					Assembler.PopupException(msg);
					return;
				}

				MultiSplitterProperties props = splitterPropertiesByPanelName[panelName];
				if (props.ManualOrder != i) {
					string msg = "YOU_SWAPPED_PANELS_AND_SAVED_IN_PROPERTIES__NEED_TO_MOVE_INSIDE_ARRAYS";
					Assembler.PopupException(msg);
					return;
				}
			}

			base.SuspendLayout();
			int thingsIchanged = this.VerticalizeAllLogic
				? this.propagateColumns(splitterPropertiesByPanelName)
				: this.propagateRows(splitterPropertiesByPanelName);
			base.ResumeLayout();
		}
		int propagateColumns(Dictionary<string, MultiSplitterProperties> splitterPropertiesByPanelName) {
			int thingsIchanged = 0;
			int xCheckDiff = 0;
			for (int i=0; i<this.splitters.Count; i++) {
				PANEL_BASE panel = this.panels[i];
				MultiSplitter splitter = this.splitters[i];

				string panelName = panel.Name;
				MultiSplitterProperties props = splitterPropertiesByPanelName[panelName];

				if (splitter.Width != this.SplitterHeight) {
					string msg = "MUST_BE_INITIALIZED_EARLIER splitter[" + splitter.Name + "].Width[" + splitter.Width+ "] => [" + this.SplitterHeight + "]";
					Assembler.PopupException(msg);
					splitter.Width  = this.SplitterHeight;
					thingsIchanged++;
				}
				if (splitter.Location.X != props.Distance) {
					splitter.Location = new Point(props.Distance, 0);
					thingsIchanged++;
				}

				int panelX = splitter.Location.X + splitter.Width;
				if (panel.Location.X != panelX) {
					panel.Location = new Point(panelX, 0);
					thingsIchanged++;
				}

				bool widthChanged = false;
				int newWidth = props.PanelHeight;	// yes PanelHeight with VerticalizeLogic = Width
				if (panel.Width != newWidth) {
					//PanelBase panelBase = panel as PanelBase;
					//MultiSplitContainer panelAsMultiSplitContainer = panel as MultiSplitContainer;
					//if (panelBase != null) {
					//    widthChanged = panelBase.SetWidthIgnoreResize(newWidth);
					//} else if (panelAsMultiSplitContainer != null) {
					//    widthChanged = panelAsMultiSplitContainer.SetWidthIgnoreResize(newWidth);
					//} else {
						panel.Width  = newWidth;
						widthChanged = true;
					//}
				}
				if (widthChanged) thingsIchanged++;
				if (panel.Width != newWidth) {
					string msg = "TO_UNDELAY_SIZE_PROPAGATION_INVOKE_base.SuspendLayout()_UPSTACK panel.Width[" + panel.Width + "] != newWidth[" + newWidth + "]";
					Assembler.PopupException(msg);
				}


				bool heightChanged = false;
		   		int baseHeight = base.Height;
				if (splitter.Height != baseHeight) {
					string msg = "splitter.Height BOTH_WIDTH_AND_HEIGHT_RESIZE I_DONT_KNOW_WHERE_TO_PUT_THIS_LOGIC";
					Assembler.PopupException(msg, null, false);
					splitter.Height = baseHeight;			// happily resized and repainted
					thingsIchanged++;
				}
				if (panel.Height != baseHeight) {
					string msg = "panel.Height BOTH_WIDTH_AND_HEIGHT_RESIZE I_DONT_KNOW_WHERE_TO_PUT_THIS_LOGIC";
					Assembler.PopupException(msg, null, false);

					//PanelBase panelBase = panel as PanelBase;
					//MultiSplitContainer panelAsMultiSplitContainer = panel as MultiSplitContainer;
					//if (panelBase != null) {
					//    heightChanged = panelBase.SetHeightIgnoreResize(baseHeight);
					//} else if (panelAsMultiSplitContainer != null) {
					//    heightChanged = panelAsMultiSplitContainer.SetHeightIgnoreResize(baseHeight);
					//} else {
						panel.Height  = baseHeight;
						heightChanged = true;
					//}
				}
				if (heightChanged) thingsIchanged++;
				if (panel.Width != newWidth) {
					string msg = "TO_UNDELAY_SIZE_PROPAGATION_INVOKE_base.SuspendLayout()_UPSTACK panel.Width[" + panel.Width + "] != newWidth[" + newWidth + "]";
					Assembler.PopupException(msg);
				}

				xCheckDiff += splitter.Width;
				xCheckDiff += panel.Width;
			}


			#if DEBUG		// TESTS_EMBEDDED
			int roundingError = Math.Abs(xCheckDiff - base.Width);
			if (roundingError > 10) {
				//USER_LEFT_WHOLE_CHART_CONTROL_TOO_NARROW_BEFORE_RESTART Debugger.Break();	// LOWER_PANEL_GETS_CUT_BY_HSCROLLBAR
			}
			if (thingsIchanged == 0) {
				string msg = "I_WAS_USELESS__REMOVE_MY_INVOCATION_FROM_UPSTACK //propagateColumns()";
				Assembler.PopupException(msg, null, false);
			}
			//if (thingsIchanged % 2 != 1) {
			//    string msg = "I_CHANGE_TWO_HEIGHTS_AND_ONE_LOCATION_EACH_CHART_CONTRO";
			//    //Assembler.PopupException(msg, null, false);
			//}
			#endif
			return thingsIchanged;
		}

		int propagateRows(Dictionary<string, MultiSplitterProperties> splitterPropertiesByPanelName) {
			int thingsIchanged = 0;
			int yCheckDiff = 0;
			for (int i=0; i<this.splitters.Count; i++) {
				PANEL_BASE panel = this.panels[i];
				MultiSplitter splitter = this.splitters[i];

				string panelName = panel.Name;
				MultiSplitterProperties props = splitterPropertiesByPanelName[panelName];

				if (splitter.Height != this.SplitterHeight) {
					string msg = "MUST_BE_INITIALIZED_EARLIER splitter[" + splitter.Name + "].Height[" + splitter.Height+ "] => [" + this.SplitterHeight + "]";
					Assembler.PopupException(msg);
					splitter.Height  = this.SplitterHeight;
					thingsIchanged++;
				}

				if (splitter.Location.Y != props.Distance) {
					splitter.Location = new Point(0, props.Distance);
					thingsIchanged++;
				}

				int panelY = splitter.Location.Y + splitter.Height;
				if (panel.Location.Y != panelY) {
					panel.Location = new Point(0, panelY);
					thingsIchanged++;
				}

				bool heightChanged = false;
				int newHeight = props.PanelHeight;
				if (panel.Height != newHeight) {
					//PanelBase panelBase = panel as PanelBase;
					//MultiSplitContainer panelAsMultiSplitContainer = panel as MultiSplitContainer;
					//if (panelBase != null) {
					//    heightChanged = panelBase.SetHeightIgnoreResize(newHeight);
					//} else if (panelAsMultiSplitContainer != null) {
					//    heightChanged = panelAsMultiSplitContainer.SetHeightIgnoreResize(newHeight);
					//} else {
						panel.Height  = props.PanelHeight;
						heightChanged = true;
					//}
				}
				if (heightChanged) thingsIchanged++;
				if (panel.Height != newHeight) {
					string msg = "TO_UNDELAY_SIZE_PROPAGATION_INVOKE_base.SuspendLayout()_UPSTACK panel.Height[" + panel.Height + "] != newHeight[" + newHeight+ "]";
					Assembler.PopupException(msg);
				}


				bool widthChanged = false;
		   		int baseWidth = base.Width;
				if (splitter.Width != baseWidth) {
					string msg = "splitter.Width BOTH_WIDTH_AND_HEIGHT_RESIZE I_DONT_KNOW_WHERE_TO_PUT_THIS_LOGIC";
					Assembler.PopupException(msg, null, false);
					splitter.Width = baseWidth;			// happily resized and repainted
					thingsIchanged++;
				}
				if (panel.Width != baseWidth) {
					string msg = "panel.Width BOTH_WIDTH_AND_HEIGHT_RESIZE I_DONT_KNOW_WHERE_TO_PUT_THIS_LOGIC";
					Assembler.PopupException(msg, null, false);

					//PanelBase panelBase = panel as PanelBase;
					//MultiSplitContainer panelAsMultiSplitContainer = panel as MultiSplitContainer;
					//if (panelBase != null) {
					//    widthChanged = panelBase.SetWidthIgnoreResize(baseWidth);
					//} else if (panelAsMultiSplitContainer != null) {
					//    widthChanged = panelAsMultiSplitContainer.SetWidthIgnoreResize(baseWidth);
					//} else {
						panel.Width  = baseWidth;
						widthChanged = true;
					//}
				}
				if (widthChanged) thingsIchanged++;
				if (panel.Width != baseWidth) {
					string msg = "TO_UNDELAY_SIZE_PROPAGATION_INVOKE_base.SuspendLayout()_UPSTACK panel.Width[" + panel.Width + "] != baseWidth[" + baseWidth + "]";
					Assembler.PopupException(msg);
				}

				yCheckDiff += splitter.Height;
				yCheckDiff += panel.Height;
			}


			#if DEBUG		// TESTS_EMBEDDED
			int roundingError = Math.Abs(yCheckDiff - base.Height);
			if (roundingError > 10) {
				//USER_LEFT_WHOLE_CHART_CONTROL_TOO_NARROW_BEFORE_RESTART Debugger.Break();	// LOWER_PANEL_GETS_CUT_BY_HSCROLLBAR
			}
			if (thingsIchanged == 0) {
				string msg = "I_WAS_USELESS__REMOVE_MY_INVOCATION_FROM_UPSTACK //propagateRows()";
				Assembler.PopupException(msg, null, false);
			}
			//if (thingsIchanged % 2 != 1) {
			//    string msg = "I_CHANGE_TWO_HEIGHTS_AND_ONE_LOCATION_EACH_CHART_CONTRO";
			//    //Assembler.PopupException(msg, null, false);
			//}
			#endif
			return thingsIchanged;
		}
	}
}
