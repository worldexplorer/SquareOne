using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Sq1.Core.Serializers;

namespace Sq1.Core.Repositories {
	public class SerializerList<SYMBOL_INFO> : Serializer<List<SYMBOL_INFO>> {

		public ToolStripItemCollection Tsi_Dynamic_Collection(ToolStrip owner,
				string				mni_prefix			= null,
				List<SYMBOL_INFO>	exclude				= null,
				EventHandler		mni_onClick			= null,
				SYMBOL_INFO			currentToSelect		= default(SYMBOL_INFO),
				SYMBOL_INFO			currentToDisable	= default(SYMBOL_INFO)
			) {

			List<ToolStripItem> menuItems = this.Tsi_Dynamic_MniList(mni_prefix, exclude, mni_onClick, currentToSelect, currentToDisable);
			ToolStripItemCollection ret = new ToolStripItemCollection(owner, menuItems.ToArray());
			//ToolStripDropDown ret = new ToolStripDropDown();
			//ret = collection;
			return ret;
		}

		public List<ToolStripItem> Tsi_Dynamic_MniList(
				string				mni_prefix			= null,
				List<SYMBOL_INFO>	exclude				= null,
				EventHandler		mni_onClick			= null,
				SYMBOL_INFO			currentToSelect		= default(SYMBOL_INFO),
				SYMBOL_INFO			currentToDisable	= default(SYMBOL_INFO)
			) {

			List<ToolStripItem> ret = new List<ToolStripItem>();
			if (mni_prefix == null) mni_prefix = "mni_" + typeof(SYMBOL_INFO).GetType().Name + "_";

			List<SYMBOL_INFO> symbolInfos = base.EntityDeserialized;

			foreach (SYMBOL_INFO eachSymbolInfo in symbolInfos) {
				if (exclude != null && exclude.Contains(eachSymbolInfo)) continue;

				ToolStripMenuItem mni = new ToolStripMenuItem();
				mni.Tag = eachSymbolInfo;

				if (currentToSelect != null) {
					mni.Checked = eachSymbolInfo.Equals(currentToSelect);
				}

				if (currentToDisable != null) {
					bool disabled = eachSymbolInfo.Equals(currentToDisable);
					if (disabled) {
						mni.Enabled = false;
						mni.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
					}
				}

				mni.Name = mni_prefix + eachSymbolInfo.ToString();
				mni.Text = eachSymbolInfo.ToString();

				if (mni_onClick != null /*&& disabled == false*/ ) mni.Click += mni_onClick;
				
				ret.Add(mni);
			}
			return ret;
		}

	}
}
