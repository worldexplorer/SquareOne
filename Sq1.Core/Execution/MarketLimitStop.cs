namespace Sq1.Core.Execution {
// http://www.metatrader5.com/en/terminal/help/trading/general_concept/order_types
//Buy Limit  trade request to buy at the Ask price that is equal to or lower
//	than that specified in the order. The current price level is higher
//	than the value in the order. Usually this order is placed in anticipation of that
//	the security price, having fallen to a certain level, will increase;
//Buy Stop  trade request to buy at the Ask price that is equal to or higher
//	than that specified in the order. The current price level is lower
//	than the value in the order. Usually this order is placed in anticipation of that
//	the security price, having reached a certain level, will keep on increasing;
//Sell Limit  trade request to sell at the Bid price that is equal to or higher
//	than that specified in the order. The current price level is lower
//	than the value in the order. Usually this order is placed in anticipation of that
//	the security price, having increased to a certain level, will fall;
//Sell Stop  trade request to sell at the Bid price that is equal to or lower
//	than that specified in the order. The current price level is higher
//	than the value in the order. Usually this order is placed in anticipation of that
//	the security price, having reached a certain level, will keep on falling;
//Buy Stop Limit  this type is the combination of the two first types being a
//	stop order for placing Buy Limit. As soon as the future Ask price reaches
//	the stop-level indicated in the order (the Price field), a Buy Limit order
//	will be placed at the level, specified in Stop Limit price field. The stop-level is set above
//	the current Ask price, and the Stop Limit price is set below the stop-level.
//Sell Stop Limit  this type is a stop order for placing Sell Limit. As soon as the future Bid price
//	reaches the stop-level indicated in the order (the Price field), a Sell Limit order
//	will be placed at the level, specified in Stop Limit price field. The stop-level is set below
//	the current Bid price, and the Stop Limit price is set above the stop-level.
//Take Profit order is intended for gaining the profit when the security price has reached
//	a certain level. Execution of this order results in complete closing of the whole position.
//	It is always connected to an open position or a pending order. The order can be requested
//	only together with a market or a pending order. Terminal checks long positions with Bid
//	price for meeting of this order provisions (the order is always set above the current Bid price),
//	and it does with Ask price for short positions (the order is always set below the current Ask price).
//Stop Loss is used for minimizing of losses if the security price has started to move in
//	an unprofitable direction. If the security price reaches this level, the whole position
//	will be closed automatically. Such orders are always connected to an open position or a pending order.
//	They can be requested only together with a market or a pending order. Terminal checks long positions with Bid
//	price for meeting of this order provisions (the order is always set below the current Bid price),
//	and it does with Ask price for short positions (the order is always set above the current Ask price).
	public enum MarketLimitStop {
		Unknown = 0,
		Market = 1,
		Limit = 2,
		Stop = 3,
		StopLimit = 4,
		AtClose = 5
	}
}
