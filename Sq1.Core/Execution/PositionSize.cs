using System;

namespace Sq1.Core.Execution {
	public class PositionSize {
		public PositionSizeMode Mode;
		public double DollarsConstantEachTrade;
		public double SharesConstantEachTrade;
		
		[Obsolete]
		private PositionSize() {
			Mode = PositionSizeMode.Unknown;
		}
		public PositionSize(PositionSizeMode mode, double value) : this() {
			Mode = mode;
			switch (Mode) {
				case PositionSizeMode.DollarsConstantForEachTrade:
					DollarsConstantEachTrade = value;
					break;
				case PositionSizeMode.SharesConstantEachTrade:
					SharesConstantEachTrade = value;
					break;
				default:
					string msg = "PositionSize: no handler for Mode[" + this.Mode + "]";
					throw new Exception(msg);
			}
		}
		
		public override string ToString() {
			switch (this.Mode) {
				case PositionSizeMode.Unknown:
					return "UNKNOWN_POSITION_SIZE";
				case PositionSizeMode.DollarsConstantForEachTrade:
					return this.DollarsConstantEachTrade.ToString("C0");
				case PositionSizeMode.SharesConstantEachTrade:
					return this.SharesConstantEachTrade + " shares";
				default:
					string msg = "PositionSize: no handler for Mode[" + this.Mode + "]";
					throw new Exception(msg);
			}
		}
		public PositionSize Clone() {
			return (PositionSize)this.MemberwiseClone();
		}
	}
}
