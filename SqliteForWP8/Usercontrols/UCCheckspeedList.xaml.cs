using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media.Imaging;
using BMTA.Item;
using System.Windows.Controls.Primitives;


namespace BMTA.Usercontrols
{
    public partial class UCCheckspeedList : UserControl
    {
        public String lang = (Application.Current as App).Language;
     

        public UCCheckspeedList()
        {
            InitializeComponent();
            
        }
    }
}
