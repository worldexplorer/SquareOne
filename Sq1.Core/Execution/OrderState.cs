namespace Sq1.Core.Execution {
	public enum OrderState {
		JustConstructed			= 0,
		Unknown					= 1,
		LeaveTheSame			= 2,

		AlertCreatedOnPreviousBarNotAutoSubmitted = 100,
		EmitOrdersNotClicked	= 110,
		MarketClosed			= 120,

		PreSubmit				= 200,
		Submitting				= 210,
		//SubmittingAsync		= 220,		// Rejected and other *PostProcessors should consider SubmittingAsync equivalent to Submitting
		_TransactionStatus		= 215,
		SubmittingSequenced		= 230,
		Submitted				= 240,
		WaitingBrokerFill		= 250,
		_OrderStatus			= 255,

		Rejected				= 300,
		RejectedLimitReached	= 330,
		Filled					= 400,
		FilledPartially			= 410,
		_TradeStatus			= 415,

		//KillSubmittingAsync	= 610,	// everything is Async! like for SubmittingAsync
		KillPendingPreSubmit	= 500,
		KillPendingSubmitting	= 510,
		KillPendingSubmitted	= 520,
		KilledPending			= 530,		// a Limit order that wasn't Filled yet - doesn't need a KillerOrder to be KilledPending

		KillerPreSubmit			= 610,		// Market order that is Filled - needs a KillerOrder to be Killed (most likely another Market with inverted Size)
		KillerSubmitting		= 620,
		KillerBulletFlying		= 630,		// KillerBulletFlying = KillerSubmitted + KillerActive
		KillerDone				= 640,		// simulated in BrokerQuik: victim is cancelled, no broker callback on a "virtual" KillerOrder

		SLAnnihilated			= 680,
		TPAnnihilated			= 681,

		Error						= 700,
		ErrorOrderInconsistent		= 701,
		ErrorSubmitOrder			= 702,
		ErrorSubmittingNotEatable	= 703,
		ErrorMarketPriceZero		= 704,
		ErrorSlippageCalc			= 705,
		ErrorCancelReplace			= 706,
		ErrorSubmittingBroker		= 707,
		SubmittedNoFeedback			= 708,

		IRefuseToCloseNonStreamingPosition	= 800,
		IRefuseToCloseUnfilledEntry			= 801,
		IRefuseOpenTillEmergencyCloses		= 802,

		EmergencyCloseSheduledForNoReason				= 900,
		EmergencyCloseSheduledForRejected				= 901,
		EmergencyCloseSheduledForRejectedLimitReached	= 902,
		EmergencyCloseSheduledForErrorSubmittingBroker	= 903,
		EmergencyCloseComplete							= 910,
		EmergencyCloseUserInterrupted					= 911,
		EmergencyCloseLimitReached						= 912
	}
}
