using System;
using System.Windows.Forms;
using System.Drawing;

using Sq1.Core;

namespace Sq1.Charting {
	public partial class PanelBase {
		protected override void OnResize(EventArgs e) {	//PanelDoubleBuffered does this already to DisposeAndNullify managed Graphics
#if NON_DOUBLE_BUFFERED	//SAFE_TO_UNCOMMENT_COMMENTED_OUT_TO_MAKE_C#DEVELOPER_EXTRACT_METHOD
			if (base.DesignMode) {
				base.OnResize(e);
				return;
			}

			string msg = "WHO_INVOKES_ME? SetHeightIgnoreResize()";
			if (Assembler.IsInitialized && Assembler.InstanceInitialized.MainFormDockFormsFullyDeserializedLayoutComplete == false) return;
			if (this.ignoreResizeImSettingWidthOrHeight) return;

			base.OnResize(e);	// empty inside but who knows what useful job it does?
			base.Invalidate();	// SplitterMoved, FormResized => repaint; Panel and UserControl don't have that (why?)
#else
			if (this.DisplayRectangle.Width != base.ClientRectangle.Width) {
				int a = 0;
			}
			// this.SetWidthIgnoreResize(base.ClientSize.Width - 8);
			//this.SetHeightIgnoreResize(base.ClientSize.Height - 8);

			//BOTH_WORK_PERFECT!!! VERTICAL_NON_FULLY_REPAINT_EXTENDED_LEFT_TO_SOLVE NO_EFFECT
			base.OnResize(e);
			//BOTH_WORK_PERFECT!!! VERTICAL_NON_FULLY_REPAINT_EXTENDED_SOLVED_AS_WELL GOES_VERY_BAD
			base.Invalidate();	// VERTICAL_NON_FULLY_REPAINT_EXTENDED_LEFT_TO_SOLVE
#endif
		}

#if NON_DOUBLE_BUFFERED	//SAFE_TO_UNCOMMENT_COMMENTED_OUT_TO_MAKE_C#DEVELOPER_EXTRACT_METHOD
		protected override void OnPaint(PaintEventArgs e) {
			if (base.DesignMode) {
				base.OnPaint(e);
				return;
			}
			base.OnPaint(e);
#else
		protected override void OnPaintDoubleBuffered(PaintEventArgs e) {
#endif
			string msig = " " + this.PanelName + ".OnPaintDoubleBuffered()";
			if (this.ChartControl == null) {
				string msg = "PanelNamedFolding[" + this.PanelName + "].ChartControl=null; invoke PanelNamedFolding.Initialize() from derived.ctor()";
				//Assembler.PopupException(msg + msig, null, false);
				this.DrawError(e.Graphics, msg);
				return;
			}
			if (this.ChartControl.BarsEmpty) {
				string msg = "CHART_CONTROL_BARS_NULL_OR_EMPTY: this.ChartControl.BarsEmpty ";
				//if (this.ChartControl.Bars != null) msg = "BUG: bars=[" + this.ChartControl.Bars + "]";
				//Assembler.PopupException(msg + msig, null, false);
				this.DrawError(e.Graphics, msg + msig);
				return;
			}
			if (this.ChartControl.PaintAllowedDuringLivesimOrAfterBacktestFinished == false) return;

			//v1 if (this.VisibleBarLeft_cached != this.ChartControl.Bars.Count) {
			if (this.VisibleBarLeft_cached != this.ChartControl.VisibleBarLeft) {
				//if (this.ChartControl.RefreshAllPanelsIsSignalled == true) { }
				string msg = ""
					+ " VisibleBarLeft_cached[" + this.VisibleBarLeft_cached + "]!=VisibleBarLeft[" + this.ChartControl.VisibleBarLeft + "]"
					+ " FORCING_BACKGROUND_PAINT_SYNC_KOZ_REFRESH()_INVOKES_ONLY_FOREGROUND"
					//+ " ChartControl.RefreshAllPanelsIsSignalled=" + this.ChartControl.RefreshAllPanelsIsSignalled
					//+ " ImPaintingBackgroundNow=" + this.ImPaintingBackgroundNow
					//+ " ImPaintingForegroundNow=" + this.ImPaintingForegroundNow
					;
#if NON_DOUBLE_BUFFERED	//SAFE_TO_UNCOMMENT_COMMENTED_OUT_TO_MAKE_C#DEVELOPER_EXTRACT_METHOD
				this.OnPaintBackground(e);
#else
				this.OnPaintBackgroundDoubleBuffered(e);
#endif
				msg = "GOT VisibleBarLeft_cached[" + this.VisibleBarLeft_cached + "] VisibleBarLeft[" + this.ChartControl.VisibleBarLeft + "] AFTER" + msg;
				//Assembler.PopupException(msg, null, false);
			}

			try {
				//DIDNT_MOVE_TO_PanelDoubleBuffered.OnPaint()_CHILDREN_DONT_GET_WHOLE_SURFACE_CLIPPED
				e.Graphics.SetClip(base.ClientRectangle);	// always repaint whole Panel; by default, only extended area is "Clipped"
			
				this.ChartControl.SyncHorizontal_scrollToBarsCount();

				//if (this.VisibleRangeWithTwoSqueezers_cached <= 0) {
				//	string msg = "MUST_BE_POSITIVE#2_this.VisibleRangeWithTwoSqueezers_cached[" + this.VisibleRangeWithTwoSqueezers_cached + "] panel[" + this.ToString() + "]";
				//	Assembler.PopupException(msg + msig);
				//	return;
				//}
			
				if (this.PanelHasValuesForVisibleBarWindow == false) {
					string msg = "PANEL_BASE_PAINT_ENTRY_POINT_PROTECTS_DERIVED_FROM_INVOKING_PaintWholeSurfaceBarsNotEmpty()"
						+ " occurs for JSON-Scripted Strategies with PanelIndicator* open without indicator's data"
						+ " moving beyond right bar makes all panels blank";
					//Assembler.PopupException(msg + msig, null, false);
					return;
				}
				
				this.RepaintSernoForeground++;
				
				this.PaintWholeSurfaceBarsNotEmpty(e.Graphics);	// GOOD: we get here once per panel
				// BT_ONSLIDERS_OFF>BT_NOW>SWITCH_SYMBOL=>INDICATOR.OWNVALUES.COUNT=0=>DONT_RENDER_INDICATORS_BUT_RENDER_BARS
				this.RenderIndicators(e.Graphics); 
				
				if (this.PanelName == null) {
					this.DrawError(e.Graphics, "SET_TO_EMPTY_STRING_TO_HIDE: this.PanelName=null");
					return;
				}
				// draw Panel Title on top of anything that the panel draws
				if (this.thisPanelIsIndicatorPanel) {
					PanelIndicator meCasted = this as PanelIndicator;
					//PanelIndicator should draw PanelName with Indicator.LineColor
					using (var brush = new SolidBrush(meCasted.Indicator.LineColor)) {
						// if (base.DesignMode) this.ChartControl will be NULL
						Font font = (this.ChartControl != null) ? this.ChartControl.ChartSettingsTemplated.PanelNameAndSymbolFont : this.Font;
						e.Graphics.DrawString(this.PanelNameAndSymbol, font, brush, new Point(2, 2));
					}
				} else {
					using (var brush = new SolidBrush(this.ForeColor)) {
						// if (base.DesignMode) this.ChartControl will be NULL
						Font font = (this.ChartControl != null) ? this.ChartControl.ChartSettingsTemplated.PanelNameAndSymbolFont : this.Font;
						e.Graphics.DrawString(this.PanelNameAndSymbol, font, brush, new Point(2, 2));
					}
				}
				//this.ChartControl.ChartSettings.DisposeAllGDIs_handlesLeakHunter();
			} catch (Exception ex) {
				string msg = "OnPaintDoubleBuffered(): caught[" + ex.Message + "]";
				//Assembler.PopupException(msg + msig, ex);
				this.DrawError(e.Graphics, msg);
			}
		}
	
		protected	int RepaintSernoForeground;
		protected	int RepaintSernoBackground;

#if NON_DOUBLE_BUFFERED	//SAFE_TO_UNCOMMENT_COMMENTED_OUT_TO_MAKE_C#DEVELOPER_EXTRACT_METHOD 
		protected override void OnPaintBackground(PaintEventArgs e) {
			if (base.DesignMode) {
				base.OnPaintBackground(e);
				return;
			}
			if (Assembler.InstanceInitialized.MainFormDockFormsFullyDeserializedLayoutComplete == false) return;
			base.OnPaintBackground(e);
#else
		protected override void OnPaintBackgroundDoubleBuffered(PaintEventArgs e) {
			if (this.ChartControl == null) {
				base.OnPaintBackgroundDoubleBuffered(e);	// will e.Graphics.Clear(base.BackColor);
				return;
			}
			if (Assembler.InstanceInitialized.MainForm_dockFormsFullyDeserialized_layoutComplete == false) return;
			if (this.ChartControl != null && this.ChartControl.PaintAllowedDuringLivesimOrAfterBacktestFinished == false) {
				if (this.Cursor != Cursors.WaitCursor) this.Cursor = Cursors.WaitCursor;
				return;
			}
#endif

			string msig = " " + this.PanelName + ".OnPaintBackgroundDoubleBuffered()";

			//DIDNT_MOVE_TO_PanelDoubleBuffered.OnPaint()_CHILDREN_DONT_GET_WHOLE_SURFACE_CLIPPED
			//if (this.DesignMode) return;
			e.Graphics.SetClip(base.ClientRectangle);	// always repaint whole Panel; by default, only extended area is "Clipped"
			if (this.ChartControl == null) {
				//e.Graphics.Clear(SystemColors.Control);
				e.Graphics.Clear(base.BackColor);
				using (Pen penRed = new Pen(Color.Brown, 1)) {
					e.Graphics.DrawLine(penRed, 0, 0, base.Width, base.Height);
				}
				this.ChartLabelsUpperLeftYincremental = 10;
				this.DrawError(e.Graphics, "OnPaintBackgroundDoubleBuffered got this.ChartControl=null");
				return;
			}
			if (this.ChartControl.BarsEmpty) {
				string msg = "CHART_CONTROL_BARS_NULL_OR_EMPTY: this.ChartControl.BarsEmpty ";
				//if (this.ChartControl.Bars != null) msg = "BUG: bars=[" + this.ChartControl.Bars + "]";
				//e.Graphics.Clear(SystemColors.Control);
				e.Graphics.Clear(base.BackColor);
				using (Pen penRed = new Pen(Color.Brown, 1)) {
					e.Graphics.DrawLine(penRed, 0, 0, base.Width, base.Height);
				}
				this.DrawError(e.Graphics, msig + msg);
				return;
			}
			
			if (this.ChartControl.BarsCanFitForCurrentWidth <= 0) {
				string msg = "NEVER_HAPPENED_SO_FAR still resizing?...";
				Assembler.PopupException(msg, null, false);
				e.Graphics.Clear(base.BackColor);
				this.DrawError(e.Graphics, msig + msg);
				return;
			}
			try {
				this.ChartLabelsUpperLeftYincremental = this.ChartControl.ChartSettingsTemplated.ChartLabelsUpperLeftYstartTopmost;
				//if (this.ChartControl.BarsNotEmpty) {}
				this.ChartControl.SyncHorizontal_scrollToBarsCount();
			
				e.Graphics.Clear(this.ChartControl.ChartSettingsTemplated.ChartColorBackground);
				
				this.RepaintSernoBackground++;
				if (this.ChartControl.PaintAllowedDuringLivesimOrAfterBacktestFinished == false) {
					//v1
					//this.DrawError(e.Graphics, "BACKTEST_IS_RUNNING_WAIT");
					//string msgRepaint = "repaintFore#" + this.RepaintSernoForeground + "/Back#" + this.RepaintSernoBackground;
					//this.DrawError(e.Graphics, msgRepaint);
					//if (this.Cursor != Cursors.WaitCursor) this.Cursor = Cursors.WaitCursor;
					//v2
					string msg = "MOVED_TO_THE_BEGINNING__SKIPPING_ALL_REPAINTS_DURING_BACKTEST__SETTING_WaitCursor";
					Assembler.PopupException(msg);
					return;
				}
				//if (this.Cursor != Cursors.Default) this.Cursor = Cursors.Default;
				if (this.dragButtonPressed == false) this.Cursor = Cursors.Default;

				
				// TODO: we get here 4 times per Panel: DockContentHandler.SetVisible, set_FlagClipWindow, WndProc * 2
				
				this.VisibleBarRight_cached = this.ChartControl.VisibleBarRight;
				if (this.VisibleBarRight_cached == 0) {
					if (this.ChartControl.Bars.Count == 0) {
						string msg = "this.ChartControl.VisibleBarRight_MUST_BE_POSITIVE_KOZ_YOU_HAVE_BARS[" + this.ChartControl.Bars.Count  + "]";
						Assembler.PopupException(msg, null, false);
					}
					//BETTER_LET_IT_COMPLAIN__RANGE_CAN_NOT_BE_ZERO_WHEN_YOU_HAVE_BARS.COUNT=1
					//if (this.ChartControl.Bars.Count == 1) {
					//	string msg = "AVOIDING RANGE_CAN_NOT_BE_ZERO_WHEN_YOU_HAVE_BARS.COUNT=1";
					//	this.VisibleBarRight_cached = 1;
					//}
					this.VisibleBarLeft_cached = this.ChartControl.VisibleBarLeft;
					return;
				}
				if (this.VisibleBarRight_cached < 0) {
					string msg = "NEVER_HAPPENED_SO_FAR this.VisibleBarRight_cached[" + this.VisibleBarRight_cached + "] < 0";
					Assembler.PopupException(msg);
				}
				//v1
				if (this.VisibleBarRight_cached > this.ValueIndexLastAvailable_minusOneUnsafe) {
					return;
				}
				//v2
				if (this.PanelHasValuesForVisibleBarWindow == false) {
					//Debugger.Break();
					return;
				}

				this.VisibleBarLeft_cached = this.ChartControl.VisibleBarLeft;
				this.VisibleBarsCount_cached = this.VisibleBarRight_cached - this.VisibleBarLeft_cached;
				if (this.VisibleBarsCount_cached <= 0) {
					string msg = "NEVER_HAPPENED_SO_FAR already checked this.BarsCanFitForCurrentWidth <= 0";
					Assembler.PopupException(msg);
					return;
				}
	
				this.BarWidth_includingPadding_cached	= this.ChartControl.ChartSettingsIndividual.BarWidthIncludingPadding;
				this.BarWidthMinusRightPadding_cached	= this.ChartControl.ChartSettingsIndividual.BarWidthMinusRightPadding;
				this.BarShadowXoffset_cached			= this.ChartControl.ChartSettingsIndividual.BarShadowXoffset;
				
				this.ensureFontMetricsAreCalculated(e.Graphics);	//MOVED_HERE_FROM_MOVE_UPSTACK_THIS_FONT_HEIGHT_CALCULATION
//DEBUGGING_FOR_MOVED_HERE_FROM_MOVE_UPSTACK_THIS_FONT_HEIGHT_CALCULATION remove next commit
//				if (this.GutterBottomDraw && this.PanelHeightMinusGutterBottomHeight_cached <= 0) {
//					Debugger.Break();
//				}
//				if (this.PanelHeightMinusGutterBottomHeight_cached != 0
//						&& this.PanelHeightMinusGutterBottomHeight_cached != this.PanelHeightMinusGutterBottomHeight) {
//					Debugger.Break();
//				}
				this.PanelHeight_minusGutterBottomHeight_cached = this.PanelHeight_minusGutterBottomHeight;
				if (this.PanelHeight_minusGutterBottomHeight_cached <= 0) {
					string msg = "WASTED_ASSIGNMENT_WILL_THROW_SOON"
						+ " this.PanelHeightMinusGutterBottomHeight[" + this.PanelHeight_minusGutterBottomHeight_cached + "]<=0";
					Assembler.PopupException(msg, null, false);
				}
				
				this.PaintBackgroundWholeSurfaceBarsNotEmpty(e.Graphics);
				//this.ChartControl.ChartSettings.DisposeAllGDIs_handlesLeakHunter();
			} catch (Exception ex) {
				string msg = "OnPaintBackgroundDoubleBuffered(): caught[" + ex.Message + "]";
				Assembler.PopupException(msg, ex);
				this.DrawError(e.Graphics, msg);
			}
		}

		//protected override void OnLayout(System.Windows.Forms.LayoutEventArgs levent) {
		//	 base.OnLayout(levent);
		//}
		//protected override void SetClientSizeCore(int x, int y) {
		//	base.SetClientSizeCore(x, y);
		//}
		//protected override void OnClientSizeChanged(EventArgs e) {
		//	base.OnClientSizeChanged(e);
		//}
	}
}