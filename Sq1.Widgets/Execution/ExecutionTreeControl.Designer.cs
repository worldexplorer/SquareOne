using System;
using System.ComponentModel;
using System.Windows.Forms;

using Sq1.Core.Support;

namespace Sq1.Widgets.Execution {
	public partial class ExecutionTreeControl {
		private IContainer components;
		protected override void Dispose(bool disposing) {
			if (disposing && this.components != null) {
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.OlvOrdersTree = new BrightIdeasSoftware.TreeListView();
			this.olvcGUID = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcReplacedByGUID = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcKilledByGUID = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcBrokerAdapterName = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcDataSourceName = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcState = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcStateTime = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcBarNum = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcOrderCreated = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcSymbol = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcDirection = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcOrderType = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcSpreadSide = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcPriceScript = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcPriceCurBidOrAsk = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcSlippageApplied = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcPriceEmitted_withSlippageApplied = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcPriceFilled = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcSlippageFilledMinusApplied = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcPriceDeposited_DollarForPoint = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcQtyRequested = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcQtyFilled = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcCommission = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcSernoSession = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcSernoExchange = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcStrategyName = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcSignalName = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcScale = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcAccount = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcLastMessage = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.ctxOrder = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.mniOrderPositionClose = new System.Windows.Forms.ToolStripMenuItem();
			this.mniOrderAlert_removeFromPending = new System.Windows.Forms.ToolStripMenuItem();
			this.mniKillPendingSelected = new System.Windows.Forms.ToolStripMenuItem();
			this.mniKillPendingAll = new System.Windows.Forms.ToolStripMenuItem();
			this.mniKillPendingAll_stopEmitting = new System.Windows.Forms.ToolStripMenuItem();
			this.sepCancel = new System.Windows.Forms.ToolStripSeparator();
			this.mniOrderReplace = new System.Windows.Forms.ToolStripMenuItem();
			this.mniStopEmergencyClose = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.mniFilterColumns = new System.Windows.Forms.ToolStripMenuItem();
			this.ctxColumnsGrouped = new System.Windows.Forms.ContextMenuStrip(this.components);
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
			this.mniToggles = new System.Windows.Forms.ToolStripMenuItem();
			this.ctxToggles = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.mniToggleBrokerTime = new System.Windows.Forms.ToolStripMenuItem();
			this.mniToggleCompletedOrders = new System.Windows.Forms.ToolStripMenuItem();
			this.mniToggleMessagesPane = new System.Windows.Forms.ToolStripMenuItem();
			this.mniToggleMessagesPaneSplitHorizontally = new System.Windows.Forms.ToolStripMenuItem();
			this.mniToggleSyncWithChart = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			this.mniToggleColorifyOrdersTree = new System.Windows.Forms.ToolStripMenuItem();
			this.mniToggleColorifyMessages = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.mniRemoveSeleted = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.mniExpandAll = new System.Windows.Forms.ToolStripMenuItem();
			this.mniCollapseAll = new System.Windows.Forms.ToolStripMenuItem();
			this.mniRebuildAll = new System.Windows.Forms.ToolStripMenuItem();
			this.mniltbDelay = new Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox();
			this.mniltbSerializationInterval = new Sq1.Widgets.LabeledTextBox.ToolStripItemLabeledTextBox();
			this.mniSerializeNow = new System.Windows.Forms.ToolStripMenuItem();
			this.imgListOrderDirection = new System.Windows.Forms.ImageList(this.components);
			this.olvMessages = new BrightIdeasSoftware.ObjectListView();
			this.olvcMessageDateTime = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcMessageState = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcMessageText = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.splitContainerMessagePane = new System.Windows.Forms.SplitContainer();
			((System.ComponentModel.ISupportInitialize)(this.OlvOrdersTree)).BeginInit();
			this.ctxOrder.SuspendLayout();
			this.ctxColumnsGrouped.SuspendLayout();
			this.ctxToggles.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.olvMessages)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerMessagePane)).BeginInit();
			this.splitContainerMessagePane.Panel1.SuspendLayout();
			this.splitContainerMessagePane.Panel2.SuspendLayout();
			this.splitContainerMessagePane.SuspendLayout();
			this.SuspendLayout();
			// 
			// OlvOrdersTree
			// 
			this.OlvOrdersTree.AllColumns.Add(this.olvcGUID);
			this.OlvOrdersTree.AllColumns.Add(this.olvcReplacedByGUID);
			this.OlvOrdersTree.AllColumns.Add(this.olvcKilledByGUID);
			this.OlvOrdersTree.AllColumns.Add(this.olvcBrokerAdapterName);
			this.OlvOrdersTree.AllColumns.Add(this.olvcDataSourceName);
			this.OlvOrdersTree.AllColumns.Add(this.olvcState);
			this.OlvOrdersTree.AllColumns.Add(this.olvcStateTime);
			this.OlvOrdersTree.AllColumns.Add(this.olvcBarNum);
			this.OlvOrdersTree.AllColumns.Add(this.olvcOrderCreated);
			this.OlvOrdersTree.AllColumns.Add(this.olvcSymbol);
			this.OlvOrdersTree.AllColumns.Add(this.olvcDirection);
			this.OlvOrdersTree.AllColumns.Add(this.olvcOrderType);
			this.OlvOrdersTree.AllColumns.Add(this.olvcSpreadSide);
			this.OlvOrdersTree.AllColumns.Add(this.olvcPriceScript);
			this.OlvOrdersTree.AllColumns.Add(this.olvcPriceCurBidOrAsk);
			this.OlvOrdersTree.AllColumns.Add(this.olvcSlippageApplied);
			this.OlvOrdersTree.AllColumns.Add(this.olvcPriceEmitted_withSlippageApplied);
			this.OlvOrdersTree.AllColumns.Add(this.olvcPriceFilled);
			this.OlvOrdersTree.AllColumns.Add(this.olvcSlippageFilledMinusApplied);
			this.OlvOrdersTree.AllColumns.Add(this.olvcPriceDeposited_DollarForPoint);
			this.OlvOrdersTree.AllColumns.Add(this.olvcQtyRequested);
			this.OlvOrdersTree.AllColumns.Add(this.olvcQtyFilled);
			this.OlvOrdersTree.AllColumns.Add(this.olvcCommission);
			this.OlvOrdersTree.AllColumns.Add(this.olvcSernoSession);
			this.OlvOrdersTree.AllColumns.Add(this.olvcSernoExchange);
			this.OlvOrdersTree.AllColumns.Add(this.olvcStrategyName);
			this.OlvOrdersTree.AllColumns.Add(this.olvcSignalName);
			this.OlvOrdersTree.AllColumns.Add(this.olvcScale);
			this.OlvOrdersTree.AllColumns.Add(this.olvcAccount);
			this.OlvOrdersTree.AllColumns.Add(this.olvcLastMessage);
			this.OlvOrdersTree.AllowColumnReorder = true;
			this.OlvOrdersTree.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.OlvOrdersTree.CausesValidation = false;
			this.OlvOrdersTree.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.F2Only;
			this.OlvOrdersTree.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvcGUID,
            this.olvcReplacedByGUID,
            this.olvcKilledByGUID,
            this.olvcBrokerAdapterName,
            this.olvcDataSourceName,
            this.olvcState,
            this.olvcStateTime,
            this.olvcBarNum,
            this.olvcOrderCreated,
            this.olvcSymbol,
            this.olvcDirection,
            this.olvcOrderType,
            this.olvcSpreadSide,
            this.olvcPriceScript,
            this.olvcPriceCurBidOrAsk,
            this.olvcSlippageApplied,
            this.olvcPriceEmitted_withSlippageApplied,
            this.olvcPriceFilled,
            this.olvcSlippageFilledMinusApplied,
            this.olvcPriceDeposited_DollarForPoint,
            this.olvcQtyRequested,
            this.olvcQtyFilled,
            this.olvcCommission,
            this.olvcSernoSession,
            this.olvcSernoExchange,
            this.olvcStrategyName,
            this.olvcSignalName,
            this.olvcScale,
            this.olvcAccount,
            this.olvcLastMessage});
			this.OlvOrdersTree.ContextMenuStrip = this.ctxOrder;
			this.OlvOrdersTree.Cursor = System.Windows.Forms.Cursors.Default;
			this.OlvOrdersTree.Dock = System.Windows.Forms.DockStyle.Fill;
			this.OlvOrdersTree.FullRowSelect = true;
			this.OlvOrdersTree.HideSelection = false;
			this.OlvOrdersTree.IncludeColumnHeadersInCopy = true;
			this.OlvOrdersTree.IncludeHiddenColumnsInDataTransfer = true;
			this.OlvOrdersTree.Location = new System.Drawing.Point(0, 0);
			this.OlvOrdersTree.Name = "OlvOrdersTree";
			this.OlvOrdersTree.OwnerDraw = true;
			this.OlvOrdersTree.ShowCommandMenuOnRightClick = true;
			this.OlvOrdersTree.ShowGroups = false;
			this.OlvOrdersTree.ShowItemToolTips = true;
			this.OlvOrdersTree.Size = new System.Drawing.Size(1033, 146);
			this.OlvOrdersTree.SmallImageList = this.imgListOrderDirection;
			this.OlvOrdersTree.TabIndex = 18;
			this.OlvOrdersTree.TintSortColumn = true;
			this.OlvOrdersTree.UnfocusedHighlightBackgroundColor = System.Drawing.SystemColors.GradientActiveCaption;
			this.OlvOrdersTree.UseCompatibleStateImageBehavior = false;
			this.OlvOrdersTree.UseExplorerTheme = true;
			this.OlvOrdersTree.UseFilterIndicator = true;
			this.OlvOrdersTree.UseFiltering = true;
			this.OlvOrdersTree.UseHotItem = true;
			this.OlvOrdersTree.UseTranslucentHotItem = true;
			this.OlvOrdersTree.UseTranslucentSelection = true;
			this.OlvOrdersTree.View = System.Windows.Forms.View.Details;
			this.OlvOrdersTree.VirtualMode = true;
			this.OlvOrdersTree.SelectedIndexChanged += new System.EventHandler(this.olvOrdersTree_SelectedIndexChanged);
			this.OlvOrdersTree.Click += new System.EventHandler(this.olvOrdersTree_Click);
			this.OlvOrdersTree.DoubleClick += new System.EventHandler(this.olvOrdersTree_DoubleClick);
			this.OlvOrdersTree.KeyDown += new System.Windows.Forms.KeyEventHandler(this.olvOrdersTree_KeyDown);
			// 
			// olvcGUID
			// 
			this.olvcGUID.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcGUID.Text = "GUID";
			this.olvcGUID.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcGUID.Width = 95;
			// 
			// olvcReplacedByGUID
			// 
			this.olvcReplacedByGUID.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcReplacedByGUID.Text = "ReplcdBy";
			this.olvcReplacedByGUID.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcReplacedByGUID.Width = 72;
			// 
			// olvcKilledByGUID
			// 
			this.olvcKilledByGUID.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcKilledByGUID.Text = "KilledBy";
			this.olvcKilledByGUID.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcKilledByGUID.Width = 72;
			// 
			// olvcBrokerAdapterName
			// 
			this.olvcBrokerAdapterName.Text = "BrokerAdapter";
			this.olvcBrokerAdapterName.Width = 80;
			// 
			// olvcDataSourceName
			// 
			this.olvcDataSourceName.Text = "DataSource";
			// 
			// olvcState
			// 
			this.olvcState.Text = "OrderState";
			this.olvcState.Width = 120;
			// 
			// olvcStateTime
			// 
			this.olvcStateTime.Text = "LastOrderState";
			this.olvcStateTime.Width = 84;
			// 
			// olvcBarNum
			// 
			this.olvcBarNum.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcBarNum.Text = "#Bar";
			this.olvcBarNum.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcBarNum.Width = 40;
			// 
			// olvcOrderCreated
			// 
			this.olvcOrderCreated.Text = "Created";
			this.olvcOrderCreated.Width = 84;
			// 
			// olvcSymbol
			// 
			this.olvcSymbol.Text = "Symbol";
			this.olvcSymbol.Width = 42;
			// 
			// olvcDirection
			// 
			this.olvcDirection.Text = "Direction";
			this.olvcDirection.Width = 65;
			// 
			// olvcOrderType
			// 
			this.olvcOrderType.Text = "OrderType";
			this.olvcOrderType.Width = 52;
			// 
			// olvcSpreadSide
			// 
			this.olvcSpreadSide.Text = "SpreadSide";
			this.olvcSpreadSide.Width = 100;
			// 
			// olvcPriceScript
			// 
			this.olvcPriceScript.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcPriceScript.Text = "$Script";
			this.olvcPriceScript.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcPriceScript.Width = 62;
			// 
			// olvcPriceCurBidOrAsk
			// 
			this.olvcPriceCurBidOrAsk.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcPriceCurBidOrAsk.Text = "Bid/Ask";
			this.olvcPriceCurBidOrAsk.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// olvcSlippageApplied
			// 
			this.olvcSlippageApplied.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcSlippageApplied.Text = "SlippageApplied";
			this.olvcSlippageApplied.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcSlippageApplied.Width = 30;
			// 
			// olvcPriceEmitted_withSlippageApplied
			// 
			this.olvcPriceEmitted_withSlippageApplied.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcPriceEmitted_withSlippageApplied.Text = "$Emitted";
			this.olvcPriceEmitted_withSlippageApplied.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcPriceEmitted_withSlippageApplied.Width = 62;
			// 
			// olvcPriceFilled
			// 
			this.olvcPriceFilled.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcPriceFilled.Text = "$Filled";
			this.olvcPriceFilled.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcPriceFilled.Width = 62;
			// 
			// olvcSlippageFilledMinusApplied
			// 
			this.olvcSlippageFilledMinusApplied.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcSlippageFilledMinusApplied.Text = "SlippageDifference";
			this.olvcSlippageFilledMinusApplied.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcSlippageFilledMinusApplied.Width = 30;
			// 
			// olvcPriceDeposited_DollarForPoint
			// 
			this.olvcPriceDeposited_DollarForPoint.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcPriceDeposited_DollarForPoint.Text = "$Deposited";
			this.olvcPriceDeposited_DollarForPoint.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcPriceDeposited_DollarForPoint.Width = 62;
			// 
			// olvcQtyRequested
			// 
			this.olvcQtyRequested.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcQtyRequested.Text = "QRequested";
			this.olvcQtyRequested.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcQtyRequested.Width = 25;
			// 
			// olvcQtyFilled
			// 
			this.olvcQtyFilled.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcQtyFilled.Text = "QFilled";
			this.olvcQtyFilled.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcQtyFilled.Width = 25;
			// 
			// olvcCommission
			// 
			this.olvcCommission.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcCommission.Text = "Commission";
			this.olvcCommission.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// olvcSernoSession
			// 
			this.olvcSernoSession.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcSernoSession.Text = "#Session";
			this.olvcSernoSession.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcSernoSession.Width = 36;
			// 
			// olvcSernoExchange
			// 
			this.olvcSernoExchange.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcSernoExchange.Text = "#Exchange";
			this.olvcSernoExchange.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcSernoExchange.Width = 74;
			// 
			// olvcStrategyName
			// 
			this.olvcStrategyName.Text = "Strategy";
			this.olvcStrategyName.Width = 53;
			// 
			// olvcSignalName
			// 
			this.olvcSignalName.Text = "Signal";
			this.olvcSignalName.Width = 42;
			// 
			// olvcScale
			// 
			this.olvcScale.Text = "Scale";
			this.olvcScale.Width = 40;
			// 
			// olvcAccount
			// 
			this.olvcAccount.Text = "Account";
			this.olvcAccount.Width = 40;
			// 
			// olvcLastMessage
			// 
			this.olvcLastMessage.FillsFreeSpace = true;
			this.olvcLastMessage.Text = "LastMessage";
			this.olvcLastMessage.Width = 400;
			// 
			// ctxOrder
			// 
			this.ctxOrder.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mniOrderPositionClose,
            this.mniOrderAlert_removeFromPending,
            this.mniKillPendingSelected,
            this.mniKillPendingAll,
            this.mniKillPendingAll_stopEmitting,
            this.sepCancel,
            this.mniOrderReplace,
            this.mniStopEmergencyClose,
            this.toolStripSeparator3,
            this.mniFilterColumns,
            this.mniFilterOrderStates,
            this.mniFilterAccounts,
            this.mniToggles,
            this.toolStripSeparator1,
            this.mniRemoveSeleted,
            this.toolStripSeparator2,
            this.mniExpandAll,
            this.mniCollapseAll,
            this.mniRebuildAll,
            this.mniltbDelay,
            this.mniltbSerializationInterval,
            this.mniSerializeNow});
			this.ctxOrder.Name = "popupOrders";
			this.ctxOrder.Size = new System.Drawing.Size(387, 448);
			this.ctxOrder.Opening += new System.ComponentModel.CancelEventHandler(this.ctxOrder_Opening);
			// 
			// mniOrderPositionClose
			// 
			this.mniOrderPositionClose.Name = "mniOrderPositionClose";
			this.mniOrderPositionClose.Size = new System.Drawing.Size(386, 22);
			this.mniOrderPositionClose.Text = "Close Position";
			this.mniOrderPositionClose.Click += new System.EventHandler(this.mniClosePosition_Click);
			// 
			// mniOrderAlert_removeFromPending
			// 
			this.mniOrderAlert_removeFromPending.Name = "mniOrderAlert_removeFromPending";
			this.mniOrderAlert_removeFromPending.Size = new System.Drawing.Size(386, 22);
			this.mniOrderAlert_removeFromPending.Text = "Remove from PendingAlerts (if Order Processor failed)";
			this.mniOrderAlert_removeFromPending.Click += new System.EventHandler(this.mniRemoveFromPendingAlerts_Click);
			// 
			// mniKillPendingSelected
			// 
			this.mniKillPendingSelected.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.mniKillPendingSelected.Name = "mniKillPendingSelected";
			this.mniKillPendingSelected.Size = new System.Drawing.Size(386, 22);
			this.mniKillPendingSelected.Text = "Kill Pending Selected, Continue Emitting    [Double Click]";
			this.mniKillPendingSelected.Click += new System.EventHandler(this.mniKillPendingSelected_Click);
			// 
			// mniKillPendingAll
			// 
			this.mniKillPendingAll.Name = "mniKillPendingAll";
			this.mniKillPendingAll.Size = new System.Drawing.Size(386, 22);
			this.mniKillPendingAll.Text = "Kill Pending All,             Continue Emitting";
			this.mniKillPendingAll.Click += new System.EventHandler(this.mniKillPendingAll_Click);
			// 
			// mniKillPendingAll_stopEmitting
			// 
			this.mniKillPendingAll_stopEmitting.Name = "mniKillPendingAll_stopEmitting";
			this.mniKillPendingAll_stopEmitting.Size = new System.Drawing.Size(386, 22);
			this.mniKillPendingAll_stopEmitting.Text = "Kill Pending All,             Stop Emitting - PANIC";
			this.mniKillPendingAll_stopEmitting.Click += new System.EventHandler(this.mniKillPendingAll_stopEmitting_Click);
			// 
			// sepCancel
			// 
			this.sepCancel.Name = "sepCancel";
			this.sepCancel.Size = new System.Drawing.Size(383, 6);
			// 
			// mniOrderReplace
			// 
			this.mniOrderReplace.Name = "mniOrderReplace";
			this.mniOrderReplace.Size = new System.Drawing.Size(386, 22);
			this.mniOrderReplace.Text = "Replace NYI";
			this.mniOrderReplace.Visible = false;
			this.mniOrderReplace.Click += new System.EventHandler(this.mniOrderReplace_Click);
			// 
			// mniStopEmergencyClose
			// 
			this.mniStopEmergencyClose.Name = "mniStopEmergencyClose";
			this.mniStopEmergencyClose.Size = new System.Drawing.Size(386, 22);
			this.mniStopEmergencyClose.Text = "Stop Emergency Close";
			this.mniStopEmergencyClose.Visible = false;
			this.mniStopEmergencyClose.Click += new System.EventHandler(this.mniEmergencyLockRemove_Click);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(383, 6);
			this.toolStripSeparator3.Visible = false;
			// 
			// mniFilterColumns
			// 
			this.mniFilterColumns.DropDown = this.ctxColumnsGrouped;
			this.mniFilterColumns.Name = "mniFilterColumns";
			this.mniFilterColumns.Size = new System.Drawing.Size(386, 22);
			this.mniFilterColumns.Text = "Filter Columns";
			// 
			// ctxColumnsGrouped
			// 
			this.ctxColumnsGrouped.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mniShowWhenWhat,
            this.mniShowKilledReplaced,
            this.mniShowPrice,
            this.mniShowQty,
            this.mniShowExchange,
            this.mniShowOrigin,
            this.mniShowExtra,
            this.mniShowPosition,
            this.mniShowLastMessage});
			this.ctxColumnsGrouped.Name = "ctxColumnsGrouped";
			this.ctxColumnsGrouped.OwnerItem = this.mniFilterColumns;
			this.ctxColumnsGrouped.Size = new System.Drawing.Size(189, 202);
			this.ctxColumnsGrouped.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.ctxColumnsGrouped_ItemClicked);
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
			this.mniFilterOrderStates.Size = new System.Drawing.Size(386, 22);
			this.mniFilterOrderStates.Text = "Filter Order States";
			this.mniFilterOrderStates.Visible = false;
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
			this.mniFilterAccounts.Size = new System.Drawing.Size(386, 22);
			this.mniFilterAccounts.Text = "Filter Accounts";
			// 
			// ctxAccounts
			// 
			this.ctxAccounts.Name = "ctxAccounts";
			this.ctxAccounts.OwnerItem = this.mniFilterAccounts;
			this.ctxAccounts.Size = new System.Drawing.Size(61, 4);
			this.ctxAccounts.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.ctxAccounts_ItemClicked);
			// 
			// mniToggles
			// 
			this.mniToggles.DropDown = this.ctxToggles;
			this.mniToggles.Name = "mniToggles";
			this.mniToggles.Size = new System.Drawing.Size(386, 22);
			this.mniToggles.Text = "Toggles";
			// 
			// ctxToggles
			// 
			this.ctxToggles.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mniToggleBrokerTime,
            this.mniToggleCompletedOrders,
            this.mniToggleMessagesPane,
            this.mniToggleMessagesPaneSplitHorizontally,
            this.mniToggleSyncWithChart,
            this.toolStripSeparator4,
            this.mniToggleColorifyOrdersTree,
            this.mniToggleColorifyMessages});
			this.ctxToggles.Name = "ctxToggles";
			this.ctxToggles.OwnerItem = this.mniToggles;
			this.ctxToggles.Size = new System.Drawing.Size(349, 164);
			// 
			// mniToggleBrokerTime
			// 
			this.mniToggleBrokerTime.CheckOnClick = true;
			this.mniToggleBrokerTime.Name = "mniToggleBrokerTime";
			this.mniToggleBrokerTime.Size = new System.Drawing.Size(348, 22);
			this.mniToggleBrokerTime.Text = "Show Broker Time in CREATED column";
			this.mniToggleBrokerTime.Click += new System.EventHandler(this.mniToggleBrokerTime_Click);
			// 
			// mniToggleCompletedOrders
			// 
			this.mniToggleCompletedOrders.CheckOnClick = true;
			this.mniToggleCompletedOrders.Name = "mniToggleCompletedOrders";
			this.mniToggleCompletedOrders.Size = new System.Drawing.Size(348, 22);
			this.mniToggleCompletedOrders.Text = "Show Completed Orders";
			this.mniToggleCompletedOrders.Click += new System.EventHandler(this.mniToggleCompletedOrders_Click);
			// 
			// mniToggleMessagesPane
			// 
			this.mniToggleMessagesPane.Checked = true;
			this.mniToggleMessagesPane.CheckOnClick = true;
			this.mniToggleMessagesPane.CheckState = System.Windows.Forms.CheckState.Checked;
			this.mniToggleMessagesPane.Name = "mniToggleMessagesPane";
			this.mniToggleMessagesPane.Size = new System.Drawing.Size(348, 22);
			this.mniToggleMessagesPane.Text = "Show Messages Pane";
			this.mniToggleMessagesPane.Click += new System.EventHandler(this.mniToggleMessagesPane_Click);
			// 
			// mniToggleMessagesPaneSplitHorizontally
			// 
			this.mniToggleMessagesPaneSplitHorizontally.Checked = true;
			this.mniToggleMessagesPaneSplitHorizontally.CheckOnClick = true;
			this.mniToggleMessagesPaneSplitHorizontally.CheckState = System.Windows.Forms.CheckState.Checked;
			this.mniToggleMessagesPaneSplitHorizontally.Name = "mniToggleMessagesPaneSplitHorizontally";
			this.mniToggleMessagesPaneSplitHorizontally.Size = new System.Drawing.Size(348, 22);
			this.mniToggleMessagesPaneSplitHorizontally.Text = "Show Messages Pane Horizontally";
			this.mniToggleMessagesPaneSplitHorizontally.Click += new System.EventHandler(this.mniToggleMessagesPaneSplitHorizontally_Click);
			// 
			// mniToggleSyncWithChart
			// 
			this.mniToggleSyncWithChart.CheckOnClick = true;
			this.mniToggleSyncWithChart.Name = "mniToggleSyncWithChart";
			this.mniToggleSyncWithChart.Size = new System.Drawing.Size(348, 22);
			this.mniToggleSyncWithChart.Text = "Show PositionAffected On Chart";
			this.mniToggleSyncWithChart.Click += new System.EventHandler(this.mniToggleSyncWithChart_Click);
			// 
			// toolStripSeparator4
			// 
			this.toolStripSeparator4.Name = "toolStripSeparator4";
			this.toolStripSeparator4.Size = new System.Drawing.Size(345, 6);
			// 
			// mniToggleColorifyOrdersTree
			// 
			this.mniToggleColorifyOrdersTree.CheckOnClick = true;
			this.mniToggleColorifyOrdersTree.Name = "mniToggleColorifyOrdersTree";
			this.mniToggleColorifyOrdersTree.Size = new System.Drawing.Size(348, 22);
			this.mniToggleColorifyOrdersTree.Text = "SLOW Colorify OrdersTree (on Position.Net)";
			this.mniToggleColorifyOrdersTree.Click += new System.EventHandler(this.mniToggleColorifyOrdersTree_Click);
			// 
			// mniToggleColorifyMessages
			// 
			this.mniToggleColorifyMessages.CheckOnClick = true;
			this.mniToggleColorifyMessages.Name = "mniToggleColorifyMessages";
			this.mniToggleColorifyMessages.Size = new System.Drawing.Size(348, 22);
			this.mniToggleColorifyMessages.Text = "SLOW Colorify Messages (Broker-provided bgColor)";
			this.mniToggleColorifyMessages.Click += new System.EventHandler(this.mniToggleColorifyMessages_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(383, 6);
			// 
			// mniRemoveSeleted
			// 
			this.mniRemoveSeleted.Name = "mniRemoveSeleted";
			this.mniRemoveSeleted.ShortcutKeys = System.Windows.Forms.Keys.Delete;
			this.mniRemoveSeleted.Size = new System.Drawing.Size(386, 22);
			this.mniRemoveSeleted.Text = "Remove Selected Non-Pending";
			this.mniRemoveSeleted.Click += new System.EventHandler(this.mniOrdersRemoveSelected_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(383, 6);
			// 
			// mniExpandAll
			// 
			this.mniExpandAll.Name = "mniExpandAll";
			this.mniExpandAll.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.E)));
			this.mniExpandAll.Size = new System.Drawing.Size(386, 22);
			this.mniExpandAll.Text = "Expand All";
			this.mniExpandAll.Click += new System.EventHandler(this.mniTreeExpandAll_Click);
			// 
			// mniCollapseAll
			// 
			this.mniCollapseAll.Name = "mniCollapseAll";
			this.mniCollapseAll.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.C)));
			this.mniCollapseAll.Size = new System.Drawing.Size(386, 22);
			this.mniCollapseAll.Text = "Collapse All";
			this.mniCollapseAll.Click += new System.EventHandler(this.mniTreeCollapseAll_Click);
			// 
			// mniRebuildAll
			// 
			this.mniRebuildAll.Name = "mniRebuildAll";
			this.mniRebuildAll.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.R)));
			this.mniRebuildAll.Size = new System.Drawing.Size(386, 22);
			this.mniRebuildAll.Text = "Rebuild All";
			this.mniRebuildAll.Click += new System.EventHandler(this.mniTreeRebuildAll_Click);
			// 
			// mniltbDelay
			// 
			this.mniltbDelay.BackColor = System.Drawing.Color.Transparent;
			this.mniltbDelay.InputFieldAlignedRight = false;
			this.mniltbDelay.InputFieldBackColor = System.Drawing.SystemColors.Info;
			this.mniltbDelay.InputFieldEditable = true;
			this.mniltbDelay.InputFieldMultiline = false;
			this.mniltbDelay.InputFieldOffsetX = 170;
			this.mniltbDelay.InputFieldValue = "200";
			this.mniltbDelay.InputFieldWidth = 40;
			this.mniltbDelay.Name = "mniltbDelay";
			this.mniltbDelay.OffsetTop = 0;
			this.mniltbDelay.Size = new System.Drawing.Size(240, 18);
			this.mniltbDelay.TextLeft = "UI buffering delay";
			this.mniltbDelay.TextLeftOffsetX = 0;
			this.mniltbDelay.TextLeftWidth = 103;
			this.mniltbDelay.TextRed = false;
			this.mniltbDelay.TextRight = "ms";
			this.mniltbDelay.TextRightOffsetX = 210;
			this.mniltbDelay.TextRightWidth = 27;
			this.mniltbDelay.UserTyped += new System.EventHandler<Sq1.Widgets.LabeledTextBox.LabeledTextBoxUserTypedArgs>(this.mniltbDelay_UserTyped);
			// 
			// mniltbSerializationInterval
			// 
			this.mniltbSerializationInterval.BackColor = System.Drawing.Color.Transparent;
			this.mniltbSerializationInterval.InputFieldAlignedRight = false;
			this.mniltbSerializationInterval.InputFieldBackColor = System.Drawing.SystemColors.Info;
			this.mniltbSerializationInterval.InputFieldEditable = true;
			this.mniltbSerializationInterval.InputFieldMultiline = true;
			this.mniltbSerializationInterval.InputFieldOffsetX = 170;
			this.mniltbSerializationInterval.InputFieldValue = "3000";
			this.mniltbSerializationInterval.InputFieldWidth = 40;
			this.mniltbSerializationInterval.Name = "mniltbSerializationInterval";
			this.mniltbSerializationInterval.OffsetTop = 0;
			this.mniltbSerializationInterval.Size = new System.Drawing.Size(240, 22);
			this.mniltbSerializationInterval.Text = "mniltbDelaySerializationSync";
			this.mniltbSerializationInterval.TextLeft = "Serialize every (logrotate)";
			this.mniltbSerializationInterval.TextLeftOffsetX = 0;
			this.mniltbSerializationInterval.TextLeftWidth = 141;
			this.mniltbSerializationInterval.TextRed = false;
			this.mniltbSerializationInterval.TextRight = "ms";
			this.mniltbSerializationInterval.TextRightOffsetX = 210;
			this.mniltbSerializationInterval.TextRightWidth = 27;
			this.mniltbSerializationInterval.UserTyped += new System.EventHandler<Sq1.Widgets.LabeledTextBox.LabeledTextBoxUserTypedArgs>(this.mniltbDelaySerializationSync_UserTyped);
			// 
			// mniSerializeNow
			// 
			this.mniSerializeNow.Name = "mniSerializeNow";
			this.mniSerializeNow.Size = new System.Drawing.Size(386, 22);
			this.mniSerializeNow.Text = "Serialize now";
			this.mniSerializeNow.Click += new System.EventHandler(this.mniSerializeNow_Click);
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
            this.olvcMessageDateTime,
            this.olvcMessageState,
            this.olvcMessageText});
			this.olvMessages.Cursor = System.Windows.Forms.Cursors.Default;
			this.olvMessages.Dock = System.Windows.Forms.DockStyle.Fill;
			this.olvMessages.EmptyListMsg = "";
			this.olvMessages.FullRowSelect = true;
			this.olvMessages.HideSelection = false;
			this.olvMessages.IncludeColumnHeadersInCopy = true;
			this.olvMessages.IncludeHiddenColumnsInDataTransfer = true;
			this.olvMessages.Location = new System.Drawing.Point(0, 0);
			this.olvMessages.Name = "olvMessages";
			this.olvMessages.ShowCommandMenuOnRightClick = true;
			this.olvMessages.ShowGroups = false;
			this.olvMessages.ShowItemToolTips = true;
			this.olvMessages.Size = new System.Drawing.Size(1033, 99);
			this.olvMessages.TabIndex = 5;
			this.olvMessages.TintSortColumn = true;
			this.olvMessages.UnfocusedHighlightBackgroundColor = System.Drawing.SystemColors.GradientActiveCaption;
			this.olvMessages.UseCompatibleStateImageBehavior = false;
			this.olvMessages.UseExplorerTheme = true;
			this.olvMessages.UseFilterIndicator = true;
			this.olvMessages.UseFiltering = true;
			this.olvMessages.UseHotItem = true;
			this.olvMessages.UseTranslucentHotItem = true;
			this.olvMessages.UseTranslucentSelection = true;
			this.olvMessages.View = System.Windows.Forms.View.Details;
			// 
			// olvcMessageDateTime
			// 
			this.olvcMessageDateTime.Text = "DateTime";
			this.olvcMessageDateTime.Width = 83;
			// 
			// olvcMessageState
			// 
			this.olvcMessageState.Text = "State";
			this.olvcMessageState.Width = 101;
			// 
			// olvcMessageText
			// 
			this.olvcMessageText.Text = "Message";
			this.olvcMessageText.Width = 2211;
			// 
			// splitContainerMessagePane
			// 
			this.splitContainerMessagePane.BackColor = System.Drawing.SystemColors.ControlLight;
			this.splitContainerMessagePane.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainerMessagePane.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainerMessagePane.Location = new System.Drawing.Point(0, 0);
			this.splitContainerMessagePane.Name = "splitContainerMessagePane";
			this.splitContainerMessagePane.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainerMessagePane.Panel1
			// 
			this.splitContainerMessagePane.Panel1.BackColor = System.Drawing.SystemColors.Control;
			this.splitContainerMessagePane.Panel1.Controls.Add(this.OlvOrdersTree);
			// 
			// splitContainerMessagePane.Panel2
			// 
			this.splitContainerMessagePane.Panel2.BackColor = System.Drawing.SystemColors.Control;
			this.splitContainerMessagePane.Panel2.Controls.Add(this.olvMessages);
			this.splitContainerMessagePane.Size = new System.Drawing.Size(1033, 249);
			this.splitContainerMessagePane.SplitterDistance = 146;
			this.splitContainerMessagePane.TabIndex = 22;
			// 
			// ExecutionTreeControl
			// 
			this.Controls.Add(this.splitContainerMessagePane);
			this.Name = "ExecutionTreeControl";
			this.Size = new System.Drawing.Size(1033, 249);
			((System.ComponentModel.ISupportInitialize)(this.OlvOrdersTree)).EndInit();
			this.ctxOrder.ResumeLayout(false);
			this.ctxColumnsGrouped.ResumeLayout(false);
			this.ctxToggles.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.olvMessages)).EndInit();
			this.splitContainerMessagePane.Panel1.ResumeLayout(false);
			this.splitContainerMessagePane.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainerMessagePane)).EndInit();
			this.splitContainerMessagePane.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		private System.Windows.Forms.ToolStripMenuItem mniToggleBrokerTime;
		private System.Windows.Forms.ToolStripMenuItem mniFilterAccounts;
		private System.Windows.Forms.ToolStripMenuItem mniFilterOrderStates;
		private System.Windows.Forms.ToolStripMenuItem mniFilterColumns;
		private System.Windows.Forms.ToolStripMenuItem mniToggleMessagesPane;
		private System.Windows.Forms.ToolStripMenuItem mniToggleMessagesPaneSplitHorizontally;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		private System.Windows.Forms.ToolStripMenuItem mniToggleCompletedOrders;
		private System.Windows.Forms.ToolStripMenuItem mniToggleSyncWithChart;
		private System.Windows.Forms.ToolStripMenuItem mniToggles;
		private System.Windows.Forms.ContextMenuStrip ctxToggles;
		private System.Windows.Forms.SplitContainer splitContainerMessagePane;
		private BrightIdeasSoftware.OLVColumn olvcMessageText;
		private BrightIdeasSoftware.OLVColumn olvcMessageState;
		private BrightIdeasSoftware.OLVColumn olvcMessageDateTime;
		private BrightIdeasSoftware.ObjectListView olvMessages;
		private System.Windows.Forms.ToolStripMenuItem mniKillPendingAll_stopEmitting;
		private System.Windows.Forms.ToolStripMenuItem mniStopEmergencyClose;
		private System.Windows.Forms.ToolStripMenuItem mniKillPendingAll;
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
		private System.Windows.Forms.ContextMenuStrip ctxColumnsGrouped;
		private System.Windows.Forms.ToolStripMenuItem mniRemoveSeleted;
		private System.Windows.Forms.ToolStripSeparator sepCancel;
		private System.Windows.Forms.ToolStripMenuItem mniOrderReplace;
		private System.Windows.Forms.ToolStripMenuItem mniKillPendingSelected;
		private System.Windows.Forms.ContextMenuStrip ctxOrder;
		private System.Windows.Forms.ImageList imgListOrderDirection;
		public BrightIdeasSoftware.TreeListView OlvOrdersTree;
		
		private BrightIdeasSoftware.OLVColumn olvcOrderCreated;
		private BrightIdeasSoftware.OLVColumn olvcBarNum;
		private BrightIdeasSoftware.OLVColumn olvcPriceScript;
		private BrightIdeasSoftware.OLVColumn olvcSernoSession;
		private BrightIdeasSoftware.OLVColumn olvcSernoExchange;
		private BrightIdeasSoftware.OLVColumn olvcGUID;
		private BrightIdeasSoftware.OLVColumn olvcReplacedByGUID;
		private BrightIdeasSoftware.OLVColumn olvcKilledByGUID;
		private BrightIdeasSoftware.OLVColumn olvcStateTime;
		private BrightIdeasSoftware.OLVColumn olvcState;
		private BrightIdeasSoftware.OLVColumn olvcSymbol;
		private BrightIdeasSoftware.OLVColumn olvcDirection;
		private BrightIdeasSoftware.OLVColumn olvcOrderType;
		private BrightIdeasSoftware.OLVColumn olvcQtyRequested;
		private BrightIdeasSoftware.OLVColumn olvcPriceEmitted_withSlippageApplied;
		private BrightIdeasSoftware.OLVColumn olvcQtyFilled;
		private BrightIdeasSoftware.OLVColumn olvcPriceFilled;
		private BrightIdeasSoftware.OLVColumn olvcPriceDeposited_DollarForPoint;
		private BrightIdeasSoftware.OLVColumn olvcSlippageApplied;
		private BrightIdeasSoftware.OLVColumn olvcSpreadSide;
		private BrightIdeasSoftware.OLVColumn olvcStrategyName;
		private BrightIdeasSoftware.OLVColumn olvcSignalName;
		private BrightIdeasSoftware.OLVColumn olvcScale;
		private BrightIdeasSoftware.OLVColumn olvcAccount;
		private BrightIdeasSoftware.OLVColumn olvcLastMessage;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripMenuItem mniExpandAll;
		private System.Windows.Forms.ToolStripMenuItem mniCollapseAll;
		private System.Windows.Forms.ToolStripMenuItem mniRebuildAll;
		private BrightIdeasSoftware.OLVColumn olvcBrokerAdapterName;
		private BrightIdeasSoftware.OLVColumn olvcDataSourceName;
		private Sq1.Widgets.LabeledTextBox.ToolStripItemLabeledTextBox mniltbSerializationInterval;
		private BrightIdeasSoftware.OLVColumn olvcPriceCurBidOrAsk;
		private BrightIdeasSoftware.OLVColumn olvcSlippageFilledMinusApplied;
		private BrightIdeasSoftware.OLVColumn olvcCommission;
		private LabeledTextBox.MenuItemLabeledTextBox mniltbDelay;
		private ToolStripMenuItem mniToggleColorifyOrdersTree;
		private ToolStripMenuItem mniToggleColorifyMessages;
		private ToolStripSeparator toolStripSeparator4;
		private ToolStripMenuItem mniOrderPositionClose;
		private ToolStripMenuItem mniOrderAlert_removeFromPending;
		private ToolStripMenuItem mniSerializeNow;
	}
}
