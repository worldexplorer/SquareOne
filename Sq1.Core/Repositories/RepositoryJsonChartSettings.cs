using System;

using Sq1.Core.Charting;

namespace Sq1.Core.Repositories {
	public class RepositoryJsonChartSettings : RepositoryJsonsInFolder<ChartSettings> {

		public void DeserializeJsonsInFolder_IfNoneCreateDefault() {
			base.DeserializeJsonsInFolder_ifNotCached();
			if (base.ItemsCachedByName.Count > 0) return;
			ChartSettings Default = new ChartSettings();
			Default.Name = ChartSettings.NAME_DEFAULT;
			base.ItemsCachedByName.Add(Default.Name, Default);
			base.SerializeAll();
		}

		public ChartSettings ChartSettingsFind_nullUnsafe(string name) {
			ChartSettings ret = null;
			string nameUpper = name.ToUpper();
			foreach (ChartSettings current in base.ItemsCachedAsList) {
				if (nameUpper != current.Name.ToUpper()) continue;
				ret = current;
			}
			return ret;
		}
	}
}
