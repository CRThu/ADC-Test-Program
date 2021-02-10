using System;
using System.Collections.Generic;
using System.Linq;
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
