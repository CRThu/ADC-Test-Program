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

        List<AdcPrimarySettingStruct> AdcSettings = new List<AdcPrimarySettingStruct>();

        private void AdcPrimarySettingsListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            UpdateAdcSecondarySettingsListBox();
            UpdateAdcSettingCommand();
        }

        private void AdcSecondarySettingsListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            UpdateAdcSettingCommand();
        }

        private void UpdateAdcSecondarySettingsListBox()
        {
            if (AdcPrimarySettingsListBox.SelectedIndex == -1)
                return;

            AdcSecondarySettingsListBox.ItemsSource = AdcSettings[AdcPrimarySettingsListBox.SelectedIndex].Configs.Select(t => t.ConfigName).ToList();
            // TODO:save settings index
            AdcSecondarySettingsListBox.SelectedItem = AdcSettings[AdcPrimarySettingsListBox.SelectedIndex].DefaultSecondaryConfigName;
        }
        private void UpdateAdcSettingCommand()
        {
            if (AdcPrimarySettingsListBox.SelectedIndex == -1 || AdcSecondarySettingsListBox.SelectedIndex == -1)
                return;
            AdcSelectedSettingCommandTextBox.Text = AdcSettings[AdcPrimarySettingsListBox.SelectedIndex].Configs[AdcSecondarySettingsListBox.SelectedIndex].ConfigCommand;
            AdcSelectedSettingDescriptionTextBox.Text = AdcSettings[AdcPrimarySettingsListBox.SelectedIndex].ConfigDescription
                + System.Environment.NewLine + AdcSettings[AdcPrimarySettingsListBox.SelectedIndex].Configs[AdcSecondarySettingsListBox.SelectedIndex].ConfigDescription;
        }


        private void InitAdcSettings()
        {
            List<AdcPrimarySettingStruct> adcSettingsFunctionStruct =
                new List<AdcPrimarySettingStruct>
                {
                    new AdcPrimarySettingStruct
                    {
                        ConfigName = "Power Mode",
                        ConfigDescription = "[Primary]: No Description",
                        DefaultSecondaryConfigName = "Low Power",
                        Configs = new List<AdcSecondarySettingStruct>
                        {
                           new AdcSecondarySettingStruct { ConfigName = "Low Power", ConfigDescription = "[Secondary]: No Description", ConfigCommand = ""},
                           new AdcSecondarySettingStruct { ConfigName = "Mid Power", ConfigDescription = "[Secondary]: No Description", ConfigCommand = ""},
                           new AdcSecondarySettingStruct { ConfigName = "Full Power", ConfigDescription = "[Secondary]: No Description", ConfigCommand = ""},
                        }
                    },
                    new AdcPrimarySettingStruct
                    {
                        ConfigName = "PGA",
                        ConfigDescription = "[Primary]: No Description",
                        DefaultSecondaryConfigName = "1",
                        Configs = new List<AdcSecondarySettingStruct>
                        {
                           new AdcSecondarySettingStruct { ConfigName = "1", ConfigDescription = "[Secondary]: No Description", ConfigCommand = ""},
                           new AdcSecondarySettingStruct { ConfigName = "2", ConfigDescription = "[Secondary]: No Description", ConfigCommand = ""},
                           new AdcSecondarySettingStruct { ConfigName = "4", ConfigDescription = "[Secondary]: No Description", ConfigCommand = ""},
                           new AdcSecondarySettingStruct { ConfigName = "8", ConfigDescription = "[Secondary]: No Description", ConfigCommand = ""},
                           new AdcSecondarySettingStruct { ConfigName = "16", ConfigDescription = "[Secondary]: No Description", ConfigCommand = ""},
                           new AdcSecondarySettingStruct { ConfigName = "32", ConfigDescription = "[Secondary]: No Description", ConfigCommand = ""},
                           new AdcSecondarySettingStruct { ConfigName = "64", ConfigDescription = "[Secondary]: No Description", ConfigCommand = ""},
                           new AdcSecondarySettingStruct { ConfigName = "128", ConfigDescription = "[Secondary]: No Description", ConfigCommand = ""}
                        }
                    },
                    new AdcPrimarySettingStruct
                    {
                        ConfigName = "Filter",
                        ConfigDescription = "[Primary]: No Description",
                        DefaultSecondaryConfigName = "Sinc4",
                        Configs = new List<AdcSecondarySettingStruct>
                        {
                           new AdcSecondarySettingStruct { ConfigName = "Sinc4", ConfigDescription = "[Secondary]: No Description", ConfigCommand = ""},
                           new AdcSecondarySettingStruct { ConfigName = "Sinc3", ConfigDescription = "[Secondary]: No Description", ConfigCommand = ""},
                           new AdcSecondarySettingStruct { ConfigName = "Sinc4+Sinc1", ConfigDescription = "[Secondary]: No Description", ConfigCommand = ""},
                           new AdcSecondarySettingStruct { ConfigName = "Sinc3+Sinc1", ConfigDescription = "[Secondary]: No Description", ConfigCommand = ""},
                           new AdcSecondarySettingStruct { ConfigName = "Post Filter", ConfigDescription = "[Secondary]: No Description", ConfigCommand = ""},
                        }
                    },
                    new AdcPrimarySettingStruct
                    {
                        ConfigName = "Speed (SincX Filter)",
                        ConfigDescription = "[Primary]: No Description",
                        DefaultSecondaryConfigName = "384",
                        Configs = new List<AdcSecondarySettingStruct>
                        {
                           new AdcSecondarySettingStruct { ConfigName = "1", ConfigDescription = "[Secondary]: No Description", ConfigCommand = ""},
                           new AdcSecondarySettingStruct { ConfigName = "2", ConfigDescription = "[Secondary]: No Description", ConfigCommand = ""},
                           new AdcSecondarySettingStruct { ConfigName = "3", ConfigDescription = "[Secondary]: No Description", ConfigCommand = ""},
                           new AdcSecondarySettingStruct { ConfigName = "4", ConfigDescription = "[Secondary]: No Description", ConfigCommand = ""},
                           new AdcSecondarySettingStruct { ConfigName = "5", ConfigDescription = "[Secondary]: No Description", ConfigCommand = ""},
                           new AdcSecondarySettingStruct { ConfigName = "6", ConfigDescription = "[Secondary]: No Description", ConfigCommand = ""},
                           new AdcSecondarySettingStruct { ConfigName = "8", ConfigDescription = "[Secondary]: No Description", ConfigCommand = ""},
                           new AdcSecondarySettingStruct { ConfigName = "10", ConfigDescription = "[Secondary]: No Description", ConfigCommand = ""},
                           new AdcSecondarySettingStruct { ConfigName = "15", ConfigDescription = "[Secondary]: No Description", ConfigCommand = ""},
                           new AdcSecondarySettingStruct { ConfigName = "20", ConfigDescription = "[Secondary]: No Description", ConfigCommand = ""},
                           new AdcSecondarySettingStruct { ConfigName = "24", ConfigDescription = "[Secondary]: No Description", ConfigCommand = ""},
                           new AdcSecondarySettingStruct { ConfigName = "30", ConfigDescription = "[Secondary]: No Description", ConfigCommand = ""},
                           new AdcSecondarySettingStruct { ConfigName = "40", ConfigDescription = "[Secondary]: No Description", ConfigCommand = ""},
                           new AdcSecondarySettingStruct { ConfigName = "48", ConfigDescription = "[Secondary]: No Description", ConfigCommand = ""},
                           new AdcSecondarySettingStruct { ConfigName = "60", ConfigDescription = "[Secondary]: No Description", ConfigCommand = ""},
                           new AdcSecondarySettingStruct { ConfigName = "80", ConfigDescription = "[Secondary]: No Description", ConfigCommand = ""},
                           new AdcSecondarySettingStruct { ConfigName = "96", ConfigDescription = "[Secondary]: No Description", ConfigCommand = ""},
                           new AdcSecondarySettingStruct { ConfigName = "120", ConfigDescription = "[Secondary]: No Description", ConfigCommand = ""},
                           new AdcSecondarySettingStruct { ConfigName = "160", ConfigDescription = "[Secondary]: No Description", ConfigCommand = ""},
                           new AdcSecondarySettingStruct { ConfigName = "240", ConfigDescription = "[Secondary]: No Description", ConfigCommand = ""},
                           new AdcSecondarySettingStruct { ConfigName = "320", ConfigDescription = "[Secondary]: No Description", ConfigCommand = ""},
                           new AdcSecondarySettingStruct { ConfigName = "384", ConfigDescription = "[Secondary]: No Description", ConfigCommand = ""},
                           new AdcSecondarySettingStruct { ConfigName = "480", ConfigDescription = "[Secondary]: No Description", ConfigCommand = ""},
                           new AdcSecondarySettingStruct { ConfigName = "640", ConfigDescription = "[Secondary]: No Description", ConfigCommand = ""},
                           new AdcSecondarySettingStruct { ConfigName = "960", ConfigDescription = "[Secondary]: No Description", ConfigCommand = ""},
                           new AdcSecondarySettingStruct { ConfigName = "1280", ConfigDescription = "[Secondary]: No Description", ConfigCommand = ""},
                           new AdcSecondarySettingStruct { ConfigName = "1920", ConfigDescription = "[Secondary]: No Description", ConfigCommand = ""},
                           new AdcSecondarySettingStruct { ConfigName = "2047", ConfigDescription = "[Secondary]: No Description", ConfigCommand = ""},
                        }
                    },
                    new AdcPrimarySettingStruct
                    {
                        ConfigName = "Speed (Post Filter)",
                        ConfigDescription = "[Primary]: No Description",
                        DefaultSecondaryConfigName = "25 SPS",
                        Configs = new List<AdcSecondarySettingStruct>
                        {
                           new AdcSecondarySettingStruct { ConfigName = "16.67 SPS", ConfigDescription = "[Secondary]: No Description", ConfigCommand = ""},
                           new AdcSecondarySettingStruct { ConfigName = "20 SPS", ConfigDescription = "[Secondary]: No Description", ConfigCommand = ""},
                           new AdcSecondarySettingStruct { ConfigName = "25 SPS", ConfigDescription = "[Secondary]: No Description", ConfigCommand = ""},
                           new AdcSecondarySettingStruct { ConfigName = "27.27 SPS", ConfigDescription = "[Secondary]: No Description", ConfigCommand = ""}
                        }
                    }
                };
            AdcSettings.AddRange(adcSettingsFunctionStruct);
        }

        public struct AdcPrimarySettingStruct
        {
            public string ConfigName;
            public string ConfigDescription;
            public string DefaultSecondaryConfigName;
            public List<AdcSecondarySettingStruct> Configs;

            public AdcPrimarySettingStruct(string _configName, string _configDescription, string _defaultSecondaryConfigName, List<AdcSecondarySettingStruct> _configs)
            {
                ConfigName = _configName;
                ConfigDescription = _configDescription;
                DefaultSecondaryConfigName = _defaultSecondaryConfigName;
                Configs = _configs;
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