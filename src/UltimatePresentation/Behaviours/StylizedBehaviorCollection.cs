using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace UltimatePresentation.Behaviours
{
        public class StylizedBehaviorCollection : FreezableCollection<Behavior>
        {
                protected override Freezable CreateInstanceCore()
                {
                        return new StylizedBehaviorCollection();
                }
        }
}
