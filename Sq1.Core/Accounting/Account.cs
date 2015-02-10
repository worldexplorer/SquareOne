using System;

using Newtonsoft.Json;
using Sq1.Core.Broker;

namespace Sq1.Core.Accounting {
	public class Account {
		[JsonProperty]	public string AccountNumber;
		[JsonProperty]	public double CashAvailable;
		[JsonProperty]	public DateTime AccountValueTimeStamp;
		[JsonIgnore]	public bool IsDdeAccount;
		[JsonIgnore]	public BrokerAdapter BrokerAdapter { get; private set; }
		
		public Account() {
			AccountNumber = "";
			AccountValueTimeStamp = DateTime.MinValue;
		}
		public Account(string accountNumber, double cashAvailable) : this() {
			AccountNumber = accountNumber;
			CashAvailable = cashAvailable;
		}
		public void Initialize(BrokerAdapter brokerAdapter) {
			this.BrokerAdapter = brokerAdapter;
		}
		public override string ToString() {
			return "AccountNumber[" + AccountNumber + "] "
				+ " CashAvailable[" + CashAvailable + "]"
				+ " IsDde[" + IsDdeAccount + "]";
		}
	}
}