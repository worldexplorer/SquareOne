namespace NDde.Test
{
    using System;
    using System.Collections;
    using System.Timers;
    using NDde;
    using NDde.Advanced;
    using NDde.Server;

    internal class TestServer : TracingServer
    {
        private Timer       _Timer        = new Timer();
        private string      _Command      = "";
        private IDictionary _Data         = new Hashtable();
        private IDictionary _Conversation = new Hashtable();

        public TestServer(string service)
            : base(service)
        {
            _Timer.Elapsed += new ElapsedEventHandler(this.OnTimerElapsed);
            _Timer.Interval = 1000;
            _Timer.SynchronizingObject = base.Context;
        }

        public TestServer(string service, DdeContext context)
            : base(service, context)
        {
            _Timer.Elapsed += new ElapsedEventHandler(this.OnTimerElapsed);
            _Timer.Interval = 1000;
            _Timer.SynchronizingObject = base.Context;
        }

        public double Interval
        {
            get { return _Timer.Interval; }
        }

        public string Command
        {
            get { return _Command; }
        }

        public byte[] GetData(string topic, string item, int format)
        {
            string key = topic + ":" + item + ":" + format.ToString();
            return (byte[])_Data[key];
        }

        public void SetData(string topic, string item, int format, byte[] data)
        {
            string key = topic + ":" + item + ":" + format.ToString();
            _Data[key] = data;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _Timer.Dispose();
            }
            base.Dispose(true);
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs args)
        {
            foreach (DdeConversation c in _Conversation.Values)
            {
                if (c.IsPaused)
                {
                    Resume(c);
                }
            }

            foreach (DdeConversation c in _Conversation.Values)
            {
                if (c.IsPaused)
                {
                    return;
                }
            }

            _Timer.Stop();
        }

        protected override bool OnBeforeConnect(string topic)
        {
            base.OnBeforeConnect(topic);
            return true;
        }

        protected override void OnAfterConnect(DdeConversation conversation)
        {
            base.OnAfterConnect(conversation);
            _Conversation.Add(conversation.Handle, conversation);
        }

        protected override void OnDisconnect(DdeConversation conversation)
        {
            base.OnDisconnect(conversation);
            _Conversation.Remove(conversation.Handle);
        }

        protected override bool OnStartAdvise(DdeConversation conversation, string item, int format)
        {
            base.OnStartAdvise(conversation, item, format);
            return true;
        }

        protected override void OnStopAdvise(DdeConversation conversation, string item)
        {
            base.OnStopAdvise(conversation, item);
        }

        protected override ExecuteResult OnExecute(DdeConversation conversation, string command)
        {
            base.OnExecute(conversation, command);
            _Command = command;
            switch (command)
            {
                case "#NotProcessed":
                    {
                        return ExecuteResult.NotProcessed;
                    }
                case "#PauseConversation":
                    {
                        if ((string)conversation.Tag == command)
                        {
                            conversation.Tag = null;
                            return ExecuteResult.Processed;
                        }
                        conversation.Tag = command;
                        if (!_Timer.Enabled) _Timer.Start();
                        return ExecuteResult.PauseConversation;
                    }
                case "#Processed":
                    {
                        return ExecuteResult.Processed;
                    }
                case "#TooBusy":
                    {
                        return ExecuteResult.TooBusy;
                    }
            }
            return ExecuteResult.Processed;
        }

        protected override PokeResult OnPoke(DdeConversation conversation, string item, byte[] data, int format)
        {
            base.OnPoke(conversation, item, data, format);
            string key = conversation.Topic + ":" + item + ":" + format.ToString();
            _Data[key] = data;
            switch (item)
            {
                case "#NotProcessed":
                    {
                        return PokeResult.NotProcessed;
                    }
                case "#PauseConversation":
                    {
                        if ((string)conversation.Tag == item)
                        {
                            conversation.Tag = null;
                            return PokeResult.Processed;
                        }
                        conversation.Tag = item;
                        if (!_Timer.Enabled) _Timer.Start();
                        return PokeResult.PauseConversation;
                    }
                case "#Processed":
                    {
                        return PokeResult.Processed;
                    }
                case "#TooBusy":
                    {
                        return PokeResult.TooBusy;
                    }
            }
            return PokeResult.Processed;
        }

        protected override RequestResult OnRequest(DdeConversation conversation, string item, int format)
        {
            base.OnRequest(conversation, item, format);
            string key = conversation.Topic + ":" + item + ":" + format.ToString();
            if (_Data.Contains(key))
            {
                return new RequestResult((byte[])_Data[key]);
            }
            return RequestResult.NotProcessed;
        }

        protected override byte[] OnAdvise(string topic, string item, int format)
        {
            base.OnAdvise(topic, item, format);
            string key = topic + ":" + item + ":" + format.ToString();
            if (_Data.Contains(key))
            {
                return (byte[])_Data[key];
            }
            return null;
        }

    } // class

} // namespace