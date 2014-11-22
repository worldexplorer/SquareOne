using System;
using System.Windows.Forms;
using System.Collections.Generic;
using Sq1.Core;

namespace Sq1.Gui.ReportersSupport {
	public class MenuItemsProvider {
		private ReportersFormsManager reportersFormsManager;
		public List<ToolStripItem> MenuItems;
		public const string MNI_NAME_PREFIX = "mniReporter_";

		public MenuItemsProvider(ReportersFormsManager reportersFormsManager, List<Type> reporterTypesAvailable) {
			this.reportersFormsManager = reportersFormsManager;
			this.MenuItems = new List<ToolStripItem>();
			foreach (Type t in reporterTypesAvailable) {
				this.MenuItems.Add(this.MenuItemFactory(t.Name));
			}
		}
		public ToolStripMenuItem MenuItemFactory(string reporterShortName) {
			var ret = new ToolStripMenuItem();
			ret.Text = "Show Reporter." + reporterShortName;
			ret.Name = MenuItemsProvider.MNI_NAME_PREFIX + reporterShortName;
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
				if (mni.Name != MenuItemsProvider.MNI_NAME_PREFIX + reporterShortName) continue;
				found = mni;
				break;
			}
			if (found == null) {
				string msg = "MenuItems doesn't contain a ToolStripMenuItem with .Text=typeNameShortOrFullAutodetect[" + reporterShortName + "]";
				throw new Exception(msg);
			}
			found.Checked = mniChecked;
		}
		public string StripPrefixFromMniName(ToolStripMenuItem tsiClicked) {
			if (tsiClicked.Name.StartsWith(MenuItemsProvider.MNI_NAME_PREFIX) == false) {
				string msg = "DONT_FEED_ME_WITH_MENUITEMS_I_DIDNT_CREATE tsiClicked.Name[" + tsiClicked.Name 
					+ "].StartsWith(" + MenuItemsProvider.MNI_NAME_PREFIX + "=false MUST_BE_TRUE //StripPrefixFromMniName(" + tsiClicked.Text + ")";
				Assembler.PopupException(msg);
				return tsiClicked.Name;
			}
			return tsiClicked.Name.Substring(MenuItemsProvider.MNI_NAME_PREFIX.Length);
		}

	}
}
