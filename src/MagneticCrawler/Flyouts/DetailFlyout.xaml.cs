using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MagneticCrawler.Flyouts
{
        public class DetailFlyoutParameter
        {
                public ResultItem ResultItem { get; set; }
        }

        public class DetailFlyoutResult
        {

        }
        /// <summary>
        /// DetailFlyout.xaml 的交互逻辑
        /// </summary>
        public partial class DetailFlyout
        {
                private DetailFlyout()
                {
                        InitializeComponent();
                }

                public static Task<DetailFlyoutResult> ShowResult(DetailFlyoutParameter parameter)
                {
                        TaskCompletionSource<DetailFlyoutResult> tcs = new TaskCompletionSource<DetailFlyoutResult>();
                        DetailFlyout flyout = new DetailFlyout();
                        flyout.DataContext = parameter.ResultItem;
                        flyout._web.Source = new Uri(parameter.ResultItem.DetailUrl);

                        //parameter.ResultItem.LoadDetail();

                        MainWindow.Instance.ShowFlyout(flyout);

                        tcs.TrySetResult(new DetailFlyoutResult
                        {

                        });

                        return tcs.Task;
                }
        }
}
