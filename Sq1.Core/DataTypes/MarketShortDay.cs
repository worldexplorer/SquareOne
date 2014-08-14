using System;
using System.Runtime.Serialization;

namespace Sq1.Core.DataTypes {
	[DataContract]
	public class MarketShortDay {
		[DataMember]
		public DateTime Date;
		[DataMember]
		public DateTime ServerTimeOpening;
		[DataMember]
		public DateTime ServerTimeClosing;
	}
}
