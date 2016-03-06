using System;

using Sq1.Core.DataTypes;

namespace Sq1.Core.Execution {
	public class ReporterPokeUnit : IDisposable {
		public Quote			QuoteGeneratedThisUnit	{ get; private set; }
		public AlertList		AlertsNew				{ get; private set; }
		public PositionList		PositionsOpened			{ get; private set; }
		public PositionList		PositionsClosed			{ get; private set; }
		public PositionList		PositionsOpenNow		{ get; private set; }
		
		public int				Positions_openedAfterExec_plusClosed_count							{ get { return this.PositionsOpened.Count + this.PositionsClosed.Count; } }
		public int				Positions_openedAfterExec_plusClosed_plusAlertsNew_count			{ get { return this.Positions_openedAfterExec_plusClosed_count + this.AlertsNew.Count; } }
		// lets Execute() return non-null PokeUnit => Reporters are notified on quoteUpdatedPositions if !GuiIsBusy
		public int				PositionsNow_plusOpened_plusClosedAfterExec_plusAlertsNew_count		{ get { return this.Positions_openedAfterExec_plusClosed_plusAlertsNew_count + this.PositionsOpenNow.Count; } }
		
		public ReporterPokeUnit() {
			AlertsNew			= new AlertList			("AlertsNew_myOwn_canDispose", null);
			PositionsOpened		= new PositionList("PositionsOpened_myOwn_canDispose", null);
			PositionsClosed		= new PositionList("PositionsClosed_myOwn_canDispose", null);
			PositionsOpenNow	= new PositionList("PositionsOpenNow_myOwn_canDispose", null);
		}
		public ReporterPokeUnit(Quote quote, AlertList alertsNew, PositionList positionsOpened, PositionList positionsClosed, PositionList positionsOpenNow = null) : this() {
			if (quote				!= null) QuoteGeneratedThisUnit	= quote;
			if (alertsNew			!= null) AlertsNew				= alertsNew;
			if (positionsOpened 	!= null) PositionsOpened		= positionsOpened;
			if (positionsClosed		!= null) PositionsClosed		= positionsClosed;
			if (positionsOpenNow	!= null) PositionsOpenNow		= positionsOpenNow;
		}
		public ReporterPokeUnit Clone() {
			ReporterPokeUnit ret = new ReporterPokeUnit(this.QuoteGeneratedThisUnit,
				this.AlertsNew.Clone(this, "ReporterPokeUnit.Clone"),
				this.PositionsOpened.Clone(this, "ReporterPokeUnit.Clone"),
				this.PositionsClosed.Clone(this, "ReporterPokeUnit.Clone"),
				this.PositionsOpenNow.Clone(this, "ReporterPokeUnit.Clone")
			);
			return ret;
		}
		public override string ToString() {
			return "AlertsNew[" + this.AlertsNew.Count + "]"
				+ " PositionsOpened[" + this.PositionsOpened.Count + "]"
				+ " PositionsClosed[" + this.PositionsClosed.Count + "]"
				+ " PositionsOpenNow[" + this.PositionsOpenNow.Count + "]";
		}

		#region IDisposable Members
		public void Dispose() {					// VS2010_DOES_NOT_DROP_IT_DOWN_IF_NOT_IMPLICITLY_PUBLIC void IDisposable.Dispose() {
			//return;		// when I continue, closePositionsLeftOpenAfterBacktest() throws at the end of Backtest
			if (this.DisposedWasAlreadyInvoked) {
				string msg = "DONT_DISPOSE_THE_KITTEN_TWICE //ReporterPokeUnit().Dispose()";
				Assembler.PopupException(msg);
				return;
			}
			this.DisposedWasAlreadyInvoked			= true;
			if (this.AlertsNew			.ReasonToExist.Contains("_myOwn_canDispose"))		this.AlertsNew			.Dispose();
		    if (this.PositionsOpened	.ReasonToExist.Contains("_myOwn_canDispose"))		this.PositionsOpened	.Dispose();
		    if (this.PositionsClosed	.ReasonToExist.Contains("_myOwn_canDispose"))		this.PositionsClosed	.Dispose();
		    if (this.PositionsOpenNow	.ReasonToExist.Contains("_myOwn_canDispose"))		this.PositionsOpenNow	.Dispose();
		}
		public bool DisposedWasAlreadyInvoked { get; private set; }
		#endregion
	}
}