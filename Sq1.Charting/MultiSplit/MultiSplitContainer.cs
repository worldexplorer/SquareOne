using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;

using Sq1.Core;

namespace Sq1.Charting.MultiSplit {
	// REASON_TO_EXIST: VS2010 Designer stupidly says The type 'Sq1.Charting.MultiSplit.MultiSplitContainerGeneric' doesn't have a constructor (doesn't recognize generics). 
	//"Could not find type 'Sq1.Charting.MultiSplit.MultiSplitContainerGeneric'. Please make sure that the assembly that contains this type is referenced. If this type is a part of your development project, make sure that the project has been successfully built using settings for your current platform or Any CPU. "
	// SharpDevelop opens MultiSplitTest containing generics no problem!!! heyo MS...

	// ScrollableControl is the closest common ancestor for
	// 1) PanelBase < PanelDoubleBuffered < ScrollableControl < Control
	// 2) MultiSplitContainer < UserControl (not DoubleBuffered to paint splitted into TargetDropRedColor FIXME) < ContainerControl < ScrollableControl  
	// but I'm using Control :) all I need is Control.Name to reliably (de)serialize distances for splitters (see MultiSplitContainerGeneric.Persistence.cs)
	public partial class MultiSplitContainer : MultiSplitContainerGeneric<Control> {

		internal Point LocationOfInnerMultisplitContainer(MultiSplitContainer multiSplitContainer) {
			int x = -1;
			int y = -1;
			List<Control> mustContainMultiSplitContainer =  base.ControlsContained;
			if (mustContainMultiSplitContainer.Contains(multiSplitContainer) == false) {
				string msg = "WAS_NOT_ADDED_AS_INNER_MULTISPLITTER  multiSplitContainer[" + multiSplitContainer + "]";
				Assembler.PopupException(msg);
				return new Point(x, y);
			}
			return multiSplitContainer.Location;
		}
	}
}
