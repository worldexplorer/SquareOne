namespace Sq1.Core.DataTypes {
	public enum ConnectionState {
		//OK,
		//Error,
		//Warning
		Unknown = 0,
		JustInitialized = 1,
		ConnectedUnsubscribed = 2,
		Disconnected = 3,
		SymbolSubscribed = 4,
		SymbolUnsubscribed = 5,
		ErrorConnecting = 6,
		ErrorDisconnecting = 7,
		ErrorSymbolSubscribing = 8,
		ErrorSymbolUnsubscribing = 9
	}
}
