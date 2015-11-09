namespace Sq1.Adapters.Quik.Dde.XlDde {
	public class XlColumn {
		public	XlTable.BlockType	TypeExpected;
		public	string				Name;
//		public	List<string>		Names;
		public	string				ToDateTimeParseFormat;
		public	int					IndexFound;
		public	object				Value;
		public	bool				Mandatory;
		public	bool				UpperLowerCaseSensitive;

		public XlColumn() {
			TypeExpected			= XlTable.BlockType.Unknown;
			IndexFound				= -1;
			Mandatory				= false;
			UpperLowerCaseSensitive	= false;
		}
		public XlColumn Clone() {
			XlColumn ret = (XlColumn)this.MemberwiseClone();
			ret.Value = null;
			return ret;
		}
	}
}
