// QScalp source code was downloaded on Apr 2012 for free from http://www.qscalp.ru/download
// SquareOne uses QScalp's modified classes and keeps original author Name and URL
// Nikolay Moroshkin can tell me to remove his code completely => I'll rewrite the pieces borrowed //Pavel Chuchkalov 
//    XlTable.cs (c) 2011 Nikolay Moroshkin, http://www.moroshkin.com/

using System;
using System.IO;
using System.Text;

namespace Sq1.Adapters.Quik.Dde.XlDde {
	public sealed class XlTable : IDisposable {
		public enum BlockType {
			Float = 0x01,
			String = 0x02,
			Bool = 0x03,
			Error = 0x04,
			Blank = 0x05,
			Int = 0x06,
			Skip = 0x07,
			Table = 0x10,
			Unknown = 0x10000,
			Bad = 0x10001
		}
		const int codepage = 1251;
		const int wsize = 2;
		const int fsize = 8;
		const int hsize = wsize * 2;
		byte[] data;
		MemoryStream ms;
		BinaryReader br;
		int blocksize;
		public int RowsCount { get; private set; }
		public int ColumnsCount { get; private set; }
		public BlockType ValueType { get; private set; }
		public double FloatValue { get; private set; }
		public string StringValue { get; private set; }
		public ushort WValue { get; private set; }
		public XlTable(byte[] data) {
			this.data = data;
			ms = new MemoryStream(data);
			br = new BinaryReader(ms, Encoding.ASCII);
			if (data.Length < wsize * 4 || (BlockType)br.ReadUInt16() != BlockType.Table)
				SetBadDataStatus();
			ms.Seek(wsize, SeekOrigin.Current);
			RowsCount = br.ReadUInt16();
			ColumnsCount = br.ReadUInt16();
			ValueType = BlockType.Unknown;
		}
		void SetBadDataStatus() {
			ValueType = BlockType.Bad;
			blocksize = 1;
		}
		public void ReadValue() {
			FloatValue = float.NaN;
			StringValue = null;
			WValue = ushort.MinValue;
			if (ValueType == BlockType.Unknown) {
				if (ms.Position + hsize > data.Length) {
					SetBadDataStatus();
				}  else {
					ValueType = (BlockType)br.ReadUInt16();
					blocksize = br.ReadUInt16();

					if (ms.Position + blocksize > data.Length)
						SetBadDataStatus();
				}
			}
			if (blocksize > 0)
				switch (ValueType) {
					case BlockType.Float:
						blocksize -= fsize;
						if (blocksize >= 0)
							FloatValue = br.ReadDouble();
						else
							SetBadDataStatus();
						break;
					case BlockType.String:
						int strlen = ms.ReadByte();
						blocksize -= strlen + 1;
						if (blocksize >= 0) {
							StringValue = Encoding.GetEncoding(codepage).GetString(data, (int)ms.Position, strlen);
							br.BaseStream.Seek(strlen, SeekOrigin.Current);
						} else
							SetBadDataStatus();

						break;
					case BlockType.Bool:
					case BlockType.Error:
					case BlockType.Blank:
					case BlockType.Int:
					case BlockType.Skip:
						blocksize -= wsize;

						if (blocksize >= 0)
							WValue = br.ReadUInt16();
						else
							SetBadDataStatus();
						break;
					default:
						SetBadDataStatus();
						break;
			} else {
				ValueType = BlockType.Unknown;
				ReadValue();
			}
		}
		public void Dispose() {
			//br.Dispose();
			ms.Dispose();
		}
	}
}
