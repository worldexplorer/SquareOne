using System;
using System.IO;

namespace Sq1.Widgets.CsvImporter {
	public class DirectoryInfoEventArgs : EventArgs {
		public DirectoryInfo DirectoryInfo;
		public DirectoryInfoEventArgs(DirectoryInfo directoryInfo) {
			this.DirectoryInfo = directoryInfo;
		}
	}
}
