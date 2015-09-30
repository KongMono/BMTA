using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO;
using System.Windows.Threading;
using Microsoft.Phone.Net.NetworkInformation;
using System.Windows.Media;
using System.Windows.Resources;
using System.Xml.Linq;
using BMTA.Usercontrols;

namespace BMTA
{
    public partial class BMTA_Howto : PhoneApplicationPage
    {
        public String lang = (Application.Current as App).Language;
        UCFeedText text = new UCFeedText();

        public BMTA_Howto()
        {
            InitializeComponent();
            loadData();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
          
        }

        private void loadData()
        {
            web.Source = new Uri("/Assets/html_howtobmta/howtobmta.html", UriKind.Relative);
        }

        private void web_Navigated(object sender, NavigationEventArgs e)
        {

        }

    }
}