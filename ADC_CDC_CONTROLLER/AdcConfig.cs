using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADC_CDC_CONTROLLER
{
    class AdcConfig
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string CurrentConfig { get; set; }
        public string DefaultConfig
        {
            get { return DefaultConfig; }
            set { DefaultConfig = value; CurrentConfig = value; }
        }
        public ObservableCollection<AdcConfigItem> Items { get; set; }

        /*
        public KeyValuePair<string, string> CurrentConfigKeyValuePair
        {
            get { return new KeyValuePair<string, string>(Name, CurrentConfig); }
            set { Name = value.Key; CurrentConfig = value.Value; }
        }

        public KeyValuePair<string, string> DefaultConfigKeyValuePair
        {
            get { return new KeyValuePair<string, string>(Name, DefaultConfig); }
            set { Name = value.Key; DefaultConfig = value.Value; }
        }
        */

        public AdcConfig()
        {
            Name = "";
            Description = "";
            DefaultConfig = "";
            Items = new ObservableCollection<AdcConfigItem>();
        }

        public AdcConfig(string name, string description)
        {
            Name = name;
            Description = description;
            DefaultConfig = "";
            Items = new ObservableCollection<AdcConfigItem>();
        }

        public AdcConfig(string name, string description, string defaultConfig, ObservableCollection<AdcConfigItem> allConfigs)
        {
            Name = name;
            Description = description;
            DefaultConfig = defaultConfig;
            Items = allConfigs;
        }
    }
}
