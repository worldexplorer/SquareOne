using System;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Widgets;

namespace Sq1.Gui.Forms {
	public partial class LivesimForm : DockContentImproved {
		ChartFormManager chartFormManager;
		
		// INVOKED_BY_DOCKCONTENT.DESERIALIZE_FROM_XML
		public LivesimForm() {
			InitializeComponent();
		}
		
		// INVOKED_AT_USER_CLICK
		public LivesimForm(ChartFormManager chartFormsManager) : this() {
			this.Initialize(chartFormsManager);
			//ERASES_LINE_IN_DOCK_CONTENT_XML_IF_WITHOUT_IGNORING this.Disposed += this.LivesimForm_Disposed;
			this.FormClosing							+= new FormClosingEventHandler(this.livesimForm_FormClosing);
			this.FormClosed								+= new  FormClosedEventHandler(this.livesimForm_FormClosed);
			//v1
			//this.LivesimControl.BtnStartStop.Click		+= new EventHandler(this.btnStartStop_Click);
			//this.LivesimControl.BtnPauseResume.Click	+= new EventHandler(this.btnPauseResume_Click);
			//v2
			this.LivesimControl.TssBtnStartStop.Click	+= new EventHandler(this.btnStartStop_Click);
			this.LivesimControl.TssBtnPauseResume.Click	+= new EventHandler(this.btnPauseResume_Click);
		}

		// http://www.codeproject.com/Articles/525541/Decoupling-Content-From-Container-in-Weifen-Luos
		// using ":" since "=" leads to an exception in DockPanelPersistor.cs
		protected override string GetPersistString() {
			return "LiveSim:" + this.LivesimControl.GetType().FullName + ",ChartSerno:" + this.chartFormManager.DataSnapshot.ChartSerno;
		}

		// INVOKED_AFTER_DOCKCONTENT.DESERIALIZE_FROM_XML
		internal void Initialize(ChartFormManager chartFormManager) {
			this.chartFormManager = chartFormManager;
			this.WindowTitlePullFromStrategy();
			//this.liveSimControl.Initialize(this.chartFormManager.Executor.Sequencer);
			//this.LivesimControl.LblStrategyAsString.Text = this.chartFormsManager.Executor.ToStringWithCurrentParameters();
			this.LivesimControl.TssLblStrategyAsString.Text = this.chartFormManager.Executor.ToStringWithCurrentParameters();
			
			try {
				//LivesimDataSource livesimDS = this.chartFormManager.Executor.Livesimulator.DataSourceAsLivesim_nullUnsafe;
				//LivesimStreaming livesimStreaming = livesimDS.StreamingAsLivesim_nullUnsafe;
				//v1: TOO_WIDE_UNTIL_StreamingAsLivesim_nullUnsafe_ WE_ARE_GOING_TO_BE_NOTIFIED_ABOUT_ANY_SYMBOL_RECEIVED_BY_STREAMING_ADAPDER
				//livesimStreaming.QuotePushedToAllDistributionChannels += new EventHandler<QuoteEventArgs>(this.livesimulator_QuotePushedToDistributor);
				//v2
				//Bars bars = this.chartFormManager.Executor.Bars;
				//SymbolScaleDistributionChannel channel = livesimStreaming.Distributor.GetDistributionChannelFor(bars.Symbol, bars.ScaleInterval);
				//channel.QuoteSyncPushedToAllConsumers += new EventHandler<QuoteEventArgs>(this.livesimulator_QuotePushedToDistributor);
				//v3
				// ALREADY_HANDLED_BY_chartControl_BarAddedUpdated_ShouldTriggerRepaint
				//this.chartFormManager.Executor.EventGenerator.OnStrategyExecutedOneQuoteOrBarOrdersEmitted +=
				//	new EventHandler<EventArgs>(this.livesimForm_StrategyExecutedOneQuoteOrBarOrdersEmitted);

				//DataSourceAsLivesim_nullUnsafe_IS_NULL_HERE this.chartFormManager.Executor.Livesimulator.DataSourceAsLivesim_nullUnsafe.StreamingAsLivesim_nullUnsafe.Initialize(this.chartFormManager.ChartForm.ChartControl);
				this.LivesimControl.StreamingLivesimEditor.Initialize(this.chartFormManager.Strategy.LivesimStreamingSettings);
				this.LivesimControl.   BrokerLivesimEditor.Initialize(this.chartFormManager.Strategy.LivesimBrokerSettings);
			} catch (Exception ex) {
				string msg = "SO_MANY_NULL_UNSAFES_RIGHT?...";
				Assembler.PopupException(msg, ex);
			}
		}

		public void WindowTitlePullFromStrategy() {
			string windowTitle = "LiveSim :: " + this.chartFormManager.WhoImServing_moveMeToExecutor;
			//if (this.chartFormsManager.Strategy.ActivatedFromDll == true) windowTitle += "-DLL";
			//if (this.chartFormsManager.ScriptEditedNeedsSaving) {
			//	windowTitle = ChartFormsManager.PREFIX_FOR_UNSAVED_STRATEGY_SOURCE_CODE + windowTitle;
			//}
			this.Text = windowTitle;
		}
	}
}
