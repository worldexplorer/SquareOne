using System;
using System.Collections.Generic;
using System.IO;

using Newtonsoft.Json;
using Sq1.Core.DataFeed;

namespace Sq1.Core.Repositories {
	public partial class RepositoryJsonsInFolderSimpleDictionary<SYSTEM_PERFORMANCE_RESTORE_ABLE>
			: RepositoryJsonsInFolderSimple<SYSTEM_PERFORMANCE_RESTORE_ABLE>
			where SYSTEM_PERFORMANCE_RESTORE_ABLE : NamedObjectJsonSerializable {

		public virtual Dictionary<string, SYSTEM_PERFORMANCE_RESTORE_ABLE> DeserializeDictionary(string fname) {
			Dictionary<string, SYSTEM_PERFORMANCE_RESTORE_ABLE> tmp = null;
			Dictionary<string, SYSTEM_PERFORMANCE_RESTORE_ABLE> ret = new Dictionary<string, SYSTEM_PERFORMANCE_RESTORE_ABLE>();
			string jsonAbsfile = Path.Combine(base.AbsPath, fname + base.Extension);
			if (File.Exists(jsonAbsfile) == false) return ret;
			try {
				string json = File.ReadAllText(jsonAbsfile);
				ret = JsonConvert.DeserializeObject<Dictionary<string, SYSTEM_PERFORMANCE_RESTORE_ABLE>>(json, new JsonSerializerSettings {
					TypeNameHandling = TypeNameHandling.Objects
				});
			} catch (Exception ex) {
				string msig = " RepositoryJsonsInFolder<" + base.OfWhat + ">::DeserializeDictionary(): ";
				string msg = "FAILED_DeserializeList_WITH_jsonAbsfile[" + jsonAbsfile + "]";
				Assembler.PopupException(msg + msig, ex);
				return ret;
			}
			return ret;
		}
		public virtual void SerializeDictionary(Dictionary<string, SYSTEM_PERFORMANCE_RESTORE_ABLE> parametersByNameChecked, string jsonRelname) {
			jsonRelname += base.Extension;
			string jsonAbsname = Path.Combine(base.AbsPath, jsonRelname);
			try {
				string json = JsonConvert.SerializeObject(parametersByNameChecked, Formatting.Indented,
					new JsonSerializerSettings {
						TypeNameHandling = TypeNameHandling.Objects
					});
				File.WriteAllText(jsonAbsname, json);
			} catch (Exception ex) {
				string msig = " RepositoryJsonsInFolder<" + base.OfWhat + ">::SerializeDictionary(): ";
				string msg = "FAILED_SerializeList_WITH_base.jsonAbsname[" + jsonAbsname + "]";
				Assembler.PopupException(msg + msig, ex);
				return;
			}
		}
	}
}
