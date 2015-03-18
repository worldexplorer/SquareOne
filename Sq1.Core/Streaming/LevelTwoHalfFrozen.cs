using System;
using System.Collections.Generic;


namespace Sq1.Core.Streaming {
	public class LevelTwoHalfFrozen : SortedDictionary<double, double> {
		public class  ASC : IComparer<double> { int IComparer<double>.Compare(double x, double y) { return x > y ? 1 : -1; } }
		public class DESC : IComparer<double> { int IComparer<double>.Compare(double x, double y) { return x < y ? 1 : -1; } }
		
		public string ReasonToExist;

		public double PriceMin	{ get; private set; }
		public double PriceMax	{ get; private set; }
		public double LotMin	{ get; private set; }
		public double LotMax	{ get; private set; }
		public double LotSum	{ get; private set; }

		public Dictionary<double, double> LotsCumulative;

		public LevelTwoHalfFrozen(string reasonToExist, Dictionary<double, double> level2half, IComparer<double> orderby) : base(level2half, orderby) {
			this.ReasonToExist = reasonToExist;
			LotsCumulative = new Dictionary<double, double>();
			if (base.Count == 0) return;
			this.calcProperties();
		}

		void calcProperties() {
			double[] keysCopy = new double[base.Keys.Count];
			//base.Keys.CopyTo(keysCopy, 0);

			//this.PriceMin = keysCopy[0];
			//this.PriceMax = keysCopy[keysCopy.Length-1];

			//v1
			//this.LotMin = base[this.PriceMin];
			//this.LotMax = base[this.PriceMax];
			//v2 VERY_INCONVENIENT_"The given key was not present in the dictionary."
			//this.LotMin = base[0];
			//this.LotMax = base[keysCopy.Length - 1];

			double prevLot = 0;
			//foreach (double price in base.Keys) {
			foreach (KeyValuePair<double, double> keyValue in this) {
				double price = keyValue.Key;
				double lot = keyValue.Value;

				if (this.PriceMin == 0) this.PriceMin = price;
				if (this.PriceMax == 0) this.PriceMax = price;

				if (this.PriceMin > price) this.PriceMin = price;
				if (this.PriceMax < price) this.PriceMax = price;

				//double lot = base[price];
				double thisLot = prevLot + lot;
				this.LotsCumulative.Add(price, thisLot);
				prevLot = thisLot;
				this.LotSum += lot;

				if (this.LotMin == 0) this.LotMin = lot;
				if (this.LotMax == 0) this.LotMax = lot;

				if (this.LotMin > lot) this.LotMin = lot;
				if (this.LotMax < lot) this.LotMax = lot;
			}
		}

		public override string ToString() {
			return this.ReasonToExist + ":[" + base.Count + "]";
		}
	}

	//v1
	//public class LevelTwoHalfSafeCopy : Dictionary<double, double> {
	//	public double PriceMin		{ get; private set; }
	//	public double PriceMax		{ get; private set; }
	//	public double LotMin		{ get; private set; }
	//	public double LotMax		{ get; private set; }
	//	public double ValuesTotal	{ get; private set; }

	//	public LevelTwoHalfSafeCopy (Dictionary<double, double> concurrentSafeCopy) : base(concurrentSafeCopy) {
	//		//foreach (double lot in base.Values) {
	//		foreach (double price in base.Keys) {
	//			if (this.PriceMin == 0) this.PriceMin = price;
	//			if (this.PriceMax == 0) this.PriceMax = price;

	//			if (this.PriceMin > price) this.PriceMin = price;
	//			if (this.PriceMax < price) this.PriceMax = price;

	//			double lot = base[price];
	//			if (this.LotMin == 0) this.LotMin = lot;
	//			if (this.LotMax == 0) this.LotMax = lot;

	//			if (this.LotMin > lot) this.LotMin = lot;
	//			if (this.LotMax < lot) this.LotMax = lot;
	//			this.ValuesTotal += lot;
	//		}
	//	}
	//}
}
