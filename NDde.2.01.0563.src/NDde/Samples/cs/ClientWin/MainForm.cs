using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using NDde.Client;

namespace ClientWin
{
    public partial class MainForm : Form
    {
        private DdeClient client;

        public MainForm()
        {
            InitializeComponent();

            client = new DdeClient("myapp", "myservice", this);
            client.Advise += client_Advise;
            client.Disconnected += client_Disconnected;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                // Connect to the server.  It must be running or an exception will be thrown.
                client.Connect();

                // Advise Loop
                client.StartAdvise("myitem", 1, true, 60000);
            }
            catch (Exception ex)
            {
                displayTextBox.Text = "MainForm_Load: " + ex.Message;
            }
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            displayTextBox.Left = this.DisplayRectangle.Left;
            displayTextBox.Width = this.DisplayRectangle.Width;
            displayTextBox.Top = this.DisplayRectangle.Top;
            displayTextBox.Height = this.DisplayRectangle.Height;
        }

        private void client_Advise(object sender, DdeAdviseEventArgs args)
        {
            displayTextBox.Text = "OnAdvise: " + args.Text;
        }

        private void client_Disconnected(object sender, DdeDisconnectedEventArgs args)
        {
            displayTextBox.Text = 
                "OnDisconnected: " +
                "IsServerInitiated=" + args.IsServerInitiated.ToString() + " " +
                "IsDisposed=" + args.IsDisposed.ToString();
        }

    } // class

} // namespace