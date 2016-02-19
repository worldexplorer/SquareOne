using System;
using System.Collections.Generic;
using System.IO;

using Sq1.Core;

namespace Sq1.Adapters.Quik.Streaming.Dde.XlDde {
	public abstract class XlDdeTable {
		protected abstract	string					DdeConsumerClassName	{ get; }
		public				string					Topic					{ get; private set; }
		public				DateTime				LastDataReceived		{ get; private set; }

							bool					topicConnected;
		public	virtual		bool					TopicConnected			{
			get { return this.topicConnected; }
			set { if (value == true) { this.DdeConnectedTimes++; } this.topicConnected = value; }
		}

		public	virtual		int						DdeConnectedTimes		{ get; private set; }
	
		public				long					DdeMessagesReceived		{ get; private set; }
		public				long					DdeRowsReceived			{ get; private set; }

		protected	List<XlColumn>					ColumnDefinitions;				// part of the abstraction to implement in children
		protected	Dictionary<string, XlColumn>	ColumnDefinitionsByNameLookup;

		public		bool							DdeWillDeliver_updatesForEachRow_inSeparateMessages			{ get; private set; }
		public		string							PrimaryKey_forEachRowUpdate									{ get; private set; }

		protected	Dictionary<int, XlColumn>		ColumnClonesFoundByIndex;
		protected	bool							ColumnsIdentified;
		protected	QuikStreaming					QuikStreaming			{ get; private set; }

		XlDdeTable() {
			this.ColumnDefinitions			= new List<XlColumn>();			// part of the abstraction to implement in children
			this.ColumnClonesFoundByIndex	= new Dictionary<int, XlColumn>();
			this.ColumnsIdentified			= false;	// sure it's false			at consctruction time
			this.ResetCounters();						// sure they are zeroes		at consctruction time
		}
		protected XlDdeTable(string topic, QuikStreaming quikStreaming, List<XlColumn> columns, bool oneRowUpdates) : this() {
			this.Topic = topic;
			this.QuikStreaming = quikStreaming;
			this.ColumnDefinitions = columns;
			this.DdeWillDeliver_updatesForEachRow_inSeparateMessages = oneRowUpdates;

			if (this.DdeWillDeliver_updatesForEachRow_inSeparateMessages) {
				foreach (XlColumn eachColumn in this.ColumnDefinitions) {
					if (eachColumn.PrimaryKey_forSingleUpdates == false) continue;
					this.PrimaryKey_forEachRowUpdate = eachColumn.Name;
					break;
				}
				if (string.IsNullOrEmpty(this.PrimaryKey_forEachRowUpdate)) {
					string msg = "YOU_DIDNT_MARK_ONE_COLUMN_AS_PRIMARY_KEY Topic[" + topic + "] ColumnDefinitions[" + this.columnsAsString(this.ColumnDefinitions) + "] //XlDdeTable.ctor()";
					Assembler.PopupException(msg);
				}
			}

			this.ColumnDefinitionsByNameLookup = new Dictionary<string, XlColumn>();
			foreach (XlColumn col in this.ColumnDefinitions) {
				this.ColumnDefinitionsByNameLookup.Add(col.Name, col);
			}
		}

		public void ParseDdeDeliveredMessage_pushToStreaming(byte[] data) {
			this.LastDataReceived = DateTime.UtcNow;

			this.IncomingTableBegun();
			using (XlReader reader = new XlReader(data)) {
				this.ParseMessage_readAsTable_convertEachRowToDataStructures(reader);
				this.DdeRowsReceived++;
			}
			this.IncomingTableTerminated();
			this.DdeMessagesReceived++;
		}
		protected virtual void ParseMessage_readAsTable_convertEachRowToDataStructures(XlReader reader) {
			int rowsIhaveReadAlready = 0;
			if (this.DdeWillDeliver_updatesForEachRow_inSeparateMessages == false) this.ColumnsIdentified = false;			// IDENTIFY_EACH_NEW_MESSAGE_DONT_CACHE
			if (this.ColumnsIdentified == false) {
				this.ColumnsIdentified = this.identifyColumnsByReadingHeader(reader);
				rowsIhaveReadAlready++;
				if (this.ColumnsIdentified == false) {
					//JUST_DROP_IT reader.Rewind();
					//this.ColumnsIdentified = this.identifyColumnsByReadingHeader(reader, true);
					return;
				} else {
					if (this.DdeWillDeliver_updatesForEachRow_inSeparateMessages) {
						string msg = "COLUMNS_IDENTIFIED[" + this.ColumnsIdentifiedAsString + "]";
						Assembler.PopupException(msg, null, false);

						string msg2 = "COLUMNS_IN_MESSAGE[" + this.ColumnsInMessageAsString + "]";
						Assembler.PopupException(msg2, null, false);
					}
				}
				if (reader.RowsCount == 1) return;
			}

			for (int i = rowsIhaveReadAlready; i < reader.RowsCount; i++) {
				XlRowParsed rowParsed = this.parseRow(reader);
				if (rowParsed == null || rowParsed.Count == 0) continue;

				#region CONSISTENCY_CHECK
				foreach (XlColumn col in this.ColumnDefinitions) {
					if (rowParsed.ContainsKey(col.Name) == false) {
						string msg = "ALL_COLUMNS_MUST_BE_PRESENT__CAN_NOT_BE_NULL col.Name[" + col.Name + "] IS_MISSING_IN rowParsed[" + rowParsed + "]";
						//Assembler.PopupException(msg, null, false);
						continue;
					}
					object valueParsed = rowParsed[col.Name];
					if (valueParsed == null) {
						string msg = "JUST_IGNORE?...";
						continue;
					}
					Type typeParsed = valueParsed.GetType();
					string typeParsedAsString = typeParsed.Name;
					if (typeParsedAsString == "Single") continue;	// I dont know how to deal with that!!! what is a Type=Single??

					if (typeParsedAsString == "Double") typeParsedAsString = "Float";	// BinaryReader/Writer can't read/write (float)s, so I transfer doubles and consume doubles in Streaming
					string typeExpected = col.TypeExpected.ToString();
					if (typeParsedAsString == "DateTime") typeParsedAsString = "String";
					if (typeParsedAsString == typeExpected) continue;

					this.ColumnsIdentified = false;
					string msg1 = "TYPE_MISTMATCH col.TypeExpected[" + col.TypeExpected + "]!=typeParsed[" + typeParsed + "]"
						+ " rowParsed[" + col.Name + "][" + valueParsed + "]"
						+ ", skipping quote and re-identifyingColumnsByReadingHeader";
					Assembler.PopupException(msg1, null, false);
				}
				#endregion

				try {
					this.IncomingTableRow_convertToDataStructure(rowParsed);
				} catch (Exception ex) {
					string msg = "[" + this.LastDataReceived.ToString("HH:mm:ss.fff ddd dd MMM yyyy") + "]";
					Assembler.PopupException(msg, ex, false);
				}
			}
		}
		List<XlColumn> columnsInMessageFoundSoFar = new List<XlColumn>();
		protected bool identifyColumnsByReadingHeader(XlReader reader, bool debugMandatoryNotFound = false) {
			bool ret = false;
			List<XlColumn> mandatoriesNotFound = new List<XlColumn>();
			this.ColumnClonesFoundByIndex.Clear();
			foreach (XlColumn col in this.ColumnDefinitions) {
				col.IndexFound = -1;
                if (col.Mandatory == false) continue;
                mandatoriesNotFound.Add(col);
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
					Assembler.PopupException(msg, null, false);
					continue;
				}

				string columnNameToIdentify = Convert.ToString(reader.ValueJustRead);
				this.columnsInMessageFoundSoFar.Add(new XlColumn(reader.BlockType, columnNameToIdentify));

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

				string msg = "MANDATORY_COLUMNS_NOT_RECEIVED: [" + columnsNotReceived + "]"
					+ " columnsIdentifiedAsString[" + this.ColumnsIdentifiedAsString + "]"
					+ " columnsInMessageAsString[" + this.ColumnsInMessageAsString + "]";
				Assembler.PopupException(msg, null, false);
			}
			return ret;
		}

		public string ColumnsIdentifiedAsString { get { return this.columnsAsString(new List<XlColumn>(this.ColumnClonesFoundByIndex.Values)); } }
		public string ColumnsInMessageAsString { get { return this.columnsAsString(this.columnsInMessageFoundSoFar); } }
		private string columnsAsString(List<XlColumn> listOfXlColumns) {
			string columnsInMessageAsString = "";
			foreach (XlColumn columnInMessage in listOfXlColumns) {
				string asString = columnInMessage.TypeExpected + ":" + columnInMessage.Name;
				if (columnsInMessageAsString != "") columnsInMessageAsString += "," + Environment.NewLine;
				columnsInMessageAsString += asString;
			}
			return columnsInMessageAsString;
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


				if (reader.BlockType == XlBlockType.Blank || reader.BlockType == XlBlockType.Skip) {
					switch (xlCol.TypeExpected) {
						case XlBlockType.Float:		rowParsed.Add_popupIfDuplicate(xlCol.Name, double.NaN);	break;
						//case XlBlockType.String:	rowParsed.Add_popupIfDuplicate(xlCol.Name, double.NaN);	break;
						//case XlBlockType.Bool:		rowParsed.Add_popupIfDuplicate(xlCol.Name, double.NaN);	break;
						default:					rowParsed.Add_popupIfDuplicate(xlCol.Name, null);		break;
					}
					continue;
				}

                object valueReceived = reader.ValueJustRead;
				if (valueReceived == null) {
					string errmsg3 = "MUST_NOT_BE_NULL reader.ValueJustRead[" + reader.ValueJustRead + "] BlockType[" + reader.BlockType + "] FOR xlCol[" + xlCol.Name + "]";
					Assembler.PopupException(errmsg3);
				} else {
				    if (reader.BlockType != xlCol.TypeExpected) {
                        if (xlCol.TypeExpected == XlBlockType.Float && reader.BlockType == XlBlockType.String) {
                            string valueReceivedAsString = valueReceived.ToString();
                            float parsedAsFloat = float.NaN;
                            float.TryParse(valueReceivedAsString, out parsedAsFloat);
                            valueReceived = parsedAsFloat;
                        } else {
					        string errmsg3 = "AUTOCONVERT_UNAVAILABLE_FOR: GOT[" + reader.BlockType + "] INSTEAD_OF TypeExpected[" + xlCol.TypeExpected + "] FOR xlCol[" + xlCol.Name + "]";
					        Assembler.PopupException(errmsg3, null, false);
					        //continue;
                        }
				    }
                }

				rowParsed.Add_popupIfDuplicate(xlCol.Name, valueReceived);
			}
			return rowParsed;
		}

		protected virtual	void	IncomingTableBegun() { }
		protected abstract	void	IncomingTableRow_convertToDataStructure(XlRowParsed row);	// you can push to Streaming here (doesn't make sense to commit quotes to QuikStreaming as a table)
		protected virtual	void	IncomingTableTerminated() { }				//  or you can push to Streaming here (it is more consistent to unlock per-symbol DepthOfMarket as whole table so that QuikStreaming Level2 consumers will get the whole frame at once)

		
		internal void ResetCounters() {
			this.DdeMessagesReceived = 0;
			this.DdeRowsReceived = 0;
		}

		public override string ToString() {
			string ret = this.DdeConsumerClassName;

			if (string.IsNullOrEmpty(this.Topic) == false) ret += " [" + this.Topic + "] ";

			ret += " "	+ this.DdeMessagesReceived;
			ret += ":"	+ this.DdeRowsReceived;

			string connectionStatus = "NEVER_CONNECTED_DDE";
			if (this.DdeConnectedTimes > 0) {
				if (this.TopicConnected) {
					connectionStatus = "LISTENING_DDE#";
					if (this.DdeMessagesReceived > 0) connectionStatus = "RECEIVING_DDE#";
				} else {
					connectionStatus = "DISCONNECTED_DDE#";
				}
				connectionStatus += this.DdeConnectedTimes;
			}

			ret += " "	+ connectionStatus;
			ret += " Symbols[" + this.QuikStreaming.StreamingDataSnapshot.SymbolsSubscribedAndReceiving + "]";
			ret += " " + (this.ColumnsIdentified ? "columnsIdentified" : "columnsNotIdentified");
			return ret;
		}
	}
}
