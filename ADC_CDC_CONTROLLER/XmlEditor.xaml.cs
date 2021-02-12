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
        public ObservableCollection<XmlTreeNode> XmlTreeNodeList = XmlTreeNode.XmlAdcInfoInit();
        private object selectTreeViewTextBox;

        public XmlEditor()
        {
            InitializeComponent();
            xmlEditorXmlTreeView.ItemsSource = XmlTreeNodeList;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        // Select Item When Mouse Right Button Down
        private void TreeViewItem_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var treeViewItem = VisualUpwardSearch<TreeViewItem>(e.OriginalSource as DependencyObject) as TreeViewItem;
            if (treeViewItem != null)
            {
                treeViewItem.Focus();
                e.Handled = true;
            }
        }

        static DependencyObject VisualUpwardSearch<T>(DependencyObject source)
        {
            while (source != null && source.GetType() != typeof(T))
                source = VisualTreeHelper.GetParent(source);

            return source;
        }

        private void XmlEditerLoadXmlButton_Click(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void XmlEditerSaveXmlButton_Click(object sender, RoutedEventArgs e)
        {
            XmlTreeNodeList[0].Children
                .Where(node => node.ItemName.Equals("config")).First().Children
                .Where(node => node.ItemName.Equals("items")).First().XmlAddAdcConfigItem();
        }

        private void XmlEditorRenameNodeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            selectTreeViewTextBox = xmlEditorXmlTreeView.SelectedItem;
            ((XmlTreeNode)selectTreeViewTextBox).Visibility = Visibility.Visible;
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            ((XmlTreeNode)selectTreeViewTextBox).Visibility = Visibility.Collapsed;
        }

        private void XmlEditorCreateConfigNodesMenuItem_Click(object sender, RoutedEventArgs e)
        {
            XmlTreeNodeList.Where(node => node.ItemName.Equals("adc")).First().XmlAddAdcConfig();
        }

        private void xmlEditorCreateConfigItemNodesMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // TODO
            ((XmlTreeNode)xmlEditorXmlTreeView.SelectedItem).Children.Add(new XmlTreeNode() { ItemName = "test" });
        }
    }
}
