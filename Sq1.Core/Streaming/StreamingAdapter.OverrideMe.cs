using System;

using Sq1.Core.DataFeed;
using Sq1.Core.DataTypes;

namespace Sq1.Core.Streaming {
	public partial class StreamingAdapter {

		public virtual void InitializeDataSource_inverse(DataSource dataSource, bool subscribeSolidifier = true) {
			this.InitializeFromDataSource(dataSource);
			if (subscribeSolidifier == false) return;
			this.SolidifierSubscribe_toAllSymbols_ofDataSource_onAppRestart();
		}

		#region the essence#1 of streaming adapter
		public virtual void UpstreamConnect() {
			//StatusReporter.UpdateConnectionStatus(ConnectionState.ErrorConnecting, 0, "ConnectStreaming(): NOT_OVERRIDEN_IN_CHILD");
			Assembler.DisplayStatus("ConnectStreaming(): NOT_OVERRIDEN_IN_CHILD " + this.ToString());
		}
		public virtual void UpstreamDisconnect() {
			//StatusReporter.UpdateConnectionStatus(ConnectionState.ErrorDisconnecting, 0, "DisconnectStreaming(): NOT_OVERRIDEN_IN_CHILD");
			Assembler.DisplayStatus("DisconnectStreaming(): NOT_OVERRIDEN_IN_CHILD " + this.ToString());
		}
		#endregion

		#region the essence#2 of streaming adapter
		public virtual void UpstreamSubscribe(string symbol) {
			throw new Exception("please override StreamingAdapter::UpstreamSubscribe()");
			//CHILDREN_TEMPLATE: base.UpstreamSubscribeRegistryHelper(symbol);
		}
		public virtual void UpstreamUnSubscribe(string symbol) {
			throw new Exception("please override StreamingAdapter::UpstreamUnSubscribe()");
			//CHILDREN_TEMPLATE: base.UpstreamUnSubscribeRegistryHelper(symbol);
		}
		public virtual bool UpstreamIsSubscribed(string symbol) {
			throw new Exception("please override StreamingAdapter::UpstreamIsSubscribed()");
			//CHILDREN_TEMPLATE: return base.UpstreamIsSubscribedRegistryHelper(symbol);
		}
		#endregion

		public virtual void EnrichQuote_withStreamingDependant_dataSnapshot(Quote quote) {
			// in Market-dependant StreamingAdapters, put in the Quote-derived quote anything like QuikQuote.FortsDepositBuy ;
		}


	}
}
