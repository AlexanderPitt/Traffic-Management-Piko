   M S B A M L   `   `   `  ����  PikoTrafficManager5    /PikoTrafficManager.Control.PlaylistMediaElement   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      .��5      � -   5          �Name$  ElementLayout��5       � RowDefinitions 5	   
   �� 5
      �� 5      �� 5       � ColumnDefinitions 5   
   �� 5      $	��128 �6   �� 5      �� 5      $	��128 �6   �� 5      �� 5      $��48 �6   .+�5   
   �� $��0��6   $��2��6   
�������$��Center=�6@   $��Center=�6[   $��22(�6x   .��6�   MEDIA6�   �� 5   
   -   5         ��Name$ btEditElement��5      $y�Visible=�6(   $��Center=�6=   $��Center=�6X   $��0��6u   $��4��6�   ��5      �� 5      .R�5      �� $%��.\..\image\ConfigWheelWhite.png��6   5      5      �� 5   
   -   5        $ btDeleteItem��5      $y�Visible=�6'   $��1��6<   $��4��6I   $��Stretch=�6Y   $��Stretch=�6u   ��5      �� 5       .R�5!      �� $��.\..\image\BinWhite.png��6   5"      5$      �� 5'   
   -   5         ��Name$ lblPlaylistElementTitle��5'   -   $��0��6   $��0��6   
�������$��0,5,0,0q�6a   .��6r   Title6y   �� 5(   
   -   5         ��Name$ tbxPlaylistElementTitle��5(      $��Center=�63   $��0��6N   $��1��6[   6m   �� 5)   
   -   5        $  lblPlaylistElementFileName��5)   -   $��1��6   $��0��6   
�������$��0,5,0,0q�6d   .��6u   
Filename6   �� 5*   
   -   5        $  tbxPlaylistElementFilename��5*      $��Center=�66   $��1��6Q   $��1��6^   6p   �� 5+   
   -   5        $  lblPlaylistElementDuration��5+   -   $��1��6   $��2��6   
�������$��0,-2,0,0q�6d   
������ .��6�   
Duration6�   �� 5,   
   -	   5        $  tbxPlaylistElementDuration��5,      $	��Top=�66   $��0,3,0,0q�6N   $��Right=�6_   $��64��6{   $��18��6�   $��1��6�   $��2��6�   .��6�   00:00:00:006�   �� 5-   
   -
   5        $! lblPlaylistElementFrameRate��5-   -   $��1��6   $��2��6   
�������$��0,18,0,0q�6e   
������ .��6�   	FrameRate6�   �� 5.   
   -   5         ��Name$! tbxPlaylistElementFrameRate��5.      $	��Top=�68   $��0,22,0,0q�6P   $��Right=�6b   $��18��6~   $��64��6�   $��1��6�   $��2��6�   .@�6�   00:00:00:006�   5/      50      