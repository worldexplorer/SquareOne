using System;
using System.Collections.Generic;

namespace Sq1.Core.Streaming {
	public partial class Distributor<STREAMING_CONSUMER_CHILD> {
				static	int																		AllDistributorsEverCreated_total;
				static	Dictionary<Type, Dictionary<StreamingAdapter, List<Distributor<STREAMING_CONSUMER_CHILD>>>>	allDistributorsEverCreated;
		public	static	Dictionary<Type, Dictionary<StreamingAdapter, List<Distributor<STREAMING_CONSUMER_CHILD>>>>	AllDistributorsEverCreated 	{ get {
			if (Distributor<STREAMING_CONSUMER_CHILD>.allDistributorsEverCreated == null) Distributor<STREAMING_CONSUMER_CHILD>.allDistributorsEverCreated = new Dictionary<Type, Dictionary<StreamingAdapter, List<Distributor<STREAMING_CONSUMER_CHILD>>>>();
			return Distributor<STREAMING_CONSUMER_CHILD>.allDistributorsEverCreated;
		} }

		void storeAllInstancesEverCreated(StreamingAdapter streamingAdapter) {
			Type adapterType = streamingAdapter.GetType();
			if (Distributor<STREAMING_CONSUMER_CHILD>.AllDistributorsEverCreated.ContainsKey(adapterType) == false) {
				Distributor<STREAMING_CONSUMER_CHILD>.AllDistributorsEverCreated.Add(adapterType, new Dictionary<StreamingAdapter, List<Distributor<STREAMING_CONSUMER_CHILD>>>());
			}
			if (Distributor<STREAMING_CONSUMER_CHILD>.AllDistributorsEverCreated[adapterType].ContainsKey(streamingAdapter) == false) {
				Distributor<STREAMING_CONSUMER_CHILD>.AllDistributorsEverCreated[adapterType].Add(streamingAdapter, new List<Distributor<STREAMING_CONSUMER_CHILD>>());
			}
			Distributor<STREAMING_CONSUMER_CHILD>.AllDistributorsEverCreated[adapterType][streamingAdapter].Add(this);
			Distributor<STREAMING_CONSUMER_CHILD>.AllDistributorsEverCreated_total++;
		}
	}
}