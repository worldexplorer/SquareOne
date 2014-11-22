using System;
using System.Diagnostics;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Core.StrategyBase;

namespace Sq1.Widgets.StrategiesTree {
	public partial class StrategiesTreeControl {
		//http://stackoverflow.com/questions/261660/how-do-i-set-an-image-for-some-but-not-all-nodes-in-a-treeview?rq=1
		public event EventHandler<StrategyEventArgs> OnFolderSelected;
		public event EventHandler<StrategyEventArgs> OnStrategySelected;

		// ctxFolder
		public event EventHandler<StrategyEventArgs> OnStrategyCreated;
		public event EventHandler<StrategyEventArgs> OnFolderRenamed;
		public event EventHandler<StrategyEventArgs> OnFolderCreated;
		public event EventHandler<StrategyEventArgs> OnFolderDeleted;

		// ctxStrategy
		// event names ARE confusing: raised 1) as confirmation of the click or 2) after action has been performed?
		// TODO: rename to clearly show the point in time when the event is raised
		public event EventHandler<StrategyEventArgs> OnStrategyDoubleClicked;
		public event EventHandler<StrategyEventArgs> OnStrategyOpenDefaultClicked;
		public event EventHandler<StrategyEventArgs> OnStrategyOpenSavedClicked;
		public event EventHandler<StrategyEventArgs> OnStrategyMovedToAnotherFolderClicked;		// 2) fired after action has been performed (already moved)
		public event EventHandler<StrategyEventArgs> OnStrategyEditClicked;
		public event EventHandler<StrategyEventArgs> OnStrategyRenamed;
		public event EventHandler<StrategyEventArgs> OnStrategyDuplicated;
		public event EventHandler<StrategyEventArgs> OnStrategyDeleteClicked;


		void RaiseOnFolderSelected() {
			if (this.OnFolderSelected == null) return;
			this.OnFolderSelected(this, new StrategyEventArgs(this.FolderSelected, this.StrategySelected));
		}
		void RaiseOnStrategySelected() {
			if (this.OnStrategySelected == null) return;
			this.OnStrategySelected(this, new StrategyEventArgs(this.FolderSelected, this.StrategySelected));
		}
		
		void RaiseOnStrategyDoubleClicked() {
			if (this.OnStrategySelected == null) return;
			try {	// downstack backtest throwing won't crash Release (Debug will halt) 
				this.OnStrategyDoubleClicked(this, new StrategyEventArgs(this.FolderSelected, this.StrategySelected));
			} catch (Exception ex) {
				Assembler.PopupException(null, ex);
			}
		}		
		void RaiseOnStrategyCreated(string msig = null) {
			if (this.OnStrategyCreated == null) {
				string msg = "event OnStrategyCreated: no subscribers";
				Assembler.PopupException(msg + msig);
				return;
			}
			this.OnStrategyCreated(this, new StrategyEventArgs(this.FolderSelected, this.StrategySelected));
		}
		void RaiseOnFolderCreated(string msig = null) {
			if (this.OnFolderCreated == null) {
				string msg = "event OnFolderCreated: no subscribers";
				Assembler.PopupException(msg + msig);
				return;
			}
			this.OnFolderCreated(this, new StrategyEventArgs(this.FolderSelected, null));
		}
		void RaiseOnStrategyOpenClicked(string msig = null) {
			if (this.OnStrategyOpenDefaultClicked == null) {
				string msg = "event OnStrategyOpenClicked: no subscribers";
				Assembler.PopupException(msg + msig);
				return;
			}
			try {	// downstack backtest throwing won't crash Release (Debug will halt) 
				this.OnStrategyOpenDefaultClicked(this, new StrategyEventArgs(this.FolderSelected, this.StrategySelected));
			} catch (Exception ex) {
				Assembler.PopupException(null, ex);
			}
		}
		void RaiseOnStrategyOpenSavedClicked(string msig, ToolStripMenuItem mniClicked) {
			if (this.OnStrategyOpenSavedClicked == null) {
				string msg = "event OnStrategyOpenSavedClicked: no subscribers";
				Assembler.PopupException(msg + msig);
				return;
			}
			try {	// downstack backtest throwing won't crash Release (Debug will halt) 
				this.OnStrategyOpenSavedClicked(this, new StrategyEventArgs(this.FolderSelected, this.StrategySelected, mniClicked.Text));
			} catch (Exception ex) {
				Assembler.PopupException(null, ex);
			}
		}
		void RaiseOnStrategyMovedToAnotherFolderClicked(string msig, ToolStripMenuItem mniClicked) {
			if (this.OnStrategyMovedToAnotherFolderClicked == null) {
				string msg = "event OnStrategyMovedToAnotherFolderClicked: no subscribers";
				Assembler.PopupException(msg + msig);
				return;
			}
			string folderPriorToMove = this.FolderSelected;
			string folderMovedTo = mniClicked.Name;
			this.OnStrategyMovedToAnotherFolderClicked(this, new StrategyEventArgs(folderPriorToMove, this.StrategySelected, folderMovedTo));
		}
		void RaiseOnStrategyEditClicked(string msig) {
			if (this.OnStrategyEditClicked == null) {
				string msg = "event OnStrategyEditClicked: no subscribers";
				Assembler.PopupException(msg + msig);
				return;
			}
			this.OnStrategyEditClicked(this, new StrategyEventArgs(this.FolderSelected, this.StrategySelected));
		}
		void RaiseOnStrategyRenamed() {
			if (this.OnStrategyRenamed != null) return;
			this.OnStrategyRenamed(this, new StrategyEventArgs(this.FolderSelected, this.StrategySelected));
		}
		void RaiseOnStrategyDuplicated(string msig) {
			if (this.OnStrategyDuplicated == null) {
				string msg = "event OnStrategyDuplicated: no subscribers";
				Assembler.PopupException(msg + msig);
				return;
			}
			this.OnStrategyDuplicated(this, new StrategyEventArgs(this.FolderSelected, this.StrategySelected));
		}
		void RaiseOnStrategyDeleteClicked() {
			if (this.OnStrategyDeleteClicked == null) {
				string msg = "StrategiesTree.mniStrategyDelete_Click(): event OnStrategyDeleted: no subscribers";
				Assembler.PopupException(msg);
				return;
			}
			this.OnStrategyDeleteClicked(this, new StrategyEventArgs(this.FolderSelected, this.StrategySelected));
		}
	}
}