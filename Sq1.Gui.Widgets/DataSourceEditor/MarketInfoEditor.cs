using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Core.DataFeed;
using Sq1.Core.DataTypes;
using Sq1.Core.Repositories;

namespace Sq1.Widgets.DataSourceEditor {
	[Designer("System.Windows.Forms.Design.ParentControlDesigner, System.Design", typeof(IDesigner))]
	public partial class MarketInfoEditor : UserControl {
		bool settingValueManuallyIgnoringEvent;
		DataSource dataSource;
		RepositoryCustomMarketInfo marketInfoRepository;
		RepositoryJsonDataSource dataSourceRepository;
		public MarketInfoEditor() {
			this.InitializeComponent();
		}
		void MarketInfoEditor_Load(object sender, EventArgs e) {
			//return;
			if (base.DesignMode) return;
			this.populateMarketFromDataSource();
		}
		public void Initialize(DataSource dataSource, RepositoryJsonDataSource dataSourceRepository, RepositoryCustomMarketInfo marketInfoRepository) {
			if (base.DesignMode) return;
			this.dataSource = dataSource;
			this.dataSourceRepository = dataSourceRepository;
			this.marketInfoRepository = marketInfoRepository;
			this.populateMarketFromDataSource();
		}
		public void populateMarketFromDataSource() {
			if (base.DesignMode) return;

			this.populateMarketNamesDataGrid();
			this.populateMarketIntradayInterruptsDataGrid();
			this.populateMarketShortDaysDataGrid();
			this.populateMarketHolidaysDataGrid();
			this.populateMarketOpenCloseTexts();
			this.populateTimeZonesSelector();
		}
		void populateMarketNamesDataGrid() {
			this.settingValueManuallyIgnoringEvent = true;
			this.dgMarketName.Rows.Clear();
			foreach (MarketInfo marketInfo in this.marketInfoRepository.Entity.Values) {
				int index = this.dgMarketName.Rows.Add(new object[] {
					marketInfo.Name,
					this.dataSourceRepository.UsedTimes(marketInfo)
				});
				this.dgMarketName.Rows[index].Tag = marketInfo;
				if (marketInfo == this.dataSource.MarketInfo) this.dgMarketName.Rows[index].Selected = true;
			}
			this.settingValueManuallyIgnoringEvent = false;
		}
		void populateMarketIntradayInterruptsDataGrid() {
			this.settingValueManuallyIgnoringEvent = true;
			this.dgClearingTimespans.Rows.Clear();
			if (this.dataSource.MarketInfo != null && this.dataSource.MarketInfo.ClearingTimespans != null) {
				foreach (MarketClearingTimespan closedHour in this.dataSource.MarketInfo.ClearingTimespans) {
					int index = this.dgClearingTimespans.Rows.Add(new object[] {
					closedHour.SuspendServerTimeOfDay.ToString("HH:mm"),
					closedHour.ResumeServerTimeOfDay.ToString("HH:mm"),
					closedHour.DaysOfWeekWhenClosingNotHappensAsString
				});
					this.dgClearingTimespans.Rows[index].Tag = closedHour;
				}
			}
			this.settingValueManuallyIgnoringEvent = false;
		}
		void populateMarketShortDaysDataGrid() {
			this.settingValueManuallyIgnoringEvent = true;
			this.dgShortDays.Rows.Clear();
			if (this.dataSource.MarketInfo != null && this.dataSource.MarketInfo.ShortDays != null) {
				foreach (MarketShortDay shortDay in this.dataSource.MarketInfo.ShortDays) {
					int index = this.dgShortDays.Rows.Add(new object[] {
					shortDay.Date.ToString("dd MMM"),
					shortDay.ServerTimeOpening.ToString("HH:mm"),
					shortDay.ServerTimeClosing.ToString("HH:mm")
				});
					this.dgShortDays.Rows[index].Tag = shortDay;
				}
			}
			this.settingValueManuallyIgnoringEvent = false;
		}
		void populateMarketHolidaysDataGrid() {
			this.settingValueManuallyIgnoringEvent = true;
			this.dgHolidays.Rows.Clear();
			if (this.dataSource.MarketInfo != null && this.dataSource.MarketInfo.ShortDays != null) {
				foreach (DateTime dateTime in this.dataSource.MarketInfo.HolidaysYMD000) {
					int index = this.dgHolidays.Rows.Add(new object[] {
					dateTime.ToString("dd MMM")
				});
					this.dgHolidays.Rows[index].Tag = dateTime;
				}
			}
			this.settingValueManuallyIgnoringEvent = false;
		}
		void populateMarketOpenCloseTexts() {
			if (this.dataSource.MarketInfo == null) return;
			this.txtMarketServerOpen.Text = this.dataSource.MarketInfo.MarketOpenServerTime.ToString("HH:mm");
			this.txtMarketServerClose.Text = this.dataSource.MarketInfo.MarketCloseServerTime.ToString("HH:mm");
			this.txtMarketDaysOfWeek.Text = this.dataSource.MarketInfo.DaysOfWeekOpenAsString;
		}
		void populateTimeZonesSelector() {
			this.settingValueManuallyIgnoringEvent = true;
			this.cbxMarketTimeZone.Items.Clear();
			Dictionary<string, string> TimeZonesWithUTC = this.marketInfoRepository.TimeZonesWithUTC;
			var sortedTimeZones = TimeZonesWithUTC.Keys.ToList();	//using System.Linq;
			sortedTimeZones.Sort();
			if (this.dataSource.MarketInfo != null && this.dataSource.MarketInfo.TimeZoneName != null) {
				foreach (string key in sortedTimeZones) {
					int index = this.cbxMarketTimeZone.Items.Add(key);
					if (this.dataSource.MarketInfo.TimeZoneName == TimeZonesWithUTC[key]) {
						this.cbxMarketTimeZone.SelectedIndex = index;
						break;
					}
				}
			}
			this.settingValueManuallyIgnoringEvent = false;
		}
		void dgMarketName_SelectionChanged(object sender, EventArgs e) {
			this.lnkMarketNameDelete.Enabled = false;
			if (this.settingValueManuallyIgnoringEvent) return;
			if (this.dgMarketName.SelectedRows.Count == 0) return;
			MarketInfo marketInfo = (MarketInfo)this.dgMarketName.SelectedRows[0].Tag;
			if (marketInfo == null) return;
			try {
				this.dataSource.MarketInfo = marketInfo;
				this.populateMarketFromDataSource();
				this.lnkMarketNameDelete.Enabled = (this.dataSourceRepository.UsedTimes(marketInfo) > 0) ? true : false;
				this.dataSourceRepository.SerializeSingle(this.dataSource);
			} catch (Exception ex) {
				Assembler.PopupException("dgMarketName_SelectionChanged", ex);
			}
		}
		void lnkMarketNameDelete_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {

		}
		void dgMarketName_CellValueChanged(object sender, DataGridViewCellEventArgs e) {
			if (this.settingValueManuallyIgnoringEvent) return;
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
			this.settingValueManuallyIgnoringEvent = true;
			switch (columnNameChanged) {
				case "colMarketName":
					if (string.IsNullOrEmpty(cellValueAsString) != null) {
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
			this.settingValueManuallyIgnoringEvent = false;
			if (string.IsNullOrEmpty(errormsg) == false) {
				string msg = errormsg + headerText;
				Assembler.PopupException(msg + " /dgMarketName_CellValueChanged");
				return;
			}
			this.marketInfoRepository.Serialize();
		}
		void dgClearingTimespans_SelectionChanged(object sender, EventArgs e) {
			this.lnkIntradayInterruptsDelete.Enabled = false;
			if (this.settingValueManuallyIgnoringEvent) return;
			if (this.dgClearingTimespans.SelectedRows.Count == 0) return;
			MarketClearingTimespan closedHour = (MarketClearingTimespan)this.dgClearingTimespans.SelectedRows[0].Tag;
			if (closedHour == null) return;
			this.lnkIntradayInterruptsDelete.Enabled = true;
		}
		void dgClearingTimespans_CellValueChanged(object sender, DataGridViewCellEventArgs e) {
			if (this.settingValueManuallyIgnoringEvent) return;
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
			this.settingValueManuallyIgnoringEvent = true;
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
						List<DayOfWeek> dowsParsed = RepositoryCustomMarketInfo.ParseDaysOfWeekCsv(cellValueAsString, ", ");
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
			this.settingValueManuallyIgnoringEvent = false;
			if (string.IsNullOrEmpty(errormsg) == false) {
				Assembler.PopupException(errormsg);
			}
			this.marketInfoRepository.Serialize();
		}
		void lnkIntradayInterruptsDelete_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {

		}
		void dgShortDays_SelectionChanged(object sender, EventArgs e) {
			if (this.settingValueManuallyIgnoringEvent) return;
			this.lnkShortDaysDelete.Enabled = (this.dgShortDays.SelectedRows.Count > 0) ? true : false;
		}
		void lnkShortDaysDelete_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {

		}
		void dgHolidays_SelectionChanged(object sender, EventArgs e) {
			if (this.settingValueManuallyIgnoringEvent) return;
			this.lnkHolidaysDelete.Enabled = (this.dgHolidays.SelectedRows.Count > 0) ? true : false;
		}
		void lnkHolidaysDelete_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {

		}
		void cbxMarketTimeZone_SelectedIndexChanged(object sender, EventArgs e) {
			if (this.settingValueManuallyIgnoringEvent) return;
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
			if (this.settingValueManuallyIgnoringEvent) return;
			this.settingValueManuallyIgnoringEvent = true;
			string cellValueAsString = this.txtMarketDaysOfWeek.Text;
			try {
				List<DayOfWeek> dowsParsed = RepositoryCustomMarketInfo.ParseDaysOfWeekCsv(cellValueAsString, ", ");
				this.dataSource.MarketInfo.DaysOfWeekOpen = dowsParsed;
			} catch (Exception ex) {
				string errormsg = "InputAsString[" + cellValueAsString + "] " + ex.Message
					+ ", back to old value [" + this.dataSource.MarketInfo.DaysOfWeekOpenAsString + "]";
				Assembler.PopupException(errormsg + " /txtMarketDaysOfWeek_Validating", ex);
			}
			this.txtMarketDaysOfWeek.Text = this.dataSource.MarketInfo.DaysOfWeekOpenAsString;
			this.settingValueManuallyIgnoringEvent = false;
			this.marketInfoRepository.Serialize();
		}
		void txtMarketServerClose_Validating(object sender, CancelEventArgs e) {
			if (this.settingValueManuallyIgnoringEvent) return;
			this.settingValueManuallyIgnoringEvent = true;
			string cellValueAsString = this.txtMarketServerClose.Text;
			try {
				DateTime newTime = DateTime.Parse(cellValueAsString);
				this.dataSource.MarketInfo.MarketCloseServerTime = newTime;
			} catch (Exception ex) {
				string errormsg = "InputAsString[" + cellValueAsString + "] " + ex.Message
					+ ", back to old value [" + this.dataSource.MarketInfo.MarketOpenServerTimeAsString + "]";
				Assembler.PopupException(errormsg + " /txtMarketServerClose_Validating", ex);
			}
			this.txtMarketServerClose.Text = this.dataSource.MarketInfo.MarketOpenServerTimeAsString;
			this.settingValueManuallyIgnoringEvent = false;
			this.marketInfoRepository.Serialize();
		}
		void txtMarketServerOpen_Validating(object sender, CancelEventArgs e) {
			if (this.settingValueManuallyIgnoringEvent) return;
			this.settingValueManuallyIgnoringEvent = true;
			string cellValueAsString = this.txtMarketServerOpen.Text;
			try {
				DateTime newTime = DateTime.Parse(cellValueAsString);
				this.dataSource.MarketInfo.MarketOpenServerTime = newTime;
			} catch (Exception ex) {
				string errormsg = "InputAsString[" + cellValueAsString + "] " + ex.Message
					+ ", back to old value [" + this.dataSource.MarketInfo.MarketCloseServerTimeAsString + "]";
				Assembler.PopupException(errormsg + " txtMarketServerOpen_Validating", ex);
			}
			this.txtMarketServerOpen.Text = this.dataSource.MarketInfo.MarketCloseServerTimeAsString;
			this.settingValueManuallyIgnoringEvent = false;
			this.marketInfoRepository.Serialize();
		}
	}
}