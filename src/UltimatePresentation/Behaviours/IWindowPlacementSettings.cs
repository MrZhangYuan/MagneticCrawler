using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltimatePresentation.Native.Win32;

namespace UltimatePresentation.Behaviours
{
        internal interface IWindowPlacementSettings
        {
		WINDOWPLACEMENT Placement { get; set; }
                void Reload();
                void Save();
        }
}
