﻿using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
        AdcConfigCollection adcConfigCollection = new AdcConfigCollection();
        string TasksFileName;

        private void SettingTabXmlEditorButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                XmlEditor xmlEditor = new XmlEditor();
                xmlEditor.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void AdcPrimarySettingsListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            UpdateAdcSecondarySettingsListBox();
            UpdateAdcSettingCommand();
        }

        private void AdcSecondarySettingsListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            UpdateAdcSettingCommand();
        }

        private void AdcSettingSetConfigNowButton_Click(object sender, RoutedEventArgs e)
        {
            string SettingsInfoStr = "";
            List<string> CommandsStr = new List<string>();
            foreach (AdcConfig i in adcConfigCollection)
            {
                //CommandsStr.Add(i.Items.First(item => item.Name == i.CurrentConfig).Command);
                CommandsStr.Add(i.CurrentConfig.Command);
                SettingsInfoStr += "[WPF]: ";
                SettingsInfoStr += i.Name;
                SettingsInfoStr += ": ";
                SettingsInfoStr += i.CurrentConfig;
                SettingsInfoStr += Environment.NewLine;
            }

            AdcSettingsInfoTextBox.Text += SettingsInfoStr;
            AdcSettingsInfoTextBox.Text += "[WPF]: Status: Writing Commands." + System.Environment.NewLine;
            AdcSettingsInfoTextBox.ScrollToEnd();
            AutoRunCmds(CommandsStr.ToArray());
            AdcSettingsInfoTextBox.Text += "[WPF]: Status: Completed." + System.Environment.NewLine;
            AdcSettingsInfoTextBox.ScrollToEnd();
        }

        private void AdcSettingCreateTasksFileButton_Click(object sender, RoutedEventArgs e)
        {
            // Open File Dialog
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Open Commands File...",
                Filter = "Text File|*.txt",
                InitialDirectory = Directory.GetCurrentDirectory(),
                CheckFileExists = false
            };
            if (openFileDialog.ShowDialog() == false)
                return;
            AdcSettingTasksFileNameTextBox.Text = openFileDialog.SafeFileName;
            TasksFileName = openFileDialog.FileName;
            if (!File.Exists(TasksFileName))
                File.Create(TasksFileName).Close();
        }

        private void AdcSettingFileAppendTasksButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StringBuilder WriteFileStringBuilder = new StringBuilder();

                // iterator for multiTasks Generating
                int[] iterCurrentConfigIndex = new int[adcConfigCollection.AdcConfigs.Count];
                int[] iterCurrentConfigCnt = adcConfigCollection.AdcConfigs.Select(item => item.CurrentConfigs.Count).ToArray();
                int iterTaskCnt = 1;

                foreach (var cnt in iterCurrentConfigCnt)
                    iterTaskCnt *= cnt;

                AdcSettingsInfoTextBox.Text += "[WPF]: Status: Program is generating " + iterTaskCnt + " Tasks." + System.Environment.NewLine;
                AdcSettingsInfoTextBox.ScrollToEnd();

                // iter Index Start At Zero
                for (int i = 0; i < iterCurrentConfigIndex.Length; i++)
                    iterCurrentConfigIndex[i] = 0;
                /*
                // iterator Verify
                for (int t = 0; t < iterCurrentConfigIndex.Length; t++)
                    AdcSettingsInfoTextBox.Text += iterCurrentConfigCnt[t].ToString() + " ";
                AdcSettingsInfoTextBox.Text += System.Environment.NewLine;
                */

                for (int iter = 0; iter < iterTaskCnt; iter++)
                {
                    // Update Value Index
                    if (iter != 0)
                        iterCurrentConfigIndex[0]++;
                    for (int i = 0; i < iterCurrentConfigIndex.Length; i++)
                    {
                        if (iterCurrentConfigIndex[i] >= iterCurrentConfigCnt[i])
                        {
                            iterCurrentConfigIndex[i] = 0;
                            if (i + 1 < iterCurrentConfigIndex.Length)
                                iterCurrentConfigIndex[i + 1]++;
                        }
                    }

                    /*
                    // iterator Verify
                    for (int t = 0; t < iterCurrentConfigIndex.Length; t++)
                        AdcSettingsInfoTextBox.Text += iterCurrentConfigIndex[t].ToString() + " ";
                    AdcSettingsInfoTextBox.Text += System.Environment.NewLine;
                    */

                    // Task Gen Values
                    // Prase Configs
                    Dictionary<string, string> configNamePraseKv = new Dictionary<string, string>();
                    for (int i = 0; i < adcConfigCollection.AdcConfigs.Count; i++)
                        configNamePraseKv.Add(adcConfigCollection.AdcConfigs[i].Name, adcConfigCollection.AdcConfigs[i].CurrentConfigsName[iterCurrentConfigIndex[i]]);
                    //configNamePraseKv.Add(adcConfigCollection.AdcConfigs[i].Name, adcConfigCollection.AdcConfigs[i].CurrentConfigs[iterCurrentConfigIndex[i]]);
                    //configNamePraseKv.Add(AdcSettings[i].ConfigName, AdcSettings[i].CurrentSecondaryConfigNames[0]);

                    // iterator Verify
                    //for (int t = 0; t < adcConfigCollection.AdcConfigs.Count; t++)
                    //    AdcSettingsInfoTextBox.Text += adcConfigCollection.AdcConfigs[t].Name + ":" + adcConfigCollection.AdcConfigs[t].CurrentConfigs[iterCurrentConfigIndex[t]] + '\t';
                    //AdcSettingsInfoTextBox.Text += Environment.NewLine;

                    // Write Tasks
                    WriteFileStringBuilder.AppendLine("### TASK.START ###");
                    WriteFileStringBuilder.AppendLine("# TASK.ITERATOR=" + (iter + 1) + "/" + iterTaskCnt);
                    WriteFileStringBuilder.AppendLine("# TASK.GENTIME=" + DateTime.Now.ToString());
                    foreach (var kv in configNamePraseKv)
                        WriteFileStringBuilder.AppendLine("# TASK.CONFIG." + kv.Key + "=" + kv.Value);
                    WriteFileStringBuilder.AppendLine("### TASK.REG ###");
                    foreach (var kv in configNamePraseKv)
                        WriteFileStringBuilder.AppendLine(adcConfigCollection.AdcConfigs.First(item => item.Name == kv.Key).Items.First(item => item.Name == kv.Value).Command);
                    WriteFileStringBuilder.AppendLine("### TASK.ADDON ###");
                    if (AdcSettingTaskAddonCmdsCheckBox.IsChecked == true)
                    {
                        string[] AddonCmdsStrs = AdcSettingTaskAddonCmdsTextBox.Text.Split(new char[] { '\r', '\n' })
                            .Where(s => !string.IsNullOrEmpty(s)).ToArray();
                        // Replace
                        for (int i = 0; i < AddonCmdsStrs.Length; i++)
                        {
                            string AddonCmdsStr = AddonCmdsStrs[i];
                            foreach (KeyValuePair<string, string> kv in configNamePraseKv)
                            {
                                int index = AddonCmdsStr.IndexOf("%" + kv.Key + "%");
                                if (index != -1)
                                    AddonCmdsStr = AddonCmdsStr.Replace("%" + kv.Key + "%", kv.Value);
                            }
                            WriteFileStringBuilder.AppendLine(AddonCmdsStr);
                        }
                    }
                    WriteFileStringBuilder.AppendLine("### TASK.END ###");
                    WriteFileStringBuilder.AppendLine();
                }

                AdcSettingsInfoTextBox.Text += "[WPF]: Status: Writing Tasks." + System.Environment.NewLine;
                AdcSettingsInfoTextBox.ScrollToEnd();
                FileStream fs = new FileStream(TasksFileName, FileMode.Append, FileAccess.Write);
                fs.Write(System.Text.Encoding.Default.GetBytes(WriteFileStringBuilder.ToString()), 0, WriteFileStringBuilder.Length);
                fs.Flush();
                fs.Close();
                AdcSettingsInfoTextBox.Text += "[WPF]: Status: Writing Tasks Completed." + System.Environment.NewLine;
                AdcSettingsInfoTextBox.ScrollToEnd();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void AdcSettingOpenCmdsFileButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(TasksFileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void UpdateAdcSecondarySettingsListBox()
        {
            if (AdcPrimarySettingsListBox.SelectedIndex == -1)
                return;

            AdcSecondarySettingsListBox.SelectedItems.Clear();

            AdcSecondarySettingsListBox.ItemsSource = adcConfigCollection.AdcConfigs[AdcPrimarySettingsListBox.SelectedIndex].Items.Select(t => t.Name).ToList();
            foreach (string ConfigName in adcConfigCollection.AdcConfigs[AdcPrimarySettingsListBox.SelectedIndex].CurrentConfigsName)
                AdcSecondarySettingsListBox.SelectedItems.Add(ConfigName);
        }

        private void UpdateAdcSettingCommand()
        {
            if (AdcPrimarySettingsListBox.SelectedIndex == -1 || AdcSecondarySettingsListBox.SelectedIndex == -1)
                return;

            adcConfigCollection.AdcConfigs[AdcPrimarySettingsListBox.SelectedIndex].CurrentConfigsName = new ObservableCollection<string>(AdcSecondarySettingsListBox.SelectedItems.Cast<string>().ToList());

            AdcSelectedSettingCommandTextBox.Text = adcConfigCollection.AdcConfigs[AdcPrimarySettingsListBox.SelectedIndex].Items[AdcSecondarySettingsListBox.SelectedIndex].Command;
            AdcSelectedSettingDescriptionTextBox.Text = adcConfigCollection.AdcConfigs[AdcPrimarySettingsListBox.SelectedIndex].Description
                + Environment.NewLine + adcConfigCollection.AdcConfigs[AdcPrimarySettingsListBox.SelectedIndex].Items[AdcSecondarySettingsListBox.SelectedIndex].Description;

            string SettingsStr = "";
            foreach (AdcConfig i in adcConfigCollection)
            {
                SettingsStr += i.Name;
                SettingsStr += ": ";
                foreach (var CurrentConfig in i.CurrentConfigsName)
                    SettingsStr += CurrentConfig + "; ";
                SettingsStr += Environment.NewLine;
            }
            AdcAllSelectedSettingsTextBox.Text = SettingsStr;

        }

        private void LoadAdcSettings(string configFile)
        {
            // Load Xml
            adcConfigCollection = new AdcConfigCollection();
            adcConfigCollection.LoadConfigs(configFile, ConfigStroageExtension.Xml);
            // Update UI
            AdcPrimarySettingsListBox.ItemsSource = adcConfigCollection.AdcConfigs.Select(t => t.Name).ToList();
            AdcPrimarySettingsListBox.SelectedIndex = 0;
        }

        private void settingTabLoadXmlButton_Click(object sender, RoutedEventArgs e)
        {
            // Open File Dialog
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Open Data File...",
                Filter = "Config File|*.xml",
                InitialDirectory = Directory.GetCurrentDirectory()
            };
            if (openFileDialog.ShowDialog() == false)
                return;

            LoadAdcSettings(openFileDialog.FileName);
        }
    }
}