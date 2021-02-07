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
        // Convert to ObservableCollection<T>
        DataTable taskTabTaskTxtList = new DataTable();
        int taskTabFileLineCount = 0;
        int taskTabFileTaskCount = 0;

        private void TaskTabLoadTaskFileButton_Click(object sender, RoutedEventArgs e)
        {
            try
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
                taskTabTaskTxtList.Clear();
                taskTabFileLineCount = 0;
                taskTabFileTaskCount = 0;
                using (StreamReader sr = new StreamReader(TaskTabTaskFileName, Encoding.Default))
                {
                    while (sr.Peek() > 0)
                    {
                        taskTabFileLineCount++;
                        string lineTxt = sr.ReadLine();
                        taskTabTaskTxtList.Rows.Add(taskTabFileLineCount, lineTxt);
                        if (lineTxt.Contains("### TASK.START ###") | lineTxt.Contains("### TASK.END ###"))
                            taskTabFileTaskCount++;
                    }
                    if (taskTabFileTaskCount % 2 != 0)
                        throw new FileFormatException("\"### TASK.START ###\" or \"### TASK.END ###\" is Not Found.");
                    taskTabFileTaskCount /= 2;
                }
                taskTabFileLineCountLabel.Content = taskTabFileLineCount;
                taskTabFileTaskCountLabel.Content = taskTabFileTaskCount;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void TaskTabTaskRunButton_Click(object sender, RoutedEventArgs e)
        {
            string[] ary = Array.ConvertAll(taskTabTaskTxtList.Rows.Cast<DataRow>().ToArray(), r => r["Commands"].ToString());
            TaskTabAutoRunCmds(ary, Path.GetDirectoryName(TaskTabTaskFileName));
        }

        private void TaskTabTaskPauseButton_Click(object sender, RoutedEventArgs e)
        {

        }
        private void TaskTabAutoRunCmds(string[] txtArray, string taskFileDir)
        {
            try
            {
                txtArray = CmdFileDeleteComments(txtArray);

                // AutoRunCommands
                int sleepTime = Convert.ToInt32(cmdIntervalTextBox.Text);
                for (int i = 0; i < txtArray.Length; i++)
                {
                    AutoRunPraseCmd(txtArray[i], taskFileDir, sleepTime);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}