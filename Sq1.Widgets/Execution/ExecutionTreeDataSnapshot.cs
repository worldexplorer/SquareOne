using System;
using System.Collections.Generic;

namespace Sq1.Widgets.Execution {
	public class ExecutionTreeDataSnapshot {
		public bool	firstRowShouldStaySelected				= true;
		public int	pricingDecimalForSymbol					= 0;
		
		public bool ToggleBrokerTime						= false;
		public bool ToggleCompletedOrders					= true;
		public bool ToggleMessagesPane						= true;
		public bool ToggleMessagePaneSplittedHorizontally	= false;
		public bool	ToggleSingleClickSyncWithChart			= true;
		
		public int	MessagePaneSplitDistanceHorizontal		= 0;
		public int	MessagePaneSplitDistanceVertical		= 0;
		
		//v1 prior to using this.OrdersTreeOLV.SaveState();
		//NOPE_I_HATE_ADDING_IF_MISSED
		//public Dictionary<string, bool>	ColumnsShown		= new Dictionary<string, bool>();
		
//		public bool ShowColumnGUID						= true;
//		public bool ShowColumnReplacedByGUID			= true;
//		public bool ShowColumnKilledByGUID				= true;
//		public bool ShowColumnBarNum					= true;
//		public bool ShowColumnDatetime					= true;
//		public bool ShowColumnSymbol					= true;
//		public bool ShowColumnDirection					= true;
//		public bool ShowColumnOrderType					= true;
//		public bool ShowColumnPriceScript				= true;
//		public bool ShowColumnSpreadSide				= true;
//		public bool ShowColumnPriceScriptRequested		= true;
//		public bool ShowColumnPriceFilled				= true;
//		public bool ShowColumnQtyRequested				= true;
//		public bool ShowColumnQtyFilled					= true;
//		public bool ShowColumnPriceDeposited			= true;
//		public bool ShowColumnSernoSession				= true;
//		public bool ShowColumnSernoExchange				= true;
//		public bool ShowColumnStrategyName				= true;
//		public bool ShowColumnSignalName				= true;
//		public bool ShowColumnScale						= true;
//		public bool ShowColumnLastMessage				= true;
		
		//v2
		public string OrdersTreeOlvStateBase64				= "";
	}
}
