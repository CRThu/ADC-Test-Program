   M S B A M L   `   `   `  ���� W  SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U<clr-namespace:System.Windows.Input;assembly=PresentationCoreSystem.Windows.Input  U QInteractiveDataDisplay.WPF, Version=0.0.0.0, Culture=neutral, PublicKeyToken=nullkLclr-namespace:InteractiveDataDisplay.WPF;assembly=InteractiveDataDisplay.WPFInteractiveDataDisplay.WPF S OADC_CDC_CONTROLLER, Version=1.0.7745.1531, Culture=neutral, PublicKeyToken=null7 clr-namespace:ADC_CDC_CONTROLLERADC_CDC_CONTROLLER #   ADC_CDC_CONTROLLER.MainWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml      @d2http://schemas.microsoft.com/expression/blend/2008      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006      Ninput<clr-namespace:System.Windows.Input;assembly=PresentationCore      [d3Lclr-namespace:InteractiveDataDisplay.WPF;assembly=InteractiveDataDisplay.WPF      2local clr-namespace:ADC_CDC_CONTROLLER      -     � Title$   ADC Test Program by carrot��$��669.417��$��871.333��.��� $��0,0,5,1q�.+��� -    ��Name$ 
tabControl��$��10q�.@��� $��Debug������  �� EndPoint$	 0,1&� �� 
StartPoint$	 0,0&�.���  � Color$ 	#FFF0F0F0�� � Offset$ 0Q�� $ 	#FFEDFFFA��$ 1Q�.��� ���	#FFEDFFFA.+��� -    ��Name$ label��$	��COM��$
��Left=�$��	15,20,0,0q�$	��Top=��� -    ��Name$ serialPortComboBox1��$
��Left=�$��	71,22,0,0q�$	��Top=�$	��120���� -    ��Name$ serialPortConnectButton��$��Connect��$
��Left=�$��	21,61,0,0q�$	��Top=�$��76���� -   $  serialPortDisconnectButton��$��
Disconnect��$
��Left=�$��
115,61,0,0q�$	��Top=�$��76���� -   $# debugTabSerialPortStatusLabel��$��Disconnected��$
��Left=�$��
213,20,0,0q�$	��Top=��� $��
OPEN&RESET��$
��Left=�$��
16,110,0,0q�$	��Top=��� -   	 ��Name$	 serialPortCommInfoTextBox��$��382,10,10,10q�
 �� TextWrapping$

 Wrap=�" �� VerticalScrollBarVisibility$
 Auto=� �� AcceptsReturn . �� $
��DATW��$
��Left=�$��
16,140,0,0q�$	��Top=��� $
��DATR��$
��Left=�$��
16,170,0,0q�$	��Top=��� $
��REGW��$
��Left=�$��
16,200,0,0q�$	��Top=��� $
��REGR��$
��Left=�$��
16,230,0,0q�$	��Top=��� $��TASK1��$
��Left=�$��
21,340,0,0q�$	��Top=��� -	   $ cmdTASK1RealTimeButton��$��REALTIME��$
��Left=�$��271,378,0,0q�$	��Top=�$��90���� -
   $ cmdOPENButton��$
��OPEN��$
��Left=�$��285,113,0,0q�$	��Top=�$��76���� -   $ cmdDATWButton��$
��DATW��$
��Left=�$��285,143,0,0q�$	��Top=�$��76���� -   $ cmdDATRButton��$
��DATR��$
��Left=�$��285,173,0,0q�$	��Top=�$��76���� -   $ cmdREGWButton��$
��REGW��$
��Left=�$��285,203,0,0q�$	��Top=�$��76���� -   $ cmdREGRButton��$
��REGR��$
��Left=�$��285,233,0,0q�$	��Top=�$��76���� -   $ cmdTASK1RUNButton��$	��RUN��$
��Left=�$��271,342,0,0q�$	��Top=�$��90���� -   $	 cmdDATWTextBox1��$
��Left=�$��23��$��
93,142,0,0q�$

 Wrap=�$��45��$	��Top=�$��33�� �� 	MaxLength$ 2�� �� IsInputMethodEnabled .  $��Center=�$��Center=��� -   $	 cmdREGRTextBox1��$
��Left=�$��23��$��
93,231,0,0q�$

 Wrap=�$��31��$	��Top=�$��33��$ 2�� .  $��Center=�$��Center=��� -   $	 cmdREGWTextBox1��$
��Left=�$��23��$��
93,201,0,0q�$

 Wrap=�$	��Top=�$��33��$ 2�� .  $��Center=�$��Center=�$��31���� -   $	 cmdREGWTextBox2��$
��Left=�$��23��$��154,201,0,0q�$

 Wrap=�$	��Top=�$��58��$ 6�� .  $��Center=�$��Center=�$��5FFFFF���� -   $	 cmdTASK1RUNTextBox1��$
��Left=�$��24��$��
96,340,0,0q�$

 Wrap=�$	��Top=�$	��159��$ 20�� .  $��Center=�$��Center=�$��64���� $��	HEX INPUT��$
��Left=�$��
144,85,0,0q�$	��Top=��� $��0x��$
��Left=�$��
70,230,0,0q�$	��Top=��� $��0x��$
��Left=�$��
70,200,0,0q�$	��Top=��� $��0x��$
��Left=�$��131,200,0,0q�$	��Top=��� $��0x��$
��Left=�$��
70,141,0,0q�$	��Top=��� -   $ cmdLoadFromFileButton��$��	LOAD CMDs��$
��Left=�$��271,412,0,0q�$	��Top=�$��90���� -   $	 cmdLoadFromFileTextBox��$
��Left=�$��22��$��
46,412,0,0q�$

 Wrap=�$	��Top=�$��98���� $��	DEC INPUT��$
��Left=�$��144,316,0,0q�$	��Top=��� -   $	 debugTabRxBufSizeTextBox��$
��Left=�$��23��$��
307,72,0,0q�$

 Wrap=�$��65536��$	��Top=�$��54��$ 10��$��Center=�$��Center=��� $��RX__BUF__SIZE��$
��Left=�$��
212,71,0,0q�$	��Top=�$��89���� $
��COMM��$
��Left=�$��	86,85,0,0q�$	��Top=��� $
��TASK��$
��Left=�$��
86,316,0,0q�$	��Top=��� $
��File��$
��Left=�$��
16,410,0,0q�$	��Top=��� $��Interval��$
��Left=�$��144,410,0,0q�$	��Top=��� -   $	 cmdIntervalTextBox��$
��Left=�$��22��$��197,412,0,0q�$

 Wrap=�$��50��$	��Top=�$��38��$��Center=�$��Center=��� $��ms��$
��Left=�$��235,410,0,0q�$	��Top=��� -   $ cmdTASK1PACKButton��$
��PACK��$
��Left=�$��165,378,0,0q�$	��Top=�$��90���� -   $	 bytesPerCodeTextBox��$
��Left=�$��22��$��110,376,0,0q�$

 Wrap=�$��4��$	��Top=�$��44��$ 10��$��Center=�$��Center=��� $��BytesPerCode��$
��Left=�$��
16,374,0,0q�$	��Top=��� -   $	 writeCmdsTextBox��$
��Left=�$��68��$��
21,466,0,0q�$

 Wrap=�$	��Top=�$	��234�� . $
 Auto=��� -   $  writeCmdsFromTextBoxButton��$��
WRITE CMDs��$
��Left=�$��271,466,0,0q�$	��Top=�$��90��$��44���� -   $ saveCmdsToFileButton_Copy��$��	SAVE CMDs��$
��Left=�$��271,515,0,0q�$	��Top=�$��90���� $
��REGM��$
��Left=�$��
16,259,0,0q�$	��Top=��� -   $ cmdREGMButton��$
��REGM��$
��Left=�$��285,262,0,0q�$	��Top=�$��76���� -   $	 cmdREGMTextBox1��$
��Left=�$��23��$��
93,260,0,0q�$

 Wrap=�$��21��$	��Top=�$��33��$ 2�� .  $��Center=�$��Center=��� $��0x��$
��Left=�$��
70,259,0,0q�$	��Top=��� -    $	 cmdREGMTextBox2��$
��Left=�$��23��$��140,260,0,0q�$

 Wrap=�$
��10:0��$	��Top=�$��50�� .  $��Center=�$��Center=��� $��[��$
��Left=�$��126,259,0,0q�$	��Top=��� $
��] 0x��$
��Left=�$��190,260,0,0q�$	��Top=��� -!   $	 cmdREGMTextBox3��$
��Left=�$��23��$��220,260,0,0q�$

 Wrap=�$	��Top=�$��55��$ 6�� .  $��Center=�$��Center=�$	��7FF���� $��TIMEOUT��$
��Left=�$��
212,45,0,0q�$	��Top=�$��89���� -"   $	 debugTabRxTimeOutTextBox��$
��Left=�$��23��$��
307,46,0,0q�$

 Wrap=�$
��1000��$	��Top=�$��54��$ 10��$��Center=�$��Center=��� -#   $ cmdRESETButton��$��RESET��$
��Left=�$��186,113,0,0q�$	��Top=�$��76���� $
��REGQ��$
��Left=�$��
16,289,0,0q�$	��Top=��� -$   $ cmdREGQButton��$
��REGQ��$
��Left=�$��285,292,0,0q�$	��Top=�$��76���� -%   $	 cmdREGQTextBox1��$
��Left=�$��23��$��
93,290,0,0q�$

 Wrap=�$��21��$	��Top=�$��33��$ 2�� .  $��Center=�$��Center=��� $��0x��$
��Left=�$��
70,289,0,0q�$	��Top=��� -&   $	 cmdREGQTextBox2��$
��Left=�$��23��$��140,290,0,0q�$

 Wrap=�$
��10:0��$	��Top=�$��50�� .  $��Center=�$��Center=��� $��[��$
��Left=�$��126,289,0,0q�$	��Top=��� $��]��$
��Left=�$��190,289,0,0q�$	��Top=��� -'   $ saveLogsToFileButton��$��	SAVE Logs��$
��Left=�$��271,539,0,0q�$	��Top=�$��90���� -(   $ clearLogsButton��$��
Clear Logs��$
��Left=�$��165,539,0,0q�$	��Top=�$��90���� -)   $ cmdVCPTESTButton��$��VCP.TEST��$
��Left=�$��271,436,0,0q�$	��Top=�$��90���� -*   $	 cmdVCPTESTPacketsTextBox��$
��Left=�$��19��$��165,436,0,0q�$

 Wrap=�$	��Top=�$��90��$ 20�� .  $��Center=�$��Center=�$��25���� $��Setting������ $	 0,1&�$	 0,0&�.��� $ 	#FFF0F0F0��$ 0Q�� $ 	#FFEDFFFA��$ 1Q�.��� ���	#FFEDFFFA.+��� -+   $ label1��$��ADC Registers��$
��Left=�$��	10,11,0,0q�$	��Top=��� -,   $ button��$��Update��$
��Left=�$��
143,14,0,0q�$	��Top=�$��75���� --   $	 textBox��$
��Left=�$��23��$��	82,41,0,0q�$

 Wrap=�$��00��$	��Top=�$��48��$��Center=�$��Center=��� -.   $	 textBox_Copy��$
��Left=�$��23��$��
172,41,0,0q�$

 Wrap=�$��38��$	��Top=�$��46��$��Center=�$��Center=��� -/   $ label1_Copy��$
��Addr��$
��Left=�$��	10,39,0,0q�$	��Top=��� -0   $ label1_Copy1��$
��- 0x��$
��Left=�$��
135,39,0,0q�$	��Top=��� -1   $ label1_Copy2��$��0x��$
��Left=�$��	54,39,0,0q�$	��Top=��� -2    ��Name$ listView��$
��Left=�$	��256��$��	10,69,0,0q�$	��Top=�$	��208�� �� View �� .P��� �� -3    ��Name$ AdcPrimarySettingsListBox��$
��Left=�$	��179��$��
232,69,0,0q�$	��Top=�$	��138���� -4   $! AdcSecondarySettingsListBox��$
��Left=�$	��179��$��
374,69,0,0q�$	��Top=�$	��138�� �� SelectionMode$ Extended=��� $��Configs��$
��Left=�$��
232,39,0,0q�$	��Top=��� $��SelectedCommand��$
��Left=�$��232,253,0,0q�$	��Top=��� -5   $&	  AdcSelectedSettingCommandTextBox��$
��Left=�$��24��$��356,254,0,0q�$

 Wrap=�$	��Top=�$	��156���� -6   $*	 $AdcSelectedSettingDescriptionTextBox��$
��Left=�$��42��$��232,283,0,0q�$

 Wrap=�$	��Top=�$	��280���� -7   $" AdcSettingSetConfigNowButton��$��Set Config Now��$
��Left=�$��402,330,0,0q�$	��Top=�$	��110��$��60���� -8   $% AdcSettingFileAppendTasksButton��$��Append
Task(s)��$
��Left=�$��402,448,0,0q�$	��Top=�$	��110��$��79���� -9   $% AdcSettingCreateTasksFileButton��$��Create Tasks File��$
��Left=�$��402,421,0,0q�$	��Top=�$	��110���� -:   $$	 AdcSettingTasksFileNameTextBox��$
��Left=�$��24��$��
66,419,0,0q�$

 Wrap=�$	��Top=�$	��331���� $
��File��$
��Left=�$��
19,418,0,0q�$	��Top=��� -;   $#	 AdcAllSelectedSettingsTextBox��$
��Left=�$��84��$��
66,330,0,0q�$

 Wrap=�$	��Top=�$	��331��$
 Auto=��� -<   $" AdcSettingGetConfigNowButton��$��Get Config Now��$
��Left=�$��402,395,0,0q�$	��Top=�$	��110��{�.  �� -=   $	 AdcSettingsInfoTextBox��$��528,14,10,9.5q�$

 Wrap=�$
 Auto=� . �� ->   $$	 AdcSettingTaskAddonCmdsTextBox��$
��Left=�$��66,448,0,9.5q�$

 Wrap=�$	��331�� . $
 Auto=�$����<setAnalysisParams>PGA;%PGA%;Filter;%Filter%;</setAnalysisParams>
TASK1.RUN;1024;
<waitCmd/>
TASK1.PACK;1024;
<storeDic>PGA.%PGA%;Filter.%Filter%</storeDic>
<storeFile>data/PGA.%PGA%;Filter.%Filter%.txt</storeFile>���� $��Addon��$
��Left=�$��
12,450,0,0q�$	��Top=��� -?    ��Name$% AdcSettingTaskAddonCmdsCheckBox��$�� ��$
��Left=�$��
28,480,0,0q�$	��Top=� d� 	IsChecked$
 TrueY��� -@   $" AdcSettingOpenCmdsFileButton��$
��View��$
��Left=�$��402,532,0,0q�$	��Top=�$	��110���� $)��#Use Ctrl/Shift to append multiTasks��$
��Left=�$��
298,39,0,0q�$	��Top=��� $��Configs��$
��Left=�$��
12,330,0,0q�$	��Top=��� -A   $ settingTabXmlEditorButton��$��
XML Editor��$
��Left=�$��
424,14,0,0q�$	��Top=�$��88���� -B   $ settingTabLoadXmlButton��$��Load XML��$
��Left=�$��
232,14,0,0q�$	��Top=�$��88���� $
��Task������ $	 0,1&�$	 0,0&�.��� $ 	#FFF0F0F0��$ 0Q�� $ 	#FFEDFFFA��$ 1Q�.��� ���	#FFEDFFFA.+��� -C   $ taskTabLoadTaskFileButton��$��Load Task File��$
��Left=�$��
250,13,0,0q�$	��Top=�$	��100���� -D   $	 taskTabTaskFileTextBox��$
��Left=�$��24��$��	74,10,0,0q�$

 Wrap=�$	��Top=�$	��171���� $��	Task File��$
��Left=�$��	10,10,0,0q�$	��Top=��� -E   $ taskTabTaskTxtListView��$
��Left=�$	��464��$��	10,88,0,0q�$	��Top=�$	��438������  �� .P��� $��Lines�� �� Width$ 50�� �� DisplayMemberBinding �� *Lines+�� $��Commands�� �� *
Commands+�� $��Line Count : ��$
��Left=�$��	10,35,0,0q�$	��Top=��� $��/��$
��Left=�$��
145,35,0,0q�$	��Top=��� $
��Left=�$��23��$��	99,37,0,0q�$

 Wrap=�$��0��$	��Top=�$��41��$��Center=�$��Center=��� -F   $ taskTabFileLineCountLabel��$��0��$
��Left=�$��
165,35,0,0q�$	��Top=�$��41��$��Center=�$��Center=��� $��Run At Selected Line��$
��Left=�$��
211,39,0,0q�$	��Top=�$	��139��{�.  �� $��Task Count : ��$
��Left=�$��	10,59,0,0q�$	��Top=��� $��/��$
��Left=�$��
145,59,0,0q�$	��Top=��� $
��Left=�$��23��$��	99,61,0,0q�$

 Wrap=�$��0��$	��Top=�$��41��$��Center=�$��Center=��� -G   $ taskTabFileTaskCountLabel��$��0��$
��Left=�$��
165,59,0,0q�$	��Top=�$��41��$��Center=�$��Center=��� $��Run At Selected Task��$
��Left=�$��
211,63,0,0q�$	��Top=�$	��139��{�.  �� -H   $ taskTabTaskRunButton��$	��Run��$
��Left=�$��
355,13,0,0q�$	��Top=�$��93��$��45���� -I   $ taskTabTaskPauseButton��$��Pause��$
��Left=�$��
355,63,0,0q�$	��Top=�$��93��$��19��{�.  �� -J   $	 taskTabLoggerTextBox��$
��Left=�$	��542��$��
453,10,0,0q�$

 Wrap=�$	��Top=�$	��373��$
��TODO���� $
��Data������ $	 0,1&�$	 0,0&�.��� $ 	#FFF0F0F0��$ 0Q�� $ 	#FFEDFFFA��$ 1Q�.��� ���	#FFEDFFFA.+��� -K   $	 dataTabStorageTextBox��$
��Left=�$	��246��$��
441,38,0,0q�$

 Wrap=�$	��Top=�$	��165��$
 Auto=��� -L   $ dataTabAdcSamplesListBox��$
��Left=�$	��246��$��	10,38,0,0q�$	��Top=�$	��220���� -M   $  dataTabUpdateListBoxButton��$��Update Storage From App��$
��Left=�$��
10,307,0,0q�$	��Top=�$	��220���� $��Storage List��$
��Left=�$��	10,10,0,0q�$	��Top=��� $
��Code��$
��Left=�$��
441,10,0,0q�$	��Top=��� -N   $ dataTabStoreStorageButton��$��Store Storage To File��$
��Left=�$��
10,331,0,0q�$	��Top=�$	��220���� -O   $ dataTabLoadStorageButton��$��Load Storage From File��$
��Left=�$��
10,355,0,0q�$	��Top=�$	��220���� $��Voltage(PGA=1)��$
��Left=�$��
624,10,0,0q�$	��Top=��� -P   $	 dataTabVoltageTextBox��$
��Left=�$	��246��$��
624,38,0,0q�$

 Wrap=�$	��Top=�$	��165��$
 Auto=��� -Q   $&	  dataTabStorageSettingInfoTextBox��$
��Left=�$	��246��$��
235,38,0,0q�$

 Wrap=�$	��Top=�$	��165��$
 Auto=��� $��Config Info��$
��Left=�$��
235,10,0,0q�$	��Top=��� -R   $ dataTabClearStorageButton��$��Clear Storage��$
��Left=�$��
10,379,0,0q�$	��Top=�$	��220���� $��Static Report������ $	 0,1&�$	 0,0&�.��� $ 	#FFF0F0F0��$ 0Q�� $ 	#FFEDFFFA��$ 1Q�.��� ���	#FFEDFFFA.+��� $��Config Info��$
��Left=�$��	10,10,0,0q�$	��Top=��� -S   $* $noiseTestTabPrimaryConfigInfoListBox��$
��Left=�$	��180��$��	10,40,0,0q�$	��Top=�$	��138���� -T   $, &noiseTestTabSecondaryConfigInfoListBox��$
��Left=�$	��180��$��
153,40,0,0q�$	��Top=�$	��138��$ Extended=��� -U   $% noiseTestTabUpdateStorageButton��$��Update Storage From Data��$
��Left=�$��
10,342,0,0q�$	��Top=�$	��220���� -V   $#	 noiseTestTabConfigViewTextBox��$
��Left=�$	��112��$��
10,225,0,0q�$

 Wrap=�$	��Top=�$	��281��$
 Auto=�	� -W    	�Name$' !noiseTestTabSingleModeRadioButton��$��Single Mode��$
��Left=�$��
375,16,0,0q�$	��Top=�{�.  	� -X   $&  noiseTestTabChartModeRadioButton��$��
Chart Mode��$
��Left=�$��
507,16,0,0q�$	��Top=�{�.  	� -Y   $' !noiseTestTabReportModeRadioButton��$��
ReportMode��$
��Left=�$��
627,16,0,0q�$	��Top=�� -Z    �Name$ SingleModeGrid��$��296,40,9.5,9.5q�$y�	Collapsed=�.+���  �� TextAlignment$ Center=�$��Bottom=�$��Center=�$y�	Collapsed=�Single Mode Grid&   System.Windows.Controls.DataGrid  -[     Name$% staticTestTabSingleModeDataGrid��$��89��$��
10,10,10,0q�$	��Top=�   CanUserAddRows .  ���� �� -\   $1 +staticTestTabSingleModeCalcResolutionButton��$��Calc Resolution��$
��Left=�$��
38,104,0,0q�$	��Top=�$	��102��&   InteractiveDataDisplay.WPF.Chart     	LeftTitle$
 Code��$��10,128,10,0q�   LegendVisibility$ 	Collapsed=�$	��183��$	��Top=�.��*  $InteractiveDataDisplay.WPF.LineGraph  -]     Name$ 
linegraph1��   Stroke
 ��d�    StrokeThickness$	  1.5Q�  $ Counts��$��10,316,10,0q�$ 	Collapsed=�!   BottomTitle$
! Code��$	��186��$	��Top=�.��)  #InteractiveDataDisplay.WPF.BarGraph  -^   "  Name$" 	bargraph1��#   Color$# CornflowerBlue��,  &InteractiveDataDisplay.WPF.MarkerGraph$   StrokeThickness$$ 1Q��� -_   $/ )staticTestTabSingleModeisBarScaleCheckBox��$
��Left=�$��165,108,0,0q�$	��Top=�$��BarScale��$
 TrueY��� -`   $/ )staticTestTabSingleModeisBarCountCheckBox��$
��Left=�$��252,108,0,0q�$	��Top=�$��BarCount��� -a   $ ChartModeGrid��$��296,40,9.5,9.5q�$y�	Collapsed=�.+�  -b   $$ staticTestTabChartModeDataGrid��$	��320��$��
10,10,10,0q�$	��Top=� .  ���� �� -c   $, &staticTestTabChartModeRotationCheckBox��$��Rot(X,Y)��$
��Left=�$��
10,355,0,0q�$	��Top=��� -d   $0 *staticTestTabChartModeCalcResolutionButton��$��Calc Resolution��$
��Left=�$��111,352,0,0q�$	��Top=�$	��102���� -e   $* $staticTestTabChartModeStoreCsvButton��$��	Store CSV��$
��Left=�$��240,352,0,0q�$	��Top=�$	��102��� -f   $ ReportModeGrid��$��296,40,9.5,9.5q�$y�	Collapsed=�.+�  -g   $% staticTestTabReportModeDataGrid��$��
10,10,10,0q� .  $	��468��$	��Top=����� �� -h   $1 +staticTestTabReportModeCalcResolutionButton��$��Calc Resolution��$
��Left=�$��
92,483,0,0q�$	��Top=�$	��102���� -i   $+ %staticTestTabReportModeStoreCsvButton��$��	Store CSV��$
��Left=�$��221,483,0,0q�$	��Top=�$	��102���� -j   % ��Name$% groupBox��$��LSB Voltage Calculator��$
��Left=�$	��190��$��
10,362,0,0q�$	��Top=�$	��281��.��� .+��� $��Bipolar��$
��Left=�$��10,9,0,0q�$	��Top=��� -k   $$ staticTestTabisBipolarCheckBox��$��	isBipolar��$
��Left=�$��	76,15,0,0q�$	��Top=�$
 TrueY��� $
��VRef��$
��Left=�$��	10,33,0,0q�$	��Top=��� -l   $	 staticTestTabVrefTextBox��$
��Left=�$��22��$��	76,35,0,0q�$

 Wrap=�$	��Top=�$��69��$	��2.5��$��Center=�$��Center=��� $
��Gain��$
��Left=�$��	11,61,0,0q�$	��Top=��� -m   $	 staticTestTabGainTextBox��$
��Left=�$��22��$��	76,63,0,0q�$

 Wrap=�$	��Top=�$��69��$��Center=�$��Center=�$��1���� $��AdcBits��$
��Left=�$��	11,87,0,0q�$	��Top=��� -n   $!	 staticTestTabAdcBitsTextBox��$
��Left=�$��22��$��	76,89,0,0q�$

 Wrap=�$	��Top=�$��69��$��Center=�$��Center=�$��24���� -o   $ button1��$��Load From App��$
��Left=�$��
10,117,0,0q�$	��Top=�$	��135��{�.  �� -p   $) #staticTestTabisGainVariableCheckBox��$
��Left=�$��
154,67,0,0q�$	��Top=��� $	��LSB��$
��Left=�$��
10,138,0,0q�$	��Top=��� -q   $	 staticTestTabLSBTextBox��$
��Left=�$��22��$��
76,141,0,0q�$

 Wrap=�$	��Top=�$��69��$��Center=�$��Center=�{�.  �� -r   $' !staticTestTabGainVariableComboBox��$
��Left=�$	��Top=�$��84��$��
175,64,0,0q�{�.  �� $��
IsVariable��$
��Left=�$��175,141,0,0q�$	��Top=��� $��About������ $	 0,1&�$	 0,0&�.��� $ 	#FFF0F0F0��$ 0Q�� $ 	#FFEDFFFA��$ 1Q�.��� ���	#FFEDFFFA.+��� -s   & ��Name$!& AboutTabAppVersionTextBlock��$$��App Version:1.0.*
Build Time:*��