using System;
using System.Collections.Generic;
using System.IO;

namespace Sq1.Core.Repositories {
	public class DllScannerExplicit<T> : DllScanner<T> {
		List<string> dllNames_explicitlyNeeded;

		public DllScannerExplicit(string rootPath, List<string> dllNamesOnly_passed) : base(rootPath) {
			if (dllNamesOnly_passed == null) {
				string msg = "DONT_PASS_NULL__dllNamesOnly_passed";
				throw new Exception(msg);
			}
			this.dllNames_explicitlyNeeded = dllNamesOnly_passed;
			this.ScanDlls();
		}
		public override int ScanDlls() {
			return this.scanDlls_allExplicitlyNeeded_minusNonExisitngInFolder();
		}
		int scanDlls_allExplicitlyNeeded_minusNonExisitngInFolder() {
			int instancesFound = 0;
			if (Directory.Exists(this.AbsPath) == false) return instancesFound;
			FileInfo[] dllsFiltered = base.DllsExplicitlyNeeded_minusNonExistingInFolder(this.dllNames_explicitlyNeeded);
			return base.ScanListPrepared(dllsFiltered);
		}
	}
}
