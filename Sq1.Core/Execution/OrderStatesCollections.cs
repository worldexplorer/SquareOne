using System;
using System.Collections.Generic;

namespace Sq1.Core.Execution {
	public class OrderStatesCollections : List<OrderState> {
		public static OrderStatesCollections AllowedForSubmissionToBrokerAdapter =
			new OrderStatesCollections(new List<OrderState>() {
				OrderState.PreSubmit,
				OrderState.Submitting,
				OrderState.SubmittingSequenced,
				OrderState.KillerPreSubmit,
				OrderState.KillerSubmitting,
				OrderState.SubmittingSequenced
			}, "AllowedForSubmissionToBrokerAdapter");

		public static OrderStatesCollections NoInterventionRequired =
			new OrderStatesCollections(new List<OrderState>() {
				OrderState.LeaveTheSame,
				OrderState.Submitted,
				OrderState.WaitingBrokerFill,
				OrderState.TradeStatus,
				OrderState.KillerBulletFlying,
				OrderState.KillPending,
				OrderState.IRefuseOpenTillEmergencyCloses,
			}, "NoInterventionRequired");

		public static OrderStatesCollections InterventionRequired =
			new OrderStatesCollections(new List<OrderState>() {
				OrderState.Rejected,
				OrderState.FilledPartially,
				OrderState.ErrorMarketPriceZero,
				OrderState.ErrorSlippageCalc,
				OrderState.ErrorCancelReplace,
				OrderState.ErrorSubmittingBroker,
			}, "InterventionRequired");

		public static OrderStatesCollections CemeteryHealthy =
			new OrderStatesCollections(new List<OrderState>() {
				OrderState.AlertCreatedOnPreviousBarNotAutoSubmitted,
				OrderState.AutoSubmitNotEnabled,
				OrderState.MarketClosed,
				OrderState.RejectedLimitReached,
				OrderState.Filled,
				OrderState.KillerDone,
				OrderState.Killed,
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
				OrderState.ErrorSubmitOrder,
				OrderState.ErrorSubmittingNotEatable,
				OrderState.SubmittedNoFeedback,
				OrderState.IRefuseToCloseNonStreamingPosition,
				OrderState.IRefuseToCloseUnfilledEntry,
			}, "CemeterySick");

		public static OrderStatesCollections Unknown =
			new OrderStatesCollections(new List<OrderState>() {
				OrderState.Unknown
			}, "None");

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