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
using BMTA.Item;


namespace BMTA
{
    public partial class BMTA_bus_line_details : PhoneApplicationPage
    {
        public String lang = (Application.Current as App).Language;
        private SQLiteConnection dbConn;
        public busline ls = new busline();
        public List<busline> retrievedTasks = new List<busline>();
        public string group = "";
        public int index;
        private MapPolyline line;
        ObservableCollection<listBuslineDetailItem> listBuslineDetailItem = new ObservableCollection<listBuslineDetailItem>();

        ProgressIndicator progressIndicator = new ProgressIndicator();

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            // Create the database connection.  
            dbConn = new SQLiteConnection(App.DB_PATH);
            // Create the table Task, if it doesn't exist.  
            dbConn.CreateTable<busline>();

            group = this.NavigationContext.QueryString["Group"];
            String indexnum = this.NavigationContext.QueryString["Index"];
            index = Convert.ToInt32(indexnum);

            if (group != null)
            {
                ShowProgressIndicator("Loading..");

                retrievedTasks = dbConn.Query<busline>("SELECT * FROM busline WHERE bus_line LIKE '" + group + "%' AND (bus_direction LIKE '%เข้าเมือง%' OR bus_direction LIKE '%วนซ้าย%')");
                ls = retrievedTasks[index];
                lblbusid.Text = ls.bus_line;
                lblStart.Text = ls.bus_start;
                lblStop.Text = ls.bus_end;
                lblbusName.Text = ls.bus_name;
                lbltime.Text = ls.bus_startstop_time;
                getListDatabusstop(lblbusid.Text);
            }

            this.LoadMap(ls.bus_line);

            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (dbConn != null)
            {
                dbConn.Close();
            }
        }

        public BMTA_bus_line_details()
        {
            InitializeComponent();
        }

        protected async void LoadMap(string bus_line)
        {
            // Map
            map.ZoomLevel = 16;

            MapLayer layer = new MapLayer();

            List<busline> retrievedTasks = dbConn.Query<busline>("SELECT bus_stop,bus_polyline FROM busline Where bus_line=" + bus_line + " LIMIT 1");
            foreach (var t in retrievedTasks)
            {
                string busStopJson = t.bus_stop;
                string buspolylineJson = t.bus_polyline;
                List<listBuslineDetailItem> results = JsonConvert.DeserializeObject<List<listBuslineDetailItem>>(busStopJson);
                JArray lspoly = JArray.Parse(buspolylineJson);
                line = new MapPolyline();
                line.StrokeColor = Colors.Red;
                line.StrokeThickness = 3;

                foreach (listBuslineDetailItem cm in results)
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
                string[][] Users = lspoly.ToObject<string[][]>();
                foreach (String[] cm in Users)
                {
                    line.Path.Add(new GeoCoordinate(Convert.ToDouble(cm[0]), Convert.ToDouble(cm[1])));
                }

                map.MapElements.Add(line);
                // Map Layer
                map.Layers.Add(layer);


                // Get my current location.
                Geolocator myGeolocator = new Geolocator();
                Geoposition myGeoposition = await myGeolocator.GetGeopositionAsync();
                Geocoordinate myGeocoordinate = myGeoposition.Coordinate;
                GeoCoordinate myGeoCoordinate = CoordinateConverter.ConvertGeocoordinate(myGeocoordinate);
                map.Center = myGeoCoordinate;

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
                map.Layers.Add(myLocationLayer);

                // CarphicMode
                map.Center = myGeoCoordinate;
                map.CartographicMode = MapCartographicMode.Road;
            }

        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (lang.Equals("th"))
            {
                titleName.Text = "เส้นทางเดินรถ";
                FixStart.Text = "ต้นทาง";
                FixStop.Text = "ปลายทาง";
            }
            else
            {
                titleName.Text = "Route";
                FixStart.Text = "Start";
                FixStop.Text = "End";
            }
        }

        public void getListDatabusstop(string queryBustop)
        {
            List<listBuslineDetailItem> results = JsonConvert.DeserializeObject<List<listBuslineDetailItem>>(queryBustop);
            List<listBuslineDetailItem> ls = new List<listBuslineDetailItem>();

            int b = 0;
            if (results != null)
            {
                foreach (var item in results)
                {
                    listBuslineDetailItem lsitem = new listBuslineDetailItem();
                    lsitem.stop_name = item.stop_name;

                    if (b < 1)
                    {
                        lsitem.bg = "#D1E1F3";
                        b++;
                    }
                    else
                    {
                        lsitem.bg = "#E8F0F9";
                        b--;
                    }

                    ls.Add(lsitem);
                }
            }
            else
            {
                MessageBox.Show("No data");
            }
            TaskListBox.ItemsSource = ls;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //ContentPanel.Visibility = Visibility;
            //this.LoadMap(lblbusid.Text);
            //btmap.Visibility = System.Windows.Visibility.Collapsed;
            //btmapback.Visibility = Visibility;

        }

        private void btback_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void btin_Click(object sender, RoutedEventArgs e)
        {
            var uriStringin = @"Assets/btn_in_atvth.png";
            btin.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(uriStringin, UriKind.Relative)) };

            var uriStringout = @"Assets/btn_out_th.png";
            btout.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(uriStringout, UriKind.Relative)) };

            List<busline> retrievedTasks = dbConn.Query<busline>("select * from busline where bus_line = '" + ls.bus_line + "' and bustype = '" + ls.bustype + "' and bus_owner = '" + ls.bus_owner + "' and (bus_direction like '%เข้าเมือง%' or bus_direction like '%วนขวา%' )");
            foreach (var t in retrievedTasks)
            {
                lblStart.Text = t.bus_start;
                lblStop.Text = t.bus_end;
                lblbusName.Text = t.bus_name;
                lbltime.Text = t.bus_startstop_time;
            }

            getListDatabusstop(ls.bus_stop);
        }

        private void btout_Click(object sender, RoutedEventArgs e)
        {
            var uriStringin = @"Assets/btn_in_th.png";
            btin.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(uriStringin, UriKind.Relative)) };

            var uriStringout = @"Assets/btn_out_atvth.png";
            btout.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(uriStringout, UriKind.Relative)) };

            List<busline> retrievedTasks = dbConn.Query<busline>("SELECT * FROM busline Where bus_line=" + ls.bus_line + " LIMIT 2");

            foreach (var t in retrievedTasks)
            {
                lblStart.Text = t.bus_start;
                lblStop.Text = t.bus_end;
                lblbusName.Text = t.bus_name;
                lbltime.Text = t.bus_startstop_time;
            }

            getListDatabusstop(ls.bus_line);
        }

        private void btmapback_Click(object sender, RoutedEventArgs e)
        {
            if (map.Visibility == System.Windows.Visibility.Visible)
            {
                map.Visibility = System.Windows.Visibility.Collapsed;
                TaskListBox.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                map.Visibility = System.Windows.Visibility.Visible;
                TaskListBox.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void btnRight_Click(object sender, RoutedEventArgs e)
        {
            if (retrievedTasks.Last() == retrievedTasks[index])
            {
                ls = retrievedTasks.Last();
            }
            else
            {
                index += 1;
                ls = retrievedTasks[index];
            }

            if (ls != null)
            {
                lblbusid.Text = ls.bus_line;
                lblStart.Text = ls.bus_start;
                lblStop.Text = ls.bus_end;
                lblbusName.Text = ls.bus_name;
                lbltime.Text = ls.bus_startstop_time;

                getListDatabusstop(ls.bus_line);
            }
        }

        private void btnLeft_Click(object sender, RoutedEventArgs e)
        {

            if (index != 0)
            {
                index -= 1;
            }

            ls = retrievedTasks[index];

            if (ls != null)
            {
                lblbusid.Text = ls.bus_line;

                lblStart.Text = ls.bus_start;
                lblStop.Text = ls.bus_end;
                lblbusName.Text = ls.bus_name;
                lbltime.Text = ls.bus_startstop_time;

                getListDatabusstop(ls.bus_line);
            }
        }

        private void close_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btTopMenu_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ShowProgressIndicator(String msg)
        {
            if (progressIndicator == null)
            {
                progressIndicator = new ProgressIndicator();
                progressIndicator.IsIndeterminate = true;
            }
            SystemTray.Opacity = 0;
            progressIndicator.Text = msg;
            progressIndicator.IsVisible = true;
            progressIndicator.IsIndeterminate = true;
            SystemTray.SetProgressIndicator(this, progressIndicator);
        }

        private void HideProgressIndicator()
        {
            progressIndicator.IsVisible = false;
            progressIndicator.IsIndeterminate = false;
            SystemTray.SetProgressIndicator(this, progressIndicator);
        }
    }
}