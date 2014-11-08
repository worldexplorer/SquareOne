using System;
using System.IO;

using Sq1.Core;

namespace Sq1.Widgets.CsvImporter {
	public class ImportSourcePathInfo : EventArgs {
		public FileSystemInfo FSI;

//		public bool checkedForCsvParsing;
//		public bool CheckedForCsvParsing {
//			get {
//				if (this.FSI is DirectoryInfo) throw new Exception();
//				return checkedForCsvParsing;
//			}
//			set {
//				checkedForCsvParsing = value;
//			}
//		}
		public string Name;
		public string FileSize { get {
			if (this.FSI is DirectoryInfo) return null;
			try {
				return this.FormatFileSize(((FileInfo)FSI).Length);
			} catch (System.IO.FileNotFoundException) {
				return null;	 // Mono 1.2.6 throws this for hidden files
			}
		} }
		public string LastWriteTime { get {
			string format = Assembler.DateTimeFormatLongFilename.Replace('_', ' ');
			return this.FSI.LastWriteTime.ToString(format);
		} }
		public bool ParsingFailedHightlightRed;

		public ImportSourcePathInfo(FileSystemInfo fsi) {
			this.FSI = fsi;
			this.Name = this.FSI.Name;
		}
		string FormatFileSize(long size) {
			int[] limits = new int[] { 1024 * 1024 * 1024, 1024 * 1024, 1024 };
			string[] units = new string[] { "GB", "MB", "KB" };
			for (int i = 0; i < limits.Length; i++) {
				if (size >= limits[i]) return String.Format("{0:#,##0.##} " + units[i], ((double)size / limits[i]));
			}
			return String.Format("{0} bytes", size);
		}
	}
}