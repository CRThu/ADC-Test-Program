﻿using System;
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

    class AdcConfigs
    {
        public string AdcName;
        public string AdcVersion;
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

        public AdcConfigs()
        {
            Configs = new List<AdcConfig>();
        }
        
        public AdcConfigs(List<AdcConfig> configs)
        {
            Configs = configs;
        }
    }
}