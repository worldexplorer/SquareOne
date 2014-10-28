namespace NDde.Test
{
    using System;
    using System.Collections;
    using System.Text;
    using NDde;
    using NDde.Advanced;
    using NDde.Client;
    using NDde.Server;
    using NUnit.Framework;

    [TestFixture]
    public sealed class Test_DdeClient
    {
        private const string ServiceName = "myservice";
        private const string TopicName = "mytopic";
        private const string ItemName = "myitem";
        private const string CommandText = "mycommand";
        private const string TestData = "Hello World";
        private const int Timeout = 1000;

        [Test]
        public void Test_Ctor_Overload_1()
        {
            DdeClient client = new DdeClient(ServiceName, TopicName);
        }

        [Test]
        public void Test_Ctor_Overload_2()
        {
            using (DdeContext context = new DdeContext())
            {
                DdeClient client = new DdeClient(ServiceName, TopicName, context);
            }
        }

        [Test]
        public void Test_Dispose()
        {
            using (DdeClient client = new DdeClient(ServiceName, TopicName))
            {
            }
        }

        [Test]
        public void Test_Service()
        {
            using (DdeClient client = new DdeClient(ServiceName, TopicName)) 
            {
                Assert.AreEqual(ServiceName, client.Service);
            }
        }

        [Test]
        public void Test_Topic()
        {
            using (DdeClient client = new DdeClient(ServiceName, TopicName)) 
            {
                Assert.AreEqual(TopicName, client.Topic);
            }
        }

        [Test]
        public void Test_Connect()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                }
            }
        }

        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void Test_Connect_After_Dispose()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Dispose();
                    client.Connect();
                }
            }
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Test_Connect_After_Connect()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    client.Connect();
                }
            }
        }

        [Test]
        public void Test_Disconnect()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    client.Disconnect();
                }
            }
        }

        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void Test_Disconnect_After_Dispose()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    client.Dispose();
                    client.Disconnect();
                }
            }
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Test_Disconnect_Before_Connect()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Disconnect();
                }
            }
        }


        [Test]
        public void Test_Handle_Variation_1()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    Assert.AreEqual(IntPtr.Zero, client.Handle);
                }
            }
        }

        [Test]
        public void Test_Handle_Variation_2()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    Assert.AreNotEqual(IntPtr.Zero, client.Handle);
                }
            }
        }

        [Test]
        public void Test_Handle_Variation_3()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    client.Disconnect();
                    Assert.AreEqual(IntPtr.Zero, client.Handle);
                }
            }
        }

        [Test]
        public void Test_IsConnected_Variation_1()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    Assert.IsFalse(client.IsConnected);
                }
            }
        }

        [Test]
        public void Test_IsConnected_Variation_2()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    Assert.IsTrue(client.IsConnected);
                }
            }
        }

        [Test]
        public void Test_IsConnected_Variation_3()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    client.Disconnect();
                    Assert.IsFalse(client.IsConnected);
                }
            }
        }

        [Test]
        public void Test_IsConnected_Variation_4()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    EventListener listener = new EventListener();
                    client.Disconnected += listener.OnEvent;
                    client.Connect();
                    server.Disconnect();
                    Assert.IsTrue(listener.Received.WaitOne(Timeout, false));
                    Assert.IsFalse(client.IsConnected);
                }
            }
        }

        [Test]
        public void Test_Pause()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    client.Pause();
                    IAsyncResult ar = client.BeginExecute(CommandText, null, null);
                    Assert.IsFalse(ar.AsyncWaitHandle.WaitOne(Timeout, false));
                }
            }
        }

        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void Test_Pause_After_Dispose()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    client.Dispose();
                    client.Pause();
                }
            }
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Test_Pause_After_Pause()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    client.Pause();
                    client.Pause();
                }
            }
        }

        [Test]
        public void Test_Resume()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    client.Pause();
                    IAsyncResult ar = client.BeginExecute(CommandText, null, null);
                    Assert.IsFalse(ar.AsyncWaitHandle.WaitOne(Timeout, false));
                    client.Resume();
                    Assert.IsTrue(ar.AsyncWaitHandle.WaitOne(Timeout, false));
                }
            }
        }

        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void Test_Resume_After_Dispose()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    client.Pause();
                    client.Dispose();
                    client.Resume();
                }
            }
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Test_Resume_Before_Pause()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    client.Resume();
                }
            }
        }

        [Test]
        public void Test_Abandon()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    client.Pause();
                    IAsyncResult ar = client.BeginExecute(CommandText, null, null);
                    Assert.IsFalse(ar.AsyncWaitHandle.WaitOne(Timeout, false));
                    client.Abandon(ar);
                    client.Resume();
                    Assert.IsFalse(ar.AsyncWaitHandle.WaitOne(Timeout, false));
                }
            }
        }

        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void Test_Abandon_After_Dispose()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    client.Pause();
                    IAsyncResult ar = client.BeginExecute(CommandText, null, null);
                    client.Dispose();
                    client.Abandon(ar);
                }
            }
        }

        [Test]
        public void Test_IsPaused_Variation_1()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    Assert.IsFalse(client.IsPaused);
                }
            }
        }

        [Test]
        public void Test_IsPaused_Variation_2()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    client.Pause();
                    Assert.IsTrue(client.IsPaused);
                }
            }
        }

        [Test]
        public void Test_IsPaused_Variation_3()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    client.Pause();
                    client.Resume();
                    Assert.IsFalse(client.IsPaused);
                }
            }
        }
        
        [Test]
        public void Test_Poke()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    client.Poke(ItemName, Encoding.ASCII.GetBytes(TestData), 1, Timeout);
                    Assert.AreEqual(TestData, Encoding.ASCII.GetString(server.GetData(TopicName, ItemName, 1)));
                }
            }
        }

        [Test]
        public void Test_TryPoke_Variation_1()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    int result = client.TryPoke(ItemName, Encoding.ASCII.GetBytes(TestData), 1, Timeout);
                    Assert.AreNotEqual(0, result);
                }
            }
        }

        [Test]
        public void Test_TryPoke_Variation_2()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    int result = client.TryPoke(ItemName, Encoding.ASCII.GetBytes(TestData), 1, Timeout);
                    Assert.AreEqual(0, result);
                    Assert.AreEqual(TestData, Encoding.ASCII.GetString(server.GetData(TopicName, ItemName, 1)));
                }
            }
        }

        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void Test_Poke_After_Dispose()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    client.Dispose();
                    client.Poke(ItemName, Encoding.ASCII.GetBytes(TestData), 1, Timeout);
                }
            }
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Test_Poke_Before_Connect()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Poke(ItemName, Encoding.ASCII.GetBytes(TestData), 1, Timeout);
                }
            }
        }

        [Test]
        public void Test_BeginPoke()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    IAsyncResult ar = client.BeginPoke(ItemName, Encoding.ASCII.GetBytes(TestData), 1, null, null);
                    Assert.IsTrue(ar.AsyncWaitHandle.WaitOne(Timeout, false));
                }
            }
        }

        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void Test_BeginPoke_After_Dispose()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    client.Dispose();
                    IAsyncResult ar = client.BeginPoke(ItemName, Encoding.ASCII.GetBytes(TestData), 1, null, null);
                }
            }
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Test_BeginPoke_Before_Connect()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    IAsyncResult ar = client.BeginPoke(ItemName, Encoding.ASCII.GetBytes(TestData), 1, null, null);
                }
            }
        }

        [Test]
        public void Test_EndPoke()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    IAsyncResult ar = client.BeginPoke(ItemName, Encoding.ASCII.GetBytes(TestData), 1, null, null);
                    Assert.IsTrue(ar.AsyncWaitHandle.WaitOne(Timeout, false));
                    client.EndPoke(ar);
                    Assert.AreEqual(TestData, Encoding.ASCII.GetString(server.GetData(TopicName, ItemName, 1)));
                }
            }
        }

        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void Test_EndPoke_After_Dispose()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    IAsyncResult ar = client.BeginPoke(ItemName, Encoding.ASCII.GetBytes(TestData), 1, null, null);
                    Assert.IsTrue(ar.AsyncWaitHandle.WaitOne(Timeout, false));
                    client.Dispose();
                    client.EndPoke(ar);
                }
            }
        }

        [Test]
        public void Test_Request_Overload_1()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                server.SetData(TopicName, ItemName, 1, Encoding.ASCII.GetBytes(TestData));
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    byte[] data = client.Request(ItemName, 1, Timeout);
                    Assert.AreEqual(TestData, Encoding.ASCII.GetString(data));
                }
            }
        }

        [Test]
        public void Test_Request_Overload_2()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                server.SetData(TopicName, ItemName, 1, Encoding.ASCII.GetBytes(TestData));
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    string data = client.Request(ItemName, Timeout);
                    Assert.AreEqual(TestData, data);
                }
            }
        }

        [Test]
        public void Test_TryRequest_Variation_1()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                server.SetData(TopicName, ItemName, 1, Encoding.ASCII.GetBytes(TestData));
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    byte[] data;
                    int result = client.TryRequest(ItemName, 1, Timeout, out data);
                    Assert.AreNotEqual(0, result);
                }
            }
        }

        [Test]
        public void Test_TryRequest_Variation_2()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                server.SetData(TopicName, ItemName, 1, Encoding.ASCII.GetBytes(TestData));
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    byte[] data;
                    int result = client.TryRequest(ItemName, 1, Timeout, out data);
                    Assert.AreEqual(0, result);
                    Assert.AreEqual(TestData, Encoding.ASCII.GetString(data));
                }
            }
        }

        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void Test_Request_After_Dispose()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                server.SetData(TopicName, ItemName, 1, Encoding.ASCII.GetBytes(TestData));
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    client.Dispose();
                    byte[] data = client.Request(ItemName, 1, Timeout);
                }
            }
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Test_Request_Before_Connect()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                server.SetData(TopicName, ItemName, 1, Encoding.ASCII.GetBytes(TestData));
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    byte[] data = client.Request(ItemName, 1, Timeout);
                }
            }
        }

        [Test]
        public void Test_BeginRequest()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                server.SetData(TopicName, ItemName, 1, Encoding.ASCII.GetBytes(TestData));
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    IAsyncResult ar = client.BeginRequest(ItemName, 1, null, null);
                    Assert.IsTrue(ar.AsyncWaitHandle.WaitOne(Timeout, false));
                }
            }
        }

        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void Test_BeginRequest_After_Dispose()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                server.SetData(TopicName, ItemName, 1, Encoding.ASCII.GetBytes(TestData));
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    client.Dispose();
                    IAsyncResult ar = client.BeginRequest(ItemName, 1, null, null);
                }
            }
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Test_BeginRequest_Before_Connect()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                server.SetData(TopicName, ItemName, 1, Encoding.ASCII.GetBytes(TestData));
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    IAsyncResult ar = client.BeginRequest(ItemName, 1, null, null);
                }
            }
        }

        [Test]
        public void Test_EndRequest()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                server.SetData(TopicName, ItemName, 1, Encoding.ASCII.GetBytes(TestData));
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    IAsyncResult ar = client.BeginRequest(ItemName, 1, null, null);
                    Assert.IsTrue(ar.AsyncWaitHandle.WaitOne(Timeout, false));
                    byte[] data = client.EndRequest(ar);
                    Assert.AreEqual(TestData, Encoding.ASCII.GetString(data));
                }
            }
        }

        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void Test_EndRequest_After_Dispose()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                server.SetData(TopicName, ItemName, 1, Encoding.ASCII.GetBytes(TestData));
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    IAsyncResult ar = client.BeginRequest(ItemName, 1, null, null);
                    Assert.IsTrue(ar.AsyncWaitHandle.WaitOne(Timeout, false));
                    client.Dispose();
                    byte[] data = client.EndRequest(ar);
                }
            }
        }

        [Test]
        public void Test_Execute()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    client.Execute(TestData, Timeout);
                    Assert.AreEqual(TestData, server.Command);
                }
            }
        }

        [Test]
        public void Test_TryExecute_Variation_1()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    int result = client.TryExecute(TestData, Timeout);
                    Assert.AreNotEqual(0, result);
                }
            }
        }

        [Test]
        public void Test_TryExecute_Variation_2()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    int result = client.TryExecute(TestData, Timeout);
                    Assert.AreEqual(0, result);
                    Assert.AreEqual(TestData, server.Command);
                }
            }
        }

        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void Test_Execute_After_Dispose()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    client.Dispose();
                    client.Execute(TestData, Timeout);
                }
            }
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Test_Execute_Before_Connect()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Execute(TestData, Timeout);
                }
            }
        }

        [Test]
        public void Test_BeginExecute()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    IAsyncResult ar = client.BeginExecute(TestData, null, null);
                    Assert.IsTrue(ar.AsyncWaitHandle.WaitOne(Timeout, false));
                }
            }
        }

        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void Test_BeginExecute_After_Dispose()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    client.Dispose();
                    IAsyncResult ar = client.BeginExecute(TestData, null, null);
                }
            }
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Test_BeginExecute_Before_Connect()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    IAsyncResult ar = client.BeginExecute(TestData, null, null);
                }
            }
        }

        [Test]
        public void Test_EndExecute()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    IAsyncResult ar = client.BeginExecute(TestData, null, null);
                    Assert.IsTrue(ar.AsyncWaitHandle.WaitOne(Timeout, false));
                    client.EndExecute(ar);
                    Assert.AreEqual(TestData, server.Command);
                }
            }
        }

        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void Test_EndExecute_After_Dispose()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    IAsyncResult ar = client.BeginExecute(TestData, null, null);
                    Assert.IsTrue(ar.AsyncWaitHandle.WaitOne(Timeout, false));
                    client.Dispose();
                    client.EndExecute(ar);
                }
            }
        }

        [Test]
        public void Test_Disconnected_Variation_1()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    EventListener listener = new EventListener();
                    client.Disconnected += listener.OnEvent;
                    client.Connect();
                    client.Disconnect();
                    Assert.IsTrue(listener.Received.WaitOne(Timeout, false));
                    DdeDisconnectedEventArgs args = (DdeDisconnectedEventArgs)listener.Events[0];
                    Assert.IsFalse(args.IsServerInitiated);
                    Assert.IsFalse(args.IsDisposed);
                }
            }
        }

        [Test]
        public void Test_Disconnected_Variation_2()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    EventListener listener = new EventListener();
                    client.Disconnected += listener.OnEvent;
                    client.Connect();
                    server.Disconnect();
                    Assert.IsTrue(listener.Received.WaitOne(Timeout, false));
                    DdeDisconnectedEventArgs args = (DdeDisconnectedEventArgs)listener.Events[0];
                    Assert.IsTrue(args.IsServerInitiated);
                    Assert.IsFalse(args.IsDisposed);
                }
            }
        }

        [Test]
        public void Test_Disconnected_Variation_3()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    EventListener listener = new EventListener();
                    client.Disconnected += listener.OnEvent;
                    client.Connect();
                    client.Dispose();
                    Assert.IsTrue(listener.Received.WaitOne(Timeout, false));
                    DdeDisconnectedEventArgs args = (DdeDisconnectedEventArgs)listener.Events[0];
                    Assert.IsFalse(args.IsServerInitiated);
                    Assert.IsTrue(args.IsDisposed);
                }
            }
        }

        [Test]
        public void Test_StartAdvise_Variation_1()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                server.SetData(TopicName, ItemName, 1, Encoding.ASCII.GetBytes(TestData));
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    EventListener listener = new EventListener();
                    client.Advise += listener.OnEvent;
                    client.Connect();
                    client.StartAdvise(ItemName, 1, true, Timeout);
                    server.Advise(TopicName, ItemName);
                    Assert.IsTrue(listener.Received.WaitOne(Timeout, false));
                    DdeAdviseEventArgs args = (DdeAdviseEventArgs)listener.Events[0];
                    Assert.AreEqual(ItemName, args.Item);
                    Assert.AreEqual(1, args.Format);
                    Assert.AreEqual(TestData, Encoding.ASCII.GetString(args.Data));
                    Assert.AreEqual(TestData, args.Text);
                }
            }
        }

        [Test]
        public void Test_StartAdvise_Variation_2()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                server.SetData(TopicName, ItemName, 1, Encoding.ASCII.GetBytes(TestData));
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    EventListener listener = new EventListener();
                    client.Advise += listener.OnEvent;
                    client.Connect();
                    client.StartAdvise(ItemName, 1, false, Timeout);
                    server.Advise(TopicName, ItemName);
                    Assert.IsTrue(listener.Received.WaitOne(Timeout, false));
                    DdeAdviseEventArgs args = (DdeAdviseEventArgs)listener.Events[0];
                    Assert.AreEqual(ItemName, args.Item);
                    Assert.AreEqual(1, args.Format);
                    Assert.IsNull(args.Data);
                    Assert.IsNull(args.Text);
                }
            }
        }

        [Test]
        public void Test_StartAdvise_Variation_3()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                server.SetData(TopicName, ItemName, 1, Encoding.ASCII.GetBytes(TestData));
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    EventListener listener = new EventListener();
                    client.Advise += listener.OnEvent;
                    client.Connect();
                    client.StartAdvise(ItemName, 1, true, true, Timeout, "MyStateObject");
                    server.Advise(TopicName, ItemName);
                    Assert.IsTrue(listener.Received.WaitOne(Timeout, false));
                    DdeAdviseEventArgs args = (DdeAdviseEventArgs)listener.Events[0];
                    Assert.AreEqual(ItemName, args.Item);
                    Assert.AreEqual(1, args.Format);
                    Assert.AreEqual("MyStateObject", args.State);
                    Assert.AreEqual(TestData, Encoding.ASCII.GetString(args.Data));
                    Assert.AreEqual(TestData, args.Text);
                }
            }
        }

        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void Test_StartAdvise_After_Dispose()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                server.SetData(TopicName, ItemName, 1, Encoding.ASCII.GetBytes(TestData));
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    client.Dispose();
                    client.StartAdvise(ItemName, 1, false, Timeout);
                }
            }
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Test_StartAdvise_Before_Connect()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                server.SetData(TopicName, ItemName, 1, Encoding.ASCII.GetBytes(TestData));
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.StartAdvise(ItemName, 1, false, Timeout);
                }
            }
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Test_StartAdvise_After_StartAdvise()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                server.SetData(TopicName, ItemName, 1, Encoding.ASCII.GetBytes(TestData));
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.StartAdvise(ItemName, 1, false, Timeout);
                    client.StartAdvise(ItemName, 1, false, Timeout);
                }
            }
        }

        [Test]
        public void Test_BeginStartAdvise_Variation_1()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                server.SetData(TopicName, ItemName, 1, Encoding.ASCII.GetBytes(TestData));
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    EventListener listener = new EventListener();
                    client.Advise += listener.OnEvent;
                    client.Connect();
                    IAsyncResult ar = client.BeginStartAdvise(ItemName, 1, true, null, null);
                    Assert.IsTrue(ar.AsyncWaitHandle.WaitOne(Timeout, false));
                    server.Advise(TopicName, ItemName);
                    Assert.IsTrue(listener.Received.WaitOne(Timeout, false));
                    DdeAdviseEventArgs args = (DdeAdviseEventArgs)listener.Events[0];
                    Assert.AreEqual(ItemName, args.Item);
                    Assert.AreEqual(1, args.Format);
                    Assert.AreEqual(TestData, Encoding.ASCII.GetString(args.Data));
                }
            }
        }

        [Test]
        public void Test_BeginStartAdvise_Variation_2()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                server.SetData(TopicName, ItemName, 1, Encoding.ASCII.GetBytes(TestData));
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    EventListener listener = new EventListener();
                    client.Advise += listener.OnEvent;
                    client.Connect();
                    IAsyncResult ar = client.BeginStartAdvise(ItemName, 1, false, null, null);
                    Assert.IsTrue(ar.AsyncWaitHandle.WaitOne(Timeout, false));
                    server.Advise(TopicName, ItemName);
                    Assert.IsTrue(listener.Received.WaitOne(Timeout, false));
                    DdeAdviseEventArgs args = (DdeAdviseEventArgs)listener.Events[0];
                    Assert.AreEqual(ItemName, args.Item);
                    Assert.AreEqual(1, args.Format);
                    Assert.IsNull(args.Data);
                }
            }
        }

        [Test]
        public void Test_BeginStartAdvise_Variation_3()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                server.SetData(TopicName, ItemName, 1, Encoding.ASCII.GetBytes(TestData));
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    EventListener listener = new EventListener();
                    client.Advise += listener.OnEvent;
                    client.Connect();
                    IAsyncResult ar = client.BeginStartAdvise(ItemName, 1, true, true, null, null, "MyStateObject");
                    Assert.IsTrue(ar.AsyncWaitHandle.WaitOne(Timeout, false));
                    server.Advise(TopicName, ItemName);
                    Assert.IsTrue(listener.Received.WaitOne(Timeout, false));
                    DdeAdviseEventArgs args = (DdeAdviseEventArgs)listener.Events[0];
                    Assert.AreEqual(ItemName, args.Item);
                    Assert.AreEqual(1, args.Format);
                    Assert.AreEqual("MyStateObject", args.State);
                    Assert.AreEqual(TestData, Encoding.ASCII.GetString(args.Data));
                }
            }
        }

        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void Test_BeginStartAdvise_After_Dispose()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                server.SetData(TopicName, ItemName, 1, Encoding.ASCII.GetBytes(TestData));
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    client.Dispose();
                    IAsyncResult ar = client.BeginStartAdvise(ItemName, 1, false, null, null);
                }
            }
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Test_BeginStartAdvise_Before_Connect()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                server.SetData(TopicName, ItemName, 1, Encoding.ASCII.GetBytes(TestData));
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    IAsyncResult ar = client.BeginStartAdvise(ItemName, 1, false, null, null);
                }
            }
        }

        [Test]
        public void Test_EndStartAdvise()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                server.SetData(TopicName, ItemName, 1, Encoding.ASCII.GetBytes(TestData));
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    IAsyncResult ar = client.BeginStartAdvise(ItemName, 1, true, null, null);
                    Assert.IsTrue(ar.AsyncWaitHandle.WaitOne(Timeout, false));
                    client.EndStartAdvise(ar);
                }
            }
        }

        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void Test_EndStartAdvise_After_Dispose()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                server.SetData(TopicName, ItemName, 1, Encoding.ASCII.GetBytes(TestData));
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    IAsyncResult ar = client.BeginStartAdvise(ItemName, 1, true, null, null);
                    Assert.IsTrue(ar.AsyncWaitHandle.WaitOne(Timeout, false));
                    client.Dispose();
                    client.EndStartAdvise(ar);
                }
            }
        }

        [Test]
        public void Test_StopAdvise()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                server.SetData(TopicName, ItemName, 1, Encoding.ASCII.GetBytes(TestData));
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    client.StartAdvise(ItemName, 1, true, Timeout);
                    client.StopAdvise(ItemName, Timeout);
                }
            }
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Test_StopAdvise_Before_StartAdvise()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                server.SetData(TopicName, ItemName, 1, Encoding.ASCII.GetBytes(TestData));
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    client.StopAdvise(ItemName, Timeout);
                }
            }
        }

    } // class

} // namespace