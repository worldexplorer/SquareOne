using System.Drawing;
using System.Runtime.Serialization;
using Sq1.Core.DataFeed;

namespace Sq1.QuikAdapter {
	[DataContract]
	//[KnownType(typeof(QuikStaticProvider))]
	public class MockStaticProvider : QuikStaticProvider {
		public MockStaticProvider() : base() {
			base.Name = "Mock StaticDummy";
			base.Description = "MockStatic Automatically stores QuikStreamingMock-generated quotes, and QuikStreamingMOCK will only start for QuikStaticMOCK datasources";
			base.Icon = (Bitmap)QuikAdapter.Properties.Resources.imgMockStaticProvider;
			base.PreferredDataSourceName = "Mock";
			base.PreferredStreamingProviderTypeName = "MockStreamingProvider";
			base.PreferredBrokerProviderTypeName = "MockBrokerProvider";
			base.UserAllowedToModifySymbols = true;
		}
		public override void Initialize(DataSource dataSource, string folderForBarDataStore) {
			base.Name = "Mock StaticProvider";
			this.InitializeWithBarsFolder(dataSource, folderForBarDataStore);
		}
	}
}