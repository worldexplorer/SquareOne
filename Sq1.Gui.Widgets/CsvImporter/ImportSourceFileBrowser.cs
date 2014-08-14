using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using BrightIdeasSoftware;
using ObjectListViewDemo;
using Sq1.Core;

namespace Sq1.Widgets.CsvImporter {
	public partial class ImportSourceFileBrowser : UserControl {
		public event EventHandler<ImportSourcePathInfo> OnFileSelectedTryParse;
		public event EventHandler<DirectoryInfoEventArgs> OnDirectoryChanged;
		
		SysImageListHelper sysImgHelper;
		
		public ImportSourceFileBrowser() {
			InitializeComponent();
			
			sysImgHelper = new SysImageListHelper(this.olvFiles);
			this.olvFilesCustomize();
		}
        void olvFilesCustomize() {
//			this.olvFiles.AllColumns.AddRange(this.olvFiles.Columns);
			List<OLVColumn> allColumns = new List<OLVColumn>();
			foreach (ColumnHeader columnHeader in this.olvFiles.Columns) {
				OLVColumn oLVColumn = columnHeader as OLVColumn; 
				if (oLVColumn == null) continue;
				allColumns.Add(oLVColumn);
			}
			if (allColumns.Count > 0) {
				this.olvFiles.AllColumns.AddRange(allColumns);
			}
//			this.olvColumnFileName.IsVisible = true;
//			this.olvColumnSize.IsVisible = false;
//			this.olvColumnFileModified.IsVisible = false;
			this.olvFiles.RebuildColumns();

			this.olvColumnFileName.AspectGetter = delegate(object x) {
			    var pi = x as ImportSourcePathInfo;
				return pi.Name;
			};
            // Draw the system icon next to the name
            this.olvColumnFileName.ImageGetter += new ImageGetterDelegate(olvColumnFileName_imageGetter);
//            = delegate(object x) {
//				return null;
//                //return helper.GetImageIndex(((FileSystemInfo)x).FullName);
//            };

            // Show the size of files as GB, MB and KBs. Also, group them by some meaningless divisions
			//this.olvColumnSize.AspectGetter = delegate(object x) {
			//    if (x is DirectoryInfo) return (long)-1;
			//    try {
			//        return ((FileInfo)x).Length;
			//    } catch (System.IO.FileNotFoundException) {
			//        return (long)-2;	 // Mono 1.2.6 throws this for hidden files
			//    }
			//};
			//this.olvColumnSize.AspectToStringConverter = delegate(object x) {
			//    if ((long)x == -1) return ""; // folder
			//    return this.FormatFileSize((long)x);
			//};
			//this.olvColumnSize.MakeGroupies(new long[] { 0, 1024 * 1024, 512 * 1024 * 1024 },
			//    new string[] { "Folders", "Small", "Big", "Disk space chewer" });
            // Group by month-year, rather than date
			//this.olvColumnFileModified.GroupKeyGetter = delegate(object x) {
			//    DateTime dt = ((FileSystemInfo)x).LastWriteTime;
			//    return new DateTime(dt.Year, dt.Month, 1);
			//};
			//this.olvColumnFileModified.GroupKeyToTitleConverter = delegate(object x) {
			//    return ((DateTime)x).ToString("MMMM yyyy");
			//};
		}

		object olvColumnFileName_imageGetter(object rowObject) {
			try {
				ImportSourcePathInfo ispi = rowObject as ImportSourcePathInfo; 
				var fsi = ispi.FSI; 
				string full = fsi.FullName;
				int index = sysImgHelper.GetImageIndex(full);
				return index;
			} catch (Exception ex) {
				Assembler.PopupException("olvColumnFileName_imageGetter()", ex);
				return null;
			}
		}
		public void PopulateListFromCsvPath(string pathCsv) {
			if (pathCsv == null) {
				Assembler.PopupException("didn't populate: pathCsv can't be null PopulateListFromCsvPath(pathCsv[null])");
				return;
			}
            DirectoryInfo pathInfo = new DirectoryInfo(pathCsv);
			if (!pathInfo.Exists) return;
			this.txtFolder.Text = pathCsv;
			if (this.OnDirectoryChanged != null) {
				this.OnDirectoryChanged(this, new DirectoryInfoEventArgs(pathInfo));
			}
            Cursor.Current = Cursors.WaitCursor;
			var fsis = pathInfo.GetFileSystemInfos();
			var fsisExtendedFolders = new List<ImportSourcePathInfo>();
			var fsisExtendedFiles = new List<ImportSourcePathInfo>();
			foreach (var fsi in fsis) {
				if (fsi is DirectoryInfo) {
					fsisExtendedFolders.Add(new ImportSourcePathInfo(fsi));
				} else {
					fsisExtendedFiles.Add(new ImportSourcePathInfo(fsi));
				}
			}
			List<ImportSourcePathInfo> foldersThenFiles = new List<ImportSourcePathInfo>(fsisExtendedFolders);
			foldersThenFiles.AddRange(fsisExtendedFiles);
			DirectoryInfo di = Directory.GetParent(pathCsv);
            if (di != null) {
				ImportSourcePathInfo parentFolder = new ImportSourcePathInfo(new DirectoryInfo(Path.Combine(pathCsv, "..")));
				parentFolder.Name = "..";
				foldersThenFiles.Insert(0, parentFolder);
            }
			this.olvFiles.SetObjects(foldersThenFiles);
			this.olvFiles.EnsureVisible(0);
			//this.olvFiles.SelectedIndex = 0;
			this.olvFiles.SelectObject("..", true);	// weird but no focus on ".."
            Cursor.Current = Cursors.Default;
        }
		void txtFolder_TextChanged(object sender, System.EventArgs e) {
            if (Directory.Exists(this.txtFolder.Text)) {
                this.txtFolder.ForeColor = Color.Black;
                this.mniGo.Enabled = true;
                this.mniUp.Enabled = true;
            } else {
                this.txtFolder.ForeColor = Color.Red;
                this.mniGo.Enabled = false;
                this.mniUp.Enabled = false;
            }
		}
		void mniGo_Click(object sender, EventArgs e) {
            try {
            	this.PopulateListFromCsvPath(this.txtFolder.Text);
            } catch (Exception ex) {
                this.txtFolder.ForeColor = Color.Red;
            }
		}
		void mniUp_Click(object sender, System.EventArgs e) {
            DirectoryInfo di = Directory.GetParent(this.txtFolder.Text);
            if (di == null) {
                System.Media.SystemSounds.Asterisk.Play();
            } else {
                this.txtFolder.Text = di.FullName;
                //this.btnGo.PerformClick();
				this.mniGo_Click(this, null);
            }
		}
		void olvFilesItem_Activate(object sender, EventArgs e) {
			Object rowObject = this.olvFiles.SelectedObject;
			if (rowObject == null) return; // clicked on the area with no files but inside olvFiles
			var pi = rowObject as ImportSourcePathInfo;
			if (pi == null) return;
			if (pi.FSI is DirectoryInfo) {
				this.txtFolder.Text = ((DirectoryInfo)pi.FSI).FullName;
				//this.btnGo.PerformClick();
				this.mniGo_Click(this, null);
				return;
			}
			//ShellUtilities.Execute(((FileInfo)pi.FSI).FullName);
			if (OnFileSelectedTryParse == null) return;
			this.OnFileSelectedTryParse(this, pi);
			if (pi.ParsingFailedHightlightRed) {
				//this.olvFiles.SelectedItem.ForeColor = Color.Red;
				this.olvFiles.SelectedItem.ForeColor = Color.White;
				this.olvFiles.SelectedItem.BackColor = Color.PaleVioletRed;
			} else {
				this.olvFiles.SelectedItem.ForeColor = Color.Black;
				this.olvFiles.SelectedItem.BackColor = Color.White;
			}
		}
		void txtFolder_KeyPress(object sender, KeyPressEventArgs e) {
			if (e.KeyChar != (char) Keys.Enter) return;
			//this.btnGo.PerformClick();
			this.mniGo_Click(this, null);
		}
		void olvFiles_SelectedIndexChanged(object sender, EventArgs e) {
			//this.olvFilesItem_Activate(sender, e);
		}
		public void SelectFile(string fileSelected) {
			// still doesn't focus on the deserialized filename
			this.olvFiles.SelectObject(fileSelected, true);
		}
	}
}