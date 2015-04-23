using System.Collections.Generic;

using Sq1.Core.Sequencing;
using Sq1.Core.DataFeed;

namespace Sq1.Core.Correlation {
	public partial class OneParameterAllAvgCorMomentums {
		public string						ParameterName				{ get; private set; }

		public SortedDictionary<double, AvgCorMomentums>	MomentumsByValue		{ get; private set; }

		OneParameterAllAvgCorMomentums() {
			MomentumsByValue				= new SortedDictionary<double, AvgCorMomentums>();
		}
		public OneParameterAllAvgCorMomentums(string parameterName) : this() {
			this.ParameterName = parameterName;
		}

		public override string ToString() {
			return this.ParameterName + ":" + this.MomentumsByValue.Count + "values";
		}
	}
}
