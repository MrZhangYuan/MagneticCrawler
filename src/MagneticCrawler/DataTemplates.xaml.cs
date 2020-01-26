using MagneticCrawler.Controls;
using MagneticCrawler.Flyouts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace MagneticCrawler
{
        /// <summary>
        /// DataTemplates.xaml 的交互逻辑
        /// </summary>
        public partial class DataTemplates
        {
                public DataTemplates()
                {
                        InitializeComponent();
                }

                private void _MagneticLinkText_MouseDown(object sender, MouseButtonEventArgs e)
                {
                        MagnetItem resultItem = ((TextBlock)sender).Tag as MagnetItem;
                        resultItem.StartMagneticLink();
                        e.Handled = true;
                }

                private void _ThunderLinkText_MouseDown(object sender, MouseButtonEventArgs e)
                {
                        MagnetItem resultItem = ((TextBlock)sender).Tag as MagnetItem;
                        resultItem.StartThunderLink();
                        e.Handled = true;
                }

                private void _BaiduNetDiskLinkText_MouseDown(object sender, MouseButtonEventArgs e)
                {
                        BaiduNetDiskItem resultItem = ((TextBlock)sender).Tag as BaiduNetDiskItem;
                        resultItem.StartBaiduLink();
                        e.Handled = true;
                }

                private void _TitleText_MouseDown(object sender, MouseButtonEventArgs e)
                {
                        TextBlock title = (TextBlock)sender;

                        Expander expander = title.FindName("_fileListExpander") as Expander;

                        expander.IsExpanded = !expander.IsExpanded;
                }

                private void PaginationVirtualizingWrapPanel_PageIndexChanged(object sender, RoutedEventArgs e)
                {
                        PaginationVirtualizingWrapPanel panel = sender as PaginationVirtualizingWrapPanel;
                        if (panel != null
                                && panel.PageIndex + 1 == panel.PageCount)
                        {
                                ItemsControl itemsControl = GetParent<ItemsControl>(panel);
                                if (itemsControl != null)
                                {
                                        ResultPage page = itemsControl.Tag as ResultPage;
                                        if (page != null)
                                        {
                                                page.Start();
                                        }
                                }
                        }
                }

                private void CustomVirtualizingStackPanel_MouseWheelDownEvent(object sender, RoutedEventArgs e)
                {
                        CustomVirtualizingStackPanel panel = sender as CustomVirtualizingStackPanel;
                        if (panel != null)
                        {
                                ScrollViewer scrollViewer = GetParent<ScrollViewer>(panel);

                                if (scrollViewer != null)
                                {
                                        double dVer = scrollViewer.VerticalOffset;
                                        double dViewport = scrollViewer.ViewportHeight;
                                        double dExtent = scrollViewer.ExtentHeight;

                                        //滚动至最底部
                                        if (dVer + dViewport >= dExtent)
                                        {
                                                ResultPage page = scrollViewer.Tag as ResultPage;
                                                if (page != null)
                                                {
                                                        page.Start();
                                                }
                                        }
                                }
                        }
                }

                private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
                {
                        ScrollViewer scrollViewer = sender as ScrollViewer;
                        double dVer = scrollViewer.VerticalOffset;
                        double dViewport = scrollViewer.ViewportHeight;
                        double dExtent = scrollViewer.ExtentHeight;

                        if (dVer == 0
                                && e.Delta > 0)
                        {
                                ScrollViewer parent = GetParent<ScrollViewer>(scrollViewer);
                                if (parent != null)
                                {
                                        parent.LineUp();
                                }
                        }
                        //滚动至最底部
                        else if (dVer + dViewport >= dExtent
                                && e.Delta < 0)
                        {
                                ScrollViewer parent = GetParent<ScrollViewer>(scrollViewer);
                                if (parent != null)
                                {
                                        parent.LineDown();
                                }
                        }
                }

                private void _fileListExpander_Expanded(object sender, RoutedEventArgs e)
                {
                        ResultItem resultItem = ((Expander)sender).Tag as ResultItem;
                        if (resultItem != null)
                        {
                                resultItem.LoadDetail();
                        }
                }

                public static T GetParent<T>(FrameworkElement element) where T : FrameworkElement
                {
                        FrameworkElement parent =
                                element.Parent as FrameworkElement
                                ?? LogicalTreeHelper.GetParent(element) as FrameworkElement
                                ?? VisualTreeHelper.GetParent(element) as FrameworkElement;

                        while (parent != null)
                        {
                                if (parent is Window || parent is T)
                                {
                                        break;
                                }

                                parent = parent.Parent as FrameworkElement
                                ?? LogicalTreeHelper.GetParent(parent) as FrameworkElement
                                ?? VisualTreeHelper.GetParent(parent) as FrameworkElement;
                        }
                        return parent as T;
                }

                private void ItemType_Click(object sender, RoutedEventArgs e)
                {
                        Filter filter = (sender as StackPanel)?.Tag as Filter;

                        if (filter != null)
                        {
                                RadioButton rb = e.OriginalSource as RadioButton;
                                switch (rb.Name)
                                {
                                        case "_typeAll":
                                                filter.ItemType = Crawlers.ItemType.All;
                                                break;
                                        case "_magent":
                                                filter.ItemType = Crawlers.ItemType.Magnet;
                                                break;
                                        case "_baidu":
                                                filter.ItemType = Crawlers.ItemType.BaiduShare;
                                                break;
                                        case "_sub":
                                                filter.ItemType = Crawlers.ItemType.Subtitle;
                                                break;
                                }
                        }
                }

                private void Size_Click(object sender, RoutedEventArgs e)
                {
                        Filter filter = (sender as StackPanel)?.Tag as Filter;

                        if (filter != null)
                        {
                                RadioButton rb = e.OriginalSource as RadioButton;
                                switch (rb.Name)
                                {
                                        case "_sizeAll":
                                                filter.SizeStart = null;
                                                filter.SizeEnd = null;
                                                break;

                                        case "_1":
                                                filter.SizeStart = 0;
                                                filter.SizeEnd = 1 * 1024 * 1024;
                                                break;

                                        case "_1_5":
                                                filter.SizeStart = 1 * 1024 * 1024;
                                                filter.SizeEnd = 5 * 1024 * 1024;
                                                break;

                                        case "_5_10":
                                                filter.SizeStart = 5 * 1024 * 1024;
                                                filter.SizeEnd = 10 * 1024 * 1024;
                                                break;

                                        case "_10":
                                                filter.SizeStart = 10 * 1024 * 1024;
                                                filter.SizeEnd = null;
                                                break;
                                }
                        }
                }

                private void Date_Click(object sender, RoutedEventArgs e)
                {
                        Filter filter = (sender as StackPanel)?.Tag as Filter;

                        if (filter != null)
                        {
                                RadioButton rb = e.OriginalSource as RadioButton;

                                DateTime date = DateTime.Now;

                                switch (rb.Name)
                                {
                                        case "_dateAll":
                                                filter.DateStart = null;
                                                filter.DateEnd = null;
                                                break;

                                        case "_today":
                                                filter.DateEnd = new DateTime(date.Year, date.Month, date.Day);
                                                date = date.AddDays(-1);
                                                filter.DateStart = new DateTime(date.Year, date.Month, date.Day);
                                                break;

                                        case "_week":
                                                filter.DateEnd = new DateTime(date.Year, date.Month, date.Day);
                                                date = date.AddDays(-7);
                                                filter.DateStart = new DateTime(date.Year, date.Month, date.Day);
                                                break;

                                        case "_month":
                                                filter.DateEnd = new DateTime(date.Year, date.Month, date.Day);
                                                date = date.AddMonths(-1);
                                                filter.DateStart = new DateTime(date.Year, date.Month, date.Day);
                                                break;

                                        case "_year":
                                                filter.DateEnd = new DateTime(date.Year, date.Month, date.Day);
                                                date = date.AddYears(-1);
                                                filter.DateStart = new DateTime(date.Year, date.Month, date.Day);
                                                break;
                                }
                        }
                }
        }
}
