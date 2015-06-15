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
    public partial class UCRoutingList : UserControl
    {
        public String lang = (Application.Current as App).Language;
        public string status { get; set; }
        public Boolean hide { get; set; }
        public List<UCRoutingList> listpopup { get; set; }

        public UCRoutingList()
        {
            InitializeComponent();
            Loaded += UCRoutingList_Loaded;
        }

        private void UCRoutingList_Loaded(object sender, RoutedEventArgs e)
        {
            if (status == "1")
            {
                img_route.Source = new BitmapImage(new Uri("/Assets/up.png", UriKind.Relative));
            }
            else if (status == "2")
            {
                if (hide)
                {
                    btn_collapsed.Visibility = System.Windows.Visibility.Collapsed;
                }
                else
                {
                    btn_collapsed.Visibility = System.Windows.Visibility.Visible;
                }
                img_route.Source = new BitmapImage(new Uri("/Assets/change.png", UriKind.Relative));
            }
            else if (status == "3")
            {

                img_route.Source = new BitmapImage(new Uri("/Assets/down.png", UriKind.Relative));
            }
            else if (status == "4")
            {

                img_route.Source = new BitmapImage(new Uri("/Assets/walk.png", UriKind.Relative));
            }
        }

     
    }
}
