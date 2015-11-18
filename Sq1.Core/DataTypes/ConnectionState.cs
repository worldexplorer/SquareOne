namespace Sq1.Core.DataTypes {
	public enum ConnectionState {
		//OK,
		//Error,
		//Warning
		Unknown = 0,
		JustInitialized = 1,
		InitiallyDisconnected = 2,

		// used in QuikBrokerAdapter
		DllNotConnectedUnsubscribed = 3,
		DllConnectedUnsubscribed = 4,
		SymbolSubscribed = 10,
		SymbolUnsubscribed = 11,
		ErrorConnectingNoRetriesAnymore = 12,
		//ErrorDisconnecting = 13,
		//ErrorSymbolSubscribing = 14,
		//ErrorSymbolUnsubscribing = 15,

		// used in QuikLivesimStreaming
		SymbolsSubscribedAllDataSource = 50,
		ConnectedSubscribedAll = 51,
		DisconnectedUnsubscribedAll = 70,
		ConnectFailed = 90,
		DisconnectFailed = 91,
	}
}
