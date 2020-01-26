using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

namespace MagneticCrawler.Controls
{
        public partial class SearchControl : UserControl
        {
                public bool CheckIfParentNotVisible()
                {
                        FrameworkElement element = this.Parent as FrameworkElement;

                        while (element != null)
                        {
                                if (!element.IsVisible)
                                {
                                        return true;
                                }

                                element = element.Parent as FrameworkElement;
                        }

                        return false;
                }

                public bool IsDropDownOpen
                {
                        get
                        {
                                return (bool)GetValue(IsDropDownOpenProperty);
                        }
                        set
                        {
                                SetValue(IsDropDownOpenProperty, value);
                        }
                }
                public static readonly DependencyProperty IsDropDownOpenProperty = DependencyProperty.Register("IsDropDownOpen", typeof(bool), typeof(SearchControl), new PropertyMetadata(false, IsDropDownOpenChangedCallBack, IsDropDownOpenChangedCoerceCallBack));

                private static object IsDropDownOpenChangedCoerceCallBack(DependencyObject d, object baseValue)
                {
                        SearchControl searchControl = d as SearchControl;
                        bool value = (bool)baseValue;

                        if (!searchControl.CheckIfParentNotVisible())
                        {
                                searchControl._openPopup.IsOpen = value;
                                return baseValue;
                        }

                        searchControl._openPopup.IsOpen = false;
                        return false;
                }

                private static void IsDropDownOpenChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
                {
                        SearchControl searchControl = d as SearchControl;
                }

                public IEnumerable ItemsSource
                {
                        get
                        {
                                return (IEnumerable)GetValue(ItemsSourceProperty);
                        }
                        set
                        {
                                SetValue(ItemsSourceProperty, value);
                        }
                }
                public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(SearchControl), new PropertyMetadata(null));


                public DataTemplate ItemTemplate
                {
                        get
                        {
                                return (DataTemplate)GetValue(ItemTemplateProperty);
                        }
                        set
                        {
                                SetValue(ItemTemplateProperty, value);
                        }
                }
                public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(SearchControl), new PropertyMetadata(null));


                public Style ItemContainerStyle
                {
                        get
                        {
                                return (Style)GetValue(ItemContainerStyleProperty);
                        }
                        set
                        {
                                SetValue(ItemContainerStyleProperty, value);
                        }
                }
                public static readonly DependencyProperty ItemContainerStyleProperty = DependencyProperty.Register("ItemContainerStyle", typeof(Style), typeof(SearchControl), new PropertyMetadata(null));


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
                public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(SearchControl), new PropertyMetadata(string.Empty, TextChangedCallBack));

                private bool _isTextEdted = false;
                private static void TextChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
                {
                        SearchControl searchControl = d as SearchControl;
                        searchControl._isTextEdted = true;
                        if (!searchControl._isInnerUpdated)
                        {
                                searchControl.OnTextEdited();
                        }
                        searchControl._isTextEdted = false;
                }

                public event RoutedEventHandler TextEdited
                {
                        add
                        {
                                base.AddHandler(SearchControl.TextEditedEvent, value);
                        }
                        remove
                        {
                                base.RemoveHandler(SearchControl.TextEditedEvent, value);
                        }
                }
                public static readonly RoutedEvent TextEditedEvent = EventManager.RegisterRoutedEvent("TextEdited", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SearchControl));

                protected virtual void OnTextEdited()
                {
                        RoutedEventArgs e = new RoutedEventArgs(SearchControl.TextEditedEvent, this);
                        base.RaiseEvent(e);
                }

                public object SelectedItem
                {
                        get
                        {
                                return this._suList.SelectedItem;
                        }
                }

                public event RoutedEventHandler SelectionChanged
                {
                        add
                        {
                                base.AddHandler(SearchControl.SelectionChangedEvent, value);
                        }
                        remove
                        {
                                base.RemoveHandler(SearchControl.SelectionChangedEvent, value);
                        }
                }
                public static readonly RoutedEvent SelectionChangedEvent = EventManager.RegisterRoutedEvent("SelectionChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SearchControl));
                protected virtual void OnSelectionChanged()
                {
                        RoutedEventArgs e = new RoutedEventArgs(SearchControl.SelectionChangedEvent, this);
                        base.RaiseEvent(e);
                }

                public SearchControl()
                {
                        InitializeComponent();
                }

                private bool _isInnerUpdated = false;
                private void _suList_SelectionChanged(object sender, SelectionChangedEventArgs e)
                {
                        if (!this._isTextEdted)
                        {
                                this._isInnerUpdated = true;
                                this.Text = this._suList.SelectedItem + "";
                                this._isInnerUpdated = false;
                        }

                        if (!this._isKeyAccess)
                        {
                                this.OnSelectionChanged();
                        }
                }

                private void _suList_MouseUp(object sender, MouseButtonEventArgs e)
                {
                        this.IsDropDownOpen = false;
                }

                private bool _isKeyAccess = false;
                private void _text_KeyDown(object sender, KeyEventArgs e)
                {

                }

                protected override void OnPreviewKeyDown(KeyEventArgs e)
                {
                        this._isKeyAccess = true;
                        switch (e.Key)
                        {
                                case Key.Enter:
                                        this.IsDropDownOpen = false;
                                        break;

                                case Key.Down:
                                case Key.Right:
                                        this._suList.SelectedIndex++;
                                        this._suList.ScrollIntoView(this._suList.SelectedItem);
                                        this.IsDropDownOpen = true;
                                        this._text.SelectionStart = (this._text.Text + "").Length;
                                        break;

                                case Key.Up:
                                case Key.Left:
                                        if (this._suList.SelectedIndex == 0)
                                        {
                                                this._text.Focus();
                                        }
                                        else
                                        {
                                                this._suList.SelectedIndex--;
                                                this._suList.ScrollIntoView(this._suList.SelectedItem);
                                                this.IsDropDownOpen = true;
                                                this._text.SelectionStart = (this._text.Text + "").Length;
                                        }
                                        break;
                        }
                        this._isKeyAccess = false;
                }

                public void SetFocus()
                {
                        this._text.Focus();
                }
        }
}
