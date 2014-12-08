using System;

namespace Sq1.Core.Support {
	public static class ObjectListViewStateSerializer {
		//http://stackoverflow.com/questions/11743160/how-do-i-encode-and-decode-a-base64-string
		//http://stackoverflow.com/questions/15992573/c-sharp-convert-binary-to-text-and-then-search-it
		//http://objectlistview.sourceforge.net/cs/features.html#save-and-restore-state
		public static string Base64Encode(byte[] stateInBinary) {
			//var plainTextBytes = System.Text.Encoding.ASCII.GetBytes(plainText);
			return System.Convert.ToBase64String(stateInBinary);
		}
		public static byte[]  Base64Decode(string base64EncodedString) {
		byte[] decodedBytes = System.Convert.FromBase64String(base64EncodedString);
			//return System.Text.Encoding.UTF8.GetString(decodedBytes);
			return decodedBytes;
		}
	}
}
