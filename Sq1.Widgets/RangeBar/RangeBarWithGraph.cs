using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

namespace Sq1.Widgets.RangeBar {
	public abstract class RangeBarWithGraph<T> : RangeBar<T> {
		protected AsyncGraphBuilderBars GraphBuilder;

		[Browsable(true), DefaultValueAttribute(typeof(TextBox), null)]
		public float GraphMinHeightGoUnderLabels { get; set; }
		
		[Browsable(true), DefaultValueAttribute(typeof(TextBox), null)]
		public float GraphPenWidth { get; set; }
		[Browsable(true), DefaultValueAttribute(typeof(TextBox), null)]
		public Color ColorFgGraph { get; set; }

		private Pen penFgGraph;
		public Pen PenFgGraph {
			get {
				//if (this.ForeColor == null) this.ForeColor = Color.Black;
				if (this.penFgGraph == null) this.penFgGraph = new Pen(this.ColorFgGraph, this.GraphPenWidth);
				return this.penFgGraph;
			}
		}
		private SolidBrush brushFgGraph;
		public SolidBrush BrushFgGraph {
			get {
				//if (this.ForeColor == null) this.ForeColor = Color.Black;
				if (this.brushFgGraph == null) this.brushFgGraph = new SolidBrush(this.ColorFgGraph);
				return this.brushFgGraph;
			}
		}

		public virtual int GraphHeight { get {
			int graphYposition = this.GraphYposition;
			if (graphYposition == 0) return base.Height;	// take full height if (graphHeight < this.GraphMinHeightGoUnderLabels) 
			return this.ValueYposition - graphYposition; 
		} }
		
		public virtual int GraphYposition { get {
			int ret = this.PaddingInner.Top + this.LabelHeight;
			int graphHeight = this.ValueYposition - ret;
			if (graphHeight < this.GraphMinHeightGoUnderLabels) {
				ret = 0;
			}
			return ret;
		} }
		
		public RangeBarWithGraph() : base() {
			this.GraphPenWidth = 1;
			this.ColorFgGraph = Color.DarkSalmon;
			this.GraphPenWidth = 1;
			this.GraphMinHeightGoUnderLabels = 20;
			//NON_GENERIC_CHILD_SHOULD_CREATE_BUILDER
			//this.GraphBuilder = new AsyncGraphBuilder<T>(this);
		}
		public void RebuildGraph_inNewThread() {
			this.GraphBuilder.BuildGraph_inNewThread_andInvalidateDelayed();
		}
		protected override void OnResize(EventArgs e) {
			try {
				this.ResizeInitializedRebuildGraphVeryConditional();
				base.Invalidate();
				base.OnResize(e);
			} catch (Exception ex) {
				string msg = "WindProc won't catch your exceptions; keep a breakpoint here";
				throw ex;
			}
		}
		protected void ResizeInitializedRebuildGraphVeryConditional() {
			if (base.DesignMode) return; 
			if (this.GraphBuilder == null) {
				string msg = "this.GraphBuilder == null";
				return;
			}
			if (this.BufferedGraphics == null) {
				string msg = "this.BufferedGraphics == null";
				return;
			}
			if (this.BufferedGraphics.Graphics == null) return;
			if (this.BufferedGraphics.Graphics.VisibleClipBounds == null) return;
			if (this.GraphBuilder.ValueYinvertedForGraphicsWidth == null) return;
			
			#region iranai deshyo?...
			int newCalculationRequest = (int)this.BufferedGraphics.Graphics.VisibleClipBounds.Width;
			int alreadyCalculated = this.GraphBuilder.ValueYinvertedForGraphicsWidth.Length;
			bool resizedWith = newCalculationRequest != alreadyCalculated; 
			bool resizedHeight = (int)this.BufferedGraphics.Graphics.VisibleClipBounds.Height != this.GraphBuilder.PrevCalculationHeight;
			if (resizedWith || resizedHeight) {
				string msg = "already have pre-calculated graph for this width & height";
				return;
			}
			#endregion
			this.GraphBuilder.BuildGraph_inNewThread_andInvalidateDelayed();
		}
		protected override void DrawGraph(Graphics g) {
			if (base.DesignMode) return;
			if (this.GraphBuilder == null) return;
			if (this.GraphBuilder.HasDataToDraw && this.GraphBuilder.ValueYinvertedForGraphicsWidth == null) {
				this.GraphBuilder.BuildGraph_inNewThread_andInvalidate();
				return;
			}
			if (this.GraphBuilder.IsCalculating) {
				return;
			}
			this.GraphBuilder.SpillGraphOnGraphics(g);
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				this.PenFgGraph.Dispose();
				this.GraphBuilder.Dispose();
				this.BrushFgGraph.Dispose();
				//if (components != null) components.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}
