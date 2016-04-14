using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Collections.Generic;

using Sq1.Core;
using Sq1.Core.Repositories;
using Sq1.Core.DataTypes;
using Sq1.Core.DataFeed;
using Sq1.Widgets.RangeBar;

namespace Sq1.Widgets.FuturesMerger {
	public partial class BarsEditorUserControl : UserControl {
		public BarsEditorUserControl() {
			InitializeComponent();
			this.olvBarsEditor_customize();
			this.rangeBar.OnValueMaxChanged			+= new EventHandler<RangeArgs<DateTime>>(rangeBar_OnAnyValueChanged);
			this.rangeBar.OnValueMinChanged			+= new EventHandler<RangeArgs<DateTime>>(rangeBar_OnAnyValueChanged);
			this.rangeBar.OnValuesMinAndMaxChanged	+= new EventHandler<RangeArgs<DateTime>>(rangeBar_OnAnyValueChanged);
		}

				RepositoryJsonDataSources	repositoryJsonDataSources;
				Bars						barsCloned;
				Bars						barsCloned_rangeIamEditing;
				Bars						barsClonedReversed;

		public	string						DataSourceName					{ get {
			return this.barsCloned == null ? "BARS_NOT_LOADED_INVOKE_LoadBars()" : this.barsCloned.DataSource.Name;
		} }
		public	string						Symbol							{ get {
			return this.barsCloned == null ? "BARS_NOT_LOADED_INVOKE_LoadBars()" : this.barsCloned.Symbol;
		} }

		[Browsable(false)]
		public	string						ParentWindowTitle {
			get {
				string ret = "PARENT_NULL";
				Form parent = base.ParentForm;
				if (parent == null) {
					Assembler.PopupException(ret + " //BarsEditorUserControl.ParentWindowTitle.get()");
					return ret;
				}
				ret = parent.Text;
				return ret;
			}
			set {
				Form parent = base.ParentForm;
				if (parent == null) {
					Assembler.PopupException("base.ParentForm=NULL //BarsEditorUserControl.ParentWindowTitle.set(" + value + ")");
					return;
				}
				parent.Text = value;
			}
		}

		bool barsDescending = true;		// move to JSON
		bool showDataSourceTree = false;
		bool showRangeBar = true;
		int lastBarsToShow = 500;

		bool dataSourcesTreeCollapsed {
			get { return this.splitContainer1.Panel1Collapsed; }
			set {
				this.splitContainer1.Panel1Collapsed = value;
				this.showDataSourceTree = !value;
			}
		}
		bool barsRangeCollapsed {
			get { return this.splitContainer2.Panel2Collapsed; }
			set {
				this.splitContainer2.Panel2Collapsed = value;
				this.showRangeBar = !value;
			}
		}

		public void Initialize(RepositoryJsonDataSources repositoryJsonDataSources) {
			this.repositoryJsonDataSources = repositoryJsonDataSources;
			this.dataSourcesTreeControl.Initialize(repositoryJsonDataSources, false);
		}
		public void LoadBars(string dataSourceName, string symbol) {
			this.loadBars(dataSourceName, symbol, this.lastBarsToShow);
			this.PopulateGuiState();
		}

		public void PopulateGuiState() {
			this.cbxShowDatasources.Checked = this.showDataSourceTree;
			this.dataSourcesTreeCollapsed	= !this.showDataSourceTree;

			this.cbxShowRange.Checked	= this.showRangeBar;
			this.barsRangeCollapsed		= !this.showRangeBar;

			this.cbxRevert.Checked		= this.barsDescending;
		}


		void reload() {
			if (this.barsCloned == null) {
				string msg = "YOU_SHOULD_HAVE_LOADED_BARS_FIRST__AND_THEN_RELOAD";
				Assembler.PopupException(msg, null, false);
				return;
			}
			this.loadBars(this.DataSourceName, this.Symbol);
		}

		void loadBars(string dataSourceName, string symbol, int lastBarsToShow_otherwizeTooSlow = 1000) {
			string msig = " //BarsEditorUserControl.loadBars(dataSourceName[" + dataSourceName + "], symbol[" + symbol + "])";
			try {
				DataSource dataSource_nullUnsafe = this.repositoryJsonDataSources.DataSourceFind_nullUnsafe(dataSourceName);
				if (dataSource_nullUnsafe == null) {
					string msg = "DATASOURCE_NOT_FOUND_IN_REPOSITORY";
					Assembler.PopupException(msg + msig, null, false);
					return;
				}
				string millisElapsed = "";
				Bars barsOriginal = dataSource_nullUnsafe.BarsLoad_nullUnsafe(symbol, out millisElapsed);
				if (barsOriginal == null) {
					string msg = "SYMBOL_DOES_NOT_EXIST_IN_DATASOURCE";
					Assembler.PopupException(msg + msig, null, false);
					return;
				}

				this.barsCloned			= barsOriginal.SafeCopy_oneCopyForEachDisposableExecutors("CLONED_FOR_BARS_EDITOR", true);

				this.bars_selectRange_flushToGui(new BarDataRange(lastBarsToShow_otherwizeTooSlow), millisElapsed);
			} catch (Exception ex) {
				Assembler.PopupException(msig, ex, false);
			}
		}

		void bars_selectRange_flushToGui(BarDataRange rangeToShow, string millisElapsed = "") {
			this.barsCloned_rangeIamEditing = this.barsCloned.Clone_selectRange(rangeToShow, true);
			this.rangeBar.Initialize(this.barsCloned, this.barsCloned_rangeIamEditing);
			string windowTitle = "Bars Editor :: " + this.ToString();
			if (string.IsNullOrEmpty(millisElapsed)) windowTitle += " [" + millisElapsed + "]ms";
			this.ParentWindowTitle = windowTitle;

			//v1 this.olvBarsEditor.SetObjects(this.barsCloned_rangeIamEditing.InnerBars_exposedOnlyForEditor);
			//v2
			this.barsClonedReversed	= this.barsCloned_rangeIamEditing.SafeCopy_oneCopyForEachDisposableExecutors("RANGE_CLONED_FOR_BARS_EDITOR", true, this.barsDescending);
			this.olvBarsEditor.SetObjects(this.barsClonedReversed.InnerBars_exposedOnlyForEditor);

			this.btnReload.Enabled = true;
			//this.btnSave.Enabled = true;
		}

		public override string ToString() {
			string ret = "BARS_NOT_LOADED";
			if (this.barsCloned_rangeIamEditing == null) return ret;
			ret = this.barsCloned_rangeIamEditing.SymbolIntervalScaleCount_dataSourceName;
			ret += this.barsCloned_rangeIamEditing.InstanceAndReasonForClone;
			return ret;
		}
	}
}
