using System;

namespace Sq1.Core.Execution {
	public class ReporterPokeUnitEventArgs : EventArgs {
		public ReporterPokeUnit PokeUnit;

		public ReporterPokeUnitEventArgs(ReporterPokeUnit pokeUnit) {
			this.PokeUnit = pokeUnit;
		}
	}
}
