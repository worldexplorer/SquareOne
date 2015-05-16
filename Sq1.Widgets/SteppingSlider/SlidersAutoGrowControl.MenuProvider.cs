using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;

using Sq1.Core;
using Sq1.Core.StrategyBase;
using Sq1.Widgets.LabeledTextBox;

namespace Sq1.Widgets.SteppingSlider {
	public partial class SlidersAutoGrowControl {
		public const string MNI_PREFIX = "mniScriptContext_";
		// http://stackoverflow.com/questions/8307959/toolstripmenuitem-for-multiple-contextmenustrip?rq=1
		// http://stackoverflow.com/questions/6275120/toolstripmenuitem-added-to-several-places?rq=1
		// WILL_ADD_PARENT_MENU_ITEMS_IN_Opening first time opened we locate common menu items from GrandParent, then we move them to the current slider; cool?
		Dictionary<string, ToolStripMenuItem> tsiScriptContextsDynamic = new Dictionary<string, ToolStripMenuItem>();
		public ToolStripItem[] TsiScriptContextsDynamic { get {
				List<string> ctx2remove = new List<string>();
				foreach (string ctxName in this.tsiScriptContextsDynamic.Keys) {
					if (this.Strategy.ScriptContextsByName.ContainsKey(ctxName)) continue;
					string msg = "removing mni<=>scriptContext[" + ctxName + "],"
						+ " most likely afterScriptParametersForm->RightClick->DELETE";
					//Assembler.PopupException(msg);
					ctx2remove.Add(ctxName);	// can't enumerate and remove!!! two-steps removal to avoid CollectionModified;
				}
				foreach (string ctxName in ctx2remove) {
					ToolStripMenuItem mni = this.tsiScriptContextsDynamic[ctxName];
					if (mni.IsDisposed) {
						string msg = "AVOIDING_NPE";
						continue;
					}
					mni.Click -= this.mniScriptContextLoad_Click;
					ToolStripMenuItem mniToRemove = mni as ToolStripMenuItem;
					if (mniToRemove != null) {
						mniToRemove.DropDownOpening -= this.mniScriptContext_DropDownOpening;
					}
					this.tsiScriptContextsDynamic.Remove(ctxName);	// two-steps removal to avoid CollectionModified;
				}

				//this.tsiScriptContextsDynamic.Clear();
				bool fromEmptyState = this.tsiScriptContextsDynamic.Keys.Count == 0;
				foreach (string ctxName in this.Strategy.ScriptContextsByName.Keys) {
					if (this.tsiScriptContextsDynamic.ContainsKey(ctxName)) continue;
					string msg = "adding new mni<=>scriptContext[" + ctxName  + "]";
					msg += fromEmptyState
						? "; most likely first time opened (from Empty State)"
						: "; most likely after ScriptParametersForm->RightClick->Duplicate/CreateNewDefault";
					//Assembler.PopupException(msg);
					ContextScript scriptCtx = this.Strategy.ScriptContextsByName[ctxName];
					ToolStripMenuItem mni = new ToolStripMenuItem();
					mni.Text = ctxName;
					mni.Name = MNI_PREFIX + ctxName;
					//CHECKING_CURRENT_JUST_BELOW mni.Checked = (this.Strategy.ScriptContextCurrent == scriptCtx);
					mni.Tag = scriptCtx;
					mni.DropDown = this.ctxOperations;	// just to make triangle appear on first load; replaced in DropDownOpening()
					mni.Click += new EventHandler(mniScriptContextLoad_Click);
					mni.DropDownOpening += new EventHandler(mniScriptContext_DropDownOpening);
					this.tsiScriptContextsDynamic.Add(ctxName, mni);
					//this.tsiScriptContextsDynamic.Insert(indexToInsert++, mni);
				}
				bool currentFound = false;
				foreach (string ctxName in this.tsiScriptContextsDynamic.Keys) {
					ToolStripMenuItem mni = this.tsiScriptContextsDynamic[ctxName];
					if (mni.IsDisposed) {
						string msg = "THIS_MNI_WILL_CAUSE_PROBLEMS_LATER";
						continue;
					}
					mni.Checked = (this.Strategy.ScriptContextCurrent.Name == ctxName);
					if (mni.Checked) currentFound = true;
				}
				if (!currentFound) {
					string msg = "this.Strategy.ScriptContextCurrent.Name[" + this.Strategy.ScriptContextCurrent.Name + "]"
						+ " not found among this.tsiScriptContextsDynamic.Keys.Count[" + this.tsiScriptContextsDynamic.Keys.Count + "]";
					Assembler.PopupException(msg);
				}
				foreach (ToolStripMenuItem mni in this.tsiScriptContextsDynamic.Values) {
					if (mni.IsDisposed) {
						#if DEBUG
						Debugger.Break();
						#endif
					}
				}

				return new List<ToolStripMenuItem>(this.tsiScriptContextsDynamic.Values).ToArray();
			} }
		public ToolStripItem[] TsiDynamic { get {
				var ret = new List<ToolStripItem>();

				#if DEBUG
				if (this.mniScriptContextsMniHeader.IsDisposed) {
					Debugger.Break();
					this.mniScriptContextsMniHeader = new Sq1.Widgets.LabeledTextBox.MenuItemLabel();
					// 
					// mniParameterBagsNotHighlighted
					// 
					this.mniScriptContextsMniHeader.BackColor = System.Drawing.Color.Transparent;
					this.mniScriptContextsMniHeader.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
					this.mniScriptContextsMniHeader.Name = "mniParameterBagsNotHighlighted";
					this.mniScriptContextsMniHeader.Size = new System.Drawing.Size(100, 23);
					this.mniScriptContextsMniHeader.Text = "Script Contexts";
				}
				#endif
				this.mniScriptContextsMniHeader.Text = "Script Contexts :: " + this.Strategy.WindowTitle;
				ret.Add(this.mniScriptContextsMniHeader);
				ret.Add(this.toolStripSeparator3);

				ret.AddRange(this.TsiScriptContextsDynamic);

				#if DEBUG
				if (this.mniltbScriptContextNewWithDefaults.IsDisposed) {
					Debugger.Break();
					this.mniltbScriptContextNewWithDefaults = new Sq1.Widgets.LabeledTextBox.MenuItemLabeledTextBox();
					// 
					// mniltbParametersBagNewWithDefaults
					// 
					this.mniltbScriptContextNewWithDefaults.BackColor = System.Drawing.Color.Transparent;
					this.mniltbScriptContextNewWithDefaults.InputFieldOffsetX = 80;
					this.mniltbScriptContextNewWithDefaults.InputFieldValue = "";
					this.mniltbScriptContextNewWithDefaults.InputFieldWidth = 85;
					this.mniltbScriptContextNewWithDefaults.Name = "mniltbParametersBagNewWithDefaults";
					this.mniltbScriptContextNewWithDefaults.Size = new System.Drawing.Size(168, 21);
					this.mniltbScriptContextNewWithDefaults.TextLeft = "New clean";
					this.mniltbScriptContextNewWithDefaults.TextRed = false;
					this.mniltbScriptContextNewWithDefaults.UserTyped += new System.EventHandler<Sq1.Widgets.LabeledTextBox.LabeledTextBoxUserTypedArgs>(this.mniltbScriptContextNewWithDefaults_UserTyped);
				}
				#endif

				if (this.Strategy.ScriptContextsByName.ContainsKey(ContextScript.DEFAULT_NAME)) {
					ContextScript ctx = this.Strategy.ScriptContextsByName[ContextScript.DEFAULT_NAME];
					this.mniltbScriptContextNewWithDefaults.TextRight = this.Strategy.ScriptContextCurrent.SpreadModelerPercent.ToString() + " %";
				} else {
					string msg = "CANT_GET_DEFAULT_SPREAD_PERCENTAGE_FOR_TsiDynamic"
						+ " DEVELOPER_NEVER_ALLOW_DEFAULT_CONTEXT_DELETION"
						+ " //this.Strategy.ScriptContextsByName.ContainsKey(" + ContextScript.DEFAULT_NAME + ")";
					Assembler.PopupException(msg);
				}

				ret.Add(this.mniltbScriptContextNewWithDefaults);

				if (this.toolStripSeparator2.IsDisposed) {
					#if DEBUG
					Debugger.Break();
					#endif
					this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
				}
				ret.Add(this.toolStripSeparator2);

				#if DEBUG
				if (this.mniAllParamsResetToScriptDefaults.IsDisposed) {
					Debugger.Break();
					this.mniAllParamsResetToScriptDefaults = new System.Windows.Forms.ToolStripMenuItem();
					// 
					// mniAllParamsResetToScriptDefaults
					// 
					this.mniAllParamsResetToScriptDefaults.Name = "mniAllParamsResetToScriptDefaults";
					this.mniAllParamsResetToScriptDefaults.Size = new System.Drawing.Size(273, 22);
					this.mniAllParamsResetToScriptDefaults.Text = "All Params -> Reset To Script Defaults";
					this.mniAllParamsResetToScriptDefaults.Click += new System.EventHandler(this.mniAllParamsResetToScriptDefaults_Click);
				}
				#endif
				ret.Add(this.mniAllParamsResetToScriptDefaults);

				#if DEBUG
				if (this.mniAllParamsShowNumeric.IsDisposed) {
					Debugger.Break();
					this.mniAllParamsShowNumeric = new System.Windows.Forms.ToolStripMenuItem();
					// 
					// mniAllParamsShowNumeric
					// 
					this.mniAllParamsShowNumeric.CheckOnClick = true;
					this.mniAllParamsShowNumeric.Name = "mniAllParamsShowNumeric";
					this.mniAllParamsShowNumeric.Size = new System.Drawing.Size(273, 22);
					this.mniAllParamsShowNumeric.Text = "All Params -> ShowNumeric";
					this.mniAllParamsShowNumeric.Click += new System.EventHandler(this.mniAllParamsShowNumeric_Click);
				}
				#endif
				ret.Add(this.mniAllParamsShowNumeric);

				#if DEBUG
				if (this.mniAllParamsShowBorder.IsDisposed) {
					Debugger.Break();
					this.mniAllParamsShowBorder = new System.Windows.Forms.ToolStripMenuItem();
					// 
					// mniAllParamsShowBorder
					// 
					this.mniAllParamsShowBorder.CheckOnClick = true;
					this.mniAllParamsShowBorder.Name = "mniAllParamsShowBorder";
					this.mniAllParamsShowBorder.Size = new System.Drawing.Size(273, 22);
					this.mniAllParamsShowBorder.Text = "All Params -> ShowBorder";
					this.mniAllParamsShowBorder.Click += new System.EventHandler(this.mniAllParamsShowBorder_Click);
				}
				#endif
				ret.Add(this.mniAllParamsShowBorder);

				return ret.ToArray();
			} }
		string stringEnteredInLabeledTextBox(string msig, object sender, KeyEventArgs e) {
			if (this.Strategy == null) {
				string msg = "you should keep your rightClickMenu disabled if current chart has no strategy";
				Assembler.PopupException(msig + msg);
				return null;
			}
			TextBox txt = sender as TextBox;
			if (txt == null) {
				string msg = "should be attached to a TextBox" + "; instead, I'm attached to sender[" + sender.GetType() + "]";
				Assembler.PopupException(msig + msg);
				return null;
			}
			LabeledTextBoxControl lblTxt = txt.Parent as LabeledTextBoxControl;
			if (lblTxt == null) {
				string msg = "TextBox's Parent should be a LabeledTextBox" + "; instead, sender[" + sender.GetType() + "].Parent[" + txt.Parent + "]";
				Assembler.PopupException(msig + msg);
				return null;
			}
			if (e.KeyCode != Keys.Enter) return null;
			string ret = txt.Text;
			ret = ret.Trim();
			ret = ret.Replace('\'', '~');	//  no single quotes against future SQL-injection: update name='456~~; DELETE *;' 
			ret = ret.Replace('"', '~');	//  no double quotes for current JSON consistency: "ScriptContextsByName": { "456~~": {} }
			//txt.Text = "";
			return ret;
		}
		ContextScript ScriptContextFromMniTag(object sender) {
			var mniOpening = sender as ToolStripItem;
			ContextScript ctxToLoad = mniOpening.Tag as ContextScript;
			if (ctxToLoad != null) {
				string msg2 = "USER_CLICKED_ON_SCRIPT_CONTEXT_ITEM_SHORTCUT attached in SlidersAutoGrowControl.TsiScriptContextsDynamic_get";
				return ctxToLoad;
			}
				
			string msg = "USER_CLICKED_ON_LOAD_SUBMENU_ITEM attached in SlidersAutoGrowControl.InitializeComponent()";
			ContextMenuStrip mniParent = mniOpening.Owner as ContextMenuStrip; 		// found in debugger
			ctxToLoad = mniParent.Tag as ContextScript;
			if (ctxToLoad == null) {
				msg = "YOU_SHOULDVE_STORED_SCRIPT_CONTEXT_IN mniOpening[" + mniOpening.Name + "].Tag[" + mniOpening.Tag + "] is not a ScriptContext";
				Assembler.PopupException(msg);
				return null;
			}
			
			return ctxToLoad;
		}
		ContextScript ScriptContextFromLabeledTextBoxTag(object sender) {
			// rightClick => [Default] => Duplicate To => onKeyPress()
			// TextBox <= LabeledTextBox[DuplicateTo] <= ParentControlHost.Owner[Default] <= Tag[ScriptContext"DEFAULT"]
//			TextBox txt = sender as TextBox;
//			LabeledTextBox lblTxt = txt.Parent as LabeledTextBox;
//			ToolStripControlHost mniOpening = lblTxt.ParentToolStripControlHost;

			MenuItemLabeledTextBox mniltbScriptContextRenameTo = sender as MenuItemLabeledTextBox;
			ContextMenuStrip mniParent = mniltbScriptContextRenameTo.Owner as ContextMenuStrip; 		// found in debugger
			ContextScript ctxToLoad = mniParent.Tag as ContextScript;
			if (ctxToLoad == null) {
				string msg = "mniltb[" + mniltbScriptContextRenameTo.Name + "].Tag[" + mniltbScriptContextRenameTo.Tag + "] is not a ScriptContext";
				Assembler.PopupException(msg);
				return null;
			}
			return ctxToLoad;
		}
	}
}