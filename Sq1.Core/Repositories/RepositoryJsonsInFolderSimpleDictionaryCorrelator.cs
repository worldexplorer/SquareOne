using System;
using System.Collections.Generic;

using Sq1.Core.Correlation;

namespace Sq1.Core.Repositories {
	public class RepositoryJsonCorrelator
			: RepositoryJsonsInFolderSimpleDictionary<OneParameterAllValuesAveraged> {

		public RepositoryJsonCorrelator() : base()	{
		}

		string symbolScaleRange;
		public void Initialize(string rootPath, string subfolder, string fileName) {
			base.Initialize(rootPath, subfolder);
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
