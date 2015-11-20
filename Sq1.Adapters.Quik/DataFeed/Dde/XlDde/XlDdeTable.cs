using System;
using System.Collections.Generic;
using System.Globalization;

using Sq1.Core;

namespace Sq1.Adapters.Quik.Dde.XlDde {
	public abstract class XlDdeTable {
		protected abstract	string				DdeConsumerClassName	{ get; }
		public				string				Topic					{ get; private set; }
		public				DateTime			LastDataReceived		{ get; protected set; }
		public	virtual		bool				ReceivingDataDde		{ get; set; }

		protected	List<XlColumn>				Columns;				// part of the abstraction to implement in children
		protected	Dictionary<int, XlColumn>	ColumnsByIndexFound;
		protected	bool						ColumnsIdentified;
		protected	QuikStreaming				QuikStreaming			{ get; private set; }

		XlDdeTable() {
			this.ColumnsIdentified		= false;
			this.Columns				= new List<XlColumn>();			// part of the abstraction to implement in children
			this.ColumnsByIndexFound	= new Dictionary<int, XlColumn>();
		}
		protected XlDdeTable(string topic, QuikStreaming quikStreaming) : this() {
			this.Topic = topic;
			this.QuikStreaming = quikStreaming;
		}

		public void ParseDeliveredDdeData_pushToStreaming(byte[] data) {
			this.LastDataReceived = DateTime.UtcNow;

			this.IncomingTableBegun();
			using (XlReader xt = new XlReader(data)) {
				this.FillTable_readParseMessage(xt);
			}
			this.IncomingTableTerminated();
		}
		protected virtual void FillTable_readParseMessage(XlReader xt) {
			for (int i = 0; i < xt.RowsCount; i++) {
				if (this.ColumnsIdentified == false) {
					this.ColumnsIdentified = this.identifyColumnsByReadingHeader(xt);
					if (this.ColumnsIdentified == false) return;
					if (xt.RowsCount == 1) return;
				}
				XlRowParsed rowParsed = this.parseRow(xt);
				if (rowParsed == null || rowParsed.Count == 0) continue;
				string columnName = Columns[0].Name;
				if (rowParsed[columnName] == null) continue;
				if (rowParsed[columnName] is string) {
					string cellParced = (string) rowParsed[columnName];
					if (cellParced == columnName) continue;
				}

				foreach (XlColumn col in this.Columns) {
					string typeParsed = rowParsed[col.Name].GetType().Name;
					if (typeParsed == "Double") typeParsed = "Float";
					string typeExpected = col.TypeExpected.ToString();
					if (typeParsed == "DateTime") typeParsed = "String";
					if (typeParsed != typeExpected) {
						this.ColumnsIdentified = false;
						string msg = "rowParsed[" + col.Name + "][" + rowParsed[col.Name] + "]"
							+ " is not a col.TypeExpected[" + col.TypeExpected.GetType() + "]"
							+ ", column order changed, skipping quote and re-identifyingColumnsByReadingHeader";
						Assembler.PopupException(msg);
						continue;
					}
				}

				try {
					this.IncomingRowParsedDelivered(rowParsed);
				} catch (Exception e) {
					string msg = "[" + this.LastDataReceived.ToString("HH:mm:ss.fff ddd dd MMM yyyy") + "] Exception in Channdel: " + e.Message;
					Assembler.PopupException(msg, e);
				}
			}
		}
		protected bool identifyColumnsByReadingHeader(XlReader xt) {
			bool ret = false;
			List<XlColumn> mandatoriesNotFound = new List<XlColumn>();
			this.ColumnsByIndexFound.Clear();
			foreach (XlColumn col in this.Columns) {
				col.IndexFound = -1;
				if (col.Mandatory) mandatoriesNotFound.Add(col);
			}
			for (int col_serno = 0; col_serno < xt.ColumnsCount; col_serno++) {
				xt.ReadValue();
				if (xt.ValueType != XlBlockType.String) continue;
				string columnNameToIdentify = xt.StringValue;
				foreach (XlColumn col in this.Columns) {
					if (col.Name != columnNameToIdentify) continue;
					this.ColumnsByIndexFound.Add(col_serno, col.Clone());
					if (mandatoriesNotFound.Contains(col)) mandatoriesNotFound.Remove(col);
					break;
				}
			}
			if (mandatoriesNotFound.Count == 0) ret = true;
			return ret;
		}
		protected XlRowParsed parseRow(XlReader xt) {
			XlRowParsed rowParsed = new XlRowParsed();
			for (int col = 0; col < xt.ColumnsCount; col++) {
				xt.ReadValue();
				if (this.ColumnsByIndexFound.ContainsKey(col) == false) continue;
				XlColumn xlCol = this.ColumnsByIndexFound[col].Clone();
				switch (xt.ValueType) {
					case XlBlockType.Float:
						xlCol.Value = xt.FloatValue;
						break;
					case XlBlockType.String:
						switch (xlCol.TypeExpected) {
							case XlBlockType.String:
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
							case XlBlockType.Float:
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

		protected virtual void IncomingTableBegun() { }
		protected abstract void IncomingRowParsedDelivered(XlRowParsed row);	// you can push to Streaming here (doesn't make sense to commit quotes to QuikStreaming as a table)
		protected virtual void IncomingTableTerminated() { }				//  or you can push to Streaming here (it is more consistent to unlock per-symbol DepthOfMarket as whole table so that QuikStreaming Level2 consumers will get the whole frame at once)

		public override string ToString() {
			string ret = "";
			if (string.IsNullOrEmpty(this.Topic) == false) ret += "Topic[" + this.Topic + "] ";
			ret += (this.ReceivingDataDde ? "GettingData" : "NeverReceived")
				//+ " " + (this.ErrorParsing ? "Error!" : "NoError")
				+ " " + (this.ColumnsIdentified ? "columnsIdentified" : "columnsNotIdentified");
			ret = this.DdeConsumerClassName + "{Symbols[" + this.QuikStreaming.StreamingDataSnapshot.SymbolsSubscribedAndReceiving + "] " + ret + "}";
			return ret;
		}
	}
}
