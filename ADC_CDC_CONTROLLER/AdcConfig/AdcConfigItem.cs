using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADC_CDC_CONTROLLER
{
    public class AdcConfigItem
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
    }
}
