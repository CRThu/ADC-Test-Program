using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ADC_CDC_CONTROLLER
{
    /// <summary>
    /// MainWindow.xaml µÄ½»»¥Âß¼­
    /// </summary>
    public partial class MainWindow : Window
    {
        string TaskTabTaskFileName;
        List<string> taskTabTaskTxtList = new List<string>();

        private void TaskTabLoadTaskFileButton_Click(object sender, RoutedEventArgs e)
        {
            // Open File Dialog
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Open Tasks File...",
                Filter = "Text File|*.txt",
                InitialDirectory = Directory.GetCurrentDirectory(),
            };
            if (openFileDialog.ShowDialog() == false)
                return;
            taskTabTaskFileTextBox.Text = openFileDialog.SafeFileName;
            TaskTabTaskFileName = openFileDialog.FileName;

            // Open File
            using (StreamReader sr = new StreamReader(TaskTabTaskFileName, Encoding.Default))
            {
                int lineCount = 0;
                while (sr.Peek() > 0)
                {
                    lineCount++;
                    string temp = sr.ReadLine();
                    taskTabTaskTxtList.Add(temp);
                }
            }
            taskTabTaskTxtListView.ItemsSource = taskTabTaskTxtList;
        }
    }
}