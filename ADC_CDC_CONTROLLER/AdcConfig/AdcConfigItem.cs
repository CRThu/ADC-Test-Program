using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace ADC_CDC_CONTROLLER
{
    [Serializable]
    public class AdcConfigItem : ICloneable
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Command { get; set; }

        public AdcConfigItem()
        {
            Name = "";
            Description = "";
            Command = "";
        }

        public AdcConfigItem(string name, string description, string command)
        {
            Name = name;
            Description = description;
            Command = command;
        }

        public object Clone()
        {
            return SerializeClone.DeepClone(this);
        }
    }
}
