using System;
using System.Collections.Generic;
using System.Drawing;
using System.ComponentModel;
using System.Reflection;

using Newtonsoft.Json;

using Sq1.Core.DataTypes;
using Sq1.Core.Charting;

namespace Sq1.Core.Charting {
	public class ChartSettingsIndividual : NamedObjectJsonSerializable {
		[Browsable(false)]
		[JsonIgnore]	public static string NAME_DEFAULT = "DONT_FORGET_TO_PROPAGATE_CHART_WINDOW_TITLE_TO_INDIVIDUAL_SETTINGS";

		[JsonProperty]	public int		ScrollPositionAtBarIndex;
		[JsonProperty]	public int		BarWidthIncludingPadding;
		[JsonProperty]	public int		SqueezeVerticalPaddingPx;

		[JsonIgnore]	public int BarPaddingRight { get {
			if (this.BarWidthIncludingPadding <= 3) return 0;
			//int nominal = (int) (this.BarWidthIncludingPadding * 0.25F);
			int nominal = 1;
			// algo below allows you have this.BarTotalWidthPx both odd and even automatically
			//int compensated = nominal;
			//int keepWidthOdd = this.BarWidthIncludingPadding - compensated;
			//if (keepWidthOdd % 2 == 0) compensated++;	// increase padding to have 1px shadows right in the middle of a bar
			//return compensated;
			return nominal;
		} }
		[JsonIgnore]	public int BarWidthMinusRightPadding { get { return this.BarWidthIncludingPadding - this.BarPaddingRight; } }
		[JsonIgnore]	public int BarShadowXoffset { get { return this.BarWidthMinusRightPadding / 2; } }
		

		[JsonProperty]	public Dictionary<string, MultiSplitterProperties> MultiSplitterRowsPropertiesByPanelName;
		[JsonProperty]	public Dictionary<string, MultiSplitterProperties> MultiSplitterColumnsPropertiesByPanelName;
	
		public ChartSettingsIndividual() {
			base.Name = ChartSettingsIndividual.NAME_DEFAULT;

			BarWidthIncludingPadding = 8;
			SqueezeVerticalPaddingPx = 0;

			MultiSplitterRowsPropertiesByPanelName		= new Dictionary<string, MultiSplitterProperties>();
			MultiSplitterColumnsPropertiesByPanelName	= new Dictionary<string, MultiSplitterProperties>();
		}

		//public ChartSettingsIndividual(string name) : this() {
		//    base.Name = name;
		//}



		public override string ToString() {
			return "ChartSettingsIndividual_FOR_CHART " + this.Name;
		}

		public virtual ChartSettingsIndividual Clone() {
			return (ChartSettingsIndividual)base.MemberwiseClone();
		}

	}
}