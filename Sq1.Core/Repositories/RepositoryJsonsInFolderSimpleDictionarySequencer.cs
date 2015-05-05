using System;

using Sq1.Core.Sequencing;

namespace Sq1.Core.Repositories {
	public class RepositoryJsonsInFolderSimpleDictionarySequencer
			: RepositoryJsonsInFolderSimple<SequencedBacktests> {

		public RepositoryJsonsInFolderSimpleDictionarySequencer() : base()	{
		}

		public override SequencedBacktests DeserializeSingle(string fname) {
			SequencedBacktests ret = base.DeserializeSingle(fname);
			if (ret == null) {
				string msg = "NOT_CHECKING_FOR_REWIDING_KPIs_KOZ_DeserializeSingle(" + fname + ")=null";
				Assembler.PopupException(msg);
			} else {
				ret.CheckPositionsCountMustIncreaseOnly();
			}
			return ret;
		}

	}
}
