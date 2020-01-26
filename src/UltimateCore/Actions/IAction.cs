using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimateCore.Actions
{

        public interface IAction
        {
                void Invoke(object parameter);
                void Invoke(object sender, object parameter);
        }
}
