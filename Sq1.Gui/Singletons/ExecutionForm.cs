using System;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Core.Broker;
using Sq1.Core.Execution;

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
			//this.ExecutionTreeControl.InitializeWith_shadowTreeRebuilt(this.orderProcessor.DataSnapshot.OrdersRootOnly, this.orderProcessor);
			this.ExecutionTreeControl.InitializeWith_shadowTreeRebuilt(this.orderProcessor.DataSnapshot.OrdersViewProxy, this.orderProcessor);
			this.ExecutionTreeControl.PopulateMenuAccounts_fromBrokerAdapter(
				Assembler.InstanceInitialized.RepositoryJsonDataSources.CtxAccounts_allChecked_fromUnderlyingBrokerAdapters);

			this.ExecutionTreeControl.OnOrderDoubleClicked_OrderProcessorShouldKillOrder -= new EventHandler<OrderEventArgs>(this.orderProcessor.ExecutionTreeControl_OnOrderDoubleClicked_OrderProcessorShouldKillOrder);
			this.ExecutionTreeControl.OnOrderDoubleClicked_OrderProcessorShouldKillOrder += new EventHandler<OrderEventArgs>(this.orderProcessor.ExecutionTreeControl_OnOrderDoubleClicked_OrderProcessorShouldKillOrder);
		}
		
		public void PopulateWindowTitle() {
			this.ExecutionTreeControl.PopulateWindowTitle();
		}
	}
}
