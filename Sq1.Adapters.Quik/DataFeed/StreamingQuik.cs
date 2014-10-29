using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Serialization;

using Sq1.Core;
using Sq1.Core.DataFeed;
using Sq1.Core.DataTypes;
using Sq1.Core.Streaming;
using Sq1.Core.Support;
using Sq1.Adapters.Quik.Dde;

namespace Sq1.Adapters.Quik {
	[DataContract]
	public class StreamingQuik : StreamingProvider {
		private DdeChannels DdeChannels;
		[DataMember]
		public string DdeServerPrefix { get; internal set; }
		[DataMember]
		public string DdeTopicQuotes { get; internal set; }
		[DataMember]
		public string DdeTopicTrades { get; internal set; }
		[DataMember]
		public string DdeTopicPrefixDom { get; internal set; }
		public override StreamingEditor StreamingEditorInitialize(IDataSourceEditor dataSourceEditor) {
			base.StreamingEditorInitializeHelper(dataSourceEditor);
			base.streamingEditorInstance = new StreamingQuikEditor(this, dataSourceEditor);
			return base.streamingEditorInstance;
		}

		public StreamingDataSnapshotQuik StreamingDataSnapshotQuik {
			get {
				if (base.StreamingDataSnapshot is StreamingDataSnapshotQuik == false) {
					string msg = "base.StreamingDataSnapshot[" + base.StreamingDataSnapshot
						+ "] got modified while should remain of type StreamingDataSnapshotQuik"
						+ " since QuikStreamingProvider is constructed";
					throw new Exception(msg);
				}
				return base.StreamingDataSnapshot as StreamingDataSnapshotQuik;
			}
		}

		public override List<string> SymbolsUpstreamSubscribed {
			get { return this.DdeChannels.SymbolsHavingIndividualChannels; }
		}

		public StreamingQuik() : base() {
			base.Name = "Quik StreamingDummy";
			base.Description = ""
				+ "1. Сначала добейтесь, чтобы работал FinamStreamingDataProvider (галку выше)\n\n"
				+ "2. QUIK: Текущая таблица параметров - редактировать (CTRL+E)\n"
				+ "2.1. добавить инструмент RIU2\n"
				+ "2.2. поля: Код класса, Код бумаги, Цена посл сделки, Время посл изм\n"
				+ "2.3. Вывод через DDE сервер (CTRL+L): \n"
				+ "2.3.1. DDE сервер: myWLD-RIU2, рабочая книга: quotes;\n"
				+ "2.3.2. с заголовками столбцов = ДА, формальные заголовки = ДА;\n"
				+ "3. WealthLab: создать в DataManager новый QuickStaticDataProvider DataSet, впечатать RIU2, открыть RIU2 чарт, нажать Stream в правом нижнем углу - запустится DDE сервер myWLD-RIU2\n"
				+ "4. кнопка НАЧАТЬ ВЫВОД пункта 2 - рилтайм квоты RIU2 прольются в канал quotes и бары запрыгают в WealthLab\n"
				+ "5. новый QuickStaticDataProvider DataSet GAZP в WealthLab создаст новый DDE сервер myWLD-GAZP с рабочей книгой quotes, останется создать новую таблицу параметров, настроить как в 1 и запустить экспорт DDE в сервер myWLD-GAZP книгу quotes. Enjoy!";
			base.Icon = (Bitmap)Sq1.Adapters.Quik.Properties.Resources.imgQuikStreamingProvider;
			base.PreferredStaticProviderName = "QuikStaticProvider";
			this.DdeServerPrefix = "myWLD";
			this.DdeTopicQuotes = "quotes";
			this.DdeTopicTrades = "trades";
			this.DdeTopicPrefixDom = "dom";
			base.StreamingDataSnapshot = new StreamingDataSnapshotQuik(this);
			StreamingDataSnapshotQuik throwAtEarlyStage = this.StreamingDataSnapshotQuik;
		}
		public override void Initialize(DataSource dataSource, IStatusReporter statusReporter) {
			base.Name = "Quik StreamingProvider";
			base.Initialize(dataSource, statusReporter);
			this.DdeChannels = new DdeChannels(this, this.StatusReporter);
			this.Connect();
		}
		public override void Connect() {
			if (base.IsConnected == true) return;
			string symbolsSubscribed = subscribeAllSymbols();
			this.DdeChannels.StartDdeServer();
			base.ConnectionState = ConnectionState.Connected;
			base.UpdateConnectionStatus(0, "Started symbolsSubscribed[" + symbolsSubscribed + "]");
			Assembler.PopupException("QUIK started DdeChannels[" + this.DdeChannels.ToString() + "]");
			base.IsConnected = true;
		}
		public override void Disconnect() {
			if (base.IsConnected == false) return;
			Assembler.PopupException("QUIK stopping DdeChannels[" + this.DdeChannels.ToString() + "]");
			string symbolsUnsubscribed = unsubscribeAllSymbols();
			base.ConnectionState = ConnectionState.Disconnected;
			base.UpdateConnectionStatus(0, "Stopped symbolsUnsubscribed[" + symbolsUnsubscribed + "]");
			this.DdeChannels.StopDdeServer();
			Assembler.PopupException("QUIK stopped DdeChannels[" + this.DdeChannels.ToString() + "]");
			base.IsConnected = false;
		}

		private string subscribeAllSymbols() {
			string ret = "";
			lock (base.SymbolsSubscribedLock) {
				foreach (string symbol in base.DataSource.Symbols) {
					this.UpstreamSubscribe(symbol);
					ret += symbol + " ";
				}
			}
			ret = ret.TrimEnd(' ');
			return ret;
		}
		private string unsubscribeAllSymbols() {
			string ret = "";
			lock (base.SymbolsSubscribedLock) {
				foreach (string symbol in base.DataSource.Symbols) {
					this.UpstreamUnSubscribe(symbol);
					ret += symbol + " ";
				}
			}
			ret = ret.TrimEnd(' ');
			return ret;
		}

		public override void UpstreamSubscribe(string symbol) {
			if (string.IsNullOrEmpty(symbol)) {
				Assembler.PopupException("can't subscribe empty symbol=[" + symbol + "]; returning");
				return;
			}
			lock (base.SymbolsSubscribedLock) {
				if (this.DdeChannels.SymbolHasIndividualChannels(symbol)) {
					String msg = "QUIK: ALREADY SymbolHasIndividualChannels(" + symbol + ")=[" + this.DdeChannels.IndividualChannelsForSymbol(symbol) + "]";
					Assembler.PopupException(msg);
					//this.StatusReporter.UpdateConnectionStatus(ConnectionState.OK, 0, msg);
					return;
				}
				this.DdeChannels.AddIndividualSymbolChannels(symbol);
			}
		}
		public override void UpstreamUnSubscribe(string symbol) {
			if (string.IsNullOrEmpty(symbol)) {
				Assembler.PopupException("can't unsubscribe empty symbol=[" + symbol + "]; returning");
				return;
			}
			lock (base.SymbolsSubscribedLock) {
				if (this.DdeChannels.SymbolHasIndividualChannels(symbol) == false) {
					string errormsg = "QUIK: NOTHING TO REMOVE SymbolHasIndividualChannels(" + symbol + ")=[" + this.DdeChannels.IndividualChannelsForSymbol(symbol) + "]";
					Assembler.PopupException(errormsg);
					return;
				}
				this.DdeChannels.RemoveIndividualSymbolChannels(symbol);
			}
		}
		public override bool UpstreamIsSubscribed(string symbol) {
			if (String.IsNullOrEmpty(symbol)) {
				Assembler.PopupException("IsSubscribed() symbol=[" + symbol + "]=IsNullOrEmpty; returning");
				return false;
			}
			lock (base.SymbolsSubscribedLock) {
				return this.DdeChannels.SymbolHasIndividualChannels(symbol);
			}
		}

		public void FilterAndDistributeDdeQuote(Quote quote) {
			DateTime thisDayClose = this.DataSource.MarketInfo.getThisDayClose(quote);
			DateTime preMarketQuotePoitingToThisDayClose = quote.ServerTime.AddSeconds(1);
			bool isQuikPreMarketQuote = preMarketQuotePoitingToThisDayClose >= thisDayClose;
			if (isQuikPreMarketQuote) {
				string msg = "skipping pre-market quote"
					+ " quote.ServerTime[" + quote.ServerTime + "].AddSeconds(1) >= thisDayClose[" + thisDayClose + "]"
					+ " quote=[" + quote + "]";
				Assembler.PopupException(msg);
				return;
			}
//			if (quote.PriceLastDeal == 0) {
//				string msg = "skipping pre-market quote since CHARTS will screw up painting price=0;"
//					+ " quote=[" + quote + "]";
//				Assembler.PopupException(msg);
//				this.StatusReporter.PopupException(new Exception(msg));
//				return;
//			}
			if (string.IsNullOrEmpty(quote.Source)) quote.Source = "Quik";
			base.PushQuoteReceived(quote);
		}
		public override void EnrichQuoteWithStreamingDependantDataSnapshot(Quote quote) {
			if (quote is QuoteQuik == false) {
				string msg = "Should be of a type Sq1.Adapters.Quik.QuoteQuik instead of Sq1.Core.DataTypes.Quote: "
					+ quote;
				throw new Exception(msg);
			}
			QuoteQuik quikQuote = quote as QuoteQuik;
			quikQuote.EnrichFromStreamingDataSnapshotQuik(this.StreamingDataSnapshotQuik);
		}

		
		
		//IQuikDataReceiver
		public void TradeDeliveredDdeCallback(string skey, DdeTrade trade) { }
		public override string ToString() {
			return Name + "/[" + this.ConnectionState + "]: Symbols[" + base.SymbolsUpstreamSubscribedAsString + "]"
				+ " DDE[" + this.DdeChannelsEstablished + "]";
		}
		public string DdeChannelsEstablished {
			get {
				string ret = "";
				lock (base.SymbolsSubscribedLock) {
					ret = this.DdeChannels.ToString();
				}
				return ret;
			}
		}
	}
}