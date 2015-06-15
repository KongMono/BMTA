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
    public partial class BMTA_BusStartStopDetailMap : PhoneApplicationPage
    {
        public String lang = (Application.Current as App).Language;
        UCCustomToolTip _tooltip = new UCCustomToolTip();
        UCToolTip _stooltip = new UCToolTip();
        UCCostomPushpin _Pushpin = new UCCostomPushpin();
        private MapPolyline line;
        MapLayer layer;
        string lat, lon;
        MapLayer mymapLayer = new MapLayer();
        List<GeoCoordinate> MyCoordinates = new List<GeoCoordinate>();

        public BMTA_BusStartStopDetailMap()
        {
            InitializeComponent();
            ShowMyLocationOnTheMap();
        }

        private async void ShowMyLocationOnTheMap()
        {
            // Get my current location.
            Geolocator myGeolocator = new Geolocator();
            Geoposition myGeoposition = await myGeolocator.GetGeopositionAsync(TimeSpan.FromMinutes(5), TimeSpan.FromSeconds(5));
            Geocoordinate myGeocoordinate = myGeoposition.Coordinate;
            GeoCoordinate myGeoCoordinate = CoordinateConverter.ConvertGeocoordinate(myGeocoordinate);

            this.map.Center = myGeoCoordinate;
            this.map.ZoomLevel = 18;

            // Create a small circle to mark the current location.
            Ellipse myCircle = new Ellipse();
            myCircle.Fill = new SolidColorBrush(Colors.Blue);
            myCircle.Height = 20;
            myCircle.Width = 20;
            myCircle.Opacity = 50;

            // Create a MapOverlay to contain the circle.
            MapOverlay myLocationOverlay = new MapOverlay();
            myLocationOverlay.Content = myCircle;
            myLocationOverlay.GeoCoordinate = myGeoCoordinate;
         
            // Create a MapLayer to contain the MapOverlay.
            mymapLayer.Add(myLocationOverlay);

            // Add the MapLayer to the Map.
            this.map.Layers.Add(mymapLayer);
            map.CartographicMode = MapCartographicMode.Road;
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {

            searchfindRoutingItem_data data = (Application.Current as App).RountingData;

            if (lang.Equals("th"))
            {
                titleName.Text = "ต้นทางปลายทาง";
                textRoute.Text = data.total.total_distance + " กม.";
            }
            else
            {
                titleName.Text = "Start - End";
                textRoute.Text = data.total.total_distance + " km.";
            }

            busStartStopFrom_search.Text = this.NavigationContext.QueryString["TextFrom"];
            busStartStopTo_search.Text = this.NavigationContext.QueryString["TextTo"];

            UCStartStopBusLine UCStartStopBusLine = new UCStartStopBusLine();

            UCStartStopBusLine.DataContext = data;


            if (data.routing.Count == 1)
            {
                UCStartStopBusLine.text_route1.Text = data.routing[0].bus_line;
                UCStartStopBusLine.img_route2.Visibility = System.Windows.Visibility.Collapsed;
                UCStartStopBusLine.img_route3.Visibility = System.Windows.Visibility.Collapsed;
                UCStartStopBusLine.img_route4.Visibility = System.Windows.Visibility.Collapsed;

                UCStartStopBusLine.img_cen2.Visibility = System.Windows.Visibility.Collapsed;
                UCStartStopBusLine.img_cen3.Visibility = System.Windows.Visibility.Collapsed;
                UCStartStopBusLine.img_cen4.Visibility = System.Windows.Visibility.Collapsed;

                UCStartStopBusLine.text_route2.Visibility = System.Windows.Visibility.Collapsed;
                UCStartStopBusLine.text_route3.Visibility = System.Windows.Visibility.Collapsed;
                UCStartStopBusLine.text_route4.Visibility = System.Windows.Visibility.Collapsed;


            }
            else if (data.routing.Count == 2)
            {
                UCStartStopBusLine.text_route1.Text = data.routing[0].bus_line;
                UCStartStopBusLine.text_route2.Text = data.routing[1].bus_line;

                UCStartStopBusLine.img_route3.Visibility = System.Windows.Visibility.Collapsed;
                UCStartStopBusLine.img_route4.Visibility = System.Windows.Visibility.Collapsed;

                UCStartStopBusLine.img_cen3.Visibility = System.Windows.Visibility.Collapsed;
                UCStartStopBusLine.img_cen4.Visibility = System.Windows.Visibility.Collapsed;

                UCStartStopBusLine.text_route3.Visibility = System.Windows.Visibility.Collapsed;
                UCStartStopBusLine.text_route4.Visibility = System.Windows.Visibility.Collapsed;

            }
            else if (data.routing.Count == 3)
            {
                UCStartStopBusLine.text_route1.Text = data.routing[0].bus_line;
                UCStartStopBusLine.text_route2.Text = data.routing[1].bus_line;
                UCStartStopBusLine.text_route3.Text = data.routing[2].bus_line;

                UCStartStopBusLine.img_route4.Visibility = System.Windows.Visibility.Collapsed;
                UCStartStopBusLine.img_cen4.Visibility = System.Windows.Visibility.Collapsed;
                UCStartStopBusLine.text_route4.Visibility = System.Windows.Visibility.Collapsed;
            }

            busStartStoplistbox.Items.Add(UCStartStopBusLine);

            this.Pushpin(data);

        }

        private async void Pushpin(searchfindRoutingItem_data bus_stop)
        {
            // Map clear
            map.Layers.Clear();
            map.MapElements.Clear();

            foreach (var item in bus_stop.routing)
            {
                layer = new MapLayer();

                foreach (var busstop in item.busstop)
                {
                    Pushpin pushpin = new Pushpin();
                    pushpin.GeoCoordinate = new GeoCoordinate(Convert.ToDouble(busstop.latitude), Convert.ToDouble(busstop.longitude));
                    var uriString = @"Assets/btn_bus.png";
                    pushpin.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(uriString, UriKind.Relative)) };
                    pushpin.Width = 35;
                    pushpin.Height = 31;
                    MapOverlay overlay = new MapOverlay();
                    overlay.Content = pushpin;
                    overlay.GeoCoordinate = new GeoCoordinate(Convert.ToDouble(busstop.latitude), Convert.ToDouble(busstop.longitude));
                    layer.Add(overlay);
                }
                // Map Layer
                map.Center = new GeoCoordinate(Convert.ToDouble(item.busstop[0].latitude), Convert.ToDouble(item.busstop[0].longitude));
                map.Layers.Add(layer);

                line = new MapPolyline();
                line.StrokeColor = Colors.Red;
                line.StrokeThickness = 3;

                foreach (String[] polyline in item.bus_polyline)
                {
                    line.Path.Add(new GeoCoordinate(Convert.ToDouble(polyline[0]), Convert.ToDouble(polyline[1])));
                }
              
                // Map Polyline
                map.MapElements.Add(line);
             
            }

            map.ZoomLevel = 12;
        }

        private void btBusStop_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btback_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }


        private void textStart_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/BMTA_BusStartStopDetailList.xaml?TextFrom=" + busStartStopFrom_search.Text + "&TextTo=" + busStartStopTo_search.Text, UriKind.Relative));
        }
    }
}