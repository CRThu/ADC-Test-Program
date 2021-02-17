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
        // <a:1> <b:2> <c:3>
        Dictionary<string, List<string>> allConfigInfos = new Dictionary<string, List<string>>();
        Dictionary<string, List<string>> selectConfigInfos = new Dictionary<string, List<string>>();

        private void NoiseTestTabUpdateStorageButton_Click(object sender, RoutedEventArgs e)
        {
            allConfigInfos.Clear();
            Dictionary<string, string> configInfo = new Dictionary<string, string>();
            foreach (var SettingsKv in adcDataStorage.AdcSamplesSettingInfo)
            {
                if (!SettingsKv.Key.Equals(AdcDataStorage.tmpAdcSample))
                {
                    configInfo = ConvertInfoToDic(SettingsKv.Value);

                    foreach (var InfoKv in configInfo)
                        if (allConfigInfos.ContainsKey(InfoKv.Key))
                        {
                            if (!allConfigInfos[InfoKv.Key].Contains(InfoKv.Value))
                                allConfigInfos[InfoKv.Key].Add(InfoKv.Value);
                        }
                        else
                        {
                            allConfigInfos.Add(InfoKv.Key, new List<string>() { InfoKv.Value });
                            selectConfigInfos.Add(InfoKv.Key, new List<string>() { });
                        }
                }
            }
            if (allConfigInfos.Count == 0)
                throw new ArgumentNullException();

            // TODO binding
            noiseTestTabPrimaryConfigInfoListBox.ItemsSource = allConfigInfos.Keys.ToList();
            noiseTestTabPrimaryConfigInfoListBox.SelectedIndex = 0;
            noiseTestTabSecondaryConfigInfoListBox.ItemsSource = allConfigInfos[(string)noiseTestTabPrimaryConfigInfoListBox.SelectedItem];
        }

        private void NoiseTestTabPrimaryConfigInfoListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (noiseTestTabPrimaryConfigInfoListBox.SelectedIndex == -1)
                return;
            
            noiseTestTabSecondaryConfigInfoListBox.ItemsSource = allConfigInfos[(string)noiseTestTabPrimaryConfigInfoListBox.SelectedItem];

            noiseTestTabSecondaryConfigInfoListBox.SelectedItems.Clear();
            foreach (string selectItem in selectConfigInfos[(string)noiseTestTabPrimaryConfigInfoListBox.SelectedItem])
                noiseTestTabSecondaryConfigInfoListBox.SelectedItems.Add(selectItem);
        }

        private void NoiseTestTabSecondaryConfigInfoListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (noiseTestTabPrimaryConfigInfoListBox.SelectedIndex == -1 || noiseTestTabSecondaryConfigInfoListBox.SelectedIndex == -1)
                return;

            selectConfigInfos[(string)noiseTestTabPrimaryConfigInfoListBox.SelectedItem] = noiseTestTabSecondaryConfigInfoListBox.SelectedItems.Cast<string>().ToList();

            List<int> selectConfigInfosCount = selectConfigInfos.Select(info => info.Value.Count).ToList();
            if(selectConfigInfosCount.Where(cnt=>cnt.Equals(0)).Count().Equals(0))
            {
                noiseTestTabSingleModeRadioButton.IsEnabled = selectConfigInfosCount.Where(cnt => cnt > 1).Count().Equals(0);
                noiseTestTabChartModeRadioButton.IsEnabled = selectConfigInfosCount.Where(cnt => cnt > 1).Count().Equals(2);
                noiseTestTabReportModeRadioButton.IsEnabled = true;
            }
            else
            {
                noiseTestTabSingleModeRadioButton.IsEnabled = false;
                noiseTestTabChartModeRadioButton.IsEnabled = false;
                noiseTestTabReportModeRadioButton.IsEnabled = false;
            }
            noiseTestTabSingleModeRadioButton.IsChecked = false;
            noiseTestTabChartModeRadioButton.IsChecked = false;
            noiseTestTabReportModeRadioButton.IsChecked = false;

            string str = "";
            foreach (var i in selectConfigInfos)
            {
                str += i.Key;
                str += ": ";
                foreach (var j in i.Value)
                    str += j + "; ";
                str += System.Environment.NewLine;
            }
            noiseTestTabConfigViewTextBox.Text = str;
        }

        private void noiseTestTabSingleModeRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            GridVisibilityChange();
        }

        private void noiseTestTabChartModeRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            GridVisibilityChange();
        }

        private void noiseTestTabReportModeRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            GridVisibilityChange();
        }
        private void GridVisibilityChange()
        {
            SingleModeGrid.Visibility = Visibility.Collapsed;
            ChartModeGrid.Visibility = Visibility.Collapsed;
            ReportModeGrid.Visibility = Visibility.Collapsed;
            if (noiseTestTabSingleModeRadioButton.IsChecked.Equals(true))
                SingleModeGrid.Visibility = Visibility.Visible;
            else if (noiseTestTabChartModeRadioButton.IsChecked.Equals(true))
                ChartModeGrid.Visibility = Visibility.Visible;
            else if (noiseTestTabReportModeRadioButton.IsChecked.Equals(true))
                ReportModeGrid.Visibility = Visibility.Visible;
        }

        // tmp
        public Dictionary<string, string> ConvertInfoToDic(string info)
        {
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
            string[] vs = info.Split(new char[] { ';' });
            for(int i=0;i<vs.Length-1;i+=2)
                keyValuePairs.Add(vs[i], vs[i + 1]);
            return keyValuePairs;
        }
    }
}
