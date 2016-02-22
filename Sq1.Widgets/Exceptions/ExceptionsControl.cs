using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Core.Serializers;
using Sq1.Core.Support;

using Sq1.Support;

namespace Sq1.Widgets.Exceptions {
	public partial class ExceptionsControl {
				ExceptionsControlDataSnapshot				dataSnapshot;
				Serializer<ExceptionsControlDataSnapshot>	dataSnapshotSerializer;
				ConcurrentDictionary<Exception, DateTime>	exceptionTimes;
		public	ConcurrentList<Exception>					Exceptions				{ get; private set; }
				ConcurrentList<Exception>					exceptions_notFlushedYet;

				Stopwatch			howLongTreeRebuilds;
				DateTime			exceptionLastDate_notFlushedYet;

				TimerSimplifiedTask	timedTask_flushingToGui;

				Exception			exceptionSingleSelectedInTree_nullUnsafe		{ get { return this.olvTreeExceptions.SelectedObject as Exception; } }
				DockContentImproved parentAsDockContentImproved_nullUnsafe			{ get { return base.Parent as DockContentImproved; } }

		public ExceptionsControl() : base() {
			exceptionTimes				= new ConcurrentDictionary<Exception, DateTime>("exceptionTimes_delayedGuiFlush");
			Exceptions					= new ConcurrentList<Exception>("Exceptions_delayedGuiFlush");
			exceptions_notFlushedYet	= new ConcurrentList<Exception>("exceptions_notFlushedYet");
			exceptionLastDate_notFlushedYet	= DateTime.MinValue;
			howLongTreeRebuilds			= new Stopwatch();

			timedTask_flushingToGui = new TimerSimplifiedTask("FLUSH_EXCEPTIONS_CONTROL__AFTER_HAVING_BUFFERED_FOR_LONG_ENOUGH", this, new Action(delegate {this.flushExceptionsToOLV_switchToGuiThread(); }));
			timedTask_flushingToGui.Delay = 200;

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
				this.timedTask_flushingToGui.Dispose();
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
			this.timedTask_flushingToGui.Delay = this.dataSnapshot.TreeRefreshDelayMsec;	// may be already started?

			this.timedTask_flushingToGui.Delay = this.dataSnapshot.TreeRefreshDelayMsec;	// may be already started?
			this.timedTask_flushingToGui.Start();
		}
		public void PopulateDataSnapshot_initializeSplitters_afterDockContentDeserialized_invokeMeFromGuiThreadOnly() {
			string msig = " //PopulateDataSnapshot_initializeSplitters_afterDockContentDeserialized()";
			if (Assembler.InstanceInitialized.MainFormDockFormsFullyDeserializedLayoutComplete == false) {
				string msg = "CHOOSE_ANOTHER_INVOCATION_SPOT__AFTER_MAIN_FORM_FULLY_DESERIALIZED";
				Assembler.PopupException(msg);
				return;
			}

			if (base.Width == 0) {
				string msg = "CANT_SET_SPLITTER_DISTANCE_FOR_UNSHOWN_CONTROL ExceptionsControl.Visible[" + this.Visible + "]; can't set SplitDistanceVertical, SplitDistanceHorizontal";
				Assembler.PopupException(msg + msig);
			} else {
				try {
					this.SuspendLayout();
					if (this.dataSnapshot.SplitDistanceVertical > 0) {
						string msg = "+51_SEEMS_TO_BE_REPRODUCED_AT_THE_SAME_DISTANCE_I_LEFT_VERTICAL";
						int newVerticalDistance = this.dataSnapshot.SplitDistanceVertical;	//  + 51 + this.splitContainerVertical.SplitterWidth;
						if (this.splitContainerVertical.SplitterDistance != newVerticalDistance) {
							this.splitContainerVertical.SplitterDistance =  newVerticalDistance;
						}
					}
					if (this.dataSnapshot.SplitDistanceHorizontal > 0) {
						string msg = "+67_SEEMS_TO_BE_REPRODUCED_AT_THE_SAME_DISTANCE_I_LEFT_HORIZONTAL";
						int newHorizontalDistance = this.dataSnapshot.SplitDistanceHorizontal;	// + 97 + this.splitContainerHorizontal.SplitterWidth;
						if (this.splitContainerHorizontal.SplitterDistance != newHorizontalDistance) {
							this.splitContainerHorizontal.SplitterDistance =  newHorizontalDistance;
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

			this.mniRecentAlwaysSelected.Checked = this.dataSnapshot.RecentAlwaysSelected;
			this.mniltbDelay.InputFieldValue = this.dataSnapshot.TreeRefreshDelayMsec.ToString();
			this.mniShowTimestamps.Checked = this.dataSnapshot.TreeShowTimestamps;

			this.flushExceptionsToOLV_switchToGuiThread();
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

			if (this.timedTask_flushingToGui.Scheduled) {
				this.exceptions_notFlushedYet.AppendUnique(exception, this, "insertTo_exceptionsNotFlushedYet_willReportIfBlocking");
				return;
			}
			if (this.Exceptions.Count == 0) {
				string msg = "SHOULD_HAPPEN_ONCE_PER_APP_LIFETIME";
				//Debugger.Break();
			}

			this.exceptionTimes.Add(exception, DateTime.Now, this, "InsertExceptionBlocking");
			this.Exceptions.InsertUnique(0, exception, this, "InsertExceptionBlocking");
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

		public void InsertAsyncAutoFlush(Exception ex) {
			this.insertTo_exceptionsNotFlushedYet_willReportIfBlocking(ex);
			if (this.timedTask_flushingToGui.Scheduled) {
				// second exception came in while tree was rebuilding (after the first one wasn't finished) => schedule rebuild in 
				this.timedTask_flushingToGui.ScheduleOnce();	// postpone flushToGui() for another 200ms
				this.exceptionLastDate_notFlushedYet = DateTime.Now;

				if (base.InvokeRequired) {	// waiting for WindowHandle to be created
					base.BeginInvoke((MethodInvoker)delegate() { this.populateWindowsTitle(); });
				}

				return;
			}
			// http://stackoverflow.com/questions/2475435/c-sharp-timer-wont-tick
			// Always stop/start a System.Windows.Forms.Timer on the UI thread, apparently! –  user145426

			// first exception came in => flush immediately; mark already now that all other fresh exceptions would line up by timer
			this.timedTask_flushingToGui.ScheduleOnce();
			this.exceptionLastDate_notFlushedYet = DateTime.Now;
			this.flushExceptionsToOLV_switchToGuiThread();
		}

		//public void FlushExceptionsToOLV_switchToGuiThread_atDockContentDeserialized() {
		//    this.flushExceptionsToOLV_switchToGuiThread();
		//}

		void flushExceptionsToOLV_switchToGuiThread(object stateForTimerCallback = null) {
			string msig = " //flushExceptionsToOLV_switchToGuiThread_ifDockContentDeserialized()";
			if (base.DesignMode) return;
			if (Assembler.IsInitialized == false) return; //I_HATE_LIVESIM_FORM_THROWING_EXCEPTIONS_IN_DESIGN_MODE
			// WINDOWS.FORMS.VISIBLE=FALSE_IS_SET_BY_DOCK_CONTENT_LUO ANALYZE_DockContentImproved.IsShown_INSTEAD if (this.Visible == false) return;
			if (Assembler.InstanceInitialized.MainFormDockFormsFullyDeserializedLayoutComplete == false) return;

			if (this.InvokeRequired) {
				this.BeginInvoke((MethodInvoker)delegate() { this.flushExceptionsToOLV_switchToGuiThread(); });
				//string msg = "I_MUST_BE_ALREADY_IN_GUI_THREAD__HOPING_TO_INSERT_IN_SEQUENCE_OF_INVOCATION";
				//Debugger.Break();
				return;
			}

			this.howLongTreeRebuilds.Restart();

			string msg = "if we are in GUI thread, go on timer immediately (correlator throwing thousands at startup, or chart.OnPaint() doing something wrong)";
			try {
				if (base.Visible == false) {
					this.populateWindowsTitle();
					return;
				}

				if (this.timedTask_flushingToGui.Scheduled == true) {
					string msg1 = "MUST_CONTINUE_FLUSHING__I_INTENTIONALLY_SCHEDULED_BEFORE_FLUSHING_TO_REDIRECT_ALL_NEWCOMERS_TO_BUFFER"
						//+ " DONT_FLUSH_ME_WHEN_ANOTHER_FLUSH_SCHEDULED"
						;
					// TEST_FOR_TWO_DISCONNECTED_EXCEPTION_QUEUES__BEWARE_RECURSION
					//Assembler.PopupException(msg1 + msig);
					//return;
				}
				try {
					this.exceptions_notFlushedYet.WaitAndLockFor(this, msig);		// avoiding CollectionModified Exception - I employed WatchDog to tell (and silently resolve by sequencing) me which thread inserts while another thread is enumerating
					List<Exception> buffered = this.exceptions_notFlushedYet.SafeCopy(this, msig);
					foreach (Exception ex in buffered) {
						this.exceptionTimes.Add(ex, DateTime.Now, this, msig);
						this.Exceptions.InsertUnique(0, ex, this, msig);
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
				this.howLongTreeRebuilds.Stop();
				this.populateWindowsTitle();
			}
		}

		void populateWindowsTitle() {
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
			//if (this.exceptions_notFlushedYet.Count > 0)
				ret += "/" + this.exceptions_notFlushedYet.Count.ToString("000") + "buffered";
			ret += "   flushed:" + howLongTreeRebuilds.ElapsedMilliseconds + "ms"
				 + "   buffering:" + this.timedTask_flushingToGui.Delay + "ms";
			if (howLongTreeRebuilds.ElapsedMilliseconds > this.timedTask_flushingToGui.Delay) {
				string msg = "YOU_MAY_NEED_TO_INCREASE_TIMER.Delay_FOR_FLUSHING " + ret;
				// STACK_OVERFLOW_AGAIN Assembler.PopupException(msg);
				//this.insertTo_exceptionsNotFlushedYet_willReportIfBlocking(new Exception(msg));
			}
			return ret;
		}
	}
}