namespace Sq1.Adapters.Quik.Dde.XlDde {
	public class XlColumn {
		public	XlBlockType		TypeExpected;
		public	string			Name;
//		public	List<string>	Names;
		public	string			ToDateTimeParseFormat;
		public	int				IndexFound;
		public	object			Value;
		public	bool			Mandatory;
		public	bool			UpperLowerCaseSensitive;

		XlColumn() {
			IndexFound				= -1;
			Mandatory				= false;
			UpperLowerCaseSensitive	= false;
		}

		public XlColumn(XlBlockType typeExpected, string name, bool mandatory = false) : this() {
			TypeExpected = typeExpected;
			Name = name;
			Mandatory = mandatory;
		}

		public XlColumn Clone() {
			XlColumn ret = (XlColumn)this.MemberwiseClone();
			ret.Value = null;
			return ret;
		}
	}
}
