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
	partial class PanelsTest
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
			this.multiSplitHorizontal = new Sq1.Charting.MultiSplit.MultiSplitContainer();
			this.multiSplitVertical = new Sq1.Charting.MultiSplit.MultiSplitContainer();
			this.panelVolume = new Sq1.Charting.PanelVolume();
			this.panelPrice = new Sq1.Charting.PanelPrice();
			this.panelLevel2 = new Sq1.Charting.PanelLevel2();
			this.tooltipPrice1 = new Sq1.Charting.TooltipPrice();
			this.SuspendLayout();
			// 
			// multiSplitHorizontal
			// 
			this.multiSplitHorizontal.BackColor = System.Drawing.SystemColors.ControlDark;
			this.multiSplitHorizontal.Location = new System.Drawing.Point(71, 50);
			this.multiSplitHorizontal.Name = "multiSplitHorizontal";
			this.multiSplitHorizontal.Size = new System.Drawing.Size(150, 150);
			this.multiSplitHorizontal.TabIndex = 4;
			// 
			// multiSplitVertical
			// 
			this.multiSplitVertical.BackColor = System.Drawing.SystemColors.ControlDark;
			this.multiSplitVertical.Location = new System.Drawing.Point(40, 13);
			this.multiSplitVertical.Name = "multiSplitVertical";
			this.multiSplitVertical.Size = new System.Drawing.Size(150, 150);
			this.multiSplitVertical.TabIndex = 3;
			// 
			// panelVolume
			// 
			this.panelVolume.AutoScroll = true;
			this.panelVolume.AutoSize = true;
			this.panelVolume.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.panelVolume.BackColor = System.Drawing.SystemColors.ControlDark;
			this.panelVolume.BarsIdent = "UNINITIALIZED_BARS_IDENT_PanelNamedFolding";
			this.panelVolume.GutterBottomDraw = false;
			this.panelVolume.GutterRightDraw = true;
			this.panelVolume.Location = new System.Drawing.Point(381, 81);
			this.panelVolume.MinimumSize = new System.Drawing.Size(25, 25);
			this.panelVolume.Name = "panelVolume";
			this.panelVolume.PanelName = "UNINITIALIZED_PANEL_NAME_PanelNamedFolding";
			this.panelVolume.Size = new System.Drawing.Size(25, 25);
			this.panelVolume.TabIndex = 2;
			// 
			// panelPrice
			// 
			this.panelPrice.AutoScroll = true;
			this.panelPrice.AutoSize = true;
			this.panelPrice.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.panelPrice.BackColor = System.Drawing.SystemColors.ControlDark;
			this.panelPrice.BarsIdent = "UNINITIALIZED_BARS_IDENT_PanelNamedFolding";
			this.panelPrice.GutterBottomDraw = false;
			this.panelPrice.GutterRightDraw = true;
			this.panelPrice.Location = new System.Drawing.Point(381, 50);
			this.panelPrice.MinimumSize = new System.Drawing.Size(25, 25);
			this.panelPrice.Name = "panelPrice";
			this.panelPrice.PanelName = "UNINITIALIZED_PANEL_NAME_PanelNamedFolding";
			this.panelPrice.Size = new System.Drawing.Size(25, 25);
			this.panelPrice.TabIndex = 1;
			// 
			// panelLevel2
			// 
			this.panelLevel2.AutoScroll = true;
			this.panelLevel2.AutoSize = true;
			this.panelLevel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.panelLevel2.BackColor = System.Drawing.SystemColors.ControlDark;
			this.panelLevel2.BarsIdent = "UNINITIALIZED_BARS_IDENT_PanelNamedFolding";
			this.panelLevel2.GutterBottomDraw = false;
			this.panelLevel2.GutterRightDraw = true;
			this.panelLevel2.Location = new System.Drawing.Point(325, 50);
			this.panelLevel2.MinimumSize = new System.Drawing.Size(20, 60);
			this.panelLevel2.Name = "panelLevel2";
			this.panelLevel2.PanelName = "UNINITIALIZED_PANEL_NAME_PanelNamedFolding";
			this.panelLevel2.Size = new System.Drawing.Size(20, 60);
			this.panelLevel2.TabIndex = 0;
			// 
			// tooltipPrice1
			// 
			this.tooltipPrice1.BackColor = System.Drawing.SystemColors.Info;
			this.tooltipPrice1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tooltipPrice1.ForeColor = System.Drawing.SystemColors.InfoText;
			this.tooltipPrice1.Location = new System.Drawing.Point(0, 0);
			this.tooltipPrice1.Name = "tooltipPrice1";
			this.tooltipPrice1.Size = new System.Drawing.Size(116, 116);
			this.tooltipPrice1.TabIndex = 0;
			// 
			// PanelsTest
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(620, 332);
			this.Controls.Add(this.multiSplitHorizontal);
			this.Controls.Add(this.multiSplitVertical);
			this.Controls.Add(this.panelVolume);
			this.Controls.Add(this.panelPrice);
			this.Controls.Add(this.panelLevel2);
			this.Name = "PanelsTest";
			this.Text = "Panels";
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		private Sq1.Charting.MultiSplit.MultiSplitContainer multiSplitHorizontal;
		private Sq1.Charting.MultiSplit.MultiSplitContainer multiSplitVertical;
		private Sq1.Charting.PanelVolume panelVolume;
		private Sq1.Charting.PanelPrice panelPrice;
		private Sq1.Charting.PanelLevel2 panelLevel2;
		private Sq1.Charting.TooltipPrice tooltipPrice1;
	}
}
