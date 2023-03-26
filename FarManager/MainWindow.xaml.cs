using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FarManager;

public partial class MainWindow : Window
{
    string parent = null;
    public ICommand OpenCommand { get; set; }
    public ICommand DeleteCommand { get; set; }
    public ICommand ButtonCommand { get; set; }
    public ICommand CopyCommand { get; set; }
    public ICommand PasteCommand { get; set; }
    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;
        string path = Path.GetPathRoot(Environment.SystemDirectory);
        DirectoryInfo directory = new(path);

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
        CopyCommand = new RelayCommand(ExecuteCopyCommand);
        PasteCommand = new RelayCommand(ExecutePasteCommand);
    }

    private async void ExecutePasteCommand(object? obj)
    {
        if (obj is ListBox lb)
        {
            string[] file_names = (string[])
            Clipboard.GetData(DataFormats.FileDrop);
            var filename = System.IO.Path.GetFileName(file_names[0]);
            await Task.Delay(50);
            File.Copy(file_names[0], lb.SelectedItem.ToString() + "/" + filename);
        }
    }

    private void ExecuteCopyCommand(object? obj)
    {
        if (obj is ListBox lb)
        {
            List<string> file_list = new List<string>
            {
                lb.SelectedItem.ToString()
            };
            Clipboard.Clear();
            Clipboard.SetData(DataFormats.FileDrop, file_list.ToArray());
            MessageBox.Show("File successfully copied to clipboard");

        }

    }

    private async void ExecuteButtonCommand(object? obj)
    {
        if (obj is ListBox lb)
        {
            if (parent is not null)
            {
                var directory = new DirectoryInfo(parent);
                parent = directory.Parent?.FullName;
                try
                {
                    var directories = directory.GetDirectories();
                    var files = directory.GetFiles();
                    lb.Items.Clear();

                    await Task.Delay(50);
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
    }

    private async void ExecuteDeleteCommand(object? obj)
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

                await Task.Delay(50);
                LeftItems.Items.Remove(item);
                RightItems.Items.Remove(item);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }
    }
    private async void ExecuteOpenCommand(object? obj)
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

                    await Task.Delay(50);
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
