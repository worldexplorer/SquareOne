using System;
using System.Collections.Generic;
using System.Drawing;

using Sq1.Core;
using Sq1.Core.Charting;

using Sq1.Charting;		//separate DLL, yes

using Sq1.Gui.Forms;
using Sq1.Gui.Singletons;

using Newtonsoft.Json;

//using Newtonsoft.Json;
//	[DataContract]
//		[JsonIgnore]
//		[DataMember]

namespace Sq1.Gui {
	//[DataContract]
	public class GuiDataSnapshot {
		[JsonIgnore]	public Dictionary<int, ChartFormManager> ChartFormManagers;
		[JsonIgnore]	public List<ChartControl> ChartControls_AllCurrentlyOpen;
		[JsonProperty]	public Point MainFormLocation;
		[JsonProperty]	public Size MainFormSize;
		[JsonProperty]	public bool MainFormIsFullScreen;
		[JsonProperty]	public int ChartSernoLastUsed;
		[JsonIgnore]	public int ChartSernoNextAvailable { get { return ++this.ChartSernoLastUsed; } }
		[JsonProperty]	public int ChartSernoLastKnownHadFocus;
		[JsonIgnore]	public string ChartSernosInstantiatedAsString { get {
				string ret = "";
				foreach (int chartSerno in this.ChartFormManagers.Keys) ret += chartSerno + ",";
				ret = ret.TrimEnd(",".ToCharArray());
				return ret;
			} }

		public GuiDataSnapshot() {
			this.ChartFormManagers = new Dictionary<int, ChartFormManager>();
			this.ChartControls_AllCurrentlyOpen = new List<ChartControl>();
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
		public void AddChartFormsManager_justDeserialized(ChartFormManager mgr) {
			if (mgr.DataSnapshot.ChartSerno > this.ChartSernoLastUsed) this.ChartSernoLastUsed = mgr.DataSnapshot.ChartSerno;

			this.ChartFormManagers.Add(mgr.DataSnapshot.ChartSerno, mgr);
			// AddChartFormsManagerJustDeserialized() IS_ONLY_INVOKED_FROM_DESERIALIZER_SKIP_REBUILDING_DROPDOWN ChartSettingsEditorForm.Instance.RebuildDropDown_dueToChartFormAddedOrRemoved();

			if (this.ChartControls_AllCurrentlyOpen.Contains(mgr.ChartForm.ChartControl)) {
				string msg = "SAME_SETTINGS_ARE_USED_FOR_MULTIPLE_CHARTS__CHANGING_ONE_IN_ChartSettingsEditorControl_WILL_AFFECT_MANY [" + mgr.ChartForm.ChartControl.ChartSettingsTemplated.Name + "]";
				Assembler.PopupException(msg, null, false);
				return;
			}
			this.ChartControls_AllCurrentlyOpen.Add(mgr.ChartForm.ChartControl);
		}
		public ChartFormManager FindChartFormsManager_bySerno(int chartSerno, string invokerMsig = "CALLER_UNKNOWN", bool throwIfNotFound = true) {
			ChartFormManager ret = null;
			if (this.ChartFormManagers.ContainsKey(chartSerno) == false) {
				if (throwIfNotFound) {
					string msg = "CANT_DESERIALIZE_REPORTER/EDITOR_PARENT_CHART_NOT_FOUND"
						+ " CHART_DESERIALIZATION_FAILED_OR_CHARTFORM_DOESNT_EXIST_IN_LAYOUT.XML"
						+ " for chartSerno[" + chartSerno + "] ChartSernosInstantiated[" + this.ChartSernosInstantiatedAsString + "]";
					throw new Exception(msg + " " + invokerMsig);
				}
				return ret;
			}
			ret = this.ChartFormManagers[chartSerno];
			return ret;
		}
	}
}
