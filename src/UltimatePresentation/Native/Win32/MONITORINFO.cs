using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltimatePresentation.Native.Win32
{
        internal struct MONITORINFO
        {
                public uint cbSize;
                public RECT rcMonitor;
                public RECT rcWork;
                public uint dwFlags;
        }
}
