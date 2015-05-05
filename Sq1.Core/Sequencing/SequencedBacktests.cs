using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using Sq1.Core.DataFeed;
using Sq1.Core.StrategyBase;
using System.Collections.ObjectModel;

namespace Sq1.Core.Sequencing {
	public class SequencedBacktests : NamedObjectJsonSerializable {
		public const string REASON_TO_EXIST = "1) keep only backtested KPIs, IndicatorParameters should not contain CorrelatorSnap; 2) allow subset selection for %backtest/walkforward";
		
		[JsonProperty]	public	string								StrategyName					{ get; private set; }
		// if PriceFormat.IsNullOrEmpty, grab from RepositorySymbolInfo.FindSymbolInfoOrNew(first.Symbol)
		[JsonProperty]	public	string								Symbol							{ get; private set; }
		[JsonProperty]	public	string								SymbolScaleIntervalDataRange 	{ get; private set; }
		
		[JsonProperty]	public	DateTime							BacktestedBarFirstDateTime		{ get; private set; }
		[JsonProperty]	public	DateTime							BacktestedBarLastDateTime		{ get; private set; }
		
		[JsonIgnore]	public	string								FileName;//						{ get; private set; }
		
		[JsonProperty]	private	List<SystemPerformanceRestoreAble>	backtests;//					{ get; private set; }
		
		[JsonIgnore]			List<SystemPerformanceRestoreAble> subset_cached;
		[JsonIgnore]	public	List<SystemPerformanceRestoreAble> Subset { get {
				if (this.SubsetPercentage == 100) return this.backtests;
				if (this.SubsetWaterLineDateTime == DateTime.MaxValue) return this.backtests;
				if (this.subset_cached != null) return this.subset_cached;
				this.subset_cached = this.SubsetPercentageFromEnd ? this.SubsetWalkforward : this.SubsetBacktest;
				return this.subset_cached;
			} }


		
		// main reason for this class to be created is to vary backtest / walkforward percentage in CorrelatorControl and see KPI deltas immediately
		[JsonIgnore]	public	List<SystemPerformanceRestoreAble>	subsetBacktest_cached;
		[JsonIgnore]	public	List<SystemPerformanceRestoreAble>	SubsetBacktest					{ get {
				if (this.subsetBacktest_cached != null) return this.subsetBacktest_cached;
				this.CheckPositionsCountMustIncreaseOnly();
				this.subsetBacktest_cached = new List<SystemPerformanceRestoreAble>();
				DateTime allMustHaveSameDate = DateTime.MinValue;
				foreach (SystemPerformanceRestoreAble eachBacktest in this.backtests) {
					if (eachBacktest.PositionsCount == 0) {		// avoiding NO_POSITIONS_CLOSED_BELOW_WATERLINE; just adding it with PF=RF=NaN
						this.subsetBacktest_cached.Add(eachBacktest);
						continue;
					}
					SystemPerformanceRestoreAble fromBeginningTillWaterline
						= eachBacktest.CreateSubsetBelowWaterline_NullUnsafe(this.SubsetWaterLineDateTime);
					if (fromBeginningTillWaterline == null) {
						string msg = "NO_POSITIONS_CLOSED_BELOW_WATERLINE["
							+ this.SubsetWaterLineDateTime.ToString(Assembler.DateTimeFormatToMinutes)
							+ "] eachBacktest[" + eachBacktest.ToString() + "]";
						Assembler.PopupException(msg, null, false);
						continue;
					}
					if (fromBeginningTillWaterline.KPIsCumulativeByDateIncreasing == null) {
						if (fromBeginningTillWaterline.IsSubset) {
							string msg = "NOT_AN_ERROR__BACKTESTS_IN_SUBSET_WILL_NOT_HAVE_KPIS [" + eachBacktest + "].KPIsCumulativeByDateIncreasing=null";
							//Assembler.PopupException(msg, null, false);
						} else {
							string msg = "HOW_DID_YOU_GET_HERE??";
							Assembler.PopupException(msg);
						}
					}
					if (eachBacktest.PositionsCount > 0) {
						if (fromBeginningTillWaterline.PositionsCount >= eachBacktest.PositionsCount) {
							string msg = "SUBSET_POSITIONS_COUNT__["
								+ fromBeginningTillWaterline.PositionsCount + "]MUST_BE_LESS[" + eachBacktest.PositionsCount
								+ "]__DO_YOU_WANNA_TRY_5% ?? fromBeginningTillWaterline["
								+ fromBeginningTillWaterline.ToString() + "]";
							Assembler.PopupException(msg, null, false);
						} else {
							string msg = "YOU_ARE_GOOD_[" + eachBacktest.PositionsCount + "]>=[" + fromBeginningTillWaterline.PositionsCount + "]";
							//Assembler.PopupException(msg, null, false);
						}
					}
					this.subsetBacktest_cached.Add(fromBeginningTillWaterline);
				}
				return this.subsetBacktest_cached;
			} }

		[JsonIgnore]	public	List<SystemPerformanceRestoreAble>	subsetWalkforward_cached;
		[JsonIgnore]	public	List<SystemPerformanceRestoreAble>	SubsetWalkforward					{ get {
				if (this.subsetWalkforward_cached != null) return this.subsetWalkforward_cached;
				this.subsetWalkforward_cached = new List<SystemPerformanceRestoreAble>();
				foreach (SystemPerformanceRestoreAble eachBacktest in this.backtests) {
					SystemPerformanceRestoreAble fromWaterlineTillEnd
						= eachBacktest.CreateSubsetAboveWaterline_NullUnsafe(this.SubsetWaterLineDateTime);
					if (fromWaterlineTillEnd == null) {
						string msg = "NO_POSITIONS_CLOSED_ABOVE_WATERLINE[" + this.SubsetWaterLineDateTime + "] eachBacktest[" + eachBacktest.ToString() + "]";
						Assembler.PopupException(msg, null, false);
						continue;
					}
					this.subsetWalkforward_cached.Add(fromWaterlineTillEnd);
				}
				return this.subsetWalkforward_cached;
			} }

		[JsonProperty]	public	double								SubsetPercentage				{ get; private set; }
		[JsonProperty]	public	bool								SubsetPercentageFromEnd			{ get; private set; }		//WalkForward=true
		[JsonProperty]	public	DateTime							SubsetWaterLineDateTime			{ get {
				DateTime ret = DateTime.MaxValue;
				//if (this.BacktestedBarFirstDateTime == DateTime.MinValue) {
					if (this.backtests.Count > 0) {
						if (this.backtests[0].KPIsCumulativeByDateIncreasing == null) {
							string msg = "DESERIALIZED_WITH_KPIsCumulativeByDateIncreasing_NULL__RESEQUENCE_ME_AGAIN";
							Assembler.PopupException(msg);
							return ret;
						}
						if (this.backtests[0].KPIsCumulativeByDateIncreasing.Count == 0) {
							string msg = "DESERIALIZED_WITH_KPIsCumulativeByDateIncreasing.Count_ZERO__RESEQUENCE_ME_AGAIN";
							Assembler.PopupException(msg);
							return ret;
						}
						this.BacktestedBarFirstDateTime = this.backtests[0].KPIsCumulativeDateFirst_DateTimeMinUnsafe;
						this.BacktestedBarLastDateTime	= this.backtests[0].KPIsCumulativeDateLast_DateTimeMaxUnsafe;
						if (this.BacktestedBarFirstDateTime == DateTime.MinValue || this.BacktestedBarLastDateTime == DateTime.MaxValue) {
							string msg = "I_TOLD_YOU__DESERIALIZED_WITH_KPIsCumulativeByDateIncreasing_NULL__RESEQUENCE_ME_AGAIN";
							Assembler.PopupException(msg);
							return ret;
						}
					} else {
						string msg = "CANT_FIGURE_BOUNDARIES__EXCEPTIONS_TBF";
						Assembler.PopupException(msg);
					}
				//}
				if (this.SubsetPercentage <= 0) {
					string msg = "fixing deserialization if serialized without this.SubsetPercentage";
					Assembler.PopupException(msg);
					this.SubsetPercentage = 100;
				}
				try {
					TimeSpan distance = this.BacktestedBarLastDateTime.Subtract(this.BacktestedBarFirstDateTime);
					long ticksPercented = (long)((double)distance.Ticks * this.SubsetPercentage / 100);
					TimeSpan distanceSubset = new TimeSpan(ticksPercented);
					ret = this.BacktestedBarFirstDateTime.Add(distanceSubset);
					#if DEBUG
					TimeSpan mustBePositive = this.BacktestedBarLastDateTime.Subtract(ret);
					if (mustBePositive.Ticks < 0) {
						string msg = "PART_OF_RANGE_CAN_NOT_BE_BEYOND_THE_RANGE";
						Assembler.PopupException(msg);
					}
					#endif
				} catch (Exception ex) {
					string msg = "INPUTS_FOR_SubsetWaterLine_NOT_INITIALIZED";
					Assembler.PopupException(msg, ex);
				}
				return ret;
			} }
		
		public SequencedBacktests() {
			string msig = "THIS_CTOR_IS_INVOKED_BY_JSON_DESERIALIZER__KEEP_ME_PUBLIC__CREATE_[JsonIgnore]d_VARIABLES_HERE";
			backtests	= new List<SystemPerformanceRestoreAble>();
			FileName 	= "SHRINKED_OPTIMIZED_NO_FNAME";
			SubsetPercentage		= 100;
			SubsetPercentageFromEnd = false;
		}
		public SequencedBacktests(string fileName, List<SystemPerformanceRestoreAble> backtests) : this() {
			if (backtests == null) {
				string msg = "I_REFUSE_TO_RECEIVE_BACKTESTS_NULL__AVOIDING_NPE";
				Assembler.PopupException(msg);
			}
			FileName 	= fileName;
			backtests	= backtests;
			//subsetWalkforward_cached = null;
			//subsetBacktest_cached = null;
			BacktestedBarFirstDateTime = DateTime.MinValue;
			BacktestedBarLastDateTime = DateTime.MaxValue;
			if (this.backtests.Count > 0) {
				this.BacktestedBarFirstDateTime = backtests[0].KPIsCumulativeDateFirst_DateTimeMinUnsafe;
				this.BacktestedBarLastDateTime = backtests[0].KPIsCumulativeDateLast_DateTimeMaxUnsafe;
			} else {
				string msg = "CANT_FIGURE_BOUNDARIES__EXCEPTIONS_TBF";
				Assembler.PopupException(msg);
			}
		}
		public SequencedBacktests(ScriptExecutor executor, string fileName) : this(fileName, new List<SystemPerformanceRestoreAble>()) {
			this.StrategyName					= executor.StrategyName;
			//this.ContextScript				= executor.Strategy.ScriptContextCurrent;
			this.StrategyName					= executor.Strategy.ScriptContextCurrent.Symbol;
			this.SymbolScaleIntervalDataRange	= executor.Strategy.ScriptContextCurrent.ToStringSymbolScaleIntervalDataRangeForScriptContextNewName();
			base.Name							= this.SymbolScaleIntervalDataRange;
		}
		//
		//public void ChangeSubset(double subsetPercentage, bool subsetPercentageFromEnd) {
		//    this.SubsetPercentage			= subsetPercentage;
		//    this.SubsetPercentageFromEnd	= subsetPercentageFromEnd;
		//    this.subsetBacktest_cached		= null;
		//}
		internal void SubsetPercentageFromEndSetInvalidate(bool subsetPercentageFromEnd) {
			this.SubsetPercentageFromEnd = subsetPercentageFromEnd;
			this.subsetBacktest_cached = null;
			this.subsetWalkforward_cached = null;
			this.subset_cached = null;
		}
		internal void SubsetPercentageSetInvalidate(double subsetPercentage) {
			if (subsetPercentage <= 0) {
				string msg = "I_REFUSE_TO_DIVIDE_BY_ZERO_OR_NEGATIVE__SET_YOU_SLIDER_MIN_VALUE_TO_1%";
				Assembler.PopupException(msg);
			}
			this.SubsetPercentage = subsetPercentage;
			this.subsetBacktest_cached = null;
			this.subsetWalkforward_cached = null;
			this.subset_cached = null;
		}

		public void CheckPositionsCountMustIncreaseOnly() {
			#if DEBUG
			foreach (SystemPerformanceRestoreAble backtest in this.backtests) {
				if (backtest.KPIsCumulativeByDateIncreasing == null) {
					string msg = "BACKTESTS_IN_DESERIALIZEDMUST_HAVE_KPIS [" + backtest + "].KPIsCumulativeByDateIncreasing=null";
					Assembler.PopupException(msg, null, false);
					return;
				}

				DateTime dateCantDecrease = DateTime.MinValue;
				double positionsCountCantDecrease = 0;
				foreach (DateTime datePositionClosed in backtest.KPIsCumulativeByDateIncreasing.Keys) {
					if (dateCantDecrease >= datePositionClosed) {
						string msg = "DATE_POSITION_CLOSED_CANT_DECREASE [" + dateCantDecrease + "] >= [" + datePositionClosed + "]";
						Assembler.PopupException(msg);
					}
					dateCantDecrease = datePositionClosed;

					KPIs KPIs = backtest.KPIsCumulativeByDateIncreasing[datePositionClosed];
					if (positionsCountCantDecrease >= KPIs.PositionsCount && positionsCountCantDecrease > 0) {
						string msg = "POSITIONS_COUNT_CANT_DECREASE [" + positionsCountCantDecrease + "] >= [" + KPIs.PositionsCount + "]";
						Assembler.PopupException(msg, null, false);
						//return;
					}
					positionsCountCantDecrease = KPIs.PositionsCount;
				}
			}
			#endif
		}
		public override string ToString() {
			return this.FileName + ":" + this.backtests.Count + "backtests;" + this.SubsetPercentage + "%";
		}


		[JsonIgnore]	public	ReadOnlyCollection<SystemPerformanceRestoreAble>	BacktestsReadonly				{ get { return this.backtests.AsReadOnly(); } }
		[JsonIgnore]	public	int		Count		{ get { return this.backtests.Count; } }

		public void Clear() {
			this.SubsetPercentage = 100;
			this.SubsetPercentageFromEnd = false;
			this.backtests.Clear();
		}

		public void Add(SystemPerformanceRestoreAble eachBacktest) {
			try {
				eachBacktest.CheckPositionsCountMustIncreaseOnly();
				this.backtests.Add(eachBacktest);
			} catch (Exception ex) {
				string msg = "MULTITHREADING_ISSUE__YOU_MUST_PASS_CLONE_AND_THEN_LET_OTHER_DISPOSABLE_EXECUTOR_TO_RUN_ANOTHER_BACKTEST";
				Assembler.PopupException(msg, ex);
			}
		}

		internal void Serialize() {
			throw new NotImplementedException();
		}
	}
}
