using System;
using System.Collections.Generic;
using System.IO;

using CsvHelper;
using CsvHelper.Configuration;
using Sq1.Core.DataTypes;
using Sq1.Core.Serializers;

namespace Sq1.Core.Repositories {
	public class RepositoryCustomMarketInfo : Serializer<Dictionary<string, MarketInfo>> {
		public static string MarketDefaultName = "FORTS";
		private const string fnameTimeZonesCsv = "MicrosoftTimeZones.csv";
		public string AbsFilenameTimeZonesCsv { get { return base.RootPath + Path.DirectorySeparatorChar + fnameTimeZonesCsv; } }
		public Dictionary<string, string> TimeZonesWithUTC { get; protected set; }

		public RepositoryCustomMarketInfo() : base() {}
		public new bool Initialize(string rootPath, string relFname,
		            string subfolder = "Workspaces", string workspaceName = "Default",
					bool createNonExistingPath = true, bool createNonExistingFile = true) {
			bool createdNewFile = base.Initialize(rootPath, relFname, subfolder, workspaceName, createNonExistingPath, createNonExistingFile);
			this.TimeZonesWithUTC = parseCsv(this.AbsFilenameTimeZonesCsv, ",");
			return createdNewFile;
		}
		public new void Serialize() {
			base.Serialize();
			this.trimHolidaysToYMD();
			string fixes = this.adjustIfClearingTimespanOutsideOpenClose();
			fixes += this.adjustIfClearingTimespansOverlap();
			if (string.IsNullOrEmpty(fixes) == false) {
				throw new Exception("ClearingTimespans fixes: ");
			}
		}
		public new void Deserialize() {
			base.Deserialize();
			if (base.Entity.Count == 0) base.Entity = this.MarketsDefault;
			this.trimHolidaysToYMD();
			this.adjustIfClearingTimespanOutsideOpenClose();
		}
		void trimHolidaysToYMD() {
			foreach (MarketInfo market in base.Entity.Values) {
				if (market.HolidaysYMD000 == null) continue;
				if (market.HolidaysYMD000.Count == 0) continue;
				List<DateTime> holidaysTrimmed = new List<DateTime>();
				foreach (DateTime holidayToTrim in market.HolidaysYMD000) {
					holidaysTrimmed.Add(holidayToTrim.Date);
				}
				market.HolidaysYMD000 = holidaysTrimmed;
			}
		}
		string adjustIfClearingTimespanOutsideOpenClose() {
			string ret = "";
			foreach (MarketInfo market in base.Entity.Values) {
				if (market.ClearingTimespans == null) continue;
				if (market.ClearingTimespans.Count == 0) continue;
				List<DateTime> holidaysTrimmed = new List<DateTime>();
				foreach (MarketClearingTimespan clearing in market.ClearingTimespans) {
					if (clearing.SuspendServerTimeOfDay.TimeOfDay < market.MarketOpenServerTime.TimeOfDay) {
						ret += " adjusting: clearing[" + clearing + "].SuspendServerTimeOfDay.TimeOfDay["
							+ clearing.SuspendServerTimeOfDay.TimeOfDay + "] < market.MarketOpenServerTime.TimeOfDay["
							+ market.MarketOpenServerTime.TimeOfDay + "]";
						clearing.SuspendServerTimeOfDay = market.MarketOpenServerTime;
					}
					if (clearing.ResumeServerTimeOfDay.TimeOfDay < market.MarketOpenServerTime.TimeOfDay) {
						ret += " adjusting: clearing[" + clearing + "].ResumeServerTimeOfDay.TimeOfDay["
							+ clearing.ResumeServerTimeOfDay.TimeOfDay + "] < market.MarketOpenServerTime.TimeOfDay["
							+ market.MarketOpenServerTime.TimeOfDay + "]";
						clearing.ResumeServerTimeOfDay = market.MarketOpenServerTime;
					}
					if (clearing.SuspendServerTimeOfDay.TimeOfDay > market.MarketCloseServerTime.TimeOfDay) {
						ret += " adjusting: clearing[" + clearing + "].SuspendServerTimeOfDay.TimeOfDay["
							+ clearing.SuspendServerTimeOfDay.TimeOfDay + "] > market.MarketCloseServerTime.TimeOfDay["
							+ market.MarketCloseServerTime.TimeOfDay + "]";
						clearing.SuspendServerTimeOfDay = market.MarketCloseServerTime;
					}
					if (clearing.ResumeServerTimeOfDay.TimeOfDay > market.MarketCloseServerTime.TimeOfDay) {
						ret += " adjusting: clearing[" + clearing + "].ResumeServerTimeOfDay.TimeOfDay["
							+ clearing.ResumeServerTimeOfDay.TimeOfDay + "] > market.MarketCloseServerTime.TimeOfDay["
							+ market.MarketCloseServerTime.TimeOfDay + "]";
						clearing.ResumeServerTimeOfDay = market.MarketCloseServerTime;
					}
				}
			}
			return ret;
		}
		string adjustIfClearingTimespansOverlap() {
			return "";
		}
		public MarketInfo FindMarketInfo(string marketName) {
			if (string.IsNullOrEmpty(marketName)) return null;
			foreach (string market in Entity.Keys) {
				if (market.ToUpper() == marketName.ToUpper()) return Entity[market];
			}
			return null;
		}
		public MarketInfo FindMarketInfoOrNew(string marketName) {
			MarketInfo ret = this.FindMarketInfo(marketName);
			if (ret == null) ret = new MarketInfo();
			return ret;
		}	
		public Dictionary<string, MarketInfo> MarketsDefault {
			get {
				Dictionary<string, MarketInfo> ret = new Dictionary<string, MarketInfo>();
				MarketInfo mockMarket = new MarketInfo() {
					Name = "MOCK",
					Description = "23:59 hours open",
					MarketOpenServerTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0),// 10, 30, 0);
					MarketCloseServerTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59)
					,TimeZoneName = "Russian Standard Time"
					//,TimeZoneInfo = TimeZoneInfo.CreateCustomTimeZone("MSK_CUSTOM", new TimeSpan(4, 0, 0),
					//	"Moscow Daylight (UTC+04:00)", "Moscow Standard non-auto-adjusting-for-winter")
				};
				mockMarket.ClearingTimespans.Add(new MarketClearingTimespan() {
					SuspendServerTimeOfDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 14, 00, 0),
					ResumeServerTimeOfDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 14, 03, 0)
				});
				ret.Add(mockMarket.Name, mockMarket);
				MarketInfo russianMarket = new MarketInfo() {
					Name = MarketDefaultName,
					Description = "RTS Index Futures and Options Market (RIM3)",
					MarketOpenServerTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 10, 0, 0),
					MarketCloseServerTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 50, 0)
					,TimeZoneName = "Russian Standard Time"
					//,TimeZoneInfo = TimeZoneInfo.CreateCustomTimeZone("MSK_CUSTOM", new TimeSpan(4, 0, 0),
					//	"Moscow Daylight (UTC+04:00)", "Moscow Standard non-auto-adjusting-for-winter")
				};
				// 10.00 - 14.00	Начало основной торговой сессии
				// 14.00 - 14.03	Промежуточный клиринговый сеанс (дневной клиринг)
				// 14.03 - 18.45	Окончание основной торговой сессии
				// 18.45 - 19.00	Вечерний клиринговый сеанс
				// 19.00 - 23.50	Вечерняя торговая сессия
				russianMarket.ClearingTimespans.Add(new MarketClearingTimespan() {
					SuspendServerTimeOfDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 14, 00, 0),
					ResumeServerTimeOfDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 14, 03, 0)
				});
				russianMarket.ClearingTimespans.Add(new MarketClearingTimespan() {
					SuspendServerTimeOfDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 18, 45, 0),
					ResumeServerTimeOfDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 19, 00, 0)
				});
				ret.Add(russianMarket.Name, russianMarket);
				MarketInfo usMarket = new MarketInfo() {
					Name = "US",
					Description = "NYSE, Nasdaq and Amex equities",
					MarketOpenServerTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 9, 30, 0),
					MarketCloseServerTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 16, 0, 0),
					TimeZoneName = "Eastern Standard Time"
				};
				for (int i = 2007; i < 2020; i++) {
					usMarket.HolidaysYMD000.Add(new DateTime(i, 7, 4));
					usMarket.HolidaysYMD000.Add(new DateTime(i, 12, 25));
					for (int j = 1 ; j < 30 ; j++) {
						DateTime item = new DateTime(i, 9, j);
						if (item.DayOfWeek == DayOfWeek.Monday) {
							usMarket.HolidaysYMD000.Add(item);
							break;
						}
					}
				}
				ret.Add(usMarket.Name, usMarket);
				return ret;
			}
		}
		public MarketInfo MarketDefault {
			get {
				if (Entity.ContainsKey(RepositoryCustomMarketInfo.MarketDefaultName) == false) {
					throw new Exception("please add Market.Name=[" + RepositoryCustomMarketInfo.MarketDefaultName + "]"
						+ " into your [" + Path.Combine(base.RootPath, base.FnameRelpath) + "]"
						+ " for MarketInfoRepository.MarketDefault");
				}
				return Entity[RepositoryCustomMarketInfo.MarketDefaultName];
			}
		}
		public static List<DayOfWeek> ParseDaysOfWeekCsv(string csv, string separator) {
			List<DayOfWeek> ret = null;
			if (string.IsNullOrEmpty(csv)) return ret;
			if (string.IsNullOrEmpty(separator)) return ret;
			string[] csvSplitted = csv.Split(separator.ToCharArray());
			if (csvSplitted.Length == 0) return ret;
			foreach (string token in csvSplitted) {
				bool dowParsedOk = false;
				DayOfWeek parsed = DayOfWeek.Sunday;
				foreach (DayOfWeek everyDow in Enum.GetValues(typeof(DayOfWeek))) {
					if (everyDow.ToString().ToUpper().IndexOf(token.ToUpper()) >= 0) {
						parsed = everyDow;
						dowParsedOk = true;
						break;
					}
				}
				if (dowParsedOk == false) throw new Exception("[" + token + "] doesn't look like a DayOfWeek");
				if (ret == null) ret = new List<DayOfWeek>();
				if (ret.Contains(parsed)) throw new Exception("[" + token + "] parsed[" + parsed + "] is mentioned twice");
				ret.Add(parsed);
			}
			return ret;
		}
		public Dictionary<string, string> parseCsv(string filename, string delimeters, int skipFirstLines = 0) {
			Dictionary<string, string> ret = new Dictionary<string, string>();

			// #http://msdn.microsoft.com/en-us/library/ms912391(v=winembedded.11).aspx
			// #Index,Name of Time Zone,Time,
			// 0,Dateline Standard Time,(GMT-12:00) International Date Line West
			// 1,Samoa Standard Time,"(GMT-11:00) Midway Island, Samoa"
			
			using (StreamReader textReader = new StreamReader(filename)) {
				CsvConfiguration cfg = new CsvConfiguration();
				cfg.Delimiter = delimeters;
				cfg.AllowComments = true;
				var csv = new CsvReader(textReader, cfg);
				while (csv.Read()) {
					string[] fields = csv.CurrentRecord;
					if (fields.Length < 3) continue;
					//cfg.AllowComments = true if (fields[0].TrimStart(" ".ToCharArray()).StartsWith("#")) continue;

					string timeZoneWindowsName = fields[1].Trim();		// Central Standard Time
					string timeZoneGmtCity = fields[2].Trim();			// (GMT-06:00) Central Time (US and Canada)

					if (ret.ContainsKey(timeZoneGmtCity)) continue;
					ret.Add(timeZoneGmtCity, timeZoneWindowsName);
				}
			}

			return ret;
		}
		public void RenameMarketInfoRearrangeDictionary(MarketInfo marketInfo) {
			if (Entity.ContainsKey(marketInfo.Name)) {
				throw new Exception("the Market[" + marketInfo.Name + "] is already defined, choose another name");
			}
			string oldMarketName = null;
			foreach (string marketName in Entity.Keys) {
				if (Entity[marketName] == marketInfo) {
					oldMarketName = marketName;
					break;
				}
			}
			if (string.IsNullOrEmpty(oldMarketName)) {
				throw new Exception("CRAZY#86 the Market[" + marketInfo.Name + "] wasn't found in Markets");
			}
			Entity.Remove(oldMarketName);
			Entity.Add(marketInfo.Name, marketInfo);
		}
	}
}