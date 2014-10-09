namespace Sq1.Gui.Singletons {
	partial class SlidersForm {
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.pnlNoParametersInScript = new System.Windows.Forms.Panel();
			this.lblNoParametersDefined = new System.Windows.Forms.Label();
			this.lblScriptName = new System.Windows.Forms.Label();
			this.SlidersAutoGrowControl = new Sq1.Widgets.SteppingSlider.SlidersAutoGrowControl();
			this.pnlNoParametersInScript.SuspendLayout();
			this.SuspendLayout();
			// 
			// pnlNoParametersInScript
			// 
			this.pnlNoParametersInScript.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.pnlNoParametersInScript.Controls.Add(this.lblNoParametersDefined);
			this.pnlNoParametersInScript.Controls.Add(this.lblScriptName);
			this.pnlNoParametersInScript.Location = new System.Drawing.Point(0, 0);
			this.pnlNoParametersInScript.Name = "pnlNoParametersInScript";
			this.pnlNoParametersInScript.Size = new System.Drawing.Size(360, 34);
			this.pnlNoParametersInScript.TabIndex = 29;
			// 
			// lblNoParametersDefined
			// 
			this.lblNoParametersDefined.AutoSize = true;
			this.lblNoParametersDefined.Location = new System.Drawing.Point(0, 0);
			this.lblNoParametersDefined.Name = "lblNoParametersDefined";
			this.lblNoParametersDefined.Size = new System.Drawing.Size(158, 13);
			this.lblNoParametersDefined.TabIndex = 26;
			this.lblNoParametersDefined.Text = "No parameters/indicators(*) defined in Script:";
			// 
			// lblScriptName
			// 
			this.lblScriptName.AutoSize = true;
			this.lblScriptName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.lblScriptName.Location = new System.Drawing.Point(-3, 16);
			this.lblScriptName.Name = "lblScriptName";
			this.lblScriptName.Size = new System.Drawing.Size(99, 13);
			this.lblScriptName.TabIndex = 27;
			this.lblScriptName.Text = "ScriptNameHere";
			// 
			// SlidersAutoGrow
			// 
			this.SlidersAutoGrowControl.BackColor = System.Drawing.SystemColors.Control;
			this.SlidersAutoGrowControl.CurrentParametersFromChildSliders = null;
			this.SlidersAutoGrowControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.SlidersAutoGrowControl.Location = new System.Drawing.Point(0, 0);
			this.SlidersAutoGrowControl.Name = "SlidersAutoGrow";
			this.SlidersAutoGrowControl.Padding = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.SlidersAutoGrowControl.Size = new System.Drawing.Size(360, 89);
			this.SlidersAutoGrowControl.TabIndex = 30;
			this.SlidersAutoGrowControl.VerticalSpaceBetweenSliders = 4;
			// 
			// SlidersForm
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			this.ClientSize = new System.Drawing.Size(360, 89);
			this.Controls.Add(this.pnlNoParametersInScript);
			this.Controls.Add(this.SlidersAutoGrowControl);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.Name = "SlidersForm";
			this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockLeft;
			this.Text = "Script Parameters";
			this.pnlNoParametersInScript.ResumeLayout(false);
			this.pnlNoParametersInScript.PerformLayout();
			this.ResumeLayout(false);
		}

		#endregion

		private System.Windows.Forms.Panel pnlNoParametersInScript;
		private System.Windows.Forms.Label lblNoParametersDefined;
		private System.Windows.Forms.Label lblScriptName;
		public Sq1.Widgets.SteppingSlider.SlidersAutoGrowControl SlidersAutoGrowControl;
	}
}