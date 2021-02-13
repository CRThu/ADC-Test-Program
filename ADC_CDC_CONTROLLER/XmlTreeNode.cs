using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ADC_CDC_CONTROLLER
{
    public class XmlTreeNode : INotifyPropertyChanged
    {
        public bool IsSelected { get; set; }
        public bool IsExpanded { get; set; }

        private string itemName;
        public string ItemName
        {
            get
            {
                return itemName;
            }
            set
            {
                itemName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ItemName"));
            }
        }
        private Visibility visibility;
        public Visibility Visibility
        {
            get
            {
                return visibility;
            }
            set
            {
                visibility = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Visibility"));
            }
        }
        // TODO
        // BUGS IN PARENT
        public XmlTreeNode Parent { get; set; }
        private ObservableCollection<XmlTreeNode> children;
        public ObservableCollection<XmlTreeNode> Children
        {
            get
            {
                return children;
            }
            set
            {
                children = value;
                foreach (var child in children)
                    child.Parent = this;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public XmlTreeNode()
        {
            IsSelected = false;
            IsExpanded = true;
            Visibility = Visibility.Collapsed;
            ItemName = "[NODE]";
            Parent = null;
            Children = new ObservableCollection<XmlTreeNode>();
        }

        public static ObservableCollection<XmlTreeNode> XmlAdcInfoInit() => new ObservableCollection<XmlTreeNode>()
        {
            new XmlTreeNode()
            {
                ItemName = "adc",
                Children = new ObservableCollection<XmlTreeNode>()
                {
                    new XmlTreeNode()
                    {
                        ItemName = "id",
                        Children = new ObservableCollection<XmlTreeNode>()
                        {
                            new XmlTreeNode()
                            {
                                ItemName = "[ADC ID]"
                            }
                        }
                    },
                    new XmlTreeNode()
                    {
                        ItemName = "name",
                        Children = new ObservableCollection<XmlTreeNode>()
                        {
                            new XmlTreeNode()
                            {
                                ItemName = "[ADC NAME]"
                            }
                        }
                    },
                    new XmlTreeNode()
                    {
                        ItemName = "version",
                        Children = new ObservableCollection<XmlTreeNode>()
                        {
                            new XmlTreeNode()
                            {
                                ItemName = "[ADC VERSION]"
                            }
                        }
                    },
                    new XmlTreeNode()
                    {
                        ItemName = "bit",
                        Children = new ObservableCollection<XmlTreeNode>()
                        {
                            new XmlTreeNode()
                            {
                                ItemName = "[ADC BIT]"
                            }
                        }
                    }
                }
            }
        };

        public void XmlAddAdcConfig()
        {
            if (!ItemName.Equals("adc"))
                throw new KeyNotFoundException();

            Children.Add(
                    new XmlTreeNode()
                    {
                        ItemName = "config",
                        Children = new ObservableCollection<XmlTreeNode>()
                        {
                            new XmlTreeNode()
                            {
                                ItemName = "name",
                                Children = new ObservableCollection<XmlTreeNode>()
                                {
                                    new XmlTreeNode()
                                    {
                                        ItemName = "[CFG NAME]"
                                    }
                                }
                            },
                            new XmlTreeNode()
                            {
                                ItemName = "description",
                                Children = new ObservableCollection<XmlTreeNode>()
                                {
                                    new XmlTreeNode()
                                    {
                                        ItemName = "[CFG DESCRIPTION]"
                                    }
                                }
                            },
                            new XmlTreeNode()
                            {
                                ItemName = "default",
                                Children = new ObservableCollection<XmlTreeNode>()
                                {
                                    new XmlTreeNode()
                                    {
                                        ItemName = "[CFG DEFAULT]"
                                    }
                                }
                            },
                            new XmlTreeNode()
                            {
                                ItemName = "items"
                            }
                        }
                    });
        }

        public void XmlAddAdcConfigItem()
        {
            if (!ItemName.Equals("items"))
                throw new KeyNotFoundException();

            Children.Add(new XmlTreeNode()
            {
                ItemName = "item",
                Children = new ObservableCollection<XmlTreeNode>()
                {
                    new XmlTreeNode()
                    {
                        ItemName = "name",
                        Children = new ObservableCollection<XmlTreeNode>()
                        {
                            new XmlTreeNode()
                            {
                                ItemName = "[CFG ITEM NAME]"
                            }
                        }
                    },
                    new XmlTreeNode()
                    {
                        ItemName = "description",
                        Children = new ObservableCollection<XmlTreeNode>()
                        {
                            new XmlTreeNode()
                            {
                                ItemName = "[CFG ITEM DESCRIPTION]"
                            }
                        }
                    },
                    new XmlTreeNode()
                    {
                        ItemName = "command",
                        Children = new ObservableCollection<XmlTreeNode>()
                        {
                            new XmlTreeNode()
                            {
                                ItemName = "[CFG ITEM CMD]"
                            }
                        }
                    },
                }
            });
        }
    }
}