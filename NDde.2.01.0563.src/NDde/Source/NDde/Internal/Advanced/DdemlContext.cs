#region Copyright (c) 2005 by Brian Gideon (briangideon@yahoo.com)
/* Shared Source License for NDde
 *
 * This license governs use of the accompanying software ('Software'), and your use of the Software constitutes acceptance of this license.
 *
 * You may use the Software for any commercial or noncommercial purpose, including distributing derivative works.
 * 
 * In return, we simply require that you agree:
 *  1. Not to remove any copyright or other notices from the Software. 
 *  2. That if you distribute the Software in source code form you do so only under this license (i.e. you must include a complete copy of this
 *     license with your distribution), and if you distribute the Software solely in object form you only do so under a license that complies with
 *     this license.
 *  3. That the Software comes "as is", with no warranties.  None whatsoever.  This means no express, implied or statutory warranty, including
 *     without limitation, warranties of merchantability or fitness for a particular purpose or any warranty of title or non-infringement.  Also,
 *     you must pass this disclaimer on whenever you distribute the Software or derivative works.
 *  4. That no contributor to the Software will be liable for any of those types of damages known as indirect, special, consequential, or incidental
 *     related to the Software or this license, to the maximum extent the law permits, no matter what legal theory it’s based on.  Also, you must
 *     pass this limitation of liability on whenever you distribute the Software or derivative works.
 *  5. That if you sue anyone over patents that you think may apply to the Software for a person's use of the Software, your license to the Software
 *     ends automatically.
 *  6. That the patent rights, if any, granted in this license only apply to the Software, not to any derivative works you make.
 *  7. That the Software is subject to U.S. export jurisdiction at the time it is licensed to you, and it may be subject to additional export or
 *     import laws in other places.  You agree to comply with all such laws and regulations that may apply to the Software after delivery of the
 *     software to you.
 *  8. That if you are an agency of the U.S. Government, (i) Software provided pursuant to a solicitation issued on or after December 1, 1995, is
 *     provided with the commercial license rights set forth in this license, and (ii) Software provided pursuant to a solicitation issued prior to
 *     December 1, 1995, is provided with “Restricted Rights” as set forth in FAR, 48 C.F.R. 52.227-14 (June 1987) or DFAR, 48 C.F.R. 252.227-7013 
 *     (Oct 1988), as applicable.
 *  9. That your rights under this License end automatically if you breach it in any way.
 * 10. That all rights not expressly granted to you in this license are reserved.
 */
#endregion
namespace NDde.Foundation.Advanced
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Text;
    using System.Threading;
    using System.Windows.Forms;
    using NDde.Foundation.Client;
    using NDde.Foundation.Server;
    using NDde.Properties;

    internal sealed class DdemlContext : IDisposable
    {
        private static WeakReferenceDictionary<int, DdemlContext> _Instances = new WeakReferenceDictionary<int, DdemlContext>();

        internal static DdemlContext GetDefault()
        {
            lock (_Instances)
            {
                DdemlContext context = _Instances[Ddeml.GetCurrentThreadId()];
                if (context == null)
                {
                    context = new DdemlContext();
                    _Instances.Add(Ddeml.GetCurrentThreadId(), context);
                }
                return context;
            }
        }

        private int                                          _InstanceId   = 0;                                                   // DDEML instance identifier
        private Ddeml.DdeCallback                            _Callback     = null;                                                // DDEML callback function
        private WeakReferenceDictionary<IntPtr, DdemlClient> _ClientTable  = new WeakReferenceDictionary<IntPtr, DdemlClient>();  // Active clients by conversation
        private WeakReferenceDictionary<IntPtr, DdemlServer> _ServerTable1 = new WeakReferenceDictionary<IntPtr, DdemlServer>();  // Active servers by conversation
        private WeakReferenceDictionary<string, DdemlServer> _ServerTable2 = new WeakReferenceDictionary<string, DdemlServer>();  // Active servers by service
        private List<IDdemlTransactionFilter>                _Filters      = new List<IDdemlTransactionFilter>();                 // ITransactionFilter objects
        private Encoding                                     _Encoding     = Encoding.ASCII;
        private bool                                         _Disposed     = false;

        public event EventHandler<DdemlRegistrationEventArgs> Register;

        public event EventHandler<DdemlRegistrationEventArgs> Unregister;

        internal event EventHandler StateChange;

        public DdemlContext()
        {
            // Create the callback that will be used by the DDEML.
            _Callback = this.OnDdeCallback;
        }

        ~DdemlContext()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_Disposed)
            {
                _Disposed = true;
                if (disposing)
                {
                    // Dispose all clients.
                    foreach (DdemlClient client in _ClientTable.Values)
                    {
                        client.Dispose();
                    }

                    // Dispose all servers.
                    foreach (DdemlServer server in _ServerTable2.Values)
                    {
                        server.Dispose();
                    }
                    
                    // Raise the StateChange event.
                    foreach (EventHandler handler in StateChange.GetInvocationList())
                    {
                        try
                        {
                            handler(this, EventArgs.Empty);
                        }
                        catch
                        {
                            // Swallow any exception that occurs.
                        }
                    }
                }

                if (IsInitialized) 
                {
                    // Uninitialize this DDEML instance.
                    InstanceManager.Uninitialize(_InstanceId);

                    // Indicate that this object is no longer initialized.
                    _InstanceId = 0;
                }
            }
        }

        public int InstanceId
        {
            get { return _InstanceId; }
        }

        public bool IsInitialized
        {
            get { return _InstanceId != 0; }
        }

        public Encoding Encoding
        {
            get { return _Encoding; }
            set { _Encoding = value; }
        }

        internal bool IsDisposed
        {
            get { return _Disposed; }
        }

        public void Initialize()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(this.GetType().ToString());
            }
            if (IsInitialized)
            {
                throw new InvalidOperationException(Resources.AlreadyInitializedMessage);
            }

            Initialize(Ddeml.APPCLASS_STANDARD);

            // Raise the StateChange event.
            if (StateChange != null)
            {
                StateChange(this, EventArgs.Empty);
            }
        }

        public void AddTransactionFilter(IDdemlTransactionFilter filter)
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(this.GetType().ToString());
            }
            if (filter == null)
            {
                throw new ArgumentNullException("filter");
            }
            if (_Filters.Contains(filter))
            {
                throw new InvalidOperationException(Resources.FilterAlreadyAddedMessage);
            }

            _Filters.Add(filter);
        }

        public void RemoveTransactionFilter(IDdemlTransactionFilter filter)
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(this.GetType().ToString());
            }
            if (filter == null)
            {
                throw new ArgumentNullException("filter");
            }
            if (!_Filters.Contains(filter))
            {
                throw new InvalidOperationException(Resources.FilterNotAddedMessage);
            }

            _Filters.Remove(filter);
        }

        internal void Initialize(int afCmd)
        {
            // Initialize a DDEML instance.
            _InstanceId = InstanceManager.Initialize(_Callback, afCmd);
            
            // If the instance identifier is null then the DDEML could not be initialized.
            if (_InstanceId == 0)
            {
                int error = Ddeml.DdeGetLastError(_InstanceId);
                throw new DdemlException(Resources.InitializeFailedMessage, error);
            }
        }

        internal void RegisterClient(DdemlClient client)
        {
            _ClientTable[client.Handle] = client;
        }

        internal void RegisterServer(DdemlServer server)
        {
            _ServerTable2[server.Service] = server;
        }

        internal void UnregisterClient(DdemlClient client)
        {
            _ClientTable[client.Handle] = null;
        }

        internal void UnregisterServer(DdemlServer server)
        {
            _ServerTable2[server.Service] = null;
        }

        private IntPtr OnDdeCallback(int uType, int uFmt, IntPtr hConv, IntPtr hsz1, IntPtr hsz2, IntPtr hData, IntPtr dwData1, IntPtr dwData2)
        {
            // Create a new transaction object that will be dispatched to a DdemlClient, DdemlServer, or ITransactionFilter.
            DdemlTransaction t = new DdemlTransaction(uType, uFmt, hConv, hsz1, hsz2, hData, dwData1, dwData2);

            // Run each transaction filter.
            foreach (IDdemlTransactionFilter filter in _Filters)
            {
                if (filter.PreFilterTransaction(t))
                {
                    return t.dwRet;
                }
            }

            // Dispatch the transaction.
            switch (uType)
            {
                case Ddeml.XTYP_ADVDATA:
                {
                    DdemlClient client = _ClientTable[hConv] as DdemlClient;
                    if (client != null)
                    {
                        if (client.ProcessCallback(t))
                        {
                            return t.dwRet;
                        }
                    }
                    break;
                }
                case Ddeml.XTYP_ADVREQ:
                {
                    DdemlServer server = _ServerTable1[hConv] as DdemlServer;
                    if (server != null)
                    {
                        if (server.ProcessCallback(t))
                        {
                            return t.dwRet;
                        }
                    }
                    break;
                }
                case Ddeml.XTYP_ADVSTART:
                {
                    DdemlServer server = _ServerTable1[hConv] as DdemlServer;
                    if (server != null)
                    {
                        if (server.ProcessCallback(t))
                        {
                            return t.dwRet;
                        }
                    }
                    break;
                }
                case Ddeml.XTYP_ADVSTOP:
                {
                    DdemlServer server = _ServerTable1[hConv] as DdemlServer;
                    if (server != null)
                    {
                        if (server.ProcessCallback(t))
                        {
                            return t.dwRet;
                        }
                    }
                    break;
                }
                case Ddeml.XTYP_CONNECT:
                {
                    // Get the service name from the hsz2 string handle.
                    StringBuilder psz = new StringBuilder(Ddeml.MAX_STRING_SIZE);
                    int length = Ddeml.DdeQueryString(_InstanceId, hsz2, psz, psz.Capacity, Ddeml.CP_WINANSI);
                    string service = psz.ToString();

                    DdemlServer server = _ServerTable2[service] as DdemlServer;
                    if (server != null)
                    {
                        if (server.ProcessCallback(t))
                        {
                            return t.dwRet;
                        }
                    }
                    break;
                }
                case Ddeml.XTYP_CONNECT_CONFIRM:
                {
                    // Get the service name from the hsz2 string handle.
                    StringBuilder psz = new StringBuilder(Ddeml.MAX_STRING_SIZE);
                    int length = Ddeml.DdeQueryString(_InstanceId, hsz2, psz, psz.Capacity, Ddeml.CP_WINANSI);
                    string service = psz.ToString();

                    DdemlServer server = _ServerTable2[service] as DdemlServer;
                    if (server != null)
                    {
                        _ServerTable1[hConv] = server;
                        if (server.ProcessCallback(t))
                        {
                            return t.dwRet;
                        }
                    }
                    break;
                }
                case Ddeml.XTYP_DISCONNECT:
                {
                    DdemlClient client = _ClientTable[hConv] as DdemlClient;
                    if (client != null)
                    {
                        _ClientTable[hConv] = null;
                        if (client.ProcessCallback(t))
                        {
                            return t.dwRet;
                        }
                    }
                    DdemlServer server = _ServerTable1[hConv] as DdemlServer;
                    if (server != null)
                    {
                        _ServerTable1[hConv] = null;
                        if (server.ProcessCallback(t))
                        {
                            return t.dwRet;
                        }
                    }
                    break;
                }
                case Ddeml.XTYP_EXECUTE:
                {
                    DdemlServer server = _ServerTable1[hConv] as DdemlServer;
                    if (server != null)
                    {
                        if (server.ProcessCallback(t))
                        {
                            return t.dwRet;
                        }
                    }
                    break;
                }
                case Ddeml.XTYP_POKE:
                {
                    DdemlServer server = _ServerTable1[hConv] as DdemlServer;
                    if (server != null)
                    {
                        if (server.ProcessCallback(t))
                        {
                            return t.dwRet;
                        }
                    }
                    break;
                }
                case Ddeml.XTYP_REQUEST:
                {
                    DdemlServer server = _ServerTable1[hConv] as DdemlServer;
                    if (server != null)
                    {
                        if (server.ProcessCallback(t))
                        {
                            return t.dwRet;
                        }
                    }
                    break;
                }
                case Ddeml.XTYP_XACT_COMPLETE:
                {
                    DdemlClient client = _ClientTable[hConv] as DdemlClient;
                    if (client != null)
                    {
                        if (client.ProcessCallback(t))
                        {
                            return t.dwRet;
                        }
                    }
                    break;
                }
                case Ddeml.XTYP_WILDCONNECT:
                {
                    // This library does not support wild connects.
                    return IntPtr.Zero;
                }
                case Ddeml.XTYP_MONITOR:
                {
                    // Monitors are handled separately in DdemlMonitor.
                    return IntPtr.Zero;
                }
                case Ddeml.XTYP_ERROR:
                {
                    // Get the error code, but do nothing with it at this time.
                    int error = dwData1.ToInt32();

                    return IntPtr.Zero;
                }
                case Ddeml.XTYP_REGISTER:
                {
                    if (Register != null)
                    {
                        // Get the service name from the hsz1 string handle.
                        StringBuilder psz = new StringBuilder(Ddeml.MAX_STRING_SIZE);
                        int length = Ddeml.DdeQueryString(_InstanceId, hsz1, psz, psz.Capacity, Ddeml.CP_WINANSI);
                        string service = psz.ToString();

                        Register(this, new DdemlRegistrationEventArgs(service));
                    }
                    return IntPtr.Zero;
                }
                case Ddeml.XTYP_UNREGISTER:
                {
                    if (Unregister != null)
                    {
                        // Get the service name from the hsz1 string handle.
                        StringBuilder psz = new StringBuilder(Ddeml.MAX_STRING_SIZE);
                        int length = Ddeml.DdeQueryString(_InstanceId, hsz1, psz, psz.Capacity, Ddeml.CP_WINANSI);
                        string service = psz.ToString();

                        Unregister(this, new DdemlRegistrationEventArgs(service));
                    }
                    return IntPtr.Zero;
                }
            }			
            return IntPtr.Zero;
        }

        /// <summary>
        /// This class is needed to dispose of DDEML resources correctly since the DDEML is thread specific.
        /// </summary>
        private sealed class InstanceManager : IMessageFilter
        {
            private const int WM_APP = unchecked((int)0x8000);

            private static readonly string DataSlot = typeof(InstanceManager).FullName;

            private static IDictionary<int, int> _Table = new Dictionary<int, int>();

            [System.Runtime.InteropServices.DllImport("user32.dll")]
            private static extern void PostThreadMessage(int idThread, int Msg, IntPtr wParam, IntPtr lParam);

            public static int Initialize(Ddeml.DdeCallback pfnCallback, int afCmd)
            {
                lock (_Table) 
                {
                    // Initialize a DDEML instance.
                    int instanceId = 0;
                    Ddeml.DdeInitialize(ref instanceId, pfnCallback, afCmd, 0);
                    
                    if (instanceId != 0)
                    {
                        // Make sure this thread has an IMessageFilter on it.
                        LocalDataStoreSlot slot = Thread.GetNamedDataSlot(DataSlot);
                        if (Thread.GetData(slot) == null) 
                        {
                            InstanceManager filter = new InstanceManager();
                            Application.AddMessageFilter(filter);
                            Thread.SetData(slot, filter);
                        }

                        // Add an entry to the table that maps the instance identifier to the current thread.
                        _Table.Add(instanceId, Ddeml.GetCurrentThreadId());
                    }
                    
                    return instanceId;
                }
            }

            public static void Uninitialize(int instanceId)
            {
                // This method could be called by the GC finalizer thread.  If it is then a direct call to the DDEML will fail since the DDEML is 
                // thread specific.  A message will be posted to the DDEML thread instead.
                lock (_Table) 
                {
                    if (_Table.ContainsKey(instanceId))
                    {
                        // Determine if the current thread matches what is in the table.
                        int threadId = (int)_Table[instanceId];
                        if (threadId == Ddeml.GetCurrentThreadId())
                        {
                            // Uninitialize the DDEML instance.
                            Ddeml.DdeUninitialize(instanceId);
                        }
                        else
                        {
                            // Post a message to the thread that needs to execute Ddeml.DdeUninitialize.
                            PostThreadMessage(threadId, WM_APP + 1, new IntPtr(instanceId), IntPtr.Zero);
                        }

                        // Remove the instance identifier from the table because it is no longer in use.
                        _Table.Remove(instanceId);
                    }
                }
            }

            bool IMessageFilter.PreFilterMessage(ref Message m)
            {
                if (m.Msg == WM_APP + 1)
                {
                    // Uninitialize the DDEML instance.
                    Ddeml.DdeUninitialize(m.WParam.ToInt32());
                }
                return false;
            }

        } // class

    } // class

} // namespace