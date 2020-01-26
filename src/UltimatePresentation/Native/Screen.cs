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
        public static class Screen
        {
                internal static void FindMaximumSingleMonitorRectangle(RECT windowRect, out RECT screenSubRect, out RECT monitorRect)
                {
                        List<RECT> rects = new List<RECT>();
                        NativeMethodsUltimate.EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, delegate(IntPtr hMonitor, IntPtr hdcMonitor, ref RECT rect, IntPtr lpData)
                        {
                                MONITORINFO mONITORINFO = default(MONITORINFO);
                                mONITORINFO.cbSize = (uint)Marshal.SizeOf(typeof(MONITORINFO));
                                NativeMethodsUltimate.GetMonitorInfo(hMonitor, ref mONITORINFO);
                                rects.Add(mONITORINFO.rcWork);
                                return true;
                        }, IntPtr.Zero);
                        long num = 0L;
                        screenSubRect = new RECT
                        {
                                Left = 0,
                                Right = 0,
                                Top = 0,
                                Bottom = 0
                        };
                        monitorRect = new RECT
                        {
                                Left = 0,
                                Right = 0,
                                Top = 0,
                                Bottom = 0
                        };
                        foreach (RECT current in rects)
                        {
                                RECT rECT = current;
                                RECT rECT2;
                                NativeMethodsUltimate.IntersectRect(out rECT2, ref rECT, ref windowRect);
                                long num2 = (long)(rECT2.Width * rECT2.Height);
                                if (num2 > num)
                                {
                                        screenSubRect = rECT2;
                                        monitorRect = current;
                                        num = num2;
                                }
                        }
                }
                internal static void FindMaximumSingleMonitorRectangle(Rect windowRect, out Rect screenSubRect, out Rect monitorRect)
                {
                        RECT windowRect2 = new RECT(windowRect);
                        RECT rECT;
                        RECT rECT2;
                        Screen.FindMaximumSingleMonitorRectangle(windowRect2, out rECT, out rECT2);
                        screenSubRect = new Rect(rECT.Position, rECT.Size);
                        monitorRect = new Rect(rECT2.Position, rECT2.Size);
                }
                internal static void FindMonitorRectsFromPoint(System.Windows.Point point, out Rect monitorRect, out Rect workAreaRect)
                {
                        IntPtr intPtr = NativeMethodsUltimate.MonitorFromPoint(new POINT
                        {
                                X = (int)point.X,
                                Y = (int)point.Y
                        }, 2);
                        monitorRect = new Rect(0.0, 0.0, 0.0, 0.0);
                        workAreaRect = new Rect(0.0, 0.0, 0.0, 0.0);
                        if (intPtr != IntPtr.Zero)
                        {
                                MONITORINFO mONITORINFO = default(MONITORINFO);
                                mONITORINFO.cbSize = (uint)Marshal.SizeOf(typeof(MONITORINFO));
                                NativeMethodsUltimate.GetMonitorInfo(intPtr, ref mONITORINFO);
                                monitorRect = new Rect(mONITORINFO.rcMonitor.Position, mONITORINFO.rcMonitor.Size);
                                workAreaRect = new Rect(mONITORINFO.rcWork.Position, mONITORINFO.rcWork.Size);
                        }
                }
        }

}
