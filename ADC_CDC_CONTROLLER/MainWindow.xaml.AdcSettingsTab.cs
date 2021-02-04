using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
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

        List<AdcPrimarySettingClass> AdcSettings = new List<AdcPrimarySettingClass>();
        string TasksFileName;

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
            foreach (AdcPrimarySettingClass i in AdcSettings)
            {
                CommandsStr.Add(i.Configs.First(item => item.ConfigName == i.CurrentSecondaryConfigNames[0]).ConfigCommand);
                SettingsInfoStr += "[WPF]: ";
                SettingsInfoStr += i.ConfigName;
                SettingsInfoStr += ": ";
                SettingsInfoStr += i.CurrentSecondaryConfigNames[0];
                SettingsInfoStr += System.Environment.NewLine;
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
                Filter = "Text File|*.txt"
            };
            if (openFileDialog.ShowDialog() == false)
                return;
            AdcSettingTasksFileNameTextBox.Text = openFileDialog.SafeFileName;
            TasksFileName = openFileDialog.FileName;
        }

        private void AdcSettingFileAppendTasksButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string WriteFileStr = "";

                // iterator for multiTasks Generating
                int[] iterCurrentConfigIndex = new int[AdcSettings.Count];
                int[] iterCurrentConfigCnt = AdcSettings.Select(item => item.CurrentSecondaryConfigNames.Count).ToArray();
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
                    for (int i = 0; i < AdcSettings.Count; i++)
                        configNamePraseKv.Add(AdcSettings[i].ConfigName, AdcSettings[i].CurrentSecondaryConfigNames[iterCurrentConfigIndex[i]]);
                    //configNamePraseKv.Add(AdcSettings[i].ConfigName, AdcSettings[i].CurrentSecondaryConfigNames[0]);

                    
                    // iterator Verify
                    for (int t = 0; t < AdcSettings.Count; t++)
                        AdcSettingsInfoTextBox.Text += AdcSettings[t].ConfigName + ":" + AdcSettings[t].CurrentSecondaryConfigNames[iterCurrentConfigIndex[t]] + '\t';
                    AdcSettingsInfoTextBox.Text += System.Environment.NewLine;
                    

                    // Write Tasks
                    WriteFileStr += "### TASK.START ###" + Environment.NewLine;
                    WriteFileStr += "# TASK.ITERATOR=" + iter + "/"+iterTaskCnt + Environment.NewLine;
                    WriteFileStr += "# TASK.GENTIME=" + DateTime.Now.ToString() + Environment.NewLine;
                    foreach (var kv in configNamePraseKv)
                        WriteFileStr += "# TASK.CONFIG." + kv.Key + "=" + kv.Value + Environment.NewLine;
                    WriteFileStr += "### TASK.REG ###" + Environment.NewLine;
                    foreach (var kv in configNamePraseKv)
                        WriteFileStr += AdcSettings.First(item => item.ConfigName == kv.Key).Configs.First(item => item.ConfigName == kv.Value).ConfigCommand + Environment.NewLine;
                    WriteFileStr += "### TASK.ADDON ###" + Environment.NewLine;
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
                            WriteFileStr += AddonCmdsStr + Environment.NewLine;
                        }
                    }
                    WriteFileStr += "### TASK.END ###" + Environment.NewLine;
                    WriteFileStr += Environment.NewLine;
                }

                AdcSettingsInfoTextBox.Text += "[WPF]: Status: Writing Tasks." + System.Environment.NewLine;
                AdcSettingsInfoTextBox.ScrollToEnd();
                FileStream fs = new FileStream(TasksFileName, FileMode.Append, FileAccess.Write);
                fs.Write(System.Text.Encoding.Default.GetBytes(WriteFileStr), 0, WriteFileStr.Length);
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

            AdcSecondarySettingsListBox.ItemsSource = AdcSettings[AdcPrimarySettingsListBox.SelectedIndex].Configs.Select(t => t.ConfigName).ToList();
            foreach (string ConfigName in AdcSettings[AdcPrimarySettingsListBox.SelectedIndex].CurrentSecondaryConfigNames)
                AdcSecondarySettingsListBox.SelectedItems.Add(ConfigName);
        }

        private void UpdateAdcSettingCommand()
        {
            if (AdcPrimarySettingsListBox.SelectedIndex == -1 || AdcSecondarySettingsListBox.SelectedIndex == -1)
                return;

            AdcSettings[AdcPrimarySettingsListBox.SelectedIndex].CurrentSecondaryConfigNames = AdcSecondarySettingsListBox.SelectedItems.Cast<string>().ToList();

            AdcSelectedSettingCommandTextBox.Text = AdcSettings[AdcPrimarySettingsListBox.SelectedIndex].Configs[AdcSecondarySettingsListBox.SelectedIndex].ConfigCommand;
            AdcSelectedSettingDescriptionTextBox.Text = AdcSettings[AdcPrimarySettingsListBox.SelectedIndex].ConfigDescription
                + System.Environment.NewLine + AdcSettings[AdcPrimarySettingsListBox.SelectedIndex].Configs[AdcSecondarySettingsListBox.SelectedIndex].ConfigDescription;

            string SettingsStr = "";
            foreach (AdcPrimarySettingClass i in AdcSettings)
            {
                SettingsStr += i.ConfigName;
                SettingsStr += ": ";
                foreach (string ConfigName in i.CurrentSecondaryConfigNames)
                    SettingsStr += ConfigName + "; ";
                //SettingsStr += i.CurrentSecondaryConfigNames;
                SettingsStr += System.Environment.NewLine;
            }
            AdcAllSelectedSettingsTextBox.Text = SettingsStr;

        }

        private void InitAdcSettings()
        {
            List<AdcPrimarySettingClass> adcSettingsFunctionStruct =
                new List<AdcPrimarySettingClass>
                {
                    new AdcPrimarySettingClass
                    {
                        ConfigName = "Power Mode",
                        ConfigDescription = "Power Mode",
                        DefaultSecondaryConfigName = "Low Power",
                        CurrentSecondaryConfigNames = new List<string>(){"Low Power"},
                        Configs = new List<AdcSecondarySettingStruct>
                        {
                           new AdcSecondarySettingStruct { ConfigName = "Low Power", ConfigDescription = "[Secondary]: POWER_MODE = 00", ConfigCommand = "REGM;01;6;2;0;"},
                           new AdcSecondarySettingStruct { ConfigName = "Mid Power", ConfigDescription = "[Secondary]: POWER_MODE = 01", ConfigCommand = "REGM;01;6;2;1;"},
                           new AdcSecondarySettingStruct { ConfigName = "Full Power", ConfigDescription = "[Secondary]: POWER_MODE = 10", ConfigCommand = "REGM;01;6;2;2;"},
                        }
                    },
                    new AdcPrimarySettingClass
                    {
                        ConfigName = "PGA",
                        ConfigDescription ="PGA",
                        DefaultSecondaryConfigName = "1",
                        CurrentSecondaryConfigNames = new List<string>(){"1"},
                        Configs = new List<AdcSecondarySettingStruct>
                        {
                           new AdcSecondarySettingStruct { ConfigName = "1", ConfigDescription = "[Secondary]: PGA = 1, ±2.5V", ConfigCommand = "REGM;19;0;3;0;"},
                           new AdcSecondarySettingStruct { ConfigName = "2", ConfigDescription = "[Secondary]: PGA = 2, ±1.25V", ConfigCommand = "REGM;19;0;3;1;"},
                           new AdcSecondarySettingStruct { ConfigName = "4", ConfigDescription = "[Secondary]: PGA = 4, ±625mV", ConfigCommand = "REGM;19;0;3;2;"},
                           new AdcSecondarySettingStruct { ConfigName = "8", ConfigDescription = "[Secondary]: PGA = 8, ±312.5mV", ConfigCommand = "REGM;19;0;3;3;"},
                           new AdcSecondarySettingStruct { ConfigName = "16", ConfigDescription = "[Secondary]: PGA = 16, ±156.25mV", ConfigCommand = "REGM;19;0;3;4;"},
                           new AdcSecondarySettingStruct { ConfigName = "32", ConfigDescription = "[Secondary]: PGA = 32, ±78.125mV", ConfigCommand = "REGM;19;0;3;5;"},
                           new AdcSecondarySettingStruct { ConfigName = "64", ConfigDescription = "[Secondary]: PGA = 64, ±39.06mV", ConfigCommand = "REGM;19;0;3;6;"},
                           new AdcSecondarySettingStruct { ConfigName = "128", ConfigDescription = "[Secondary]: PGA = 128, ±19.53mV", ConfigCommand = "REGM;19;0;3;7;"}
                        }
                    }
                    ,
                    new AdcPrimarySettingClass
                    {
                        ConfigName = "Filter",
                        ConfigDescription = "Filter",
                        DefaultSecondaryConfigName = "Sinc4",
                        CurrentSecondaryConfigNames = new List<string>(){"Sinc4"},
                        Configs = new List<AdcSecondarySettingStruct>
                        {
                           new AdcSecondarySettingStruct { ConfigName = "Sinc4", ConfigDescription = "[Secondary]: Filter = 000", ConfigCommand = "REGM;21;21;3;0;"},
                           new AdcSecondarySettingStruct { ConfigName = "Sinc3", ConfigDescription = "[Secondary]: Filter = 010", ConfigCommand = "REGM;21;21;3;2;"},
                           new AdcSecondarySettingStruct { ConfigName = "Sinc4+Sinc1", ConfigDescription = "[Secondary]: Filter = 100", ConfigCommand = "REGM;21;21;3;4;"},
                           new AdcSecondarySettingStruct { ConfigName = "Sinc3+Sinc1", ConfigDescription = "[Secondary]: Filter = 101", ConfigCommand = "REGM;21;21;3;5;"},
                           new AdcSecondarySettingStruct { ConfigName = "Post Filter", ConfigDescription = "[Secondary]: Filter = 111", ConfigCommand = "REGM;21;21;3;7;"},
                        }
                    },
                    new AdcPrimarySettingClass
                    {
                        ConfigName = "Speed (SincX Filter)",
                        ConfigDescription = "Speed (SincX Filter)",
                        DefaultSecondaryConfigName = "384",
                        CurrentSecondaryConfigNames = new List<string>(){"384"},
                        Configs = new List<AdcSecondarySettingStruct>
                        {
                           new AdcSecondarySettingStruct { ConfigName = "1", ConfigDescription = "[Secondary]: No Description", ConfigCommand = "REGM;21;0;11;1;"},
                           new AdcSecondarySettingStruct { ConfigName = "2", ConfigDescription = "[Secondary]: No Description", ConfigCommand = "REGM;21;0;11;2;"},
                           new AdcSecondarySettingStruct { ConfigName = "3", ConfigDescription = "[Secondary]: No Description", ConfigCommand = "REGM;21;0;11;3;"},
                           new AdcSecondarySettingStruct { ConfigName = "4", ConfigDescription = "[Secondary]: No Description", ConfigCommand = "REGM;21;0;11;4;"},
                           new AdcSecondarySettingStruct { ConfigName = "5", ConfigDescription = "[Secondary]: No Description", ConfigCommand = "REGM;21;0;11;5;"},
                           new AdcSecondarySettingStruct { ConfigName = "6", ConfigDescription = "[Secondary]: No Description", ConfigCommand = "REGM;21;0;11;6;"},
                           new AdcSecondarySettingStruct { ConfigName = "8", ConfigDescription = "[Secondary]: No Description", ConfigCommand = "REGM;21;0;11;8;"},
                           new AdcSecondarySettingStruct { ConfigName = "10", ConfigDescription = "[Secondary]: No Description", ConfigCommand = "REGM;21;0;11;A;"},
                           new AdcSecondarySettingStruct { ConfigName = "15", ConfigDescription = "[Secondary]: No Description", ConfigCommand = "REGM;21;0;11;F;"},
                           new AdcSecondarySettingStruct { ConfigName = "20", ConfigDescription = "[Secondary]: No Description", ConfigCommand = "REGM;21;0;11;14;"},
                           new AdcSecondarySettingStruct { ConfigName = "24", ConfigDescription = "[Secondary]: No Description", ConfigCommand = "REGM;21;0;11;18;"},
                           new AdcSecondarySettingStruct { ConfigName = "30", ConfigDescription = "[Secondary]: No Description", ConfigCommand = "REGM;21;0;11;1E;"},
                           new AdcSecondarySettingStruct { ConfigName = "40", ConfigDescription = "[Secondary]: No Description", ConfigCommand = "REGM;21;0;11;28;"},
                           new AdcSecondarySettingStruct { ConfigName = "48", ConfigDescription = "[Secondary]: No Description", ConfigCommand = "REGM;21;0;11;30;"},
                           new AdcSecondarySettingStruct { ConfigName = "60", ConfigDescription = "[Secondary]: No Description", ConfigCommand = "REGM;21;0;11;3C;"},
                           new AdcSecondarySettingStruct { ConfigName = "80", ConfigDescription = "[Secondary]: No Description", ConfigCommand = "REGM;21;0;11;50;"},
                           new AdcSecondarySettingStruct { ConfigName = "96", ConfigDescription = "[Secondary]: No Description", ConfigCommand = "REGM;21;0;11;60;"},
                           new AdcSecondarySettingStruct { ConfigName = "120", ConfigDescription = "[Secondary]: No Description", ConfigCommand = "REGM;21;0;11;78;"},
                           new AdcSecondarySettingStruct { ConfigName = "160", ConfigDescription = "[Secondary]: No Description", ConfigCommand = "REGM;21;0;11;A0;"},
                           new AdcSecondarySettingStruct { ConfigName = "240", ConfigDescription = "[Secondary]: No Description", ConfigCommand = "REGM;21;0;11;F0;"},
                           new AdcSecondarySettingStruct { ConfigName = "320", ConfigDescription = "[Secondary]: No Description", ConfigCommand = "REGM;21;0;11;140;"},
                           new AdcSecondarySettingStruct { ConfigName = "384", ConfigDescription = "[Secondary]: No Description", ConfigCommand = "REGM;21;0;11;180;"},
                           new AdcSecondarySettingStruct { ConfigName = "480", ConfigDescription = "[Secondary]: No Description", ConfigCommand = "REGM;21;0;11;1E0;"},
                           new AdcSecondarySettingStruct { ConfigName = "640", ConfigDescription = "[Secondary]: No Description", ConfigCommand = "REGM;21;0;11;280;"},
                           new AdcSecondarySettingStruct { ConfigName = "960", ConfigDescription = "[Secondary]: No Description", ConfigCommand = "REGM;21;0;11;3C0;"},
                           new AdcSecondarySettingStruct { ConfigName = "1280", ConfigDescription = "[Secondary]: No Description", ConfigCommand = "REGM;21;0;11;500;"},
                           new AdcSecondarySettingStruct { ConfigName = "1920", ConfigDescription = "[Secondary]: No Description", ConfigCommand = "REGM;21;0;11;780;"},
                           new AdcSecondarySettingStruct { ConfigName = "2047", ConfigDescription = "[Secondary]: No Description", ConfigCommand = "REGM;21;0;11;7FF;"},
                        }
                    },
                    new AdcPrimarySettingClass
                    {
                        ConfigName = "Speed (Post Filter)",
                        ConfigDescription = "Speed (Post Filter)",
                        DefaultSecondaryConfigName = "25 SPS",
                        CurrentSecondaryConfigNames = new List<string>(){"25 SPS"},
                        Configs = new List<AdcSecondarySettingStruct>
                        {
                           new AdcSecondarySettingStruct { ConfigName = "16.67 SPS", ConfigDescription = "[Secondary]: POST_FILTER = 010", ConfigCommand = "REGM;21;17;3;2;"},
                           new AdcSecondarySettingStruct { ConfigName = "20 SPS", ConfigDescription = "[Secondary]: POST_FILTER = 011", ConfigCommand = "REGM;21;17;3;3;"},
                           new AdcSecondarySettingStruct { ConfigName = "25 SPS", ConfigDescription = "[Secondary]: POST_FILTER = 101", ConfigCommand = "REGM;21;17;3;5;"},
                           new AdcSecondarySettingStruct { ConfigName = "27.27 SPS", ConfigDescription = "[Secondary]: POST_FILTER = 110", ConfigCommand = "REGM;21;17;3;6;"}
                        }
                    }
                };
            AdcSettings.AddRange(adcSettingsFunctionStruct);
        }

        public class AdcPrimarySettingClass
        {
            public string ConfigName;
            public string ConfigDescription;
            public string DefaultSecondaryConfigName;
            public List<string> CurrentSecondaryConfigNames;
            public List<AdcSecondarySettingStruct> Configs;

            public AdcPrimarySettingClass()
            {
                ConfigName = "";
                ConfigDescription = "[Primary]: No Description";
                DefaultSecondaryConfigName = "";
                CurrentSecondaryConfigNames = new List<string>();
                Configs = new List<AdcSecondarySettingStruct>();
            }
            /*
            public AdcPrimarySettingClass(string ConfigName, string DefaultSecondaryConfigName, List<AdcSecondarySettingStruct> Configs)
            {
                this.ConfigName = ConfigName;
                this.ConfigDescription = "[Primary]: " + ConfigName;
                this.DefaultSecondaryConfigName = DefaultSecondaryConfigName;
                this.CurrentSecondaryConfigName = DefaultSecondaryConfigName;
                this.Configs = Configs;
            }
            public AdcPrimarySettingClass(string ConfigName, string ConfigDescription, string DefaultSecondaryConfigName, string CurrentSecondaryConfigName, List<AdcSecondarySettingStruct> Configs)
            {
                this.ConfigName = ConfigName;
                this.ConfigDescription = ConfigDescription;
                this.DefaultSecondaryConfigName = DefaultSecondaryConfigName;
                this.CurrentSecondaryConfigName = CurrentSecondaryConfigName;
                this.Configs = Configs;
            }
            */
        }
        public struct AdcSecondarySettingStruct
        {
            public string ConfigName;
            public string ConfigDescription;
            public string ConfigCommand;
        }
    }
}