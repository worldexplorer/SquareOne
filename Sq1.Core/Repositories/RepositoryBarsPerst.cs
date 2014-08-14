using System;
using Perst;
using Sq1.Core.DataTypes;

namespace Sq1.Core.Repositories {
	public class BarPerst : TimeSeriesTick {
		public long Time { get; set; }
		public double Open;
		public double High;
		public double Low;
		public double Close;
		public double Volume;
		public BarPerst(Bar bar) {
			this.Time = bar.DateTimeOpen.ToBinary();
			this.Open = bar.Open;
			this.High = bar.High;
			this.Low = bar.Low;
			this.Close = bar.Close;
			this.Volume = bar.Volume;
		}
	}

    public class BarsPerst : Persistent { 
		public string Symbol;
		public string SecurityName;
		public BarScaleInterval ScaleInterval;
        public TimeSeries<BarPerst> BarsStored;
    }

	public class RepositoryBarsPerst {
		public const int N_ELEMS_PER_BLOCK = 100;
	    long TICKS_PER_SECOND = 10000000L;
	    const int pagePoolSize = 32*1024*1024;

		public RepositoryBarsSameScaleInterval BarsRepository { get; protected set; }
		public string Symbol { get; protected set; }
		public string Abspath { get; protected set; }
		
		public RepositoryBarsPerst(RepositoryBarsSameScaleInterval barsFolder, string symbol, bool throwIfDoesntExist = true, bool createIfDoesntExist = false) {
			this.BarsRepository = barsFolder;
			this.Symbol = symbol;
			this.Abspath = this.BarsRepository.AbspathForSymbol(this.Symbol, throwIfDoesntExist, createIfDoesntExist);
		}
		
		public int BarsSave(Bars bars) {
			int ret = 0;
	        DateTime start = DateTime.Now;
	        Storage db = StorageFactory.Instance.CreateStorage();
	        db.Open(this.Abspath, pagePoolSize);
	        BarsPerst barsPerst = db.Root as BarsPerst;
	        barsPerst = (BarsPerst) db.CreateClass(typeof(BarsPerst));
	        barsPerst.Symbol = bars.Symbol;
	        barsPerst.SecurityName = bars.SecurityName;
	        barsPerst.ScaleInterval = bars.ScaleInterval;

			/// If number of element in block is 100, time series period is 1 day, then
			/// value of maxBlockTimeInterval can be set as 100*(24*60*60*10000000L)*2
			//long maxBlockTimeInterval = (number of elements in block)*(tick interval)*2;
			TICKS_PER_SECOND = bars.ScaleInterval.TimeSpanInSeconds;

	        barsPerst.BarsStored = db.CreateTimeSeries<BarPerst>(N_ELEMS_PER_BLOCK, N_ELEMS_PER_BLOCK*TICKS_PER_SECOND*2);
	        foreach (Bar bar in bars.Values) {
	        	BarPerst barPerst = new BarPerst(bar);
				barsPerst.BarsStored.Add(barPerst);
	        }
	        ret = barsPerst.BarsStored.Count;
	        db.Root = barsPerst;
	        db.Commit();
	        db.Close();
			string msg = "Elapsed time for storing " + ret + " bars: " + (DateTime.Now - start);
			return ret;
		}
		public Bars BarsRead() {
			Bars ret = null;
			DateTime start = DateTime.Now;
			Storage db = StorageFactory.Instance.CreateStorage();
			db.Open(this.Abspath, pagePoolSize);
			BarsPerst barsPerst = db.Root as BarsPerst;
			if (barsPerst == null) return ret;
			ret = new Bars(barsPerst.Symbol, barsPerst.ScaleInterval, this.Abspath);
			foreach (BarPerst barPerst in barsPerst.BarsStored) {
				ret.BarCreateAppend(new DateTime(barPerst.Time), barPerst.Open, barPerst.High, barPerst.Low, barPerst.Close, barPerst.Volume);
			}
			db.Close();
			string msg = "Elapsed time for reading " + ret.Count + " bars: " + (DateTime.Now - start);
			return ret;
		}
	}
}
