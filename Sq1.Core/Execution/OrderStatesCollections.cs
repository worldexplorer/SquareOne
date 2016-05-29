using System;
using System.Collections.Generic;

namespace Sq1.Core.Execution {
	public class OrderStatesCollections : List<OrderState> {
		public static OrderStatesCollections AllowedForSubmissionToBrokerAdapter =
			new OrderStatesCollections(new List<OrderState>() {
				OrderState.PreSubmit,
				OrderState.Submitting,
				OrderState.SubmittingSequenced,
				OrderState.VictimBulletPreSubmit,
				OrderState.KillerPreSubmit,
				OrderState.KillerSubmitting,
				OrderState.SubmittingSequenced
			}, "AllowedForSubmissionToBrokerAdapter");

		public static OrderStatesCollections NoInterventionRequired =
			new OrderStatesCollections(new List<OrderState>() {
				OrderState.LeaveTheSame,
				OrderState.Submitted,
				OrderState.WaitingBrokerFill,
				OrderState._TradeStatus,		// an order must never be actually analyzed for this status koz it never has it assigned
				OrderState._OrderStatus,		// an order must never be actually analyzed for this status koz it never has it assigned
				OrderState._TransactionStatus,	// an order must never be actually analyzed for this status koz it never has it assigned
				OrderState.KillerBulletFlying,
				OrderState.KillerTransSubmittedOK,
				OrderState.VictimBulletSubmitted,
				OrderState.VictimBulletFlying,
				OrderState.VictimBulletConfirmed,
				OrderState.SLAnnihilating,
				OrderState.SLAnnihilated,
				OrderState.TPAnnihilating,
				OrderState.TPAnnihilated,
				OrderState.IRefuseOpenTillEmergencyCloses,
			}, "NoInterventionRequired");

		public static OrderStatesCollections InterventionRequired =
			new OrderStatesCollections(new List<OrderState>() {
				OrderState.Rejected,
				OrderState.FilledPartially,
				OrderState.ErrorMarketPriceZero,
				OrderState.ErrorSlippageCalc,
				OrderState.ErrorCancelReplace,
				OrderState.ErrorSubmitting_BrokerTerminalDisconnected,
			}, "InterventionRequired");

		public static OrderStatesCollections CemeteryHealthy =
			new OrderStatesCollections(new List<OrderState>() {
				OrderState.AlertCreatedOnPreviousBarNotAutoSubmitted,
				OrderState.EmitOrdersNotClicked,
				OrderState.MarketClosed,
				OrderState.RejectedLimitReached,
				OrderState.Filled,
				OrderState.KillerDone,
				OrderState.KillerTransSubmittedOK,
				OrderState.SLAnnihilated,
				OrderState.TPAnnihilated,
				OrderState.RejectedLimitReached,
				OrderState.EmergencyCloseSheduledForNoReason,
				OrderState.EmergencyCloseSheduledForRejected,
				OrderState.EmergencyCloseSheduledForRejectedLimitReached,
				OrderState.EmergencyCloseSheduledForErrorSubmittingBroker,
				OrderState.EmergencyCloseComplete,
				OrderState.EmergencyCloseUserInterrupted,
				OrderState.EmergencyCloseLimitReached,
			}, "CemeteryHealthy");

		public static OrderStatesCollections CemeterySick =
			new OrderStatesCollections(new List<OrderState>() {
				OrderState.Error,
				OrderState.ErrorOrderInconsistent,
				OrderState.ErrorSubmittingOrder_classifyMe,
				OrderState.ErrorSubmittingOrder_unexecutableParameters,
				OrderState.ErrorSubmittingOrder_wrongAccount,
				OrderState.ErrorSubmittingNotEatable,
				OrderState.SubmittedNoFeedback,
				OrderState.IRefuseToCloseNonStreamingPosition,
				OrderState.IRefuseToCloseUnfilledEntry,
			}, "CemeterySick");

		public static OrderStatesCollections Unknown =
			new OrderStatesCollections(new List<OrderState>() {
				OrderState.Unknown
			}, "None");

		public static OrderStatesCollections CanBeKilled =
			new OrderStatesCollections(new List<OrderState>() {
				OrderState.WaitingBrokerFill,
				OrderState.SLAnnihilating,
				OrderState.TPAnnihilating,
			}, "CanBeKilled");
	
		
		public string CollectionName { get; private set; }

		public OrderStatesCollections() {
			throw new Exception("You can't pass an empty list, please Use predefined static collections");
		}

		OrderStatesCollections(List<OrderState> orderStates, string collectionName) {
			base.Clear();
			base.AddRange(orderStates);
			this.CollectionName = collectionName;
		}

		public override string ToString() {
			string ret = "";
			foreach (var status in this) {
				ret += status + ",";
			}
			ret = ret.TrimEnd(",".ToCharArray());
			return this.CollectionName + "[" + ret + "]";
		}
	}
}