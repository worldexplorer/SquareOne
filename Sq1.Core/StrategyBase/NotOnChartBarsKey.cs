using System;

using Sq1.Core.DataTypes;

namespace Sq1.Core.StrategyBase {
	public class NotOnChartBarsKey {
		public string DataSourceName;
		public string Symbol;
		public BarScaleInterval BarScaleInterval;
		
		public NotOnChartBarsKey(BarScaleInterval notOnChartBarScaleInterval, string notOnChartSymbol, string notOnChartDataSourceName) {
			this.DataSourceName = notOnChartDataSourceName;
			this.Symbol = notOnChartSymbol;
			this.BarScaleInterval = notOnChartBarScaleInterval;
		}
		
		#region Equals and GetHashCode implementation
		public override bool Equals(object obj) {
			NotOnChartBarsKey other = obj as NotOnChartBarsKey;
			if (other == null) return false;
			return this.DataSourceName == other.DataSourceName && this.Symbol == other.Symbol && object.Equals(this.BarScaleInterval, other.BarScaleInterval);
		}
		public override int GetHashCode() {
			int hashCode = 0;
			unchecked {
				if (DataSourceName != null) 	hashCode += 1000000007 * DataSourceName.GetHashCode();
				if (Symbol != null)				hashCode += 1000000009 * Symbol.GetHashCode();
				if (BarScaleInterval != null)	hashCode += 1000000021 * BarScaleInterval.GetHashCode();
			}
			return hashCode;
		}
		
		public static bool operator ==(NotOnChartBarsKey lhs, NotOnChartBarsKey rhs) {
			if (ReferenceEquals(lhs, rhs)) return true;
			if (ReferenceEquals(lhs, null) || ReferenceEquals(rhs, null)) return false;
			return lhs.Equals(rhs);
		}
		
		public static bool operator !=(NotOnChartBarsKey lhs, NotOnChartBarsKey rhs) {
			return !(lhs == rhs);
		}
		#endregion
		
		public override string ToString() {
			return "BarScaleInterval[" + this.BarScaleInterval + "] Symbol[" + this.Symbol + "] DataSourceName=[" + DataSourceName + "]";
		}
	}
}
