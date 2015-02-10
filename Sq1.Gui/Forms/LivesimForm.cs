using System;

using Sq1.Core;
using Sq1.Core.DataTypes;
using Sq1.Core.Livesim;
using Sq1.Core.Streaming;
using Sq1.Widgets;

namespace Sq1.Gui.Forms {
	public partial class LivesimForm : DockContentImproved {
		ChartFormManager chartFormManager;
		
		public LivesimForm() {
			InitializeComponent();
		}
		
		public LivesimForm(ChartFormManager chartFormsManager) : this() {
			this.Initialize(chartFormsManager);
			this.Disposed += this.LivesimForm_Disposed;
			this.LivesimControl.BtnStartStop.Click += new EventHandler(this.btnStartStop_Click);
			this.LivesimControl.BtnPauseResume.Click += new EventHandler(this.btnPauseResume_Click);
		}
		
		// http://www.codeproject.com/Articles/525541/Decoupling-Content-From-Container-in-Weifen-Luos
		// using ":" since "=" leads to an exception in DockPanelPersistor.cs
		protected override string GetPersistString() {
			return "LiveSim:" + this.LivesimControl.GetType().FullName + ",ChartSerno:" + this.chartFormManager.DataSnapshot.ChartSerno;
		}

		internal void Initialize(ChartFormManager chartFormManager) {
			this.chartFormManager = chartFormManager;
			this.Text = "LiveSim :: " + this.chartFormManager.Strategy.Name;
			//this.liveSimControl.Initialize(this.chartFormManager.Executor.Optimizer);
			this.LivesimControl.LblStrategyAsString.Text = this.chartFormManager.Executor.ToStringWithCurrentParameters();
			
			try {
				//LivesimDataSource livesimDS = this.chartFormManager.Executor.Livesimulator.DataSourceAsLivesimNullUnsafe;
				//LivesimStreaming livesimStreaming = livesimDS.StreamingAsLivesimNullUnsafe;
				//v1: TOO_WIDE_UNTIL_StreamingAsLivesimNullUnsafe_ WE_ARE_GOING_TO_BE_NOTIFIED_ABOUT_ANY_SYMBOL_RECEIVED_BY_STREAMING_ADAPDER
				//livesimStreaming.QuotePushedToAllDistributionChannels += new EventHandler<QuoteEventArgs>(this.livesimulator_QuotePushedToDistributor);
				//v2
				//Bars bars = this.chartFormManager.Executor.Bars;
				//SymbolScaleDistributionChannel channel = livesimStreaming.DataDistributor.GetDistributionChannelFor(bars.Symbol, bars.ScaleInterval);
				//channel.QuoteSyncPushedToAllConsumers += new EventHandler<QuoteEventArgs>(this.livesimulator_QuotePushedToDistributor);
				//v3
				this.chartFormManager.Executor.StrategyExecutionComplete += new EventHandler<QuoteEventArgs>(this.livesimForm_StrategyExecutionComplete);
				this.chartFormManager.Executor.Livesimulator.DataSourceAsLivesimNullUnsafe.StreamingAsLivesimNullUnsafe.Initialize(this.chartFormManager.ChartForm.ChartControl);
				this.LivesimControl.StreamingLivesimEditor.Initialize(this.chartFormManager.Strategy.LivesimStreamginSettings);
				this.LivesimControl.BrokerLivesimEditor.Initialize(this.chartFormManager.Strategy.LivesimBrokerSettings);
			} catch (Exception ex) {
				string msg = "SO_MANY_NULL_UNSAFES_RIGHT?...";
				Assembler.PopupException(msg, ex);
			}
		}
	}
}
