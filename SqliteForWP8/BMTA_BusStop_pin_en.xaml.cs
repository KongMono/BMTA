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
using System.Windows.Media.Imaging;
using BMTA.Resources;
using System.Collections.ObjectModel;
using SQLite;
using Windows.Storage;
using System.Windows.Input;

namespace BMTA
{
    public partial class BMTA_BusStop_pin_en : PhoneApplicationPage
    {
        /// <summary>
        /// The database path.
        /// </summary>
        public static string DB_PATH = Path.Combine(Path.Combine(ApplicationData.Current.LocalFolder.Path, "bmtadatabase.sqlite"));

        /// <summary>
        /// The sqlite connection.
        /// </summary>
        private SQLiteConnection dbConn;

        string _busline = "";
        string stop_name = "";
        List<Article> articles = new List<Article>();

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            dbConn = new SQLiteConnection(DB_PATH);
            base.OnNavigatedTo(e);
            if (NavigationContext.QueryString.TryGetValue("_busline", out _busline)) { }
            if (NavigationContext.QueryString.TryGetValue("stop_name", out stop_name)) { }
            // lblbusid.Text = key;
            /// Create the database connection.
            //   dbConn = new SQLiteConnection(App.DB_PATH);
            //   SQLiteCommand sqlComm = new SQLiteCommand(dbConn);
            //  sqlComm.CommandText = "SELECT * FROM busline Where bus_line=" + _busline + " LIMIT 1";

            //   List<busline> retrievedTasks = sqlComm.ExecuteQuery<busline>();

            string[] split = _busline.Split(new Char[] { ',', ' ' });



            articles.Clear();
            foreach (string s in split)
            {
                Article article = new Article() { btcontent = s };
                articles.Add(article);
            }
            busno_bg.Content = split[0];
            lblstop_name.Text = stop_name;
            // listbox1.DataContext = _busline;
            lblbusline.Text = _busline;
            busstop(split[0], 1);

        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (dbConn != null)
            {
                /// Close the database connection.
                dbConn.Close();
            }
        }

        public BMTA_BusStop_pin_en()
        {
            InitializeComponent();
        }

        public void busstop(string bus_line, int li)
        {

            SQLiteCommand sqlComm = new SQLiteCommand(dbConn);
            sqlComm.CommandText = "SELECT * FROM busline Where bus_line='" + bus_line.ToString() + "' LIMIT " + li;
            List<busline> retrievedTasks = sqlComm.ExecuteQuery<busline>();
            foreach (var t in retrievedTasks)
            {
                lblbustype.Text = t.bus_name;
                lblbustype1.Text = t.bus_discription;
                lbltime.Text = t.bus_startstop_time;
            }
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            rightmenu.Visibility = System.Windows.Visibility.Collapsed;
            rightmenux.Visibility = System.Windows.Visibility.Collapsed;
            close.Visibility = System.Windows.Visibility.Collapsed;
            //  BuildLocalizedApplicationbar();
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

                string x = BMTA.clGetResolution.Width.ToString();
                string y = BMTA.clGetResolution.Height.ToString();
                string xy = x + "x" + y;
                if (x == "480")
                {
                    ImageBrush brush = new ImageBrush
                    {
                        ImageSource = new System.Windows.Media.Imaging.BitmapImage(new Uri("/Assets/480x852/BMTA_busstop_line_en.png", UriKind.Relative)),
                        Opacity = 1d
                    };
                    this.LayoutRoot.Background = brush;
                    brush.Stretch = Stretch.Fill;
                }
                else if (x == "720")
                {
                    ImageBrush brush = new ImageBrush
                    {
                        ImageSource = new System.Windows.Media.Imaging.BitmapImage(new Uri("/Assets/720x1280/BMTA_busstop_line_en.png", UriKind.Relative)),
                        Opacity = 1d
                    };
                    this.LayoutRoot.Background = brush;
                    brush.Stretch = Stretch.Fill;
                }
                else
                {
                    ImageBrush brush = new ImageBrush
                    {
                        ImageSource = new System.Windows.Media.Imaging.BitmapImage(new Uri("/Assets/768x1280/BMTA_busstop_line_en.png", UriKind.Relative)),
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

        private void Box_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Enter))
            {
                //Do something
                // MessageBox.Show(busline_search.Text);
                //  NavigationService.Navigate(new Uri("/BMTA_bus_line.xaml?key=" + busline_search.Text, UriKind.Relative));
            }
        }

        private void btback_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_BusStop_en.xaml", UriKind.Relative));
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_bus_line_en.xaml", UriKind.Relative));
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_BusCoordinates_en.xaml", UriKind.Relative));
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
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


        public sealed class busline
        {
            /// <summary>
            /// You can create an integer primary key and let the SQLite control it.
            /// </summary>
            [PrimaryKey, AutoIncrement]
            public int id { get; set; }

            public string bus_id { get; set; }
            public string bus_name { get; set; }
            public string bus_line { get; set; }
            public string bus_start { get; set; }
            public string bus_stop { get; set; }
            public string bus_type { get; set; }
            public string bus_startstop_time { get; set; }
            public string bus_stoplist { get; set; }
            public string stop_name { get; set; }
            public string bus_discription { get; set; }

            public override string ToString()
            {
                return bus_id;
            }
        }

        public class Article
        {
            public string Start { get; set; }

            public string Stop { get; set; }

            public string ImagePath { get; set; }

            public string bg { get; set; }

            public string btcontent { get; set; }
        }


        public class Member
        {

            public string bus_type { get; set; }
            public string bus_discription { get; set; }
            public string bus_startstop_time { get; set; }

            public Member(string strName)
            {
                this.bus_type = strName;
            }


        }

        private void btrigth_Click(object sender, RoutedEventArgs e)
        {
            if (articles != null)
            {
                // SelectNextCity();
                var next = SelectNextBusline();
                busno_bg.Content = next.btcontent;
                busstop(next.btcontent, 1);
            }
        }

        private int currentPosition = 0;
        public Article SelectNextBusline()
        {
            currentPosition++;
            if (currentPosition == articles.Count)
            {
                currentPosition = 0;
                return articles[0];
            }
            else
            {
                return articles[currentPosition];
            }
            // in this context `list` would be a class-level field.
        }
        public Article SelectPreviousBusline()
        {
            currentPosition--;
            if (currentPosition < 0)
            {
                // int carticle = articles.Count;
                currentPosition = articles.Count - 1;
                return articles[currentPosition];
            }
            else if (currentPosition == 0)
            {
                //  currentPosition = articles.Count;
                return articles[currentPosition];
            }
            else
            {
                // currentPosition = 0;
                return articles[currentPosition];
            }
            // in this context `list` would be a class-level field.
        }

        private void btleft_Click(object sender, RoutedEventArgs e)
        {
            if (articles != null)
            {
                // SelectNextCity();
                var next = SelectPreviousBusline();
                busno_bg.Content = next.btcontent;
                busstop(next.btcontent, 1);
            }
        }
    }
}