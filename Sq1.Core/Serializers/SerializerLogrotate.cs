using System;
using System.Collections.Generic;
using System.IO;

using Newtonsoft.Json;

namespace Sq1.Core.Serializers {
	public class SerializerLogrotate<T> : Serializer<List<T>> {
		long logRotateSizeLimit;
		string logRotateDateFormat;

		Object entityLock;
		public bool HasChangesToSave;
		bool currentlySerializing;
		object itemsBufferedWhileSerializingLock;
		List<T> itemsBufferedWhileSerializing;

		public SerializerLogrotate() : base() {
			this.logRotateSizeLimit = 2 * 1024 * 1024;	// 2Mb
			this.logRotateDateFormat = "yyyy-MM-dd_HH-mm-ss#fff";

			this.entityLock = new Object();
			this.currentlySerializing = false;
			this.itemsBufferedWhileSerializingLock = new object();
			this.itemsBufferedWhileSerializing = new List<T>();
		}

		public override List<T> Deserialize() {
			if (File.Exists(base.JsonAbsFile) == false) return base.EntityDeserialized;
			try {
				string json = File.ReadAllText(base.JsonAbsFile);
				lock (this.entityLock) {
					base.Deserialize();
				}
			} catch (Exception e) {
				string msig = " LogrotateSerializer<" + base.OfWhat + ">::Deserialize(): ";
				string msg = "FAILED_DeserializeLogrotate_WITH_base.JsonAbsFile[" + base.JsonAbsFile + "]";
				throw new Exception(msg + msig, e);
			}
			return base.EntityDeserialized;
		}
		public override void Serialize() {
			if (this.currentlySerializing == true) return;
			if (this.HasChangesToSave == false) return;
			this.HasChangesToSave = false;

			try {
				this.currentlySerializing = true;
				lock (this.entityLock) {
					if (base.EntityDeserialized.Count == 0) return;
					string json = JsonConvert.SerializeObject(base.EntityDeserialized, Formatting.Indented,
						new JsonSerializerSettings {
							TypeNameHandling = TypeNameHandling.Objects
						});
					this.safeRotateWriteAll(base.JsonAbsFile, json);
					lock (this.itemsBufferedWhileSerializingLock) {
						if (this.itemsBufferedWhileSerializing.Count > 0) {
							base.EntityDeserialized.AddRange(this.itemsBufferedWhileSerializing);
							this.itemsBufferedWhileSerializing.Clear();
						}
					}
				}
			} catch (Exception ex) {
				string msig = " LogrotateSerializer<" + base.OfWhat + ">::Serialize(): ";
				string msg = "FAILED_SerializeLogrotate_WITH_this.JsonAbsFile[" + base.JsonAbsFile + "]";
				Assembler.PopupException(msg + msig, ex);
			} finally {
				this.currentlySerializing = false;
			}
		}
		void safeRotateWriteAll(string fileAbspath, string json) {
			this.safeLogRotate(fileAbspath);
			File.WriteAllText(fileAbspath, json);
		}
		void safeLogRotate(string fileAbspath) {
			string msig = " LogrotateSerializer<" + base.OfWhat + ">::safeLogRotate(fileAbspath=[" + fileAbspath + "]): ";
			string relpath = Path.GetDirectoryName(fileAbspath);
			if (Directory.Exists(relpath) == false) {
				try {
					Directory.CreateDirectory(relpath);
				} catch (Exception ex) {
					string msg = "FAILED_TO Directory.CreateDirectory(" + relpath + ")";
					Assembler.PopupException(msg + msig, ex);
				}
			}
			if (File.Exists(fileAbspath) == false) File.Create(fileAbspath).Dispose();

			long size = new FileInfo(fileAbspath).Length;
			if (size < logRotateSizeLimit) return;
			string fileNameWithDate = Path.GetFileNameWithoutExtension(fileAbspath)
				+ "-" + DateTime.Now.ToString(this.logRotateDateFormat);
			string fileAbsNameWithDate = Path.GetDirectoryName(fileAbspath) + Path.DirectorySeparatorChar 
				 + fileNameWithDate + Path.GetExtension(fileAbspath);
			try {
				File.Move(fileAbspath, fileAbsNameWithDate);
				base.EntityDeserialized.Clear();		// hardcore cleanup!!
			} catch (Exception ex) {
				string msg = "FAILED_TO logRotate() File.Move([" + fileAbspath + "], [" + fileAbsNameWithDate + "])";
				Assembler.PopupException(msg + msig, ex);
			}
		}
		public void Insert(int index, T order) {
			if (this.currentlySerializing == true) {
				lock (this.itemsBufferedWhileSerializingLock) {
					this.itemsBufferedWhileSerializing.Insert(index, order);
				}
			} else {
				lock (this.entityLock) {
					base.EntityDeserialized.Insert(index, order);
				}
			}
			this.HasChangesToSave = true;
		}
		public void Remove(List<T> ordersToRemove) { lock (this.entityLock) {
				foreach (T orderRemoving in ordersToRemove) {
					if (base.EntityDeserialized.Contains(orderRemoving)) {
						base.EntityDeserialized.Remove(orderRemoving);
					}
				}
			this.HasChangesToSave = true;
		} }
	}
}