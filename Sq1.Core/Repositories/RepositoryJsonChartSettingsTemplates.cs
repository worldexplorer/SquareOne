using System;
using System.Collections.Generic;

using Sq1.Core.Charting;

namespace Sq1.Core.Repositories {
	public class RepositoryJsonChartSettingsTemplates : RepositoryJsonsInFolder<ChartSettings> {

		public void DeserializeJsonsInFolder_IfNoneCreateDefault() {
			base.DeserializeJsonsInFolder();
			if (base.ItemsByName.Count > 0) return;
			ChartSettings Default = new ChartSettings();
			Default.Name = ChartSettings.NAME_DEFAULT;
			base.ItemsByName.Add(Default.Name, Default);
			base.SerializeAll();
		}

		public ChartSettings ChartSettingsFind_nullUnsafe(string name) {
			ChartSettings ret = null;
			string nameUpper = name.ToUpper();
			foreach (ChartSettings current in base.ItemsAsList) {
				if (nameUpper != current.Name.ToUpper()) continue;
				ret = current;
			}
			return ret;
		}
	}
}
