using System;
using System.Collections.Generic;

namespace Sq1.Core.Correlation {
	public static class Stats {
		//http://www.mathsisfun.com/data/standard-deviation.html

		public static double Mean(this List<double> list) {
			double ret = 0;

			int count = list.Count;
			if (count <= 1) return ret;

			foreach (double val in list) ret += val;
			ret /= count;
			return ret;
		}
		public static double StdDev(this List<double> list) {
			double var = list.Variance();
			double ret = Math.Sqrt(var);
			return ret;
		}
		public static double Variance(this List<double> list) {
			double ret = 0;
			int count = list.Count;
			if (count <= 1) return ret;

			double avg = list.Mean();
			
			double sum = 0;
			foreach (double val in list) {
				double diff = val - avg;
				double diffSquare = diff * diff;
				sum += diffSquare;
			}

			ret = sum / count;
			return ret;
		}
	}
}
