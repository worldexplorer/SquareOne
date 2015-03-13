using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;

namespace Sq1.Charting {
	[Designer("System.Windows.Forms.Design.ParentControlDesigner, System.Design", typeof(IDesigner))]
	public partial class TooltipPrice : UserControl {
		private IContainer components;
		private Label lblOpen;
		private Label lblHigh;
		private Label lblLow;
		private Label lblClose;
		private Label lblHeaderVal;
		private Label lblOpenVal;
		private Label lblHighVal;
		private Label lblLowVal;
		private Label lblCloseVal;
		private Label lblVolumeVal;
		private Label lblVolume;
		private Label lblDateValue;
		private Label lblDate;
		private LinkLabel lnkAlerts;
		private LinkLabel lnkAlertsVal;
		private LinkLabel lnkOrders;
		private Label lblOrdersVal;
		private LinkLabel lnkPositions;
		private Label lblPositionsVal;
		protected override void Dispose(bool disposing) {
			if (disposing && this.components != null) {
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.lblOpen = new System.Windows.Forms.Label();
			this.lblHigh = new System.Windows.Forms.Label();
			this.lblLow = new System.Windows.Forms.Label();
			this.lblClose = new System.Windows.Forms.Label();
			this.lblHeaderVal = new System.Windows.Forms.Label();
			this.lblOpenVal = new System.Windows.Forms.Label();
			this.lblHighVal = new System.Windows.Forms.Label();
			this.lblLowVal = new System.Windows.Forms.Label();
			this.lblCloseVal = new System.Windows.Forms.Label();
			this.lblVolumeVal = new System.Windows.Forms.Label();
			this.lblVolume = new System.Windows.Forms.Label();
			this.lblDateValue = new System.Windows.Forms.Label();
			this.lblDate = new System.Windows.Forms.Label();
			this.lnkAlertsVal = new System.Windows.Forms.LinkLabel();
			this.lnkAlerts = new System.Windows.Forms.LinkLabel();
			this.lblOrdersVal = new System.Windows.Forms.Label();
			this.lnkOrders = new System.Windows.Forms.LinkLabel();
			this.lblPositionsVal = new System.Windows.Forms.Label();
			this.lnkPositions = new System.Windows.Forms.LinkLabel();
			this.SuspendLayout();
			// 
			// lblOpen
			// 
			this.lblOpen.AutoSize = true;
			this.lblOpen.Location = new System.Drawing.Point(3, 39);
			this.lblOpen.Name = "lblOpen";
			this.lblOpen.Size = new System.Drawing.Size(33, 13);
			this.lblOpen.TabIndex = 1;
			this.lblOpen.Text = "Open";
			// 
			// lblHigh
			// 
			this.lblHigh.AutoSize = true;
			this.lblHigh.Location = new System.Drawing.Point(3, 52);
			this.lblHigh.Name = "lblHigh";
			this.lblHigh.Size = new System.Drawing.Size(29, 13);
			this.lblHigh.TabIndex = 2;
			this.lblHigh.Text = "High";
			// 
			// lblLow
			// 
			this.lblLow.AutoSize = true;
			this.lblLow.Location = new System.Drawing.Point(3, 65);
			this.lblLow.Name = "lblLow";
			this.lblLow.Size = new System.Drawing.Size(27, 13);
			this.lblLow.TabIndex = 3;
			this.lblLow.Text = "Low";
			// 
			// lblClose
			// 
			this.lblClose.AutoSize = true;
			this.lblClose.Location = new System.Drawing.Point(3, 78);
			this.lblClose.Name = "lblClose";
			this.lblClose.Size = new System.Drawing.Size(33, 13);
			this.lblClose.TabIndex = 4;
			this.lblClose.Text = "Close";
			// 
			// lblHeaderVal
			// 
			this.lblHeaderVal.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.lblHeaderVal.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
			this.lblHeaderVal.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblHeaderVal.Location = new System.Drawing.Point(0, 0);
			this.lblHeaderVal.Name = "lblHeaderVal";
			this.lblHeaderVal.Padding = new System.Windows.Forms.Padding(2);
			this.lblHeaderVal.Size = new System.Drawing.Size(117, 16);
			this.lblHeaderVal.TabIndex = 7;
			this.lblHeaderVal.Text = "16:50 #515123";
			this.lblHeaderVal.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblOpenVal
			// 
			this.lblOpenVal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lblOpenVal.Location = new System.Drawing.Point(54, 38);
			this.lblOpenVal.Name = "lblOpenVal";
			this.lblOpenVal.Size = new System.Drawing.Size(57, 13);
			this.lblOpenVal.TabIndex = 8;
			this.lblOpenVal.Text = "123,456";
			this.lblOpenVal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lblHighVal
			// 
			this.lblHighVal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lblHighVal.Location = new System.Drawing.Point(54, 52);
			this.lblHighVal.Name = "lblHighVal";
			this.lblHighVal.Size = new System.Drawing.Size(57, 13);
			this.lblHighVal.TabIndex = 9;
			this.lblHighVal.Text = "$12.34";
			this.lblHighVal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lblLowVal
			// 
			this.lblLowVal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lblLowVal.Location = new System.Drawing.Point(54, 64);
			this.lblLowVal.Name = "lblLowVal";
			this.lblLowVal.Size = new System.Drawing.Size(57, 13);
			this.lblLowVal.TabIndex = 10;
			this.lblLowVal.Text = "$12.34";
			this.lblLowVal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lblCloseVal
			// 
			this.lblCloseVal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lblCloseVal.Location = new System.Drawing.Point(54, 77);
			this.lblCloseVal.Name = "lblCloseVal";
			this.lblCloseVal.Size = new System.Drawing.Size(57, 13);
			this.lblCloseVal.TabIndex = 11;
			this.lblCloseVal.Text = "$12.34";
			this.lblCloseVal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lblVolumeVal
			// 
			this.lblVolumeVal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lblVolumeVal.Location = new System.Drawing.Point(54, 97);
			this.lblVolumeVal.Name = "lblVolumeVal";
			this.lblVolumeVal.Size = new System.Drawing.Size(57, 13);
			this.lblVolumeVal.TabIndex = 15;
			this.lblVolumeVal.Text = "12.34";
			this.lblVolumeVal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lblVolume
			// 
			this.lblVolume.AutoSize = true;
			this.lblVolume.Location = new System.Drawing.Point(3, 97);
			this.lblVolume.Name = "lblVolume";
			this.lblVolume.Size = new System.Drawing.Size(42, 13);
			this.lblVolume.TabIndex = 14;
			this.lblVolume.Text = "Volume";
			// 
			// lblDateValue
			// 
			this.lblDateValue.AutoSize = true;
			this.lblDateValue.Location = new System.Drawing.Point(3, 18);
			this.lblDateValue.Name = "lblDateValue";
			this.lblDateValue.Size = new System.Drawing.Size(84, 13);
			this.lblDateValue.TabIndex = 17;
			this.lblDateValue.Text = " Fri 16-Feb-2012";
			this.lblDateValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblDate
			// 
			this.lblDate.AutoSize = true;
			this.lblDate.Location = new System.Drawing.Point(20, 173);
			this.lblDate.Name = "lblDate";
			this.lblDate.Size = new System.Drawing.Size(30, 13);
			this.lblDate.TabIndex = 16;
			this.lblDate.Text = "Date";
			this.lblDate.Visible = false;
			// 
			// lnkAlertsVal
			// 
			this.lnkAlertsVal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lnkAlertsVal.BackColor = System.Drawing.SystemColors.ButtonFace;
			this.lnkAlertsVal.Location = new System.Drawing.Point(97, 0);
			this.lnkAlertsVal.Name = "lnkAlertsVal";
			this.lnkAlertsVal.Size = new System.Drawing.Size(20, 16);
			this.lnkAlertsVal.TabIndex = 19;
			this.lnkAlertsVal.TabStop = true;
			this.lnkAlertsVal.Text = "12";
			this.lnkAlertsVal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lnkAlertsVal.Visible = false;
			// 
			// lnkAlerts
			// 
			this.lnkAlerts.AutoSize = true;
			this.lnkAlerts.Location = new System.Drawing.Point(67, 146);
			this.lnkAlerts.Name = "lnkAlerts";
			this.lnkAlerts.Size = new System.Drawing.Size(33, 13);
			this.lnkAlerts.TabIndex = 18;
			this.lnkAlerts.TabStop = true;
			this.lnkAlerts.Text = "Alerts";
			this.lnkAlerts.Visible = false;
			// 
			// lblOrdersVal
			// 
			this.lblOrdersVal.Location = new System.Drawing.Point(20, 160);
			this.lblOrdersVal.Name = "lblOrdersVal";
			this.lblOrdersVal.Size = new System.Drawing.Size(41, 14);
			this.lblOrdersVal.TabIndex = 21;
			this.lblOrdersVal.Text = "13";
			this.lblOrdersVal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lblOrdersVal.Visible = false;
			// 
			// lnkOrders
			// 
			this.lnkOrders.AutoSize = true;
			this.lnkOrders.Location = new System.Drawing.Point(67, 160);
			this.lnkOrders.Name = "lnkOrders";
			this.lnkOrders.Size = new System.Drawing.Size(38, 13);
			this.lnkOrders.TabIndex = 20;
			this.lnkOrders.TabStop = true;
			this.lnkOrders.Text = "Orders";
			this.lnkOrders.Visible = false;
			// 
			// lblPositionsVal
			// 
			this.lblPositionsVal.Location = new System.Drawing.Point(23, 146);
			this.lblPositionsVal.Name = "lblPositionsVal";
			this.lblPositionsVal.Size = new System.Drawing.Size(38, 14);
			this.lblPositionsVal.TabIndex = 23;
			this.lblPositionsVal.Text = "2";
			this.lblPositionsVal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lblPositionsVal.Visible = false;
			// 
			// lnkPositions
			// 
			this.lnkPositions.AutoSize = true;
			this.lnkPositions.Location = new System.Drawing.Point(67, 173);
			this.lnkPositions.Name = "lnkPositions";
			this.lnkPositions.Size = new System.Drawing.Size(49, 13);
			this.lnkPositions.TabIndex = 22;
			this.lnkPositions.TabStop = true;
			this.lnkPositions.Text = "Positions";
			this.lnkPositions.Visible = false;
			// 
			// TooltipPrice
			// 
			this.BackColor = System.Drawing.SystemColors.Info;
			this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.Controls.Add(this.lblPositionsVal);
			this.Controls.Add(this.lnkPositions);
			this.Controls.Add(this.lblOrdersVal);
			this.Controls.Add(this.lnkOrders);
			this.Controls.Add(this.lnkAlertsVal);
			this.Controls.Add(this.lnkAlerts);
			this.Controls.Add(this.lblDateValue);
			this.Controls.Add(this.lblDate);
			this.Controls.Add(this.lblVolumeVal);
			this.Controls.Add(this.lblVolume);
			this.Controls.Add(this.lblCloseVal);
			this.Controls.Add(this.lblLowVal);
			this.Controls.Add(this.lblHighVal);
			this.Controls.Add(this.lblOpenVal);
			this.Controls.Add(this.lblHeaderVal);
			this.Controls.Add(this.lblClose);
			this.Controls.Add(this.lblLow);
			this.Controls.Add(this.lblHigh);
			this.Controls.Add(this.lblOpen);
			this.ForeColor = System.Drawing.SystemColors.InfoText;
			this.Name = "TooltipPrice";
			this.Size = new System.Drawing.Size(116, 116);
			this.ResumeLayout(false);
			this.PerformLayout();
		}

	}
}
