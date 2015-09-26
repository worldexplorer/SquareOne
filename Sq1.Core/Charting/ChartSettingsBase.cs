using System;

using Sq1.Core.DataFeed;

namespace Sq1.Core.Charting {
	public class ChartSettingsBase : NamedObjectJsonSerializable {
		public virtual ChartSettingsBase Clone() {
			return (ChartSettingsBase)base.MemberwiseClone();
		}
	}
}
