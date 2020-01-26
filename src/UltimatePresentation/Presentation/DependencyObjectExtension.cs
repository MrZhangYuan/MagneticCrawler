using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace UltimatePresentation.Presentation
{
        static class DependencyObjectExtension
        {
                public static bool IsConnectedToPresentationSource(this DependencyObject obj)
                {
                        return PresentationSource.FromDependencyObject(obj) != null;
                }
                public static DependencyObject GetVisualOrLogicalParent(this DependencyObject sourceElement)
                {
                        if (sourceElement == null)
                        {
                                return null;
                        }
                        if (sourceElement is Visual)
                        {
                                return VisualTreeHelper.GetParent(sourceElement) ?? LogicalTreeHelper.GetParent(sourceElement);
                        }
                        return LogicalTreeHelper.GetParent(sourceElement);
                }
        }

}
