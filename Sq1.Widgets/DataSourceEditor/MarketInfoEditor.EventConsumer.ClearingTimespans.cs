using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

using Sq1.Core;
using Sq1.Core.DataTypes;
using Sq1.Core.Repositories;

namespace Sq1.Widgets.DataSourceEditor {
	public partial class MarketInfoEditor {
		void dgClearingTimespans_SelectionChanged(object sender, EventArgs e) {
			this.lnkClearingTimespanDelete.Enabled = false;
			if (this.ignoreSelectionEventDuringPopulate) return;
			if (this.dgClearingTimespans.SelectedRows.Count == 0) return;

			this.clearingTimespan_selectedRowTag = this.dgClearingTimespans.SelectedRows[0].Tag as MarketClearingTimespan;
			if (	this.clearingTimespan_selectedRowTag == null
				 || this.clearingTimespan_selectedRowTag != this.clearingTimespanEditing) {
				this.clearingTimespanEditing = new MarketClearingTimespan();
			}
			if (this.clearingTimespan_selectedRowTag == null) return;

			this.clearingTimespanEditing.SuspendServerTimeOfDay = this.clearingTimespan_selectedRowTag.SuspendServerTimeOfDay;
			this.clearingTimespanEditing.ResumeServerTimeOfDay	= this.clearingTimespan_selectedRowTag. ResumeServerTimeOfDay;

			this.lnkClearingTimespanDelete.Enabled = true;
		}

		int clearingTimespanEditing_rowIndexPrev = 0;
		int clearingTimespanEditing_rowIndex = 0;
		MarketClearingTimespan clearingTimespan_selectedRowTag = null;
		MarketClearingTimespan clearingTimespanEditing = null;

		bool dgClearingTimespan_AddDelete_selectedRowTag() {
			bool shouldSerialize_rePopuplate = false;

			if (this.clearingTimespanEditing == null) {
				string err = "ASSIGN_ON_ROW_CHANGED__this.clearingTimespanEditing==null";
				Assembler.PopupException(err);
				return shouldSerialize_rePopuplate;
			}
			var row	= this.dgClearingTimespans.Rows[clearingTimespanEditing_rowIndex];
			MarketInfo imEditing = this.dataSource.MarketInfo;
			List<MarketClearingTimespan> spans = imEditing.ClearingTimespans;

			bool rowChanged = this.clearingTimespanEditing_rowIndex != this.clearingTimespanEditing_rowIndexPrev;

			bool bothEmpty_shouldBeDeleted_ifExists =
				this.clearingTimespanEditing.SuspendServerTimeOfDay == DateTime.MinValue &&
				this.clearingTimespanEditing. ResumeServerTimeOfDay == DateTime.MinValue;

			bool bothFilled_shouldBeAdded_ifDoesntExist =
				this.clearingTimespanEditing.SuspendServerTimeOfDay != DateTime.MinValue &&
				this.clearingTimespanEditing. ResumeServerTimeOfDay != DateTime.MinValue;

			if (bothFilled_shouldBeAdded_ifDoesntExist) {
				base.BackColor = Color.White;
				if (	this.clearingTimespan_selectedRowTag != null 
					 && this.clearingTimespan_selectedRowTag != this.clearingTimespanEditing) {
					string msg_updated = "BOTH_VALID__WILL_UPDATE_UPSTACK__WITH_WHATEVER_USER_TYPED__MAY_BE_NOTHING_CHANGED";
					this.clearingTimespan_selectedRowTag.AbsorbFrom(this.clearingTimespanEditing);
					shouldSerialize_rePopuplate = true;
					return shouldSerialize_rePopuplate;
				}
				string msg_adding = "ADDING_TO_MarketInfo.ClearingTimespans_NEW_ROW[" + this.clearingTimespanEditing + "]";
				row.Tag = this.clearingTimespanEditing;
				if (spans.Contains(this.clearingTimespanEditing) == false) {
					this.dataSource.MarketInfo.ClearingTimespans.Add(this.clearingTimespanEditing);
				} else {
					msg_adding = "YOU_CLEARED_OVERLAPS__SO_IT_ALREADY_EXISTS?? YOU_DIDNT_ASSIGN_this.clearingTimespanEditing=row.Tag " + msg_adding;
					Assembler.PopupException(msg_adding);
					base.BackColor = Color.Yellow;
				}
				shouldSerialize_rePopuplate = true;
			}

			if (bothEmpty_shouldBeDeleted_ifExists) {
				if (this.clearingTimespan_selectedRowTag == null) {
					string msg_nothingUpdated = "TYPED_UPARSEABLE_TO_DATE__NOTHING_ASSIGNED";
					return shouldSerialize_rePopuplate;
				}
				if (spans.Contains(this.clearingTimespan_selectedRowTag)) {
					base.BackColor = Color.AntiqueWhite;
					this.dataSource.MarketInfo.ClearingTimespans.Remove(this.clearingTimespan_selectedRowTag);
					this.marketInfoRepository.Serialize();
				} else {
					string msg = "ON_SELECTION_CHANGED_SET_this.clearingTimespan_currentlyInTagOfSelectedRow";
					Assembler.PopupException(msg);
					base.BackColor = Color.Yellow;
				}
				shouldSerialize_rePopuplate = true;
				row.Tag = null;
			}

			//if (rowChanged == false) {
			//    row.Cells[0].Value = this.clearingTimespanEditing.SuspendServerTimeOfDay_asString;
			//    row.Cells[1].Value = this.clearingTimespanEditing. ResumeServerTimeOfDay_asString;
			//} else {
			//    shouldSerialize_rePopuplateFromJson = true;
			//}

			return shouldSerialize_rePopuplate;
		}

		void dgClearingTimespans_CellValueChanged(object sender, DataGridViewCellEventArgs e) {
			if (this.ignoreSelectionEventDuringPopulate) return;
			if (e.	 RowIndex < 0) return;
			if (e.ColumnIndex < 0) return;
			string errormsg = "";
			int clearingTimespanEditing_rowIndex = e.RowIndex;
			var column	= this.dgClearingTimespans.Columns	[e.ColumnIndex];
			var row		= this.dgClearingTimespans.Rows		[e.RowIndex];
			string headerText = column.HeaderText;

			this.clearingTimespan_selectedRowTag = row.Tag as MarketClearingTimespan;
			if (this.clearingTimespan_selectedRowTag == null) {
				//this.clearingTimespanEditing = null;
			}
			if (this.clearingTimespanEditing == null) {
				this.clearingTimespanEditing = new MarketClearingTimespan();
			}

			DataGridViewCell cell = this.dgClearingTimespans[e.ColumnIndex, e.RowIndex];
			string cellValueAsString = (cell.Value != null) ? cell.Value.ToString() : "";
			string columnNameChanged = column.Name;
			this.ignoreSelectionEventDuringPopulate = true;
			switch (columnNameChanged) {
				case "colClearingTimespansSuspends":
					try {
						DateTime newTime = DateTime.Parse(cellValueAsString);
						this.clearingTimespanEditing.SuspendServerTimeOfDay = newTime;
						base.BackColor = Color.White;
					} catch (Exception ex) {
						errormsg = "MUST_BE_HH:mm_SuspendTime[" + cellValueAsString + "] "
						    + "<=[" + this.clearingTimespanEditing.SuspendServerTimeOfDay_asString + "]"
							+ ex.Message;
						this.clearingTimespanEditing.SuspendServerTimeOfDay = DateTime.MinValue;
						base.BackColor = Assembler.InstanceInitialized.ColorBackgroundRed_forPositionLoss;
					}
					//cell.Value = this.clearingTimespanEditing.SuspendServerTimeOfDayAsString;
					break;
				case "colClearingTimespansResumes":
					try {
						DateTime newTime = DateTime.Parse(cellValueAsString);
						this.clearingTimespanEditing.ResumeServerTimeOfDay = newTime;
						base.BackColor = Color.White;
					} catch (Exception ex) {
						errormsg = "MUST_BE_HH:mm_ResumeTime[" + cellValueAsString + "] "
						    + "<=[" + this.clearingTimespanEditing.ResumeServerTimeOfDay_asString + "]"
							+ ex.Message;
						this.clearingTimespanEditing.ResumeServerTimeOfDay = DateTime.MinValue;
						base.BackColor = Assembler.InstanceInitialized.ColorBackgroundRed_forPositionLoss;
					}
					//cell.Value = this.clearingTimespanEditing.ResumeServerTimeOfDayAsString;
					break;
				case "colClearingTimespansDaysOfWeek":
					try {
						List<DayOfWeek> dowsParsed = RepositorySerializerMarketInfos.ParseDaysOfWeekCsv(cellValueAsString, ", ");
						this.clearingTimespanEditing.DaysOfWeekWhenClearingHappens = dowsParsed;
					} catch (Exception ex) {
						errormsg = columnNameChanged + ": cellValueAsString[" + cellValueAsString + "] " + ex.Message
						    + ", back to old value [" + this.clearingTimespanEditing.DaysOfWeekWhenClearingHappens_asString + "]";
						this.clearingTimespanEditing.DaysOfWeekWhenClearingHappens = new List<DayOfWeek>();
						base.BackColor = Assembler.InstanceInitialized.ColorBackgroundRed_forPositionLoss;
					}
					//cell.Value = this.clearingTimespanEditing.DaysOfWeekWhenClosingNotHappensAsString;
					break;
				default:
					errormsg = "no handler for field:[" + columnNameChanged + "]";
					break;
			}

			bool shouldSerialize_rePopuplate = this.dgClearingTimespan_AddDelete_selectedRowTag();
			this.clearingTimespanEditing_rowIndexPrev = this.clearingTimespanEditing_rowIndex;

			this.ignoreSelectionEventDuringPopulate = false;
			if (string.IsNullOrEmpty(errormsg) == false) {
				Assembler.PopupException(errormsg, null, false);
			}

			if (shouldSerialize_rePopuplate) {
				this.dataSourceRepository.SerializeAll();
				this.marketInfoRepository.Serialize();
				//this.populateDataGrid_MarketInterruptionsIntraday();
			}
		}

		void lnkClearingTimespanDelete_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
			if (this.dgClearingTimespans.SelectedRows.Count == 0) return;
			MarketClearingTimespan clearingTimespan_toDelete = this.dgClearingTimespans.SelectedRows[0].Tag as MarketClearingTimespan;
			if (clearingTimespan_toDelete == null) return;
			MarketInfo imEditing = this.dataSource.MarketInfo;
			List<MarketClearingTimespan> clearingTimespans = imEditing.ClearingTimespans;
			if (clearingTimespans.Contains(clearingTimespan_toDelete) == false) {
				string msg = "NOT_PRESENT_IN_dataSource.MarketInfo.ClearingTimespans: clearingTimespan_toDelete[" + clearingTimespan_toDelete + "]"
					+ " dataSource.MarketInfo[" + imEditing + "]";
				Assembler.PopupException(msg);
				return;
			}
			this.dataSource.MarketInfo.ClearingTimespans.Remove(this.clearingTimespan_selectedRowTag);
			this.marketInfoRepository.Serialize();
			//this.populateDataGrid_MarketInterruptionsIntraday();
		}
	}
}
