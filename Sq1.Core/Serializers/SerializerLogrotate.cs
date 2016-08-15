using System;
using System.Collections.Generic;
using System.IO;

using Newtonsoft.Json;

namespace Sq1.Core.Serializers {
	public class SerializerLogrotate<T> : Serializer<List<T>> {
		public const float KILOBYTE = 1024f;
		public const float MEGABYTE = (1024f * 1024f);
		//public const float LIMIT_DEFAULT = 2 * MEGABYTE;

				string	logRotateDateFormat;
				long	logRotateSizeLimit_bytes;
		public	float	LogRotateSizeLimit_Mb {
			get { return this.logRotateSizeLimit_bytes / MEGABYTE; }
			set { this.logRotateSizeLimit_bytes = (int) Math.Round(value * MEGABYTE); }
		}

				Object	entityLock;
		public	bool	HasChangesToSave;
				bool	currentlySerializing;
				object	itemsBufferedWhileSerializingLock;
				List<T>	itemsBuffered_whileSerializing;

		public SerializerLogrotate(float twoMb = 2.0f) : base() {
			this.entityLock = new Object();
			this.currentlySerializing = false;
			this.itemsBufferedWhileSerializingLock = new object();
			this.itemsBuffered_whileSerializing = new List<T>();
			base.OfWhat = typeof(T).Name;

			this.logRotateDateFormat = "yyyy-MM-dd_HH-mm-ss#fff";
			//v1 this.logRotateSizeLimit_bytes = 2 * 1024 * 1024;	// 2Mb
			this.LogRotateSizeLimit_Mb = twoMb;
		}

		public override List<T> Deserialize() {
			if (File.Exists(base.JsonAbsFile) == false) return base.EntityDeserialized;
			try {
				string json = File.ReadAllText(base.JsonAbsFile);
				lock (this.entityLock) {
					base.Deserialize();
				}
			} catch (Exception e) {
				string msig = " //" + base.ToString() + "::Deserialize()";
				string msg = "FAILED_DeserializeLogrotate_WITH_base.JsonAbsFile[" + base.JsonAbsFile + "]";
				throw new Exception(msg + msig, e);
			}
			return base.EntityDeserialized;
		}
		public override int Serialize() {
			int recordsSerialized = 0;
			if (this.currentlySerializing == true) {
				string msg = "WILL_SERIALIZE_NEXT_TIME__PERIODIC_THREAD_WILL_INVOKE_ME_ONCE_AGAIN";
				Assembler.PopupException(msg, null, false);
				return recordsSerialized;
			}
			if (this.HasChangesToSave == false) return recordsSerialized;
			this.HasChangesToSave = false;

			try {
				this.currentlySerializing = true;
				lock (this.entityLock) {
					// AFTER_I_DELETED_ALL_I_SHOULD_SAVE_ZERO if (base.EntityDeserialized.Count == 0) return recordsSerialized;
					recordsSerialized = base.EntityDeserialized.Count;
					string json = JsonConvert.SerializeObject(base.EntityDeserialized, Formatting.Indented,
						new JsonSerializerSettings {
							TypeNameHandling = TypeNameHandling.Objects
						});
					this.safeRotateWriteAll(base.JsonAbsFile, json);
					lock (this.itemsBufferedWhileSerializingLock) {
						if (this.itemsBuffered_whileSerializing.Count > 0) {
							base.EntityDeserialized.AddRange(this.itemsBuffered_whileSerializing);
							this.itemsBuffered_whileSerializing.Clear();
						}
					}
				}
			} catch (Exception ex) {
				string msig = "  //" + base.ToString() + "::Serialize()";
				string msg = "FAILED_SerializeLogrotate_WITH_this.JsonAbsFile[" + base.JsonAbsFile + "]";
				Assembler.PopupException(msg + msig, ex, false);
			} finally {
				this.currentlySerializing = false;
			}
			return recordsSerialized;
		}
		void safeRotateWriteAll(string fileAbspath, string json) {
			this.safeLogRotate(fileAbspath);
			File.WriteAllText(fileAbspath, json);
		}
		void safeLogRotate(string fileAbspath) {
			string msig = " //" + base.ToString() + "::safeLogRotate(fileAbspath=[" + fileAbspath + "])";
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
			if (size < logRotateSizeLimit_bytes) return;
			string fileNameWithDate = Path.GetFileNameWithoutExtension(fileAbspath)
				+ "-" + DateTime.Now.ToString(this.logRotateDateFormat);	// Orders-2016-07-05_10-58-48#448.json
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
					this.itemsBuffered_whileSerializing.Insert(index, order);
				}
			} else {
				lock (this.entityLock) {
					base.EntityDeserialized.Insert(index, order);
				}
			}
			this.HasChangesToSave = true;
		}
		public int RemoveRange(List<T> ordersToRemove) { lock (this.entityLock) {
			int ret = 0;
			foreach (T orderRemoving in ordersToRemove) {
				if (base.EntityDeserialized.Contains(orderRemoving) == false) continue;
				base.EntityDeserialized.Remove(orderRemoving);
				ret++;
			}
			this.HasChangesToSave = true;
			return ret;
		} }
		public void Clear() { lock (this.entityLock) {
			base.EntityDeserialized.Clear();
			this.HasChangesToSave = true;
		} }


		public string			 AllLogrotatedSize_butNotMainJson_scanned { get; private set; }		// filled after accessing AllLogrotatedAbsFnames_butNotMainJson_scanned
		public List<string> AllLogrotatedAbsFnames_butNotMainJson_scanned { get {
			this.AllLogrotatedSize_butNotMainJson_scanned = "[xx]Mb";
			List<string> logrotatedAbsFnames = new List<string>();
			if (Directory.Exists(base.AbsPath) == false) {
				return logrotatedAbsFnames;
			}

			string mask = this.OfWhat + "s-*.json";		// Orders-2016-07-05_10-58-48#448.json
			string[] absFileNames = Directory.GetFiles(this.AbsPath, mask);
			long totalBytes = 0;
			for (int i = 0; i < absFileNames.Length; i++) {
				string absFileName = absFileNames[i];
				string thisOne = "[" + absFileName + "]=[" + i + "]/[" + absFileNames.Length + "]";
				logrotatedAbsFnames.Add(absFileName);

				FileInfo finfo = new FileInfo(absFileName);
				totalBytes += finfo.Length;
			}

			float totalKiloBytes = totalBytes / 1024f; 
			if (totalKiloBytes < 1024) {
				string sizeKb = "[" + Math.Round(totalKiloBytes).ToString("N0") + "]Kb";
				this.AllLogrotatedSize_butNotMainJson_scanned = sizeKb;
			} else {
				float totalMegaBytes = totalKiloBytes / 1024f; 
				string sizeMb = "[" + Math.Round(totalMegaBytes, 2).ToString("N2") + "]Mb";
				this.AllLogrotatedSize_butNotMainJson_scanned = sizeMb;
			}
			return logrotatedAbsFnames;
		} }
		public int FindAndDelete_allLogrotatedFiles_butNotMainJson() {
			string msig = " //" + base.ClassIdent + "::FindAndDelete_allLogrotatedFiles_butNotMainJson()";
			int filesDeleted = 0;

			List<string> logrotatedAbsFnames = this.AllLogrotatedAbsFnames_butNotMainJson_scanned;
			if (logrotatedAbsFnames.Count == 0) {
				string msg = "YOU_SHOULD_HAVE_ANALYZED_this.AllLogrotatedFiles_butNotMainJson_BEFORE_INVOKING_FindAndDelete";
				Assembler.PopupException(msg);
				return filesDeleted;
			}
			foreach (string logrotatedAbsFname in logrotatedAbsFnames) {
				try {
					File.Delete(logrotatedAbsFname);
					filesDeleted++;
				} catch (Exception ex) {
					string msg = "YOU_MUST_HAVE_UNZIPPED_WITH_INEFFECTIVE_PERMISSIONS[" + logrotatedAbsFname + "]";
					Assembler.PopupException(msg + msig, ex);
				}
			}
			if (this.AllLogrotatedAbsFnames_butNotMainJson_scanned.Count > 0) {
				string msg = "NO_LOGROTATED_FILE_MUST_BE_LEFT";
				Assembler.PopupException(msg + msig);
			}
			return filesDeleted;
		}
	}
}