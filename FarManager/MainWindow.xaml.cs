using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

namespace FarManager;

public partial class MainWindow : Window
{
    string parent = null;
    public ICommand OpenCommand { get; set; }
    public ICommand DeleteCommand { get; set; }
    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;
        var path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
        //string path = System.IO.Path.GetPathRoot(Environment.SystemDirectory);
        DirectoryInfo directory = new(path);

        //parent = directory?.Parent.FullName;

        foreach (var d in directory.GetDirectories())
        {
            RightItems.Items.Add(d);
            LeftItems.Items.Add(d);
        }

        foreach (var f in directory.GetFiles())
        {
            RightItems.Items.Add(f);
            LeftItems.Items.Add(f);
        }
        OpenCommand = new RelayCommand(ExecuteOpenCommand);
        DeleteCommand = new RelayCommand(ExecuteDeleteCommand);
    }
    private void ExecuteDeleteCommand(object? obj)
    {
       
        if (obj is ListBox lb)
        {
            var item = lb.SelectedItem;
            try
            {
                if (item is DirectoryInfo directory)
                {
                    try
                    {
                        var dir = directory;
                        dir.Attributes = dir.Attributes & ~FileAttributes.ReadOnly;
                        dir.Delete(true);
                    }
                    catch (IOException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }

                if (item is FileInfo file)
                    file.Delete();


                LeftItems.Items.Remove(item);
                RightItems.Items.Remove(item);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }
    }
    private void ExecuteOpenCommand(object? obj)
    {
        if (obj is ListBox lb)
        {
            MessageBox.Show(lb.SelectedItem.ToString());


        }
    }

}
