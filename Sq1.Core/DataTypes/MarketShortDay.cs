using System;
using Newtonsoft.Json;

namespace Sq1.Core.DataTypes {
	public class MarketShortDay {
		[JsonProperty]	public DateTime Date;
		[JsonProperty]	public DateTime ServerTimeOpening;
		[JsonProperty]	public DateTime ServerTimeClosing;
	}
}
