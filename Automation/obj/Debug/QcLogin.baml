   M S B A M L   `   `   `  ����I  ECANTxGenerator, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:CANTxGeneratorCANTxGenerator  5   	       CANTxGenerator.Window1   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   .localclr-namespace:CANTxGenerator     5   	   -   5        $��
WinQcLogin��5   :     � Title$  	QC Import��6	   $	��370��6   $	��420��6+   $	��420��6g   .��5	      � .+�5
   
   �� -   5         ��Name$ label��5
      $��	UserName:��6   $
��Left=�63   $��	55,38,0,0q�6N   $	��Top=�6a   �� 5   
   -   5         ��Name$ textBox��5      $
��Left=�6#   $��23��6>   $��
129,40,0,0q�6J    �� TextWrapping$
 Wrap=�6^   $	��Top=�6�   $	��228��6�   ��6r   ��  �� ElementName$ 
WinQcLogin�� �� Path$ QcImporter.UserName� �� Mode$ TwoWay=�6�   �� 5   
   -   5        $ 
label_Copy��5      $��	Password:��6$   $
��Left=�68   $��	55,77,0,0q�6S   $	��Top=�6f   G� 5   
   -   5         G�Name$ pwdBox��5      $
��Left=�6&   $��23��6A   $��
129,79,0,0q�6M   $	��Top=�6a   $	��228��6y   �� 5   
   -   5        $ label_Copy1��5      $��Domains:��6%   $
��Left=�68   $��
55,148,0,0q�6S   $	��Top=�6g   �� 5   
   -   5        $ label_Copy2��5      $��	Projects:��6%   $
��Left=�69   $��
55,191,0,0q�6T   $	��Top=�6h   �� 5   
   -   5         ��Name$ comboDomain��5      	 �� SelectedValuePath$	 Content��6(   #��["�6�   $
��Left=�61  $��129,148,0,0q�6L  $	��Top=�6a  $	��228��6y  
 �� SelectedItem
 6D   �� $ 
WinQcLogin��$ QcImporter.Domain�$ TwoWay=���6�   �� $ 
WinQcLogin��$! QcImporter.AuthrizedDomains�$ OneWay=�6y  �� 5   
   -	   5        $ comboPrj��5     $	 Content��6   #��["�6�   $
��Left=�6/  $��129,192,0,0q�6J  $	��Top=�6_  $	��228��6w  
 6/   �� $ 
WinQcLogin��$ QcImporter.Project�$ TwoWay=���6�   �� $ 
WinQcLogin��$" QcImporter.AuthrizedProjects�$ OneWay=�6w  �� 5   
   -
   5         ��Name$ btnLogin��5   _   
���P��#��[" �6$   $��Login��6q   $
��Left=�6�   $��224,121,0,0q�6�   $	��Top=�6�   $	��133��6�    J� RenderTransformOrigin$ -0.214,-1.108&�6�   6�   �� 5   
   -   5        $ 
btnConnect��5   �   
���P��#��[" �6l   $��Connect2Import��6�   $
��Left=�6�   $��224,226,0,0q�6�   $	��Top=�6  $	��133��6  $ -0.214,-1.108&�6(  {�6   �� $ 
WinQcLogin��$ QcImporter.IsLoggedIn�6N  � 5   
   -   5        $��pbBar��5      $
��Left=�6#   $��25��6>   $��
26,285,0,0q�6J   $	��Top=�6^   $	��362��6v   �� 5   
    �� Orientation$ 
Horizontal=�6   .+�5      �� -   5         ��Name$ tbTotal��5      $
��Left=�6�   $��
27,248,0,0q�6�   $	��Top=�6�   $��26��6�   $��47��6�   $ 0.978,0.633&�6�   ��6)   �� *QcImporter.Test2Import.Count+$ 
WinQcLogin�� �� StringFormat$ Total: {0:G}��6�   �� 5      -   5        $ lbShow��5      $�� ��6$   $
��Left=�6/   $��
27,248,0,0q�6J   $	��Top=�6^   $��84��6v   5      5      5      