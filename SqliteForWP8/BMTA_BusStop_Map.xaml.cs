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
using System.Diagnostics;
using System.Net.Browser;
using BMTA.Usercontrols;
using Microsoft.Phone.Maps.Services;
using System.IO.IsolatedStorage;
using BMTA.Item;


namespace BMTA
{
    public partial class BMTA_BusStop_Map : PhoneApplicationPage
    {
        public String lang = (Application.Current as App).Language;
        UCCustomToolTip _tooltip = new UCCustomToolTip();
        UCToolTip _stooltip = new UCToolTip();
        UCCostomPushpin _Pushpin = new UCCostomPushpin();
        List<GeoCoordinate> MyCoordinates = new List<GeoCoordinate>();
        MapLayer mymapLayer = new MapLayer();
        RouteQuery MyQuery = null;
        dataNearBusStopItem data;
        Boolean alreadyBusStop = false;
        ProgressIndicator progressIndicator = new ProgressIndicator();
        static WebClient webClient;
        private searchlandmarkAndBusstopdetailItem itemBusStop;

        public BMTA_BusStop_Map()
        {
            InitializeComponent();
            this.GetCoordinates();
            busstop_search.ItemFilter = SearchText;
        }

        bool SearchText(string search, object value)
        {
            if (value != null)
            {
                return true;
            }
            //... If no match, return false. 
            return false;
        }

        private async void GetCoordinates()
        {
            // Get the phone's current location.
            Geolocator MyGeolocator = new Geolocator();
            MyGeolocator.DesiredAccuracyInMeters = 5;
            try
            {
               
                MyCoordinates.Add(new GeoCoordinate(Convert.ToDouble((Application.Current as App).lat_current), Convert.ToDouble((Application.Current as App).lon_current)));

                this.map.Center = MyCoordinates[0];
                this.map.ZoomLevel = 13;

                // Create a small circle to mark the current location.
                Ellipse myCircle = new Ellipse();
                myCircle.Fill = new SolidColorBrush(Colors.Blue);
                myCircle.Height = 20;
                myCircle.Width = 20;
                myCircle.Opacity = 50;

                // Create a MapOverlay to contain the circle.
                MapOverlay myLocationOverlay = new MapOverlay();
                myLocationOverlay.Content = myCircle;
                myLocationOverlay.GeoCoordinate = MyCoordinates[0];

                // Create a MapLayer to contain the MapOverlay.
                mymapLayer.Add(myLocationOverlay);

                // Add the MapLayer to the Map.
                this.map.Layers.Add(mymapLayer);


            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Location is disabled in phone settings or capabilities are not checked.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void MyQuery_QueryCompleted(object sender, QueryCompletedEventArgs<Route> e)
        {
            if (e.Error == null)
            {
                Route MyRoute = e.Result;
                MapRoute MyMapRoute = new MapRoute(MyRoute);
                this.map.AddRoute(MyMapRoute);

                List<string> RouteList = new List<string>();
                foreach (RouteLeg leg in MyRoute.Legs)
                {
                    foreach (RouteManeuver maneuver in leg.Maneuvers)
                    {
                        RouteList.Add(maneuver.InstructionText);
                    }
                }
                MyQuery.Dispose();
            }
        }

        private void Pushpin(Double lat, Double lon)
        {
            // Map clear
            MapLayer layer = new MapLayer();

            Pushpin pushpin = new Pushpin();
            pushpin.GeoCoordinate = new GeoCoordinate(lat, lon);
            var uriString = @"Assets/pin_blue.png";
            pushpin.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(uriString, UriKind.Relative)) };
            pushpin.Width = 35;
            pushpin.Height = 31;
            MapOverlay overlay = new MapOverlay();
            overlay.Content = pushpin;
            overlay.GeoCoordinate = new GeoCoordinate(lat, lon);
            layer.Add(overlay);

            // Map Layer
            map.Layers.Add(layer);

        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            data = (Application.Current as App).DataBusstopDetail;
            if (lang.Equals("th"))
            {
                titleName.Text = "ป้ายหยุดรถประจำทาง";
                textName.Text = data.stop_name;
                textRoute.Text = data.distance.Substring(0, 4) + " ม.";
            }
            else
            {
                titleName.Text = "Bus Stop";
                textName.Text = data.stop_name_en;
                textRoute.Text = data.distance.Substring(0, 4) + " m.";
            }


            textBusline.Text = data.busline;

            Pushpin(Convert.ToDouble(data.latitude), Convert.ToDouble(data.longitude));

            MyQuery = new RouteQuery();
            MyCoordinates.Add(new GeoCoordinate(Convert.ToDouble(data.latitude), Convert.ToDouble(data.longitude)));
            MyQuery.Waypoints = MyCoordinates;
            MyQuery.QueryCompleted += MyQuery_QueryCompleted;
            MyQuery.QueryAsync();
        }


        private void btback_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void textStart_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/BMTA_BusStop_CircleDetail.xaml?TextFrom=" + busstop_search.Text, UriKind.Relative));
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
            progressIndicator.IsIndeterminate = false;
            SystemTray.SetIsVisible(this, true);
            SystemTray.SetProgressIndicator(this, progressIndicator);
        }

        private void HideProgressIndicator()
        {
            progressIndicator.IsVisible = false;
            progressIndicator.IsIndeterminate = false;
            SystemTray.SetIsVisible(this, false);
            SystemTray.SetProgressIndicator(this, progressIndicator);
        }

        private void busstop_search_TextChanged(object sender, RoutedEventArgs e)
        {
            if (busstop_search.Text.Length > 2)
            {
                if (!alreadyBusStop)
                {
                    ShowProgressIndicator("Loading..");
                    alreadyBusStop = true;
                    callServicegetAutocompleteBusStop();
                }
            }
        }

        public void callServicegetAutocompleteBusStop()
        {
            webClient = new WebClient();
            String url = "http://128.199.232.94/webservice/keyword.php";
            string myParameters;
            try
            {
                myParameters = url + "?type=" + "busstop" + "&q=" + busstop_search.Text + "&lang=" + lang;
                Debug.WriteLine("URL callServicegetAutocompleteBusStop = " + myParameters);
                webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(callServicegetAutocompleteBusStop_Completed);
                webClient.DownloadStringAsync(new Uri(myParameters));
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void callServicegetAutocompleteBusStop_Completed(object sender, DownloadStringCompletedEventArgs e)
        {
            searchbusstopItem results = JsonConvert.DeserializeObject<searchbusstopItem>(e.Result);

            busstop_search.ItemsSource = results.data;

            HideProgressIndicator();
            alreadyBusStop = false;
        }

        private void busstop_search_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            searchlandmarkAndBusstopdetailItem item = (sender as AutoCompleteBox).SelectedItem as searchlandmarkAndBusstopdetailItem;
            if (item != null)
            {
                itemBusStop = item;
            }
        }

        public void callService_busstop_currentfindRouting()
        {
            webClient = new WebClient();
            webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            String url = "http://202.6.18.31:7777/currentfindRouting";
            string myParameters;
            try
            {
                if (itemBusStop == null)
                {
                    myParameters = "lat=" + (Application.Current as App).lat_current + "&long=" + (Application.Current as App).lon_current + "&busstop_end_id=" + "0" + "&bus_type=&running_type=&orderby=" + "";
                }
                else
                {
                    myParameters = "lat=" + (Application.Current as App).lat_current + "&long=" + (Application.Current as App).lon_current + "&busstop_end_id=" + itemBusStop.id + "&bus_type=&running_type=&orderby=" + "";
                    //myParameters = "lat=" + "13.741709" + "&long=" + "100.420125" + "&busstop_end_id=" + "4101" + "&bus_type=&running_type=&orderby=" + "";
                }

                Debug.WriteLine("URL callService_busstop_currentfindRouting = " + url);

                webClient.UploadStringCompleted += new UploadStringCompletedEventHandler(callService_busstop_currentfindRouting_Completed);
                webClient.UploadStringAsync(new Uri(url), myParameters);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void callService_busstop_currentfindRouting_Completed(object sender, UploadStringCompletedEventArgs e)
        {
            searchfindRoutingItem results = JsonConvert.DeserializeObject<searchfindRoutingItem>(e.Result);
            if (results == null)
            {
                MessageBox.Show("ไม่พบข้อมูล");
                return;
            }
            if (results.status == "0")
            {
                MessageBox.Show("ไม่พบข้อมูล");
                return;
            }
            (Application.Current as App).DataStop = results;

            this.NavigationService.Navigate(new Uri("/BMTA_BusStopDetailBus.xaml?Search=false&TextFrom=" + busstop_search.Text, UriKind.Relative));
        }

        private void busstop_search_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (string.IsNullOrWhiteSpace(busstop_search.Text))
                {
                    MessageBox.Show("กรุณาใส่ป้ายรถเมล์ที่ต้องการ");
                    return;
                }
                callService_busstop_currentfindRouting();
            }
        }
    }

}