using System;
using System.IO;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Core.StrategyBase;
using Sq1.Gui.Forms;
using Sq1.Widgets.ScriptEditor;

namespace Sq1.Gui.FormFactories {
	public class ScriptEditorFormFactory {	// REASON_TO_EXIST: allows to run backtest in ChartForm context, stores Strategy.SourceCode in JSON, bridges Sq1.Widgets.dll and Sq1.Core.dll
		ChartFormManager chartFormManager;
		public ScriptEditorFormFactory(ChartFormManager chartFormsManager) {
			this.chartFormManager = chartFormsManager;
		}
		public void CreateEditorFormSubscribePushToManager(ChartFormManager chartFormsManager) {
			ScriptEditorForm scriptEditorForm = new ScriptEditorForm(chartFormsManager);
			scriptEditorForm.ScriptEditorControl.OnSave				+= scriptEditorControl_OnSave;
			scriptEditorForm.ScriptEditorControl.OnCompile			+= scriptEditorControl_OnCompile;
			scriptEditorForm.ScriptEditorControl.OnRun				+= scriptEditorControl_OnRun;
			scriptEditorForm.ScriptEditorControl.OnDebug			+= scriptEditorControl_OnDebug;
			scriptEditorForm.ScriptEditorControl.OnTextNotSaved		+= scriptEditorControl_OnTextNotSaved;
			scriptEditorForm.FormClosing							+= new FormClosingEventHandler(scriptEditorForm_FormClosing);
			scriptEditorForm.FormClosed								+= new  FormClosedEventHandler(scriptEditorForm_FormClosed);
			this.chartFormManager.ScriptEditorForm = scriptEditorForm;
		}
		void scriptEditorControl_OnTextNotSaved(object sender, ScriptEditorEventArgs e) {
			try {
				if (this.chartFormManager.Strategy.ScriptEditedNeedsSaving) return;
				this.chartFormManager.Strategy.ScriptEditedNeedsSaving = true;
				this.chartFormManager.PopulateWindowTitlesFromChartContextOrStrategy();
			} catch (Exception ex) {
				Assembler.PopupException("ScriptEditorControl_OnTextNotSaved", ex);
			}
		}
		void scriptEditorControl_OnSave(object sender, ScriptEditorEventArgs e) {
			try {
				Strategy strategy = this.chartFormManager.Strategy;
				strategy.ScriptSourceCode = e.ScriptText;
				strategy.Serialize();
				this.chartFormManager.MainForm.DisplayStatus("Strategy [" + Path.Combine(strategy.StoredInFolderRelName, strategy.StoredInJsonRelName) + "] saved");
				this.chartFormManager.Strategy.ScriptEditedNeedsSaving = false;
				this.chartFormManager.PopulateWindowTitlesFromChartContextOrStrategy();
			} catch (Exception ex) {
				Assembler.PopupException("ScriptEditorControl_OnSave", ex);
			}
		}
		void scriptEditorControl_OnCompile(object sender, ScriptEditorEventArgs e) {
			try {
				this.scriptEditorControl_OnSave(sender, e);
			} catch (Exception ex) {
				Assembler.PopupException("SAVING_STRATEGY_SOURCE_CODE_FAILED //ScriptEditorControl_OnSave() << ScriptEditorControl_OnCompile()", ex);
			}
			try {
				this.chartFormManager.StrategyCompileActivatePopulateSlidersShow();
				this.chartFormManager.SequencerFormIfOpenPropagateTextboxesOrMarkStaleResultsAndDeleteHistory(true);
			} catch (Exception ex) {
				Assembler.PopupException("COMPILING_STRATEGY_SOURCE_CODE_FAILED //ScriptEditorControl_OnCompile() << StrategyCompileActivatePopulateSlidersShow() has thrown", ex);
			}
		}
		void scriptEditorControl_OnDebug(object sender, ScriptEditorEventArgs e) {
			throw new NotImplementedException();
		}
		void scriptEditorControl_OnRun(object sender, ScriptEditorEventArgs e) {
			try {
				this.scriptEditorControl_OnCompile(sender, e);
				this.chartFormManager.BacktesterRunSimulation();
			} catch (Exception ex) {
				Assembler.PopupException("ScriptEditorControl_OnRun", ex);
			}
		}
		void scriptEditorForm_FormClosing(object sender, FormClosingEventArgs e) {
			// only when user closed => allow scriptEditorForm_FormClosed() to serialize
			if (this.chartFormManager.MainForm.MainFormClosingSkipChartFormsRemoval) {
				e.Cancel = true;
				return;
			}
			if (Assembler.InstanceInitialized.MainFormClosingIgnoreReLayoutDockedForms) {
				e.Cancel = true;
				return;
			}
		}
		void scriptEditorForm_FormClosed(object sender, FormClosedEventArgs e) {
			// both at FormCloseByX and MainForm.onClose()
			this.chartFormManager.ChartForm.MniShowSourceCodeEditor.Checked = false;
			this.chartFormManager.ScriptEditorForm = null;
			this.chartFormManager.MainForm.MainFormSerialize();
		}
	}
}
