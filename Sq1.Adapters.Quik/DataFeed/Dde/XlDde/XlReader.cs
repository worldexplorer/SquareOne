using System;
using System.IO;
using System.Text;

namespace Sq1.Adapters.Quik.Dde.XlDde {
	public sealed class XlReader : IDisposable {
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
		public XlBlockType	ValueType		{ get; private set; }
		public double		FloatValue		{ get; private set; }
		public string		StringValue		{ get; private set; }
		public ushort		WValue			{ get; private set; }
		
		public XlReader(byte[] data) {
			this.data = data;
			this.ms = new MemoryStream(data);
			this.br = new BinaryReader(this.ms, Encoding.ASCII);
			if (this.data.Length < wsize * 4 || (XlBlockType)br.ReadUInt16() != XlBlockType.Table) this.SetBadDataStatus();
			this.ms.Seek(wsize, SeekOrigin.Current);
			this.RowsCount		= br.ReadUInt16();
			this.ColumnsCount	= br.ReadUInt16();
			this.ValueType		= XlBlockType.Unknown;
		}
		void SetBadDataStatus() {
			this.ValueType = XlBlockType.Bad;
			this.blocksize = 1;
		}
		public void ReadValue() {
			this.FloatValue		= float.NaN;
			this.StringValue	= null;
			this.WValue			= ushort.MinValue;
			if (this.ValueType == XlBlockType.Unknown) {
				if (this.ms.Position + hsize > this.data.Length) {
					this.SetBadDataStatus();
				}  else {
					this.ValueType = (XlBlockType)this.br.ReadUInt16();
					this.blocksize = this.br.ReadUInt16();

					if (this.ms.Position + this.blocksize > this.data.Length)
						this.SetBadDataStatus();
				}
			}
			if (this.blocksize <= 0) {
				this.ValueType = XlBlockType.Unknown;
				this.ReadValue();
				return;
			}
			switch (this.ValueType) {
				case XlBlockType.Float:
					this.blocksize -= fsize;
					if (this.blocksize >= 0) {
						this.FloatValue = br.ReadDouble();
					} else {
						this.SetBadDataStatus();
					}
					break;
					
				case XlBlockType.String:
					int strlen = ms.ReadByte();
					this.blocksize -= strlen + 1;
					if (this.blocksize >= 0) {
						this.StringValue = Encoding.GetEncoding(codepage).GetString(this.data, (int)this.ms.Position, strlen);
						br.BaseStream.Seek(strlen, SeekOrigin.Current);
					} else {
						this.SetBadDataStatus();
					}
					break;
					
				case XlBlockType.Bool:
				case XlBlockType.Error:
				case XlBlockType.Blank:
				case XlBlockType.Int:
				case XlBlockType.Skip:
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
			br.Dispose();
			ms.Dispose();
		}
	}
}
