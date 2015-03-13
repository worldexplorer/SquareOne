using System;

namespace Sq1.Core.Support {
	public class SkipInstantiationAtAttribute : Attribute {
		public bool Startup = false;

		public static bool AtStartup(Type type) {
			bool ret = false;
			foreach (Attribute attr in type.GetCustomAttributes(true)) {
				SkipInstantiationAtAttribute skipInstantiationAt = attr as SkipInstantiationAtAttribute;
				if (skipInstantiationAt != null) {
					ret = skipInstantiationAt.Startup;
					break;
				}
			}
			return ret;
		}
	}
}
