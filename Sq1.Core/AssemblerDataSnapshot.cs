using System;
using System.Collections.Generic;

using Newtonsoft.Json;
//	[DataContract]
//		[JsonIgnore]
//		[DataMember]
//		[IgnoreDataMember]

namespace Sq1.Core {
	public class AssemblerDataSnapshot {
		[JsonProperty]	public string		WorkspaceCurrentlyLoaded;
		[JsonProperty]	public int			SplitterEventsShouldBeIgnoredSecondsAfterAppLaunch;

		[JsonProperty]	public List<string>	ReferencedNetAssemblyNames_forCompilingScripts_System	{ get; private set; }
		[JsonProperty]	public List<string> ReferencedNetAssemblyNames_forCompilingScripts_Sq1		{ get; private set; }
		[JsonIgnore]	public List<string> ReferencedNetAssemblyNames_forCompilingScripts			{ get {
			List<string> ret = new List<string>();
			ret.AddRange(this.ReferencedNetAssemblyNames_forCompilingScripts_System);
			ret.AddRange(this.ReferencedNetAssemblyNames_forCompilingScripts_Sq1);
			return ret;
		} }

		[JsonProperty]	public List<string> ReferencedNetAssemblyNames_StreamingBrokerAdapters		{ get; private set; }
		[JsonProperty]	public List<string> ReferencedNetAssemblyNames_Reporters					{ get; private set; }
		[JsonProperty]	public List<string> ReferencedNetAssemblyNames_Indicators					{ get; private set; }
		[JsonProperty]	public List<string> ReferencedNetAssemblyNames_Scripts						{ get; private set; }
		
		public AssemblerDataSnapshot() {
			WorkspaceCurrentlyLoaded = "Default";
			SplitterEventsShouldBeIgnoredSecondsAfterAppLaunch = 10;	// = 0 on 3.1GHz restores exactly as it was saved; ZERO_HELPS_BUT_MEANS_NO_DELAY: for Charts but not for Exceptions and Execution 

			ReferencedNetAssemblyNames_forCompilingScripts_System = new List<string>() {
				"System.dll", "System.Windows.Forms.dll", "System.Drawing.dll"
			};

			ReferencedNetAssemblyNames_forCompilingScripts_Sq1 = new List<string>() {
				"CsvHelper35.dll", "DigitalRune.Windows.TextEditor.dll", "Newtonsoft.Json.dll", "ObjectListView.dll", "WeifenLuo.WinFormsUI.Docking.dll",
				"Sq1.Charting.dll", "Sq1.Core.dll", "Sq1.Indicators.dll", "Sq1.Reporters.dll", "Sq1.Charting.dll", "Sq1.Gui.exe", "Sq1.Widgets.dll",
				"Sq1.Adapters.Quik.dll", "NDde.dll"
			};

			ReferencedNetAssemblyNames_StreamingBrokerAdapters = new List<string>() {
				"Sq1.Core.dll", "Sq1.Adapters.Quik.dll"
			};

			ReferencedNetAssemblyNames_Reporters = new List<string>() {
				"Sq1.Reporters.dll"
			};

			ReferencedNetAssemblyNames_Indicators = new List<string>() {
				"Sq1.Core.dll", "Sq1.Indicators.dll"
			};

			ReferencedNetAssemblyNames_Scripts = new List<string>() {
				"Sq1.Strategies.*.dll"
			};

		}
	}
}
