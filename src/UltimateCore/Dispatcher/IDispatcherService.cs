using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;

namespace UltimateCore.Dispatcher
{
        public interface IDispatcherService
        {
                void BeginInvokeAtUI(Action action, DispatcherPriority priority);
                void BeginInvokeAtUI(Action action);
                void InvokeAtUI(Action action, DispatcherPriority priority);
                void InvokeAtUI(Action action);
        }
}
