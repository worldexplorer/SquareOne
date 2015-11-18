// QScalp source code was downloaded on O2-Jun-2012 for free from http://www.qscalp.ru/download/qscalp_src.zip
// SquareOne uses QScalp's modified classes and keeps original author Name and URL
// Nikolay Moroshkin can tell me to remove his code completely => I'll rewrite the pieces borrowed //Pavel Chuchkalov 
//    XlDdeServer.cs (c) 2011 Nikolay Moroshkin, http://www.moroshkin.com/

using System;
using System.Collections.Generic;
using System.Globalization;
using Sq1.Core;

namespace Sq1.Adapters.Quik.Dde.XlDde {
	public abstract class XlDdeTable {
		public			string		Topic				{ get; private set; }
		public			DateTime	LastDataReceived	{ get; protected set; }
		public	virtual	bool		ReceivingDataDde				{ get; set; }
		public			bool		ErrorParsing		{ get; protected set; }
		public			void		ResetError()		{ ErrorParsing = false; }

		protected	List<XlColumn>				columns;
		protected	Dictionary<int, XlColumn>	columnsByIndexFound;
		protected	bool						columnsIdentified;
		//List<string> primaryKey;
		//Dictionary<string, XlColumn> rowParsed;

		public XlDdeTable(string topic) : this() {
			this.Topic = topic;
		}
		public XlDdeTable() {
			this.columnsIdentified = false;
			this.columns = new List<XlColumn>();
		}

		public void PutDdeData(byte[] data) {
			this.LastDataReceived = DateTime.UtcNow;

			using (XlTable xt = new XlTable(data)) {
				this.PutDdeTable(xt);
			}
		}
		protected virtual void PutDdeTable(XlTable xt) {
			for (int i = 0; i < xt.RowsCount; i++) {
				if (this.columnsIdentified == false) {
					this.columnsIdentified = this.identifyColumnsByReadingHeader(xt);
					if (this.columnsIdentified == false) return;
					if (xt.RowsCount == 1) return;
				}
				XlRowParsed rowParsed = this.parseRow(xt);
				if (rowParsed == null || rowParsed.Count == 0) continue;
				string columnName = columns[0].Name;
				if (rowParsed[columnName] == null) continue;
				if (rowParsed[columnName] is string) {
					string cellParced = (string) rowParsed[columnName];
					if (cellParced == columnName) continue;
				}

				foreach (XlColumn col in this.columns) {
					string typeParsed = rowParsed[col.Name].GetType().Name;
					if (typeParsed == "Double") typeParsed = "Float";
					string typeExpected = col.TypeExpected.ToString();
					if (typeParsed == "DateTime") typeParsed = "String";
					if (typeParsed != typeExpected) {
						this.columnsIdentified = false;
						string msg = "rowParsed[" + col.Name + "][" + rowParsed[col.Name] + "]"
							+ " is not a col.TypeExpected[" + col.TypeExpected.GetType() + "]"
							+ ", column order changed, skipping quote and re-identifyingColumnsByReadingHeader";
						Assembler.PopupException(msg);
						continue;
					}
				}

				try {
					this.processNonHeaderRowParsed(rowParsed);
				} catch (Exception e) {
					string msg = "[" + this.LastDataReceived.ToString("HH:mm:ss.fff ddd dd MMM yyyy") + "] Exception in Channdel: " + e.Message;
					Assembler.PopupException(msg, e);
				}
			}
		}
		protected bool identifyColumnsByReadingHeader(XlTable xt) {
			bool ret = false;
			List<XlColumn> mandatoriesNotFound = new List<XlColumn>();
			columnsByIndexFound = new Dictionary<int, XlColumn>();
			foreach (XlColumn col in this.columns) {
				col.IndexFound = -1;
				if (col.Mandatory) mandatoriesNotFound.Add(col);
			}
			for (int col_serno = 0; col_serno < xt.ColumnsCount; col_serno++) {
				xt.ReadValue();
				if (xt.ValueType != XlTable.BlockType.String) continue;
				string columnNameToIdentify = xt.StringValue;
				foreach (XlColumn col in this.columns) {
					if (col.Name != columnNameToIdentify) continue;
					columnsByIndexFound.Add(col_serno, col.Clone());
					if (mandatoriesNotFound.Contains(col)) mandatoriesNotFound.Remove(col);
					break;
				}
			}
			if (mandatoriesNotFound.Count == 0) ret = true;
			return ret;
		}
		protected XlRowParsed parseRow(XlTable xt) {
			XlRowParsed rowParsed = new XlRowParsed();
			for (int col = 0; col < xt.ColumnsCount; col++) {
				xt.ReadValue();
				if (columnsByIndexFound.ContainsKey(col) == false) continue;
				XlColumn xlCol = columnsByIndexFound[col].Clone();
				switch (xt.ValueType) {
					case XlTable.BlockType.Float:
						xlCol.Value = xt.FloatValue;
						break;
					case XlTable.BlockType.String:
						switch (xlCol.TypeExpected) {
							case XlTable.BlockType.String:
								if (string.IsNullOrEmpty(xlCol.ToDateTimeParseFormat) == false) {
									try {
										xlCol.Value = DateTime.ParseExact(xt.StringValue,
											xlCol.ToDateTimeParseFormat, CultureInfo.InvariantCulture);
									} catch (Exception ex) {
										string errmsg = "can not convert StringValue[" + xt.StringValue + "]"
											+ " to xlCol[" + xlCol.Name + "].TypeExpected[" + xlCol.TypeExpected + "]"
											+ " // xt.ValueType[" + xt.ValueType + "] with StringValue[" + xt.StringValue + "] FloatValue[" + xt.FloatValue + "] ";
										rowParsed.ErrorMessages.Add(errmsg);
										xlCol.Value = DateTime.MinValue;
										break;	// leave the cell blank but add it with the last command of the method
									}
								} else {
									xlCol.Value = xt.StringValue;
								}
								break;
							case XlTable.BlockType.Float:
								try {
									xlCol.Value = (double) Convert.ToDouble(xt.StringValue);
								} catch (Exception e) {
									// crazy but for last="2000.8" xt.ValueType=String, xt.StringValue="", xt.FloatValue=2000.8
									xlCol.Value = xt.FloatValue;
								}
								break;
							default:
								string errmsg2 = "no handler to convert StringValue[" + xt.StringValue + "]"
									+ " to xlCol[" + xlCol.Name + "].TypeExpected[" + xlCol.TypeExpected + "]"
									+ " // xt.ValueType[" + xt.ValueType + "] with StringValue[" + xt.StringValue + "] FloatValue[" + xt.FloatValue + "] ";
								rowParsed.ErrorMessages.Add(errmsg2);
								break;
						}
						break;
					default:
						string errmsg3 = "xlCol[" + xlCol.Name + "].TypeExpected[" + xlCol.TypeExpected + "]!=xt.ValueType[" + xt.ValueType + "]"
							+ " with StringValue[" + xt.StringValue + "] FloatValue[" + xt.FloatValue + "]";
						rowParsed.ErrorMessages.Add(errmsg3);
						break;
				}
				rowParsed.Add(xlCol.Name, xlCol.Value);
			}
			return rowParsed;
		}
		protected abstract void processNonHeaderRowParsed(XlRowParsed row);
		public override string ToString() {
			string ret = "";
			if (string.IsNullOrEmpty(this.Topic) == false) ret += "Topic[" + this.Topic + "] ";
			ret += (this.ReceivingDataDde ? "GettingData" : "NeverReceived")
				+ " " + (this.ErrorParsing ? "Error!" : "NoError")
				+ " " + (this.columnsIdentified ? "columnsIdentified" : "columnsNotIdentified");
			return ret;
		}
	}
}
