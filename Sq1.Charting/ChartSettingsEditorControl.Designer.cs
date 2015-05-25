using System.Windows.Forms;

namespace Sq1.Charting {
	partial class ChartSettingsEditorControl {
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChartSettingsEditorControl));
			this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.toolStripItemComboBox1 = new Sq1.Widgets.ToolStripImproved.ToolStripItemComboBox();
			this.mniAbsorbFromChart = new System.Windows.Forms.ToolStripDropDownButton();
			this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
			this.statusStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// propertyGrid1
			// 
			this.propertyGrid1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.propertyGrid1.CommandsVisibleIfAvailable = false;
			this.propertyGrid1.Location = new System.Drawing.Point(0, -27);
			this.propertyGrid1.Name = "propertyGrid1";
			this.propertyGrid1.Size = new System.Drawing.Size(234, 546);
			this.propertyGrid1.TabIndex = 1;
			this.propertyGrid1.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid1_PropertyValueChanged);
			// 
			// statusStrip1
			// 
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripItemComboBox1,
            this.mniAbsorbFromChart});
			this.statusStrip1.Location = new System.Drawing.Point(0, 515);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(234, 25);
			this.statusStrip1.SizingGrip = false;
			this.statusStrip1.TabIndex = 2;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// toolStripItemComboBox1
			// 
			this.toolStripItemComboBox1.Name = "toolStripItemComboBox1";
			this.toolStripItemComboBox1.Size = new System.Drawing.Size(160, 23);
			// 
			// mniAbsorbFromChart
			// 
			this.mniAbsorbFromChart.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.mniAbsorbFromChart.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.mniAbsorbFromChart.Name = "mniAbsorbFromChart";
			this.mniAbsorbFromChart.Size = new System.Drawing.Size(121, 23);
			this.mniAbsorbFromChart.Text = "Absorb From Chart";
			this.mniAbsorbFromChart.Click += new System.EventHandler(this.mniAbsorbFromChart_Click);
			// 
			// toolStripMenuItem5
			// 
			this.toolStripMenuItem5.Name = "toolStripMenuItem5";
			this.toolStripMenuItem5.Size = new System.Drawing.Size(181, 22);
			this.toolStripMenuItem5.Text = "toolStripMenuItem5";
			// 
			// ChartSettingsEditorControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.statusStrip1);
			this.Controls.Add(this.propertyGrid1);
			this.Name = "ChartSettingsEditorControl";
			this.Size = new System.Drawing.Size(234, 540);
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.PropertyGrid propertyGrid1;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private ToolStripMenuItem toolStripMenuItem5;
		public Widgets.ToolStripImproved.ToolStripItemComboBox toolStripItemComboBox1;
		private ToolStripDropDownButton mniAbsorbFromChart;
	}
}
