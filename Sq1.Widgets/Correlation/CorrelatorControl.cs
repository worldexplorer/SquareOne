using System;
using System.Windows.Forms;
using System.Collections.Generic;

using Sq1.Core;
using Sq1.Core.Sequencing;
using Sq1.Core.Correlation;
using Sq1.Core.Repositories;
using Sq1.Core.DataTypes;

namespace Sq1.Widgets.Correlation {
	public partial class CorrelatorControl : UserControl {
				SequencedBacktests		sequencedOriginal;
				SequencedBacktests		sequencedChosen;
		public	Correlator				Correlator			{ get; private set; }
		public  string					PriceFormat			{ get; private set; }

		public CorrelatorControl() {
			InitializeComponent();
			this.toolStripItemTrackBarWalkForward.ValueCurrentChanged += new EventHandler<EventArgs>(toolStripItemTrackBar1_ValueCurrentChanged);
			this.toolStripItemTrackBarWalkForward.WalkForwardCheckedChanged += new EventHandler<EventArgs>(toolStripItemTrackBar1_WalkForwardCheckedChanged);
		}

		public void Initialize(Correlator correlator) {
			this.Correlator = correlator;
			this.oneParameterControl1.Initialize_byMovingControlsToInner();
		}

		public void Initialize(SequencedBacktests originalSequencedBacktests, string relPathAndNameForSequencerResults, string fileName) {
			if (originalSequencedBacktests == null) {
				string msg = "DONT_PASS_NULL_originalSequencedBacktests";
				Assembler.PopupException(msg);
				return;
			}
			if (this.sequencedOriginal != null && this.sequencedOriginal.ToString() == originalSequencedBacktests.ToString()) return;

			RepositorySerializerSymbolInfos symbolInfoRep = Assembler.InstanceInitialized.RepositorySymbolInfos;
			if (this.sequencedOriginal != null) {
				SymbolInfo symbolInfoOld = symbolInfoRep.FindSymbolInfo_nullUnsafe(this.sequencedOriginal.Symbol);
				if (symbolInfoOld != null) {
					symbolInfoOld.PriceDecimalsChanged -= new EventHandler<EventArgs>(reloadNetWhenSymbolInfoChanged_PriceDecimalsChanged);
				} else {
					string msg = "SYMBOL_WAS_NOT_SERIALIZED_IN_foundBySymbolScaleRange[" + this.sequencedOriginal.Symbol + "]";
					Assembler.PopupException(msg);
				}
			}

			SymbolInfo symbolInfoNew = symbolInfoRep.FindSymbolInfo_nullUnsafe(originalSequencedBacktests.Symbol);
			if (symbolInfoNew != null) {
				symbolInfoNew.PriceDecimalsChanged += new EventHandler<EventArgs>(reloadNetWhenSymbolInfoChanged_PriceDecimalsChanged);
			} else {
				string msg = "SYMBOL_WAS_NOT_SERIALIZED_IN_foundBySymbolScaleRange[" + originalSequencedBacktests.Symbol + "]";
				Assembler.PopupException(msg);
			}

			this.sequencedOriginal = originalSequencedBacktests;

			if (this.sequencedOriginal.Count > 0) {
				//v1
				//int i = 0;
				//int scanFirstLimit = 200;
				//string symbolFound = null;
				//foreach (SystemPerformanceRestoreAble firstScanned in this.sequencedOriginal.Backtests) {
				//	if (string.IsNullOrEmpty(firstScanned.Symbol) == false) {
				//		symbolFound = firstScanned.Symbol;
				//	}
				//	this.PriceFormat = firstScanned.PriceFormat;
				//	if (string.IsNullOrEmpty(this.PriceFormat) == false) break;
				//	if (++i >= scanFirstLimit) break;
				//}
				string symbolFound = this.sequencedOriginal.Symbol;
				if (string.IsNullOrEmpty(symbolFound)) {
					string msg = "SYMBOL_WAS_NOT_SERIALIZED_IN_foundBySymbolScaleRange[" + fileName + "]";
					Assembler.PopupException(msg);
				} else {
					if (string.IsNullOrEmpty(this.PriceFormat)) {
						this.PriceFormat = Assembler.InstanceInitialized.RepositorySymbolInfos.FindSymbolInfoOrNew(symbolFound).PriceFormat;
					}
				}
			}
			if (string.IsNullOrEmpty(this.PriceFormat)) {
				this.PriceFormat = "N1";
			}

			try {
				this.setCursorWait();
				this.Correlator.Initialize(originalSequencedBacktests, relPathAndNameForSequencerResults, fileName);

				// only available after Correlator.Initialize otherwize NPE due to this.sequencedOriginal=null
				this.toolStripItemTrackBarWalkForward.ValueCurrent = (decimal)this.Correlator.SubsetPercentage;
				this.toolStripItemTrackBarWalkForward.WalkForwardChecked = this.Correlator.SubsetPercentageFromEnd;
				//this.toolStripItemTrackBarWalkForward.WalkForwardEnabled = this.toolStripItemTrackBarWalkForward.ValueCurrent < 100;

				this.flushCalculationsToGui();
			} catch (Exception ex) {
				Assembler.PopupException("SHOULD_NEVER_HAPPEN_I_ONLY_NEEDED_FINALLY", ex);
			} finally {
				this.setCursorDefault();
			}
		}

		void reloadNetWhenSymbolInfoChanged_PriceDecimalsChanged(object sender, EventArgs e) {
			if (base.InvokeRequired) {
				string msg = "NYI__SYMBOL_INFO.PRICE_DECIMALS__CHANGED_IN_A_NON_GUI_THREAD //OneParameterControl.reloadNetWhenSymbolInfoChanged_PriceDecimalsChanged()";
				Assembler.PopupException(msg);
				return;
			}
			SymbolInfo iUpdatedPriceFormat = sender as SymbolInfo;
			if (iUpdatedPriceFormat == null) {
				string msg = "MUST_BE_SymbolInfo_sender[" + sender + "] //reloadNetWhenSymbolInfoChanged_PriceDecimalsChanged()";
				Assembler.PopupException(msg);
				return;
			}

			this.PriceFormat = iUpdatedPriceFormat.PriceFormat;
			foreach (OneParameterControl eachControl in this.allParameterControlsOpen.Values) {
				eachControl.OlvRebuildColumns();
			}
		}

		Dictionary<OneParameterAllValuesAveraged, OneParameterControl> allParameterControlsOpen { get {
			Dictionary<OneParameterAllValuesAveraged, OneParameterControl> ret = new Dictionary<OneParameterAllValuesAveraged, OneParameterControl>();
			foreach (Control ctrl in this.flowLayoutPanel1.Controls) {
				OneParameterControl casted = ctrl as OneParameterControl;
				if (casted == null) {
					string msg = "OneParameterControl_ONLY_ARE_EXPECTED__GOT_WEIRDO_IN_flowLayoutPanel1: " + ctrl.GetType() + "/" + ctrl.ToString();
					continue;
				}
				ret.Add(casted.Parameter, casted);
			}
			return ret;
		} }
		void flushCalculationsToGui() {
			if (base.InvokeRequired) {
				//base.BeginInvoke((MethodInvoker)delegate { this.flushCalculationsToGui(); });
				base.BeginInvoke(new MethodInvoker(this.flushCalculationsToGui));
				return;
			}
			this.flowLayoutPanel1.Controls.Clear();
			foreach (string parameterName in this.Correlator.ParametersByName.Keys) {
				OneParameterAllValuesAveraged oneParamForOneOLV = this.Correlator.ParametersByName[parameterName];
				if (oneParamForOneOLV.OneParamOneValueByValues.Count <= 1) continue;
				//List<OneParameterOneValue> objectsForList = oneParamForOneOLV.AllValuesForOneParameterWithAverages;
				OneParameterControl adding = new OneParameterControl(this, oneParamForOneOLV);
				//OneParameterControlAsMdiForm adding = new OneParameterControlAsMdiForm(this, oneParamForOneOLV);
				//OneParameterControl adding = resizableWrapper.OneParameterControl;
				this.flowLayoutPanel1.Controls.Add(adding);
			}
		}

		void setCursorDefault() {
			if (base.InvokeRequired) {
				base.BeginInvoke((MethodInvoker)delegate { this.setCursorDefault(); });
				return;
			}
			this.Cursor = Cursors.Default;
		}

		void setCursorWait() {
			if (base.InvokeRequired) {
				base.BeginInvoke((MethodInvoker)delegate { this.setCursorWait(); });
				return;
			}
			this.Cursor = Cursors.WaitCursor;
		}
	}
}
