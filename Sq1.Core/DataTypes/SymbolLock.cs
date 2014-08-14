using System.Collections.Generic;
using System.Threading;
namespace Sq1.Core.DataTypes {
	public class SymbolLock {
		List<string> symbols = new List<string>();
		public void LockSymbol(string symbol) {
			bool containsSymbol = true;
			while (containsSymbol) {
				lock(this.symbols) {
				//using (DdMonitor.Lock(this.symbols)) {
					containsSymbol = this.symbols.Contains(symbol);
					if (containsSymbol == false) {
						break;
					}
				}
				Thread.Sleep(10);
			}
			lock (this.symbols) {
			//using (DdMonitor.Lock(this.symbols)) {
				if (!this.symbols.Contains(symbol)) {
					this.symbols.Add(symbol);
				}
			}
		}
		public void UnlockSymbol(string symbol) {
			lock (this.symbols) {
			//using (DdMonitor.Lock(this.symbols)) {
				while (this.symbols.Contains(symbol)) {
					this.symbols.Remove(symbol);
				}
			}
		}
	}
}
