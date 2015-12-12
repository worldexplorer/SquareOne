using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using Sq1.Core;

namespace Sq1.Charting.Demo {
	public partial class PanelsTest : Form {
		public PanelsTest() {
			InitializeComponent();
			base.HScroll = true;
			base.VScroll = true;
			
			ExceptionsForm exceptionsForm = new ExceptionsForm();
			exceptionsForm.Show();
			Assembler.InstanceUninitialized.Initialize(exceptionsForm, true);
			Assembler.InstanceInitialized.MainFormDockFormsFullyDeserializedLayoutComplete = true;
			//exceptionsForm.ExceptionControl.FlushExceptionsToOLVIfDockContentDeserialized_inGuiThread();

			base.SuspendLayout();

			this.multiSplitVertical.DebugSplitter = true;
			this.multiSplitHorizontal.DebugSplitter = true;

			List<Control> twoColumns = new List<Control>() {
				this.panelLevel2,
				this.multiSplitHorizontal
			};
			this.multiSplitVertical.VerticalizeAllLogic = true;
			this.multiSplitVertical.Dock = DockStyle.Fill;		// invokes Resize() that will SET the size of inner controls
			this.multiSplitVertical.InitializeCreateSplittersDistributeFor(twoColumns);

	
			List<Control> twoRows = new List<Control>() {
				this.panelVolume,
				this.panelPrice
			};
			this.multiSplitHorizontal.InitializeCreateSplittersDistributeFor(twoRows);
			
			base.ResumeLayout();
			//v1
			this.multiSplitHorizontal.PerformLayout();	// I hope it will invoke Resize(), not overridden in MSContainer
			//base.ResumeLayout(true);

			// NO_THIS_IS_ESSENTIAL LAYOUT_MANAGER_IS_OUT_OF_POWER
		}

	}
}
