using System;

using Newtonsoft.Json;

namespace Sq1.Widgets.Execution {
	public class ExecutionTreeDataSnapshot {
		[JsonProperty]	public bool	FirstRowShouldStaySelected				= true;
		[JsonProperty]	public int	PricingDecimalForSymbol					= 0;
		
		[JsonProperty]	public bool ToggleBrokerTime						= false;
		[JsonProperty]	public bool ToggleCompletedOrders					= true;
		[JsonProperty]	public bool ToggleMessagesPane						= true;
		[JsonProperty]	public bool ToggleMessagePaneSplittedHorizontally	= false;
		[JsonProperty]	public bool	ToggleSingleClickSyncWithChart			= false;
		
		[JsonProperty]	public int	MessagePaneSplitDistanceHorizontal		= 0;
		[JsonProperty]	public int	MessagePaneSplitDistanceVertical		= 0;
		[JsonProperty]	public int	SerializationInterval					= 3000;
		
		//v1 prior to using this.OrdersTreeOLV.SaveState();
		//NOPE_I_HATE_ADDING_IF_MISSED
		//[JsonProperty]	public Dictionary<string, bool>	ColumnsShown		= new Dictionary<string, bool>();
		
//		[JsonProperty]	public bool ShowColumnGUID						= true;
//		[JsonProperty]	public bool ShowColumnReplacedByGUID			= true;
//		[JsonProperty]	public bool ShowColumnKilledByGUID				= true;
//		[JsonProperty]	public bool ShowColumnBarNum					= true;
//		[JsonProperty]	public bool ShowColumnDatetime					= true;
//		[JsonProperty]	public bool ShowColumnSymbol					= true;
//		[JsonProperty]	public bool ShowColumnDirection					= true;
//		[JsonProperty]	public bool ShowColumnOrderType					= true;
//		[JsonProperty]	public bool ShowColumnPriceScript				= true;
//		[JsonProperty]	public bool ShowColumnSpreadSide				= true;
//		[JsonProperty]	public bool ShowColumnPriceScriptRequested		= true;
//		[JsonProperty]	public bool ShowColumnPriceFilled				= true;
//		[JsonProperty]	public bool ShowColumnQtyRequested				= true;
//		[JsonProperty]	public bool ShowColumnQtyFilled					= true;
//		[JsonProperty]	public bool ShowColumnPriceDeposited			= true;
//		[JsonProperty]	public bool ShowColumnSernoSession				= true;
//		[JsonProperty]	public bool ShowColumnSernoExchange				= true;
//		[JsonProperty]	public bool ShowColumnStrategyName				= true;
//		[JsonProperty]	public bool ShowColumnSignalName				= true;
//		[JsonProperty]	public bool ShowColumnScale						= true;
//		[JsonProperty]	public bool ShowColumnLastMessage				= true;
		
		//v2
		[JsonProperty]	public string OrdersTreeOlvStateBase64				= "";
	}
}
