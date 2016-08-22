using System;
using System.Diagnostics;
using System.Threading;
using System.Collections.Generic;

using Sq1.Core.Serializers;

namespace Sq1.Core.Serializers {
	public class SerializerLogrotatePeriodic<T> : SerializerLogrotate<T> {
				Timer		timer;
		public	long		PeriodMillis;
		public	List<T>		Orders							{ get { return base.EntityDeserialized; } }

				Stopwatch	lastSerialization_elapsed;
				Stopwatch	lastSerialization_millisAgo;
		public	int			LastSerialization_records		{ get; private set; }
		public	TimeSpan	NextSerialization_estimatedIn { get {
			TimeSpan diff = new TimeSpan();
			if (this.lastSerialization_millisAgo.IsRunning == false) return diff;
			diff = new TimeSpan(this.PeriodMillis - this.lastSerialization_millisAgo.ElapsedMilliseconds);
			return diff;
		} }


		public SerializerLogrotatePeriodic(long periodMillis = 10 * 1000) : base() {
			this.PeriodMillis = periodMillis;
			this.lastSerialization_elapsed = new Stopwatch();
			this.lastSerialization_millisAgo = new Stopwatch();
			this.LastSerialization_records = -1;
		}

		public void StartSerializerThread() {
			this.timer = new Timer(new TimerCallback(serializerThreadEntry),
				null, this.PeriodMillis, Timeout.Infinite);
		}
		void serializerThreadEntry(object stateWePassNullHere) {
			if (Assembler.InstanceInitialized.MainFormClosingIgnoreReLayoutDockedForms) {
				this.timer.Change(Timeout.Infinite, Timeout.Infinite);
				return;
			}

			if (string.IsNullOrEmpty(Thread.CurrentThread.Name) &&
				Thread.CurrentThread.Name != "DataSnapshotSerializer") {
				//Thread.CurrentThread.Name = "DataSnapshotSerializer";
			}
			this.lastSerialization_elapsed.Restart();
			try {
				if (base.HasChangesToSave) {
					this.LastSerialization_records = base.Serialize();
				}
				this.lastSerialization_millisAgo.Start();
			} catch (Exception ex) {
				string msig = " SerializerLogrotatePeriodic<" + base.OfWhat + ">::serializerThreadEntry() ";
				string msg = "JSON serialization problems?";
				Assembler.PopupException(msg + msig, ex);
			} finally {
				this.lastSerialization_elapsed.Stop();
				//this.DataSnapshot.OrderProcessor.exceptionsForm.DisplayStatus(
				//	"serialized Orders and HistoricalTrades in [" + watch.ElapsedMilliseconds
				//	+ "]milliseconds at [" + DateTime.Now.ToString("HH:mm:ss.fff") + "]");
				this.timer.Change(this.PeriodMillis, Timeout.Infinite);		// this.PeriodMillis will be different each time (publicly accessible field)
			}
		}
	}
}