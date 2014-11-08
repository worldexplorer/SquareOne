using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Sq1.Core.Accounting;
using Sq1.Core.DataFeed;

namespace Sq1.Core.Repositories {
	// the hackiest class in the whole solution :(
	public partial class RepositoryJsonDataSource {
		public List<Account> AccountsFromUnderlyingBrokerProviders { get {
				List<Account> ret = new List<Account>();
				foreach (DataSource ds in base.ItemsAsList) {
					if (ds.BrokerProvider == null) continue;
					if (ds.BrokerProvider.AccountAutoPropagate == null) continue;
					if (ret.Contains(ds.BrokerProvider.AccountAutoPropagate)) continue;
					ret.Add(ds.BrokerProvider.AccountAutoPropagate);
				}
				return ret;
			} }
		public string AccountsFromUnderlyingBrokerProvidersAsString { get {
				string ret = "";
				foreach (Account account in AccountsFromUnderlyingBrokerProviders) {
					ret += "[" + account.ToString() + "] ";
				}
				ret.TrimEnd(' ');
				return ret;
			} }
		public ToolStripMenuItem[] CtxAccountsAllCheckedFromUnderlyingBrokerProviders { get {
				List<Account> accts = this.AccountsFromUnderlyingBrokerProviders;
				var ret = new List<ToolStripMenuItem>();
				foreach (Account acct in accts) {
					ToolStripMenuItem mni = new ToolStripMenuItem();
					mni.Checked = true;
					mni.Name = "mniAccount_" + acct.AccountNumber;
					mni.Text = acct.AccountNumber;
					if (acct.BrokerProvider != null) {
						mni.Image = acct.BrokerProvider.Icon;
					}
					//mni.Size = new System.Drawing.Size(236, 22);
					//mni.Click += new System.EventHandler(this.btnEdit_Click);
					ret.Add(mni);
				}
				return ret.ToArray();
			} }
		public List<string> AccountNumbersFromUnderlyingBrokerProviders { get {
				List<string> ret = new List<string>();
				foreach (Account account in this.AccountsFromUnderlyingBrokerProviders) {
					if (ret.Contains(account.AccountNumber)) continue;
					ret.Add(account.AccountNumber);
				}
				return ret;
			} }
		public Account FindAccount(string accountNumber) {
			foreach (Account current in this.AccountsFromUnderlyingBrokerProviders) {
				if (current.AccountNumber == accountNumber) return current;
			}
			return null;
		}
	}
}