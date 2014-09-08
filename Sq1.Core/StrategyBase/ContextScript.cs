using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using Sq1.Core.DataTypes;
using Sq1.Core.Execution;

namespace Sq1.Core.StrategyBase {
	[DataContract]
	public class ContextScript : ContextChart {
		[DataMember] public PositionSize PositionSize;
		[DataMember] public Dictionary<int, double> ParameterValuesById { get; set; }
		[DataMember] public bool IsCurrent;
		[DataMember] public bool ChartAutoSubmitting;

		[DataMember] public List<string> ReporterShortNamesUserInvokedJSONcheck;
		[DataMember] public bool BacktestOnRestart;
		[DataMember] public bool BacktestOnSelectorsChange;
		[DataMember] public bool BacktestOnDataSourceSaved;
		[DataMember] public Dictionary<string, object> ReportersSnapshots;
		
		[DataMember] public bool ApplyCommission;
		[DataMember] public bool EnableSlippage;
		[DataMember] public bool LimitOrderSlippage;
		[DataMember] public bool RoundEquityLots;
		[DataMember] public bool RoundEquityLotsToUpperHundred;
		[DataMember] public bool NoDecimalRoundingForLimitStopPrice;
		[DataMember] public double SlippageUnits;
		[DataMember] public int SlippageTicks;
		[DataMember] public int PriceLevelSizeForBonds;


		public ContextScript(ContextChart upgradingFromSimpleChart = null, string name = "UNDEFINED") : this(name) {
			base.AbsorbFrom(upgradingFromSimpleChart);
		}
		public ContextScript(string name = "UNDEFINED") : this() {
			this.Name = name;
		}
		protected ContextScript() : base() {
			PositionSize = new PositionSize(PositionSizeMode.SharesConstantEachTrade, 1);
			ParameterValuesById = new Dictionary<int, double>();
			IsCurrent = false;
			ChartAutoSubmitting = false;
			BacktestOnRestart = true;
			BacktestOnSelectorsChange = true;
			BacktestOnDataSourceSaved = true;
			ReporterShortNamesUserInvokedJSONcheck = new List<string>();
			ReportersSnapshots = new Dictionary<string, object>();
			
			ApplyCommission = false;
			EnableSlippage = false;
			LimitOrderSlippage = false;
			RoundEquityLots = false;
			RoundEquityLotsToUpperHundred = false;
			SlippageTicks = 1;
			SlippageUnits = 1.0;
		}
		public void AbsorbFrom(ContextScript found, bool absorbParams = false) {
			if (found == null) return;
			//KEEP_CLONE_UNDEFINED this.Name = found.Name;
//			this.Symbol = found.Symbol;
//			this.DataSourceName = found.DataSourceName;
//			this.ScaleInterval = found.ScaleInterval.Clone();
//			this.DataRange = found.DataRange.Clone();
//			this.ChartStreaming = found.ChartStreaming;
//			this.ShowRangeBar = found.ShowRangeBar;
			base.AbsorbFrom(found);
			
			this.PositionSize = found.PositionSize.Clone();
			if (absorbParams) this.ParameterValuesById = new Dictionary<int, double>(found.ParameterValuesById);
			//this.ChartBarSpacing = found.ChartBarSpacing;
			this.ChartAutoSubmitting = found.ChartAutoSubmitting;
			this.BacktestOnRestart = found.BacktestOnRestart;
			this.BacktestOnSelectorsChange = found.BacktestOnSelectorsChange;
			this.BacktestOnDataSourceSaved = found.BacktestOnDataSourceSaved;
			this.ReporterShortNamesUserInvokedJSONcheck = new List<string>(found.ReporterShortNamesUserInvokedJSONcheck);
		}
		public new ContextScript MemberwiseCloneMadePublic() {
			return (ContextScript)base.MemberwiseClone();
		}
		public object FindOrCreateReportersSnapshot(Reporter reporterActivated) {
			string reporterName = reporterActivated.TabText;
			if (this.ReportersSnapshots.ContainsKey(reporterName) == false) {
				this.ReportersSnapshots.Add(reporterName, reporterActivated.CreateSnapshotToStoreInScriptContext());
			}
			return this.ReportersSnapshots[reporterName];
		}
	}
}