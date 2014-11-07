using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using Newtonsoft.Json;
using Sq1.Core.DataFeed;
using Sq1.Core.DataTypes;
using Sq1.Core.Streaming;

namespace Sq1.Core.Static {
	public class StaticProvider {
		[JsonIgnore]	public string Name { get; protected set; }
		[JsonIgnore]	public string Description { get; protected set; }
		[JsonIgnore]	public Bitmap Icon { get; protected set; }

		[JsonIgnore]	public bool UserAllowedToModifySymbols { get; protected set; }
		[JsonIgnore]	public List<string> SymbolsStored;
		[JsonIgnore]	private string RootFolder;
		[JsonIgnore]	public string PreferredDataSourceName { get; protected set; }
		[JsonIgnore]	public string PreferredStreamingProviderTypeName { get; protected set; }
		[JsonIgnore]	public string PreferredBrokerProviderTypeName { get; protected set; }
		[JsonIgnore]	public DataSource DataSource { get; protected set; }
		[JsonIgnore]	public virtual string SymbolsStoredCommaSeparated { get {
				string ret = "";
				foreach (string symbol in this.SymbolsStored) ret += symbol + ",";
				ret = ret.TrimEnd(',');
				return ret;
			} }
		public StaticProvider() : base() {
			SymbolsStored = new List<string>();
			PreferredDataSourceName = "StaticProvider_PreferredDataSourceName_NOT_SET";
			PreferredStreamingProviderTypeName = "StaticProvider_PreferredStreamingProviderName_NOT_SET";
			PreferredBrokerProviderTypeName = "StaticProvider_PreferredBrokerProviderName_NOT_SET";
		}
		public virtual void Initialize(DataSource dataSource, string rootPath) {
			this.InitializeWithBarsFolder(dataSource, rootPath);
		}
		protected void InitializeWithBarsFolder(DataSource dataSource, string rootPath) {
			this.DataSource = dataSource;
			this.RootFolder = Path.Combine(rootPath, this.Name);
		}

		public override string ToString() {
			return this.Name;
		}
	}
}
