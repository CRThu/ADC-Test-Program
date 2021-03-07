using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;

namespace ADC_CDC_CONTROLLER
{
    public enum ConfigStroageExtension
    {
        Xml
        /*
         * XML Format
         *  adc
         *      info
         *          id,name,version,bit
         *      configs
         *          config[]
         *              name,description,default
         *              items[]
         *                  item
         *                      name,description,command
         */
    }

    public class AdcConfigCollection : IEnumerable
    {
        // Keys: id name version bit 
        public Dictionary<string, string> AdcInfos;
        public ObservableCollection<AdcConfig> AdcConfigs;

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)AdcConfigs).GetEnumerator();
        }

        public AdcConfigCollection()
        {
            AdcInfos = new Dictionary<string, string>();
            AdcConfigs = new ObservableCollection<AdcConfig>();
        }

        public AdcConfigCollection(Dictionary<string, string> adcInfos, ObservableCollection<AdcConfig> configs)
        {
            AdcInfos = adcInfos;
            AdcConfigs = configs;
        }

        public void LoadConfigs(string path, ConfigStroageExtension ext)
        {
            if (ext == ConfigStroageExtension.Xml)
                LoadConfigsFromXml(path, AdcInfos, AdcConfigs);
        }

        private void LoadConfigsFromXml(string path, Dictionary<string, string> adcInfos, ObservableCollection<AdcConfig> adcConfigs)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(path);

            // Root: <info>
            XmlNode InfoXmlNode = doc.SelectSingleNode("adc/info");
            //string xmlContent = InfoXmlNode.Name + "\n";
            foreach (XmlNode xmlNode in InfoXmlNode.ChildNodes)
            {
                // Child: <id> <name> <version> <bit> 
                // #text
                adcInfos.Add(xmlNode.Name, xmlNode.ChildNodes[0].InnerText);
                //xmlContent += "." + xmlNode.Name + "." + xmlNode.ChildNodes[0].InnerText + "\n";
            }
            // Root: <configs>
            XmlNode ConfigsXmlNode = doc.SelectSingleNode("adc/configs");
            //xmlContent += ConfigsXmlNode.Name + "\n";
            foreach (XmlNode ConfigXmlNode in ConfigsXmlNode.ChildNodes)
            {
                // Child: <config> <config>
                AdcConfig adcConfig = new AdcConfig();
                foreach (XmlNode ConfigXmlChildNode in ConfigXmlNode.ChildNodes)
                {
                    if (!ConfigXmlChildNode.Name.Equals("items"))
                    {
                        // Child: <name> <description> <default>
                        //xmlContent += "." + ConfigXmlNode.Name + "." + ConfigXmlChildNode.Name + "." + ConfigXmlChildNode.InnerText + "\n";
                        switch (ConfigXmlChildNode.Name)
                        {
                            case "name": adcConfig.Name = ConfigXmlChildNode.InnerText; break;
                            case "description": adcConfig.Description = ConfigXmlChildNode.InnerText; break;
                            case "default": adcConfig.DefaultConfig = ConfigXmlChildNode.InnerText; break;
                            default: break;
                        }
                    }
                    else
                    {
                        // Child: <items>
                        foreach (XmlNode ItemXmlNode in ConfigXmlChildNode.ChildNodes)
                        {
                            // Child: <item> <item>
                            AdcConfigItem adcConfigItem = new AdcConfigItem();
                            foreach (XmlNode ItemXmlChildNode in ItemXmlNode.ChildNodes)
                            {
                                // Child: <name> <description> <command>
                                //xmlContent += "." + ConfigXmlNode.Name + "." + ConfigXmlChildNode.Name + "." + ItemXmlNode.Name + "." + ItemXmlChildNode.Name + "." + ItemXmlChildNode.InnerText + "\n";
                                switch (ItemXmlChildNode.Name)
                                {
                                    case "name": adcConfigItem.Name = ItemXmlChildNode.InnerText; break;
                                    case "description": adcConfigItem.Description = ItemXmlChildNode.InnerText; break;
                                    case "command": adcConfigItem.Command = ItemXmlChildNode.InnerText; break;
                                    default: break;
                                }
                            }
                            adcConfig.Items.Add(adcConfigItem);
                        }
                    }
                }
                adcConfigs.Add(adcConfig);
            }
        }

        public void StoreConfigs(string path, ConfigStroageExtension ext)
        {
            if (ext == ConfigStroageExtension.Xml)
                StoreConfigsToXml(path);
        }

        private void StoreConfigsToXml(string path)
        {

        }

    }
}
