using System;
using System.IO;
using System.Text;

namespace Sq1.Adapters.Quik.Dde.XlDde {
	public sealed class XlReader : IDisposable {
		const int codepage = 1251;
		const int wsize = 2;
		const int fsize = 8;
		const int hsize = wsize * 2;
		
		byte[]			byteArrayReceived;
		MemoryStream	ms;
		BinaryReader	br;
		int				blocksize;
		
		public int			RowsCount		{ get; private set; }
		public int			ColumnsCount	{ get; private set; }
		public XlBlockType	ValueType		{ get; private set; }
		public double		FloatValue		{ get; private set; }
		public string		StringValue		{ get; private set; }
		public ushort		WValue			{ get; private set; }
		
		public XlReader(byte[] byteArrayReceived) {
			this.byteArrayReceived = byteArrayReceived;
			this.ms = new MemoryStream(byteArrayReceived);
			this.br = new BinaryReader(this.ms, Encoding.ASCII);
			if (this.byteArrayReceived.Length < wsize * 4) this.SetBadDataStatus();

			XlBlockType messageType = (XlBlockType) br.ReadUInt16();
			if (messageType != XlBlockType.Table) this.SetBadDataStatus();
			long positionAfterMessageType = this.ms.Position;	// this.ms.Position=2 now

			//this.ms.Seek(wsize, SeekOrigin.Current);			// add two dummy bytes?...
			UInt16 twoDummyBytes = (UInt16) br.ReadUInt16();
			long positionMagicWsize = this.ms.Position;			// this.ms.Position=4 now
			
			this.RowsCount		= br.ReadUInt16();
			if (this.RowsCount == 0) {
				string msg = "CLIENT_SENT_ZERO_ROWS this.ms.Position[" + this.ms.Position + "]";
				string breakpoint = "here";
			}
			this.ColumnsCount	= br.ReadUInt16();
			this.ValueType		= XlBlockType.Unknown;
		}
		void SetBadDataStatus() {
			this.ValueType = XlBlockType.Bad;
			this.blocksize = 1;
		}
		public void ReadNext() {
			this.FloatValue		= float.NaN;
			this.StringValue	= null;
			this.WValue			= ushort.MinValue;
			if (this.ms.Position == 8) {
				int a = 1;
			}
			if (this.ValueType == XlBlockType.Unknown) {
				if (this.ms.Position + hsize > this.byteArrayReceived.Length) {
					this.SetBadDataStatus();
					throw new EndOfStreamException();
				}  else {
					this.ValueType = (XlBlockType)this.br.ReadUInt16();	//Position=8
					this.blocksize = this.br.ReadUInt16();				//Position=10

					if (this.ms.Position + this.blocksize > this.byteArrayReceived.Length) {
						this.SetBadDataStatus();		//Position=193 while reading: magically beoynd buffer size
						throw new EndOfStreamException();
					}
				}
			}
			if (this.blocksize <= 0) {
				this.ValueType = XlBlockType.Unknown;
				this.ReadNext();
				return;
			}
			switch (this.ValueType) {
				case XlBlockType.Float:
					this.blocksize -= fsize;
					if (this.blocksize >= 0) {
						this.FloatValue = br.ReadDouble();		// br.Read() cant read (float)s
					} else {
						this.SetBadDataStatus();
					}
					break;
					
				case XlBlockType.String:
					int strlen = ms.ReadByte();
					this.blocksize -= strlen + 1;
					if (this.blocksize >= 0) {
						this.StringValue = Encoding.GetEncoding(codepage).GetString(this.byteArrayReceived, (int)this.ms.Position, strlen);
						br.BaseStream.Seek(strlen, SeekOrigin.Current);
					} else {
						this.SetBadDataStatus();
					}
					break;
					
				case XlBlockType.Bool:
				case XlBlockType.Int:
					this.blocksize -= wsize;
					if (this.blocksize >= 0) {
						this.WValue = br.ReadUInt16();
					} else {
						this.SetBadDataStatus();
					}
					break;

				case XlBlockType.Error:
				case XlBlockType.Blank:
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
