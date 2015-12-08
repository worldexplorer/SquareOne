using System;
using System.Windows.Forms;
using System.Collections.Generic;

using Sq1.Core;
using Sq1.Core.Support;

namespace Sq1.Charting.Demo {
	public partial class MultiSplitTest : Form {
		public MultiSplitTest() {
			InitializeComponent();
			base.HScroll = true;
			base.VScroll = true;
			
			// ChartControl, Panels and MultiSplitter all throw exceptions using Assembler.Instance 
			//v1 Assembler.InstanceUninitialized.Initialize(this);
			//v2 employed Sq1.Gui.ExceptionsForm, non-singletonized copy here in Sq1.Charting (LOTS OF EXCEPTIONS ABOUT REPOSITORIES DESERIALIZATION) 
			ExceptionsForm exceptionsForm = new ExceptionsForm();
			exceptionsForm.Show();
			Assembler.InstanceUninitialized.Initialize(exceptionsForm, true);
			Assembler.InstanceInitialized.MainFormDockFormsFullyDeserializedLayoutComplete = true;
			//exceptionsForm.ExceptionControl.FlushExceptionsToOLVIfDockContentDeserialized_inGuiThread();

			this.multiSplitContainerOfPanelBase1.DebugSplitter = true;
			this.multiSplitContainerOfPanelBase1.SplitterHeight = 20;
			this.multiSplitContainerOfPanelBase1.VerticalizeAllLogic = true;
			this.multiSplitContainerOfPanelBase1.InitializeCreateSplittersDistributeFor(
				new List<Control>() {
					this.panel4,
					this.panel5,
					this.panel6,
				});

			this.multiSplitContainer1.DebugSplitter = true;
			this.multiSplitContainer1.SplitterHeight = 20;
			this.multiSplitContainer1.InitializeCreateSplittersDistributeFor(
				new List<Control>() {
					this.panel1,
					this.multiSplitContainerOfPanelBase1,
					this.panel2,
					this.panel3,
				});

		}

	}
}
