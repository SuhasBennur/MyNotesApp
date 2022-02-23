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

namespace MyNotes.View
{
    /// <summary>
    /// Interaction logic for BindablePwdBox.xaml
    /// </summary>
    public partial class BindablePwdBox : UserControl
    {

        private bool _isPasswordChanging;
        public string Password
        {
            get { return (string)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register("Password", typeof(string), typeof(BindablePwdBox),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,PasswordPropertyChanged, null, false, UpdateSourceTrigger.PropertyChanged));

        private static void PasswordPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(d is BindablePwdBox passwordBox)
            {
                passwordBox.UpdatePassword();
            }
        }

        public BindablePwdBox()
        {
            InitializeComponent();
        }

        private void Pwdbox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            _isPasswordChanging=true;
            Password= passwordBox.Password;
            _isPasswordChanging=false;
        }
        private void UpdatePassword()
        {
            if(!_isPasswordChanging)
            {
                passwordBox.Password = Password;
            }
        }

        private void ShowPassword_Checked(object sender, RoutedEventArgs e)
        {
            Password = passwordBox.Password;
            passwordBox.Visibility = Visibility.Collapsed;
            passwordBox.Visibility = Visibility.Visible;
        }

        private void ShowPassword_Unchecked(object sender, RoutedEventArgs e)
        {
            passwordBox.Password = Password;
            passwordBox.Visibility = Visibility.Collapsed;
            passwordBox.Visibility = Visibility.Visible;
        }
    }
}
