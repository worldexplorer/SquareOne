namespace Sq1.Core.DataTypes {
	public enum ConnectionState {
		//OK,
		//Error,
		//Warning
		Unknown = 0,
		JustInitialized_solidifiersUnsubscribed = 1,
		JustInitialized_solidifiersSubscribed = 2,
		DisconnectedJustConstructed = 3,

		// used in QuikBrokerAdapter
		UpstreamConnected_downstreamUnsubscribed = 10,
		UpstreamConnected_downstreamSubscribed = 11,
		UpstreamConnected_downstreamSubscribedAll = 12,
		UpstreamConnected_downstreamUnsubscribedAll = 13,

		UpstreamDisconnected_downstreamSubscribed = 15,
		UpstreamDisconnected_downstreamUnsubscribed = 16,

		SymbolSubscribed = 30,
		SymbolUnsubscribed = 31,
		ErrorConnectingNoRetriesAnymore = 32,
		//ErrorDisconnecting = 13,
		//ErrorSymbolSubscribing = 14,
		//ErrorSymbolUnsubscribing = 15,

		// used in QuikLivesimStreaming
		DdeClientConnected = 70,
		DdeClientDisconnected = 71,

		ConnectFailed = 90,
		DisconnectFailed = 91,
	}
}
