using System;
using System.Text;
using NDde.Client;

namespace Client
{
    public sealed class Client
    {
        public static void Main(string[] args)
        {
            // Wait for the user to press ENTER before proceding.
            Console.WriteLine("The Server sample must be running before the client can connect.");
            Console.WriteLine("Press ENTER to continue...");
            Console.ReadLine();
            try
            {
                // Create a client that connects to 'myapp|mytopic'. 
                using (DdeClient client = new DdeClient("myapp", "mytopic"))
                {
                    // Subscribe to the Disconnected event.  This event will notify the application when a conversation has been terminated.
                    client.Disconnected += OnDisconnected;

                    // Connect to the server.  It must be running or an exception will be thrown.
                    client.Connect();

                    // Synchronous Execute Operation
                    client.Execute("mycommand", 60000);

                    // Synchronous Poke Operation
                    client.Poke("myitem", DateTime.Now.ToString(), 60000);

                    // Syncronous Request Operation
                    Console.WriteLine("Request: " + client.Request("myitem", 60000));

                    // Asynchronous Execute Operation
                    client.BeginExecute("mycommand", OnExecuteComplete, client);

                    // Asynchronous Poke Operation
                    client.BeginPoke("myitem", Encoding.ASCII.GetBytes(DateTime.Now.ToString() + "\0"), 1, OnPokeComplete, client);

                    // Asynchronous Request Operation
                    client.BeginRequest("myitem", 1, OnRequestComplete, client);

                    // Advise Loop
                    client.StartAdvise("myitem", 1, true, 60000);
                    client.Advise += OnAdvise;

                    // Wait for the user to press ENTER before proceding.
                    Console.WriteLine("Press ENTER to quit...");
                    Console.ReadLine();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.WriteLine("Press ENTER to quit...");
                Console.ReadLine();
            }
        }

        private static void OnExecuteComplete(IAsyncResult ar)
        {
            try
            {
                DdeClient client = (DdeClient)ar.AsyncState;
                client.EndExecute(ar);
                Console.WriteLine("OnExecuteComplete");
            }
            catch (Exception e)
            {
                Console.WriteLine("OnExecuteComplete: " + e.Message);
            }
        }

        private static void OnPokeComplete(IAsyncResult ar)
        {
            try
            {
                DdeClient client = (DdeClient)ar.AsyncState;
                client.EndPoke(ar);
                Console.WriteLine("OnPokeComplete");
            }
            catch (Exception e)
            {
                Console.WriteLine("OnPokeComplete: " + e.Message);
            }
        }

        private static void OnRequestComplete(IAsyncResult ar)
        {
            try
            {
                DdeClient client = (DdeClient)ar.AsyncState;
                byte[] data = client.EndRequest(ar);
                Console.WriteLine("OnRequestComplete: " + Encoding.ASCII.GetString(data));
            }
            catch (Exception e)
            {
                Console.WriteLine("OnRequestComplete: " + e.Message);
            }
        }

        private static void OnStartAdviseComplete(IAsyncResult ar)
        {
            try
            {
                DdeClient client = (DdeClient)ar.AsyncState;
                client.EndStartAdvise(ar);
                Console.WriteLine("OnStartAdviseComplete");
            }
            catch (Exception e)
            {
                Console.WriteLine("OnStartAdviseComplete: " + e.Message);
            }
        }

        private static void OnStopAdviseComplete(IAsyncResult ar)
        {
            try
            {
                DdeClient client = (DdeClient)ar.AsyncState;
                client.EndStopAdvise(ar);
                Console.WriteLine("OnStopAdviseComplete");
            }
            catch (Exception e)
            {
                Console.WriteLine("OnStopAdviseComplete: " + e.Message);
            }
        }

        private static void OnAdvise(object sender, DdeAdviseEventArgs args)
        {
            Console.WriteLine("OnAdvise: " + args.Text);
        }

        private static void OnDisconnected(object sender, DdeDisconnectedEventArgs args)
        {
            Console.WriteLine(
                "OnDisconnected: " +
                "IsServerInitiated=" + args.IsServerInitiated.ToString() + " " +
                "IsDisposed=" + args.IsDisposed.ToString());
        }

    } // class

} // namespace
