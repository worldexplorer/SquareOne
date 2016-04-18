using System;
using System.ComponentModel;

using Newtonsoft.Json;
using System.Reflection;

namespace Sq1.Core.DataTypes {
	public class NamedObjectJsonSerializable {
		[Browsable(false)]
		[JsonProperty]	public	string	Name;
		[JsonIgnore]	public	string	NameImStoredUnder_asUniqueKeyForRename;


		//v1 was needed for NamedObjectJsonSerializableList.Contains() in Serializer<T>; now using RepositoryJsonChartSettingsTemplates
		//public override bool Equals(object obj) {
		//	NamedObjectJsonSerializable objCasted = obj as NamedObjectJsonSerializable;
		//	if (objCasted == null) {
		//		#if DEBUG
		//		Debugger.Break();
		//		#endif
		//		throw new Exception("MUST_BE_A_NamedObjectJsonSerializable_OR_DERIVED NamedObjectJsonSerializable.Equals(" + obj + ")");
		//	}
		//	return this.Name == objCasted.Name;
		//}

		//public virtual NamedObjectJsonSerializable Clone() {
		//	return (NamedObjectJsonSerializable) base.MemberwiseClone();
		//}

		public void AbsorbFrom(NamedObjectJsonSerializable tpl) {
			PropertyInfo[] props = this.GetType().GetProperties();
			foreach (PropertyInfo prop in props) {
				bool jsonPropertyFound = false;
				bool browseable = true;
				//foreach (Attribute attr in prop.GetCustomAttributes(typeof(JsonPropertyAttribute), true)) {
				foreach (Attribute attr in ((MemberInfo)prop).GetCustomAttributes(true)) {
					if (attr == null) continue;

					BrowsableAttribute attrAsBrowsable = attr as BrowsableAttribute;
					if (attrAsBrowsable != null) {
						if (attrAsBrowsable.Browsable) continue;
						browseable = false;
						continue;
					}

					JsonPropertyAttribute attrAsJsonProperty = attr as JsonPropertyAttribute;
					if (attrAsJsonProperty != null) {
						jsonPropertyFound = true;
					}
				}
				if (jsonPropertyFound == false || browseable == false) continue;

				PropertyInfo  tplProperty =  tpl.GetType().GetProperty(prop.Name);
				object tplValue = prop.GetValue(tpl, null);
				prop.SetValue(this, tplValue, null);
			}
			this.Name = tpl.Name;
		}
	}
}
