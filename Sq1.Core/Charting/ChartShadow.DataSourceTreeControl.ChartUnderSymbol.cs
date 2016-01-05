using System.Collections.Generic;
using Sq1.Core.DataFeed;

namespace Sq1.Core.Charting {
	public partial class ChartShadow {
		public SymbolOfDataSource SymbolOfDataSource { get; private set; }

		public void ChartShadow_AddToDataSource() {
			string msig = " //ChartShadow_AddToDataSource("  + this.ToString() + ")";
			if (this.Bars == null) {
				string msg = "DONT_ALLOW_CHART_WITHOUT_BARS";
				Assembler.PopupException(msg + msig);
				return;
			}
			if (this.Bars.DataSource == null) {
				string msg = "IM_LOADING_RANDOM_GENERATED_BARS_250";
				//Assembler.PopupException(msg);
				return;
			}
			string symbol = this.Bars.Symbol;
			DictionaryManyToOne<SymbolOfDataSource, ChartShadow> chartsOpenForSymbol = this.Bars.DataSource.ChartsOpenForSymbol;
			SymbolOfDataSource newForLivesim = new SymbolOfDataSource(symbol, this.Bars.DataSource);
			SymbolOfDataSource symbolOfDataSourceFound = chartsOpenForSymbol.FindSimilarKey(newForLivesim);
			if (symbolOfDataSourceFound != null) {
				List<ChartShadow> addingToList = chartsOpenForSymbol.FindContentsOf_NullUnsafe(symbolOfDataSourceFound);
				if (addingToList.Contains(this) == false) {
					chartsOpenForSymbol.Add(symbolOfDataSourceFound, this);
				} else {
					string msg = "YOU_ALREADY_ADDED_CHART_SHADOW_TO_DATASOURCE this.Bars.DataSource[" + this.Bars.DataSource + "].ChartsOpenForSymbol[" + symbolOfDataSourceFound + "]";
					//DESERIALIZATION AND CHART_LOADED Assembler.PopupException(msg + msig);
				}
			} else {
				string msg = "LIVESIM_HAS_OWN_DATASOURCE__YOU_DIDNT_ADD_SYMBOL_TO_DATASOURCE.ChartsOpenForSymbol.Keys this.Bars.DataSource[" + this.Bars.DataSource + "].ChartsOpenForSymbol[" + symbolOfDataSourceFound + "]";
				Assembler.PopupException(msg + msig);
				chartsOpenForSymbol.Add(newForLivesim, this);
			}
			this.SymbolOfDataSource = this.Bars.DataSource.ChartsOpenForSymbol.FindContainerFor_throws(this);
		}
		public void ChartShadow_RemoveFromDataSource() {
			string msig = " //ChartShadow_RemoveFromDataSource("  + this.ToString() + ")";
			if (this.Bars == null) {
				string msg = "DONT_ALLOW_CHART_WITHOUT_BARS";
				Assembler.PopupException(msg + msig);
				return;
			}
			if (this.Bars.DataSource == null) {
				string msg = "IM_REPLACING_RANDOM_GENERATED_BARS_250_WITH_SELECTED_FROM_DATASOURCE";
				//Assembler.PopupException(msg);
				return;
			}
			string symbol = this.Bars.Symbol;
			DictionaryManyToOne<SymbolOfDataSource, ChartShadow> chartsOpenForSymbol = this.Bars.DataSource.ChartsOpenForSymbol;
			SymbolOfDataSource symbolOfDataSourceFound = chartsOpenForSymbol.FindSimilarKey(new SymbolOfDataSource(symbol, this.Bars.DataSource));
			if (symbolOfDataSourceFound != null) {
				List<ChartShadow> iMustBeHere = chartsOpenForSymbol.FindContentsOf_NullUnsafe(symbolOfDataSourceFound);
				if (iMustBeHere.Contains(this)) {
					chartsOpenForSymbol.Remove(symbolOfDataSourceFound, this);
				} else {
					string msg = "YOU_DIDNT_ADD_CHART_SHADOW_TO_DATASOURCE this.Bars.DataSource[" + this.Bars.DataSource + "].ChartsOpenForSymbol[" + symbol + "]";
					Assembler.PopupException(msg + msig);
				}
			} else {
				string msg = "YOU_DIDNT_ADD_SYMBOL_TO_DATASOURCE.ChartsOpenForSymbol.Keys this.Bars.DataSource[" + this.Bars.DataSource + "].ChartsOpenForSymbol[" + symbol + "]";
				Assembler.PopupException(msg + msig);
			}
			this.SymbolOfDataSource = null;
		}
	}
}
