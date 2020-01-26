using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltimatePresentation.Native.Win32
{
        internal struct WINDOWINFO
        {
                public int cbSize;
                public RECT rcWindow;
                public RECT rcClient;
                public int dwStyle;
                public int dwExStyle;
                public uint dwWindowStatus;
                public uint cxWindowBorders;
                public uint cyWindowBorders;
                public ushort atomWindowType;
                public ushort wCreatorVersion;
        }
}
