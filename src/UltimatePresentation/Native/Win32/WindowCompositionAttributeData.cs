using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace UltimatePresentation.Native.Win32
{
        [StructLayout(LayoutKind.Sequential)]
        internal struct WindowCompositionAttributeData
        {
                public WindowCompositionAttribute Attribute;
                public IntPtr Data;
                public int SizeOfData;
        }
}
