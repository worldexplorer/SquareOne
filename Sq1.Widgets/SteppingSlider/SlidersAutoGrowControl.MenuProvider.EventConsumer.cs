using System;
using System.ComponentModel;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Core.StrategyBase;
using Sq1.Widgets.LabeledTextBox;
using System.Diagnostics;
using Sq1.Core.Indicators;

namespace Sq1.Widgets.SteppingSlider {
	public partial class SlidersAutoGrowControl {
		void mniScriptContextLoad_Click(object sender, EventArgs e) {
			// otherwize crash on slider change while "Parameter Bags" CTX is open this.ctxParameterBags_Opening(this, null);
			//this.ctxParameterBags.Hide();

			ContextScript scriptContextToLoad = this.ScriptContextFromMniTag(sender);
			if (scriptContextToLoad == null) {
				#if DEBUG
				Debugger.Break();
				#endif
				return;
			}
			if (this.Strategy.ScriptContextCurrent == scriptContextToLoad) {
				string msg = "SCRIPT_CONTEXT_ALREADY_LOADED won't RaiseOnScriptContextLoad(scriptContextToLoad.Name[" + scriptContextToLoad.Name + "])"
					+ ": GUI should disable the option to (re)load the same (current) ScriptContextCurrent.Name[" + this.Strategy.ScriptContextCurrent.Name + "]";
				// FIRST_LEVEL_MNI_HAS_VISUAL_TICK_CANT_DISABLE_SINCE_SUBMENU_HAS_CLONE_RENAME_DELETE_SO_SHUT_UP Assembler.PopupException(msg);
				return;
			}
			this.RaiseOnScriptContextLoadRequested(scriptContextToLoad.Name);
			// otherwize crash on slider change while "Parameter Bags" CTX is open
			this.ctxScriptContexts_Opening(this, null);
		}
		void mniltbScriptContextNewWithDefaults_UserTyped(object sender, LabeledTextBoxUserTypedArgs e) {
			string newScriptContextName = e.StringUserTyped;
			this.Strategy.ScriptContextAdd(newScriptContextName);
			this.RaiseOnScriptContextCreated(newScriptContextName);
			this.ctxScriptContexts_Opening(this, null);
		}
		void mniltbScriptContextDuplicateTo_UserTyped(object sender, LabeledTextBoxUserTypedArgs e) {
			string dupeScriptContextName = e.StringUserTyped;
			ContextScript scriptContextToDuplicate = this.ScriptContextFromMniTag(sender);
			this.Strategy.ScriptContextAdd(dupeScriptContextName, scriptContextToDuplicate);
			this.RaiseOnScriptContextDuplicated(dupeScriptContextName);
			this.ctxScriptContexts_Opening(this, null);
		}
		void mniltbScriptContextRenameTo_UserTyped(object sender, LabeledTextBoxUserTypedArgs e) {
			string scriptContextNewName = e.StringUserTyped;
			ContextScript scriptContextToRename = this.ScriptContextFromMniTag(sender);
			this.Strategy.ScriptContextRename(scriptContextToRename, scriptContextNewName);
			this.RaiseOnScriptContextRenamed(scriptContextNewName);
			this.ctxScriptContexts_Opening(this, null);
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
			this.ctxScriptContexts_Opening(this, null);
		}
		void ctxScriptContexts_Opening(object sender, CancelEventArgs e) {
			this.ctxScriptContexts.SuspendLayout();
			try {
				this.ctxScriptContexts.Items.Clear();
				var newMnis = this.TsiDynamic;
				//this.ctxParameterBags.Items.AddRange(newMnis);
				foreach (var mni in newMnis) {
					try {
						this.ctxScriptContexts.Items.Add(mni);
					} catch (Exception ex) {
						throw;
					}
				}
				if (sender == this && e == null) {
					// after NewContext -> Delete, ctxOperations menu pane jumps on top of application window
					this.ctxOperations.Hide();
					if (this.ctxScriptContexts.Visible == false) {
						this.ctxScriptContexts.Visible = true;
					}
				}
				this.syncMniAllParamsShowBorderAndNumeric();
			} catch (Exception ex) {
				string msg = "DID_YOU_CHANGE_NUMERIC_UPDOWN?";
				Assembler.PopupException("SlidersAutoGrow.EventConsumer.cs::ctxParameterBags_Opening()", ex);
			} finally {
				this.ctxScriptContexts.ResumeLayout(true);
			}
		}
		void mniScriptContext_DropDownOpening(object sender, EventArgs e) {
			try {
				// part#1/2 make operations relevant to current contextSript mouseovered
				var mniOpening = sender as ToolStripMenuItem;
				mniOpening.DropDown = this.ctxOperations;
				//this.ctxOperations.OwnerItem = mniOpening;	// will help to reach mniOpening.Tag as ScriptContext, from ctxOperations.Items[TextLabelBox]
				this.mniParameterBagLoad.Text = "Load [" + mniOpening.Text + "]";
				this.mniParameterBagDelete.Text = "Delete [" + mniOpening.Text + "]";
				
				if (this.Strategy.ScriptContextCurrent.Name == mniOpening.Text) {
					this.mniParameterBagDelete.Enabled = false;
					this.mniParameterBagDelete.Text += " [load next ctx NYI]";
					
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

				// part#2/2 rebuild ctxOperations to display all the ContextScript parameter values;
				ContextScript ctx = (ContextScript)this.ctxOperations.Tag;
				if (ctx == null) {
					string msg = "mniOpening.Tag[" + mniOpening.Tag + "] must inject ContextScript into ctxOperations.Tag[" + this.ctxOperations.Tag + "]";
					Assembler.PopupException(msg);
					return;
				}

				this.ctxOperations.SuspendLayout();
				this.ctxOperations.Items.Clear();

				foreach (IndicatorParameter param in ctx.ScriptAndIndicatorParametersMergedClonedForSequencerAndSliders) {
					//v1
					//ToolStripMenuItem mni = new ToolStripMenuItem();
					//mni.CheckOnClick = false;
					//mni.Text = param.ToString();

					//v2
					MenuItemLabeledTextBox mni = new MenuItemLabeledTextBox();
					mni.Text = param.FullName;
					mni.TextOffsetX = 42;
					//mni.TextWidth = 200;
					//TODO MAKE_IT_STRETCH mni.TextAutoSize = true;
					mni.InputFieldValue = param.ValueCurrent.ToString();
					mni.InputFieldOffsetX = 0;
					mni.InputFieldWidth = 40;
					mni.InputFieldEditable = false;
					this.ctxOperations.Items.Add(mni);
				}

				this.ctxOperations.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
					this.toolStripSeparator1,
					this.mniltbParameterBagRenameTo,
					this.mniltbParameterBagDuplicateTo,
					this.mniParameterBagDelete});
				this.ctxOperations.ResumeLayout(true);

			} catch (Exception ex) {
				Assembler.PopupException("mni[" + sender + "].DropDownOpening()", ex);
			}
		}
	}
}