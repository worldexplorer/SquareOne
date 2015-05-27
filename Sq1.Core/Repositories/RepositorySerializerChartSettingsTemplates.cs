using System.IO;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Newtonsoft.Json;
using Sq1.Core.DataTypes;
using Sq1.Core.Serializers;
using Sq1.Core.Charting;

namespace Sq1.Core.Repositories {
	public class RepositorySerializerChartSettingsTemplates : Serializer<ChartSettingsBaseList> {
		public RepositorySerializerChartSettingsTemplates() : base() {}

		public ChartSettingsBaseList Templates { get { return base.EntityDeserialized; } }

		public ChartSettingsBase FindChartSettingsBaseNullUnsafe(string templateName) {
			ChartSettingsBase ret = null;
			if (string.IsNullOrEmpty(templateName)) return ret;
			foreach (ChartSettingsBase eachChartSettingsBase in this.Templates) {
				if (eachChartSettingsBase.Name.ToUpper() != templateName.ToUpper()) continue;
				ret = eachChartSettingsBase;
				break;
			}
			return ret;
		}
		public ChartSettingsBase FindChartSettingsBaseOrNew(string symbol) {
			ChartSettingsBase ret = this.FindChartSettingsBaseNullUnsafe(symbol);
			if (ret == null) ret = this.Add(symbol);
			return ret;
		}
		public ChartSettingsBase Duplicate(ChartSettingsBase symbolInfo, string dupeTemplateName) {
			if (this.FindChartSettingsBaseNullUnsafe(dupeTemplateName) != null){
				string msg = "I_REFUSE_TO_DUPLICATE_CHART_SETTINGS__TEMPLATE_ALREADY_EXISTS[" + dupeTemplateName + "]";
				Assembler.PopupException(msg);
				return null;
			}
			ChartSettingsBase clone = symbolInfo.Clone();
			clone.Name = dupeTemplateName;
			this.Templates.Add(clone);
			this.sort();
			base.Serialize();
			return clone;
		}
		public ChartSettingsBase Rename(ChartSettingsBase template, string newTemplateName) {
			if (this.FindChartSettingsBaseNullUnsafe(newTemplateName) != null) {
				string msg = "I_REFUSE_TO_RENAME_CHART_SETTINGS__TEMPLATE_ALREADY_EXISTS[" + newTemplateName + "]";
				Assembler.PopupException(msg);
				return null;
			}
			template.Name = newTemplateName;
			this.sort();
			base.Serialize();
			return template;
		}
		public ChartSettingsBase Add(string newTemplateName) {
			if (this.FindChartSettingsBaseNullUnsafe(newTemplateName) != null) {
				string msg = "I_REFUSE_TO_ADD_CHART_SETTINGS__TEMPLATE_ALREADY_EXISTS[" + newTemplateName + "]";
				Assembler.PopupException(msg);
				return null;
			}
			ChartSettingsBase adding = new ChartSettingsBase();
			adding.Name = newTemplateName;
			this.Templates.Add(adding);
			this.sort();
			base.Serialize();
			return adding;
		}
		public ChartSettingsBase Delete(ChartSettingsBase template) {
			int index = this.Templates.IndexOf(template);
			if (index == -1) {
				string msg = "I_REFUSE_TO_DELETE_CHART_SETTINGS__NOT_FOUND[" + template.ToString() + "]";
				Assembler.PopupException(msg);
				return null;
			}
			this.Templates.Remove(template);
			this.sort();
			base.Serialize();
			if (index == 0) {
				string msg = "LAST_CHART_SETTINGS_DELETED[" + template.ToString() + "]";
				Assembler.PopupException(msg);
				return null;
			}
			ChartSettingsBase prior = this.Templates[index-1];
			return prior;
		}


		public class ASC : IComparer<ChartSettingsBase> { int IComparer<ChartSettingsBase>.Compare(ChartSettingsBase x, ChartSettingsBase y) { return x.Name.CompareTo(y.Name); } }
		public void sort() {
			this.Templates.Sort(new ASC());
		}
		internal void DeserializeAndSort() {
			this.Deserialize();
			this.sort();
		}

		bool deserializedOnce_nowSyncOnly = false;
		public override ChartSettingsBaseList Deserialize() {
			if (base.EntityDeserialized == null) base.EntityDeserialized = new ChartSettingsBaseList();
			ChartSettingsBaseList backup = base.EntityDeserialized;
			base.Deserialize();

			if (this.deserializedOnce_nowSyncOnly == false) {
				this.deserializedOnce_nowSyncOnly = true;
				return base.EntityDeserialized;
			}

			ChartSettingsBaseList dontOverwriteInstancesKozSubscribersWillLooseEventGenerators = base.EntityDeserialized;
			base.EntityDeserialized = backup;

			string msg = "YOU_SHOULD_NOT_DESERIALIZE_TWICE__RepositorySerializerChartSettingsBaseTemplates.Deserialize()";
			Assembler.PopupException(msg, null, false);
			// if you never saw the Exception above, the code below is never invoked; keeping it to eternify my stupidity

			ChartSettingsBaseList toBeAdded = new ChartSettingsBaseList();
			ChartSettingsBaseList toBeDeleted = new ChartSettingsBaseList();
			foreach (ChartSettingsBase eachExisting in base.EntityDeserialized) {
				if (backup.Contains(eachExisting)) continue;
				toBeAdded.Add(eachExisting);
			}
			foreach (ChartSettingsBase eachDeserialized in dontOverwriteInstancesKozSubscribersWillLooseEventGenerators) {
				if (this.Templates.ContainsName(eachDeserialized.Name)) continue;
				toBeDeleted.Add(eachDeserialized);
			}
			foreach (var deleteEach in toBeDeleted) {
				this.Templates.Remove(deleteEach);
			}
			base.EntityDeserialized.AddRange(toBeAdded);
			return base.EntityDeserialized;
		}

	}
}