using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace MagneticCrawler
{
        /// <summary>
        /// App.xaml 的交互逻辑
        /// </summary>
        public partial class App : Application
        {
                public App()
                {
                        this.DispatcherUnhandledException += App_DispatcherUnhandledException;
                        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                }

                private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
                {

                }

                private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
                {
                        e.Handled = true;
                }
        }
}
