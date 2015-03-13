/*
 * Created by SharpDevelop.
 * User: worldexplorer
 * Date: 29-Sep-14
 * Time: 8:39 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Sq1.Charting.MultiSplit
{
	/// <summary>
	/// Description of MultiSplitTest.
	/// </summary>
	public partial class MultiSplitTest : Form
	{
		public MultiSplitTest()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
			this.multiSplitContainer1.DebugSplitter = true;
			this.multiSplitContainer1.SplitterHeight = 20;
			this.multiSplitContainer1.InitializeCreateSplittersDistributeFor(
				new List<Panel>() {
					this.panel1,
					this.panel2,
					this.panel3,
					this.panel4,
				});
		}
	}
}
