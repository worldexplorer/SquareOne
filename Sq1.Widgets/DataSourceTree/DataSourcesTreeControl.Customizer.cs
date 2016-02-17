using System;
using System.Collections.Generic;
using System.Drawing;

using BrightIdeasSoftware;

using Sq1.Core;
using Sq1.Core.DataFeed;
using Sq1.Core.Charting;

namespace Sq1.Widgets.DataSourcesTree {
	public partial class DataSourcesTreeControl {
		void dataSourceTreeListViewCustomize() {
			//v2	http://stackoverflow.com/questions/9802724/how-to-create-a-multicolumn-treeview-like-this-in-c-sharp-winforms-app/9802753#9802753
			this.OlvTree.CanExpandGetter = delegate(object o) {
				DataSource dataSource = o as DataSource;
				if (dataSource != null) return dataSource.Symbols.Count > 0;
				
				//string symbol = o as string;
				//if (symbol != null) return true;

				SymbolOfDataSource symbol = o as SymbolOfDataSource;
				if (symbol != null) {
					List<ChartShadow> charts = symbol.DataSource.ChartsOpenForSymbol.FindContentsOf__nullUnsafe(symbol);
					return charts.Count > 0;
				}

				return false;
			};
			this.OlvTree.ChildrenGetter = delegate(object o) {
				DataSource dataSource = o as DataSource;
				if (dataSource != null) return dataSource.ChartsOpenForSymbol.Keys;
					
				//string symbol = o as string;
				//if (symbol != null && dataSource.ChartsOpenForSymbol.ContainsKey(symbol)) return dataSource.ChartsOpenForSymbol[symbol];

				SymbolOfDataSource symbol = o as SymbolOfDataSource;
				if (symbol != null) {
					List<ChartShadow> charts = symbol.DataSource.ChartsOpenForSymbol.FindContentsOf__nullUnsafe(symbol);
					return charts;
				}

				ChartShadow chartShadow = o as ChartShadow;
				if (chartShadow != null) {
					return null;	// no children for 4th level; only DataSource => Symbols => Charts
				}

				string msig = " //DataSourcesTreeControl.OlvTree.ChildrenGetter()";
				string msg = "SHOULD_CONTAIN_ONLY_THREE_TYPES:DataSource,SymbolOfDataSource,ChartShadow";
				Assembler.PopupException(msg + msig);

				return null;
			};

			this.olvcName.ImageGetter = delegate(object o) {
				DataSource dataSource = o as DataSource;
				if (dataSource != null) return this.getProviderImageIndexForDataSource(dataSource);
				return null;
			};
			this.olvcName.AspectGetter = delegate(object o) {
				if (o == null) return "NULL_olvcName.AspectGetter";

				DataSource dataSource = o as DataSource;
				if (dataSource != null) {
					string dsName = dataSource.Name;
					if (this.AppendMarketToDataSourceName && dataSource.MarketInfo != null) {
						dsName += " :: " + dataSource.MarketInfo.Name;
					}
					return dsName;
				}

				//string symbol = o as string;
				//if (symbol != null) return symbol;

				SymbolOfDataSource symbol = o as SymbolOfDataSource;
				if (symbol != null) return symbol.Symbol;

				ChartShadow chartShadow = o as ChartShadow;
				if (chartShadow != null) return chartShadow.ToString();

				return o.ToString();
			};
			this.olvcTimeFrame.AspectGetter = delegate(object o) {
				if (o == null) return "NULL_olvcTimeFrame.AspectGetter";

				DataSource dataSource = o as DataSource;
				//if (dataSource != null) return dataSource.ScaleInterval.AsStringShort_cached;
				if (dataSource != null) return null;
					
				//string symbol = o as string;
				SymbolOfDataSource symbol = o as SymbolOfDataSource;
				if (symbol != null) return symbol.DataSource.ScaleInterval.AsStringShort_cached;

				ChartShadow chartShadow = o as ChartShadow;
				if (chartShadow != null && chartShadow.Bars != null) return chartShadow.Bars.ScaleInterval.AsStringShort_cached;

				return o.ToString();
			};

			this.OlvTree.UseCellFormatEvents = true;
			this.OlvTree.FormatRow += new EventHandler<FormatRowEventArgs>(olvTree_FormatRow);
		}
		

		void olvTree_FormatRow(object sender, FormatRowEventArgs e) {
			ChartShadow chartShadow = e.Model as ChartShadow;
			if (chartShadow == null) return;

			OLVListItem li = this.OlvTree.ModelToItem(chartShadow);
			//if (e.Item.BackColor == li.BackColor) return;

			if (chartShadow.ColorBackground_inDataSourceTree == null) return;
			if (e.Item.BackColor == chartShadow.ColorBackground_inDataSourceTree) return;
			e.Item.BackColor = chartShadow.ColorBackground_inDataSourceTree;
		}
	}
}