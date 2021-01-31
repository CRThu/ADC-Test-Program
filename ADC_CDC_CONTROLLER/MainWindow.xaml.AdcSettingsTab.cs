using Microsoft.Win32;
using System;
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
                CommandsStr.Add(i.Configs.First(item => item.ConfigName == i.CurrentSecondaryConfigName).ConfigCommand);
                SettingsInfoStr += "[WPF]: ";
                SettingsInfoStr += i.ConfigName;
                SettingsInfoStr += ": ";
                SettingsInfoStr += i.CurrentSecondaryConfigName;
                SettingsInfoStr += System.Environment.NewLine;
            }

            AdcSettingsInfoTextBox.Text += SettingsInfoStr;
            AdcSettingsInfoTextBox.Text += "[WPF]: Status: Writing Commands." + System.Environment.NewLine;
            AutoRunCmds(CommandsStr.ToArray());
            AdcSettingsInfoTextBox.Text += "[WPF]: Status: Completed." + System.Environment.NewLine;
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

        private void AdcSettingFileAppendTaskButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string WriteFileStr = "";
                WriteFileStr += "### TASK.START ###" + Environment.NewLine;
                WriteFileStr += "# TASK.GENTIME="+ DateTime.Now.ToString() + Environment.NewLine;
                foreach (AdcPrimarySettingClass i in AdcSettings)
                    WriteFileStr += "# TASK.CONFIG." + i.ConfigName + "=" + i.CurrentSecondaryConfigName + Environment.NewLine;
                foreach (AdcPrimarySettingClass i in AdcSettings)
                    WriteFileStr += i.Configs.First(item => item.ConfigName == i.CurrentSecondaryConfigName).ConfigCommand + Environment.NewLine;
                
                if (AdcSettingTaskAddonCmdsCheckBox.IsChecked == true)
                {
                    string[] AddonCmdsStrs = AdcSettingTaskAddonCmdsTextBox.Text.Split(new char[] { '\r', '\n' })
                        .Where(s => !string.IsNullOrEmpty(s)).ToArray();
                    foreach(string AddonCmdsStr in AddonCmdsStrs)
                        WriteFileStr += AddonCmdsStr + Environment.NewLine;
                }
                WriteFileStr += "### TASK.END ###" + Environment.NewLine;
                WriteFileStr += Environment.NewLine;

                // TODO multi selection
                // TODO print to status textbox
                FileStream fs = new FileStream(TasksFileName, FileMode.Append, FileAccess.Write);
                fs.Write(System.Text.Encoding.Default.GetBytes(WriteFileStr), 0, WriteFileStr.Length);
                fs.Flush();
                fs.Close();
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
            AdcSecondarySettingsListBox.SelectedItem = AdcSettings[AdcPrimarySettingsListBox.SelectedIndex].CurrentSecondaryConfigName;
        }

        private void UpdateAdcSettingCommand()
        {
            if (AdcPrimarySettingsListBox.SelectedIndex == -1 || AdcSecondarySettingsListBox.SelectedIndex == -1)
                return;

            AdcSettings[AdcPrimarySettingsListBox.SelectedIndex].CurrentSecondaryConfigName = (string)AdcSecondarySettingsListBox.SelectedItem;

            AdcSelectedSettingCommandTextBox.Text = AdcSettings[AdcPrimarySettingsListBox.SelectedIndex].Configs[AdcSecondarySettingsListBox.SelectedIndex].ConfigCommand;
            AdcSelectedSettingDescriptionTextBox.Text = AdcSettings[AdcPrimarySettingsListBox.SelectedIndex].ConfigDescription
                + System.Environment.NewLine + AdcSettings[AdcPrimarySettingsListBox.SelectedIndex].Configs[AdcSecondarySettingsListBox.SelectedIndex].ConfigDescription;

            string SettingsStr = "";
            foreach (AdcPrimarySettingClass i in AdcSettings)
            {
                SettingsStr += i.ConfigName;
                SettingsStr += ": ";
                SettingsStr += i.CurrentSecondaryConfigName;
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
                        CurrentSecondaryConfigName = "Low Power",
                        Configs = new List<AdcSecondarySettingStruct>
                        {
                           new AdcSecondarySettingStruct { ConfigName = "Low Power", ConfigDescription = "[Secondary]: POWER_MODE = 00", ConfigCommand = "REGM;01;6;2;0;"},
                           new AdcSecondarySettingStruct { ConfigName = "Mid Power", ConfigDescription = "[Secondary]: POWER_MODE = 01", ConfigCommand = "REGM;01;6;2;1;"},
                           new AdcSecondarySettingStruct { ConfigName = "Full Power", ConfigDescription = "[Secondary]: POWER_MODE = 10", ConfigCommand = "REGM;01;6;2;2;"},
                        }
                    },
                    new AdcPrimarySettingClass
                    (
                        "PGA",
                        "1",
                        new List<AdcSecondarySettingStruct>
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
                    ),
                    new AdcPrimarySettingClass
                    (
                        "Filter",
                        "Sinc4",
                        new List<AdcSecondarySettingStruct>
                        {
                           new AdcSecondarySettingStruct { ConfigName = "Sinc4", ConfigDescription = "[Secondary]: Filter = 000", ConfigCommand = "REGM;21;21;3;0;"},
                           new AdcSecondarySettingStruct { ConfigName = "Sinc3", ConfigDescription = "[Secondary]: Filter = 010", ConfigCommand = "REGM;21;21;3;2;"},
                           new AdcSecondarySettingStruct { ConfigName = "Sinc4+Sinc1", ConfigDescription = "[Secondary]: Filter = 100", ConfigCommand = "REGM;21;21;3;4;"},
                           new AdcSecondarySettingStruct { ConfigName = "Sinc3+Sinc1", ConfigDescription = "[Secondary]: Filter = 101", ConfigCommand = "REGM;21;21;3;5;"},
                           new AdcSecondarySettingStruct { ConfigName = "Post Filter", ConfigDescription = "[Secondary]: Filter = 111", ConfigCommand = "REGM;21;21;3;7;"},
                        }
                    ),
                    new AdcPrimarySettingClass
                    (
                        "Speed (SincX Filter)",
                        "384",
                        new List<AdcSecondarySettingStruct>
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
                    ),
                    new AdcPrimarySettingClass
                    (
                        "Speed (Post Filter)",
                        "25 SPS",
                        new List<AdcSecondarySettingStruct>
                        {
                           new AdcSecondarySettingStruct { ConfigName = "16.67 SPS", ConfigDescription = "[Secondary]: POST_FILTER = 010", ConfigCommand = "REGM;21;17;3;2;"},
                           new AdcSecondarySettingStruct { ConfigName = "20 SPS", ConfigDescription = "[Secondary]: POST_FILTER = 011", ConfigCommand = "REGM;21;17;3;3;"},
                           new AdcSecondarySettingStruct { ConfigName = "25 SPS", ConfigDescription = "[Secondary]: POST_FILTER = 101", ConfigCommand = "REGM;21;17;3;5;"},
                           new AdcSecondarySettingStruct { ConfigName = "27.27 SPS", ConfigDescription = "[Secondary]: POST_FILTER = 110", ConfigCommand = "REGM;21;17;3;6;"}
                        }
                    )
                };
            AdcSettings.AddRange(adcSettingsFunctionStruct);
        }

        public class AdcPrimarySettingClass
        {
            public string ConfigName;
            public string ConfigDescription;
            public string DefaultSecondaryConfigName;
            public string CurrentSecondaryConfigName;
            public List<AdcSecondarySettingStruct> Configs;

            public AdcPrimarySettingClass()
            {
                ConfigName = "";
                ConfigDescription = "[Primary]: No Description";
                DefaultSecondaryConfigName = "";
                CurrentSecondaryConfigName = "";
                Configs = new List<AdcSecondarySettingStruct>();
            }
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
        }
        public struct AdcSecondarySettingStruct
        {
            public string ConfigName;
            public string ConfigDescription;
            public string ConfigCommand;
        }
    }
}