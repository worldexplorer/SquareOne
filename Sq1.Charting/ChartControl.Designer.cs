using System;

namespace Sq1.Charting {
	partial class ChartControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
				this.barEventsDetach();
			}
			base.Dispose(disposing);
		}
		
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChartControl));
			this.hScrollBar = new System.Windows.Forms.HScrollBar();
			this.splitContainerChartVsRange = new System.Windows.Forms.SplitContainer();
			this.tooltipPrice = new Sq1.Charting.TooltipPrice();
			this.panelLevel2 = new Sq1.Charting.PanelLevel2();
			this.tooltipPosition = new Sq1.Charting.TooltipPosition();
			this.multiSplitContainerColumns = new Sq1.Charting.MultiSplit.MultiSplitContainer();
			this.lblWinFormDesignerComment = new System.Windows.Forms.Label();
			this.multiSplitContainerRows = new Sq1.Charting.MultiSplit.MultiSplitContainer();
			this.panelVolume = new Sq1.Charting.PanelVolume();
			this.PanelPrice = new Sq1.Charting.PanelPrice();
			this.RangeBar = new Sq1.Widgets.RangeBar.RangeBarDateTime();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerChartVsRange)).BeginInit();
			this.splitContainerChartVsRange.Panel1.SuspendLayout();
			this.splitContainerChartVsRange.SuspendLayout();
			this.SuspendLayout();
			// 
			// hScrollBar
			// 
			this.hScrollBar.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.hScrollBar.Location = new System.Drawing.Point(0, 347);
			this.hScrollBar.Name = "hScrollBar";
			this.hScrollBar.Size = new System.Drawing.Size(811, 17);
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
			this.splitContainerChartVsRange.Panel1.Controls.Add(this.tooltipPrice);
			this.splitContainerChartVsRange.Panel1.Controls.Add(this.panelLevel2);
			this.splitContainerChartVsRange.Panel1.Controls.Add(this.tooltipPosition);
			this.splitContainerChartVsRange.Panel1.Controls.Add(this.multiSplitContainerColumns);
			this.splitContainerChartVsRange.Panel1.Controls.Add(this.lblWinFormDesignerComment);
			this.splitContainerChartVsRange.Panel1.Controls.Add(this.multiSplitContainerRows);
			this.splitContainerChartVsRange.Panel1.Controls.Add(this.panelVolume);
			this.splitContainerChartVsRange.Panel1.Controls.Add(this.PanelPrice);
			this.splitContainerChartVsRange.Size = new System.Drawing.Size(811, 347);
			this.splitContainerChartVsRange.SplitterDistance = 290;
			this.splitContainerChartVsRange.TabIndex = 0;
			this.splitContainerChartVsRange.TabStop = false;
			// 
			// tooltipPrice
			// 
			this.tooltipPrice.BackColor = System.Drawing.SystemColors.Info;
			this.tooltipPrice.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tooltipPrice.ForeColor = System.Drawing.SystemColors.InfoText;
			this.tooltipPrice.Location = new System.Drawing.Point(547, 171);
			this.tooltipPrice.Name = "tooltipPrice";
			this.tooltipPrice.Size = new System.Drawing.Size(116, 116);
			this.tooltipPrice.TabIndex = 1;
			this.tooltipPrice.Visible = false;
			// 
			// panelLevel2
			// 
			this.panelLevel2.BarsIdent = "UNINITIALIZED_BARS_IDENT_PanelNamedFolding";
			this.panelLevel2.GutterBottomDraw = false;
			this.panelLevel2.GutterRightDraw = true;
			this.panelLevel2.Location = new System.Drawing.Point(22, 0);
			this.panelLevel2.MinimumSize = new System.Drawing.Size(1, 15);
			this.panelLevel2.Name = "panelLevel2";
			this.panelLevel2.PanelName = "Level2";
			this.panelLevel2.Size = new System.Drawing.Size(31, 238);
			this.panelLevel2.TabIndex = 4;
			// 
			// tooltipPosition
			// 
			this.tooltipPosition.BackColor = System.Drawing.SystemColors.Info;
			this.tooltipPosition.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tooltipPosition.ForeColor = System.Drawing.SystemColors.InfoText;
			this.tooltipPosition.Location = new System.Drawing.Point(669, 72);
			this.tooltipPosition.Name = "tooltipPosition";
			this.tooltipPosition.Size = new System.Drawing.Size(139, 215);
			this.tooltipPosition.TabIndex = 0;
			this.tooltipPosition.Visible = false;
			// 
			// multiSplitContainerColumns
			// 
			this.multiSplitContainerColumns.BackColor = System.Drawing.SystemColors.ControlDark;
			this.multiSplitContainerColumns.Location = new System.Drawing.Point(399, 160);
			this.multiSplitContainerColumns.Name = "multiSplitContainerColumns";
			this.multiSplitContainerColumns.Size = new System.Drawing.Size(97, 118);
			this.multiSplitContainerColumns.TabIndex = 3;
			// 
			// lblWinFormDesignerComment
			// 
			this.lblWinFormDesignerComment.Location = new System.Drawing.Point(396, 3);
			this.lblWinFormDesignerComment.Name = "lblWinFormDesignerComment";
			this.lblWinFormDesignerComment.Size = new System.Drawing.Size(293, 97);
			this.lblWinFormDesignerComment.TabIndex = 2;
			this.lblWinFormDesignerComment.Text = resources.GetString("lblWinFormDesignerComment.Text");
			this.lblWinFormDesignerComment.Visible = false;
			// 
			// multiSplitContainerRows
			// 
			this.multiSplitContainerRows.BackColor = System.Drawing.Color.DarkGray;
			this.multiSplitContainerRows.Location = new System.Drawing.Point(439, 103);
			this.multiSplitContainerRows.Name = "multiSplitContainerRows";
			this.multiSplitContainerRows.Size = new System.Drawing.Size(84, 88);
			this.multiSplitContainerRows.TabIndex = 1;
			// 
			// panelVolume
			// 
			this.panelVolume.BarsIdent = "UNINITIALIZED_BARS_IDENT_PanelNamedFolding";
			this.panelVolume.Font = new System.Drawing.Font("Consolas", 8.25F);
			this.panelVolume.GutterBottomDraw = false;
			this.panelVolume.GutterRightDraw = true;
			this.panelVolume.Location = new System.Drawing.Point(22, 244);
			this.panelVolume.MinimumSize = new System.Drawing.Size(20, 15);
			this.panelVolume.Name = "panelVolume";
			this.panelVolume.PanelName = "Volume";
			this.panelVolume.Size = new System.Drawing.Size(368, 45);
			this.panelVolume.TabIndex = 0;
			// 
			// panelPrice
			// 
			this.PanelPrice.BackColor = System.Drawing.SystemColors.Window;
			this.PanelPrice.BarsIdent = "UNINITIALIZED_BARS_IDENT_PanelNamedFolding";
			this.PanelPrice.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.PanelPrice.GutterBottomDraw = true;
			this.PanelPrice.GutterRightDraw = true;
			this.PanelPrice.Location = new System.Drawing.Point(59, 0);
			this.PanelPrice.MinimumSize = new System.Drawing.Size(20, 25);
			this.PanelPrice.Name = "panelPrice";
			this.PanelPrice.PanelName = "Price";
			this.PanelPrice.Size = new System.Drawing.Size(331, 238);
			this.PanelPrice.TabIndex = 1;
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
			this.RangeBar.Size = new System.Drawing.Size(811, 53);
			this.RangeBar.TabIndex = 0;
			this.RangeBar.ValueFormat = "dd-MMM-yy";
			this.RangeBar.ValueMax = new System.DateTime(2012, 5, 12, 0, 0, 0, 0);
			this.RangeBar.ValueMin = new System.DateTime(2011, 5, 12, 0, 0, 0, 0);
			// 
			// ChartControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.hScrollBar);
			this.Controls.Add(this.splitContainerChartVsRange);
			this.Name = "ChartControl";
			this.Size = new System.Drawing.Size(811, 364);
			this.splitContainerChartVsRange.Panel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainerChartVsRange)).EndInit();
			this.splitContainerChartVsRange.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		private Sq1.Charting.MultiSplit.MultiSplitContainer multiSplitContainerColumns;

		private System.Windows.Forms.Label lblWinFormDesignerComment;
		private Sq1.Charting.MultiSplit.MultiSplitContainer multiSplitContainerRows;
		private System.Windows.Forms.HScrollBar hScrollBar;
		public PanelPrice PanelPrice { get; private set; }
		private System.Windows.Forms.SplitContainer splitContainerChartVsRange;
		public  Widgets.RangeBar.RangeBarDateTime RangeBar;
		private PanelVolume panelVolume;
		private Sq1.Charting.TooltipPrice tooltipPrice;
		private Sq1.Charting.TooltipPosition tooltipPosition;
		private PanelLevel2 panelLevel2;
	}
}