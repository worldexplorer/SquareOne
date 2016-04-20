using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Threading;

using Sq1.Core.Broker;
using Sq1.Core.Charting;
using Sq1.Core.DataTypes;
using Sq1.Core.Execution;
using Sq1.Core.Repositories;
using Sq1.Core.Serializers;
using Sq1.Core.Support;

namespace Sq1.Core {
	public class Assembler {
		public	RepositorySerializerSymbolInfos			RepositorySymbolInfos;
		public	RepositorySerializerMarketInfos			RepositoryMarketInfos;
		public	RepositoryDllJsonStrategies				RepositoryDllJsonStrategies;
		public	RepositoryJsonDataSources				RepositoryJsonDataSources;

		public	RepositoryDllStreamingAdapters			RepositoryDllStreamingAdapters;
		public	RepositoryDllBrokerAdapters				RepositoryDllBrokerAdapters;
		public	RepositoryDllReporters					RepositoryDllReporters;

		public	RepositoryFoldersNoJson					WorkspacesRepository;
		
		public	OrderProcessor							OrderProcessor;
		public	IStatusReporter							StatusReporter;
		//public	DockContentImproved						ExecutionForm;
		
		public	DictionaryManyToOne<ChartShadow, Alert>	AlertsForChart;
		public	AssemblerDataSnapshot					AssemblerDataSnapshot;
		public	Serializer<AssemblerDataSnapshot>		AssemblerDataSnapshotSerializer;
		//public	RepositorySerializerChartSettingsTemplates	RepositorySerializerChartSettingsTemplates;
		public RepositoryJsonChartSettings				RepositoryJsonChartSettings;

		public	const string							DateTimeFormatIndicatorHasNoValuesFor	= "yyyy-MMM-dd ddd HH:mm";
		public	const string							DateTimeFormatToDays					= "yyyy-MMM-dd";
		//public	const string							DateTimeFormatToHours					= "yyyy-MMM-dd HH";
		public	const string							DateTimeFormatToMinutes					= "yyyy-MMM-dd HH:mm";
		public	const string							DateTimeFormatToMinutes_noYear			= "MMM-dd HH:mm:ss";
		public	const string							DateTimeFormatToMinutesSeconds_noYear	= "MMM-dd HH:mm:ss";
		public	const string							DateTimeFormatLong						= "HH:mm:ss.fff ddd dd MMM yyyy";
		public	const string							DateTimeFormatLongFilename				= "yyyy-MMM-dd_ddd_HH.mm.ss";
		
		public	static string FormattedLongFilename(DateTime dt) {
			return dt.ToString(Assembler.DateTimeFormatLongFilename);
		}

		public	bool									MainFormClosingIgnoreReLayoutDockedForms = false;
		public	bool									MainForm_dockFormsFullyDeserialized_layoutComplete = false;

				static Assembler						instance = null;
		public	static Assembler						InstanceInitialized { get {
				string usage = "; use Assembler.InstanceUninitialized.Initialize(MainForm)"
					+ "; this singleton requires IStatusReporter to get fully initialized"
					+ "; if you see me in ChartForm designer, uncomment in ChartShadow: [will let you open ChartControl in windows forms Designer]";
				if (Assembler.instance == null) {
					throw (new Exception("Assembler.instance=null" + usage));
				}
				if (Assembler.instance.StatusReporter == null) {
					throw (new Exception("Assembler.instance.StatusReporter=null" + usage));
				}
				return Assembler.instance;
			} }
		public	static bool								IsInitialized { get { return Assembler.instance != null && Assembler.instance.StatusReporter != null; } }
		public	static Assembler						InstanceUninitialized { get {
				if (Assembler.instance == null) {
					Assembler.instance = new Assembler();
				}
				return instance;
			} }
		public	string									AppStartupPath { get {
				string ret = Application.StartupPath;
				if (ret.EndsWith(Path.DirectorySeparatorChar.ToString()) == false) ret += Path.DirectorySeparatorChar;
				return ret;
			} }
		public	List<Exception>							ExceptionsWhileInstantiating { get {
				List<Exception> ret = new List<Exception>();
				ret.AddRange(this.RepositoryDllStreamingAdapters.ExceptionsWhileScanning);
				ret.AddRange(this.RepositoryDllBrokerAdapters.ExceptionsWhileScanning);
				ret.AddRange(this.RepositoryDllReporters.ExceptionsWhileScanning);
				ret.AddRange(this.RepositoryDllJsonStrategies.ExceptionsWhileInstantiating);
				return ret;
			} }
#if DEBUG
		// C:\Sq1\Data-debug
		public	readonly string DATA_FOLDER_DEBUG_RELEASE = ".." + Path.DirectorySeparatorChar + ".." + Path.DirectorySeparatorChar + ".." + Path.DirectorySeparatorChar
			+ "Data-debug";
#else
		// C:\Sq1\Sq1.Gui\bin\Debug\Data
		public	readonly string DATA_FOLDER_DEBUG_RELEASE = "Data";
#endif
		public	string									AppDataPath { get {
				string ret = this.AppStartupPath + DATA_FOLDER_DEBUG_RELEASE;
				//if (defined("DEBUG")) ret = Application.UserAppDataPath + "" + Path.DirectorySeparatorChar + "Data";
				if (Directory.Exists(ret) == false) Directory.CreateDirectory(ret);
				return ret;
			} }

		// NOT_UNDER_WINDOWS
				Stopwatch								stopWatchIfProcessUnsupported;
		public	bool									SplitterEventsAllowed_tenSecondsPassed_afterAppLaunch_hopingInitialInnerDockResizingIsFinished { get {
				bool ret = false;
				//v1 looks like Process.GetCurrentProcess().StartTime counts the milliseconds elapsed by the processor for the app
				//(1sec during last 2mins with 1% CPU load), while I need solarTimeNow minus solarTimeAppStarted => switching to Stopwatch
				double sinceApplicationStartSeconds = -1;
				try {
					if (stopWatchIfProcessUnsupported == null) {
						stopWatchIfProcessUnsupported = new Stopwatch();
						stopWatchIfProcessUnsupported.Start();
					}
					TimeSpan sinceApplicationStart = DateTime.Now - Process.GetCurrentProcess().StartTime;
					sinceApplicationStartSeconds = sinceApplicationStart.TotalSeconds;
					//v2
					//int sinceApplicationStartSeconds2 = this.stopWatchIfProcessUnsupported.Elapsed.Seconds;
				} catch (Exception ex) {
					Assembler.PopupException("SEEMS_TO_BE_UNSUPPORTED_Process.GetCurrentProcess()", ex);
					sinceApplicationStartSeconds = this.stopWatchIfProcessUnsupported.Elapsed.Seconds;
				}

				double secondsLeftToIgnore = this.AssemblerDataSnapshot.SplitterEvents_shouldBeIgnored_duringFirstTenSeconds_afterAppLaunch - sinceApplicationStartSeconds;
				if (secondsLeftToIgnore > 0) {
					string msg = "SPLITTER_EVENTS_IGNORED_FOR_MORE_SECONDS " + secondsLeftToIgnore + "/"
						+ this.AssemblerDataSnapshot.SplitterEvents_shouldBeIgnored_duringFirstTenSeconds_afterAppLaunch;
					//Assembler.PopupException(msg, null, false);
				} else {
					ret = true;
				}
				return ret;
			} }
		
		public Assembler() {
			//AssemblerDataSnapshot					= new AssemblerDataSnapshot();
			AssemblerDataSnapshotSerializer			= new Serializer<AssemblerDataSnapshot>();
			bool createdNewFile = this.AssemblerDataSnapshotSerializer.Initialize(this.AppDataPath, "AssemblerDataSnapshot.json", "", null);
			if (createdNewFile) {
				this.AssemblerDataSnapshotSerializer.Serialize();
			} else {
				this.AssemblerDataSnapshot = this.AssemblerDataSnapshotSerializer.Deserialize();
			}

			RepositorySymbolInfos					= new RepositorySerializerSymbolInfos();
			RepositoryMarketInfos					= new RepositorySerializerMarketInfos();
			RepositoryDllJsonStrategies				= new RepositoryDllJsonStrategies();
			RepositoryJsonDataSources				= new RepositoryJsonDataSources();

			WorkspacesRepository					= new RepositoryFoldersNoJson();

			OrderProcessor							= new OrderProcessor();
			AlertsForChart							= new DictionaryManyToOne<ChartShadow, Alert>();

			//RepositorySerializerChartSettingsTemplates = new RepositorySerializerChartSettingsTemplates();
			RepositoryJsonChartSettings	= new RepositoryJsonChartSettings();
		}
		public Assembler Initialize(IStatusReporter mainForm, bool usedOnlyToPopupExceptions_NPEunsafe = false) {
			if (this.StatusReporter != null && this.StatusReporter != mainForm) {
				string msg = "Assembler.InstanceInitialized.StatusReporter[" + this.StatusReporter + "] != mainForm[" + mainForm + "]";
				msg += "; you initialize the StatusReporter and ExecutionForm once per lifetime;"
					+ " if you need to re-initialize singleton with new IStatusReporter then refactor it"
					+ " and introduce Reset(IStatusReporter) method which won't throw this;"
					+ " or you can just set Assembler.InstanceInitialized.StatusReporter={your different IStatusReporter}"
					+ " to use Assembler.PopupException(), but no guarantee that OrderProcessor and DataSourceRepository will work properly";
				throw new Exception();
			}
			this.StatusReporter = mainForm;
			if (usedOnlyToPopupExceptions_NPEunsafe) return Assembler.InstanceInitialized;

			#region moved from Ctor() because those use Assembler.InstanceInitialized.AssemblerDataSnapshot.ReferencedNetAssmblies*
			RepositoryDllStreamingAdapters			= new RepositoryDllStreamingAdapters(this.AppStartupPath);
			RepositoryDllBrokerAdapters				= new RepositoryDllBrokerAdapters	(this.AppStartupPath);
			RepositoryDllReporters					= new RepositoryDllReporters		(this.AppStartupPath);
			#endregion


			bool createdNewFile = this.RepositorySymbolInfos.Initialize(this.AppDataPath, "SymbolInfo.json", "", null);

			//v1  this.RepositorySymbolInfo		.DeserializeAndSort();
			//v2 SORTED_IN_SymbolInfoEditorControl()_BY_this.toolStripItemComboBox1.ComboBox.Sorted=true;
			this.RepositorySymbolInfos			.Deserialize();
			
			createdNewFile = this.RepositoryMarketInfos.Initialize(this.AppDataPath, "MarketInfo.json", "", null);
			this.RepositoryMarketInfos			.Deserialize();
			
			this.RepositoryDllJsonStrategies	.Initialize(this.AppDataPath, this.AppStartupPath);

			this.WorkspacesRepository			.Initialize(this.AppDataPath, "Workspaces");
			this.WorkspacesRepository			.RescanFolders();

			this.OrderProcessor					.Initialize(this.AppDataPath);

			//v1 this.RepositoryJsonDataSource	.Initialize(this.AppDataPath);
			//v1 this.RepositoryJsonDataSource	.DataSourcesDeserialize(this.MarketInfoRepository, this.OrderProcessor, this.StatusReporter);
			
			this.RepositoryJsonDataSources		.Initialize(this.AppDataPath, "DataSources", this.RepositoryMarketInfos, this.OrderProcessor);
			this.RepositoryJsonDataSources		.DeserializeJsonsInFolder_ifNotCached();
			//SNAP_IS_NOT_SERIALIZED_ANYMORE this.RepositoryJsonDataSource		.ReattachDataSnaphotsToOwnersStreamingAdapters();

			//createdNewFile = this.RepositorySerializerChartSettingsTemplates.Initialize(this.AppDataPath, "MarketInfo.json", "", null);
			//this.RepositoryMarketInfo.Deserialize();

			this.RepositoryJsonChartSettings.Initialize(this.AppDataPath, "ChartSettingsTemplates");
			this.RepositoryJsonChartSettings.DeserializeJsonsInFolder_IfNoneCreateDefault();
			
			
			//v1
			try {
				TimeSpan sinceApplicationStart = DateTime.Now - Process.GetCurrentProcess().StartTime;
				string msg = "ASSEMBLER_INITIALIZED_AFTER_MILLIS_RUNNING: "
					+ Math.Round(sinceApplicationStart.Milliseconds / (decimal)1000, 2)
					+ " [" + this.AssemblerDataSnapshot.SplitterEvents_shouldBeIgnored_duringFirstTenSeconds_afterAppLaunch
					+ "]=SplitterEventsShouldBeIgnoredSecondsAfterAppLaunch";
				//Assembler.PopupException(msg, null, false);
			} catch (Exception ex) {
				string msg = "NEVER_HAPPENED_UNDER_WINDOWS PLATFORM_DOESNT_SUPPORT_Process.GetCurrentProcess().StartTime_AS_DESCRIBED_IN_MSDN__LAUNCHING_STOPWATCH";
				Assembler.PopupException(msg, ex);
				//this.stopWatchIfProcessUnsupported = Stopwatch.StartNew();
			}
			//v2
			//this.stopWatchIfProcessUnsupported = Stopwatch.StartNew();

			return Assembler.InstanceInitialized;
		}
		
		public void checkThrowIfNotInitializedStaticHelper(Exception ex = null) {
//			if (Assembler.InstanceInitialized == null) {
//				MessageBox.Show(ex.Message, "Assembler.InstanceInitialized=null");
//				//throw ex;
//			}
			if (Assembler.InstanceInitialized.StatusReporter == null) {
				string msg2 = "Assembler.InstanceInitialized.StatusReporter=null";
				//MessageBox.Show(ex.Message, msg);
				throw new Exception(msg2, ex);
			}
		}
		
		public static void PopupException(string msg, Exception ex = null, bool debuggingBreak = true) {
			if (Assembler.IsInitialized == false) {
				throw new Exception(msg, ex);
				//return;
			}
			Assembler.InstanceInitialized.checkThrowIfNotInitializedStaticHelper();
			
			try {
				// this is gonna throw from a non-GUI thread, right?!... (moved to MainForm.PopupException() with base.BeginInvoke() as first step)
				// if I PopupException from a BrokerAdapter thread, exceptionsForm.Visible and others should throw
				Form exceptionsForm = Assembler.InstanceInitialized.StatusReporter as Form;
				Assembler.InstanceInitialized.StatusReporter.PopupException(msg, ex, debuggingBreak);
			} catch (Exception ex1) {
				string msg1 = "NOWHERE_ELSE_ASSEMBLER_COULD_DUMP_THIS_EXCEPTION";
				#if DEBUG
				Debugger.Break();
				#else
				throw new Exception (msg1, ex1);
				#endif
			}
		}
		public static void DisplayStatus(string msg) {
			Assembler.InstanceInitialized.checkThrowIfNotInitializedStaticHelper();
			Assembler.InstanceInitialized.StatusReporter.DisplayStatus(msg);
		}

		public static void DisplayConnectionStatus(ConnectionState state, string msg) {
			Assembler.InstanceInitialized.checkThrowIfNotInitializedStaticHelper();
			Assembler.InstanceInitialized.StatusReporter.DisplayConnectionStatus(state, msg);
		}

		#region self-managed, no need to construct nor initialize
		public static void Exceptions_duringApplicationShutdown_InsertAndSerialize(Exception exc, int indexToInsert = 0) {
			Assembler.InstanceInitialized.checkThrowIfNotInitializedStaticHelper();
			Assembler.InstanceInitialized.exceptions_duringApplicationShutdown_InsertAndSerialize(exc, indexToInsert);
		}
		public static void ExceptionsDuringApplicationShutdown_PopupNotepad() {
			Assembler.InstanceInitialized.checkThrowIfNotInitializedStaticHelper();
			Assembler.InstanceInitialized.exception_duringApplicationShutdown_popupNotepad();
		}
		
		public	List<Exception>							Exceptions_duringApplicationShutdown;
		public	Serializer<List<Exception>>				Exceptions_duringApplicationShutdown_serializer;
		void exceptions_duringApplicationShutdown_InsertAndSerialize(Exception exc, int indexToInsert = 0) {
			if (exc == null) return;
			if (this.Exceptions_duringApplicationShutdown == null) {
				//v1 TRYING_TO_FIX_BY_MOVING_TO_ASSEMBLER produced useless "[ null ]" file
				this.Exceptions_duringApplicationShutdown = new List<Exception>();
				this.Exceptions_duringApplicationShutdown_serializer = new Serializer<List<Exception>>();
				string now = Assembler.FormattedLongFilename(DateTime.Now);
				bool createdNewFile = this.Exceptions_duringApplicationShutdown_serializer.Initialize(this.AppDataPath,
					"ExceptionsDuringSquareOneShutdown-" + now + ".json", "Exceptions", null, true, true);
				this.Exceptions_duringApplicationShutdown = this.Exceptions_duringApplicationShutdown_serializer.Deserialize(); 
				//v2
				//return;
			}
			this.Exceptions_duringApplicationShutdown.Insert(0, exc);
			//COLLECTION_MODIFIED_EXCEPTION__LAZY_TO_LOCK_MOVED_TO_PopupNotepad
			if (this.mainFormAlreadyClosed) {
				this.Exceptions_duringApplicationShutdown_serializer.Serialize();
				Process.Start("notepad.exe ", this.Exceptions_duringApplicationShutdown_serializer.JsonAbsFile);
			}
		}
		bool mainFormAlreadyClosed = false;
		void exception_duringApplicationShutdown_popupNotepad() {
			this.mainFormAlreadyClosed = true;
			if (this.Exceptions_duringApplicationShutdown == null) return;
			if (this.Exceptions_duringApplicationShutdown.Count == 0) return;
			this.Exceptions_duringApplicationShutdown_serializer.Serialize();
			// http://stackoverflow.com/questions/7613576/how-to-open-text-in-notepad-from-net
			Process.Start("notepad.exe ", this.Exceptions_duringApplicationShutdown_serializer.JsonAbsFile);
		}
		#endregion

		public List<FileInfo> ReferencedNetAssemblies_forCompilingScripts { get {
			List<FileInfo> ret = new List<FileInfo>();
			DirectoryInfo directoryInfo = new DirectoryInfo(Assembler.InstanceInitialized.AppStartupPath);		//Application.ExecutablePath
			FileInfo[] dllsAllFoundInFolder = directoryInfo.GetFiles("*.dll");
			List<string> referencedNetAssemblyNames = this.AssemblerDataSnapshot.ReferencedNetAssemblyNames_forCompilingScripts_Sq1;
			foreach (FileInfo eachDllFound in dllsAllFoundInFolder) {
				if (referencedNetAssemblyNames.Contains(eachDllFound.Name) == false) continue;
				ret.Add(eachDllFound);
			}
			return ret;
		} }

		public static void SetThreadName(string threadName, string msg_exception = null, bool popup = false) {
		    try {
		        if (string.IsNullOrEmpty(Thread.CurrentThread.Name)) {
		            Thread.CurrentThread.Name = threadName;
		        }
		    } catch (Exception ex) {
				if (string.IsNullOrEmpty(msg_exception)) msg_exception = "FAILED_TO_SET_THREAD_NAME OR_NPE";
		        Assembler.PopupException(msg_exception + threadName, ex, popup);
		    }
		}
	}
}