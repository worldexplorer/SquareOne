using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Gui.Singletons;
using Sq1.Widgets;
using WeifenLuo.WinFormsUI.Docking;

namespace Sq1.Gui {
	public partial class MainForm {
		void mainForm_Load(object sender, System.EventArgs e) {
			try {
				// separate try {} for MONO: Reflection bla-bla-bla exception
				this.MainFormEventManagerInitializeWhenDockingIsNotNullAnymore();
			} catch (Exception ex) {
				Assembler.PopupException("MainFormEventManagerInitializeWhenDockingIsNotNullAnymore()", ex);
			}
			try {
				this.createWorkspacesManager();
				
				// it looks like ChartForm doesn't propagate its DockContent-set size to ChartControl =>
				// for wider than in Designer ChartConrtrol sizes I see gray horizontal lines and SliderOutOfBoundaries Exceptions for smaller than in Designer
				// (Disable Resize during DockContent XML deserialization and fire manually for each ChartForm (Document only?) )
				this.SuspendLayout();
				
				this.WorkspaceLoad(Assembler.InstanceInitialized.AssemblerDataSnapshot.CurrentWorkspaceName);

				// it looks like ChartForm doesn't propagate its DockContent-set size to ChartControl =>
				// for wider than in Designer ChartConrtrol sizes I see gray horizontal lines and SliderOutOfBoundaries Exceptions for smaller than in Designer
				// (Disable Resize during DockContent XML deserialization and fire manually for each ChartForm (Document only?) )
				this.ResumeLayout(true);
			} catch (Exception ex) {
				Assembler.PopupException("mainForm_Load()", ex);
			}
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
			this.MainFormSerialize();
			this.MainFormClosingSkipChartFormsRemoval = true;
		}
		void mniExceptions_Click(object sender, EventArgs e) {
			try {
				if (this.mniExceptions.Checked == false) {
					ExceptionsForm.Instance.Show(this.DockPanel);
				} else {
					ExceptionsForm.Instance.Hide();
				}
				this.MainFormSerialize();
			} catch (Exception ex) {
				Assembler.PopupException("mniExceptions_Click()", ex);
			}
		}
		void mniSymbols_Click(object sender, EventArgs e) {
			try {
				if (this.mniSymbols.Checked == false) {
					DataSourcesForm.Instance.Show(this.DockPanel);
				} else {
					DataSourcesForm.Instance.Hide();
				}
				this.MainFormSerialize();
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
			} catch (Exception ex) {
				Assembler.PopupException("mniStrategies_Click()", ex);
			}
		}
		void mniExecution_Click(object sender, EventArgs e) {
			try {
				if (this.mniExecution.Checked == false) {
					ExecutionForm.Instance.Show(this.DockPanel);
					ExecutionForm.Instance.ExecutionTreeControl.PopulateDataSnapshotInitializeSplittersIfDockContentDeserialized();
				} else {
					ExecutionForm.Instance.Hide();
				}
				this.MainFormSerialize();
			} catch (Exception ex) {
				Assembler.PopupException("mniExecution_Click()", ex);
			}
		}
		void mniCsvImporter_Click(object sender, System.EventArgs e) {
			try {
				if (this.mniCsvImporter.Checked == false) {
					CsvImporterForm.Instance.ShowAsDocumentTabNotPane(this.DockPanel);
				} else {
					CsvImporterForm.Instance.Hide();
				}
				this.MainFormSerialize();
			} catch (Exception ex) {
				Assembler.PopupException("mniCsvImporter_Click()", ex);
			}
		}
		
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
				var ctxChildrenForms = new ContextMenuStrip();
				foreach (string textForMenu in mgr.FormsAllRelated.Keys) {
					DockContent form = mgr.FormsAllRelated[textForMenu];
					var mniChartForWindowsGrouped = new ToolStripMenuItem();
					mniChartForWindowsGrouped.Text = textForMenu;
					mniChartForWindowsGrouped.Tag = form;
					mniChartForWindowsGrouped.Checked = form.Visible;
					if (form.IsActivated) {
						mniChartForWindowsGrouped.Enabled = false;
					} else {
						mniChartForWindowsGrouped.Click += new EventHandler(mniWindowsCtxCharts_Click);
					}
					ctxChildrenForms.Items.Add(mniChartForWindowsGrouped);
				}
				mniRoot.DropDown = ctxChildrenForms;
				this.ctxWindows.Items.Add(mniRoot);
			}
		}
		void mniWindowsCtxCharts_Click(object sender, EventArgs e) {
			var mniWindowChartAnyRelatedForm = sender as ToolStripMenuItem;
			DockContentImproved anyAdressableForm = mniWindowChartAnyRelatedForm.Tag as DockContentImproved;
			if (anyAdressableForm == null) {
				string msg = "reporterToPopup.Parent IS_NOT DockContentImproved";
				#if DEBUG
				Debugger.Break();
				#endif
				Assembler.PopupException(msg + " //mniWindowsCtxCharts_Click()");
				return;
			}
			if (mniWindowChartAnyRelatedForm.Checked) {
				anyAdressableForm.ToggleAutoHide();		// forms in Document should be ignored
			} else {
				anyAdressableForm.ActivateDockContentPopupAutoHidden(false);
			}
		}
	}
}