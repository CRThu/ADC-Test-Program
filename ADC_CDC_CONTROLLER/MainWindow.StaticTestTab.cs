using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

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
        DataTable singleModeDataTable, chartModeDataTable;

        private void NoiseTestTabUpdateStorageButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                allConfigInfos.Clear();
                selectConfigInfos.Clear();
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void NoiseTestTabPrimaryConfigInfoListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (noiseTestTabPrimaryConfigInfoListBox.SelectedIndex == -1)
                return;

            noiseTestTabSecondaryConfigInfoListBox.SelectedItems.Clear();

            noiseTestTabSecondaryConfigInfoListBox.ItemsSource = allConfigInfos[(string)noiseTestTabPrimaryConfigInfoListBox.SelectedItem];

            foreach (string selectItem in selectConfigInfos[(string)noiseTestTabPrimaryConfigInfoListBox.SelectedItem])
                noiseTestTabSecondaryConfigInfoListBox.SelectedItems.Add(selectItem);
        }

        private void NoiseTestTabSecondaryConfigInfoListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (noiseTestTabPrimaryConfigInfoListBox.SelectedIndex == -1 || noiseTestTabSecondaryConfigInfoListBox.SelectedIndex == -1)
                return;

            selectConfigInfos[(string)noiseTestTabPrimaryConfigInfoListBox.SelectedItem] = noiseTestTabSecondaryConfigInfoListBox.SelectedItems.Cast<string>().ToList();

            List<int> selectConfigInfosCount = selectConfigInfos.Select(info => info.Value.Count).ToList();
            if (selectConfigInfosCount.Where(cnt => cnt.Equals(0)).Count().Equals(0))
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
            {
                SingleModeGrid.Visibility = Visibility.Visible;
                SingleModeDataTableUpdate();
            }
            else if (noiseTestTabChartModeRadioButton.IsChecked.Equals(true))
            {
                ChartModeGrid.Visibility = Visibility.Visible;
                ChartModeDataTableUpdate();
            }
            else if (noiseTestTabReportModeRadioButton.IsChecked.Equals(true))
            {
                ReportModeGrid.Visibility = Visibility.Visible;
                MessageBox.Show("TODO");
            }
        }

        private void StaticTestTabSingleModeCalcResolutionButton_Click(object sender, RoutedEventArgs e)
        {
            bool isBipolar = (bool)staticTestTabSingleModeisBipolarCheckBox.IsChecked;
            double vRef = Convert.ToDouble(staticTestTabSingleModeVrefTextBox.Text);
            double gain = Convert.ToDouble(staticTestTabSingleModeGainTextBox.Text);
            int adcBits = Convert.ToInt32(staticTestTabSingleModeAdcBitsTextBox.Text);
            SingleModeDataTableCalc(isBipolar, vRef, gain, adcBits);
        }

        private void StaticTestTabChartModeRotationCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            ChartModeDataTableUpdate();
        }

        private void StaticTestTabChartModeRotationCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            ChartModeDataTableUpdate();
        }

        private void StaticTestTabChartModeCalcResolutionButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (var sample in adcDataStorage.AdcSamplesStorage)
                {
                    double effRes = AdcPerfCalcUtil.EffResolution(sample.Value, 24);
                    double noiseRes = AdcPerfCalcUtil.NoiseFreeResolution(sample.Value, 24);
                    Dictionary<string, string> sampleSettingInfo = ConvertInfoToDic(adcDataStorage.AdcSamplesSettingInfo[sample.Key]);
                    bool isSelected = true;
                    foreach (var kv in sampleSettingInfo)
                    {

                        if (!selectConfigInfos.ContainsKey(kv.Key))
                        {
                            isSelected = false;
                            break;
                        }
                        if (!selectConfigInfos[kv.Key].Contains(kv.Value))
                        {
                            isSelected = false;
                            break;
                        }
                    }

                    if (isSelected)
                    {
                        Dictionary<string, List<string>> diffValueDic = selectConfigInfos.Where(info => info.Value.Count > 1).ToDictionary(k => k.Key, k => k.Value);
                        List<string> str = new List<string>();
                        foreach (var kv in diffValueDic)
                        {
                            if(sampleSettingInfo.ContainsKey(kv.Key))
                                str.Add(sampleSettingInfo[kv.Key]);
                        }
                        if (!str.Count.Equals(2))
                            continue;
                        if (staticTestTabChartModeRotationCheckBox.IsChecked.Equals(false))
                        {
                            DataTableUtil.DataTableAddData(chartModeDataTable, str.First(), str.Last(), effRes.ToString("f1") + "(" + noiseRes.ToString("f1") + ")");
                        }
                        else
                        {
                            DataTableUtil.DataTableAddData(chartModeDataTable, str.Last(), str.First(), effRes.ToString("f1") + "(" + noiseRes.ToString("f1") + ")");
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void StaticTestTabChartModeStoreCsvButton_Click(object sender, RoutedEventArgs e)
        {
            Dictionary<string, List<string>> sameValueDic = selectConfigInfos.Where(info => info.Value.Count == 1).ToDictionary(k => k.Key, k => k.Value);
            Dictionary<string, List<string>> diffValueDic = selectConfigInfos.Where(info => info.Value.Count > 1).ToDictionary(k => k.Key, k => k.Value);
            //MessageBox.Show("sameKey = "+string.Join(", ", sameValueDic.Keys) +"\ntableKey = "+string.Join(", ", diffValueDic.Keys));

            string str = "";
            foreach (var i in sameValueDic)
                str += i.Key + "=" + i.Value.First() + ";";
            // Save File Dialog
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Title = "Save Report File...",
                Filter = "Report File|*.csv",
                FileName = "StaticTestReport;" + str,
                InitialDirectory = Directory.GetCurrentDirectory()
            };
            if (saveFileDialog.ShowDialog() == false)
                return;

            DataTableUtil.DatatableToCSV(noiseTestTabConfigViewTextBox.Text.Replace("\r", "").Replace("\n", ""), chartModeDataTable, saveFileDialog.FileName);
        }

        void SingleModeDataTableUpdate()
        {
            string rowName = "Sample", colName = "Pref";
            string[] rows = { "Code(LSB)", "Voltage(uVolt)" };
            string[] cols = { "LSB", "Min", "Max", "Avg|Offset Err", "Std|RMS Noise", "Peak Noise", "Peak Noise Calc", "Eff Res(b)", "NoiseFree Res(b)", "NoiseFree Res Calc(b)" };

            singleModeDataTable = DataTableUtil.DataTableInit(colName, cols, rowName, rows);

            staticTestTabSingleModeDataGrid.ItemsSource = null;
            staticTestTabSingleModeDataGrid.ItemsSource = singleModeDataTable.DefaultView;
        }

        void SingleModeDataTableCalc(bool isBipolar, double vRef, double gain, int adcBits)
        {
            try
            {
                Dictionary<string, string> selectSingleSampleInfo = selectConfigInfos.Select(r => new KeyValuePair<string, string>(r.Key, r.Value.First())).ToDictionary(k => k.Key, k => k.Value);
                bool hasSample = false;
                foreach (var sample in adcDataStorage.AdcSamplesStorage)
                {
                    bool isEqual = true;
                    Dictionary<string, string> sampleSettingInfo = ConvertInfoToDic(adcDataStorage.AdcSamplesSettingInfo[sample.Key]);
                    foreach (var kv in selectSingleSampleInfo)
                    {
                        if (!sampleSettingInfo.ContainsKey(kv.Key))
                        {
                            isEqual = false;
                            break;
                        }
                        if (!sampleSettingInfo[kv.Key].Equals(kv.Value))
                        {
                            isEqual = false;
                            break;
                        }
                    }

                    if (isEqual)
                    {
                        hasSample = true;
                        // lsb: Min Max Avg nrms npp nppcalc
                        DataTableUtil.DataTableAddData(singleModeDataTable, 0, 0, 1.ToString());
                        DataTableUtil.DataTableAddData(singleModeDataTable, 1, 0, AdcPerfCalcUtil.MinCode(sample.Value).ToString());
                        DataTableUtil.DataTableAddData(singleModeDataTable, 2, 0, AdcPerfCalcUtil.MaxCode(sample.Value).ToString());
                        DataTableUtil.DataTableAddData(singleModeDataTable, 3, 0, AdcPerfCalcUtil.AvgCode(sample.Value).ToString("f3"));
                        DataTableUtil.DataTableAddData(singleModeDataTable, 4, 0, AdcPerfCalcUtil.RmsNoise(sample.Value, 1).ToString("f3"));
                        DataTableUtil.DataTableAddData(singleModeDataTable, 5, 0, AdcPerfCalcUtil.PeakNoise(sample.Value, 1).ToString());
                        DataTableUtil.DataTableAddData(singleModeDataTable, 6, 0, AdcPerfCalcUtil.PeakNoiseCalc(sample.Value, 1).ToString("f3"));
                        // eff noisefree noisefreecalc
                        DataTableUtil.DataTableAddData(singleModeDataTable, 7, 0, AdcPerfCalcUtil.EffResolution(sample.Value, 24).ToString("f2"));
                        DataTableUtil.DataTableAddData(singleModeDataTable, 8, 0, AdcPerfCalcUtil.NoiseFreeResolution(sample.Value, 24).ToString("f2"));
                        DataTableUtil.DataTableAddData(singleModeDataTable, 9, 0, AdcPerfCalcUtil.NoiseFreeResolutionCalc(sample.Value, 24).ToString("f2"));
                        // volt: offset nrms npp nppcalc
                        double lsb = AdcPerfCalcUtil.LsbVoltage(isBipolar, vRef, gain, adcBits);
                        DataTableUtil.DataTableAddData(singleModeDataTable, 0, 1, (1e6 * lsb).ToString("G4"));
                        DataTableUtil.DataTableAddData(singleModeDataTable, 3, 1, (1e6 * AdcPerfCalcUtil.OffsetErrorVoltage(sample.Value, isBipolar, vRef, gain, adcBits)).ToString("G3"));
                        DataTableUtil.DataTableAddData(singleModeDataTable, 4, 1, (1e6 * AdcPerfCalcUtil.RmsNoise(sample.Value, lsb)).ToString("G3"));
                        DataTableUtil.DataTableAddData(singleModeDataTable, 5, 1, (1e6 * AdcPerfCalcUtil.PeakNoise(sample.Value, lsb)).ToString("G3"));
                        DataTableUtil.DataTableAddData(singleModeDataTable, 6, 1, (1e6 * AdcPerfCalcUtil.PeakNoiseCalc(sample.Value, lsb)).ToString("G3"));

                        SingleModeChartUpdate(sample.Value);
                        break;
                    }
                }
                if (!hasSample)
                    throw new KeyNotFoundException();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        void SingleModeChartUpdate(List<ulong> sample)
        {
            linegraph1.PlotY(sample);

            Dictionary<ulong, double> dataStats = new Dictionary<ulong, double>();
            for(ulong i=AdcPerfCalcUtil.MinCode(sample);i<= AdcPerfCalcUtil.MaxCode(sample);i++)
                dataStats.Add(i, (double)sample.Where(code=>code.Equals(i)).Count()/ (double)sample.Count);
            bargraph1.PlotBars(dataStats.Keys,dataStats.Values);
            // TODO?
            //barmatch1.PlotY(sample);
        }

        void ChartModeDataTableUpdate()
        {
            Dictionary<string, List<string>> sameValueDic = selectConfigInfos.Where(info => info.Value.Count == 1).ToDictionary(k => k.Key, k => k.Value);
            Dictionary<string, List<string>> diffValueDic = selectConfigInfos.Where(info => info.Value.Count > 1).ToDictionary(k => k.Key, k => k.Value);

            //MessageBox.Show("sameKey = "+string.Join(", ", sameValueDic.Keys) +"\ntableKey = "+string.Join(", ", diffValueDic.Keys));

            string rowName, colName;
            string[] rows, cols;
            if (staticTestTabChartModeRotationCheckBox.IsChecked.Equals(false))
            {
                colName = diffValueDic.First().Key;
                cols = diffValueDic.First().Value.ToArray();
                rowName = diffValueDic.Last().Key;
                rows = diffValueDic.Last().Value.ToArray();
            }
            else
            {
                colName = diffValueDic.Last().Key;
                cols = diffValueDic.Last().Value.ToArray();
                rowName = diffValueDic.First().Key;
                rows = diffValueDic.First().Value.ToArray();
            }

            chartModeDataTable = DataTableUtil.DataTableInit(colName, cols, rowName, rows);

            staticTestTabChartModeDataGrid.ItemsSource = null;
            staticTestTabChartModeDataGrid.ItemsSource = chartModeDataTable.DefaultView;
        }

        // tmp
        public Dictionary<string, string> ConvertInfoToDic(string info)
        {
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
            string[] vs = info.Split(new char[] { ';' });
            for (int i = 0; i < vs.Length - 1; i += 2)
                keyValuePairs.Add(vs[i], vs[i + 1]);
            return keyValuePairs;
        }
    }
}
