using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Sq1.Reporters {
	public partial class Positions {
		private BrightIdeasSoftware.ObjectListView olvPositions;
		private IContainer components;
		//private ImageList tradeTypes;
		private BrightIdeasSoftware.OLVColumn olvcPosition;
		private BrightIdeasSoftware.OLVColumn olvcSerno;
		private BrightIdeasSoftware.OLVColumn olvcSymbol;
		private BrightIdeasSoftware.OLVColumn olvcQuantity;
		private BrightIdeasSoftware.OLVColumn olvcEntryDate;
		private BrightIdeasSoftware.OLVColumn olvcEntryPrice;
		private BrightIdeasSoftware.OLVColumn olvcEntryOrder;
		private BrightIdeasSoftware.OLVColumn olvcExitDate;
		private BrightIdeasSoftware.OLVColumn olvcExitPrice;
		private BrightIdeasSoftware.OLVColumn olvcExitOrder;
		private BrightIdeasSoftware.OLVColumn olvcProfitPct;
		private BrightIdeasSoftware.OLVColumn olvcProfitDollar;
		private BrightIdeasSoftware.OLVColumn olvcBarsHeld;
		private BrightIdeasSoftware.OLVColumn olvcProfitPerBar;
		private BrightIdeasSoftware.OLVColumn olvcEntrySignalName;
		private BrightIdeasSoftware.OLVColumn olvcExitSignalName;
		private BrightIdeasSoftware.OLVColumn olvcMaePct;
		private BrightIdeasSoftware.OLVColumn olvcMfePct;
		private BrightIdeasSoftware.OLVColumn olvcCumProfitDollar;
		private BrightIdeasSoftware.OLVColumn olvcCumProfitPct;
		private BrightIdeasSoftware.OLVColumn olvcComission;

		private ContextMenuStrip ctxPopup;
		private ToolStripMenuItem mniColorify;

		protected override void Dispose(bool disposing) {
			if (disposing && this.components != null) {
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}
		void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.olvPositions = new BrightIdeasSoftware.ObjectListView();
			this.olvcPosition = new BrightIdeasSoftware.OLVColumn();
			this.olvcSerno = new BrightIdeasSoftware.OLVColumn();
			this.olvcSymbol = new BrightIdeasSoftware.OLVColumn();
			this.olvcQuantity = new BrightIdeasSoftware.OLVColumn();
			this.olvcEntryDate = new BrightIdeasSoftware.OLVColumn();
			this.olvcEntryPrice = new BrightIdeasSoftware.OLVColumn();
			this.olvcEntryOrder = new BrightIdeasSoftware.OLVColumn();
			this.olvcExitDate = new BrightIdeasSoftware.OLVColumn();
			this.olvcExitPrice = new BrightIdeasSoftware.OLVColumn();
			this.olvcExitOrder = new BrightIdeasSoftware.OLVColumn();
			this.olvcProfitPct = new BrightIdeasSoftware.OLVColumn();
			this.olvcProfitDollar = new BrightIdeasSoftware.OLVColumn();
			this.olvcBarsHeld = new BrightIdeasSoftware.OLVColumn();
			this.olvcProfitPerBar = new BrightIdeasSoftware.OLVColumn();
			this.olvcEntrySignalName = new BrightIdeasSoftware.OLVColumn();
			this.olvcExitSignalName = new BrightIdeasSoftware.OLVColumn();
			this.olvcMaePct = new BrightIdeasSoftware.OLVColumn();
			this.olvcMfePct = new BrightIdeasSoftware.OLVColumn();
			this.olvcCumProfitPct = new BrightIdeasSoftware.OLVColumn();
			this.olvcCumProfitDollar = new BrightIdeasSoftware.OLVColumn();
			this.olvcComission = new BrightIdeasSoftware.OLVColumn();
			this.ctxPopup = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.mniColorify = new System.Windows.Forms.ToolStripMenuItem();
			this.mniCopyToClipboard = new System.Windows.Forms.ToolStripMenuItem();
			((System.ComponentModel.ISupportInitialize)(this.olvPositions)).BeginInit();
			this.ctxPopup.SuspendLayout();
			this.SuspendLayout();
			// 
			// olvPositions
			// 
			this.olvPositions.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
			this.olvcPosition,
			this.olvcSerno,
			this.olvcSymbol,
			this.olvcQuantity,
			this.olvcEntryDate,
			this.olvcEntryPrice,
			this.olvcEntryOrder,
			this.olvcExitDate,
			this.olvcExitPrice,
			this.olvcExitOrder,
			this.olvcProfitPct,
			this.olvcProfitDollar,
			this.olvcBarsHeld,
			this.olvcProfitPerBar,
			this.olvcEntrySignalName,
			this.olvcExitSignalName,
			this.olvcMaePct,
			this.olvcMfePct,
			this.olvcCumProfitPct,
			this.olvcCumProfitDollar,
			this.olvcComission});
			this.olvPositions.AllowColumnReorder = true;
			this.olvPositions.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.olvPositions.CausesValidation = false;
			this.olvPositions.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.F2Only;
			this.olvPositions.ContextMenuStrip = this.ctxPopup;
			this.olvPositions.Dock = System.Windows.Forms.DockStyle.Fill;
			this.olvPositions.FullRowSelect = true;
			this.olvPositions.HideSelection = false;
			this.olvPositions.IncludeColumnHeadersInCopy = true;
			this.olvPositions.IncludeHiddenColumnsInDataTransfer = true;
			this.olvPositions.Location = new System.Drawing.Point(0, 0);
			this.olvPositions.MultiSelect = false;
			this.olvPositions.Name = "olvPositions";
			this.olvPositions.ShowGroups = false;
			this.olvPositions.ShowCommandMenuOnRightClick = true;
			this.olvPositions.ShowItemToolTips = true;
			this.olvPositions.Size = new System.Drawing.Size(905, 303);
			//this.OrdersTreeOLV.SmallImageList = this.imgListOrderDirection;
			this.olvPositions.TabIndex = 0;
			this.olvPositions.TintSortColumn = true;
			this.olvPositions.UnfocusedHighlightBackgroundColor = System.Drawing.SystemColors.GradientActiveCaption;
			this.olvPositions.UseCompatibleStateImageBehavior = false;
			this.olvPositions.UseFilterIndicator = true;
			this.olvPositions.UseFiltering = true;
			this.olvPositions.UseHotItem = true;
			this.olvPositions.UseTranslucentHotItem = true;
			this.olvPositions.View = System.Windows.Forms.View.Details;
			this.olvPositions.DoubleClick += new System.EventHandler(this.olvPositions_DoubleClick);
			// 
			// olvcPosition
			// 
			this.olvcPosition.Text = "Position";
			this.olvcPosition.Width = 35;
			// 
			// olvcSerno
			// 
			this.olvcSerno.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcSerno.Text = "Serno";
			this.olvcSerno.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcSerno.Width = 30;
			// 
			// olvcSymbol
			// 
			this.olvcSymbol.Text = "Symbol";
			this.olvcSymbol.Width = 40;
			// 
			// olvcQuantity
			// 
			this.olvcQuantity.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcQuantity.Text = "Shares";
			this.olvcQuantity.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcQuantity.Width = 19;
			// 
			// olvcEntryDate
			// 
			this.olvcEntryDate.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcEntryDate.Text = "Entry Date";
			this.olvcEntryDate.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcEntryDate.Width = 108;
			// 
			// olvcEntryPrice
			// 
			this.olvcEntryPrice.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcEntryPrice.Text = "Entry Price";
			this.olvcEntryPrice.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcEntryPrice.Width = 55;
			// 
			// olvcEntryOrder
			// 
			this.olvcEntryOrder.Text = "EntryOrder";
			this.olvcEntryOrder.Width = 35;
			// 
			// olvcExitDate
			// 
			this.olvcExitDate.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcExitDate.Text = "Exit Date";
			this.olvcExitDate.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcExitDate.Width = 108;
			// 
			// olvcExitPrice
			// 
			this.olvcExitPrice.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcExitPrice.Text = "Exit Price";
			this.olvcExitPrice.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcExitPrice.Width = 55;
			// 
			// olvcExitOrder
			// 
			this.olvcExitOrder.Text = "ExitOrder";
			this.olvcExitOrder.Width = 35;
			// 
			// olvcProfitPct
			// 
			this.olvcProfitPct.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcProfitPct.Text = "Profit %";
			this.olvcProfitPct.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcProfitPct.Width = 36;
			// 
			// olvcProfitDollar
			// 
			this.olvcProfitDollar.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcProfitDollar.Text = "Profit $";
			this.olvcProfitDollar.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcProfitDollar.Width = 45;
			// 
			// olvcBarsHeld
			// 
			this.olvcBarsHeld.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcBarsHeld.Text = "Bars Held";
			this.olvcBarsHeld.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcBarsHeld.Width = 31;
			// 
			// olvcProfitPerBar
			// 
			this.olvcProfitPerBar.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcProfitPerBar.Text = "Profit per Bar";
			this.olvcProfitPerBar.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcProfitPerBar.Width = 40;
			// 
			// olvcEntrySignalName
			// 
			this.olvcEntrySignalName.Text = "Entry Signal";
			this.olvcEntrySignalName.Width = 70;
			// 
			// olvcExitSignalName
			// 
			this.olvcExitSignalName.Text = "Exit Signal";
			this.olvcExitSignalName.Width = 70;
			// 
			// olvcMaePct
			// 
			this.olvcMaePct.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcMaePct.Text = "MAE %";
			this.olvcMaePct.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcMaePct.Width = 42;
			// 
			// olvcMfePct
			// 
			this.olvcMfePct.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcMfePct.Text = "MFE %";
			this.olvcMfePct.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcMfePct.Width = 42;
			// 
			// olvcCumProfitPct
			// 
			this.olvcCumProfitPct.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcCumProfitPct.Text = "%Profit Cumulative";
			this.olvcCumProfitPct.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcCumProfitPct.Width = 41;
			// 
			// olvcCumProfitDollar
			// 
			this.olvcCumProfitDollar.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcCumProfitDollar.Text = "$Profit Cumulative";
			this.olvcCumProfitDollar.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcCumProfitDollar.Width = 50;
			// 
			// olvcComission
			// 
			this.olvcComission.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcComission.Text = "$Commission";
			this.olvcComission.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcComission.Width = 40;
			// 
			// ctxPopup
			// 
			this.ctxPopup.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.mniColorify,
			this.mniCopyToClipboard});
			this.ctxPopup.Name = "contextMenuStrip1";
			this.ctxPopup.Size = new System.Drawing.Size(243, 214);
			// 
			// mniColorify
			// 
			this.mniColorify.Checked = true;
			this.mniColorify.CheckOnClick = true;
			this.mniColorify.CheckState = System.Windows.Forms.CheckState.Checked;
			this.mniColorify.Name = "mniColorify";
			this.mniColorify.Size = new System.Drawing.Size(242, 22);
			this.mniColorify.Text = "Colorify (slow)";
			this.mniColorify.Click += new System.EventHandler(this.mniColorify_Click);
			// 
			// mniCopyToClipboard
			// 
			this.mniCopyToClipboard.Name = "mniCopyToClipboard";
			this.mniCopyToClipboard.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
			this.mniCopyToClipboard.Size = new System.Drawing.Size(242, 22);
			this.mniCopyToClipboard.Text = "Copy";
			this.mniCopyToClipboard.Click += new System.EventHandler(this.mniCopyToClipboard_Click);
			// 
			// Positions
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.olvPositions);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.Name = "Positions";
			this.Size = new System.Drawing.Size(905, 303);
			((System.ComponentModel.ISupportInitialize)(this.olvPositions)).EndInit();
			this.ctxPopup.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		private ToolStripMenuItem mniCopyToClipboard;
	}
}