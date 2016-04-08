using System;

using Sq1.Core.Support;

namespace Sq1.Core.DataTypes {
	public class LevelTwoHalf : ConcurrentDictionary<double, double> {
		public LevelTwoHalf(string reasonToExist) : base(reasonToExist) {
		}
	}
}
