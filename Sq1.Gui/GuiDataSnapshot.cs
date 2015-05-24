using System;
using System.Collections.Generic;
using System.Drawing;

using Newtonsoft.Json;
using Sq1.Gui.Forms;

//using Newtonsoft.Json;
//	[DataContract]
//		[JsonIgnore]
//		[DataMember]

namespace Sq1.Gui {
	//[DataContract]
	public class GuiDataSnapshot {
		[JsonIgnore]	public Dictionary<int, ChartFormsManager> ChartFormsManagers;
		[JsonProperty]	public Point MainFormLocation;
		[JsonProperty]	public Size MainFormSize;
		[JsonProperty]	public bool MainFormIsFullScreen;
		[JsonProperty]	public int ChartSernoLastUsed;
		[JsonIgnore]	public int ChartSernoNextAvailable { get { return ++this.ChartSernoLastUsed; } }
		[JsonProperty]	public int ChartSernoLastKnownHadFocus;
		[JsonIgnore]	public string ChartSernosInstantiatedAsString { get {
				string ret = "";
				foreach (int chartSerno in this.ChartFormsManagers.Keys) ret += chartSerno + ",";
				ret = ret.TrimEnd(",".ToCharArray());
				return ret;
			} }

		public GuiDataSnapshot() {
			this.ChartFormsManagers = new Dictionary<int, ChartFormsManager>();
		}

		//public void RebuildDeserializedChartFormsManagers(MainForm mainForm) {
		//	foreach (int serno in this.ChartFormsManagers.Keys) {
		//		if (serno > ChartSernoLastUsed) ChartSernoLastUsed = serno;
		//	}

		//	foreach (int serno in this.ChartFormsManagers.Keys) {
		//		ChartFormsManager mgr = this.ChartFormsManagers[serno];
		//		//mgr.ChartSerno = serno;
		//		mgr.RebuildAfterDeserialization(mainForm);
		//	}
		//}
		public void AddChartFormsManagerJustDeserialized(ChartFormsManager mgr) {
			if (mgr.DataSnapshot.ChartSerno > this.ChartSernoLastUsed) this.ChartSernoLastUsed = mgr.DataSnapshot.ChartSerno;
			this.ChartFormsManagers.Add(mgr.DataSnapshot.ChartSerno, mgr);
		}
		public ChartFormsManager FindChartFormsManagerBySerno(int chartSerno, string invokerMsig = "CALLER_UNKNOWN", bool throwIfNotFound = true) {
			ChartFormsManager ret = null;
			if (this.ChartFormsManagers.ContainsKey(chartSerno) == false) {
				if (throwIfNotFound) {
					string msg = "CANT_DESERIALIZE_REPORTER/EDITOR_PARENT_CHART_NOT_FOUND"
						+ " CHART_DESERIALIZATION_FAILED_OR_CHARTFORM_DOESNT_EXIST_IN_LAYOUT.XML"
						+ " for chartSerno[" + chartSerno + "] ChartSernosInstantiated[" + this.ChartSernosInstantiatedAsString + "]";
					throw new Exception(msg + " " + invokerMsig);
				}
				return ret;
			}
			ret = this.ChartFormsManagers[chartSerno];
			return ret;
		}
	}
}
