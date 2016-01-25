using System;
using System.ComponentModel;
using System.Diagnostics;

using Newtonsoft.Json;

namespace Sq1.Core.DataFeed {
	public class NamedObjectJsonSerializable {
		[Browsable(false)]
		[JsonProperty]	public	string	Name;
		[JsonIgnore]	public	string	NameImStoredUnder_asUniqueKeyForRename;


		//v1 was needed for NamedObjectJsonSerializableList.Contains() in Serializer<T>; now using RepositoryJsonChartSettingsTemplates
		//public override bool Equals(object obj) {
		//    NamedObjectJsonSerializable objCasted = obj as NamedObjectJsonSerializable;
		//    if (objCasted == null) {
		//        #if DEBUG
		//        Debugger.Break();
		//        #endif
		//        throw new Exception("MUST_BE_A_NamedObjectJsonSerializable_OR_DERIVED NamedObjectJsonSerializable.Equals(" + obj + ")");
		//    }
		//    return this.Name == objCasted.Name;
		//}

		//public virtual NamedObjectJsonSerializable Clone() {
		//    return (NamedObjectJsonSerializable) base.MemberwiseClone();
		//}
	}
}
