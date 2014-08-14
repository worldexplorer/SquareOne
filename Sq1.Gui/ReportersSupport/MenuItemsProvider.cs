using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Sq1.Gui.ReportersSupport {
	public class MenuItemsProvider {
		private ReportersFormsManager reportersFormsManager;
		public List<ToolStripItem> MenuItems;

		public MenuItemsProvider(ReportersFormsManager reportersFormsManager, List<Type> reporterTypesAvailable) {
			this.reportersFormsManager = reportersFormsManager;
			this.MenuItems = new List<ToolStripItem>();
			foreach (Type t in reporterTypesAvailable) {
				this.MenuItems.Add(this.MenuItemFactory(t.Name));
			}
		}
		public ToolStripMenuItem MenuItemFactory(string reporterShortName) {
			var ret = new ToolStripMenuItem();
			ret.Text = reporterShortName;
			ret.Name = "mni_" + reporterShortName;
			//mni.Size = new System.Drawing.Size(152, 22);
			ret.Click += new System.EventHandler(this.Mni_Click);
			return ret;
		}
		void Mni_Click(object sender, EventArgs e) {
			this.reportersFormsManager.ChartForm_OnReporterMniClicked(sender, e);
		}

		internal void FindMniByShortNameAndTick(string reporterShortName, bool mniChecked=true) {
			ToolStripMenuItem found = null;
			//foreach (ToolStripItem tsi in this.CtxMenuWithReportersAvailable.Items) {
			foreach (ToolStripMenuItem mni in this.MenuItems) {
				if (mni.Text != reporterShortName) continue;
				found = mni;
				break;
			}
			if (found == null) {
				string msg = "MenuItems doesn't contain a ToolStripMenuItem with .Text=typeNameShortOrFullAutodetect[" + reporterShortName + "]";
				throw new Exception(msg);
			}
			found.Checked = mniChecked;
		}
	}
}
