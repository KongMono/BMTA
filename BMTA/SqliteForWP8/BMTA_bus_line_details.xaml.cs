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
    public partial class BMTA_bus_line_details : PhoneApplicationPage
    {
        private SQLiteConnection dbConn;
        string key = "";
        private MapPolyline line;


        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (NavigationContext.QueryString.TryGetValue("key", out key)) { }
            lblbusid.Text = key;
            /// Create the database connection.
            dbConn = new SQLiteConnection(App.DB_PATH);
            SQLiteCommand sqlComm = new SQLiteCommand(dbConn);
            sqlComm.CommandText = "SELECT * FROM busline Where bus_line=" + key + " LIMIT 1";

            List<busline> retrievedTasks = sqlComm.ExecuteQuery<busline>();
            foreach (var t in retrievedTasks)
            {
                lblStart.Text = t.bus_start;
                lblStop.Text = t.bus_stop;
                lblbustype.Text = t.bus_type;
                lbltime.Text = t.bus_startstop_time;
            }

            busstop(key,1);
       
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (dbConn != null)
            {
                /// Close the database connection.
                dbConn.Close();
            }
        }

        public BMTA_bus_line_details()
        {
            InitializeComponent();

       

        }

        protected async void LoadMap(string bus_line)
        {
            // this.LoadMapCurrent();
            //*** Map
            Map MyMap = new Map();
            MyMap.ZoomLevel = 18;


            MapLayer layer = new MapLayer();

            SQLiteCommand sqlComm = new SQLiteCommand(dbConn);
            sqlComm.CommandText = "SELECT * FROM busline Where bus_line=" + bus_line + " LIMIT 1";

            List<busline> retrievedTasks = sqlComm.ExecuteQuery<busline>();
            foreach (var t in retrievedTasks)
            {
                string strJSON = null;
                strJSON = t.bus_stoplist;
                MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(strJSON));
                ObservableCollection<Member> list = new ObservableCollection<Member>();
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(ObservableCollection<Member>));
                list = (ObservableCollection<Member>)serializer.ReadObject(ms);

                List<Member> myMember = new List<Member>();

                foreach (Member cm in list)
                {
                    Pushpin pushpin = new Pushpin();
                    pushpin.GeoCoordinate = new GeoCoordinate(cm.latitude, cm.longitude);
                    var uriString = @"Assets/btn_bus.png";
                    pushpin.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(uriString, UriKind.Relative)) };
                    pushpin.Width = 40;
                    pushpin.Height = 31;
                    MapOverlay overlay = new MapOverlay();
                    overlay.Content = pushpin;
                    overlay.GeoCoordinate = new GeoCoordinate(cm.latitude, cm.longitude);
                    layer.Add(overlay);

                }
                    line = new MapPolyline();
                    line.StrokeColor = Colors.Red;
                    line.StrokeThickness = 5;
                foreach (Member pl in list)
                {
                    line.Path.Add(new GeoCoordinate(pl.latitude, pl.longitude));
                }
                MyMap.MapElements.Add(line);
                // Map Layer
                MyMap.Layers.Add(layer);


                // Get my current location.
                Geolocator myGeolocator = new Geolocator();
                Geoposition myGeoposition = await myGeolocator.GetGeopositionAsync();
                Geocoordinate myGeocoordinate = myGeoposition.Coordinate;
                GeoCoordinate myGeoCoordinate = CoordinateConverter.ConvertGeocoordinate(myGeocoordinate);
                MyMap.Center = myGeoCoordinate;

                // Create a small circle to mark the current location.
                Ellipse myCircle = new Ellipse();
                myCircle.Fill = new SolidColorBrush(Colors.Blue);
                myCircle.Height = 20;
                myCircle.Width = 20;
                myCircle.Opacity = 50;

                // Create a MapOverlay to contain the circle.
                MapOverlay myLocationOverlay = new MapOverlay();
                myLocationOverlay.Content = myCircle;
                myLocationOverlay.PositionOrigin = new Point(0.5, 0.5);
                myLocationOverlay.GeoCoordinate = myGeoCoordinate;

                // Create a MapLayer to contain the MapOverlay.
                MapLayer myLocationLayer = new MapLayer();
                myLocationLayer.Add(myLocationOverlay);

                // Add the MapLayer to the Map.
                MyMap.Layers.Add(myLocationLayer);

                // CarphicMode
                MyMap.Center = myGeoCoordinate;
                MyMap.CartographicMode = MapCartographicMode.Road;

                // Add map to display
                ContentPanel.Children.Add(MyMap);
            }

        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
           // BuildLocalizedApplicationbar();
            rightmenu.Visibility = System.Windows.Visibility.Collapsed;
            rightmenux.Visibility = System.Windows.Visibility.Collapsed;
            close.Visibility = System.Windows.Visibility.Collapsed;
            if (!HasNetwork())
            {
                Application.Current.Terminate();
                // new Microsoft.Xna.Framework.Game().Exit();
            }
            else if (!HasInternet())
            {
                Application.Current.Terminate();
                // new Microsoft.Xna.Framework.Game().Exit();
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
                        ImageSource = new System.Windows.Media.Imaging.BitmapImage(new Uri("/Assets/480x852/BMTA_busline_bg.png", UriKind.Relative)),
                        Opacity = 1d
                    };
                    this.LayoutRoot.Background = brush;
                    brush.Stretch = Stretch.Fill;
                }
                else if (x == "720")
                {
                    ImageBrush brush = new ImageBrush
                    {
                        ImageSource = new System.Windows.Media.Imaging.BitmapImage(new Uri("/Assets/720x1280/BMTA_buslinedetails_bg.png", UriKind.Relative)),
                        Opacity = 1d
                    };
                    this.LayoutRoot.Background = brush;
                    brush.Stretch = Stretch.Fill;
                }
                else
                {
                    ImageBrush brush = new ImageBrush
                    {
                        ImageSource = new System.Windows.Media.Imaging.BitmapImage(new Uri("/Assets/768x1280/BMTA_busline_bg.png", UriKind.Relative)),
                        Opacity = 1d
                    };
                    this.LayoutRoot.Background = brush;
                    brush.Stretch = Stretch.Fill;
                }

            }

        }

        private void BuildLocalizedApplicationbar()
        {
            ApplicationBar = new ApplicationBar();

            ApplicationBar.Mode = ApplicationBarMode.Default;
            ApplicationBar.Opacity = 0.78;
            ApplicationBar.IsVisible = true;
            ApplicationBar.IsMenuEnabled = true;
            // ApplicationBar.BackgroundColor = Color.FromArgb(100, 0, 165, 78);
            ApplicationBar.BackgroundColor = Colors.Green;

            ApplicationBarIconButton button1 = new ApplicationBarIconButton();
            button1.IconUri = new Uri("/Assets/bt_main_th/btf_bus.png", UriKind.Relative);
            button1.Text = "สายรถเมล์";
            ApplicationBar.Buttons.Add(button1);
            button1.Click += new EventHandler(button1_Click);

            ApplicationBarIconButton button2 = new ApplicationBarIconButton();
            button2.IconUri = new Uri("/Assets/bt_main_th/btf_busstop.png", UriKind.Relative);
            button2.Text = "ป้ายรถเมล์";
            ApplicationBar.Buttons.Add(button2);
            button2.Click += new EventHandler(button2_Click);

            ApplicationBarIconButton button3 = new ApplicationBarIconButton();
            button3.IconUri = new Uri("/Assets/bt_main_th/btf_place.png", UriKind.Relative);
            button3.Text = "สถานที่สำคัญ";
            ApplicationBar.Buttons.Add(button3);
            button3.Click += new EventHandler(button3_Click);

            ApplicationBarIconButton button4 = new ApplicationBarIconButton();
            button4.IconUri = new Uri("/Assets/bt_main_th/btf_startstop.png", UriKind.Relative);
            button4.Text = "ต้นทางปลายทาง";
            ApplicationBar.Buttons.Add(button4);
            button4.Click += new EventHandler(button4_Click);


        }
        private void button1_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_bus_line.xaml", UriKind.Relative));
        }
        private void button2_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_BusStop.xaml", UriKind.Relative));
        }
        private void button3_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_BusCoordinates.xaml", UriKind.Relative));
        }
        private void button4_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_BusStartStop.xaml", UriKind.Relative));
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

        public sealed class busline
        {
            /// <summary>
            /// You can create an integer primary key and let the SQLite control it.
            /// </summary>
            [PrimaryKey, AutoIncrement]
            public int id { get; set; }

            public string bus_id { get; set; }

            public string bus_line { get; set; }
            public string bus_start { get; set; }
            public string bus_stop { get; set; }
            public string bus_type { get; set; }
            public string bus_startstop_time { get; set; }
            public string bus_stoplist { get; set; }
            public string stop_name { get; set; }

            public override string ToString()
            {
                return bus_id;
            }
        }

        public void busstop(string bus_line, int li)
        {
           
            SQLiteCommand sqlComm = new SQLiteCommand(dbConn);
            sqlComm.CommandText = "SELECT * FROM busline Where bus_line=" + bus_line + " LIMIT "+li;

            List<busline> retrievedTasks = sqlComm.ExecuteQuery<busline>();
            foreach (var t in retrievedTasks)
            {

                string strJSON = null;
                strJSON = t.bus_stoplist;

                MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(strJSON));
                ObservableCollection<Member> list = new ObservableCollection<Member>();
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(ObservableCollection<Member>));
                list = (ObservableCollection<Member>)serializer.ReadObject(ms);

                List<Member> myMember = new List<Member>();
                TaskListBox.Items.Clear();
                foreach (Member cm in list)
                {
                    string sName = cm.stop_name.ToString();
                    TaskListBox.Items.Add(sName);
                }
            }
        }

        [DataContract]
        public class Member
        {

            [DataMember]
            public string stop_name { get; set; }
            [DataMember]
            public double longitude { get; set; }

            [DataMember]
            public double latitude { get; set; }


            public Member(string strName)
            {
                this.stop_name = strName;
            }


        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ContentPanel.Visibility = Visibility;
            this.LoadMap(lblbusid.Text);
            btmap.Visibility = System.Windows.Visibility.Collapsed;
            btmapback.Visibility = Visibility;
  
        }

        private void btback_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var uriStringin = @"Assets/btn_in_atvth.png";
            btin.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(uriStringin, UriKind.Relative)) };

            var uriStringout = @"Assets/btn_out_th.png";
            btout.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(uriStringout, UriKind.Relative)) };

            dbConn = new SQLiteConnection(App.DB_PATH);


            SQLiteCommand sqlComm = new SQLiteCommand(dbConn);
            sqlComm.CommandText = "SELECT * FROM busline Where bus_line=" + lblbusid.Text + " LIMIT 1";

            List<busline> retrievedTasks = sqlComm.ExecuteQuery<busline>();
            foreach (var t in retrievedTasks)
            {
                lblStart.Text = t.bus_start;
                lblStop.Text = t.bus_stop;
                lblbustype.Text = t.bus_type;
                lbltime.Text = t.bus_startstop_time;
            }

            busstop(key, 1);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            var uriStringin = @"Assets/btn_in_th.png";
            btin.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(uriStringin, UriKind.Relative)) };

            var uriStringout = @"Assets/btn_out_atvth.png";
            btout.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(uriStringout, UriKind.Relative)) };

            dbConn = new SQLiteConnection(App.DB_PATH);


            SQLiteCommand sqlComm = new SQLiteCommand(dbConn);
            sqlComm.CommandText = "SELECT * FROM busline Where bus_line=" + lblbusid.Text + " LIMIT 2";

            List<busline> retrievedTasks = sqlComm.ExecuteQuery<busline>();
            foreach (var t in retrievedTasks)
            {
                lblStart.Text = t.bus_start;
                lblStop.Text = t.bus_stop;
                lblbustype.Text = t.bus_type;
                lbltime.Text = t.bus_startstop_time;
            }

            busstop(key, 2);
        }

      

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_bus_line.xaml", UriKind.Relative));
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_BusStop.xaml", UriKind.Relative));
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_BusCoordinates.xaml", UriKind.Relative));
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_BusStartStop.xaml", UriKind.Relative));
        }

        private void btmapback_Click(object sender, RoutedEventArgs e)
        {
            ContentPanel.Visibility = System.Windows.Visibility.Collapsed;
            btmap.Visibility = Visibility;
            btmapback.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
          //  if (NavigationContext.QueryString.TryGetValue("key", out key)) { }
            
            dbConn = new SQLiteConnection(App.DB_PATH);
            SQLiteCommand sqlComm = new SQLiteCommand(dbConn);
            sqlComm.CommandText = "SELECT * FROM busline where bus_type !=''";// Group by bus_line";
            List<busline> retrievedTasksall = sqlComm.ExecuteQuery<busline>();
            List<string> num = new List<string>();

            foreach (var t in retrievedTasksall)
            {
                int a = Convert.ToInt32(t.bus_line);
                int b = Convert.ToInt32(lblbusid.Text);
                if(a > b)
                {
                   // num.Add(t.bus_line);
                    lblStart.Text = t.bus_start;
                    lblStop.Text = t.bus_stop;
                    lblbustype.Text = t.bus_type;
                    lbltime.Text = t.bus_startstop_time;
                    lblbusid.Text = t.bus_line;
                    busstop(lblbusid.Text, 1);
                    return;
                }

            }

        }

        private void Button_Click_8(object sender, RoutedEventArgs e)
        {
           // NavigationService.GoBack(1);
            //dbConn = new SQLiteConnection(App.DB_PATH);
            //SQLiteCommand sqlComm = new SQLiteCommand(dbConn);
            //sqlComm.CommandText = "SELECT * FROM busline where bus_type !='' ORDER BY bus_line DESC ";// Group by bus_line";
            //List<busline> retrievedTasksall = sqlComm.ExecuteQuery<busline>();
            //List<string> num = new List<string>();

            //foreach (var t in retrievedTasksall)
            //{
            //    int a = Convert.ToInt32(t.bus_line);
            //    int b = Convert.ToInt32(lblbusid.Text);
            //    if (a < b)
            //    {
            //        // num.Add(t.bus_line);
            //        lblStart.Text = t.bus_start;
            //        lblStop.Text = t.bus_stop;
            //        lblbustype.Text = t.bus_type;
            //        lbltime.Text = t.bus_startstop_time;
            //        lblbusid.Text = t.bus_line;
            //        busstop(lblbusid.Text, 1);
            //        return;
            //    }
            //}
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