using System.Collections.Generic;

namespace Sq1.Adapters.Quik.Dde.XlDde {
	public class XlRowParsed : Dictionary<string, object> {
		public List<string> ErrorMessages;
		public XlRowParsed() : base() {
			ErrorMessages = new List<string>();
		}
	}
}
