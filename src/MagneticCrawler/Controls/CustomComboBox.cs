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

namespace MagneticCrawler.Controls
{
        public class CustomComboBox : ComboBox
        {
                public event RoutedEventHandler TextChanged
                {
                        add
                        {
                                base.AddHandler(CustomComboBox.TextChangedEvent, value);
                        }
                        remove
                        {
                                base.RemoveHandler(CustomComboBox.TextChangedEvent, value);
                        }
                }
                public static readonly RoutedEvent TextChangedEvent = EventManager.RegisterRoutedEvent("TextChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(CustomComboBox));
                protected virtual void OnTextChanged()
                {
                        RoutedEventArgs e = new RoutedEventArgs(CustomComboBox.TextChangedEvent, this);
                        base.RaiseEvent(e);
                }


                public event RoutedEventHandler EditTextKeyDown
                {
                        add
                        {
                                base.AddHandler(CustomComboBox.EditTextKeyDownEvent, value);
                        }
                        remove
                        {
                                base.RemoveHandler(CustomComboBox.EditTextKeyDownEvent, value);
                        }
                }
                public static readonly RoutedEvent EditTextKeyDownEvent = EventManager.RegisterRoutedEvent("EditTextKeyDown", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(CustomComboBox));
                protected virtual void OnEditTextKeyDown()
                {
                        RoutedEventArgs e = new RoutedEventArgs(CustomComboBox.EditTextKeyDownEvent, this);
                        base.RaiseEvent(e);
                }

                static CustomComboBox()
                {
                        DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomComboBox), new FrameworkPropertyMetadata(typeof(CustomComboBox)));
                }

                private TextBox _editTextBox = null;
                public override void OnApplyTemplate()
                {
                        base.OnApplyTemplate();
                        _editTextBox = this.GetTemplateChild("PART_EditableTextBox") as TextBox;
                        if (_editTextBox!=null)
                        {
                                this._editTextBox.TextChanged += _editTextBox_TextChanged;
                                this._editTextBox.AddHandler(TextBox.KeyDownEvent, new KeyEventHandler(this._editTextBox_KeyDown), true);
                        }
                }
                private void _editTextBox_KeyDown(object sender, KeyEventArgs e)
                {
                        this.OnEditTextKeyDown();
                }

                private void _editTextBox_TextChanged(object sender, TextChangedEventArgs e)
                {
                        this.OnTextChanged();
                }
        }
}
