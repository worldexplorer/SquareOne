namespace Sq1.Core.Execution {
	public enum OrderState {
		JustCreated = 0,
		Unknown = 1,
		LeaveTheSame = 2,
		AlertCreatedOnPreviousBarNotAutoSubmitted = 100,
		AutoSubmitNotEnabled = 110,
		MarketClosed = 120,
		PreSubmit = 200,
		Submitting = 210,
		//Rejected and other *PostProcessors should consider SubmittingAsync equivalent to Submitting
		//SubmittingAsync = 220,
		SubmittingSequenced = 230,
		Submitted = 240,
		WaitingBrokerFill = 250,
		Rejected = 300,
		RejectedLimitReached = 330,
		Filled = 400,
		FilledPartially = 410,
		TradeStatus = 450,
		//too much of details, like for SubmittingAsync
		//KillSubmittingAsync = 610,
		// the only Killer order StateFlow: KillerSubmitting => KillerBulletFlying => KillerDone
		KillerPreSubmit = 600,
		KillerSubmitting = 610,
		KillerBulletFlying = 620,	// KillerBulletFlying = KillerSubmitted + KillerActive
		KillerDone = 630,	// simulated in BrokerQuikProvider: victim is cancelled, no broker callback on a "virtual" KillerOrder
		// the only Victim order StateFlow: KillPending => Killed
		KillPending = 640,
		Killed = 650,
		SLAnnihilated = 660,
		TPAnnihilated = 661,
		Error = 700,
		ErrorOrderInconsistent = 701,
		ErrorSubmitOrder = 702,
		ErrorSubmittingNotEatable = 703,
		ErrorMarketPriceZero = 704,
		ErrorSlippageCalc = 705,
		ErrorCancelReplace = 706,
		ErrorSubmittingBroker = 707,
		SubmittedNoFeedback = 708,
		IRefuseToCloseNonStreamingPosition = 800,
		IRefuseToCloseUnfilledEntry = 801,
		IRefuseOpenTillEmergencyCloses = 802,
		EmergencyCloseSheduledForNoReason = 900,
		EmergencyCloseSheduledForRejected = 901,
		EmergencyCloseSheduledForRejectedLimitReached = 902,
		EmergencyCloseSheduledForErrorSubmittingBroker = 903,
		EmergencyCloseComplete = 910,
		EmergencyCloseUserInterrupted = 911,
		EmergencyCloseLimitReached = 912
	}
}