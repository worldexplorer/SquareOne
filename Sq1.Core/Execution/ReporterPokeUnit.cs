using System;
using System.Collections.Generic;

using Sq1.Core.DataTypes;

namespace Sq1.Core.Execution {
	public class ReporterPokeUnit {
		public Quote			QuoteGeneratedThisUnit	{ get; protected set; }
		public AlertList		AlertsNew				{ get; protected set; }
		public PositionList		PositionsOpened			{ get; protected set; }
		public PositionList		PositionsClosed			{ get; protected set; }
		public PositionList		PositionsOpenNow		{ get; protected set; }
		
		public int				PositionsCount				{ get { return this.PositionsOpened.Count + this.PositionsClosed.Count; } }
		public int				PositionsPlusAlertsCount	{ get { return this.PositionsCount + this.AlertsNew.Count; } }
		
		public ReporterPokeUnit() {
			AlertsNew			= new AlertList("AlertsNew");
			PositionsOpened		= new PositionList("PositionsOpened");
			PositionsClosed		= new PositionList("PositionsClosed");
			PositionsOpenNow	= new PositionList("PositionsOpenNow");
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
				this.AlertsNew.Clone(),
				this.PositionsOpened.Clone(),
				this.PositionsClosed.Clone(),
				this.PositionsOpenNow.Clone()
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