using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace UltimatePresentation.Native.Win32
{
        [StructLayout(LayoutKind.Sequential)]
        internal class WINDOWPOS
        {
                public IntPtr hwnd;
                public IntPtr hwndInsertAfter;
                public int x;
                public int y;
                public int cx;
                public int cy;
                public uint flags;
        }
}
