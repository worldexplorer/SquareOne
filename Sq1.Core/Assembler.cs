using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
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
		public	RepositoryJsonDataSource				RepositoryJsonDataSource;
		public	RepositorySerializerSymbolInfo			RepositorySymbolInfo;
		public	RepositorySerializerMarketInfo			RepositoryMarketInfo;
		public	RepositoryDllStreamingProvider			RepositoryDllStreamingProvider;
		public	RepositoryDllBrokerProvider				RepositoryDllBrokerProvider;
		public	RepositoryDllReporters					RepositoryDllReporters;
		public	RepositoryDllJsonStrategy				RepositoryDllJsonStrategy;

		public	RepositoryFoldersNoJson					WorkspacesRepository;
		
		public	OrderProcessor							OrderProcessor;
		public	IStatusReporter							StatusReporter;
		//public	DockContentImproved						ExecutionForm;
		
		public	DictionaryManyToOne<ChartShadow, Alert>	AlertsForChart;
		public	AssemblerDataSnapshot					AssemblerDataSnapshot;
		public	Serializer<AssemblerDataSnapshot>		AssemblerDataSnapshotSerializer;		
		
		public	const string							DateTimeFormatIndicatorHasNoValuesFor = "yyyy-MMM-dd ddd HH:mm";
		public	const string							DateTimeFormatLong = "HH:mm:ss.fff ddd dd MMM yyyy";
		public	const string							DateTimeFormatLongFilename = "yyyy-MMM-dd_ddd_HH.mm.ss";
		
		public	static string FormattedLongFilename(DateTime dt) {
			return dt.ToString(Assembler.DateTimeFormatLongFilename);
		}

		public	bool									MainFormClosingIgnoreReLayoutDockedForms = false;
		public	bool									MainFormDockFormsFullyDeserializedLayoutComplete = false;

				static Assembler						instance = null;
		public	static Assembler						InstanceInitialized { get {
				string usage = "; use Assembler.InstanceUninitialized.Initialize(MainForm); this singleton requires IStatusReporter to get fully initialized";
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
				ret.AddRange(this.RepositoryDllStreamingProvider.ExceptionsWhileScanning);
				ret.AddRange(this.RepositoryDllBrokerProvider.ExceptionsWhileScanning);
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

		public	bool									SplitterEventsAreAllowedAssumingInitialInnerDockResizingFinished { get {
				bool ret = false;
				int sinceApplicationStartSeconds = -1;
				try {
					TimeSpan sinceApplicationStart = DateTime.Now - Process.GetCurrentProcess().StartTime;
					sinceApplicationStartSeconds = sinceApplicationStart.Seconds;
				} catch (Exception ex) {
					Assembler.PopupException("SEEMS_TO_BE_UNSUPPORTED_Process.GetCurrentProcess()", ex);
					sinceApplicationStartSeconds = this.stopWatchIfProcessUnsupported.Elapsed.Seconds;
				}
				int secondsLeftToIgnore = this.AssemblerDataSnapshot.SplitterEventsShouldBeIgnoredSecondsAfterAppLaunch - sinceApplicationStartSeconds;
				if (secondsLeftToIgnore < 0) {
					ret = true;
				} else {
					Assembler.PopupException("SPLITTER_EVENTS_IGNORED_FOR_MORE_SECONDS " + secondsLeftToIgnore, null, false);
				}
				return ret;
			} }

				Stopwatch								stopWatchIfProcessUnsupported;
		
		public Assembler() {
			RepositorySymbolInfo			= new RepositorySerializerSymbolInfo();
			RepositoryMarketInfo			= new RepositorySerializerMarketInfo();
			RepositoryJsonDataSource		= new RepositoryJsonDataSource();
			RepositoryDllJsonStrategy		= new RepositoryDllJsonStrategy();

			RepositoryDllStreamingProvider	= new RepositoryDllStreamingProvider();
			RepositoryDllBrokerProvider		= new RepositoryDllBrokerProvider();
			RepositoryDllReporters			= new RepositoryDllReporters();
			
			WorkspacesRepository			= new RepositoryFoldersNoJson();

			OrderProcessor					= new OrderProcessor();
			AlertsForChart					= new DictionaryManyToOne<ChartShadow, Alert>();
			
			AssemblerDataSnapshot			= new AssemblerDataSnapshot();
			AssemblerDataSnapshotSerializer	= new Serializer<AssemblerDataSnapshot>();
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

			this.RepositoryDllStreamingProvider	.InitializeAndScan(this.AppStartupPath);
			this.RepositoryDllBrokerProvider	.InitializeAndScan(this.AppStartupPath);
			this.RepositoryDllReporters			.InitializeAndScan(this.AppStartupPath);
			
			this.WorkspacesRepository			.Initialize(this.AppDataPath, "Workspaces", this.StatusReporter);
			this.WorkspacesRepository			.ScanFolders();

			this.OrderProcessor					.Initialize(this.AppDataPath);

			//v1 this.RepositoryJsonDataSource	.Initialize(this.AppDataPath);
			//v1 this.RepositoryJsonDataSource	.DataSourcesDeserialize(this.MarketInfoRepository, this.OrderProcessor, this.StatusReporter);
			
			this.RepositoryJsonDataSource		.Initialize(this.AppDataPath, "DataSources",
				this.StatusReporter, this.RepositoryMarketInfo, this.OrderProcessor);
			this.RepositoryJsonDataSource		.DeserializeJsonsInFolder();

			createdNewFile = this.AssemblerDataSnapshotSerializer.Initialize(this.AppDataPath, "AssemblerDataSnapshot.json", "", null);
			this.AssemblerDataSnapshot = this.AssemblerDataSnapshotSerializer.Deserialize();
			
			try {
				TimeSpan sinceApplicationStart = DateTime.Now - Process.GetCurrentProcess().StartTime;
				string msg = "ASSEMBLER_INITIALIZED_AFTER_MILLIS_RUNNING: "
					+ Math.Round(sinceApplicationStart.Milliseconds / (decimal)1000, 2)
					+ " [" + this.AssemblerDataSnapshot.SplitterEventsShouldBeIgnoredSecondsAfterAppLaunch
					+ "]=SplitterEventsShouldBeIgnoredSecondsAfterAppLaunch";
				Assembler.PopupException(msg, null, false);
			} catch (Exception ex) {
				Assembler.PopupException("PLATFORM_DOESNT_SUPPORT_Process.GetCurrentProcess().StartTime_AS_DESCRIBED_IN_MSDN__LAUNCHING_STOPWATCH", ex);
				this.stopWatchIfProcessUnsupported = Stopwatch.StartNew();
			}

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
				// if I PopupException from a BrokerProvider thread, exceptionsForm.Visible and others should throw
				Form exceptionsForm = Assembler.InstanceInitialized.StatusReporter as Form;
			} catch (Exception ex1) {
				Debugger.Break();
			}

			Assembler.InstanceInitialized.StatusReporter.PopupException(msg, ex, debuggingBreak);
		}
		public static void DisplayStatus(string msg) {
			Assembler.InstanceInitialized.checkThrowIfNotInitializedStaticHelper();
			Assembler.InstanceInitialized.StatusReporter.DisplayStatus(msg);
		}

		//internal static void PopupExecutionForm() {
		//    if (Assembler.InstanceInitialized.ExecutionForm == null) {
		//        string msg = "I_CAN_NOT_CONTINUE_WITHOUT_EXECUTION_FORM__ORDER_PROCESSOR_HAS_SOME_ORDERS_TO_DISPLAY";
		//        Assembler.PopupException(msg);
		//        return;
		//    }
		//    Assembler.InstanceInitialized.ExecutionForm.ShowPopupSwitchToGuiThread();
		//}
	}
}