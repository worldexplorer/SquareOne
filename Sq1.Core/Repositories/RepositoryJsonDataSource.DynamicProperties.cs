using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Sq1.Core.Accounting;
using Sq1.Core.DataFeed;

namespace Sq1.Core.Repositories {
	// the hackiest class in the whole solution :(
	public partial class RepositoryJsonDataSource {
		public List<Account> AccountsFromUnderlyingBrokerAdapters { get {
				List<Account> ret = new List<Account>();
				foreach (DataSource ds in base.ItemsAsList) {
					if (ds.BrokerAdapter == null) continue;
					if (ds.BrokerAdapter.AccountAutoPropagate == null) continue;
					if (ret.Contains(ds.BrokerAdapter.AccountAutoPropagate)) continue;
					ret.Add(ds.BrokerAdapter.AccountAutoPropagate);
				}
				return ret;
			} }
		public string AccountsFromUnderlyingBrokerAdaptersAsString { get {
				string ret = "";
				foreach (Account account in AccountsFromUnderlyingBrokerAdapters) {
					ret += "[" + account.ToString() + "] ";
				}
				ret.TrimEnd(' ');
				return ret;
			} }
		public ToolStripMenuItem[] CtxAccountsAllCheckedFromUnderlyingBrokerAdapters { get {
				List<Account> accts = this.AccountsFromUnderlyingBrokerAdapters;
				var ret = new List<ToolStripMenuItem>();
				foreach (Account acct in accts) {
					ToolStripMenuItem mni = new ToolStripMenuItem();
					mni.Checked = true;
					mni.Name = "mniAccount_" + acct.AccountNumber;
					mni.Text = acct.AccountNumber;
					if (acct.BrokerAdapter != null) {
						mni.Image = acct.BrokerAdapter.Icon;
					}
					//mni.Size = new System.Drawing.Size(236, 22);
					//mni.Click += new System.EventHandler(this.btnEdit_Click);
					ret.Add(mni);
				}
				return ret.ToArray();
			} }
		public List<string> AccountNumbersFromUnderlyingBrokerAdapters { get {
				List<string> ret = new List<string>();
				foreach (Account account in this.AccountsFromUnderlyingBrokerAdapters) {
					if (ret.Contains(account.AccountNumber)) continue;
					ret.Add(account.AccountNumber);
				}
				return ret;
			} }
		public Account FindAccount(string accountNumber) {
			foreach (Account current in this.AccountsFromUnderlyingBrokerAdapters) {
				if (current.AccountNumber == accountNumber) return current;
			}
			return null;
		}
	}
}