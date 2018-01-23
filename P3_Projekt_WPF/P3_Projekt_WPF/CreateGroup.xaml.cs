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
    public partial class CreateGroup : Window
    {
        public StorageController ControllerSto;
        private ConcurrentDictionary<int, Group> _groups;
        private Group groupToEdit;

        public CreateGroup(StorageController stoController)
        {
            ControllerSto = stoController;
            InitializeComponent();
            SetupCreate();
            btn_deleteGroup.Visibility = Visibility.Hidden;
        }

        public CreateGroup(StorageController stoController, Group groupToEdit)
        {
            ControllerSto = stoController;
            this.groupToEdit = groupToEdit;
            InitializeComponent();
            SetupEdit();
        }

        public void SetupCreate()
        {
            output_GroupID.Text = Group.GetNextID().ToString();
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
                    string groupName = textBox_Name.Text;
                    string groupDescr = textBox_descr.Text;
                    ControllerSto.CreateGroup(groupName, groupDescr);
                    this.Close();
                }
            };
        }

        public void SetupEdit()
        {
            btn_deleteGroup.Visibility = Visibility.Visible;
            output_GroupID.Text = groupToEdit.ID.ToString();
            textBox_Name.Text = groupToEdit.Name;
            textBox_descr.Text = groupToEdit.Description;
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
                    string GroupName = textBox_Name.Text;
                    string GroupDesc = textBox_descr.Text;
                    ControllerSto.EditGroup(groupToEdit.ID, GroupName, GroupDesc);
                    this.Close();
                }

            };
            btn_deleteGroup.Click += delegate
            {
                MessageBoxResult results = MessageBox.Show($"Er du sikker på at du vil slette denne gruppe: {groupToEdit.Name} ?", "Slet gruppe:", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (results == MessageBoxResult.Yes)
                {
                    ControllerSto.DeleteGroup(groupToEdit.ID);
                    this.Close();
                }
            };
        }
    }
}
