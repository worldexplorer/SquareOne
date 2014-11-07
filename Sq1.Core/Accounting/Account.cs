using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using Sq1.Core.Broker;

namespace Sq1.Core.Accounting {
	public class Account {
		[JsonProperty]	public string AccountNumber;
		[JsonProperty]	public double CashAvailable;
		[JsonProperty]	public DateTime AccountValueTimeStamp;
		[JsonIgnore]	public bool IsDdeAccount;
		[JsonIgnore]	public BrokerProvider BrokerProvider { get; private set; }
		
		public Account() {
			AccountNumber = "";
			AccountValueTimeStamp = DateTime.MinValue;
		}
		public Account(string accountNumber, double cashAvailable) : this() {
			AccountNumber = accountNumber;
			CashAvailable = cashAvailable;
		}
		public void Initialize(BrokerProvider brokerProvider) {
			this.BrokerProvider = brokerProvider;
		}
		public override string ToString() {
			return "AccountNumber[" + AccountNumber + "] "
				+ " CashAvailable[" + CashAvailable + "]"
				+ " IsDde[" + IsDdeAccount + "]";
		}
	}
}