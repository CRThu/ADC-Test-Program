using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADC_CDC_CONTROLLER
{
    class AdcSampleData
    {
        // TODO
        public AdcConfigCollection AdcSampleConfigInfo;
        public ObservableCollection<ulong> AdcSampleDataCollection;

        public AdcSampleData()
        {
            AdcSampleConfigInfo = new AdcConfigCollection();
            AdcSampleDataCollection = new ObservableCollection<ulong>();
        }
    }
}
