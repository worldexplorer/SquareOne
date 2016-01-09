using System;
using System.Collections.Generic;

using NDde;
using NDde.Client;

using Sq1.Core;
using Sq1.Core.DataTypes;
using Sq1.Core.Backtesting;

using Sq1.Adapters.Quik.Streaming.Dde;
using Sq1.Adapters.Quik.Streaming.Livesim.Dde.XlDde;

namespace Sq1.Adapters.Quik.Streaming.Livesim.Dde {
	public class DdeTableGeneratorQuotes : XlDdeTableGenerator {
		protected override string DdeGeneratorClassName { get { return "DdeTableGeneratorQuotes"; } }

		public DdeTableGeneratorQuotes(string ddeService, string ddeTopic, QuikStreamingLivesim quikLivesimStreaming) : base(ddeService, ddeTopic, quikLivesimStreaming) {
			base.Initialize(TableDefinitions.XlColumnsForTable_Quotes);
		}

		public void OutgoingObjectBufferize_eachRow(object quoteAsObject) {
			string msig = " //" + this.DdeGeneratorClassName + ".OutgoingObjectBufferize_eachRow(" + quoteAsObject + ")";
			if (quoteAsObject == null) {
				Assembler.PopupException("MUST_NOT_BE_NULL" + msig);
				return;
			}
			QuoteGenerated quote = quoteAsObject as QuoteGenerated;
			if (quote == null) {
				Assembler.PopupException("MUST_BE_A_QuoteGenerated [" + quoteAsObject.GetType() + "]" + msig);
				return;
			}
			base.XlWriter.Put("SHORTNAME",	quote.Symbol);
			base.XlWriter.Put("CLASS_CODE",	quote.SymbolClass);
			base.XlWriter.Put("bid",		quote.Bid);
			//base.XlWriter.Put("biddepth",	quote.Symbol);
			base.XlWriter.Put("offer",		quote.Ask);
			//base.XlWriter.Put("offerdepth",	quote.Symbol);
			base.XlWriter.Put("last",		quote.TradedPrice);
			//base.XlWriter.Put("stepprice",	quote.Symbol);


			string dateFormat = base.ColumnsLookup["date"].ToDateTimeParseFormat;
			string timeFormat = base.ColumnsLookup["time"].ToDateTimeParseFormat;

			string date = quote.ServerTime.ToString(dateFormat);
			string time = quote.ServerTime.ToString(timeFormat);

			base.XlWriter.Put("date",		date);
			base.XlWriter.Put("time",		time);

			//base.XlWriter.Put("changetime",	quote.Symbol);
			//base.XlWriter.Put("selldepo",	quote.Symbol);
			//base.XlWriter.Put("buydepo",	quote.Symbol);
			base.XlWriter.Put("qty",		quote.Size);
			//base.XlWriter.Put("pricemin",	quote.Symbol);
			//base.XlWriter.Put("pricemax",	quote.Symbol);
			//base.XlWriter.Put("stepprice",	quote.Symbol);
		}

		internal void Send_DdeClientPokesDdeServer_waitServerProcessed(QuoteGenerated quote) {
			try {
				base.OutgoingTableBegin();
				this.OutgoingObjectBufferize_eachRow(quote);
				base.OutgoingTableTerminate();

				byte[] bufferToSend = base.XlWriter.ConvertToXlDdeMessage();
				
				//IRANAI_DES IAsyncResult handle = this.DdeClient.BeginPoke("item-quote", bufferToSend, 0, new AsyncCallback(this.ddePokeAsyncCallback), this);
				IAsyncResult handle = base.DdeClient.BeginPoke("item-quote", bufferToSend, 0, null, this);
	
				bool isCompleted_hereNo		= handle.IsCompleted;
				// this.DdeClient.EndPoke(handle) is waiting for DdeServer.OnPoke() to return PokeResult.Processed;
				// with straight (non-threaded) QuotePump that means Strategy.OnQuote() has returned
				this.DdeClient.EndPoke(handle);		//SYNCHRONOUS_IS_EASIER_TO_DEBUG
				//bool completedSynchronously_hereFalse_noIdea	= handle.CompletedSynchronously;
				bool isCompleted_hereYes	= handle.IsCompleted;
			} catch (ArgumentNullException ex) {
				Assembler.PopupException("This is thrown when item or data is a null reference.", ex);
			} catch (ArgumentException ex) {
				Assembler.PopupException("This is thown when item exceeds 255 characters.", ex);
			} catch (InvalidOperationException ex) {
				Assembler.PopupException("This is thrown when the client is not connected.", ex);
			} catch (DdeException ex) {
				Assembler.PopupException("This is thrown when the asynchronous operation could not begin.", ex);
			} catch (Exception ex) {
				Assembler.PopupException("UNKNOWN_ERROR_DDE_CLIENT_BEGIN_POKE", ex);
			}
		}
	}
}