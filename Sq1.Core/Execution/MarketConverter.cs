using System;
using System.Drawing;
using System.Collections.Generic;
using Sq1.Core.DataTypes;
using Sq1.Core.StrategyBase;

namespace Sq1.Core.Execution {
	public class MarketConverter {
		public static Brush AlertPendingToBrushColor(Alert alert) {
			Brush brush = Brushes.Black;
			switch (alert.Direction) {
				case Direction.Buy:
					brush = Brushes.Blue;
					break;
				case Direction.Sell:
					brush = Brushes.Red;
					break;
				case Direction.Short:
					brush = Brushes.Fuchsia;
					break;
				case Direction.Cover:
					brush = Brushes.Green;
					break;
			}
			return brush;
		}
		public static PositionLongShort LongShortFromDirection(Direction direction) {
			switch (direction) {
				case Direction.Buy:
				case Direction.Cover:
					return PositionLongShort.Long;
				case Direction.Sell:
				case Direction.Short:
					return PositionLongShort.Short;
				default:
					string msg = " No Direction[" + direction + "] handler "
						+ "; must be one of those: Buy/Cover/Sell/Short";
					throw new Exception(msg);
			}
		}
		public static Direction EntryDirectionFromLongShort(PositionLongShort positionLongShort) {
			switch (positionLongShort) {
				case PositionLongShort.Long:
					return Direction.Buy;
				case PositionLongShort.Short:
					return Direction.Short;
				default:
					string msg = " No PositionLongShort[" + positionLongShort + "] handler "
						+ "; must be one of those: Long/Short";
					throw new Exception(msg);
			}
		}
		public static Direction ExitDirectionFromLongShort(PositionLongShort positionToClose) {
			switch (positionToClose) {
				case PositionLongShort.Long:
					return Direction.Sell;
				case PositionLongShort.Short:
					return Direction.Cover;
				default:
					string msg = " No PositionLongShort[" + positionToClose + "] handler "
						+ "; must be one of those: Long/Short";
					throw new Exception(msg);
			}
		}
		// http://forum.mql4.com/6611
		public static MarketLimitStop EntryMarketLimitStopFromDirection(double priceCurrent,
				double priceExecutionDesired, PositionLongShort positionDesired) {
			if (priceExecutionDesired == 0) return MarketLimitStop.Market;
			if (priceExecutionDesired == priceCurrent) return MarketLimitStop.Market;
			switch (positionDesired) {
				case PositionLongShort.Long:
					if (priceExecutionDesired > priceCurrent) return MarketLimitStop.Stop;
					return MarketLimitStop.Limit;
				case PositionLongShort.Short:
					if (priceExecutionDesired < priceCurrent) return MarketLimitStop.Stop;
					return MarketLimitStop.Limit;
				default:
					string msg = " No PositionLongShort[" + positionDesired + "] handler "
						+ "; must be one of those: Long/Short";
					throw new Exception(msg);
			}
		}

		public static double ExitPriceFromOffset(PositionLongShort positionToClose,
				double priceEntered, double priceOffsetSigned) {
			switch (positionToClose) {
				case PositionLongShort.Long:
					if (priceOffsetSigned > 0) return priceEntered + priceOffsetSigned;
					throw new Exception("WRONG! PositionPrototype contained negative offset for Long Take Profit");
				case PositionLongShort.Short:
					if (priceOffsetSigned < 0) return priceEntered + priceOffsetSigned;
					throw new Exception("WRONG! PositionPrototype contained positive offset for Short Take Profit");
				default:
					string msg = " No PositionLongShort[" + positionToClose + "] handler "
						+ "; must be one of those: Long/Short";
					throw new Exception(msg);
			}
		}

		public static bool IsEntryFromDirection(Direction direction) {
			switch (direction) {
				case Direction.Buy:
				case Direction.Short:
					return true;
				case Direction.Cover:
				case Direction.Sell:
					return false;
				default:
					string msg = " No Direction[" + direction + "] handler"
						+ "; must be one of those: Buy/Cover/Sell/Short";
					throw new Exception(msg);
			}
		}
		public static BidOrAsk BidOrAskWillFillAlert(Alert alert) {
			BidOrAsk ret = BidOrAsk.UNKNOWN;
			switch (alert.MarketLimitStop) {
				case MarketLimitStop.Market:
				case MarketLimitStop.Limit:
					switch (alert.Direction) {
						//http://www.metatrader5.com/en/terminal/help/trading/general_concept/order_types
						case Direction.Short:
						case Direction.Sell:
							ret = BidOrAsk.Bid;
							break;
						case Direction.Buy:
						case Direction.Cover:
							ret = BidOrAsk.Ask;
							break;
						default:
							throw new Exception("BidOrAskWillFillAlert(): NYI direction[" + alert.Direction + "] for [" + alert + "]");
					}
					break;
				case MarketLimitStop.Stop:
				case MarketLimitStop.StopLimit:
					switch (alert.Direction) {
						//http://www.metatrader5.com/en/terminal/help/trading/general_concept/order_types
						case Direction.Short:
						case Direction.Sell:
							ret = BidOrAsk.Bid;
							break;
						case Direction.Buy:
						case Direction.Cover:
							ret = BidOrAsk.Ask;
							break;
						default:
							throw new Exception("BidOrAskWillFillAlert(): NYI direction[" + alert.Direction + "] for [" + alert + "]");
					}
					break;
			}
			return ret;
		}
	}
}
