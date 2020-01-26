using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace UltimatePresentation.Native.Win32
{
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        internal struct SIZE
        {
                [ComAliasName("Microsoft.VisualStudio.OLE.Interop.LONG")]
                public int cx;
                [ComAliasName("Microsoft.VisualStudio.OLE.Interop.LONG")]
                public int cy;
        }
}
