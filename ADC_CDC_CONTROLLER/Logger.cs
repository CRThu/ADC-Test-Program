using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADC_CDC_CONTROLLER
{
    class Logger// : INotifyPropertyChanged
    {
        // Log Binding
        //public event PropertyChangedEventHandler PropertyChanged;
        private string logText;
        public string LogText
        {
            get
            {
                return logText;
            }
            set
            {
                if (logText != value)
                {
                    logText = value;
                    //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LogText"));
                }
            }
        }
    }
}
