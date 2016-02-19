using System;

namespace Sq1.Core.DataFeed {
	public interface IDataSourceEditor {
		void PushEditedSettingsToAdapters_initializeDataSource_updateDataSourceTree_rebacktestCharts();
		void SerializeDataSource_saveAdapters();
	}
}
