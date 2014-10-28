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
namespace NDde.Foundation
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;

    internal static class Ddeml
    {
        public const int MAX_STRING_SIZE                = 255;

        public const int APPCMD_CLIENTONLY              = unchecked((int)0x00000010);
        public const int APPCMD_FILTERINITS             = unchecked((int)0x00000020);
        public const int APPCMD_MASK                    = unchecked((int)0x00000FF0);
        public const int APPCLASS_STANDARD              = unchecked((int)0x00000000);
        public const int APPCLASS_MONITOR               = unchecked((int)0x00000001);
        public const int APPCLASS_MASK                  = unchecked((int)0x0000000F);
        
        public const int CBR_BLOCK                      = unchecked((int)0xFFFFFFFF);
        
        public const int CBF_FAIL_SELFCONNECTIONS       = unchecked((int)0x00001000);
        public const int CBF_FAIL_CONNECTIONS           = unchecked((int)0x00002000);
        public const int CBF_FAIL_ADVISES               = unchecked((int)0x00004000);
        public const int CBF_FAIL_EXECUTES              = unchecked((int)0x00008000);
        public const int CBF_FAIL_POKES                 = unchecked((int)0x00010000);
        public const int CBF_FAIL_REQUESTS              = unchecked((int)0x00020000);
        public const int CBF_FAIL_ALLSVRXACTIONS        = unchecked((int)0x0003f000);
        public const int CBF_SKIP_CONNECT_CONFIRMS      = unchecked((int)0x00040000);
        public const int CBF_SKIP_REGISTRATIONS         = unchecked((int)0x00080000);
        public const int CBF_SKIP_UNREGISTRATIONS       = unchecked((int)0x00100000);
        public const int CBF_SKIP_DISCONNECTS           = unchecked((int)0x00200000);
        public const int CBF_SKIP_ALLNOTIFICATIONS      = unchecked((int)0x003c0000);

        public const int CF_TEXT                        = 1;
        
        public const int CP_WINANSI                     = 1004;
        public const int CP_WINUNICODE                  = 1200;
        
        public const int DDE_FACK                       = unchecked((int)0x8000);
        public const int DDE_FBUSY                      = unchecked((int)0x4000);
        public const int DDE_FDEFERUPD                  = unchecked((int)0x4000);
        public const int DDE_FACKREQ                    = unchecked((int)0x8000);
        public const int DDE_FRELEASE                   = unchecked((int)0x2000);
        public const int DDE_FREQUESTED                 = unchecked((int)0x1000);
        public const int DDE_FAPPSTATUS                 = unchecked((int)0x00ff);
        public const int DDE_FNOTPROCESSED              = unchecked((int)0x0000);
        
        public const int DMLERR_NO_ERROR                = unchecked((int)0x0000);
        public const int DMLERR_FIRST                   = unchecked((int)0x4000);
        public const int DMLERR_ADVACKTIMEOUT           = unchecked((int)0x4000);
        public const int DMLERR_BUSY                    = unchecked((int)0x4001);
        public const int DMLERR_DATAACKTIMEOUT          = unchecked((int)0x4002);
        public const int DMLERR_DLL_NOT_INITIALIZED     = unchecked((int)0x4003);
        public const int DMLERR_DLL_USAGE               = unchecked((int)0x4004);
        public const int DMLERR_EXECACKTIMEOUT          = unchecked((int)0x4005);
        public const int DMLERR_INVALIDPARAMETER        = unchecked((int)0x4006);
        public const int DMLERR_LOW_MEMORY              = unchecked((int)0x4007);
        public const int DMLERR_MEMORY_ERROR            = unchecked((int)0x4008);
        public const int DMLERR_NOTPROCESSED            = unchecked((int)0x4009);
        public const int DMLERR_NO_CONV_ESTABLISHED     = unchecked((int)0x400A);
        public const int DMLERR_POKEACKTIMEOUT          = unchecked((int)0x400B);
        public const int DMLERR_POSTMSG_FAILED          = unchecked((int)0x400C);
        public const int DMLERR_REENTRANCY              = unchecked((int)0x400D);
        public const int DMLERR_SERVER_DIED             = unchecked((int)0x400E);
        public const int DMLERR_SYS_ERROR               = unchecked((int)0x400F);
        public const int DMLERR_UNADVACKTIMEOUT         = unchecked((int)0x4010);
        public const int DMLERR_UNFOUND_QUEUE_ID        = unchecked((int)0x4011);
        public const int DMLERR_LAST                    = unchecked((int)0x4011);
                                
        public const int DNS_REGISTER                   = unchecked((int)0x0001);
        public const int DNS_UNREGISTER                 = unchecked((int)0x0002);
        public const int DNS_FILTERON                   = unchecked((int)0x0004);
        public const int DNS_FILTEROFF                  = unchecked((int)0x0008);
        
        public const int EC_ENABLEALL                   = unchecked((int)0x0000);
        public const int EC_ENABLEONE                   = unchecked((int)0x0080);
        public const int EC_DISABLE                     = unchecked((int)0x0008);
        public const int EC_QUERYWAITING                = unchecked((int)0x0002);
                
        public const int HDATA_APPOWNED                 = unchecked((int)0x0001);
        
        public const int MF_HSZ_INFO                    = unchecked((int)0x01000000);
        public const int MF_SENDMSGS                    = unchecked((int)0x02000000);
        public const int MF_POSTMSGS                    = unchecked((int)0x04000000);
        public const int MF_CALLBACKS                   = unchecked((int)0x08000000);
        public const int MF_ERRORS                      = unchecked((int)0x10000000);
        public const int MF_LINKS                       = unchecked((int)0x20000000);
        public const int MF_CONV                        = unchecked((int)0x40000000);
        
        public const int MH_CREATE                      = 1;
        public const int MH_KEEP                        = 2;
        public const int MH_DELETE                      = 3;
        public const int MH_CLEANUP                     = 4;

        public const int QID_SYNC                       = unchecked((int)0xFFFFFFFF);
        public const int TIMEOUT_ASYNC                  = unchecked((int)0xFFFFFFFF);
        
        public const int XTYPF_NOBLOCK                  = unchecked((int)0x0002);
        public const int XTYPF_NODATA                   = unchecked((int)0x0004);
        public const int XTYPF_ACKREQ                   = unchecked((int)0x0008);
        public const int XCLASS_MASK                    = unchecked((int)0xFC00);
        public const int XCLASS_BOOL                    = unchecked((int)0x1000);
        public const int XCLASS_DATA                    = unchecked((int)0x2000);
        public const int XCLASS_FLAGS                   = unchecked((int)0x4000);
        public const int XCLASS_NOTIFICATION            = unchecked((int)0x8000);
        public const int XTYP_ERROR                     = unchecked((int)(0x0000 | XCLASS_NOTIFICATION | XTYPF_NOBLOCK));
        public const int XTYP_ADVDATA                   = unchecked((int)(0x0010 | XCLASS_FLAGS));
        public const int XTYP_ADVREQ                    = unchecked((int)(0x0020 | XCLASS_DATA | XTYPF_NOBLOCK));
        public const int XTYP_ADVSTART                  = unchecked((int)(0x0030 | XCLASS_BOOL));
        public const int XTYP_ADVSTOP                   = unchecked((int)(0x0040 | XCLASS_NOTIFICATION));
        public const int XTYP_EXECUTE                   = unchecked((int)(0x0050 | XCLASS_FLAGS));
        public const int XTYP_CONNECT                   = unchecked((int)(0x0060 | XCLASS_BOOL | XTYPF_NOBLOCK));
        public const int XTYP_CONNECT_CONFIRM           = unchecked((int)(0x0070 | XCLASS_NOTIFICATION | XTYPF_NOBLOCK));
        public const int XTYP_XACT_COMPLETE             = unchecked((int)(0x0080 | XCLASS_NOTIFICATION));
        public const int XTYP_POKE                      = unchecked((int)(0x0090 | XCLASS_FLAGS));
        public const int XTYP_REGISTER                  = unchecked((int)(0x00A0 | XCLASS_NOTIFICATION | XTYPF_NOBLOCK));
        public const int XTYP_REQUEST                   = unchecked((int)(0x00B0 | XCLASS_DATA));
        public const int XTYP_DISCONNECT                = unchecked((int)(0x00C0 | XCLASS_NOTIFICATION | XTYPF_NOBLOCK));
        public const int XTYP_UNREGISTER                = unchecked((int)(0x00D0 | XCLASS_NOTIFICATION | XTYPF_NOBLOCK));
        public const int XTYP_WILDCONNECT               = unchecked((int)(0x00E0 | XCLASS_DATA | XTYPF_NOBLOCK));
        public const int XTYP_MONITOR                   = unchecked((int)(0x00F0 | XCLASS_NOTIFICATION | XTYPF_NOBLOCK));
        public const int XTYP_MASK                      = unchecked((int)0x00F0);
        public const int XTYP_SHIFT                     = unchecked((int)0x0004);

        public delegate IntPtr DdeCallback(
            int uType, int uFmt, IntPtr hConv, IntPtr hsz1, IntPtr hsz2, IntPtr hData, IntPtr dwData1, IntPtr dwData2);

        [DllImport("kernel32.dll")]
        public static extern int GetCurrentThreadId();

        [DllImport("user32.dll", EntryPoint="DdeAbandonTransaction", CharSet=CharSet.Ansi)]
        public static extern bool DdeAbandonTransaction(int idInst, IntPtr hConv, int idTransaction);

        [DllImport("user32.dll", EntryPoint="DdeAccessData", CharSet=CharSet.Ansi)]
        public static extern IntPtr DdeAccessData(IntPtr hData, ref int pcbDataSize);

        [DllImport("user32.dll", EntryPoint="DdeAddData", CharSet=CharSet.Ansi)]
        public static extern IntPtr DdeAddData(IntPtr hData, byte[] pSrc, int cb, int cbOff);

        [DllImport("user32.dll", EntryPoint="DdeClientTransaction", CharSet=CharSet.Ansi)]
        public static extern IntPtr DdeClientTransaction(
            IntPtr pData, int cbData, IntPtr hConv, IntPtr hszItem, int wFmt, int wType, int dwTimeout, ref int pdwResult);
        
        [DllImport("user32.dll", EntryPoint="DdeClientTransaction", CharSet=CharSet.Ansi)]
        public static extern IntPtr DdeClientTransaction(
            byte[] pData, int cbData, IntPtr hConv, IntPtr hszItem, int wFmt, int wType, int dwTimeout, ref int pdwResult);

        [DllImport("user32.dll", EntryPoint="DdeCmpStringHandles", CharSet=CharSet.Ansi)]
        public static extern int DdeCmpStringHandles(IntPtr hsz1, IntPtr hsz2);

        [DllImport("user32.dll", EntryPoint="DdeConnect", CharSet=CharSet.Ansi)]
        public static extern IntPtr DdeConnect(int idInst, IntPtr hszService, IntPtr hszTopic, IntPtr pCC);

        [DllImport("user32.dll", EntryPoint="DdeConnectList", CharSet=CharSet.Ansi)]
        public static extern IntPtr DdeConnectList(int idInst, IntPtr hszService, IntPtr hszTopic, IntPtr hConvList, IntPtr pCC);

        [DllImport("user32.dll", EntryPoint="DdeCreateDataHandle", CharSet=CharSet.Ansi)]
        public static extern IntPtr DdeCreateDataHandle(int idInst, byte[] pSrc, int cb, int cbOff, IntPtr hszItem, int wFmt, int afCmd);

        [DllImport("user32.dll", EntryPoint="DdeCreateStringHandle", CharSet=CharSet.Ansi)]
        public static extern IntPtr DdeCreateStringHandle(int idInst, string psz, int iCodePage);

        [DllImport("user32.dll", EntryPoint="DdeDisconnect", CharSet=CharSet.Ansi)]
        public static extern bool DdeDisconnect(IntPtr hConv);

        [DllImport("user32.dll", EntryPoint="DdeDisconnectList", CharSet=CharSet.Ansi)]
        public static extern bool DdeDisconnectList(IntPtr hConvList);

        [DllImport("user32.dll", EntryPoint="DdeEnableCallback", CharSet=CharSet.Ansi)]
        public static extern bool DdeEnableCallback(int idInst, IntPtr hConv, int wCmd);

        [DllImport("user32.dll", EntryPoint="DdeFreeDataHandle", CharSet=CharSet.Ansi)]
        public static extern bool DdeFreeDataHandle(IntPtr hData);
        
        [DllImport("user32.dll", EntryPoint="DdeFreeStringHandle", CharSet=CharSet.Ansi)]
        public static extern bool DdeFreeStringHandle(int idInst, IntPtr hsz);
        
        [DllImport("user32.dll", EntryPoint="DdeGetData", CharSet=CharSet.Ansi)]
        public static extern int DdeGetData(IntPtr hData, [Out] byte[] pDst, int cbMax, int cbOff);

        [DllImport("user32.dll", EntryPoint="DdeGetLastError", CharSet=CharSet.Ansi)]
        public static extern int DdeGetLastError(int idInst);

        [DllImport("user32.dll", EntryPoint="DdeImpersonateClient", CharSet=CharSet.Ansi)]
        public static extern bool DdeImpersonateClient(IntPtr hConv);

        [DllImport("user32.dll", EntryPoint="DdeInitialize", CharSet=CharSet.Ansi)]
        public static extern int DdeInitialize(ref int pidInst, DdeCallback pfnCallback, int afCmd, int ulRes);
    
        [DllImport("user32.dll", EntryPoint="DdeKeepStringHandle", CharSet=CharSet.Ansi)]
        public static extern bool DdeKeepStringHandle(int idInst, IntPtr hsz);
        
        [DllImport("user32.dll", EntryPoint="DdeNameService", CharSet=CharSet.Ansi)]
        public static extern IntPtr DdeNameService(int idInst, IntPtr hsz1, IntPtr hsz2, int afCmd);

        [DllImport("user32.dll", EntryPoint="DdePostAdvise", CharSet=CharSet.Ansi)]
        public static extern bool DdePostAdvise(int idInst, IntPtr hszTopic, IntPtr hszItem);

        [DllImport("user32.dll", EntryPoint="DdeQueryConvInfo", CharSet=CharSet.Ansi)]
        public static extern int DdeQueryConvInfo(IntPtr hConv, int idTransaction, IntPtr pConvInfo);

        [DllImport("user32.dll", EntryPoint="DdeQueryNextServer", CharSet=CharSet.Ansi)]
        public static extern IntPtr DdeQueryNextServer(IntPtr hConvList, IntPtr hConvPrev);

        [DllImport("user32.dll", EntryPoint="DdeQueryString", CharSet=CharSet.Ansi)]
        public static extern int DdeQueryString(int idInst, IntPtr hsz, StringBuilder psz, int cchMax, int iCodePage);

        [DllImport("user32.dll", EntryPoint="DdeReconnect", CharSet=CharSet.Ansi)]
        public static extern IntPtr DdeReconnect(IntPtr hConv);

        [DllImport("user32.dll", EntryPoint="DdeSetUserHandle", CharSet=CharSet.Ansi)]
        public static extern bool DdeSetUserHandle(IntPtr hConv, int id, IntPtr hUser);

        [DllImport("user32.dll", EntryPoint="DdeUnaccessData", CharSet=CharSet.Ansi)]
        public static extern bool DdeUnaccessData(IntPtr hData);

        [DllImport("user32.dll", EntryPoint="DdeUninitialize", CharSet=CharSet.Ansi)]
        public static extern bool DdeUninitialize(int idInst);

        [StructLayout(LayoutKind.Sequential)]
        public struct HSZPAIR
        {
            public IntPtr hszSvc;
            public IntPtr hszTopic;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct CONVINFO
        {
            public int         cb;
            public IntPtr      hUser;
            public IntPtr      hConvPartner;
            public IntPtr      hszSvcPartner;
            public IntPtr      hszServiceReq;
            public IntPtr      hszTopic;
            public IntPtr      hszItem;
            public int         wFmt;
            public int         wType;
            public int         wStatus;
            public int         wConvst;
            public int         wLastError;
            public IntPtr      hConvList;
            public CONVCONTEXT ConvCtxt;
            public IntPtr      hwnd;
            public IntPtr      hwndPartner;

        } // struct

        [StructLayout(LayoutKind.Sequential)]
        public struct CONVCONTEXT
        {
            public int    cb;
            public int    wFlags;
            public int    wCountryID;
            public int    iCodePage;
            public int    dwLangID;
            public int    dwSecurity;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst=12)]
            public byte[] filler;

        } // struct

        [StructLayout(LayoutKind.Sequential)]
        public struct MONCBSTRUCT
        {
            public int         cb;
            public int         dwTime;
            public IntPtr      hTask;
            public IntPtr      dwRet;
            public int         wType;
            public int         wFmt;
            public IntPtr      hConv;
            public IntPtr      hsz1;
            public IntPtr      hsz2;
            public IntPtr      hData;
            public IntPtr      dwData1;
            public IntPtr      dwData2;
            public CONVCONTEXT cc;
            public int         cbData;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst=32)]
            public byte[]      Data;

        } // struct

        [StructLayout(LayoutKind.Sequential)]
        public struct MONCONVSTRUCT
        {
            public int    cb;
            public bool   fConnect;
            public int    dwTime;
            public IntPtr hTask;
            public IntPtr hszSvc;
            public IntPtr hszTopic;
            public IntPtr hConvClient;
            public IntPtr hConvServer;

        } // struct

        [StructLayout(LayoutKind.Sequential)]
        public struct MONERRSTRUCT
        {
            public int    cb;
            public int    wLastError;
            public int    dwTime;
            public IntPtr hTask;

        } // struct

        [StructLayout(LayoutKind.Sequential)]
        public struct MONHSZSTRUCT
        {
            public int    cb;
            public int    fsAction;
            public int    dwTime;
            public IntPtr hsz;
            public IntPtr hTask;
            public IntPtr str;

        } // struct

        [StructLayout(LayoutKind.Sequential)]
        public struct MONLINKSTRUCT
        {
            public int    cb;
            public int    dwTime;
            public IntPtr hTask;
            public bool   fEstablished;
            public bool   fNoData;
            public IntPtr hszSvc;
            public IntPtr hszTopic;
            public IntPtr hszItem;
            public int    wFmt;
            public bool   fServer;
            public IntPtr hConvClient;
            public IntPtr hConvServer;

        } // struct

        [StructLayout(LayoutKind.Sequential)]
        public struct MONMSGSTRUCT
        {
            public int                 cb;
            public IntPtr              hwndTo;
            public int                 dwTime;
            public IntPtr              hTask;
            public int                 wMsg;
            public IntPtr              wParam;
            public IntPtr              lParam;
            public DDEML_MSG_HOOK_DATA dmhd;

        } // struct

        [StructLayout(LayoutKind.Sequential)]
        public struct DDEML_MSG_HOOK_DATA
        {
            public IntPtr uiLo;
            public IntPtr uiHi;
            public int    cbData;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst=32)]
            public byte[] Data;

        } // struct
            
    } // class

} // namespace
