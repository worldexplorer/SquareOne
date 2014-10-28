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
namespace NDde.Foundation.Client
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;
    using System.Windows.Forms;
    using NDde.Foundation.Advanced;
    using NDde.Properties;

    internal class DdemlClient : IDisposable
    {
        private DdemlContext                      _Context                      = null;                                   // DDEML instance manager
        private int                               _InstanceId                   = 0;                                      // DDEML instance identifier
        private string                            _Service                      = "";                                     // DDEML service name
        private string                            _Topic                        = "";                                     // DDEML topic name
        private IntPtr                            _ConversationHandle           = IntPtr.Zero;                            // DDEML conversation handle
        private bool                              _Paused                       = false;                                  // DDEML callback enabled?
        private IDictionary<int, AsyncResultBase> _AsynchronousTransactionTable = new Dictionary<int, AsyncResultBase>(); // Active DDEML transactions
        private IDictionary<string, AdviseLoop>   _AdviseLoopTable              = new Dictionary<string, AdviseLoop>();   // Active DDEML advise loops
        private bool                              _Disposed                     = false;

        public event EventHandler<DdemlAdviseEventArgs> Advise;

        public event EventHandler<DdemlDisconnectedEventArgs> Disconnected;

        internal event EventHandler StateChange;

        public DdemlClient(string service, string topic) 
            : this(service, topic, DdemlContext.GetDefault())
        {
        }

        public DdemlClient(string service, string topic, DdemlContext context)
        {
            if (service == null)
            {
                throw new ArgumentNullException("service");
            }
            if (service.Length > Ddeml.MAX_STRING_SIZE)
            {
                throw new ArgumentException(Resources.StringParameterInvalidMessage, "service");
            }
            if (topic == null)
            {
                throw new ArgumentNullException("topic");
            }
            if (topic.Length > Ddeml.MAX_STRING_SIZE)
            {
                throw new ArgumentException(Resources.StringParameterInvalidMessage, "topic");
            }
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            _Service = service;
            _Topic = topic;
            _Context = context;
        }

        ~DdemlClient()
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
                    if (IsConnected) 
                    {
                        // Terminate the conversation.
                        ConversationManager.Disconnect(_ConversationHandle);

                        // Assign each active asynchronous transaction an exception so that the EndXXX methods do not deadlock.
                        foreach (AsyncResultBase arb in _AsynchronousTransactionTable.Values)
                        {
                            arb.Process(new DdemlException(Resources.NotConnectedMessage));
                        }

                        // Make sure the asynchronous transaction and advise loop tables are empty.
                        _AsynchronousTransactionTable.Clear();
                        _AdviseLoopTable.Clear();

                        // Unregister this client from the context so that it will not receive DDEML callbacks.
                        _Context.UnregisterClient(this);

                        // Indicate that this object is no longer connected or paused.
                        _Paused = false;
                        _ConversationHandle = IntPtr.Zero;
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
                        
                        // Raise the Disconnected event.
                        foreach (EventHandler<DdemlDisconnectedEventArgs> handler in Disconnected.GetInvocationList())
                        {
                            try 
                            {
                                handler(this, new DdemlDisconnectedEventArgs(false, true));
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
                    if (IsConnected) 
                    {
                        // Terminate the conversation.
                        ConversationManager.Disconnect(_ConversationHandle);
                    }
                }
            }
        }

        public virtual string Service
        {
            get { return _Service; }
        }

        public virtual string Topic
        {
            get { return _Topic; }
        }

        public virtual IntPtr Handle
        {
            get { return _ConversationHandle; }
        }

        public virtual bool IsPaused
        {
            get { return _Paused; }
        }

        public virtual bool IsConnected
        {
            get { return _ConversationHandle != IntPtr.Zero; }
        }

        internal bool IsDisposed
        {
            get { return _Disposed; }
        }

        public virtual void Connect()
        {
            int error = TryConnect();

            if (error == -1)
            {
                throw new ObjectDisposedException(this.GetType().ToString());
            }
            if (error == -2)
            {
                throw new InvalidOperationException(Resources.AlreadyConnectedMessage);
            }
            if (error > Ddeml.DMLERR_NO_ERROR)
            {
                string message = Resources.ConnectFailedMessage;
                message = message.Replace("${service}", _Service);
                message = message.Replace("${topic}", _Topic);
                throw new DdemlException(message, error);
            }
        }

        public virtual int TryConnect()
        {
            if (IsDisposed)
            {
                return -1;
            }
            if (IsConnected)
            {
                return -2;
            }

            // Make sure the context is initialized.
            if (!_Context.IsInitialized)
            {
                _Context.Initialize();
            }

            // Get a local copy of the DDEML instance identifier so that it can be used in the finalizer.
            _InstanceId = _Context.InstanceId;

            // Establish a conversation with a server that supports the service name and topic name pair.
            _ConversationHandle = ConversationManager.Connect(_InstanceId, _Service, _Topic);

            // If the conversation handle is null then the conversation could not be established.
            if (_ConversationHandle == IntPtr.Zero)
            {
                return Ddeml.DdeGetLastError(_InstanceId);
            }

            // Register this client with the context so that it can receive DDEML callbacks.
            _Context.RegisterClient(this);

            // Raise the StateChange event.
            if (StateChange != null)
            {
                StateChange(this, EventArgs.Empty);
            }

            return Ddeml.DMLERR_NO_ERROR;
        }

        public virtual void Disconnect()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(this.GetType().ToString());
            }
            if (!IsConnected)
            {
                throw new InvalidOperationException(Resources.NotConnectedMessage);
            }

            // Terminate the conversation.
            ConversationManager.Disconnect(_ConversationHandle);

            // Assign each active asynchronous transaction an exception so that the EndXXX methods do not deadlock.
            foreach (AsyncResultBase arb in _AsynchronousTransactionTable.Values)
            {
                arb.Process(new DdemlException(Resources.NotConnectedMessage));
            }

            // Make sure the asynchronous transaction and advise loop tables are empty.
            _AsynchronousTransactionTable.Clear();
            _AdviseLoopTable.Clear();

            // Unregister this client from the context so that it will not receive DDEML callbacks.
            _Context.UnregisterClient(this);

            // Indicate that this object is no longer connected or paused.
            _Paused = false;
            _ConversationHandle = IntPtr.Zero;
            _InstanceId = 0;

            // Raise the StateChange event.
            if (StateChange != null)
            {
                StateChange(this, EventArgs.Empty);
            }
            
            // Raise the Disconnected event.
            if (Disconnected != null)
            {
                Disconnected(this, new DdemlDisconnectedEventArgs(false, false));
            }            
        }

        public virtual void Pause()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(this.GetType().ToString());
            }
            if (!IsConnected)
            {
                throw new InvalidOperationException(Resources.NotConnectedMessage);
            }
            if (IsPaused)
            {
                throw new InvalidOperationException(Resources.AlreadyPausedMessage);
            }

            // Disable the DDEML callback.
            bool result = Ddeml.DdeEnableCallback(_InstanceId, _ConversationHandle, Ddeml.EC_DISABLE);

            // Check to see if the DDEML callback was disabled.
            if (!result)
            {
                int error = Ddeml.DdeGetLastError(_InstanceId);
                throw new DdemlException(Resources.ClientPauseFailedMessage, error);
            }

            // The DDEML callback was disabled successfully.
            _Paused = true;

            // Raise the StateChange event.
            if (StateChange != null)
            {
                StateChange(this, EventArgs.Empty);
            }
        }

        public virtual void Resume()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(this.GetType().ToString());
            }
            if (!IsConnected)
            {
                throw new InvalidOperationException(Resources.NotConnectedMessage);
            }
            if (!IsPaused)
            {
                throw new InvalidOperationException(Resources.NotPausedMessage);
            }

            // Enable the DDEML callback.
            bool result = Ddeml.DdeEnableCallback(_InstanceId, _ConversationHandle, Ddeml.EC_ENABLEALL);

            // Check to see if the DDEML callback was enabled.
            if (!result)
            {
                int error = Ddeml.DdeGetLastError(_InstanceId);
                throw new DdemlException(Resources.ClientResumeFailedMessage, error);
            }

            // The DDEML callback was enabled successfully.
            _Paused = false;

            // Raise the StateChange event.
            if (StateChange != null)
            {
                StateChange(this, EventArgs.Empty);
            }
        }

        public virtual void Abandon(IAsyncResult asyncResult)
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(this.GetType().ToString());
            }
            if (!IsConnected)
            {
                throw new InvalidOperationException(Resources.NotConnectedMessage);
            }
            if (!(asyncResult is AsyncResultBase))
            {
                throw new ArgumentException(Resources.AsyncResultParameterInvalidMessage, "asyncResult");
            }

            AsyncResultBase arb = (AsyncResultBase)asyncResult;
            if (!arb.IsCompleted) 
            {
                // Abandon the asynchronous transaction.
                bool result = Ddeml.DdeAbandonTransaction(_InstanceId, _ConversationHandle, arb.TransactionId);

                // Remove the IAsyncResult from the transaction table.
                if (_AsynchronousTransactionTable.ContainsKey(arb.TransactionId))
                {
                    _AsynchronousTransactionTable.Remove(arb.TransactionId);
                }
            }
        }

        public virtual void Execute(string command, int timeout)
        {
            int error = TryExecute(command, timeout);

            if (error == -1)
            {
                throw new ObjectDisposedException(this.GetType().ToString());
            }
            if (error == -2)
            {
                throw new InvalidOperationException(Resources.NotConnectedMessage);
            }
            if (error == -3 && command == null)
            {
                throw new ArgumentNullException("command");
            }
            if (error == -3 && command.Length > Ddeml.MAX_STRING_SIZE)
            {
                throw new ArgumentException(Resources.StringParameterInvalidMessage, "command");
            }
            if (error == -3 && timeout <= 0)
            {
                throw new ArgumentException(Resources.TimeoutParameterInvalidMessage, "timeout");
            }
            if (error > Ddeml.DMLERR_NO_ERROR)
            {
                string message = Resources.ExecuteFailedMessage;
                message = message.Replace("${command}", command);
                throw new DdemlException(message, error);
            }
        }

        public virtual int TryExecute(string command, int timeout)
        {
            if (IsDisposed)
            {
                return -1;
            }
            if (!IsConnected)
            {
                return -2;
            }
            if (command == null)
            {
                return -3;
            }
            if (command.Length > Ddeml.MAX_STRING_SIZE)
            {
                return -3;
            }
            if (timeout <= 0)
            {
                return -3;
            }

            // Convert the command to a byte array with a null terminating character.
            byte[] data = _Context.Encoding.GetBytes(command + "\0");

            // Send the command to the server.
            int returnFlags = 0;
            IntPtr result = Ddeml.DdeClientTransaction(
                data,
                data.Length,
                _ConversationHandle,
                IntPtr.Zero,
                Ddeml.CF_TEXT,
                Ddeml.XTYP_EXECUTE,
                timeout,
                ref returnFlags);

            // If the result is null then the server did not process the command.
            if (result == IntPtr.Zero)
            {
                return Ddeml.DdeGetLastError(_InstanceId);
            }

            return Ddeml.DMLERR_NO_ERROR;
        }

        public virtual IAsyncResult BeginExecute(string command, AsyncCallback callback, object state)
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(this.GetType().ToString());
            }
            if (!IsConnected)
            {
                throw new InvalidOperationException(Resources.NotConnectedMessage);
            }
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }
            if (command.Length > Ddeml.MAX_STRING_SIZE)
            {
                throw new ArgumentException(Resources.StringParameterInvalidMessage, "command");
            }

            // Convert the command to a byte array with a null terminating character.
            byte[] data = _Context.Encoding.GetBytes(command + "\0");
            
            // Send the command to the server.
            int transactionId = 0;
            IntPtr result = Ddeml.DdeClientTransaction(
                data, 
                data.Length, 
                _ConversationHandle, 
                IntPtr.Zero, 
                Ddeml.CF_TEXT,
                Ddeml.XTYP_EXECUTE, 
                Ddeml.TIMEOUT_ASYNC, 
                ref transactionId);

            // If the result is null then the asynchronous operation could not begin.
            if (result == IntPtr.Zero)
            {	
                int error = Ddeml.DdeGetLastError(_InstanceId);
                string message = Resources.ExecuteFailedMessage;
                message = message.Replace("${command}", command);
                throw new DdemlException(message, error);
            }

            // Create an IAsyncResult for this asynchronous operation and add it to the asynchronous transaction table.
            ExecuteAsyncResult ar = new ExecuteAsyncResult(this);
            ar.Command = command;
            ar.Callback = callback;
            ar.AsyncState = state;
            ar.TransactionId = transactionId;
            _AsynchronousTransactionTable.Add(transactionId, ar);

            return ar;
        }

        public virtual void EndExecute(IAsyncResult asyncResult)
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(this.GetType().ToString());
            }
            if (!(asyncResult is ExecuteAsyncResult))
            {
                string message = Resources.AsyncResultParameterInvalidMessage;
                message = message.Replace("${method}", System.Reflection.MethodInfo.GetCurrentMethod().Name);
                throw new ArgumentException(message, "asyncResult");
            }

            ExecuteAsyncResult ar = (ExecuteAsyncResult)asyncResult;
            if (!ar.IsCompleted)
            {
                // WaitOne pumps messages so there is no chance of a deadlock.
                ar.AsyncWaitHandle.WaitOne();
            }
            if (ar.ExceptionObject != null)
            {
                throw ar.ExceptionObject;
            }
        }

        public virtual void Poke(string item, byte[] data, int format, int timeout)
        {
            int error = TryPoke(item, data, format, timeout);

            if (error == -1)
            {
                throw new ObjectDisposedException(this.GetType().ToString());
            }
            if (error == -2)
            {
                throw new InvalidOperationException(Resources.NotConnectedMessage);
            }
            if (error == -3 && data == null)
            {
                throw new ArgumentNullException("data");
            }
            if (error == -3 && item == null)
            {
                throw new ArgumentNullException("item");
            }
            if (error == -3 && item.Length > Ddeml.MAX_STRING_SIZE)
            {
                throw new ArgumentException(Resources.StringParameterInvalidMessage, "item");
            }
            if (error == -3 && timeout <= 0)
            {
                throw new ArgumentException(Resources.TimeoutParameterInvalidMessage, "timeout");
            }
            if (error > Ddeml.DMLERR_NO_ERROR)
            {
                string message = Resources.PokeFailedMessage;
                message = message.Replace("${service}", _Service);
                message = message.Replace("${topic}", _Topic);
                message = message.Replace("${item}", item);
                throw new DdemlException(message, error);
            }
        }

        public virtual int TryPoke(string item, byte[] data, int format, int timeout)
        {
            if (IsDisposed)
            {
                return -1;
            }
            if (!IsConnected)
            {
                return -2;
            }
            if (data == null)
            {
                return -3;
            }
            if (item == null)
            {
                return -3;
            }
            if (item.Length > Ddeml.MAX_STRING_SIZE)
            {
                return -3;
            }
            if (timeout <= 0)
            {
                return -3;
            }

            // Create a string handle for the item name.
            IntPtr itemHandle = Ddeml.DdeCreateStringHandle(_InstanceId, item, Ddeml.CP_WINANSI);

            try
            {
                // Create a data handle for the data being poked.
                IntPtr dataHandle = Ddeml.DdeCreateDataHandle(_InstanceId, data, data.Length, 0, itemHandle, format, 0);

                // If the data handle is null then it could not be created.
                if (dataHandle == IntPtr.Zero)
                {
                    return Ddeml.DdeGetLastError(_InstanceId);
                }

                // Send the data to the server.
                int returnFlags = 0;
                IntPtr result = Ddeml.DdeClientTransaction(
                    dataHandle,
                    -1,
                    _ConversationHandle,
                    itemHandle,
                    format,
                    Ddeml.XTYP_POKE,
                    timeout,
                    ref returnFlags);

                // If the result is null then the server did not process the poke.
                if (result == IntPtr.Zero)
                {
                    return Ddeml.DdeGetLastError(_InstanceId);
                }
            }
            finally
            {
                // Free the string handle created earlier.
                Ddeml.DdeFreeStringHandle(_InstanceId, itemHandle);
            }

            return Ddeml.DMLERR_NO_ERROR;
        }

        public virtual IAsyncResult BeginPoke(string item, byte[] data, int format, AsyncCallback callback, object state)
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(this.GetType().ToString());
            }
            if (!IsConnected)
            {
                throw new InvalidOperationException(Resources.NotConnectedMessage);
            }
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            if (item.Length > Ddeml.MAX_STRING_SIZE)
            {
                throw new ArgumentException(Resources.StringParameterInvalidMessage, "item");
            }

            // Create a string handle for the item name.
            IntPtr itemHandle = Ddeml.DdeCreateStringHandle(_InstanceId, item, Ddeml.CP_WINANSI);

            try
            {
                // Create a data handle for the data being poked.
                IntPtr dataHandle = Ddeml.DdeCreateDataHandle(_InstanceId, data, data.Length, 0, itemHandle, format, 0);
            
                // If the data handle is null then it could not be created.
                if (dataHandle == IntPtr.Zero)
                {
                    int error = Ddeml.DdeGetLastError(_InstanceId);
                    string message = Resources.PokeFailedMessage;
                    message = message.Replace("${service}", _Service);
                    message = message.Replace("${topic}", _Topic);
                    message = message.Replace("${item}", item);
                    throw new DdemlException(message, error);
                }

                // Send the data to the server.
                int transactionId = 0;
                IntPtr result = Ddeml.DdeClientTransaction(
                    dataHandle, 
                    -1, 
                    _ConversationHandle, 
                    itemHandle, 
                    format,
                    Ddeml.XTYP_POKE, 
                    Ddeml.TIMEOUT_ASYNC, 
                    ref transactionId);			

                // If the result is null then the asynchronous operation could not begin.
                if (result == IntPtr.Zero)
                {	
                    int error = Ddeml.DdeGetLastError(_InstanceId);
                    string message = Resources.PokeFailedMessage;
                    message = message.Replace("${service}", _Service);
                    message = message.Replace("${topic}", _Topic);
                    message = message.Replace("${item}", item);
                    throw new DdemlException(message, error);
                }

                // Create an IAsyncResult for the asynchronous operation and add it to the asynchronous transaction table.
                PokeAsyncResult ar = new PokeAsyncResult(this);
                ar.Item = item;
                ar.Format = format;
                ar.Callback = callback;
                ar.AsyncState = state;
                ar.TransactionId = transactionId;
                _AsynchronousTransactionTable.Add(transactionId, ar);

                return ar;
            }
            finally
            {
                // Free the string handle created earlier.
                Ddeml.DdeFreeStringHandle(_InstanceId, itemHandle);				
            }
        }

        public virtual void EndPoke(IAsyncResult asyncResult)
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(this.GetType().ToString());
            }
            if (!(asyncResult is PokeAsyncResult))
            {
                string message = Resources.AsyncResultParameterInvalidMessage;
                message = message.Replace("${method}", System.Reflection.MethodInfo.GetCurrentMethod().Name);
                throw new ArgumentException(message, "asyncResult");
            }

            PokeAsyncResult ar = (PokeAsyncResult)asyncResult;
            if (!ar.IsCompleted)
            {
                // WaitOne pumps messages so there is no chance of a deadlock.
                ar.AsyncWaitHandle.WaitOne();
            }
            if (ar.ExceptionObject != null)
            {
                throw ar.ExceptionObject;
            }
        }

        public virtual byte[] Request(string item, int format, int timeout)
        {
            byte[] data;

            int error = TryRequest(item, format, timeout, out data);

            if (error == -1)
            {
                throw new ObjectDisposedException(this.GetType().ToString());
            }
            if (error == -2)
            {
                throw new InvalidOperationException(Resources.NotConnectedMessage);
            }
            if (error == -3 && item == null)
            {
                throw new ArgumentNullException("item");
            }
            if (error == -3 && item.Length > Ddeml.MAX_STRING_SIZE)
            {
                throw new ArgumentException(Resources.StringParameterInvalidMessage, "item");
            }
            if (error == -3 && timeout <= 0)
            {
                throw new ArgumentException(Resources.TimeoutParameterInvalidMessage, "timeout");
            }
            if (error > Ddeml.DMLERR_NO_ERROR)
            {
                string message = Resources.RequestFailedMessage;
                message = message.Replace("${service}", _Service);
                message = message.Replace("${topic}", _Topic);
                message = message.Replace("${item}", item);
                throw new DdemlException(message, error);
            }

            return data;
        }

        public virtual int TryRequest(string item, int format, int timeout, out byte[] data)
        {
            data = null;

            if (IsDisposed)
            {
                return -1;
            }
            if (!IsConnected)
            {
                return -2;
            }
            if (item == null)
            {
                return -3;
            }
            if (item.Length > Ddeml.MAX_STRING_SIZE)
            {
                return -3;
            }
            if (timeout <= 0)
            {
                return -3;
            }

            // Create a string handle for the item name.
            IntPtr itemHandle = Ddeml.DdeCreateStringHandle(_InstanceId, item, Ddeml.CP_WINANSI);

            // Request the data from the server.
            int returnFlags = 0;
            IntPtr dataHandle = Ddeml.DdeClientTransaction(
                IntPtr.Zero,
                0,
                _ConversationHandle,
                itemHandle,
                format,
                Ddeml.XTYP_REQUEST,
                timeout,
                ref returnFlags);

            // Free the string handle created earlier.
            Ddeml.DdeFreeStringHandle(_InstanceId, itemHandle);

            // If the data handle is null then the server did not process the request.
            if (dataHandle == IntPtr.Zero)
            {
                return Ddeml.DdeGetLastError(_InstanceId);
            }

            // Get the data from the data handle.
            int length = Ddeml.DdeGetData(dataHandle, null, 0, 0);
            data = new byte[length];
            length = Ddeml.DdeGetData(dataHandle, data, data.Length, 0);

            // Free the data handle created by the server.
            Ddeml.DdeFreeDataHandle(dataHandle);

            return Ddeml.DMLERR_NO_ERROR;
        }

        public virtual IAsyncResult BeginRequest(string item, int format, AsyncCallback callback, object state)
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(this.GetType().ToString());
            }
            if (!IsConnected)
            {
                throw new InvalidOperationException(Resources.NotConnectedMessage);
            }
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            if (item.Length > Ddeml.MAX_STRING_SIZE)
            {
                throw new ArgumentException(Resources.StringParameterInvalidMessage, "item");
            }

            // Create a string handle for the item name.
            IntPtr itemHandle = Ddeml.DdeCreateStringHandle(_InstanceId, item, Ddeml.CP_WINANSI);

            // TODO: It might be possible that the request completed synchronously.  
            // Request the data from the server.
            int transactionId = 0;
            IntPtr result = Ddeml.DdeClientTransaction(
                IntPtr.Zero, 
                0, 
                _ConversationHandle, 
                itemHandle, 
                format,
                Ddeml.XTYP_REQUEST, 
                Ddeml.TIMEOUT_ASYNC,
                ref transactionId);

            // Free the string handle created earlier.
            Ddeml.DdeFreeStringHandle(_InstanceId, itemHandle);

            // If the result is null then the asynchronous operation could not begin.
            if (result == IntPtr.Zero)
            {
                int error = Ddeml.DdeGetLastError(_InstanceId);
                string message = Resources.RequestFailedMessage;
                message = message.Replace("${service}", _Service);
                message = message.Replace("${topic}", _Topic);
                message = message.Replace("${item}", item);
                throw new DdemlException(message, error);
            }

            // Create an IAsyncResult for the asynchronous operation and add it to the asynchronous transaction table.
            RequestAsyncResult ar = new RequestAsyncResult(this);
            ar.Item = item;
            ar.Format = format;
            ar.Callback = callback;
            ar.AsyncState = state;
            ar.TransactionId = transactionId;
            _AsynchronousTransactionTable.Add(transactionId, ar);

            return ar;
        }

        public virtual byte[] EndRequest(IAsyncResult asyncResult)
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(this.GetType().ToString());
            }
            if (!(asyncResult is RequestAsyncResult))
            {
                string message = Resources.AsyncResultParameterInvalidMessage;
                message = message.Replace("${method}", System.Reflection.MethodInfo.GetCurrentMethod().Name);
                throw new ArgumentException(message, "asyncResult");
            }

            RequestAsyncResult ar = (RequestAsyncResult)asyncResult;
            if (!ar.IsCompleted)
            {
                // WaitOne pumps messages so there is no chance of a deadlock.
                ar.AsyncWaitHandle.WaitOne();
            }
            if (ar.ExceptionObject != null)
            {
                throw ar.ExceptionObject;
            }

            return ar.Data;
        }

        public virtual void StartAdvise(string item, int format, bool hot, bool acknowledge, int timeout, object adviseState)
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(this.GetType().ToString());
            }
            if (!IsConnected)
            {
                throw new InvalidOperationException(Resources.NotConnectedMessage);
            }
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            if (item.Length > Ddeml.MAX_STRING_SIZE)
            {
                throw new ArgumentException(Resources.StringParameterInvalidMessage, "item");
            }
            if (timeout <= 0)
            {
                throw new ArgumentException(Resources.TimeoutParameterInvalidMessage, "timeout");
            }
            if (_AdviseLoopTable.ContainsKey(item))
            {
                string message = Resources.AlreadyBeingAdvisedMessage;
                message = message.Replace("${service}", _Service);
                message = message.Replace("${topic}", _Topic);
                message = message.Replace("${item}", item);
                throw new InvalidOperationException(message);
            }

            // Create a AdviseLoop object to associate with this advise loop and add it to the advise loop table.
            // The object is added to the advise loop table first because an advisory could come in synchronously during the call
            // DdeClientTransaction.  The assumption is that the advise loop will be initiated successfully.  If it is not then the object must
            // be removed from the advise loop table prior to leaving this method.
            AdviseLoop adviseLoop = new AdviseLoop(this);
            adviseLoop.Item = item;
            adviseLoop.Format = format;
            adviseLoop.State = adviseState;
            _AdviseLoopTable.Add(item, adviseLoop);
            
            // Determine whether the client should acknowledge an advisory before the server posts another.
            bool ack = acknowledge;

            // Create a string handle for the item name.
            IntPtr itemHandle = Ddeml.DdeCreateStringHandle(_InstanceId, item, Ddeml.CP_WINANSI);
        
            // Initiate an advise loop.
            int type = Ddeml.XTYP_ADVSTART;
            type = !hot ? type | Ddeml.XTYPF_NODATA : type;
            type =  ack ? type | Ddeml.XTYPF_ACKREQ : type;
            int returnFlags = 0;
            IntPtr result = Ddeml.DdeClientTransaction(
                IntPtr.Zero, 
                0, 
                _ConversationHandle, 
                itemHandle, 
                format,
                type,
                timeout,
                ref returnFlags);
        
            // Free the string handle created earlier.
            Ddeml.DdeFreeStringHandle(_InstanceId, itemHandle);

            // If the result is null then the server did not initate the advise loop.
            if (result == IntPtr.Zero)
            {
                // Remove the AdviseLoop object created earlier from the advise loop table.  It is no longer valid.
                _AdviseLoopTable.Remove(item);

                int error = Ddeml.DdeGetLastError(_InstanceId);
                string message = Resources.StartAdviseFailedMessage;
                message = message.Replace("${service}", _Service);
                message = message.Replace("${topic}", _Topic);
                message = message.Replace("${item}", item);
                throw new DdemlException(message, error);
            }
        }

        public virtual IAsyncResult BeginStartAdvise(string item, int format, bool hot, bool acknowledge, AsyncCallback callback, object asyncState, object adviseState)
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(this.GetType().ToString());
            }
            if (!IsConnected)
            {
                throw new InvalidOperationException(Resources.NotConnectedMessage);
            }
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            if (item.Length > Ddeml.MAX_STRING_SIZE)
            {
                throw new ArgumentException(Resources.StringParameterInvalidMessage, "item");
            }
            if (_AdviseLoopTable.ContainsKey(item))
            {
                string message = Resources.AlreadyBeingAdvisedMessage;
                message = message.Replace("${service}", _Service);
                message = message.Replace("${topic}", _Topic);
                message = message.Replace("${item}", item);
                throw new InvalidOperationException(message);
            }

            // Determine whether the client should acknowledge an advisory before the server posts another.
            bool ack = acknowledge;

            // Create a string handle for the item name.
            IntPtr itemHandle = Ddeml.DdeCreateStringHandle(_InstanceId, item, Ddeml.CP_WINANSI);
        
            // Initiate an advise loop.
            int type = Ddeml.XTYP_ADVSTART;
            type = !hot ? type | Ddeml.XTYPF_NODATA : type;
            type =  ack ? type | Ddeml.XTYPF_ACKREQ : type;
            int transactionId = 0;
            IntPtr result = Ddeml.DdeClientTransaction(
                IntPtr.Zero, 
                0, 
                _ConversationHandle, 
                itemHandle, 
                format,
                type,
                Ddeml.TIMEOUT_ASYNC,
                ref transactionId);
        
            // Free the string handle created earlier.
            Ddeml.DdeFreeStringHandle(_InstanceId, itemHandle);

            // If the result is null then the asynchronous operation could not begin.
            if (result == IntPtr.Zero)
            {
                int error = Ddeml.DdeGetLastError(_InstanceId);
                string message = Resources.StartAdviseFailedMessage;
                message = message.Replace("${service}", _Service);
                message = message.Replace("${topic}", _Topic);
                message = message.Replace("${item}", item);
                throw new DdemlException(message, error);
            }

            // Create an IAsyncResult for the asynchronous operation and add it to the asynchronous transaction table.
            StartAdviseAsyncResult ar = new StartAdviseAsyncResult(this);
            ar.Item = item;
            ar.Format = format;
            ar.State = adviseState;
            ar.Callback = callback;
            ar.AsyncState = asyncState;
            ar.TransactionId = transactionId;
            _AsynchronousTransactionTable.Add(transactionId, ar);

            return ar;
        }

        public virtual void EndStartAdvise(IAsyncResult asyncResult)
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(this.GetType().ToString());
            }
            if (!(asyncResult is StartAdviseAsyncResult))
            {
                string message = Resources.AsyncResultParameterInvalidMessage;
                message = message.Replace("${method}", System.Reflection.MethodInfo.GetCurrentMethod().Name);
                throw new ArgumentException(message, "asyncResult");
            }

            StartAdviseAsyncResult ar = (StartAdviseAsyncResult)asyncResult;
            if (!ar.IsCompleted)
            {
                // WaitOne pumps messages so there is no chance of a deadlock.
                ar.AsyncWaitHandle.WaitOne();
            }
            if (ar.ExceptionObject != null)
            {
                throw ar.ExceptionObject;
            }
        }

        public virtual void StopAdvise(string item, int timeout)
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(this.GetType().ToString());
            }
            if (!IsConnected)
            {
                throw new InvalidOperationException(Resources.NotConnectedMessage);
            }
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            if (item.Length > Ddeml.MAX_STRING_SIZE)
            {
                throw new ArgumentException(Resources.StringParameterInvalidMessage, "item");
            }
            if (timeout <= 0)
            {
                throw new ArgumentException(Resources.TimeoutParameterInvalidMessage, "timeout");
            }
            if (!_AdviseLoopTable.ContainsKey(item))
            {
                string message = Resources.NotBeingAdvisedMessage;
                message = message.Replace("${service}", _Service);
                message = message.Replace("${topic}", _Topic);
                message = message.Replace("${item}", item);
                throw new InvalidOperationException(message);
            }

            // Get the advise loop object from the advise loop table.
            AdviseLoop adviseLoop = _AdviseLoopTable[item];

            // Create a string handle for the item name.
            IntPtr itemHandle = Ddeml.DdeCreateStringHandle(_InstanceId, item, Ddeml.CP_WINANSI);

            // Terminate the advise loop.
            int returnFlags = 0;
            IntPtr result = Ddeml.DdeClientTransaction(
                IntPtr.Zero, 
                0, 
                _ConversationHandle, 
                itemHandle, 
                adviseLoop.Format,
                Ddeml.XTYP_ADVSTOP,
                timeout,
                ref returnFlags);
            
            // Free the string handle created earlier.
            Ddeml.DdeFreeStringHandle(_InstanceId, itemHandle);
            
            // If the result is null then the server could not terminate the advise loop.
            if (result == IntPtr.Zero)
            {
                int error = Ddeml.DdeGetLastError(_InstanceId);
                string message = Resources.StopAdviseFailedMessage;
                message = message.Replace("${service}", _Service);
                message = message.Replace("${topic}", _Topic);
                message = message.Replace("${item}", item);
                throw new DdemlException(message, error);
            }

            // Remove the advise loop object from the advise loop table.
            _AdviseLoopTable.Remove(item);
        }

        public virtual IAsyncResult BeginStopAdvise(string item, AsyncCallback callback, object state)
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(this.GetType().ToString());
            }
            if (!IsConnected)
            {
                throw new InvalidOperationException(Resources.NotConnectedMessage);
            }
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            if (item.Length > Ddeml.MAX_STRING_SIZE)
            {
                throw new ArgumentException(Resources.StringParameterInvalidMessage, "item");
            }
            if (!_AdviseLoopTable.ContainsKey(item))
            {
                string message = Resources.NotBeingAdvisedMessage;
                message = message.Replace("${service}", _Service);
                message = message.Replace("${topic}", _Topic);
                message = message.Replace("${item}", item);
                throw new InvalidOperationException(message);
            }
            
            // Get the advise object from the advise loop table.
            AdviseLoop adviseLoop = _AdviseLoopTable[item];

            // Create a string handle for the item name.
            IntPtr itemHandle = Ddeml.DdeCreateStringHandle(_InstanceId, item, Ddeml.CP_WINANSI);
            
            // Terminate the advise loop.
            int transactionId = 0;
            IntPtr result = Ddeml.DdeClientTransaction(
                IntPtr.Zero, 
                0, 
                _ConversationHandle, 
                itemHandle, 
                adviseLoop.Format,
                Ddeml.XTYP_ADVSTOP,
                Ddeml.TIMEOUT_ASYNC,
                ref transactionId);
        
            // Free the string handle created earlier.
            Ddeml.DdeFreeStringHandle(_InstanceId, itemHandle);

            // If the result is null then the asynchronous operation could not begin.
            if (result == IntPtr.Zero)
            {
                int error = Ddeml.DdeGetLastError(_InstanceId);
                string message = Resources.StopAdviseFailedMessage;
                message = message.Replace("${service}", _Service);
                message = message.Replace("${topic}", _Topic);
                message = message.Replace("${item}", item);
                throw new DdemlException(message, error);
            }

            // Create an IAsyncResult for the asyncronous operation and add it to the asynchronous transaction table.
            StopAdviseAsyncResult ar = new StopAdviseAsyncResult(this);
            ar.Item = item;
            ar.Format = adviseLoop.Format;
            ar.Callback = callback;
            ar.AsyncState = state;
            ar.TransactionId = transactionId;
            _AsynchronousTransactionTable.Add(transactionId, ar);

            return ar;
        }

        public virtual void EndStopAdvise(IAsyncResult asyncResult)
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(this.GetType().ToString());
            }
            if (!(asyncResult is StopAdviseAsyncResult))
            {
                string message = Resources.AsyncResultParameterInvalidMessage;
                message = message.Replace("${method}", System.Reflection.MethodInfo.GetCurrentMethod().Name);
                throw new ArgumentException(message, "asyncResult");
            }

            StopAdviseAsyncResult ar = (StopAdviseAsyncResult)asyncResult;
            if (!ar.IsCompleted)
            {
                // WaitOne pumps messages so there is no chance of a deadlock.
                ar.AsyncWaitHandle.WaitOne();
            }
            if (ar.ExceptionObject != null)
            {
                throw ar.ExceptionObject;
            }
        }

        internal bool ProcessCallback(DdemlTransaction transaction)
        {
            // This is here to alias the transaction object with a shorter variable name.
            DdemlTransaction t = transaction;
            
            switch (t.uType)
            {
                case Ddeml.XTYP_ADVDATA:
                {
                    // Get the item name from the hsz2 string handle.
                    StringBuilder psz = new StringBuilder(Ddeml.MAX_STRING_SIZE);
                    int length = Ddeml.DdeQueryString(_InstanceId, t.hsz2, psz, psz.Capacity, Ddeml.CP_WINANSI);
                    string item = psz.ToString();

                    // Delegate processing to the advise loop object.
                    if (_AdviseLoopTable.ContainsKey(item))
                    {
                        t.dwRet = _AdviseLoopTable[item].Process(t.uType, t.uFmt, t.hConv, t.hsz1, t.hsz2, t.hData, t.dwData1, t.dwData2);
                        return true;
                    }

                    // This transaction could not be processed here.
                    return false;
                }
                case Ddeml.XTYP_XACT_COMPLETE:
                {
                    // Get the transaction identifier from dwData1.
                    int transactionId = t.dwData1.ToInt32();

                    // Get the IAsyncResult from the asynchronous transaction table and delegate processing to it.
                    if (_AsynchronousTransactionTable.ContainsKey(transactionId))
                    {
                        AsyncResultBase arb = _AsynchronousTransactionTable[transactionId];

                        // Remove the IAsyncResult from the asynchronous transaction table.
                        _AsynchronousTransactionTable.Remove(arb.TransactionId);

                        t.dwRet = arb.Process(t.uType, t.uFmt, t.hConv, t.hsz1, t.hsz2, t.hData, t.dwData1, t.dwData2);
                        return true;
                    }
                    
                    // This transaction could not be processed here.
                    return false;
                }
                case Ddeml.XTYP_DISCONNECT:
                {
                    // Assign each active asynchronous transaction an exception so that the EndXXX methods do not deadlock.
                    foreach (AsyncResultBase arb in _AsynchronousTransactionTable.Values)
                    {
                        arb.Process(new DdemlException(Resources.NotConnectedMessage));
                    }

                    // Make sure the asynchronous transaction and advise loop tables are empty.
                    _AsynchronousTransactionTable.Clear();
                    _AdviseLoopTable.Clear();

                    // Unregister this client from the context so that it will not receive DDEML callbacks.
                    _Context.UnregisterClient(this);

                    // Indicate that this object is no longer connected or paused.
                    _Paused = false;
                    _ConversationHandle = IntPtr.Zero;
                    _InstanceId = 0;

                    // Raise the StateChange event.
                    if (StateChange != null)
                    {
                        StateChange(this, EventArgs.Empty);
                    }

                    // Raise the Disconnected event.
                    if (Disconnected != null)
                    {
                        Disconnected(this, new DdemlDisconnectedEventArgs(true, false));
                    }

                    // Return zero to indicate that there are no problems.
                    t.dwRet = IntPtr.Zero;
                    return true;
                }
            }
            
            // This transaction could not be processed here.
            return false;
        }

        /// <summary>
        /// This class is needed to dispose of DDEML resources correctly since the DDEML is thread specific.
        /// </summary>
        private sealed class ConversationManager : IMessageFilter
        {
            private const int WM_APP = unchecked((int)0x8000);

            private static readonly string DataSlot = typeof(ConversationManager).FullName;

            private static IDictionary<IntPtr, int> _Table = new Dictionary<IntPtr, int>();

            [System.Runtime.InteropServices.DllImport("user32.dll")]
            private static extern void PostThreadMessage(int idThread, int Msg, IntPtr wParam, IntPtr lParam);

            public static IntPtr Connect(int instanceId, string service, string topic)
            {
                lock (_Table) 
                {
                    // Create string handles for the service name and topic name.
                    IntPtr serviceHandle = Ddeml.DdeCreateStringHandle(instanceId, service, Ddeml.CP_WINANSI);
                    IntPtr topicHandle = Ddeml.DdeCreateStringHandle(instanceId, topic, Ddeml.CP_WINANSI);

                    // Establish a conversation with a server that suppoerts the service name and topic name pair.
                    IntPtr handle = Ddeml.DdeConnect(instanceId, serviceHandle, topicHandle, IntPtr.Zero);

                    // Free the string handles that were created earlier.
                    Ddeml.DdeFreeStringHandle(instanceId, topicHandle);
                    Ddeml.DdeFreeStringHandle(instanceId, serviceHandle);

                    if (handle != IntPtr.Zero)
                    {
                        // Make sure this thread has an IMessageFilter on it.
                        LocalDataStoreSlot slot = Thread.GetNamedDataSlot(DataSlot);
                        if (Thread.GetData(slot) == null) 
                        {
                            ConversationManager filter = new ConversationManager();
                            Application.AddMessageFilter(filter);
                            Thread.SetData(slot, filter);
                        }

                        // Add an entry to the table that maps the conversation handle to the current thread.
                        _Table.Add(handle, Ddeml.GetCurrentThreadId());
                    }
                    return handle;
                }
            }

            public static void Disconnect(IntPtr conversationHandle)
            {
                // This method could be called by the GC finalizer thread.  If it is then a direct call to the DDEML will fail since the DDEML is 
                // thread specific.  A message will be posted to the DDEML thread instead.
                lock (_Table) 
                {
                    if (_Table.ContainsKey(conversationHandle))
                    {
                        // Determine if the current thread matches what is in the table.
                        int threadId = (int)_Table[conversationHandle];
                        if (threadId == Ddeml.GetCurrentThreadId())
                        {
                            // Terminate the conversation.
                            Ddeml.DdeDisconnect(conversationHandle);
                        }
                        else
                        {
                            // Post a message to the thread that needs to execute Ddeml.DdeDisconnect.
                            PostThreadMessage(threadId, WM_APP + 2, conversationHandle, IntPtr.Zero);
                        }

                        // Remove the conversation handle from the table because it is no longer in use.
                        _Table.Remove(conversationHandle);
                    }
                }
            }

            bool IMessageFilter.PreFilterMessage(ref Message m)
            {
                if (m.Msg == WM_APP + 2)
                {
                    // Terminate the conversation.
                    Ddeml.DdeDisconnect(m.WParam);
                }
                return false;
            }

        } // class

        private sealed class AdviseLoop
        {
            private string      _Item   = "";
            private int         _Format = 0;
            private DdemlClient _Client = null;
            private object      _State  = null;

            public AdviseLoop(DdemlClient client)
            {
                _Client = client;
            }

            public string Item
            {
                get { return _Item; }
                set { _Item = value; }
            }

            public int Format
            {
                get { return _Format; }
                set { _Format = value; }
            }

            public object State
            {
                get { return _State; }
                set { _State = value; }
            }

            public IntPtr Process(int uType, int uFmt, IntPtr hConv, IntPtr hsz1, IntPtr hsz2, IntPtr hData, IntPtr dwData1, IntPtr dwData2)
            {
                if (_Client.Advise != null) 
                {
                    // Assume this is a warm advise (XTYPF_NODATA).
                    byte[] data = null;

                    // If the data handle is not null then it is a hot advise.
                    if (hData != IntPtr.Zero) 
                    {
                        // Get the data from the data handle.
                        int length = Ddeml.DdeGetData(hData, null, 0, 0);
                        data = new byte[length];
                        length = Ddeml.DdeGetData(hData, data, data.Length, 0);
                    }

                    // Raise the Advise event.
                    _Client.Advise(_Client, new DdemlAdviseEventArgs(_Item, _Format, _State, data));
                }

                // Return DDE_FACK to indicate that are no problems.
                return new IntPtr(Ddeml.DDE_FACK);
            }

        } // class

        private abstract class AsyncResultBase : IAsyncResult
        {
            private object           _State           = null;
            private ManualResetEvent _CompletionEvent = new ManualResetEvent(false);
            private bool             _IsCompleted     = false;
            private AsyncCallback    _Callback        = null;
            private DdemlClient      _Client          = null;
            private int              _TransactionId   = 0;
            private Exception        _Exception       = null;

            public AsyncResultBase(DdemlClient client)
            {
                _Client = client;
            }

            public object AsyncState
            {
                get { return _State; }
                set { _State = value; }
            }

            public WaitHandle AsyncWaitHandle
            {
                get { return _CompletionEvent; }
            }

            public bool CompletedSynchronously
            {
                get { return false; }
            }

            public bool IsCompleted
            {
                get { return _IsCompleted; }
            }

            public AsyncCallback Callback
            {
                get { return _Callback; }
                set { _Callback = value; }
            }

            public DdemlClient Client
            {
                get { return _Client; }
            }

            public int TransactionId
            {
                get { return _TransactionId; }
                set { _TransactionId = value; }
            }

            public Exception ExceptionObject
            {
                get { return _Exception; }
                set { _Exception = value; }
            }

            public void Process(Exception exception)
            {
                _Exception = exception;

                // Mark this IAsyncResult as complete and invoke the callback.
                _IsCompleted = true;
                _CompletionEvent.Set();
                if (_Callback != null)
                {
                    _Callback(this);
                }
            }

            public IntPtr Process(int uType, int uFmt, IntPtr hConv, IntPtr hsz1, IntPtr hsz2, IntPtr hData, IntPtr dwData1, IntPtr dwData2)
            {
                // Delegate processing to the concrete class.
                IntPtr returnValue = ProcessCallback(uType, uFmt, hConv, hsz1, hsz2, hData, dwData1, dwData2);

                // Mark this IAsyncResult as complete and invoke the callback.
                _IsCompleted = true;
                _CompletionEvent.Set();
                if (_Callback != null)
                {
                    _Callback(this);
                }

                // The return value is sent to the DDEML.
                return returnValue;
            }

            protected virtual IntPtr ProcessCallback(
                int uType, int uFmt, IntPtr hConv, IntPtr hsz1, IntPtr hsz2, IntPtr hData, IntPtr dwData1, IntPtr dwData2)
            {
                // The default implementation will return zero to the DDEML.
                return IntPtr.Zero;
            }

        } // class

        private sealed class ExecuteAsyncResult : AsyncResultBase
        {
            private string _Command = "";

            public ExecuteAsyncResult(DdemlClient client) : base(client)
            {
            }

            public string Command
            {
                get { return _Command; }
                set { _Command = value; }
            }
            
            protected override IntPtr ProcessCallback(
                int uType, int uFmt, IntPtr hConv, IntPtr hsz1, IntPtr hsz2, IntPtr hData, IntPtr dwData1, IntPtr dwData2)
            {
                // If the data handle is null then the server did not process the command.
                if (hData == IntPtr.Zero)
                {
                    string message = Resources.ExecuteFailedMessage;
                    message = message.Replace("${command}", _Command);
                    this.ExceptionObject = new DdemlException(message);
                }

                // Return zero to indicate that there are no problems.
                return IntPtr.Zero;
            }

        } // class

        private sealed class PokeAsyncResult : AsyncResultBase
        {
            private string _Item   = "";
            private int    _Format = 0;

            public PokeAsyncResult(DdemlClient client) : base(client)
            {
            }
            
            public string Item
            {
                get { return _Item; }
                set { _Item = value; }
            }

            public int Format
            {
                get { return _Format; }
                set { _Format = value; }
            }

            protected override IntPtr ProcessCallback(
                int uType, int uFmt, IntPtr hConv, IntPtr hsz1, IntPtr hsz2, IntPtr hData, IntPtr dwData1, IntPtr dwData2)
            {
                // If the data handle is null then the server did not process the poke.
                if (hData == IntPtr.Zero)
                {
                    string message = Resources.PokeFailedMessage;
                    message = message.Replace("${service}", this.Client._Service);
                    message = message.Replace("${topic}", this.Client._Topic);
                    message = message.Replace("${item}", _Item);
                    this.ExceptionObject = new DdemlException(message);
                }

                // Return zero to indicate that there are no problems.
                return IntPtr.Zero;
            }

        } // class

        private sealed class RequestAsyncResult : AsyncResultBase
        {
            private string _Item   = "";
            private int    _Format = 0;
            private byte[] _Data   = null;

            public RequestAsyncResult(DdemlClient client) : base(client)
            {
            }
            
            public byte[] Data
            {
                get { return _Data; }
                set { _Data = value; }
            }
    
            public string Item
            {
                get { return _Item; }
                set { _Item = value; }
            }

            public int Format
            {
                get { return _Format; }
                set { _Format = value; }
            }

            protected override IntPtr ProcessCallback(
                int uType, int uFmt, IntPtr hConv, IntPtr hsz1, IntPtr hsz2, IntPtr hData, IntPtr dwData1, IntPtr dwData2)
            {
                // If the data handle is null then the server did not process the request.
                // TODO: Some servers may process the request, but return null anyway?
                if (hData == IntPtr.Zero)
                {
                    string message = Resources.RequestFailedMessage;
                    message = message.Replace("${service}", this.Client._Service);
                    message = message.Replace("${topic}", this.Client._Topic);
                    message = message.Replace("${item}", _Item);
                    this.ExceptionObject = new DdemlException(message);
                }
                else 
                {
                    // Get the data from the data handle.
                    int length = Ddeml.DdeGetData(hData, null, 0, 0);
                    _Data = new byte[length];
                    length = Ddeml.DdeGetData(hData, _Data, _Data.Length, 0);
                }

                // Return zero to indicate that there are no problems.
                return IntPtr.Zero;
            }

        } // class

        private sealed class StartAdviseAsyncResult : AsyncResultBase
        {
            private string _Item   = "";
            private int    _Format = 0;
            private object _State  = null;

            public StartAdviseAsyncResult(DdemlClient client) : base(client)
            {
            }
            
            public string Item
            {
                get { return _Item; }
                set { _Item = value; }
            }

            public int Format
            {
                get { return _Format; }
                set { _Format = value; }
            }

            public object State
            {
                get { return _State; }
                set { _State = value; }
            }

            protected override IntPtr ProcessCallback(
                int uType, int uFmt, IntPtr hConv, IntPtr hsz1, IntPtr hsz2, IntPtr hData, IntPtr dwData1, IntPtr dwData2)
            {
                // If the data handle is null then the server did not initiate the advise loop.
                if (hData == IntPtr.Zero)
                {
                    string message = Resources.StartAdviseFailedMessage;
                    message = message.Replace("${service}", this.Client._Service);
                    message = message.Replace("${topic}", this.Client._Topic);
                    message = message.Replace("${item}", _Item);
                    this.ExceptionObject = new DdemlException(message);
                }
                else
                {
                    // Create a AdviseLoop object to associate with this advise loop and add it to the owner's advise loop table.
                    AdviseLoop adviseLoop = new AdviseLoop(this.Client);
                    adviseLoop.Item = _Item;
                    adviseLoop.Format = _Format;
                    adviseLoop.State = _State;
                    this.Client._AdviseLoopTable.Add(_Item, adviseLoop);
                }

                // Return zero to indicate that there are no problems.
                return IntPtr.Zero;
            }

        } // class

        private sealed class StopAdviseAsyncResult : AsyncResultBase
        {
            private string _Item   = "";
            private int    _Format = 0;

            public StopAdviseAsyncResult(DdemlClient client) : base(client)
            {
            }
            
            public string Item
            {
                get { return _Item; }
                set { _Item = value; }
            }

            public int Format
            {
                get { return _Format; }
                set { _Format = value; }
            }

            protected override IntPtr ProcessCallback(
                int uType, int uFmt, IntPtr hConv, IntPtr hsz1, IntPtr hsz2, IntPtr hData, IntPtr dwData1, IntPtr dwData2)
            {
                // If the data handle is null then the server could not terminate the advise loop.
                if (hData == IntPtr.Zero)
                {
                    string message = Resources.StopAdviseFailedMessage;
                    message = message.Replace("${service}", this.Client._Service);
                    message = message.Replace("${topic}", this.Client._Topic);
                    message = message.Replace("${item}", _Item);
                    this.ExceptionObject = new DdemlException(message);
                }
                else
                {
                    // Remove the advise object from the owner's advise loop table.
                    this.Client._AdviseLoopTable.Remove(_Item);
                }

                // Return zero to indicate that there are no problems.
                return IntPtr.Zero;
            }

        } // class

    } // class

} // namespace