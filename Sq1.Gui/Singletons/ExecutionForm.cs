using System;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Core.Broker;


namespace Sq1.Gui.Singletons {
	public partial class ExecutionForm : DockContentSingleton<ExecutionForm> {
		OrderProcessor orderProcessor;
		bool isHiddenPrevState;

		public ExecutionForm() {
			this.InitializeComponent();
		}
		
		public void Initialize(OrderProcessor orderProcessor) {
			this.orderProcessor = orderProcessor;
			
			//this.executionTree.Initialize(this.orderProcessor.DataSnapshot.OrdersAll.SafeCopy);
			this.ExecutionTreeControl.InitializeWith_shadowTreeRebuilt(this.orderProcessor.DataSnapshot.OrdersAutoTree);
			this.ExecutionTreeControl.PopulateAccountsMenuFromBrokerAdapter(Assembler.InstanceInitialized.RepositoryJsonDataSources.CtxAccountsAllCheckedFromUnderlyingBrokerAdapters);			
		}
		
		[Obsolete("if the form is hidden mark it needs to be repopulated OnActivate() and do full TreeRebuild there")]
		bool IsHiddenHandlingRepopulate { get {
			if (this.isHiddenPrevState != base.IsHidden) {
				//if (base.IsHidden == false) this.executionTree.RebuildTree();
				if (base.IsHidden == false) this.ExecutionTreeControl.RebuildAllTreeFocusOnTopmost();
				this.isHiddenPrevState = base.IsHidden;
			}
			return base.IsHidden;
		} }

		public void PopulateWindowText() {
			if (base.InvokeRequired) {
				base.BeginInvoke(new MethodInvoker(this.PopulateWindowText));
				return;
			}
			string ret = "";
			int itemsCnt			= this.ExecutionTreeControl.OrdersTreeOLV.Items.Count;
			int allCnt				= this.orderProcessor.DataSnapshot.OrdersAll.Count;
			int submittingCnt		= this.orderProcessor.DataSnapshot.OrdersSubmitting.Count;
			int pendingCnt			= this.orderProcessor.DataSnapshot.OrdersPending.Count;
			int pendingFailedCnt	= this.orderProcessor.DataSnapshot.OrdersPendingFailed.Count;
			int cemeteryHealtyCnt	= this.orderProcessor.DataSnapshot.OrdersCemeteryHealthy.Count;
			int cemeterySickCnt		= this.orderProcessor.DataSnapshot.OrdersCemeterySick.Count;
			int fugitive			= allCnt - (submittingCnt + pendingCnt + pendingFailedCnt + cemeteryHealtyCnt + cemeterySickCnt);

										ret +=		   cemeteryHealtyCnt + " Filled/Killed/Killers";
										ret += " | " + pendingCnt + " Pending";
			if (submittingCnt > 0)		ret += " | " + submittingCnt + " Submitting";
			if (pendingFailedCnt > 0)	ret += " | " + pendingFailedCnt + " PendingFailed";
			if (cemeterySickCnt > 0)	ret += " | " + cemeterySickCnt + " DeadFromSickness";
										ret += " :: "+ allCnt + " Total";
			if (itemsCnt != allCnt)		ret += " | " + itemsCnt + " Displayed";
			if (fugitive > 0)			ret += ", " + fugitive + " DeserializedPrevLaunch";

			this.Text = "Execution :: " + ret;
		}

	}
}
