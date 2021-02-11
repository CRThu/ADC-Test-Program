using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ADC_CDC_CONTROLLER
{
    /// <summary>
    /// XmlEditor.xaml 的交互逻辑
    /// </summary>
    public partial class XmlEditor : Window
    {
        public ObservableCollection<XmlTreeNode> ItemTreeDataList;

        public XmlEditor()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TreeViewItem treeNode = new TreeViewItem();
            treeNode.Header = "test1";
            xmlEditorXmlTreeView.Items.Add(treeNode);
            treeNode = new TreeViewItem();
            treeNode.Header = "test2";
            xmlEditorXmlTreeView.Items.Add(treeNode);
        }

        private void XmlEditerLoadXmlButton_Click(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void XmlEditerSaveXmlButton_Click(object sender, RoutedEventArgs e)
        {
            TreeViewItem treeNode = new TreeViewItem();
            treeNode.Header = "test3";
            xmlEditorXmlTreeView.Items.Add(treeNode);
        }

    }
}
