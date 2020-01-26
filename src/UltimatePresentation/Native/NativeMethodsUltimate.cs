using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UltimatePresentation.Native.Win32;

namespace UltimatePresentation.Native
{
        internal static class NativeMethodsUltimate
        {
                public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
                public delegate IntPtr SubclassProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, UIntPtr id, IntPtr refData);
                public delegate IntPtr WindowsHookProc(NativeMethodsUltimate.CbtHookAction code, IntPtr wParam, IntPtr lParam);
                public enum CbtHookAction
                {
                        HCBT_MOVESIZE,
                        HCBT_MINMAX,
                        HCBT_QS,
                        HCBT_CREATEWND,
                        HCBT_DESTROYWND,
                        HCBT_ACTIVATE,
                        HCBT_CLICKSKIPPED,
                        HCBT_KEYSKIPPED,
                        HCBT_SYSCOMMAND,
                        HCBT_SETFOCUS
                }
                public delegate IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
                [Flags]
                public enum RedrawWindowFlags : uint
                {
                        Invalidate = 1u,
                        InternalPaint = 2u,
                        Erase = 4u,
                        Validate = 8u,
                        NoInternalPaint = 16u,
                        NoErase = 32u,
                        NoChildren = 64u,
                        AllChildren = 128u,
                        UpdateNow = 256u,
                        EraseNow = 512u,
                        Frame = 1024u,
                        NoFrame = 2048u
                }
                [return: MarshalAs(UnmanagedType.Bool)]
                internal delegate bool EnumMonitorsDelegate(IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData);
                internal struct PictDescBitmap
                {
                        internal int cbSizeOfStruct;
                        internal int pictureType;
                        internal IntPtr hBitmap;
                        internal IntPtr hPalette;
                        internal int unused;
                        internal static NativeMethodsUltimate.PictDescBitmap Default
                        {
                                get
                                {
                                        return new NativeMethodsUltimate.PictDescBitmap
                                        {
                                                cbSizeOfStruct = 20,
                                                pictureType = 1,
                                                hBitmap = IntPtr.Zero,
                                                hPalette = IntPtr.Zero
                                        };
                                }
                        }
                }
                internal struct BITMAPINFO
                {
                        internal int biSize;
                        internal int biWidth;
                        internal int biHeight;
                        internal short biPlanes;
                        internal short biBitCount;
                        internal int biCompression;
                        internal int biSizeImage;
                        internal int biXPelsPerMeter;
                        internal int biYPelsPerMeter;
                        internal int biClrUsed;
                        internal int biClrImportant;
                        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
                        internal byte[] bmiColors;
                        internal static NativeMethodsUltimate.BITMAPINFO Default
                        {
                                get
                                {
                                        return new NativeMethodsUltimate.BITMAPINFO
                                        {
                                                biSize = 40,
                                                biPlanes = 1
                                        };
                                }
                        }
                }
                internal struct BITMAPINFOHEADER
                {
                        internal uint biSize;
                        internal int biWidth;
                        internal int biHeight;
                        internal ushort biPlanes;
                        internal ushort biBitCount;
                        internal uint biCompression;
                        internal uint biSizeImage;
                        internal int biXPelsPerMeter;
                        internal int biYPelsPerMeter;
                        internal uint biClrUsed;
                        internal uint biClrImportant;
                        internal static NativeMethodsUltimate.BITMAPINFOHEADER Default
                        {
                                get
                                {
                                        return new NativeMethodsUltimate.BITMAPINFOHEADER
                                        {
                                                biSize = 40u,
                                                biPlanes = 1
                                        };
                                }
                        }
                }
                [StructLayout(LayoutKind.Sequential)]
                internal class BITMAPFILEHEADER
                {
                        public ushort bfType;
                        public uint bfSize;
                        public ushort bfReserved1;
                        public ushort bfReserved2;
                        public uint bfOffBits;
                        internal static NativeMethodsUltimate.BITMAPFILEHEADER Default
                        {
                                get
                                {
                                        return new NativeMethodsUltimate.BITMAPFILEHEADER
                                        {
                                                bfSize = 14u
                                        };
                                }
                        }
                }
                internal struct BLENDFUNCTION
                {
                        public byte BlendOp;
                        public byte BlendFlags;
                        public byte SourceConstantAlpha;
                        public byte AlphaFormat;
                }
                public enum GWL
                {
                        STYLE = -16,
                        EXSTYLE = -20
                }
                public enum GWLP
                {
                        WNDPROC = -4,
                        HINSTANCE = -6,
                        HWNDPARENT = -8,
                        USERDATA = -21,
                        ID = -12
                }
                public enum CombineMode
                {
                        RGN_AND = 1,
                        RGN_OR,
                        RGN_XOR,
                        RGN_DIFF,
                        RGN_COPY,
                        RGN_MIN = 1,
                        RGN_MAX = 5
                }
                public const int MAX_PATH = 260;
                private const byte KeyDown = 128;
                public const uint MAPVK_VK_TO_VSC = 0u;
                public const uint MAPVK_VSC_TO_VK = 1u;
                public const uint MAPVK_VK_TO_CHAR = 2u;
                public const uint MAPVK_VSC_TO_VK_EX = 3u;
                public const int VK_LBUTTON = 1;
                public const int VK_RBUTTON = 2;
                public const int VK_MBUTTON = 4;
                public const int VK_XBUTTON1 = 5;
                public const int VK_XBUTTON2 = 6;
                public const int VK_TAB = 9;
                public const int VK_SHIFT = 16;
                public const int VK_CONTROL = 17;
                public const int VK_MENU = 18;
                public const int VK_LSHIFT = 160;
                public const int VK_RSHIFT = 161;
                public const int VK_LCONTROL = 162;
                public const int VK_RCONTROL = 163;
                public const int VK_LMENU = 164;
                public const int VK_RMENU = 165;
                public const int VK_LWIN = 91;
                public const int VK_RWIN = 92;
                public const int VK_F1 = 112;
                public const int VK_ESC = 27;
                public const int VK_RETURN = 13;
                public const int MK_CONTROL = 8;
                public const int MK_LBUTTON = 1;
                public const int MK_MBUTTON = 16;
                public const int MK_RBUTTON = 2;
                public const int MK_SHIFT = 4;
                public const int MK_XBUTTON1 = 32;
                public const int MK_XBUTTON2 = 64;
                public const int XBUTTON1 = 1;
                public const int XBUTTON2 = 2;
                public const int RPC_E_SERVERCALL_RETRYLATER = -2147417846;
                public const int RPC_E_SERVERCALL_REJECTED = -2147417845;
                public const int RPC_E_RETRY = -2147417847;
                public const int RPC_E_DISCONNECTED = -2147417848;
                public const int RPC_E_SYS_CALL_FAILED = -2147417856;
                public const int E_SHARING_VIOLATION = -2147024864;
                internal const int PICTYPE_UNINITIALIZED = -1;
                internal const int PICTYPE_NONE = 0;
                internal const int PICTYPE_BITMAP = 1;
                internal const int PICTYPE_METAFILE = 2;
                internal const int PICTYPE_ICON = 3;
                internal const int PICTYPE_ENHMETAFILE = 4;
                internal const int BITMAPINFO_MAX_COLORSIZE = 256;
                internal const int DIB_RGB_COLORS = 0;
                internal const int DIB_PAL_COLORS = 1;
                internal const int AC_SRC_OVER = 0;
                internal const int AC_SRC_ALPHA = 1;
                internal const int ULW_ALPHA = 2;
                internal const int BI_RGB = 0;
                internal const int BI_RLE8 = 1;
                internal const int BI_RLE4 = 2;
                internal const int BI_BITFIELDS = 3;
                internal const int BI_JPEG = 4;
                internal const int BI_PNG = 5;
                public const int TRUE = 1;
                public const int FALSE = 0;
                public const int DCX_WINDOW = 1;
                public const int DCX_CACHE = 2;
                public const int DCX_NORESETATTRS = 4;
                public const int DCX_CLIPCHILDREN = 8;
                public const int DCX_CLIPSIBLINGS = 16;
                public const int DCX_PARENTCLIP = 32;
                public const int DCX_EXCLUDERGN = 64;
                public const int DCX_INTERSECTRGN = 128;
                public const int DCX_EXCLUDEUPDATE = 256;
                public const int DCX_INTERSECTUPDATE = 512;
                public const int DCX_LOCKWINDOWUPDATE = 1024;
                public const int ILD_NORMAL = 0;
                public const int ILD_TRANSPARENT = 1;
                public const int ILD_MASK = 16;
                public const int ILD_IMAGE = 32;
                public const int ILD_ROP = 64;
                public const int ILD_BLEND25 = 2;
                public const int ILD_BLEND50 = 4;
                public const int ILD_OVERLAYMASK = 3840;
                public const int ILD_SELECTED = 4;
                public const int ILD_FOCUS = 2;
                public const int ILD_BLEND = 4;
                public const int GA_PARENT = 1;
                public const int GA_ROOT = 2;
                public const int GA_ROOTOWNER = 3;
                public const int GW_FIRST = 0;
                public const int GW_LAST = 1;
                public const int GW_HWNDNEXT = 2;
                public const int GW_HWNDPREV = 3;
                public const int GW_OWNER = 4;
                public const int GW_CHILD = 5;
                public const int HTNOWHERE = 0;
                public const int HTCLIENT = 1;
                public const int HTCAPTION = 2;
                public const int HTSYSMENU = 3;
                public const int HTLEFT = 10;
                public const int HTRIGHT = 11;
                public const int HTTOP = 12;
                public const int HTTOPLEFT = 13;
                public const int HTTOPRIGHT = 14;
                public const int HTBOTTOM = 15;
                public const int HTBOTTOMLEFT = 16;
                public const int HTBOTTOMRIGHT = 17;
                public const int ICON_BIG = 1;
                public const int ICON_SMALL = 0;
                public const int LWA_COLORKEY = 1;
                public const int LWA_ALPHA = 2;
                public const int LOGPIXELSX = 88;
                public const int LOGPIXELSY = 90;
                public const int MA_ACTIVATE = 1;
                public const int MA_ACTIVATEANDEAT = 2;
                public const int MA_NOACTIVATE = 3;
                public const int MA_NOACTIVATEANDEAT = 4;
                public const int MONITOR_DEFAULTTONEAREST = 2;
                public const int SW_HIDE = 0;
                public const int SW_SHOWNORMAL = 1;
                public const int SW_NORMAL = 1;
                public const int SW_SHOWMINIMIZED = 2;
                public const int SW_SHOWMAXIMIZED = 3;
                public const int SW_MAXIMIZE = 3;
                public const int SW_SHOWNOACTIVATE = 4;
                public const int SW_SHOW = 5;
                public const int SW_MINIMIZE = 6;
                public const int SW_SHOWMINNOACTIVE = 7;
                public const int SW_SHOWNA = 8;
                public const int SW_RESTORE = 9;
                public const int SW_SHOWDEFAULT = 10;
                public const int SW_FORCEMINIMIZE = 11;
                public const int SW_MAX = 11;
                public const int SW_PARENTCLOSING = 1;
                public const int SW_OTHERZOOM = 2;
                public const int SW_PARENTOPENING = 3;
                public const int SW_OTHERUNZOOM = 4;
                public const int WA_INACTIVE = 0;
                public const int WA_ACTIVE = 1;
                public const int WA_CLICKACTIVE = 2;
                public const int SC_SIZE = 61440;
                public const int SC_MOVE = 61456;
                public const int SC_MINIMIZE = 61472;
                public const int SC_MAXIMIZE = 61488;
                public const int SC_NEXTWINDOW = 61504;
                public const int SC_PREVWINDOW = 61520;
                public const int SC_CLOSE = 61536;
                public const int SC_VSCROLL = 61552;
                public const int SC_HSCROLL = 61568;
                public const int SC_MOUSEMENU = 61584;
                public const int SC_KEYMENU = 61696;
                public const int SC_ARRANGE = 61712;
                public const int SC_RESTORE = 61728;
                public const int SC_TASKLIST = 61744;
                public const int SC_SCREENSAVE = 61760;
                public const int SC_HOTKEY = 61776;
                public const int SC_DEFAULT = 61792;
                public const int SC_MONITORPOWER = 61808;
                public const int SC_CONTEXTHELP = 61824;
                public const int SC_SEPARATOR = 61455;
                public const int SM_SWAPBUTTON = 23;
                public const int SM_MENUDROPALIGNMENT = 40;
                public const int SPI_SETHIGHCONTRAST = 67;
                public const int SPI_GETNONCLIENTMETRICS = 41;
                public const int SPI_SETNONCLIENTMETRICS = 42;
                public const int SWP_NOSIZE = 1;
                public const int SWP_NOMOVE = 2;
                public const int SWP_NOZORDER = 4;
                public const int SWP_NOREDRAW = 8;
                public const int SWP_NOACTIVATE = 16;
                public const int SWP_FRAMECHANGED = 32;
                public const int SWP_SHOWWINDOW = 64;
                public const int SWP_HIDEWINDOW = 128;
                public const int SWP_NOCOPYBITS = 256;
                public const int SWP_NOOWNERZORDER = 512;
                public const int SWP_NOSENDCHANGING = 1024;
                public const int SWP_DEFERERASE = 8192;
                public const int SWP_ASYNCWINDOWPOS = 16384;
                public const uint TPM_LEFTBUTTON = 0u;
                public const uint TPM_RIGHTBUTTON = 2u;
                public const uint TPM_LEFTALIGN = 0u;
                public const uint TPM_CENTERALIGN = 4u;
                public const uint TPM_RIGHTALIGN = 8u;
                public const uint TPM_TOPALIGN = 0u;
                public const uint TPM_VCENTERALIGN = 16u;
                public const uint TPM_BOTTOMALIGN = 32u;
                public const uint TPM_HORIZONTAL = 0u;
                public const uint TPM_VERTICAL = 64u;
                public const uint TPM_NONOTIFY = 128u;
                public const uint TPM_RETURNCMD = 256u;
                public const uint TPM_RECURSE = 1u;
                public const uint TPM_HORPOSANIMATION = 1024u;
                public const uint TPM_HORNEGANIMATION = 2048u;
                public const uint TPM_VERPOSANIMATION = 4096u;
                public const uint TPM_VERNEGANIMATION = 8192u;
                public const uint TPM_NOANIMATION = 16384u;
                public const uint TPM_LAYOUTRTL = 32768u;
                public const uint TPM_WORKAREA = 65536u;
                public const int WM_NULL = 0;
                public const int WM_CREATE = 1;
                public const int WM_DESTROY = 2;
                public const int WM_MOVE = 3;
                public const int WM_SIZE = 5;
                public const int WM_ACTIVATE = 6;
                public const int WM_SETFOCUS = 7;
                public const int WM_KILLFOCUS = 8;
                public const int WM_ENABLE = 10;
                public const int WM_SETREDRAW = 11;
                public const int WM_SETTEXT = 12;
                public const int WM_GETTEXT = 13;
                public const int WM_GETTEXTLENGTH = 14;
                public const int WM_PAINT = 15;
                public const int WM_CLOSE = 16;
                public const int WM_QUERYENDSESSION = 17;
                public const int WM_QUERYOPEN = 19;
                public const int WM_ENDSESSION = 22;
                public const int WM_QUIT = 18;
                public const int WM_ERASEBKGND = 20;
                public const int WM_SYSCOLORCHANGE = 21;
                public const int WM_SHOWWINDOW = 24;
                public const int WM_WININICHANGE = 26;
                public const int WM_SETTINGCHANGE = 26;
                public const int WM_DEVMODECHANGE = 27;
                public const int WM_ACTIVATEAPP = 28;
                public const int WM_FONTCHANGE = 29;
                public const int WM_TIMECHANGE = 30;
                public const int WM_CANCELMODE = 31;
                public const int WM_SETCURSOR = 32;
                public const int WM_MOUSEACTIVATE = 33;
                public const int WM_CHILDACTIVATE = 34;
                public const int WM_QUEUESYNC = 35;
                public const int WM_GETMINMAXINFO = 36;
                public const int WM_PAINTICON = 38;
                public const int WM_ICONERASEBKGND = 39;
                public const int WM_NEXTDLGCTL = 40;
                public const int WM_SPOOLERSTATUS = 42;
                public const int WM_DRAWITEM = 43;
                public const int WM_MEASUREITEM = 44;
                public const int WM_DELETEITEM = 45;
                public const int WM_VKEYTOITEM = 46;
                public const int WM_CHARTOITEM = 47;
                public const int WM_SETFONT = 48;
                public const int WM_GETFONT = 49;
                public const int WM_SETHOTKEY = 50;
                public const int WM_GETHOTKEY = 51;
                public const int WM_QUERYDRAGICON = 55;
                public const int WM_COMPAREITEM = 57;
                public const int WM_GETOBJECT = 61;
                public const int WM_COMPACTING = 65;
                public const int WM_COMMNOTIFY = 68;
                public const int WM_WINDOWPOSCHANGING = 70;
                public const int WM_WINDOWPOSCHANGED = 71;
                public const int WM_POWER = 72;
                public const int WM_COPYDATA = 74;
                public const int WM_CANCELJOURNAL = 75;
                public const int WM_NOTIFY = 78;
                public const int WM_INPUTLANGCHANGEREQUEST = 80;
                public const int WM_INPUTLANGCHANGE = 81;
                public const int WM_TCARD = 82;
                public const int WM_HELP = 83;
                public const int WM_USERCHANGED = 84;
                public const int WM_NOTIFYFORMAT = 85;
                public const int WM_CONTEXTMENU = 123;
                public const int WM_STYLECHANGING = 124;
                public const int WM_STYLECHANGED = 125;
                public const int WM_DISPLAYCHANGE = 126;
                public const int WM_GETICON = 127;
                public const int WM_SETICON = 128;
                public const int WM_NCCREATE = 129;
                public const int WM_NCDESTROY = 130;
                public const int WM_NCCALCSIZE = 131;
                public const int WM_NCHITTEST = 132;
                public const int WM_NCPAINT = 133;
                public const int WM_NCACTIVATE = 134;
                public const int WM_GETDLGCODE = 135;
                public const int WM_SYNCPAINT = 136;
                public const int WM_NCMOUSEMOVE = 160;
                public const int WM_NCLBUTTONDOWN = 161;
                public const int WM_NCLBUTTONUP = 162;
                public const int WM_NCLBUTTONDBLCLK = 163;
                public const int WM_NCRBUTTONDOWN = 164;
                public const int WM_NCRBUTTONUP = 165;
                public const int WM_NCRBUTTONDBLCLK = 166;
                public const int WM_NCMBUTTONDOWN = 167;
                public const int WM_NCMBUTTONUP = 168;
                public const int WM_NCMBUTTONDBLCLK = 169;
                public const int WM_NCXBUTTONDOWN = 171;
                public const int WM_NCXBUTTONUP = 172;
                public const int WM_NCXBUTTONDBLCLK = 173;
                public const int WM_NCUAHDRAWCAPTION = 174;
                public const int WM_NCUAHDRAWFRAME = 175;
                public const int WM_INPUT = 255;
                public const int WM_KEYFIRST = 256;
                public const int WM_KEYDOWN = 256;
                public const int WM_KEYUP = 257;
                public const int WM_CHAR = 258;
                public const int WM_DEADCHAR = 259;
                public const int WM_SYSKEYDOWN = 260;
                public const int WM_SYSKEYUP = 261;
                public const int WM_SYSCHAR = 262;
                public const int WM_SYSDEADCHAR = 263;
                public const int WM_UNICHAR = 265;
                public const int WM_KEYLAST = 264;
                public const int WM_IME_STARTCOMPOSITION = 269;
                public const int WM_IME_ENDCOMPOSITION = 270;
                public const int WM_IME_COMPOSITION = 271;
                public const int WM_IME_KEYLAST = 271;
                public const int WM_INITDIALOG = 272;
                public const int WM_COMMAND = 273;
                public const int WM_SYSCOMMAND = 274;
                public const int WM_TIMER = 275;
                public const int WM_HSCROLL = 276;
                public const int WM_VSCROLL = 277;
                public const int WM_INITMENU = 278;
                public const int WM_INITMENUPOPUP = 279;
                public const int WM_MENUSELECT = 287;
                public const int WM_MENUCHAR = 288;
                public const int WM_ENTERIDLE = 289;
                public const int WM_MENURBUTTONUP = 290;
                public const int WM_MENUDRAG = 291;
                public const int WM_MENUGETOBJECT = 292;
                public const int WM_UNINITMENUPOPUP = 293;
                public const int WM_MENUCOMMAND = 294;
                public const int WM_CHANGEUISTATE = 295;
                public const int WM_UPDATEUISTATE = 296;
                public const int WM_QUERYUISTATE = 297;
                public const int WM_CTLCOLOR = 25;
                public const int WM_CTLCOLORMSGBOX = 306;
                public const int WM_CTLCOLOREDIT = 307;
                public const int WM_CTLCOLORLISTBOX = 308;
                public const int WM_CTLCOLORBTN = 309;
                public const int WM_CTLCOLORDLG = 310;
                public const int WM_CTLCOLORSCROLLBAR = 311;
                public const int WM_CTLCOLORSTATIC = 312;
                public const int WM_MOUSEFIRST = 512;
                public const int WM_MOUSEMOVE = 512;
                public const int WM_LBUTTONDOWN = 513;
                public const int WM_LBUTTONUP = 514;
                public const int WM_LBUTTONDBLCLK = 515;
                public const int WM_RBUTTONDOWN = 516;
                public const int WM_RBUTTONUP = 517;
                public const int WM_RBUTTONDBLCLK = 518;
                public const int WM_MBUTTONDOWN = 519;
                public const int WM_MBUTTONUP = 520;
                public const int WM_MBUTTONDBLCLK = 521;
                public const int WM_MOUSEWHEEL = 522;
                public const int WM_XBUTTONDOWN = 523;
                public const int WM_XBUTTONUP = 524;
                public const int WM_XBUTTONDBLCLK = 525;
                public const int WM_MOUSELAST = 525;
                public const int WM_PARENTNOTIFY = 528;
                public const int WM_ENTERMENULOOP = 529;
                public const int WM_EXITMENULOOP = 530;
                public const int WM_NEXTMENU = 531;
                public const int WM_SIZING = 532;
                public const int WM_CAPTURECHANGED = 533;
                public const int WM_MOVING = 534;
                public const int WM_POWERBROADCAST = 536;
                public const int WM_DEVICECHANGE = 537;
                public const int WM_MDICREATE = 544;
                public const int WM_MDIDESTROY = 545;
                public const int WM_MDIACTIVATE = 546;
                public const int WM_MDIRESTORE = 547;
                public const int WM_MDINEXT = 548;
                public const int WM_MDIMAXIMIZE = 549;
                public const int WM_MDITILE = 550;
                public const int WM_MDICASCADE = 551;
                public const int WM_MDIICONArANGE = 552;
                public const int WM_MDIGETACTIVE = 553;
                public const int WM_MDISETMENU = 560;
                public const int WM_ENTERSIZEMOVE = 561;
                public const int WM_EXITSIZEMOVE = 562;
                public const int WM_DROPFILES = 563;
                public const int WM_MDIREFRESHMENU = 564;
                public const int WM_IME_SETCONTEXT = 641;
                public const int WM_IME_NOTIFY = 642;
                public const int WM_IME_CONTROL = 643;
                public const int WM_IME_COMPOSITIONFULL = 644;
                public const int WM_IME_SELECT = 645;
                public const int WM_IME_CHAR = 646;
                public const int WM_IME_REQUEST = 648;
                public const int WM_IME_KEYDOWN = 656;
                public const int WM_IME_KEYUP = 657;
                public const int WM_MOUSEHOVER = 673;
                public const int WM_MOUSELEAVE = 675;
                public const int WM_NCMOUSELEAVE = 674;
                public const int WM_WTSSESSION_CHANGE = 689;
                public const int WM_TABLET_FIRST = 704;
                public const int WM_TABLET_LAST = 735;
                public const int WM_CUT = 768;
                public const int WM_COPY = 769;
                public const int WM_PASTE = 770;
                public const int WM_CLEAR = 771;
                public const int WM_UNDO = 772;
                public const int WM_RENDERFORMAT = 773;
                public const int WM_RENDERALLFORMATS = 774;
                public const int WM_DESTROYCLIPBOARD = 775;
                public const int WM_DRAWCLIPBOARD = 776;
                public const int WM_PAINTCLIPBOARD = 777;
                public const int WM_VSCROLLCLIPBOARD = 778;
                public const int WM_SIZECLIPBOARD = 779;
                public const int WM_ASKCBFORMATNAME = 780;
                public const int WM_CHANGECBCHAIN = 781;
                public const int WM_HSCROLLCLIPBOARD = 782;
                public const int WM_QUERYNEWPALETTE = 783;
                public const int WM_PALETTEISCHANGING = 784;
                public const int WM_PALETTECHANGED = 785;
                public const int WM_HOTKEY = 786;
                public const int WM_PRINT = 791;
                public const int WM_PRINTCLIENT = 792;
                public const int WM_APPCOMMAND = 793;
                public const int WM_THEMECHANGED = 794;
                public const int WM_HANDHELDFIRST = 856;
                public const int WM_HANDHELDLAST = 863;
                public const int WM_AFXFIRST = 864;
                public const int WM_AFXLAST = 895;
                public const int WM_PENWINFIRST = 896;
                public const int WM_PENWINLAST = 911;
                public const int WM_USER = 1024;
                public const int WM_REFLECT = 8192;
                public const int WM_APP = 32768;
                public const int WS_OVERLAPPED = 0;
                public const int WS_POPUP = -2147483648;
                public const int WS_CHILD = 1073741824;
                public const int WS_MINIMIZE = 536870912;
                public const int WS_VISIBLE = 268435456;
                public const int WS_DISABLED = 134217728;
                public const int WS_CLIPSIBLINGS = 67108864;
                public const int WS_CLIPCHILDREN = 33554432;
                public const int WS_MAXIMIZE = 16777216;
                public const int WS_CAPTION = 12582912;
                public const int WS_BORDER = 8388608;
                public const int WS_DLGFRAME = 4194304;
                public const int WS_VSCROLL = 2097152;
                public const int WS_HSCROLL = 1048576;
                public const int WS_SYSMENU = 524288;
                public const int WS_THICKFRAME = 262144;
                public const int WS_GROUP = 131072;
                public const int WS_TABSTOP = 65536;
                public const int WS_MINIMIZEBOX = 131072;
                public const int WS_MAXIMIZEBOX = 65536;
                public const int WS_TILED = 0;
                public const int WS_ICONIC = 536870912;
                public const int WS_SIZEBOX = 262144;
                public const int WS_TILEDWINDOW = 13565952;
                public const int WS_OVERLAPPEDWINDOW = 13565952;
                public const int WS_POPUPWINDOW = -2138570752;
                public const int WS_CHILDWINDOW = 1073741824;
                public const int WS_EX_DLGMODALFRAME = 1;
                public const int WS_EX_NOPARENTNOTIFY = 4;
                public const int WS_EX_TOPMOST = 8;
                public const int WS_EX_ACCEPTFILES = 16;
                public const int WS_EX_TRANSPARENT = 32;
                public const int WS_EX_MDICHILD = 64;
                public const int WS_EX_TOOLWINDOW = 128;
                public const int WS_EX_WINDOWEDGE = 256;
                public const int WS_EX_CLIENTEDGE = 512;
                public const int WS_EX_CONTEXTHELP = 1024;
                public const int WS_EX_RIGHT = 4096;
                public const int WS_EX_LEFT = 0;
                public const int WS_EX_RTLREADING = 8192;
                public const int WS_EX_LTRREADING = 0;
                public const int WS_EX_LEFTSCROLLBAR = 16384;
                public const int WS_EX_RIGHTSCROLLBAR = 0;
                public const int WS_EX_CONTROLPARENT = 65536;
                public const int WS_EX_STATICEDGE = 131072;
                public const int WS_EX_APPWINDOW = 262144;
                public const int WS_EX_OVERLAPPEDWINDOW = 768;
                public const int WS_EX_PALETTEWINDOW = 392;
                public const int WS_EX_LAYERED = 524288;
                public const int WS_EX_NOINHERITLAYOUT = 1048576;
                public const int WS_EX_LAYOUTRTL = 4194304;
                public const int WS_EX_COMPOSITED = 33554432;
                public const int WS_EX_NOACTIVATE = 134217728;
                public const int CBN_ERRSPACE = -1;
                public const int CBN_SELCHANGE = 1;
                public const int CBN_DBLCLK = 2;
                public const int CBN_SETFOCUS = 3;
                public const int CBN_KILLFOCUS = 4;
                public const int CBN_EDITCHANGE = 5;
                public const int CBN_EDITUPDATE = 6;
                public const int CBN_DROPDOWN = 7;
                public const int CBN_CLOSEUP = 8;
                public const int CBN_SELENDOK = 9;
                public const int CBN_SELENDCANCEL = 10;
                public const int UIS_SET = 1;
                public const int UIS_CLEAR = 2;
                public const int UIS_INITIALIZE = 3;
                public const int UISF_HIDEFOCUS = 1;
                public const int UISF_HIDEACCEL = 2;
                public const int UISF_ACTIVE = 4;
                internal const uint CLSCTX_INPROC_SERVER = 1u;
                public const int CHILDID_SELF = 0;
                public const uint MF_BYCOMMAND = 0u;
                public const uint MF_BYPOSITION = 1024u;
                public const uint MF_ENABLED = 0u;
                public const uint MF_GRAYED = 1u;
                public const uint MF_DISABLED = 2u;
                private const int VBM__BASE = 4096;
                public const int VSINPUT_PROCESSING_MSG = 4242;
                internal const int PSN_APPLY = -202;
                internal const int PSN_KILLACTIVE = -201;
                internal const int PSN_RESET = -203;
                internal const int PSN_SETACTIVE = -200;
                internal const int PSN_QUERYCANCEL = -209;
                internal const int QS_KEY = 1;
                internal const int QS_MOUSEMOVE = 2;
                internal const int QS_MOUSEBUTTON = 4;
                internal const int QS_POSTMESSAGE = 8;
                internal const int QS_TIMER = 16;
                internal const int QS_PAINT = 32;
                internal const int QS_SENDMESSAGE = 64;
                internal const int QS_HOTKEY = 128;
                internal const int QS_ALLPOSTMESSAGE = 256;
                internal const int QS_MOUSE = 6;
                internal const int QS_INPUT = 7;
                internal const int QS_ALLEVENTS = 191;
                internal const int QS_ALLINPUT = 255;
                internal const int QS_EVENT = 8192;
                internal const int PM_NOREMOVE = 0;
                internal const int PM_REMOVE = 1;
                internal const int PM_NOYIELD = 2;
                internal const int MWMO_WAITALL = 1;
                internal const int MWMO_ALERTABLE = 2;
                internal const int MWMO_INPUTAVAILABLE = 4;
                internal const int DLGC_WANTARROWS = 1;
                internal const int DLGC_WANTTAB = 2;
                internal const int DLGC_WANTALLKEYS = 4;
                internal const int DLGC_WANTMESSAGE = 4;
                internal const int DLGC_HASSETSEL = 8;
                internal const int DLGC_DEFPUSHBUTTON = 16;
                internal const int DLGC_UNDEFPUSHBUTTON = 32;
                internal const int DLGC_RADIOBUTTON = 64;
                internal const int DLGC_WANTCHARS = 128;
                internal const int DLGC_STATIC = 256;
                internal const int DLGC_BUTTON = 8192;
                private static int vsmNotifyOwnerActivate;
                public static readonly IntPtr HRGN_NONE = new IntPtr(-1);
                public static readonly IntPtr HWND_TOP = IntPtr.Zero;
                public static readonly IntPtr HWND_BOTTOM = new IntPtr(1);
                public static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
                public static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
                public static readonly IntPtr HWND_BROADCAST = new IntPtr(65535);
                public static int NOTIFYOWNERACTIVATE
                {
                        get
                        {
                                if (NativeMethodsUltimate.vsmNotifyOwnerActivate == 0)
                                {
                                        NativeMethodsUltimate.vsmNotifyOwnerActivate = NativeMethodsUltimate.RegisterWindowMessage("NOTIFYOWNERACTIVATE{A982313C-756C-4da9-8BD0-0C375A45784B}");
                                }
                                return NativeMethodsUltimate.vsmNotifyOwnerActivate;
                        }
                }
                [DllImport("user32.dll", SetLastError = true)]
                [return: MarshalAs(UnmanagedType.Bool)]
                private static extern bool GetWindowPlacement(IntPtr hwnd, WINDOWPLACEMENT lpwndpl);
                public static WINDOWPLACEMENT GetWindowPlacement(IntPtr hwnd)
                {
                        WINDOWPLACEMENT wINDOWPLACEMENT = new WINDOWPLACEMENT();
                        if (NativeMethodsUltimate.GetWindowPlacement(hwnd, wINDOWPLACEMENT))
                        {
                                return wINDOWPLACEMENT;
                        }
                        throw new Win32Exception(Marshal.GetLastWin32Error());
                }
                [DllImport("user32.dll")]
                internal static extern int GetSystemMetrics(int index);
                [DllImport("user32.dll")]
                internal static extern IntPtr GetSystemMenu(IntPtr hwnd, bool bRevert);
                [DllImport("user32.dll")]
                internal static extern int TrackPopupMenuEx(IntPtr hmenu, uint fuFlags, int x, int y, IntPtr hwnd, IntPtr lptpm);
                [DllImport("user32.dll")]
                [return: MarshalAs(UnmanagedType.Bool)]
                internal static extern bool EnableMenuItem(IntPtr menu, uint uIDEnableItem, uint uEnable);
                [DllImport("user32.dll", CharSet = CharSet.Unicode)]
                public static extern int RegisterWindowMessage(string lpString);
                [DllImport("user32.dll")]
                internal static extern short GetKeyState(int vKey);
                internal static bool IsKeyPressed(int vKey)
                {
                        return NativeMethodsUltimate.GetKeyState(vKey) < 0;
                }
                internal static IntPtr MakeParam(int lowWord, int highWord)
                {
                        return new IntPtr((lowWord & 65535) | highWord << 16);
                }
                internal static int GetXLParam(int lParam)
                {
                        return NativeMethodsUltimate.LoWord(lParam);
                }
                internal static int GetYLParam(int lParam)
                {
                        return NativeMethodsUltimate.HiWord(lParam);
                }
                internal static int HiWord(int value)
                {
                        return (int)((short)(value >> 16));
                }
                internal static int LoWord(int value)
                {
                        return (int)((short)(value & 65535));
                }
                [DllImport("user32.dll")]
                [return: MarshalAs(UnmanagedType.Bool)]
                internal static extern bool ScreenToClient(IntPtr hWnd, ref POINT point);
                [DllImport("user32.dll")]
                internal static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);
                [DllImport("user32.dll")]
                [return: MarshalAs(UnmanagedType.Bool)]
                internal static extern bool ClientToScreen(IntPtr hWnd, ref POINT point);
                [DllImport("user32.dll", CharSet = CharSet.Unicode)]
                internal static extern IntPtr DefWindowProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
                [DllImport("user32.dll")]
                [return: MarshalAs(UnmanagedType.Bool)]
                public static extern bool EnumThreadWindows(uint dwThreadId, NativeMethodsUltimate.EnumWindowsProc lpfn, IntPtr lParam);
                [DllImport("user32.dll", SetLastError = true)]
                [return: MarshalAs(UnmanagedType.Bool)]
                public static extern bool PostMessage(IntPtr hWnd, int nMsg, IntPtr wParam, IntPtr lParam);
                [DllImport("User32.dll", CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
                internal static extern IntPtr GetDC(IntPtr hWnd);

                [DllImport("User32.dll")]
                internal static extern IntPtr GetDCEx(IntPtr hWnd, IntPtr hrgnClip, int dwFlags);
                [DllImport("User32.dll", CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
                internal static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
                [DllImport("gdi32.dll")]
                [return: MarshalAs(UnmanagedType.Bool)]
                internal static extern bool DeleteDC(IntPtr hdc);
                [DllImport("gdi32.dll", SetLastError = true)]
                internal static extern IntPtr CreateCompatibleDC(IntPtr hdc);

                [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
                internal static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);
                [DllImport("Gdi32.dll", CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
                internal static extern int GetDeviceCaps(IntPtr hdc, int nIndex);
                [DllImport("Gdi32.dll", CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
                internal static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);
                [DllImport("Gdi32.dll", CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
                internal static extern IntPtr CreateRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);
                [DllImport("Gdi32.dll", CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
                internal static extern IntPtr CreateRectRgnIndirect(ref RECT lprc);
                [DllImport("Gdi32.dll", CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
                private static extern int CombineRgn(IntPtr hrngDest, IntPtr hrgnSrc1, IntPtr hrgnSrc2, int fnCombineMode);
                internal static int CombineRgn(IntPtr hrnDest, IntPtr hrgnSrc1, IntPtr hrgnSrc2, NativeMethodsUltimate.CombineMode combineMode)
                {
                        return NativeMethodsUltimate.CombineRgn(hrnDest, hrgnSrc1, hrgnSrc2, (int)combineMode);
                }
                [DllImport("User32.dll", CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
                internal static extern int SetWindowRgn(IntPtr hWnd, IntPtr hRgn, [MarshalAs(UnmanagedType.Bool)] bool redraw);

                [DllImport("user32.dll")]
                [return: MarshalAs(UnmanagedType.Bool)]
                internal static extern bool IsWindow(IntPtr hWnd);
                [DllImport("user32.dll")]
                [return: MarshalAs(UnmanagedType.Bool)]
                public static extern bool ShowOwnedPopups(IntPtr hwnd, [MarshalAs(UnmanagedType.Bool)] bool fShow);

                [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
                public static extern IntPtr SendMessage(IntPtr hWnd, int nMsg, IntPtr wParam, IntPtr lParam);

                [DllImport("user32.dll")]
                internal static extern IntPtr MonitorFromPoint(POINT pt, int flags);
                [DllImport("user32.dll")]
                public static extern IntPtr GetWindow(IntPtr hwnd, int nCmd);
                [DllImport("user32.dll")]
                [return: MarshalAs(UnmanagedType.Bool)]
                public static extern bool GetWindowInfo(IntPtr hwnd, ref WINDOWINFO pwi);

                [DllImport("user32.dll")]
                [return: MarshalAs(UnmanagedType.Bool)]
                public static extern bool IsWindowVisible(IntPtr hwnd);
                [DllImport("user32.dll")]
                [return: MarshalAs(UnmanagedType.Bool)]
                public static extern bool IsIconic(IntPtr hwnd);
                [DllImport("user32.dll")]
                [return: MarshalAs(UnmanagedType.Bool)]
                public static extern bool IsZoomed(IntPtr hwnd);
                [DllImport("User32", CharSet = CharSet.Auto, ExactSpelling = true)]
                [return: MarshalAs(UnmanagedType.Bool)]
                public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, int flags);

                [DllImport("user32.dll")]
                [return: MarshalAs(UnmanagedType.Bool)]
                internal static extern bool GetCursorPos(ref POINT point);
                internal static System.Windows.Point GetCursorPos()
                {
                        POINT pOINT = new POINT
                        {
                                X = 0,
                                Y = 0
                        };
                        System.Windows.Point result = default(System.Windows.Point);
                        if (NativeMethodsUltimate.GetCursorPos(ref pOINT))
                        {
                                result.X = (double)pOINT.X;
                                result.Y = (double)pOINT.Y;
                        }
                        return result;
                }

                [DllImport("user32.dll")]
                private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

                public static int GetWindowLong(IntPtr hWnd, NativeMethodsUltimate.GWL nIndex)
                {
                        return NativeMethodsUltimate.GetWindowLong(hWnd, (int)nIndex);
                }
                [DllImport("user32.dll")]
                [return: MarshalAs(UnmanagedType.Bool)]
                public static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);
                [DllImport("user32.dll")]
                [return: MarshalAs(UnmanagedType.Bool)]
                public static extern bool GetClientRect(IntPtr hwnd, out RECT lpRect);

                public static IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
                {
                        if (IntPtr.Size == 4)
                        {
                                return NativeMethodsUltimate.SetWindowLongPtr32(hWnd, nIndex, dwNewLong);
                        }
                        return NativeMethodsUltimate.SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
                }
                [DllImport("user32.dll", CharSet = CharSet.Auto, EntryPoint = "SetWindowLong")]
                public static extern IntPtr SetWindowLongPtr32(IntPtr hWnd, int nIndex, IntPtr dwNewLong);
                [DllImport("user32.dll", CharSet = CharSet.Auto, EntryPoint = "SetWindowLongPtr")]
                public static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

                [DllImport("user32.dll", CharSet = CharSet.Unicode)]
                private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
                [DllImport("user32.dll", CharSet = CharSet.Unicode)]
                private static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);
                public static IntPtr SetWindowLongPtr(IntPtr hWnd, NativeMethodsUltimate.GWLP nIndex, IntPtr dwNewLong)
                {
                        if (IntPtr.Size == 8)
                        {
                                return NativeMethodsUltimate.SetWindowLongPtr(hWnd, (int)nIndex, dwNewLong);
                        }
                        return new IntPtr(NativeMethodsUltimate.SetWindowLong(hWnd, (int)nIndex, dwNewLong.ToInt32()));
                }
                public static int SetWindowLong(IntPtr hWnd, NativeMethodsUltimate.GWL nIndex, int dwNewLong)
                {
                        return NativeMethodsUltimate.SetWindowLong(hWnd, (int)nIndex, dwNewLong);
                }

                [DllImport("user32.dll", CharSet = CharSet.Unicode)]
                public static extern ushort RegisterClass(ref WNDCLASS lpWndClass);

                [DllImport("kernel32.dll")]
                public static extern uint GetCurrentThreadId();

                [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
                public static extern IntPtr CreateWindowEx(int dwExStyle, IntPtr classAtom, string lpWindowName, int dwStyle, int x, int y, int nWidth, int nHeight, IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);

                [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
                public static extern IntPtr GetModuleHandle(string moduleName);

                [DllImport("gdi32.dll")]
                public static extern IntPtr CreateSolidBrush(int colorref);
                [DllImport("gdi32.dll")]
                [return: MarshalAs(UnmanagedType.Bool)]
                public static extern bool DeleteObject(IntPtr hObject);

                [DllImport("user32.dll")]
                [return: MarshalAs(UnmanagedType.Bool)]
                public static extern bool DestroyWindow(IntPtr hwnd);
                [DllImport("user32.dll")]
                [return: MarshalAs(UnmanagedType.Bool)]
                public static extern bool UnregisterClass(IntPtr classAtom, IntPtr hInstance);
                [DllImport("user32.dll")]
                [return: MarshalAs(UnmanagedType.Bool)]
                public static extern bool UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDest, ref POINT pptDest, ref SIZE psize, IntPtr hdcSrc, ref POINT pptSrc, uint crKey, [In] ref NativeMethodsUltimate.BLENDFUNCTION pblend, uint dwFlags);
                [DllImport("user32.dll")]
                public static extern bool RedrawWindow(IntPtr hWnd, IntPtr lprcUpdate, IntPtr hrgnUpdate, NativeMethodsUltimate.RedrawWindowFlags flags);
                [DllImport("user32.dll")]
                [return: MarshalAs(UnmanagedType.Bool)]
                public static extern bool FillRect(IntPtr hDC, ref RECT rect, IntPtr hbrush);
                [DllImport("user32.dll")]
                [return: MarshalAs(UnmanagedType.Bool)]
                internal static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, NativeMethodsUltimate.EnumMonitorsDelegate lpfnEnum, IntPtr dwData);
                [DllImport("user32.dll")]
                [return: MarshalAs(UnmanagedType.Bool)]
                internal static extern bool IntersectRect(out RECT lprcDst, [In] ref RECT lprcSrc1, [In] ref RECT lprcSrc2);
                [DllImport("user32.dll")]
                [return: MarshalAs(UnmanagedType.Bool)]
                internal static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO monitorInfo);
                [DllImport("gdi32.dll", SetLastError = true)]
                internal static extern IntPtr CreateDIBSection(IntPtr hdc, ref NativeMethodsUltimate.BITMAPINFO pbmi, uint iUsage, out IntPtr ppvBits, IntPtr hSection, uint dwOffset);
                [DllImport("msimg32.dll")]
                [return: MarshalAs(UnmanagedType.Bool)]
                internal static extern bool AlphaBlend(IntPtr hdcDest, int xoriginDest, int yoriginDest, int wDest, int hDest, IntPtr hdcSrc, int xoriginSrc, int yoriginSrc, int wSrc, int hSrc, NativeMethodsUltimate.BLENDFUNCTION pfn);
                public static int GET_SC_WPARAM(IntPtr wParam)
                {
                        return (int)wParam & 65520;
                }


                [DllImport("user32.dll")]
                public static extern IntPtr SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);

                [DllImport("DwmApi.dll")]
                public static extern int DwmExtendFrameIntoClientArea(
                    IntPtr hwnd,
                    ref MARGINS pMarInset);




		/// <summary>
		/// WINDOWPLACEMENT若是结构，则：[In] ref WINDOWPLACEMENT lpwndpl
		/// 现在是类
		/// WINDOWPLACEMENT必须为public类型
		/// </summary>
		/// <param name="hWnd"></param>
		/// <param name="lpwndpl"></param>
		/// <returns></returns>
                [DllImport("user32.dll")]
		internal static extern bool SetWindowPlacement(IntPtr hWnd, [In] WINDOWPLACEMENT lpwndpl);

		//[DllImport("user32.dll")]
		//internal static extern bool GetWindowPlacement(IntPtr hWnd, out WINDOWPLACEMENT lpwndpl);
        }
}
