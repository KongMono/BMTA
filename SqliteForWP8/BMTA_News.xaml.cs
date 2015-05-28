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

//using XML_Parsing.Resources;
using System.Windows.Resources;
using System.Xml.Linq;
using SQLite;
using Windows.Storage;
using System.Windows.Input;


namespace BMTA
{
    public partial class BMTA_News : PhoneApplicationPage
    {
        // Constructor
        public static string DB_PATH = Path.Combine(Path.Combine(ApplicationData.Current.LocalFolder.Path, "bmtadatabase.sqlite"));
        private SQLiteConnection dbConn;

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            dbConn = new SQLiteConnection(DB_PATH);
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (dbConn != null)
            {
                /// Close the database connection.
                dbConn.Close();
            }
        }


        public class itemsnews
        {
            public string title { get; set; }
            public string imgtmp { get; set; }
            public string description { get; set; }
            public string image { get; set; }
            public string subject { get; set; }
            public string links { get; set; }
        }
        public static List<itemsnews> items = new List<itemsnews>();

        public BMTA_News()
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
                ContentPanel.Visibility = Visibility;
                loadData();
                string x = BMTA.clGetResolution.Width.ToString();
                string y = BMTA.clGetResolution.Height.ToString();
                string xy = x + "x" + y;
                if (x == "480")
                {
                    ImageBrush brush = new ImageBrush
                    {
                        ImageSource = new System.Windows.Media.Imaging.BitmapImage(new Uri("/Assets/480x852/BMTA_new.png", UriKind.Relative)),
                        Opacity = 1d
                    };
                    this.LayoutRoot.Background = brush;
                    brush.Stretch = Stretch.Fill;
                }
                else if (x == "720")
                {
                    ImageBrush brush = new ImageBrush
                    {
                        ImageSource = new System.Windows.Media.Imaging.BitmapImage(new Uri("/Assets/720x1280/BMTA_new.png", UriKind.Relative)),
                        Opacity = 1d
                    };
                    this.LayoutRoot.Background = brush;
                    brush.Stretch = Stretch.Fill;
                }
                else
                {
                    ImageBrush brush = new ImageBrush
                    {
                        ImageSource = new System.Windows.Media.Imaging.BitmapImage(new Uri("/Assets/768x1280/BMTA_new.png", UriKind.Relative)),
                        Opacity = 1d
                    };
                    this.LayoutRoot.Background = brush;
                    brush.Stretch = Stretch.Fill;
                }
                ContentPanel.Visibility = System.Windows.Visibility.Collapsed;
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
           // WebClient client = new WebClient();
          //  client.OpenReadCompleted += client_OpenReadCompleted;
          //  client.OpenReadAsync(new Uri("http://www.bmta.co.th/?q=th/feed/news", UriKind.Absolute));

            SQLiteCommand sqlComm = new SQLiteCommand(dbConn);
            sqlComm.CommandText = "SELECT * FROM news";
            List<itemsnews> retrievedTasks = sqlComm.ExecuteQuery<itemsnews>();
            foreach (var t in retrievedTasks)
            {
               // lblbustype.Text = t.bus_name;
               // lblbustype1.Text = t.bus_discription;
               // lbltime.Text = t.bus_startstop_time;
                itemsnews c = new itemsnews();
                c.title = t.title;
                c.imgtmp = t.image;
                c.subject = t.description;
                c.links = t.links;
                // c.likes = Convert.ToInt32(item.Element("voting").Value);
                items.Add(c);
            }
            listbox1.ItemsSource = items;
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
                       // c.likes = Convert.ToInt32(item.Element("voting").Value);
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


        private void btnew_Click(object sender, RoutedEventArgs e)
        {
            //  MessageBox.Show();
            Button _button = (Button)sender;
            string subject = _button.Content.ToString();
            string img = _button.CommandParameter.ToString();
           
            // NavigationService.Navigate(new System.Uri(_button.Tag.ToString()));
            NavigationService.Navigate(new Uri("/BMTA_News_details.xaml?key=" + _button.Tag.ToString() + "&subject=" + subject.ToString() + "&img=" + img.ToString(), UriKind.Relative));

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

        private void HyperlinkButton_TextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {

        }

    }
}