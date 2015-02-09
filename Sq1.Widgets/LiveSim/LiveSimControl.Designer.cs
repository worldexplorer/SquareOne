/*
 * Created by SharpDevelop.
 * User: worldexplorer
 * Date: 03-Dec-14
 * Time: 10:07 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace Sq1.Widgets.Livesim
{
	partial class LivesimControl
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
			this.BtnStartStop = new System.Windows.Forms.Button();
			this.BtnPauseResume = new System.Windows.Forms.Button();
			this.panel1 = new System.Windows.Forms.Panel();
			this.LblStrategyAsString = new System.Windows.Forms.Label();
			this.streamingLivesimEditor1 = new Sq1.Core.Livesim.LivesimStreamingEditor();
			this.brokerLivesimEditor1 = new Sq1.Core.Livesim.LivesimBrokerEditor();
			this.exceptionsControl3 = new Sq1.Widgets.Exceptions.ExceptionsControl();
			this.exceptionsControl4 = new Sq1.Widgets.Exceptions.ExceptionsControl();
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
			this.splitContainer1.Panel2MinSize = 1;
			this.splitContainer1.Size = new System.Drawing.Size(689, 518);
			this.splitContainer1.SplitterDistance = 512;
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
			this.splitContainer2.Size = new System.Drawing.Size(689, 512);
			this.splitContainer2.SplitterDistance = 338;
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
			this.splitContainer3.Panel1.Controls.Add(this.exceptionsControl3);
			// 
			// splitContainer3.Panel2
			// 
			this.splitContainer3.Panel2.BackColor = System.Drawing.SystemColors.Control;
			this.splitContainer3.Panel2.Controls.Add(this.exceptionsControl4);
			this.splitContainer3.Size = new System.Drawing.Size(689, 2);
			this.splitContainer3.SplitterDistance = 338;
			this.splitContainer3.TabIndex = 0;
			// 
			// BtnStartStop
			// 
			this.BtnStartStop.Location = new System.Drawing.Point(3, 3);
			this.BtnStartStop.Name = "BtnStartStop";
			this.BtnStartStop.Size = new System.Drawing.Size(109, 23);
			this.BtnStartStop.TabIndex = 1;
			this.BtnStartStop.Text = "Start";
			this.BtnStartStop.UseVisualStyleBackColor = true;
			// 
			// BtnPauseResume
			// 
			this.BtnPauseResume.Enabled = false;
			this.BtnPauseResume.Location = new System.Drawing.Point(118, 3);
			this.BtnPauseResume.Name = "BtnPauseResume";
			this.BtnPauseResume.Size = new System.Drawing.Size(111, 23);
			this.BtnPauseResume.TabIndex = 2;
			this.BtnPauseResume.Text = "Pause";
			this.BtnPauseResume.UseVisualStyleBackColor = true;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.LblStrategyAsString);
			this.panel1.Controls.Add(this.BtnPauseResume);
			this.panel1.Controls.Add(this.BtnStartStop);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(0, 521);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(689, 30);
			this.panel1.TabIndex = 3;
			// 
			// LblStrategyAsString
			// 
			this.LblStrategyAsString.AutoSize = true;
			this.LblStrategyAsString.Location = new System.Drawing.Point(235, 8);
			this.LblStrategyAsString.Name = "LblStrategyAsString";
			this.LblStrategyAsString.Size = new System.Drawing.Size(271, 13);
			this.LblStrategyAsString.TabIndex = 3;
			this.LblStrategyAsString.Text = "EntryEveryBar Default MAfast[10] MAslow[40] SLtype[3]";
			// 
			// streamingLivesimEditor1
			// 
			this.streamingLivesimEditor1.AutoScroll = true;
			this.streamingLivesimEditor1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.streamingLivesimEditor1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.streamingLivesimEditor1.Location = new System.Drawing.Point(0, 0);
			this.streamingLivesimEditor1.Name = "streamingLivesimEditor1";
			this.streamingLivesimEditor1.Size = new System.Drawing.Size(338, 512);
			this.streamingLivesimEditor1.TabIndex = 0;
			// 
			// brokerLivesimEditor1
			// 
			this.brokerLivesimEditor1.AutoScroll = true;
			this.brokerLivesimEditor1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.brokerLivesimEditor1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.brokerLivesimEditor1.Location = new System.Drawing.Point(0, 0);
			this.brokerLivesimEditor1.Name = "brokerLivesimEditor1";
			this.brokerLivesimEditor1.Size = new System.Drawing.Size(347, 512);
			this.brokerLivesimEditor1.TabIndex = 0;
			// 
			// exceptionsControl3
			// 
			this.exceptionsControl3.BackColor = System.Drawing.SystemColors.MenuHighlight;
			this.exceptionsControl3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.exceptionsControl3.Location = new System.Drawing.Point(0, 0);
			this.exceptionsControl3.Name = "exceptionsControl3";
			this.exceptionsControl3.Size = new System.Drawing.Size(338, 2);
			this.exceptionsControl3.TabIndex = 0;
			// 
			// exceptionsControl4
			// 
			this.exceptionsControl4.BackColor = System.Drawing.SystemColors.MenuHighlight;
			this.exceptionsControl4.Dock = System.Windows.Forms.DockStyle.Fill;
			this.exceptionsControl4.Location = new System.Drawing.Point(0, 0);
			this.exceptionsControl4.Name = "exceptionsControl4";
			this.exceptionsControl4.Size = new System.Drawing.Size(347, 2);
			this.exceptionsControl4.TabIndex = 0;
			// 
			// LivesimControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.splitContainer1);
			this.Name = "LivesimControl";
			this.Size = new System.Drawing.Size(689, 551);
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
			this.panel1.PerformLayout();
			this.ResumeLayout(false);

		}

		private Core.Livesim.LivesimStreamingEditor streamingLivesimEditor1;
		private Exceptions.ExceptionsControl exceptionsControl1;
		private Exceptions.ExceptionsControl exceptionsControl2;
		private Core.Livesim.LivesimBrokerEditor brokerLivesimEditor1;
		public System.Windows.Forms.Button BtnPauseResume;
		private System.Windows.Forms.Panel panel1;
		public System.Windows.Forms.Button BtnStartStop;
		public System.Windows.Forms.Label LblStrategyAsString;
		private Exceptions.ExceptionsControl exceptionsControl3;
		private Exceptions.ExceptionsControl exceptionsControl4;
	}
}
