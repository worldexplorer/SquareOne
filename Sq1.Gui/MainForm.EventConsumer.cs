using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;

using WeifenLuo.WinFormsUI.Docking;

using Sq1.Core;
using Sq1.Core.Streaming;
using Sq1.Core.Livesim;

using Sq1.Widgets;

using Sq1.Gui.Forms;
using Sq1.Gui.Singletons;
using Sq1.Gui.ReportersSupport;

namespace Sq1.Gui {
	public partial class MainForm {
		void mainForm_Load(object sender, System.EventArgs e) {
			try {
				// separate try {} for MONO: Reflection bla-bla-bla exception
				this.MainFormEventManager_initialize_whenDockingIsNotNullAnymore();
			} catch (Exception ex) {
				Assembler.PopupException("MainFormEventManagerInitializeWhenDockingIsNotNullAnymore()", ex);
			}
			try {
				this.WorkspacesManager.RescanRebuildWorkspacesMenu();
				this.WorkspaceLoad();
			} catch (Exception ex) {
				Assembler.PopupException("mainForm_Load()", ex);
			}
		}
		void mainForm_ResizeEnd(object sender, EventArgs e) {
			if (this.GuiDataSnapshot == null) return;
			this.GuiDataSnapshot.MainFormSize = base.Size;
		}
		void mainForm_LocationChanged(object sender, EventArgs e) {
			if (this.GuiDataSnapshot == null) {
				string msg = "Forms.Control.Visible.set() invokes LocationChaned, we'll come back after this.DataSnapshot gets created";
				return;
			}
			if (base.Location.X < 0) return;
			if (base.Location.Y < 0) return;
			this.GuiDataSnapshot.MainFormLocation = base.Location;
		}

		protected override void WndProc(ref Message m) {
			//http://stackoverflow.com/questions/3155782/what-is-the-difference-between-wm-quit-wm-close-and-wm-destroy-in-a-windows-pr
//			public enum Msgs {
//				...WM_CLOSE				  = 0x0010,...
//			}
			if (m.Msg == (int)WeifenLuo.WinFormsUI.Docking.Win32.Msgs.WM_CLOSE) {
				Assembler.InstanceInitialized.MainFormClosingIgnoreReLayoutDockedForms = true;
			}
			base.WndProc(ref m);
		}
		void mainForm_FormClosing(object sender, FormClosingEventArgs e) {
			string msig = " //mainForm_FormClosing()";
			this.MainFormClosing_skipChartFormsRemoval_serializeExceptionsToPopupInNotepad = true;

			// serialize XML first, because all Charts will seem to be physically .Close()d?... (XML after shutdown is shrunk?)
			this.MainFormSerialize();

			foreach (ChartFormManager eachChartManager in this.GuiDataSnapshot.ChartFormManagers.Values) {
				bool exitingLivesimThreadWillTakeTime_cancelAppClose = eachChartManager.Shutdown_mainFormClosing();
				e.Cancel = exitingLivesimThreadWillTakeTime_cancelAppClose;
			}
		}
		void mainForm_FormClosed(object sender, FormClosedEventArgs e) {
			Assembler.ExceptionsDuringApplicationShutdown_PopupNotepad();
		}

		#region ctxTools
		void mniExceptions_Click(object sender, EventArgs e) {
			try {
				if (this.mniExceptions.Checked == false) {
					ExceptionsForm.Instance.Show(this.DockPanel);
				} else {
					ExceptionsForm.Instance.Hide();
				}
				this.MainFormSerialize();
				this.ctxTools.Show();
			} catch (Exception ex) {
				Assembler.PopupException("mniExceptions_Click()", ex);
			}
		}
		void mniSymbols_Click(object sender, EventArgs e) {
			try {
				if (this.mniDataSources.Checked == false) {
					DataSourcesForm.Instance.Show(this.DockPanel);
				} else {
					DataSourcesForm.Instance.Hide();
				}
				this.MainFormSerialize();
				this.ctxTools.Show();
			} catch (Exception ex) {
				Assembler.PopupException("mniSymbols_Click()", ex);
			}
		}
		void mniSliders_Click(object sender, EventArgs e) {
			try {
				if (this.mniSliders.Checked == false) {
					SlidersForm.Instance.Show(this.DockPanel);
				} else {
					SlidersForm.Instance.Hide();
				}
				this.MainFormSerialize();
				this.ctxTools.Show();
			} catch (Exception ex) {
				Assembler.PopupException("mniSliders_Click()", ex);
			}
		}
		void mniStrategies_Click(object sender, EventArgs e) {
			try {
				if (this.mniStrategies.Checked == false) {
					StrategiesForm.Instance.Show(this.DockPanel);
				} else {
					StrategiesForm.Instance.Hide();
				}
				this.MainFormSerialize();
				this.ctxTools.Show();
			} catch (Exception ex) {
				Assembler.PopupException("mniStrategies_Click()", ex);
			}
		}
		void mniExecution_Click(object sender, EventArgs e) {
			try {
				if (this.mniExecution.Checked == false) {
					ExecutionForm.Instance.Show(this.DockPanel);
					ExecutionForm.Instance.ExecutionTreeControl.PopulateDataSnapshot_initializeSplitters_ifDockContentDeserialized();
				} else {
					ExecutionForm.Instance.Hide();
				}
				this.MainFormSerialize();
				this.ctxTools.Show();
			} catch (Exception ex) {
				Assembler.PopupException("mniExecution_Click()", ex);
			}
		}
		void mniCsvImporter_Click(object sender, System.EventArgs e) {
			try {
				if (this.mniCsvImporter.Checked == false) {
					//v1 IT_WILL_FOLLOW_DockContent.ShowHint__REMOVED_DockPanel_ActiveDocumentChanged() CsvImporterForm.Instance.ShowAsDocumentTabNotPane(this.DockPanel);
					//this.mniCsvImporter.Checked = true;
					//v2
					//CsvImporterForm.Instance.Show(this.DockPanel);
					//CsvImporterForm.Instance.SubscribeVisibleChanged_onlyOnce(delegate(bool visibleReceived) {
					//    this.mniCsvImporter.Checked = visibleReceived;
					//});
					//CsvImporterForm.Instance.ActivateDockContent_popupAutoHidden(false, true);

					CsvImporterForm.Instance.ShowHinted_ActivateIfAutoHidden_SubscribeToVisibleChanged(this.DockPanel, delegate(bool visibleReceived) {
						this.mniCsvImporter.Checked = visibleReceived;
					});
				} else {
					//YOU_CAN_CLOSE_IT_WITH_X__HERE_IT_WORKS_AS_TASKBAR_MINIMIZING_INTO_DOCKED_STATE CsvImporterForm.Instance.Close();
					//NO_NEED this.mniCsvImporter.Checked = false;
					CsvImporterForm.Instance.ToggleAutoHide();
				}
				this.MainFormSerialize();
				this.ctxTools.Show();
			} catch (Exception ex) {
				Assembler.PopupException("mniCsvImporter_Click()", ex);
			}
		}
		void mniSymbolInfoEditor_Click(object sender, System.EventArgs e) {
			try {
				//if (this.mniSymbolInfoEditor.Checked == false) {
				if (SymbolInfoEditorForm.Instance.IsShown == false) {
					SymbolInfoEditorForm.Instance.Show(this.DockPanel);
					//NO_NEED_HANDLED_IN_MainFormEventManagerInitializeWhenDockingIsNotNullAnymore this.mniSymbolInfoEditor.Checked = true;
				} else {
					SymbolInfoEditorForm.Instance.Hide();
					//NO_NEED_HANDLED_IN_MainFormEventManagerInitializeWhenDockingIsNotNullAnymore this.mniSymbolInfoEditor.Checked = false;
				}
				this.MainFormSerialize();
				this.ctxTools.Show();
			} catch (Exception ex) {
				Assembler.PopupException("mniSymbolInfoEditor_Click()", ex);
			}
		}
		void mniChartSettingsEditor_Click(object sender, EventArgs e) {
			try {
				//if (this.mniSymbolInfoEditor.Checked == false) {
				if (ChartSettingsEditorForm.Instance.IsShown == false) {
					ChartSettingsEditorForm.Instance.Show(this.DockPanel);
					//NO_NEED_HANDLED_IN_MainFormEventManagerInitializeWhenDockingIsNotNullAnymore this.mniSymbolInfoEditor.Checked = true;
				} else {
					ChartSettingsEditorForm.Instance.Hide();
					//NO_NEED_HANDLED_IN_MainFormEventManagerInitializeWhenDockingIsNotNullAnymore this.mniSymbolInfoEditor.Checked = false;
				}
				this.MainFormSerialize();
				this.ctxTools.Show();
			} catch (Exception ex) {
				Assembler.PopupException("mniChartSettingsEditor_Click()", ex);
			}
		}
		void mniBarsEditor_Click(object sender, System.EventArgs e) {
			try {
				//if (this.mniSymbolInfoEditor.Checked == false) {
				if (BarsEditorForm.Instance.IsShown == false) {
					//BarsEditorForm.Instance.Show(this.DockPanel);
					BarsEditorForm.Instance.ShowStackedHinted(this.DockPanel);
					//NO_NEED_HANDLED_IN_MainFormEventManagerInitializeWhenDockingIsNotNullAnymore this.mniSymbolInfoEditor.Checked = true;
				} else {
					BarsEditorForm.Instance.Hide();
					//NO_NEED_HANDLED_IN_MainFormEventManagerInitializeWhenDockingIsNotNullAnymore this.mniSymbolInfoEditor.Checked = false;
				}
				this.MainFormSerialize();
				this.ctxTools.Show();
			} catch (Exception ex) {
				Assembler.PopupException("mniBarsEditor_Click()", ex);
			}
		}
		void mniFuturesMerger_Click(object sender, System.EventArgs e) {
			try {
				//if (this.mniSymbolInfoEditor.Checked == false) {
				if (FuturesMergerForm.Instance.IsShown == false) {
					FuturesMergerForm.Instance.Show(this.DockPanel);
					//NO_NEED_HANDLED_IN_MainFormEventManagerInitializeWhenDockingIsNotNullAnymore this.mniSymbolInfoEditor.Checked = true;
				} else {
					FuturesMergerForm.Instance.Hide();
					//NO_NEED_HANDLED_IN_MainFormEventManagerInitializeWhenDockingIsNotNullAnymore this.mniSymbolInfoEditor.Checked = false;
				}
				this.MainFormSerialize();
				this.ctxTools.Show();
			} catch (Exception ex) {
				Assembler.PopupException("mniFuturesMerger_Click()", ex);
			}
		}
		#endregion

		//http://stackoverflow.com/questions/2272019/how-to-display-a-windows-form-in-full-screen-on-top-of-the-taskbar
		Rectangle boundsBeforeGoingFullScreen;
		void btnFullScreen_Click(object sender, EventArgs e) {
			this.btnFullScreen.Checked = !this.btnFullScreen.Checked;
			this.GuiDataSnapshot.MainFormIsFullScreen = this.btnFullScreen.Checked;
			if (this.btnFullScreen.Checked) {
				base.WindowState = FormWindowState.Normal;
				base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
				this.boundsBeforeGoingFullScreen = this.Bounds;
				//this.Bounds = Screen.PrimaryScreen.Bounds;
				base.Bounds = Screen.GetWorkingArea(this);
			} else {
				//base.WindowState = FormWindowState.Maximized;
				base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
				base.Bounds = this.boundsBeforeGoingFullScreen;
			}
		}
		void mniExit_Click(object sender, EventArgs e) {
			//v1 closes current DockContent under DockPanel (and makes app irresponsive?)
			//Application.Exit();
			//v2
			base.Close();
		}
		void ctxWindowsOpening(object sender, System.ComponentModel.CancelEventArgs e) {
			this.ctxWindows.Items.Clear();
			foreach (var mgr in this.GuiDataSnapshot.ChartFormManagers.Values) {
				var mniRoot = new ToolStripMenuItem();
				mniRoot.Text = mgr.ChartForm.Text;
				var ctxChartRelatedForms = new ContextMenuStrip();
				
				bool separatorAdded = false;
				foreach (string textForMenu in mgr.FormsAllRelated.Keys) {
					DockContentImproved formImproved = mgr.FormsAllRelated[textForMenu];
					
					if (formImproved is ReporterFormWrapper && separatorAdded == false) {
						ToolStripSeparator sep = new ToolStripSeparator();
						ctxChartRelatedForms.Items.Add(sep);
						separatorAdded = true;
						continue;
					}
					
					ToolStripMenuItem mniChartSubitem = new ToolStripMenuItem();
					mniChartSubitem.Text = textForMenu;
					mniChartSubitem.Tag = formImproved;
					mniChartSubitem.Checked = (formImproved.IsCoveredOrAutoHidden == false);
					mniChartSubitem.Click += new EventHandler(mniWindowsCtxChart_SubitemClick);
					mniChartSubitem.MouseEnter += new EventHandler(mniWindowsCtxChart_SubitemMouseEnter);
					mniChartSubitem.MouseLeave += new EventHandler(mniWindowsCtxChart_SubitemMouseLeave);
					mniChartSubitem.CheckOnClick = false;
					ctxChartRelatedForms.Items.Add(mniChartSubitem);
				}
				mniRoot.DropDown = ctxChartRelatedForms;
				this.ctxWindows.Items.Add(mniRoot);
			}
		}
		void mniWindowsCtxChart_SubitemClick(object sender, EventArgs e) {
			try {
				ToolStripMenuItem mniChartRelatedForm = sender as ToolStripMenuItem;
				if (mniChartRelatedForm == null) {
					string msg = "SENDER_MUST_BE_ToolStripMenuItem sender[" + sender + "]";
					Assembler.PopupException(msg + " //mniWindowsCtxShart_SubitemClick()");
					return;
				}
				DockContentImproved chartRelatedForm = mniChartRelatedForm.Tag as DockContentImproved;
				if (chartRelatedForm == null) {
					string msg = "CHART_RELATED_FORM_MUST_BE_DockContentImproved mniChartRelatedForm["
						+ mniChartRelatedForm.Text + "].Tag[" + mniChartRelatedForm.Tag + "]";
					Assembler.PopupException(msg + " //mniWindowsCtxShart_SubitemClick()");
					return;
				}
				mniChartRelatedForm.Checked = !mniChartRelatedForm.Checked;
				if (mniChartRelatedForm.Checked) {
					chartRelatedForm.Activate();
				}
				this.ctxWindows.Show();
			} catch (Exception ex) {
				Assembler.PopupException("//mniWindowsCtxChart_SubitemClick()", ex, false);
			}
		}
		void mniWindowsCtxChart_SubitemMouseEnter(object sender, EventArgs e) {
			try {
				ToolStripMenuItem mniChartRelatedForm = sender as ToolStripMenuItem;
				if (mniChartRelatedForm == null) {
					string msg = "SENDER_MUST_BE_ToolStripMenuItem sender[" + sender + "]";
					Assembler.PopupException(msg + " //mniWindowsCtxChart_SubitemMouseEnter()");
					return;
				}
				DockContentImproved chartRelatedForm = mniChartRelatedForm.Tag as DockContentImproved;
				if (chartRelatedForm == null) {
					string msg = "CHART_RELATED_FORM_MUST_BE_DockContentImproved mniChartRelatedForm["
						+ mniChartRelatedForm.Text + "].Tag[" + mniChartRelatedForm.Tag + "]";
					Assembler.PopupException(msg + " //mniWindowsCtxChart_SubitemMouseEnter()");
					return;
				}
				if (chartRelatedForm.IsFloatingWindow) {
					string msg = "RETURNS_BACK_TO_DOCUMENT_A_CHART__FLOATING_NOONE_KNOWS_WHERE__AFTER_ACCIDENTAL_DOUBLECLICK_ON_DOCUMENT_TAB";
					chartRelatedForm.Show(this.DockPanel, DockState.Document);	// a floating window somewhere outside the screen boundaries is returned back to life
				}
				if (mniChartRelatedForm.Checked == false) {
					chartRelatedForm.ToggleAutoHide();
				}
				chartRelatedForm.Activate();
			} catch (Exception ex) {
				Assembler.PopupException("//mniWindowsCtxChart_SubitemMouseEnter()", ex, false);
			}
		}
		void mniWindowsCtxChart_SubitemMouseLeave(object sender, EventArgs e) {
			ToolStripMenuItem mniChartRelatedForm = sender as ToolStripMenuItem;
			if (mniChartRelatedForm == null) {
				string msg = "SENDER_MUST_BE_ToolStripMenuItem sender[" + sender + "]";
				Assembler.PopupException(msg + " //mniWindowsCtxChart_SubitemMouseLeave()");
				return;
			}
			DockContentImproved chartRelatedForm = mniChartRelatedForm.Tag as DockContentImproved;
			if (chartRelatedForm == null) {
				string msg = "CHART_RELATED_FORM_MUST_BE_DockContentImproved mniChartRelatedForm["
					+ mniChartRelatedForm.Text + "].Tag[" + mniChartRelatedForm.Tag + "]";
				Assembler.PopupException(msg + " //mniWindowsCtxChart_SubitemMouseLeave()");
				return;
			}
			if (mniChartRelatedForm.Checked) return;	// leave it open after I clicked to confirm I want it open
			// otherwize I didn't click and it was IsCoveredOrAutoHidden==false; put it back Left=>LeftHidden
			chartRelatedForm.ToggleAutoHide();
		}

		void mniWorkSpaces_DropDownOpening(object sender, EventArgs e) {
			this.WorkspacesManager.RescanRebuildWorkspacesMenu();
		}
	}
}
