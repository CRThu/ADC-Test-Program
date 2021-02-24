using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ADC_CDC_CONTROLLER
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            taskTabTaskTxtList.Columns.Add("Lines");
            taskTabTaskTxtList.Columns.Add("Commands");
            taskTabTaskTxtListView.DataContext = taskTabTaskTxtList;

            bytesPerCode = Convert.ToInt32(bytesPerCodeTextBox.Text);

            //serialPortCommInfoTextBox.DataContext = log1;
            serialPortCommInfoTextBox.IsUndoEnabled = false;

            // About Tab
            string appVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            string buildTime = File.GetLastWriteTime(this.GetType().Assembly.Location).ToString();
            var assembly = Assembly.GetEntryAssembly();
            if (assembly == null)
                assembly = new StackTrace().GetFrames().Last().GetMethod().Module.Assembly;
            var debuggableAttribute = assembly.GetCustomAttribute<DebuggableAttribute>();
            bool isDebugMode = debuggableAttribute.DebuggingFlags.HasFlag(DebuggableAttribute.DebuggingModes.EnableEditAndContinue);
            string envVersion = Environment.Version.ToString();
            AboutTabAppVersionTextBlock.Text = "App Version:\t" + appVersion + Environment.NewLine
                + "Build Time:\t" + buildTime + Environment.NewLine
                + "Build Mode:\t" + (isDebugMode ? "Debug" : "Release") + Environment.NewLine
                + ".NET Version:\t" + envVersion + Environment.NewLine
                + "Hardware Info:" + Environment.NewLine;
            string[] coms = HardwareInfoUtil.GetSerialPortName();
            foreach(var com in coms)
                AboutTabAppVersionTextBlock.Text += com+Environment.NewLine;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            InitAdcSettings();
            AdcPrimarySettingsListBox.ItemsSource = AdcSettings.Select(t => t.ConfigName).ToList();
            AdcPrimarySettingsListBox.SelectedIndex = 0;
            UpdateAdcSecondarySettingsListBox();

            string[] portList = System.IO.Ports.SerialPort.GetPortNames();
            for (int i = 0; i < portList.Length; ++i)
            {
                string name = portList[i];
                serialPortComboBox1.Items.Add(name);
            }
            if (portList.Length > 0)
                serialPortComboBox1.SelectedIndex = 0;
        }

    }
}
