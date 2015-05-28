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

namespace BMTA
{
    public partial class BMTA_Abouts : PhoneApplicationPage
    {
        public BMTA_Abouts()
        {
            InitializeComponent();
       
        }
        // Constructor
        public class itemabout
        {
            public string title { get; set; }
        }
        public class itemvision
        {
            public string title { get; set; }
        }
        public class itemcontact
        {
            public string title { get; set; }
        }
        public static List<itemabout> items = new List<itemabout>();
        public static List<itemvision> itemvisions = new List<itemvision>();
        public static List<itemcontact> itemcons = new List<itemcontact>();

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            rightmenu.Visibility = System.Windows.Visibility.Collapsed;
            rightmenux.Visibility = System.Windows.Visibility.Collapsed;
            close.Visibility = System.Windows.Visibility.Collapsed;

            if (!HasNetwork())
            {

                Application.Current.Terminate();
            }
            else if (!HasInternet())
            {
                Application.Current.Terminate();
            }
            else
            {
                loadData();
                string x = BMTA.clGetResolution.Width.ToString();
                string y = BMTA.clGetResolution.Height.ToString();
                string xy = x + "x" + y;
                if (x == "480")
                {
                    ImageBrush brush = new ImageBrush
                    {
                        ImageSource = new System.Windows.Media.Imaging.BitmapImage(new Uri("/Assets/480x852/BMTA_abouts.png", UriKind.Relative)),
                        Opacity = 1d
                    };
                    this.LayoutRoot.Background = brush;
                    brush.Stretch = Stretch.Fill;
                }
                else if (x == "720")
                {
                    ImageBrush brush = new ImageBrush
                    {
                        ImageSource = new System.Windows.Media.Imaging.BitmapImage(new Uri("/Assets/720x1280/BMTA_abouts.png", UriKind.Relative)),
                        Opacity = 1d
                    };
                    this.LayoutRoot.Background = brush;
                    brush.Stretch = Stretch.Fill;
                }
                else
                {
                    ImageBrush brush = new ImageBrush
                    {
                        ImageSource = new System.Windows.Media.Imaging.BitmapImage(new Uri("/Assets/768x1280/BMTA_abouts.png", UriKind.Relative)),
                        Opacity = 1d
                    };
                    this.LayoutRoot.Background = brush;
                    brush.Stretch = Stretch.Fill;
                }
            }
        }

        private void loadData()
        {
            WebClient client = new WebClient();
            client.OpenReadCompleted += client_OpenReadCompleted;
            client.OpenReadAsync(new Uri("http://www.bmta.co.th/?q=th/feed/about-us", UriKind.Absolute));

            WebClient client1 = new WebClient();
            client1.OpenReadCompleted += client_OpenReadCompleted1;
            client1.OpenReadAsync(new Uri("http://www.bmta.co.th/?q=th/feed/vision", UriKind.Absolute));

            WebClient client2 = new WebClient();
            client2.OpenReadCompleted += client_OpenReadCompleted2;
            client2.OpenReadAsync(new Uri("http://www.bmta.co.th/?q=th/feed/contact", UriKind.Absolute));

        }

        void client_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Error != null)
                return;
            Stream str = e.Result;
            try
            {
                String eve = "item";
                XDocument loadedData = XDocument.Load(str);
                foreach (var item in loadedData.Descendants(eve))
                {

                    try
                    {
                        itemabout c = new itemabout();
                        c.title = item.Element("title").Value;
                        items.Add(c);
                    }
                    catch (Exception ex)
                    {
                        //GoogleAnalytics.EasyTracker.GetTracker().SendException(ex.Message, false);
                    }
                }
                listbox1.ItemsSource = items;
            }
            catch (System.Xml.XmlException ex)
            {
                MessageBox.Show("limited connectivity or invalid data.\nplease try again");
            }
        }

        void client_OpenReadCompleted1(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Error != null)
                return;
            Stream str = e.Result;
            try
            {
                String eve = "item";
                XDocument loadedData = XDocument.Load(str);
                foreach (var item in loadedData.Descendants(eve))
                {

                    try
                    {
                        itemvision c = new itemvision();
                        c.title = item.Element("title").Value;
                        itemvisions.Add(c);
                    }
                    catch (Exception ex)
                    {
                        //GoogleAnalytics.EasyTracker.GetTracker().SendException(ex.Message, false);
                    }
                }
                listbox2.ItemsSource = itemvisions;
            }
            catch (System.Xml.XmlException ex)
            {
                MessageBox.Show("limited connectivity or invalid data.\nplease try again");
            }
        }

        void client_OpenReadCompleted2(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Error != null)
                return;
            Stream str = e.Result;
            try
            {
                String eve = "item";
                XDocument loadedData = XDocument.Load(str);
                foreach (var item in loadedData.Descendants(eve))
                {

                    try
                    {
                        itemcontact c = new itemcontact();
                        c.title = item.Element("title").Value;
                        itemcons.Add(c);
                    }
                    catch (Exception ex)
                    {
                        //GoogleAnalytics.EasyTracker.GetTracker().SendException(ex.Message, false);
                    }
                }
                listbox3.ItemsSource = itemcons;
            }
            catch (System.Xml.XmlException ex)
            {
                MessageBox.Show("limited connectivity or invalid data.\nplease try again");
            }
        }

        private void btnew_Click(object sender, RoutedEventArgs e)
        {
            //  MessageBox.Show();
            Button _button = (Button)sender;
            // NavigationService.Navigate(new System.Uri(_button.Tag.ToString()));
            NavigationService.Navigate(new Uri("/BMTA_Abouts_details.xaml?key=" + _button.Tag.ToString(), UriKind.Relative));

        }

        private void btvision_Click(object sender, RoutedEventArgs e)
        {
            //  MessageBox.Show();
            Button _button = (Button)sender;
            // NavigationService.Navigate(new System.Uri(_button.Tag.ToString()));
            NavigationService.Navigate(new Uri("/BMTA_Vision_details.xaml?key=" + _button.Tag.ToString(), UriKind.Relative));

        }

        private bool HasInternet()
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                MessageBox.Show("No internet connection is available. Try again later.");
                return false;
            }
            return true;
        }

        private bool HasNetwork()
        {
            if (!DeviceNetworkInformation.IsNetworkAvailable)
            {
                MessageBox.Show("No network is available. Try again later.");
                return false;
            }
            return true;
        }

        private void btback_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_bus_line.xaml", UriKind.Relative));
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_BusStop.xaml", UriKind.Relative));
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_BusCoordinates.xaml", UriKind.Relative));
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_BusStartStop.xaml", UriKind.Relative));
        }

        private void close_Click(object sender, RoutedEventArgs e)
        {
            rightmenu.Visibility = System.Windows.Visibility.Collapsed;
            rightmenux.Visibility = System.Windows.Visibility.Collapsed;
            close.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void btTopMenu_Click(object sender, RoutedEventArgs e)
        {
            rightmenux.Visibility = Visibility;
            rightmenu.Visibility = Visibility;
            close.Visibility = Visibility;
        }

        private void rhome_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_AppTh.xaml", UriKind.Relative));
        }

        private void rbusline_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_bus_line.xaml", UriKind.Relative));
        }

        private void rbusstop_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_BusStop.xaml", UriKind.Relative));
        }

        private void rcoor_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_BusCoordinates.xaml", UriKind.Relative));
        }

        private void rbusstartstop_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_BusStartStop.xaml", UriKind.Relative));
        }

        private void rbusspeed_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_Speed_history.xaml", UriKind.Relative));
        }

        private void rbusnew_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_EventNew.xaml", UriKind.Relative));
        }

    }
}