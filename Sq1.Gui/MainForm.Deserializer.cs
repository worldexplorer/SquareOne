using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Core.StrategyBase;
using Sq1.Gui.Forms;
using Sq1.Gui.Singletons;
using WeifenLuo.WinFormsUI.Docking;

namespace Sq1.Gui {
	public partial class MainForm {
		const string dockContentLayoutXml			= "Sq1.Gui.Layout.xml";
		const string dockContentLayoutXmlInitial	= "Sq1.Gui.Layout.Initial.xml";
		public string LayoutXml			{ get { return Path.Combine(this.GuiDataSnapshotSerializer.AbsPath, dockContentLayoutXml); } }
		public string LayoutXmlInitial	{ get { return Path.Combine(this.GuiDataSnapshotSerializer.AbsPath, dockContentLayoutXmlInitial); } }
	
		IDockContent persistStringInstantiator(string persistedTypeFullName) {
			string msig = " //persistStringInstantiator(" + persistedTypeFullName + ")";
			IDockContent ret = null;
			try {
				if (persistedTypeFullName == typeof(ExceptionsForm).ToString()) {
					ret = ExceptionsForm.Instance;
				} else if (persistedTypeFullName == typeof(DataSourcesForm).ToString()) {
					ret = DataSourcesForm.Instance;
				} else if (persistedTypeFullName == typeof(StrategiesForm).ToString()) {
					ret = StrategiesForm.Instance;
				} else if (persistedTypeFullName == typeof(SlidersForm).ToString()) {
					ret = SlidersForm.Instance;
				//} else if (persistedTypeFullName == typeof(DataSourceEditorForm).ToString()) {
				//	ret = DataSourceEditorForm.Instance;
				} else if (persistedTypeFullName == typeof(ExecutionForm).ToString()) {
					ret = ExecutionForm.Instance;
				} else if (persistedTypeFullName == typeof(CsvImporterForm).ToString()) {
					ret = CsvImporterForm.Instance;
				} else if (persistedTypeFullName == typeof(SymbolInfoEditorForm).ToString()) {
					ret = SymbolInfoEditorForm.Instance;
				} else if (persistedTypeFullName == typeof(ChartSettingsEditorForm).ToString()) {
					ret = ChartSettingsEditorForm.Instance;
				} else {
					// http://www.codeproject.com/Articles/525541/Decoupling-Content-From-Container-in-Weifen-Luos
					return this.handleClassesWithGetPersistStringOverridden(persistedTypeFullName);
					/*
					// DummyDoc overrides GetPersistString to add extra information into persistString.
					// Any DockContent may override this value to add any needed information for deserialization.
				
					string[] parsedStrings = persistString.Split(new char[] { ',' });
					if (parsedStrings.Length != 3) return null;
					if (parsedStrings[0] != typeof(DummyDoc).ToString()) return null;

					DummyDoc dummyDoc = new DummyDoc();
					if (parsedStrings[1] != string.Empty)
						dummyDoc.FileName = parsedStrings[1];
					if (parsedStrings[2] != string.Empty)
						dummyDoc.Text = parsedStrings[2];

					return dummyDoc;*/
				}
			} catch (Exception ex) {
				Assembler.PopupException("FORM_HANDLER_THROWN" + msig, ex);
			}
			if (ret == null) {
				string msg = "returning null will confuse DockPanel; instead, return an instance and initialize it later";
			}
			return ret;
		}
		IDockContent handleClassesWithGetPersistStringOverridden(string persistedSpecialString) {
			string msig = "handleClassesWithGetPersistStringOverridden(" + persistedSpecialString + "): ";
			IDockContent ret = null;
			//CsvImporter doesn't contain COMMAS if (persistedSpecialString.Contains(",") == false) return ret;
			int firstColumnColonIndex = persistedSpecialString.IndexOf(":");
			if (firstColumnColonIndex == -1) return ret;
			Dictionary<string, string> persistedParsedToHash = this.parseAsHash(persistedSpecialString);
			string managedFormCase = persistedSpecialString.Substring(0, firstColumnColonIndex);

			// common for all - this method is intended to link chart and its relatives
			int chartSerno = persistedParsedToHash.ContainsKey("ChartSerno") ? Int32.Parse(persistedParsedToHash["ChartSerno"]) : -1;

			// DockContent.Layout.xml contains definitions of "ReporterWrapped" and "ScriptEditor" next lines AFTER parent ChartFormsManager,
			// so we must've had parent chart deserialized on the previous invocation at {case ("Chart"):}
			// if too unreliable, then switch back to GuiDataSnapshot.ChartFormsManagers + GuiDataSnapshot.RebuildDeserializedChartFormsManagers()
			ChartFormManager parentChart = null;

			switch (managedFormCase) {
				case ("Chart"):
					// string ret = "Chart:" + this.GetType().FullName + ",ChartSerno:" + this.ChartFormsManager.ChartSerno;
					// if (this.ChartFormsManager.Strategy != null) ret += ",StrategyGuid=" + this.ChartFormsManager.Strategy.Guid;
					//if (this.ChartFormsManager.Strategy.ScriptContextCurrent != null) {
					//	ret += ",StrategyScriptContextName:" + this.ChartFormsManager.Strategy.ScriptContextCurrent.Name;
					//}
					if (this.GuiDataSnapshot.ChartFormManagers.ContainsKey(chartSerno)) {
						// who knows why LoadFromXml invokes me twice?
						return ret;
					}
					ChartFormManager chartFormsManagerDeserialized = new ChartFormManager(this, chartSerno);
					//chartFormsManagerDeserialized.Initialize(this, strategy);
					string strategyGuid;
					bool existsGuid = persistedParsedToHash.TryGetValue("StrategyGuid", out strategyGuid);
					if (string.IsNullOrEmpty(strategyGuid)) {
						chartFormsManagerDeserialized.InitializeChartNoStrategyAfterDeserialization();
						this.GuiDataSnapshot.AddChartFormsManagerJustDeserialized(chartFormsManagerDeserialized);
					} else {
						string strategyName;
						bool existsName = persistedParsedToHash.TryGetValue("StrategyName", out strategyName);
						if (existsName == false) {
							#if DEBUG
							Debugger.Break();
							#endif
							strategyName = "STRATEGY_NAME_HAVENT_BEEN_SERIALIZED";
						}
						chartFormsManagerDeserialized.InitializeStrategyAfterDeserialization(strategyGuid, strategyName);
						if (chartFormsManagerDeserialized.StrategyFoundDuringDeserialization == false) {
							chartFormsManagerDeserialized.InitializeChartNoStrategyAfterDeserialization();
						}
						this.GuiDataSnapshot.AddChartFormsManagerJustDeserialized(chartFormsManagerDeserialized);
					}
					chartFormsManagerDeserialized.PopulateWindowTitlesFromChartContextOrStrategy();
					ret = chartFormsManagerDeserialized.ChartForm;
					break;

				case ("ReporterWrapped"):
					// "Reporter:" + this.reporter.GetType().FullName + ",ChartSerno:" + this.reportersFormsManager.ChartFormsManager.ChartSerno;
					parentChart = this.GuiDataSnapshot.FindChartFormsManagerBySerno(chartSerno, msig, true);
					if (parentChart.StrategyFoundDuringDeserialization == false) break;
					string typeFullName = persistedParsedToHash["ReporterWrapped"];
					ret = parentChart.ReportersFormsManager.ReporterActivateShowRegisterMniTick(typeFullName);
					break;

				case ("ScriptEditor"):
					//return "ScriptEditor:" + this.ScriptEditorControl.GetType().FullName + ",ChartSerno:" + this.chartFormsManager.ChartSerno;
					parentChart = this.GuiDataSnapshot.FindChartFormsManagerBySerno(chartSerno, msig, true);
					if (parentChart.StrategyFoundDuringDeserialization == false) break;
					ret = parentChart.ScriptEditorFormConditionalInstance;
					break;

				case ("Sequencer"):
					//return "Sequencer:" + this.ScriptEditorControl.GetType().FullName + ",ChartSerno:" + this.chartFormsManager.ChartSerno;
					parentChart = this.GuiDataSnapshot.FindChartFormsManagerBySerno(chartSerno, msig, true);
					if (parentChart.StrategyFoundDuringDeserialization == false) break;
					ret = parentChart.SequencerFormConditionalInstance;
					break;

				case ("LiveSim"):
					//return "Livesim:" + this.ScriptEditorControl.GetType().FullName + ",ChartSerno:" + this.chartFormsManager.ChartSerno;
					parentChart = this.GuiDataSnapshot.FindChartFormsManagerBySerno(chartSerno, msig, true);
					if (parentChart.StrategyFoundDuringDeserialization == false) break;
					ret = parentChart.LivesimFormConditionalInstance;
					break;

				case ("Correlator"):
					//return "Livesim:" + this.ScriptEditorControl.GetType().FullName + ",ChartSerno:" + this.chartFormsManager.ChartSerno;
					parentChart = this.GuiDataSnapshot.FindChartFormsManagerBySerno(chartSerno, msig, true);
					if (parentChart.StrategyFoundDuringDeserialization == false) break;
					ret = parentChart.CorrelatorFormConditionalInstance;
					break;

				case ("DataSourceEditor"):
					//Assembler.PopupException("DataSourceEditorForm: initializing with datasource[" + persistedParsedToHash["DataSourceEditing"] + "]");
					DataSourceEditorForm instance = DataSourceEditorForm.Instance;
					string dsName = persistedParsedToHash["DataSourceEditing"];
					instance.Initialize(dsName);
					ret = instance; 
					break;

				default:
					string msg2 = "please add switch->case for managedFormCase[" + managedFormCase + "]";
					//throw new Exception(msig + msg2);
					Assembler.PopupException(msg2);
					break;
			}
			return ret;
		}
		public Dictionary<string, string> parseAsHash(string input, string fieldDelimiter = ",", string keyValueSeparator = ":") {
			string msig = " //parseAsHash(input[" + input + "] fieldDelimiter[" + fieldDelimiter + "] keyValueSeparator[" + keyValueSeparator + "])";
			Dictionary<string, string> ret = new Dictionary<string, string>();
			string[] parsedStrings = input.Split(fieldDelimiter.ToCharArray());
			foreach (string keyValue in parsedStrings) {
				// using ":" since "=" leads to an exception in DockPanelPersistor.cs
				string[] parsedKeyValue = keyValue.Split(keyValueSeparator.ToCharArray());
				if (parsedKeyValue.Length < 1) {
					string msg = "SPLITTER_COULDNT_RECOGNIZE_KEY";
					Assembler.PopupException(msg + msig);
					throw new Exception(msg);
				}
				if (parsedKeyValue.Length < 2) {
					string msg = "SPLITTER_COULDNT_RECOGNIZE_VALUE";
					Assembler.PopupException(msg + msig);
					throw new Exception(msg);
				}
				try {
					string key = parsedKeyValue[0];
					string value = parsedKeyValue[1];
					key = key.TrimEnd(keyValueSeparator.ToCharArray());
					ret.Add(key, value);
				} catch (Exception ex) {
					Assembler.PopupException("UNPREDICTED_ERROR " + msig, ex);
					throw ex;
				}
			}
			return ret;
		}
		void initializeMainFromDeserializedDataSnapshot() {
			//Assembler.InstanceInitialized.ExecutionForm = ExecutionForm.Instance;
			
			Screen screenFormIsShownOn = null;
			Rectangle workingAreaCurrentScreen = new Rectangle();		//dummy assignment to avoid "Rectangle is non-nullable type"
			if (this.GuiDataSnapshot.MainFormLocation != null) {
				//v1
				//base.Location = this.DataSnapshot.MainFormLocation;
				//v2 BEGIN relocate when app was closed on secondary display and then it was turned off
				bool locationVisible = false;
				foreach (Screen eachScreen in Screen.AllScreens) {
					if (eachScreen.Bounds.Contains(this.GuiDataSnapshot.MainFormLocation)) {
						base.Location = this.GuiDataSnapshot.MainFormLocation;
						locationVisible = true;
						screenFormIsShownOn = eachScreen;
						workingAreaCurrentScreen = eachScreen.WorkingArea;
						break;
					}
				}
				if (locationVisible == false) {
					workingAreaCurrentScreen = Screen.PrimaryScreen.WorkingArea;
					// sometimes I read "MainFormLocation": "-32000, -32000",
					if (this.GuiDataSnapshot.MainFormLocation.X < 0) this.GuiDataSnapshot.MainFormLocation.X = 0;
					if (this.GuiDataSnapshot.MainFormLocation.Y < 0) this.GuiDataSnapshot.MainFormLocation.Y = 0;
						
					Point locationWentOffScreen = this.GuiDataSnapshot.MainFormLocation;
					if (locationWentOffScreen.X > workingAreaCurrentScreen.Width) {
						locationWentOffScreen.X = workingAreaCurrentScreen.Width - 600;
						if (locationWentOffScreen.X > workingAreaCurrentScreen.X) {
							locationWentOffScreen.X = workingAreaCurrentScreen.X;
						}
					}
					if (locationWentOffScreen.Y > workingAreaCurrentScreen.Height) {
						locationWentOffScreen.Y = workingAreaCurrentScreen.Height - 400;
						if (locationWentOffScreen.Y > workingAreaCurrentScreen.Y) {
							locationWentOffScreen.Y = workingAreaCurrentScreen.Y;
						}
					}
					base.Location = locationWentOffScreen;
				}
				//v2 END
			}
			if (this.GuiDataSnapshot.MainFormSize != null) {
				//v1
				//base.Size = this.DataSnapshot.MainFormSize;
				//v2 BEGIN move to the left (and shrink) when screen became smaller
				Point locationMovedToLeftUp = base.Location;
				Size appWindowSize = this.GuiDataSnapshot.MainFormSize;
				int rightmostPointWindow = base.Location.X + appWindowSize.Width;
				int rightmostPointScreen = workingAreaCurrentScreen.X + workingAreaCurrentScreen.Width;
				if (rightmostPointWindow > rightmostPointScreen) {
					int slideLeftOffset = rightmostPointWindow - rightmostPointScreen;
					int appXabsLocation = base.Location.X - slideLeftOffset;
					if (appXabsLocation < workingAreaCurrentScreen.X) {
						appXabsLocation = workingAreaCurrentScreen.X;
						appWindowSize.Width = workingAreaCurrentScreen.Width;
					}
					locationMovedToLeftUp.X = appXabsLocation;
				}
				int bottommostPointWindow = base.Location.Y + appWindowSize.Height;
				int bottommostPointScreen = workingAreaCurrentScreen.Y + workingAreaCurrentScreen.Height;
				if (bottommostPointWindow > bottommostPointScreen) {
					int slideUpOffset = bottommostPointWindow - bottommostPointScreen;
					int appYabsLocation = base.Location.Y - slideUpOffset;
					if (appYabsLocation < workingAreaCurrentScreen.Y) {
						appYabsLocation = workingAreaCurrentScreen.Y;
						appWindowSize.Width = workingAreaCurrentScreen.Width;
					}
					locationMovedToLeftUp.Y = appYabsLocation;
				}
				base.Location = locationMovedToLeftUp;
				base.Size = appWindowSize;
				if (this.GuiDataSnapshot.MainFormIsFullScreen) {
					this.btnFullScreen_Click(this, null);
				}
				//v2 END
			}
			ChartSettingsEditorForm.Instance.Initialize(this.GuiDataSnapshot.ChartSettingsForChartSettingsEditor);
			if (this.ChartFormActiveNullUnsafe == null) {
				string msg = "I_REFUSE_TO_PopulateWithChartSettings() this.ChartFormActiveNullUnsafe=null //initializeMainFromDeserializedDataSnapshot()";
				Assembler.PopupException(msg);
				return;
			}
			ChartSettingsEditorForm.Instance.PopulateWithChartSettings(this.ChartFormActiveNullUnsafe.ChartControl.ChartSettings);
		}
	}
}
