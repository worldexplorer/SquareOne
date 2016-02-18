using System;
using System.Text;
using System.Globalization;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Core.Serializers;
using Sq1.Support;

namespace Sq1.Widgets.Exceptions {
	public partial class ExceptionsControl {
				object		lockedByTreeListView;
				Exception	exceptionSelectedInTree { get { return this.treeExceptions.SelectedObject as Exception; } }

		public	ExceptionsControlDataSnapshot				DataSnapshot;
		public	Serializer<ExceptionsControlDataSnapshot>	DataSnapshotSerializer;
		public	List<Exception>								Exceptions				{ get; protected set; }
		public	Dictionary<Exception, DateTime>				ExceptionTimes			{ get; protected set; }

		//v2 for delayed auto-flushing
				List<Exception>			exceptionsNotFlushedYet;
				Stopwatch				howLongTreeRebuilds;
				System.Windows.Forms.Timer	rebuildTimerWF;
				int						rebuildInitialDelay;
				bool					rebuildScheduled;
				DateTime				exceptionDateNotFlushedLast;
				object					lockInsertAsync;

		public ExceptionsControl() : base() {
			lockedByTreeListView = new object();
			Exceptions = new List<Exception>();
			ExceptionTimes = new Dictionary<Exception, DateTime>();

			exceptionsNotFlushedYet = new List<Exception>();
			howLongTreeRebuilds = new Stopwatch();
			exceptionDateNotFlushedLast = DateTime.MinValue;
			rebuildInitialDelay = 200;
			lockInsertAsync = new object();

			this.InitializeComponent();
			//WindowsFormsUtils.SetDoubleBuffered(this.tree);	//doesn't help, still flickers
			WindowsFormsUtils.SetDoubleBuffered(this.lvStackTrace);
			WindowsFormsUtils.SetDoubleBuffered(this);
			
			this.exceptionsTreeListViewCustomize();
			this.treeExceptions.SetObjects(this.Exceptions);

			this.rebuildTimerWF = new System.Windows.Forms.Timer();
			this.rebuildTimerWF.Enabled = true;
			this.rebuildTimerWF.Tick += new EventHandler(rebuildTimerWF_Tick);
#if USE_CONTROL_IMPROVED
			this.Ident_UserControlImproved = "ExceptionsControl";
#endif
		}

		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			if (rebuildTimerWF != null) {
				this.rebuildTimerWF.Stop();
				this.rebuildTimerWF.Enabled = false;
				this.rebuildTimerWF.Dispose();
			}
			base.Dispose(disposing);
		}

		public void Initialize() {
			//WARNING! LiveSimControl uses non-ExceptionsForm-singleton MULTIPLE ExceptionsControls for LivesimStreamingEditor and LivesimBrokerEditor
			//LivesimControl throws here in Designer (unbelieabable though)
			if (Assembler.IsInitialized == false) return;		// useful for MultiSplitTest, PanelsTest
				
			this.DataSnapshotSerializer = new Serializer<ExceptionsControlDataSnapshot>();
			bool createdNewFile = this.DataSnapshotSerializer.Initialize(Assembler.InstanceInitialized.AppDataPath,
				"Sq1.Widgets.ExceptionsControlDataSnapshot.json", "Workspaces",
				Assembler.InstanceInitialized.AssemblerDataSnapshot.WorkspaceCurrentlyLoaded);
			this.DataSnapshot = this.DataSnapshotSerializer.Deserialize();
			if (createdNewFile) {
				this.DataSnapshot.SplitDistanceVertical = this.splitContainerVertical.SplitterDistance;
				this.DataSnapshot.SplitDistanceHorizontal = this.splitContainerHorizontal.SplitterDistance;
				this.DataSnapshotSerializer.Serialize();
			}
		}
		public void PopulateDataSnapshotInitializeSplittersAfterDockContentDeserialized() {
			string msig = " //PopulateDataSnapshotInitializeSplittersAfterDockContentDeserialized()";
			// WILL_SET_THIS_FLAG_NEXT_LINE_UPSTACK
			// NOT_ANYMORE!!!_NOW_TIMER____ALSO_in_SplitContainerVertical/Horizontal_SplitterMoved_I_IGNORE_DISTANCES_RELYING_ON_MainFormDockFormsFullyDeserializedLayoutComplete
//			if (Assembler.InstanceInitialized.MainFormDockFormsFullyDeserializedLayoutComplete == false) {
//				string msg = "CHOOSE_ANOTHER_INVOCATION_SPOT__AFTER_MAIN_FORM_FULLY_DESERIALIZED";
//				Assembler.PopupException(msg);
//			}
			if (this.Width == 0) {
				string msg = "CANT_SET_SPLITTER_DISTANCE_FOR_UNSHOWN_CONTROL ExceptionsControl.Visible[" + this.Visible + "]; can't set SplitDistanceVertical, SplitDistanceHorizontal";
				Assembler.PopupException(msg + msig);
			} else {
				try {
					this.SuspendLayout();
					if (this.DataSnapshot.SplitDistanceVertical > 0) {
						string msg = "+51_SEEMS_TO_BE_REPRODUCED_AT_THE_SAME_DISTANCE_I_LEFT_VERTICAL";
						int newVerticalDistance = this.DataSnapshot.SplitDistanceVertical;	//  + 51 + this.splitContainerVertical.SplitterWidth;
						if (this.splitContainerVertical.SplitterDistance != newVerticalDistance) {
							this.splitContainerVertical.SplitterDistance =  newVerticalDistance;
						}
					}
					if (this.DataSnapshot.SplitDistanceHorizontal > 0) {
						string msg = "+67_SEEMS_TO_BE_REPRODUCED_AT_THE_SAME_DISTANCE_I_LEFT_HORIZONTAL";
						int newHorizontalDistance = this.DataSnapshot.SplitDistanceHorizontal;	// + 97 + this.splitContainerHorizontal.SplitterWidth;
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

			this.mniRecentAlwaysSelected.Checked = this.DataSnapshot.RecentAlwaysSelected;
			this.mniltbDelay.InputFieldValue = this.DataSnapshot.TreeRefreshDelayMsec.ToString();
			this.mniTreeShowExceptionTime.Checked = this.DataSnapshot.TreeShowExceptionTime;
			this.olvTime.Text = this.DataSnapshot.TreeShowExceptionTime ? "Time" : "Message";
		}
		public void InsertExceptionBlocking(Exception exception) {
			if (exception == null) {
				string msg = "DONT_INVOKE_ME_WITH_NULL";
				//v1 IM_AFRAID_OF_RECURSION_HERE Assembler.PopupException(msg);
				//v2
				#if DEBUG
				Debugger.Break();
				#endif
				return;
			}
			lock (this.lockedByTreeListView) {
				if (this.rebuildScheduled) {
					this.exceptionsNotFlushedYet.Add(exception);
					return;
				}
				if (this.Exceptions.Count == 0) {
					string msg = "SHOULD_HAPPEN_ONCE_PER_APP_LIFETIME";
					//Debugger.Break();
				}

				this.ExceptionTimes.Add(exception, DateTime.Now);
				this.Exceptions.Insert(0, exception);
			}

			//flushToTree will visualize whole the control AFTER IT WILL BE SHOWN 
//			this.treeExceptions.SetObjects(this.Exceptions);
//			//this.tree.RefreshObject(exception);
//			this.treeExceptions.RebuildAll(true);
//			//v2 http://stackoverflow.com/questions/7949887/how-to-add-a-new-item-into-objectlistview
//			//"When the ListView is in virtual mode, you cannot add items to the ListView items collection. Use  the VirtualListSize property instead to change the size of the ListView items collection."
//			//this.tree.InsertObjects(0, new List<Exception>() { exception });
//			this.treeExceptions.Expand(exception);
//			// MAKES StrategiesTreeControl.CellClick invoke handlers 100 times!!! nonsense I know Application.DoEvents();	// TsiProgressBarETAClick doesn't get control when every quote there is an exception and user can't interrupt the backtest
			// MOVED_TO_EXCEPTIONS_FORM this.FlushListToTreeIfDockContentDeserialized();
		}
		void selectMostRecentException() {
			if (this.treeExceptions.GetItemCount() == 0) return;
			this.treeExceptions.SelectedIndex = 0;
			//WHAT_FOR? this.treeExceptions.RefreshSelectedObjects();
			this.treeExceptions.EnsureVisible(0);
		}
		void displayStackTrace() {
			try {
				this.lvStackTrace.BeginUpdate();
				this.lvStackTrace.Items.Clear();
				StackTrace stackTrace = new StackTrace(this.exceptionSelectedInTree);
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
					this.lvStackTrace.Items.Add(item);
				}
			} finally {
				this.lvStackTrace.EndUpdate();
			}
		}

		public void InsertAsyncAutoFlush(Exception ex) {
			// http://stackoverflow.com/questions/2475435/c-sharp-timer-wont-tick
			// Always stop/start a System.Windows.Forms.Timer on the UI thread, apparently! –  user145426

			// Timer is Enabled until event fired; after that Enabled can be used for a repetitive firing (I don't use repetitive so on every re-use of Start() I set Enabled
			// hint: https://msdn.microsoft.com/en-us/library/system.windows.forms.timer.tick(v=vs.110).aspx

			if (this.InvokeRequired) {
				base.BeginInvoke((MethodInvoker)delegate { this.InsertAsyncAutoFlush(ex); });
				return;
			}
			
			lock (this.lockInsertAsync) {
				this.InsertExceptionBlocking(ex);
				//v1
				//if (Assembler.InstanceInitialized.MainFormDockFormsFullyDeserializedLayoutComplete == false) {
				//	return;
				//}
				if (this.rebuildScheduled) {
					// third exception came in (tree might have finished rebuilding from first), and we scheduled a rebuild for second => postpone the second rebuild
					//TimeSpan rebuildPostponingDelay = DateTime.Now.Subtract(this.exceptionDateNotFlushedLast);
					//if (rebuildPostponingDelay.TotalMilliseconds == 0) {
					//    string msg = "LOOKS_VERY_POSSIBLE__BUT_I_HAVE_NO_SOLUTION";
					//    //#if DEBUG
					//    //Debugger.Break();
					//    //#endif
					//    return;
					//    //rebuildPostponingDelay = new TimeSpan(0, 0, 0, 0, this.rebuildInitialDelay);
					//}
					//rebuildPostponingDelay = new TimeSpan(0, 0, 0, 1);
					this.rebuildTimerWF.Stop();
					//this.rebuildTimerWF.Interval = (int)rebuildPostponingDelay.TotalMilliseconds;		// OUTRAGEOUS_NUMBER, WHOLE IF(){} IS BUSTED
					this.rebuildTimerWF.Interval = this.rebuildInitialDelay;
					this.rebuildTimerWF.Start();
					//this.rebuildTimerWF.Enabled = true;
					this.exceptionDateNotFlushedLast = DateTime.Now;
					return;
				}
				//if (this.InvokeRequired == true	// if we are in GUI thread, go on timer immediately (correlator throwing thousands at startup, or chart.OnPaint() doing something wrong)
				//    || Assembler.InstanceInitialized.MainFormDockFormsFullyDeserializedLayoutComplete == false
				//    ) {
					// second exception came in while tree was rebuilding (after the first one wasn't finished) => schedule rebuild in 
					this.rebuildTimerWF.Interval = this.rebuildInitialDelay;
					this.rebuildTimerWF.Start();
					//this.rebuildTimerWF.Enabled = true;
					this.exceptionDateNotFlushedLast = DateTime.Now;
				//    return;
				//}
				// first exception came in => rebuild immediately, but mark already now that all other fresh exceptions would line up by timer
				this.rebuildScheduled = true;
			}
		}

		void rebuildTimerWF_Tick(object sender, EventArgs e) {
#if USE_CONTROL_IMPROVED
			if (base.ParentControlsLoaded_NonBlocking == false) {
				return;			// Timer will re-invoke rebuildTimerWF_Tick, and when base.ParentControlsLoaded_NonBlocking==true, we'll flushExceptionsToOLVIfDockContentDeserialized_inGuiThread()
			}
#endif
			// NOT_NEEDED_DUE_TO_NON_BLOCKING_ABOVE bool loadedIwaited = base.ParentControlIsLoaded_Blocking;
			this.flushExceptionsToOLVIfDockContentDeserialized_inGuiThread();
		}
		public void flushExceptionsToOLVIfDockContentDeserialized_inGuiThread(object stateForTimerCallback = null) {
			if (base.DesignMode) return;
			if (Assembler.IsInitialized == false) return; //I_HATE_LIVESIM_FORM_THROWING_EXCEPTIONS_IN_DESIGN_MODE
			// WINDOWS.FORMS.VISIBLE=FALSE_IS_SET_BY_DOCK_CONTENT_LUO ANALYZE_DockContentImproved.IsShown_INSTEAD if (this.Visible == false) return;
			if (Assembler.InstanceInitialized.MainFormDockFormsFullyDeserializedLayoutComplete == false) {
				return;
			}
			if (this.InvokeRequired) {
				this.BeginInvoke((MethodInvoker)delegate() { this.flushExceptionsToOLVIfDockContentDeserialized_inGuiThread(); });
				//string msg = "I_MUST_BE_ALREADY_IN_GUI_THREAD__HOPING_TO_INSERT_IN_SEQUENCE_OF_INVOCATION";
				//Debugger.Break();
				return;
			} else {
				string msg = "if we are in GUI thread, go on timer immediately (correlator throwing thousands at startup, or chart.OnPaint() doing something wrong)";
				//return;
			}
			try {
				lock (this.lockedByTreeListView) {
					//if (this.rebuildScheduled == true) return;
					foreach (Exception ex in this.exceptionsNotFlushedYet) {
						this.ExceptionTimes.Add(ex, DateTime.Now);
						this.Exceptions.Insert(0, ex);
					}
					this.exceptionsNotFlushedYet.Clear();
					if (this.Exceptions.Count > 0) {
						Exception lastAdded = this.Exceptions[0];
						string msg = "HELPS_TO_FIGURE_OUT_MESSAGE_WHILE_ALREADY_IN_GUI_THREAD";
					}
					//v1
					this.treeExceptions.SetObjects(this.Exceptions);
					this.treeExceptions.RebuildAll(true);
					if (this.Exceptions.Count == 0) {
						this.txtExceptionMessage.Text = "";
						this.lvStackTrace.Items.Clear();
						return;
					}
					this.treeExceptions.ExpandAll();
					if (this.DataSnapshot.RecentAlwaysSelected) {
						this.selectMostRecentException();
					}
					//v2
					//return;
					//v3
					//Thread.Sleep(10000);
				}
			} catch(Exception ex) {
				string msg = "NOWHERE_TO_DUMP_EXCEPTION_DURING_FLUSHING";
				throw new Exception(msg, ex);
			} finally {
				//this.rebuildTimer.Dispose();
				//this.rebuildTimer = null;
				//if (this.rebuildTimerWF != null) {
					this.rebuildTimerWF.Stop();
					//this.rebuildTimerWF.Enabled = false;
				//	this.rebuildTimerWF.Dispose();
				//	this.rebuildTimerWF = null;
				//} else {
				//	string msg = "YOU_CLICKED_MNI_EXCEPTIONS_CLEAR_OR_REFRESH";
				//}
				this.rebuildScheduled = false;
				if (this.InvokeRequired == false) {
					Form parentForm = this.Parent as Form;
					if (parentForm == null) {
						string msg = "all that was probably needed for messy LivesimControl having splitContainer3<splitContainer1<LivesimControl - deleted; otherwize no idea why so many nested splitters";
						Assembler.PopupException(msg);
						//SplitterPanel parentSplitterPanel = this.Parent as SplitterPanel;
						//if (parentSplitterPanel != null) {
						//    SplitContainer parentSplitContainer = parentSplitterPanel.Parent as SplitContainer;
						//    if (parentSplitContainer != null) {
						//        parentForm = parentSplitContainer.Parent as Form;
						//    }
						//}
					}
					if (parentForm != null) {
						string counters = this.Exceptions.Count.ToString();
						if (this.exceptionsNotFlushedYet.Count > 0) counters += "/" + this.exceptionsNotFlushedYet.Count;
						parentForm.Text = "Exceptions :: " + counters;
					}
				}
			}
		}
		public override string ToString() {
			StringBuilder formattedException = new StringBuilder();
			if (this.exceptionSelectedInTree != null) {
				formattedException.Append("EXCEPTION INFORMATION").Append(Environment.NewLine)
					.Append(Environment.NewLine)
					.Append("Date/Time: ").Append(DateTime.Now.ToString("F", CultureInfo.CurrentCulture))
					.Append(Environment.NewLine)
					.Append("Type: ").Append(this.exceptionSelectedInTree.GetType().FullName).Append(Environment.NewLine)
					.Append("Message: ").Append(this.exceptionSelectedInTree.Message).Append(Environment.NewLine)
					.Append("Source: ").Append(this.exceptionSelectedInTree.Source).Append(Environment.NewLine)
					.Append("Target Method: ");
				if (this.exceptionSelectedInTree.TargetSite != null) {
					formattedException.Append(this.exceptionSelectedInTree.TargetSite.ToString());
				}
				formattedException.Append(Environment.NewLine).Append(Environment.NewLine)
					.Append("Call Stack:").Append(Environment.NewLine);

				StackTrace exceptionStack = new StackTrace(this.exceptionSelectedInTree);

				for (int i = 0; i < exceptionStack.FrameCount; i++) {
					StackFrame exceptionFrame = exceptionStack.GetFrame(i);

					formattedException.Append("\t").Append("Method Name: ").Append(exceptionFrame.GetMethod().ToString()).Append(Environment.NewLine)
						.Append("\t").Append("\t").Append("File Name: ").Append(exceptionFrame.GetFileName()).Append(Environment.NewLine)
						.Append("\t").Append("\t").Append("Column: ").Append(exceptionFrame.GetFileColumnNumber()).Append(Environment.NewLine)
						.Append("\t").Append("\t").Append("Line: ").Append(exceptionFrame.GetFileLineNumber()).Append(Environment.NewLine)
						.Append("\t").Append("\t").Append("CIL Offset: ").Append(exceptionFrame.GetILOffset()).Append(Environment.NewLine)
						.Append("\t").Append("\t").Append("Native Offset: ").Append(exceptionFrame.GetNativeOffset()).Append(Environment.NewLine)
						.Append(Environment.NewLine);
				}

				formattedException.Append("Inner Exception(s)").Append(Environment.NewLine);

				Exception innerException = this.exceptionSelectedInTree.InnerException;

				while (innerException != null) {
					formattedException.Append("\t").Append("Exception: ")
						.Append(innerException.GetType().FullName).Append(Environment.NewLine);
					innerException = innerException.InnerException;
				}

				formattedException.Append(Environment.NewLine).Append("Custom Properties")
					.Append(Environment.NewLine);

				Type exceptionType = typeof(Exception);

				foreach (PropertyInfo propertyInfo in this.exceptionSelectedInTree.GetType().GetProperties()) {
					if (exceptionType.GetProperty(propertyInfo.Name) == null) {
						formattedException.Append("\t").Append(propertyInfo.Name).Append(": ")
							.Append(propertyInfo.GetValue(this.exceptionSelectedInTree, null))
							.Append(Environment.NewLine);
					}
				}
			}

			return formattedException.ToString();
		}
		public void CopyExceptionDataToClipboard() {
			Clipboard.SetDataObject(this.ToString(), true);
		}
	}
}