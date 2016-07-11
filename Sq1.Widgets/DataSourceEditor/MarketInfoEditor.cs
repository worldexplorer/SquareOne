using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
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
			MarketInfo marketInfo_fromDataSource = this.dataSource.MarketInfo;
			this.populateDataGrid_MarketNames					(marketInfo_fromDataSource);
			this.populateAllDataGrids_withMarketInfo			(marketInfo_fromDataSource);
		}

		void populateAllDataGrids_withMarketInfo(MarketInfo marketInfo_fromDataSource_orJustViewingBySingleClick) {
			this.populateDataGrid_MarketInterruptionsIntraday	(marketInfo_fromDataSource_orJustViewingBySingleClick);
			this.populateDataGrid_MarketShortDays				(marketInfo_fromDataSource_orJustViewingBySingleClick);
			this.populateDataGrid_MarketHolidays				(marketInfo_fromDataSource_orJustViewingBySingleClick);
			this.populateTexts_MarketOpenClose					(marketInfo_fromDataSource_orJustViewingBySingleClick);
			this.populateSelector_TimeZones						(marketInfo_fromDataSource_orJustViewingBySingleClick);
		}
		void populateDataGrid_MarketNames(MarketInfo marketInfo_fromDataSource_orJustViewingBySingleClick) {
			if (this.marketInfoRepository == null) {
				string msg = "AVOIDING_NPE this.marketInfoRepository=null";
				Sq1.Core.Assembler.PopupException(msg, null, false);
				return;
			}

			this.ignoreSelectionEventDuringPopulate = true;
			this.dgMarketName.Rows.Clear();
			foreach (MarketInfo marketInfo_each in this.marketInfoRepository.MarketsByName.Values) {
				int index = this.dgMarketName.Rows.Add(new object[] {
					marketInfo_each.Name,
					this.dataSourceRepository.SameMarketInfoInHowManyDataSources(marketInfo_each)
				});
				this.dgMarketName.Rows[index].Tag = marketInfo_each;
				if (marketInfo_each == marketInfo_fromDataSource_orJustViewingBySingleClick) {
					this.dgMarketName.Rows[index].Selected = true;
				}
			}
			this.ignoreSelectionEventDuringPopulate = false;
		}
		void populateDataGrid_MarketInterruptionsIntraday(MarketInfo marketInfo_fromDataSource_orJustViewingBySingleClick) {
			this.ignoreSelectionEventDuringPopulate = true;
			this.dgClearingTimespans.Rows.Clear();
			base.BackColor = System.Drawing.Color.White;
			if (	marketInfo_fromDataSource_orJustViewingBySingleClick					!= null &&
					marketInfo_fromDataSource_orJustViewingBySingleClick.ClearingTimespans	!= null) {
				foreach (MarketClearingTimespan clearingTimespan_each in marketInfo_fromDataSource_orJustViewingBySingleClick.ClearingTimespans) {
					int index = this.dgClearingTimespans.Rows.Add(new object[] {
						clearingTimespan_each.SuspendServerTimeOfDay_asString,
						clearingTimespan_each.ResumeServerTimeOfDay_asString,
						clearingTimespan_each.DaysOfWeekWhenClearingHappens_asString
					});
					this.dgClearingTimespans.Rows[index].Tag = clearingTimespan_each;
				}
			}
			this.ignoreSelectionEventDuringPopulate = false;
		}
		void populateDataGrid_MarketShortDays(MarketInfo marketInfo_fromDataSource_orJustViewingBySingleClick) {
			this.ignoreSelectionEventDuringPopulate = true;
			this.dgShortDays.Rows.Clear();
			if (	marketInfo_fromDataSource_orJustViewingBySingleClick			!= null &&
					marketInfo_fromDataSource_orJustViewingBySingleClick.ShortDays	!= null) {
				foreach (MarketShortDay shortDay in marketInfo_fromDataSource_orJustViewingBySingleClick.ShortDays) {
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
		void populateDataGrid_MarketHolidays(MarketInfo marketInfo_fromDataSource_orJustViewingBySingleClick) {
			this.ignoreSelectionEventDuringPopulate = true;
			this.dgHolidays.Rows.Clear();
			if (	marketInfo_fromDataSource_orJustViewingBySingleClick			!= null &&
					marketInfo_fromDataSource_orJustViewingBySingleClick.ShortDays	!= null) {
				foreach (DateTime dateTimeHoliday_each in marketInfo_fromDataSource_orJustViewingBySingleClick.HolidaysYMD000) {
					int index = this.dgHolidays.Rows.Add(new object[] {
					dateTimeHoliday_each.ToString("dd MMM")
				});
					this.dgHolidays.Rows[index].Tag = dateTimeHoliday_each;
				}
			}
			this.ignoreSelectionEventDuringPopulate = false;
		}
		void populateTexts_MarketOpenClose(MarketInfo marketInfo_fromDataSource_orJustViewingBySingleClick) {
			if (marketInfo_fromDataSource_orJustViewingBySingleClick == null) return;
			this.txtMarketServerOpen	.Text = marketInfo_fromDataSource_orJustViewingBySingleClick.MarketOpen_serverTime.ToString("HH:mm");
			this.txtMarketServerClose	.Text = marketInfo_fromDataSource_orJustViewingBySingleClick.MarketClose_serverTime.ToString("HH:mm");
			this.txtMarketDaysOfWeek	.Text = marketInfo_fromDataSource_orJustViewingBySingleClick.DaysOfWeekOpenAsString;
		}
		void populateSelector_TimeZones(MarketInfo marketInfo_fromDataSource_orJustViewingBySingleClick) {
			if (this.marketInfoRepository == null) {
				string msg = "AVOIDING_NPE this.marketInfoRepository=null";
				Sq1.Core.Assembler.PopupException(msg, null, false);
				return;
			}
			this.ignoreSelectionEventDuringPopulate = true;
			this.cbxMarketTimeZone.Items.Clear();
			Dictionary<string, string> tzReadableNames_withUTC = this.marketInfoRepository.TimeZonesWithUTC;
			List<string> sortedTimeZones = new List<string>(tzReadableNames_withUTC.Keys);
			sortedTimeZones.Sort();

			string tzName_inDataSource = "UNKNOWN___THIS_WILL_NEVER_MATCH";
			if (	marketInfo_fromDataSource_orJustViewingBySingleClick				!= null &&
					marketInfo_fromDataSource_orJustViewingBySingleClick.TimeZoneName	!= null) {
				tzName_inDataSource = marketInfo_fromDataSource_orJustViewingBySingleClick.TimeZoneName;
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