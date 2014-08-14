using System;
using System.Collections.Generic;
using System.Drawing;

using Sq1.Core.DataTypes;
using Sq1.Core.Execution;

namespace Sq1.Core.Charting {
	public class ScriptToChartCommunicator {
		object lockPositionsMaster;
		object lockPositionsOpened;
		object lockPositionsClosed;
		
		public List<Position> PositionsMasterCloneCanBeZeriofiedAfterPickup { get; private set; }
		public List<Position> PositionsOpenedCloneCanBeZeriofiedAfterPickup { get; private set; }
		public List<Position> PositionsClosedCloneCanBeZeriofiedAfterPickup { get; private set; }
		
		public ScriptToChartCommunicator() {
			object lockPositionsMaster = new object();
			object lockPositionsOpened = new object();
			object lockPositionsClosed = new object();
		}
		public void PositionsBacktestClearAfterChartPickedUp() {
			this.PositionsRealtimeClearAfterChartPickedUp();
			lock (lockPositionsMaster) {
				this.PositionsMasterCloneCanBeZeriofiedAfterPickup.Clear();
			}
		}
		public void AddPositionsBacktest(List<Position> positionsMaster) {
			if (positionsMaster == null) return;
			if (positionsMaster.Count == 0) return;
			lock (lockPositionsMaster) {
				if (this.PositionsMasterCloneCanBeZeriofiedAfterPickup == null) this.PositionsMasterCloneCanBeZeriofiedAfterPickup = new List<Position>();
				foreach (Position eachMaster in positionsMaster) {
					if (this.PositionsMasterCloneCanBeZeriofiedAfterPickup.Contains(eachMaster)) continue;
					this.PositionsMasterCloneCanBeZeriofiedAfterPickup.Add(eachMaster);
				}
			}
		}
		public void PositionsRealtimeClearAfterChartPickedUp() {
			lock (lockPositionsOpened) {
				this.PositionsOpenedCloneCanBeZeriofiedAfterPickup = null;
			}
			lock (lockPositionsClosed) {
				this.PositionsClosedCloneCanBeZeriofiedAfterPickup = null;
			}
		}
		public void PositionsRealtimeAdd(ReporterPokeUnit pokeUnit) {
			if (pokeUnit.PositionsOpened != null && pokeUnit.PositionsOpened.Count > 0) {
				lock (lockPositionsOpened) {
					if (this.PositionsOpenedCloneCanBeZeriofiedAfterPickup == null) this.PositionsOpenedCloneCanBeZeriofiedAfterPickup = new List<Position>();
					foreach (Position eachNewOpened in pokeUnit.PositionsOpened) {
						if (this.PositionsOpenedCloneCanBeZeriofiedAfterPickup.Contains(eachNewOpened)) continue;
						this.PositionsOpenedCloneCanBeZeriofiedAfterPickup.Add(eachNewOpened);
					}
				}
			}
			if (pokeUnit.PositionsClosed != null && pokeUnit.PositionsClosed.Count > 0) {
				lock (lockPositionsClosed) {
					if (this.PositionsClosedCloneCanBeZeriofiedAfterPickup == null) this.PositionsClosedCloneCanBeZeriofiedAfterPickup = new List<Position>();
					foreach (Position eachNewClosed in pokeUnit.PositionsClosed) {
						if (this.PositionsClosedCloneCanBeZeriofiedAfterPickup.Contains(eachNewClosed)) continue;
						this.PositionsClosedCloneCanBeZeriofiedAfterPickup.Add(eachNewClosed);
					}
				}
			}
		}
	}
}
