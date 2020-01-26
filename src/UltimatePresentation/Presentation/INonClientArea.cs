using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltimatePresentation.Presentation
{
        public interface INonClientArea
        {
                int HitTest(System.Windows.Point point);
        }
}
