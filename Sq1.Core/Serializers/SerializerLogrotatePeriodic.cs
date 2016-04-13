using System;
using System.Diagnostics;
using System.Threading;
using System.Collections.Generic;

using Sq1.Core.Serializers;

namespace Sq1.Core.Serializers {
	public class SerializerLogrotatePeriodic<T> : SerializerLogrotate<T> {
				Timer		timer;
		public	int			PeriodMillis;
		public	List<T>		Orders { get { return base.EntityDeserialized; } }

		public SerializerLogrotatePeriodic(int periodMillis = 10 * 1000) : base() {
			this.PeriodMillis = periodMillis;
		}

		public void StartSerializerThread() {
			this.timer = new Timer(new TimerCallback(serializerThreadEntry),
				null, this.PeriodMillis, Timeout.Infinite);
		}
		void serializerThreadEntry(object stateWePassNullHere) {
			if (string.IsNullOrEmpty(Thread.CurrentThread.Name) &&
				Thread.CurrentThread.Name != "DataSnapshotSerializer") {
				//Thread.CurrentThread.Name = "DataSnapshotSerializer";
			}
			Stopwatch watch = new Stopwatch();
			watch.Start();
			try {
				base.Serialize();
			} catch (Exception ex) {
				string msig = " SerializerLogrotatePeriodic<" + base.OfWhat + ">::serializerThreadEntry() ";
				string msg = "JSON serialization problems?";
				Assembler.PopupException(msg + msig, ex);
			} finally {
				watch.Stop();
				//this.DataSnapshot.OrderProcessor.exceptionsForm.DisplayStatus(
				//	"serialized Orders and HistoricalTrades in [" + watch.ElapsedMilliseconds
				//	+ "]milliseconds at [" + DateTime.Now.ToString("HH:mm:ss.fff") + "]");
				this.timer.Change(this.PeriodMillis, Timeout.Infinite);		// this.PeriodMillis will be different each time (publicly accessible field)
			}
		}
	}
}