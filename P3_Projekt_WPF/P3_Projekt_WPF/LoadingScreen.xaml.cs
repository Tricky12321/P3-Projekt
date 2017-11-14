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
using System.Threading;
using System.Diagnostics;
using System.Windows.Threading;
namespace P3_Projekt_WPF
{
    /// <summary>
    /// Interaction logic for LoadingScreen.xaml
    /// </summary>
    public partial class LoadingScreen : Window
    {
        private static event Action CloseRegionWindows = delegate { }; // won't have to check for null
        private Thread _startThread;
        public LoadingScreen()
        {
            InitializeComponent();
            Loaded += MyWindow_Loaded;
        }

        private void CloseWindow()
        {
            if (!MainWindow.runLoading)
            {
                this.Close();
            }
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
             CloseWindow();
        }

        private void MyWindow_Loaded(object sender, RoutedEventArgs e)
        {
            DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            dispatcherTimer.Start();
        }
    }
}
