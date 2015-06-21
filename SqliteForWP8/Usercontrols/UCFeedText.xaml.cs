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
    public partial class UCFeedText : UserControl
    {
        public string setTag { get; set; }
        public String lang = (Application.Current as App).Language;
        public UCFeedText()
        {
            InitializeComponent();
            Loaded += UCFeedList_Loaded;
        }

        private void UCFeedList_Loaded(object sender, RoutedEventArgs e)
        {
           
        }

    }
}
