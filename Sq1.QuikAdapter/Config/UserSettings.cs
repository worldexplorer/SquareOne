//   UserSettings.cs (c) 2012 Nikolay Moroshkin, http://www.moroshkin.com/
namespace Sq1.QuikAdapter {
	public class UserSettings35 {
		public int PriceRatio = 1;
		public int PriceStep = 5;
		public UserSettings35 Clone() {
			UserSettings35 u = (UserSettings35)MemberwiseClone();
			return u;
		}
	}
}
