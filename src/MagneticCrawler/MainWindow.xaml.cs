using MagneticCrawler.Dialogs;
using Microsoft.VisualStudio.PlatformUI.Shell;
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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MagneticCrawler
{
        /// <summary>
        /// MainWindow.xaml 的交互逻辑
        /// </summary>
        public partial class MainWindow
        {
                public static MainWindow Instance
                {
                        get;
                        private set;
                }
                public MainWindow()
                {
                        Instance = this;
                        InitializeComponent();
                        //this.Height = SystemParameters.WorkArea.Height * 0.8;
                        this.Loaded += MainWindow_Loaded;
                        //this._searchList.Items.SortDescriptions.Add(new SortDescription("Order", ListSortDirection.Descending));
                }

                private void MainWindow_Loaded(object sender, RoutedEventArgs e)
                {
                        this.UpdateClipRegion();

                        this._searchView._searchControl.SetFocus();
                }

                protected override void OnSourceInitialized(EventArgs e)
                {
                        base.OnSourceInitialized(e);

                        IntPtr handle = new WindowInteropHelper((Window)this).Handle;
                        ViewManager.Instance.Initialize(this._mainAera);
                        ViewManager.Instance.FloatingWindowManager.OwnerWindow = handle;

                        //停靠的偏好设置
                        ViewManager.Instance.Preferences.ShowAutoHiddenWindowsOnHover = false;
                        ViewManager.Instance.Preferences.DocumentDockPreference = Microsoft.VisualStudio.PlatformUI.Shell.Preferences.DockPreference.DockAtEnd;
                        ViewManager.Instance.Preferences.TabDockPreference = Microsoft.VisualStudio.PlatformUI.Shell.Preferences.DockPreference.DockAtEnd;

                        ViewManager.Instance.Preferences.AllowDocumentTabAutoDocking = false;
                        ViewManager.Instance.Preferences.AllowTabGroupTabAutoDocking = false;

                        ViewManager.Instance.Preferences.EnableIndependentFloatingDocumentGroups = true;
                        ViewManager.Instance.Preferences.EnableIndependentFloatingToolwindows = true;

                        ViewManager.Instance.WindowProfile = WindowProfile.Create("DefaultWindowProfile");
                }

                private void SearchBt_Click(object sender, RoutedEventArgs e)
                {
                        if (!string.IsNullOrEmpty(this._searchControl.Text)
                              && !string.IsNullOrWhiteSpace(this._searchControl.Text))
                        {
                                MainViewModel.Instance.Search(this._searchControl.Text);
                        }
                }

                private void _searchControl_SelectionChanged(object sender, RoutedEventArgs e)
                {
                        SearchSuggestionItem history = this._searchControl.SelectedItem as SearchSuggestionItem;
                        if (history != null)
                        {
                                MainViewModel.Instance.Search(history);
                        }
                }
                private void _searchControl_TextEdited(object sender, RoutedEventArgs e)
                {
                        MainViewModel.Instance.EditTextChanged(this._searchControl.Text);
                }
                private void GlyphButton_Click(object sender, RoutedEventArgs e)
                {
                        this.Close();
                }

                private void _deleteHistoryBt_Click(object sender, RoutedEventArgs e)
                {
                        SearchSuggestionItem history =(sender as Button)?.Tag as SearchSuggestionItem;
                        MainViewModel.Instance.DeleteHistory(history);
                }

                private void _menuBt_Click(object sender, RoutedEventArgs e)
                {
                        this._menuPopup.IsOpen = true;
                }

                private void _menuPopup_Click(object sender, RoutedEventArgs e)
                {
                        this._menuPopup.IsOpen = false;

                        Button bt = e.OriginalSource as Button;

                        switch (bt.Name)
                        {
                                case "_mainViewBt":
                                        MainViewModel.Instance.SearchPageVisibility = Visibility.Visible;
                                        MainViewModel.Instance.MainPageVisibility = Visibility.Hidden;
                                        break;

                                case "_cfgBt":
                                        this.ShowDialog(new SettingsDialog());
                                        break;

                                case "_aboutBt":
                                        this.ShowDialog(new AboutDialog());
                                        break;
                        }
                }
        }
}
