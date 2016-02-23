using System;
using System.Collections.Generic;

using Sq1.Core;
using Sq1.Core.Sequencing;

namespace Sq1.Widgets.Sequencing {
	public partial class SequencerControl {
		public event EventHandler<SystemPerformanceRestoreAbleEventArgs> OnCopyToContextDefault;
		public event EventHandler<SystemPerformanceRestoreAbleEventArgs> OnCopyToContextDefaultBacktest;
		public event EventHandler<SystemPerformanceRestoreAbleEventArgs> OnCopyToContextNew;
		public event EventHandler<SystemPerformanceRestoreAbleEventArgs> OnCopyToContextNewBacktest;

		public event EventHandler<SequencedBacktestsEventArgs> OnCorrelatorShouldPopulate;

		void raiseOnCopyToContextDefault(SystemPerformanceRestoreAble scriptAndParametersHolder) {
			if (this.OnCopyToContextDefault == null) return;
			try {
				this.OnCopyToContextDefault(this, new SystemPerformanceRestoreAbleEventArgs(scriptAndParametersHolder));
			} catch (Exception ex) {
				Assembler.PopupException("RaiseOnCopyToContextDefault(" + scriptAndParametersHolder + ")", ex);
			}
		}
		void raiseOnCopyToContextDefaultBacktest(SystemPerformanceRestoreAble scriptAndParametersHolder) {
			if (this.OnCopyToContextDefaultBacktest == null) return;
			try {
				this.OnCopyToContextDefaultBacktest(this, new SystemPerformanceRestoreAbleEventArgs(scriptAndParametersHolder));
			} catch (Exception ex) {
				Assembler.PopupException("RaiseOnCopyToContextDefaultBacktest(" + scriptAndParametersHolder + ")", ex);
			}
		}
		void raiseOnCopyToContextNew(SystemPerformanceRestoreAble scriptAndParametersHolder, string scriptContextNewName) {
			if (this.OnCopyToContextNew == null) return;
			try {
				this.OnCopyToContextNew(this, new SystemPerformanceRestoreAbleEventArgs(scriptAndParametersHolder, scriptContextNewName));
			} catch (Exception ex) {
				Assembler.PopupException("RaiseOnCopyToContextNew(" + scriptAndParametersHolder + ")", ex);
			}
		}
		void raiseOnCopyToContextNewBacktest(SystemPerformanceRestoreAble scriptAndParametersHolder, string scriptContextNewName) {
			if (this.OnCopyToContextNewBacktest == null) return;
			try {
				this.OnCopyToContextNewBacktest(this, new SystemPerformanceRestoreAbleEventArgs(scriptAndParametersHolder, scriptContextNewName));
			} catch (Exception ex) {
				Assembler.PopupException("RaiseOnCopyToContextNewBacktest(" + scriptAndParametersHolder + ")", ex);
			}
		}
		void raiseOnCorrelatorShouldPopulate(SequencedBacktests deserialized) {
			if (this.OnCorrelatorShouldPopulate == null) return;
			try {
				this.olvBacktests.UseWaitCursor = true;
				var eventArg = new SequencedBacktestsEventArgs(deserialized);
				this.OnCorrelatorShouldPopulate(this, eventArg);
			} catch (Exception ex) {
				Assembler.PopupException("RaiseOnCorrelatorShouldOpen(" + deserialized.FileName + ":" + deserialized.Count + "deserialized)", ex);
			} finally {
				this.olvBacktests.UseWaitCursor = false;
			}
		}
		public void RaiseOnCorrelatorShouldPopulate_usedByMainFormAfterBothAreInstantiated() {
			if (this.backtestsLocalEasierToSync == null) {
				string msg = "I_REFUSE_TO_RAISE_WITH_NULL_DESERIALIZED //RaiseOnCorrelatorShouldPopulate()";
				Assembler.PopupException(msg, null, false);
				return;
			}
			this.raiseOnCorrelatorShouldPopulate(this.backtestsLocalEasierToSync);
		}
	}
}
