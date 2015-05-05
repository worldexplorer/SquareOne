using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace Sq1.Core.Correlation {
	public class AvgCorMomentums {
		[JsonIgnore]			OneParameterAllAvgCorMomentums	parentMomentums;
		[JsonIgnore]			OneParameterOneValue			parentValue;
		[JsonIgnore]			double							optimizedValue		{ get { return this.parentValue.ValueSequenced; } }

		[JsonIgnore]	public	KPIsAveraged					KPIsAvgAverage		{ get; private set; }
		[JsonIgnore]	public	KPIsAveraged					KPIsAvgDispersion	{ get; private set; }
		[JsonIgnore]	public	KPIsAveraged					KPIsAvgVariance		{ get; private set; }


		AvgCorMomentums() {
			KPIsAvgAverage		= new KPIsAveraged(KPIsAveraged.KPIS_AVG_AVERAGE);
			KPIsAvgDispersion	= new KPIsAveraged(KPIsAveraged.KPIS_AVG_DISPERSION);
			KPIsAvgVariance		= new KPIsAveraged(KPIsAveraged.KPIS_AVG_VARIANCE);
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
