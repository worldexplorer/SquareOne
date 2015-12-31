using System;
using Sq1.Core;

namespace Sq1.Charting {
	partial class ChartControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
				this.barEventsDetach();

				if (this.ChartSettings != null) {
					this.ChartSettings.PensAndBrushesCached_DisposeAndNullify();
				} else {
					string msg = "this.ChartSettings=null //ChartControl.Dispose()";
					Assembler.PopupException(msg);
				}
			}
			base.Dispose(disposing);
		}
		
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChartControl));
			this.hScrollBar = new System.Windows.Forms.HScrollBar();
			this.splitContainerChartVsRange = new System.Windows.Forms.SplitContainer();
			this.panelLevel2 = new Sq1.Charting.PanelLevel2();
			this.tooltipPrice = new Sq1.Charting.TooltipPrice();
			this.tooltipPosition = new Sq1.Charting.TooltipPosition();
			this.multiSplitColumns_Level2_PriceVolumeMultisplit = new Sq1.Charting.MultiSplit.MultiSplitContainer();
			this.lblWinFormDesignerComment = new System.Windows.Forms.Label();
			this.multiSplitRowsVolumePrice = new Sq1.Charting.MultiSplit.MultiSplitContainer();
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
			this.hScrollBar.Location = new System.Drawing.Point(0, 303);
			this.hScrollBar.Name = "hScrollBar";
			this.hScrollBar.Size = new System.Drawing.Size(840, 17);
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
			this.splitContainerChartVsRange.Panel1.Controls.Add(this.panelLevel2);
			this.splitContainerChartVsRange.Panel1.Controls.Add(this.tooltipPrice);
			this.splitContainerChartVsRange.Panel1.Controls.Add(this.tooltipPosition);
			this.splitContainerChartVsRange.Panel1.Controls.Add(this.multiSplitColumns_Level2_PriceVolumeMultisplit);
			this.splitContainerChartVsRange.Panel1.Controls.Add(this.lblWinFormDesignerComment);
			this.splitContainerChartVsRange.Panel1.Controls.Add(this.multiSplitRowsVolumePrice);
			this.splitContainerChartVsRange.Panel1.Controls.Add(this.panelVolume);
			this.splitContainerChartVsRange.Panel1.Controls.Add(this.PanelPrice);
			this.splitContainerChartVsRange.Size = new System.Drawing.Size(840, 303);
			this.splitContainerChartVsRange.SplitterDistance = 246;
			this.splitContainerChartVsRange.TabIndex = 0;
			this.splitContainerChartVsRange.TabStop = false;
			// 
			// panelLevel2
			// 
			this.panelLevel2.BarsIdent = "UNINITIALIZED_BARS_IDENT_PanelNamedFolding";
			this.panelLevel2.GutterBottomDraw = false;
			this.panelLevel2.GutterRightDraw = true;
			this.panelLevel2.Location = new System.Drawing.Point(363, 13);
			this.panelLevel2.MinimumSize = new System.Drawing.Size(1, 15);
			this.panelLevel2.Name = "panelLevel2";
			this.panelLevel2.PanelName = "Level2";
			this.panelLevel2.Size = new System.Drawing.Size(11, 212);
			this.panelLevel2.TabIndex = 4;
			// 
			// tooltipPrice
			// 
			this.tooltipPrice.BackColor = System.Drawing.SystemColors.Info;
			this.tooltipPrice.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tooltipPrice.ForeColor = System.Drawing.SystemColors.InfoText;
			this.tooltipPrice.Location = new System.Drawing.Point(564, 102);
			this.tooltipPrice.Name = "tooltipPrice";
			this.tooltipPrice.Size = new System.Drawing.Size(116, 116);
			this.tooltipPrice.TabIndex = 1;
			this.tooltipPrice.Visible = false;
			// 
			// tooltipPosition
			// 
			this.tooltipPosition.BackColor = System.Drawing.SystemColors.Info;
			this.tooltipPosition.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tooltipPosition.ForeColor = System.Drawing.SystemColors.InfoText;
			this.tooltipPosition.Location = new System.Drawing.Point(696, 3);
			this.tooltipPosition.Name = "tooltipPosition";
			this.tooltipPosition.Size = new System.Drawing.Size(139, 215);
			this.tooltipPosition.TabIndex = 0;
			this.tooltipPosition.Visible = false;
			// 
			// multiSplitColumns_Level2_PriceVolumeMultisplit
			// 
			this.multiSplitColumns_Level2_PriceVolumeMultisplit.BackColor = System.Drawing.SystemColors.ControlDark;
			this.multiSplitColumns_Level2_PriceVolumeMultisplit.Location = new System.Drawing.Point(399, 91);
			this.multiSplitColumns_Level2_PriceVolumeMultisplit.Name = "multiSplitColumns_Level2_PriceVolumeMultisplit";
			this.multiSplitColumns_Level2_PriceVolumeMultisplit.Size = new System.Drawing.Size(97, 118);
			this.multiSplitColumns_Level2_PriceVolumeMultisplit.TabIndex = 3;
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
			// multiSplitRowsVolumePrice
			// 
			this.multiSplitRowsVolumePrice.BackColor = System.Drawing.Color.DarkGray;
			this.multiSplitRowsVolumePrice.Location = new System.Drawing.Point(474, 130);
			this.multiSplitRowsVolumePrice.Name = "multiSplitRowsVolumePrice";
			this.multiSplitRowsVolumePrice.Size = new System.Drawing.Size(84, 88);
			this.multiSplitRowsVolumePrice.TabIndex = 1;
			// 
			// panelVolume
			// 
			this.panelVolume.BarsIdent = "UNINITIALIZED_BARS_IDENT_PanelNamedFolding";
			this.panelVolume.Font = new System.Drawing.Font("Consolas", 8.25F);
			this.panelVolume.GutterBottomDraw = false;
			this.panelVolume.GutterRightDraw = true;
			this.panelVolume.Location = new System.Drawing.Point(3, 13);
			this.panelVolume.MinimumSize = new System.Drawing.Size(20, 15);
			this.panelVolume.Name = "panelVolume";
			this.panelVolume.PanelName = "Volume";
			this.panelVolume.Size = new System.Drawing.Size(354, 15);
			this.panelVolume.TabIndex = 0;
			// 
			// PanelPrice
			// 
			this.PanelPrice.BackColor = System.Drawing.SystemColors.Window;
			this.PanelPrice.BarsIdent = "UNINITIALIZED_BARS_IDENT_PanelNamedFolding";
			this.PanelPrice.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.PanelPrice.GutterBottomDraw = true;
			this.PanelPrice.GutterRightDraw = true;
			this.PanelPrice.Location = new System.Drawing.Point(3, 34);
			this.PanelPrice.MinimumSize = new System.Drawing.Size(20, 25);
			this.PanelPrice.Name = "PanelPrice";
			this.PanelPrice.PanelName = "Price";
			this.PanelPrice.Size = new System.Drawing.Size(354, 191);
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
			this.Size = new System.Drawing.Size(840, 320);
			this.splitContainerChartVsRange.Panel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainerChartVsRange)).EndInit();
			this.splitContainerChartVsRange.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		private Sq1.Charting.MultiSplit.MultiSplitContainer multiSplitColumns_Level2_PriceVolumeMultisplit;

		private System.Windows.Forms.Label lblWinFormDesignerComment;
		private Sq1.Charting.MultiSplit.MultiSplitContainer multiSplitRowsVolumePrice;
		private System.Windows.Forms.HScrollBar hScrollBar;
		private System.Windows.Forms.SplitContainer splitContainerChartVsRange;
		public  Widgets.RangeBar.RangeBarDateTime RangeBar;
		private Sq1.Charting.TooltipPrice tooltipPrice;
		private Sq1.Charting.TooltipPosition tooltipPosition;

		private PanelLevel2 panelLevel2;
		public PanelPrice PanelPrice;		// ID_LIKE_TO_ENFORCE_VISIBILITY_BUT_DESIGNER_THROWS { get; private set; }
		private PanelVolume panelVolume;
	}
}