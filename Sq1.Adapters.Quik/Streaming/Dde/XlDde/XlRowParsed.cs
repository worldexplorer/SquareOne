using System;
using System.Collections.Generic;

using Sq1.Core;
using System.ComponentModel;

namespace Sq1.Adapters.Quik.Streaming.Dde.XlDde {
	public class XlRowParsed : Dictionary<string, object> {
		public	List<string>					ErrorMessages;
		public	string							topicImServing					{ get; private set; }
		public	Dictionary<string, XlColumn>	ColumnDefinitionsByNameLookup	{ get; private set; }

		public XlRowParsed(string topic, Dictionary<string, XlColumn> columnDefinitionsByNameLookup_passed) : base() {
			ErrorMessages = new List<string>();
			topicImServing = topic;
			ColumnDefinitionsByNameLookup = columnDefinitionsByNameLookup_passed;
		}


		#region one more step for 1) automation 2) serialization of TableDefinitions.cs
		internal T Get<T>(string columnNameRequested) {
			string ofWhat = typeof(T).ToString();
			string msig = " //XlRowParsed.Get<" + ofWhat + ">(columnNameRequested[" + columnNameRequested + "])";

			T ret = default(T);
			object asObject = null;
			T valueWhenNotReceived_nullUnsafe = default(T);

			XlColumn columnDefinition = columnDefinitionFor(columnNameRequested, msig);
			bool popup = columnDefinition.PopupIf_keyDoesNotExist;
			if (columnDefinition.ValueWhenNotReceived == null) {
				string msg = "YOU_MUST_DEFINE_ValueWhenNotReceived_IN_TableDefinitions.cs_FOR_columnNameRequested";
				throw new Exception(msg + msig);
			}

			try {
				//if (typeof(T) == typeof(string)) {
				//    //valueWhenNotReceived_nullUnsafe = (T) columnDefinition.ValueWhenNotReceived;
				//} else if (typeof(T) == typeof(double)) {
				//    if (double.IsNaN(columnDefinition.ValueWhenNotReceived)) {
				//        valueWhenNotReceived_nullUnsafe = double.NaN;
				//    }
				//} else if (typeof(T) == typeof(DateTime)) {
				//    if (double.IsNaN(columnDefinition.ValueWhenNotReceived == DateTime.MinValue) {
				//    valueWhenNotReceived_nullUnsafe = (T) columnDefinition.ValueWhenNotReceived;
				//} else {
				//    valueWhenNotReceived_nullUnsafe = (T) columnDefinition.ValueWhenNotReceived;
				//}
				if (valueWhenNotReceived_nullUnsafe == null) {
					string msg = "FAILED_TO_CONVERT_columnDefinition.ValueWhenNotReceived_TO_TYPE_REQUESTED CHECK_TableDefinitions.cs_FOR_columnNameRequested";
					throw new Exception(msg + msig);
				}
			} catch (Exception ex) {
				string msg = "VALUE_WHEN_NOT_RECEIVED_UNCASTABLE_TO_[" + ofWhat + "] ValueWhenNotReceived[" + columnDefinition.ValueWhenNotReceived + "]";
				Assembler.PopupException(msg + msig, ex);
				return ret;
			}

			try {
				//TypeConverter toMyType = TypeDescriptor.GetConverter(typeof(T));
				if (typeof(T) == typeof(string)) {
					string gotString = this.GetString(columnNameRequested, Convert.ToString(valueWhenNotReceived_nullUnsafe), popup);
					//ret = ((object) gotString) as T;
					//ret = (T) TypeDescriptor.GetConverter(typeof(T)).ConvertFrom(gotString);
					//asObject = toMyType.ConvertFrom(gotString);
					asObject = gotString as object;
				} else if (typeof(T) == typeof(double)) {
					double gotDouble = this.GetDouble(columnNameRequested, Convert.ToDouble(valueWhenNotReceived_nullUnsafe), popup);
					//ret = gotDouble;
					//ret = (T) TypeDescriptor.GetConverter(typeof(T)).ConvertFrom(gotDouble);
					//asObject = toMyType.ConvertFrom(gotDouble);
					asObject = gotDouble as object;
				} else if (typeof(T) == typeof(DateTime)) {
					DateTime gotDateTime = this.GetDateTime(columnNameRequested, Convert.ToDateTime(valueWhenNotReceived_nullUnsafe), popup);
					//ret = gotDateTime;
					//ret = (T) TypeDescriptor.GetConverter(typeof(T)).ConvertFrom(gotDateTime);
					//asObject = toMyType.ConvertFrom(gotDateTime);
					asObject = gotDateTime as object;
				} else {
					string msg = "CONVERSION_NOT_SUPPORTED for typeof<" + ofWhat + ">";
					throw new Exception(msg + msig);
				}
				ret = (T) asObject;
			} catch (Exception ex) {
				string msg = "asObject[" + asObject + "]_UNCASTABLE_TO_[" + ofWhat + "]";
				Assembler.PopupException(msg + msig, ex);
			}
			return ret;
		}

		XlColumn columnDefinitionFor(string columnNameRequested, string invoker = "INVOKER_UNKNOWN") {
			if (this.ColumnDefinitionsByNameLookup.ContainsKey(columnNameRequested) == false) {
				string msg = "DONT_REQUEST_COLUMN_YOU_DIDNT_DEFINE_IN_TableDefinitions.cs";
				throw new Exception(msg + invoker);
			}
			XlColumn columnDefinition = this.ColumnDefinitionsByNameLookup[columnNameRequested];
			return columnDefinition;
		}
		#endregion


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
				//	ret = (double)value;
				//} else {
				//	// slowest?
				//	Double.TryParse(value.ToString(), out ret);
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
