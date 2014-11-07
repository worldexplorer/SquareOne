using System;
using System.Drawing;
using System.Runtime.Serialization;
using System.Windows.Forms;

using Sq1.Core.DataFeed;
using Sq1.Core.DataTypes;
using Sq1.Core.Static;

namespace Sq1.Adapters.Quik {
	public class StaticQuik : StaticProvider {
		public StaticQuik() : base () {
			base.Name = "Quik StaticDummy";
			base.Description = "QuikStatic DOESNT_GET_STATIC_HISTORICAL_BARS_FROM_QUIK_YET";
			//base.Icon = (Bitmap)QuikAdapter.Properties.Resources.imgQuikStreamingProvider;
			base.PreferredDataSourceName = "Quik";
			base.PreferredStreamingProviderTypeName = "QuikStreamingProvider";
			base.PreferredBrokerProviderTypeName = "BrokerQuik";
			base.UserAllowedToModifySymbols = true;
		}
		public override void Initialize(DataSource dataSource, string folderForBarDataStore) {
			base.Name = "Quik StaticProvider";
			this.InitializeWithBarsFolder(dataSource, folderForBarDataStore);
		}
	}
}