using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Sq1.Core.Broker;

namespace Sq1.Core.Accounting {
	[DataContract]
	public class Account {
		[DataMember] public string AccountNumber;
		[DataMember] public double CashAvailable;
		[DataMember] public DateTime AccountValueTimeStamp;
		public bool IsDdeAccount;
		public BrokerProvider BrokerProvider { get; private set; }
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