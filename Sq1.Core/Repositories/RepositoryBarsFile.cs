using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using Sq1.Core.DataTypes;

namespace Sq1.Core.Repositories {
	public class RepositoryBarsFile {
		public RepositoryBarsSameScaleInterval BarsRepository { get; protected set; }
		public string Symbol { get; protected set; }
		public string Abspath { get; protected set; }
		public string Relpath { get { return RepositoryBarsFile.GetRelpathFromEnd(this.Abspath, 5); } }
		double barFileCurrentVersion = 3;	// yeps double :) 8 bytes!
		int symbolMaxLength = 64;			// 32 UTF8 characters
		int symbolHRMaxLength = 128;		// 64 UTF8 characters
		long headerSize;
		long oneBarSize;
		object sequentialLock;

		Dictionary<double, int> headerSizesByVersion = new Dictionary<double, int>() { { 3, 212 } };	// got 212 in Debugger from this.headerSize while reading saved v3 file
		Dictionary<double, int> barSizesByVersion = new Dictionary<double, int>() { { 3, 48 } };	// got 212 in Debugger from this.oneBarSize while reading saved v3 file
		
		public RepositoryBarsFile(RepositoryBarsSameScaleInterval barsFolder, string symbol, bool throwIfDoesntExist = true, bool createIfDoesntExist = false) {
			sequentialLock = new object();
			this.BarsRepository = barsFolder;
			this.Symbol = symbol;
			this.Abspath = this.BarsRepository.AbspathForSymbol(this.Symbol, throwIfDoesntExist, createIfDoesntExist);
		}

		public Bars BarsLoadAllThreadSafe() {
			Bars bars = null;
			lock(this.sequentialLock) {
				if (File.Exists(this.Abspath) == false) {
					string msg = "LoadBarsThreadSafe(): File doesn't exist [" + this.Abspath + "]";
					//Assembler.PopupException(msg);
					//throw new Exception(msg);
					return null;
//					return bars;
				}
				bars = this.BarsLoadAll();
			}
			return bars;
		}
		public Bars BarsLoadThreadSafe(DateTime startDate, DateTime endDate, int maxBars) {
			Bars barsAll = this.BarsLoadAllThreadSafe();
			//Assembler.PopupException("Loaded [ " + bars.Count + "] bars; symbol[" + this.Symbol + "] scaleInterval[" + this.BarsFolder.ScaleInterval + "]");
			if (startDate == DateTime.MinValue && endDate == DateTime.MaxValue && maxBars == 0) return barsAll;
							  
			string start = (startDate == DateTime.MinValue) ? "MIN" : startDate.ToString("dd-MMM-yyyy");
			string end = (endDate == DateTime.MaxValue) ? "MAX" : endDate.ToString("dd-MMM-yyyy");
			Bars bars = new Bars(barsAll.Symbol, barsAll.ScaleInterval, barsAll.ReasonToExist + " [" + start + "..." + end + "]max[" + maxBars + "]");
			for (int i=0; i<barsAll.Count; i++) {
				if (maxBars > 0 && i >= maxBars) break; 
				Bar barAdding = barsAll[i];
				bool skipThisBar = false;
				if (startDate > DateTime.MinValue && barAdding.DateTimeOpen < startDate) skipThisBar = true; 
				if (endDate < DateTime.MaxValue && barAdding.DateTimeOpen > endDate) skipThisBar = true;
				if (skipThisBar) continue;
				bars.BarAppendBindStatic(barAdding.CloneDetached());
			}
			return bars;
		}
		public Bars BarsLoadAll() {
			string msig = " BarsLoadAll(this.Abspath=[" + this.Abspath + "]): ";
			Bars bars = null;
			DateTime dateTime = DateTime.Now;
			FileStream fileStream = null;
			try {
				fileStream = File.Open(this.Abspath, FileMode.Open, FileAccess.Read, FileShare.Read);
				BinaryReader binaryReader = new BinaryReader(fileStream);

				string symbol_IGNOREDv3 = "NOT_READ_FROM_FILE";
				string symbolHumanReadable_IGNOREDv3;
				
				double version = binaryReader.ReadDouble();
				#if DEBUG
				//Debugger.Break();
				#endif
				
				if (version == 1) {
					//Assembler.PopupException("LoadBars[" + this.Relpath + "]: version[" + version + "]");
					symbol_IGNOREDv3 = binaryReader.ReadString();
					symbolHumanReadable_IGNOREDv3 = binaryReader.ReadString();
				} else if (version <= 2) {
					byte[] bytesSymbol = new byte[this.symbolMaxLength];
					binaryReader.Read(bytesSymbol, 0, this.symbolMaxLength);
					symbol_IGNOREDv3 = this.byteArrayToString(bytesSymbol);

					byte[] bytesSymbolHR = new byte[this.symbolHRMaxLength];
					binaryReader.Read(bytesSymbolHR, 0, this.symbolHRMaxLength);
					symbolHumanReadable_IGNOREDv3 = this.byteArrayToString(bytesSymbolHR);
				} else if (version <= 3) {
					// NO_SYMBOL_AND_HR_IS_PRESENT_IN_FILE
					int a = 1;
				}
				BarScale barScale = (BarScale)binaryReader.ReadInt32();
				int barInterval = binaryReader.ReadInt32();
				int barsStored = binaryReader.ReadInt32();
				#if DEBUG
				this.headerSize = binaryReader.BaseStream.Position;
				#endif

				BarScaleInterval scaleInterval = new BarScaleInterval(barScale, barInterval);
				//string shortFnameIneedMorePathParts = Path.GetFileName(this.Abspath);
				//string shortFname = this.Abspath.Substring(this.Abspath.IndexOf("" + Path.DirectorySeparatorChar + "Data" + Path.DirectorySeparatorChar + "") + 6);
				string shortFname = this.Relpath;
				//v1,2 AFTER_IMPLEMENTING_FIXED_SYMBOL_WIDTH_IGNORING_WHAT_I_READ_FROM_FILE  bars = new Bars(symbol, scaleInterval, shortFname);
				string v3ignoresSymbolFromFile = (this.barFileCurrentVersion <=2) ? symbol_IGNOREDv3 : this.Symbol;
				bars = new Bars(v3ignoresSymbolFromFile, scaleInterval, shortFname);
				//for (int barsRead = 0; barsRead<barsStored; barsRead++) {
				while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length) {
					DateTime dateTimeOpen = new DateTime(binaryReader.ReadInt64());
					double open = binaryReader.ReadDouble();
					double high = binaryReader.ReadDouble();
					double low = binaryReader.ReadDouble();
					double close = binaryReader.ReadDouble();
					double volume = binaryReader.ReadDouble();
					#if DEBUG
					if (this.oneBarSize == 0) {
						this.oneBarSize = binaryReader.BaseStream.Position - this.headerSize;
					}
					#endif
					Bar barAdded = bars.BarCreateAppendBindStatic(dateTimeOpen, open, high, low, close, volume);
				}
				//Debugger.Break();
			} catch (EndOfStreamException ex) {
				Assembler.PopupException(ex.Message + msig, ex);
			} finally {
				if (fileStream != null) fileStream.Close();
			}
			return bars;
		}
		public int BarsSaveThreadSafe(Bars bars) {
			//BARS_INITIALIZED_EMPTY if (bars.Count == 0) return 0;
			int barsSaved = -1;
			lock (this.sequentialLock) {
				barsSaved = this.BarsSave(bars);
				//Assembler.PopupException("Saved [ " + bars.Count + "] bars; symbol[" + bars.Symbol + "] scaleInterval[" + bars.ScaleInterval + "]");
			}
			return barsSaved;
		}
		int BarsSave(Bars bars) {
			int barsSaved = 0;
			FileStream fileStream = null;
			try {
				fileStream = File.Create(this.Abspath);
				// TODO create header structure and have its length the same both for Read & Write
				// HEADER BEGIN
				BinaryWriter binaryWriter = new BinaryWriter(fileStream);
				binaryWriter.Write((double)this.barFileCurrentVersion); // yes it was double :)
				if (this.barFileCurrentVersion == 1) {
					binaryWriter.Write(bars.Symbol);
					binaryWriter.Write(bars.SymbolHumanReadable);
				} else if (this.barFileCurrentVersion <= 2) {
					byte[] byteBufferSymbol = this.stringToByteArray(bars.Symbol, this.symbolMaxLength);
					#if DEBUG
					//TESTED Debugger.Break();
					#endif
					binaryWriter.Write(byteBufferSymbol);
					byte[] byteBufferSymbolHR = this.stringToByteArray(bars.SymbolHumanReadable, this.symbolHRMaxLength);
					binaryWriter.Write(byteBufferSymbolHR);
				} else if (this.barFileCurrentVersion <= 3) {
					// NO_SYMBOL_AND_HR_IS_PRESENT_IN_FILE
				}
				binaryWriter.Write((int)bars.ScaleInterval.Scale);
				binaryWriter.Write(bars.ScaleInterval.Interval);
				binaryWriter.Write(bars.Count);
				// HEADER END
				for (int i = 0; i < bars.Count; i++) {
					Bar bar = bars[i];
					binaryWriter.Write(bar.DateTimeOpen.Ticks);
					binaryWriter.Write(bar.Open);
					binaryWriter.Write(bar.High);
					binaryWriter.Write(bar.Low);
					binaryWriter.Write(bar.Close);
					binaryWriter.Write(bar.Volume);
					barsSaved++;
				}
			} catch (Exception ex) {
				string msg = "Error while Saving bars[" + this + "] into [" + this.Abspath + "]";
				Assembler.PopupException(msg, ex);
			} finally {
				if (fileStream != null) fileStream.Close();
			}
			return barsSaved;
		}
		#region v2-related fixed-width routines for Symbol and SymbolHR (useless for v3 but still invoked)
		// http://stackoverflow.com/questions/472906/converting-a-string-to-byte-array
		byte[] stringToByteArray(string symbol, int bufferLength) {
			byte[] ret = new byte[bufferLength];
			//v1
			//string symbolTruncated = symbol;
			//if (symbolTruncated.Length > byteBuffer.Length) {
			//	symbolTruncated = symbolTruncated.Substring(0, byteBuffer.Length);
			//	string msg = "";
			//	Assembler.PopupException("TRUNCATED_SYMBOL_TO_FIT_BARFILE_HEADER v[" + this.barFileCurrentVersion + "](" + bufferLength+ ")bytes [" + symbol + "]=>[" + symbolTruncated + "]");
			//}
			//System.Buffer.BlockCopy(symbolTruncated.ToCharArray(), 0, byteBuffer, 0, symbolTruncated.Length);
			//v2
			byte[] utf8 = System.Text.Encoding.UTF8.GetBytes(symbol);
			int min = Math.Min(utf8.Length, ret.Length);
			System.Buffer.BlockCopy(utf8, 0, ret, 0, min);
			string reconstructed = this.reconstructFromByteArray(ret);
			if (reconstructed.Length != symbol.Length) {
				string msg = "TRUNCATED_SYMBOL_TO_FIT_BARFILE_HEADER v[" + this.barFileCurrentVersion + "](" + bufferLength + ")bytes [" + symbol + "]=>[" + reconstructed + "]";
				Assembler.PopupException(msg);
			}
			return ret;
		}
		string byteArrayToString(byte[] bytes) {
			char[] chars = new char[bytes.Length / sizeof(char)];
			//v1 System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
			//v2
			//for (int i = 0; i < chars.Length; i++) {
			//	if (chars[i] == 0) break;	// want to avoid "RIM3\0\0\0\0\0\0"
			//	System.Buffer.BlockCopy(bytes, i, chars, i, 1);
			//}
			//string ret = new string(chars);

			string ret = this.reconstructFromByteArray(bytes);
			return ret;
		}
		string reconstructFromByteArray(byte[] bytes) {
			string reconstructed = System.Text.Encoding.UTF8.GetString(bytes);
			char[] filtered = new char[bytes.Length];
			int validDestIndex = 0;
			foreach (char c in reconstructed.ToCharArray()) {
				if (c == 0) continue;	// avoiding "RIM3\0\0\0\0\0\0" and  "R\0I\0M\03\0\0\0\0\0\0"
				filtered[validDestIndex++] = c;
			}
			char[] final = new char[validDestIndex];
			//System.Buffer.BlockCopy(filtered, 0, final, 0, final.Length);
			for (int i = 0; i < final.Length; i++) {
				final[i] = filtered[i];
			}
			string ret = new string(final);
			return ret;
		}
		#endregion

		public int BarsAppendThreadSafe(Bar barLastFormed) {
			//BARS_INITIALIZED_EMPTY if (bars.Count == 0) return 0;
			int barsAppended = -1;
			lock (this.sequentialLock) {
				barsAppended = this.BarAppend(barLastFormed);
				//Assembler.PopupException("Saved [ " + bars.Count + "] bars; symbol[" + bars.Symbol + "] scaleInterval[" + bars.ScaleInterval + "]");
			}
			return barsAppended;
		}
		protected int BarAppend(Bar barLastFormed) {
			//v1
			//Bars allBars = this.BarsLoadAllThreadSafe();
			//if (allBars == null) {
			//	allBars = new Bars(barLastFormed.Symbol, barLastFormed.ScaleInterval, "DUMMY: LoadBars()=null");
			//}
			////allBars.DumpPartialInitFromStreamingBar(bar);

			//// this happens on a very first quote - this.pushBarToConsumers(StreamingBarFactory.LastBarFormed.Clone());
			//if (allBars.BarStaticLastNullUnsafe.DateTimeOpen == barLastFormed.DateTimeOpen) return 0;

			//// not really needed to clone to save it in a file, but we became strict to eliminate other bugs
			//barLastFormed = barLastFormed.CloneDetached();

			//// SetParentForBackwardUpdateAutoindex used within Bar only()
			////barLastFormed.SetParentForBackwardUpdateAutoindex(allBars);
			//if (allBars.BarStaticLastNullUnsafe.DateTimeOpen == barLastFormed.DateTimeOpen) {
			//	return 0;
			//}

			//allBars.BarAppendBindStatic(barLastFormed);
			//int barsSaved = this.BarsSaveThreadSafe(allBars);

			//v2, starting from barFileCurrentVersion=3: seek to the end, read last Bar, overwrite if same date or append if greater; 0.1ms instead of reading all - appending - writing all
			#if DEBUG
			//Debugger.Break();
			#endif

			string msig = " BarAppend(" + barLastFormed + ")=>[" + this.Abspath + "]";
			FileStream fileStream = null;
			try {
				fileStream = File.Open(this.Abspath, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
			} catch (Exception ex) {
				string msg = "1/4_FILE_OPEN_THROWN";
				Assembler.PopupException(msg + msig, ex);
				return 0;
			}
			try {
				BinaryWriter binaryWriter = new BinaryWriter(fileStream);
				BinaryReader binaryReader = new BinaryReader(fileStream);
				int barSize = this.barSizesByVersion[this.barFileCurrentVersion];
				try {
					fileStream.Seek(barSize, SeekOrigin.End);
				} catch (Exception ex) {
					string msg = "2/4_FILESTREAM_SEEK_END_THROWN barSize[" + barSize + "]";
					Assembler.PopupException(msg + msig, ex);
					return 0;
				}
				//DateTime dateTimeOpen = new DateTime(binaryReader.ReadInt64());
				//if (dateTimeOpen >= barLastFormed.DateTimeOpen) {
				//    try {
				//        fileStream.Seek(barSize, SeekOrigin.End);	// overwrite the last bar, apparently streaming has been solidified
				//    } catch (Exception ex) {
				//        string msg = "3/4_FILESTREAM_SEEK_END_THROWN barSize[" + barSize + "]";
				//        Assembler.PopupException(msg + msig, ex);
				//        return 0;
				//    }
				//} else {
				//    try {
				//        fileStream.Seek(0, SeekOrigin.End);			// append barLastFormed to file since it's newer than last saved/read
				//    } catch (Exception ex) {
				//        string msg = "3/4_FILESTREAM_SEEK_0_THROWN";
				//        Assembler.PopupException(msg + msig, ex);
				//        return 0;
				//    }
				//}
				try {
					binaryWriter.Write(barLastFormed.DateTimeOpen.Ticks);
					binaryWriter.Write(barLastFormed.Open);
					binaryWriter.Write(barLastFormed.High);
					binaryWriter.Write(barLastFormed.Low);
					binaryWriter.Write(barLastFormed.Close);
					binaryWriter.Write(barLastFormed.Volume);
				} catch (Exception ex) {
					string msg = "4/4_BINARYWRITER_WRITER_THROWN";
					Assembler.PopupException(msg + msig, ex);
					return 0;
				}
			} finally {
				if (fileStream != null) fileStream.Close();
			}
			return 1;
		}
		
		public override string ToString() {
			return this.Relpath;
		}
		public static string GetRelpathFromEnd(string abspath, int partsFromEnd = 3) {
			string ret = "";
			string[] splitted = abspath.Split(new char[] {Path.DirectorySeparatorChar});
			for (int i=1; i<=partsFromEnd; i++) {
				int partToTake = splitted.Length - i;
				if (partToTake < 0) break;
				string thisPart = splitted[partToTake];
				if (ret != "") ret = Path.DirectorySeparatorChar + ret;
				ret = thisPart + ret;
			}
			return ret;
		}
	}
}