using System;
using System.Collections.Generic;

namespace Sq1.Core.Support {
	public class SymbolParser {
		public static string SEPARATORS = ", \n\r";
		
		public static List<string> ParseSymbols(string csv) {
			List<string> ret = new List<string>();
			if (string.IsNullOrEmpty(csv)) return ret;
			csv = csv.Trim();
			string[] csvSplitted = csv.Split(SEPARATORS.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
			foreach (string token in csvSplitted) {
				ret.Add(token);
			}
			return ret;
		}
	}
}
