using System;

using Newtonsoft.Json;
using System.Drawing;

namespace Sq1.Widgets.Execution {
	public class ExecutionTreeDataSnapshot {
		[JsonProperty]	public int		PricingDecimalForSymbol					= 0;
		
		[JsonProperty]	public bool		RecentAlwaysSelected					= true;
		[JsonProperty]	public bool		ShowBrokerTime							= true;
		[JsonProperty]	public bool		ShowCompletedOrders						= true;
		[JsonProperty]	public bool		ShowMessagesPane						= true;
		[JsonProperty]	public bool		ShowMessagePaneSplittedHorizontally		= false;
		[JsonProperty]	public bool		ShowKillerOrders						= true;
		[JsonProperty]	public bool		SingleClickSyncWithChart				= false;

		[JsonProperty]	public bool		ColorifyOrderTree_positionNet			= true;
		[JsonProperty]	public bool		ColorifyMessages_askBrokerProvider		= true;
		[JsonProperty]	public Color	ColorBackground_forMessagesThatChangedOrderState = Color.Gainsboro;
		
		[JsonProperty]	public int		MessagePane_splitDistance_horizontal	= 0;
		[JsonProperty]	public int		MessagePane_splitDistance_vertical		= 0;
		[JsonProperty]	public int		FlushToGuiDelayMsec						= 200;
		[JsonProperty]	public int		SerializationInterval_Millis			= 3000;
		[JsonProperty]	public float	LogRotateSizeLimit_Mb					= 10.0f;
		
		[JsonProperty]	public string	OrdersTreeOlvStateBase64				= "";

		[JsonProperty]	public bool		ShowSearchbar						= true;
		[JsonProperty]	public string	ShowSearchbar_ExcludeKeywordsCsv	= "";
		[JsonProperty]	public bool		ShowSearchbar_SearchKeywordApplied	= false;
		[JsonProperty]	public string	ShowSearchbar_SearchKeywordsCsv		= "";
		[JsonProperty]	public bool		ShowSearchbar_ExcludeKeywordApplied	= false;
	}
}
