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
        DataTable singleModeDataTable, chartModeDataTable, reportModeDataTable;

        string[] reportModeDataTableperfNames = { "Count","Min", "Max", "Avg|Offset Err",
                "Std|RMS Noise(LSB)", "Peak Noise(LSB)", "Peak Noise Calc(LSB)",
                "Eff Res(b)", "NoiseFree Res(b)", "NoiseFree Res Calc(b)",
                "LSB(u)","Min(u)", "Max(u)", "Avg|Offset Err(u)","Std|RMS Noise(u)", "Peak Noise(u)", "Peak Noise Calc(u)",
            };

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

                staticTestTabGainVariableComboBox.ItemsSource = allConfigInfos.Keys.ToList();
                noiseTestTabPrimaryConfigInfoListBox.SelectedIndex = 0;
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
                noiseTestTabReportModeRadioButton.IsEnabled = true;
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
                ReportModeDataTableUpdate();
            }
        }

        private void StaticTestTabSingleModeCalcResolutionButton_Click(object sender, RoutedEventArgs e)
        {
            SingleModeDataTableCalc();
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
                    double effRes = AdcPerfCalculation.EffResolution(sample.Value, 24);
                    double noiseRes = AdcPerfCalculation.NoiseFreeResolution(sample.Value, 24);
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
                            if (sampleSettingInfo.ContainsKey(kv.Key))
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
            string[] cols = { "LSB", "Min", "Max", "Avg|Offset Err",
                "Std|RMS Noise", "Peak Noise", "Peak Noise Calc",
                "Eff Res(b)", "NoiseFree Res(b)", "NoiseFree Res Calc(b)" };

            singleModeDataTable = DataTableUtil.DataTableInit(colName, cols, rowName, rows);

            staticTestTabSingleModeDataGrid.ItemsSource = null;
            staticTestTabSingleModeDataGrid.ItemsSource = singleModeDataTable.DefaultView;
        }

        void SingleModeDataTableCalc()
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

                        bool isBipolar = (bool)staticTestTabisBipolarCheckBox.IsChecked;
                        double vRef = Convert.ToDouble(staticTestTabVrefTextBox.Text);
                        double gain;
                        if ((bool)!staticTestTabisGainVariableCheckBox.IsChecked)
                            gain = Convert.ToDouble(staticTestTabGainTextBox.Text);
                        else
                        {
                            gain = Convert.ToDouble(selectSingleSampleInfo[staticTestTabGainVariableComboBox.Text]);
                            staticTestTabGainTextBox.Text = gain.ToString();
                        }
                        int adcBits = Convert.ToInt32(staticTestTabAdcBitsTextBox.Text);
                        double lsb = AdcPerfCalculation.LsbVoltage(isBipolar, vRef, gain, adcBits);
                        staticTestTabLSBTextBox.Text = (1e6 * lsb).ToString("G4");

                        // lsb: Min Max Avg nrms npp nppcalc
                        DataTableUtil.DataTableAddData(singleModeDataTable, 0, 0, 1.ToString());
                        DataTableUtil.DataTableAddData(singleModeDataTable, 1, 0, AdcPerfCalculation.MinCode(sample.Value).ToString());
                        DataTableUtil.DataTableAddData(singleModeDataTable, 2, 0, AdcPerfCalculation.MaxCode(sample.Value).ToString());
                        DataTableUtil.DataTableAddData(singleModeDataTable, 3, 0, AdcPerfCalculation.AvgCode(sample.Value).ToString("f3"));
                        DataTableUtil.DataTableAddData(singleModeDataTable, 4, 0, AdcPerfCalculation.RmsNoise(sample.Value, 1).ToString("f3"));
                        DataTableUtil.DataTableAddData(singleModeDataTable, 5, 0, AdcPerfCalculation.PeakNoise(sample.Value, 1).ToString());
                        DataTableUtil.DataTableAddData(singleModeDataTable, 6, 0, AdcPerfCalculation.PeakNoiseCalc(sample.Value, 1).ToString("f3"));
                        // eff noisefree noisefreecalc
                        DataTableUtil.DataTableAddData(singleModeDataTable, 7, 0, AdcPerfCalculation.EffResolution(sample.Value, adcBits).ToString("f2"));
                        DataTableUtil.DataTableAddData(singleModeDataTable, 8, 0, AdcPerfCalculation.NoiseFreeResolution(sample.Value, adcBits).ToString("f2"));
                        DataTableUtil.DataTableAddData(singleModeDataTable, 9, 0, AdcPerfCalculation.NoiseFreeResolutionCalc(sample.Value, adcBits).ToString("f2"));
                        // volt: offset nrms npp nppcalc
                        DataTableUtil.DataTableAddData(singleModeDataTable, 0, 1, (1e6 * lsb).ToString("G4"));
                        DataTableUtil.DataTableAddData(singleModeDataTable, 1, 1, (1e6 * AdcPerfCalculation.MinVoltage(sample.Value, isBipolar, vRef, gain, adcBits)).ToString("G3"));
                        DataTableUtil.DataTableAddData(singleModeDataTable, 2, 1, (1e6 * AdcPerfCalculation.MaxVoltage(sample.Value, isBipolar, vRef, gain, adcBits)).ToString("G3"));
                        DataTableUtil.DataTableAddData(singleModeDataTable, 3, 1, (1e6 * AdcPerfCalculation.OffsetErrorVoltage(sample.Value, isBipolar, vRef, gain, adcBits)).ToString("G3"));
                        DataTableUtil.DataTableAddData(singleModeDataTable, 4, 1, (1e6 * AdcPerfCalculation.RmsNoise(sample.Value, lsb)).ToString("G3"));
                        DataTableUtil.DataTableAddData(singleModeDataTable, 5, 1, (1e6 * AdcPerfCalculation.PeakNoise(sample.Value, lsb)).ToString("G3"));
                        DataTableUtil.DataTableAddData(singleModeDataTable, 6, 1, (1e6 * AdcPerfCalculation.PeakNoiseCalc(sample.Value, lsb)).ToString("G3"));

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
            ulong sampleMinCode = AdcPerfCalculation.MinCode(sample);
            ulong sampleMaxCode = AdcPerfCalculation.MaxCode(sample);
            // isScale or barCnt>100
            ulong scale = 1;
            if (staticTestTabSingleModeisBarScaleCheckBox.IsChecked.Equals(false) || sampleMaxCode - sampleMinCode + 1 <= 100)
                for (ulong i = sampleMinCode; i <= sampleMaxCode; i++)
                    dataStats.Add(i, (double)sample.Where(code => code.Equals(i)).Count() / ((bool)staticTestTabSingleModeisBarCountCheckBox.IsChecked ? 1 : sample.Count));
            else
            {
                scale = (ulong)Math.Ceiling((sampleMaxCode - sampleMinCode + 1) / 100m);
                for (ulong i = sampleMinCode; i <= sampleMaxCode; i += scale)
                    dataStats.Add(i, (double)sample.Where(code => code >= i && code < (i + scale)).Count() / ((bool)staticTestTabSingleModeisBarCountCheckBox.IsChecked ? 1 : sample.Count));
            }
            bargraph1.BarsWidth = scale;
            bargraph1.PlotBars(dataStats.Keys, dataStats.Values);
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

        void ReportModeDataTableUpdate()
        {
            string rowName = "Sample", colName = "Pref";

            string[] rows = adcDataStorage.AdcSamplesStorage.Keys.ToArray();
            List<string> cols = new List<string>();
            cols.AddRange(allConfigInfos.Keys.ToArray());
            cols.AddRange(reportModeDataTableperfNames);

            reportModeDataTable = DataTableUtil.DataTableInit(colName, cols.ToArray(), rowName, rows);

            staticTestTabReportModeDataGrid.ItemsSource = null;
            staticTestTabReportModeDataGrid.ItemsSource = reportModeDataTable.DefaultView;
        }

        private void StaticTestTabReportModeCalcResolutionButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (var kvSample in adcDataStorage.AdcSamplesStorage)
                {
                    Dictionary<string, string> sampleSettingInfo = ConvertInfoToDic(adcDataStorage.AdcSamplesSettingInfo[kvSample.Key]);
                    foreach (var kvInfo in allConfigInfos)
                    {
                        if (sampleSettingInfo.ContainsKey(kvInfo.Key))
                            DataTableUtil.DataTableAddData(reportModeDataTable, kvInfo.Key, kvSample.Key, sampleSettingInfo[kvInfo.Key]);
                    }

                    bool isBipolar = (bool)staticTestTabisBipolarCheckBox.IsChecked;
                    double vRef = Convert.ToDouble(staticTestTabVrefTextBox.Text);
                    double gain;
                    if ((bool)!staticTestTabisGainVariableCheckBox.IsChecked)
                        gain = Convert.ToDouble(staticTestTabGainTextBox.Text);
                    else
                    {
                        gain = Convert.ToDouble(sampleSettingInfo[staticTestTabGainVariableComboBox.Text]);
                        staticTestTabGainTextBox.Text = gain.ToString();
                    }
                    int adcBits = Convert.ToInt32(staticTestTabAdcBitsTextBox.Text);
                    double lsb = AdcPerfCalculation.LsbVoltage(isBipolar, vRef, gain, adcBits);
                    staticTestTabLSBTextBox.Text = (1e6 * lsb).ToString("G4");

                    List<(string name, string perf)> vals = new List<(string name, string perf)>();
                    //foreach (var perfName in reportModeDataTableperfNames)
                    Parallel.ForEach(reportModeDataTableperfNames, perfName =>
                        {
                            string calcResult;
                            switch (perfName)
                            {
                                case "Count": calcResult = kvSample.Value.Count.ToString(); break;
                                case "Min": calcResult = AdcPerfCalculation.MinCode(kvSample.Value).ToString(); break;
                                case "Max": calcResult = AdcPerfCalculation.MaxCode(kvSample.Value).ToString(); break;
                                case "Avg|Offset Err": calcResult = AdcPerfCalculation.AvgCode(kvSample.Value).ToString("f3"); break;
                                case "Std|RMS Noise(LSB)": calcResult = AdcPerfCalculation.RmsNoise(kvSample.Value, 1).ToString("f3"); break;
                                case "Peak Noise(LSB)": calcResult = AdcPerfCalculation.PeakNoise(kvSample.Value, 1).ToString(); break;
                                case "Peak Noise Calc(LSB)": calcResult = AdcPerfCalculation.PeakNoiseCalc(kvSample.Value, 1).ToString("f3"); break;
                                case "Eff Res(b)": calcResult = AdcPerfCalculation.EffResolution(kvSample.Value, adcBits).ToString("f2"); break;
                                case "NoiseFree Res(b)": calcResult = AdcPerfCalculation.NoiseFreeResolution(kvSample.Value, adcBits).ToString("f2"); break;
                                case "NoiseFree Res Calc(b)": calcResult = AdcPerfCalculation.NoiseFreeResolutionCalc(kvSample.Value, adcBits).ToString("f2"); break;
                                case "LSB(u)": calcResult = (1e6 * lsb).ToString("G4"); break;
                                case "Min(u)": calcResult = (1e6 * AdcPerfCalculation.MinVoltage(kvSample.Value, isBipolar, vRef, gain, adcBits)).ToString("G7"); break;
                                case "Max(u)": calcResult = (1e6 * AdcPerfCalculation.MaxVoltage(kvSample.Value, isBipolar, vRef, gain, adcBits)).ToString("G7"); break;
                                case "Avg|Offset Err(u)": calcResult = (1e6 * AdcPerfCalculation.OffsetErrorVoltage(kvSample.Value, isBipolar, vRef, gain, adcBits)).ToString("G7"); break;
                                case "Std|RMS Noise(u)": calcResult = (1e6 * AdcPerfCalculation.RmsNoise(kvSample.Value, lsb)).ToString("G3"); break;
                                case "Peak Noise(u)": calcResult = (1e6 * AdcPerfCalculation.PeakNoise(kvSample.Value, lsb)).ToString("G3"); break;
                                case "Peak Noise Calc(u)": calcResult = (1e6 * AdcPerfCalculation.PeakNoiseCalc(kvSample.Value, lsb)).ToString("G3"); break;
                                default: calcResult = ""; break;
                            }
                            lock(vals)
                            {
                                vals.Add((perfName, calcResult));
                            }

                        });
                    foreach (var val in vals)
                        DataTableUtil.DataTableAddData(reportModeDataTable, val.name, kvSample.Key, val.perf);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void StaticTestTabReportModeStoreCsvButton_Click(object sender, RoutedEventArgs e)
        {
            // Save File Dialog
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Title = "Save Report File...",
                Filter = "Report File|*.csv",
                FileName = "StaticTestReport;All;",
                InitialDirectory = Directory.GetCurrentDirectory()
            };
            if (saveFileDialog.ShowDialog() == false)
                return;

            DataTableUtil.DatatableToCSV("All;", reportModeDataTable, saveFileDialog.FileName);
        }

        private void StaticTestTabisGainVariableCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            staticTestTabGainVariableComboBox.IsEnabled = (bool)staticTestTabisGainVariableCheckBox.IsChecked;
            staticTestTabGainTextBox.IsEnabled = !(bool)staticTestTabisGainVariableCheckBox.IsChecked;
        }
        private void StaticTestTabisGainVariableCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            staticTestTabGainVariableComboBox.IsEnabled = (bool)staticTestTabisGainVariableCheckBox.IsChecked;
            staticTestTabGainTextBox.IsEnabled = !(bool)staticTestTabisGainVariableCheckBox.IsChecked;
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
