using System;
using System.Collections.Generic;
using System.Text;
using NDde;
using NDde.Advanced.Monitor;

namespace Monitor
{
    class Program
    {
        static void Main(string[] args)
        {
            using (DdeMonitor monitor = new DdeMonitor())
            {
                monitor.Start(DdeMonitorFlags.Link | DdeMonitorFlags.Callback | DdeMonitorFlags.Conversation | DdeMonitorFlags.Message);
                monitor.LinkActivity += DdeMonitor_LinkActivity;
                monitor.CallbackActivity += DdeMonitor_CallbackActivity;
                monitor.ConversationActivity += DdeMonitor_ConversationActivity;
                monitor.MessageActivity += DdeMonitor_MessageActivity;
                Console.WriteLine("Press ENTER to quit...");
                Console.ReadLine();
            }
        }

        static void DdeMonitor_MessageActivity(object sender, DdeMessageActivityEventArgs e)
        {
            Console.WriteLine("MessageActivity: " + e.ToString());
        }

        static void DdeMonitor_CallbackActivity(object sender, DdeCallbackActivityEventArgs e)
        {
            Console.WriteLine("CallbackActivity: " + e.ToString());
        }

        static void DdeMonitor_LinkActivity(object sender, DdeLinkActivityEventArgs e)
        {
            Console.WriteLine("LinkActivity: " + e.ToString());
        }

        static void DdeMonitor_ConversationActivity(object sender, DdeConversationActivityEventArgs e)
        {
            Console.WriteLine("ConversationActivity: " + e.ToString());
        }

    } // class

} // namespace
