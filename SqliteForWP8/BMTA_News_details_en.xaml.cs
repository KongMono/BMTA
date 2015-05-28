using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Net.NetworkInformation;
using System.IO;
using System.Windows.Media;
using BMTA.Resources;
using System.Collections.ObjectModel;
using SQLite;
using Windows.Storage;
using System.Windows.Input;
using System.Xml;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Device.Location;
using Windows.Devices.Geolocation;
using System.Windows.Shapes;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Maps.Toolkit;
using System.Windows.Media.Imaging;
using System.Windows.Controls.Primitives;
using System.ComponentModel;

namespace BMTA
{
    public partial class BMTA_News_details_en : PhoneApplicationPage
    {
        string key = "";
        string subject = "";
        string img = "";
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (NavigationContext.QueryString.TryGetValue("key", out key)) { }
            if (NavigationContext.QueryString.TryGetValue("subject", out subject)) { }
            if (NavigationContext.QueryString.TryGetValue("img", out img)) { }

            Uri uri = new Uri(img, UriKind.Absolute);

            // simg.Source = 
            // ImageBrush licoriceImage = new System.Windows.Media.Imaging.BitmapImage(new Uri(img, UriKind.Relative));
            simg.Source = new BitmapImage(uri); //new System.Windows.Media.Imaging.BitmapImage(new Uri("http://www.bmta.co.th/sites/default/files/images/news/img_7686.jpg", UriKind.Relative));
            //  bnt.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(img, UriKind.Relative)) };
            // imgx.Source = uriString.ToString();// new ImageBrush { ImageSource = new BitmapImage(new Uri(uriString, UriKind.Relative)) };
            titlex.Text = key;
            subjectx.Text = subject;
        }
        // Constructor
        public class itemsnews
        {
            public string title { get; set; }
            public string imgtmp { get; set; }
            public string subject { get; set; }
            public string links { get; set; }
        }
        public static List<itemsnews> items = new List<itemsnews>();

        public BMTA_News_details_en()
        {
            InitializeComponent();
        }

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
                        ImageSource = new System.Windows.Media.Imaging.BitmapImage(new Uri("/Assets/480x852/BMTA_new_en.png", UriKind.Relative)),
                        Opacity = 1d
                    };
                    this.LayoutRoot.Background = brush;
                    brush.Stretch = Stretch.Fill;
                }
                else if (x == "720")
                {
                    ImageBrush brush = new ImageBrush
                    {
                        ImageSource = new System.Windows.Media.Imaging.BitmapImage(new Uri("/Assets/720x1280/BMTA_new_en.png", UriKind.Relative)),
                        Opacity = 1d
                    };
                    this.LayoutRoot.Background = brush;
                    brush.Stretch = Stretch.Fill;
                }
                else
                {
                    ImageBrush brush = new ImageBrush
                    {
                        ImageSource = new System.Windows.Media.Imaging.BitmapImage(new Uri("/Assets/768x1280/BMTA_new_en.png", UriKind.Relative)),
                        Opacity = 1d
                    };
                    this.LayoutRoot.Background = brush;
                    brush.Stretch = Stretch.Fill;
                }
            }
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

        private void loadData()
        {
            WebClient client = new WebClient();
            client.OpenReadCompleted += client_OpenReadCompleted;
            client.OpenReadAsync(new Uri("http://www.bmta.co.th/?q=th/feed/news", UriKind.Absolute));

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
                        itemsnews c = new itemsnews();
                        c.title = item.Element("title").Value;
                        c.imgtmp = item.Element("enclosure").Value;
                        c.subject = item.Element("description").Value;
                        c.links = item.Element("link").Value;
                        items.Add(c);
                    }
                    catch (Exception ex)
                    {
                        //GoogleAnalytics.EasyTracker.GetTracker().SendException(ex.Message, false);
                    }
                }
               // listbox1.ItemsSource = items;
            }
            catch (System.Xml.XmlException ex)
            {
                MessageBox.Show("limited connectivity or invalid data.\nplease try again");
            }
        }

        private void btback_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_bus_line_en.xaml", UriKind.Relative));
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_BusStop_en.xaml", UriKind.Relative));
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_BusCoordinates_en.xaml", UriKind.Relative));
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_BusStartStop_en.xaml", UriKind.Relative));
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
            NavigationService.Navigate(new Uri("/BMTA_AppTh_en.xaml", UriKind.Relative));
        }

        private void rbusline_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_bus_line_en.xaml", UriKind.Relative));
        }

        private void rbusstop_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_BusStop_en.xaml", UriKind.Relative));
        }

        private void rcoor_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_BusCoordinates_en.xaml", UriKind.Relative));
        }

        private void rbusstartstop_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_BusStartStop_en.xaml", UriKind.Relative));
        }

        private void rbusspeed_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_Speed_history_en.xaml", UriKind.Relative));
        }

        private void rbusnew_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_EventNew_en.xaml", UriKind.Relative));
        }
    }
    }