using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

using Sq1.Core.Broker;
using Sq1.Core.Charting;
using Sq1.Core.DataTypes;
using Sq1.Core.Execution;
using Sq1.Core.Repositories;
using Sq1.Core.Serializers;
using Sq1.Core.Support;

namespace Sq1.Core {
	public class Assembler {
		public	RepositorySerializerSymbolInfo			RepositorySymbolInfo;
		public	RepositorySerializerMarketInfo			RepositoryMarketInfo;
		public	RepositoryDllJsonStrategy				RepositoryDllJsonStrategy;
		public	RepositoryJsonDataSource				RepositoryJsonDataSource;

		public	RepositoryDllStreamingAdapter			RepositoryDllStreamingAdapter;
		public	RepositoryDllBrokerAdapter				RepositoryDllBrokerAdapter;
		public	RepositoryDllReporters					RepositoryDllReporters;

		public	RepositoryFoldersNoJson					WorkspacesRepository;
		
		public	OrderProcessor							OrderProcessor;
		public	IStatusReporter							StatusReporter;
		//public	DockContentImproved						ExecutionForm;
		
		public	DictionaryManyToOne<ChartShadow, Alert>	AlertsForChart;
		public	AssemblerDataSnapshot					AssemblerDataSnapshot;
		public	Serializer<AssemblerDataSnapshot>		AssemblerDataSnapshotSerializer;

		public	const string							DateTimeFormatIndicatorHasNoValuesFor 	= "yyyy-MMM-dd ddd HH:mm";
		public	const string							DateTimeFormatToMinutes				 	= "yyyy-MMM-dd HH:mm";
		public	const string							DateTimeFormatLong						= "HH:mm:ss.fff ddd dd MMM yyyy";
		public	const string							DateTimeFormatLongFilename				= "yyyy-MMM-dd_ddd_HH.mm.ss";
		
		public	static string FormattedLongFilename(DateTime dt) {
			return dt.ToString(Assembler.DateTimeFormatLongFilename);
		}

		public	bool									MainFormClosingIgnoreReLayoutDockedForms = false;
		public	bool									MainFormDockFormsFullyDeserializedLayoutComplete = false;

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
				ret.AddRange(this.RepositoryDllStreamingAdapter.ExceptionsWhileScanning);
				ret.AddRange(this.RepositoryDllBrokerAdapter.ExceptionsWhileScanning);
				ret.AddRange(this.RepositoryDllReporters.ExceptionsWhileScanning);
				ret.AddRange(this.RepositoryDllJsonStrategy.ExceptionsWhileInstantiating);
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

		public	bool									SplitterEventsAreAllowedNsecAfterLaunchHopingInitialInnerDockResizingIsFinished { get {
				bool ret = false;
				//v1 looks like Process.GetCurrentProcess().StartTime counts the milliseconds elapsed by the processor for the app
				//(1sec during last 2mins with 1% CPU load), while I need solarTimeNow minus solarTimeAppStarted => switching to Stopwatch
				double sinceApplicationStartSeconds = -1;
				try {
					TimeSpan sinceApplicationStart = DateTime.Now - Process.GetCurrentProcess().StartTime;
					sinceApplicationStartSeconds = sinceApplicationStart.TotalSeconds;
					//v2
					//int sinceApplicationStartSeconds2 = this.stopWatchIfProcessUnsupported.Elapsed.Seconds;
				} catch (Exception ex) {
					Assembler.PopupException("SEEMS_TO_BE_UNSUPPORTED_Process.GetCurrentProcess()", ex);
					sinceApplicationStartSeconds = this.stopWatchIfProcessUnsupported.Elapsed.Seconds;
				}

				double secondsLeftToIgnore = this.AssemblerDataSnapshot.SplitterEventsShouldBeIgnoredSecondsAfterAppLaunch - sinceApplicationStartSeconds;
				if (secondsLeftToIgnore > 0) {
					string msg = "SPLITTER_EVENTS_IGNORED_FOR_MORE_SECONDS " + secondsLeftToIgnore + "/"
						+ this.AssemblerDataSnapshot.SplitterEventsShouldBeIgnoredSecondsAfterAppLaunch;
					//Assembler.PopupException(msg, null, false);
				} else {
					ret = true;
				}
				return ret;
			} }

				Stopwatch								stopWatchIfProcessUnsupported;
		
		public Assembler() {
			RepositorySymbolInfo					= new RepositorySerializerSymbolInfo();
			RepositoryMarketInfo					= new RepositorySerializerMarketInfo();
			RepositoryDllJsonStrategy				= new RepositoryDllJsonStrategy();
			RepositoryJsonDataSource				= new RepositoryJsonDataSource();

			RepositoryDllStreamingAdapter			= new RepositoryDllStreamingAdapter();
			RepositoryDllBrokerAdapter				= new RepositoryDllBrokerAdapter();
			RepositoryDllReporters					= new RepositoryDllReporters();
			
			WorkspacesRepository					= new RepositoryFoldersNoJson();

			OrderProcessor							= new OrderProcessor();
			AlertsForChart							= new DictionaryManyToOne<ChartShadow, Alert>();
			
			AssemblerDataSnapshot					= new AssemblerDataSnapshot();
			AssemblerDataSnapshotSerializer			= new Serializer<AssemblerDataSnapshot>();

		}
		public Assembler Initialize(IStatusReporter mainForm) {
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
			
			bool createdNewFile = this.RepositorySymbolInfo.Initialize(this.AppDataPath, "SymbolInfo.json", "", null);
			List<SymbolInfo> symbolInfosNotUsed = this.RepositorySymbolInfo.Deserialize();
			
			createdNewFile = this.RepositoryMarketInfo.Initialize(this.AppDataPath, "MarketInfo.json", "", null);
			this.RepositoryMarketInfo			.Deserialize();
			
			this.RepositoryDllJsonStrategy		.Initialize(this.AppDataPath, this.AppStartupPath);

			this.RepositoryDllStreamingAdapter	.InitializeAndScan(this.AppStartupPath);
			this.RepositoryDllBrokerAdapter		.InitializeAndScan(this.AppStartupPath);
			this.RepositoryDllReporters			.InitializeAndScan(this.AppStartupPath);
			
			this.WorkspacesRepository			.Initialize(this.AppDataPath, "Workspaces", this.StatusReporter);
			this.WorkspacesRepository			.ScanFolders();

			this.OrderProcessor					.Initialize(this.AppDataPath);

			//v1 this.RepositoryJsonDataSource	.Initialize(this.AppDataPath);
			//v1 this.RepositoryJsonDataSource	.DataSourcesDeserialize(this.MarketInfoRepository, this.OrderProcessor, this.StatusReporter);
			
			this.RepositoryJsonDataSource		.Initialize(this.AppDataPath, "DataSources", this.RepositoryMarketInfo, this.OrderProcessor);
			this.RepositoryJsonDataSource		.DeserializeJsonsInFolder();
			
			createdNewFile = this.AssemblerDataSnapshotSerializer.Initialize(this.AppDataPath, "AssemblerDataSnapshot.json", "", null);
			this.AssemblerDataSnapshot = this.AssemblerDataSnapshotSerializer.Deserialize();
			
			//v1
			try {
				TimeSpan sinceApplicationStart = DateTime.Now - Process.GetCurrentProcess().StartTime;
				string msg = "ASSEMBLER_INITIALIZED_AFTER_MILLIS_RUNNING: "
					+ Math.Round(sinceApplicationStart.Milliseconds / (decimal)1000, 2)
					+ " [" + this.AssemblerDataSnapshot.SplitterEventsShouldBeIgnoredSecondsAfterAppLaunch
					+ "]=SplitterEventsShouldBeIgnoredSecondsAfterAppLaunch";
				//Assembler.PopupException(msg, null, false);
			} catch (Exception ex) {
				Assembler.PopupException("PLATFORM_DOESNT_SUPPORT_Process.GetCurrentProcess().StartTime_AS_DESCRIBED_IN_MSDN__LAUNCHING_STOPWATCH", ex);
				this.stopWatchIfProcessUnsupported = Stopwatch.StartNew();
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
			if (Assembler.IsInitialized == false) return;
			Assembler.InstanceInitialized.checkThrowIfNotInitializedStaticHelper();
			
			//v1-SHARP_DEVELOP_THROWS_WHEN_TRYING_TO_POPUP_EXCEPTION_FROM_QUIK_TERMINAL_MOCK_THREAD 
//			#if DEBUG
//			if (debuggingBreak) {
//				Debugger.Break();
//				// SHARP_DEVELOP_THROWS_WHEN_TRYING_TO_POPUP_EXCEPTION_FROM_QUIK_TERMINAL_MOCK_THREAD
//				// break here and add to ExceptionControl.ExceptionsList later; if you let it go then
//				// MainForm might switch to GUI thread and you'll loose your callstack in VS/SharpDevelop
//				debuggingBreak = false;
//				//return;		// FIXED_IN_MAIN_FORM tmp hack for SHARP_DEVELOP_THROWS_WHEN_TRYING_TO_POPUP_EXCEPTION_FROM_QUIK_TERMINAL_MOCK_THREAD
//			}
//			#endif

			try {
				// this is gonna throw from a non-GUI thread, right?!... (moved to MainForm.PopupException() with base.BeginInvoke() as first step)
				// if I PopupException from a BrokerAdapter thread, exceptionsForm.Visible and others should throw
				Form exceptionsForm = Assembler.InstanceInitialized.StatusReporter as Form;
				Assembler.InstanceInitialized.StatusReporter.PopupException(msg, ex, debuggingBreak);
			} catch (Exception ex1) {
				#if DEBUG
				string msg1 = "NOWHERE_ELSE_I_COULD_DUMP_EXCEPTION_BRO";
				Debugger.Break();
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
		public static void ExceptionsDuringApplicationShutdown_InsertAndSerialize(Exception exc, int indexToInsert = 0) {
			Assembler.InstanceInitialized.checkThrowIfNotInitializedStaticHelper();
			Assembler.InstanceInitialized.exceptionsDuringApplicationShutdown_InsertAndSerialize(exc, indexToInsert);
		}
		
		public	List<Exception>							ExceptionsDuringApplicationShutdown;
		public	Serializer<List<Exception>>				ExceptionsDuringApplicationShutdownSerializer;
		public void exceptionsDuringApplicationShutdown_InsertAndSerialize(Exception exc, int indexToInsert = 0) {
			if (exc == null) return;
			if (this.ExceptionsDuringApplicationShutdown == null) {
				//v1 TRYING_TO_FIX_BY_MOVING_TO_ASSEMBLER produced useless "[ null ]" file
				this.ExceptionsDuringApplicationShutdown = new List<Exception>();
				this.ExceptionsDuringApplicationShutdownSerializer = new Serializer<List<Exception>>();
				string now = Assembler.FormattedLongFilename(DateTime.Now);
				bool createdNewFile = this.ExceptionsDuringApplicationShutdownSerializer.Initialize(this.AppDataPath,
					"ExceptionsDuringApplicationShutdown-" + now + ".json", "Exceptions", null, true, true);
				this.ExceptionsDuringApplicationShutdown = this.ExceptionsDuringApplicationShutdownSerializer.Deserialize(); 
				//v2
				//return;
			}
			this.ExceptionsDuringApplicationShutdown.Insert(0, exc);
			this.ExceptionsDuringApplicationShutdownSerializer.Serialize();

			//DIDNT_CHECK http://stackoverflow.com/questions/7613576/how-to-open-text-in-notepad-from-net
			Process.Start("notepad.exe ", this.ExceptionsDuringApplicationShutdownSerializer.AbsPath);
		}
		#endregion
	}
}