// QScalp source code was downloaded on O2-Jun-2012 for free from http://www.qscalp.ru/download/qscalp_src.zip
// SquareOne uses QScalp's modified classes and keeps original author Name and URL
// Nikolay Moroshkin can tell me to remove his code completely => I'll rewrite the pieces borrowed //Pavel Chuchkalov 
//    XlTable.cs (c) 2011 Nikolay Moroshkin, http://www.moroshkin.com/

using System;
using System.IO;
using System.Text;

namespace Sq1.Adapters.Quik.Dde.XlDde {
	public sealed class XlTable : IDisposable {
		public enum BlockType {
			Float	= 0x01,
			String	= 0x02,
			Bool	= 0x03,
			Error	= 0x04,
			Blank	= 0x05,
			Int		= 0x06,
			Skip	= 0x07,
			Table	= 0x10,
			Unknown = 0x10000,
			Bad		= 0x10001
		}
		const int codepage = 1251;
		const int wsize = 2;
		const int fsize = 8;
		const int hsize = wsize * 2;
		
		byte[]			data;
		MemoryStream	ms;
		BinaryReader	br;
		int				blocksize;
		
		public int			RowsCount		{ get; private set; }
		public int			ColumnsCount	{ get; private set; }
		public BlockType	ValueType		{ get; private set; }
		public double		FloatValue		{ get; private set; }
		public string		StringValue		{ get; private set; }
		public ushort		WValue			{ get; private set; }
		
		public XlTable(byte[] data) {
			this.data = data;
			this.ms = new MemoryStream(data);
			this.br = new BinaryReader(this.ms, Encoding.ASCII);
			if (this.data.Length < wsize * 4 || (BlockType)br.ReadUInt16() != BlockType.Table) this.SetBadDataStatus();
			this.ms.Seek(wsize, SeekOrigin.Current);
			this.RowsCount		= br.ReadUInt16();
			this.ColumnsCount	= br.ReadUInt16();
			this.ValueType		= BlockType.Unknown;
		}
		void SetBadDataStatus() {
			this.ValueType = BlockType.Bad;
			this.blocksize = 1;
		}
		public void ReadValue() {
			this.FloatValue		= float.NaN;
			this.StringValue	= null;
			this.WValue			= ushort.MinValue;
			if (this.ValueType == BlockType.Unknown) {
				if (this.ms.Position + hsize > this.data.Length) {
					this.SetBadDataStatus();
				}  else {
					this.ValueType = (BlockType)this.br.ReadUInt16();
					this.blocksize = this.br.ReadUInt16();

					if (this.ms.Position + this.blocksize > this.data.Length)
						this.SetBadDataStatus();
				}
			}
			if (this.blocksize <= 0) {
				this.ValueType = BlockType.Unknown;
				this.ReadValue();
				return;
			}
			switch (this.ValueType) {
				case BlockType.Float:
					this.blocksize -= fsize;
					if (this.blocksize >= 0) {
						this.FloatValue = br.ReadDouble();
					} else {
						this.SetBadDataStatus();
					}
					break;
					
				case BlockType.String:
					int strlen = ms.ReadByte();
					this.blocksize -= strlen + 1;
					if (this.blocksize >= 0) {
						this.StringValue = Encoding.GetEncoding(codepage).GetString(this.data, (int)this.ms.Position, strlen);
						br.BaseStream.Seek(strlen, SeekOrigin.Current);
					} else {
						this.SetBadDataStatus();
					}
					break;
					
				case BlockType.Bool:
				case BlockType.Error:
				case BlockType.Blank:
				case BlockType.Int:
				case BlockType.Skip:
					this.blocksize -= wsize;
					if (this.blocksize >= 0) {
						this.WValue = br.ReadUInt16();
					} else {
						this.SetBadDataStatus();
					}
					break;
					
				default:
					this.SetBadDataStatus();
					break;
			}	// switch
		}
		public void Dispose() {
			//br.Dispose();
			ms.Dispose();
		}
	}
}
