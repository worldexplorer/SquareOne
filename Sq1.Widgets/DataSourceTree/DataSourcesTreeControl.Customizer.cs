using System;
using System.Collections.Generic;

using Sq1.Core.DataFeed;
using Sq1.Core.Charting;

namespace Sq1.Widgets.DataSourcesTree {
	public partial class DataSourcesTreeControl {
		void dataSourceTreeListViewCustomize() {
			//v2	http://stackoverflow.com/questions/9802724/how-to-create-a-multicolumn-treeview-like-this-in-c-sharp-winforms-app/9802753#9802753
			this.tree.CanExpandGetter = delegate(object o) {
				DataSource dataSource = o as DataSource;
				if (dataSource != null) return dataSource.Symbols.Count > 0;
				
				//string symbol = o as string;
				//if (symbol != null) return true;

				SymbolOfDataSource symbol = o as SymbolOfDataSource;
				if (symbol != null) {
					List<ChartShadow> charts = symbol.DataSource.ChartsOpenForSymbol.FindContentsOf_NullUnsafe(symbol);
					return charts.Count > 0;
				}

				return false;
			};
			this.tree.ChildrenGetter = delegate(object o) {
				DataSource dataSource = o as DataSource;
				if (dataSource != null) return dataSource.ChartsOpenForSymbol.Keys;
					
				//string symbol = o as string;
				//if (symbol != null && dataSource.ChartsOpenForSymbol.ContainsKey(symbol)) return dataSource.ChartsOpenForSymbol[symbol];

				SymbolOfDataSource symbol = o as SymbolOfDataSource;
				if (symbol != null) {
					List<ChartShadow> charts = symbol.DataSource.ChartsOpenForSymbol.FindContentsOf_NullUnsafe(symbol);
					return charts;
				}

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
				if (chartShadow != null) return chartShadow.Bars.ScaleInterval.AsStringShort_cached;

				return o.ToString();
			};
		}
	}
}