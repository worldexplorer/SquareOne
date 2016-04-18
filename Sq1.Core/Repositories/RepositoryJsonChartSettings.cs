using System;

using Sq1.Core.Charting;

namespace Sq1.Core.Repositories {
	public class RepositoryJsonChartSettings : RepositoryJsonsInFolder<ChartSettingsTemplated> {

		public void DeserializeJsonsInFolder_IfNoneCreateDefault() {
			base.DeserializeJsonsInFolder_ifNotCached();
			if (base.ItemsCachedByName.Count > 0) return;
			ChartSettingsTemplated Default = new ChartSettingsTemplated();
			Default.Name = ChartSettingsTemplated.NAME_DEFAULT;
			base.ItemsCachedByName.Add(Default.Name, Default);
			base.SerializeAll();
		}

		public ChartSettingsTemplated ChartSettingsFind_nullUnsafe(string name) {
			ChartSettingsTemplated ret = null;
			string nameUpper = name.ToUpper();
			foreach (ChartSettingsTemplated current in base.ItemsCachedAsList) {
				if (nameUpper != current.Name.ToUpper()) continue;
				ret = current;
			}
			return ret;
		}
	}
}
