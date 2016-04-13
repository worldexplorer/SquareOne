using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;		// Process.Start(target);
using System.Collections.Generic;

using BrightIdeasSoftware;

using Sq1.Core;
using Sq1.Core.DataFeed;
using Sq1.Core.DataTypes;

using Sq1.Widgets.LabeledTextBox;
using Sq1.Widgets.SteppingSlider;
using Sq1.Widgets.RangeBar;

namespace Sq1.Widgets.CsvImporter {
	public partial class CsvImporterControl {
		void importSourceFileBrowser_OnDirectoryChanged(object sender, DirectoryInfoEventArgs e) {
			if (this.csvDataSnapshot.PathCsv == e.DirectoryInfo.FullName) return;
			this.csvDataSnapshot.PathCsv = e.DirectoryInfo.FullName;
			this.csvDataSnapshot.FileSelected = null;
			this.csvDataSnapshotSerializer.Serialize();
		}
		void importSourceFileBrowser_OnFileSelectedTryParse(object sender, ImportSourcePathInfo e) {
			//if (this.dataSnapshot.FileSelectedAbsname == e.FSI.FullName) return;
			this.csvDataSnapshot.PathCsv = Path.GetDirectoryName(e.FSI.FullName);
			this.csvDataSnapshot.FileSelected = e.FSI.Name;
			this.csvDataSnapshotSerializer.Serialize();
			bool success = this.stepsAllparseFromDataSnapshot();
			e.ParsingFailedHightlightRed = !success;
		}
		void csvPreviewParsedRaw_DoubleClick(object sender, EventArgs e) {
			if (this.csvDataSnapshot.FileSelected == null) {
				this.olvCsvParsedRaw.EmptyListMsg = "Select a file in the left then Double Click here\r\nto open the file selected in Notepad";
				return;
			}
			ObjectListViewDemo.ShellUtilities.Execute(this.csvDataSnapshot.FileSelectedAbsname, "edit");
		}
		void mniEdit_Click(object sender, EventArgs e) {
			this.csvPreviewParsedRaw_DoubleClick(sender, e);
		}
		void olvFieldSetup_CellEditStarting(object sender, CellEditEventArgs e) {
			OLVColumn column = e.Column;
			ColumnCatcher iCatcher = this.olvColGenFieldSetup[column];
			FieldSetup fieldSetup = (FieldSetup) e.RowObject;

			
			ComboBox cb;
			switch (fieldSetup.RowIndexInObjectListView) {
				case 0:
					cb = iCatcher.CreateEditorTypesAvailable();
					cb.DropDownStyle = ComboBoxStyle.DropDownList;
					//cb.Width = 85;
					break;
				case 1:
					cb = iCatcher.CreateEditorFormatsAvailable();
					cb.DropDownStyle = ComboBoxStyle.DropDown;
					//cb.Width = 95;
					break;
				default:
					string msg = "this.olvFieldSetup should contain exactly TWO identical rows; OLV should've passed rowIndex into AspectGetter and CellEdit";
					//throw new Exception(msg);
					Assembler.PopupException(msg);
					return;
			}
			if (cb != null) {
				if (cb.Width - 20 > 90) cb.Width -= 20;
				cb.Font = ((ObjectListView)sender).Font;
				cb.SelectedIndexChanged += new EventHandler(cb_SelectedIndexChanged);
				//cb.Bounds = e.CellBounds;
				cb.Location = new Point(e.CellBounds.Location.X, e.CellBounds.Location.Y + 2);
				//v1 cb.DroppedDown = true;
				//v2 https://connect.microsoft.com/VisualStudio/feedback/details/316175/dropdown-for-combo-box-is-separated-from-its-textbox
				SendKeys.Send("{F4}");	//PF4 == drop down the box
				e.Control = cb; 
			}
		}
		void cb_SelectedIndexChanged(object sender, EventArgs e) {
			ComboBox cb = sender as ComboBox;
			cb.Hide();
		}
		void olvFieldSetup_CellEditFinishing(object sender, CellEditEventArgs e) {
			if (e.Control is ComboBox == false) {
				string msg = "EDITOR_SHOULD_BE_COMBOBOX, instead I got e.Control.GetType()=[" + e.Control.GetType() + "]";
				Assembler.PopupException(msg + " olvFieldSetup_CellEditFinishing()");
				return;
			}

			ComboBox editor = (ComboBox) e.Control;
			FieldSetup fieldSetup = (FieldSetup) e.RowObject;
			ColumnCatcher iCatcherEdited = this.olvColGenFieldSetup[e.Column];
			
			switch (fieldSetup.RowIndexInObjectListView) {
				case 0:
					//CsvFieldType enumValueSelected1 = (CsvFieldType) Enum.Parse(typeof(CsvFieldType), editor.SelectedItem.ToString());
					CsvFieldType selectedType = (CsvFieldType) editor.SelectedIndex;
					iCatcherEdited.Parser.CsvType = selectedType;
					break;
				case 1:
					//CsvFieldType enumValueSelected1 = (CsvFieldType) Enum.Parse(typeof(CsvFieldType), editor.SelectedItem.ToString());
					string selectedFormat = editor.SelectedItem as string;
					if (selectedFormat == null) {
						selectedFormat = editor.Text;	//user typed new value => add it to source list
						this.csvDataSnapshot.AddFormatForTypeUnique(selectedFormat, iCatcherEdited.Parser.CsvType);
					}
					if (selectedFormat == CsvTypeParser.FORMAT_VISUALIZE_EMPTY_STRING) selectedFormat = "";
					iCatcherEdited.Parser.CsvTypeFormat = selectedFormat;
					break;
				default:
					string msg = "this.olvFieldSetup should contain exactly TWO identical rows; OLV should've passed rowIndex into AspectGetter and CellEdit";
					//throw new Exception(msg);
					Assembler.PopupException(msg);
					break;
			}
			editor.SelectedIndexChanged -= cb_SelectedIndexChanged;
			this.csvDataSnapshotSerializer.Serialize();

			this.olvFieldSetup.RefreshObject(iCatcherEdited);
			// Any updating will have been down in the SelectedIndexChanged event handler
			// Here we simply make the list redraw the involved ListViewItem
			//((ObjectListView)sender).RefreshItem(e.ListViewItem);
			// We have updated the model object, so we cancel the auto update
			//e.Cancel = true;
	
			//this.step3syncCsvRawAndFieldSetupToParsedByFormat();
			this.step3safe();
		}
		void btnImport_Click(object sender, EventArgs e) {
			string msig = " //CsvImporterControl.btnImport_Click()";
			Cursor.Current = Cursors.WaitCursor;
			try {
				if (this.targetDataSource == null) {
					throw new Exception("this.dataSourcesTree1.DataSourceSelected=null");
				}
				if (this.targetDataSource != this.dataSourcesTree.DataSourceSelected) {
					throw new Exception("this.targetDataSource[" + this.targetDataSource + "]!=this.dataSourcesTree1.DataSourceSelected[" + this.dataSourcesTree.DataSourceSelected + "]");
				}
				if (this.targetDataSource.BarsRepository == null) {
					throw new Exception("dataSourcesTree1.DataSourceSelected.BarsFolder=null");
				}

				string symbolClickedThenDetected = this.targetSymbolClicked;
				if (string.IsNullOrEmpty(symbolClickedThenDetected)) {
					symbolClickedThenDetected = this.symbolDetectedInCsv;
				}
				if (string.IsNullOrEmpty(symbolClickedThenDetected)) {
					throw new Exception("CANT_IMPORT_SYMBOL_NULL this.ImportTargetSymbol[" + this.targetSymbolClicked
						+ "] or this.ImportSourceSymbolDetected[" + this.symbolDetectedInCsv + "] should not be empty");
				}

				if (this.targetSymbolClicked != this.barsParsed.Symbol) {
					int			barsRead_total = 0;
					List<int>	barsIndexes_failedOHLCVcheck = new List<int>();
					string		msg_barsFailed = "";

					this.barsParsed = new Bars(symbolClickedThenDetected, this.barsParsed.ScaleInterval, this.barsParsed.ReasonToExist);
					foreach (CsvBar csvBar in this.csvParsedByFormat) {
						barsRead_total++;
						try {
							Bar barAdded = this.barsParsed.BarStatic_createAppendAttach(csvBar.DateTime,
								csvBar.Open, csvBar.High, csvBar.Low, csvBar.Close, csvBar.Volume, true);
						} catch (Exception exception_DateOHLCV_NaNs__orZeroes) {
							barsIndexes_failedOHLCVcheck.Add(barsRead_total-1);
						}
					}

					if (barsIndexes_failedOHLCVcheck.Count > 0) {
						string barsIndexes_failedOHLCVcheck_asString = string.Join(",", barsIndexes_failedOHLCVcheck);
						msg_barsFailed = "CSV_PARTIALLY_UNPARSEABLE BARS_NANs_OR_ZEROes"
							+ " indexes[" + barsIndexes_failedOHLCVcheck_asString + "] of barsReadTotal[" + barsRead_total + "]";
						Assembler.PopupException(msg_barsFailed + msig, null, false);
					}
				}
				bool overwrote = true;
				if (this.targetDataSource.Symbols.Contains(this.barsParsed.Symbol) == false) {
					Assembler.InstanceInitialized.RepositoryJsonDataSources.SymbolAdd(this.targetDataSource, this.barsParsed.Symbol);
					overwrote = false;
				}
				string millisElapsed;
				int barsSaved = this.targetDataSource.BarsSave(this.barsParsed, out millisElapsed);
				string msg = overwrote ? "FullyReplaced" : "Imported";
				msg += " [" + barsSaved + "]bars " + millisElapsed;
				msg += " into DataSource[" + this.targetDataSource.Name + "]";
				msg += " to Symbol[" + this.barsParsed.Symbol + "]";
				this.btnImport.Text = msg;
				this.btnImport.Enabled = true;
				this.btnImport.BackColor = SystemColors.Control;
			} catch (Exception ex) {
				Assembler.PopupException(null, ex);
			} finally {
				Cursor.Current = Cursors.Default;
			}
		}

		void mniltbCsvSeparator_UserTyped(object sender, LabeledTextBoxUserTypedArgs e) {
			this.csvDataSnapshot.CsvDelimiter = e.StringUserTyped;
			this.csvDataSnapshotSerializer.Serialize();
			this.stepsAllparseFromDataSnapshot();
		}
		void dataSourcesTree_OnDataSourceSelected(object sender, DataSourceEventArgs e) {
			this.targetDataSource = e.DataSource;
			this.targetSymbolClicked = null;
			this.populateImportButton();
		}
		void dataSourcesTree_OnSymbolSelected(object sender, DataSourceSymbolEventArgs e) {
			this.targetDataSource = e.DataSource;
			this.targetSymbolClicked = e.Symbol;
			this.populateImportButton();
		}
		void lnkDownload_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
			//http://stackoverflow.com/questions/4261234/hyperlink-a-email-address-using-linklabel-in-c-sharp
			//this.lnkDownload.Links[this.lnkDownload.Links.IndexOf(e.Link)].Visited = true;
			this.lnkDownload.LinkVisited = true;
			string target = e.Link.LinkData as string;
			Process.Start(target);
		}
		void rangeBar_OnValueMaxChanged(object sender, RangeArgs<DateTime> e) {
			BarDataRange userDragged_bBarDataRange = new BarDataRange(e.ValueMin, e.ValueMax);
			this.bars_selectRange_flushToGui(userDragged_bBarDataRange);
		}
	}
}