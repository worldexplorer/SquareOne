using System;
using System.IO;

using Sq1.Core.Sequencing;

namespace Sq1.Core.Repositories {
	public class RepositoryJsonsInFolderSimpleDictionarySequencer
			: RepositoryJsonsInFolderSimple<SequencedBacktests> {

		public RepositoryJsonsInFolderSimpleDictionarySequencer() : base()	{
		}


		public void SerializeSingle(SequencedBacktests backtests) {
			if (Assembler.InstanceInitialized.MainFormDockFormsFullyDeserializedLayoutComplete == false) return;
			if (backtests.ProfitFactorAverage == 0) {
				backtests.CalculateProfitFactorAverage();
			}
			string fnameWithPFappended = FnameDateSizeColorPFavg.AppendProfitFactorAverage(
				backtests.SymbolScaleIntervalDataRange, backtests.ProfitFactorAverage);
			this.SerializeSingle(backtests, fnameWithPFappended);
		}
		public override void SerializeSingle(SequencedBacktests backtests, string jsonRelname) {
			string jsonRelnameForSaving = jsonRelname;
			jsonRelnameForSaving += base.Extension;
			string jsonAbsname = Path.Combine(base.AbsPath, jsonRelname);
			backtests.FileName = jsonAbsname;

			base.SerializeSingle(backtests, jsonRelname);
		}
		public override SequencedBacktests DeserializeSingle(string fname) {
			SequencedBacktests ret = base.DeserializeSingle(fname);
			if (ret == null) {
				string msg = "NOT_CHECKING_FOR_REWIDING_KPIs_KOZ_DeserializeSingle(" + fname + ")=null";
				Assembler.PopupException(msg);
			} else {
				//ret.CheckPositionsCountMustIncreaseOnly();
			}
			return ret;
		}

	}
}
