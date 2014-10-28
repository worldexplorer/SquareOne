namespace NDde.Test
{
    using System;
    using NDde;
    using NDde.Advanced;
    using NDde.Server;

    internal class TracingServer : DdeServer
    {
        public TracingServer(string service)
            : base(service)
        {
        }

        public TracingServer(string service, DdeContext context)
            : base(service, context)
        {
        }

        protected override bool OnBeforeConnect(string topic)
        {
            Console.WriteLine("OnBeforeConnect:".PadRight(16)
                + " Service='" + base.Service + "'"
                + " Topic='" + topic + "'");

            return base.OnBeforeConnect(topic);
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

            return base.OnStartAdvise(conversation, item, format);
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

            return base.OnExecute(conversation, command);
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

            return base.OnPoke(conversation, item, data, format);
        }

        protected override RequestResult OnRequest(DdeConversation conversation, string item, int format)
        {
            Console.WriteLine("OnRequest:".PadRight(16)
                + " Service='" + conversation.Service + "'"
                + " Topic='" + conversation.Topic + "'"
                + " Handle=" + conversation.Handle.ToString()
                + " Item='" + item + "'"
                + " Format=" + format.ToString());

            return base.OnRequest(conversation, item, format);
        }

        protected override byte[] OnAdvise(string topic, string item, int format)
        {
            Console.WriteLine("OnAdvise:".PadRight(16)
                + " Service='" + this.Service + "'"
                + " Topic='" + topic + "'"
                + " Item='" + item + "'"
                + " Format=" + format.ToString());

            return base.OnAdvise(topic, item, format);
        }

    } // class

} // namespace