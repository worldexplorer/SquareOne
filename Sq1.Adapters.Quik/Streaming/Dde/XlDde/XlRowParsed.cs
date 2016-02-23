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
			if (this.popupIf_keyDoesNotExist(columnName, "string", popup) == false) return valueIfNotFound;
			object value = base[columnName];
			if (value == null) value = valueIfNotFound;		// when XlType.Blank was received
			//string ret = (string)value;
			string ret = Convert.ToString(value);
			return ret;
		}

		internal double GetDouble(string columnName, double valueIfNotFound, bool popup = true) {
			if (this.popupIf_keyDoesNotExist(columnName, "double", popup) == false) return valueIfNotFound;
			object value = base[columnName];
			if (value == null) value = valueIfNotFound;		// when XlType.Blank was received
			double ret = double.NaN;
			try {
				//if (value is float) {
					ret = Convert.ToDouble(value);
				//} else if (value is double) {
				//    ret = (double)value;
				//} else {
				//    // slowest?
				//    Double.TryParse(value.ToString(), out ret);
				//}
			} catch (Exception ex) {
				string msg = "IS_NOT_DOUBLE[" + value + "] //GetDouble(columnName[" + columnName + "], valueIfNotFound[" + valueIfNotFound + "], popup[" + popup + "])";
				Assembler.PopupException(msg, ex);
			}
			return ret;
		}

		internal DateTime GetDateTime(string columnName, DateTime valueIfNotFound, bool popup = true) {
			if (this.popupIf_keyDoesNotExist(columnName, "DateTime", popup) == false) return valueIfNotFound;
			object value = base[columnName];
			if (value == null) value = valueIfNotFound;		// when XlType.Blank was received
			DateTime ret = (DateTime)value;
			return ret;
		}

		bool popupIf_keyDoesNotExist(string columnName, string typeExpected = "NOT_SPECIFIED", bool popup = true) {
			if (base.ContainsKey(columnName)) return true;
			if (popup == false) return false;

			string msg = "NULL_VALUE_RECEIVED_DDE_FOR " + this.topicImServing + "[" + columnName + "]";
			if (string.IsNullOrEmpty(typeExpected) == false) {
				msg += " MUST_BE " + typeExpected;
			}
			Assembler.PopupException(msg, null, false);
			return false;
		}

		internal void Add_popupIfDuplicate(string columnName, object value) {
			if (base.ContainsKey(columnName) == false) {
				base.Add(columnName, value);
			} else {
				Assembler.PopupException("DUPLICATE_COLUMN_ALREADY_EXISTED [" + columnName + "]", null, false);
				base[columnName] = value;
			}
		}

		public override string ToString() {
			string ret = "";
			foreach (KeyValuePair<string, object> each in this) {
				string asString = each.Key + "[" + Convert.ToString(each.Value) + "]";
				if (ret != "") ret += "," + Environment.NewLine;
				ret += asString;
			}
			return ret;
		}

		internal void AddOrReplace(string key, object value) {
			if (base.ContainsKey(key) == false) {
				base.Add(key, value);
				return;
			}
			base[key] = value;
		}
	}
}
