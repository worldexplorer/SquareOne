using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using Sq1.Core;

namespace Sq1.Charting.Demo {
	public partial class TestPanels : Form {
		public TestPanels() {
			InitializeComponent();
			base.HScroll = true;
			base.VScroll = true;
			
			ExceptionsForm exceptionsForm = new ExceptionsForm();
			exceptionsForm.Show();
			Assembler.InstanceUninitialized.Initialize(exceptionsForm, true);
			Assembler.InstanceInitialized.MainForm_dockFormsFullyDeserialized_layoutComplete = true;
			//exceptionsForm.ExceptionControl.FlushExceptionsToOLVIfDockContentDeserialized_inGuiThread();


			base.SuspendLayout();

			this.multiSplitColumns_Level2_AnotherMultisplit.DebugSplitter = true;
			this.multiSplitRowsVolumePrice.DebugSplitter = true;

			List<Control> twoColumns = new List<Control>() {
				this.panelLevel2,
				this.multiSplitRowsVolumePrice
			};
			this.multiSplitColumns_Level2_AnotherMultisplit.VerticalizeAllLogic = true;
			this.multiSplitColumns_Level2_AnotherMultisplit.Dock = DockStyle.Fill;		// invokes Resize() that will SET the size of inner controls
			this.multiSplitColumns_Level2_AnotherMultisplit.InitializeCreateSplittersDistributeFor(twoColumns);

	
			List<Control> twoRows = new List<Control>() {
				this.panelVolume,
				this.panelPrice
			};
			this.multiSplitRowsVolumePrice.InitializeCreateSplittersDistributeFor(twoRows);
			
			//v1
			//base.ResumeLayout();
			//this.multiSplitHorizontal.PerformLayout();	// I hope it will invoke Resize(), not overridden in MSContainer
			base.ResumeLayout(true);

			//Assembler.InstanceInitialized.MainFormDockFormsFullyDeserializedLayoutComplete = true;
		}

	}
}
