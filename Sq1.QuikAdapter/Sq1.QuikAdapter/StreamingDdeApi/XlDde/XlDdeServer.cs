//    XlDdeServer.cs (c) 2011 Nikolay Moroshkin, http://www.moroshkin.com/
using System.Collections.Generic;
using NDde.Server;

namespace Sq1.QuikAdapter.StreamingDdeApi.XlDde {
	class XlDdeServer : DdeServer {
		Dictionary<string, XlDdeChannel> channels;
		public XlDdeServer(string service) : base(service) {
			channels = new Dictionary<string, XlDdeChannel>();
		}
		public void AddChannel(string topic, XlDdeChannel channel) {
			if (channels.ContainsKey(topic)) return;
			channels.Add(topic, channel);
		}
		public void RemoveChannel(string topic) {
			if (channels.ContainsKey(topic) == false) return;
			channels.Remove(topic);
		}
		protected override bool OnBeforeConnect(string topic) {
			return channels.ContainsKey(topic);
		}
		protected override void OnAfterConnect(DdeConversation c) {
			XlDdeChannel channel = channels[c.Topic];
			c.Tag = channel;
			channel.IsConnected = true;
		}
		protected override void OnDisconnect(DdeConversation c) {
			((XlDdeChannel)c.Tag).IsConnected = false;
		}
		protected override PokeResult OnPoke(DdeConversation c, string item, byte[] data, int format) {
			//if(format != xlTableFormat) return PokeResult.NotProcessed;

			((XlDdeChannel)c.Tag).PutDdeData(data);
			return PokeResult.Processed;
		}
	}
}
