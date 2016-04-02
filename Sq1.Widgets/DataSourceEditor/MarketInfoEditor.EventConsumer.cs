using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Core.DataTypes;
using Sq1.Core.Repositories;

namespace Sq1.Widgets.DataSourceEditor {
	public partial class MarketInfoEditor {
		void dgMarketName_SelectionChanged(object sender, EventArgs e) {
			if (this.ignoreSelectionEventDuringPopulate) return;
			this.lnkMarketNameDelete.Enabled = false;
			if (this.dgMarketName.SelectedRows.Count == 0) return;
			MarketInfo marketInfo = (MarketInfo)this.dgMarketName.SelectedRows[0].Tag;
			if (marketInfo == null) return;
			try {
				this.dataSource.MarketInfo = marketInfo;
				this.populateMarket_fromDataSource();
				this.lnkMarketNameDelete.Enabled = (this.dataSourceRepository.SameMarketInfoInHowManyDataSources(marketInfo) > 0) ? true : false;
				this.dataSourceRepository.SerializeSingle(this.dataSource);
			} catch (Exception ex) {
				Assembler.PopupException("dgMarketName_SelectionChanged", ex);
			}
		}
		void lnkMarketNameDelete_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {

		}
		void dgMarketName_CellValueChanged(object sender, DataGridViewCellEventArgs e) {
			if (this.ignoreSelectionEventDuringPopulate) return;
			if (e.RowIndex < 0) return;
			if (e.ColumnIndex < 0) return;
			string errormsg = "";
			string headerText = this.dgMarketName.Columns[e.ColumnIndex].HeaderText;
			MarketInfo marketInfo = this.dgMarketName.Rows[e.RowIndex].Tag as MarketInfo;
			if (marketInfo == null) {
				string msg = "Rows[" + e.RowIndex + "].Tag=null (expecting MarketInfo) " + headerText;
				Assembler.PopupException(msg + " /dgMarketName_SelectionChanged");
				return;
			}
			DataGridViewCell cell = this.dgMarketName[e.ColumnIndex, e.RowIndex];
			string cellValueAsString = cell.Value.ToString();
			string columnNameChanged = this.dgMarketName.Columns[e.ColumnIndex].Name;
			this.ignoreSelectionEventDuringPopulate = true;
			switch (columnNameChanged) {
				case "colMarketName":
					if (string.IsNullOrEmpty(cellValueAsString) != false) {
						marketInfo.Name = cellValueAsString;
						this.marketInfoRepository.RenameMarketInfoRearrangeDictionary(marketInfo);
					} else {
						errormsg = columnNameChanged + ": cellValueAsString[" + cellValueAsString + "] IsNullOrEmpty"
							+ ", back to old value [" + marketInfo.Name + "]";
					}
					cell.Value = marketInfo.Name;
					break;
				default:
					errormsg = "no handler for field:[" + columnNameChanged + "]";
					break;
			}
			this.ignoreSelectionEventDuringPopulate = false;
			if (string.IsNullOrEmpty(errormsg) == false) {
				string msg = errormsg + headerText;
				Assembler.PopupException(msg + " /dgMarketName_CellValueChanged");
				return;
			}
			this.marketInfoRepository.Serialize();
		}
		void dgClearingTimespans_SelectionChanged(object sender, EventArgs e) {
			this.lnkIntradayInterruptsDelete.Enabled = false;
			if (this.ignoreSelectionEventDuringPopulate) return;
			if (this.dgClearingTimespans.SelectedRows.Count == 0) return;
			MarketClearingTimespan closedHour = (MarketClearingTimespan)this.dgClearingTimespans.SelectedRows[0].Tag;
			if (closedHour == null) return;
			this.lnkIntradayInterruptsDelete.Enabled = true;
		}
		void dgClearingTimespans_CellValueChanged(object sender, DataGridViewCellEventArgs e) {
			if (this.ignoreSelectionEventDuringPopulate) return;
			if (e.RowIndex < 0) return;
			if (e.ColumnIndex < 0) return;
			string errormsg = "";
			string headerText = this.dgClearingTimespans.Columns[e.ColumnIndex].HeaderText;
			MarketClearingTimespan clearingTimespan = this.dgClearingTimespans.Rows[e.RowIndex].Tag as MarketClearingTimespan;
			if (clearingTimespan == null) {
				string msg = "Rows[" + e.RowIndex + "].Tag=null (expecting RegularClosedHour) " + headerText;
				Assembler.PopupException(msg + " /dgClearingTimespans_CellValueChanged");
				return;
			}
			DataGridViewCell cell = this.dgClearingTimespans[e.ColumnIndex, e.RowIndex];
			string cellValueAsString = (cell.Value != null) ? cell.Value.ToString() : "";
			string columnNameChanged = this.dgClearingTimespans.Columns[e.ColumnIndex].Name;
			this.ignoreSelectionEventDuringPopulate = true;
			switch (columnNameChanged) {
				case "colClearingTimespansSuspends":
					try {
						DateTime newTime = DateTime.Parse(cellValueAsString);
						clearingTimespan.SuspendServerTimeOfDay = newTime;
					} catch (Exception ex) {
						errormsg = columnNameChanged + ": cellValueAsString[" + cellValueAsString + "] " + ex.Message
							+ ", back to old value [" + clearingTimespan.SuspendServerTimeOfDayAsString + "]";
					}
					cell.Value = clearingTimespan.SuspendServerTimeOfDayAsString;
					break;
				case "colClearingTimespansResumes":
					try {
						DateTime newTime = DateTime.Parse(cellValueAsString);
						clearingTimespan.ResumeServerTimeOfDay = newTime;
					} catch (Exception ex) {
						errormsg = columnNameChanged + ": cellValueAsString[" + cellValueAsString + "] " + ex.Message
							+ ", back to old value [" + clearingTimespan.ResumeServerTimeOfDayAsString + "]";
					}
					cell.Value = clearingTimespan.ResumeServerTimeOfDayAsString;
					break;
				case "colClearingTimespansDaysOfWeek":
					try {
						List<DayOfWeek> dowsParsed = RepositorySerializerMarketInfos.ParseDaysOfWeekCsv(cellValueAsString, ", ");
						clearingTimespan.DaysOfWeekWhenClearingHappens = dowsParsed;
					} catch (Exception ex) {
						errormsg = columnNameChanged + ": cellValueAsString[" + cellValueAsString + "] " + ex.Message
							+ ", back to old value [" + clearingTimespan.DaysOfWeekWhenClosingNotHappensAsString + "]";
					}
					cell.Value = clearingTimespan.DaysOfWeekWhenClosingNotHappensAsString;
					break;
				default:
					errormsg = "no handler for field:[" + columnNameChanged + "]";
					break;
			}
			this.ignoreSelectionEventDuringPopulate = false;
			if (string.IsNullOrEmpty(errormsg) == false) {
				Assembler.PopupException(errormsg);
			}
			this.marketInfoRepository.Serialize();
		}
		void lnkIntradayInterruptsDelete_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {

		}
		void dgShortDays_SelectionChanged(object sender, EventArgs e) {
			if (this.ignoreSelectionEventDuringPopulate) return;
			this.lnkShortDaysDelete.Enabled = (this.dgShortDays.SelectedRows.Count > 0) ? true : false;
		}
		void lnkShortDaysDelete_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {

		}
		void dgHolidays_SelectionChanged(object sender, EventArgs e) {
			if (this.ignoreSelectionEventDuringPopulate) return;
			this.lnkHolidaysDelete.Enabled = (this.dgHolidays.SelectedRows.Count > 0) ? true : false;
		}
		void lnkHolidaysDelete_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {

		}
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
		void dgShortDays_CellValueChanged(object sender, DataGridViewCellEventArgs e) {

		}
		void dgHolidays_CellValueChanged(object sender, DataGridViewCellEventArgs e) {

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
