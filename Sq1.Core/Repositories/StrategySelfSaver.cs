using Newtonsoft.Json;
using Sq1.Core.StrategyBase;

namespace Sq1.Core.Repositories {
	public class StrategySelfSaver {
		[JsonIgnore]			Strategy strategy;

		public void Initialize(Strategy strategy) {
			this.strategy = strategy;
		}
		public void SaveStrategy() {
			Assembler.InstanceInitialized.RepositoryDllJsonStrategy.StrategySave(this.strategy);
		}
	}
}
