using System;

namespace Sq1.Core.Charting {
	public interface HostPanelForIndicator {
		int BarShadowOffset { get; }
		int BarToX(int barVisible);
		//int XToBar(int xMouseOver);
		int ValueToYinverted(double volume);
		//double YinvertedToValue(int yMouseInverted);
	}
}
