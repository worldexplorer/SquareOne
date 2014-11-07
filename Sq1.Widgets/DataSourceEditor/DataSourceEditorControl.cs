using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using Sq1.Core;
using Sq1.Core.Broker;
using Sq1.Core.DataFeed;
using Sq1.Core.DataTypes;
using Sq1.Core.Static;
using Sq1.Core.Streaming;
using Sq1.Core.Support;

namespace Sq1.Widgets.DataSourceEditor {
	public partial class DataSourceEditorControl : UserControl, IDataSourceEditor {
		DataSource ds;
		public string DataSourceName { get {
				if (this.ds == null) return "NO_DATASOURCE_LOADED_FOR_EDITING";
				return this.ds.Name;
			} }
		public Dictionary<string, StaticProvider> StaticProvidersByName { get; private set; }
		public Dictionary<string, StreamingProvider> StreamingProvidersByName { get; private set; }
		public Dictionary<string, BrokerProvider> BrokerProvidersByName { get; private set; }
		public string symbolsDefault;
		public string windowTitleDefault;
		Assembler assemblerInstance;

		public DataSourceEditorControl() {
			this.symbolsDefault = "RIZ2,RIH3";
			InitializeComponent();
		}

		public void InitializeProviders(
				Dictionary<string, StaticProvider> staticProvidersByName,
				Dictionary<string, StreamingProvider> streamingProvidersByName,
				Dictionary<string, BrokerProvider> brokerProvidersByName) {
			this.StaticProvidersByName = staticProvidersByName;
			this.StreamingProvidersByName = streamingProvidersByName;
			this.BrokerProvidersByName = brokerProvidersByName;
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
				this.Parent.Text = ds.Name;
			} else {
				this.Text = ds.Name;
			}
			if (this.ds.Name != windowTitleDefault) {
				this.txtDataSourceName.Text = this.ds.Name;
			}
			this.txtSymbols.Text = this.ds.SymbolsCSV;
			this.PopulateScaleIntervalFromDataSource();
			this.PopulateStaticStreamingBrokerListViewsFromDataSource();

			if (this.ds.StaticProvider != null) HighlightStaticByName(this.ds.StaticProvider.GetType().Name);

			if (this.ds.StreamingProvider != null) {
				HighlightStreamingByName(this.ds.StreamingProvider.GetType().Name);
			} else {
				HighlightStreamingByName(StreamingProvider.NO_STREAMING_PROVIDER);
			}

			if (this.ds.BrokerProvider != null) HighlightBrokerByName(this.ds.BrokerProvider.GetType().Name);
			else HighlightBrokerByName(BrokerProvider.NO_BROKER_PROVIDER);

			this.marketInfoEditor.Initialize(ds, this.assemblerInstance.RepositoryJsonDataSource, this.assemblerInstance.MarketInfoRepository);
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
		public void PopulateStaticStreamingBrokerListViewsFromDataSource() {
			if (base.IsDisposed) {
				string msg = "base.IsDisposed=true for DataSourceEditorForm::PopulateStaticStreamingBrokerListViewsFromDataSource()";
				Assembler.PopupException(msg);
				return;
			}
			
			this.lvStaticProviders.Items.Clear();
			foreach (StaticProvider staticProviderPrototype in StaticProvidersByName.Values) {
				try {
					StaticProvider staticProviderEditingInstance = null;	// staticProviderPrototype;
					if (ds.StaticProvider != null && ds.StaticProvider.GetType().FullName == staticProviderPrototype.GetType().FullName) {
						staticProviderEditingInstance = ds.StaticProvider;
					}
					// I still want to get a new instance, so if user choses it, I'll Initialize() it and put into serialize-able DataSource
					if (staticProviderEditingInstance == null) {
						staticProviderEditingInstance = (StaticProvider)Activator.CreateInstance(staticProviderPrototype.GetType());
					}
					ListViewItem lvi = new ListViewItem() {
						Text = staticProviderEditingInstance.Name,
						Name = staticProviderEditingInstance.GetType().Name,
						Tag = staticProviderEditingInstance
					};
					if (staticProviderEditingInstance.Icon != null) {
						this.imglStaticProviders.Images.Add(staticProviderEditingInstance.Icon);
						lvi.ImageIndex = this.imglStaticProviders.Images.Count - 1;
					}
					this.lvStaticProviders.Items.Add(lvi);
				} catch (Exception e) {
					this.assemblerInstance.StatusReporter.PopupException(null, e);
					return;
				}
			}

			this.lvStreamingProviders.Items.Clear();
			ListViewItem lviAbsentStreaming = new ListViewItem() {
				Text = StreamingProvider.NO_STREAMING_PROVIDER,
				Name = StreamingProvider.NO_STREAMING_PROVIDER,
			};
			this.lvStreamingProviders.Items.Add(lviAbsentStreaming);
			foreach (StreamingProvider streamingProviderPrototype in StreamingProvidersByName.Values) {
				try {
					StreamingProvider streamingProviderInstance = null;	// streamingProviderPrototype;
					if (ds.StreamingProvider != null && ds.StreamingProvider.GetType().FullName == streamingProviderPrototype.GetType().FullName) {
						streamingProviderInstance = ds.StreamingProvider;
					}
					// I still want to get a new instance, so if user choses it, I'll Initialize() it and put into serialize-able DataSource
					if (streamingProviderInstance == null) {
						streamingProviderInstance = (StreamingProvider)Activator.CreateInstance(streamingProviderPrototype.GetType());
					}

					if (streamingProviderInstance.EditorInstanceInitialized == false) {
						try {
							streamingProviderInstance.StreamingEditorInitialize(this);
						} catch (Exception e) {
							string msg = "can't initialize streamingProviderInstance[" + streamingProviderInstance + "]";
							this.assemblerInstance.StatusReporter.PopupException(msg, e);
							return;
						}
					}

					ListViewItem lvi = new ListViewItem() {
						Text = streamingProviderInstance.Name,
						Name = streamingProviderInstance.GetType().Name,
						Tag = streamingProviderInstance
					};
					if (streamingProviderInstance.Icon != null) {
						this.imglStreamingProviders.Images.Add(streamingProviderInstance.Icon);
						lvi.ImageIndex = this.imglStreamingProviders.Images.Count - 1;
					}
					this.lvStreamingProviders.Items.Add(lvi);
				} catch (Exception e) {
					this.assemblerInstance.StatusReporter.PopupException(null, e);
					return;
				}
			}

			this.lvBrokerProviders.Items.Clear();
			ListViewItem lviAbsentBroker = new ListViewItem() {
				Text = BrokerProvider.NO_BROKER_PROVIDER,
				Name = BrokerProvider.NO_BROKER_PROVIDER,
			};
			this.lvBrokerProviders.Items.Add(lviAbsentBroker);
			foreach (BrokerProvider brokerProviderPrototype in BrokerProvidersByName.Values) {
				try {
					BrokerProvider brokerProviderInstance = null;	// brokerProviderPrototype;
					if (ds.BrokerProvider != null && ds.BrokerProvider.GetType().FullName == brokerProviderPrototype.GetType().FullName) {
						brokerProviderInstance = ds.BrokerProvider;
					}
					// I still want to get a new instance, so if user choses it, I'll Initialize() it and put into serialize-able DataSource
					if (brokerProviderInstance == null) {
						brokerProviderInstance = (BrokerProvider)Activator.CreateInstance(brokerProviderPrototype.GetType());
					}

					if (brokerProviderInstance.EditorInstanceInitialized == false) {
						try {
							brokerProviderInstance.BrokerEditorInitialize(this);
						} catch (Exception e) {
							string msg = "can't initialize brokerProviderInstance[" + brokerProviderInstance + "]";
							this.assemblerInstance.StatusReporter.PopupException(msg, e);
							return;
						}
					}

					//if (this.DataSource == null) return;	//changing market for ASCII DataProvider
					//this.DataSource.DataSourceManager.DataSourceSaveTreeviewRefill(this.DataSource);


					ListViewItem lvi = new ListViewItem() {
						Text = brokerProviderInstance.Name,
						Name = brokerProviderInstance.GetType().Name,
						Tag = brokerProviderInstance
					};
					if (brokerProviderInstance.Icon != null) {
						this.imglBrokerProviders.Images.Add(brokerProviderInstance.Icon);
						lvi.ImageIndex = this.imglBrokerProviders.Images.Count - 1;
					}
					this.lvBrokerProviders.Items.Add(lvi);
				} catch (Exception e) {
					this.assemblerInstance.StatusReporter.PopupException(null, e);
					return;
				}
			}
		}
		public void HighlightStaticByName(string staticName) {
			int staticIndex = this.lvStaticProviders.Items.IndexOfKey(staticName);
			if (staticIndex < 0) {
				string msg = "staticName[" + staticName + "] not found in this.lvStaticProviders";
				this.assemblerInstance.StatusReporter.PopupException(msg);
				return;
			}
			this.lvStaticProviders.Items[staticIndex].Selected = true;
			//lvStaticProviders_SelectedIndexChanged(null, null);
		}
		public void HighlightStreamingByName(string streamingName) {
			int streamingIndex = this.lvStreamingProviders.Items.IndexOfKey(streamingName);
			if (streamingIndex < 0) {
				string msg = "streamingName[" + streamingName + "] not found in this.lvStreamingProviders";
				this.assemblerInstance.StatusReporter.PopupException(msg);
				return;
			}
			this.lvStreamingProviders.Items[streamingIndex].Selected = true;
			lvStreamingProviders_SelectedIndexChanged(null, null);
		}
		public void HighlightBrokerByName(string brokerName) {
			int brokerIndex = this.lvBrokerProviders.Items.IndexOfKey(brokerName);
			if (brokerIndex < 0) {
				string msg = "brokerName[" + brokerName + "] not found in this.lvBrokerProviders";
				this.assemblerInstance.StatusReporter.PopupException(msg);
				return;
			}
			this.lvBrokerProviders.Items[brokerIndex].Selected = true;
			lvBrokerProviders_SelectedIndexChanged(null, null);
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
			if (ds.StreamingProvider != null) ds.StreamingProvider.EditorInstance.PushEditedSettingsToStreamingProvider();
			if (ds.BrokerProvider != null) ds.BrokerProvider.EditorInstance.PushEditedSettingsToBrokerProvider();
			// for DataSource, nothing changed, but providers were assigned by user clicks, so DS will Initialize() each
			ds.Initialize(this.assemblerInstance.RepositoryJsonDataSource.AbsPath, this.assemblerInstance.OrderProcessor, this.assemblerInstance.StatusReporter);
			this.assemblerInstance.RepositoryJsonDataSource.SerializeSingle(ds);
			// JITTERING_HERE_LOOKS_LIKE_STARTS_BACKTESTING_BEFORE_DSEDITOR_CLOSED
			this.btnCancel_Click(this, null);
			this.ds.RaiseDataSourceEditedChartsDisplayedShouldRunBacktestAgain();
		}
	}
}
