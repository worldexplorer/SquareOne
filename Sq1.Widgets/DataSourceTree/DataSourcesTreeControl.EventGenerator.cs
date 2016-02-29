using System;

using Sq1.Core;
using Sq1.Core.DataFeed;
using Sq1.Widgets.DataSourceTree;

namespace Sq1.Widgets.DataSourcesTree {
	public partial class DataSourcesTreeControl {
		//http://stackoverflow.com/questions/261660/how-do-i-set-an-image-for-some-but-not-all-nodes-in-a-treeview?rq=1
		public event EventHandler<DataSourceEventArgs>			OnDataSourceSelected;
		public event EventHandler<DataSourceSymbolEventArgs>	OnSymbolSelected;
		public event EventHandler<ChartSelectedEventArgs>		OnChartSelected;

		public event EventHandler<DataSourceSymbolEventArgs>	OnNewChartForSymbolClicked;
		public event EventHandler<DataSourceSymbolEventArgs>	OnOpenStrategyForSymbolClicked;
		public event EventHandler<DataSourceSymbolEventArgs>	OnBarsEditorClicked;
		public event EventHandler<DataSourceSymbolEventArgs>	OnSymbolInfoEditorClicked;

		public event EventHandler<DataSourceEventArgs>			OnDataSourceEditClicked;
		//public event EventHandler<DataSourceEventArgs>		OnDataSourceDeleteClicked;
		//public event EventHandler<EventArgs>					OnDataSourceNewClicked;
		
		void raiseOnDataSourceSelected() {
			if (this.OnDataSourceSelected == null) {
				string msg = "DataSourcesTree.treeListView_CellClick(): event OnDataSourceSelected: no subscribers";
				//Assembler.PopupException(msg);
				return;
			}
			this.OnDataSourceSelected(this, new DataSourceEventArgs(this.DataSourceSelected));
		}
		void raiseOnSymbolSelected() {
			if (this.OnSymbolSelected == null) {
				string msg = "DataSourcesTree.treeListView_CellClick(): event OnSymbolSelected: no subscribers";
				//Assembler.PopupException(msg);
				return;
			}
			this.OnSymbolSelected(this, new DataSourceSymbolEventArgs(this.DataSourceSelected, this.SymbolSelected));
		}
		void raiseOnChartSelected() {
			if (this.OnChartSelected == null && this.DisplayingThirdLevel_withChartsOpen) {
				string msg = "DataSourcesTree.treeListView_CellClick(): event OnChartSelected: no subscribers";
				Assembler.PopupException(msg);
				return;
			}
			this.OnChartSelected(this, new ChartSelectedEventArgs(this.ChartSelected));
		}
		
		void raiseOnNewChartForSymbolClicked() {
			if (this.OnNewChartForSymbolClicked == null) {
				string msg = "DataSourcesTree.mniNewChartSymbol_Click(): event OnNewChartForSymbolClicked: no subscribers";
				Assembler.PopupException(msg);
				return;
			}
			this.OnNewChartForSymbolClicked(this, new DataSourceSymbolEventArgs(this.DataSourceSelected, this.SymbolSelected));
		}
		void raiseOnBarsEditorClicked() {
			if (this.OnBarsEditorClicked == null) {
				string msg = "DataSourcesTree.mniBarsEditorSymbol_Click(): event OnBarsEditorClicked: no subscribers";
				Assembler.PopupException(msg);
				return;
			}
			this.OnBarsEditorClicked(this, new DataSourceSymbolEventArgs(this.DataSourceSelected, this.SymbolSelected));
		}
		void raiseOnOpenStrategyForSymbolClicked() {
			if (this.OnOpenStrategyForSymbolClicked == null) {
				string msg = "DataSourcesTree.mniOpenStrategySymbol_Click(): event OnOpenStrategyForSymbolClicked: no subscribers";
				Assembler.PopupException(msg);
				return;
			}
			this.OnOpenStrategyForSymbolClicked(this, new DataSourceSymbolEventArgs(this.DataSourceSelected, this.SymbolSelected));
		}

		void raiseOnDataSourceDeleteClicked() {
			this.dataSourceRepository.ItemDelete(this.DataSourceSelected, this);
			//if (this.OnDataSourceDeleteClicked == null) {
			//	string msg = "DataSourcesTree.mniDeleteDS_Click(): event OnDataSourceDeleted: no subscribers";
			//	Assembler.PopupException(msg);
			//	return;
			//}
			//this.OnDataSourceDeleteClicked(this, new DataSourceEventArgs(this.DataSourceSelected));
		}
//		void RaiseOnDataSourceCreateClicked() {
//			if (this.OnDataSourceNewClicked == null) {
//				string msg = "DataSourcesTree.mniNewDataSource_Click(): event OnDataSourceNewClicked: no subscribers";
//				Assembler.PopupException(msg);
//				return;
//			}
//			this.OnDataSourceNewClicked(this, EventArgs.Empty);
//		}
		void raiseOnDataSourceEditClicked(DataSource foundWithSameName = null) {
			if (this.OnDataSourceEditClicked == null) {
				string msg = "DataSourcesTree.mniEditDataSource_Click(): event OnDataSourceEditClicked: no subscribers";
				Assembler.PopupException(msg);
				return;
			}
			if (foundWithSameName == null) {
				foundWithSameName = this.DataSourceSelected;
			}
			this.OnDataSourceEditClicked(this, new DataSourceEventArgs(foundWithSameName));
		}
		public void RaiseOnSymbolInfoEditorClicked() {
			if (this.DataSourceSelected	== null) return;
			if (this.SymbolSelected		== null) return;

			if (this.OnSymbolInfoEditorClicked == null) {
				string msg = "DataSourcesTree.mniEditDataSource_Click(): event OnDataSourceEditClicked: no subscribers";
				Assembler.PopupException(msg);
				return;
			}
			this.OnSymbolInfoEditorClicked(this, new DataSourceSymbolEventArgs(this.DataSourceSelected, this.SymbolSelected));
		}
	}
}