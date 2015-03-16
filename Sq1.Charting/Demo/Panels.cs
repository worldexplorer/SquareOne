/*
 * Created by SharpDevelop.
 * User: worldexplorer
 * Date: 14/03/2015
 * Time: 12:33 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using Sq1.Core;

namespace Sq1.Charting.Demo
{
	/// <summary>
	/// Description of Panels.
	/// </summary>
	public partial class Panels : Form
	{
		public Panels()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
			
			
			ExceptionsForm exceptionsForm = new ExceptionsForm();
			exceptionsForm.Show();
			Assembler.InstanceUninitialized.Initialize(exceptionsForm);
			Assembler.InstanceInitialized.MainFormDockFormsFullyDeserializedLayoutComplete = true;
			exceptionsForm.ExceptionControl.FlushListToTreeIfDockContentDeserialized_inGuiThread();


			List<Control> twoColumns = new List<Control>() {
				this.panelLevel21,
				this.panelPrice1
			};
			this.multiSplitContainerOfPanelBase1.VerticalizeAllLogic = true;
			this.multiSplitContainerOfPanelBase1.InitializeCreateSplittersDistributeFor(twoColumns);


			List<Control> twoRows = new List<Control>() {
				this.panelVolume1,
				this.multiSplitContainerOfPanelBase1
			};
			this.multiSplitContainerOfPanelBase2.InitializeCreateSplittersDistributeFor(twoRows);
			this.multiSplitContainerOfPanelBase2.Dock = DockStyle.Fill;
		}

	}
}
