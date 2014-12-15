using System;
using System.Collections.Generic;

using Sq1.Core.DataTypes;

namespace Sq1.Core.Execution {
	public class ReporterPokeUnit {
		public Quote			QuoteGeneratedThisUnit		{ get; protected set; }
		public List<Alert>		AlertsNew					{ get; protected set; }
		public List<Position>	PositionsOpened				{ get; protected set; }
		public List<Position>	PositionsClosed				{ get; protected set; }
		
		public int				PositionsCount				{ get { return this.PositionsOpened.Count + this.PositionsClosed.Count; } }
		public int				PositionsPlusAlertsCount	{ get { return this.PositionsCount + this.AlertsNew.Count; } }
		public List<Position> PositionsOpenedClosedMergedTogether { get {
				List<Position> ret = new List<Position>();
				ret.AddRange(this.PositionsClosed);
				ret.AddRange(this.PositionsOpened);
				return ret;
			} }
		public Dictionary<int, List<Position>> PositionsOpenedByBarFilled { get {
				Dictionary<int, List<Position>> ret = new Dictionary<int, List<Position>>();
				foreach (Position pos in this.PositionsOpened) {
					int barIndex = pos.EntryBar.ParentBarsIndex;
					//v2 int barIndex = pos.EntryFilledBarIndex;
					//v3 int barIndex = pos.Bars.FindBarIndexWithDateEqualOrLaterThan(pos.EntryDate);
					if (ret.ContainsKey(barIndex) == false) ret.Add(barIndex, new List<Position>());
					ret[barIndex].Add(pos);
				}
				return ret;
			} }
		public Dictionary<int, List<Position>> PositionsClosedByBarFilled { get {
				Dictionary<int, List<Position>> ret = new Dictionary<int, List<Position>>();
				foreach (Position pos in this.PositionsClosed) {
					int barIndex = pos.ExitBar.ParentBarsIndex;
					//v2 int barIndex = pos.ExitFilledBarIndex;
					//v3 int barIndex = pos.Bars.FindBarIndexWithDateEqualOrLaterThan(pos.Exitdate);
					if (ret.ContainsKey(barIndex) == false) ret.Add(barIndex, new List<Position>());
					ret[barIndex].Add(pos);
				}
				return ret;
			}
		}

		public ReporterPokeUnit() {
			AlertsNew		= new List<Alert>();
			PositionsOpened = new List<Position>();
			PositionsClosed = new List<Position>();
		}
		public ReporterPokeUnit(Quote quote, List<Alert> alertsNew, List<Position> positionsOpened, List<Position> positionsClosed) : this() {
			if (quote			!= null) QuoteGeneratedThisUnit	= quote;
			if (alertsNew		!= null) AlertsNew				= alertsNew;
			if (positionsOpened != null) PositionsOpened		= positionsOpened;
			if (positionsClosed != null) PositionsClosed		= positionsClosed;
		}
		public ReporterPokeUnit Clone() {
			ReporterPokeUnit ret = new ReporterPokeUnit(this.QuoteGeneratedThisUnit,
				new List<Alert>(this.AlertsNew),
				new List<Position>(this.PositionsOpened),
				new List<Position>(this.PositionsClosed));
			return ret;
		}
		public override string ToString() {
			return "AlertsNew[" + this.AlertsNew.Count + "] PositionsOpened[" + this.PositionsOpened.Count + "] PositionsClosed[" + this.PositionsClosed.Count + "]";
		}
	}
}