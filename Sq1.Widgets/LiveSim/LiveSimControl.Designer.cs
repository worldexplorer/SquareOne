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
		private System.Windows.Forms.SplitContainer splitContainer2;
		
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
			this.components = new System.ComponentModel.Container();
			this.splitContainer2 = new System.Windows.Forms.SplitContainer();
			this.StreamingLivesimEditor = new Sq1.Core.Livesim.LivesimStreamingEditor();
			this.BrokerLivesimEditor = new Sq1.Core.Livesim.LivesimBrokerEditor();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.TssBtnStartStop = new System.Windows.Forms.ToolStripButton();
			this.TssBtnPauseResume = new System.Windows.Forms.ToolStripButton();
			this.TssLblStrategyAsString = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
			this.splitContainer2.Panel1.SuspendLayout();
			this.splitContainer2.Panel2.SuspendLayout();
			this.splitContainer2.SuspendLayout();
			this.statusStrip1.SuspendLayout();
			this.SuspendLayout();
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
			this.splitContainer2.Size = new System.Drawing.Size(689, 571);
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
			this.StreamingLivesimEditor.Size = new System.Drawing.Size(346, 571);
			this.StreamingLivesimEditor.TabIndex = 0;
			// 
			// BrokerLivesimEditor
			// 
			this.BrokerLivesimEditor.AutoScroll = true;
			this.BrokerLivesimEditor.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.BrokerLivesimEditor.Dock = System.Windows.Forms.DockStyle.Fill;
			this.BrokerLivesimEditor.Location = new System.Drawing.Point(0, 0);
			this.BrokerLivesimEditor.Name = "BrokerLivesimEditor";
			this.BrokerLivesimEditor.Size = new System.Drawing.Size(339, 571);
			this.BrokerLivesimEditor.TabIndex = 0;
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
			this.TssBtnPauseResume.Enabled = false;
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
			this.TssLblStrategyAsString.ToolTipText = "qwerqwerqwer";
			// 
			// toolTip1
			// 
			this.toolTip1.IsBalloon = true;
			this.toolTip1.UseAnimation = false;
			// 
			// LivesimControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.splitContainer2);
			this.Controls.Add(this.statusStrip1);
			this.Name = "LivesimControl";
			this.Size = new System.Drawing.Size(689, 593);
			this.splitContainer2.Panel1.ResumeLayout(false);
			this.splitContainer2.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
			this.splitContainer2.ResumeLayout(false);
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		public Core.Livesim.LivesimStreamingEditor StreamingLivesimEditor;
		public Core.Livesim.LivesimBrokerEditor BrokerLivesimEditor;
		private System.Windows.Forms.StatusStrip statusStrip1;
		public System.Windows.Forms.ToolStripButton TssBtnStartStop;
		public System.Windows.Forms.ToolStripButton TssBtnPauseResume;
		public System.Windows.Forms.ToolStripStatusLabel TssLblStrategyAsString;
		private System.Windows.Forms.ToolTip toolTip1;
	}
}
