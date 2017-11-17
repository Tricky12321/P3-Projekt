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
            this.passwordBox_InputPassword.PasswordChanged += new RoutedEventHandler(GreyTextRemover);
        }

        private void btn_Validate_Click(object sender, RoutedEventArgs e)
        {
            if (passwordBox_InputPassword.Password.Length > 0)
            {
                Admin.CreateNewPassword(passwordBox_InputPassword.Password);
                this.Close();
            }
            else
                label_InputNotValid.Visibility = Visibility.Visible;
        }

        private void GreyTextRemover(object sender, RoutedEventArgs e)
        {
            if (passwordBox_InputPassword.Password.Length == 0)
            {
                GreyText.Visibility = Visibility.Collapsed;
            }
            else
                GreyText.Visibility = Visibility.Visible;
        }
    }
}
