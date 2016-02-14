using System;
using System.Collections.Generic;
using System.IO;

using Sq1.Core.DataTypes;

namespace Sq1.Core.Repositories {
	public partial class RepositoryBarsFile {
		RepositoryBarsSameScaleInterval barsRepository; // WAS_PUBLIC_EARLIER{ get; protected set; }
		public string Symbol { get; protected set; }
		public string Abspath { get; protected set; }
		public string Relpath { get { return RepositoryBarsFile.GetRelpathFromEnd(this.Abspath, 5); } }
		
		double barFileCurrentVersion = 3;	// yeps double :) 8 bytes!
		int symbolMaxLength = 64;			// IRRELEVANT_FOR_barFileCurrentVersion=3 32 UTF8 characters
		int symbolHRMaxLength = 128;		// IRRELEVANT_FOR_barFileCurrentVersion=3 64 UTF8 characters
		long headerSize;			// BARS_LOAD_TELEMETRY
		long oneBarSize;			// BARS_LOAD_TELEMETRY
		
		// 1)	in RepositoryBarsFile, strictly File.Open(FileAccess.(Read)Write, FileShare.None) OR File.Open(FileAccess.Read, FileShare.Read) are used;
		// 		I wish there was a way for all binaryWriter.Write()s to block all concurrent non-conflicting File.Open(FileAccess.Read)s until previous File.Open(FileAccess.(Read)Write, FileShare.None).Close()s;
		// 		it would be nicer if File.Open(FileAccess.Read, FileShare.Read) wouldn't throw but would wait for (FileAccess.(Read)Write+FileShare.None) to complete;
		// 2)	fileReadWriteSequentialLock is used since there is no way for File.Open to wait current writes to Close(), it will always throw after File.Open(FileAccess.(Read)Write, FileShare.None): http://msdn.microsoft.com/en-us/library/system.io.fileshare%28v=vs.110%29.aspx
		//		so I use fileReadWriteSequentialLock at all times including multiple non-conflicting File.Open(FileAccess.Read)s;
		// 3)	if your BAR file is 400% fragmented and you can't wait for Seek(-barSize, SeekOrigin.End), please implement ASYNC_APPEND_TBI and you'll need
		//		another lock or ManualResetEvent to notify readers that they can open a file closed after writer is complete;
		// 4)	ASYNC_APPEND_TBI-related Task<LastBarAppender> could use this one: http://stackoverflow.com/questions/50744/wait-until-file-is-unlocked-in-net
		object fileReadWriteSequentialLock;

		Dictionary<double, long> headerSizesByVersion	= new Dictionary<double, long>() {
			{ 1, 42 },	// got 20 in below, but length depends on Symbol/ClassName
			{ 2, 20 },	// got 20 in Debugger from this.headerSize while reading saved v3 file
			{ 3, 20 },	// got 20 in Debugger from this.headerSize while reading saved v3 file
		};
		Dictionary<double, long> barSizesByVersion		= new Dictionary<double, long>() {
			{ 1, 48 },	// got 48 in Debugger from this.oneBarSize while reading saved v3 file
			{ 2, 48 },	// got 48 in Debugger from this.oneBarSize while reading saved v3 file
			{ 3, 48 }	// got 48 in Debugger from this.oneBarSize while reading saved v3 file
		};
		
		public RepositoryBarsFile(RepositoryBarsSameScaleInterval barsFolder, string symbol, bool throwIfDoesntExist = true, bool createIfDoesntExist = false) {
			fileReadWriteSequentialLock = new object();
			this.barsRepository = barsFolder;
			this.Symbol = symbol;
			this.Abspath = this.barsRepository.AbspathForSymbol(this.Symbol, throwIfDoesntExist, createIfDoesntExist);
		}

		public Bars BarsLoad_nullUnsafeThreadSafe_NOT_USED(DateTime dateFrom, DateTime dateTill, int maxBars) {
			Bars barsAll = this.BarsLoadAll_nullUnsafeThreadSafe();
			if (barsAll == null) return barsAll;
			
			//Assembler.PopupException("Loaded [ " + bars.Count + "] bars; symbol[" + this.Symbol + "] scaleInterval[" + this.BarsFolder.ScaleInterval + "]");
			if (dateFrom == DateTime.MinValue && dateTill == DateTime.MaxValue && maxBars == 0) return barsAll;

			string start = (dateFrom == DateTime.MinValue) ? "MIN" : dateFrom.ToString("dd-MMM-yyyy");
			string end = (dateTill == DateTime.MaxValue) ? "MAX" : dateTill.ToString("dd-MMM-yyyy");
			Bars bars = new Bars(barsAll.Symbol, barsAll.ScaleInterval, barsAll.ReasonToExist + " [" + start + "..." + end + "]max[" + maxBars + "]");
			for (int i = 0; i < barsAll.Count; i++) {
				if (maxBars > 0 && i >= maxBars) break;
				Bar barAdding = barsAll[i];
				bool skipThisBar = false;
				if (dateFrom > DateTime.MinValue && barAdding.DateTimeOpen < dateFrom) skipThisBar = true;
				if (dateTill < DateTime.MaxValue && barAdding.DateTimeOpen > dateTill) skipThisBar = true;
				if (skipThisBar) continue;
				bars.BarAppendBindStatic(barAdding.CloneDetached());
			}
			return bars;
		}
		public Bars BarsLoadAll_nullUnsafeThreadSafe(bool saveBarsIfThereWasFailedCheckOHLCV = true) {
			Bars bars = null;
			lock(this.fileReadWriteSequentialLock) {
				if (File.Exists(this.Abspath) == false) {
					string msg = "LoadBarsThreadSafe(): File doesn't exist [" + this.Abspath + "]";
					//Assembler.PopupException(msg);
					//throw new Exception(msg);
					return null;
//					return bars;
				}
				bars = this.barsLoadAll_nullUnsafe(saveBarsIfThereWasFailedCheckOHLCV);
			}
			return bars;
		}
		Bars barsLoadAll_nullUnsafe(bool resaveBarsIfThereWasFailedCheckOHLCV = true) {
			string msig = " BarsLoadAll(this.Abspath=[" + this.Abspath + "]): ";
			int barsReadTotal = 0;
			int barsFailedCheckOHLCV = 0;
			bool resaveRequiredByVersionMismatch = false;
			
			Bars bars = null;
			DateTime dateTime = DateTime.Now;
			FileStream fileStream = null;
			BinaryReader binaryReader = null;
			try {
				fileStream = File.Open(this.Abspath, FileMode.Open, FileAccess.Read, FileShare.Read);
				binaryReader = new BinaryReader(fileStream);

				string symbol_IGNOREDv3 = "NOT_READ_FROM_FILE";
				string symbolHumanReadable_IGNOREDv3;
				
				double version = binaryReader.ReadDouble();
				if (version != this.barFileCurrentVersion) {
					resaveRequiredByVersionMismatch = true;
					string msg = "WILL_RESAVE_IN_CURRENT_BAR_BINARY_FORMAT"
						+ " version[" + version + "] => this.barFileCurrentVersion[" + this.barFileCurrentVersion + "]"
						+ " resaveRequiredByVersionMismatch[" + resaveRequiredByVersionMismatch + "]";
					Assembler.PopupException(msg + msig, null, false);
				}
				
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
				this.headerSize = binaryReader.BaseStream.Position;		// BARS_LOAD_TELEMETRY

				BarScaleInterval scaleInterval = new BarScaleInterval(barScale, barInterval);
				//string shortFnameIneedMorePathParts = Path.GetFileName(this.Abspath);
				//string shortFname = this.Abspath.Substring(this.Abspath.IndexOf("" + Path.DirectorySeparatorChar + "Data" + Path.DirectorySeparatorChar + "") + 6);
				string shortFname = this.Relpath;
				//v1,2 AFTER_IMPLEMENTING_FIXED_SYMBOL_WIDTH_IGNORING_WHAT_I_READ_FROM_FILE  bars = new Bars(symbol, scaleInterval, shortFname);
				string v3ignoresSymbolFromFile = (this.barFileCurrentVersion <=2) ? symbol_IGNOREDv3 : this.Symbol;
				bars = new Bars(v3ignoresSymbolFromFile, scaleInterval, shortFname);
				
				//http://stackoverflow.com/questions/58380/avoiding-first-chance-exception-messages-when-the-exception-is-safely-handled
				//for (int barsRead = 0; barsRead<barsStored; barsRead++) {
				while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length) {
					long ticks = binaryReader.ReadInt64();
					DateTime dateTimeOpen;
					try {
						dateTimeOpen = new DateTime(ticks);
					} catch (Exception ex) {
						string msg = "FAILED_TO_CONVERT_TO_DATE_TICKS=[" + ticks + "]";
						long leftBoundaryOffendedBy = DateTime.MinValue.Ticks - ticks;
						if (leftBoundaryOffendedBy > 0) {
							msg += " leftBoundaryOffendedBy[" + leftBoundaryOffendedBy + "]";
						}
						long rightBoundaryOffendedBy = ticks - DateTime.MaxValue.Ticks;
						if (rightBoundaryOffendedBy > 0) {
							msg += " rightBoundaryOffendedBy[" + rightBoundaryOffendedBy + "]";
						}

						string illiteracyDive = ""
							+ " DateTime.MinValue.Ticks[" + DateTime.MinValue.Ticks+ "] ..."
							+ " DateTime.MaxValue.Ticks[" + DateTime.MaxValue.Ticks + "]";
						Assembler.PopupException(msg + illiteracyDive, ex, false);
						break;
					}
					double open		= binaryReader.ReadDouble();
					double high		= binaryReader.ReadDouble();
					double low		= binaryReader.ReadDouble();
					double close	= binaryReader.ReadDouble();
					double volume	= binaryReader.ReadDouble();
					barsReadTotal++;
					if (this.oneBarSize == 0) {
						// I want to print out the size of header and bar, but I don't want to extract save-able members from Bars and Bar to use Marshal.SizeOf(<T>)
						this.oneBarSize = binaryReader.BaseStream.Position - this.headerSize;	// BARS_LOAD_TELEMETRY
					}
					try {
						Bar barAdded = bars.BarCreateAppendBindStatic(dateTimeOpen, open, high, low, close, volume, true);
					} catch (Exception ex) {
						barsFailedCheckOHLCV++;
						// already reported exception in CheckOHLCVthrow
						string msg2 = " barsFailedCheckOHLCV[" + barsFailedCheckOHLCV + "]  barsReadTotal[" + barsReadTotal + "] bars.Count[" + bars.Count + "]"
							+ " binaryReader.BaseStream.Position[" + binaryReader.BaseStream.Position + "]/[" + binaryReader.BaseStream.Length + "]";
						Assembler.PopupException(msg2 + msig, ex, false);
						continue;	//just in case if you add code below :)
					}
				}
				
				string msg3 = "BARS_LOAD_ALL_TELEMETRY SIZEOF(header)[" + this.headerSize + "] SIZEOF(Bar)[" + this.oneBarSize + "]"
					+ " version[" + version + "] bars[" + bars + "] Relpath[" + this.Relpath + "]";
				//Assembler.PopupException(msg3 + msig, null, false);
				try {
					long barSize = this.barSizesByVersion[version];
					if (barSize != this.oneBarSize) {
						this.barSizesByVersion[version] = this.oneBarSize;
					}
				} catch (Exception ex) {
					string msg2 = "FAILED_TO_SYNC this.barSizesByVersion[" + version + "]";
					Assembler.PopupException(msg2 + msig, ex);
				}
	
				try {
					long headerSize = this.headerSizesByVersion[version];
					if (headerSize != this.headerSize) {
						#if DEBUG
						this.headerSizesByVersion[version] = this.headerSize;
						Assembler.PopupException("WHILE_LOADING_BARS_FILE_OVERWRITTEN_WITH_HEADER_FROM_NEWER_VERSION NEW_HEADER_SIZE[" + this.headerSize + "] " + msg3);
						#endif
					}
				} catch (Exception ex) {
					string msg2 = "FAILED_TO_SYNC this.headerSizesByVersion[" + version + "]";
					Assembler.PopupException(msg2 + msig, ex);
				}
			} catch (Exception ex) {
				string msg = "BARS_LOAD_ALL_FAILED[" + this.Abspath + "]";
				Assembler.PopupException(msg + msig, ex);
			} finally {
				if (binaryReader != null) {
					binaryReader.Close();
					binaryReader.Dispose();
				}
				if (fileStream != null) {
					fileStream.Close();
					fileStream.Dispose();
				}
			}

			
			bool resaveRequired = resaveRequiredByVersionMismatch;
			if (barsFailedCheckOHLCV > 0) {
				string msg = "SOME_BARS_SKIPPED_WHILE_SAVING barsFailedCheckOHLCV[" + barsFailedCheckOHLCV + "] barsReadTotal[" + barsReadTotal + "] bars.Count[" + bars.Count + "]";
				Assembler.PopupException(msg, null, false);
				if (resaveBarsIfThereWasFailedCheckOHLCV) {
					resaveRequired = true;
				}
			}
			if (resaveRequired) {
				int reSaved = this.BarsSaveThreadSafe(bars);
				string msg2 = "RE-SAVED_TO_REMOVE_BARS_ALL_ZEROES reSaved[" + reSaved + "]";
				Assembler.PopupException(msg2, null, false);
			}

			return bars;
		}
		public int BarsSaveThreadSafe(Bars bars) {
			//BARS_INITIALIZED_EMPTY if (bars.Count == 0) return 0;
			int barsSaved = -1;
			lock (this.fileReadWriteSequentialLock) {
				barsSaved = this.barsSave(bars);
				string msg = "barsSaved[" + barsSaved + "] " + this.Symbol + ":" + this.barsRepository.ScaleInterval;
				Assembler.PopupException(msg, null, false);
			}
			return barsSaved;
		}
		int barsSave(Bars bars) {
			string msig = " barsSave(" + bars + ")=>[" + this.Abspath + "]";
			int barsSaved = 0;
			int barsFailedCheckOHLCV = 0;

			FileStream fileStream = null;
			BinaryWriter binaryWriter = null;
			try {
				// WILL_binaryWriter.Close()_HELP?... ALL_THROW_IN_VS2010
				//fileStream = File.OpenWrite(this.Abspath);
				//fileStream = File.Create(this.Abspath);
				//fileStream = File.Create(this.Abspath, 1024*1024, FileOptions.SequentialScan);
				//fileStream = File.Open(this.Abspath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
				fileStream = File.Open(this.Abspath, FileMode.Truncate, FileAccess.Write, FileShare.None);
				// SAME_THING_WORKS_IN_SHARP_DEVELOP
				//fileStream = File.Open(this.Abspath, FileMode.Truncate, FileAccess.Write, FileShare.None);
			} catch (Exception ex) {
				string msg = "1/4_FILE_OPEN_THROWN";
				Assembler.PopupException(msg + msig, ex);
				return barsSaved;
			}
			try {
				// LAZY to extract save-able members from Bars and Bar to use Marshal.SizeOf(<T>) TODO create header structure and have its length the same both for Read & Write
				
				// HEADER BEGIN
				binaryWriter = new BinaryWriter(fileStream);
				binaryWriter.Write((double)this.barFileCurrentVersion); // yes it was double :)
				if (this.barFileCurrentVersion == 1) {
					binaryWriter.Write(bars.Symbol);
					binaryWriter.Write(bars.SymbolHumanReadable);
				} else if (this.barFileCurrentVersion <= 2) {
					byte[] byteBufferSymbol = this.stringToByteArray(bars.Symbol, this.symbolMaxLength);
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
					try {
						bar.CheckOHLCVthrow();	//	catching the exception will display stacktrace in ExceptionsForm
					} catch (Exception ex) {
						barsFailedCheckOHLCV++;
						string msg = "NOT_SAVING_TO_FILE_THIS_BAR__TOO_LATE_TO_FIND_WHO_GENERATED_IT barAllZeroes bar[" + bar + "]";
						Assembler.PopupException(msg, ex, false);
						continue;
					}
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
				if (binaryWriter != null) {
					binaryWriter.Close();
					binaryWriter.Dispose();
				}
				if (fileStream != null) {
					fileStream.Close();
					fileStream.Dispose();
				}
			}
			if (barsFailedCheckOHLCV > 0) {
				string msg = "SOME_BARS_SKIPPED_WHILE_SAVING barsFailedCheckOHLCV[" + barsFailedCheckOHLCV + "] barsSaved[" + barsSaved + "] bars.Count[" + bars.Count + "]";
				Assembler.PopupException(msg, null, false);
			}
			return barsSaved;
		}

		public int BarAppendStaticOrReplaceStreamingThreadSafe(Bar barLastFormed) {
			int barsAppendedOrReplaced = -1;
			lock (this.fileReadWriteSequentialLock) {
				barsAppendedOrReplaced = this.barAppendStaticOrReplaceStreaming(barLastFormed);
				string msg = "barsAppendedOrReplaced[" + barsAppendedOrReplaced + "] " + this.Symbol + ":" + this.barsRepository.ScaleInterval;
				Assembler.PopupException(msg, null, false);
			}
			return barsAppendedOrReplaced;
		}

		// for a daily chart, you may want to sync the streaming bar every 10 seconds (ASYNC_APPEND_TBI) otherwize you loose whole past day 
		int barAppendStaticOrReplaceStreaming(Bar barLastFormedStaticOrCurrentStreaming) {
			//starting from barFileCurrentVersion=3: seek to the end, read last Bar, overwrite if same date or append if greater; 0.1ms instead of reading all - appending - writing all
			int saved = 0;
			string msig = " barAppendStaticOrReplaceStreaming(" + barLastFormedStaticOrCurrentStreaming + ")=>[" + this.Abspath + "]";

			try {
				barLastFormedStaticOrCurrentStreaming.CheckOHLCVthrow();	//	catching the exception will display stacktrace in ExceptionsForm
			} catch (Exception ex) {
				string msg = "NOT_APPENDING_TO_FILE_THIS_BAR__FIX_WHO_GENERATED_IT_UPSTACK barAllZeroes barLastFormed[" + barLastFormedStaticOrCurrentStreaming + "]";
				Assembler.PopupException(msg + msig, ex, false);
				return saved;
			}

			FileStream fileStream = null;
			BinaryWriter binaryWriter = null;
			BinaryReader binaryReader = null;
			try {
				fileStream = File.Open(this.Abspath, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
			} catch (Exception ex) {
				string msg = "1/4_FILE_OPEN_THROWN";
				Assembler.PopupException(msg + msig, ex);
				return saved;
			}
			try {
				binaryWriter = new BinaryWriter(fileStream);
				binaryReader = new BinaryReader(fileStream);
				long barSize = this.barSizesByVersion[this.barFileCurrentVersion];
				try {
					// THIS_WAS_GENERATING_ZERO_BAR__YOU_WANTED_TO_PASS_NEGATIVE_VALUE_RELATIVE_TO_END_TO_SEEK_BACK_AND_ANALYZE_DATE_IF_STREAMING_SHOULD_BE_OVERWRITTEN_OR_STATIC_APPENDED fileStream.Seek(barSize, SeekOrigin.End);
					fileStream.Seek(-barSize, SeekOrigin.End);
				} catch (Exception ex) {
					string msg = "2/4_FILESTREAM_SEEK_ONE_BAR_FROM_END_THROWN barSize[" + barSize + "]";
					Assembler.PopupException(msg + msig, ex);
					return saved;
				}
				// 1/3 - read the date and see if the timestamp is the same as 
				DateTime dateTimeOpenLastStored = new DateTime(binaryReader.ReadInt64());
				if (dateTimeOpenLastStored > barLastFormedStaticOrCurrentStreaming.DateTimeOpen) {
					string msg = "I_REFUSE_TO_STORE_BAR_EARLIER_THAN_LAST_STORED barLastFormedStaticOrCurrentStreaming.DateTimeOpen["
						+ barLastFormedStaticOrCurrentStreaming.DateTimeOpen + "] > dateTimeOpenLastStored[" + dateTimeOpenLastStored + "]"
						+ " IMPOSSIBLE_TO_CATCH_UPSTACK_KOZ_SOLIDIFIER_DOESNT_KEEP_BARS";
					Assembler.PopupException(msg, null, false);
					return saved;
				}
				long fileStreamLength = fileStream.Length;
				if (barLastFormedStaticOrCurrentStreaming.DateTimeOpen == dateTimeOpenLastStored) {
					try {
						long fileStreamPositionAfterSeekToLastBar = fileStream.Seek(-barSize, SeekOrigin.End);
						string msg = "DEBUGGINNG__OVERWRITING_LAST_BAR_WITH_STREAMING barLastFormedStaticOrCurrentStreaming.DateTimeOpen["
							+ barLastFormedStaticOrCurrentStreaming.DateTimeOpen + "] == dateTimeOpenLastStored[" + dateTimeOpenLastStored + "]"
							+ " fileStreamPositionAfterSeekToLastBar[" + fileStreamPositionAfterSeekToLastBar + "] fileStreamLength[" + fileStreamLength + "]"
							;
						//Assembler.PopupException(msg, null, false);
					} catch (Exception ex) {
						string msg = "3/4_FILESTREAM_SEEK_ONE_BAR_FROM_END_THROWN barSize[" + barSize + "]";
						Assembler.PopupException(msg + msig, ex);
						return saved;
					}
				} else {
					try {
						long fileStreamPositionAfterSeekToEnd = fileStream.Seek(0, SeekOrigin.End);
						string msg = "DEBUGGINNG__APPENDING_FRESHLY_FORMED_STATIC_BAR: barLastFormedStaticOrCurrentStreaming.DateTimeOpen["
							+ barLastFormedStaticOrCurrentStreaming.DateTimeOpen + "] > dateTimeOpenLastStored[" + dateTimeOpenLastStored + "]"
							+ " fileStreamPositionAfterSeekToEnd[" + fileStreamPositionAfterSeekToEnd + "] fileStreamLength[" + fileStreamLength + "]"
							;
						//Assembler.PopupException(msg, null, false);
					} catch (Exception ex) {
						string msg = "3/4_FILESTREAM_SEEK_END_THROWN";
						Assembler.PopupException(msg + msig, ex);
						return saved;
					}
				}
				try {
					binaryWriter.Write(barLastFormedStaticOrCurrentStreaming.DateTimeOpen.Ticks);
					binaryWriter.Write(barLastFormedStaticOrCurrentStreaming.Open);
					binaryWriter.Write(barLastFormedStaticOrCurrentStreaming.High);
					binaryWriter.Write(barLastFormedStaticOrCurrentStreaming.Low);
					binaryWriter.Write(barLastFormedStaticOrCurrentStreaming.Close);
					binaryWriter.Write(barLastFormedStaticOrCurrentStreaming.Volume);
					saved++;
				} catch (Exception ex) {
					string msg = "4/4_BINARYWRITER_WRITER_THROWN";
					Assembler.PopupException(msg + msig, ex);
					return saved;
				}
			} finally {
				if (binaryReader != null) {
					binaryReader.Close();
					binaryReader.Dispose();
				}
				if (binaryWriter != null) {
					binaryWriter.Close();
					binaryWriter.Dispose();
				}
				if (fileStream != null) {
					fileStream.Close();
					fileStream.Dispose();
				}
			}
			return saved;
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