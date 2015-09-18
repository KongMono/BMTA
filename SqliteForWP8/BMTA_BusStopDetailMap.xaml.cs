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
    public partial class BMTA_BusStopDetailMap : PhoneApplicationPage
    {
        public String lang = (Application.Current as App).Language;
        UCCustomToolTip _tooltip = new UCCustomToolTip();
        UCToolTip _stooltip = new UCToolTip();
        UCCostomPushpin _Pushpin = new UCCostomPushpin();
        private MapPolyline line;
        MapLayer layer;
        MapLayer mymapLayer = new MapLayer();
        List<GeoCoordinate> MyCoordinates = new List<GeoCoordinate>();

        public BMTA_BusStopDetailMap()
        {
            InitializeComponent();
            ShowMyLocationOnTheMap();

            new_searchfindRoutingItem_data data = (Application.Current as App).RountingDataBusStop;

            var d = Convert.ToDouble(data.distance);
            if (d < 1000)
            {
                if (lang.Equals("th"))
                {
                    titleName.Text = "ป้ายรถเมล์";
                    textRoute.Text = Convert.ToString(Math.Round(d, 2)) + " ม.";
                }
                else
                {
                    titleName.Text = "Bus Stop";
                    textRoute.Text = Convert.ToString(Math.Round(d, 2)) + " m.";
                }
            }
            else
            {
                d = d / 1000;
                if (lang.Equals("th"))
                {
                    titleName.Text = "ป้ายรถเมล์";
                    textRoute.Text = Convert.ToString(Math.Round(d, 2)) + " กม.";
                }
                else
                {
                    titleName.Text = "Bus Stop";
                    textRoute.Text = Convert.ToString(Math.Round(d, 2)) + " km.";
                }
            }

            UCStartStopBusLine UCStartStopBusLine = new UCStartStopBusLine();

            UCStartStopBusLine.DataContext = data;


            if (data.list.Count == 1)
            {
                UCStartStopBusLine.text_route1.Text = getNameBustype(data.list[0].busline, data.list[0].bustype);
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
            else if (data.list.Count == 2)
            {
                UCStartStopBusLine.text_route1.Text = getNameBustype(data.list[0].busline, data.list[0].bustype);
                UCStartStopBusLine.text_route2.Text = getNameBustype(data.list[1].busline, data.list[1].bustype);

                UCStartStopBusLine.img_route3.Visibility = System.Windows.Visibility.Collapsed;
                UCStartStopBusLine.img_route4.Visibility = System.Windows.Visibility.Collapsed;

                UCStartStopBusLine.img_cen3.Visibility = System.Windows.Visibility.Collapsed;
                UCStartStopBusLine.img_cen4.Visibility = System.Windows.Visibility.Collapsed;

                UCStartStopBusLine.text_route3.Visibility = System.Windows.Visibility.Collapsed;
                UCStartStopBusLine.text_route4.Visibility = System.Windows.Visibility.Collapsed;

            }
            else if (data.list.Count == 3)
            {
                UCStartStopBusLine.text_route1.Text = getNameBustype(data.list[0].busline, data.list[0].bustype);
                UCStartStopBusLine.text_route2.Text = getNameBustype(data.list[1].busline, data.list[1].bustype);
                UCStartStopBusLine.text_route3.Text = getNameBustype(data.list[2].busline, data.list[2].bustype);

                UCStartStopBusLine.img_route4.Visibility = System.Windows.Visibility.Collapsed;
                UCStartStopBusLine.img_cen4.Visibility = System.Windows.Visibility.Collapsed;
                UCStartStopBusLine.text_route4.Visibility = System.Windows.Visibility.Collapsed;
            }

            busStartStoplistbox.Items.Add(UCStartStopBusLine);
            this.Pushpin(data);
        }
        public String getNameBustype(string busline, string bustype)
        {
            string value = "";

            if (bustype == "2")
            {
                if (lang.Equals("th"))
                {
                    value = busline + " ปอ.";
                }
                else
                {
                    value = busline + " air";
                }
            }
            else
            {
                value = busline;
            }

            return value;
        }
        private async void ShowMyLocationOnTheMap()
        {
            // Get my current location.
            Geolocator myGeolocator = new Geolocator();
            Geoposition myGeoposition = await myGeolocator.GetGeopositionAsync(TimeSpan.FromMinutes(5), TimeSpan.FromSeconds(5));
            Geocoordinate myGeocoordinate = myGeoposition.Coordinate;
            GeoCoordinate myGeoCoordinate = CoordinateConverter.ConvertGeocoordinate(myGeocoordinate);

            this.map.Center = myGeoCoordinate;
            this.map.ZoomLevel = 14;

            // Create a small circle to mark the current location.
            Ellipse myCircle = new Ellipse();
            myCircle.Fill = new SolidColorBrush(Colors.Transparent);
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
            busstop_search.Text = this.NavigationContext.QueryString["TextFrom"];
        }

        private void Pushpin(new_searchfindRoutingItem_data bus_stop)
        {
            // Map clear
            map.Layers.Clear();
            map.MapElements.Clear();
            map.Center = new GeoCoordinate(Convert.ToDouble(bus_stop.route[0].busstop.latitude), Convert.ToDouble(bus_stop.route[0].busstop.longitude));
            foreach (var item in bus_stop.route)
            {
                layer = new MapLayer();
                // Map Layer

                map.Layers.Add(layer);
                line = new MapPolyline();
                line.StrokeColor = Colors.Red;
                line.StrokeThickness = 3;

                if (item == bus_stop.route.First())
                {
                    UCCustomToolTipStart _tooltip = new UCCustomToolTipStart();
                    if (lang.Equals("th"))
                    {
                        _tooltip.Description = item.busstop.name;
                    }
                    else
                    {
                        _tooltip.Description = item.busstop.name_en;
                    }

                    _tooltip.DataContext = item;
                   
                    MapOverlay overlay = new MapOverlay();
                    overlay.Content = _tooltip;
                    overlay.GeoCoordinate = new GeoCoordinate(Convert.ToDouble(item.busstop.latitude), Convert.ToDouble(item.busstop.longitude));
                    layer.Add(overlay);
                }
                else if (item == bus_stop.route.Last() || item.step == "6")
                {
                    UCCustomToolTipStop _tooltip = new UCCustomToolTipStop();
                    if (lang.Equals("th"))
                    {
                        _tooltip.Description = item.busstop.name;
                    }
                    else
                    {
                        _tooltip.Description = item.busstop.name_en;
                    }

                    _tooltip.DataContext = item;
                   
                    MapOverlay overlay = new MapOverlay();
                    overlay.Content = _tooltip;
                    overlay.GeoCoordinate = new GeoCoordinate(Convert.ToDouble(item.busstop.latitude), Convert.ToDouble(item.busstop.longitude));
                    layer.Add(overlay);
                }
                else if (item.step == "4")
                {
                    UCCustomToolTipWalkTranfer _tooltip = new UCCustomToolTipWalkTranfer();
                    if (lang.Equals("th"))
                    {
                        _tooltip.Description = item.busstop.name;
                    }
                    else
                    {
                        _tooltip.Description = item.busstop.name_en;
                    }

                    _tooltip.DataContext = item;
                   
                    MapOverlay overlay = new MapOverlay();
                    overlay.Content = _tooltip;
                    overlay.GeoCoordinate = new GeoCoordinate(Convert.ToDouble(item.busstop.latitude), Convert.ToDouble(item.busstop.longitude));
                    layer.Add(overlay);
                }
                else
                {
                    UCCustomToolTipTranfer _tooltip = new UCCustomToolTipTranfer();
                    if (lang.Equals("th"))
                    {
                        _tooltip.Description = item.busstop.name;
                    }
                    else
                    {
                        _tooltip.Description = item.busstop.name_en;
                    }

                    _tooltip.DataContext = item;
                   
                    MapOverlay overlay = new MapOverlay();
                    overlay.Content = _tooltip;
                    overlay.GeoCoordinate = new GeoCoordinate(Convert.ToDouble(item.busstop.latitude), Convert.ToDouble(item.busstop.longitude));
                    layer.Add(overlay);
                }
            }

            foreach (String[] polyline in bus_stop.polyline)
            {
                line.Path.Add(new GeoCoordinate(Convert.ToDouble(polyline[0]), Convert.ToDouble(polyline[1])));
            }
            map.MapElements.Add(line);

            map.ZoomLevel = 14;
        }

        private void btback_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void textStart_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/BMTA_BusStopDetailList.xaml?TextFrom=" + busstop_search.Text, UriKind.Relative));
        }
    }
}