using System;
using System.IO;

using Sq1.Core.DataTypes;

namespace Sq1.Core.Repositories {
	public partial class RepositoryBarsFile {
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
		
		//[Obsolete("replaced by barAppendStaticUnconditional()")]
		//int barStaticAppend_saveToFileFullCopy_slow(Bar barLastFormed) {
		//    //v1
		//    Bars allBars = this.BarsLoadAll_nullUnsafe_threadSafe();
		//    if (allBars == null) {
		//        allBars = new Bars(barLastFormed.Symbol, barLastFormed.ScaleInterval, "DUMMY: LoadBars()=null");
		//    }
		//    //allBars.DumpPartialInitFromStreamingBar(bar);
			
		//    // this happens on a very first quote - this.pushBarToConsumers(StreamingBarFactory.LastBarFormed.Clone());
		//    if (allBars.BarStaticLast_nullUnsafe.DateTimeOpen == barLastFormed.DateTimeOpen) return 0;
			
		//    // not really needed to clone to save it in a file, but we became strict to eliminate other bugs
		//    barLastFormed = barLastFormed.CloneDetached();
			
		//    // SetParentForBackwardUpdateAutoindex used within Bar only()
		//    //barLastFormed.SetParentForBackwardUpdateAutoindex(allBars);
		//    if (allBars.BarStaticLast_nullUnsafe.DateTimeOpen == barLastFormed.DateTimeOpen) {
		//        return 0;
		//    }
			
		//    allBars.BarStatic_appendAttach(barLastFormed);
		//    int barsSaved = this.BarsSave_threadSafe(allBars);
		//    return barsSaved;
		//}
		//[Obsolete("BarAppendStatic_unconditional_threadSafe() doesnt allow StreamingBar be saved every 10 seconds so there is risk of loosing current-day-bar after app restart")]
		//public int BarAppendStatic_unconditional_threadSafe(Bar barLastFormed) {
		//    int barsAppended = -1;
		//    lock (this.fileReadWriteSequentialLock) {
		//        barsAppended = this.barAppendStatic_unconditional(barLastFormed);
		//        //Assembler.PopupException("Saved [ " + bars.Count + "] bars; symbol[" + bars.Symbol + "] scaleInterval[" + bars.ScaleInterval + "]");
		//    }
		//    return barsAppended;
		//}
		[Obsolete("barAppendStaticUnconditional() doesnt allow StreamingBar be saved every 10 seconds so there is risk of loosing current-day-bar after app restart")]
		int barAppendStatic_unconditional(Bar barLastFormed) {
			int saved = 0;
			string msig = " barAppendStatic(" + barLastFormed + ")=>[" + this.Abspath + "]";

			try {
				barLastFormed.ValidateBar_alignToSteps_fixOCbetweenHL();	//	catching the exception will display stacktrace in ExceptionsForm
			} catch (Exception ex) {
				string msg = "NOT_APPENDING_TO_FILE_THIS_BAR__FIX_WHO_GENERATED_IT_UPSTACK barAllZeroes barLastFormed[" + barLastFormed + "]";
				Assembler.PopupException(msg + msig, ex, false);
				return saved;
			}

			FileStream fileStream = null;
			BinaryWriter binaryWriter = null;
			try {
				fileStream = File.Open(this.Abspath, FileMode.Open, FileAccess.Write, FileShare.None);
			} catch (Exception ex) {
				string msg = "1/4_FILE_OPEN_THROWN";
				Assembler.PopupException(msg + msig, ex);
				return saved;
			}
			try {
				binaryWriter = new BinaryWriter(fileStream);
				try {
					fileStream.Seek(0, SeekOrigin.End);
				} catch (Exception ex) {
					string msg = "2/4_FILESTREAM_SEEK_END_THROWN Seek(0, SeekOrigin.End)";
					Assembler.PopupException(msg + msig, ex);
					return saved;
				}
				try {
					binaryWriter.Write(barLastFormed.DateTimeOpen.Ticks);
					binaryWriter.Write(barLastFormed.Open);
					binaryWriter.Write(barLastFormed.High);
					binaryWriter.Write(barLastFormed.Low);
					binaryWriter.Write(barLastFormed.Close);
					binaryWriter.Write(barLastFormed.Volume);
					saved++;
				} catch (Exception ex) {
					string msg = "3/4_BINARYWRITER_WRITER_THROWN";
					Assembler.PopupException(msg + msig, ex);
					return saved;
				}
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
			return saved;
		}
		
		//public Bars BarsLoad_nullUnsafeThreadSafe_NOT_USED(DateTime dateFrom, DateTime dateTill, int maxBars) {
		//    Bars barsAll = this.BarsLoadAll_nullUnsafe_threadSafe();
		//    if (barsAll == null) return barsAll;
			
		//    //Assembler.PopupException("Loaded [ " + bars.Count + "] bars; symbol[" + this.Symbol + "] scaleInterval[" + this.BarsFolder.ScaleInterval + "]");
		//    if (dateFrom == DateTime.MinValue && dateTill == DateTime.MaxValue && maxBars == 0) return barsAll;

		//    string start = (dateFrom == DateTime.MinValue) ? "MIN" : dateFrom.ToString("dd-MMM-yyyy");
		//    string end = (dateTill == DateTime.MaxValue) ? "MAX" : dateTill.ToString("dd-MMM-yyyy");
		//    Bars bars = new Bars(barsAll.Symbol, barsAll.ScaleInterval, barsAll.ReasonToExist + " [" + start + "..." + end + "]max[" + maxBars + "]");
		//    for (int i = 0; i < barsAll.Count; i++) {
		//        if (maxBars > 0 && i >= maxBars) break;
		//        Bar barAdding = barsAll[i];
		//        bool skipThisBar = false;
		//        if (dateFrom > DateTime.MinValue && barAdding.DateTimeOpen < dateFrom) skipThisBar = true;
		//        if (dateTill < DateTime.MaxValue && barAdding.DateTimeOpen > dateTill) skipThisBar = true;
		//        if (skipThisBar) continue;
		//        bars.BarStatic_appendAttach(barAdding.CloneDetached());
		//    }
		//    return bars;
		//}

	}
}