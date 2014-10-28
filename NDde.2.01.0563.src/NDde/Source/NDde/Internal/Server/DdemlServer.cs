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
namespace NDde.Foundation.Server
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;
    using System.Windows.Forms;
    using NDde.Foundation.Advanced;
    using NDde.Properties;

    internal abstract class DdemlServer : IDisposable
    {
        private DdemlContext                            _Context            = null;                                        // DDEML instance manager
        private int                                     _InstanceId         = 0;                                           // DDEML instance identifier
        private string                                  _Service            = "";                                          // DDEML service name
        private IntPtr                                  _ServiceHandle      = IntPtr.Zero;                                 // DDEML service handle
        private IDictionary<IntPtr, DdemlConversation>  _ConversationTable  = new Dictionary<IntPtr, DdemlConversation>(); // Active DDEML conversations
        private IDictionary<string, byte[]>             _AdviseRequestCache = new Dictionary<string, byte[]>();            // Cached advise data
        private bool                                    _Disposed           = false;

        internal event EventHandler StateChange;

        public DdemlServer(string service) 
            : this(service, DdemlContext.GetDefault())
        {
        }

        public DdemlServer(string service, DdemlContext context)
        {
            if (service == null)
            {
                throw new ArgumentNullException("service");
            }
            if (service.Length > Ddeml.MAX_STRING_SIZE)
            {
                throw new ArgumentException(Resources.StringParameterInvalidMessage, "service");
            }
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            
            _Service = service;
            _Context = context;			
        }

        ~DdemlServer()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_Disposed)
            {
                _Disposed = true;
                if (disposing)
                {
                    if (IsRegistered)
                    {
                        // Unregister the service name.
                        RegistrationManager.Unregister(_InstanceId, _ServiceHandle);

                        // Unregister this server from the context so that it will not receive DDEML callbacks.
                        _Context.UnregisterServer(this);

                        // Indicate that the service name is no longer registered.
                        _ServiceHandle = IntPtr.Zero;
                        _InstanceId = 0;
                        
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
                }
                else 
                {
                    if (IsRegistered) 
                    {
                        // Unregister the service name.
                        RegistrationManager.Unregister(_InstanceId, _ServiceHandle);
                    }
                }
            }
        }

        public virtual string Service
        {
            get { return _Service; }
        }

        public virtual bool IsRegistered
        {
            get { return _ServiceHandle != IntPtr.Zero; }
        }

        internal bool IsDisposed
        {
            get { return _Disposed; }
        }

        public virtual void Register()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(this.GetType().ToString());
            }
            if (IsRegistered)
            {
                throw new InvalidOperationException(Resources.AlreadyRegisteredMessage);
            }

            // Make sure the context is initialized.
            if (!_Context.IsInitialized)
            {
                _Context.Initialize();
            }

            // Get a local copy of the DDEML instance identifier so that it can be used in the finalizer.
            _InstanceId = _Context.InstanceId;

            // Make sure the conversation table is empty.
            _ConversationTable.Clear();

            // Register the service name.
            _ServiceHandle = RegistrationManager.Register(_InstanceId, _Service);
    
            // If the service handle is null then the service name could not be registered.
            if (_ServiceHandle == IntPtr.Zero)
            {
                int error = Ddeml.DdeGetLastError(_InstanceId);
                string message = Resources.RegisterFailedMessage;
                message = message.Replace("${service}", _Service);
                throw new DdemlException(message, error);
            }

            // Register this server with the context so that it can receive DDEML callbacks.
            _Context.RegisterServer(this);

            // Raise the StateChange event.
            if (StateChange != null)
            {
                StateChange(this, EventArgs.Empty);
            }
        }

        public virtual void Unregister()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(this.GetType().ToString());
            }
            if (!IsRegistered)
            {
                throw new InvalidOperationException(Resources.NotRegisteredMessage);
            }

            // Unregister the service name.
            RegistrationManager.Unregister(_InstanceId, _ServiceHandle);

            // Unregister this server from the context so that it will not receive DDEML callbacks.
            _Context.UnregisterServer(this);

            // Indicate that the service name is no longer registered.
            _ServiceHandle = IntPtr.Zero;
            _InstanceId = 0;

            // Raise the StateChange event.
            if (StateChange != null)
            {
                StateChange(this, EventArgs.Empty);
            }
        }

        public virtual void Advise(string topic, string item)
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(this.GetType().ToString());
            }
            if (!IsRegistered)
            {
                throw new InvalidOperationException(Resources.NotRegisteredMessage);
            }
            if (topic == null)
            {
                throw new ArgumentNullException("topic");
            }
            if (topic.Length > Ddeml.MAX_STRING_SIZE)
            {
                throw new ArgumentException(Resources.StringParameterInvalidMessage, "topic");
            }
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            if (item.Length > Ddeml.MAX_STRING_SIZE)
            {
                throw new ArgumentException(Resources.StringParameterInvalidMessage, "item");
            }

            // Assume the topic name and item name are wild.
            IntPtr topicHandle = IntPtr.Zero;
            IntPtr itemHandle  = IntPtr.Zero;

            // Create a string handle for the topic name if it is not wild.
            if (topic != "*") 
            {
                topicHandle = Ddeml.DdeCreateStringHandle(_InstanceId, topic, Ddeml.CP_WINANSI);
            }

            // Create a string handle for the item name if it is not wild.
            if (item != "*") 
            {
                itemHandle = Ddeml.DdeCreateStringHandle(_InstanceId, item, Ddeml.CP_WINANSI);
            }

            // Post an advise notification.  This will cause an XTYP_ADVREQ transaction for each conversation.
            bool result = Ddeml.DdePostAdvise(_InstanceId, topicHandle, itemHandle);

            // Free the string handles created earlier.
            Ddeml.DdeFreeStringHandle(_InstanceId, itemHandle);
            Ddeml.DdeFreeStringHandle(_InstanceId, topicHandle);

            // Check the result to see if the post failed.
            if (!result)
            {
                int error = Ddeml.DdeGetLastError(_InstanceId);
                string message = Resources.AdviseFailedMessage;
                message = message.Replace("${service}", _Service);
                message = message.Replace("${topic}", topic);
                message = message.Replace("${item}", item);
                throw new DdemlException(message, error);
            }
        }

        public virtual void Pause(DdemlConversation conversation)
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(this.GetType().ToString());
            }
            if (!IsRegistered)
            {
                throw new InvalidOperationException(Resources.NotRegisteredMessage);
            }
            if (conversation == null)
            {
                throw new ArgumentNullException("conversation");
            }
            if (conversation.IsPaused)
            {
                throw new InvalidOperationException(Resources.AlreadyPausedMessage);
            }

            // Disable the DDEML callback for the specified conversation only.
            bool result = Ddeml.DdeEnableCallback(_InstanceId, conversation.Handle, Ddeml.EC_DISABLE);

            // Check the result to see if the DDEML callback was disabled.
            if (!result)
            {
                int error = Ddeml.DdeGetLastError(_InstanceId);
                throw new DdemlException(Resources.ServerPauseFailedMessage, error);
            }

            // Increment the conversation's waiting count.
            conversation.IncrementWaiting();
        }

        public virtual void Pause()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(this.GetType().ToString());
            }
            if (!IsRegistered)
            {
                throw new InvalidOperationException(Resources.NotRegisteredMessage);
            }

            // Disable the DDEML callback for all conversations.
            bool result = Ddeml.DdeEnableCallback(_InstanceId, IntPtr.Zero, Ddeml.EC_DISABLE);

            // Check the result to see if the DDEML callback was disabled.
            if (!result)
            {
                int error = Ddeml.DdeGetLastError(_InstanceId);
                throw new DdemlException(Resources.ServerPauseAllFailedMessage, error);
            }

            // Increment each conversation's waiting count.
            foreach (DdemlConversation conversation in _ConversationTable.Values)
            {
                conversation.IncrementWaiting();
            }
        }

        public virtual void Resume(DdemlConversation conversation)
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(this.GetType().ToString());
            }
            if (!IsRegistered)
            {
                throw new InvalidOperationException(Resources.NotRegisteredMessage);
            }
            if (conversation == null)
            {
                throw new ArgumentNullException("conversation");
            }
            if (!conversation.IsPaused)
            {
                throw new InvalidOperationException(Resources.NotPausedMessage);
            }

            // Enable the DDEML callback for the specified conversation only.
            bool result = Ddeml.DdeEnableCallback(_InstanceId, conversation.Handle, Ddeml.EC_ENABLEALL);

            // Check the result to see if the DDEML callback was enabled.
            if (!result)
            {
                int error = Ddeml.DdeGetLastError(_InstanceId);
                throw new DdemlException(Resources.ServerResumeFailedMessage, error);
            }

            // Decrement the conversation's waiting count.  The conversation will only resume if the count is zero.
            conversation.DecrementWaiting();
        }

        public virtual void Resume()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(this.GetType().ToString());
            }
            if (!IsRegistered)
            {
                throw new InvalidOperationException(Resources.NotRegisteredMessage);
            }

            // Enable the DDEML callback for all conversations.
            bool result = Ddeml.DdeEnableCallback(_InstanceId, IntPtr.Zero, Ddeml.EC_ENABLEALL);

            // Check the result to see if the DDEML callback was enabled.
            if (!result)
            {
                int error = Ddeml.DdeGetLastError(_InstanceId);
                throw new DdemlException(Resources.ServerResumeAllFailedMessage, error);
            }

            // Decrement each conversation's waiting count.  The conversation will only resume if the count is zero.
            foreach (DdemlConversation conversation in _ConversationTable.Values)
            {
                conversation.DecrementWaiting();
            }
        }

        public virtual void Disconnect(DdemlConversation conversation)
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(this.GetType().ToString());
            }
            if (!IsRegistered)
            {
                throw new InvalidOperationException(Resources.NotRegisteredMessage);
            }
            if (conversation == null)
            {
                throw new ArgumentNullException("conversation");
            }
            
            if (_ConversationTable.ContainsKey(conversation.Handle))
            {
                // Terminate the conversation.
                Ddeml.DdeDisconnect(conversation.Handle);

                // Remove the Conversation from the conversation table.
                _ConversationTable.Remove(conversation.Handle);
            }
        }

        public virtual void Disconnect()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(this.GetType().ToString());
            }
            if (!IsRegistered)
            {
                throw new InvalidOperationException(Resources.NotRegisteredMessage);
            }

            // Terminate all conversations.
            foreach (DdemlConversation conversation in _ConversationTable.Values)
            {
                Ddeml.DdeDisconnect(conversation.Handle);
            }

            // clear the conversation table.
            _ConversationTable.Clear();
        }

        internal bool ProcessCallback(DdemlTransaction transaction)
        {
            // This is here to alias the transaction object with a shorter variable name.
            DdemlTransaction t = transaction;

            switch (t.uType)
            {
                case Ddeml.XTYP_ADVREQ:
                {
                    StringBuilder psz;
                    int length;

                    // Get the topic name from the hsz1 string handle.
                    psz = new StringBuilder(Ddeml.MAX_STRING_SIZE);
                    length = Ddeml.DdeQueryString(_InstanceId, t.hsz1, psz, psz.Capacity, Ddeml.CP_WINANSI);
                    string topic = psz.ToString();

                    // Get the item name from the hsz2 string handle.
                    psz = new StringBuilder(Ddeml.MAX_STRING_SIZE);
                    length = Ddeml.DdeQueryString(_InstanceId, t.hsz2, psz, psz.Capacity, Ddeml.CP_WINANSI);
                    string item = psz.ToString();

                    // Create the advise request cache key.
                    string key = topic + "!" + item + ":" + t.uFmt.ToString();
                    
                    // Get the data being advised if the cache does not contain it already.
                    if (!_AdviseRequestCache.ContainsKey(key))
                    {
                        // Get the data from the subclass.
                        byte[] data = OnAdvise(topic, item, t.uFmt);

                        // Add the data to the cache because it will be needed later.
                        _AdviseRequestCache.Add(key, data);
                    }

                    // Get the data from the advise request cache.
                    byte[] cached = _AdviseRequestCache[key];

                    // Get the number of remaining transactions of this type for the same topic name, item name, and format tuple.
                    int remaining = t.dwData1.ToInt32();

                    // If this is the last transaction then free the data handle.
                    if (remaining == 0)
                    {
                        // TODO: Does the data handle really need to be freed here?

                        // Remove the data from the cache because it is no longer needed.
                        _AdviseRequestCache.Remove(key);
                    }

                    // Create and return the data handle representing the data being advised.
                    if (cached != null && cached.Length > 0) 
                    {
                        t.dwRet = Ddeml.DdeCreateDataHandle(_InstanceId, cached, cached.Length, 0, t.hsz2, t.uFmt, 0);
                        return true;
                    }

                    // This transaction could not be processed here.
                    return false;
                }
                case Ddeml.XTYP_ADVSTART:
                {
                    // Get the item name from the hsz2 string handle.
                    StringBuilder psz = new StringBuilder(Ddeml.MAX_STRING_SIZE);
                    int length = Ddeml.DdeQueryString(_InstanceId, t.hsz2, psz, psz.Capacity, Ddeml.CP_WINANSI);
                    string item = psz.ToString();

                    // Get the Conversation from the conversation table.
                    DdemlConversation conversation = _ConversationTable[t.hConv];

                    // Get a value indicating whether an advise loop should be initiated from the subclass.
                    t.dwRet = OnStartAdvise(conversation, item, t.uFmt) ? new IntPtr(1) : IntPtr.Zero;
                    return true;
                }
                case Ddeml.XTYP_ADVSTOP:
                {
                    // Get the item name from the hsz2 string handle.
                    StringBuilder psz = new StringBuilder(Ddeml.MAX_STRING_SIZE);
                    int length = Ddeml.DdeQueryString(_InstanceId, t.hsz2, psz, psz.Capacity, Ddeml.CP_WINANSI);
                    string item = psz.ToString();

                    // Get the Conversation from the conversation table.
                    DdemlConversation conversation = _ConversationTable[t.hConv];

                    // Inform the subclass that the advise loop has been terminated.
                    OnStopAdvise(conversation, item);
                    
                    // Return zero to indicate that there are no problems.
                    t.dwRet = IntPtr.Zero;
                    return true;
                }
                case Ddeml.XTYP_CONNECT:
                {
                    // Get the topic name from the hsz1 string handle.
                    StringBuilder psz = new StringBuilder(Ddeml.MAX_STRING_SIZE);
                    int length = Ddeml.DdeQueryString(_InstanceId, t.hsz1, psz, psz.Capacity, Ddeml.CP_WINANSI);
                    string topic = psz.ToString();

                    // Get a value from the subclass indicating whether the connection should be allowed.
                    t.dwRet = OnBeforeConnect(topic) ? new IntPtr(1) : IntPtr.Zero;
                    return true;
                }
                case Ddeml.XTYP_CONNECT_CONFIRM:
                {
                    // Get the topic name from the hsz1 string handle.
                    StringBuilder psz = new StringBuilder(Ddeml.MAX_STRING_SIZE);
                    int length = Ddeml.DdeQueryString(_InstanceId, t.hsz1, psz, psz.Capacity, Ddeml.CP_WINANSI);
                    string topic = psz.ToString();

                    // Create a Conversation object and add it to the conversation table.
                    _ConversationTable.Add(t.hConv, new DdemlConversation(t.hConv, _Service, topic));

                    // Inform the subclass that a conversation has been established.
                    OnAfterConnect(_ConversationTable[t.hConv]);
                    
                    // Return zero to indicate that there are no problems.
                    t.dwRet = IntPtr.Zero;
                    return true;
                }
                case Ddeml.XTYP_DISCONNECT:
                {
                    // Get the Conversation from the conversation table.
                    DdemlConversation conversation = _ConversationTable[t.hConv];

                    // Remove the Conversation from the conversation table.
                    _ConversationTable.Remove(t.hConv);

                    // Inform the subclass that the conversation has been disconnected.
                    OnDisconnect(conversation);

                    // Return zero to indicate that there are no problems.
                    t.dwRet = IntPtr.Zero;
                    return true;
                }
                case Ddeml.XTYP_EXECUTE:
                {
                    // Get the command from the data handle.
                    int length = Ddeml.DdeGetData(t.hData, null, 0, 0);
                    byte[] data = new byte[length];
                    length = Ddeml.DdeGetData(t.hData, data, data.Length, 0);
                    string command = _Context.Encoding.GetString(data, 0, data.Length);
                    if (command[command.Length - 1] == '\0')
                    {
                        command = command.Substring(0, command.Length - 1);
                    }
                    
                    // Get the Conversation from the conversation table.
                    DdemlConversation conversation = _ConversationTable[t.hConv];

                    // Send the command to the subclass and get the result.
                    ExecuteResult result = OnExecute(conversation, command);

                    // Return DDE_FACK if the subclass processed the command successfully.
                    if (result == ExecuteResult.Processed)
                    {
                        t.dwRet = new IntPtr(Ddeml.DDE_FACK);
                        return true;
                    }

                    // Return CBR_BLOCK if the subclass needs time to process the command.
                    if (result == ExecuteResult.PauseConversation)
                    {
                        // Increment the conversation's waiting count.
                        conversation.IncrementWaiting();
                        t.dwRet = new IntPtr(Ddeml.CBR_BLOCK);
                        return true;
                    }

                    // Return DDE_FBUSY if the subclass is too busy.
                    if (result == ExecuteResult.TooBusy)
                    {
                        t.dwRet = new IntPtr(Ddeml.DDE_FBUSY);
                        return true;
                    }

                    // Return DDE_FNOTPROCESSED if the subclass did not process the command.
                    t.dwRet = new IntPtr(Ddeml.DDE_FNOTPROCESSED);
                    return true;
                }
                case Ddeml.XTYP_POKE:
                {
                    // Get the item name from the hsz2 string handle.
                    StringBuilder psz = new StringBuilder(Ddeml.MAX_STRING_SIZE);
                    int length = Ddeml.DdeQueryString(_InstanceId, t.hsz2, psz, psz.Capacity, Ddeml.CP_WINANSI);
                    string item = psz.ToString();

                    // Get the data from the data handle.
                    length = Ddeml.DdeGetData(t.hData, null, 0, 0);
                    byte[] data = new byte[length];
                    length = Ddeml.DdeGetData(t.hData, data, data.Length, 0);

                    // Get the Conversation from the conversation table.
                    DdemlConversation conversation = _ConversationTable[t.hConv];

                    // Send the data to the subclass and get the result.
                    PokeResult result = OnPoke(conversation, item, data, t.uFmt);

                    // Return DDE_FACK if the subclass processed the data successfully.
                    if (result == PokeResult.Processed)
                    {
                        t.dwRet = new IntPtr(Ddeml.DDE_FACK);
                        return true;
                    }

                    // Return CBR_BLOCK if the subclass needs time to process the data.
                    if (result == PokeResult.PauseConversation)
                    {
                        // Increment the conversation's waiting count.
                        conversation.IncrementWaiting();
                        t.dwRet = new IntPtr(Ddeml.CBR_BLOCK);
                        return true;
                    }

                    // Return DDE_FBUSY if the subclass is too busy.
                    if (result == PokeResult.TooBusy)
                    {
                        t.dwRet = new IntPtr(Ddeml.DDE_FBUSY);
                        return true;
                    }

                    // Return DDE_FNOTPROCESSED if the subclass did not process the data.
                    t.dwRet = new IntPtr(Ddeml.DDE_FNOTPROCESSED);
                    return true;
                }
                case Ddeml.XTYP_REQUEST:
                {
                    // Get the item name from the hsz2 string handle.
                    StringBuilder psz = new StringBuilder(Ddeml.MAX_STRING_SIZE);
                    int length = Ddeml.DdeQueryString(_InstanceId, t.hsz2, psz, psz.Capacity, Ddeml.CP_WINANSI);
                    string item = psz.ToString();

                    // Get the Conversation from the conversation table.
                    DdemlConversation conversation = _ConversationTable[t.hConv];

                    // Send the request to the subclass and get the result.
                    RequestResult result = OnRequest(conversation, item, t.uFmt);

                    // Return a data handle if the subclass processed the request successfully.
                    if (result == RequestResult.Processed)
                    {
                        // Create and return the data handle for the data being requested.
                        if (result.Data != null)
                        {
                            t.dwRet = Ddeml.DdeCreateDataHandle(_InstanceId, result.Data, result.Data.Length, 0, t.hsz2, t.uFmt, 0);
                        }
                        return true;
                    }

                    // Return CBR_BLOCK if the subclass needs time to process the request.
                    if (result == RequestResult.PauseConversation)
                    {
                        conversation.IncrementWaiting();
                        t.dwRet = new IntPtr(Ddeml.CBR_BLOCK);
                        return true;
                    }

                    // Return DDE_FNOTPROCESSED if the subclass did not process the command.
                    t.dwRet = new IntPtr(Ddeml.DDE_FNOTPROCESSED);
                    return true;
                }
            }

            // This transaction could not be processed here.
            return false;
        }

        protected virtual bool OnStartAdvise(DdemlConversation conversation, string item, int format)
        {
            return true;
        }

        protected virtual void OnStopAdvise(DdemlConversation conversation, string item)
        {
        }

        protected virtual bool OnBeforeConnect(string topic)
        {
            return true;
        }

        protected virtual void OnAfterConnect(DdemlConversation conversation)
        {
        }

        protected virtual void OnDisconnect(DdemlConversation conversation)
        {
        }

        protected virtual ExecuteResult OnExecute(DdemlConversation conversation, string command)
        {
            return ExecuteResult.NotProcessed;
        }

        protected virtual PokeResult OnPoke(DdemlConversation conversation, string item, byte[] data, int format)
        {
            return PokeResult.NotProcessed;
        }

        protected virtual RequestResult OnRequest(DdemlConversation conversation, string item, int format)
        {
            return RequestResult.NotProcessed;
        }

        protected virtual byte[] OnAdvise(string topic, string item, int format)
        {
            return null;
        }

        public struct ExecuteResult
        {
            public static readonly ExecuteResult Processed         = new ExecuteResult(Ddeml.DDE_FACK);
            public static readonly ExecuteResult NotProcessed      = new ExecuteResult(Ddeml.DDE_FNOTPROCESSED);
            public static readonly ExecuteResult TooBusy           = new ExecuteResult(Ddeml.DDE_FBUSY);
            public static readonly ExecuteResult PauseConversation = new ExecuteResult(Ddeml.CBR_BLOCK);

            private int _Result;

            private ExecuteResult(int result)
            {
                _Result = result;
            }

            public override bool Equals(object o)
            {
                if (o is ExecuteResult)
                {
                    ExecuteResult r = (ExecuteResult)o;
                    return _Result == r._Result;
                }
                return false;
            }

            public override int GetHashCode()
            {
                return _Result.GetHashCode();
            }

            public static bool operator ==(ExecuteResult lhs, ExecuteResult rhs)
            {
                return lhs._Result == rhs._Result;
            }

            public static bool operator !=(ExecuteResult lhs, ExecuteResult rhs)
            {
                return lhs._Result != rhs._Result;
            }

        } // struct

        public struct PokeResult
        {
            public static readonly PokeResult Processed         = new PokeResult(Ddeml.DDE_FACK);
            public static readonly PokeResult NotProcessed      = new PokeResult(Ddeml.DDE_FNOTPROCESSED);
            public static readonly PokeResult TooBusy           = new PokeResult(Ddeml.DDE_FBUSY);
            public static readonly PokeResult PauseConversation = new PokeResult(Ddeml.CBR_BLOCK);

            private int _Result;

            private PokeResult(int result)
            {
                _Result = result;
            }

            public override bool Equals(object o)
            {
                if (o is PokeResult)
                {
                    PokeResult r = (PokeResult)o;
                    return _Result == r._Result;
                }
                return false;
            }

            public override int GetHashCode()
            {
                return _Result.GetHashCode();
            }

            public static bool operator ==(PokeResult lhs, PokeResult rhs)
            {
                return lhs._Result == rhs._Result;
            }

            public static bool operator !=(PokeResult lhs, PokeResult rhs)
            {
                return lhs._Result != rhs._Result;
            }

        } // struct

        public struct RequestResult
        {
            internal static readonly RequestResult Processed         = new RequestResult(Ddeml.DDE_FACK);
            public   static readonly RequestResult NotProcessed      = new RequestResult(Ddeml.DDE_FNOTPROCESSED);
            public   static readonly RequestResult PauseConversation = new RequestResult(Ddeml.CBR_BLOCK);

            private int    _Result;
            private byte[] _Data;

            private RequestResult(int result)
            {
                _Result = result;
                _Data = null;
            }

            public RequestResult(byte[] data)
            {
                _Result = data != null ? Ddeml.DDE_FACK : Ddeml.DDE_FNOTPROCESSED;
                _Data = data;
            }

            public byte[] Data
            {
                get { return _Data; }
                set { _Data = value; }
            }

            public override bool Equals(object o)
            {
                if (o is RequestResult)
                {
                    RequestResult r = (RequestResult)o;
                    return _Result == r._Result;
                }
                return false;
            }

            public override int GetHashCode()
            {
                return _Result.GetHashCode();
            }

            public static bool operator ==(RequestResult lhs, RequestResult rhs)
            {
                return lhs._Result == rhs._Result;
            }

            public static bool operator !=(RequestResult lhs, RequestResult rhs)
            {
                return lhs._Result != rhs._Result;
            }

        } // struct

        /// <summary>
        /// This class is needed to dispose of DDEML resources correctly since the DDEML is thread specific.
        /// </summary>
        private sealed class RegistrationManager : IMessageFilter
        {
            private const int WM_APP = unchecked((int)0x8000);

            private static readonly string DataSlot = typeof(RegistrationManager).FullName;

            private static IDictionary<IntPtr, int> _Table = new Dictionary<IntPtr, int>();

            [System.Runtime.InteropServices.DllImport("user32.dll")]
            private static extern void PostThreadMessage(int idThread, int Msg, IntPtr wParam, IntPtr lParam);

            public static IntPtr Register(int instanceId, string service)
            {
                lock (_Table) 
                {
                    // Create a string handle for the service name.
                    IntPtr serviceHandle = Ddeml.DdeCreateStringHandle(instanceId, service, Ddeml.CP_WINANSI);

                    // Register the service name.
                    IntPtr result = Ddeml.DdeNameService(instanceId, serviceHandle, IntPtr.Zero, Ddeml.DNS_REGISTER);
                    
                    if (result != IntPtr.Zero)
                    {
                        // Make sure this thread has an IMessageFilter on it.
                        LocalDataStoreSlot slot = Thread.GetNamedDataSlot(DataSlot);
                        if (Thread.GetData(slot) == null) 
                        {
                            RegistrationManager filter = new RegistrationManager();
                            Application.AddMessageFilter(filter);
                            Thread.SetData(slot, filter);
                        }

                        // Add an entry to the table that maps the service handle to the current thread.
                        _Table.Add(serviceHandle, Ddeml.GetCurrentThreadId());
                    }
                    else
                    {
                        // Free the string handle created earlier.
                        Ddeml.DdeFreeStringHandle(instanceId, serviceHandle);
                        serviceHandle = IntPtr.Zero;
                    }
                    
                    return serviceHandle;
                }
            }

            public static void Unregister(int instanceId, IntPtr serviceHandle)
            {
                // This method could be called by the GC finalizer thread.  If it is then a direct call to the DDEML will fail since the DDEML is 
                // thread specific.  A message will be posted to the DDEML thread instead.
                lock (_Table) 
                {
                    if (_Table.ContainsKey(serviceHandle))
                    {
                        // Determine if the current thread matches what is in the table.
                        int threadId = (int)_Table[serviceHandle];
                        if (threadId == Ddeml.GetCurrentThreadId())
                        {
                            // Unregister the service name.
                            Ddeml.DdeNameService(instanceId, serviceHandle, IntPtr.Zero, Ddeml.DNS_UNREGISTER);

                            // Free the service string handle.    
                            Ddeml.DdeFreeStringHandle(instanceId, serviceHandle);
                        }
                        else
                        {
                            // Post a message to the thread that needs to execute the Ddeml.DdeXXX methods.
                            PostThreadMessage(threadId, WM_APP + 3, new IntPtr(instanceId), serviceHandle);
                        }

                        // Remove the service handle from the table because it is no longer in use.
                        _Table.Remove(serviceHandle);
                    }
                }
            }

            bool IMessageFilter.PreFilterMessage(ref Message m)
            {
                if (m.Msg == WM_APP + 3)
                {
                    // Unregister the service name.
                    Ddeml.DdeNameService(m.WParam.ToInt32(), m.LParam, IntPtr.Zero, Ddeml.DNS_UNREGISTER);

                    // Free the service string handle.    
                    Ddeml.DdeFreeStringHandle(m.WParam.ToInt32(), m.LParam);
                }
                return false;
            }

        } // class

    } // class

} // namespace