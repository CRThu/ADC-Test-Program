   M S B A M L   `   `   `  ����S  OADC_CDC_CONTROLLER, Version=1.0.7745.1518, Culture=neutral, PublicKeyToken=null7 clr-namespace:ADC_CDC_CONTROLLERADC_CDC_CONTROLLER  5   	   "    ADC_CDC_CONTROLLER.XmlEditor   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   2local clr-namespace:ADC_CDC_CONTROLLER     5   	   -   5          � Title$  	XmlEditor��5   	   $	��348��6   $��455.5��6(   .��5	      � .+�5
   
   �� -   5         ��Name$ xmlEditerLoadXmlButton��5
      $��Load XML��61   $
��Left=�6D   $��	10,10,0,0q�6_   $	��Top=�6r   $��84��6�   6�   �� 5   
   -   5        $ xmlEditerSaveXmlButton��5      $��Save XML��61   $
��Left=�6D   $��	99,10,0,0q�6_   $	��Top=�6r   $��84��6�   6�   �� 5   
   -   5         ��Name$ xmlEditerXmlPathTextBox��5      $
��Left=�63   $��23��6N   $��	188,6,0,0q�6Z    �� TextWrapping$
 Wrap=�6m   $	��Top=�6�   $	��252��6�   R� 5   
   -   5         R�Name$ xmlEditorXmlTreeView��5      $��10,34,10.5,9.5q�6I   ��61   �� ��5      �� 5       �� 
TargetType" Q�6   .�5      -   5        �� 5       �� Property �@Q�
IsExpanded �� Value 63   �� *
IsExpanded+ �� Mode$ TwoWay=��� 5       �@Q�
IsSelected 63   �� *
IsSelected+$ TwoWay=��� 5       � ��$ Normal"�63   	 �� Triggers	 5      P� 5      
 P� Property
 �@Q�
IsSelected P� Value . . �5      ��  � ��$
 Bold"�6;   5      5      ��5      �� 5       �� ItemsSource 6+   �� *
Children+.R�5      � .+�5      �� ��6$   �� *
ItemName+$ TwoWay=��� 5      -   5        ��5   "   �� *
ItemName+$ TwoWay=�y�6I   �� *
Visibility+$ TwoWay=�6�   5       5!       � ContextMenu 5#      �� 5$      -   5         ��Name$ ContextMenu_EditNode��5$      .@�5%      m� -	   5         m�Name$! xmlEditorRenameNodeMenuItem��5%      $��Rename Node��6D   6Y   m� 5&      -
   5        $( "xmlEditorCreateConfigNodesMenuItem��5&      $��Create Config Nodes��6K   6h   m� 5'      -   5        $, &xmlEditorCreateConfigItemNodesMenuItem��5'      $��Create Config Item Nodes��6O   6q   m� 5(      -   5        $! xmlEditorDeleteNodeMenuItem��5(      $��Delete Node��6D   6Y   5)      5+      5,      5-      