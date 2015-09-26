using System;
using System.Collections.Generic;

using Sq1.Core.DataFeed;

namespace Sq1.Core.Charting {
	public class ChartSettingsBaseList : List<ChartSettingsBase> {
		public bool ContainsName(string anotherName) {
			bool ret = false;
			foreach (ChartSettingsBase each in base.ToArray()) {
				if (each.Name != anotherName) continue;
				ret = true;
				break;
			}
			return ret;
		}
	}
}
