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
using System.Collections.Concurrent;
using P3_Projekt_WPF.Classes.Database;
using P3_Projekt_WPF.Classes.Utilities;
using P3_Projekt_WPF.Classes.Exceptions;
using P3_Projekt_WPF.Classes;

namespace P3_Projekt_WPF
{
    /// <summary>
    /// Interaction logic for CreateStorageRoom.xaml
    /// </summary>
    public partial class CreateStorageRoom : Window
    {
        public StorageController ControllerSto;
        private ConcurrentDictionary<int, StorageRoom> _storageRooms;
        private MainWindow _mainWindow = null;

        public CreateStorageRoom(StorageController stoController, MainWindow mainWin)
        {
            this._mainWindow = mainWin;
            ControllerSto = stoController;
            InitializeComponent();
            setupCreate();
        }

        public CreateStorageRoom()
        {
            InitializeComponent();
        }

        public void setupCreate()
        {
            output_StorageID.Text = StorageRoom.GetNextID().ToString();
            btn_JustQuit.Click += delegate { this.Close(); };
            btn_SaveAndQuit.Click += delegate
            {
                string storageRoomName = textBox_Name.Text;
                string storageRoomDescr = textBox_descr.Text;
                ControllerSto.CreateStorageRoom(storageRoomName, storageRoomDescr);
                this.Close();
            };
        }
    }
}
