using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using Sq1.Core;
using Sq1.Core.DataTypes;

using Sq1.Adapters.Quik.Dde.XlDde;
using Sq1.Adapters.QuikLiveism.Dde;

namespace Sq1.Adapters.QuikLivesim.Dde.XlDde {
	public class XlWriter : IDisposable {
		const int codepage = 1251;
		
		MemoryStream	ms;
		BinaryWriter	bw;

		XlDdeTableGenerator					xlDdeTableGenerator;
		List<XlColumn>						XlColumnsDescription	{ get { return this.xlDdeTableGenerator.Columns; } }
		Dictionary<string, XlColumn>		XlColumnsLookup			{ get { return this.xlDdeTableGenerator.ColumnsLookup; } }

		int									currentColumn;
		int									currentRow;
		
		List<Dictionary<string, object>>	rows;

		Dictionary<string, object>			rowLast			{ get { return this.rows[this.rows.Count - 1]; } }

		XlWriter() {
			this.ms = new MemoryStream();
			this.bw = new BinaryWriter(this.ms, Encoding.ASCII);
			this.rows = new List<Dictionary<string, object>>();
			this.currentColumn	= -1;
			this.currentRow		= -1;
		}
		public XlWriter(XlDdeTableGenerator xlDdeTableGenerator) : this() {
			this.xlDdeTableGenerator = xlDdeTableGenerator;
			//this.StartNewRow();
			this.rows.Add(new Dictionary<string, object>());	// that's for the header
		}

		public byte[] ConvertToXlDdeMessage() {
			//this.Dispose();
			//this.ms = new MemoryStream();
			//this.bw = new BinaryWriter(this.ms, Encoding.ASCII);

			this.ms.Seek(0, SeekOrigin.Begin);						// hope I don't need disposing and re-creation
			this.bw.Write((UInt16)XlBlockType.Table);				// [0-2]
			this.bw.Write((UInt16)0xFF);							// [2-4] XlReader expects two dummy bytes more prior to rows count?
			this.bw.Write((UInt16)(this.rows.Count + 1));			// [4-6] at least I will dump DDE table header (which must be non-empty) and one empty data row
			this.bw.Write((UInt16)this.XlColumnsDescription.Count);	// [6-8] number of columns

			// flushing the header
			int currentRowWhileFlushing = 1;
			int currentColumnWhileFlushing = 0;
			foreach (XlColumn col in this.XlColumnsDescription) {
				currentColumnWhileFlushing++;
				this.bw.Write((UInt16)XlBlockType.String);
				string colName = col.Name;
				if (colName.Length > 255) colName = colName.Substring(0, 255);
				this.bw.Write((UInt16)(colName.Length+1));			// blocksize must include (byte)stringLength + whole string
				this.bw.Write((string)colName);
			}

			// flushing rows (at least one empty, appended in the constructor)
			foreach (Dictionary<string, object> row in rows) {
				currentRowWhileFlushing++;
				currentColumnWhileFlushing = 0;
				foreach (XlColumn col in this.XlColumnsDescription) {
					currentColumnWhileFlushing++;
					if (row.ContainsKey(col.Name) == false) {
						this.bw.Write((UInt16)XlBlockType.Blank);
						this.bw.Write((UInt16)2);		//blank is 2 bytes wide
						this.bw.Write((UInt16)0xFF);	//and can contain whatever => ignored
						continue;
					}

					object cell = row[col.Name];

					if (col.Mandatory == false && cell == null) {
						this.bw.Write((UInt16)XlBlockType.Blank);
						this.bw.Write((UInt16)2);		//blank is 2 bytes wide
						this.bw.Write((UInt16)0xFF);	//and can contain whatever => ignored
						continue;
					}
		
					this.bw.Write((UInt16)col.TypeExpected);
					try {
						switch (col.TypeExpected) {
							case XlBlockType.Float:
								this.bw.Write((UInt16)8);
								this.bw.Write((double)cell);		// bw.Write() cant write (float)s
								break;

							case XlBlockType.Int:	// it must take 2 bytes here!
							case XlBlockType.Bool:
								this.bw.Write((UInt16)2);
								this.bw.Write((UInt16)cell);
								break;

							case XlBlockType.Error:
							case XlBlockType.Blank:
							case XlBlockType.Skip:
								this.bw.Write((UInt16)2);		//blank is 2 bytes wide
								this.bw.Write((UInt16)0xFF);	//and can contain whatever => ignored
								break;

							case XlBlockType.String:
								string cellAsString = (string)cell;
								if (cellAsString.Length > 255) cellAsString = cellAsString.Substring(0, 255);
								this.bw.Write((UInt16)(cellAsString.Length+1));	// 256 is max, see XlReader:69 "int strlen = ms.ReadByte();"
								this.bw.Write(cellAsString);
								break;

							case XlBlockType.Table:
								string msg = "NO_NESTED_TABLES_NEEDED_HERE";
								Assembler.PopupException(msg);
								break;

							default:
								string msg1 = "ADD_NEW_WRITER_HANDLER_FOR_NEW_TYPE[" + col.TypeExpected + "]";
								Assembler.PopupException(msg1);
								break;
						}
					} catch (Exception ex) {
						string msg = "cell[ "+ cell + "] col.TypeExpected[" + col.TypeExpected + "]";
						Assembler.PopupException(msg, ex);
					}
				}
			}
			byte[] ddeMessage = this.ms.ToArray();
			string msg2 = "ddeMessage.Length=[" + ddeMessage.Length + "]";
			return ddeMessage;
		}
		
		public void StartNewRow() {
			if (this.currentColumn == 0) {
				string msg = "IGNORING_DUPLICATE__NO_VALUE_APPENDED_SINCE_LAST_StartNewRow()";
				return;
			}
			this.currentColumn = 0;
			this.currentRow++;
			this.rows.Add(new Dictionary<string, object>());
		}

		public void Put(string colName, object value) {
			if (this.XlColumnsLookup.ContainsKey(colName) == false) {
				string msg = "YOU_ARE_ADDING_COLUMN_WHICH_DOESNT_EXIST_IN_TABLE_DEFINITIONS this.XlColumnsLookup.ContainsKey(" + colName + ") == false";
				Assembler.PopupException(msg);
				return;
			}
			if (this.rowLast.ContainsKey(colName)) {
				string msg = "YOU_ALREADY_ADD_COLUMN [" + colName + "]";
				Assembler.PopupException(msg);
				return;
			}

			XlColumn colForName = this.XlColumnsLookup[colName];

			string type = value == null ? "NULL" : value.GetType().Name;
			string msig = " IS_[" + type + "] //Put(" + colName + ", " + value + ")";

			if (value == null && colForName.Mandatory == false) {
				this.rowLast.Add(colName, value);	//yes add null
				this.currentColumn++;
				return;
			}

			switch (colForName.TypeExpected) {
				case XlBlockType.Float:
					if ((value is double) == false) {
						Assembler.PopupException("MUST_BE_DOUBLE" + msig);
						return;
					}
					break;
				case XlBlockType.Int:	// it must take 2 bytes here!
					if ((value is Int16) == false) {
						Assembler.PopupException("MUST_BE_Int16" + msig);
						return;
					}
					break;
				case XlBlockType.String:
					if ((value is string) == false) {
						Assembler.PopupException("MUST_BE_STRING" + msig);
						return;
					}
					break;
				case XlBlockType.Bool:
					if ((value is bool) == false) {
						Assembler.PopupException("MUST_BE_BOOL");
						return;
					}
					break;
				case XlBlockType.Blank:
					if (value != null) {
						Assembler.PopupException("MUST_BE_NULL");
						return;
					}
					break;
				case XlBlockType.Error:		break;
				case XlBlockType.Skip:		break;
				case XlBlockType.Table:		break;
				//case XlBlockType.Unknown:	break;
				//case XlBlockType.Bad:		break;
			}
			this.rowLast.Add(colName, value);
			this.currentColumn++;
		}

		public void Dispose() {
			bw.Dispose();
			ms.Dispose();
		}

		internal void StartNewTable() {
			this.rows.Clear();
			this.rows.Add(new Dictionary<string, object>());
		}
	}
}
