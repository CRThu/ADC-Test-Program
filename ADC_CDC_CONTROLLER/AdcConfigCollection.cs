using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADC_CDC_CONTROLLER
{
    public enum ConfigStroageExtension
    {
        Xml
    }

    class AdcConfigCollection
    {
        public string AdcId;
        public string AdcName;
        public string AdcVersion;
        public int AdcBits;
        public List<AdcConfig> Configs;

        public void LoadConfigs(string path, ConfigStroageExtension ext)
        {
            if (ext == ConfigStroageExtension.Xml)
                LoadConfigsFromXml(path, Configs);
        }

        public void LoadConfigsFromXml(string path,List<AdcConfig> configs)
        {

        }

        public void StoreConfigs(string path, ConfigStroageExtension ext)
        {
            if (ext == ConfigStroageExtension.Xml)
                StoreConfigsToXml(path);
        }
        public void StoreConfigsToXml(string path)
        {

        }

        public AdcConfigCollection()
        {
            AdcId = "";
            AdcName = "";
            AdcVersion = "";
            AdcBits = 0;
            Configs = new List<AdcConfig>();
        }
        
        public AdcConfigCollection(List<AdcConfig> configs)
        {
            AdcId = "";
            AdcName = "";
            AdcVersion = "";
            AdcBits = 0;
            Configs = configs;
        }
    }
}
