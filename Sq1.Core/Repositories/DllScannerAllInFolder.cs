using System;
using System.Collections.Generic;
using System.IO;

namespace Sq1.Core.Repositories {
	public class DllScannerAllInFolder<T> : DllScanner<T> {
		List<string>						dllNames_notNeeded = new List<string>() {
				"CsvHelper35.dll", "DigitalRune.Windows.TextEditor.dll", "NDde.dll", "Newtonsoft.Json.dll",		//"log4net.dll", 
				"ObjectListView.dll", "WeifenLuo.WinFormsUI.Docking.dll", "Sq1.Charting.dll", "Sq1.Gui.dll", "Sq1.Widgets.dll"
				, "TRANS2QUIK.dll"
				// I_WANT_LIVESIM_STREAMING_BROKER_BE_AUTOASSIGNED_AND_VISIBLE_IN_DATASOURCE_EDITOR, "Sq1.Core.dll"
				};

		public DllScannerAllInFolder(string rootPath, List<string> dllNames_notNeeded_passed = null) : base(rootPath) {
			if (dllNames_notNeeded_passed != null) this.dllNames_notNeeded = dllNames_notNeeded_passed;
			this.ScanDlls();
		}
		public override int ScanDlls() {
			return this.scanDlls_allFoundInFolder_minusSkipped();
		}
		int scanDlls_allFoundInFolder_minusSkipped() {
			int instancesFound = 0;
			if (Directory.Exists(this.AbsPath) == false) return instancesFound;
			FileInfo[] dllsFiltered = base.DllsFoundInFolder_minusNotNeeded(this.dllNames_notNeeded);
			return base.ScanListPrepared(dllsFiltered);
		}
	}
}
