using System;
using System.Collections.Generic;
using System.IO;

using Sq1.Core.DataTypes;

namespace Sq1.Core.Repositories {
	public class RepositoryBarsFile {
		public RepositoryBarsSameScaleInterval BarsRepository { get; protected set; }
		public string Symbol { get; protected set; }
		public string Abspath { get; protected set; }
		public string Relpath { get { return RepositoryBarsFile.GetRelpathFromEnd(this.Abspath, 5); } }
		double version = 1.0;
		
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

				double version = binaryReader.ReadDouble();
				//Assembler.PopupException("LoadBars[" + this.Relpath + "]: version[" + version + "]");
				string symbol = binaryReader.ReadString();
				string symbolHumanReadable = binaryReader.ReadString();
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
				BinaryWriter binaryWriter = new BinaryWriter(fileStream);
				binaryWriter.Write(this.version);
				binaryWriter.Write(bars.Symbol);
				binaryWriter.Write(bars.SymbolHumanReadable);
				binaryWriter.Write((int)bars.ScaleInterval.Scale);
				binaryWriter.Write(bars.ScaleInterval.Interval);
				binaryWriter.Write(bars.Count);
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
		
		public int BarAppend(Bar barLastFormed) {
			Bars allBars = this.BarsLoadAllThreadSafe();
			if (allBars == null) {
				allBars = new Bars(barLastFormed.Symbol, barLastFormed.ScaleInterval, "DUMMY: LoadBars()=null");
			}
			//allBars.DumpPartialInitFromStreamingBar(bar);

			// this happens on a very first quote - this.pushBarToConsumers(StreamingBarFactory.LastBarFormed.Clone());
			if (allBars.BarStaticLast.DateTimeOpen == barLastFormed.DateTimeOpen) return 0;

			// not really needed to clone to save it in a file, but we became strict to eliminate other bugs
			barLastFormed = barLastFormed.CloneDetached();

			// SetParentForBackwardUpdateAutoindex used within Bar only()
			//barLastFormed.SetParentForBackwardUpdateAutoindex(allBars);
			if (allBars.BarStaticLast.DateTimeOpen == barLastFormed.DateTimeOpen) {
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