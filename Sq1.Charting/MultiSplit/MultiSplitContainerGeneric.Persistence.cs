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
				int distance = this.VerticalizeAllLogic == false ? s.Location.Y : s.Location.X;
				ret.Add(s.PanelBelow.Name, new MultiSplitterProperties(manualOrder, distance));
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
	   		int baseHeight = base.Height;
	   		if (this.VerticalizeAllLogic) baseHeight = base.Width; 
try {
			foreach (string panelName in splitterPropertiesByPanelName.Keys) {
				MultiSplitterProperties prop = splitterPropertiesByPanelName[panelName];
				if (prop.ManualOrder < 0 && prop.ManualOrder >= this.splitters.Count) {
					string msg = "SPLITTER_SKIPPED_CANT_MOVE_NO_DESTINATION_WHERE_TO panelName[" + panelName + "]";
					Assembler.PopupException(msg);
					continue;
				}
				if (prop.ManualOrder < 0 && prop.ManualOrder >= this.panels.Count) {
					string msg = "SPLITTER_SKIPPED_CANT_MOVE_PANEL_NO_DESTINATION_WHERE_TO panelName[" + panelName + "]";
					Assembler.PopupException(msg);
					continue;
				}

				MultiSplitter splitterFound = null;
				foreach (MultiSplitter each in this.splitters) {
					if (each.PanelBelow == null) continue;
					if (each.PanelBelow.Name != panelName) continue;
					splitterFound = each;
					break;
				}
				if (splitterFound == null) {
					string msg = "SPLITTER_NOT_FOUND_WHILE_SETTING_MANORDER_DISTANCE panelName[" + panelName + "]";
					//LET_IT_SPARSELY_ASSIGN_PANEL_HEIGHTS__OR_RECONSTRUCT_PANELS_HERE_FORCIBLY? Assembler.PopupException(msg);
					continue;
				}
				//if (this.VerticalizeAllLogic) {
				//	Debugger.Break();
				//}
				splitterFound.Location = this.VerticalizeAllLogic == false
					? new Point(splitterFound.Location.X, prop.Distance)
					: new Point(prop.Distance, splitterFound.Location.Y);
				
				int splitterFoundIndex = this.splitters.IndexOf(splitterFound);
				if (splitterFoundIndex == prop.ManualOrder) {
					continue;
				}
				try {
					this.panels.Move(splitterFoundIndex, prop.ManualOrder);
					this.splitters.Move(splitterFoundIndex, prop.ManualOrder);
				} catch (Exception ex) {
					string msg = "very illogical way to sync-up; splitters may have holes and implies MultiSplitterPropertiesByPanelName.*.ManualOrder must have no holes/duplicates";
					Assembler.PopupException(msg, ex, false);
				}
			}
			
			// align panels to splitters; I need to know the prevSplitterLocationY to set panelHeight
			int yCheckDiff = 0;
			for (int i=this.splitters.Count-1; i>=0; i--) {
				PANEL_BASE panel = this.panels[i];
				MultiSplitter splitter = this.splitters[i];
				var panelControl = panel as Control;

				if (this.VerticalizeAllLogic == false) {
					int minimumPanelHeight = this.MinimumPanelHeight;
					if (panelControl != null) {
						minimumPanelHeight = panelControl.MinimumSize.Height;
					}
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
						if (panelHeight < minimumPanelHeight) {
							panelHeight = minimumPanelHeight;
						}
					} else {
						MultiSplitter lowerSplitter = this.splitters[i+1];		//prevSplitterLocationY
						panelHeight = lowerSplitter.Location.Y - panelY;  
						if (panelHeight < minimumPanelHeight) {
							panelHeight = minimumPanelHeight;
						}
					}
	
					//panelHeight -= splitter.Height + 40;	// LOWER_PANEL_GETS_CUT_BY_HSCROLLBAR
					if (panel.Height != panelHeight) {
						panel.Height = panelHeight;
					}
					yCheckDiff += splitter.Height;
					yCheckDiff += panel.Height;
				} else {
					int minimumPanelWidth = this.MinimumPanelHeight;
					if (panelControl != null) {
						minimumPanelWidth = panelControl.MinimumSize.Width;
					}
					int panelX = splitter.Location.X + splitter.Width;
					if (panel.Location.X != panelX) {
						panel.Location = new Point(panelX, panel.Location.Y);
					}
					int panelWidth = -1;
					if (i == this.splitters.Count-1) {
						panelWidth = baseHeight - panelX;
						if (panelWidth < 0) {
							#if DEBUG
							//POSTPONE_WINDOWS_MAGIC Debugger.Break();	// AFTERDRAG_MANUAL_ORDER_WASNT_SYNCED YOUVE_HAD_MAXIMIZED_OK_BUT_NORMAL_WINDOW_SIZE_MADE_THE_PANEL_TOO_SMALL MAYBE_YOU_CAN_COLLAPSE_EXCEPTIONS_PANEL_TO_SEE_VOLUME?
							#endif
							return;
						}
						if (panelWidth < minimumPanelWidth) {
							panelWidth = minimumPanelWidth;
						}
					} else {
						MultiSplitter rightSplitter = this.splitters[i+1];		//prevSplitterLocationY
						panelWidth = rightSplitter.Location.X - panelX;  
						if (panelWidth < minimumPanelWidth) {
							panelWidth = minimumPanelWidth;
						}
					}
	
					//panelHeight -= splitter.Height + 40;	// LOWER_PANEL_GETS_CUT_BY_HSCROLLBAR
					if (panel.Width != panelWidth) {
						panel.Width = panelWidth;
					}
					yCheckDiff += splitter.Width;
					yCheckDiff += panel.Width;
				}
			}

			#if DEBUG		// TESTS_EMBEDDED
			if (this.VerticalizeAllLogic == false) {
				if (base.Height != baseHeight) {
					Debugger.Break();
				}
				baseHeight = base.Height;
				int roundingError = Math.Abs(yCheckDiff - baseHeight);
				if (roundingError > 10) {
					//USER_LEFT_WHOLE_CHART_CONTROL_TOO_NARROW_BEFORE_RESTART Debugger.Break();	// LOWER_PANEL_GETS_CUT_BY_HSCROLLBAR
				}
			} else {
				if (base.Width != baseHeight) {
					Debugger.Break();
				}
				baseHeight = base.Width;
				int roundingError = Math.Abs(yCheckDiff - baseHeight);
				if (roundingError > 10) {
					//USER_LEFT_WHOLE_CHART_CONTROL_TOO_NARROW_BEFORE_RESTART Debugger.Break();	// LOWER_PANEL_GETS_CUT_BY_HSCROLLBAR
				}
			}
			#endif
			
} catch (Exception ex) {
	string msg = "YOU_GOT_PANES_DECLARED_FOR_NON-YET_INSTANTIATED_INDICATORS MOVING_PANES_ON_DESERIALIZATION_TO_RESTORE_LAYOUT_NYI";
	Assembler.PopupException(msg, ex, false);
}
		}
	}
}
