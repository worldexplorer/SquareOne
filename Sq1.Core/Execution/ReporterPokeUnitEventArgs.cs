using System;

namespace Sq1.Core.Execution {
	public class ReporterPokeUnitEventArgs : EventArgs {
		public ReporterPokeUnit PokeUnit;

		public ReporterPokeUnitEventArgs(ReporterPokeUnit pokeUnit) {
			// TODO: Complete member initialization
			this.PokeUnit = pokeUnit;
		}
	}
}
