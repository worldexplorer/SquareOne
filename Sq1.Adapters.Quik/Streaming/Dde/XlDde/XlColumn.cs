namespace Sq1.Adapters.Quik.Streaming.Dde.XlDde {
	public class XlColumn {
		public	XlBlockType		TypeExpected;
		public	string			Name;
//		public	List<string>	NameSynonims;
		public	string			ToDateParseFormat;	// mention some format to indicate it's a Date, I'll try to parse from anything first and then I'll try the mentioned one
		public	string			ToTimeParseFormat;	// mention some format to indicate it's a Time, I'll try to parse from anything first and then I'll try the mentioned one
		public	int				IndexFound;
		public	object			Value;
		public	bool			Mandatory;
		public	bool			UpperLowerCaseSensitive;
		public	bool			PrimaryKey_forSingleUpdates;

		//mess starts here
		public	object			ValueWhenNotReceived;
		public	bool			PopupIf_keyDoesNotExist;

		XlColumn() {
			IndexFound				= -1;
			Mandatory				= false;
			UpperLowerCaseSensitive = false;
			PrimaryKey_forSingleUpdates = false;
		}

		public XlColumn(XlBlockType typeExpected, string name, bool mandatory = false,
						object valueWhenNotReceived = null, bool popupIf_keyDoesNotExist = true,
						bool primaryKey = false) : this() {
			TypeExpected				= typeExpected;
			Name						= name;
			Mandatory					= mandatory;
			PrimaryKey_forSingleUpdates	= primaryKey;
			ValueWhenNotReceived		= valueWhenNotReceived;
			PopupIf_keyDoesNotExist		= popupIf_keyDoesNotExist;
		}

		public XlColumn Clone() {
			XlColumn ret = (XlColumn)this.MemberwiseClone();
			ret.Value = null;
			return ret;
		}

		public override string ToString() {
			string ret = "{" + this.IndexFound + "}" + this.TypeExpected + ":" + this.Name + "[" + this.Value + "]";
			if (this.PrimaryKey_forSingleUpdates) ret += "PK";
			return ret;
		}
	}
}
