using System;
using System.Collections.Generic;

using Sq1.Core.Support;

namespace Sq1.Core.Streaming {
	public class LevelTwoHalf : ConcurrentDictionaryGeneric<double, double> {
		public LevelTwoHalf(string reasonToExist) : base(reasonToExist) {
		}
	}
}
