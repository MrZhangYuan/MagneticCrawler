using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace UltimatePresentation.Native.Win32
{
        [StructLayout(LayoutKind.Sequential)]
        internal struct MARGINS
        {
                /// <summary>Width of left border that retains its size.</summary>
                public int cxLeftWidth;
                /// <summary>Width of right border that retains its size.</summary>
                public int cxRightWidth;
                /// <summary>Height of top border that retains its size.</summary>
                public int cyTopHeight;
                /// <summary>Height of bottom border that retains its size.</summary>
                public int cyBottomHeight;
        }
}
