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

namespace MagneticCrawler.Views
{
        /// <summary>
        /// MainSearchView.xaml 的交互逻辑
        /// </summary>
        public partial class MainSearchView : UserControl
        {

                public string Text
                {
                        get
                        {
                                return (string)GetValue(TextProperty);
                        }
                        set
                        {
                                SetValue(TextProperty, value);
                        }
                }
                public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(MainSearchView), new PropertyMetadata(string.Empty));

                public MainSearchView()
                {
                        InitializeComponent();
                }

                private void SearchBt_Click(object sender, RoutedEventArgs e)
                {
                        if (!string.IsNullOrEmpty(this._searchControl.Text)
                                && !string.IsNullOrWhiteSpace(this._searchControl.Text))
                        {
                                MainViewModel.Instance.Search(this._searchControl.Text);
                        }
                }

                private void _deleteHistoryBt_Click(object sender, RoutedEventArgs e)
                {
                        SearchSuggestionItem history = (sender as Button)?.Tag as SearchSuggestionItem;
                        MainViewModel.Instance.DeleteHistory(history);
                }

                private void _searchControl_TextEdited(object sender, RoutedEventArgs e)
                {
                        MainViewModel.Instance.EditTextChanged(this._searchControl.Text);
                }

                private void _searchControl_SelectionChanged(object sender, RoutedEventArgs e)
                {
                        if (this.IsLoaded)
                        {
                                SearchSuggestionItem history = this._searchControl.SelectedItem as SearchSuggestionItem;
                                if (history != null)
                                {
                                        MainViewModel.Instance.Search(history);
                                }
                        }
                }
        }
}
