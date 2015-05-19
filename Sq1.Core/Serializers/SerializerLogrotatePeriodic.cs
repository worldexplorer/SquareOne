using System;
using System.Diagnostics;
using System.Threading;

using Sq1.Core.Serializers;
using System.Collections.Generic;
using Sq1.Core.Execution;

namespace Sq1.Core.Serializers {
	public class SerializerLogrotatePeriodic<T> : SerializerLogrotate<T> {
		public string OfWhat { get { return typeof(T).Name; } }

		public List<T> Orders { get { return base.EntityDeserialized; } }

		Timer timer;
		int periodMillis;

		public SerializerLogrotatePeriodic(int periodMillis = 10 * 1000) : base() {
			this.periodMillis = periodMillis;
		}

		public void StartSerializerThread() {
			this.timer = new Timer(new TimerCallback(serializerThreadEntry),
				null, this.periodMillis, Timeout.Infinite);
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
				string msig = " SerializerLogrotatePeriodic<" + this.OfWhat + ">::serializerThreadEntry() ";
				string msg = "JSON serialization problems?";
				base.ThrowOrPopup(msg + msig, ex);
			} finally {
				watch.Stop();
				//this.DataSnapshot.OrderProcessor.exceptionsForm.DisplayStatus(
				//	"serialized Orders and HistoricalTrades in [" + watch.ElapsedMilliseconds
				//	+ "]milliseconds at [" + DateTime.Now.ToString("HH:mm:ss.fff") + "]");
				this.timer.Change(this.periodMillis, Timeout.Infinite);
			}
		}
	}
}