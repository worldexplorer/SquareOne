using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Sq1.Charting.Properties {
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0"), DebuggerNonUserCode, CompilerGenerated]
	internal class Resources {
		private static ResourceManager resourceManager_0;
		private static CultureInfo cultureInfo_0;
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager {
			get {
				if (object.ReferenceEquals(Resources.resourceManager_0, null)) {
					ResourceManager resourceManager = new ResourceManager("Sq1.Charting.Properties.Resources", typeof(Resources).Assembly);
					Resources.resourceManager_0 = resourceManager;
				}
				return Resources.resourceManager_0;
			}
		}
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture {
			get {
				return Resources.cultureInfo_0;
			}
			set {
				Resources.cultureInfo_0 = value;
			}
		}

		internal Resources() {
		}
		internal static Bitmap LongEntryUnknown {
			get {
				object @object = Resources.ResourceManager.GetObject("45degrees3-LongEntryUnknown", Resources.cultureInfo_0);
				return (Bitmap)@object;
			}
		}
		internal static Bitmap LongEntryProfit {
			get {
				object @object = Resources.ResourceManager.GetObject("45degrees3-LongEntryProfit", Resources.cultureInfo_0);
				return (Bitmap)@object;
			}
		}
		internal static Bitmap LongEntryLoss {
			get {
				object @object = Resources.ResourceManager.GetObject("45degrees3-LongEntryLoss", Resources.cultureInfo_0);
				return (Bitmap)@object;
			}
		}
		internal static Bitmap LongExitUnknown {
			get {
				object @object = Resources.ResourceManager.GetObject("45degrees3-LongExitUnknown", Resources.cultureInfo_0);
				return (Bitmap)@object;
			}
		}
		internal static Bitmap LongExitProfit {
			get {
				object @object = Resources.ResourceManager.GetObject("45degrees3-LongExitProfit", Resources.cultureInfo_0);
				return (Bitmap)@object;
			}
		}
		internal static Bitmap LongExitLoss {
			get {
				object @object = Resources.ResourceManager.GetObject("45degrees3-LongExitLoss", Resources.cultureInfo_0);
				return (Bitmap)@object;
			}
		}
		internal static Bitmap ShortEntryUnknown {
			get {
				object @object = Resources.ResourceManager.GetObject("45degrees3-ShortEntryUnknown", Resources.cultureInfo_0);
				return (Bitmap)@object;
			}
		}
		internal static Bitmap ShortEntryProfit {
			get {
				object @object = Resources.ResourceManager.GetObject("45degrees3-ShortEntryProfit", Resources.cultureInfo_0);
				return (Bitmap)@object;
			}
		}
		internal static Bitmap ShortEntryLoss {
			get {
				object @object = Resources.ResourceManager.GetObject("45degrees3-ShortEntryLoss", Resources.cultureInfo_0);
				return (Bitmap)@object;
			}
		}
		internal static Bitmap ShortExitUnknown {
			get {
				object @object = Resources.ResourceManager.GetObject("45degrees3-ShortExitUnknown", Resources.cultureInfo_0);
				return (Bitmap)@object;
			}
		}
		internal static Bitmap ShortExitProfit {
			get {
				object @object = Resources.ResourceManager.GetObject("45degrees3-ShortExitProfit", Resources.cultureInfo_0);
				return (Bitmap)@object;
			}
		}
		internal static Bitmap ShortExitLoss {
			get {
				object @object = Resources.ResourceManager.GetObject("45degrees3-ShortExitLoss", Resources.cultureInfo_0);
				return (Bitmap)@object;
			}
		}
	}
}
