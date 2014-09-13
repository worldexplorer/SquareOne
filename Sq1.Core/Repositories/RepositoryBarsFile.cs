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
		double barFileCurrentVersion = 2;	// yeps double :) 8 bytes!
		int symbolMaxLength = 64;
		int symbolHRMaxLength = 128;
		
		public RepositoryBarsFile(RepositoryBarsSameScaleInterval barsFolder, string symbol, bool throwIfDoesntExist = true, bool createIfDoesntExist = false) {
			this.BarsRepository = barsFolder;
			this.Symbol = symbol;
			this.Abspath = this.BarsRepository.AbspathForSymbol(this.Symbol, throwIfDoesntExist, createIfDoesntExist);
		}

		public Bars BarsLoadAllThreadSafe() {
			Bars bars = null;
			lock(this) {
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

				string symbol = "NOT_READ_FROM_FILE";
				string symbolHumanReadable;
				
				double version = binaryReader.ReadDouble();
				#if DEBUG
				//Debugger.Break();
				#endif
				
				if (version == 1) {
					//Assembler.PopupException("LoadBars[" + this.Relpath + "]: version[" + version + "]");
					symbol = binaryReader.ReadString();
					symbolHumanReadable = binaryReader.ReadString();
				} else if (version <= 2) {
					byte[] bytesSymbol = new byte[this.symbolMaxLength];
					binaryReader.Read(bytesSymbol, 0, this.symbolMaxLength);
					symbol = this.byteArrayToString(bytesSymbol);

					byte[] bytesSymbolHR = new byte[this.symbolHRMaxLength];
					binaryReader.Read(bytesSymbolHR, 0, this.symbolHRMaxLength);
					symbolHumanReadable = this.byteArrayToString(bytesSymbolHR);
				}
				BarScale barScale = (BarScale)binaryReader.ReadInt32();
				int barInterval = binaryReader.ReadInt32();
				BarScaleInterval scaleInterval = new BarScaleInterval(barScale, barInterval);
				//string shortFnameIneedMorePathParts = Path.GetFileName(this.Abspath);
				//string shortFname = this.Abspath.Substring(this.Abspath.IndexOf("" + Path.DirectorySeparatorChar + "Data" + Path.DirectorySeparatorChar + "") + 6);
				string shortFname = this.Relpath;
				bars = new Bars(symbol, scaleInterval, shortFname);
				int barsStored = binaryReader.ReadInt32();
				//int securityType = binaryReader.ReadInt32();
				//bars.SymbolInfo.SecurityType = (SecurityType)securityType;
				for (int barsRead = 0; barsRead<barsStored; barsRead++) {
					DateTime dateTimeOpen = new DateTime(binaryReader.ReadInt64());
					double open = binaryReader.ReadDouble();
					double high = binaryReader.ReadDouble();
					double low = binaryReader.ReadDouble();
					double close = binaryReader.ReadDouble();
					double volume = binaryReader.ReadDouble();
					Bar barAdded = bars.BarCreateAppendBindStatic(dateTimeOpen, open, high, low, close, volume);
				}
			} catch (EndOfStreamException ex) {
				Assembler.PopupException(ex.Message + msig, ex);
			} finally {
				if (fileStream != null) fileStream.Close();
			}
			return bars;
		}
		public int BarsSaveThreadSafe(Bars bars) {
			if (bars.Count == 0) return 0;
			int barsSaved = -1;
			lock (this) {
				barsSaved = BarsSave(bars);
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
				if (this.barFileCurrentVersion == 1) {
					binaryWriter.Write((double)this.barFileCurrentVersion); // yes it was double :)
					binaryWriter.Write(bars.Symbol);
					binaryWriter.Write(bars.SymbolHumanReadable);
				} else if (this.barFileCurrentVersion <= 2) {
					binaryWriter.Write(this.barFileCurrentVersion);
					byte[] byteBufferSymbol = this.stringToByteArray(bars.Symbol, this.symbolMaxLength);
					#if DEBUG
					//TESTED Debugger.Break();
					#endif
					binaryWriter.Write(byteBufferSymbol);
					byte[] byteBufferSymbolHR = this.stringToByteArray(bars.SymbolHumanReadable, this.symbolHRMaxLength);
					binaryWriter.Write(byteBufferSymbolHR);
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
		
		// http://stackoverflow.com/questions/472906/converting-a-string-to-byte-array
		byte[] stringToByteArray(string symbol, int bufferLength) {
			byte[] ret = new byte[bufferLength];
			//v1
			//string symbolTruncated = symbol;
			//if (symbolTruncated.Length > byteBuffer.Length) {
			//    symbolTruncated = symbolTruncated.Substring(0, byteBuffer.Length);
			//    string msg = "";
			//    Assembler.PopupException("TRUNCATED_SYMBOL_TO_FIT_BARFILE_HEADER v[" + this.barFileCurrentVersion + "](" + bufferLength+ ")bytes [" + symbol + "]=>[" + symbolTruncated + "]");
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
			//    if (chars[i] == 0) break;	// want to avoid "RIM3\0\0\0\0\0\0"
			//    System.Buffer.BlockCopy(bytes, i, chars, i, 1);
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

			//string ret = filtered.ToString();
			string ret = new string(final);

			return ret;
		}
		
		//TODO : seek to the end, read last Bar, overwrite if same date or append if greater; 0.1ms instead of reading all - appending - writing all
		public int BarAppend(Bar barLastFormed) {
			Bars allBars = this.BarsLoadAllThreadSafe();
			if (allBars == null) {
				allBars = new Bars(barLastFormed.Symbol, barLastFormed.ScaleInterval, "DUMMY: LoadBars()=null");
			}
			//allBars.DumpPartialInitFromStreamingBar(bar);

			// this happens on a very first quote - this.pushBarToConsumers(StreamingBarFactory.LastBarFormed.Clone());
			if (allBars.BarStaticLastNullUnsafe.DateTimeOpen == barLastFormed.DateTimeOpen) return 0;

			// not really needed to clone to save it in a file, but we became strict to eliminate other bugs
			barLastFormed = barLastFormed.CloneDetached();

			// SetParentForBackwardUpdateAutoindex used within Bar only()
			//barLastFormed.SetParentForBackwardUpdateAutoindex(allBars);
			if (allBars.BarStaticLastNullUnsafe.DateTimeOpen == barLastFormed.DateTimeOpen) {
				return 0;
			}

			allBars.BarAppendBindStatic(barLastFormed);
			int barsSaved = this.BarsSaveThreadSafe(allBars);
			return barsSaved;
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