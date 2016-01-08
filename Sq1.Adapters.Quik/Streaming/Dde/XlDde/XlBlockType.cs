using System;

namespace Sq1.Adapters.Quik.Dde.XlDde {
	public enum XlBlockType {
		Float	= 0x01,
		String	= 0x02,
		Bool	= 0x03,
		Error	= 0x04,
		Blank	= 0x05,
		Int		= 0x06,
		Skip	= 0x07,
		Table	= 0x10,
	}
}
