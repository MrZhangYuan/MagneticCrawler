using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UltimatePresentation.Native.Win32;

namespace UltimatePresentation.Native
{
        internal class WindowsAeroHelper2
        {
                [DllImport("dwmapi.dll")]
                public static extern void DwmIsCompositionEnabled(ref int enabledptr);
                [DllImport("dwmapi.dll")]
                public static extern void DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS margin);



                [DllImport("User32.dll")]
                public static extern IntPtr GetSystemMenu(IntPtr hWnd, [MarshalAs(UnmanagedType.Bool)]bool bRevert);



                [DllImport("user32.dll")]
                public static extern bool ReleaseCapture();
                [DllImport("user32.dll")]
                public static extern long TrackPopupMenuEx(IntPtr hMenu, uint un, uint n1, uint n2, IntPtr hWnd, IntPtr lpTPMParams);
                [DllImport("user32.dll", EntryPoint = "PostMessage", CallingConvention = CallingConvention.Winapi)]
                public static extern bool PostMessage(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam);
                [DllImport("user32.dll")]
                public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
                [DllImport("user32.dll")]
                public static extern long GetCursorPos(ref Point lpPoint);

                public const int WM_SYSCOMMAND = 0x0112;
                public const int SC_MOVE = 0xF010;
                public const int HTCAPTION = 0x0002;






                [DllImport("user32.dll")]
                public static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);

                //Win10专用
                public static void EnableBlur(IntPtr HWnd)
                {

                        var accent = new AccentPolicy();
                        accent.AccentState = AccentState.ACCENT_ENABLE_BLURBEHIND;
                        accent.GradientColor = 0xFF0000;

                        var accentStructSize = Marshal.SizeOf(accent);

                        var accentPtr = Marshal.AllocHGlobal(accentStructSize);
                        Marshal.StructureToPtr(accent, accentPtr, false);

                        var data = new WindowCompositionAttributeData();
                        data.Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY;
                        data.SizeOfData = accentStructSize;
                        data.Data = accentPtr;

                        SetWindowCompositionAttribute(HWnd, ref data);

                        Marshal.FreeHGlobal(accentPtr);
                }

                //Vista之后通用
                public static void EnableAero(IntPtr handle)
                {
                        MARGINS mg = new MARGINS();
                        mg.cyBottomHeight = -1;
                        mg.cxLeftWidth = -1;
                        mg.cxRightWidth = -1;
                        mg.cyTopHeight = -1;
                        DwmExtendFrameIntoClientArea(handle, ref mg);
                }

                public static void DragWindow(IntPtr hwnd)
                {
                        ReleaseCapture();
                        SendMessage(hwnd, WM_SYSCOMMAND, SC_MOVE + HTCAPTION, 0);
                }
        }

        internal static class WindowsAeroHelper
        {
                /// <summary>
                /// win10
                /// </summary>
                /// <param name="HWnd"></param>
                /// <param name="hasFrame"></param>
                public static void EnableBlur(IntPtr HWnd, bool hasFrame = true)
                {
                        AccentPolicy accent = new AccentPolicy();
                        accent.AccentState = AccentState.ACCENT_ENABLE_BLURBEHIND;
                        if (hasFrame)
                        {
                                accent.AccentFlags = 0x20 | 0x40 | 0x80 | 0x100;
                        }

                        int accentStructSize = Marshal.SizeOf(accent);

                        IntPtr accentPtr = Marshal.AllocHGlobal(accentStructSize);
                        Marshal.StructureToPtr(accent, accentPtr, false);

                        WindowCompositionAttributeData data = new WindowCompositionAttributeData();
                        data.Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY;
                        data.SizeOfData = accentStructSize;
                        data.Data = accentPtr;

                        NativeMethodsUltimate.SetWindowCompositionAttribute(HWnd, ref data);

                        Marshal.FreeHGlobal(accentPtr);
                }

                //Vista之后通用
                public static void EnableAero(IntPtr handle)
                {
                        MARGINS mg = new MARGINS();
                        mg.cyBottomHeight = -1;
                        mg.cxLeftWidth = -1;
                        mg.cxRightWidth = -1;
                        mg.cyTopHeight = -1;
                        NativeMethodsUltimate.DwmExtendFrameIntoClientArea(handle, ref mg);
                }
        }

        internal class DwmApi
        {
                [DllImport("dwmapi.dll", PreserveSig = false)]
                public static extern void DwmEnableBlurBehindWindow(
                    IntPtr hWnd, DWM_BLURBEHIND pBlurBehind);

                [DllImport("dwmapi.dll", PreserveSig = false)]
                public static extern void DwmExtendFrameIntoClientArea(
                    IntPtr hWnd, MARGINS pMargins);

                [DllImport("dwmapi.dll", PreserveSig = false)]
                public static extern bool DwmIsCompositionEnabled();

                [DllImport("dwmapi.dll", PreserveSig = false)]
                public static extern void DwmEnableComposition(bool bEnable);

                [DllImport("dwmapi.dll", PreserveSig = false)]
                public static extern void DwmGetColorizationColor(
                    out int pcrColorization,
                    [MarshalAs(UnmanagedType.Bool)]out bool pfOpaqueBlend);

                [DllImport("dwmapi.dll", PreserveSig = false)]
                public static extern IntPtr DwmRegisterThumbnail(
                    IntPtr dest, IntPtr source);

                [DllImport("dwmapi.dll", PreserveSig = false)]
                public static extern void DwmUnregisterThumbnail(IntPtr hThumbnail);

                [DllImport("dwmapi.dll", PreserveSig = false)]
                public static extern void DwmUpdateThumbnailProperties(
                    IntPtr hThumbnail, DWM_THUMBNAIL_PROPERTIES props);

                //[DllImport("dwmapi.dll", PreserveSig = false)]
                //public static extern void DwmQueryThumbnailSourceSize(
                //    IntPtr hThumbnail, out Size size);

                [StructLayout(LayoutKind.Sequential)]
                public class DWM_THUMBNAIL_PROPERTIES
                {
                        public uint dwFlags;
                        public RECT rcDestination;
                        public RECT rcSource;
                        public byte opacity;
                        [MarshalAs(UnmanagedType.Bool)]
                        public bool fVisible;
                        [MarshalAs(UnmanagedType.Bool)]
                        public bool fSourceClientAreaOnly;
                        public const uint DWM_TNP_RECTDESTINATION = 0x00000001;
                        public const uint DWM_TNP_RECTSOURCE = 0x00000002;
                        public const uint DWM_TNP_OPACITY = 0x00000004;
                        public const uint DWM_TNP_VISIBLE = 0x00000008;
                        public const uint DWM_TNP_SOURCECLIENTAREAONLY = 0x00000010;
                }

                [StructLayout(LayoutKind.Sequential)]
                public class MARGINS
                {
                        public int cxLeftWidth, cxRightWidth,
                                   cyTopHeight, cyBottomHeight;

                        public MARGINS(int left, int top, int right, int bottom)
                        {
                                cxLeftWidth = left; cyTopHeight = top;
                                cxRightWidth = right; cyBottomHeight = bottom;
                        }
                }

                [StructLayout(LayoutKind.Sequential)]
                public class DWM_BLURBEHIND
                {
                        public uint dwFlags;
                        [MarshalAs(UnmanagedType.Bool)]
                        public bool fEnable;
                        public IntPtr hRegionBlur;
                        [MarshalAs(UnmanagedType.Bool)]
                        public bool fTransitionOnMaximized;

                        public const uint DWM_BB_ENABLE = 0x00000001;
                        public const uint DWM_BB_BLURREGION = 0x00000002;
                        public const uint DWM_BB_TRANSITIONONMAXIMIZED = 0x00000004;
                }

                [StructLayout(LayoutKind.Sequential)]
                public struct RECT
                {
                        public int left, top, right, bottom;

                        public RECT(int left, int top, int right, int bottom)
                        {
                                this.left = left; this.top = top;
                                this.right = right; this.bottom = bottom;
                        }
                }
        }
}
