   M S B A M L   `   `   `  ����M  IPikoTrafficManager, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullG(clr-namespace:PikoTrafficManager.ControlPikoTrafficManager.Control  5   	   /    )PikoTrafficManager.Control.MediaDetection   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   :local(clr-namespace:PikoTrafficManager.Control     5   	   -   5          �Name$  MediaDetectionForm��5   	    � Title$ New Media Detected��5	   	   $	��527��6$   $	��520��61   .��5
      � -   5         �Name$ MediaDetectionLayout��5
      ���	#FF454548 � ColumnDefinitions 5   
   �� 5       � RowDefinitions 5   
   �� 5      �� 5      $��48 �6   .+�5   
   �� -   5         ��Name$ 
svNewFiles��5   $   $��0��6   $
��Auto=�68   $��Hidden=�6[   .��5      �� -   5         ��Name$ spNewFilesInVolumes��5      5      5      �� 5   
   -   5         ��Name$ btAddSelected��5   u   $��1��6   $��Add Selected Files��6    � ToolTip$5 /Add Selected Discovered Files to Piko Data Base��6;   $
��48px��6�   $��128px��6�   $��Right=�6�   $��0,0,48,0q�6�   6�   �� 5   
   -   5        $ btCancel��5   .   $��1��6   $��Close��6   $
��48px��6@   $��128px��6N   $
��Left=�6\   $��48,0,0,0q�6w   6�   5      5      