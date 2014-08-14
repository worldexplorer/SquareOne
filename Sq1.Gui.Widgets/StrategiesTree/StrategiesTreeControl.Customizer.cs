using System;
using System.Collections.Generic;
using Sq1.Core.StrategyBase;

namespace Sq1.Widgets.StrategiesTree {
	public partial class StrategiesTreeControl {
		void strategyTreeListViewCustomize() {
			//v2	http://stackoverflow.com/questions/9802724/how-to-create-a-multicolumn-treeview-like-this-in-c-sharp-winforms-app/9802753#9802753
			this.tree.CanExpandGetter = delegate(object o) {
				var folder = o as string;
				if (folder == null) return false;
				bool ret = false;
				if (strategyRepository.StrategiesInFolders.ContainsKey(folder)) {
					ret = strategyRepository.StrategiesInFolders[folder].Count > 0;
				} else {
					string msg = "I modify the folder, saying DLL_NOT_FOUND so there is no entry";
				}
				return ret;
			};
			this.tree.ChildrenGetter = delegate(object o) {
				var folder = o as string;
				var strategies = strategyRepository.StrategiesInFolders[folder];
				if (strategies == null) return o.ToString();
				return strategies.ToArray();
			};

			this.olvColumnName.AspectGetter = delegate(object o) {
				var strategy = o as Strategy;
				if (strategy == null) return o.ToString();	//folder, then
				string humanReadable = strategy.Name;
				//if (strategy.ScriptContextCurrent != null) {
				//	humanReadable += " :: " + strategy.ScriptContextCurrent.Name;
				//}
				return humanReadable;
			};
			this.olvColumnName.ImageGetter = delegate(object o) {
				var strategy = o as Strategy;
				if (strategy != null) return null;
				var folder = o as string;
				if (string.IsNullOrEmpty(folder)) return null;
				//if (this.strategyRepository.StrategiesInDlls.ContainsKey(folder)) {
				if (folder.ToUpper().EndsWith(".DLL")) {
					// 2:dll-closed.png, 3:dll-opened.png
					return this.tree.IsExpanded(o) ? 2 : 3;
				}
				// 0:folder-closed.png, 1:folder-opened.png
				return this.tree.IsExpanded(o) ? 1 : 0;
			};
		}
	}
}