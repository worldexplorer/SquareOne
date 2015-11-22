using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using Sq1.Core;
using Sq1.Core.DataTypes;
using Sq1.Adapters.Quik.Dde.XlDde;
using Sq1.Adapters.QuikLiveism.Dde;

namespace Sq1.Adapters.QuikLivesim.Dde.XlDde {
	public sealed class XlWriter : IDisposable {
		const int codepage = 1251;
		const int wsize = 2;
		const int fsize = 8;
		const int hsize = wsize * 2;
		
		MemoryStream	ms;
		BinaryWriter	bw;
		MemoryStream	msRow;
		BinaryWriter	bwRow;

		XlDdeTableGenerator				xlDdeTableGenerator;
		List<XlColumn>					XlColumnsDescription	{ get { return this.xlDdeTableGenerator.Columns; } }
		Dictionary<string, XlColumn>	XlColumnsLookup			{ get { return this.xlDdeTableGenerator.ColumnsLookup; } }

		int								currentColumn;
		
		List<Dictionary<string, object>>	rows;

		Dictionary<string, object>			rowLast			{ get { return this.rows[this.rows.Count - 1]; } }

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
			//this.ms.Seek(0, SeekOrigin.Begin);	// hope I don't need disposing and re-creation
			foreach (XlColumn col in this.XlColumnsDescription) {
				this.bw.Write((UInt16)XlBlockType.String);
				string colName = col.Name;
				//if (colName.Length > 255) colName = colName.Substring(0, 255);
				//this.bwRow.Write((byte)(colName.Length+1));			// blocksize must include (byte)stringLength + whole string
				this.bw.Write((UInt16)(colName.Length+1));			// blocksize must include (byte)stringLength + whole string
				this.bw.Write((string)colName);
			}
			//long	rowByteArrayLength		= msRow.Length;
			//byte[]	rowByteArrayCapped		= msRow.ToArray();

			//this.bw.Write((UInt16)rowByteArrayLength);	// [10-12] blocksize
			//this.bw.Write(rowByteArrayCapped);			// [10-181] header (15 strings)

			// flushing rows (at least one empty, appended in the constructor)
			foreach (Dictionary<string, object> row in rows) {
				//this.msRow.Seek(0, SeekOrigin.Begin);
				foreach (XlColumn col in this.XlColumnsDescription) {
					if (row.ContainsKey(col.Name) == false) {
						this.bw.Write((UInt16)XlBlockType.Blank);
						this.bw.Write((UInt16)(2));		//blank is 2 bytes wide
						this.bw.Write((UInt16)0xFF);	//and can contain whatever => ignored
						continue;
					}

					object cell = row[col.Name];
					this.bw.Write((UInt16)col.TypeExpected);
					try {
						switch (col.TypeExpected) {
							case XlBlockType.Float:
								this.bw.Write((UInt16)(8));
								this.bw.Write((double)cell);		// bw.Write() cant write (float)s
								break;

							case XlBlockType.Int:	// it must take 2 bytes here!
							case XlBlockType.Bool:
								this.bw.Write((UInt16)(2));
								this.bw.Write((UInt16)cell);
								break;

							case XlBlockType.Error:
							case XlBlockType.Blank:
							case XlBlockType.Skip:
								this.bw.Write((UInt16)(2));		//blank is 2 bytes wide
								this.bw.Write((UInt16)0xFF);	//and can contain whatever => ignored
								break;

							case XlBlockType.String:
								string cellAsString = (string)cell;
								if (cellAsString.Length > 255) cellAsString = cellAsString.Substring(0, 255);
								this.bw.Write((UInt16)(cellAsString.Length+1));	// 256 is max, see XlReader:69 "int strlen = ms.ReadByte();"
								this.bw.Write(cellAsString);
								break;

							case XlBlockType.Unknown:	break;
							case XlBlockType.Table:		break;
							case XlBlockType.Bad:		break;
						}
					} catch (Exception ex) {
						string msg = "cell[ "+ cell + "] col.TypeExpected[" + col.TypeExpected + "]";
						Assembler.PopupException(msg, ex);
					}
				}

				//this.bw.Write((uint)msRow.Length);	// blocksize
				//this.bw.Write(msRow.GetBuffer(), 0, (int)msRow.Length);
				//this.bw.Write(msRow.ToArray());
			}

			//byte[] bufferUncapped = this.ms.GetBuffer();		// lots of zeroes at the end and bufferSize is always 1024??
			//int bufferUncappedMeaningfulLength = (int)this.ms.Length;
			// SAME_LENGTH this.ms.Flush(); int bufferUncappedMeaningfulLengt1 = (int)this.ms.Length;

			//byte[] ddeMessage = new byte[bufferUncappedMeaningfulLength];
			//bufferUncapped.CopyTo(ddeMessage, 0);

			byte[] ddeMessage = this.ms.ToArray();
			
			string msg1 = "ddeMessage.Length=[" + ddeMessage.Length + "]";
			return ddeMessage;
		}
		
		XlWriter() {
			this.ms = new MemoryStream();
			this.bw = new BinaryWriter(this.ms, Encoding.ASCII);
			this.msRow = new MemoryStream();
			this.bwRow = new BinaryWriter(this.msRow, Encoding.ASCII);
			this.rows = new List<Dictionary<string, object>>();
			this.currentColumn = -1;
		}
		public XlWriter(XlDdeTableGenerator xlDdeTableGenerator) : this() {
			this.xlDdeTableGenerator = xlDdeTableGenerator;
			//this.StartNewRow();
			this.rows.Add(new Dictionary<string, object>());
		}

		public void StartNewRow() {
			if (this.currentColumn == 0) {
				string msg = "IGNORING_DUPLICATE__NO_VALUE_APPENDED_SINCE_LAST_StartNewRow()";
				return;
			}
			this.currentColumn = 0;
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
			string msig = " IS_[" + value.GetType() + "] //Put(" + colName + ", " + value + ")";
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
				case XlBlockType.Error:		break;
				case XlBlockType.Blank:		break;
				case XlBlockType.Skip:		break;
				case XlBlockType.Unknown:	break;
				case XlBlockType.Table:		break;
				case XlBlockType.Bad:		break;
			}
			this.rowLast.Add(colName, value);
			this.currentColumn++;
		}

		//public void AppendDouble(string colName, double doubleValue) {
		//    this.currentColumn++;
		//    this.rowLast.Add(colName, doubleValue);
		//}
		//public void AppendString(string colName, string stringValue) {
		//    this.currentColumn++;
		//}
		//public void AppendBool(string colName, bool boolValue) {
		//    this.currentColumn++;
		//}
		//public void AppendInt(string colName, int intValue) {
		//    this.currentColumn++;
		//}

		public void Dispose() {
			//bwRow.Dispose();
			//msRow.Dispose();
			bw.Dispose();
			ms.Dispose();
		}

		internal void StartNewTable() {
			this.rows.Clear();
			this.rows.Add(new Dictionary<string, object>());
		}
	}
}
