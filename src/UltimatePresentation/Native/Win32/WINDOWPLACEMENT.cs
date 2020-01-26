using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace UltimatePresentation.Native.Win32
{
        [StructLayout(LayoutKind.Sequential)]
        public class WINDOWPLACEMENT
        {
                public int length = Marshal.SizeOf(typeof(WINDOWPLACEMENT));
                public int flags;
                public int showCmd;
                public POINT ptMinPosition;
                public POINT ptMaxPosition;
                public RECT rcNormalPosition;
        }
}
