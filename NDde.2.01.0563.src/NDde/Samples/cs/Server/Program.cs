using System;
using System.Collections;
using System.Timers;
using NDde.Server;

namespace Server
{
    public class Server
    {
        public static void Main()
        {
            try
            {
                // Create a server that will register the service name 'myapp'.
                using (DdeServer server = new MyServer("myapp"))
                {
                    // Register the service name.
                    server.Register();

                    // Wait for the user to press ENTER before proceding.
                    Console.WriteLine("Press ENTER to quit...");
                    Console.ReadLine();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine("Press ENTER to quit...");
                Console.ReadLine();
            }
        }

        private sealed class MyServer : DdeServer
        {
            private System.Timers.Timer _Timer = new System.Timers.Timer();

            public MyServer(string service) : base(service)
            {
                // Create a timer that will be used to advise clients of new data.
                _Timer.Elapsed += this.OnTimerElapsed;
                _Timer.Interval = 1000;
                _Timer.SynchronizingObject = this.Context;
            }

            private void OnTimerElapsed(object sender, ElapsedEventArgs args)
            {
                // Advise all topic name and item name pairs.
                Advise("*", "*");
            }

            public override void Register()
            {
                base.Register();
                _Timer.Start();
            }

            public override void Unregister()
            {
                _Timer.Stop();
                base.Unregister();
            }

            protected override bool OnBeforeConnect(string topic)
            {
                Console.WriteLine("OnBeforeConnect:".PadRight(16)
                    + " Service='" + base.Service + "'"
                    + " Topic='" + topic + "'");

                return true;
            }

            protected override void OnAfterConnect(DdeConversation conversation)
            {
                Console.WriteLine("OnAfterConnect:".PadRight(16)
                    + " Service='" + conversation.Service + "'"
                    + " Topic='" + conversation.Topic + "'"
                    + " Handle=" + conversation.Handle.ToString());
            }

            protected override void OnDisconnect(DdeConversation conversation)
            {
                Console.WriteLine("OnDisconnect:".PadRight(16)
                    + " Service='" + conversation.Service + "'"
                    + " Topic='" + conversation.Topic + "'"
                    + " Handle=" + conversation.Handle.ToString());
            }

            protected override bool OnStartAdvise(DdeConversation conversation, string item, int format)
            {
                Console.WriteLine("OnStartAdvise:".PadRight(16)
                    + " Service='" + conversation.Service + "'"
                    + " Topic='" + conversation.Topic + "'"
                    + " Handle=" + conversation.Handle.ToString()
                    + " Item='" + item + "'"
                    + " Format=" + format.ToString());

                // Initiate the advisory loop only if the format is CF_TEXT.
                return format == 1;
            }

            protected override void OnStopAdvise(DdeConversation conversation, string item)
            {
                Console.WriteLine("OnStopAdvise:".PadRight(16)
                    + " Service='" + conversation.Service + "'"
                    + " Topic='" + conversation.Topic + "'"
                    + " Handle=" + conversation.Handle.ToString()
                    + " Item='" + item + "'");
            }

            protected override ExecuteResult OnExecute(DdeConversation conversation, string command)
            {
                Console.WriteLine("OnExecute:".PadRight(16)
                    + " Service='" + conversation.Service + "'"
                    + " Topic='" + conversation.Topic + "'"
                    + " Handle=" + conversation.Handle.ToString()
                    + " Command='" + command + "'");

                // Tell the client that the command was processed.
                return ExecuteResult.Processed;
            }

            protected override PokeResult OnPoke(DdeConversation conversation, string item, byte[] data, int format)
            {
                Console.WriteLine("OnPoke:".PadRight(16)
                    + " Service='" + conversation.Service + "'"
                    + " Topic='" + conversation.Topic + "'"
                    + " Handle=" + conversation.Handle.ToString()
                    + " Item='" + item + "'"
                    + " Data=" + data.Length.ToString()
                    + " Format=" + format.ToString());

                // Tell the client that the data was processed.
                return PokeResult.Processed;
            }

            protected override RequestResult OnRequest(DdeConversation conversation, string item, int format)
            {
                Console.WriteLine("OnRequest:".PadRight(16)
                    + " Service='" + conversation.Service + "'"
                    + " Topic='" + conversation.Topic + "'"
                    + " Handle=" + conversation.Handle.ToString()
                    + " Item='" + item + "'"
                    + " Format=" + format.ToString());

                // Return data to the client only if the format is CF_TEXT.
                if (format == 1)
                {
                    return new RequestResult(System.Text.Encoding.ASCII.GetBytes("Time=" + DateTime.Now.ToString() + "\0"));
                }
                return RequestResult.NotProcessed;
            }

            protected override byte[] OnAdvise(string topic, string item, int format)
            {
                Console.WriteLine("OnAdvise:".PadRight(16)
                    + " Service='" + this.Service + "'"
                    + " Topic='" + topic + "'"
                    + " Item='" + item + "'"
                    + " Format=" + format.ToString());

                // Send data to the client only if the format is CF_TEXT.
                if (format == 1)
                {
                    return System.Text.Encoding.ASCII.GetBytes("Time=" + DateTime.Now.ToString() + "\0");
                }
                return null;
            }

        } // class

    } // class

} // namespace
