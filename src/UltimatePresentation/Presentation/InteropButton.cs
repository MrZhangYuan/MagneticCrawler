using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace UltimatePresentation.Presentation
{
        public class InteropButton : Button, INonClientArea
        {
                int INonClientArea.HitTest(Point point)
                {
                        return 1;
                }
        }
}
