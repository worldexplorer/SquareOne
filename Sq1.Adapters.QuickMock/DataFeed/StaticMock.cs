using System.Drawing;
using Sq1.Adapters.Quik;
using Sq1.Core.DataFeed;

namespace Sq1.Adapters.QuikMock {
	//[KnownType(typeof(QuikStaticProvider))]
	public class StaticMock : StaticQuik {
		public StaticMock() : base() {
			base.Name = "Mock StaticDummy";
			base.Description = "MockStatic Automatically stores QuikStreamingMock-generated quotes, and QuikStreamingMOCK will only start for QuikStaticMOCK datasources";
			//base.Icon = (Bitmap)Sq1.Adapters.QuikMock.Properties.Resources.imgMockQuikStaticProvider;
			base.PreferredDataSourceName = "Mock";
			base.PreferredStreamingProviderTypeName = "StreamingMock";
			base.PreferredBrokerProviderTypeName = "BrokerMock";
			base.UserAllowedToModifySymbols = true;
		}
		public override void Initialize(DataSource dataSource, string folderForBarDataStore) {
			base.Name = "Mock StaticProvider";
			this.InitializeWithBarsFolder(dataSource, folderForBarDataStore);
		}
	}
}