namespace Sq1.Adapters.Quik.Dde.XlDde {
	public class XlColumn {
		public string Name;
		//public List<string> Names;
		public XlTable.BlockType TypeExpected = XlTable.BlockType.Unknown;
		public string ToDateTimeParseFormat;
		public int IndexFound = -1;
		public object Value;
		public bool Mandatory = false;
		public bool UpperLowerCaseSensitive = false;
		public XlColumn Clone() {
			XlColumn ret = (XlColumn)this.MemberwiseClone();
			ret.Value = null;
			return ret;
		}
	}
}
