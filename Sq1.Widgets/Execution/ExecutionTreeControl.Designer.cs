using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Sq1.Widgets.Execution {
	public partial class ExecutionTreeControl : UserControl {
		private IContainer components;
		protected override void Dispose(bool disposing) {
			if (disposing && this.components != null) {
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.OrdersTreeOLV = new BrightIdeasSoftware.TreeListView();
			this.colheGUID = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.colheReplacedByGUID = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.colheKilledByGUID = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.colheState = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.colheStateTime = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.colheBarNum = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.colheOrderCreated = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.colheSymbol = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.colheDirection = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.colheOrderType = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.colheSpreadSide = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.colhePriceScript = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.colheSlippage = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.colhePriceScriptRequested = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.colhePriceFilled = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.colhePriceDeposited = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.colheQtyRequested = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.colheQtyFilled = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.colheSernoSession = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.colheSernoExchange = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.colheStrategyName = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.colheSignalName = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.colheScale = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.colheAccount = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.colheLastMessage = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.ctxOrder = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.mniKillPending = new System.Windows.Forms.ToolStripMenuItem();
			this.mniOrderReplace = new System.Windows.Forms.ToolStripMenuItem();
			this.sepCancel = new System.Windows.Forms.ToolStripSeparator();
			this.mniKillAllPending = new System.Windows.Forms.ToolStripMenuItem();
			this.mniKillPendingAllStopEmitting = new System.Windows.Forms.ToolStripMenuItem();
			this.mniStopEmergencyClose = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.mniFilterColumns = new System.Windows.Forms.ToolStripMenuItem();
			this.ctxColumns = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.mniShowWhenWhat = new System.Windows.Forms.ToolStripMenuItem();
			this.mniShowKilledReplaced = new System.Windows.Forms.ToolStripMenuItem();
			this.mniShowPrice = new System.Windows.Forms.ToolStripMenuItem();
			this.mniShowQty = new System.Windows.Forms.ToolStripMenuItem();
			this.mniShowExchange = new System.Windows.Forms.ToolStripMenuItem();
			this.mniShowOrigin = new System.Windows.Forms.ToolStripMenuItem();
			this.mniShowExtra = new System.Windows.Forms.ToolStripMenuItem();
			this.mniShowPosition = new System.Windows.Forms.ToolStripMenuItem();
			this.mniShowLastMessage = new System.Windows.Forms.ToolStripMenuItem();
			this.mniFilterOrderStates = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
			this.mniFilterAccounts = new System.Windows.Forms.ToolStripMenuItem();
			this.ctxAccounts = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.mniVisualOptions = new System.Windows.Forms.ToolStripMenuItem();
			this.ctxListControl = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.mniToggleBrokerTime = new System.Windows.Forms.ToolStripMenuItem();
			this.mniToggleCompletedOrders = new System.Windows.Forms.ToolStripMenuItem();
			this.mniToggleMessagesPane = new System.Windows.Forms.ToolStripMenuItem();
			this.mniToggleMessagesPaneSplitHorizontally = new System.Windows.Forms.ToolStripMenuItem();
			this.mniToggleSyncWithChart = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.mniExpandAll = new System.Windows.Forms.ToolStripMenuItem();
			this.mniCollapseAll = new System.Windows.Forms.ToolStripMenuItem();
			this.mniRebuildAll = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.mniWipeWhiteboard = new System.Windows.Forms.ToolStripMenuItem();
			this.imgListOrderDirection = new System.Windows.Forms.ImageList(this.components);
			this.olvMessages = new BrightIdeasSoftware.ObjectListView();
			this.colheMessageDateTime = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.colheMessageState = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.colheMessageText = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.splitContainerMessagePane = new System.Windows.Forms.SplitContainer();
			((System.ComponentModel.ISupportInitialize)(this.OrdersTreeOLV)).BeginInit();
			this.ctxOrder.SuspendLayout();
			this.ctxColumns.SuspendLayout();
			this.ctxListControl.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.olvMessages)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerMessagePane)).BeginInit();
			this.splitContainerMessagePane.Panel1.SuspendLayout();
			this.splitContainerMessagePane.Panel2.SuspendLayout();
			this.splitContainerMessagePane.SuspendLayout();
			this.SuspendLayout();
			// 
			// OrdersTreeOLV
			// 
			this.OrdersTreeOLV.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
			this.colheGUID,
			this.colheReplacedByGUID,
			this.colheKilledByGUID,
			this.colheState,
			this.colheStateTime,
			this.colheBarNum,
			this.colheOrderCreated,
			this.colheSymbol,
			this.colheDirection,
			this.colheOrderType,
			this.colheSpreadSide,
			this.colhePriceScript,
			this.colheSlippage,
			this.colhePriceScriptRequested,
			this.colhePriceFilled,
			this.colhePriceDeposited,
			this.colheQtyRequested,
			this.colheQtyFilled,
			this.colheSernoSession,
			this.colheSernoExchange,
			this.colheStrategyName,
			this.colheSignalName,
			this.colheScale,
			this.colheAccount,
			this.colheLastMessage});
			this.OrdersTreeOLV.AllowColumnReorder = true;
			this.OrdersTreeOLV.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.OrdersTreeOLV.CausesValidation = false;
			this.OrdersTreeOLV.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.F2Only;
			this.OrdersTreeOLV.ContextMenuStrip = this.ctxOrder;
			this.OrdersTreeOLV.Dock = System.Windows.Forms.DockStyle.Fill;
			this.OrdersTreeOLV.FullRowSelect = true;
			this.OrdersTreeOLV.HideSelection = false;
			this.OrdersTreeOLV.IncludeColumnHeadersInCopy = true;
			this.OrdersTreeOLV.IncludeHiddenColumnsInDataTransfer = true;
			this.OrdersTreeOLV.Location = new System.Drawing.Point(0, 0);
			this.OrdersTreeOLV.MultiSelect = true;
			this.OrdersTreeOLV.Name = "OrdersTreeOLV";
			this.OrdersTreeOLV.ShowGroups = false;
			this.OrdersTreeOLV.ShowCommandMenuOnRightClick = true;
			this.OrdersTreeOLV.ShowItemToolTips = true;
			this.OrdersTreeOLV.Size = new System.Drawing.Size(334, 411);
			this.OrdersTreeOLV.SmallImageList = this.imgListOrderDirection;
			this.OrdersTreeOLV.TabIndex = 18;
			this.OrdersTreeOLV.TintSortColumn = true;
			this.OrdersTreeOLV.UnfocusedHighlightBackgroundColor = System.Drawing.SystemColors.GradientActiveCaption;
			this.OrdersTreeOLV.UseCompatibleStateImageBehavior = false;
			this.OrdersTreeOLV.UseFilterIndicator = true;
			this.OrdersTreeOLV.UseFiltering = true;
			this.OrdersTreeOLV.UseHotItem = true;
			this.OrdersTreeOLV.UseTranslucentHotItem = true;
			this.OrdersTreeOLV.View = System.Windows.Forms.View.Details;
			this.OrdersTreeOLV.FormatRow += new System.EventHandler<BrightIdeasSoftware.FormatRowEventArgs>(this.tree_FormatRow);
			this.OrdersTreeOLV.SelectedIndexChanged += new System.EventHandler(this.ordersTree_SelectedIndexChanged);
			this.OrdersTreeOLV.DoubleClick += new System.EventHandler(this.ordersTree_DoubleClick);
			this.OrdersTreeOLV.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ordersTree_KeyDown);
			// 
			// colheGUID
			// 
			this.colheGUID.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.colheGUID.Text = "GUID";
			this.colheGUID.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.colheGUID.Width = 95;
			// 
			// colheReplacedByGUID
			// 
			this.colheReplacedByGUID.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.colheReplacedByGUID.Text = "ReplcdBy";
			this.colheReplacedByGUID.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.colheReplacedByGUID.Width = 67;
			// 
			// colheKilledByGUID
			// 
			this.colheKilledByGUID.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.colheKilledByGUID.Text = "KilledBy";
			this.colheKilledByGUID.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.colheKilledByGUID.Width = 67;
			// 
			// colheState
			// 
			this.colheState.Text = "OrderState";
			this.colheState.Width = 111;
			// 
			// colheStateTime
			// 
			this.colheStateTime.Text = "LastOrderState";
			this.colheStateTime.Width = 84;
			// 
			// colheBarNum
			// 
			this.colheBarNum.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.colheBarNum.Text = "#Bar";
			this.colheBarNum.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.colheBarNum.Width = 52;
			// 
			// colheOrderCreated
			// 
			this.colheOrderCreated.Text = "Created";
			this.colheOrderCreated.Width = 84;
			// 
			// colheSymbol
			// 
			this.colheSymbol.Text = "Symbol";
			this.colheSymbol.Width = 42;
			// 
			// colheDirection
			// 
			this.colheDirection.Text = "Direction";
			this.colheDirection.Width = 51;
			// 
			// colheOrderType
			// 
			this.colheOrderType.Text = "OrderType";
			this.colheOrderType.Width = 56;
			// 
			// colheSpreadSide
			// 
			this.colheSpreadSide.Text = "SpreadSide";
			this.colheSpreadSide.Width = 100;
			// 
			// colhePriceScript
			// 
			this.colhePriceScript.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.colhePriceScript.Text = "$Script";
			this.colhePriceScript.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.colhePriceScript.Width = 53;
			// 
			// colheSlippage
			// 
			this.colheSlippage.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.colheSlippage.Text = "Slippage";
			this.colheSlippage.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.colheSlippage.Width = 30;
			// 
			// colhePriceScriptRequested
			// 
			this.colhePriceScriptRequested.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.colhePriceScriptRequested.Text = "$Requested";
			this.colhePriceScriptRequested.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.colhePriceScriptRequested.Width = 51;
			// 
			// colhePriceFilled
			// 
			this.colhePriceFilled.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.colhePriceFilled.Text = "$Filled";
			this.colhePriceFilled.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.colhePriceFilled.Width = 53;
			// 
			// colhePriceDeposited
			// 
			this.colhePriceDeposited.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.colhePriceDeposited.Text = "$Deposited";
			this.colhePriceDeposited.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.colhePriceDeposited.Width = 53;
			// 
			// colheQtyRequested
			// 
			this.colheQtyRequested.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.colheQtyRequested.Text = "QRequested";
			this.colheQtyRequested.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.colheQtyRequested.Width = 25;
			// 
			// colheQtyFilled
			// 
			this.colheQtyFilled.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.colheQtyFilled.Text = "QFilled";
			this.colheQtyFilled.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.colheQtyFilled.Width = 25;
			// 
			// colheSernoSession
			// 
			this.colheSernoSession.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.colheSernoSession.Text = "#Session";
			this.colheSernoSession.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.colheSernoSession.Width = 36;
			// 
			// colheSernoExchange
			// 
			this.colheSernoExchange.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.colheSernoExchange.Text = "#Exchange";
			this.colheSernoExchange.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.colheSernoExchange.Width = 74;
			// 
			// colheStrategyName
			// 
			this.colheStrategyName.Text = "Strategy";
			this.colheStrategyName.Width = 53;
			// 
			// colheSignalName
			// 
			this.colheSignalName.Text = "Signal";
			this.colheSignalName.Width = 42;
			// 
			// colheScale
			// 
			this.colheScale.Text = "Scale";
			this.colheScale.Width = 40;
			// 
			// colheAccount
			// 
			this.colheAccount.Text = "Account";
			this.colheAccount.Width = 40;
			// 
			// colheLastMessage
			// 
			this.colheLastMessage.FillsFreeSpace = true;
			this.colheLastMessage.Text = "LastMessage";
			this.colheLastMessage.Width = 400;
			// 
			// ctxOrder
			// 
			this.ctxOrder.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.mniKillPending,
			this.mniKillAllPending,
			this.mniKillPendingAllStopEmitting,
			this.sepCancel,
			this.mniOrderReplace,
			this.mniStopEmergencyClose,
			this.toolStripSeparator3,
			this.mniFilterColumns,
			this.mniFilterOrderStates,
			this.mniFilterAccounts,
			this.mniVisualOptions,
			this.toolStripSeparator1,
			this.mniWipeWhiteboard});
			this.ctxOrder.Name = "popupOrders";
			this.ctxOrder.Size = new System.Drawing.Size(233, 264);
			// 
			// mniKillPending
			// 
			this.mniKillPending.Name = "mniKillPending";
			this.mniKillPending.Size = new System.Drawing.Size(232, 22);
			this.mniKillPending.Text = "Kill Pending";
			this.mniKillPending.Click += new System.EventHandler(this.mniOrderKill_Click);
			// 
			// mniOrderReplace
			// 
			this.mniOrderReplace.Name = "mniOrderReplace";
			this.mniOrderReplace.Size = new System.Drawing.Size(232, 22);
			this.mniOrderReplace.Text = "Replace NYI";
			this.mniOrderReplace.Click += new System.EventHandler(this.mniOrderReplace_Click);
			// 
			// sepCancel
			// 
			this.sepCancel.Name = "sepCancel";
			this.sepCancel.Size = new System.Drawing.Size(229, 6);
			// 
			// mniKillAllPending
			// 
			this.mniKillAllPending.Name = "mniKillAllPending";
			this.mniKillAllPending.Size = new System.Drawing.Size(232, 22);
			this.mniKillAllPending.Text = "Kill Pending All";
			this.mniKillAllPending.ToolTipText = "Cancel all Active Orders and DOESN\'T disable Auto-Trading";
			this.mniKillAllPending.Click += new System.EventHandler(this.mniOrdersCancel_Click);
			// 
			// mniKillPendingAllStopEmitting
			// 
			this.mniKillPendingAllStopEmitting.Name = "mniKillPendingAllStopEmitting";
			this.mniKillPendingAllStopEmitting.Size = new System.Drawing.Size(232, 22);
			this.mniKillPendingAllStopEmitting.Text = "Kill Pending All, Stop Emitting";
			this.mniKillPendingAllStopEmitting.ToolTipText = "Kill all (even Completed) Orders and DISABLE AutoSubmit";
			this.mniKillPendingAllStopEmitting.Click += new System.EventHandler(this.mniKillAllStopAutoSubmit_Click);
			// 
			// mniStopEmergencyClose
			// 
			this.mniStopEmergencyClose.Name = "mniStopEmergencyClose";
			this.mniStopEmergencyClose.Size = new System.Drawing.Size(232, 22);
			this.mniStopEmergencyClose.Text = "Stop Emergency Close";
			this.mniStopEmergencyClose.Click += new System.EventHandler(this.mniEmergencyLockRemove_Click);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(229, 6);
			// 
			// mniFilterColumns
			// 
			this.mniFilterColumns.DropDown = this.ctxColumns;
			this.mniFilterColumns.Name = "mniFilterColumns";
			this.mniFilterColumns.Size = new System.Drawing.Size(232, 22);
			this.mniFilterColumns.Text = "Filter Columns";
			// 
			// ctxColumns
			// 
			this.ctxColumns.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.mniShowWhenWhat,
			this.mniShowKilledReplaced,
			this.mniShowPrice,
			this.mniShowQty,
			this.mniShowExchange,
			this.mniShowOrigin,
			this.mniShowExtra,
			this.mniShowPosition,
			this.mniShowLastMessage});
			this.ctxColumns.Name = "ctxColumns";
			this.ctxColumns.Size = new System.Drawing.Size(189, 202);
			this.ctxColumns.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.ctxColumns_ItemClicked);
			// 
			// mniShowWhenWhat
			// 
			this.mniShowWhenWhat.Checked = true;
			this.mniShowWhenWhat.CheckState = System.Windows.Forms.CheckState.Checked;
			this.mniShowWhenWhat.Name = "mniShowWhenWhat";
			this.mniShowWhenWhat.Size = new System.Drawing.Size(188, 22);
			this.mniShowWhenWhat.Text = "Show WhenWhat";
			// 
			// mniShowKilledReplaced
			// 
			this.mniShowKilledReplaced.Checked = true;
			this.mniShowKilledReplaced.CheckState = System.Windows.Forms.CheckState.Checked;
			this.mniShowKilledReplaced.Name = "mniShowKilledReplaced";
			this.mniShowKilledReplaced.Size = new System.Drawing.Size(188, 22);
			this.mniShowKilledReplaced.Text = "Show Killed/Replaced";
			// 
			// mniShowPrice
			// 
			this.mniShowPrice.Checked = true;
			this.mniShowPrice.CheckState = System.Windows.Forms.CheckState.Checked;
			this.mniShowPrice.Name = "mniShowPrice";
			this.mniShowPrice.Size = new System.Drawing.Size(188, 22);
			this.mniShowPrice.Text = "Show Price";
			// 
			// mniShowQty
			// 
			this.mniShowQty.Checked = true;
			this.mniShowQty.CheckState = System.Windows.Forms.CheckState.Checked;
			this.mniShowQty.Name = "mniShowQty";
			this.mniShowQty.Size = new System.Drawing.Size(188, 22);
			this.mniShowQty.Text = "Show Qnty";
			// 
			// mniShowExchange
			// 
			this.mniShowExchange.Checked = true;
			this.mniShowExchange.CheckState = System.Windows.Forms.CheckState.Checked;
			this.mniShowExchange.Name = "mniShowExchange";
			this.mniShowExchange.Size = new System.Drawing.Size(188, 22);
			this.mniShowExchange.Text = "Show Exchange";
			// 
			// mniShowOrigin
			// 
			this.mniShowOrigin.Checked = true;
			this.mniShowOrigin.CheckState = System.Windows.Forms.CheckState.Checked;
			this.mniShowOrigin.Name = "mniShowOrigin";
			this.mniShowOrigin.Size = new System.Drawing.Size(188, 22);
			this.mniShowOrigin.Text = "Show Origin";
			// 
			// mniShowExtra
			// 
			this.mniShowExtra.Checked = true;
			this.mniShowExtra.CheckState = System.Windows.Forms.CheckState.Checked;
			this.mniShowExtra.Name = "mniShowExtra";
			this.mniShowExtra.Size = new System.Drawing.Size(188, 22);
			this.mniShowExtra.Text = "Show Extra";
			// 
			// mniShowPosition
			// 
			this.mniShowPosition.Checked = true;
			this.mniShowPosition.CheckState = System.Windows.Forms.CheckState.Checked;
			this.mniShowPosition.Name = "mniShowPosition";
			this.mniShowPosition.Size = new System.Drawing.Size(188, 22);
			this.mniShowPosition.Text = "Show Position";
			// 
			// mniShowLastMessage
			// 
			this.mniShowLastMessage.Checked = true;
			this.mniShowLastMessage.CheckState = System.Windows.Forms.CheckState.Checked;
			this.mniShowLastMessage.Name = "mniShowLastMessage";
			this.mniShowLastMessage.Size = new System.Drawing.Size(188, 22);
			this.mniShowLastMessage.Text = "Show Last Message";
			// 
			// mniFilterOrderStates
			// 
			this.mniFilterOrderStates.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.toolStripMenuItem4});
			this.mniFilterOrderStates.Name = "mniFilterOrderStates";
			this.mniFilterOrderStates.Size = new System.Drawing.Size(232, 22);
			this.mniFilterOrderStates.Text = "Filter Order States";
			// 
			// toolStripMenuItem4
			// 
			this.toolStripMenuItem4.Name = "toolStripMenuItem4";
			this.toolStripMenuItem4.Size = new System.Drawing.Size(92, 22);
			this.toolStripMenuItem4.Text = "999";
			// 
			// mniFilterAccounts
			// 
			this.mniFilterAccounts.DropDown = this.ctxAccounts;
			this.mniFilterAccounts.Name = "mniFilterAccounts";
			this.mniFilterAccounts.Size = new System.Drawing.Size(232, 22);
			this.mniFilterAccounts.Text = "Filter Accounts";
			// 
			// ctxAccounts
			// 
			this.ctxAccounts.Name = "ctxAccounts";
			this.ctxAccounts.Size = new System.Drawing.Size(61, 4);
			this.ctxAccounts.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.ctxAccounts_ItemClicked);
			// 
			// mniVisualOptions
			// 
			this.mniVisualOptions.DropDown = this.ctxListControl;
			this.mniVisualOptions.Name = "mniVisualOptions";
			this.mniVisualOptions.Size = new System.Drawing.Size(232, 22);
			this.mniVisualOptions.Text = "Toggles";
			// 
			// ctxListControl
			// 
			this.ctxListControl.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.mniToggleBrokerTime,
			this.mniToggleCompletedOrders,
			this.mniToggleMessagesPane,
			this.mniToggleMessagesPaneSplitHorizontally,
			this.mniToggleSyncWithChart,
			this.toolStripSeparator2,
			this.mniExpandAll,
			this.mniCollapseAll,
			this.mniRebuildAll});
			this.ctxListControl.Name = "ctxListControl";
			this.ctxListControl.OwnerItem = this.mniVisualOptions;
			this.ctxListControl.Size = new System.Drawing.Size(254, 186);
			// 
			// mniToggleBrokerTime
			// 
			this.mniToggleBrokerTime.CheckOnClick = true;
			this.mniToggleBrokerTime.Name = "mniToggleBrokerTime";
			this.mniToggleBrokerTime.Size = new System.Drawing.Size(253, 22);
			this.mniToggleBrokerTime.Text = "Show Broker Time";
			this.mniToggleBrokerTime.Click += new System.EventHandler(this.mniToggleBrokerTime_Click);
			// 
			// mniToggleCompletedOrders
			// 
			this.mniToggleCompletedOrders.CheckOnClick = true;
			this.mniToggleCompletedOrders.Name = "mniToggleCompletedOrders";
			this.mniToggleCompletedOrders.Size = new System.Drawing.Size(253, 22);
			this.mniToggleCompletedOrders.Text = "Show Completed Orders";
			this.mniToggleCompletedOrders.Click += new System.EventHandler(this.mniToggleCompletedOrders_Click);
			// 
			// mniToggleMessagesPane
			// 
			this.mniToggleMessagesPane.Checked = true;
			this.mniToggleMessagesPane.CheckOnClick = true;
			this.mniToggleMessagesPane.CheckState = System.Windows.Forms.CheckState.Checked;
			this.mniToggleMessagesPane.Name = "mniToggleMessagesPane";
			this.mniToggleMessagesPane.Size = new System.Drawing.Size(253, 22);
			this.mniToggleMessagesPane.Text = "Show Messages Pane";
			this.mniToggleMessagesPane.Click += new System.EventHandler(this.mniToggleMessagesPane_Click);
			// 
			// mniToggleMessagesPaneSplitHorizontally
			// 
			this.mniToggleMessagesPaneSplitHorizontally.Checked = true;
			this.mniToggleMessagesPaneSplitHorizontally.CheckOnClick = true;
			this.mniToggleMessagesPaneSplitHorizontally.CheckState = System.Windows.Forms.CheckState.Checked;
			this.mniToggleMessagesPaneSplitHorizontally.Name = "mniToggleMessagesPaneSplitHorizontally";
			this.mniToggleMessagesPaneSplitHorizontally.Size = new System.Drawing.Size(253, 22);
			this.mniToggleMessagesPaneSplitHorizontally.Text = "Show Messages Pane Horizontally";
			this.mniToggleMessagesPaneSplitHorizontally.Click += new System.EventHandler(this.mniToggleMessagesPaneSplitHorizontally_Click);
			// 
			// mniToggleSyncWithChart
			// 
			this.mniToggleSyncWithChart.CheckOnClick = true;
			this.mniToggleSyncWithChart.Name = "mniToggleSyncWithChart";
			this.mniToggleSyncWithChart.Size = new System.Drawing.Size(253, 22);
			this.mniToggleSyncWithChart.Text = "Show PositionAffected On Chart";
			this.mniToggleSyncWithChart.Click += new System.EventHandler(this.mniToggleSyncWithChart_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(250, 6);
			// 
			// mniExpandAll
			// 
			this.mniExpandAll.Name = "mniExpandAll";
			this.mniExpandAll.Size = new System.Drawing.Size(253, 22);
			this.mniExpandAll.Text = "Expand All";
			this.mniExpandAll.Click += new System.EventHandler(this.mniTreeExpandAll_Click);
			// 
			// mniCollapseAll
			// 
			this.mniCollapseAll.Name = "mniCollapseAll";
			this.mniCollapseAll.Size = new System.Drawing.Size(253, 22);
			this.mniCollapseAll.Text = "Collapse All";
			this.mniCollapseAll.Click += new System.EventHandler(this.mniTreeCollapseAll_Click);
			// 
			// mniRebuildAll
			// 
			this.mniRebuildAll.Name = "mniRebuildAll";
			this.mniRebuildAll.Size = new System.Drawing.Size(253, 22);
			this.mniRebuildAll.Text = "Rebuild All";
			this.mniRebuildAll.Click += new System.EventHandler(this.mniRebuildAll_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(229, 6);
			// 
			// mniWipeWhiteboard
			// 
			this.mniWipeWhiteboard.Name = "mniWipeWhiteboard";
			this.mniWipeWhiteboard.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.C)));
			this.mniWipeWhiteboard.Size = new System.Drawing.Size(232, 22);
			this.mniWipeWhiteboard.Text = "Wipe Whiteboard";
			this.mniWipeWhiteboard.Click += new System.EventHandler(this.mniOrdersRemoveSelected_Click);
			// 
			// imgListOrderDirection
			// 
			this.imgListOrderDirection.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
			this.imgListOrderDirection.ImageSize = new System.Drawing.Size(16, 16);
			this.imgListOrderDirection.TransparentColor = System.Drawing.Color.Silver;
			// 
			// olvMessages
			// 
			this.olvMessages.Activation = System.Windows.Forms.ItemActivation.OneClick;
			this.olvMessages.AllowColumnReorder = true;
			this.olvMessages.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.olvMessages.CausesValidation = false;
			this.olvMessages.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.F2Only;
			this.olvMessages.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
			this.colheMessageDateTime,
			this.colheMessageState,
			this.colheMessageText});
			this.olvMessages.Dock = System.Windows.Forms.DockStyle.Fill;
			this.olvMessages.EmptyListMsg = "";
			this.olvMessages.FullRowSelect = true;
			this.olvMessages.HideSelection = false;
			this.olvMessages.IncludeColumnHeadersInCopy = true;
			this.olvMessages.IncludeHiddenColumnsInDataTransfer = true;
			this.olvMessages.Location = new System.Drawing.Point(0, 0);
			this.olvMessages.Name = "olvMessages";
			this.olvMessages.ShowGroups = false;
			this.olvMessages.ShowItemToolTips = true;
			this.olvMessages.Size = new System.Drawing.Size(495, 411);
			this.olvMessages.TabIndex = 5;
			this.olvMessages.TintSortColumn = true;
			this.olvMessages.UnfocusedHighlightBackgroundColor = System.Drawing.SystemColors.GradientActiveCaption;
			this.olvMessages.UseCompatibleStateImageBehavior = false;
			this.olvMessages.UseFilterIndicator = true;
			this.olvMessages.UseFiltering = true;
			this.olvMessages.View = System.Windows.Forms.View.Details;
			// 
			// colheMessageDateTime
			// 
			this.colheMessageDateTime.Text = "DateTime";
			this.colheMessageDateTime.Width = 83;
			// 
			// colheMessageState
			// 
			this.colheMessageState.Text = "State";
			this.colheMessageState.Width = 101;
			// 
			// colheMessageText
			// 
			this.colheMessageText.Text = "Message";
			this.colheMessageText.Width = 2211;
			// 
			// splitContainerMessagePane
			// 
			this.splitContainerMessagePane.BackColor = System.Drawing.SystemColors.ControlLight;
			this.splitContainerMessagePane.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainerMessagePane.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainerMessagePane.Location = new System.Drawing.Point(0, 0);
			this.splitContainerMessagePane.Name = "splitContainerMessagePane";
			// 
			// splitContainerMessagePane.Panel1
			// 
			this.splitContainerMessagePane.Panel1.BackColor = System.Drawing.SystemColors.Control;
			this.splitContainerMessagePane.Panel1.Controls.Add(this.OrdersTreeOLV);
			// 
			// splitContainerMessagePane.Panel2
			// 
			this.splitContainerMessagePane.Panel2.BackColor = System.Drawing.SystemColors.Control;
			this.splitContainerMessagePane.Panel2.Controls.Add(this.olvMessages);
			this.splitContainerMessagePane.Size = new System.Drawing.Size(833, 411);
			this.splitContainerMessagePane.SplitterDistance = 334;
			//this.splitContainerMessagePane.SplitterIncrement = 20;
			this.splitContainerMessagePane.TabIndex = 22;
			// 
			// ExecutionTreeControl
			// 
			this.Controls.Add(this.splitContainerMessagePane);
			this.Name = "ExecutionTreeControl";
			this.Size = new System.Drawing.Size(833, 411);
			((System.ComponentModel.ISupportInitialize)(this.OrdersTreeOLV)).EndInit();
			this.ctxOrder.ResumeLayout(false);
			this.ctxColumns.ResumeLayout(false);
			this.ctxListControl.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.olvMessages)).EndInit();
			this.splitContainerMessagePane.Panel1.ResumeLayout(false);
			this.splitContainerMessagePane.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainerMessagePane)).EndInit();
			this.splitContainerMessagePane.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		private System.Windows.Forms.ToolStripMenuItem mniRebuildAll;
		private System.Windows.Forms.ToolStripMenuItem mniToggleBrokerTime;
		private System.Windows.Forms.ToolStripMenuItem mniFilterAccounts;
		private System.Windows.Forms.ToolStripMenuItem mniFilterOrderStates;
		private System.Windows.Forms.ToolStripMenuItem mniFilterColumns;
		private System.Windows.Forms.ToolStripMenuItem mniToggleMessagesPane;
		private System.Windows.Forms.ToolStripMenuItem mniToggleMessagesPaneSplitHorizontally;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		private System.Windows.Forms.ToolStripMenuItem mniToggleCompletedOrders;
		private System.Windows.Forms.ToolStripMenuItem mniToggleSyncWithChart;
		private System.Windows.Forms.ToolStripMenuItem mniVisualOptions;
		private System.Windows.Forms.ContextMenuStrip ctxListControl;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripMenuItem mniCollapseAll;
		private System.Windows.Forms.ToolStripMenuItem mniExpandAll;
		private System.Windows.Forms.SplitContainer splitContainerMessagePane;
		private BrightIdeasSoftware.OLVColumn colheMessageText;
		private BrightIdeasSoftware.OLVColumn colheMessageState;
		private BrightIdeasSoftware.OLVColumn colheMessageDateTime;
		private BrightIdeasSoftware.ObjectListView olvMessages;
		private System.Windows.Forms.ToolStripMenuItem mniKillPendingAllStopEmitting;
		private System.Windows.Forms.ToolStripMenuItem mniStopEmergencyClose;
		private System.Windows.Forms.ToolStripMenuItem mniKillAllPending;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ContextMenuStrip ctxAccounts;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem4;
		private System.Windows.Forms.ToolStripMenuItem mniShowLastMessage;
		private System.Windows.Forms.ToolStripMenuItem mniShowPosition;
		private System.Windows.Forms.ToolStripMenuItem mniShowKilledReplaced;
		private System.Windows.Forms.ToolStripMenuItem mniShowExtra;
		private System.Windows.Forms.ToolStripMenuItem mniShowOrigin;
		private System.Windows.Forms.ToolStripMenuItem mniShowExchange;
		private System.Windows.Forms.ToolStripMenuItem mniShowQty;
		private System.Windows.Forms.ToolStripMenuItem mniShowPrice;
		private System.Windows.Forms.ToolStripMenuItem mniShowWhenWhat;
		private System.Windows.Forms.ContextMenuStrip ctxColumns;
		private System.Windows.Forms.ToolStripMenuItem mniWipeWhiteboard;
		private System.Windows.Forms.ToolStripSeparator sepCancel;
		private System.Windows.Forms.ToolStripMenuItem mniOrderReplace;
		private System.Windows.Forms.ToolStripMenuItem mniKillPending;
		private System.Windows.Forms.ContextMenuStrip ctxOrder;
		private System.Windows.Forms.ImageList imgListOrderDirection;
		public BrightIdeasSoftware.TreeListView OrdersTreeOLV;
		
		private BrightIdeasSoftware.OLVColumn colheOrderCreated;
		private BrightIdeasSoftware.OLVColumn colheBarNum;
		private BrightIdeasSoftware.OLVColumn colhePriceScript;
		private BrightIdeasSoftware.OLVColumn colheSernoSession;
		private BrightIdeasSoftware.OLVColumn colheSernoExchange;
		private BrightIdeasSoftware.OLVColumn colheGUID;
		private BrightIdeasSoftware.OLVColumn colheReplacedByGUID;
		private BrightIdeasSoftware.OLVColumn colheKilledByGUID;
		private BrightIdeasSoftware.OLVColumn colheStateTime;
		private BrightIdeasSoftware.OLVColumn colheState;
		private BrightIdeasSoftware.OLVColumn colheSymbol;
		private BrightIdeasSoftware.OLVColumn colheDirection;
		private BrightIdeasSoftware.OLVColumn colheOrderType;
		private BrightIdeasSoftware.OLVColumn colheQtyRequested;
		private BrightIdeasSoftware.OLVColumn colhePriceScriptRequested;
		private BrightIdeasSoftware.OLVColumn colheQtyFilled;
		private BrightIdeasSoftware.OLVColumn colhePriceFilled;
		private BrightIdeasSoftware.OLVColumn colhePriceDeposited;
		private BrightIdeasSoftware.OLVColumn colheSlippage;
		private BrightIdeasSoftware.OLVColumn colheSpreadSide;
		private BrightIdeasSoftware.OLVColumn colheStrategyName;
		private BrightIdeasSoftware.OLVColumn colheSignalName;
		private BrightIdeasSoftware.OLVColumn colheScale;
		private BrightIdeasSoftware.OLVColumn colheAccount;
		private BrightIdeasSoftware.OLVColumn colheLastMessage;
	}
}
