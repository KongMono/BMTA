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

        }

        void Query_QueryCompleted(object sender, QueryCompletedEventArgs<IList<MapLocation>> e)
        {
            _tooltip.Description = "";
            StringBuilder _description = new StringBuilder();
            foreach (var item in e.Result)
            {
                if (!(item.Information.Address.BuildingName == ""))
                {
                    _description.Append(item.Information.Address.BuildingName + ", ");

                }
                if (!(item.Information.Address.BuildingFloor == ""))
                {
                    _description.Append(item.Information.Address.BuildingFloor + ", ");

                }
                if (!(item.Information.Address.Street == ""))
                {
                    _description.Append(item.Information.Address.Street + ", ");

                }
                if (!(item.Information.Address.District == ""))
                {
                    _description.Append(item.Information.Address.District + ",");

                }
                if (!(item.Information.Address.City == ""))
                {
                    _description.Append(item.Information.Address.City + ", ");

                }
                if (!(item.Information.Address.State == ""))
                {
                    _description.Append(item.Information.Address.State + ", ");

                }
                if (!(item.Information.Address.Street == ""))
                {
                    _description.Append(item.Information.Address.Street + ", ");

                }
                if (!(item.Information.Address.Country == ""))
                {
                    _description.Append(item.Information.Address.Country + ", ");

                }

                if (!(item.Information.Address.Province == ""))
                {
                    _description.Append(item.Information.Address.Province + ", ");

                }
                if (!(item.Information.Address.PostalCode == ""))
                {
                    _description.Append(item.Information.Address.PostalCode);

                }

                _tooltip.Description = _description.ToString();
                _tooltip.FillDescription();
                break;
            }
        }

        private void MapView_Loaded(object sender, RoutedEventArgs e)
        {
            loadLocation();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {

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
                dataNearBusStopItem data = (Application.Current as App).DataBusstopDetail;

                if (lang.Equals("th"))
                {
                    titleName.Text = "ป้ายหยุดรถประจำทาง";
                    textName.Text = data.stop_name;
                }
                else
                {
                    titleName.Text = "Bus Stop";
                    textName.Text = data.stop_name_en;
                }
                
              
                btBusStop.Click += btBusStop_Click;
            }
        }

        private void btBusStop_Click(object sender, RoutedEventArgs e)
        {
         
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

            }
        }

        private void btback_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        public async void loadLocation()
        {
            Geolocator myGeolocator = new Geolocator();
            try
            {
                Geoposition myGeoposition = await myGeolocator.GetGeopositionAsync();
                Geocoordinate myGeocoordinate = myGeoposition.Coordinate;
                GeoCoordinate myGeoCoordinate = CoordinateConverter.ConvertGeocoordinate(myGeocoordinate);
                lat = myGeocoordinate.Latitude.ToString();
                lon = myGeocoordinate.Longitude.ToString();
            }
            catch (UnauthorizedAccessException)
            {

                MessageBox.Show("location is disabled in phone settings.");
                return;
            }
        }
    }
}