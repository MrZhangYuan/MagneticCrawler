using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace UltimatePresentation.Native.Win32
{
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct WNDCLASS
        {
                public uint style;
                public Delegate lpfnWndProc;
                public int cbClsExtra;
                public int cbWndExtra;
                public IntPtr hInstance;
                public IntPtr hIcon;
                public IntPtr hCursor;
                public IntPtr hbrBackground;
                [MarshalAs(UnmanagedType.LPWStr)]
                public string lpszMenuName;
                [MarshalAs(UnmanagedType.LPWStr)]
                public string lpszClassName;
        }
}
