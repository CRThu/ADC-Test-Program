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
        DataTable chartModeDataTable = new DataTable("Chart Mode Data Table");

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
                MessageBox.Show("TODO");
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
            foreach(var sample in adcDataStorage.AdcSamplesStorage)
            {
                double effRes = AdcPerfCalcUtil.EffResolution(sample.Value, 24);
                double noiseRes = AdcPerfCalcUtil.NoiseFreeResolution(sample.Value, 24);
                Dictionary<string, string> sampleSettingInfo = ConvertInfoToDic(adcDataStorage.AdcSamplesSettingInfo[sample.Key]);
                bool isSelected=true;
                foreach(var kv in sampleSettingInfo)
                {
                    if (!selectConfigInfos[kv.Key].Contains(kv.Value))
                    {
                        isSelected = false;
                        break;
                    }
                    if (isSelected.Equals(false))
                        break;
                }
                if (isSelected)
                {
                    Dictionary<string, List<string>> diffValueDic = selectConfigInfos.Where(info => info.Value.Count > 1).ToDictionary(k => k.Key, k => k.Value);
                    List<string> str = new List<string>();
                    foreach (var kv in diffValueDic)
                        str.Add(sampleSettingInfo[kv.Key]);
                    if (staticTestTabChartModeRotationCheckBox.IsChecked.Equals(false))
                    {
                        ChartModeDataTableAddData(str.First(), str.Last(), effRes.ToString("f1") +"("+noiseRes.ToString("f1")+")");
                    }
                    else
                    {
                        ChartModeDataTableAddData(str.Last(), str.First(), effRes.ToString("f1") + "(" + noiseRes.ToString("f1") + ")");
                    }
                }
                }

            }
            catch(Exception ex)
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
                FileName = "StaticTestReport;"+ str,
                InitialDirectory = Directory.GetCurrentDirectory()
            };
            if (saveFileDialog.ShowDialog() == false)
                return;

            datatableToCSV(noiseTestTabConfigViewTextBox.Text.Replace("\r", "").Replace("\n",""), chartModeDataTable, saveFileDialog.FileName);
        }

        void ChartModeDataTableUpdate()
        {
            Dictionary<string, List<string>> sameValueDic = selectConfigInfos.Where(info => info.Value.Count == 1).ToDictionary(k=>k.Key,k=>k.Value);
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
            ChartModeDataTableInit(colName, cols, rowName, rows);
            /*
            ChartModeDataTableInit("colName", new string[] { "1", "2", "3", "4" },
                                    "rowName", new string[] { "a", "b", "c", "d" });
            ChartModeDataTableAddData("b", "3", "B3");
            ChartModeDataTableAddData("d", "4", "D4");
            ChartModeDataTableAddData("a", "3", "A3");
            */
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

        void ChartModeDataTableInit(string colName, string[] columns, string rowName, string[] rows)
        {
            chartModeDataTable = new DataTable("Chart Mode Data Table");

            chartModeDataTable.Columns.Add(rowName + @"\" + colName);

            ChartModeDataTableAddColumnsName(columns);
            ChartModeDataTableAddRowsName(rows);

            staticTestTabChartModeDataGrid.ItemsSource = null;
            staticTestTabChartModeDataGrid.ItemsSource = chartModeDataTable.DefaultView;
        }
        void ChartModeDataTableAddColumnsName(string[] columns)
        {
            foreach (string col in columns)
                chartModeDataTable.Columns.Add(col);
        }
        void ChartModeDataTableAddRowsName(string[] rows)
        {
            foreach (string row in rows)
            {
                DataRow dr = chartModeDataTable.NewRow();
                dr[0] = row;
                chartModeDataTable.Rows.Add(dr);
            }
            string firstColumnName = chartModeDataTable.Columns[0].ColumnName;
            // sort
            //chartModeDataTable = chartModeDataTable.Clone().Rows.Cast<DataRow>().OrderBy(r => Convert.ToDecimal((string)r[firstColumnName])).CopyToDataTable();
        }
        void ChartModeDataTableAddData(string column, string row, string data)
        {
            string firstColumnName = chartModeDataTable.Columns[0].ColumnName;
            DataRow dr = chartModeDataTable.Select("[" + firstColumnName + "]='" + row + "'").First();
            dr[column] = data;
        }

        // only for test
        public static bool datatableToCSV(string info, DataTable dt, string pathFile)
        {
            string strLine = "";
            StreamWriter sw;
            try
            {
                sw = new StreamWriter(pathFile, false, System.Text.Encoding.GetEncoding(-0));

                if (!string.IsNullOrEmpty(info))
                    sw.WriteLine(info);

                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    if (i > 0)
                        strLine += ",";
                    strLine += dt.Columns[i].ColumnName;
                }
                strLine.Remove(strLine.Length - 1);
                sw.WriteLine(strLine);
                strLine = "";

                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    strLine = "";
                    int colCount = dt.Columns.Count;
                    for (int k = 0; k < colCount; k++)
                    {
                        if (k > 0 && k < colCount)
                            strLine += ",";
                        if (dt.Rows[j][k] == null)
                            strLine += "";
                        else
                        {
                            string cell = dt.Rows[j][k].ToString().Trim();

                            cell = cell.Replace("\"", "\"\"");
                            cell = "\"" + cell + "\"";
                            strLine += cell;
                        }
                    }
                    sw.WriteLine(strLine);
                }
                sw.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return false;
            }
            return true;
        }
    }
}
