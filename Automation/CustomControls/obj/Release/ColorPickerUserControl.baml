   M S B A M L   `   `   `  ����   CustomControls+    %CustomControls.ColorPickerUserControl   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     -   $��colorPicker��.���   � RowDefinitions  �� $
��Auto ��� $
��Auto ��� $
��Auto � � ColumnDefinitions �� �� $
��Auto �.+��� -   $��	sliderRed�� � Minimum$ 0Q� � Maximum$	 255Q�����  �� ElementName$ colorPicker�� �� Path$ Padding� � Value �� $ colorPicker��$	 Red��� -   $��sliderGreen��$��1��$ 0Q�$	 255Q����� $ colorPicker��$ Padding� �� $ colorPicker��$ Green��� -   $��
sliderBlue��$��2��$ 0Q�$	 255Q����� $ colorPicker��$ Padding� �� $ colorPicker��$
 Blue��� $��1��$��3��$��50��
���   �$��1������ $ colorPicker��$ Padding�����  �� Color �� $ colorPicker��$ Color�