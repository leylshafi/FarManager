using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
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
    public ICommand ButtonCommand { get; set; }
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
        ButtonCommand = new RelayCommand(ExecuteButtonCommand);
    }

    private void ExecuteButtonCommand(object? obj)
    {
        if (obj is ListBox lb)
        {
            var directory = new DirectoryInfo(parent);
            parent = directory.Parent?.FullName;
            try
            {
                var directories = directory.GetDirectories();
                var files = directory.GetFiles();
                lb.Items.Clear();
                foreach (var d in directories)
                    lb.Items.Add(d);

                foreach (var f in files)
                    lb.Items.Add(f);
            }
            catch (Exception)
            {
                MessageBox.Show("Access Denied");
                return;
            }
        }
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

            if (lb.SelectedItem is DirectoryInfo directory)
            {
                parent = directory.Parent.FullName;
                try
                {
                    var directories = directory.GetDirectories();
                    var files = directory.GetFiles();
                    lb.Items.Clear();
                    foreach (var d in directories)
                        lb.Items.Add(d);

                    foreach (var f in files)
                        lb.Items.Add(f);
                }
                catch (Exception)
                {
                    MessageBox.Show("Access Denied");
                    return;
                }
            }
            else if (lb.SelectedItem is FileInfo file)
            {
                parent = file.Directory.FullName;
                Process.Start(new ProcessStartInfo { FileName = file.FullName, UseShellExecute = true });
            }

        }
    }

}
