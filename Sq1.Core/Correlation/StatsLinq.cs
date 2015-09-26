using System;
using System.Collections.Generic;
using System.Linq;

namespace Sq1.Core.Correlation {
	public static class StatsLinq {
		//http://stackoverflow.com/questions/2253874/linq-equivalent-for-standard-deviation
		public static double StdDev(this IEnumerable<double> values) {
			double ret = 0;
			int count = values.Count();
			if (count > 1) {
				//Compute the Average
				double avg = values.Average();

				//Perform the Sum of (value-avg)^2
				double sum = values.Sum(d => (d - avg) * (d - avg));

				//Put it all together
				ret = Math.Sqrt(sum / count);
			}
			return ret;
		}

		//http://www.remondo.net/calculate-mean-median-mode-averages-csharp/
		public static double Mean(this IEnumerable<double> list) {
			return list.Average(); // :-)
		}
		public static double Median(this IEnumerable<double> list) {
			List<double> orderedList = list
				.OrderBy(numbers => numbers)
				.ToList();

			int listSize = orderedList.Count;
			double result;

			if (listSize % 2 == 0) // even
            {
				int midIndex = listSize / 2;
				result = ((orderedList.ElementAt(midIndex - 1) +
						   orderedList.ElementAt(midIndex)) / 2);
			} else // odd
            {
				double element = (double)listSize / 2;
				element = Math.Round(element, MidpointRounding.AwayFromZero);

				result = orderedList.ElementAt((int)(element - 1));
			}

			return result;
		}
		public static IEnumerable<double> Modes(this IEnumerable<double> list) {
			var modesList = list
				.GroupBy(values => values)
				.Select(valueCluster =>
						new {
							Value = valueCluster.Key,
							Occurrence = valueCluster.Count(),
						})
				.ToList();

			int maxOccurrence = modesList
				.Max(g => g.Occurrence);

			return modesList
				.Where(x => x.Occurrence == maxOccurrence && maxOccurrence > 1) // Thanks Rui!
				.Select(x => x.Value);
		}

		//http://www.remondo.net/calculate-the-variance-and-standard-deviation-in-csharp/
		public static double Variance(this IEnumerable<double> list) {
			List<double> numbers = list.ToList();

			double mean = numbers.Mean();
			double result = numbers.Sum(number => Math.Pow(number - mean, 2.0));

			return result / numbers.Count;
		}
		public static double StandardDeviation(this IEnumerable<double> list) {
			return Math.Sqrt(list.Variance());
		}
	}
}
