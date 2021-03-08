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
        // TODO string -> AdcConfigItem
        ///*
        public ObservableCollection<string> CurrentConfigs { get; set; }
        public string CurrentConfig
        {
            get { return CurrentConfigs.First(); }
            set { CurrentConfigs.Clear(); CurrentConfigs.Add(value); }
        }
        private string defaultConfig;
        public string DefaultConfig
        {
            get { return defaultConfig; }
            set { defaultConfig = value; CurrentConfig = value; }
        }
        //*/
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
            CurrentConfigs = new ObservableCollection<string>();
            DefaultConfig = "";
            Items = new ObservableCollection<AdcConfigItem>();
        }

        public AdcConfig(string name, string description)
        {
            Name = name;
            Description = description;
            CurrentConfigs = new ObservableCollection<string>();
            DefaultConfig = "";
            Items = new ObservableCollection<AdcConfigItem>();
        }

        public AdcConfig(string name, string description, string defaultConfig, ObservableCollection<AdcConfigItem> allConfigs)
        {
            Name = name;
            Description = description;
            CurrentConfigs = new ObservableCollection<string>();
            DefaultConfig = defaultConfig;
            Items = allConfigs;
        }

    }
}
