using System;
using System.Collections.Generic;
using System.Diagnostics;

using Sq1.Core.DataFeed;
using Sq1.Core.DataTypes;

namespace Sq1.Core.StrategyBase {
	public class NotOnChartBarsHelper {
		public ScriptExecutor ScriptExecutor;
		public Dictionary<NotOnChartBarsKey, Bars> Registry;
		
		public NotOnChartBarsHelper(ScriptExecutor scriptExecutor) {
			this.ScriptExecutor = scriptExecutor;
			this.Registry = new Dictionary<NotOnChartBarsKey, Bars>();
		}
		public Bars FindNonChartBarsSubscribeRegisterForIndicator(NotOnChartBarsKey symbolScaleInterval) {
			return this.RescaleBarsAndRegister(symbolScaleInterval);
		}
		public void RescaledBarsUnregisterFor(NotOnChartBarsKey symbolScaleInterval) {
			this.Registry.Remove(symbolScaleInterval);
		}
		public Bars RescaledBarsGetRegisteredFor(NotOnChartBarsKey symbolScaleInterval) {
			if (this.Registry.ContainsKey(symbolScaleInterval) == false) return null;
			return this.Registry[symbolScaleInterval];
		}
		public Bars RescaleBarsAndRegister(NotOnChartBarsKey symbolScaleInterval) {
			DataSource ds = Assembler.InstanceInitialized.RepositoryJsonDataSource.DataSourceFindNullUnsafe(symbolScaleInterval.DataSourceName);
			BarDataRange range = new BarDataRange(this.ScriptExecutor.Bars.BarFirst.DateTimeOpen, this.ScriptExecutor.Bars.BarLast.DateTimeOpen);
			Bars bars = null;
			try {
				Bars barsAll = ds.BarsLoadAndCompress(symbolScaleInterval.Symbol, symbolScaleInterval.BarScaleInterval);
				Bars bar = barsAll.SelectRange(range);
			} catch (Exception ex) {
				Debugger.Break();
			}
			try {
				this.Registry.Add(symbolScaleInterval, bars);
			} catch (Exception ex) {
				Debugger.Break();
			}
			return bars;
		}
	}
}
