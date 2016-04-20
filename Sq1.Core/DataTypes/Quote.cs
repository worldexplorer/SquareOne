using System;
using System.Text;

using Newtonsoft.Json;

using Sq1.Core.Backtesting;

namespace Sq1.Core.DataTypes {
	public class Quote {
		[JsonIgnore]	public	const string	GENERATED_TO_FILL_ALERT = "GENERATED_TO_FILL_ALERT";

		[JsonProperty]	public	string		Symbol;
		[JsonProperty]	public	string		SymbolClass;
		[JsonProperty]	public	string		Source;
		[JsonProperty]	public	DateTime	ServerTime;
		[JsonProperty]	public	DateTime	LocalTime				{ get; protected set; }

		[JsonProperty]	public	double		Bid;
		[JsonProperty]	public	double		Ask;
		[JsonProperty]	public	double		Size;
		
		[JsonIgnore]	public	int			IntraBarSerno			{ get; private set; }
		[JsonIgnore]	public	bool		IamInjectedToFillPendingAlerts;	//		{ get; protected set; }
		[JsonProperty]	public	long		AbsnoPerSymbol;

		[JsonIgnore]	public	Bar			ParentBarStreaming		{ get; protected set; }
		[JsonIgnore]	public	bool		HasParentBarStreaming	{ get { return this.ParentBarStreaming != null; } }
		[JsonProperty]	public	string		ParentBarIdent			{ get { return (this.HasParentBarStreaming) ? this.ParentBarStreaming.ParentBarsIdent : "NO_PARENT_BAR"; } }

		[Obsolete("NOT_REALLY_USED")]
		[JsonIgnore]	public	BidOrAsk	ItriggeredFillAtBidOrAsk;
		[JsonProperty]	public	BidOrAsk	TradedAt;
		[JsonProperty]	public	double		TradedPrice				{ get {
				if (this.TradedAt == BidOrAsk.UNKNOWN) return this.Median_forBarOpen_fromLevel2;		// CAUSED CANT_FILL_STREAMING_CLOSE_FROM_BID_OR_ASK_UNKNOWN double.NaN;
				return (this.TradedAt == BidOrAsk.Bid) ? this.Bid : this.Ask;
			} }
		[JsonProperty]	public	double		Spread					{ get { return this.Ask - this.Bid; } }
		[JsonIgnore]	public	double		Median_forBarOpen_fromLevel2				{ get { return this.Bid - this.Spread / 2d; } }

		#region long story short
		[JsonIgnore]			SymbolInfo	symbolInfo_nullUnsafe					{ get {
			SymbolInfo ret = this.symbolInfo_fromParentBars_nullUnsafe;
			if (ret == null) {
				ret = Assembler.InstanceInitialized.RepositorySymbolInfos.FindSymbolInfo_nullUnsafe(this.Symbol);
			}
			return ret;
		} }
		[JsonIgnore]			SymbolInfo	symbolInfo_fromParentBars_nullUnsafe	{ get {
			SymbolInfo ret = null;
			if (this.ParentBarStreaming							== null) return ret;
			if (this.ParentBarStreaming.ParentBars				== null) return ret;
			if (this.ParentBarStreaming.ParentBars.SymbolInfo	== null) return ret;
			ret = this.ParentBarStreaming.ParentBars.SymbolInfo;
			return ret;
		} }

		[JsonIgnore]	protected	string		PriceFormat	{ get {
			SymbolInfo symbolInfo = this.symbolInfo_nullUnsafe;
			return symbolInfo != null ? symbolInfo.PriceFormat : "N2";
		} }
		[JsonIgnore]	protected	string		VolumeFormat	{ get {
			SymbolInfo symbolInfo = this.symbolInfo_nullUnsafe;
			return symbolInfo != null ? symbolInfo.VolumeFormat : "N0";
		} }
		#endregion

		[JsonIgnore]	public	string		Ask_formatted	{ get { return string.Format("{0:" + this.PriceFormat + "}", this.Ask); } }
		[JsonIgnore]	public	string		Bid_formatted	{ get { return string.Format("{0:" + this.PriceFormat + "}", this.Bid); } }
		[JsonIgnore]	public	string		Size_formatted	{ get { return string.Format("{0:" + this.VolumeFormat + "}", this.Size); } }
		
		[JsonProperty]	public	string		QuoteTiming_localRemoteLeft { get {
			StringBuilder sb = new StringBuilder();
			sb.Append(" #");
			sb.Append(this.IntraBarSerno.ToString("000"));
			sb.Append(" ");
			sb.Append(this.ServerTime.ToString("HH:mm:ss.fff"));
			bool quoteTimesDifferMoreThanOneDeciSecond = this.ServerTime.ToString("HH:mm:ss.f") != this.LocalTime.ToString("HH:mm:ss.f");
			if (quoteTimesDifferMoreThanOneDeciSecond) {
				sb.Append(" :: ");
				sb.Append(this.LocalTime.ToString("HH:mm:ss.fff"));
			}
			if (this.HasParentBarStreaming) {
				TimeSpan timeLeft = (this.ParentBarStreaming.DateTime_nextBarOpen_unconditional > this.ServerTime)
				    ? this.ParentBarStreaming.DateTime_nextBarOpen_unconditional.Subtract(this.ServerTime)
				    : this.ServerTime.Subtract(this.ParentBarStreaming.DateTime_nextBarOpen_unconditional);
				string format = "mm:ss";
				if (timeLeft.Minutes > 0) format = "mm:ss";
				if (timeLeft.Hours > 0) format = "HH:mm:ss";
				sb.Append(" ");
				string timeLeftFormatted = new DateTime(timeLeft.Ticks).ToString(format);
				if (this.ParentBarStreaming.DateTime_nextBarOpen_unconditional < this.ServerTime) timeLeftFormatted = "-" + timeLeftFormatted;
				sb.Append(timeLeftFormatted);
			}
			return sb.ToString();
		} }

		[JsonIgnore]	public	bool		HasGeneratedSource { get {
			bool ret = this.Source.Contains(Quote.GENERATED_TO_FILL_ALERT)
				//&& this.IntraBarSerno >= Quote.IntraBarSernoShift_forGenerated_towardsPendingFill
				&& this is QuoteGenerated;
			return ret;
		} }

		

		Quote() {
			IntraBarSerno = -1;		// filled in lateBinder
			ItriggeredFillAtBidOrAsk = BidOrAsk.UNKNOWN;
		}

		public Quote(DateTime localTime, DateTime serverTime,
						string symbol, long absno_perSymbol_perStreamingAdapter = -1,
						double bid = double.NaN, double ask = double.NaN, double size = -1,
						BidOrAsk tradedAt = BidOrAsk.UNKNOWN) : this() {
			LocalTime		= localTime;
			ServerTime		= serverTime;
			Symbol			= symbol;
			AbsnoPerSymbol	= absno_perSymbol_perStreamingAdapter;
			Bid				= bid;
			Ask				= ask;
			Size			= size;
			TradedAt		= tradedAt;
		}
		public void StreamingBar_Replace(Bar streamingParentBar, bool streamingParentBars_mustBeNullforSolidifier = false) {
			string msig = " //(" + streamingParentBar.ToString() + ") => quote[" + this.ToString() + "]";
			if (streamingParentBar == null) {
				string msg = "NULL_BAR_NOT_ATTACHED_TO_THIS_QUOTE";
			} else if (streamingParentBar.ParentBars == null) {
				if (streamingParentBars_mustBeNullforSolidifier) {
					string msg = "SOLIDIFIERS_WILL_ATTACH_BAR_EMULATED__UNATTACHED_BARS";
				} else {
					string msg = "UNATTACHED_BAR_ASSIGNED_INTO_THIS_QUOTE";
					Assembler.PopupException(msg + msig, null, false);
				}
			} else {
				string msg = "ATTACHED_BAR_ASSIGNED_INTO_THIS_QUOTE";
			}
			if (streamingParentBar != null && this.Symbol != streamingParentBar.Symbol) {
				string msg = "SYMBOL_MISMATCH__CANT_SET_PARENT_BAR_FOR_QUOTE quote.Symbol[" + this.Symbol + "] != parentBar.Symbol[" + streamingParentBar.Symbol + "]";
				Assembler.PopupException(msg);
			}
			if (streamingParentBars_mustBeNullforSolidifier) {
				if (streamingParentBar.ParentBars != null) {
					string msg = "I_REFUSE_TO_BIND_QUOTE_TO_ATTACHED_STREAMING_BAR____MUST_BE_NULL I_REFUSE_TO_PUSH_QUOTE_TO_SOLIDIFIER";
					Assembler.PopupException(msg + msig, null, true);
				}
			} else {
				if (streamingParentBar.ParentBars == null) {
					string msg = "I_REFUSE_TO_BIND_QUOTE_TO_UNATTACHED_STREAMING_BAR";
					Assembler.PopupException(msg + msig, null, true);
				}
			}
			if (this.ParentBarStreaming == streamingParentBar) {
				string msg = "BAR_ALREADY_ATTACHED__UPSTACK_DIDNT_REALIZE_THIS";
				Assembler.PopupException(msg + msig, null, false);
			}
			this.ParentBarStreaming = streamingParentBar;
		}
		public void StreamingBar_Bind_toConsumersStreamingBar_expandedAttached(Bar streamingBar_fromBars) {
			if (streamingBar_fromBars == null) {
				string msg = "I_REFUSE_TO_BIND_TO_NULL_STREAMING_BAR";
				throw new Exception(msg);
			}
			if (streamingBar_fromBars.ParentBars != null) {
				string msg = "I_REFUSE_TO_BIND_ATTACHED_BAR__MUST_HAVE_NO_PARENTS";
				throw new Exception(msg);
			}
			if (this.ParentBarStreaming != null) {
				string msg = "I_REFUSE_TO_BIND_UNATTACHED_STREAMING_BAR__THIS_QUOTE_IS_ALREADY_BOUND__USE_Replace_myStreamingBar_withConsumersStreamingBar()";
				throw new Exception(msg);
			}
			this.StreamingBar_Replace(streamingBar_fromBars, false);
		}

		#region SORRY_FOR_THE_MESS__I_NEED_TO_DERIVE_IDENTICAL_ONLY_FOR_GENERATED__IF_YOU_NEED_IT_IN_BASE_QUOTE_MOVE_IT_THERE
		public Quote Clone_asCoreQuote() {
			string prefix = "MEMBERWISE_CLONE_OF_";
			if (this.Source.Contains(prefix)) {
				string msg = "QuoteCreatedThisAlert_deserializable <= WHERE_DO_YOU_NEED_CLONED_CLONE?";
				//Assembler.PopupException(msg);
				return this;
			}
			Quote clone = (Quote)this.MemberwiseClone();
			if (clone.ParentBarStreaming != null) {
				Bar barCloned_justInCase = clone.ParentBarStreaming.CloneDetached();
				clone.StreamingBar_Replace(barCloned_justInCase);
			}
			clone.Source = prefix + this.ToStringShort() + " " + clone.Source;
			return clone;
		}
		#endregion

		public override string ToString() {
			StringBuilder sb = new StringBuilder();
			sb.Append("#");
			sb.Append(this.IntraBarSerno);
			sb.Append("/");
			sb.Append(this.AbsnoPerSymbol);
			sb.Append(" ");
			sb.Append(this.Symbol);
			sb.Append(" bid{");
			sb.Append(this.Bid);
			sb.Append("-");
			sb.Append(this.Ask);
			sb.Append("}ask size{");
			sb.Append(this.Size);
			sb.Append("@");
			sb.Append(this.TradedPrice);
			sb.Append("}lastDeal");
			sb.Append(this.TradedAt);
			sb.Append(" ");
			bool timesAreDifferent = true;
			if (this.ServerTime != null) {
				if (this.ServerTime == this.LocalTime) {
					timesAreDifferent = false;
				}
				if (timesAreDifferent == true) {
					sb.Append(" SERVER[");
					sb.Append(this.ServerTime.ToString("HH:mm:ss.fff"));
					sb.Append("]");
				}
			}
			sb.Append("[");
			sb.Append(this.LocalTime.ToString("HH:mm:ss.fff"));
			sb.Append("]");
			if (timesAreDifferent == true) {
				sb.Append("LOCAL");
			}
			if (string.IsNullOrEmpty(this.Source) == false) {
				sb.Append(" ");
				sb.Append(this.Source);
			}

			sb.Append(" STR:");
			string firstEver_withNaNs_willThrow = "NULL";
			if (this.ParentBarStreaming != null) {
				firstEver_withNaNs_willThrow = double.IsNaN(this.ParentBarStreaming.Open)
					? "NaN"
					//: this.ParentBarIdent
					: this.ParentBarStreaming.ToString()
					;
			}
			sb.Append(firstEver_withNaNs_willThrow);

			return sb.ToString();
		}
		//public string ToStringShort() {
		//	string ret = "#" + this.IntraBarSerno + "/" + this.Absno + " " + this.Symbol
		//		+ " bid{" + this.Bid + "-" + this.Ask + "}ask size{" + this.Size + "}"
		//		+ ": " + this.ParentBarIdent;
		//	return ret;
		//}
		public string ToStringShort() {
			StringBuilder sb = new StringBuilder();
			sb.Append("#");
			sb.Append(this.IntraBarSerno);
			sb.Append("/");
			sb.Append(this.AbsnoPerSymbol);
			sb.Append(" ");
			sb.Append(this.Symbol);
			sb.Append(" bid{");
			sb.Append(this.Bid);
			sb.Append("-");
			sb.Append(this.Ask);
			sb.Append("}ask size{");
			sb.Append(this.Size);
			sb.Append("}");
			sb.Append(": ");
			sb.Append(this.ParentBarIdent);
			return sb.ToString();
		}
		public string ToStringShortest() {
			StringBuilder sb = new StringBuilder();
			sb.Append("#");
			sb.Append(this.IntraBarSerno);
			sb.Append("/");
			sb.Append(this.AbsnoPerSymbol);
			return sb.ToString();
		}
		public bool SameBidAsk(Quote other) {
			return (this.Bid == other.Bid && this.Ask == other.Ask);
		}
		public bool PriceBetweenBidAsk(double price) {
			//return (price >= this.Bid && price <= this.Ask);
			double diffUp = this.Ask - price;						// WTF 1760.2-1706.2=-2.27E-13
			double diffDn = price - this.Bid;
			bool ret = diffUp >= 0 && diffDn >= 0;

			if (ret == false) {
				//Debugger.Break();									// WTF 1760.2-1706.2=-2.27E-13
				double diffUpRounded = Math.Round(diffUp, 5);		// WTF 1760.2-1706.2=-2.27E-13
				double diffDnRounded = Math.Round(diffDn, 5);
				ret = diffUpRounded >= 0 && diffDnRounded >= 0;
	
				// moved to post-fill check upstack:
				// if (filled == 1 && this.fillOutsideQuoteSpreadParanoidCheckThrow == true) {alert.IsFilledOutsideQuote_DEBUG_CHECK;alert.IsFilledOutsideBarSnapshotFrozen_DEBUG_CHECK;}
				//if (ret == false) {
				//	#if DEBUG
				//	Debugger.Break();	// ENABLE_BREAK_UPSTACK_IF_YOU_COMMENT_IT_OUT_HERE
				//	#endif
				//}
			}
			return ret;
		}
		public void Set_IntraBarSerno__onlyInConsumer(int intraBarSerno) {
			this.IntraBarSerno = intraBarSerno;
		}
	}
}