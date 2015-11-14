using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Drawing;

using Sq1.Core;
using Sq1.Core.Broker;
using Sq1.Core.Support;
using Sq1.Core.Execution;
using Sq1.Core.StrategyBase;
using Sq1.Core.Backtesting;
using Sq1.Core.DataTypes;
using Sq1.Core.Livesim;
using Sq1.Core.DataFeed;

namespace Sq1.Adapters.QuikLivesim {
	[SkipInstantiationAt(Startup = false)]		// overriding LivesimBroker's TRUE to have QuikLivesimStreaming appear in DataSourceEditor
	public partial class QuikLivesimBroker : LivesimBroker {
				QuikLivesimBrokerSettings	settings { get { return base.livesimDataSource.Executor.Strategy.LivesimBrokerSettings as QuikLivesimBrokerSettings; } }
				object						threadEntryLockToHaveQuoteSentToThread;

		public QuikLivesimBroker() : base() {
			base.Name = "QuikLivesimBroker-DllFound";
			base.Icon = (Bitmap)Sq1.Adapters.QuikLivesim.Properties.Resources.imgQuikLivesimBroker;
		}

		public QuikLivesimBroker(LivesimDataSource livesimDataSource) : base(livesimDataSource) {
			base.Name = "QuikLivesimBroker";
		}

		
		public override BrokerEditor BrokerEditorInitialize(IDataSourceEditor dataSourceEditor) {
			base.BrokerEditorInitializeHelper(dataSourceEditor);
			base.brokerEditorInstance = new QuikLivesimBrokerEditor(this, dataSourceEditor);
			return base.brokerEditorInstance;
		}

		//public override void OrderSubmit(Order order) {
		//    this.OrdersSubmittedForOneLivesimBacktest.Add(order);
		//    string msg = "IS_ASYNC_CALLBACK_NEEDED? THREAD_ID " + Thread.CurrentThread.ManagedThreadId;
		//    base.OrderProcessor.UpdateOrderStateByGuidNoPostProcess(order.GUID, OrderState.Submitted, msg);
		//}

		//public override void OrderPendingKillWithoutKillerSubmit(Order orderPendingToKill) {
		//    string msig = " //LivesimBroker.OrderPendingKillWithoutKillerSubmit()";
			
		//    string orderGUID = orderPendingToKill.GUID;
		//    Order orderPendingFound = base.ScanEvidentLanesForGuidNullUnsafe(orderGUID);
		//    if (orderPendingFound != orderPendingToKill) {
		//        string msg = "PARANOID_SCAN_FAILED orderPendingFound[" + orderPendingFound + "] != orderPendingToKill[" + orderPendingToKill + "]";
		//        Assembler.PopupException(msg);
		//    }

		//    var omsg2 = new OrderStateMessage(orderPendingToKill, OrderState.KillPendingSubmitting, "Step#2");
		//    base.OrderProcessor.UpdateOrderStateDontPostProcess(orderPendingToKill, omsg2);

		//    var omsg3 = new OrderStateMessage(orderPendingToKill, OrderState.KillPendingSubmitted, "Step#3");
		//    base.OrderProcessor.UpdateOrderStateDontPostProcess(orderPendingToKill, omsg3);

		//    int delay = 0;
		//    if (this.settings.KillPendingDelayEnabled) {
		//        delay = settings.KillPendingDelayMillisMin;
		//        if (settings.KillPendingDelayMillisMax > 0) {
		//            int range = Math.Abs(settings.KillPendingDelayMillisMax - settings.KillPendingDelayMillisMin);
		//            double rnd0to1 = new Random().NextDouble();
		//            int rangePart = (int)Math.Round(range * rnd0to1);
		//            delay += rangePart;
		//        }
		//    }
		//    if (delay == 0) {
		//        var omsg = new OrderStateMessage(orderPendingToKill, OrderState.KilledPending, "DELAY_PENDING_KILL_ZERO");
		//        base.OrderProcessor.UpdateOrderStateDontPostProcess(orderPendingToKill, omsg);
		//        base.OrderProcessor.PostKillWithoutKiller_removeAlertsPendingFromExecutorDataSnapshot(orderPendingToKill, msig);
		//        return;
		//    }

		//    Task t = new Task(delegate() {
		//        try {
		//            Thread.CurrentThread.Name = "DELAYED_PENDING_KILL " + orderPendingToKill.ToString();
		//        } catch (Exception ex) {
		//            Assembler.PopupException("CANT_SET_THREAD_NAME //LivesimBroker", ex, false);
		//        }

		//        Thread.Sleep(delay);
		//        var omsg = new OrderStateMessage(orderPendingToKill, OrderState.KilledPending, "DELAY_PENDING_KILL[" + delay + "]ms");
		//        base.OrderProcessor.UpdateOrderStateDontPostProcess(orderPendingToKill, omsg);
		//        base.OrderProcessor.PostKillWithoutKiller_removeAlertsPendingFromExecutorDataSnapshot(orderPendingToKill, msig);
		//    });
		//    t.ContinueWith(delegate {
		//        string msg = "TASK_THREW";
		//        Assembler.PopupException(msg + msig, t.Exception);
		//    }, TaskContinuationOptions.OnlyOnFaulted);
		//    t.Start();
		//}

	}
}