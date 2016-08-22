using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Collections.Generic;

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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExecutionTreeControl));
			this.olvOrdersTree = new BrightIdeasSoftware.TreeListView();
			this.olvcGUID = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcReplacedByGUID = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcKilledByGUID = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcStrategyName = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcBrokerAdapterName = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcDataSourceName = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcBarNum = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcSymbol = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcSernoSession = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcSernoExchange = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcState = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcDirection = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcOrderType = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcSpreadSide = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcPriceScript = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcSlippageApplied = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcPriceEmitted_withSlippageApplied = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcPriceFilled = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcSlippageFilled = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcSlippageFilledMinusApplied = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcPriceDeposited_DollarForPoint = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcQtyRequested = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcQtyFilled = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcBidAndAsk_whenEmitted = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcBidAndAsk_whenFilled = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcCommission = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcSignalName = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcScale = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcAccount = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcOrderCreated = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcStateTime = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcLastMessage = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.ctxOrder = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.mniPosition_info = new System.Windows.Forms.ToolStripMenuItem();
			this.mniExitAlert_info = new System.Windows.Forms.ToolStripMenuItem();
			this.mniOrderPositionClose = new System.Windows.Forms.ToolStripMenuItem();
			this.mniOrderAlert_removeFromPending = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
			this.mniKillPendingSelected = new System.Windows.Forms.ToolStripMenuItem();
			this.mniKillPendingAll = new System.Windows.Forms.ToolStripMenuItem();
			this.mniKillPendingAll_stopEmitting = new System.Windows.Forms.ToolStripMenuItem();
			this.sepCancel = new System.Windows.Forms.ToolStripSeparator();
			this.mniOrderReplace = new System.Windows.Forms.ToolStripMenuItem();
			this.mniStopEmergencyClose = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.mniRemoveSeleted = new System.Windows.Forms.ToolStripMenuItem();
			this.mniFilterColumns = new System.Windows.Forms.ToolStripMenuItem();
			this.mniFilterOrderStates = new System.Windows.Forms.ToolStripMenuItem();
			this.mniFilterAccounts = new System.Windows.Forms.ToolStripMenuItem();
			this.mniToggles = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.mniExpandAll = new System.Windows.Forms.ToolStripMenuItem();
			this.mniCollapseAll = new System.Windows.Forms.ToolStripMenuItem();
			this.mniRebuildAll = new System.Windows.Forms.ToolStripMenuItem();
			this.mniltbFlushToGuiDelayMsec = new Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.mniltbSerializationInterval = new Sq1.Widgets.LabeledTextBox.ToolStripItemLabeledTextBox();
			this.mniSerializeNow = new System.Windows.Forms.ToolStripMenuItem();
			this.mniltbLogrotateLargerThan = new Sq1.Widgets.LabeledTextBox.ToolStripItemLabeledTextBox();
			this.mniDeleteAllLogrotatedOrderJsons = new System.Windows.Forms.ToolStripMenuItem();
			this.imgListOrderDirection = new System.Windows.Forms.ImageList(this.components);
			this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
			this.mniOrderStateAll = new System.Windows.Forms.ToolStripMenuItem();
			this.mniOrderStateNone = new System.Windows.Forms.ToolStripMenuItem();
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
			this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
			this.ctxOrderStates = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.mniFilter_OrderStates = new System.Windows.Forms.ToolStripMenuItem();
			this.tsiDbxFilters = new System.Windows.Forms.ToolStripDropDownButton();
			this.ctxFilters = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.mniFilter_Accounts = new System.Windows.Forms.ToolStripMenuItem();
			this.ctxAccounts = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.mniFilter_Symbols = new System.Windows.Forms.ToolStripMenuItem();
			this.mniFilter_Strategies = new System.Windows.Forms.ToolStripMenuItem();
			this.ctxToggles = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.mniRecentAlwaysSelected = new System.Windows.Forms.ToolStripMenuItem();
			this.mniToggleBrokerTime = new System.Windows.Forms.ToolStripMenuItem();
			this.mniToggleCompletedOrders = new System.Windows.Forms.ToolStripMenuItem();
			this.mniToggleMessagesPane = new System.Windows.Forms.ToolStripMenuItem();
			this.mniToggleMessagesPaneSplitHorizontally = new System.Windows.Forms.ToolStripMenuItem();
			this.mniToggleSyncWithChart = new System.Windows.Forms.ToolStripMenuItem();
			this.mniToggleKillerOrders = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			this.mniToggleColorifyOrdersTree = new System.Windows.Forms.ToolStripMenuItem();
			this.mniToggleColorifyMessages = new System.Windows.Forms.ToolStripMenuItem();
			this.tsiDbxToggles = new System.Windows.Forms.ToolStripDropDownButton();
			this.olvMessages = new BrightIdeasSoftware.ObjectListView();
			this.olvcMessageDateTime = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcMessageSerno = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcMessageState = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvcMessageText = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.splitContainerMessagePane = new System.Windows.Forms.SplitContainer();
			this.statusStrip_search = new System.Windows.Forms.StatusStrip();
			this.tsilbl_separator = new System.Windows.Forms.ToolStripStatusLabel();
			this.tsiCbx_SearchApply = new Sq1.Widgets.ToolStripImproved.ToolStripItemCheckBox();
			this.tsiLtb_SearchKeywords = new Sq1.Widgets.LabeledTextBox.ToolStripItemLabeledTextBox();
			this.tsiCbx_ExcludeApply = new Sq1.Widgets.ToolStripImproved.ToolStripItemCheckBox();
			this.tsiLtb_ExcludeKeywords = new Sq1.Widgets.LabeledTextBox.ToolStripItemLabeledTextBox();
			this.tsiBtnClear = new System.Windows.Forms.ToolStripButton();
			((System.ComponentModel.ISupportInitialize)(this.olvOrdersTree)).BeginInit();
			this.ctxOrder.SuspendLayout();
			this.ctxColumnsGrouped.SuspendLayout();
			this.ctxOrderStates.SuspendLayout();
			this.ctxFilters.SuspendLayout();
			this.ctxToggles.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.olvMessages)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerMessagePane)).BeginInit();
			this.splitContainerMessagePane.Panel1.SuspendLayout();
			this.splitContainerMessagePane.Panel2.SuspendLayout();
			this.splitContainerMessagePane.SuspendLayout();
			this.statusStrip_search.SuspendLayout();
			this.SuspendLayout();
			// 
			// olvOrdersTree
			// 
			this.olvOrdersTree.AllColumns.Add(this.olvcGUID);
			this.olvOrdersTree.AllColumns.Add(this.olvcReplacedByGUID);
			this.olvOrdersTree.AllColumns.Add(this.olvcKilledByGUID);
			this.olvOrdersTree.AllColumns.Add(this.olvcStrategyName);
			this.olvOrdersTree.AllColumns.Add(this.olvcBrokerAdapterName);
			this.olvOrdersTree.AllColumns.Add(this.olvcDataSourceName);
			this.olvOrdersTree.AllColumns.Add(this.olvcBarNum);
			this.olvOrdersTree.AllColumns.Add(this.olvcSymbol);
			this.olvOrdersTree.AllColumns.Add(this.olvcSernoSession);
			this.olvOrdersTree.AllColumns.Add(this.olvcSernoExchange);
			this.olvOrdersTree.AllColumns.Add(this.olvcState);
			this.olvOrdersTree.AllColumns.Add(this.olvcDirection);
			this.olvOrdersTree.AllColumns.Add(this.olvcOrderType);
			this.olvOrdersTree.AllColumns.Add(this.olvcSpreadSide);
			this.olvOrdersTree.AllColumns.Add(this.olvcPriceScript);
			this.olvOrdersTree.AllColumns.Add(this.olvcSlippageApplied);
			this.olvOrdersTree.AllColumns.Add(this.olvcPriceEmitted_withSlippageApplied);
			this.olvOrdersTree.AllColumns.Add(this.olvcPriceFilled);
			this.olvOrdersTree.AllColumns.Add(this.olvcSlippageFilled);
			this.olvOrdersTree.AllColumns.Add(this.olvcSlippageFilledMinusApplied);
			this.olvOrdersTree.AllColumns.Add(this.olvcPriceDeposited_DollarForPoint);
			this.olvOrdersTree.AllColumns.Add(this.olvcQtyRequested);
			this.olvOrdersTree.AllColumns.Add(this.olvcQtyFilled);
			this.olvOrdersTree.AllColumns.Add(this.olvcBidAndAsk_whenEmitted);
			this.olvOrdersTree.AllColumns.Add(this.olvcBidAndAsk_whenFilled);
			this.olvOrdersTree.AllColumns.Add(this.olvcCommission);
			this.olvOrdersTree.AllColumns.Add(this.olvcSignalName);
			this.olvOrdersTree.AllColumns.Add(this.olvcScale);
			this.olvOrdersTree.AllColumns.Add(this.olvcAccount);
			this.olvOrdersTree.AllColumns.Add(this.olvcOrderCreated);
			this.olvOrdersTree.AllColumns.Add(this.olvcStateTime);
			this.olvOrdersTree.AllColumns.Add(this.olvcLastMessage);
			this.olvOrdersTree.AllowColumnReorder = true;
			this.olvOrdersTree.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.olvOrdersTree.CausesValidation = false;
			this.olvOrdersTree.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.F2Only;
			this.olvOrdersTree.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvcGUID,
            this.olvcStrategyName,
            this.olvcBrokerAdapterName,
            this.olvcDataSourceName,
            this.olvcBarNum,
            this.olvcSymbol,
            this.olvcSernoSession,
            this.olvcSernoExchange,
            this.olvcState,
            this.olvcDirection,
            this.olvcOrderType,
            this.olvcSpreadSide,
            this.olvcPriceScript,
            this.olvcSlippageApplied,
            this.olvcPriceEmitted_withSlippageApplied,
            this.olvcPriceFilled,
            this.olvcSlippageFilledMinusApplied,
            this.olvcQtyFilled,
            this.olvcBidAndAsk_whenEmitted,
            this.olvcBidAndAsk_whenFilled,
            this.olvcOrderCreated,
            this.olvcStateTime});
			this.olvOrdersTree.ContextMenuStrip = this.ctxOrder;
			this.olvOrdersTree.Cursor = System.Windows.Forms.Cursors.Default;
			this.olvOrdersTree.Dock = System.Windows.Forms.DockStyle.Fill;
			this.olvOrdersTree.FullRowSelect = true;
			this.olvOrdersTree.HideSelection = false;
			this.olvOrdersTree.IncludeColumnHeadersInCopy = true;
			this.olvOrdersTree.IncludeHiddenColumnsInDataTransfer = true;
			this.olvOrdersTree.Location = new System.Drawing.Point(0, 0);
			this.olvOrdersTree.Name = "olvOrdersTree";
			this.olvOrdersTree.OwnerDraw = true;
			this.olvOrdersTree.ShowCommandMenuOnRightClick = true;
			this.olvOrdersTree.ShowGroups = false;
			this.olvOrdersTree.ShowItemToolTips = true;
			this.olvOrdersTree.Size = new System.Drawing.Size(1033, 146);
			this.olvOrdersTree.SmallImageList = this.imgListOrderDirection;
			this.olvOrdersTree.TabIndex = 18;
			this.olvOrdersTree.TintSortColumn = true;
			this.olvOrdersTree.UnfocusedHighlightBackgroundColor = System.Drawing.SystemColors.GradientActiveCaption;
			this.olvOrdersTree.UseCompatibleStateImageBehavior = false;
			this.olvOrdersTree.UseExplorerTheme = true;
			this.olvOrdersTree.UseFilterIndicator = true;
			this.olvOrdersTree.UseFiltering = true;
			this.olvOrdersTree.UseHotItem = true;
			this.olvOrdersTree.UseTranslucentHotItem = true;
			this.olvOrdersTree.UseTranslucentSelection = true;
			this.olvOrdersTree.View = System.Windows.Forms.View.Details;
			this.olvOrdersTree.VirtualMode = true;
			this.olvOrdersTree.SelectedIndexChanged += new System.EventHandler(this.olvOrdersTree_SelectedIndexChanged);
			this.olvOrdersTree.DoubleClick += new System.EventHandler(this.olvOrdersTree_DoubleClick);
			this.olvOrdersTree.KeyDown += new System.Windows.Forms.KeyEventHandler(this.olvOrdersTree_KeyDown);
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
			this.olvcReplacedByGUID.DisplayIndex = 1;
			this.olvcReplacedByGUID.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcReplacedByGUID.IsVisible = false;
			this.olvcReplacedByGUID.Text = "ReplcdBy";
			this.olvcReplacedByGUID.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcReplacedByGUID.Width = 72;
			// 
			// olvcKilledByGUID
			// 
			this.olvcKilledByGUID.DisplayIndex = 2;
			this.olvcKilledByGUID.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcKilledByGUID.IsVisible = false;
			this.olvcKilledByGUID.Text = "KilledBy";
			this.olvcKilledByGUID.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcKilledByGUID.Width = 72;
			// 
			// olvcStrategyName
			// 
			this.olvcStrategyName.Text = "Strategy";
			this.olvcStrategyName.Width = 53;
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
			// olvcBarNum
			// 
			this.olvcBarNum.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcBarNum.Text = "#Bar";
			this.olvcBarNum.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcBarNum.Width = 40;
			// 
			// olvcSymbol
			// 
			this.olvcSymbol.Text = "Symbol";
			this.olvcSymbol.Width = 42;
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
			// olvcState
			// 
			this.olvcState.Text = "OrderState";
			this.olvcState.Width = 120;
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
			// olvcSlippageFilled
			// 
			this.olvcSlippageFilled.DisplayIndex = 16;
			this.olvcSlippageFilled.IsVisible = false;
			this.olvcSlippageFilled.Text = "SlippageFilled";
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
			this.olvcPriceDeposited_DollarForPoint.DisplayIndex = 21;
			this.olvcPriceDeposited_DollarForPoint.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcPriceDeposited_DollarForPoint.IsVisible = false;
			this.olvcPriceDeposited_DollarForPoint.Text = "$Deposited";
			this.olvcPriceDeposited_DollarForPoint.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcPriceDeposited_DollarForPoint.Width = 62;
			// 
			// olvcQtyRequested
			// 
			this.olvcQtyRequested.DisplayIndex = 22;
			this.olvcQtyRequested.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcQtyRequested.IsVisible = false;
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
			// olvcBidAndAsk_whenEmitted
			// 
			this.olvcBidAndAsk_whenEmitted.Text = "Bid:Ask/EMITTED";
			// 
			// olvcBidAndAsk_whenFilled
			// 
			this.olvcBidAndAsk_whenFilled.Text = "Bid:Ask/FILLED";
			// 
			// olvcCommission
			// 
			this.olvcCommission.DisplayIndex = 24;
			this.olvcCommission.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.olvcCommission.IsVisible = false;
			this.olvcCommission.Text = "Commission";
			this.olvcCommission.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// olvcSignalName
			// 
			this.olvcSignalName.DisplayIndex = 28;
			this.olvcSignalName.IsVisible = false;
			this.olvcSignalName.Text = "Signal";
			this.olvcSignalName.Width = 42;
			// 
			// olvcScale
			// 
			this.olvcScale.DisplayIndex = 29;
			this.olvcScale.IsVisible = false;
			this.olvcScale.Text = "Scale";
			this.olvcScale.Width = 40;
			// 
			// olvcAccount
			// 
			this.olvcAccount.DisplayIndex = 30;
			this.olvcAccount.IsVisible = false;
			this.olvcAccount.Text = "Account";
			this.olvcAccount.Width = 40;
			// 
			// olvcOrderCreated
			// 
			this.olvcOrderCreated.Text = "Created";
			this.olvcOrderCreated.Width = 84;
			// 
			// olvcStateTime
			// 
			this.olvcStateTime.Text = "LastOrderState";
			this.olvcStateTime.Width = 84;
			// 
			// olvcLastMessage
			// 
			this.olvcLastMessage.DisplayIndex = 31;
			this.olvcLastMessage.FillsFreeSpace = true;
			this.olvcLastMessage.IsVisible = false;
			this.olvcLastMessage.Text = "LastMessage";
			this.olvcLastMessage.Width = 400;
			// 
			// ctxOrder
			// 
			this.ctxOrder.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mniPosition_info,
            this.mniExitAlert_info,
            this.mniOrderPositionClose,
            this.mniOrderAlert_removeFromPending,
            this.toolStripSeparator5,
            this.mniKillPendingSelected,
            this.mniKillPendingAll,
            this.mniKillPendingAll_stopEmitting,
            this.sepCancel,
            this.mniOrderReplace,
            this.mniStopEmergencyClose,
            this.toolStripSeparator3,
            this.mniRemoveSeleted,
            this.mniFilterColumns,
            this.mniFilterOrderStates,
            this.mniFilterAccounts,
            this.mniToggles,
            this.toolStripSeparator1,
            this.mniExpandAll,
            this.mniCollapseAll,
            this.mniRebuildAll,
            this.mniltbFlushToGuiDelayMsec,
            this.toolStripSeparator2,
            this.mniltbSerializationInterval,
            this.mniSerializeNow,
            this.mniltbLogrotateLargerThan,
            this.mniDeleteAllLogrotatedOrderJsons});
			this.ctxOrder.Name = "popupOrders";
			this.ctxOrder.Size = new System.Drawing.Size(423, 549);
			this.ctxOrder.Opening += new System.ComponentModel.CancelEventHandler(this.ctxOrder_Opening);
			// 
			// mniPosition_info
			// 
			this.mniPosition_info.Enabled = false;
			this.mniPosition_info.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.mniPosition_info.Name = "mniPosition_info";
			this.mniPosition_info.Size = new System.Drawing.Size(422, 22);
			this.mniPosition_info.Text = "Position: BLA BLA BLA";
			// 
			// mniExitAlert_info
			// 
			this.mniExitAlert_info.Enabled = false;
			this.mniExitAlert_info.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
			this.mniExitAlert_info.Name = "mniExitAlert_info";
			this.mniExitAlert_info.Size = new System.Drawing.Size(422, 22);
			this.mniExitAlert_info.Text = "ExitAlert: BLA BLA BLA";
			// 
			// mniOrderPositionClose
			// 
			this.mniOrderPositionClose.Name = "mniOrderPositionClose";
			this.mniOrderPositionClose.Size = new System.Drawing.Size(422, 22);
			this.mniOrderPositionClose.Text = "Close Position";
			this.mniOrderPositionClose.Click += new System.EventHandler(this.mniClosePosition_Click);
			// 
			// mniOrderAlert_removeFromPending
			// 
			this.mniOrderAlert_removeFromPending.Name = "mniOrderAlert_removeFromPending";
			this.mniOrderAlert_removeFromPending.Size = new System.Drawing.Size(422, 22);
			this.mniOrderAlert_removeFromPending.Text = "Remove from PendingAlerts (if Order Processor failed)";
			this.mniOrderAlert_removeFromPending.Click += new System.EventHandler(this.mniRemoveFromPendingAlerts_Click);
			// 
			// toolStripSeparator5
			// 
			this.toolStripSeparator5.Name = "toolStripSeparator5";
			this.toolStripSeparator5.Size = new System.Drawing.Size(419, 6);
			// 
			// mniKillPendingSelected
			// 
			this.mniKillPendingSelected.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.mniKillPendingSelected.Name = "mniKillPendingSelected";
			this.mniKillPendingSelected.Size = new System.Drawing.Size(422, 22);
			this.mniKillPendingSelected.Text = "Kill Pending Selected[1],        Continue Emitting    [Double Click]";
			this.mniKillPendingSelected.Click += new System.EventHandler(this.mniKillPendingSelected_Click);
			// 
			// mniKillPendingAll
			// 
			this.mniKillPendingAll.Name = "mniKillPendingAll";
			this.mniKillPendingAll.Size = new System.Drawing.Size(422, 22);
			this.mniKillPendingAll.Text = "Kill Pending AllForStrat[0],      Continue Emitting";
			this.mniKillPendingAll.Click += new System.EventHandler(this.mniKillPendingAll_Click);
			// 
			// mniKillPendingAll_stopEmitting
			// 
			this.mniKillPendingAll_stopEmitting.Name = "mniKillPendingAll_stopEmitting";
			this.mniKillPendingAll_stopEmitting.Size = new System.Drawing.Size(422, 22);
			this.mniKillPendingAll_stopEmitting.Text = "Kill Pending AllForStrat[0],              Stop Emitting - PANIC";
			this.mniKillPendingAll_stopEmitting.Click += new System.EventHandler(this.mniKillPendingAll_stopEmitting_Click);
			// 
			// sepCancel
			// 
			this.sepCancel.Name = "sepCancel";
			this.sepCancel.Size = new System.Drawing.Size(419, 6);
			// 
			// mniOrderReplace
			// 
			this.mniOrderReplace.Name = "mniOrderReplace";
			this.mniOrderReplace.Size = new System.Drawing.Size(422, 22);
			this.mniOrderReplace.Text = "Replace NYI";
			this.mniOrderReplace.Visible = false;
			this.mniOrderReplace.Click += new System.EventHandler(this.mniOrderReplace_Click);
			// 
			// mniStopEmergencyClose
			// 
			this.mniStopEmergencyClose.Name = "mniStopEmergencyClose";
			this.mniStopEmergencyClose.Size = new System.Drawing.Size(422, 22);
			this.mniStopEmergencyClose.Text = "Stop Emergency Close";
			this.mniStopEmergencyClose.Visible = false;
			this.mniStopEmergencyClose.Click += new System.EventHandler(this.mniEmergencyLockRemove_Click);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(419, 6);
			this.toolStripSeparator3.Visible = false;
			// 
			// mniRemoveSeleted
			// 
			this.mniRemoveSeleted.Name = "mniRemoveSeleted";
			this.mniRemoveSeleted.ShortcutKeys = System.Windows.Forms.Keys.Delete;
			this.mniRemoveSeleted.Size = new System.Drawing.Size(422, 22);
			this.mniRemoveSeleted.Text = "Remove Selected Non-Pending";
			this.mniRemoveSeleted.Click += new System.EventHandler(this.mniOrdersRemoveSelected_Click);
			// 
			// mniFilterColumns
			// 
			this.mniFilterColumns.Name = "mniFilterColumns";
			this.mniFilterColumns.Size = new System.Drawing.Size(422, 22);
			this.mniFilterColumns.Text = "Filter Columns";
			this.mniFilterColumns.Visible = false;
			// 
			// mniFilterOrderStates
			// 
			this.mniFilterOrderStates.Name = "mniFilterOrderStates";
			this.mniFilterOrderStates.Size = new System.Drawing.Size(422, 22);
			this.mniFilterOrderStates.Text = "Filter Order States";
			this.mniFilterOrderStates.Visible = false;
			// 
			// mniFilterAccounts
			// 
			this.mniFilterAccounts.Name = "mniFilterAccounts";
			this.mniFilterAccounts.Size = new System.Drawing.Size(422, 22);
			this.mniFilterAccounts.Text = "Filter Accounts";
			this.mniFilterAccounts.Visible = false;
			// 
			// mniToggles
			// 
			this.mniToggles.Name = "mniToggles";
			this.mniToggles.Size = new System.Drawing.Size(422, 22);
			this.mniToggles.Text = "Toggles";
			this.mniToggles.Visible = false;
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(419, 6);
			// 
			// mniExpandAll
			// 
			this.mniExpandAll.Name = "mniExpandAll";
			this.mniExpandAll.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.E)));
			this.mniExpandAll.Size = new System.Drawing.Size(422, 22);
			this.mniExpandAll.Text = "Expand All";
			this.mniExpandAll.Click += new System.EventHandler(this.mniTreeExpandAll_Click);
			// 
			// mniCollapseAll
			// 
			this.mniCollapseAll.Name = "mniCollapseAll";
			this.mniCollapseAll.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.C)));
			this.mniCollapseAll.Size = new System.Drawing.Size(422, 22);
			this.mniCollapseAll.Text = "Collapse All";
			this.mniCollapseAll.Click += new System.EventHandler(this.mniTreeCollapseAll_Click);
			// 
			// mniRebuildAll
			// 
			this.mniRebuildAll.Name = "mniRebuildAll";
			this.mniRebuildAll.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.R)));
			this.mniRebuildAll.Size = new System.Drawing.Size(422, 22);
			this.mniRebuildAll.Text = "Rebuild All";
			this.mniRebuildAll.Click += new System.EventHandler(this.mniTreeRebuildAll_Click);
			// 
			// mniltbFlushToGuiDelayMsec
			// 
			this.mniltbFlushToGuiDelayMsec.AutoSize = false;
			this.mniltbFlushToGuiDelayMsec.BackColor = System.Drawing.Color.Transparent;
			this.mniltbFlushToGuiDelayMsec.InputFieldAlignedRight = false;
			this.mniltbFlushToGuiDelayMsec.InputFieldBackColor = System.Drawing.SystemColors.Info;
			this.mniltbFlushToGuiDelayMsec.InputFieldEditable = true;
			this.mniltbFlushToGuiDelayMsec.InputFieldMultiline = true;
			this.mniltbFlushToGuiDelayMsec.InputFieldOffsetX = 170;
			this.mniltbFlushToGuiDelayMsec.InputFieldValue = "200";
			this.mniltbFlushToGuiDelayMsec.InputFieldWidth = 40;
			this.mniltbFlushToGuiDelayMsec.Name = "mniltbFlushToGuiDelayMsec";
			this.mniltbFlushToGuiDelayMsec.OffsetTop = 0;
			this.mniltbFlushToGuiDelayMsec.Size = new System.Drawing.Size(240, 22);
			this.mniltbFlushToGuiDelayMsec.TextLeft = "Flush To GUI Delay";
			this.mniltbFlushToGuiDelayMsec.TextLeftOffsetX = 0;
			this.mniltbFlushToGuiDelayMsec.TextLeftWidth = 108;
			this.mniltbFlushToGuiDelayMsec.TextRed = false;
			this.mniltbFlushToGuiDelayMsec.TextRight = "ms";
			this.mniltbFlushToGuiDelayMsec.TextRightOffsetX = 210;
			this.mniltbFlushToGuiDelayMsec.TextRightWidth = 27;
			this.mniltbFlushToGuiDelayMsec.UserTyped += new System.EventHandler<Sq1.Widgets.LabeledTextBox.LabeledTextBoxUserTypedArgs>(this.mniltbFlushToGuiDelayMsec_UserTyped);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(419, 6);
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
			this.mniSerializeNow.Size = new System.Drawing.Size(422, 22);
			this.mniSerializeNow.Text = "Serialize now";
			this.mniSerializeNow.Click += new System.EventHandler(this.mniSerializeNow_Click);
			// 
			// mniltbLogrotateLargerThan
			// 
			this.mniltbLogrotateLargerThan.BackColor = System.Drawing.Color.Transparent;
			this.mniltbLogrotateLargerThan.InputFieldAlignedRight = false;
			this.mniltbLogrotateLargerThan.InputFieldBackColor = System.Drawing.SystemColors.Info;
			this.mniltbLogrotateLargerThan.InputFieldEditable = true;
			this.mniltbLogrotateLargerThan.InputFieldMultiline = true;
			this.mniltbLogrotateLargerThan.InputFieldOffsetX = 170;
			this.mniltbLogrotateLargerThan.InputFieldValue = "3000";
			this.mniltbLogrotateLargerThan.InputFieldWidth = 40;
			this.mniltbLogrotateLargerThan.Name = "mniltbLogrotateLargerThan";
			this.mniltbLogrotateLargerThan.OffsetTop = 0;
			this.mniltbLogrotateLargerThan.Size = new System.Drawing.Size(242, 22);
			this.mniltbLogrotateLargerThan.Text = "mniltbLogrotateLargerThan";
			this.mniltbLogrotateLargerThan.TextLeft = "Logrotate if larger than";
			this.mniltbLogrotateLargerThan.TextLeftOffsetX = 0;
			this.mniltbLogrotateLargerThan.TextLeftWidth = 130;
			this.mniltbLogrotateLargerThan.TextRed = false;
			this.mniltbLogrotateLargerThan.TextRight = "Mb";
			this.mniltbLogrotateLargerThan.TextRightOffsetX = 210;
			this.mniltbLogrotateLargerThan.TextRightWidth = 29;
			this.mniltbLogrotateLargerThan.UserTyped += new System.EventHandler<Sq1.Widgets.LabeledTextBox.LabeledTextBoxUserTypedArgs>(this.mniltbLogrotateLargerThan_UserTyped);
			// 
			// mniDeleteAllLogrotatedOrderJsons
			// 
			this.mniDeleteAllLogrotatedOrderJsons.Name = "mniDeleteAllLogrotatedOrderJsons";
			this.mniDeleteAllLogrotatedOrderJsons.Size = new System.Drawing.Size(422, 22);
			this.mniDeleteAllLogrotatedOrderJsons.Text = "Delete All[x] logrotated Order*.json";
			this.mniDeleteAllLogrotatedOrderJsons.Click += new System.EventHandler(this.mniDeleteAllLogrotatedJsons_Click);
			// 
			// imgListOrderDirection
			// 
			this.imgListOrderDirection.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
			this.imgListOrderDirection.ImageSize = new System.Drawing.Size(16, 16);
			this.imgListOrderDirection.TransparentColor = System.Drawing.Color.Silver;
			// 
			// toolStripSeparator6
			// 
			this.toolStripSeparator6.Name = "toolStripSeparator6";
			this.toolStripSeparator6.Size = new System.Drawing.Size(167, 6);
			// 
			// mniOrderStateAll
			// 
			this.mniOrderStateAll.CheckOnClick = true;
			this.mniOrderStateAll.Name = "mniOrderStateAll";
			this.mniOrderStateAll.Size = new System.Drawing.Size(170, 22);
			this.mniOrderStateAll.Text = "OrderStates: All";
			this.mniOrderStateAll.Click += new System.EventHandler(this.mniOrderStateAll_Click);
			// 
			// mniOrderStateNone
			// 
			this.mniOrderStateNone.CheckOnClick = true;
			this.mniOrderStateNone.Name = "mniOrderStateNone";
			this.mniOrderStateNone.Size = new System.Drawing.Size(170, 22);
			this.mniOrderStateNone.Text = "OrderStates: None";
			this.mniOrderStateNone.Click += new System.EventHandler(this.mniOrderStateNone_Click);
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
			this.ctxColumnsGrouped.OwnerItem = this.toolStripDropDownButton1;
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
			// toolStripDropDownButton1
			// 
			this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripDropDownButton1.DropDown = this.ctxColumnsGrouped;
			this.toolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton1.Image")));
			this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
			this.toolStripDropDownButton1.Size = new System.Drawing.Size(68, 22);
			this.toolStripDropDownButton1.Text = "Columns";
			// 
			// ctxOrderStates
			// 
			this.ctxOrderStates.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mniOrderStateAll,
            this.mniOrderStateNone,
            this.toolStripSeparator6});
			this.ctxOrderStates.Name = "ctxOrderStates";
			this.ctxOrderStates.OwnerItem = this.mniFilter_OrderStates;
			this.ctxOrderStates.Size = new System.Drawing.Size(171, 54);
			this.ctxOrderStates.Opening += new System.ComponentModel.CancelEventHandler(this.ctxOrderStates_Opening);
			// 
			// mniFilter_OrderStates
			// 
			this.mniFilter_OrderStates.CheckOnClick = true;
			this.mniFilter_OrderStates.DropDown = this.ctxOrderStates;
			this.mniFilter_OrderStates.Name = "mniFilter_OrderStates";
			this.mniFilter_OrderStates.Size = new System.Drawing.Size(135, 22);
			this.mniFilter_OrderStates.Text = "OrderStates";
			// 
			// tsiDbxFilters
			// 
			this.tsiDbxFilters.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.tsiDbxFilters.DropDown = this.ctxFilters;
			this.tsiDbxFilters.Image = ((System.Drawing.Image)(resources.GetObject("tsiDbxFilters.Image")));
			this.tsiDbxFilters.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsiDbxFilters.Name = "tsiDbxFilters";
			this.tsiDbxFilters.Size = new System.Drawing.Size(51, 22);
			this.tsiDbxFilters.Text = "Filters";
			this.tsiDbxFilters.ToolTipText = "OrderStates Filter";
			// 
			// ctxFilters
			// 
			this.ctxFilters.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mniFilter_OrderStates,
            this.mniFilter_Accounts,
            this.mniFilter_Symbols,
            this.mniFilter_Strategies});
			this.ctxFilters.Name = "ctxFilters";
			this.ctxFilters.OwnerItem = this.tsiDbxFilters;
			this.ctxFilters.Size = new System.Drawing.Size(136, 92);
			// 
			// mniFilter_Accounts
			// 
			this.mniFilter_Accounts.DropDown = this.ctxAccounts;
			this.mniFilter_Accounts.Name = "mniFilter_Accounts";
			this.mniFilter_Accounts.Size = new System.Drawing.Size(135, 22);
			this.mniFilter_Accounts.Text = "Accounts";
			// 
			// ctxAccounts
			// 
			this.ctxAccounts.Name = "ctxAccounts";
			this.ctxAccounts.OwnerItem = this.mniFilter_Accounts;
			this.ctxAccounts.Size = new System.Drawing.Size(61, 4);
			this.ctxAccounts.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.ctxAccounts_ItemClicked);
			// 
			// mniFilter_Symbols
			// 
			this.mniFilter_Symbols.Name = "mniFilter_Symbols";
			this.mniFilter_Symbols.Size = new System.Drawing.Size(135, 22);
			this.mniFilter_Symbols.Text = "Symbols";
			// 
			// mniFilter_Strategies
			// 
			this.mniFilter_Strategies.Name = "mniFilter_Strategies";
			this.mniFilter_Strategies.Size = new System.Drawing.Size(135, 22);
			this.mniFilter_Strategies.Text = "Strategies";
			// 
			// ctxToggles
			// 
			this.ctxToggles.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mniRecentAlwaysSelected,
            this.mniToggleBrokerTime,
            this.mniToggleCompletedOrders,
            this.mniToggleMessagesPane,
            this.mniToggleMessagesPaneSplitHorizontally,
            this.mniToggleSyncWithChart,
            this.mniToggleKillerOrders,
            this.toolStripSeparator4,
            this.mniToggleColorifyOrdersTree,
            this.mniToggleColorifyMessages});
			this.ctxToggles.Name = "ctxToggles";
			this.ctxToggles.OwnerItem = this.tsiDbxToggles;
			this.ctxToggles.Size = new System.Drawing.Size(349, 208);
			// 
			// mniRecentAlwaysSelected
			// 
			this.mniRecentAlwaysSelected.CheckOnClick = true;
			this.mniRecentAlwaysSelected.Name = "mniRecentAlwaysSelected";
			this.mniRecentAlwaysSelected.Size = new System.Drawing.Size(348, 22);
			this.mniRecentAlwaysSelected.Text = "Recent Always Selected";
			this.mniRecentAlwaysSelected.Click += new System.EventHandler(this.mniRecentAlwaysSelected_Click);
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
			this.mniToggleMessagesPane.CheckOnClick = true;
			this.mniToggleMessagesPane.Name = "mniToggleMessagesPane";
			this.mniToggleMessagesPane.Size = new System.Drawing.Size(348, 22);
			this.mniToggleMessagesPane.Text = "Show Messages Pane";
			this.mniToggleMessagesPane.Click += new System.EventHandler(this.mniToggleMessagesPane_Click);
			// 
			// mniToggleMessagesPaneSplitHorizontally
			// 
			this.mniToggleMessagesPaneSplitHorizontally.CheckOnClick = true;
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
			// mniToggleKillerOrders
			// 
			this.mniToggleKillerOrders.CheckOnClick = true;
			this.mniToggleKillerOrders.Name = "mniToggleKillerOrders";
			this.mniToggleKillerOrders.Size = new System.Drawing.Size(348, 22);
			this.mniToggleKillerOrders.Text = "Show KILLER orders";
			this.mniToggleKillerOrders.Click += new System.EventHandler(this.mniToggleKillerOrders_click);
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
			// tsiDbxToggles
			// 
			this.tsiDbxToggles.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.tsiDbxToggles.DropDown = this.ctxToggles;
			this.tsiDbxToggles.Image = ((System.Drawing.Image)(resources.GetObject("tsiDbxToggles.Image")));
			this.tsiDbxToggles.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsiDbxToggles.Name = "tsiDbxToggles";
			this.tsiDbxToggles.Size = new System.Drawing.Size(62, 22);
			this.tsiDbxToggles.Text = "Toggles";
			// 
			// olvMessages
			// 
			this.olvMessages.Activation = System.Windows.Forms.ItemActivation.OneClick;
			this.olvMessages.AllColumns.Add(this.olvcMessageDateTime);
			this.olvMessages.AllColumns.Add(this.olvcMessageSerno);
			this.olvMessages.AllColumns.Add(this.olvcMessageState);
			this.olvMessages.AllColumns.Add(this.olvcMessageText);
			this.olvMessages.AllowColumnReorder = true;
			this.olvMessages.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.olvMessages.CausesValidation = false;
			this.olvMessages.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.DoubleClick;
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
			this.olvMessages.Size = new System.Drawing.Size(1033, 75);
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
			// olvcMessageSerno
			// 
			this.olvcMessageSerno.DisplayIndex = 0;
			this.olvcMessageSerno.IsVisible = false;
			this.olvcMessageSerno.Text = "#";
			this.olvcMessageSerno.Width = 20;
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
			this.splitContainerMessagePane.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.splitContainerMessagePane.BackColor = System.Drawing.SystemColors.ControlLight;
			this.splitContainerMessagePane.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainerMessagePane.Location = new System.Drawing.Point(0, 1);
			this.splitContainerMessagePane.Name = "splitContainerMessagePane";
			this.splitContainerMessagePane.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainerMessagePane.Panel1
			// 
			this.splitContainerMessagePane.Panel1.BackColor = System.Drawing.SystemColors.Control;
			this.splitContainerMessagePane.Panel1.Controls.Add(this.olvOrdersTree);
			// 
			// splitContainerMessagePane.Panel2
			// 
			this.splitContainerMessagePane.Panel2.BackColor = System.Drawing.SystemColors.Control;
			this.splitContainerMessagePane.Panel2.Controls.Add(this.olvMessages);
			this.splitContainerMessagePane.Size = new System.Drawing.Size(1033, 225);
			this.splitContainerMessagePane.SplitterDistance = 146;
			this.splitContainerMessagePane.TabIndex = 22;
			// 
			// statusStrip_search
			// 
			this.statusStrip_search.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripDropDownButton1,
            this.tsiDbxToggles,
            this.tsiDbxFilters,
            this.tsilbl_separator,
            this.tsiCbx_SearchApply,
            this.tsiLtb_SearchKeywords,
            this.tsiCbx_ExcludeApply,
            this.tsiLtb_ExcludeKeywords,
            this.tsiBtnClear});
			this.statusStrip_search.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
			this.statusStrip_search.Location = new System.Drawing.Point(0, 225);
			this.statusStrip_search.Name = "statusStrip_search";
			this.statusStrip_search.Size = new System.Drawing.Size(1033, 24);
			this.statusStrip_search.SizingGrip = false;
			this.statusStrip_search.TabIndex = 23;
			this.statusStrip_search.Text = "statusStrip1";
			// 
			// tsilbl_separator
			// 
			this.tsilbl_separator.Name = "tsilbl_separator";
			this.tsilbl_separator.Size = new System.Drawing.Size(16, 19);
			this.tsilbl_separator.Text = "   ";
			// 
			// tsiCbx_SearchApply
			// 
			this.tsiCbx_SearchApply.AutoSize = false;
			this.tsiCbx_SearchApply.CheckBoxChecked = true;
			this.tsiCbx_SearchApply.CheckBoxText = "Search for:";
			this.tsiCbx_SearchApply.Name = "tsiCbx_SearchApply";
			this.tsiCbx_SearchApply.Size = new System.Drawing.Size(82, 22);
			this.tsiCbx_SearchApply.Text = "Search for:";
			// 
			// tsiLtb_SearchKeywords
			// 
			this.tsiLtb_SearchKeywords.AutoSize = false;
			this.tsiLtb_SearchKeywords.BackColor = System.Drawing.SystemColors.Control;
			this.tsiLtb_SearchKeywords.InputFieldAlignedRight = false;
			this.tsiLtb_SearchKeywords.InputFieldBackColor = System.Drawing.SystemColors.Window;
			this.tsiLtb_SearchKeywords.InputFieldEditable = true;
			this.tsiLtb_SearchKeywords.InputFieldMultiline = false;
			this.tsiLtb_SearchKeywords.InputFieldOffsetX = 0;
			this.tsiLtb_SearchKeywords.InputFieldValue = "";
			this.tsiLtb_SearchKeywords.InputFieldWidth = 110;
			this.tsiLtb_SearchKeywords.Margin = new System.Windows.Forms.Padding(0);
			this.tsiLtb_SearchKeywords.Name = "tsiLtb_SearchKeywords";
			this.tsiLtb_SearchKeywords.OffsetTop = 0;
			this.tsiLtb_SearchKeywords.Size = new System.Drawing.Size(130, 24);
			this.tsiLtb_SearchKeywords.Text = "toolStripItemLabeledTextBox1";
			this.tsiLtb_SearchKeywords.TextLeft = "";
			this.tsiLtb_SearchKeywords.TextLeftOffsetX = 0;
			this.tsiLtb_SearchKeywords.TextLeftWidth = 2;
			this.tsiLtb_SearchKeywords.TextRed = false;
			this.tsiLtb_SearchKeywords.TextRight = "";
			this.tsiLtb_SearchKeywords.TextRightOffsetX = 115;
			this.tsiLtb_SearchKeywords.TextRightWidth = 4;
			// 
			// tsiCbx_ExcludeApply
			// 
			this.tsiCbx_ExcludeApply.AutoSize = false;
			this.tsiCbx_ExcludeApply.CheckBoxChecked = true;
			this.tsiCbx_ExcludeApply.CheckBoxText = "Exclude:";
			this.tsiCbx_ExcludeApply.Name = "tsiCbx_ExcludeApply";
			this.tsiCbx_ExcludeApply.Size = new System.Drawing.Size(69, 22);
			this.tsiCbx_ExcludeApply.Text = "Exclude:";
			// 
			// tsiLtb_ExcludeKeywords
			// 
			this.tsiLtb_ExcludeKeywords.AutoSize = false;
			this.tsiLtb_ExcludeKeywords.BackColor = System.Drawing.Color.Transparent;
			this.tsiLtb_ExcludeKeywords.InputFieldAlignedRight = false;
			this.tsiLtb_ExcludeKeywords.InputFieldBackColor = System.Drawing.SystemColors.Window;
			this.tsiLtb_ExcludeKeywords.InputFieldEditable = true;
			this.tsiLtb_ExcludeKeywords.InputFieldMultiline = false;
			this.tsiLtb_ExcludeKeywords.InputFieldOffsetX = 0;
			this.tsiLtb_ExcludeKeywords.InputFieldValue = "";
			this.tsiLtb_ExcludeKeywords.InputFieldWidth = 170;
			this.tsiLtb_ExcludeKeywords.Margin = new System.Windows.Forms.Padding(0, -1, 0, 0);
			this.tsiLtb_ExcludeKeywords.Name = "tsiLtb_ExcludeKeywords";
			this.tsiLtb_ExcludeKeywords.OffsetTop = 0;
			this.tsiLtb_ExcludeKeywords.Size = new System.Drawing.Size(182, 24);
			this.tsiLtb_ExcludeKeywords.Text = "toolStripItemLabeledTextBox1";
			this.tsiLtb_ExcludeKeywords.TextLeft = "";
			this.tsiLtb_ExcludeKeywords.TextLeftOffsetX = 0;
			this.tsiLtb_ExcludeKeywords.TextLeftWidth = 2;
			this.tsiLtb_ExcludeKeywords.TextRed = false;
			this.tsiLtb_ExcludeKeywords.TextRight = "";
			this.tsiLtb_ExcludeKeywords.TextRightOffsetX = 175;
			this.tsiLtb_ExcludeKeywords.TextRightWidth = 4;
			// 
			// tsiBtnClear
			// 
			this.tsiBtnClear.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsiBtnClear.Name = "tsiBtnClear";
			this.tsiBtnClear.Size = new System.Drawing.Size(38, 22);
			this.tsiBtnClear.Text = "Clear";
			this.tsiBtnClear.Click += new System.EventHandler(this.tsiBtnClear_Click);
			// 
			// ExecutionTreeControl
			// 
			this.Controls.Add(this.statusStrip_search);
			this.Controls.Add(this.splitContainerMessagePane);
			this.Name = "ExecutionTreeControl";
			this.Size = new System.Drawing.Size(1033, 249);
			((System.ComponentModel.ISupportInitialize)(this.olvOrdersTree)).EndInit();
			this.ctxOrder.ResumeLayout(false);
			this.ctxColumnsGrouped.ResumeLayout(false);
			this.ctxOrderStates.ResumeLayout(false);
			this.ctxFilters.ResumeLayout(false);
			this.ctxToggles.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.olvMessages)).EndInit();
			this.splitContainerMessagePane.Panel1.ResumeLayout(false);
			this.splitContainerMessagePane.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainerMessagePane)).EndInit();
			this.splitContainerMessagePane.ResumeLayout(false);
			this.statusStrip_search.ResumeLayout(false);
			this.statusStrip_search.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

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
		private System.Windows.Forms.ToolStripMenuItem mniOrderStateAll;
		private System.Windows.Forms.ToolStripMenuItem mniOrderStateNone;
		private System.Windows.Forms.ContextMenuStrip ctxOrder;
		private System.Windows.Forms.ImageList imgListOrderDirection;
		private BrightIdeasSoftware.TreeListView olvOrdersTree;
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
		private BrightIdeasSoftware.OLVColumn olvcBidAndAsk_whenEmitted;
		private BrightIdeasSoftware.OLVColumn olvcSlippageFilledMinusApplied;
		private BrightIdeasSoftware.OLVColumn olvcCommission;
		private LabeledTextBox.MenuItemLabeledTextBox mniltbFlushToGuiDelayMsec;
		private ToolStripMenuItem mniToggleColorifyOrdersTree;
		private ToolStripMenuItem mniToggleColorifyMessages;
		private ToolStripSeparator toolStripSeparator4;
		private ToolStripMenuItem mniOrderPositionClose;
		private ToolStripMenuItem mniOrderAlert_removeFromPending;
		private ToolStripMenuItem mniSerializeNow;
		private ToolStripMenuItem mniPosition_info;
		private ToolStripMenuItem mniExitAlert_info;
		private ToolStripSeparator toolStripSeparator5;
		private ToolStripSeparator toolStripSeparator6;
		private BrightIdeasSoftware.OLVColumn olvcSlippageFilled;
		private ToolStripMenuItem mniRecentAlwaysSelected;
		private ToolStripMenuItem mniToggleKillerOrders;
		private ToolStripMenuItem mniDeleteAllLogrotatedOrderJsons;
		private StatusStrip statusStrip_search;
		public ToolStripButton tsiBtnClear;
		private ToolStripStatusLabel tsilbl_separator;
		private ToolStripImproved.ToolStripItemCheckBox tsiCbx_SearchApply;
		private LabeledTextBox.ToolStripItemLabeledTextBox tsiLtb_SearchKeywords;
		private ToolStripImproved.ToolStripItemCheckBox tsiCbx_ExcludeApply;
		private LabeledTextBox.ToolStripItemLabeledTextBox tsiLtb_ExcludeKeywords;
		private LabeledTextBox.ToolStripItemLabeledTextBox mniltbLogrotateLargerThan;
		private BrightIdeasSoftware.OLVColumn olvcMessageSerno;
		private BrightIdeasSoftware.OLVColumn olvcBidAndAsk_whenFilled;
		private ContextMenuStrip ctxOrderStates;
		private ToolStripDropDownButton tsiDbxFilters;
		private ToolStripDropDownButton tsiDbxToggles;
		private ToolStripDropDownButton toolStripDropDownButton1;
		private ContextMenuStrip ctxFilters;
		private ToolStripMenuItem mniFilter_OrderStates;
		private ToolStripMenuItem mniFilter_Accounts;
		private ToolStripMenuItem mniFilter_Symbols;
		private ToolStripMenuItem mniFilter_Strategies;
	}
}
