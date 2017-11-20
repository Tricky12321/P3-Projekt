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
using System.Windows.Shapes;
using P3_Projekt_WPF.Classes.Utilities;

namespace P3_Projekt_WPF
{
    /// <summary>
    /// Interaction logic for AdminNewPassword.xaml
    /// </summary>
    public partial class AdminNewPassword : Window
    {
        public AdminNewPassword()
        {
            InitializeComponent();
            passwordBox_InputPassword.PasswordChanged += new RoutedEventHandler(GreyTextRemover);
            passwordBox_InputPasswordAgain.PasswordChanged += new RoutedEventHandler(GreyTextAgainRemover);
        }

        private void btn_Validate_Click(object sender, RoutedEventArgs e)
        {
            Validate();
        }

        private void GreyTextRemover(object sender, RoutedEventArgs e)
        {
            if (passwordBox_InputPassword.Password.Length == 0)
                GreyText.Visibility = Visibility.Visible;
            else
                GreyText.Visibility = Visibility.Collapsed;
        }
        
        private void GreyTextAgainRemover(object sender, RoutedEventArgs e)
        {
            if (passwordBox_InputPasswordAgain.Password.Length == 0)
                GreyTextAgain.Visibility = Visibility.Visible;
            else
                GreyTextAgain.Visibility = Visibility.Collapsed;
        }

        private void KeyboardHook(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Validate();
            }
        }

        private void Validate()
        {
            if (passwordBox_InputPassword.Password != passwordBox_InputPasswordAgain.Password)
            {
                label_InputTooShort.Visibility = Visibility.Hidden;
                label_InputNotSame.Visibility = Visibility.Visible;
            }
            else if (passwordBox_InputPassword.Password.Length < 1)
            {
                label_InputNotSame.Visibility = Visibility.Hidden;
                label_InputTooShort.Visibility = Visibility.Visible;
            }
            else
            {
                Admin.CreateNewPassword(passwordBox_InputPassword.Password);
                this.Close();
            }
        }
    }
}
