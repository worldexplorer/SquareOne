//    Config.cs (c) 2012 Nikolay Moroshkin, http://www.moroshkin.com/
using System;
using System.IO;
using System.Globalization;
using System.Reflection;

namespace Sq1.QuikAdapter {
	static class cfg {
		public const string ProgName = "Sq1";
		public static readonly string FullProgName;

		public static UserSettings35 u = new UserSettings35();

		public static string MainFormTitle { get; private set; }

		public static CultureInfo BaseCulture { get; private set; }
		public static NumberFormatInfo PriceFormat { get; private set; }

		public const string UserCfgFileExt = "cfg";
		public const string TradeLogFileExt = "csv";
		public const string FlushScArg = "-FlushStaticConfig";

		public static readonly string ExecFile;
		public static readonly string UserCfgFile;
		public static readonly string StatCfgFile;
		public static readonly string LogFile;
		public static readonly string SecFile;
		public static readonly string TradeLogFile;
		static cfg() {
			if (u == null) {
				u = new UserSettings35();
				Reinit();
			}
			Version ver = Assembly.GetExecutingAssembly().GetName().Version;
			FullProgName = ProgName + " " + ver.Major.ToString() + "." + ver.Minor.ToString();
			ExecFile = Assembly.GetExecutingAssembly().Location;
			string fs = ExecFile.Remove(ExecFile.LastIndexOf('.') + 1);
			UserCfgFile = fs + UserCfgFileExt;
			StatCfgFile = fs + "sc";
			LogFile = fs + "Log.csv";
			fs = fs.Remove(fs.LastIndexOf(Path.DirectorySeparatorChar) + 1);
			SecFile = fs + "seclist.csv";
			TradeLogFile = fs + "trades." + TradeLogFileExt;
			BaseCulture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
			BaseCulture.NumberFormat.NumberDecimalDigits = 0;
			PriceFormat = (NumberFormatInfo)NumberFormatInfo.CurrentInfo.Clone();
		}
		public static void Reinit() {
			MainFormTitle = cfg.FullProgName;

			PriceFormat.NumberDecimalDigits = (int)Math.Log10(u.PriceRatio);

		}
	}
}
