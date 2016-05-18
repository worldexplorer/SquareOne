using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Windows.Forms;

using Sq1.Core.DataFeed;
using Sq1.Core.DataTypes;
using Sq1.Core.Repositories;

namespace Sq1.Widgets.DataSourceEditor {
	[Designer("System.Windows.Forms.Design.ParentControlDesigner, System.Design", typeof(IDesigner))]
	public partial class MarketInfoEditor : UserControl {
		bool ignoreSelectionEventDuringPopulate;
		DataSource dataSource;
		RepositorySerializerMarketInfos marketInfoRepository;
		RepositoryJsonDataSources dataSourceRepository;
		public MarketInfoEditor() {
			this.ignoreSelectionEventDuringPopulate = true;
			this.InitializeComponent();
			this.ignoreSelectionEventDuringPopulate = false;
		}
		void MarketInfoEditor_Load(object sender, EventArgs e) {
			//return;
			//if (base.DesignMode) return;
			// DOESNT_RESET_SELECTED_INDEX_TO_ZERO_DESPITE_LOOKS_CLOWNISH DockContent.Shows() resets SelectedIndex...
			this.populateMarket_fromDataSource();
			this.dgMarketName.SelectionChanged += new System.EventHandler(this.dgMarketName_SelectionChanged);
		}
		public void Initialize(DataSource dataSource, RepositoryJsonDataSources dataSourceRepository, RepositorySerializerMarketInfos marketInfoRepository) {
			if (base.DesignMode) return;
			this.dataSource = dataSource;
			this.dataSourceRepository = dataSourceRepository;
			this.marketInfoRepository = marketInfoRepository;
			this.populateMarket_fromDataSource();
		}
		public void populateMarket_fromDataSource() {
			if (base.DesignMode) return;
			if (this.dataSource == null) return;
			this.populateMarketNamesDataGrid();
			this.populateMarketIntradayInterruptsDataGrid();
			this.populateMarketShortDaysDataGrid();
			this.populateMarketHolidaysDataGrid();
			this.populateMarketOpenCloseTexts();
			this.populateTimeZonesSelector();
		}
		void populateMarketNamesDataGrid() {
			if (this.marketInfoRepository == null) {
				string msg = "AVOIDING_NPE this.marketInfoRepository=null";
				Sq1.Core.Assembler.PopupException(msg, null, false);
				return;
			}

			this.ignoreSelectionEventDuringPopulate = true;
			this.dgMarketName.Rows.Clear();
			foreach (MarketInfo marketInfo in this.marketInfoRepository.MarketsByName.Values) {
				int index = this.dgMarketName.Rows.Add(new object[] {
					marketInfo.Name,
					this.dataSourceRepository.SameMarketInfoInHowManyDataSources(marketInfo)
				});
				this.dgMarketName.Rows[index].Tag = marketInfo;
				if (marketInfo == this.dataSource.MarketInfo) {
					this.dgMarketName.Rows[index].Selected = true;
				}
			}
			this.ignoreSelectionEventDuringPopulate = false;
		}
		void populateMarketIntradayInterruptsDataGrid() {
			this.ignoreSelectionEventDuringPopulate = true;
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
			this.ignoreSelectionEventDuringPopulate = false;
		}
		void populateMarketShortDaysDataGrid() {
			this.ignoreSelectionEventDuringPopulate = true;
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
			this.ignoreSelectionEventDuringPopulate = false;
		}
		void populateMarketHolidaysDataGrid() {
			this.ignoreSelectionEventDuringPopulate = true;
			this.dgHolidays.Rows.Clear();
			if (this.dataSource.MarketInfo != null && this.dataSource.MarketInfo.ShortDays != null) {
				foreach (DateTime dateTime in this.dataSource.MarketInfo.HolidaysYMD000) {
					int index = this.dgHolidays.Rows.Add(new object[] {
					dateTime.ToString("dd MMM")
				});
					this.dgHolidays.Rows[index].Tag = dateTime;
				}
			}
			this.ignoreSelectionEventDuringPopulate = false;
		}
		void populateMarketOpenCloseTexts() {
			if (this.dataSource.MarketInfo == null) return;
			this.txtMarketServerOpen.Text = this.dataSource.MarketInfo.MarketOpen_serverTime.ToString("HH:mm");
			this.txtMarketServerClose.Text = this.dataSource.MarketInfo.MarketClose_serverTime.ToString("HH:mm");
			this.txtMarketDaysOfWeek.Text = this.dataSource.MarketInfo.DaysOfWeekOpenAsString;
		}
		void populateTimeZonesSelector() {
			if (this.marketInfoRepository == null) {
				string msg = "AVOIDING_NPE this.marketInfoRepository=null";
				Sq1.Core.Assembler.PopupException(msg, null, false);
				return;
			}
			this.ignoreSelectionEventDuringPopulate = true;
			this.cbxMarketTimeZone.Items.Clear();
			Dictionary<string, string> tzReadableNames_withUTC = this.marketInfoRepository.TimeZonesWithUTC;
			var sortedTimeZones = tzReadableNames_withUTC.Keys.ToList();	//using System.Linq;
			sortedTimeZones.Sort();

			string tzName_inDataSource = "UNKNOWN___THIS_WILL_NEVER_MATCH";
			if (this.dataSource.MarketInfo != null && this.dataSource.MarketInfo.TimeZoneName != null) {
				tzName_inDataSource = this.dataSource.MarketInfo.TimeZoneName;
			}

			foreach (string key in sortedTimeZones) {
				int index = this.cbxMarketTimeZone.Items.Add(key);

				string tzName_each = tzReadableNames_withUTC[key];
				if (tzName_each != tzName_inDataSource) continue;

				this.cbxMarketTimeZone.SelectedIndex = index;
			}
			this.ignoreSelectionEventDuringPopulate = false;
		}
	}
}