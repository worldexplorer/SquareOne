/*
 * Created by SharpDevelop.
 * User: worldexplorer
 * Date: 14/03/2015
 * Time: 12:33 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace Sq1.Charting.Demo
{
	partial class Panels
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.tooltipPrice1 = new Sq1.Charting.TooltipPrice();
			this.panelLevel21 = new Sq1.Charting.PanelLevel2();
			this.panelPrice1 = new Sq1.Charting.PanelPrice();
			this.panelVolume1 = new Sq1.Charting.PanelVolume();
			this.multiSplitContainerOfPanelBase1 = new Sq1.Charting.MultiSplit.MultiSplitContainer();
			this.multiSplitContainerOfPanelBase2 = new Sq1.Charting.MultiSplit.MultiSplitContainer();
			this.SuspendLayout();
			// 
			// tooltipPrice1
			// 
			this.tooltipPrice1.BackColor = System.Drawing.SystemColors.Info;
			this.tooltipPrice1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tooltipPrice1.ForeColor = System.Drawing.SystemColors.InfoText;
			this.tooltipPrice1.Location = new System.Drawing.Point(0, 0);
			this.tooltipPrice1.Name = "TooltipPrice";
			this.tooltipPrice1.Size = new System.Drawing.Size(116, 116);
			this.tooltipPrice1.TabIndex = 0;
			// 
			// panelLevel21
			// 
			this.panelLevel21.BarsIdent = "UNINITIALIZED_BARS_IDENT_PanelNamedFolding";
			this.panelLevel21.GutterBottomDraw = false;
			this.panelLevel21.GutterRightDraw = true;
			this.panelLevel21.Location = new System.Drawing.Point(157, 29);
			this.panelLevel21.Name = "panelLevel21";
			this.panelLevel21.PanelName = "UNINITIALIZED_PANEL_NAME_PanelNamedFolding";
			this.panelLevel21.Size = new System.Drawing.Size(200, 100);
			this.panelLevel21.TabIndex = 0;
			// 
			// panelPrice1
			// 
			this.panelPrice1.BarsIdent = "UNINITIALIZED_BARS_IDENT_PanelNamedFolding";
			this.panelPrice1.GutterBottomDraw = false;
			this.panelPrice1.GutterRightDraw = true;
			this.panelPrice1.Location = new System.Drawing.Point(351, 26);
			this.panelPrice1.MinimumSize = new System.Drawing.Size(20, 25);
			this.panelPrice1.Name = "panelPrice1";
			this.panelPrice1.PanelName = "UNINITIALIZED_PANEL_NAME_PanelNamedFolding";
			this.panelPrice1.Size = new System.Drawing.Size(200, 100);
			this.panelPrice1.TabIndex = 1;
			// 
			// panelVolume1
			// 
			this.panelVolume1.BarsIdent = "UNINITIALIZED_BARS_IDENT_PanelNamedFolding";
			this.panelVolume1.GutterBottomDraw = false;
			this.panelVolume1.GutterRightDraw = true;
			this.panelVolume1.Location = new System.Drawing.Point(327, 145);
			this.panelVolume1.MinimumSize = new System.Drawing.Size(20, 15);
			this.panelVolume1.Name = "panelVolume1";
			this.panelVolume1.PanelName = "UNINITIALIZED_PANEL_NAME_PanelNamedFolding";
			this.panelVolume1.Size = new System.Drawing.Size(200, 100);
			this.panelVolume1.TabIndex = 2;
			// 
			// multiSplitContainerOfPanelBase1
			// 
			this.multiSplitContainerOfPanelBase1.BackColor = System.Drawing.SystemColors.ControlDark;
			this.multiSplitContainerOfPanelBase1.Location = new System.Drawing.Point(40, 13);
			this.multiSplitContainerOfPanelBase1.Name = "multiSplitContainerOfPanelBase1";
			this.multiSplitContainerOfPanelBase1.Size = new System.Drawing.Size(150, 150);
			this.multiSplitContainerOfPanelBase1.TabIndex = 3;
			// 
			// multiSplitContainerOfPanelBase2
			// 
			this.multiSplitContainerOfPanelBase2.BackColor = System.Drawing.SystemColors.ControlDark;
			this.multiSplitContainerOfPanelBase2.Location = new System.Drawing.Point(139, 104);
			this.multiSplitContainerOfPanelBase2.Name = "multiSplitContainerOfPanelBase2";
			this.multiSplitContainerOfPanelBase2.Size = new System.Drawing.Size(150, 150);
			this.multiSplitContainerOfPanelBase2.TabIndex = 4;
			// 
			// Panels
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(620, 332);
			this.Controls.Add(this.multiSplitContainerOfPanelBase2);
			this.Controls.Add(this.multiSplitContainerOfPanelBase1);
			this.Controls.Add(this.panelVolume1);
			this.Controls.Add(this.panelPrice1);
			this.Controls.Add(this.panelLevel21);
			this.Name = "Panels";
			this.Text = "Panels";
			this.ResumeLayout(false);
		}
		private Sq1.Charting.MultiSplit.MultiSplitContainer multiSplitContainerOfPanelBase2;
		private Sq1.Charting.MultiSplit.MultiSplitContainer multiSplitContainerOfPanelBase1;
		private Sq1.Charting.PanelVolume panelVolume1;
		private Sq1.Charting.PanelPrice panelPrice1;
		private Sq1.Charting.PanelLevel2 panelLevel21;
		private Sq1.Charting.TooltipPrice tooltipPrice1;
	}
}
