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
        private StorageRoom storageRoomToEdit;

        public CreateStorageRoom(StorageController stoController)
        {
            ControllerSto = stoController;
            InitializeComponent();
            SetupCreate();
            btn_deleteStorageRoom.Visibility = Visibility.Hidden;
        }

        public CreateStorageRoom(StorageController stoController, StorageRoom stoRoom)
        {
            ControllerSto = stoController;
            storageRoomToEdit = stoRoom;
            InitializeComponent();
            SetupEdit();
        }

        public void SetupCreate()
        {
            output_StorageID.Text = StorageRoom.GetNextID().ToString();
            btn_JustQuit.Click += delegate { this.Close(); };
            btn_SaveAndQuit.Click += delegate
            {
                if (textBox_Name.Text == "" || textBox_descr.Text == "")
                {
                    textBox_Name.BorderBrush = Brushes.DarkGray;
                    textBox_descr.BorderBrush = Brushes.DarkGray;
                    if (textBox_Name.Text == "")
                    {
                        textBox_Name.BorderBrush = Brushes.Red;
                    }
                    if (textBox_descr.Text == "")
                    {
                        textBox_descr.BorderBrush = Brushes.Red;
                    }
                }
                else
                {
                    string storageRoomName = textBox_Name.Text;
                    string storageRoomDescr = textBox_descr.Text;
                    ControllerSto.CreateStorageRoom(storageRoomName, storageRoomDescr);
                    this.Close();
                }
            };
        }

        public void SetupEdit()
        {
            btn_deleteStorageRoom.Visibility = Visibility.Visible;
            output_StorageID.Text = storageRoomToEdit.ID.ToString();
            textBox_Name.Text = storageRoomToEdit.Name;
            textBox_descr.Text = storageRoomToEdit.Description;
            btn_JustQuit.Click += delegate { this.Close(); };
            btn_SaveAndQuit.Click += delegate
            {
                if(textBox_Name.Text == "" || textBox_descr.Text == "")
                {
                    textBox_Name.BorderBrush = Brushes.DarkGray;
                    textBox_descr.BorderBrush = Brushes.DarkGray;
                    if (textBox_Name.Text == "")
                    {
                        textBox_Name.BorderBrush = Brushes.Red;
                    }
                    if (textBox_descr.Text == "")
                    {
                        textBox_descr.BorderBrush = Brushes.Red;
                    }
                }
                else
                {
                    string storageRoomName = textBox_Name.Text;
                    string storageRoomDescr = textBox_descr.Text;
                    ControllerSto.EditStorageRoom(storageRoomToEdit.ID, storageRoomName, storageRoomDescr);
                    this.Close();
                }

            };
            btn_deleteStorageRoom.Click += delegate
            {
                MessageBoxResult results = MessageBox.Show($"Er du sikker på at du vil slette dette lagerrum: {storageRoomToEdit.Name} ?", "Slet lagerrum:", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (results == MessageBoxResult.Yes)
                {
                    ControllerSto.DeleteStorageRoom(storageRoomToEdit.ID);
                    this.Close();
                }
            };
        }
    }
}
