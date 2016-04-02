using System;
using System.Collections.Generic;

using Sq1.Core.DataTypes;
using Sq1.Core.Charting;
using Sq1.Core.Backtesting;
using Sq1.Core.Livesim;

namespace Sq1.Core.Streaming {
	public partial class Distributor {
				static	int																		AllDistributorsEverCreated_total;
				static	Dictionary<Type, Dictionary<StreamingAdapter, List<Distributor>>>	allDistributorsEverCreated;
		public	static	Dictionary<Type, Dictionary<StreamingAdapter, List<Distributor>>>	AllDistributorsEverCreated 	{ get {
			if (Distributor.allDistributorsEverCreated == null) Distributor.allDistributorsEverCreated = new Dictionary<Type, Dictionary<StreamingAdapter, List<Distributor>>>();
			return Distributor.allDistributorsEverCreated;
		} }

		void storeAllInstancesEverCreated(StreamingAdapter streamingAdapter) {
			Type adapterType = streamingAdapter.GetType();
			if (Distributor.AllDistributorsEverCreated.ContainsKey(adapterType) == false) {
				Distributor.AllDistributorsEverCreated.Add(adapterType, new Dictionary<StreamingAdapter, List<Distributor>>());
			}
			if (Distributor.AllDistributorsEverCreated[adapterType].ContainsKey(streamingAdapter) == false) {
				Distributor.AllDistributorsEverCreated[adapterType].Add(streamingAdapter, new List<Distributor>());
			}
			Distributor.AllDistributorsEverCreated[adapterType][streamingAdapter].Add(this);
			Distributor.AllDistributorsEverCreated_total++;
		}
	}
}