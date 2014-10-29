using System;
using System.Drawing;
using System.Runtime.Serialization;

using Sq1.Adapters.Quik;
using Sq1.Adapters.QuikMock.Terminal;
using Sq1.Core;
using Sq1.Core.Broker;
using Sq1.Core.DataFeed;
using Sq1.Core.Execution;
using Sq1.Core.Streaming;
using Sq1.Core.Support;

namespace Sq1.Adapters.QuikMock {
	[DataContract]
	public class BrokerMock : BrokerQuik {
		public QuikTerminalMock MockTerminal;

		[DataMember]
		public int ExecutionDelayMillis { get; internal set; }
		[DataMember]
		public int RejectFirstNOrders { get; internal set; }
		[DataMember]
		public bool RejectRandomly { get; internal set; }
		[DataMember]
		public bool RejectAllUpcoming { get; internal set; }

		public BrokerMock() : base() {
			base.Name = "Mock BrokerDummy";
			base.Icon = (Bitmap)Sq1.Adapters.QuickMock.Properties.Resources.imgMockQuikStreamingProvider;
			base.QuikTerminal = new QuikTerminalMock(this);
			this.ExecutionDelayMillis = 1000;
			this.RejectFirstNOrders = 5;
			this.RejectRandomly = true;
			this.RejectAllUpcoming = false;
		}
		public override void Initialize(DataSource dataSource, StreamingProvider streamingProvider, OrderProcessor orderProcessor, IStatusReporter connectionStatus) {
			base.Initialize(dataSource, streamingProvider, orderProcessor, connectionStatus);
			base.QuikTerminal.ConnectDll();
			base.Name = "Mock BrokerProvider";
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