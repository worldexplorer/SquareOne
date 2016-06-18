using System;

using Newtonsoft.Json;

namespace Sq1.Widgets.Execution {
	public class ExecutionTreeDataSnapshot {
		[JsonProperty]	public int		PricingDecimalForSymbol					= 0;
		
		[JsonProperty]	public bool		RecentAlwaysSelected					= true;
		[JsonProperty]	public bool		ShowBrokerTime							= true;
		[JsonProperty]	public bool		ShowCompletedOrders						= true;
		[JsonProperty]	public bool		ShowMessagesPane						= true;
		[JsonProperty]	public bool		ShowMessagePaneSplittedHorizontally		= false;
		[JsonProperty]	public bool		SingleClickSyncWithChart				= false;

		[JsonProperty]	public bool		ColorifyOrderTree_positionNet			= true;
		[JsonProperty]	public bool		ColorifyMessages_askBrokerProvider		= true;
		
		[JsonProperty]	public int		MessagePaneSplitDistanceHorizontal		= 0;
		[JsonProperty]	public int		MessagePaneSplitDistanceVertical		= 0;
		[JsonProperty]	public int		FlushToGuiDelayMsec						= 200;
		[JsonProperty]	public int		SerializationInterval					= 3000;
		
		[JsonProperty]	public string	OrdersTreeOlvStateBase64				= "";
	}
}
