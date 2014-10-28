namespace NDde.Test
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using NDde;

    internal sealed class EventListener
    {
        private System.Threading.ManualResetEvent _Received = new System.Threading.ManualResetEvent(false);
        private List<DdeEventArgs>                _Events   = new List<DdeEventArgs>();

        public List<DdeEventArgs> Events
        {
            get { return _Events; }
        }

        public System.Threading.WaitHandle Received
        {
            get { return _Received; }
        }

        public void OnEvent(object sender, DdeEventArgs args)
        {
            _Events.Add(args);
            _Received.Set();
        }

    } // class

} // namespace