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
    public partial class UCFeedList : UserControl
    {
        public string path_img { get; set; }
        public String lang = (Application.Current as App).Language;
        public UCFeedList()
        {
            InitializeComponent();
            Loaded += UCFeedList_Loaded;
        }

        private void UCFeedList_Loaded(object sender, RoutedEventArgs e)
        {
            img_feed.Source = new BitmapImage(new Uri(path_img, UriKind.RelativeOrAbsolute));
        }

    }
}
