using System;

using Sq1.Core.DataTypes;

namespace Sq1.Core.Execution {
	public class ReporterPokeUnit {
		public Quote			QuoteGeneratedThisUnit	{ get; protected set; }
		public AlertList		AlertsNew				{ get; protected set; }
		public PositionList		PositionsOpened			{ get; protected set; }
		public PositionList		PositionsClosed			{ get; protected set; }
		public PositionList		PositionsOpenNow		{ get; protected set; }
		
		public int				PositionsOpenedAfterExecPlusClosedCount							{ get { return this.PositionsOpened.Count + this.PositionsClosed.Count; } }
		public int				PositionsOpenedAfterExecPlusClosedPlusAlertsNewCount			{ get { return this.PositionsOpenedAfterExecPlusClosedCount + this.AlertsNew.Count; } }
		// lets Execute() return non-null PokeUnit => Reporters are notified on quoteUpdatedPositions if !GuiIsBusy
		public int				PositionsNowPlusOpenedPlusClosedAfterExecPlusAlertsNewCount		{ get { return this.PositionsOpenedAfterExecPlusClosedPlusAlertsNewCount + this.PositionsOpenNow.Count; } }
		
		public ReporterPokeUnit() {
			AlertsNew			= new AlertList("AlertsNew", null);
			PositionsOpened		= new PositionList("PositionsOpened", null);
			PositionsClosed		= new PositionList("PositionsClosed", null);
			PositionsOpenNow	= new PositionList("PositionsOpenNow", null);
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
	}
}