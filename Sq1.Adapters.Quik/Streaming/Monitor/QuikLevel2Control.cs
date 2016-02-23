using System;
using System.Diagnostics;
using System.Collections.Generic;

using Sq1.Core;
using Sq1.Core.Streaming;

using Sq1.Widgets.Level2;

using Sq1.Adapters.Quik.Streaming.Dde;
using Sq1.Adapters.Quik.Streaming.Dde.XlDde;

namespace Sq1.Adapters.Quik.Streaming.Monitor {
	public partial class QuikLevel2Control : LevelTwoUserControl {
		DdeTableDepth		ddeTableDepth;
		Stopwatch			stopwatchRarifyingUIupdates;

		public QuikLevel2Control(DdeTableDepth ddeTableDepth_passed, Stopwatch stopwatchRarifyingUIupdates_passed) : base() {
			this.ddeTableDepth = ddeTableDepth_passed;
			this.stopwatchRarifyingUIupdates = stopwatchRarifyingUIupdates_passed;

			base.Initialize(ddeTableDepth.QuikStreaming, ddeTableDepth.SymbolInfo, ddeTableDepth.ToString());
		}

		public void MyTableDepth_subscribe() {
			try {
				this.ddeTableDepth.OnDataStructureParsed_One			+= new EventHandler<XlDdeTableMonitoringEventArg<LevelTwo>>			(this.ddeTableDepth_OnDataStructureParsed_One);
				this.ddeTableDepth.OnDataStructuresParsed_Table			+= new EventHandler<XlDdeTableMonitoringEventArg<List<LevelTwo>>>	(this.ddeTableDepth_OnDataStructuresParsed_Table_butAlwaysOneElementInList);
				this.ddeTableDepth.QuikStreaming.OnConnectionStateChanged += new EventHandler<EventArgs>(this.quikStreaming_OnConnectionStateChanged);
				base.PopulateLevel2ToTitle();
			} catch (Exception ex) {
				string msg = "IS_DATASOURCE_EDITOR_NULL? this.quikStreaming.DataSourceEditor[" + this.ddeTableDepth.QuikStreaming.DataSourceEditor + "] //QuikLevel2Control.OnControlAdded()";
				Assembler.PopupException(msg, ex);
			}
		}
		public void MyTableDepth_unsubscribe() {
			try {
				this.ddeTableDepth.QuikStreaming.OnConnectionStateChanged	-= new EventHandler<EventArgs>(this.quikStreaming_OnConnectionStateChanged);
				this.ddeTableDepth.OnDataStructureParsed_One				-= new EventHandler<XlDdeTableMonitoringEventArg<LevelTwo>>			(this.ddeTableDepth_OnDataStructureParsed_One);
				this.ddeTableDepth.OnDataStructuresParsed_Table				-= new EventHandler<XlDdeTableMonitoringEventArg<List<LevelTwo>>>	(this.ddeTableDepth_OnDataStructuresParsed_Table_butAlwaysOneElementInList);
			} catch (Exception ex) {
				string msg = "IS_DATASOURCE_EDITOR_NULL? this.quikStreaming.DataSourceEditor[" + this.ddeTableDepth.QuikStreaming.DataSourceEditor + "] //QuikLevel2Control.OnControlRemoved()";
				Assembler.PopupException(msg, ex);
			}
		}
	}
}
