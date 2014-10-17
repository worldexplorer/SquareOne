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
		private ToolStripMenuItem mniShowEntriesExits;
		private ToolStripMenuItem mniShowPercentage;
		private ToolStripMenuItem mniShowBarsHeld;
		private ToolStripMenuItem mniShowMaeMfe;
		private ToolStripMenuItem mniShowSignals;
		private ToolStripMenuItem mniShowCommission;
		private ToolStripMenuItem mniColorify;

		protected override void Dispose(bool disposing) {
			if (disposing && this.components != null) {
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}
		void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			//System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Positions));
			//this.tradeTypes = new System.Windows.Forms.ImageList(this.components);
			this.olvPositions = new BrightIdeasSoftware.ObjectListView();
			this.olvcPosition = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcSerno = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcSymbol = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcQuantity = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
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
			this.olvcCumProfitPct = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcCumProfitDollar = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcComission = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.ctxPopup = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.mniShowEntriesExits = new System.Windows.Forms.ToolStripMenuItem();
			this.mniShowPercentage = new System.Windows.Forms.ToolStripMenuItem();
			this.mniShowBarsHeld = new System.Windows.Forms.ToolStripMenuItem();
			this.mniShowMaeMfe = new System.Windows.Forms.ToolStripMenuItem();
			this.mniShowSignals = new System.Windows.Forms.ToolStripMenuItem();
			this.mniShowCommission = new System.Windows.Forms.ToolStripMenuItem();
			this.mniColorify = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.mniCopyToClipboard = new System.Windows.Forms.ToolStripMenuItem();
			this.mniSaveToFile = new System.Windows.Forms.ToolStripMenuItem();
			((System.ComponentModel.ISupportInitialize)(this.olvPositions)).BeginInit();
			this.ctxPopup.SuspendLayout();
			this.SuspendLayout();
			// 
			// tradeTypes
			// 
			//this.tradeTypes.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("tradeTypes.ImageStream")));
			//this.tradeTypes.TransparentColor = System.Drawing.Color.Transparent;
			//this.tradeTypes.Images.SetKeyName(0, "45degrees3-LongEntryUnknown.png");
			//this.tradeTypes.Images.SetKeyName(1, "45degrees3-LongEntryProfit.png");
			//this.tradeTypes.Images.SetKeyName(2, "45degrees3-LongEntryLoss.png");
			//this.tradeTypes.Images.SetKeyName(3, "45degrees3-ShortEntryUnknown.png");
			//this.tradeTypes.Images.SetKeyName(4, "45degrees3-ShortEntryProfit.png");
			//this.tradeTypes.Images.SetKeyName(5, "45degrees3-ShortEntryLoss.png");
			// 
			// olvPositions
			// 
			this.olvPositions.BackColor = System.Drawing.SystemColors.Window;
			this.olvPositions.BorderStyle = System.Windows.Forms.BorderStyle.None;
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
			this.olvPositions.AllColumns.Add(this.olvcPosition);
			this.olvPositions.AllColumns.Add(this.olvcSerno);
			this.olvPositions.AllColumns.Add(this.olvcSymbol);
			this.olvPositions.AllColumns.Add(this.olvcQuantity);
			this.olvPositions.AllColumns.Add(this.olvcEntryDate);
			this.olvPositions.AllColumns.Add(this.olvcEntryPrice);
			this.olvPositions.AllColumns.Add(this.olvcEntryOrder);
			this.olvPositions.AllColumns.Add(this.olvcExitDate);
			this.olvPositions.AllColumns.Add(this.olvcExitPrice);
			this.olvPositions.AllColumns.Add(this.olvcExitOrder);
			this.olvPositions.AllColumns.Add(this.olvcProfitPct);
			this.olvPositions.AllColumns.Add(this.olvcProfitDollar);
			this.olvPositions.AllColumns.Add(this.olvcBarsHeld);
			this.olvPositions.AllColumns.Add(this.olvcProfitPerBar);
			this.olvPositions.AllColumns.Add(this.olvcEntrySignalName);
			this.olvPositions.AllColumns.Add(this.olvcExitSignalName);
			this.olvPositions.AllColumns.Add(this.olvcMaePct);
			this.olvPositions.AllColumns.Add(this.olvcMfePct);
			this.olvPositions.AllColumns.Add(this.olvcCumProfitPct);
			this.olvPositions.AllColumns.Add(this.olvcCumProfitDollar);
			this.olvPositions.AllColumns.Add(this.olvcComission);
			this.olvPositions.ContextMenuStrip = this.ctxPopup;
			this.olvPositions.Dock = System.Windows.Forms.DockStyle.Fill;
			this.olvPositions.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.olvPositions.FullRowSelect = true;
			this.olvPositions.HeaderUsesThemes = false;
			this.olvPositions.HideSelection = false;
			this.olvPositions.Location = new System.Drawing.Point(0, 0);
			this.olvPositions.MultiSelect = false;
			this.olvPositions.ShowCommandMenuOnRightClick = true;
			this.olvPositions.ShowGroups = false;
			this.olvPositions.TintSortColumn = true;
			this.olvPositions.UnfocusedHighlightBackgroundColor = System.Drawing.SystemColors.GradientActiveCaption;
			this.olvPositions.UseCompatibleStateImageBehavior = false;
			this.olvPositions.UseFilterIndicator = true;
			this.olvPositions.UseFiltering = true;
			this.olvPositions.UseHotItem = true;
			this.olvPositions.UseTranslucentHotItem = true;
			this.olvPositions.Name = "olvPositions";
			this.olvPositions.Size = new System.Drawing.Size(905, 303);
			//this.olvPositions.SmallImageList = this.tradeTypes;
			this.olvPositions.TabIndex = 0;
			this.olvPositions.UseCompatibleStateImageBehavior = false;
			this.olvPositions.View = System.Windows.Forms.View.Details;
			this.olvPositions.DoubleClick += new System.EventHandler(this.olvPositions_DoubleClick);
			// 
			// olvcPosition
			// 
			this.olvcPosition.CellPadding = null;
			this.olvcPosition.Text = "Position";
			this.olvcPosition.Width = 35;
			// 
			// olvcSerno
			// 
			this.olvcSerno.CellPadding = null;
			this.olvcSerno.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcSerno.Text = "Serno";
			this.olvcSerno.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcSerno.Width = 30;
			// 
			// olvcSymbol
			// 
			this.olvcSymbol.CellPadding = null;
			this.olvcSymbol.Text = "Symbol";
			this.olvcSymbol.Width = 40;
			// 
			// olvcQuantity
			// 
			this.olvcQuantity.CellPadding = null;
			this.olvcQuantity.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcQuantity.Text = "Shares";
			this.olvcQuantity.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcQuantity.Width = 19;
			// 
			// olvcEntryDate
			// 
			this.olvcEntryDate.CellPadding = null;
			this.olvcEntryDate.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcEntryDate.Text = "Entry Date";
			this.olvcEntryDate.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcEntryDate.Width = 108;
			// 
			// olvcEntryPrice
			// 
			this.olvcEntryPrice.CellPadding = null;
			this.olvcEntryPrice.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcEntryPrice.Text = "Entry Price";
			this.olvcEntryPrice.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcEntryPrice.Width = 55;
			// 
			// olvcEntryOrder
			// 
			this.olvcEntryOrder.CellPadding = null;
			this.olvcEntryOrder.Text = "EntryOrder";
			this.olvcEntryOrder.Width = 35;
			// 
			// olvcExitDate
			// 
			this.olvcExitDate.CellPadding = null;
			this.olvcExitDate.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcExitDate.Text = "Exit Date";
			this.olvcExitDate.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcExitDate.Width = 108;
			// 
			// olvcExitPrice
			// 
			this.olvcExitPrice.CellPadding = null;
			this.olvcExitPrice.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcExitPrice.Text = "Exit Price";
			this.olvcExitPrice.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcExitPrice.Width = 55;
			// 
			// olvcExitOrder
			// 
			this.olvcExitOrder.CellPadding = null;
			this.olvcExitOrder.Text = "ExitOrder";
			this.olvcExitOrder.Width = 35;
			// 
			// olvcProfitPct
			// 
			this.olvcProfitPct.CellPadding = null;
			this.olvcProfitPct.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcProfitPct.Text = "Profit %";
			this.olvcProfitPct.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcProfitPct.Width = 36;
			// 
			// olvcProfitDollar
			// 
			this.olvcProfitDollar.CellPadding = null;
			this.olvcProfitDollar.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcProfitDollar.Text = "Profit $";
			this.olvcProfitDollar.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcProfitDollar.Width = 45;
			// 
			// olvcBarsHeld
			// 
			this.olvcBarsHeld.CellPadding = null;
			this.olvcBarsHeld.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcBarsHeld.Text = "Bars Held";
			this.olvcBarsHeld.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcBarsHeld.Width = 31;
			// 
			// olvcProfitPerBar
			// 
			this.olvcProfitPerBar.CellPadding = null;
			this.olvcProfitPerBar.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcProfitPerBar.Text = "Profit per Bar";
			this.olvcProfitPerBar.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcProfitPerBar.Width = 40;
			// 
			// olvcEntrySignalName
			// 
			this.olvcEntrySignalName.CellPadding = null;
			this.olvcEntrySignalName.Text = "Entry Signal";
			this.olvcEntrySignalName.Width = 70;
			// 
			// olvcExitSignalName
			// 
			this.olvcExitSignalName.CellPadding = null;
			this.olvcExitSignalName.Text = "Exit Signal";
			this.olvcExitSignalName.Width = 70;
			// 
			// olvcMaePct
			// 
			this.olvcMaePct.CellPadding = null;
			this.olvcMaePct.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcMaePct.Text = "MAE %";
			this.olvcMaePct.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcMaePct.Width = 42;
			// 
			// olvcMfePct
			// 
			this.olvcMfePct.CellPadding = null;
			this.olvcMfePct.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcMfePct.Text = "MFE %";
			this.olvcMfePct.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcMfePct.Width = 42;
			// 
			// olvcCumProfitPct
			// 
			this.olvcCumProfitPct.CellPadding = null;
			this.olvcCumProfitPct.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcCumProfitPct.Text = "%Profit Cumulative";
			this.olvcCumProfitPct.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcCumProfitPct.Width = 41;
			// 
			// olvcCumProfitDollar
			// 
			this.olvcCumProfitDollar.CellPadding = null;
			this.olvcCumProfitDollar.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcCumProfitDollar.Text = "$Profit Cumulative";
			this.olvcCumProfitDollar.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcCumProfitDollar.Width = 50;
			// 
			// olvcComission
			// 
			this.olvcComission.CellPadding = null;
			this.olvcComission.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcComission.Text = "$Commission";
			this.olvcComission.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcComission.Width = 40;
			// 
			// ctxPopup
			// 
			this.ctxPopup.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
				this.mniShowEntriesExits,
				this.mniShowPercentage,
				this.mniShowBarsHeld,
				this.mniShowMaeMfe,
				this.mniShowSignals,
				this.mniShowCommission,
				this.toolStripSeparator2,
				this.mniColorify,
				this.toolStripSeparator1,
				this.mniCopyToClipboard,
				this.mniSaveToFile});
			this.ctxPopup.Name = "contextMenuStrip1";
			this.ctxPopup.Size = new System.Drawing.Size(183, 208);
			// 
			// mniShowEntriesExits
			// 
			this.mniShowEntriesExits.Checked = true;
			this.mniShowEntriesExits.CheckOnClick = true;
			this.mniShowEntriesExits.CheckState = System.Windows.Forms.CheckState.Checked;
			this.mniShowEntriesExits.Name = "mniShowEntriesExits";
			this.mniShowEntriesExits.Size = new System.Drawing.Size(182, 22);
			this.mniShowEntriesExits.Text = "Show EntriesExits";
			this.mniShowEntriesExits.Click += new System.EventHandler(this.mniShowEntriesExits_Click);
			// 
			// mniShowPercentage
			// 
			this.mniShowPercentage.Checked = true;
			this.mniShowPercentage.CheckOnClick = true;
			this.mniShowPercentage.CheckState = System.Windows.Forms.CheckState.Checked;
			this.mniShowPercentage.Name = "mniShowPercentage";
			this.mniShowPercentage.Size = new System.Drawing.Size(182, 22);
			this.mniShowPercentage.Text = "Show Percentages";
			this.mniShowPercentage.Click += new System.EventHandler(this.mniShowPercentage_Click);
			// 
			// mniShowBarsHeld
			// 
			this.mniShowBarsHeld.Checked = true;
			this.mniShowBarsHeld.CheckOnClick = true;
			this.mniShowBarsHeld.CheckState = System.Windows.Forms.CheckState.Checked;
			this.mniShowBarsHeld.Name = "mniShowBarsHeld";
			this.mniShowBarsHeld.Size = new System.Drawing.Size(182, 22);
			this.mniShowBarsHeld.Text = "Show BarsHeld";
			this.mniShowBarsHeld.Click += new System.EventHandler(this.mniShowBarsHeld_Click);
			// 
			// mniShowMaeMfe
			// 
			this.mniShowMaeMfe.Checked = true;
			this.mniShowMaeMfe.CheckOnClick = true;
			this.mniShowMaeMfe.CheckState = System.Windows.Forms.CheckState.Checked;
			this.mniShowMaeMfe.Name = "mniShowMaeMfe";
			this.mniShowMaeMfe.Size = new System.Drawing.Size(182, 22);
			this.mniShowMaeMfe.Text = "Show MAE MFE";
			this.mniShowMaeMfe.Click += new System.EventHandler(this.mniShowMaeMfe_Click);
			// 
			// mniShowSignals
			// 
			this.mniShowSignals.Checked = true;
			this.mniShowSignals.CheckOnClick = true;
			this.mniShowSignals.CheckState = System.Windows.Forms.CheckState.Checked;
			this.mniShowSignals.Name = "mniShowSignals";
			this.mniShowSignals.Size = new System.Drawing.Size(182, 22);
			this.mniShowSignals.Text = "Show Signals";
			this.mniShowSignals.Click += new System.EventHandler(this.mniShowSignals_Click);
			// 
			// mniShowCommission
			// 
			this.mniShowCommission.Checked = true;
			this.mniShowCommission.CheckOnClick = true;
			this.mniShowCommission.CheckState = System.Windows.Forms.CheckState.Checked;
			this.mniShowCommission.Name = "mniShowCommission";
			this.mniShowCommission.Size = new System.Drawing.Size(182, 22);
			this.mniShowCommission.Text = "Show Commission";
			this.mniShowCommission.Click += new System.EventHandler(this.mniShowCommission_Click);
			// 
			// mniColorify
			// 
			this.mniColorify.Checked = true;
			this.mniColorify.CheckOnClick = true;
			this.mniColorify.CheckState = System.Windows.Forms.CheckState.Checked;
			this.mniColorify.Name = "mniColorify";
			this.mniColorify.Size = new System.Drawing.Size(182, 22);
			this.mniColorify.Text = "Colorify (slow)";
			this.mniColorify.Click += new System.EventHandler(this.mniColorify_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(179, 6);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(179, 6);
			// 
			// mniCopyToClipboard
			// 
			this.mniCopyToClipboard.Name = "mniCopyToClipboard";
			this.mniCopyToClipboard.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
			this.mniCopyToClipboard.Size = new System.Drawing.Size(182, 22);
			this.mniCopyToClipboard.Text = "Copy";
			this.mniCopyToClipboard.Click += new System.EventHandler(this.mniCopyToClipboard_Click);
			// 
			// mniSaveToFile
			// 
			this.mniSaveToFile.Name = "mniSaveToFile";
			this.mniSaveToFile.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
			this.mniSaveToFile.Size = new System.Drawing.Size(182, 22);
			this.mniSaveToFile.Text = "Save (GUI-BLOCKING !!!)";
			this.mniSaveToFile.Visible = false;
			this.mniSaveToFile.Click += new System.EventHandler(this.mniSaveToFile_Click);
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

		private ToolStripSeparator toolStripSeparator1;
		private ToolStripSeparator toolStripSeparator2;
		private ToolStripMenuItem mniCopyToClipboard;
		private ToolStripMenuItem mniSaveToFile;
	}
}