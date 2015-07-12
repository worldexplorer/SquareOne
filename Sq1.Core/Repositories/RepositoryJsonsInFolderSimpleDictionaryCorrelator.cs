using System;
using System.IO;
using System.Collections.Generic;

using Sq1.Core.Correlation;
using Sq1.Core.Sequencing;

namespace Sq1.Core.Repositories {
	public class RepositoryJsonCorrelator
			: RepositoryJsonsInFolderSimpleDictionary<OneParameterAllValuesAveraged> {

		public RepositoryJsonCorrelator() : base()	{
		}

		string symbolScaleRange;
		public void Initialize(string rootPath, string subfolder, string fileName) {
			if (fileName == SequencedBacktests.SEQUENCED_BACKTESTS_NO_FNAME__REINITIALIZE_REPOSITORY) {
				Assembler.PopupException(fileName);
			}
			base.Initialize(rootPath, subfolder);
			if (fileName != Path.GetFileName(fileName)) {
				string msg = "DONT_FEED_ME_WITH_ABSOLUTE_FILENAME__IT_CONTAINS_SEQUENCER_WHILE_I_AM_A_CORRELATOR fileName[" + fileName + "]";
				Assembler.PopupException(msg, null, false);
			}
			this.symbolScaleRange = fileName;
		}
	
		public Dictionary<string, OneParameterAllValuesAveraged> DeserializeSortedDictionary() {
 			return base.DeserializeDictionary(this.symbolScaleRange);
		}
		public void SerializeSortedDictionary(Dictionary<string, OneParameterAllValuesAveraged> paramsByName) {
			base.SerializeDictionary(paramsByName, this.symbolScaleRange);
		}
	}
}
