using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Core.Streaming;

using Sq1.Widgets;

using Sq1.Adapters.Quik.Streaming.Dde;
using Sq1.Adapters.Quik.Streaming.Dde.XlDde;

namespace Sq1.Adapters.Quik.Streaming.Monitor {
	public partial class QuikLevel2Control {
		protected override void OnLoad(EventArgs e) {
		    try {
				this.tableLevel2.OnDataStructureParsed_One				+= new EventHandler<XlDdeTableMonitoringEventArg<LevelTwo>>			(tableLevel2_OnDataStructureParsed_One);
				this.tableLevel2.OnDataStructuresParsed_Table			+= new EventHandler<XlDdeTableMonitoringEventArg<List<LevelTwo>>>	(tableLevel2_OnDataStructuresParsed_Table);
				//this.tableLevel2.QuikStreaming.OnConnectionStateChanged += new EventHandler<EventArgs>(this.quikStreaming_OnConnectionStateChanged);
		        base.OnLoad(e);

		        base.PopulateLevel2ToTitle();

				//if (this.quikStreaming.DdeMonitorPopupOnRestart) return;	// we are deserializing => popping up => no need to set the flag I'm to complying to
				//this.quikStreaming.DdeMonitorPopupOnRestart = true;
				//this.quikStreaming.DataSourceEditor.SerializeDataSource_saveAdapters();
		    } catch (Exception ex) {
		        string msg = "IS_DATASOURCE_EDITOR_NULL? this.quikStreaming.DataSourceEditor[" + this.tableLevel2.QuikStreaming.DataSourceEditor + "] //QuikStreamingMonitorForm.OnLoad()";
		        Assembler.PopupException(msg, ex);
		    }
		}
		//protected override void OnFormClosing(FormClosingEventArgs e) {
		//    try {
		//        //base.OnFormClosing(e);
		//        //this.tableLevel2.QuikStreaming.OnConnectionStateChanged -= new EventHandler<EventArgs>(this.quikStreaming_OnConnectionStateChanged);
		//        this.tableLevel2.OnDataStructureParsed_One				-= new EventHandler<XlDdeTableMonitoringEventArg<LevelTwo>>			(tableLevel2_OnDataStructureParsed_One);
		//        this.tableLevel2.OnDataStructuresParsed_Table			-= new EventHandler<XlDdeTableMonitoringEventArg<List<LevelTwo>>>	(tableLevel2_OnDataStructuresParsed_Table);

		//        if (Assembler.InstanceInitialized.MainFormClosingIgnoreReLayoutDockedForms) return;	// I closed the app, but user didn't click the Monitor and wants it be restored on appRestart
		//        //this.quikStreaming.DdeMonitorPopupOnRestart = false;
		//        //this.quikStreaming.DataSourceEditor.SerializeDataSource_saveAdapters();
		//    } catch (Exception ex) {
		//        string msg = "IS_DATASOURCE_EDITOR_NULL? this.quikStreaming.DataSourceEditor[" + this.quikStreaming.DataSourceEditor + "] //QuikStreamingMonitorForm.OnFormClosing()";
		//        Assembler.PopupException(msg, ex);
		//    }
		//}


		void tableLevel2_OnDataStructuresParsed_Table(object sender, XlDdeTableMonitoringEventArg<List<LevelTwo>> e) {
		    if (base.IsDisposed) return;
		    if (this.InvokeRequired) {
		        base.BeginInvoke((MethodInvoker)delegate { this.tableLevel2_OnDataStructuresParsed_Table(sender, e); });
		        return;
		    }
		    // I paid the price of switching to GuiThread, but I don' have to worry if I already StopwatchRarifyingUIupdates.Restart()ed
		    //if (this.stopwatchRarifyingUIupdates.ElapsedMilliseconds < this.tableLevel2.QuikStreaming.DdeMonitorRefreshRateMs) return;
		    //NOPE_RESTARTED_FOR_THE_WHOLE_CONTROL this.stopwatchRarifyingUIupdates.Restart();

		    //base.OlvLevelTwo.SetObjects(e.DataStructureParsed);
		    this.level2_OnDataStructuresParsed_Table_butAlwaysOneElementInList(sender, e);
		}

		// copypaste from pushing DdeBatchSubscriber.EventConsumer
		void level2_OnDataStructuresParsed_Table_butAlwaysOneElementInList(
							object sender, XlDdeTableMonitoringEventArg<List<LevelTwo>> alwaysJustOneDom) {
			string msig = " //level2_OnDataStructuresParsed_Table_butAlwaysOneElementInList(" + sender + ")";
			XlDdeTableMonitoreable<LevelTwo> tableLevel2 = sender as XlDdeTableMonitoreable<LevelTwo>;
			if (tableLevel2 == null) {
				string msg = "I_MUST_HAVE_BEEN_INVOKED_WITH_XlDdeTableMonitoreable<Level2>";
				Assembler.PopupException(msg + msig);
				return;
			}

			// finally Form and inner Control are in the same DLL!!
			//DockContentImproved ddeMonitorForm = base.DdeMonitorForm_nullUnsafe;
			//if (ddeMonitorForm != null) {
			//    if (ddeMonitorForm.Visible == false) return;
			//    if (ddeMonitorForm.IsCoveredOrAutoHidden) return;
			//}

			// second BeginInvoke below is hell of overhead, but this one is light, and succeeds if the second fails => visible counters increase
			base.PopulateLevel2ToTitle(tableLevel2.ToString());

			if (alwaysJustOneDom == null) {
				string msg = "MUST_NOT_BE_NULL_EVENT_ARG alwaysJustOneDom[" + alwaysJustOneDom + "]";
				Assembler.PopupException(msg + msig);
				return;
			}
			if (alwaysJustOneDom.DataStructureParsed == null) {
				string msg = "MUST_NOT_BE_NULL_PARSED alwaysJustOneDom.DataStructureParsed[" + alwaysJustOneDom.DataStructureParsed + "]";
				Assembler.PopupException(msg + msig);
				return;
			}
			LevelTwo level2fromDde_pushTo_baseUserControl = null;
			if (alwaysJustOneDom.DataStructureParsed.Count != 1) {
				//v1 string msg = "MUST_BE_ONLY_ONE_LEVEL2_IN_THE_LIST alwaysJustOneDom.DataStructureParsed.Count[" + alwaysJustOneDom.DataStructureParsed.Count + "]";
				//v1 Assembler.PopupException(msg + msig);
				//v1 return;
				string msg = "I_MANUALLY_RAISED_EVENT_WITH_EMPTY_LIST_TO_CLEAR_ANYTHING_(QUOTES/LEVEL2/TRADES)_RIGHT_AFTER_USER_STOPPED_DDE_FEED";
				level2fromDde_pushTo_baseUserControl = new LevelTwo(null, null, null);
			} else {
				level2fromDde_pushTo_baseUserControl = alwaysJustOneDom.DataStructureParsed[0];
			}
			base.PopulateLevel2ToDomControl(level2fromDde_pushTo_baseUserControl);
		}


		void tableLevel2_OnDataStructureParsed_One(object sender, XlDdeTableMonitoringEventArg<LevelTwo> e) {
		}
	
	}
}
