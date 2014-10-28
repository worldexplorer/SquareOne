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

    internal sealed class DdemlConversation
    {
        private IntPtr _Handle  = IntPtr.Zero;
        private string _Service = "";
        private string _Topic   = "";
        private int    _Waiting = 0;
        private object _Tag     = null;

        internal event EventHandler StateChange;

        public DdemlConversation(IntPtr handle, string service, string topic)
        {
            _Handle = handle;
            _Service = service;
            _Topic = topic;
        }

        public IntPtr Handle
        {
            get { return _Handle; }
        }

        public string Topic
        {
            get { return _Topic; }
        }

        public string Service
        {
            get { return _Service; }
        }

        public bool IsPaused
        {
            get { return _Waiting > 0; }
        }

        public object Tag
        {
            get { return _Tag; }
            set { _Tag = value; }
        }

        public override string ToString()
        {
            string s = "";
            foreach (System.Reflection.PropertyInfo property in this.GetType().GetProperties())
            {
                if (s.Length == 0)
                {
                    s += property.Name + "=" + property.GetValue(this, null).ToString();
                }
                else
                {
                    s += " " + property.Name + "=" + property.GetValue(this, null).ToString();
                }
            }
            return s;
        }

        internal void IncrementWaiting()
        {
            _Waiting++;
            if (StateChange != null)
            {
                StateChange(this, EventArgs.Empty);
            }
        }

        internal void DecrementWaiting()
        {
            _Waiting--;
            if (StateChange != null)
            {
                StateChange(this, EventArgs.Empty);
            }
        }

    } // class

} // namespace