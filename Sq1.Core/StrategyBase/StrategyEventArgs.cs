using System;

namespace Sq1.Core.StrategyBase {
	public class StrategyEventArgs : EventArgs {
		public Strategy Strategy;
		public string Folder;
		public string scriptContextName;

		public StrategyEventArgs(string folder, Strategy strategy) {
			this.Folder = folder;
			this.Strategy = strategy;
		}

		public StrategyEventArgs(string folder, StrategyBase.Strategy strategy, string scriptContextName)
			: this(folder, strategy) {
			this.scriptContextName = scriptContextName;
		}
	}
}