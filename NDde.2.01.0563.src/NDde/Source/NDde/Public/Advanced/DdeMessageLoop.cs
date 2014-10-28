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
namespace NDde.Advanced
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Windows.Forms;

    /// <summary>
    /// This is a synchronizing object that can run a message loop on any thread.
    /// </summary>
    /// <threadsafety static="true" instance="false" />
    public sealed class DdeMessageLoop : IDisposable, ISynchronizeInvoke
    {
        [DllImport("kernel32.dll")]
        private static extern int GetCurrentThreadId();

        private int  _ThreadId = GetCurrentThreadId();
        private Form _Form     = new HiddenForm();

        /// <summary>
        /// This initializes a new instance of the <c>DdeMessageLoop</c> class.
        /// </summary>
        public DdeMessageLoop()
        {
        }

        /// <summary>
        /// This releases all resources held by this instance.
        /// </summary>
        public void Dispose()
        {
            _Form.Dispose();
        }

        /// <summary>
        /// This begins an asynchronous operation to execute a delegate on the thread hosting this object.
        /// </summary>
        /// <param name="method">
        /// The delegate to execute.
        /// </param>
        /// <param name="args">
        /// The arguments to pass to the delegate.
        /// </param>
        /// <returns>
        /// An <c>IAsyncResult</c> object for this operation.
        /// </returns>
        IAsyncResult ISynchronizeInvoke.BeginInvoke(Delegate method, object[] args)
        {
            return _Form.BeginInvoke(method, args);
        }

        /// <summary>
        /// This returns the object that the delegate returned in the operation.
        /// </summary>
        /// <param name="asyncResult">
        /// The <c>IAsyncResult</c> object returned by a call to <c>BeginInvoke</c>.
        /// </param>
        /// <returns>
        /// The object returned by the delegate.
        /// </returns>
        object ISynchronizeInvoke.EndInvoke(IAsyncResult asyncResult)
        {
            return _Form.EndInvoke(asyncResult);
        }

        /// <summary>
        /// This executes a delegate on the thread hosting this object.
        /// </summary>
        /// <param name="method">
        /// The delegate to execute.
        /// </param>
        /// <param name="args">
        /// The arguments to pass to the delegate.
        /// </param>
        /// <returns>
        /// The object returned by the delegate.
        /// </returns>
        object ISynchronizeInvoke.Invoke(Delegate method, object[] args)
        {
            if (Thread.VolatileRead(ref _ThreadId) != GetCurrentThreadId())
            {
                return _Form.Invoke(method, args);
            }
            else
            {
                return method.DynamicInvoke(args);
            }
        }

        /// <summary>
        /// This gets a bool indicating whether the caller must use Invoke.
        /// </summary>
        bool ISynchronizeInvoke.InvokeRequired
        {
            get { return Thread.VolatileRead(ref _ThreadId) != GetCurrentThreadId(); }
        }

        /// <summary>
        /// This starts a message loop on the current thread.
        /// </summary>
        public void Run()
        {
            _Form.Show();
            Application.Run();
        }

        /// <summary>
        /// This starts a message loop on the current thread and shows the specified form.
        /// </summary>
        /// <param name="form">
        /// The Form to display.
        /// </param>
        public void Run(Form form)
        {
            _Form.Show();
            Application.Run(form);
        }

        /// <threadsafety static="true" instance="false" />
        private sealed class HiddenForm : Form
        {
            [DllImport("user32.dll")]
            private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hwndNewParent);

            public HiddenForm()
            {
                this.Load += this.HiddenForm_Load;
            }

            protected override CreateParams CreateParams
            {
                get
                {
                    const int WS_POPUP = unchecked((int)0x80000000);
                    const int WS_EX_TOOLWINDOW = 0x80;

                    CreateParams cp = base.CreateParams;
                    cp.ExStyle = WS_EX_TOOLWINDOW;
                    cp.Style = WS_POPUP;
                    cp.Height = 0;
                    cp.Width = 0;
                    return cp;
                }
            }

            private void HiddenForm_Load(object source, EventArgs e)
            {
                if (Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version.Major >= 5)
                {
                    // Make this a message only window if the OS is WinXP or higher.
                    const int HWND_MESSAGE = -1;
                    SetParent(this.Handle, new IntPtr(HWND_MESSAGE));
                }
            }

        } // class

    } // class

} // namespace