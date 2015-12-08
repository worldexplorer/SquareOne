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

			List<Control> twoRows = new List<Control>() {
				this.panelVolume,
				this.panelPrice
			};
			this.multiSplitHorizontal.InitializeCreateSplittersDistributeFor(twoRows);
			this.multiSplitHorizontal.PerformLayout();	// I hope it will invoke Resize(), not overridden in MSContainer

			List<Control> twoColumns = new List<Control>() {
				this.panelLevel2,
				this.multiSplitHorizontal
			};
			this.multiSplitVertical.VerticalizeAllLogic = true;
			this.multiSplitVertical.InitializeCreateSplittersDistributeFor(twoColumns);
			this.multiSplitVertical.Dock = DockStyle.Fill;		// invokes Resize() that will SET the size of inner controls
		}

	}
}
