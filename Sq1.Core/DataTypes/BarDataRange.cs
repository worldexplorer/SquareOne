using System;

namespace Sq1.Core.DataTypes {
	public class BarDataRange {
		public BarRange Range;
		public DateTime DateFrom;
		public DateTime DateTill;
		public int RecentBars;
		public int RecentTimeunits;

		public BarDataRange() {
			Range = BarRange.AllData;
			DateFrom = DateTime.MinValue;
			DateTill = DateTime.MaxValue;
		}
		public BarDataRange(int recentBars) : this() {
			Range = BarRange.RecentBars;
			RecentBars = recentBars;
		}
		public BarDataRange(int recentTimeUnits, BarRange range) : this()  {
			Range = range;
			RecentTimeunits = recentTimeUnits;
		}
		public BarDataRange(DateTime startDate, DateTime endDate) : this()  {
			Range = BarRange.DateRange;
			DateFrom = startDate;
			DateTill = endDate;
		}

		public override string ToString() {
			string ret = "";
			switch (this.Range) {
				case BarRange.AllData:
					ret = "All Data";
					break;
				case BarRange.RecentBars:
					ret = this.RecentBars.ToString("N0") + " Bars";
					break;
				case BarRange.RecentYears:
					ret = this.RecentTimeunits.ToString("N0") + " Years";
					break;
				case BarRange.RecentMonths:
					ret = this.RecentTimeunits.ToString("N0") + " Months";
					break;
				case BarRange.RecentWeeks:
					ret = this.RecentTimeunits.ToString("N0") + " Weeks";
					break;
				case BarRange.RecentDays:
					ret = this.RecentTimeunits.ToString("N0") + " Days";
					break;
				case BarRange.DateRange:
					if (this.DateFrom.Year == this.DateTill.Year) {
						if (this.DateFrom.Month == this.DateTill.Month) {
							ret = this.DateFrom.ToString("dd");
						} else {
							ret = this.DateFrom.ToString("dd MMM");
						}
					} else {
						ret = this.DateFrom.ToString("dd MMM yyyy");
					}
					ret += " ... ";
					//if (this.IsStreaming) {
					//	ret += "NOW";
					//} else {
						ret += this.DateTill.ToString("dd MMM yyyy");
					//}
					break;
				default:
					break;
			}
			return ret;
		}

		public void FillStartEndDate(out DateTime startDate, out DateTime endDate) {
			startDate = DateTime.MinValue;
			endDate = DateTime.MaxValue;
			switch (this.Range) {
				case BarRange.RecentYears:
					startDate = DateTime.Now.Date.AddYears(-this.RecentTimeunits).AddDays(1.0);
					endDate = DateTime.MaxValue;
					break;
				case BarRange.RecentMonths:
					startDate = DateTime.Now.Date.AddMonths(-this.RecentTimeunits).AddDays(1.0);
					endDate = DateTime.MaxValue;
					break;
				case BarRange.RecentWeeks:
					startDate = DateTime.Now.Date.AddDays((double)(-(double)this.RecentTimeunits * 7)).AddDays(1.0);
					endDate = DateTime.MaxValue;
					break;
				case BarRange.RecentDays:
					startDate = DateTime.Now.Date.AddDays((double)(-(double)this.RecentTimeunits + 1));
					endDate = DateTime.MaxValue;
					break;
				case BarRange.DateRange:
					startDate = this.DateFrom;
					endDate = this.DateTill;
					break;
				case BarRange.RecentBars:
					startDate = DateTime.MinValue;
					endDate = DateTime.MaxValue;
					break;
				case BarRange.AllData:
					startDate = DateTime.MinValue;
					endDate = DateTime.MaxValue;
					break;
				default:
					throw new InvalidCastException();
			}
		}

		public BarDataRange Clone() {
			return (BarDataRange)this.MemberwiseClone();
		}
	}
}
