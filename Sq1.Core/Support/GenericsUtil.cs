using System;
using System.Collections.Generic;

namespace Sq1.Core.Support {
	public class GenericsUtil {
		public static void SyncToEthalon<T>(List<T> ethalon, List<T> makingIdentical_toEthalon, out List<T> itemsToAdd, out List<T> itemsToRemove) {
					itemsToAdd		= new List<T>();
					itemsToRemove	= new List<T>();

			foreach (T exists_inEthalon in ethalon) {
				if (makingIdentical_toEthalon.Contains(exists_inEthalon)) continue;
				itemsToAdd.Add(exists_inEthalon);
			}

			foreach (T doesntExist_inEthalon in makingIdentical_toEthalon) {
				if (ethalon.Contains(doesntExist_inEthalon)) continue;
				itemsToRemove.Add(doesntExist_inEthalon);
			}
		}
	}
}
