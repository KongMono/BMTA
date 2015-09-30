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
    public partial class UCLandMarkItem : UserControl
    {
        public String lang = (Application.Current as App).Language;

        public UCLandMarkItem()
        {
            InitializeComponent();
            Loaded += UCRoutingList_Loaded;
        }

        private void UCRoutingList_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void Grid_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

        }
    }
}
