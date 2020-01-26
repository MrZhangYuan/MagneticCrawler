using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace UltimatePresentation.Native.Win32
{
        [StructLayout(LayoutKind.Sequential)]
        internal struct AccentPolicy
        {
                public AccentState AccentState;
                public int AccentFlags;
                public int GradientColor;
                public int AnimationId;
        }
}
