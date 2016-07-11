namespace Sq1.Core.DataTypes {
	public enum ConnectionState {
		UnknownConnectionState								= 0,

		Streaming_JustInitialized_solidifiersUnsubscribed	= 1,
		Streaming_JustInitialized_solidifiersSubscribed		= 2,
		Streaming_DisconnectedJustConstructed				= 3,

		// used in QuikStreamingAdapter
		Streaming_UpstreamConnected_downstreamUnsubscribed		= 10,
		Streaming_UpstreamConnected_downstreamSubscribed		= 11,
		Streaming_UpstreamConnected_downstreamSubscribedAll		= 12,
		Streaming_UpstreamConnected_downstreamUnsubscribedAll	= 13,

		Streaming_UpstreamDisconnected_downstreamSubscribed		= 15,
		Streaming_UpstreamDisconnected_downstreamUnsubscribed	= 16,


		// used in QuikLivesimStreaming
		Streaming_DdeClientConnected		= 20,
		Streaming_DdeClientDisconnected		= 21,

		
		// used in QuikBroker
		Broker_TerminalConnected_22		= 50,	// step2of2, where 1=Sq1 connected to QUIK terminal, 2=Quik connected to server
		Broker_TerminalDisconnected		= 51,
		Broker_DllConnected_12			= 55,	// step1of2, where 1=Sq1 connected to QUIK terminal, 2=Quik connected to server
		Broker_DllConnecting			= 56,
		Broker_DllDisconnected			= 57,

		Broker_Connected_SymbolSubscribed				= 60,
		Broker_Connected_SymbolUnsubscribed				= 61,
		Broker_Connected_SymbolsSubscribedAll			= 62,
		Broker_Connected_SymbolsUnsubscribedAll			= 63,

		Broker_Disconnected_SymbolsSubscribedAll		= 64,
		Broker_Disconnected_SymbolsUnsubscribedAll		= 65,


		BrokerErrorConnectingNoRetriesAnymore = 70,
		//ErrorDisconnecting			= 13,
		//ErrorSymbolSubscribing		= 14,
		//ErrorSymbolUnsubscribing		= 15,

		FailedToConnect			= 90,
		FailedToDisconnect		= 91,
	}
}
