using System;

namespace Sq1.Core.Optimization {
	public class SystemPerformanceRestoreAbleEventArgs : EventArgs {
		public SystemPerformanceRestoreAble SystemPerformanceRestoreAble	{get; private set;}
		public string						ScriptContextNewName			{get; private set;}
		
		public SystemPerformanceRestoreAbleEventArgs(SystemPerformanceRestoreAble systemPerformanceRestoreAble, string scriptContextNewName = null) {
			SystemPerformanceRestoreAble	= systemPerformanceRestoreAble;
			ScriptContextNewName			= scriptContextNewName;
		}

	}
}
