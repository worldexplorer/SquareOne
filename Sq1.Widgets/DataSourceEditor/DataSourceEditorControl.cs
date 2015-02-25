using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Core.Broker;
using Sq1.Core.DataFeed;
using Sq1.Core.DataTypes;
using Sq1.Core.Streaming;
using Sq1.Core.Support;

namespace Sq1.Widgets.DataSourceEditor {
	public partial class DataSourceEditorControl : UserControl, IDataSourceEditor {
		DataSource ds;
		public string DataSourceName { get {
				if (this.ds == null) return "NO_DATASOURCE_LOADED_FOR_EDITING";
				return this.ds.Name;
			} }
		public Dictionary<string, StreamingAdapter> StreamingAdaptersByName { get; private set; }
		public Dictionary<string, BrokerAdapter> BrokerAdaptersByName { get; private set; }
		public string symbolsDefault;
		public string windowTitleDefault;
		Assembler assemblerInstance;

		public DataSourceEditorControl() {
			this.symbolsDefault = "RIZ2,RIH3";
			InitializeComponent();
		}

		public void InitializeAdapters(
				Dictionary<string, StreamingAdapter> streamingAdaptersByName,
				Dictionary<string, BrokerAdapter> brokerAdaptersByName) {
			this.StreamingAdaptersByName = streamingAdaptersByName;
			this.BrokerAdaptersByName = brokerAdaptersByName;
		}
		public void InitializeContext(Assembler assembler) {
			this.assemblerInstance = assembler;
		}
		public void Initialize(DataSource dsEdit) {
			this.panel_0 = this.pnlIntro;
			if (dsEdit == null) {
				throw new Exception("DataSourceEditor can not create the DataSource; pass an existing datasource for editing, not NULL");
			}
			this.ds = dsEdit;
			if (this.Parent != null) {
				this.Parent.Text = "DataSourceEdit :: " + ds.Name;
			} else {
				this.Text = ds.Name;
			}
			if (this.ds.Name != windowTitleDefault) {
				this.txtDataSourceName.Text = this.ds.Name;
			}
			this.txtSymbols.Text = this.ds.SymbolsCSV;
			this.PopulateScaleIntervalFromDataSource();
			this.PopulateStreamingBrokerListViewsFromDataSource();

			if (this.ds.StreamingAdapter != null) 	this.highlightStreamingByName(this.ds.StreamingAdapter.GetType().Name);
			else									this.highlightStreamingByName(StreamingAdapter.NO_STREAMING_ADAPTER);

			if (this.ds.BrokerAdapter != null)		this.highlightBrokerByName(this.ds.BrokerAdapter.GetType().Name);
			else 									this.highlightBrokerByName(BrokerAdapter.NO_BROKER_ADAPTER);

			this.marketInfoEditor.Initialize(ds, this.assemblerInstance.RepositoryJsonDataSource, this.assemblerInstance.RepositoryMarketInfo);
		}
		public void PopulateScaleIntervalFromDataSource() {
			this.cmbScale.Items.Clear();
			int indexSelected = 0;
			int i = 0;
			foreach (BarScale barScale in Enum.GetValues(typeof(BarScale))) {
				this.cmbScale.Items.Add(barScale.ToString());
				if (this.ds.ScaleInterval == null) {
					if (barScale == BarScale.Unknown) indexSelected = i;  
				} else {
					if (this.ds.ScaleInterval.Scale == barScale) indexSelected = i;
				}
				i++;
			}
			this.cmbScale.SelectedIndex = indexSelected;
			if (this.ds.ScaleInterval != null) {
				this.nmrInterval.Value = ds.ScaleInterval.Interval;
			}
		}
		public void PopulateStreamingBrokerListViewsFromDataSource() {
			if (base.IsDisposed) {
				string msg = "base.IsDisposed=true for DataSourceEditorForm::PopulateStaticStreamingBrokerListViewsFromDataSource()";
				Assembler.PopupException(msg);
				return;
			}
			
			this.lvStreamingAdapters.Items.Clear();
			ListViewItem lviAbsentStreaming = new ListViewItem() {
				Text = StreamingAdapter.NO_STREAMING_ADAPTER,
				Name = StreamingAdapter.NO_STREAMING_ADAPTER,
			};
			this.lvStreamingAdapters.Items.Add(lviAbsentStreaming);
			foreach (StreamingAdapter streamingAdapterPrototype in StreamingAdaptersByName.Values) {
				try {
					StreamingAdapter streamingAdapterInstance = null;	// streamingAdapterPrototype;
					if (ds.StreamingAdapter != null && ds.StreamingAdapter.GetType().FullName == streamingAdapterPrototype.GetType().FullName) {
						streamingAdapterInstance = ds.StreamingAdapter;
					}
					// I still want to get a new instance, so if user choses it, I'll Initialize() it and put into serialize-able DataSource
					if (streamingAdapterInstance == null) {
						streamingAdapterInstance = (StreamingAdapter)Activator.CreateInstance(streamingAdapterPrototype.GetType());
					}

					if (streamingAdapterInstance.EditorInstanceInitialized == false) {
						try {
							streamingAdapterInstance.StreamingEditorInitialize(this);
						} catch (Exception e) {
							string msg = "can't initialize streamingAdapterInstance[" + streamingAdapterInstance + "]";
							Assembler.PopupException(msg, e);
							return;
						}
					}

					ListViewItem lvi = new ListViewItem() {
						Text = streamingAdapterInstance.Name,
						Name = streamingAdapterInstance.GetType().Name,
						Tag = streamingAdapterInstance
					};
					if (streamingAdapterInstance.Icon != null) {
						this.imglStreamingAdapters.Images.Add(streamingAdapterInstance.Icon);
						lvi.ImageIndex = this.imglStreamingAdapters.Images.Count - 1;
					}
					this.lvStreamingAdapters.Items.Add(lvi);
				} catch (Exception e) {
					Assembler.PopupException(null, e);
					return;
				}
			}

			this.lvBrokerAdapters.Items.Clear();
			ListViewItem lviAbsentBroker = new ListViewItem() {
				Text = BrokerAdapter.NO_BROKER_ADAPTER,
				Name = BrokerAdapter.NO_BROKER_ADAPTER,
			};
			this.lvBrokerAdapters.Items.Add(lviAbsentBroker);
			foreach (BrokerAdapter brokerAdapterPrototype in BrokerAdaptersByName.Values) {
				try {
					BrokerAdapter brokerAdapterInstance = null;	// brokerAdapterPrototype;
					if (ds.BrokerAdapter != null && ds.BrokerAdapter.GetType().FullName == brokerAdapterPrototype.GetType().FullName) {
						brokerAdapterInstance = ds.BrokerAdapter;
					}
					// I still want to get a new instance, so if user choses it, I'll Initialize() it and put into serialize-able DataSource
					if (brokerAdapterInstance == null) {
						brokerAdapterInstance = (BrokerAdapter)Activator.CreateInstance(brokerAdapterPrototype.GetType());
					}

					if (brokerAdapterInstance.EditorInstanceInitialized == false) {
						try {
							brokerAdapterInstance.BrokerEditorInitialize(this);
						} catch (Exception e) {
							string msg = "can't initialize brokerAdapterInstance[" + brokerAdapterInstance + "]";
							Assembler.PopupException(msg, e);
							return;
						}
					}

					//if (this.DataSource == null) return;	//changing market for ASCII DataProvider
					//this.DataSource.DataSourceManager.DataSourceSaveTreeviewRefill(this.DataSource);


					ListViewItem lvi = new ListViewItem() {
						Text = brokerAdapterInstance.Name,
						Name = brokerAdapterInstance.GetType().Name,
						Tag = brokerAdapterInstance
					};
					if (brokerAdapterInstance.Icon != null) {
						this.imglBrokerAdapters.Images.Add(brokerAdapterInstance.Icon);
						lvi.ImageIndex = this.imglBrokerAdapters.Images.Count - 1;
					}
					this.lvBrokerAdapters.Items.Add(lvi);
				} catch (Exception e) {
					Assembler.PopupException(null, e);
					return;
				}
			}
		}
		void highlightStreamingByName(string streamingName) {
			int streamingIndex = this.lvStreamingAdapters.Items.IndexOfKey(streamingName);
			if (streamingIndex < 0) {
				string msg = "streamingName[" + streamingName + "] not found in this.lvStreamingAdapters";
				Assembler.PopupException(msg);
				return;
			}
			this.lvStreamingAdapters.Items[streamingIndex].Selected = true;
			lvStreamingAdapters_SelectedIndexChanged(null, null);
		}
		void highlightBrokerByName(string brokerName) {
			int brokerIndex = this.lvBrokerAdapters.Items.IndexOfKey(brokerName);
			if (brokerIndex < 0) {
				string msg = "brokerName[" + brokerName + "] not found in this.lvBrokerAdapters";
				Assembler.PopupException(msg);
				return;
			}
			this.lvBrokerAdapters.Items[brokerIndex].Selected = true;
			lvBrokerAdapters_SelectedIndexChanged(null, null);
		}
		public void ApplyEditorsToDataSourceAndClose() {
			if (this.txtDataSourceName.Text == "") {
				string msg = "Please provide Name for this new DataSet";
				Assembler.PopupException(msg);
				this.txtDataSourceName.Focus();
				return;
			}
//			DataSource foundSameName = this.assemblerInstance.RepositoryJsonDataSource.DataSourceFind(this.txtDataSourceName.Text);
//			if (foundSameName != null) {
//				string msg = "DataSource[" + this.txtDataSourceName.Text + "] existed before; Overwrite?";
//				DialogResult dialogResult = MessageBox.Show(msg, "DataSource [" + this.txtDataSourceName.Text + "] already exists", MessageBoxButtons.YesNoCancel);
//				switch (dialogResult) {
//					case System.Windows.Forms.DialogResult.Cancel:
//					case DialogResult.No:
//						this.txtDataSourceName.SelectAll();
//						this.txtDataSourceName.Focus();
//						return;
//					case DialogResult.Yes:
//						this.assemblerInstance.RepositoryJsonDataSource.SerializeSingle(ds);
//						base.DialogResult = System.Windows.Forms.DialogResult.OK;
//						return;
//				}
//			}

			ds.Name = this.txtDataSourceName.Text;
			ds.Symbols = SymbolParser.ParseSymbols(this.txtSymbols.Text);
			if (ds.StreamingAdapter != null) ds.StreamingAdapter.EditorInstance.PushEditedSettingsToStreamingAdapter();
			if (ds.BrokerAdapter != null) ds.BrokerAdapter.EditorInstance.PushEditedSettingsToBrokerAdapter();
			// for DataSource, nothing changed, but providers were assigned by user clicks, so DS will Initialize() each
			ds.Initialize(this.assemblerInstance.RepositoryJsonDataSource.AbsPath, this.assemblerInstance.OrderProcessor);
			this.assemblerInstance.RepositoryJsonDataSource.SerializeSingle(ds);
			//LOOKS_LIKE_STARTS_BACKTESTING_BEFORE_DSEDITOR_CLOSED
			//USER_HAS_X_TO_CLOSE_THIS_WINDOW this.btnCancel_Click(this, null);
			this.ds.RaiseDataSourceEditedChartsDisplayedShouldRunBacktestAgain();
		}
	}
}
