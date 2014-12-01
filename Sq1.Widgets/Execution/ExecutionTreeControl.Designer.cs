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
			this.OrdersTree = new BrightIdeasSoftware.TreeListView();
			this.colheGUID = new BrightIdeasSoftware.OLVColumn();
			this.colheReplacedByGUID = new BrightIdeasSoftware.OLVColumn();
			this.colheKilledByGUID = new BrightIdeasSoftware.OLVColumn();
			this.colheBarNum = new BrightIdeasSoftware.OLVColumn();
			this.colheDatetime = new BrightIdeasSoftware.OLVColumn();
			this.colheSymbol = new BrightIdeasSoftware.OLVColumn();
			this.colheDirection = new BrightIdeasSoftware.OLVColumn();
			this.colheOrderType = new BrightIdeasSoftware.OLVColumn();
			this.colheSpreadSide = new BrightIdeasSoftware.OLVColumn();
			this.colhePriceScript = new BrightIdeasSoftware.OLVColumn();
			this.colheSlippage = new BrightIdeasSoftware.OLVColumn();
			this.colhePriceScriptRequested = new BrightIdeasSoftware.OLVColumn();
			this.colhePriceFilled = new BrightIdeasSoftware.OLVColumn();
			this.colheStateTime = new BrightIdeasSoftware.OLVColumn();
			this.colheState = new BrightIdeasSoftware.OLVColumn();
			this.colhePriceDeposited = new BrightIdeasSoftware.OLVColumn();
			this.colheQtyRequested = new BrightIdeasSoftware.OLVColumn();
			this.colheQtyFilled = new BrightIdeasSoftware.OLVColumn();
			this.colheSernoSession = new BrightIdeasSoftware.OLVColumn();
			this.colheSernoExchange = new BrightIdeasSoftware.OLVColumn();
			this.colheStrategyName = new BrightIdeasSoftware.OLVColumn();
			this.colheSignalName = new BrightIdeasSoftware.OLVColumn();
			this.colheScale = new BrightIdeasSoftware.OLVColumn();
			this.colheAccount = new BrightIdeasSoftware.OLVColumn();
			this.colheLastMessage = new BrightIdeasSoftware.OLVColumn();
			this.ctxOrder = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.mniOrderCancel = new System.Windows.Forms.ToolStripMenuItem();
			this.mniOrderCancelReplace = new System.Windows.Forms.ToolStripMenuItem();
			this.mniOrderRemoveSelected = new System.Windows.Forms.ToolStripMenuItem();
			this.sepCancel = new System.Windows.Forms.ToolStripSeparator();
			this.mniCancelAllPending = new System.Windows.Forms.ToolStripMenuItem();
			this.mniStopEmergencyClose = new System.Windows.Forms.ToolStripMenuItem();
			this.mniKillAlStopAutoSubmit = new System.Windows.Forms.ToolStripMenuItem();
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
			this.mniRemoveCompleted = new System.Windows.Forms.ToolStripMenuItem();
			this.mniOrderEdit = new System.Windows.Forms.ToolStripMenuItem();
			this.mniOrderSubmit = new System.Windows.Forms.ToolStripMenuItem();
			this.imgListOrderDirection = new System.Windows.Forms.ImageList(this.components);
			this.lvMessages = new BrightIdeasSoftware.ObjectListView();
			this.colheMessageDateTime = new BrightIdeasSoftware.OLVColumn();
			this.colheMessageState = new BrightIdeasSoftware.OLVColumn();
			this.colheMessageText = new BrightIdeasSoftware.OLVColumn();
			this.splitContainerMessagePane = new System.Windows.Forms.SplitContainer();
			((System.ComponentModel.ISupportInitialize)(this.OrdersTree)).BeginInit();
			this.ctxOrder.SuspendLayout();
			this.ctxColumns.SuspendLayout();
			this.ctxListControl.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.lvMessages)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerMessagePane)).BeginInit();
			this.splitContainerMessagePane.Panel1.SuspendLayout();
			this.splitContainerMessagePane.Panel2.SuspendLayout();
			this.splitContainerMessagePane.SuspendLayout();
			this.SuspendLayout();
			// 
			// OrdersTree
			// 
			this.OrdersTree.Activation = System.Windows.Forms.ItemActivation.OneClick;
			this.OrdersTree.AllowColumnReorder = true;
			this.OrdersTree.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.OrdersTree.CausesValidation = false;
			this.OrdersTree.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.F2Only;
			this.OrdersTree.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
			this.colheGUID,
			this.colheReplacedByGUID,
			this.colheKilledByGUID,
			this.colheState,
			this.colheStateTime,
			this.colheBarNum,
			this.colheDatetime,
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
			this.OrdersTree.ContextMenuStrip = this.ctxOrder;
			this.OrdersTree.Dock = System.Windows.Forms.DockStyle.Fill;
			this.OrdersTree.EmptyListMsg = "";
			this.OrdersTree.FullRowSelect = true;
			this.OrdersTree.HideSelection = false;
			this.OrdersTree.IncludeColumnHeadersInCopy = true;
			this.OrdersTree.IncludeHiddenColumnsInDataTransfer = true;
			this.OrdersTree.Location = new System.Drawing.Point(0, 0);
			this.OrdersTree.Name = "OrdersTree";
			this.OrdersTree.OwnerDraw = true;
			this.OrdersTree.ShowGroups = false;
			this.OrdersTree.ShowItemToolTips = true;
			this.OrdersTree.Size = new System.Drawing.Size(334, 411);
			this.OrdersTree.SmallImageList = this.imgListOrderDirection;
			this.OrdersTree.TabIndex = 18;
			this.OrdersTree.TintSortColumn = true;
			this.OrdersTree.UnfocusedHighlightBackgroundColor = System.Drawing.SystemColors.GradientActiveCaption;
			this.OrdersTree.UseCompatibleStateImageBehavior = false;
			this.OrdersTree.UseFilterIndicator = true;
			this.OrdersTree.UseFiltering = true;
			this.OrdersTree.View = System.Windows.Forms.View.Details;
			this.OrdersTree.VirtualMode = true;
			this.OrdersTree.FormatRow += new System.EventHandler<BrightIdeasSoftware.FormatRowEventArgs>(this.tree_FormatRow);
			this.OrdersTree.SelectedIndexChanged += new System.EventHandler(this.ordersTree_SelectedIndexChanged);
			this.OrdersTree.DoubleClick += new System.EventHandler(this.ordersTree_DoubleClick);
			this.OrdersTree.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ordersTree_KeyDown);
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
			// colheBarNum
			// 
			this.colheBarNum.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.colheBarNum.Text = "#Bar";
			this.colheBarNum.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.colheBarNum.Width = 52;
			// 
			// colheDatetime
			// 
			this.colheDatetime.Text = "Order Time";
			this.colheDatetime.Width = 84;
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
			// colheStateTime
			// 
			this.colheStateTime.Text = "OrderStateTime";
			this.colheStateTime.Width = 84;
			// 
			// colheState
			// 
			this.colheState.Text = "OrderState";
			this.colheState.Width = 111;
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
			this.mniOrderCancel,
			this.mniOrderCancelReplace,
			this.mniOrderRemoveSelected,
			this.sepCancel,
			this.mniCancelAllPending,
			this.mniStopEmergencyClose,
			this.mniKillAlStopAutoSubmit,
			this.toolStripSeparator3,
			this.mniFilterColumns,
			this.mniFilterOrderStates,
			this.mniFilterAccounts,
			this.mniVisualOptions,
			this.toolStripSeparator1,
			this.mniRemoveCompleted,
			this.mniOrderEdit,
			this.mniOrderSubmit});
			this.ctxOrder.Name = "popupOrders";
			this.ctxOrder.Size = new System.Drawing.Size(237, 308);
			// 
			// mniOrderCancel
			// 
			this.mniOrderCancel.Name = "mniOrderCancel";
			this.mniOrderCancel.Size = new System.Drawing.Size(236, 22);
			this.mniOrderCancel.Text = "Cancel Selected Order(s)";
			this.mniOrderCancel.Click += new System.EventHandler(this.btnCancelSelected_Click);
			// 
			// mniOrderCancelReplace
			// 
			this.mniOrderCancelReplace.Name = "mniOrderCancelReplace";
			this.mniOrderCancelReplace.Size = new System.Drawing.Size(236, 22);
			this.mniOrderCancelReplace.Text = "Cancel/Replace Selected Order";
			this.mniOrderCancelReplace.Click += new System.EventHandler(this.mniOrderCancelReplace_Click);
			// 
			// mniOrderRemoveSelected
			// 
			this.mniOrderRemoveSelected.Name = "mniOrderRemoveSelected";
			this.mniOrderRemoveSelected.Size = new System.Drawing.Size(236, 22);
			this.mniOrderRemoveSelected.Text = "Remove Selected Order(s)";
			this.mniOrderRemoveSelected.Click += new System.EventHandler(this.btnRemoveSelected_Click);
			// 
			// sepCancel
			// 
			this.sepCancel.Name = "sepCancel";
			this.sepCancel.Size = new System.Drawing.Size(233, 6);
			// 
			// mniCancelAllPending
			// 
			this.mniCancelAllPending.Name = "mniCancelAllPending";
			this.mniCancelAllPending.Size = new System.Drawing.Size(236, 22);
			this.mniCancelAllPending.Text = "Cancel All Active";
			this.mniCancelAllPending.ToolTipText = "Cancel all Active Orders and DOESN\'T disable Auto-Trading";
			this.mniCancelAllPending.Click += new System.EventHandler(this.btnCancelAll_Click);
			// 
			// mniStopEmergencyClose
			// 
			this.mniStopEmergencyClose.Name = "mniStopEmergencyClose";
			this.mniStopEmergencyClose.Size = new System.Drawing.Size(236, 22);
			this.mniStopEmergencyClose.Text = "Unlock Emergency";
			this.mniStopEmergencyClose.Click += new System.EventHandler(this.btnRemoveEmergencyLock_Click);
			// 
			// mniKillAlStopAutoSubmit
			// 
			this.mniKillAlStopAutoSubmit.Name = "mniKillAlStopAutoSubmit";
			this.mniKillAlStopAutoSubmit.Size = new System.Drawing.Size(236, 22);
			this.mniKillAlStopAutoSubmit.Text = "Kill All Stop AutoSubmit";
			this.mniKillAlStopAutoSubmit.ToolTipText = "Kill all (even Completed) Orders and DISABLE AutoSubmit";
			this.mniKillAlStopAutoSubmit.Click += new System.EventHandler(this.mniKillAlStopAutoSubmit_Click);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(233, 6);
			// 
			// mniFilterColumns
			// 
			this.mniFilterColumns.DropDown = this.ctxColumns;
			this.mniFilterColumns.Name = "mniFilterColumns";
			this.mniFilterColumns.Size = new System.Drawing.Size(236, 22);
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
			this.ctxColumns.OwnerItem = this.mniFilterColumns;
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
			this.mniFilterOrderStates.Size = new System.Drawing.Size(236, 22);
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
			this.mniFilterAccounts.Size = new System.Drawing.Size(236, 22);
			this.mniFilterAccounts.Text = "Filter Accounts";
			// 
			// ctxAccounts
			// 
			this.ctxAccounts.Name = "ctxAccounts";
			this.ctxAccounts.OwnerItem = this.mniFilterAccounts;
			this.ctxAccounts.Size = new System.Drawing.Size(61, 4);
			this.ctxAccounts.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.ctxAccounts_ItemClicked);
			// 
			// mniVisualOptions
			// 
			this.mniVisualOptions.DropDown = this.ctxListControl;
			this.mniVisualOptions.Name = "mniVisualOptions";
			this.mniVisualOptions.Size = new System.Drawing.Size(236, 22);
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
			this.mniExpandAll.Click += new System.EventHandler(this.mniExpandAllClick);
			// 
			// mniCollapseAll
			// 
			this.mniCollapseAll.Name = "mniCollapseAll";
			this.mniCollapseAll.Size = new System.Drawing.Size(253, 22);
			this.mniCollapseAll.Text = "Collapse All";
			this.mniCollapseAll.Click += new System.EventHandler(this.mniCollapseAllClick);
			// 
			// mniRebuildAll
			// 
			this.mniRebuildAll.Name = "mniRebuildAll";
			this.mniRebuildAll.Size = new System.Drawing.Size(253, 22);
			this.mniRebuildAll.Text = "Rebuild All";
			this.mniRebuildAll.Click += new System.EventHandler(this.RebuildAllToolStripMenuItemClick);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(233, 6);
			// 
			// mniRemoveCompleted
			// 
			this.mniRemoveCompleted.Name = "mniRemoveCompleted";
			this.mniRemoveCompleted.Size = new System.Drawing.Size(236, 22);
			this.mniRemoveCompleted.Text = "Remove Completed (Clear)";
			this.mniRemoveCompleted.Click += new System.EventHandler(this.mniOrdersRemoveCompleted_Click);
			// 
			// mniOrderEdit
			// 
			this.mniOrderEdit.Name = "mniOrderEdit";
			this.mniOrderEdit.Size = new System.Drawing.Size(236, 22);
			this.mniOrderEdit.Text = "Edit Selected Order";
			this.mniOrderEdit.Click += new System.EventHandler(this.mniOrderEdit_Click);
			// 
			// mniOrderSubmit
			// 
			this.mniOrderSubmit.Name = "mniOrderSubmit";
			this.mniOrderSubmit.Size = new System.Drawing.Size(236, 22);
			this.mniOrderSubmit.Text = "Submit Selected Order(s)";
			this.mniOrderSubmit.Click += new System.EventHandler(this.mniOrderSubmit_Click);
			// 
			// imgListOrderDirection
			// 
			this.imgListOrderDirection.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
			this.imgListOrderDirection.ImageSize = new System.Drawing.Size(16, 16);
			this.imgListOrderDirection.TransparentColor = System.Drawing.Color.Silver;
			// 
			// lvMessages
			// 
			this.lvMessages.Activation = System.Windows.Forms.ItemActivation.OneClick;
			this.lvMessages.AllowColumnReorder = true;
			this.lvMessages.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.lvMessages.CausesValidation = false;
			this.lvMessages.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.F2Only;
			this.lvMessages.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
			this.colheMessageDateTime,
			this.colheMessageState,
			this.colheMessageText});
			this.lvMessages.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lvMessages.EmptyListMsg = "";
			this.lvMessages.FullRowSelect = true;
			this.lvMessages.HideSelection = false;
			this.lvMessages.IncludeColumnHeadersInCopy = true;
			this.lvMessages.IncludeHiddenColumnsInDataTransfer = true;
			this.lvMessages.Location = new System.Drawing.Point(0, 0);
			this.lvMessages.Name = "lvMessages";
			this.lvMessages.ShowGroups = false;
			this.lvMessages.ShowItemToolTips = true;
			this.lvMessages.Size = new System.Drawing.Size(495, 411);
			this.lvMessages.TabIndex = 5;
			this.lvMessages.TintSortColumn = true;
			this.lvMessages.UnfocusedHighlightBackgroundColor = System.Drawing.SystemColors.GradientActiveCaption;
			this.lvMessages.UseCompatibleStateImageBehavior = false;
			this.lvMessages.UseFilterIndicator = true;
			this.lvMessages.UseFiltering = true;
			this.lvMessages.View = System.Windows.Forms.View.Details;
			// 
			// colheMessageDateTime
			// 
			this.colheMessageDateTime.Text = "DateTime";
			this.colheMessageDateTime.Width = 83;
			// 
			// colheMessageState
			// 
			this.colheMessageState.Text = "OrderState";
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
			this.splitContainerMessagePane.Panel1.Controls.Add(this.OrdersTree);
			// 
			// splitContainerMessagePane.Panel2
			// 
			this.splitContainerMessagePane.Panel2.BackColor = System.Drawing.SystemColors.Control;
			this.splitContainerMessagePane.Panel2.Controls.Add(this.lvMessages);
			this.splitContainerMessagePane.Size = new System.Drawing.Size(833, 411);
			this.splitContainerMessagePane.SplitterDistance = 334;
			this.splitContainerMessagePane.SplitterIncrement = 20;
			this.splitContainerMessagePane.TabIndex = 22;
			// 
			// ExecutionTreeControl
			// 
			this.Controls.Add(this.splitContainerMessagePane);
			this.Name = "ExecutionTreeControl";
			this.Size = new System.Drawing.Size(833, 411);
			((System.ComponentModel.ISupportInitialize)(this.OrdersTree)).EndInit();
			this.ctxOrder.ResumeLayout(false);
			this.ctxColumns.ResumeLayout(false);
			this.ctxListControl.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.lvMessages)).EndInit();
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
		private BrightIdeasSoftware.ObjectListView lvMessages;
		private System.Windows.Forms.ToolStripMenuItem mniKillAlStopAutoSubmit;
		private System.Windows.Forms.ToolStripMenuItem mniStopEmergencyClose;
		private System.Windows.Forms.ToolStripMenuItem mniCancelAllPending;
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
		private System.Windows.Forms.ToolStripMenuItem mniRemoveCompleted;
		private System.Windows.Forms.ToolStripMenuItem mniOrderRemoveSelected;
		private System.Windows.Forms.ToolStripSeparator sepCancel;
		private System.Windows.Forms.ToolStripMenuItem mniOrderCancelReplace;
		private System.Windows.Forms.ToolStripMenuItem mniOrderCancel;
		private System.Windows.Forms.ToolStripMenuItem mniOrderSubmit;
		private System.Windows.Forms.ToolStripMenuItem mniOrderEdit;
		private System.Windows.Forms.ContextMenuStrip ctxOrder;
		private System.Windows.Forms.ImageList imgListOrderDirection;
		public BrightIdeasSoftware.TreeListView OrdersTree;
		
		private BrightIdeasSoftware.OLVColumn colheDatetime;
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
