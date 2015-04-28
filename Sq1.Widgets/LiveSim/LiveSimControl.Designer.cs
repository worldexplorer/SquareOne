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
			this.StreamingLivesimEditor = new Sq1.Core.Livesim.LivesimStreamingEditor();
			this.BrokerLivesimEditor = new Sq1.Core.Livesim.LivesimBrokerEditor();
			this.splitContainer3 = new System.Windows.Forms.SplitContainer();
			this.exceptionsControl3 = new Sq1.Widgets.Exceptions.ExceptionsControl();
			this.exceptionsControl4 = new Sq1.Widgets.Exceptions.ExceptionsControl();
			this.BtnStartStop = new System.Windows.Forms.Button();
			this.BtnPauseResume = new System.Windows.Forms.Button();
			this.panel1 = new System.Windows.Forms.Panel();
			this.LblStrategyAsString = new System.Windows.Forms.Label();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.TssBtnStartStop = new System.Windows.Forms.ToolStripButton();
			this.TssBtnPauseResume = new System.Windows.Forms.ToolStripButton();
			this.TssLblStrategyAsString = new System.Windows.Forms.ToolStripStatusLabel();
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
			this.statusStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer1
			// 
			this.splitContainer1.BackColor = System.Drawing.SystemColors.ControlDark;
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
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
			this.splitContainer1.Panel2Collapsed = true;
			this.splitContainer1.Panel2MinSize = 1;
			this.splitContainer1.Size = new System.Drawing.Size(689, 593);
			this.splitContainer1.SplitterDistance = 25;
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
			this.splitContainer2.Panel1.Controls.Add(this.StreamingLivesimEditor);
			// 
			// splitContainer2.Panel2
			// 
			this.splitContainer2.Panel2.BackColor = System.Drawing.SystemColors.Control;
			this.splitContainer2.Panel2.Controls.Add(this.BrokerLivesimEditor);
			this.splitContainer2.Size = new System.Drawing.Size(689, 593);
			this.splitContainer2.SplitterDistance = 346;
			this.splitContainer2.TabIndex = 0;
			// 
			// StreamingLivesimEditor
			// 
			this.StreamingLivesimEditor.AutoScroll = true;
			this.StreamingLivesimEditor.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.StreamingLivesimEditor.Dock = System.Windows.Forms.DockStyle.Fill;
			this.StreamingLivesimEditor.Location = new System.Drawing.Point(0, 0);
			this.StreamingLivesimEditor.Margin = new System.Windows.Forms.Padding(4);
			this.StreamingLivesimEditor.Name = "StreamingLivesimEditor";
			this.StreamingLivesimEditor.Size = new System.Drawing.Size(346, 593);
			this.StreamingLivesimEditor.TabIndex = 0;
			// 
			// BrokerLivesimEditor
			// 
			this.BrokerLivesimEditor.AutoScroll = true;
			this.BrokerLivesimEditor.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.BrokerLivesimEditor.Dock = System.Windows.Forms.DockStyle.Fill;
			this.BrokerLivesimEditor.Location = new System.Drawing.Point(0, 0);
			this.BrokerLivesimEditor.Name = "BrokerLivesimEditor";
			this.BrokerLivesimEditor.Size = new System.Drawing.Size(339, 593);
			this.BrokerLivesimEditor.TabIndex = 0;
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
			this.splitContainer3.Size = new System.Drawing.Size(150, 46);
			this.splitContainer3.SplitterDistance = 72;
			this.splitContainer3.TabIndex = 0;
			// 
			// exceptionsControl3
			// 
			this.exceptionsControl3.BackColor = System.Drawing.SystemColors.MenuHighlight;
			this.exceptionsControl3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.exceptionsControl3.Location = new System.Drawing.Point(0, 0);
			this.exceptionsControl3.Name = "exceptionsControl3";
			this.exceptionsControl3.Size = new System.Drawing.Size(72, 46);
			this.exceptionsControl3.TabIndex = 0;
			// 
			// exceptionsControl4
			// 
			this.exceptionsControl4.BackColor = System.Drawing.SystemColors.MenuHighlight;
			this.exceptionsControl4.Dock = System.Windows.Forms.DockStyle.Fill;
			this.exceptionsControl4.Location = new System.Drawing.Point(0, 0);
			this.exceptionsControl4.Name = "exceptionsControl4";
			this.exceptionsControl4.Size = new System.Drawing.Size(74, 46);
			this.exceptionsControl4.TabIndex = 0;
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
			this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(689, 29);
			this.panel1.TabIndex = 3;
			this.panel1.Visible = false;
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
			// statusStrip1
			// 
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TssBtnStartStop,
            this.TssBtnPauseResume,
            this.TssLblStrategyAsString});
			this.statusStrip1.Location = new System.Drawing.Point(0, 571);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(689, 22);
			this.statusStrip1.SizingGrip = false;
			this.statusStrip1.TabIndex = 4;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// TssBtnStartStop
			// 
			this.TssBtnStartStop.CheckOnClick = true;
			this.TssBtnStartStop.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.TssBtnStartStop.Name = "TssBtnStartStop";
			this.TssBtnStartStop.Size = new System.Drawing.Size(35, 20);
			this.TssBtnStartStop.Text = "Start";
			// 
			// TssBtnPauseResume
			// 
			this.TssBtnPauseResume.CheckOnClick = true;
			this.TssBtnPauseResume.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.TssBtnPauseResume.Name = "TssBtnPauseResume";
			this.TssBtnPauseResume.Size = new System.Drawing.Size(42, 20);
			this.TssBtnPauseResume.Text = "Pause";
			// 
			// TssLblStrategyAsString
			// 
			this.TssLblStrategyAsString.Name = "TssLblStrategyAsString";
			this.TssLblStrategyAsString.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
			this.TssLblStrategyAsString.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
			this.TssLblStrategyAsString.Size = new System.Drawing.Size(566, 17);
			this.TssLblStrategyAsString.Spring = true;
			this.TssLblStrategyAsString.Text = "EntryEveryBar Default MAfast[10] MAslow[40] SLtype[3]";
			this.TssLblStrategyAsString.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// LivesimControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.statusStrip1);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.splitContainer1);
			this.Name = "LivesimControl";
			this.Size = new System.Drawing.Size(689, 593);
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
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		private System.Windows.Forms.Panel panel1;
		public System.Windows.Forms.Button BtnStartStop;
		public System.Windows.Forms.Button BtnPauseResume;
		public System.Windows.Forms.Label LblStrategyAsString;
		private Exceptions.ExceptionsControl exceptionsControl3;
		private Exceptions.ExceptionsControl exceptionsControl4;
		public Core.Livesim.LivesimStreamingEditor StreamingLivesimEditor;
		public Core.Livesim.LivesimBrokerEditor BrokerLivesimEditor;
		private System.Windows.Forms.StatusStrip statusStrip1;
		public System.Windows.Forms.ToolStripButton TssBtnStartStop;
		public System.Windows.Forms.ToolStripButton TssBtnPauseResume;
		public System.Windows.Forms.ToolStripStatusLabel TssLblStrategyAsString;
	}
}
