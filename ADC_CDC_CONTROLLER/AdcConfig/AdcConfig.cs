using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADC_CDC_CONTROLLER
{
    [Serializable]
    public class AdcConfig : IEnumerable, ICloneable
    {
        public string Name { get; set; }
        public string Description { get; set; }

        // Current Configs
        public ObservableCollection<AdcConfigItem> CurrentConfigs { get; set; }
        public AdcConfigItem CurrentConfig
        {
            get { return CurrentConfigs.First(); }
            set { CurrentConfigs = new ObservableCollection<AdcConfigItem>() { value }; }
        }

        // Match String to AdcConfigItem
        public ObservableCollection<string> CurrentConfigsName
        {
            get { return new ObservableCollection<string>(CurrentConfigs.Select(item => item.Name)); }
            set
            {
                CurrentConfigs = new ObservableCollection<AdcConfigItem>();
                foreach (string currentconfigName in value)
                    CurrentConfigs.Add(Items.First(item => item.Name == currentconfigName));
            }
        }

        public string CurrentConfigName
        {
            get { return CurrentConfigsName.First(); }
            set { CurrentConfigsName = new ObservableCollection<string>() { value }; }
        }

        // Default Config
        private AdcConfigItem defaultConfig;
        public AdcConfigItem DefaultConfig
        {
            get { return defaultConfig; }
            set { defaultConfig = value; CurrentConfig = value; }
        }

        public string DefaultConfigName
        {
            get { return DefaultConfig.Name; }
            set { DefaultConfig = Items.First(Items => Items.Name == value); }
        }

        public ObservableCollection<AdcConfigItem> Items { get; set; }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)Items).GetEnumerator();
        }

        public object Clone()
        {
            return SerializeClone.DeepClone(this);
        }

        public AdcConfig()
        {
            Name = "";
            Description = "";
            //CurrentConfigs = new ObservableCollection<string>();
            //DefaultConfig = "";
            CurrentConfigs = new ObservableCollection<AdcConfigItem>();
            DefaultConfig = new AdcConfigItem();
            Items = new ObservableCollection<AdcConfigItem>();
        }

        public AdcConfig(string name, string description)
        {
            Name = name;
            Description = description;
            //CurrentConfigs = new ObservableCollection<string>();
            //DefaultConfig = "";
            CurrentConfigs = new ObservableCollection<AdcConfigItem>();
            DefaultConfig = new AdcConfigItem();
            Items = new ObservableCollection<AdcConfigItem>();
        }

        public AdcConfig(string name, string description, AdcConfigItem defaultConfig, ObservableCollection<AdcConfigItem> allConfigs)
        {
            Name = name;
            Description = description;
            //CurrentConfigs = new ObservableCollection<string>();
            //DefaultConfig = defaultConfig;
            CurrentConfigs = new ObservableCollection<AdcConfigItem>();
            DefaultConfig = defaultConfig;
            Items = allConfigs;
        }

    }
}
