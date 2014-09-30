using System;
using System.Drawing;
using System.Windows.Forms;

using Sq1.Core;
using WeifenLuo.WinFormsUI.Docking;

namespace Sq1.Gui.Singletons {
	public partial class MainForm {
		void mainForm_Load(object sender, System.EventArgs e) {
			try {
				// separate try {} for MONO: Reflection bla-bla-bla exception
				this.MainFormEventManagerInitializeWhenDockingIsNotNullAnymore();
			} catch (Exception ex) {
				this.PopupException(ex);
			}
			try {
				this.createWorkspacesManager();
				this.WorkspaceLoad(Assembler.InstanceInitialized.AssemblerDataSnapshot.CurrentWorkspaceName);
			} catch (Exception ex) {
				this.PopupException(ex);
			}
		}

		protected override void WndProc(ref Message m) {
			//http://stackoverflow.com/questions/3155782/what-is-the-difference-between-wm-quit-wm-close-and-wm-destroy-in-a-windows-pr
//		    public enum Msgs {
//		        ...WM_CLOSE                  = 0x0010,...
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
				this.PopupException(ex);
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
				this.PopupException(ex);
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
				this.PopupException(ex);
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
				this.PopupException(ex);
			}
		}
		void mniExecution_Click(object sender, EventArgs e) {
			try {
				if (this.mniExecution.Checked == false) {
					ExecutionForm.Instance.Show(this.DockPanel);
					ExecutionForm.Instance.ExecutionTreeControl.PopulateDataSnapshotInitializeSplittersAfterDockContentDeserialized();
				} else {
					ExecutionForm.Instance.Hide();
				}
				this.MainFormSerialize();
			} catch (Exception ex) {
				this.PopupException(ex);
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
				this.PopupException(ex);
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
		void ctxChartsOpening(object sender, System.ComponentModel.CancelEventArgs e) {
			this.ctxWindows.Items.Clear();
			foreach (var mgr in this.GuiDataSnapshot.ChartFormManagers.Values) {
				var mniRoot = new ToolStripMenuItem();
				mniRoot.Text = mgr.ChartForm.Text;
				var ctxChildrenForms = new ContextMenuStrip();
				foreach (string textForMenu in mgr.FormsAllRelated.Keys) {
					DockContent form = mgr.FormsAllRelated[textForMenu];
					var mniChild = new ToolStripMenuItem();
					mniChild.Text = textForMenu;
					mniChild.Tag = form;
					mniChild.Checked = form.Visible;
					if (form.IsActivated) {
						mniChild.Enabled = false;
					} else {
						mniChild.Click += new EventHandler(mniChildctxCharts_Click);
					}
					ctxChildrenForms.Items.Add(mniChild);
				}
				mniRoot.DropDown = ctxChildrenForms;
				this.ctxWindows.Items.Add(mniRoot);
			}
		}
		void mniChildctxCharts_Click(object sender, EventArgs e) {
			var mniChild = sender as ToolStripMenuItem;
			DockContent frmClicked = mniChild.Tag as DockContent;
			if (mniChild.Checked) {
				DockHelper.ToggleAutoHide(frmClicked);		// forms in Document should be ignored
			} else {
				DockHelper.ActivateDockContentPopupAutoHidden(frmClicked, false);
			}
		}
	}
}