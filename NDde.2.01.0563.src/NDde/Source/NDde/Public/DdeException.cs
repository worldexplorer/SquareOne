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
namespace NDde
{
    using System;
    using System.Runtime.Serialization;
    using NDde.Foundation;
    using NDde.Properties;

    /// <summary>
    /// This is thrown when a DDE exception occurs.
    /// </summary>
    /// <threadsafety static="true" instance="false" />
    [Serializable]
    public class DdeException : Exception
    {
        private DdemlException _DdemlObject = null;

        internal DdeException(string message) : this(new DdemlException(message))
        {
        }

        internal DdeException(DdemlException exception) : base(exception.Message, exception)
        { 
            _DdemlObject = exception;
        }

        /// <summary>
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected DdeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            _DdemlObject = (DdemlException)info.GetValue("NDde.DdeException.DdemlObject", typeof(DdemlException));
        }

        /// <summary>
        /// This gets an error code returned by the DDEML.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The value is zero if the exception was not thrown because of the DDEML.
        /// </para>
        /// <para>
        /// <list type="bullet">
        /// <item><term>0x0000</term><description>DMLERR_NO_DMLERROR</description></item>
        /// <item><term>0x4000</term><description>DMLERR_ADVACKTIMEOUT</description></item>
        /// <item><term>0x4001</term><description>DMLERR_BUSY</description></item>
        /// <item><term>0x4002</term><description>DMLERR_DATAACKTIMEOUT</description></item>
        /// <item><term>0x4003</term><description>DMLERR_DLL_NOT_INITIALIZED</description></item>
        /// <item><term>0x4004</term><description>DMLERR_DLL_USAGE</description></item>
        /// <item><term>0x4005</term><description>DMLERR_EXECACKTIMEOUT</description></item>
        /// <item><term>0x4006</term><description>DMLERR_INVALIDPARAMETER</description></item>
        /// <item><term>0x4007</term><description>DMLERR_LOW_MEMORY</description></item>
        /// <item><term>0x4008</term><description>DMLERR_MEMORY_DMLERROR</description></item>
        /// <item><term>0x4009</term><description>DMLERR_NOTPROCESSED</description></item>
        /// <item><term>0x400A</term><description>DMLERR_NO_CONV_ESTABLISHED</description></item>
        /// <item><term>0x400B</term><description>DMLERR_POKEACKTIMEOUT</description></item>
        /// <item><term>0x400C</term><description>DMLERR_POSTMSG_FAILED</description></item>
        /// <item><term>0x400D</term><description>DMLERR_REENTRANCY</description></item>
        /// <item><term>0x400E</term><description>DMLERR_SERVER_DIED</description></item>
        /// <item><term>0x400F</term><description>DMLERR_SYS_DMLERROR</description></item>
        /// <item><term>0x4010</term><description>DMLERR_UNADVACKTIMEOUT</description></item>
        /// <item><term>0x4011</term><description>DMLERR_UNFOUND_QUEUE_ID</description></item>
        /// </list>
        /// </para>
        /// </remarks>
        public int Code
        {
            get { return _DdemlObject.Code; }
        }

        /// <summary>
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("NDde.DdeException.DdemlObject", _DdemlObject);
            base.GetObjectData(info, context);
        }

    } // class

} // namespace