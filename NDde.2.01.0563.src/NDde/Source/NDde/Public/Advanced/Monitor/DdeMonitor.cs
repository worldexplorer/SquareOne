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
namespace NDde.Advanced.Monitor
{
    using System;
    using System.Threading;
    using NDde.Foundation;
    using NDde.Foundation.Advanced.Monitor;
    using NDde.Properties;

    /// <summary>
    /// This specifies the different kinds of DDE activity that can be monitored.
    /// </summary>
    [Flags]
    public enum DdeMonitorFlags
    {
        /// <summary>
        /// This indicates activity caused by the execution of a DDEML callback.
        /// </summary>
        Callback = DdemlMonitorFlags.Callback,

        /// <summary>
        /// This indicates activity caused by conversation.
        /// </summary>
        Conversation = DdemlMonitorFlags.Conversation,

        /// <summary>
        /// This indicates activity caused by an error.
        /// </summary>
        Error = DdemlMonitorFlags.Error,

        ///// <summary>
        ///// 
        ///// </summary>
        //String = DdemlMonitorFlags.String,

        /// <summary>
        /// This indicates activity caused by an advise loop.
        /// </summary>
        Link = DdemlMonitorFlags.Link,

        /// <summary>
        /// This indicates activity caused by DDE messages.
        /// </summary>
        Message = DdemlMonitorFlags.Message,

    } // enum

    /// <summary>
    /// This is used to monitor DDE activity.
    /// </summary>
    public sealed class DdeMonitor : IDisposable
    {
        private Object _LockObject = new Object();

        private DdemlMonitor _DdemlObject = null; // This has lazy initialization through a property.
        private DdeContext   _Context     = null;
        
        private event EventHandler<DdeCallbackActivityEventArgs>     _CallbackActivityEvent     = null;
        private event EventHandler<DdeConversationActivityEventArgs> _ConversationActivityEvent = null;
        private event EventHandler<DdeErrorActivityEventArgs>        _ErrorActivityEvent        = null;
        private event EventHandler<DdeLinkActivityEventArgs>         _LinkActivityEvent         = null;
        private event EventHandler<DdeMessageActivityEventArgs>      _MessageActivityEvent      = null;
        //private event EventHandler<DdeStringActivityEventArgs>       _StringActivityEvent       = null;

        /// <summary>
        /// This is raised anytime a DDEML callback is executed.
        /// </summary>
        public event EventHandler<DdeCallbackActivityEventArgs> CallbackActivity
        {
            add
            {
                lock (_LockObject)
                {
                    _CallbackActivityEvent += value;
                }
            }
            remove
            {
                lock (_LockObject)
                {
                    _CallbackActivityEvent -= value;
                }
            }
        }

        /// <summary>
        /// This is raised anytime a conversation is established or terminated.
        /// </summary>
        public event EventHandler<DdeConversationActivityEventArgs> ConversationActivity
        {
            add
            {
                lock (_LockObject)
                {
                    _ConversationActivityEvent += value;
                }
            }
            remove
            {
                lock (_LockObject)
                {
                    _ConversationActivityEvent -= value;
                }
            }
        }

        /// <summary>
        /// This is raised anytime there is an error.
        /// </summary>
        public event EventHandler<DdeErrorActivityEventArgs> ErrorActivity
        {
            add
            {
                lock (_LockObject)
                {
                    _ErrorActivityEvent += value;
                }
            }
            remove
            {
                lock (_LockObject)
                {
                    _ErrorActivityEvent -= value;
                }
            }
        }

        /// <summary>
        /// This is raised anytime an advise loop is established or terminated.
        /// </summary>
        public event EventHandler<DdeLinkActivityEventArgs> LinkActivity
        {
            add
            {
                lock (_LockObject)
                {
                    _LinkActivityEvent += value;
                }
            }
            remove
            {
                lock (_LockObject)
                {
                    _LinkActivityEvent -= value;
                }
            }
        }

        /// <summary>
        /// This is raised anytime a DDE message is sent or posted.
        /// </summary>
        public event EventHandler<DdeMessageActivityEventArgs> MessageActivity
        {
            add
            {
                lock (_LockObject)
                {
                    _MessageActivityEvent += value;
                }
            }
            remove
            {
                lock (_LockObject)
                {
                    _MessageActivityEvent -= value;
                }
            }
        }

        ///// <summary>
        ///// 
        ///// </summary>
        //public event EventHandler<DdeStringActivityEventArgs> StringActivity
        //{
        //    add
        //    {
        //        lock (_LockObject)
        //        {
        //            _StringActivityEvent += value;
        //        }
        //    }
        //    remove
        //    {
        //        lock (_LockObject)
        //        {
        //            _StringActivityEvent -= value;
        //        }
        //    }
        //}

        /// <summary>
        /// This initializes a new instance of the <c>DdeMonitor</c> class.
        /// </summary>
        public DdeMonitor()
        {
            Context = new DdeContext();
        }

        /// <summary>
        /// This releases all resources held by this instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                ThreadStart method = delegate()
                {
                    DdemlObject.Dispose();
                };

                try
                {
                    Context.Invoke(method);
                }
                catch
                {
                    // Swallow any exception that occurs.
                }
            }
        }

        /// <summary>
        /// This gets the context associated with this instance.
        /// </summary>
        public DdeContext Context
        {
            get
            {
                lock (_LockObject)
                {
                    return _Context;
                }
            }
            private set
            {
                lock (_LockObject)
                {
                    _Context = value;
                }
            }
        }

        /// <summary>
        /// This starts monitoring the system for DDE activity.
        /// </summary>
        /// <param name="flags">
        /// A bitwise combination of <c>DdeMonitorFlags</c> that indicate what DDE activity will be monitored.
        /// </param>
        public void Start(DdeMonitorFlags flags)
        {
            ThreadStart method = delegate()
            {
                DdemlObject.Start((DdemlMonitorFlags)(int)flags);
            };

            try
            {
                Context.Invoke(method);
            }
            catch (DdemlException e)
            {
                throw new DdeException(e);
            }
            catch (ObjectDisposedException e)
            {
                throw new ObjectDisposedException(this.GetType().ToString(), e);
            }
        }

        internal DdemlMonitor DdemlObject
        {
            get
            {
                lock (_LockObject)
                {
                    if (_DdemlObject == null)
                    {
                        _DdemlObject = new DdemlMonitor(Context.DdemlObject);
                        _DdemlObject.CallbackActivity += new EventHandler<DdemlCallbackActivityEventArgs>(this.OnCallbackActivity);
                        _DdemlObject.ConversationActivity += new EventHandler<DdemlConversationActivityEventArgs>(this.OnConversationActivity);
                        _DdemlObject.ErrorActivity += new EventHandler<DdemlErrorActivityEventArgs>(this.OnErrorActivity);
                        _DdemlObject.LinkActivity += new EventHandler<DdemlLinkActivityEventArgs>(this.OnLinkActivity);
                        _DdemlObject.MessageActivity += new EventHandler<DdemlMessageActivityEventArgs>(this.OnMessageActivity);
                        //_DdemlObject.StringActivity += new EventHandler<DdemlStringActivityEventArgs>(this.OnStringActivity);
                    }
                    return _DdemlObject;
                }
            }
        }

        //private void OnStringActivity(object sender, DdemlStringActivityEventArgs e)
        //{
        //    EventHandler<DdeStringActivityEventArgs> copy;

        //    // To make this thread-safe we need to hold a local copy of the reference to the invocation list.  This works because delegates are
        //    //immutable.
        //    lock (_LockObject)
        //    {
        //        copy = _StringActivityEvent;
        //    }

        //    if (copy != null)
        //    {
        //        copy(this, new DdeStringActivityEventArgs(e));
        //    }
        //}

        private void OnMessageActivity(object sender, DdemlMessageActivityEventArgs e)
        {
            EventHandler<DdeMessageActivityEventArgs> copy;

            // To make this thread-safe we need to hold a local copy of the reference to the invocation list.  This works because delegates are
            //immutable.
            lock (_LockObject)
            {
                copy = _MessageActivityEvent;
            }

            if (copy != null)
            {
                copy(this, new DdeMessageActivityEventArgs(e));
            }
        }

        private void OnLinkActivity(object sender, DdemlLinkActivityEventArgs e)
        {
            EventHandler<DdeLinkActivityEventArgs> copy;

            // To make this thread-safe we need to hold a local copy of the reference to the invocation list.  This works because delegates are
            //immutable.
            lock (_LockObject)
            {
                copy = _LinkActivityEvent;
            }

            if (copy != null)
            {
                copy(this, new DdeLinkActivityEventArgs(e));
            }
        }

        private void OnErrorActivity(object sender, DdemlErrorActivityEventArgs e)
        {
            EventHandler<DdeErrorActivityEventArgs> copy;

            // To make this thread-safe we need to hold a local copy of the reference to the invocation list.  This works because delegates are
            //immutable.
            lock (_LockObject)
            {
                copy = _ErrorActivityEvent;
            }

            if (copy != null)
            {
                copy(this, new DdeErrorActivityEventArgs(e));
            }
        }

        private void OnConversationActivity(object sender, DdemlConversationActivityEventArgs e)
        {
            EventHandler<DdeConversationActivityEventArgs> copy;

            // To make this thread-safe we need to hold a local copy of the reference to the invocation list.  This works because delegates are
            //immutable.
            lock (_LockObject)
            {
                copy = _ConversationActivityEvent;
            }

            if (copy != null)
            {
                copy(this, new DdeConversationActivityEventArgs(e));
            }
        }

        private void OnCallbackActivity(object sender, DdemlCallbackActivityEventArgs e)
        {
            EventHandler<DdeCallbackActivityEventArgs> copy;

            // To make this thread-safe we need to hold a local copy of the reference to the invocation list.  This works because delegates are
            //immutable.
            lock (_LockObject)
            {
                copy = _CallbackActivityEvent;
            }

            if (copy != null)
            {
                copy(this, new DdeCallbackActivityEventArgs(e));
            }
        }

    } // class

} // namespace