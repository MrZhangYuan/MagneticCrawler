using Microsoft.VisualStudio.PlatformUI.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MagneticCrawler
{
        public static class ViewHelper
        {
                public static event Action<object> ViewAllClosed;

                private static int _viewCount;
                public static int ViewCount
                {
                        get
                        {
                                return _viewCount;
                        }
                        set
                        {
                                if (value < 0)
                                {
                                        value = 0;
                                }

                                _viewCount = value;
                                if (value == 0)
                                {
                                        ViewAllClosed?.Invoke(null);
                                }
                        }
                }

                public static void ShowPage(ResultPage newpage)
                {
                        View view = CreateView(newpage);
                        if (ViewManager.Instance.ActiveView == null)
                        {
                                DocumentGroup mainsite = ViewManager.Instance.WindowProfile.Find<DocumentGroup>();
                                mainsite.Parent.Dock(view, DockDirection.Fill);
                        }
                        else
                        {
                                DocumentGroup activeView = ViewManager.Instance.ActiveView.FindAncestorOrSelf<DocumentGroup>();

                                if (activeView != null)
                                {
                                        activeView.Dock(view, DockDirection.Fill);
                                }
                                else
                                {
                                        view.SnapToBookmark(ViewBookmarkType.DocumentWell);
                                }
                        }
                        ViewCount++;
                        view.ShowInFront();
                        view.IsActive = true;
                }

                static int _innerIndex = 1;
                private static View CreateView(ResultPage page)
                {
                        View view = View.Create(ViewManager.Instance.WindowProfile, (_innerIndex++).ToString());
                        view.Title = page.SearchText;
                        view.Content = new ContentControl
                        {
                                Content = page
                        };
                        view.IsVisibleChanged += View_IsVisibleChanged;
                        return view;
                }

                private static void View_IsVisibleChanged(object sender, EventArgs e)
                {
                        View view = sender as View;
                        if (!view.IsVisible)
                        {
                                ResultPage page = view.Content as ResultPage;
                                if (page != null)
                                {
                                        page.Stop();
                                }

                                view.IsVisibleChanged -= View_IsVisibleChanged;
                                view.Detach();
                                ViewCount--;
                        }
                }
        }
}
