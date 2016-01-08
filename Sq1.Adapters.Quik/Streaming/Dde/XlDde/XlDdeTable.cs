using System;
using System.Collections.Generic;
using System.IO;

using Sq1.Core;

namespace Sq1.Adapters.Quik.Dde.XlDde {
	public abstract class XlDdeTable {
		protected abstract	string					DdeConsumerClassName	{ get; }
		public				string					Topic					{ get; private set; }
		public				DateTime				LastDataReceived		{ get; protected set; }
		public	virtual		bool					ReceivingDataDde		{ get; set; }
		public				long					DdeTablesReceived		{ get; set; }

		protected	List<XlColumn>					ColumnDefinitions;				// part of the abstraction to implement in children
		protected	Dictionary<string, XlColumn>	ColumnDefinitionsByNameLookup;

		protected	Dictionary<int, XlColumn>		ColumnClonesFoundByIndex;
		protected	bool							ColumnsIdentified;
		protected	QuikStreaming					QuikStreaming			{ get; private set; }

		XlDdeTable() {
			this.ColumnsIdentified			= false;
			this.ColumnDefinitions			= new List<XlColumn>();			// part of the abstraction to implement in children
			this.ColumnClonesFoundByIndex	= new Dictionary<int, XlColumn>();
		}
		protected XlDdeTable(string topic, QuikStreaming quikStreaming, List<XlColumn> columns) : this() {
			this.Topic = topic;
			this.QuikStreaming = quikStreaming;
			this.ColumnDefinitions = columns;

			this.ColumnDefinitionsByNameLookup = new Dictionary<string, XlColumn>();
			foreach (XlColumn col in this.ColumnDefinitions) {
				this.ColumnDefinitionsByNameLookup.Add(col.Name, col);
			}
		}

		public void ParseDeliveredDdeData_pushToStreaming(byte[] data) {
			this.LastDataReceived = DateTime.UtcNow;

			this.IncomingTableBegun();
			using (XlReader reader = new XlReader(data)) {
				this.ParseMessage_readAsTable_convertEachRowToDataStructures(reader);
			}
			this.IncomingTableTerminated();
			this.DdeTablesReceived++;
		}
		protected virtual void ParseMessage_readAsTable_convertEachRowToDataStructures(XlReader reader) {
			// IDENTIFY_EACH_NEW_MESSAGE_DONT_CACHE if (this.ColumnsIdentified == false) {
				this.ColumnsIdentified = this.identifyColumnsByReadingHeader(reader);
				if (this.ColumnsIdentified == false) {
					reader.Rewind();
					this.ColumnsIdentified = this.identifyColumnsByReadingHeader(reader, true);
					return;
				}
				if (reader.RowsCount == 1) return;
			//}

			for (int i = 1; i < reader.RowsCount; i++) {
				XlRowParsed rowParsed = this.parseRow(reader);
				if (rowParsed == null || rowParsed.Count == 0) continue;

				#region CONSISTENCY_CHECK
				foreach (XlColumn col in this.ColumnDefinitions) {
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
					this.IncomingTableRow_convertToDataStructure(rowParsed);
				} catch (Exception ex) {
					string msg = "[" + this.LastDataReceived.ToString("HH:mm:ss.fff ddd dd MMM yyyy") + "]";
					Assembler.PopupException(msg, ex);
				}
			}
		}
		protected bool identifyColumnsByReadingHeader(XlReader reader, bool debugMandatoryNotFound = false) {
			bool ret = false;
			List<XlColumn> mandatoriesNotFound = new List<XlColumn>();
			this.ColumnClonesFoundByIndex.Clear();
			foreach (XlColumn col in this.ColumnDefinitions) {
				col.IndexFound = -1;
				if (col.Mandatory) mandatoriesNotFound.Add(col);
			}
			for (int colSerno = 0; colSerno < reader.ColumnsCount; colSerno++) {
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
				foreach (XlColumn colDef in this.ColumnDefinitions) {
					if (colDef.Name != columnNameToIdentify) continue;
					XlColumn colFound = colDef.Clone();
					colFound.IndexFound = colSerno;
					this.ColumnClonesFoundByIndex.Add(colSerno, colFound);
					if (mandatoriesNotFound.Contains(colDef)) mandatoriesNotFound.Remove(colDef);
					if (debugMandatoryNotFound) {
						Assembler.PopupException("MANDATORY_NOT_FOUND");
					}
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
				if (this.ColumnClonesFoundByIndex.ContainsKey(col) == false) continue;

				XlColumn xlCol = this.ColumnClonesFoundByIndex[col].Clone();

				
				if (reader.BlockType == XlBlockType.Blank) {
					switch (xlCol.TypeExpected) {
						case XlBlockType.Float:		rowParsed.Add_popupIfDuplicate(xlCol.Name, double.NaN);	break;
						//case XlBlockType.String:	rowParsed.Add_popupIfDuplicate(xlCol.Name, double.NaN);	break;
						//case XlBlockType.Bool:		rowParsed.Add_popupIfDuplicate(xlCol.Name, double.NaN);	break;
						default:					rowParsed.Add_popupIfDuplicate(xlCol.Name, null);		break;
					}
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

		protected virtual	void	IncomingTableBegun() { }
		protected abstract	void	IncomingTableRow_convertToDataStructure(XlRowParsed row);	// you can push to Streaming here (doesn't make sense to commit quotes to QuikStreaming as a table)
		protected virtual	void	IncomingTableTerminated() { }				//  or you can push to Streaming here (it is more consistent to unlock per-symbol DepthOfMarket as whole table so that QuikStreaming Level2 consumers will get the whole frame at once)

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
