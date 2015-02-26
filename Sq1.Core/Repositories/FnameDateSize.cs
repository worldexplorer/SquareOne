using System;

namespace Sq1.Core.Repositories {
	public class FnameDateSize {
		public string	Name	{ get; private set; }
		public DateTime	Date	{ get; private set; }
		public long		Size	{ get; private set; }
		
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
		
		public FnameDateSize(string name, DateTime date, long size) {
			Name = name;
			Date = date;
			Size = size;
		}

		public override string ToString() {
			return this.Name;
		}
	}
}
