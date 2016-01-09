using System;
using System.Collections.Generic;

using Sq1.Core;

namespace Sq1.Adapters.Quik.Streaming.Dde.XlDde {
	public class XlRowParsed : Dictionary<string, object> {
		public List<string> ErrorMessages;
		private string topicImServing;

		public XlRowParsed(string topic) : base() {
			ErrorMessages = new List<string>();
			this.topicImServing = topic;
		}

		internal string GetString(string columnName, string valueIfNotFound, bool popup = true) {
			if (this.popupIf_keyDoesNotExist(columnName, "string", popup)) return valueIfNotFound;
			object value = base[columnName];
			if (value == null) value = valueIfNotFound;		// when XlType.Blank was received
			string ret = (string)value;
			return ret;
		}

		internal double GetDouble(string columnName, double valueIfNotFound, bool popup = true) {
			if (this.popupIf_keyDoesNotExist(columnName, "double", popup)) return valueIfNotFound;
			object value = base[columnName];
			if (value == null) value = valueIfNotFound;		// when XlType.Blank was received
			double ret = (double)value;
			return ret;
		}

		internal DateTime GetDateTime(string columnName, DateTime valueIfNotFound, bool popup = true) {
			if (this.popupIf_keyDoesNotExist(columnName, "DateTime", popup)) return valueIfNotFound;
			object value = base[columnName];
			if (value == null) value = valueIfNotFound;		// when XlType.Blank was received
			DateTime ret = (DateTime)value;
			return ret;
		}

		bool popupIf_keyDoesNotExist(string columnName, string typeExpected = "NOT_SPECIFIED", bool popup = true) {
			if (base.ContainsKey(columnName)) return false;
			if (popup == false) return false;

			string msg = "NULL_VALUE_RECEIVED_DDE_FOR " + this.topicImServing + "[" + columnName + "]";
			if (string.IsNullOrEmpty(typeExpected) == false) {
				msg += " MUST_BE_" + typeExpected;
			}
			Assembler.PopupException(msg, null, false);
			return true;
		}

		internal void Add_popupIfDuplicate(string columnName, object value) {
			if (base.ContainsKey(columnName) == false) {
				base.Add(columnName, value);
			} else {
				Assembler.PopupException("DUPLICATE_COLUMN_ALREADY_EXISTED [" + columnName + "]", null, false);
				base[columnName] = value;
			}
		}
	}
}
