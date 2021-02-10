using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
        private void DataTabUpdateListBoxButton_Click(object sender, RoutedEventArgs e)
        {
            dataTabAdcSamplesListBox.Items.Clear();
            dataTabAdcSamplesListBox.Items.Add("tmpAdcSamples");
            foreach (var key in adcDataStorage.AdcSamplesStorageKeys)
                dataTabAdcSamplesListBox.Items.Add(key);

            dataTabAdcSamplesListBox.SelectedIndex = -1;
            dataTabStorageTextBox.Text = "";
            dataTabVoltageTextBox.Text = "";
        }

        private void DataTabAdcSamplesListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (dataTabAdcSamplesListBox.SelectedItem == null)
                return;

            List<ulong> currentList;
            if (dataTabAdcSamplesListBox.SelectedItem.Equals("tmpAdcSamples"))
                currentList = adcDataStorage.ReadTmpAdcSamples();
            else
                currentList = adcDataStorage.ReadDataStorage((string)dataTabAdcSamplesListBox.SelectedItem);
            string data_str = "";
            foreach (var data in currentList)
                data_str += data.ToString() + Environment.NewLine;
            dataTabStorageTextBox.Text = data_str;

            data_str = "";
            List<double> voltageList = adcDataStorage.ConvertVoltages(currentList, true, 2.5, 1, 24);
            foreach (var data in voltageList)
                data_str += data.ToString() + Environment.NewLine;
            dataTabVoltageTextBox.Text = data_str;
        }

        private void DataTabStoreStorageButton_Click(object sender, RoutedEventArgs e)
        {
            // Save File Dialog
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Title = "Save Data File...",
                Filter = "Data File|*.csv",
                InitialDirectory = Directory.GetCurrentDirectory()
            };
            if (saveFileDialog.ShowDialog() == false)
                return;

            adcDataStorage.StoreAllDataToFile(saveFileDialog.FileName, DataStroageExtension.Csv);
        }

        private void dataTabLoadStorageButton_Click(object sender, RoutedEventArgs e)
        {
            // Open File Dialog
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Open Data File...",
                Filter = "Data File|*.csv",
                InitialDirectory = Directory.GetCurrentDirectory()
            };
            if (openFileDialog.ShowDialog() == false)
                return;
            adcDataStorage.LoadAllDataFromFile(openFileDialog.FileName, DataStroageExtension.Csv);
            DataTabUpdateListBoxButton_Click(null, null);
        }
    }
}
