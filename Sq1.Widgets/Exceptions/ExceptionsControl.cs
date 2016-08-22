using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Core.Serializers;
using Sq1.Core.Support;

namespace Sq1.Widgets.Exceptions {

#if USE_CONTROL_IMPROVED
	public partial class ExceptionsControl : UserControlPeriodicFlush {
#else
	public partial class ExceptionsControl : UserControl {
#endif

				ExceptionsControlDataSnapshot				dataSnapshot;
				Serializer<ExceptionsControlDataSnapshot>	dataSnapshotSerializer;
				ConcurrentDictionary<Exception, DateTime>	exceptionTimes;
		public	ExceptionListLogrotated						Exceptions				{ get; private set; }
				ExceptionList								exceptions_notFlushedYet;

				DateTime			exceptionLastDate_notFlushedYet;

				Exception			exceptionSingleSelectedInTree_nullUnsafe		{ get { return this.olvTreeExceptions.SelectedObject as Exception; } }
				DockContentImproved parentAsDockContentImproved_nullUnsafe			{ get { return base.Parent as DockContentImproved; } }

		public ExceptionsControl() : base() {
			exceptionTimes				= new ConcurrentDictionary<Exception, DateTime>("exceptionTimes_delayedGuiFlush");
			Exceptions					= new ExceptionListLogrotated("Exceptions_delayedGuiFlush");
			exceptions_notFlushedYet	= new ExceptionList("exceptions_notFlushedYet");
			exceptionLastDate_notFlushedYet	= DateTime.MinValue;

			this.InitializeComponent();
			//WindowsFormsUtils.SetDoubleBuffered(this.tree);	//doesn't help, still flickers
			//WindowsFormsUtils.SetDoubleBuffered(this.olvStackTrace);
			//WindowsFormsUtils.SetDoubleBuffered(this);
			//this.olvTreeExceptions.EmptyListMsg = "No Exceptions occured so far";
			
			this.olvTreeExceptions_customize();
			//this.olvTreeExceptions.SetObjects(this.Exceptions);		// obviously empty => just cleaning "No Exceptions occured so far" olvTreeExceptions.EmptyListMsg
		}

		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
				// hopefully disposing=true will be invoked once/lifetime - I don't care when; I'm lazy to work around NPEs and alreadyDisposed
				this.exceptionTimes.Dispose();
				this.Exceptions.Dispose();
				base.Timed_flushingToGui.Dispose();
			}
			base.Dispose(disposing);
		}

		public void Initialize() {
			//WARNING! LiveSimControl uses non-ExceptionsForm-singleton MULTIPLE ExceptionsControls for LivesimStreamingEditor and LivesimBrokerEditor
			//LivesimControl throws here in Designer (unbelieabable though)
			if (Assembler.IsInitialized == false) return;		// useful for MultiSplitTest, PanelsTest
				
			this.dataSnapshotSerializer = new Serializer<ExceptionsControlDataSnapshot>();
			bool createdNewFile = this.dataSnapshotSerializer.Initialize(Assembler.InstanceInitialized.AppDataPath,
				"Sq1.Widgets.ExceptionsControlDataSnapshot.json", "Workspaces",
				Assembler.InstanceInitialized.AssemblerDataSnapshot.WorkspaceCurrentlyLoaded);
			this.dataSnapshot = this.dataSnapshotSerializer.Deserialize();
			if (createdNewFile) {
				this.dataSnapshot.SplitDistanceVertical = this.splitContainerVertical.SplitterDistance;
				this.dataSnapshot.SplitDistanceHorizontal = this.splitContainerHorizontal.SplitterDistance;
				this.dataSnapshotSerializer.Serialize();
			}
			this.Exceptions.Logrotator.LogRotateSizeLimit_Mb = this.dataSnapshot.LogRotateSizeLimit_Mb;

			if (Thread.CurrentThread.ManagedThreadId != 1) {
				string msg = "YOU_MUST_CREATE_ME_FROM_GUI_THREAD__HAVING_MAIN_FORM_HANDLE__SEE_SIMILAR_CHECK_IN_MainForm_and_Assembler.Exceptions_duringStartup";
				#if DEBUG
				Debugger.Break();
				#endif
				return;
			}
			base.Initialize_periodicFlushing("FLUSH_EXCEPTIONS_CONTROL",
				new Action(this.flushExceptionsToOLV_switchToGuiThread), this.dataSnapshot.FlushToGuiDelayMsec);
			//base.Timed_flushingToGui.Start();

			this.InitializeSearch();

			this.mniShowSearchbar.Checked = this.dataSnapshot.ShowSearchbar;

			this.mniShowTimestamps.Checked = this.dataSnapshot.ShowTimestamps;
			this.mniRecentAlwaysSelected.Checked = this.dataSnapshot.RecentAlwaysSelected;

			this.mniltbFlushToGuiDelayMsec.InputFieldValue = this.dataSnapshot.FlushToGuiDelayMsec.ToString();		// I will be stuck here - if ExceptionsForm.Instance was created from non-GUI thread, by Assembler.PopupException() while MainForm haven't got Window Handle yet
			this.mniltbLogrotateLargerThan.InputFieldValue = this.dataSnapshot.LogRotateSizeLimit_Mb.ToString();	// I will be stuck here - if ExceptionsForm.Instance was created from non-GUI thread, by Assembler.PopupException() while MainForm haven't got Window Handle yet

			this.olvcTimesOccured.IsVisible = this.dataSnapshot.ShowTimesOccured;
			this.olvcTimestamp.IsVisible = this.dataSnapshot.ShowTimestamps;

			this.Exceptions.Initialize();
		}


		public void PopulateDataSnapshot_initializeSplitters_afterDockContentDeserialized() {
			string msig = " //PopulateDataSnapshot_initializeSplitters_afterDockContentDeserialized()";
			if (Assembler.InstanceInitialized.MainForm_dockFormsFullyDeserialized_layoutComplete == false) {
				string msg = "CHOOSE_ANOTHER_INVOCATION_SPOT__AFTER_MAIN_FORM_FULLY_DESERIALIZED";
				Assembler.PopupException(msg + msig, null, false);
				return;
			}

			if (base.InvokeRequired) {
				base.BeginInvoke(new MethodInvoker(this.PopulateDataSnapshot_initializeSplitters_afterDockContentDeserialized));
				return;
			}

			if (base.Width == 0) {
				string msg = "CANT_SET_SPLITTER_DISTANCE_FOR_UNSHOWN_CONTROL ExceptionsControl.Visible[" + this.Visible + "]; can't set SplitDistanceVertical, SplitDistanceHorizontal";
				Assembler.PopupException(msg + msig, null, false);
			} else {
				try {
					this.SuspendLayout();
					if (this.dataSnapshot.SplitDistanceVertical > 0) {
						string msg = "+51_SEEMS_TO_BE_REPRODUCED_AT_THE_SAME_DISTANCE_I_LEFT_VERTICAL";
						int newVerticalDistance = this.dataSnapshot.SplitDistanceVertical;	//  + 51 + this.splitContainerVertical.SplitterWidth;
						if (this.splitContainerVertical.SplitterDistance != newVerticalDistance) {
							this.splitContainerVertical.SplitterDistance  = newVerticalDistance;
						}
					}
					if (this.dataSnapshot.SplitDistanceHorizontal > 0) {
						string msg = "+67_SEEMS_TO_BE_REPRODUCED_AT_THE_SAME_DISTANCE_I_LEFT_HORIZONTAL";
						int newHorizontalDistance = this.dataSnapshot.SplitDistanceHorizontal;	// + 97 + this.splitContainerHorizontal.SplitterWidth;
						if (this.splitContainerHorizontal.SplitterDistance != newHorizontalDistance) {
							this.splitContainerHorizontal.SplitterDistance  = newHorizontalDistance;
						}
					}
				} catch (Exception ex) {
					string msg = "TRYING_TO_LOCALIZE_SPLITTER_MUST_BE_BETWEEN_0_AND_PANEL_MIN";
					Assembler.PopupException(msg + msig, null, false);
				} finally {
					this.ResumeLayout(true);
				}
			}
			//late binding prevents SplitterMoved() induced by DockContent layouting LoadAsXml()ed docked forms 
			this.splitContainerVertical.SplitterMoved -= new System.Windows.Forms.SplitterEventHandler(this.splitContainerVertical_SplitterMoved);
			this.splitContainerVertical.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitContainerVertical_SplitterMoved);
			this.splitContainerHorizontal.SplitterMoved -= new System.Windows.Forms.SplitterEventHandler(this.splitContainerHorizontal_SplitterMoved);
			this.splitContainerHorizontal.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitContainerHorizontal_SplitterMoved);

			//this.flushExceptionsToOLV_switchToGuiThread();
		}
		void insertTo_exceptionsNotFlushedYet_willReportIfBlocking(Exception exception) {
			if (exception == null) {
				string msg = "DONT_INVOKE_ME_WITH_NULL";
				//v1 IM_AFRAID_OF_RECURSION_HERE Assembler.PopupException(msg);
				//v2
				#if DEBUG
				Debugger.Break();
				#endif
				return;
			}

			bool avoidingConcurrentAccess = base.Timed_flushingToGui != null && base.Timed_flushingToGui.Scheduled;
			//if (avoidingConcurrentAccess) {
			bool freeToAdd = this.exceptions_notFlushedYet.IsUnlocked;
			if (freeToAdd == false) {
				return;
			}
			string lockPurpose = "insertTo_exceptionsNotFlushedYet_willReportIfBlocking";
			bool iLockedIt = false;
			try {
			// GUI freezes after another thread was paused for too long in debugger
				iLockedIt = this.exceptions_notFlushedYet.WaitAndLockFor(exception, lockPurpose, 500);
				//this.exceptions_notFlushedYet.UnLockFor(exception, lockPurpose);
				//iLockedIt = this.exceptions_notFlushedYet.WaitAndLockFor(exception, lockPurpose, 500);
				if (iLockedIt == false) {
					#if DEBUG
					Debugger.Break();
					#endif
					return;
				}
				this.exceptions_notFlushedYet.AppendUnique(exception, this, lockPurpose);
				this.exceptionTimes.Add(exception, DateTime.Now, this, "InsertExceptionBlocking");
			} catch (Exception ex) {
				throw ex;
			} finally {
				if (iLockedIt) this.exceptions_notFlushedYet.UnLockFor(exception, lockPurpose);
			}
			return;
		}
		void selectMostRecentException() {
			if (this.olvTreeExceptions.GetItemCount() == 0) return;
			this.olvTreeExceptions.SelectedIndex = 0;		// YES_CONFIRMED will invoke tree_SelectedIndexChanged() with displayStackTrace_forSingleSelected()
			//WHAT_FOR? this.treeExceptions.RefreshSelectedObjects();
			this.olvTreeExceptions.EnsureVisible(0);
		}
		void displayStackTrace_forSingleSelected() {
			try {
				this.olvStackTrace.BeginUpdate();
				this.olvStackTrace.Items.Clear();
				StackTrace stackTrace = new StackTrace(this.exceptionSingleSelectedInTree_nullUnsafe);
				for (int i = 0; i < stackTrace.FrameCount - 1; i++) {
					StackFrame stackFrame = stackTrace.GetFrame(i);
					if (stackFrame == null) continue;
					string declaringType = stackFrame.GetMethod().DeclaringType.Name;
					string methodName = stackFrame.GetMethod().Name;	//.ToString();
					//if (methodName == null) continue;
					string fileName = stackFrame.GetFileName();
					string lineNumber = stackFrame.GetFileLineNumber().ToString();
					ListViewItem item = new ListViewItem();
					item.Text = methodName;
					item.SubItems.Add(declaringType);
					item.SubItems.Add(lineNumber);
					item.SubItems.Add(fileName);
					this.olvStackTrace.Items.Add(item);
				}
			} finally {
				this.olvStackTrace.EndUpdate();
			}
		}

		public void InsertAsync_autoFlush(Exception ex) {
			if (base.InvokeRequired) {	// waiting for WindowHandle to be created
				//base.BeginInvoke((MethodInvoker)delegate() { this.populateWindowsTitle(); });
				//base.BeginInvoke(new MethodInvoker(this.populateWindowsTitle));
				base.BeginInvoke((MethodInvoker)delegate() { this.InsertAsync_autoFlush(ex); });
				return;
			}

			int flushedStartup = this.flushStartupExceptions_toBuffered();

			this.insertTo_exceptionsNotFlushedYet_willReportIfBlocking(ex);
			this.exceptionLastDate_notFlushedYet = DateTime.Now;

			this.populateWindowTitle();
			//if (base.Timed_flushingToGui.Scheduled) return;

			// http://stackoverflow.com/questions/2475435/c-sharp-timer-wont-tick
			// Always stop/start a System.Windows.Forms.Timer on the UI thread, apparently! –  user145426
			//v1 this.flushExceptionsToOLV_switchToGuiThread();
			//v2
			base.Timed_flushingToGui.ScheduleOnce_dontPostponeIfAlreadyScheduled();
			//base.Timed_flushingToGui.ScheduleOnce();
		}

		int flushStartupExceptions_toBuffered() {
			int ret = 0;
			if (Assembler.InstanceInitialized.ExceptionsWhileInstantiating.Count > 0) {
				foreach (Exception beforeFormInstantiated in Assembler.InstanceInitialized.ExceptionsWhileInstantiating) {
					this.insertTo_exceptionsNotFlushedYet_willReportIfBlocking(beforeFormInstantiated);
					ret++;
				}
				Assembler.InstanceInitialized.ExceptionsWhileInstantiating.Clear();
			}

			if (Assembler.InstanceInitialized.Exceptions_duringApplicationStartup_beforeMainForm_gotWindowHandle.Count > 0) {
				foreach (Exception excStartup in Assembler.InstanceInitialized.Exceptions_duringApplicationStartup_beforeMainForm_gotWindowHandle) {
					this.insertTo_exceptionsNotFlushedYet_willReportIfBlocking(excStartup);
					ret++;
				}
				Assembler.InstanceInitialized.Exceptions_duringApplicationStartup_beforeMainForm_gotWindowHandle.Clear();
			}
			return ret;
		}

		void flushExceptionsToOLV_switchToGuiThread() {
			string msig = " //flushExceptionsToOLV_switchToGuiThread()";
			if (base.DesignMode) return;
			if (Assembler.IsInitialized == false) return; //I_HATE_LIVESIM_FORM_THROWING_EXCEPTIONS_IN_DESIGN_MODE
			// WINDOWS.FORMS.VISIBLE=FALSE_IS_SET_BY_DOCK_CONTENT_LUO ANALYZE_DockContentImproved.IsShown_INSTEAD if (this.Visible == false) return;
			if (Assembler.InstanceInitialized.MainForm_dockFormsFullyDeserialized_layoutComplete == false) return;

			if (this.InvokeRequired) {
				this.BeginInvoke(new MethodInvoker(this.flushExceptionsToOLV_switchToGuiThread));
				//string msg = "I_MUST_BE_ALREADY_IN_GUI_THREAD__HOPING_TO_INSERT_IN_SEQUENCE_OF_INVOCATION";
				//Debugger.Break();
				return;
			}

			//if (this.pnlSearch.Visible && string.IsNullOrWhiteSpace(this.txtSearch.Text) == false) return;
			bool searchApplied = this.tsiCbx_SearchApply.CheckBoxChecked && string.IsNullOrWhiteSpace(this.tsiLtb_SearchKeywords.InputFieldValue) == false;
			if (this.statusStrip_search.Visible && searchApplied) {
				string msg3 = "REMEMBER?__WHEN_IM_SEARCHING_I_DONT_FLUSH_CONTROL_WHEN_SEARCHING";
				return;
			}

			base.HowLongTreeRebuilds.Restart();

			string msg = "if we are in GUI thread, go on timer immediately (correlator throwing thousands at startup, or chart.OnPaint() doing something wrong)";
			try {
				if (base.Visible == false) {
					this.populateWindowTitle();
					return;
				}

				if (base.Timed_flushingToGui.Scheduled == true) {
					string msg1 = "MUST_CONTINUE_FLUSHING__I_INTENTIONALLY_SCHEDULED_BEFORE_FLUSHING_TO_REDIRECT_ALL_NEWCOMERS_TO_BUFFER"
						//+ " DONT_FLUSH_ME_WHEN_ANOTHER_FLUSH_SCHEDULED"
						;
					// TEST_FOR_TWO_DISCONNECTED_EXCEPTION_QUEUES__BEWARE_RECURSION
					//Assembler.PopupException(msg1 + msig);
					//return;
				}
				try {
					this.exceptions_notFlushedYet.WaitAndLockFor(this, msig);		// avoiding CollectionModified Exception - I employed WatchDog to tell (and silently resolve by sequencing) me which thread inserts while another thread is enumerating
					int flushedStartup = this.flushStartupExceptions_toBuffered();
					List<Exception> buffered = this.exceptions_notFlushedYet.SafeCopy(this, msig);
					foreach (Exception ex in buffered) {
						bool time4exAdded	= this.exceptionTimes	.Add(ex, DateTime.Now	, this, msig, ConcurrentWatchdog.TIMEOUT_DEFAULT, true);
						bool excInserted	= this.Exceptions		.InsertUnique(ex		, this, msig, ConcurrentWatchdog.TIMEOUT_DEFAULT, false);
					}
					this.exceptions_notFlushedYet.Clear(this, msig);
				} catch (Exception ex) {
					string msg2 = "BLOCKING_BUFFER_FLUSH this.exceptions_notFlushedYet => this.Exceptions";
					Assembler.PopupException(msg2 + msig, ex);
				} finally {
					this.exceptions_notFlushedYet.UnLockFor(this, msig);
				}
#if DEBUG
				if (this.Exceptions.Count > 0) {
					Exception lastInserted = this.Exceptions.First_nullUnsafe(this, msig);
					string msg2 = "HELPS_TO_FIGURE_OUT_MESSAGE_WHILE_ALREADY_IN_GUI_THREAD";
					//Assembler.PopupException(msg2 + msig);		// any Assembler.PopupException() will go to this.exceptionsNotFlushedYet()
				}
#endif

				this.olvTreeExceptions.SetObjects(this.Exceptions.SafeCopy(this, msig));		// it's slow but thread-safe and you set a delay 200ms EXACTLY because it's all expensive
				// ISNT_IT_ALREADY_INVOKED_BY_SetObjects() this.olvTreeExceptions.RebuildAll(true);
				if (this.Exceptions.Count == 0) {
					this.txtExceptionMessage.Text = "";
					this.olvStackTrace.Items.Clear();
					return;
				}
				this.olvTreeExceptions.ExpandAll();
				if (this.dataSnapshot.RecentAlwaysSelected) {
					this.selectMostRecentException();
				}

				if (this.dataSnapshot.PopupOnIncomingException == false) return;
				DockContentImproved exceptionsForm = this.Parent as DockContentImproved;
				if (exceptionsForm == null) return;
				if (exceptionsForm.IsCoveredOrAutoHidden == false) return;
				base.BringToFront();
			} catch(Exception ex) {
				string msg3 = "NOWHERE_TO_DUMP_EXCEPTION_DURING_FLUSHING";
				Assembler.PopupException(msg3 + msig);		// any Assembler.PopupException() will go to this.exceptionsNotFlushedYet()
			} finally {
				base.HowLongTreeRebuilds.Stop();
				this.populateWindowTitle();
			}
		}

		void populateWindowTitle() {
			Form parentForm = this.Parent as Form;
			if (parentForm == null) {
				string msg = "all that was probably needed for messy LivesimControl having splitContainer3<splitContainer1<LivesimControl - deleted; otherwize no idea why so many nested splitters";
				Assembler.PopupException(msg);
				return;
			}
			parentForm.Text = "Exceptions :: " + this.ToString();
		}
		public override string ToString() {
			string ret = "";
			// ALWAYS_SCHEDULED_AFTER_ANY_NEWCOMER_BUFFERED_OR_FLUSHED ret += this.timerFlushToGui_noNewcomersWithinDelay.Scheduled ? "BUFFERING " : "";
			// ALREADY_PRINTED_2_LINES_LATER ret += this.exceptions_notFlushedYet.Count ? "BUFFERING " : "";
			ret += this.Exceptions.Count.ToString("000");
			ret += "/" + this.exceptions_notFlushedYet.Count.ToString("000") + "buffered";

			List<Exception> exStartup = Assembler.InstanceInitialized.Exceptions_duringApplicationStartup_beforeMainForm_gotWindowHandle;
			if (exStartup != null && exStartup.Count > 0) {
				ret += "/" + exStartup.Count.ToString("000") + "startup";
			}

			ret += base.FlushingStats;

			ret += "   tillSerialization:"
				+ this.Exceptions.Logrotator.NextSerialization_estimatedIn.TotalMilliseconds.ToString("N2")
				+ "ms";
			ret += "   records:"
				+ this.Exceptions.Logrotator.LastSerialization_records;
			return ret;
		}

		public void Clear() {
			//this.flushExceptionsToOLV_switchToGuiThread();
			this.Exceptions.Clear(this, "mniClear_Click");
			this.flushExceptionsToOLV_switchToGuiThread();
		}
	}
}