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

        public object Clone()
        {
            return DeepClone(this);
        }

        // Reference: https://www.jb51.net/article/67891.htm
        public static T DeepClone<T>(T obj)
        {
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, obj);
                stream.Position = 0;
                return (T)formatter.Deserialize(stream);
            }
        }

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
    }
}
