using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Core.StrategyBase;
using Sq1.Widgets.LabeledTextBox;
using System.Diagnostics;

namespace Sq1.Widgets.SteppingSlider {
	public partial class SlidersAutoGrowControl {
		void mniScriptContextLoad_Click(object sender, EventArgs e) {
			// otherwize crash on slider change while "Parameter Bags" CTX is open this.ctxParameterBags_Opening(this, null);
			this.ctxParameterBags.Hide();

			ContextScript scriptContextToLoad = this.ScriptContextFromMniTag(sender);
			if (scriptContextToLoad == null) {
				Debugger.Break();
				return;
			}
			if (this.Strategy.ScriptContextCurrent == scriptContextToLoad) {
				string msg = "SCRIPT_CONTEXT_ALREADY_LOADED won't RaiseOnScriptContextLoad(scriptContextToLoad.Name[" + scriptContextToLoad.Name + "])"
					+ ": GUI should disable the option to (re)load the same (current) ScriptContextCurrent.Name[" + this.Strategy.ScriptContextCurrent.Name + "]";
				// FIRST_LEVEL_MNI_HAS_VISUAL_TICK_CANT_DISABLE_SINCE_SUBMENU_HAS_CLONE_RENAME_DELETE_SO_SHUT_UP Assembler.PopupException(msg);
				return;
			}
			this.RaiseOnScriptContextLoadRequested(scriptContextToLoad.Name);
		}
		void mniltbScriptContextNewWithDefaults_UserTyped(object sender, LabeledTextBoxUserTypedArgs e) {
			string newScriptContextName = e.StringUserTyped;
			this.Strategy.ScriptContextAdd(newScriptContextName);
			this.RaiseOnScriptContextCreated(newScriptContextName);
			this.ctxParameterBags_Opening(this, null);
		}
		void mniltbScriptContextDuplicateTo_UserTyped(object sender, LabeledTextBoxUserTypedArgs e) {
			string dupeScriptContextName = e.StringUserTyped;
			ContextScript scriptContextToDuplicate = this.ScriptContextFromMniTag(sender);
			this.Strategy.ScriptContextAdd(dupeScriptContextName, scriptContextToDuplicate);
			this.RaiseOnScriptContextDuplicated(dupeScriptContextName);
			this.ctxParameterBags_Opening(this, null);
		}
		void mniltbScriptContextRenameTo_UserTyped(object sender, LabeledTextBoxUserTypedArgs e) {
			string scriptContextNewName = e.StringUserTyped;
			ContextScript scriptContextToRename = this.ScriptContextFromMniTag(sender);
			this.Strategy.ScriptContextRename(scriptContextToRename, scriptContextNewName);
			this.RaiseOnScriptContextRenamed(scriptContextNewName);
			this.ctxParameterBags_Opening(this, null);
		}
		void mniScriptContextDelete_Click(object sender, EventArgs e) {
			string msig = "mniParameterBagDelete_Click() ";
			if (this.Strategy == null) {
				string msg = "you should keep your rightClickMenu disabled if current chart has no strategy";
				Assembler.PopupException(msig + msg);
				return;
			}
			ContextScript scriptContextToDelete = this.ScriptContextFromMniTag(sender);
			if (scriptContextToDelete == null) return;
			this.Strategy.ScriptContextDelete(scriptContextToDelete.Name);
			this.RaiseOnScriptContextDeleted(scriptContextToDelete.Name);
			this.ctxParameterBags_Opening(this, null);
		}
		void ctxParameterBags_Opening(object sender, CancelEventArgs e) {
			this.ctxParameterBags.SuspendLayout();
			try {
				this.ctxParameterBags.Items.Clear();
				this.ctxParameterBags.Items.AddRange(this.TsiDynamic);
				if (sender == this && e == null) {
					// after NewContext -> Delete, ctxOperations menu pane jumps on top of application window
					this.ctxOperations.Hide();
				}
				this.syncMniAllParamsShowBorderAndNumeric();
			} catch (Exception ex) {
				string msg = "DID_YOU_CHANGE_NUMERIC_UPDOWN?";
				Assembler.PopupException("SlidersAutoGrow.EventConsumer.cs::ctxParameterBags_Opening()", ex);
			} finally {
				this.ctxParameterBags.ResumeLayout(true);
			}
		}
		void mniScriptContext_DropDownOpening(object sender, EventArgs e) {
			try {
				var mniOpening = sender as ToolStripMenuItem;
				mniOpening.DropDown = this.ctxOperations;
				//this.ctxOperations.OwnerItem = mniOpening;	// will help to reach mniOpening.Tag as ScriptContext, from ctxOperations.Items[TextLabelBox]
				this.mniParameterBagLoad.Text = "Load [" + mniOpening.Text + "]";
				this.mniParameterBagDelete.Text = "Delete [" + mniOpening.Text + "]";
				
				if (this.Strategy.ScriptContextCurrent.Name == mniOpening.Text) {
					this.mniParameterBagDelete.Enabled = false;
					this.mniParameterBagDelete.Text += " [load next bag NYI]";
					
					this.mniParameterBagLoad.Enabled = false;
					this.mniParameterBagLoad.Text = "Already Loaded [" + mniOpening.Text + "]";
				} else {
					this.mniParameterBagDelete.Enabled = true;
					this.mniParameterBagLoad.Enabled = true;
				}
				
				if (mniOpening.Text == "Default") {
					this.mniltbParameterBagRenameTo.Enabled = false;
					this.mniParameterBagDelete.Enabled = false;
				} else {
					this.mniltbParameterBagRenameTo.Enabled = true;
				}
				
				this.ctxOperations.Tag = (ContextScript) mniOpening.Tag;
			} catch (Exception ex) {
				Assembler.PopupException("mni[" + sender + "].DropDownOpening()", ex);
			}
		}
	}
}