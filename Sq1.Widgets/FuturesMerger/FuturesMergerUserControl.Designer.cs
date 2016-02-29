namespace Sq1.Widgets.FuturesMerger {
	partial class FuturesMergerUserControl {
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
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.BarsEditorUserControl_top = new Sq1.Widgets.FuturesMerger.BarsEditorUserControl();
			this.BarsEditorUserControl_bottom = new Sq1.Widgets.FuturesMerger.BarsEditorUserControl();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.BarsEditorUserControl_bottom);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.BarsEditorUserControl_top);
			this.splitContainer1.Size = new System.Drawing.Size(888, 346);
			this.splitContainer1.SplitterDistance = 163;
			this.splitContainer1.TabIndex = 0;
			// 
			// barsEditorUserControl1
			// 
			this.BarsEditorUserControl_top.BackColor = System.Drawing.SystemColors.ControlDark;
			this.BarsEditorUserControl_top.Dock = System.Windows.Forms.DockStyle.Fill;
			this.BarsEditorUserControl_top.Location = new System.Drawing.Point(0, 0);
			this.BarsEditorUserControl_top.Margin = new System.Windows.Forms.Padding(0);
			this.BarsEditorUserControl_top.Name = "barsEditorUserControl1";
			this.BarsEditorUserControl_top.Size = new System.Drawing.Size(888, 179);
			this.BarsEditorUserControl_top.TabIndex = 0;
			// 
			// barsEditorUserControl2
			// 
			this.BarsEditorUserControl_bottom.BackColor = System.Drawing.SystemColors.ControlDark;
			this.BarsEditorUserControl_bottom.Dock = System.Windows.Forms.DockStyle.Fill;
			this.BarsEditorUserControl_bottom.Location = new System.Drawing.Point(0, 0);
			this.BarsEditorUserControl_bottom.Margin = new System.Windows.Forms.Padding(0);
			this.BarsEditorUserControl_bottom.Name = "barsEditorUserControl2";
			this.BarsEditorUserControl_bottom.Size = new System.Drawing.Size(888, 163);
			this.BarsEditorUserControl_bottom.TabIndex = 0;
			// 
			// FuturesMergerUserControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.splitContainer1);
			this.Name = "FuturesMergerUserControl";
			this.Size = new System.Drawing.Size(888, 346);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitContainer1;
		public BarsEditorUserControl BarsEditorUserControl_top;
		public BarsEditorUserControl BarsEditorUserControl_bottom;
	}
}
