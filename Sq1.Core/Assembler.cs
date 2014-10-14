using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

using Sq1.Core.Broker;
using Sq1.Core.Charting;
using Sq1.Core.DataFeed;
using Sq1.Core.DataTypes;
using Sq1.Core.Execution;
using Sq1.Core.Repositories;
using Sq1.Core.Serializers;
using Sq1.Core.Static;
using Sq1.Core.StrategyBase;
using Sq1.Core.Streaming;
using Sq1.Core.Support;

namespace Sq1.Core {
	public class Assembler {
		public RepositoryJsonDataSource RepositoryJsonDataSource;
		public RepositoryCustomSymbolInfo RepositoryCustomSymbolInfo;
		public RepositoryCustomMarketInfo MarketInfoRepository;
		public RepositoryDllJsonStrategy RepositoryDllJsonStrategy;

		public RepositoryDllStaticProvider RepositoryDllStaticProvider;
		public RepositoryDllStreamingProvider RepositoryDllStreamingProvider;
		public RepositoryDllBrokerProvider RepositoryDllBrokerProvider;
		public RepositoryDllReporters RepositoryDllReporters;

		public RepositoryFoldersNoJson WorkspacesRepository;
		
		public OrderProcessor OrderProcessor;
		public IStatusReporter StatusReporter;
		
		public DictionaryManyToOne<ChartShadow, Alert> AlertsForChart;
		public AssemblerDataSnapshot AssemblerDataSnapshot;
		public Serializer<AssemblerDataSnapshot> AssemblerDataSnapshotSerializer;		
		
		public const string DateTimeFormatIndicatorHasNoValuesFor = "yyyy-MMM-dd ddd HH:mm";
		public const string DateTimeFormatLong = "HH:mm:ss.fff ddd dd MMM yyyy";
		public const string DateTimeFormatLongFilename = "yyyy-MMM-dd_ddd_HH.mm.ss";
		public static string FormattedLongFilename(DateTime dt) {
			return dt.ToString(Assembler.DateTimeFormatLongFilename);
		}

		public bool MainFormClosingIgnoreReLayoutDockedForms = false;
		public bool MainFormDockFormsFullyDeserializedLayoutComplete = false;

		private static Assembler instance = null;
		public static Assembler InstanceInitialized { get {
				string usage = "; use Assembler.InstanceUninitialized.Initialize(MainForm); this singleton requires IStatusReporter to get fully initialized";
				if (Assembler.instance == null) {
					throw (new Exception("Assembler.instance=null" + usage));
				}
				if (Assembler.instance.StatusReporter == null) {
					throw (new Exception("Assembler.instance.StatusReporter=null" + usage));
				}
				return Assembler.instance;
			} }
		public static bool IsInitialized { get { return Assembler.instance != null && Assembler.instance.StatusReporter != null; } }
		public static Assembler InstanceUninitialized { get {
				if (Assembler.instance == null) {
					Assembler.instance = new Assembler();
				}
				return instance;
			} }
		public string AppStartupPath { get {
				string ret = Application.StartupPath;
				if (ret.EndsWith(Path.DirectorySeparatorChar.ToString()) == false) ret += Path.DirectorySeparatorChar;
				return ret;
			} }
#if DEBUG
		// C:\Sq1\Data-debug
		public readonly string DATA_FOLDER_DEBUG_RELEASE = ".." + Path.DirectorySeparatorChar + ".." + Path.DirectorySeparatorChar + ".." + Path.DirectorySeparatorChar
			+ "Data-debug";
#else
		// C:\Sq1\Sq1.Gui\bin\Debug\Data
		public readonly string DATA_FOLDER_DEBUG_RELEASE = "Data";
#endif
		public string AppDataPath { get {
				string ret = this.AppStartupPath + DATA_FOLDER_DEBUG_RELEASE;
				//if (defined("DEBUG")) ret = Application.UserAppDataPath + "" + Path.DirectorySeparatorChar + "Data";
				if (Directory.Exists(ret) == false) Directory.CreateDirectory(ret);
				return ret;
			} }
//		[Obsolete("looks illogical, move IStatusReporter to Initialize() and use Assembler.InstanceInitialized instead of Assembler.Constructed")]
//		protected Assembler(IStatusReporter mainForm) : this() {
//			this.StatusReporter = mainForm;
//			Assembler.instance = this;
//		}
		public Assembler() {
			this.RepositoryCustomSymbolInfo = new RepositoryCustomSymbolInfo();
			this.MarketInfoRepository = new RepositoryCustomMarketInfo();
			this.RepositoryJsonDataSource = new RepositoryJsonDataSource();
			this.RepositoryDllJsonStrategy = new RepositoryDllJsonStrategy();

			this.RepositoryDllStaticProvider = new RepositoryDllStaticProvider();
			this.RepositoryDllStreamingProvider = new RepositoryDllStreamingProvider();
			this.RepositoryDllBrokerProvider = new RepositoryDllBrokerProvider();
			this.RepositoryDllReporters = new RepositoryDllReporters();
			
			this.WorkspacesRepository = new RepositoryFoldersNoJson();

			//this.ChartRendererConfigured = new ChartRenderer();
			this.OrderProcessor = new OrderProcessor();
			this.AlertsForChart = new DictionaryManyToOne<ChartShadow, Alert>();
			
			this.AssemblerDataSnapshot = new AssemblerDataSnapshot();
			this.AssemblerDataSnapshotSerializer = new Serializer<AssemblerDataSnapshot>();
		}
		public Assembler InitializedWithSame(IStatusReporter mainForm) {
			if (this.StatusReporter == mainForm) return Assembler.InstanceInitialized;
			return this.Initialize(mainForm);
		}
		public Assembler Initialize(IStatusReporter mainForm) {
			if (this.StatusReporter != null && this.StatusReporter != mainForm) {
				string msg = "Assembler.InstanceInitialized.StatusReporter[" + this.StatusReporter + "] != mainForm[" + mainForm + "]";
				msg += "; you initialize the StatusReporter once per lifetime;"
					+ " if you need to re-initialize singleton with new IStatusReporter then refactor it"
					+ " and introduce Reset(IStatusReporter) method which won't throw this;"
					+ " or you can just set Assembler.InstanceInitialized.StatusReporter={your different IStatusReporter}"
					+ " to use Assembler.PopupException(), but no guarantee that OrderProcessor and DataSourceRepository will work properly";
				throw new Exception();
			}
			this.StatusReporter = mainForm;
			
			bool createdNewFile = this.RepositoryCustomSymbolInfo.Initialize(this.AppDataPath, "SymbolInfo.json", "", null);
			List<SymbolInfo> symbolInfosNotUsed = this.RepositoryCustomSymbolInfo.Deserialize();
			
			createdNewFile = this.MarketInfoRepository.Initialize(this.AppDataPath, "MarketInfo.json", "", null);
			this.MarketInfoRepository.Deserialize();
			
			this.RepositoryDllJsonStrategy.Initialize(this.AppDataPath, this.AppStartupPath);

			this.RepositoryDllStaticProvider.InitializeAndScan(this.AppStartupPath);
			this.RepositoryDllStreamingProvider.InitializeAndScan(this.AppStartupPath);
			this.RepositoryDllBrokerProvider.InitializeAndScan(this.AppStartupPath);
			this.RepositoryDllReporters.InitializeAndScan(this.AppStartupPath);
			
			this.WorkspacesRepository.Initialize(this.AppDataPath, "Workspaces", this.StatusReporter);
			this.WorkspacesRepository.ScanFolders();

			this.OrderProcessor.Initialize(this.AppDataPath, this.StatusReporter);

			//v1 this.RepositoryJsonDataSource.Initialize(this.AppDataPath);
			//v1 this.RepositoryJsonDataSource.DataSourcesDeserialize(this.MarketInfoRepository, this.OrderProcessor, this.StatusReporter);
			
			this.RepositoryJsonDataSource.Initialize(this.AppDataPath, "DataSources",
				this.StatusReporter, this.MarketInfoRepository, this.OrderProcessor);
			this.RepositoryJsonDataSource.DeserializeJsonsInFolder();

			createdNewFile = this.AssemblerDataSnapshotSerializer.Initialize(this.AppDataPath, "AssemblerDataSnapshot.json", "", null);
			this.AssemblerDataSnapshot = this.AssemblerDataSnapshotSerializer.Deserialize();
			
			return Assembler.InstanceInitialized;
		}
		public static void PopupException(string msg, Exception ex = null, bool debuggingBreak = true) {
			#if DEBUG
			if (debuggingBreak) {
				Debugger.Break();
			}
			#endif

			if (msg != null) ex = new Exception(msg, ex);
			if (Assembler.InstanceInitialized == null) {
				MessageBox.Show(ex.Message, "Assembler.InstanceInitialized=null");
				//throw ex;
			}
			if (Assembler.InstanceInitialized.StatusReporter == null) {
				string msg2 = "Assembler.InstanceInitialized.StatusReporter=null";
				//MessageBox.Show(ex.Message, msg);
				throw new Exception(msg2, ex);
			}
			Form exceptionsForm = Assembler.InstanceInitialized.StatusReporter as Form;
			if (exceptionsForm == null) {
				string msg2 = "Assembler.InstanceInitialized.StatusReporter is not a Form";
				//MessageBox.Show(ex.Message, msg);
				throw new Exception(msg2, ex);
			}
			if (exceptionsForm.Visible == false) {
//				//v1 throw (exc);
//				//string msg2 = "Assembler.InstanceInitialized.StatusReporter is a Form but Visible=false";
//				//v2 MessageBox.Show(ex.Message, msg);
//				//v3 throw new Exception(msg2, ex);
				string msg2 = "ExceptionForm.Visible=false"
					+ "; but .PopupException() will insert your exception into the tree and display OnLoad()";
			}
			Assembler.InstanceInitialized.StatusReporter.PopupException(ex);
		}
	}
}