/*
 * Created by SharpDevelop.
 * User: worldexplorer
 * Date: 03-Dec-14
 * Time: 10:07 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace Sq1.Widgets.LiveSim
{
	partial class LiveSimControl
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.SplitContainer splitContainer2;
		private System.Windows.Forms.SplitContainer splitContainer3;
		
		/// <summary>
		/// Disposes resources used by the control.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.splitContainer2 = new System.Windows.Forms.SplitContainer();
			this.splitContainer3 = new System.Windows.Forms.SplitContainer();
			this.streamingLivesimEditor1 = new Sq1.Core.Livesim.StreamingLivesimEditor();
			this.exceptionsControl1 = new Sq1.Widgets.Exceptions.ExceptionsControl();
			this.exceptionsControl2 = new Sq1.Widgets.Exceptions.ExceptionsControl();
			this.brokerLivesimEditor1 = new Sq1.Core.Livesim.BrokerLivesimEditor();
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.panel1 = new System.Windows.Forms.Panel();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
			this.splitContainer2.Panel1.SuspendLayout();
			this.splitContainer2.Panel2.SuspendLayout();
			this.splitContainer2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
			this.splitContainer3.Panel1.SuspendLayout();
			this.splitContainer3.Panel2.SuspendLayout();
			this.splitContainer3.SuspendLayout();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer1
			// 
			this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.splitContainer1.BackColor = System.Drawing.SystemColors.ControlDark;
			this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.BackColor = System.Drawing.SystemColors.Control;
			this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.BackColor = System.Drawing.SystemColors.Control;
			this.splitContainer1.Panel2.Controls.Add(this.splitContainer3);
			this.splitContainer1.Size = new System.Drawing.Size(689, 767);
			this.splitContainer1.SplitterDistance = 629;
			this.splitContainer1.TabIndex = 0;
			// 
			// splitContainer2
			// 
			this.splitContainer2.BackColor = System.Drawing.SystemColors.ControlDark;
			this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer2.Location = new System.Drawing.Point(0, 0);
			this.splitContainer2.Name = "splitContainer2";
			// 
			// splitContainer2.Panel1
			// 
			this.splitContainer2.Panel1.BackColor = System.Drawing.SystemColors.Control;
			this.splitContainer2.Panel1.Controls.Add(this.streamingLivesimEditor1);
			// 
			// splitContainer2.Panel2
			// 
			this.splitContainer2.Panel2.BackColor = System.Drawing.SystemColors.Control;
			this.splitContainer2.Panel2.Controls.Add(this.brokerLivesimEditor1);
			this.splitContainer2.Size = new System.Drawing.Size(689, 629);
			this.splitContainer2.SplitterDistance = 344;
			this.splitContainer2.TabIndex = 0;
			// 
			// splitContainer3
			// 
			this.splitContainer3.BackColor = System.Drawing.SystemColors.ControlDark;
			this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer3.Location = new System.Drawing.Point(0, 0);
			this.splitContainer3.Name = "splitContainer3";
			// 
			// splitContainer3.Panel1
			// 
			this.splitContainer3.Panel1.BackColor = System.Drawing.SystemColors.Control;
			this.splitContainer3.Panel1.Controls.Add(this.exceptionsControl1);
			// 
			// splitContainer3.Panel2
			// 
			this.splitContainer3.Panel2.BackColor = System.Drawing.SystemColors.Control;
			this.splitContainer3.Panel2.Controls.Add(this.exceptionsControl2);
			this.splitContainer3.Size = new System.Drawing.Size(689, 134);
			this.splitContainer3.SplitterDistance = 344;
			this.splitContainer3.TabIndex = 0;
			// 
			// streamingLivesimEditor1
			// 
			this.streamingLivesimEditor1.AutoScroll = true;
			this.streamingLivesimEditor1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.streamingLivesimEditor1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.streamingLivesimEditor1.Location = new System.Drawing.Point(0, 0);
			this.streamingLivesimEditor1.Name = "streamingLivesimEditor1";
			this.streamingLivesimEditor1.Size = new System.Drawing.Size(344, 629);
			this.streamingLivesimEditor1.TabIndex = 0;
			// 
			// exceptionsControl1
			// 
			this.exceptionsControl1.BackColor = System.Drawing.SystemColors.MenuHighlight;
			this.exceptionsControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.exceptionsControl1.Location = new System.Drawing.Point(0, 0);
			this.exceptionsControl1.Name = "exceptionsControl1";
			this.exceptionsControl1.Size = new System.Drawing.Size(344, 134);
			this.exceptionsControl1.TabIndex = 0;
			// 
			// exceptionsControl2
			// 
			this.exceptionsControl2.BackColor = System.Drawing.SystemColors.MenuHighlight;
			this.exceptionsControl2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.exceptionsControl2.Location = new System.Drawing.Point(0, 0);
			this.exceptionsControl2.Name = "exceptionsControl2";
			this.exceptionsControl2.Size = new System.Drawing.Size(341, 134);
			this.exceptionsControl2.TabIndex = 0;
			// 
			// brokerLivesimEditor1
			// 
			this.brokerLivesimEditor1.AutoScroll = true;
			this.brokerLivesimEditor1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.brokerLivesimEditor1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.brokerLivesimEditor1.Location = new System.Drawing.Point(0, 0);
			this.brokerLivesimEditor1.Name = "brokerLivesimEditor1";
			this.brokerLivesimEditor1.Size = new System.Drawing.Size(341, 629);
			this.brokerLivesimEditor1.TabIndex = 0;
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(3, 3);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(109, 23);
			this.button1.TabIndex = 1;
			this.button1.Text = "Start / Abort";
			this.button1.UseVisualStyleBackColor = true;
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(118, 3);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(111, 23);
			this.button2.TabIndex = 2;
			this.button2.Text = "Pause Simulation";
			this.button2.UseVisualStyleBackColor = true;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.button2);
			this.panel1.Controls.Add(this.button1);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(0, 770);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(689, 30);
			this.panel1.TabIndex = 3;
			// 
			// LiveSimControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.splitContainer1);
			this.Name = "LiveSimControl";
			this.Size = new System.Drawing.Size(689, 800);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.splitContainer2.Panel1.ResumeLayout(false);
			this.splitContainer2.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
			this.splitContainer2.ResumeLayout(false);
			this.splitContainer3.Panel1.ResumeLayout(false);
			this.splitContainer3.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
			this.splitContainer3.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		private Core.Livesim.StreamingLivesimEditor streamingLivesimEditor1;
		private Exceptions.ExceptionsControl exceptionsControl1;
		private Exceptions.ExceptionsControl exceptionsControl2;
		private Core.Livesim.BrokerLivesimEditor brokerLivesimEditor1;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Panel panel1;
	}
}
