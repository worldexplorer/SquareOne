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
		private BrightIdeasSoftware.OLVColumn olvcCost;
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
		private BrightIdeasSoftware.OLVColumn olvcMae;
		private BrightIdeasSoftware.OLVColumn olvcMfe;
		private BrightIdeasSoftware.OLVColumn olvcMaePct;
		private BrightIdeasSoftware.OLVColumn olvcMfePct;
		private BrightIdeasSoftware.OLVColumn olvcCumNetProfitDollar;
		private BrightIdeasSoftware.OLVColumn olvcCumNetProfitPct;
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
			this.olvcPosition = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcSerno = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcSymbol = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcQuantity = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcCost = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcEntryDate = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcEntryPrice = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcEntryOrder = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcExitDate = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcExitPrice = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcExitOrder = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcProfitPct = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcProfitDollar = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcBarsHeld = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcProfitPerBar = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcEntrySignalName = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcExitSignalName = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcMaePct = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcMfePct = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcMae = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcMfe = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcCumNetProfitPct = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcCumNetProfitDollar = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcComission = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.ctxPopup = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.mniColorify = new System.Windows.Forms.ToolStripMenuItem();
			this.mniCopyToClipboard = new System.Windows.Forms.ToolStripMenuItem();
			((System.ComponentModel.ISupportInitialize)(this.olvPositions)).BeginInit();
			this.ctxPopup.SuspendLayout();
			this.SuspendLayout();
			// 
			// olvPositions
			// 
			this.olvPositions.AllowColumnReorder = true;
			this.olvPositions.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.olvPositions.CausesValidation = false;
			this.olvPositions.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.F2Only;
			this.olvPositions.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvcPosition,
            this.olvcSerno,
            this.olvcSymbol,
            this.olvcQuantity,
            this.olvcCost,
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
            this.olvcMae,
            this.olvcMfe,
            this.olvcCumNetProfitPct,
            this.olvcCumNetProfitDollar,
            this.olvcComission});
			this.olvPositions.ContextMenuStrip = this.ctxPopup;
			this.olvPositions.Cursor = System.Windows.Forms.Cursors.Default;
			this.olvPositions.Dock = System.Windows.Forms.DockStyle.Fill;
			this.olvPositions.FullRowSelect = true;
			this.olvPositions.HideSelection = false;
			this.olvPositions.IncludeColumnHeadersInCopy = true;
			this.olvPositions.IncludeHiddenColumnsInDataTransfer = true;
			this.olvPositions.Location = new System.Drawing.Point(0, 0);
			this.olvPositions.Name = "olvPositions";
			this.olvPositions.ShowCommandMenuOnRightClick = true;
			this.olvPositions.ShowGroups = false;
			this.olvPositions.ShowItemToolTips = true;
			this.olvPositions.Size = new System.Drawing.Size(905, 303);
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
			this.olvcQuantity.Width = 15;
			// 
			// olvcCost
			// 
			this.olvcCost.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcCost.Text = "$Cost";
			this.olvcCost.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
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
			// 
			// olvcExitOrder
			// 
			this.olvcExitOrder.Text = "ExitOrder";
			this.olvcExitOrder.Width = 35;
			// 
			// olvcProfitPct
			// 
			this.olvcProfitPct.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcProfitPct.Text = "%Profit";
			this.olvcProfitPct.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcProfitPct.Width = 40;
			// 
			// olvcProfitDollar
			// 
			this.olvcProfitDollar.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcProfitDollar.Text = "$Profit";
			this.olvcProfitDollar.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcProfitDollar.Width = 50;
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
			this.olvcMaePct.Text = "%MAE";
			this.olvcMaePct.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcMaePct.Width = 47;
			// 
			// olvcMfePct
			// 
			this.olvcMfePct.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcMfePct.Text = "%MFE";
			this.olvcMfePct.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcMfePct.Width = 47;
			// 
			// olvcMae
			// 
			this.olvcMae.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcMae.Text = "$MAE";
			this.olvcMae.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcMae.Width = 47;
			// 
			// olvcMfe
			// 
			this.olvcMfe.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcMfe.Text = "$MFE";
			this.olvcMfe.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcMfe.Width = 47;
			// 
			// olvcCumNetProfitPct
			// 
			this.olvcCumNetProfitPct.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcCumNetProfitPct.Text = "%ProfitCumulative";
			this.olvcCumNetProfitPct.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcCumNetProfitPct.Width = 46;
			// 
			// olvcCumNetProfitDollar
			// 
			this.olvcCumNetProfitDollar.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcCumNetProfitDollar.Text = "$ProfitCumulative";
			this.olvcCumNetProfitDollar.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcCumNetProfitDollar.Width = 55;
			// 
			// olvcComission
			// 
			this.olvcComission.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcComission.Text = "$Commission";
			this.olvcComission.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcComission.Width = 45;
			// 
			// ctxPopup
			// 
			this.ctxPopup.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mniColorify,
            this.mniCopyToClipboard});
			this.ctxPopup.Name = "contextMenuStrip1";
			this.ctxPopup.Size = new System.Drawing.Size(152, 48);
			// 
			// mniColorify
			// 
			this.mniColorify.Checked = true;
			this.mniColorify.CheckOnClick = true;
			this.mniColorify.CheckState = System.Windows.Forms.CheckState.Checked;
			this.mniColorify.Name = "mniColorify";
			this.mniColorify.Size = new System.Drawing.Size(151, 22);
			this.mniColorify.Text = "Colorify (slow)";
			this.mniColorify.Click += new System.EventHandler(this.mniColorify_Click);
			// 
			// mniCopyToClipboard
			// 
			this.mniCopyToClipboard.Name = "mniCopyToClipboard";
			this.mniCopyToClipboard.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
			this.mniCopyToClipboard.Size = new System.Drawing.Size(151, 22);
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