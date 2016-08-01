using System;

using Newtonsoft.Json;

namespace Sq1.Core.Execution {
	public partial class Alert {
		[JsonIgnore]	public string ExecutionControl_AlertsPendingClear_knowHow { get {
			string ret = "Clear PendingAlerts ";

			string executorDataSnapshot_inaccessibleReason = this.ExecutionControl_alertIsDeserialized__ExecSnapSafe_ifNull;
			if (string.IsNullOrEmpty(executorDataSnapshot_inaccessibleReason) == false) {
				ret += "IMPOSSIBLE[" + executorDataSnapshot_inaccessibleReason + "]";
				return ret;
			}

			ExecutorDataSnapshot snap = this.Strategy.Script.Executor.ExecutionDataSnapshot;
			int alertClickedsPendingFound = snap.AlertsUnfilled.Count;
			bool strategy_hasPendingAlerts = alertClickedsPendingFound > 0;
			ret += "[" + alertClickedsPendingFound + "] from ExecutorDataSnapshot";

			return ret;
		} }

		[JsonIgnore]	public string ExecutionControl_PositionClose_knowHow { get {
			string ret = "";

			Position positionClicked = this.PositionAffected;
			if (positionClicked == null) {
				ret = "Close Position: ";
				ret += "IMPOSSIBLE[PositionAffected_MUST_NOT_BE_NULL]";
				if (this.IsEntryAlert) {
					ret += " for_EntryAlert";
				}
				if (this.IsExitAlert) {
					ret += " for_ExitAlert";
				}
				return ret;
			}

			ret = positionClicked.ExecutionControl_PositionClose_knowHow;

			if (this.IsEntryAlert && positionClicked.IsEntryFilled) {
				string msg = " EntryAlert[createdQ#" + this.QuoteCreatedThisAlert.IntraBarSerno + "]"
						//+ ".FilledBarIndex[" + this.FilledBarIndex + "]"
						;
				ret += msg;
			}

			string executorDataSnapshot_inaccessibleReason = this.ExecutionControl_alertIsDeserialized__ExecSnapSafe_ifNull;
			if (string.IsNullOrEmpty(executorDataSnapshot_inaccessibleReason)) {
				ExecutorDataSnapshot snap = this.Strategy.Script.Executor.ExecutionDataSnapshot;
				int	positions_OpenNow = snap.Positions_OpenNow.Count;
				ret += " #posOpeNow=" + positions_OpenNow;
			}

			return ret;
		} }

		[JsonIgnore]	public	string	ExecutionControl_alertIsDeserialized__ExecSnapSafe_ifNull { get {
			string ret = null;

			bool deserializedAlert_isNotClose_able =
				this.BrokerName != null &&
				this.BrokerName.Contains("BARS_NULL");
			
			if (deserializedAlert_isNotClose_able) {
				ret = "NO_POSITION__FOR_DESERIALIZED_ORDER";
				return ret;
			}

			if (	this.Strategy										== null ||
					this.Strategy.Script								== null ||
					this.Strategy.Script								== null ||
					this.Strategy.Script.Executor						== null ||
					this.Strategy.Script.Executor.ExecutionDataSnapshot	== null ) {
				ret = "EXECUTOR_MUST_NOT_BE_NULL";
				return ret;
			}
			return ret;
		} }
	}
}
