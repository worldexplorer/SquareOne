namespace Sq1.Widgets.DataSourceEditor {
	partial class MarketInfoEditor {
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (this.components != null)) {
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
			this.cbxMarketTimeZone = new System.Windows.Forms.ComboBox();
			this.txtMarketServerClose = new System.Windows.Forms.TextBox();
			this.txtMarketServerOpen = new System.Windows.Forms.TextBox();
			this.lnkMarketNameDelete = new System.Windows.Forms.LinkLabel();
			this.lblMarketServerClose = new System.Windows.Forms.Label();
			this.lblMarketServerOpen = new System.Windows.Forms.Label();
			this.lblMarketInfo = new System.Windows.Forms.Label();
			this.lblMarketTimeZone = new System.Windows.Forms.Label();
			this.dgMarketName = new System.Windows.Forms.DataGridView();
			this.colMarketName = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colMarketUsedTimes = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.dgHolidays = new System.Windows.Forms.DataGridView();
			this.colHolidaysDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.dgShortDays = new System.Windows.Forms.DataGridView();
			this.colShortDaysDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colShortDaysTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colShortDaysClose = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.lblShortDays = new System.Windows.Forms.Label();
			this.lblHolidays = new System.Windows.Forms.Label();
			this.lnkHolidaysDelete = new System.Windows.Forms.LinkLabel();
			this.lnkShortDaysDelete = new System.Windows.Forms.LinkLabel();
			this.dgClearingTimespans = new System.Windows.Forms.DataGridView();
			this.colClearingTimespansSuspends = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colClearingTimespansResumes = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colClearingTimespansDaysOfWeek = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.lblClearingTimespans = new System.Windows.Forms.Label();
			this.lnkIntradayInterruptsDelete = new System.Windows.Forms.LinkLabel();
			this.txtMarketDaysOfWeek = new System.Windows.Forms.TextBox();
			this.lblMarketDaysOfWeek = new System.Windows.Forms.Label();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
			this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
			this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
			this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
			this.tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
			this.tableLayoutPanel7 = new System.Windows.Forms.TableLayoutPanel();
			((System.ComponentModel.ISupportInitialize)(this.dgMarketName)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dgHolidays)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dgShortDays)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dgClearingTimespans)).BeginInit();
			this.tableLayoutPanel1.SuspendLayout();
			this.tableLayoutPanel2.SuspendLayout();
			this.tableLayoutPanel3.SuspendLayout();
			this.tableLayoutPanel4.SuspendLayout();
			this.tableLayoutPanel5.SuspendLayout();
			this.tableLayoutPanel6.SuspendLayout();
			this.tableLayoutPanel7.SuspendLayout();
			this.SuspendLayout();
			// 
			// cbxMarketTimeZone
			// 
			this.cbxMarketTimeZone.Dock = System.Windows.Forms.DockStyle.Fill;
			this.cbxMarketTimeZone.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbxMarketTimeZone.FormattingEnabled = true;
			this.cbxMarketTimeZone.Location = new System.Drawing.Point(245, 16);
			this.cbxMarketTimeZone.Name = "cbxMarketTimeZone";
			this.cbxMarketTimeZone.Size = new System.Drawing.Size(209, 21);
			this.cbxMarketTimeZone.TabIndex = 36;
			this.cbxMarketTimeZone.SelectedIndexChanged += new System.EventHandler(this.cbxMarketTimeZone_SelectedIndexChanged);
			// 
			// txtMarketServerClose
			// 
			this.txtMarketServerClose.Location = new System.Drawing.Point(53, 16);
			this.txtMarketServerClose.Name = "txtMarketServerClose";
			this.txtMarketServerClose.Size = new System.Drawing.Size(44, 20);
			this.txtMarketServerClose.TabIndex = 35;
			this.txtMarketServerClose.Validating += new System.ComponentModel.CancelEventHandler(this.txtMarketServerClose_Validating);
			// 
			// txtMarketServerOpen
			// 
			this.txtMarketServerOpen.Location = new System.Drawing.Point(3, 16);
			this.txtMarketServerOpen.Name = "txtMarketServerOpen";
			this.txtMarketServerOpen.Size = new System.Drawing.Size(44, 20);
			this.txtMarketServerOpen.TabIndex = 34;
			this.txtMarketServerOpen.Validating += new System.ComponentModel.CancelEventHandler(this.txtMarketServerOpen_Validating);
			// 
			// lnkMarketNameDelete
			// 
			this.lnkMarketNameDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lnkMarketNameDelete.AutoSize = true;
			this.lnkMarketNameDelete.Enabled = false;
			this.lnkMarketNameDelete.Location = new System.Drawing.Point(53, 0);
			this.lnkMarketNameDelete.Name = "lnkMarketNameDelete";
			this.lnkMarketNameDelete.Size = new System.Drawing.Size(15, 13);
			this.lnkMarketNameDelete.TabIndex = 33;
			this.lnkMarketNameDelete.TabStop = true;
			this.lnkMarketNameDelete.Text = "Delete";
			this.lnkMarketNameDelete.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkMarketNameDelete_LinkClicked);
			// 
			// lblMarketServerClose
			// 
			this.lblMarketServerClose.AutoSize = true;
			this.lblMarketServerClose.Location = new System.Drawing.Point(53, 0);
			this.lblMarketServerClose.Name = "lblMarketServerClose";
			this.lblMarketServerClose.Size = new System.Drawing.Size(33, 13);
			this.lblMarketServerClose.TabIndex = 31;
			this.lblMarketServerClose.Text = "Close";
			// 
			// lblMarketServerOpen
			// 
			this.lblMarketServerOpen.AutoSize = true;
			this.lblMarketServerOpen.Location = new System.Drawing.Point(3, 0);
			this.lblMarketServerOpen.Name = "lblMarketServerOpen";
			this.lblMarketServerOpen.Size = new System.Drawing.Size(33, 13);
			this.lblMarketServerOpen.TabIndex = 30;
			this.lblMarketServerOpen.Text = "Open";
			// 
			// lblMarketInfo
			// 
			this.lblMarketInfo.AutoSize = true;
			this.lblMarketInfo.Location = new System.Drawing.Point(3, 0);
			this.lblMarketInfo.Name = "lblMarketInfo";
			this.lblMarketInfo.Size = new System.Drawing.Size(40, 13);
			this.lblMarketInfo.TabIndex = 29;
			this.lblMarketInfo.Text = "Market";
			// 
			// lblMarketTimeZone
			// 
			this.lblMarketTimeZone.AutoSize = true;
			this.lblMarketTimeZone.Location = new System.Drawing.Point(245, 0);
			this.lblMarketTimeZone.Name = "lblMarketTimeZone";
			this.lblMarketTimeZone.Size = new System.Drawing.Size(177, 13);
			this.lblMarketTimeZone.TabIndex = 37;
			this.lblMarketTimeZone.Text = "TimeZone (rewinding time will pump streaming bar \"forever\")";
			// 
			// dgMarketName
			// 
			this.dgMarketName.AllowUserToOrderColumns = true;
			this.dgMarketName.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
			this.dgMarketName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.dgMarketName.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Sunken;
			this.dgMarketName.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.dgMarketName.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			this.dgMarketName.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgMarketName.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colMarketName,
            this.colMarketUsedTimes});
			this.dgMarketName.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgMarketName.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
			this.dgMarketName.Location = new System.Drawing.Point(3, 22);
			this.dgMarketName.MultiSelect = false;
			this.dgMarketName.Name = "dgMarketName";
			this.dgMarketName.RowHeadersVisible = false;
			this.dgMarketName.RowHeadersWidth = 5;
			this.dgMarketName.RowTemplate.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			this.dgMarketName.RowTemplate.DividerHeight = 1;
			this.dgMarketName.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dgMarketName.Size = new System.Drawing.Size(71, 139);
			this.dgMarketName.TabIndex = 38;
			this.dgMarketName.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgMarketName_CellValueChanged);
			// 
			// colMarketName
			// 
			this.colMarketName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.colMarketName.HeaderText = "Name";
			this.colMarketName.Name = "colMarketName";
			// 
			// colMarketUsedTimes
			// 
			this.colMarketUsedTimes.HeaderText = "Used";
			this.colMarketUsedTimes.Name = "colMarketUsedTimes";
			this.colMarketUsedTimes.ReadOnly = true;
			this.colMarketUsedTimes.Width = 33;
			// 
			// dgHolidays
			// 
			this.dgHolidays.BackgroundColor = System.Drawing.SystemColors.Control;
			this.dgHolidays.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.dgHolidays.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Sunken;
			this.dgHolidays.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
			dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.dgHolidays.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
			this.dgHolidays.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgHolidays.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colHolidaysDate});
			this.dgHolidays.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgHolidays.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
			this.dgHolidays.Location = new System.Drawing.Point(344, 22);
			this.dgHolidays.Name = "dgHolidays";
			this.dgHolidays.RowHeadersVisible = false;
			this.dgHolidays.RowTemplate.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			this.dgHolidays.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dgHolidays.Size = new System.Drawing.Size(110, 139);
			this.dgHolidays.TabIndex = 44;
			this.dgHolidays.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgHolidays_CellValueChanged);
			this.dgHolidays.SelectionChanged += new System.EventHandler(this.dgHolidays_SelectionChanged);
			// 
			// colHolidaysDate
			// 
			this.colHolidaysDate.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.colHolidaysDate.HeaderText = "Date";
			this.colHolidaysDate.Name = "colHolidaysDate";
			// 
			// dgShortDays
			// 
			this.dgShortDays.BackgroundColor = System.Drawing.SystemColors.Control;
			this.dgShortDays.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.dgShortDays.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Sunken;
			this.dgShortDays.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
			dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.dgShortDays.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
			this.dgShortDays.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgShortDays.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colShortDaysDate,
            this.colShortDaysTime,
            this.colShortDaysClose});
			this.dgShortDays.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgShortDays.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
			this.dgShortDays.Location = new System.Drawing.Point(230, 22);
			this.dgShortDays.Name = "dgShortDays";
			this.dgShortDays.RowHeadersVisible = false;
			this.dgShortDays.RowTemplate.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			this.dgShortDays.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dgShortDays.Size = new System.Drawing.Size(108, 139);
			this.dgShortDays.TabIndex = 43;
			this.dgShortDays.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgShortDays_CellValueChanged);
			this.dgShortDays.SelectionChanged += new System.EventHandler(this.dgShortDays_SelectionChanged);
			// 
			// colShortDaysDate
			// 
			this.colShortDaysDate.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.colShortDaysDate.HeaderText = "Date";
			this.colShortDaysDate.Name = "colShortDaysDate";
			// 
			// colShortDaysTime
			// 
			this.colShortDaysTime.HeaderText = "Open";
			this.colShortDaysTime.Name = "colShortDaysTime";
			this.colShortDaysTime.Width = 45;
			// 
			// colShortDaysClose
			// 
			this.colShortDaysClose.HeaderText = "Close";
			this.colShortDaysClose.Name = "colShortDaysClose";
			this.colShortDaysClose.Width = 45;
			// 
			// lblShortDays
			// 
			this.lblShortDays.AutoSize = true;
			this.lblShortDays.Location = new System.Drawing.Point(3, 0);
			this.lblShortDays.Name = "lblShortDays";
			this.lblShortDays.Size = new System.Drawing.Size(56, 13);
			this.lblShortDays.TabIndex = 39;
			this.lblShortDays.Text = "ShortDays";
			// 
			// lblHolidays
			// 
			this.lblHolidays.AutoSize = true;
			this.lblHolidays.Location = new System.Drawing.Point(3, 0);
			this.lblHolidays.Name = "lblHolidays";
			this.lblHolidays.Size = new System.Drawing.Size(47, 13);
			this.lblHolidays.TabIndex = 40;
			this.lblHolidays.Text = "Holidays";
			// 
			// lnkHolidaysDelete
			// 
			this.lnkHolidaysDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lnkHolidaysDelete.AutoSize = true;
			this.lnkHolidaysDelete.Enabled = false;
			this.lnkHolidaysDelete.Location = new System.Drawing.Point(69, 0);
			this.lnkHolidaysDelete.Name = "lnkHolidaysDelete";
			this.lnkHolidaysDelete.Size = new System.Drawing.Size(38, 13);
			this.lnkHolidaysDelete.TabIndex = 42;
			this.lnkHolidaysDelete.TabStop = true;
			this.lnkHolidaysDelete.Text = "Delete";
			this.lnkHolidaysDelete.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkHolidaysDelete_LinkClicked);
			// 
			// lnkShortDaysDelete
			// 
			this.lnkShortDaysDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lnkShortDaysDelete.AutoSize = true;
			this.lnkShortDaysDelete.Enabled = false;
			this.lnkShortDaysDelete.Location = new System.Drawing.Point(73, 0);
			this.lnkShortDaysDelete.Name = "lnkShortDaysDelete";
			this.lnkShortDaysDelete.Size = new System.Drawing.Size(32, 13);
			this.lnkShortDaysDelete.TabIndex = 41;
			this.lnkShortDaysDelete.TabStop = true;
			this.lnkShortDaysDelete.Text = "Delete";
			this.lnkShortDaysDelete.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkShortDaysDelete_LinkClicked);
			// 
			// dgClearingTimespans
			// 
			this.dgClearingTimespans.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
			this.dgClearingTimespans.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.dgClearingTimespans.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Sunken;
			this.dgClearingTimespans.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
			dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.dgClearingTimespans.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
			this.dgClearingTimespans.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgClearingTimespans.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colClearingTimespansSuspends,
            this.colClearingTimespansResumes,
            this.colClearingTimespansDaysOfWeek});
			this.dgClearingTimespans.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgClearingTimespans.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
			this.dgClearingTimespans.Location = new System.Drawing.Point(80, 22);
			this.dgClearingTimespans.MultiSelect = false;
			this.dgClearingTimespans.Name = "dgClearingTimespans";
			this.dgClearingTimespans.RowHeadersVisible = false;
			this.dgClearingTimespans.RowTemplate.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			this.dgClearingTimespans.RowTemplate.DividerHeight = 1;
			this.dgClearingTimespans.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dgClearingTimespans.Size = new System.Drawing.Size(144, 139);
			this.dgClearingTimespans.TabIndex = 47;
			this.dgClearingTimespans.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgClearingTimespans_CellValueChanged);
			this.dgClearingTimespans.SelectionChanged += new System.EventHandler(this.dgClearingTimespans_SelectionChanged);
			// 
			// colClearingTimespansSuspends
			// 
			this.colClearingTimespansSuspends.HeaderText = "Closes";
			this.colClearingTimespansSuspends.Name = "colClearingTimespansSuspends";
			this.colClearingTimespansSuspends.Width = 45;
			// 
			// colClearingTimespansResumes
			// 
			this.colClearingTimespansResumes.HeaderText = "Resumes";
			this.colClearingTimespansResumes.Name = "colClearingTimespansResumes";
			this.colClearingTimespansResumes.Width = 45;
			// 
			// colClearingTimespansDaysOfWeek
			// 
			this.colClearingTimespansDaysOfWeek.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.colClearingTimespansDaysOfWeek.HeaderText = "DaysOfWeek";
			this.colClearingTimespansDaysOfWeek.Name = "colClearingTimespansDaysOfWeek";
			// 
			// lblClearingTimespans
			// 
			this.lblClearingTimespans.AutoSize = true;
			this.lblClearingTimespans.Location = new System.Drawing.Point(3, 0);
			this.lblClearingTimespans.Name = "lblClearingTimespans";
			this.lblClearingTimespans.Size = new System.Drawing.Size(99, 13);
			this.lblClearingTimespans.TabIndex = 45;
			this.lblClearingTimespans.Text = "Clearing Timespans";
			// 
			// lnkIntradayInterruptsDelete
			// 
			this.lnkIntradayInterruptsDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lnkIntradayInterruptsDelete.AutoSize = true;
			this.lnkIntradayInterruptsDelete.Enabled = false;
			this.lnkIntradayInterruptsDelete.Location = new System.Drawing.Point(118, 0);
			this.lnkIntradayInterruptsDelete.Name = "lnkIntradayInterruptsDelete";
			this.lnkIntradayInterruptsDelete.Size = new System.Drawing.Size(23, 13);
			this.lnkIntradayInterruptsDelete.TabIndex = 46;
			this.lnkIntradayInterruptsDelete.TabStop = true;
			this.lnkIntradayInterruptsDelete.Text = "Delete";
			this.lnkIntradayInterruptsDelete.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkIntradayInterruptsDelete_LinkClicked);
			// 
			// txtMarketDaysOfWeek
			// 
			this.txtMarketDaysOfWeek.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtMarketDaysOfWeek.Location = new System.Drawing.Point(103, 16);
			this.txtMarketDaysOfWeek.Name = "txtMarketDaysOfWeek";
			this.txtMarketDaysOfWeek.Size = new System.Drawing.Size(136, 20);
			this.txtMarketDaysOfWeek.TabIndex = 49;
			this.txtMarketDaysOfWeek.Validating += new System.ComponentModel.CancelEventHandler(this.txtMarketDaysOfWeek_Validating);
			// 
			// lblMarketDaysOfWeek
			// 
			this.lblMarketDaysOfWeek.AutoSize = true;
			this.lblMarketDaysOfWeek.Location = new System.Drawing.Point(103, 0);
			this.lblMarketDaysOfWeek.Name = "lblMarketDaysOfWeek";
			this.lblMarketDaysOfWeek.Size = new System.Drawing.Size(100, 13);
			this.lblMarketDaysOfWeek.TabIndex = 48;
			this.lblMarketDaysOfWeek.Text = "DaysOfWeek (type Mon,Tue,Wed)";
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 4;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.97531F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.02469F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
			this.tableLayoutPanel1.Controls.Add(this.dgMarketName, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.dgClearingTimespans, 1, 1);
			this.tableLayoutPanel1.Controls.Add(this.dgShortDays, 2, 1);
			this.tableLayoutPanel1.Controls.Add(this.dgHolidays, 3, 1);
			this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel4, 2, 0);
			this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel5, 3, 0);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 2;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 19F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(457, 164);
			this.tableLayoutPanel1.TabIndex = 50;
			// 
			// tableLayoutPanel2
			// 
			this.tableLayoutPanel2.ColumnCount = 2;
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel2.Controls.Add(this.lblMarketInfo, 0, 0);
			this.tableLayoutPanel2.Controls.Add(this.lnkMarketNameDelete, 1, 0);
			this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
			this.tableLayoutPanel2.Name = "tableLayoutPanel2";
			this.tableLayoutPanel2.RowCount = 1;
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel2.Size = new System.Drawing.Size(71, 13);
			this.tableLayoutPanel2.TabIndex = 48;
			// 
			// tableLayoutPanel3
			// 
			this.tableLayoutPanel3.ColumnCount = 2;
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 110F));
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel3.Controls.Add(this.lblClearingTimespans, 0, 0);
			this.tableLayoutPanel3.Controls.Add(this.lnkIntradayInterruptsDelete, 1, 0);
			this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel3.Location = new System.Drawing.Point(80, 3);
			this.tableLayoutPanel3.Name = "tableLayoutPanel3";
			this.tableLayoutPanel3.RowCount = 1;
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel3.Size = new System.Drawing.Size(144, 13);
			this.tableLayoutPanel3.TabIndex = 49;
			// 
			// tableLayoutPanel4
			// 
			this.tableLayoutPanel4.ColumnCount = 2;
			this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 65F));
			this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel4.Controls.Add(this.lblShortDays, 0, 0);
			this.tableLayoutPanel4.Controls.Add(this.lnkShortDaysDelete, 1, 0);
			this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel4.Location = new System.Drawing.Point(230, 3);
			this.tableLayoutPanel4.Name = "tableLayoutPanel4";
			this.tableLayoutPanel4.RowCount = 1;
			this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel4.Size = new System.Drawing.Size(108, 13);
			this.tableLayoutPanel4.TabIndex = 50;
			// 
			// tableLayoutPanel5
			// 
			this.tableLayoutPanel5.ColumnCount = 2;
			this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 55F));
			this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel5.Controls.Add(this.lblHolidays, 0, 0);
			this.tableLayoutPanel5.Controls.Add(this.lnkHolidaysDelete, 1, 0);
			this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel5.Location = new System.Drawing.Point(344, 3);
			this.tableLayoutPanel5.Name = "tableLayoutPanel5";
			this.tableLayoutPanel5.RowCount = 1;
			this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel5.Size = new System.Drawing.Size(110, 13);
			this.tableLayoutPanel5.TabIndex = 51;
			// 
			// tableLayoutPanel6
			// 
			this.tableLayoutPanel6.ColumnCount = 4;
			this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
			this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
			this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
			this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
			this.tableLayoutPanel6.Controls.Add(this.lblMarketTimeZone, 3, 0);
			this.tableLayoutPanel6.Controls.Add(this.lblMarketDaysOfWeek, 2, 0);
			this.tableLayoutPanel6.Controls.Add(this.cbxMarketTimeZone, 3, 1);
			this.tableLayoutPanel6.Controls.Add(this.txtMarketDaysOfWeek, 2, 1);
			this.tableLayoutPanel6.Controls.Add(this.lblMarketServerClose, 1, 0);
			this.tableLayoutPanel6.Controls.Add(this.lblMarketServerOpen, 0, 0);
			this.tableLayoutPanel6.Controls.Add(this.txtMarketServerClose, 1, 1);
			this.tableLayoutPanel6.Controls.Add(this.txtMarketServerOpen, 0, 1);
			this.tableLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel6.Location = new System.Drawing.Point(3, 173);
			this.tableLayoutPanel6.Name = "tableLayoutPanel6";
			this.tableLayoutPanel6.RowCount = 2;
			this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 13F));
			this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel6.Size = new System.Drawing.Size(457, 39);
			this.tableLayoutPanel6.TabIndex = 51;
			// 
			// tableLayoutPanel7
			// 
			this.tableLayoutPanel7.ColumnCount = 1;
			this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel7.Controls.Add(this.tableLayoutPanel6, 0, 1);
			this.tableLayoutPanel7.Controls.Add(this.tableLayoutPanel1, 0, 0);
			this.tableLayoutPanel7.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel7.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel7.Name = "tableLayoutPanel7";
			this.tableLayoutPanel7.RowCount = 2;
			this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 45F));
			this.tableLayoutPanel7.Size = new System.Drawing.Size(463, 215);
			this.tableLayoutPanel7.TabIndex = 52;
			// 
			// MarketInfoEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.Controls.Add(this.tableLayoutPanel7);
			this.Name = "MarketInfoEditor";
			this.Size = new System.Drawing.Size(463, 215);
			this.Load += new System.EventHandler(this.MarketInfoEditor_Load);
			((System.ComponentModel.ISupportInitialize)(this.dgMarketName)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dgHolidays)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dgShortDays)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dgClearingTimespans)).EndInit();
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel2.ResumeLayout(false);
			this.tableLayoutPanel2.PerformLayout();
			this.tableLayoutPanel3.ResumeLayout(false);
			this.tableLayoutPanel3.PerformLayout();
			this.tableLayoutPanel4.ResumeLayout(false);
			this.tableLayoutPanel4.PerformLayout();
			this.tableLayoutPanel5.ResumeLayout(false);
			this.tableLayoutPanel5.PerformLayout();
			this.tableLayoutPanel6.ResumeLayout(false);
			this.tableLayoutPanel6.PerformLayout();
			this.tableLayoutPanel7.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ComboBox cbxMarketTimeZone;
		private System.Windows.Forms.TextBox txtMarketServerClose;
		private System.Windows.Forms.TextBox txtMarketServerOpen;
		private System.Windows.Forms.LinkLabel lnkMarketNameDelete;
		private System.Windows.Forms.Label lblMarketServerClose;
		private System.Windows.Forms.Label lblMarketServerOpen;
		private System.Windows.Forms.Label lblMarketInfo;
		private System.Windows.Forms.Label lblMarketTimeZone;
		private System.Windows.Forms.DataGridView dgMarketName;
		private System.Windows.Forms.DataGridView dgHolidays;
		private System.Windows.Forms.DataGridView dgShortDays;
		private System.Windows.Forms.Label lblShortDays;
		private System.Windows.Forms.Label lblHolidays;
		private System.Windows.Forms.LinkLabel lnkHolidaysDelete;
		private System.Windows.Forms.LinkLabel lnkShortDaysDelete;
		private System.Windows.Forms.DataGridView dgClearingTimespans;
		private System.Windows.Forms.Label lblClearingTimespans;
		private System.Windows.Forms.LinkLabel lnkIntradayInterruptsDelete;
		private System.Windows.Forms.TextBox txtMarketDaysOfWeek;
		private System.Windows.Forms.Label lblMarketDaysOfWeek;
		private System.Windows.Forms.DataGridViewTextBoxColumn colMarketName;
		private System.Windows.Forms.DataGridViewTextBoxColumn colMarketUsedTimes;
		private System.Windows.Forms.DataGridViewTextBoxColumn colHolidaysDate;
		private System.Windows.Forms.DataGridViewTextBoxColumn colShortDaysDate;
		private System.Windows.Forms.DataGridViewTextBoxColumn colShortDaysTime;
		private System.Windows.Forms.DataGridViewTextBoxColumn colShortDaysClose;
		private System.Windows.Forms.DataGridViewTextBoxColumn colClearingTimespansSuspends;
		private System.Windows.Forms.DataGridViewTextBoxColumn colClearingTimespansResumes;
		private System.Windows.Forms.DataGridViewTextBoxColumn colClearingTimespansDaysOfWeek;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel6;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel7;
	}
}
