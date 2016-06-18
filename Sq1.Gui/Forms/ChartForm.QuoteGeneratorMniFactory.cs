using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Core.Backtesting;
using Sq1.Core.StrategyBase;

namespace Sq1.Gui.Forms {
	public partial class ChartForm {
		static string PARENT_MNI_PREFIX = "QuotesGenerator: ";
		static string MNI_PREFIX = "mniQuoteGenerator_";

		public ToolStripItem[] QuoteGeneratorMenuItems { get {
			List<BacktestStrokesPerBar> generatorsPotentiallyImplemented = new List<BacktestStrokesPerBar>() {
				BacktestStrokesPerBar.Unknown,
				BacktestStrokesPerBar.FourStrokeOHLC,
				BacktestStrokesPerBar.TenStroke,
				BacktestStrokesPerBar.SixteenStroke
			};

			BacktestStrokesPerBar current = this.currentStrokes();
			List<ToolStripMenuItem> ret = new List<ToolStripMenuItem>();
			foreach (BacktestStrokesPerBar generator in generatorsPotentiallyImplemented) {
				string generatorName = Enum.GetName(typeof(BacktestStrokesPerBar), generator);
				ToolStripMenuItem mniFabricated_quoteGenerator = new ToolStripMenuItem();
				mniFabricated_quoteGenerator.Text = generatorName;
				mniFabricated_quoteGenerator.Name = MNI_PREFIX + generatorName;
				mniFabricated_quoteGenerator.CheckOnClick = true;
				mniFabricated_quoteGenerator.Click += new EventHandler(mniQuoteGeneratorFabricated_Click);
				if (generator == current) mniFabricated_quoteGenerator.Checked = true;
				if (generator == BacktestStrokesPerBar.Unknown) mniFabricated_quoteGenerator.Enabled = false;
				ret.Add(mniFabricated_quoteGenerator);
			}

			return ret.ToArray();
		} }

		BacktestStrokesPerBar currentStrokes() {
			BacktestStrokesPerBar current = BacktestStrokesPerBar.Unknown;
			string strategyAsString = "";
			try {
				ScriptExecutor executor = this.ChartFormManager.Executor;
				if (executor == null) return current;
				if (executor.Strategy == null) return current;
				strategyAsString = executor.Strategy.ToString();
				current = executor.Strategy.ScriptContextCurrent.BacktestStrokesPerBar;
			} catch (Exception ex) {
				string msg = "COULDNT_FIGURE_OUT_CURRENT_QUOTE_GENERATOR_FOR " + strategyAsString;
				Assembler.PopupException(msg, ex);
			}
			return current;
		}

		void mniQuoteGeneratorFabricated_Click(object sender, EventArgs e) {
			ToolStripMenuItem mni = sender as ToolStripMenuItem;
			if (mni == null) {
				string msg = "sender_MUST_BE_ToolStripMenuItem " + sender;
				Assembler.PopupException(msg, null, false);
				return;
			}

			string clickedGenerator = mni.Text;
			if (string.IsNullOrEmpty(clickedGenerator)) {
				string msg = "mni.Text_MUST_BE_[FourStrokeOHLC]/[NineStroke]/[TwelveStrokeMT5]: " + mni.Tag;
				Assembler.PopupException(msg, null, false);
				return;
			}

			try {
				// I_HATE_WHEN_ONLY_LAST_ONE_SHOWS_UP this.ctxStrokesForQuoteGenerator.Visible = true;
				this.ctxBacktest.Visible = true;

				BacktestStrokesPerBar generatorStrokeAmount = (BacktestStrokesPerBar)Enum.Parse(typeof(BacktestStrokesPerBar), clickedGenerator);
				Backtester backtester = this.ChartFormManager.Executor.BacktesterOrLivesimulator;
				BacktestQuotesGenerator clone = BacktestQuotesGenerator.CreateForQuotesPerBar_initialize(generatorStrokeAmount, backtester);
				backtester.SetQuoteGeneratorAndConditionallyRebacktest_invokedInGuiThread(clone);
				this.ctxStrokesPopulate_orSelectCurrent();
				if (backtester.ImRunningLivesim) {
					string msg = "DONT_ABORT_LIVESIM__JUST_RETURN_NOW_HERE"
						+ " EVEN_IF_PREV_GENERATOR_DIDNT_FINISH_GENERATING__NEXT_BAR_WILL_BE_GENERATED_BY_THE_NEW_ONE PAUSING_QUOTE_PUMP_IS_NOT_REQUIRED";
					//Assembler.PopupException(msg, null, false);
					return;
				}
				// to inform SequencerControl of new strokes selected
				this.ChartFormManager.PopulateSelectors_fromCurrentChartOrScriptContext_loadBars_saveStrategyOrCtx_backtestIfStrategy("ChartForm_OnBacktestStrokesClicked");
				this.ChartFormManager.SequencerFormIfOpen_propagateTextboxesOrMarkStaleResultsAndDeleteHistory();
			} catch (Exception ex) {
				string msg = "REBACKTEST_FAILED?";
				Assembler.PopupException(msg, ex, false);
			}
		}

		void ctxStrokesForQuoteGenerator_Opening_SelectCurrent(object sender, CancelEventArgs e) {
			ContextMenuStrip ctx = sender as ContextMenuStrip;
			if (ctx == null) {
				string msg = "sender_MUST_BE_ContextMenuStrip " + sender;
				Assembler.PopupException(msg, null, false);
				return;
			}
			if (ctx != this.ctxStrokesForQuoteGenerator) {
				string msg = "sender_MUST_BE_ctxStrokesForQuoteGenerator " + sender;
				Assembler.PopupException(msg, null, false);
				return;
			}
			this.ctxStrokesPopulate_orSelectCurrent();
		}

		void ctxStrokesPopulate_orSelectCurrent() {
			if (this.ctxStrokesForQuoteGenerator.Items.Count == 0) {
				this.ctxStrokesForQuoteGenerator.Items.AddRange(this.QuoteGeneratorMenuItems);
			}

			BacktestStrokesPerBar current = this.currentStrokes();
			string currentAsString = Enum.GetName(typeof(BacktestStrokesPerBar), current);
			foreach (var tsi in this.ctxStrokesForQuoteGenerator.Items) {
				ToolStripMenuItem mni = tsi as ToolStripMenuItem;
				if (mni == null) {
					string msg = "ALL_MUST_BE_ToolStripMenuItem_ctx[" + this.ctxStrokesForQuoteGenerator.Text + "].Items tsi[" + tsi + "]";
					Assembler.PopupException(msg);
					continue;
				}
				mni.Checked = (mni.Text == currentAsString);
			}
			this.mniStrokes.Text = PARENT_MNI_PREFIX + "[" + currentAsString + "]";
		}
	}
}
