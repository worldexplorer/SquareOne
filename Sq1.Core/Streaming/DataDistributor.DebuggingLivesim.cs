using System;
using System.Collections.Generic;

using Sq1.Core.DataTypes;
using Sq1.Core.Charting;
using Sq1.Core.Backtesting;
using Sq1.Core.Livesim;

namespace Sq1.Core.Streaming {
	public partial class DataDistributor {
				static	int																		AllDataDistributorsEverCreated_total;
				static	Dictionary<Type, Dictionary<StreamingAdapter, List<DataDistributor>>>	allDataDistributorsEverCreated;
		public	static	Dictionary<Type, Dictionary<StreamingAdapter, List<DataDistributor>>>	AllDataDistributorsEverCreated 	{ get {
			if (DataDistributor.allDataDistributorsEverCreated == null) DataDistributor.allDataDistributorsEverCreated = new Dictionary<Type, Dictionary<StreamingAdapter, List<DataDistributor>>>();
			return DataDistributor.allDataDistributorsEverCreated;
		} }

		void storeAllInstancesEverCreated(StreamingAdapter streamingAdapter) {
			Type adapterType = streamingAdapter.GetType();
			if (DataDistributor.AllDataDistributorsEverCreated.ContainsKey(adapterType) == false) {
				DataDistributor.AllDataDistributorsEverCreated.Add(adapterType, new Dictionary<StreamingAdapter, List<DataDistributor>>());
			}
			if (DataDistributor.AllDataDistributorsEverCreated[adapterType].ContainsKey(streamingAdapter) == false) {
				DataDistributor.AllDataDistributorsEverCreated[adapterType].Add(streamingAdapter, new List<DataDistributor>());
			}
			DataDistributor.AllDataDistributorsEverCreated[adapterType][streamingAdapter].Add(this);
			DataDistributor.AllDataDistributorsEverCreated_total++;
		}
	}
}