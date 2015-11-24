using System;
using System.Collections.Generic;
using System.Globalization;

using Sq1.Core;
using System.IO;

namespace Sq1.Adapters.Quik.Dde.XlDde {
	public abstract class XlDdeTable {
		protected abstract	string				DdeConsumerClassName	{ get; }
		public				string				Topic					{ get; private set; }
		public				DateTime			LastDataReceived		{ get; protected set; }
		public	virtual		bool				ReceivingDataDde		{ get; set; }

		protected	List<XlColumn>					Columns;				// part of the abstraction to implement in children
		protected	Dictionary<string, XlColumn>	ColumnsLookup;

		protected	Dictionary<int, XlColumn>		ColumnsByIndexFound;
		protected	bool							ColumnsIdentified;
		protected	QuikStreaming					QuikStreaming			{ get; private set; }

		XlDdeTable() {
			this.ColumnsIdentified		= false;
			this.Columns				= new List<XlColumn>();			// part of the abstraction to implement in children
			this.ColumnsByIndexFound	= new Dictionary<int, XlColumn>();
		}
		protected XlDdeTable(string topic, QuikStreaming quikStreaming, List<XlColumn> columns) : this() {
			this.Topic = topic;
			this.QuikStreaming = quikStreaming;
			this.Columns = columns;

			this.ColumnsLookup = new Dictionary<string, XlColumn>();
			foreach (XlColumn col in this.Columns) {
				this.ColumnsLookup.Add(col.Name, col);
			}
		}

		public void ParseDeliveredDdeData_pushToStreaming(byte[] data) {
			this.LastDataReceived = DateTime.UtcNow;

			this.IncomingTableBegun();
			using (XlReader reader = new XlReader(data)) {
				this.FillTable_readParseMessage(reader);
			}
			this.IncomingTableTerminated();
		}
		protected virtual void FillTable_readParseMessage(XlReader reader) {
			// IDENTIFY_EACH_NEW_MESSAGE_DONT_CACHE if (this.ColumnsIdentified == false) {
				this.ColumnsIdentified = this.identifyColumnsByReadingHeader(reader);
				if (this.ColumnsIdentified == false) return;
				if (reader.RowsCount == 1) return;
			//}

			for (int i = 1; i < reader.RowsCount; i++) {
				XlRowParsed rowParsed = this.parseRow(reader);
				if (rowParsed == null || rowParsed.Count == 0) continue;
				string columnName = Columns[0].Name;
				if (rowParsed[columnName] == null) continue;
				if (rowParsed[columnName] is string) {
					string cellParced = (string) rowParsed[columnName];
					if (cellParced == columnName) continue;
				}

				#region CONSISTENCY_CHECK
				foreach (XlColumn col in this.Columns) {
					if (rowParsed.ContainsKey(col.Name) == false) {
						string msg = "I_EXPECTED_ALL_COLUMNS_BE_PRESENT__THEY_JUST_MUST";
						continue;
					}
					object valueParsed = rowParsed[col.Name];
					if (valueParsed == null) {
						string msg = "JUST_IGNORE?...";
						continue;
					}
					string typeParsed = valueParsed.GetType().Name;
					if (typeParsed == "Double") typeParsed = "Float";	// BinaryReader/Writer can't read/write (float)s, so I transfer doubles and consume doubles in Streaming
					string typeExpected = col.TypeExpected.ToString();
					if (typeParsed == "DateTime") typeParsed = "String";
					if (typeParsed == typeExpected) continue;

					this.ColumnsIdentified = false;
					string msg1 = "TYPE_MISTMATCH col.TypeExpected[" + col.TypeExpected + "]!=typeParsed[" + typeParsed + "]"
						+ " rowParsed[" + col.Name + "][" + valueParsed + "]"
						+ ", skipping quote and re-identifyingColumnsByReadingHeader";
					Assembler.PopupException(msg1);
				}
				#endregion

				try {
					this.IncomingRowParsedDelivered(rowParsed);
				} catch (Exception ex) {
					string msg = "[" + this.LastDataReceived.ToString("HH:mm:ss.fff ddd dd MMM yyyy") + "]";
					Assembler.PopupException(msg, ex);
				}
			}
		}
		protected bool identifyColumnsByReadingHeader(XlReader reader) {
			bool ret = false;
			List<XlColumn> mandatoriesNotFound = new List<XlColumn>();
			this.ColumnsByIndexFound.Clear();
			foreach (XlColumn col in this.Columns) {
				col.IndexFound = -1;
				if (col.Mandatory) mandatoriesNotFound.Add(col);
			}
			for (int col_serno = 0; col_serno < reader.ColumnsCount; col_serno++) {
				try {
					reader.ReadNext();
				} catch (EndOfStreamException ex) {
					string errmsg3 = "ARE_YOU_READING_17_COLUMN_WHEN_ONLY_16_WERE_TRANSMITTED?";
					Assembler.PopupException(errmsg3, ex);
					return ret;
				}

				if (reader.BlockType != XlBlockType.String) {
					string msg = "FIRST_HEADER_ROW_MUST_CONTAIN_ONLY_STRINGS";
					Assembler.PopupException(msg);
					continue;
				}
				string columnNameToIdentify = (string) reader.ValueJustRead;
				foreach (XlColumn col in this.Columns) {
					if (col.Name != columnNameToIdentify) continue;
					this.ColumnsByIndexFound.Add(col_serno, col.Clone());
					if (mandatoriesNotFound.Contains(col)) mandatoriesNotFound.Remove(col);
					break;
				}
			}
			ret = mandatoriesNotFound.Count == 0;
			if (ret == false) {
				string columnsNotReceived = "";
				foreach (XlColumn colNF in mandatoriesNotFound) {
					columnsNotReceived += colNF.Name + ",";
				}
				columnsNotReceived = columnsNotReceived.TrimEnd(',');
				string msg = "MANDATORY_COLUMNS_NOT_RECEIVED: [" + columnsNotReceived + "]";
				Assembler.PopupException(msg);
			}
			return ret;
		}
		protected virtual XlRowParsed parseRow(XlReader reader) {
			XlRowParsed rowParsed = new XlRowParsed(this.DdeConsumerClassName);
			for (int col = 0; col < reader.ColumnsCount; col++) {
				try {
					reader.ReadNext();
				} catch (EndOfStreamException ex) {
					string errmsg3 = "ARE_YOU_READING_3RD_ROW_WHEN_ONLY_2_WERE_TRANSMITTED?";
					Assembler.PopupException(errmsg3, ex);
					return rowParsed;
				}
				if (this.ColumnsByIndexFound.ContainsKey(col) == false) continue;

				XlColumn xlCol = this.ColumnsByIndexFound[col].Clone();

				
				if (reader.BlockType == XlBlockType.Blank) {
					rowParsed.Add_popupIfDuplicate(xlCol.Name, null);
					continue;
				}

				if (reader.BlockType != xlCol.TypeExpected) {
					string errmsg3 = "GOT[" + reader.BlockType + "] INSTEAD_OF TypeExpected[" + xlCol.TypeExpected + "] FOR xlCol[" + xlCol.Name + "]";
					Assembler.PopupException(errmsg3);
					//continue;
				}

				object valueReceived = reader.ValueJustRead;
				if (valueReceived == null) {
					string errmsg3 = "MUST_NOT_BE_NULL reader.ValueJustRead[" + reader.ValueJustRead + "] BlockType[" + reader.BlockType + "] FOR xlCol[" + xlCol.Name + "]";
					Assembler.PopupException(errmsg3);
				}

				rowParsed.Add_popupIfDuplicate(xlCol.Name, valueReceived);
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
