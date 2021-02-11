using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADC_CDC_CONTROLLER
{
    public class XmlTreeNode
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public int ItemStep { get; set; }
        public int ItemParent { get; set; }
        public ObservableCollection<XmlTreeNode> Children { get; set; }

        public XmlTreeNode()
        {
            Children = new ObservableCollection<XmlTreeNode>();
        }
    }
}
