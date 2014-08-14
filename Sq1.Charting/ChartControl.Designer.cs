/*
 * Created by SharpDevelop.
 * User: worldexplorer
 * Date: 01-Jul-14
 * Time: 3:24 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace Sq1.Charting
{
	partial class ChartControl
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
				this.barEventsDetach();
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
			this.hScrollBar = new System.Windows.Forms.HScrollBar();
			this.splitContainerChartVsRange = new System.Windows.Forms.SplitContainer();
			this.splitContainerPriceVsVolume = new System.Windows.Forms.SplitContainer();
			this.panelPrice = new Sq1.Charting.PanelPrice();
			this.panelVolume = new Sq1.Charting.PanelVolume();
			this.RangeBar = new Sq1.Widgets.RangeBar.RangeBarDateTime();
			this.tooltipPosition = new Sq1.Charting.TooltipPosition();
			this.tooltipPrice = new Sq1.Charting.TooltipPrice();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerChartVsRange)).BeginInit();
			this.splitContainerChartVsRange.Panel1.SuspendLayout();
			this.splitContainerChartVsRange.Panel2.SuspendLayout();
			this.splitContainerChartVsRange.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerPriceVsVolume)).BeginInit();
			this.splitContainerPriceVsVolume.Panel1.SuspendLayout();
			this.splitContainerPriceVsVolume.Panel2.SuspendLayout();
			this.splitContainerPriceVsVolume.SuspendLayout();
			this.SuspendLayout();
			// 
			// hScrollBar
			// 
			this.hScrollBar.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.hScrollBar.Location = new System.Drawing.Point(0, 346);
			this.hScrollBar.Name = "hScrollBar";
			this.hScrollBar.Size = new System.Drawing.Size(821, 17);
			this.hScrollBar.TabIndex = 0;
			this.hScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.hScrollBar_Scroll);
			// 
			// splitContainerChartVsRange
			// 
			this.splitContainerChartVsRange.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.splitContainerChartVsRange.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.splitContainerChartVsRange.Location = new System.Drawing.Point(0, 0);
			this.splitContainerChartVsRange.Name = "splitContainerChartVsRange";
			this.splitContainerChartVsRange.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainerChartVsRange.Panel1
			// 
			this.splitContainerChartVsRange.Panel1.Controls.Add(this.splitContainerPriceVsVolume);
			// 
			// splitContainerChartVsRange.Panel2
			// 
			this.splitContainerChartVsRange.Panel2.Controls.Add(this.RangeBar);
			this.splitContainerChartVsRange.Size = new System.Drawing.Size(821, 346);
			this.splitContainerChartVsRange.SplitterDistance = 289;
			this.splitContainerChartVsRange.TabIndex = 0;
			this.splitContainerChartVsRange.TabStop = false;
			// 
			// splitContainerPriceVsVolume
			// 
			this.splitContainerPriceVsVolume.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainerPriceVsVolume.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.splitContainerPriceVsVolume.Location = new System.Drawing.Point(0, 0);
			this.splitContainerPriceVsVolume.Name = "splitContainerPriceVsVolume";
			this.splitContainerPriceVsVolume.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainerPriceVsVolume.Panel1
			// 
			this.splitContainerPriceVsVolume.Panel1.Controls.Add(this.panelPrice);
			this.splitContainerPriceVsVolume.Panel1MinSize = 5;
			// 
			// splitContainerPriceVsVolume.Panel2
			// 
			this.splitContainerPriceVsVolume.Panel2.BackColor = System.Drawing.SystemColors.Window;
			this.splitContainerPriceVsVolume.Panel2.Controls.Add(this.panelVolume);
			this.splitContainerPriceVsVolume.Panel2MinSize = 5;
			this.splitContainerPriceVsVolume.Size = new System.Drawing.Size(821, 289);
			this.splitContainerPriceVsVolume.SplitterDistance = 240;
			this.splitContainerPriceVsVolume.TabIndex = 0;
			this.splitContainerPriceVsVolume.TabStop = false;
			this.splitContainerPriceVsVolume.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitContainerPriceVsVolume_SplitterMoved);
			// 
			// panelPrice
			// 
			this.panelPrice.BackColor = System.Drawing.SystemColors.Window;
			this.panelPrice.BarsIdent = "UNINITIALIZED_BARS_IDENT_PanelNamedFolding";
			this.panelPrice.CollapsedToName = false;
			this.panelPrice.Collapsible = true;
			this.panelPrice.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelPrice.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.panelPrice.GutterBottomDraw = true;
			this.panelPrice.GutterRightDraw = true;
			this.panelPrice.Location = new System.Drawing.Point(0, 0);
			this.panelPrice.Name = "panelPrice";
			this.panelPrice.PanelName = "Price";
			this.panelPrice.Size = new System.Drawing.Size(821, 240);
			this.panelPrice.TabIndex = 1;
			// 
			// panelVolume
			// 
			this.panelVolume.BarsIdent = "UNINITIALIZED_BARS_IDENT_PanelNamedFolding";
			this.panelVolume.CollapsedToName = false;
			this.panelVolume.Collapsible = true;
			this.panelVolume.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelVolume.Font = new System.Drawing.Font("Consolas", 8.25F);
			this.panelVolume.GutterBottomDraw = false;
			this.panelVolume.GutterRightDraw = true;
			this.panelVolume.Location = new System.Drawing.Point(0, 0);
			this.panelVolume.Name = "panelVolume";
			this.panelVolume.PanelName = "Volume";
			this.panelVolume.Size = new System.Drawing.Size(821, 45);
			this.panelVolume.TabIndex = 0;
			// 
			// RangeBar
			// 
			this.RangeBar.BackColor = System.Drawing.Color.AliceBlue;
			this.RangeBar.ColorBgOutsideMouseOver = System.Drawing.Color.LightBlue;
			this.RangeBar.ColorBgOutsideRange = System.Drawing.Color.LightSteelBlue;
			this.RangeBar.ColorFgGraph = System.Drawing.Color.DarkSalmon;
			this.RangeBar.Dock = System.Windows.Forms.DockStyle.Fill;
			this.RangeBar.ForeColor = System.Drawing.Color.Black;
			this.RangeBar.GraphMinHeightGoUnderLabels = 20F;
			this.RangeBar.GraphPenWidth = 1F;
			this.RangeBar.Location = new System.Drawing.Point(0, 0);
			this.RangeBar.Name = "RangeBar";
			this.RangeBar.RangeMax = new System.DateTime(2013, 5, 12, 0, 0, 0, 0);
			this.RangeBar.RangeMin = new System.DateTime(2010, 5, 12, 0, 0, 0, 0);
			this.RangeBar.RangeScaleLabelDistancePx = 0;
			this.RangeBar.ScalePenWidth = 1F;
			this.RangeBar.Size = new System.Drawing.Size(821, 53);
			this.RangeBar.TabIndex = 0;
			this.RangeBar.ValueFormat = "dd-MMM-yy";
			this.RangeBar.ValueMax = new System.DateTime(2012, 5, 12, 0, 0, 0, 0);
			this.RangeBar.ValueMin = new System.DateTime(2011, 5, 12, 0, 0, 0, 0);
			// 
			// tooltipPosition
			// 
			this.tooltipPosition.BackColor = System.Drawing.SystemColors.Info;
			this.tooltipPosition.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tooltipPosition.ForeColor = System.Drawing.SystemColors.InfoText;
			this.tooltipPosition.Location = new System.Drawing.Point(170, 3);
			this.tooltipPosition.Name = "tooltipPosition1";
			this.tooltipPosition.Size = new System.Drawing.Size(139, 215);
			this.tooltipPosition.TabIndex = 0;
			this.tooltipPosition.Visible = false;
			// 
			// tooltipPrice
			// 
			this.tooltipPrice.BackColor = System.Drawing.SystemColors.Info;
			this.tooltipPrice.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tooltipPrice.ForeColor = System.Drawing.SystemColors.InfoText;
			this.tooltipPrice.Location = new System.Drawing.Point(22, 77);
			this.tooltipPrice.Name = "tooltipPrice1";
			this.tooltipPrice.Size = new System.Drawing.Size(115, 116);
			this.tooltipPrice.TabIndex = 1;
			this.tooltipPrice.Visible = false;
			// 
			// ChartControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tooltipPrice);
			this.Controls.Add(this.tooltipPosition);
			this.Controls.Add(this.hScrollBar);
			this.Controls.Add(this.splitContainerChartVsRange);
			this.Name = "ChartControl";
			this.Size = new System.Drawing.Size(821, 363);
			this.splitContainerChartVsRange.Panel1.ResumeLayout(false);
			this.splitContainerChartVsRange.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainerChartVsRange)).EndInit();
			this.splitContainerChartVsRange.ResumeLayout(false);
			this.splitContainerPriceVsVolume.Panel1.ResumeLayout(false);
			this.splitContainerPriceVsVolume.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainerPriceVsVolume)).EndInit();
			this.splitContainerPriceVsVolume.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.HScrollBar hScrollBar;
		private PanelPrice panelPrice;
		private System.Windows.Forms.SplitContainer splitContainerChartVsRange;
		private System.Windows.Forms.SplitContainer splitContainerPriceVsVolume;
		public  Widgets.RangeBar.RangeBarDateTime RangeBar;
		private PanelVolume panelVolume;
		private Sq1.Charting.TooltipPrice tooltipPrice;
		private Sq1.Charting.TooltipPosition tooltipPosition;
	}
}