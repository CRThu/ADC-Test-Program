   M S B A M L   `   `   `  ���� S  OADC_CDC_CONTROLLER, Version=1.0.7745.1531, Culture=neutral, PublicKeyToken=null7 clr-namespace:ADC_CDC_CONTROLLERADC_CDC_CONTROLLER  "    ADC_CDC_CONTROLLER.XmlEditor   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     2local clr-namespace:ADC_CDC_CONTROLLER     -     � Title$  	XmlEditor��$	��348��$��455.5��.��� .+��� -    ��Name$ xmlEditerLoadXmlButton��$��Load XML��$
��Left=�$��	10,10,0,0q�$	��Top=�$��84���� -   $ xmlEditerSaveXmlButton��$��Save XML��$
��Left=�$��	99,10,0,0q�$	��Top=�$��84���� -    ��Name$ xmlEditerXmlPathTextBox��$
��Left=�$��23��$��	188,6,0,0q� �� TextWrapping$
 Wrap=�$	��Top=�$	��252��R� -    R�Name$ xmlEditorXmlTreeView��$��10,34,10.5,9.5q����� ����  �� 
TargetType" Q�.�-   ��  �� Property �@Q�
IsExpanded �� Value �� *
IsExpanded+ �� Mode$ TwoWay=���  �@Q�
IsSelected �� *
IsSelected+$ TwoWay=���  � ��$ Normal"�	 �� Triggers	 P� 
 P� Property
 �@Q�
IsSelected P� Value . . ���  � ��$
 Bold"�����  �� ItemsSource �� *
Children+.R�� .+��� ���� *
ItemName+$ TwoWay=��� -   ���� *
ItemName+$ TwoWay=�y��� *
Visibility+$ TwoWay=� � ContextMenu �� -    ��Name$ ContextMenu_EditNode��.@�m� -	    m�Name$! xmlEditorRenameNodeMenuItem��$��Rename Node��m� -
   $( "xmlEditorCreateConfigNodesMenuItem��$��Create Config Nodes��m� -   $, &xmlEditorCreateConfigItemNodesMenuItem��$��Create Config Item Nodes��m� -   $! xmlEditorDeleteNodeMenuItem��$��Delete Node��