using System;
using System.IO;
using System.Text;
using Sq1.Core;

namespace Sq1.Adapters.Quik.Dde.XlDde {
	public class XlReader : IDisposable {
		const int codepage = 1251;
		const int bytes2 = 2;
		const int bytes8 = 8;
		const int bytes16 = bytes2 * 2;
		
		byte[]				byteArrayReceived;
		MemoryStream		ms;
		BinaryReader		br;
		int					blocksize;

		public int			RowsCount			{ get; private set; }
		public int			ColumnsCount		{ get; private set; }

		public bool			BlockIdentified		{ get; private set; }
		public bool			BlockBad			{ get; private set; }
		public XlBlockType	BlockType			{ get; private set; }
		
		public object		ValueJustRead		{ get; private set; }
		
		public XlReader(byte[] byteArrayReceived) {
			this.byteArrayReceived = byteArrayReceived;
			this.ms = new MemoryStream(byteArrayReceived);
			this.br = new BinaryReader(this.ms, Encoding.ASCII);

			if (this.byteArrayReceived.Length < bytes2 * 4) this.SetBadDataStatus();

			XlBlockType messageType = (XlBlockType) br.ReadUInt16();	// [2]
			if (messageType != XlBlockType.Table) this.SetBadDataStatus();

			UInt16 twoDummyBytes = (UInt16) br.ReadUInt16();			// [4] add two dummy bytes?...
			
			this.RowsCount		= br.ReadUInt16();
			if (this.RowsCount == 0) {
				string msg = "CLIENT_SENT_ZERO_ROWS this.ms.Position[" + this.ms.Position + "]";
				string breakpoint = "here";
			}
			this.ColumnsCount	= br.ReadUInt16();
			this.BlockIdentified = false;
		}
		void SetBadDataStatus() {
			this.BlockBad = true;
			this.blocksize = 1;
		}
		public void ReadNext() {
			this.ValueJustRead = null;
			if (this.BlockIdentified == false) {
				if (this.ms.Position + bytes16 > this.byteArrayReceived.Length) {
					throw new EndOfStreamException();
				}
				this.BlockType = (XlBlockType)this.br.ReadUInt16();	//Position=8
				this.BlockBad = false;
				this.blocksize = this.br.ReadUInt16();				//Position=10
				if (this.ms.Position + this.blocksize > this.byteArrayReceived.Length) {
					throw new EndOfStreamException();
				}
			}
			if (this.blocksize <= 0) {
				this.BlockIdentified = false;
				this.ReadNext();
				return;
			}

			if (this.BlockBad) {
				string msg1 = "REFACTOR_BLOCK_BAD";
				Assembler.PopupException(msg1);
				return;
			}

			switch (this.BlockType) {
				case XlBlockType.Float:
					this.blocksize -= bytes8;
					if (this.blocksize >= 0) {
						this.BlockIdentified = true;
						this.ValueJustRead = br.ReadDouble();		// br.Read() cant read (float)s
					} else {
						this.SetBadDataStatus();
					}
					break;
					
				case XlBlockType.String:
					int strlen = ms.ReadByte();
					this.blocksize -= strlen + 1;
					if (this.blocksize >= 0) {
						this.BlockIdentified = true;
						this.ValueJustRead = Encoding.GetEncoding(codepage).GetString(this.byteArrayReceived, (int)this.ms.Position, strlen);
						br.BaseStream.Seek(strlen, SeekOrigin.Current);
					} else {
						this.SetBadDataStatus();
					}
					break;
					
				case XlBlockType.Bool:
				case XlBlockType.Int:
					this.blocksize -= bytes2;
					if (this.blocksize >= 0) {
						this.BlockIdentified = true;
						this.ValueJustRead = br.ReadUInt16();
					} else {
						this.SetBadDataStatus();
					}
					break;

				case XlBlockType.Error:
				case XlBlockType.Blank:
				case XlBlockType.Skip:
					this.blocksize -= bytes2;
					if (this.blocksize >= 0) {
						this.BlockIdentified = true;
						UInt16 toBeIgnored = br.ReadUInt16();
						this.ValueJustRead = null;
					} else {
						this.SetBadDataStatus();
					}
					break;
					
				default:
					this.SetBadDataStatus();
					string msg1 = "ADD_NEW_READER_HANDLER_FOR_NEW_TYPE[" + this.BlockType + "]";
					Assembler.PopupException(msg1);
					break;
			}	// switch
		}
		public void Dispose() {
			br.Dispose();
			ms.Dispose();
		}

		internal void Rewind() {
			string msg = "COLUMNS_ARE_NOT_IDENTIFIED__I_WANTED_TO_FALL_TO_PARSING_WITH_DEBUGGING__BUT_REWIND_IS_UNTESTED";
			Assembler.PopupException(msg);
			this.ms.Seek(0, SeekOrigin.Begin);
		}
	}
}
