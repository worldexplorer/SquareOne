using System;

namespace Sq1.Core.Repositories {
	public class FnameDateSizeColorPFavg {
		const string pfMarkerLeft	= " PFAvg[";
		const string pfMarkerRight	= "]";

		public string	NameWithMarker		{ get; private set; }
		public DateTime	Date				{ get; private set; }
		public long		Size				{ get; private set; }

		public string	SymbolScaleRange	{ get; private set; }
		public double	PFavg				{ get; private set; }
		
		public string	DateSmart	{ get {
				if (this.Date.Date == DateTime.Now.Date) {
					TimeSpan timeOfDay = this.Date.TimeOfDay;
					//return this.Date.TimeOfDay.ToString("HH:mm");
					return timeOfDay.Hours.ToString("00") + ":" + timeOfDay.Minutes.ToString("00");
				}
				return this.Date.ToString("MMM-dd HH:mm");
			} }
		public string	SizeKb	{ get { return Math.Round(this.Size / (double)1024).ToString("N0") + " Kb"; } }
		public string	SizeMb	{ get { return Math.Round(this.Size / (double)(1024 * 1024), 2).ToString("N2") + " Mb"; } }
		
		public FnameDateSizeColorPFavg(string name, DateTime date, long size) {
			NameWithMarker = name;
			Date = date;
			Size = size;

			// "RIM3 1-Minute 500 Bars PFAvg[0.56].json"
			SymbolScaleRange	= NameWithMarker;
			PFavg				= 0;

			int pfMarkerStart = NameWithMarker.IndexOf(FnameDateSizeColorPFavg.pfMarkerLeft);
			if (pfMarkerStart != -1) {
				int pfEnd = NameWithMarker.IndexOf(pfMarkerRight, pfMarkerStart);
				if (pfEnd != -1) {
					int pfStart = pfMarkerStart + FnameDateSizeColorPFavg.pfMarkerLeft.Length;
					int pfWidth = pfEnd - pfStart;
					string pfAsString = NameWithMarker.Substring(pfStart, pfWidth);
					double pfParsed = 0;
					double.TryParse(pfAsString, out pfParsed);
					PFavg = pfParsed;
					SymbolScaleRange = NameWithMarker.Substring(0, pfMarkerStart);
				}
			}
		}
		public override string ToString() {
			return this.NameWithMarker;
		}
		public static string AppendProfitFactorAverage(string symbolScaleRange, double profitFactorAverage) {
			string ret = symbolScaleRange + pfMarkerLeft + profitFactorAverage + pfMarkerRight;
			return ret;
		}
	}
}
