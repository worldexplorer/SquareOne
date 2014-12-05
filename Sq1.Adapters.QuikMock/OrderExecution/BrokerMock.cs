using System;
using System.Drawing;

using Newtonsoft.Json;
using Sq1.Core;
using Sq1.Core.Broker;
using Sq1.Core.DataFeed;
using Sq1.Core.Execution;
using Sq1.Core.Streaming;

using Sq1.Adapters.Quik;
using Sq1.Adapters.QuikMock.Terminal;

namespace Sq1.Adapters.QuikMock {
	public class BrokerMock : BrokerQuik {
		[JsonIgnore]	public	QuikTerminalMock	MockTerminal;
		[JsonProperty]	public	int					ExecutionDelayMillis	{ get; internal set; }		// internal <= POPULATED_IN_EDITOR
		[JsonProperty]	public	int					RejectFirstNOrders		{ get; internal set; }		// internal <= POPULATED_IN_EDITOR
		[JsonProperty]	public	bool				RejectRandomly			{ get; internal set; }		// internal <= POPULATED_IN_EDITOR
		[JsonProperty]	public	bool				RejectAllUpcoming		{ get; internal set; }		// internal <= POPULATED_IN_EDITOR

		public BrokerMock() : base() {
			base.Name = "BrokerQuikMockDummy";
			base.Icon = (Bitmap)Sq1.Adapters.QuikMock.Properties.Resources.imgMockQuikStreamingProvider;
			base.QuikTerminal = new QuikTerminalMock(this);
			this.ExecutionDelayMillis = 1000;
			this.RejectFirstNOrders = 5;
			this.RejectRandomly = true;
			this.RejectAllUpcoming = false;
		}
		public override void Initialize(DataSource dataSource, StreamingProvider streamingProvider, OrderProcessor orderProcessor) {
			base.Initialize(dataSource, streamingProvider, orderProcessor);
			base.QuikTerminal.ConnectDll();
			base.Name = "BrokerQuikMock";
		}
		public override BrokerEditor BrokerEditorInitialize(IDataSourceEditor dataSourceEditor) {
			base.BrokerEditorInitializeHelper(dataSourceEditor);
			base.brokerEditorInstance = new BrokerMockEditor(this, dataSourceEditor);
			return base.brokerEditorInstance;
		}
		public override void CancelReplace(Order order, Order newOrder) {
			if (order.Alert.AccountNumber.StartsWith("Paper")) {
				//this.paperBrokerProvider_0.CancelReplace(order, newOrder);
				Assembler.PopupException("order[" + order + "].AccountNumber.StartsWith(Paper); returning");
				return;
			}
			if (order.GUID.Length > 10) {
				//base.TradeManager.updateOrderStatus(orderFromAlert.GUID, OrderStatus.Error, this.FidAuthProvider.GetFidelityTime()
				//	, 0.0, 0.0, 0, "Error(s): Can'tp replace an orderFromAlert in submitted statusOut");
				//base.TradeManager.updateOrderStatus(newOrder.GUID, OrderStatus.ErrorCancelReplace
				//, this.FidAuthProvider.GetFidelityTime(), 0.0, 0.0, 0, "Error(s): Can'tp replace an orderFromAlert in submitted statusOut");
				Assembler.PopupException("order.Guid.Length[" + order.GUID.Length + "] > 10; returning");
				return;
			}
		}
	}
}