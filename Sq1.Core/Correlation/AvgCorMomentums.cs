using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace Sq1.Core.Correlation {
	public class AvgCorMomentums {
		[JsonIgnore]			OneParameterAllAvgCorMomentums	parentMomentums;
		[JsonIgnore]			OneParameterOneValue			parentValue;
		[JsonIgnore]			double							optimizedValue		{ get { return this.parentValue.ValueSequenced; } }

		[JsonIgnore]	public	KPIs							KPIsAvgAverage		{ get; private set; }
		[JsonIgnore]	public	KPIs							KPIsAvgDispersion	{ get; private set; }
		[JsonIgnore]	public	KPIs							KPIsAvgVariance		{ get; private set; }


		AvgCorMomentums() {
			KPIsAvgAverage		= new KPIs();
			KPIsAvgDispersion	= new KPIs();
			KPIsAvgVariance		= new KPIs();
		}

		public AvgCorMomentums(OneParameterAllAvgCorMomentums parentMomentums
				, OneParameterOneValue parentValue) : this() {
			this.parentMomentums	= parentMomentums;
			this.parentValue		= parentValue;

			this.parentValue.KPIsMomentumsAverage		= this.KPIsAvgAverage;
			this.parentValue.KPIsMomentumsDispersion	= this.KPIsAvgDispersion;
			this.parentValue.KPIsMomentumsVariance		= this.KPIsAvgVariance;
		}
	}
}
