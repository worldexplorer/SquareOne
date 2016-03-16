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
		LimitExpired			= 310,
		RejectedLimitReached	= 330,


		Filled					= 400,
		FilledPartially			= 410,
		_TradeStatus			= 415,


		KillerPreSubmit			= 510,		// KillerPreSubmit			<=> VictimsBulletPreSubmit		Market order that is Filled - needs a KillerOrder to be Killed (most likely another Market with inverted Size)
		KillerSubmitting		= 520,		// KillerSubmitting			<=> VictimsBulletConfirmed		
		KillerTransSubmittedOK	= 530,		// KillerTransSubmittedOK	<=> VictimsBulletSubmitted		???Limit order that wasn't Filled yet - doesn't need a KillerOrder to be KilledPending
		KillerBulletFlying		= 540,		// KillerBulletFlying		<=> VictimsBulletFlying
		KillerDone				= 550,		// KillerDone				<=> VictimKilled				simulated in BrokerQuik: victim is cancelled, no broker callback on a "virtual" KillerOrder

		SLAnnihilated			= 580,
		TPAnnihilated			= 581,

		VictimsBulletPreSubmit	= 610,		// KillerPreSubmit			<=> VictimsBulletPreSubmit
		VictimsBulletConfirmed	= 620,		// KillerSubmitting			<=> VictimsBulletConfirmed		
		VictimsBulletSubmitted	= 630,		// KillerTransSubmittedOK	<=> VictimsBulletSubmitted
		VictimsBulletFlying		= 640,		// KillerBulletFlying		<=> VictimsBulletFlying
		VictimKilled			= 650,		// KillerDone				<=> VictimKilled


		Error						= 700,
		ErrorOrderInconsistent		= 701,
		ErrorSubmittingOrder		= 702,
		ErrorSubmittingNotEatable	= 703,
		ErrorSubmitting_KillerOrder	= 704,
		ErrorMarketPriceZero		= 705,
		ErrorSlippageCalc			= 706,
		ErrorCancelReplace			= 707,
		SubmittedNoFeedback			= 708,

		ErrorSubmitting_BrokerTerminalDisconnected	= 720,
		ErrorSubmitting_BrokerDllDisconnected		= 721,

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
