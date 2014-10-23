using System;
using Sq1.Charting.MultiSplit;
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
			this.lblWinFormDesignerComment = new System.Windows.Forms.Label();
			this.panelVolume = new Sq1.Charting.PanelVolume();
			this.panelPrice = new Sq1.Charting.PanelPrice();
			this.multiSplitContainer = new Sq1.Charting.MultiSplit.MultiSplitContainer<PanelBase>();
			this.RangeBar = new Sq1.Widgets.RangeBar.RangeBarDateTime();
			this.tooltipPosition = new Sq1.Charting.TooltipPosition();
			this.tooltipPrice = new Sq1.Charting.TooltipPrice();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerChartVsRange)).BeginInit();
			this.splitContainerChartVsRange.Panel1.SuspendLayout();
			this.splitContainerChartVsRange.Panel2.SuspendLayout();
			this.splitContainerChartVsRange.SuspendLayout();
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
			this.splitContainerChartVsRange.Panel1.Controls.Add(this.lblWinFormDesignerComment);
			this.splitContainerChartVsRange.Panel1.Controls.Add(this.panelVolume);
			this.splitContainerChartVsRange.Panel1.Controls.Add(this.panelPrice);
			this.splitContainerChartVsRange.Panel1.Controls.Add(this.multiSplitContainer);
			// 
			// splitContainerChartVsRange.Panel2
			// 
			this.splitContainerChartVsRange.Panel2.Controls.Add(this.RangeBar);
			this.splitContainerChartVsRange.Size = new System.Drawing.Size(821, 346);
			this.splitContainerChartVsRange.SplitterDistance = 289;
			this.splitContainerChartVsRange.TabIndex = 0;
			this.splitContainerChartVsRange.TabStop = false;
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
			// panelVolume
			// 
			this.panelVolume.BarsIdent = "UNINITIALIZED_BARS_IDENT_PanelNamedFolding";
			this.panelVolume.Font = new System.Drawing.Font("Consolas", 8.25F);
			this.panelVolume.GutterBottomDraw = false;
			this.panelVolume.GutterRightDraw = true;
			this.panelVolume.Location = new System.Drawing.Point(22, 244);
			this.panelVolume.Name = "panelVolume";
			this.panelVolume.PanelName = "Volume";
			this.panelVolume.Size = new System.Drawing.Size(368, 45);
			this.panelVolume.TabIndex = 0;
			// 
			// panelPrice
			// 
			this.panelPrice.BackColor = System.Drawing.SystemColors.Window;
			this.panelPrice.BarsIdent = "UNINITIALIZED_BARS_IDENT_PanelNamedFolding";
			this.panelPrice.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.panelPrice.GutterBottomDraw = true;
			this.panelPrice.GutterRightDraw = true;
			this.panelPrice.Location = new System.Drawing.Point(22, 0);
			this.panelPrice.Name = "panelPrice";
			this.panelPrice.PanelName = "Price";
			this.panelPrice.Size = new System.Drawing.Size(368, 238);
			this.panelPrice.TabIndex = 1;
			// 
			// multiSplitContainer
			// 
			this.multiSplitContainer.BackColor = System.Drawing.Color.DarkGray;
			this.multiSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.multiSplitContainer.Location = new System.Drawing.Point(0, 0);
			this.multiSplitContainer.Name = "multiSplitContainer";
			this.multiSplitContainer.Size = new System.Drawing.Size(821, 289);
			this.multiSplitContainer.TabIndex = 1;
			this.multiSplitContainer.OnSplitterMoveEnded += new EventHandler<MultiSplitterEventArgs>(multiSplitContainer_OnResizing_OnSplitterMoveOrDragEnded);
			this.multiSplitContainer.OnSplitterDragEnded += new EventHandler<MultiSplitterEventArgs>(multiSplitContainer_OnResizing_OnSplitterMoveOrDragEnded);
			this.multiSplitContainer.Resize += new EventHandler(multiSplitContainer_OnResizing_OnSplitterMoveOrDragEnded);
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
			//this.tooltipPrice.Size = new System.Drawing.Size(115, 116);
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
			this.ResumeLayout(false);
		}

		private System.Windows.Forms.Label lblWinFormDesignerComment;
		private Sq1.Charting.MultiSplit.MultiSplitContainer<PanelBase> multiSplitContainer;
		private System.Windows.Forms.HScrollBar hScrollBar;
		private PanelPrice panelPrice;
		private System.Windows.Forms.SplitContainer splitContainerChartVsRange;
		public  Widgets.RangeBar.RangeBarDateTime RangeBar;
		private PanelVolume panelVolume;
		private Sq1.Charting.TooltipPrice tooltipPrice;
		private Sq1.Charting.TooltipPosition tooltipPosition;
	}
}