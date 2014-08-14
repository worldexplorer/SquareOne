using System;
using System.Collections.Generic;
using Sq1.Core.DataTypes;

namespace Sq1.Core.Execution {
	public class ReporterPokeUnit {
		public Quote QuoteGeneratedThisUnit;
		public List<Position> PositionsOpened;
		public List<Position> PositionsClosed;
		public List<Position> PositionsOpenedClosedMergedTogether { get {
				List<Position> ret = new List<Position>();
				ret.AddRange(this.PositionsClosed);
				ret.AddRange(this.PositionsOpened);
				return ret;
			} }
		public int PositionsChanged { get { return this.PositionsOpened.Count + this.PositionsClosed.Count; } }
		public List<Alert> AlertsNew;

		public ReporterPokeUnit(Quote quote) {
			QuoteGeneratedThisUnit = quote;
			AlertsNew = new List<Alert>();
			PositionsOpened = new List<Position>();
			PositionsClosed = new List<Position>();
		}
		public ReporterPokeUnit Clone() {
			ReporterPokeUnit ret = new ReporterPokeUnit(this.QuoteGeneratedThisUnit);
			ret.AlertsNew = new List<Alert>(this.AlertsNew);
			ret.PositionsOpened = new List<Position>(this.PositionsOpened);
			ret.PositionsClosed = new List<Position>(this.PositionsClosed);
			return ret;
		}
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
		public Dictionary<int, List<Position>> PositionsClosedByBarFilled {
			get {
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
	}
}