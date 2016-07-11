using System;
using System.Collections.Generic;
using System.ComponentModel;

using Sq1.Core;
using Sq1.Core.Repositories;

namespace Sq1.Widgets.DataSourceEditor {
	public partial class MarketInfoEditor {
		void cbxMarketTimeZone_SelectedIndexChanged(object sender, EventArgs e) {
			if (this.ignoreSelectionEventDuringPopulate) return;
			if (string.IsNullOrEmpty(this.cbxMarketTimeZone.Text)) return;
			Dictionary<string, string> TimeZonesWithUTC = this.marketInfoRepository.TimeZonesWithUTC;
			if (TimeZonesWithUTC.ContainsKey(this.cbxMarketTimeZone.Text) == false) {
				Assembler.PopupException("cbxMarketTimeZone.Text[" + this.cbxMarketTimeZone.Text + "] is not valid; " +
					"Check your MicrosoftTimeZones.csv");
				return;
			}
			string timezoneId = TimeZonesWithUTC[this.cbxMarketTimeZone.Text];
			this.dataSource.MarketInfo.TimeZoneName = timezoneId;
			this.marketInfoRepository.Serialize();
		}
		void txtMarketDaysOfWeek_Validating(object sender, CancelEventArgs e) {
			if (this.ignoreSelectionEventDuringPopulate) return;
			this.ignoreSelectionEventDuringPopulate = true;
			string cellValueAsString = this.txtMarketDaysOfWeek.Text;
			try {
				List<DayOfWeek> dowsParsed = RepositorySerializerMarketInfos.ParseDaysOfWeekCsv(cellValueAsString, ", ");
				this.dataSource.MarketInfo.DaysOfWeekOpen = dowsParsed;
			} catch (Exception ex) {
				string errormsg = "InputAsString[" + cellValueAsString + "] " + ex.Message
					+ ", back to old value [" + this.dataSource.MarketInfo.DaysOfWeekOpenAsString + "]";
				Assembler.PopupException(errormsg + " /txtMarketDaysOfWeek_Validating", ex);
			}
			this.txtMarketDaysOfWeek.Text = this.dataSource.MarketInfo.DaysOfWeekOpenAsString;
			this.ignoreSelectionEventDuringPopulate = false;
			this.marketInfoRepository.Serialize();
		}
		void txtMarketServerClose_Validating(object sender, CancelEventArgs e) {
			if (this.ignoreSelectionEventDuringPopulate) return;
			this.ignoreSelectionEventDuringPopulate = true;
			string cellValueAsString = this.txtMarketServerClose.Text;
			try {
				DateTime newTime = DateTime.Parse(cellValueAsString);
				this.dataSource.MarketInfo.MarketClose_serverTime = newTime;
			} catch (Exception ex) {
				string errormsg = "InputAsString[" + cellValueAsString + "] " + ex.Message
					+ ", back to old value [" + this.dataSource.MarketInfo.MarketOpen_serverTime_asString + "]";
				Assembler.PopupException(errormsg + " /txtMarketServerClose_Validating", ex);
			}
			this.txtMarketServerClose.Text = this.dataSource.MarketInfo.MarketOpen_serverTime_asString;
			this.ignoreSelectionEventDuringPopulate = false;
			this.marketInfoRepository.Serialize();
		}
		void txtMarketServerOpen_Validating(object sender, CancelEventArgs e) {
			if (this.ignoreSelectionEventDuringPopulate) return;
			this.ignoreSelectionEventDuringPopulate = true;
			string cellValueAsString = this.txtMarketServerOpen.Text;
			try {
				DateTime newTime = DateTime.Parse(cellValueAsString);
				this.dataSource.MarketInfo.MarketOpen_serverTime = newTime;
			} catch (Exception ex) {
				string errormsg = "InputAsString[" + cellValueAsString + "] " + ex.Message
					+ ", back to old value [" + this.dataSource.MarketInfo.MarketClose_serverTime_asString + "]";
				Assembler.PopupException(errormsg + " txtMarketServerOpen_Validating", ex);
			}
			this.txtMarketServerOpen.Text = this.dataSource.MarketInfo.MarketClose_serverTime_asString;
			this.ignoreSelectionEventDuringPopulate = false;
			this.marketInfoRepository.Serialize();
		}
	}
}
