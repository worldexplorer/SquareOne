using System.Collections.Generic;

using Newtonsoft.Json;

namespace Sq1.Core.Execution {
	public partial class Order {
		[JsonProperty]	public	int				SlippageAppliedIndex;		// SET_IN_POSTPROCESSOR_EMERGENCY	{ get; protected set; }
		[JsonProperty]	public	double			SlippageApplied;			// SET_IN_POSTPROCESSOR_EMERGENCY	{ get; protected set; }
		[JsonProperty]	public	double			SlippageFilled;				// SET_IN_POSTPROCESSOR_EMERGENCY	{ get; protected set; }
		[JsonProperty]	public	double			SlippageFilledMinusApplied	{ get {
				if (this.SlippageApplied	== 0) return 0;
				if (this.PriceFilled		== 0) return 0;
				if (this.PriceEmitted		== 0) return 0;
				double slippageFilled = this.PriceFilled - this.PriceEmitted;
				return slippageFilled - this.SlippageApplied;
		} }

		[JsonIgnore]	public	List<double>	Slippages { get {
				string msg1 = "[JsonIgnore]	to let orders restored after app restart fly over it; they don't have alert.Bars restored yet";
				if (this.Alert == null) {
					string msg = "PROBLEMATIC_Order.Alert=NULL_hasSlippagesDefined";
					Assembler.PopupException(msg);
				}
				if (this.Alert.Bars == null) {
					string msg = "ORDERS_RESTORED_AFTER_APP_RESTART_HAVE_ALERT.BARS=NULL__ADDED_[JsonIgnore]; //Order.hasSlippagesDefined";
					Assembler.PopupException(msg);
				}
				if (this.Alert.Bars.SymbolInfo == null) {
					string msg = "PROBLEMATIC_Order.Alert.Bars.SymbolInfo=NULL_hasSlippagesDefined";
					Assembler.PopupException(msg);
				}
				List<double> ret = this.Alert.Slippages_forLimitOrdersOnly;
				return ret;
			} }
		[JsonIgnore]	public	string			Slippages_asCSV { get {
				return string.Join(",", this.Slippages);
			} }
		[JsonIgnore]	public	int				SlippagesIndexMax { get {
				return this.Slippages.Count - 1;
			} }
		[JsonIgnore]	public	bool			HasSlippagesDefined { get {				
				return this.SlippagesIndexMax != -1;
			} }
		[JsonIgnore]	public	int				SlippagesLeftAvailable { get {
				string msg1 = "[JsonIgnore]	to let orders restored after app restart fly over it; they don't have alert.Bars restored yet";
				if (this.Alert == null) {
					string msg = "PROBLEMATIC_Order.Alert=NULL_noMoreSlippagesAvailable";
					Assembler.PopupException(msg);
				}
				if (this.Alert.Bars == null) {
					string msg = "ORDERS_RESTORED_AFTER_APP_RESTART_HAVE_ALERT.BARS=NULL__ADDED_[JsonIgnore] //Order.noMoreSlippagesAvailable";
					Assembler.PopupException(msg);
				}
				if (this.Alert.Bars.SymbolInfo == null) {
					string msg = "PROBLEMATIC_Order.Alert.Bars.SymbolInfo=NULL_noMoreSlippagesAvailable";
					Assembler.PopupException(msg);
				}
				string msg2 = "slippagesNotDefinedOr?";
				int slippageIndexMax = this.Alert.Slippage_maxIndex_forLimitOrdersOnly;
				if (slippageIndexMax == -1) return slippageIndexMax;
				int ret = slippageIndexMax - this.SlippageAppliedIndex;
				return ret;
			}}
		[JsonIgnore]	public	bool			SlippagesLeftAvailable_noMore { get {
				int slippagesLeftAvailable = this.SlippagesLeftAvailable;
				bool noMoreSlippagesLeft = slippagesLeftAvailable <= 0;	// last slippage is still a valid slippage; 0 is okay
				return noMoreSlippagesLeft;
			} }
		[JsonIgnore]	public	double			SlippageNextAvailable_forLimitAlertsOnly_NanWhenNoMore { get {
			return this.Alert.GetSlippage_signAware_forLimitAlertsOnly_NanWhenNoMore(this.SlippageAppliedIndex + 1, true);
		} }
	}
}