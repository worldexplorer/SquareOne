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
		RepositorySerializerMarketInfo marketInfoRepository;
		RepositoryJsonDataSource dataSourceRepository;
		public MarketInfoEditor() {
			this.ignoreSelectionEventDuringPopulate = true;
			this.InitializeComponent();
			this.ignoreSelectionEventDuringPopulate = false;
		}
		void MarketInfoEditor_Load(object sender, EventArgs e) {
			//return;
			//if (base.DesignMode) return;
			// DOESNT_RESET_SELECTED_INDEX_TO_ZERO_DESPITE_LOOKS_CLOWNISH DockContent.Shows() resets SelectedIndex...
			this.populateMarketFromDataSource();
			this.dgMarketName.SelectionChanged += new System.EventHandler(this.dgMarketName_SelectionChanged);
		}
		public void Initialize(DataSource dataSource, RepositoryJsonDataSource dataSourceRepository, RepositorySerializerMarketInfo marketInfoRepository) {
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
			this.ignoreSelectionEventDuringPopulate = true;
			this.dgMarketName.Rows.Clear();
			foreach (MarketInfo marketInfo in this.marketInfoRepository.Entity.Values) {
				int index = this.dgMarketName.Rows.Add(new object[] {
					marketInfo.Name,
					this.dataSourceRepository.UsedTimes(marketInfo)
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
			this.txtMarketServerOpen.Text = this.dataSource.MarketInfo.MarketOpenServerTime.ToString("HH:mm");
			this.txtMarketServerClose.Text = this.dataSource.MarketInfo.MarketCloseServerTime.ToString("HH:mm");
			this.txtMarketDaysOfWeek.Text = this.dataSource.MarketInfo.DaysOfWeekOpenAsString;
		}
		void populateTimeZonesSelector() {
			this.ignoreSelectionEventDuringPopulate = true;
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
			this.ignoreSelectionEventDuringPopulate = false;
		}
	}
}