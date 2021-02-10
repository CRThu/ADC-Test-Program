using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADC_CDC_CONTROLLER
{
    class AdcConfig
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Config { get; set; }
        public string DefaultConfig { get; set; }
        public List<string> AllConfigs { get; set; }

        public KeyValuePair<string, string> CurrentConfigKeyValuePair
        {
            get { return new KeyValuePair<string, string>(Name, Config); }
            set { Name = value.Key; Config = value.Value; }
        }

        public KeyValuePair<string, string> DefaultConfigKeyValuePair
        {
            get { return new KeyValuePair<string, string>(Name, DefaultConfig); }
            set { Name = value.Key; DefaultConfig = value.Value; }
        }

        public AdcConfig()
        {
            Name = "";
            Description = "";
            Config = "";
            DefaultConfig = "";
            AllConfigs = new List<string>();
        }

        public AdcConfig(string name, string description)
        {
            Name = name;
            Description = description;
            Config = "";
            DefaultConfig = "";
            AllConfigs = new List<string>();
        }

        public AdcConfig(string name, string description, string config, string defaultConfig, List<string> allConfigs)
        {
            Name = name;
            Description = description;
            Config = config;
            DefaultConfig = defaultConfig;
            AllConfigs = allConfigs;
        }
    }
}
