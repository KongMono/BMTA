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
using System.Net.Http;
using System.Net.Http.Headers;



namespace BMTA
{
    public partial class BMTA_BusStop_not : PhoneApplicationPage
    {
        GeoCoordinate currentLocation = null;
        UCCustomToolTip _tooltip = new UCCustomToolTip();
        UCToolTip _stooltip = new UCToolTip();
        UCCostomPushpin _Pushpin = new UCCostomPushpin();
        ProgressIndicator prog;
        string responetext = "";
        private MapPolyline line;


        List<GeoCoordinate> MyCoordinates = new List<GeoCoordinate>();


        public BMTA_BusStop_not()
        {
            InitializeComponent();
           
            this.Loaded += MapView_Loaded;

            List<WP7Phone> dataSource = new List<WP7Phone>() 
			{
			new WP7Phone(){Image="/Images/DellVenue.jpg", Name = "Dell Venue"},
			new WP7Phone(){Image="/Images/HTChd7.jpg", Name = "HTC HD 7"},
			new WP7Phone(){Image="/Images/HTCMozart.jpg", Name = "HTC Mozart"},
			new WP7Phone(){Image="/Images/LGOptimus.jpg", Name = "LG Optimus"},
			new WP7Phone(){Image="/Images/LGQuantumC900.jpg", Name = "LG Quantum C 900"},
			};
            //this.acBox.ItemsSource = dataSource;
        }

        bool SearchPhones(string search, object value)
        {
            if (value != null)
            {
                WP7Phone datasourceValue = value as WP7Phone;
                string name = datasourceValue.Name;

                if (name.ToLower().StartsWith(search.ToLower()))
                    return true;
            }
            //... If no match, return false. 
            return false;
        } 

        private async void GetLocation()
        {
            // Get current location.
            Geolocator myGeolocator = new Geolocator();
            Geoposition myGeoposition = await myGeolocator.GetGeopositionAsync(maximumAge: TimeSpan.FromMinutes(5), timeout: TimeSpan.FromSeconds(10));
            Geocoordinate myGeocoordinate = myGeoposition.Coordinate;
            currentLocation = CoordinateConverter.ConvertGeocoordinate(myGeocoordinate);
            txtlat.Text = myGeocoordinate.Latitude.ToString();
            txtlon.Text = myGeocoordinate.Longitude.ToString();
         
          //  this.Loaded += MapView_Loaded;

            MapDisplay(currentLocation);

        }

        private void MapDisplay(GeoCoordinate LocationsData)
        {

            ReverseGeocodeQuery Query = new ReverseGeocodeQuery()
            {
                GeoCoordinate = new GeoCoordinate(LocationsData.Latitude, LocationsData.Longitude)
            };
            Query.QueryCompleted += Query_QueryCompleted;
            Query.QueryAsync();


            MapOverlay mylocationOverlay = new MapOverlay();
            mylocationOverlay.Content = _Pushpin;
            mylocationOverlay.GeoCoordinate = LocationsData;
            MapLayer myLocationLayer = new MapLayer();
            myLocationLayer.Add(mylocationOverlay);
            mymap.Layers.Add(myLocationLayer);
            mymap.Center = LocationsData;

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
            LoadAvalableParks();
            loadLocation();
            getNearBusStop();

        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            rightmenu.Visibility = System.Windows.Visibility.Collapsed;
            rightmenux.Visibility = System.Windows.Visibility.Collapsed;
            close.Visibility = System.Windows.Visibility.Collapsed;
            btBusStop.Visibility = System.Windows.Visibility.Collapsed;
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

                ContentPanelp.Visibility = Visibility;
                string x = BMTA.clGetResolution.Width.ToString();
                string y = BMTA.clGetResolution.Height.ToString();
                string xy = x + "x" + y;
                if (x == "480")
                {
                    ImageBrush brush = new ImageBrush
                    {
                        ImageSource = new System.Windows.Media.Imaging.BitmapImage(new Uri("/Assets/480x852/BMTA_busstop.png", UriKind.Relative)),
                        Opacity = 1d
                    };
                    this.LayoutRoot.Background = brush;
                    brush.Stretch = Stretch.Fill;
                }
                else if (x == "720")
                {
                    ImageBrush brush = new ImageBrush
                    {
                        ImageSource = new System.Windows.Media.Imaging.BitmapImage(new Uri("/Assets/720x1280/BMTA_busstop.png", UriKind.Relative)),
                        Opacity = 1d
                    };
                    this.LayoutRoot.Background = brush;
                    brush.Stretch = Stretch.Fill;
                }
                else
                {
                    ImageBrush brush = new ImageBrush
                    {
                        ImageSource = new System.Windows.Media.Imaging.BitmapImage(new Uri("/Assets/768x1280/BMTA_busstop.png", UriKind.Relative)),
                        Opacity = 1d
                    };
                    this.LayoutRoot.Background = brush;
                    brush.Stretch = Stretch.Fill;
                }

                getNearBusStop();
                ContentPanelp.Visibility = System.Windows.Visibility.Collapsed;

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

        public async void loadLocation()
        {
            Geolocator myGeolocator = new Geolocator();
            try
            {
                Geoposition myGeoposition = await myGeolocator.GetGeopositionAsync();
                Geocoordinate myGeocoordinate = myGeoposition.Coordinate;
                GeoCoordinate myGeoCoordinate = CoordinateConverter.ConvertGeocoordinate(myGeocoordinate);
                txtlat.Text = myGeocoordinate.Latitude.ToString();
                txtlon.Text = myGeocoordinate.Longitude.ToString();

                getNearBusStop();
            }
            catch (UnauthorizedAccessException)
            {
                // the app does not have the right capability or the location master switch is off 
                MessageBox.Show("location is disabled in phone settings.");
                return;
            }

          //  GetLocation();

        }



        public void getNearBusStop()
        {
            string url = "http://202.6.18.31/bmta/webservice/bmta_server.php/";
            HttpWebRequest request = WebRequest.CreateHttp(new Uri(url)) as HttpWebRequest;

            string data = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
            "<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\n" +
            "xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"\n" +
            "xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">\n" +
            "<soap:Body>\n" +
            "<getNearBusStop xmlns=\"http://202.6.18.31/bmta/webservice/bmta_service.php\">\n" +
            "<lat>" + txtlat.Text + "</lat>\n" +
            "<long>" + txtlon.Text + "</long>\n" +
            "<distance>5000</distance>\n" +
            "</getNearBusStop>\n" +
            "</soap:Body>\n" +
            "</soap:Envelope>";
            request.ContentType = "text/xml; charset=utf-8";// "application/x-www-form-urlencoded";
            request.UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0; Touch)";
            request.Headers["SOAPAction"] = "http://202.6.18.31/bmta/webservice/bmta_service.php?wsdl";
            // Set the Method property to 'POST' to post data to the URI.
            request.Method = "POST";
            request.ContentLength = data.Length;
            // start the asynchronous operation
            request.BeginGetRequestStream(new AsyncCallback(GetRequestStreamCallback), request);
        }

       void GetRequestStreamCallback(IAsyncResult asynchronousResult)
        {
            HttpWebRequest request = (HttpWebRequest)asynchronousResult.AsyncState;

            // End the operation
            Stream postStream = request.EndGetRequestStream(asynchronousResult);
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {

                string data = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
               "<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\n" +
               "xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"\n" +
               "xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">\n" +
               "<soap:Body>\n" +
               "<getNearBusStop xmlns=\"http://202.6.18.31/bmta/webservice/bmta_service.php\">\n" +
               "<lat>" + txtlat.Text + "</lat>\n" +
               "<long>" + txtlon.Text + "</long>\n" +
               "<distance>5000</distance>\n" +
               "</getNearBusStop>\n" +
               "</soap:Body>\n" +
               "</soap:Envelope>";
                // Convert the string into a byte array. 
                byte[] byteArray = Encoding.UTF8.GetBytes(data);

                // Write to the request stream.
                postStream.Write(byteArray, 0, data.Length);
                postStream.Close();
            });

            // Start the asynchronous operation to get the response
            request.BeginGetResponse(new AsyncCallback(GetResponseCallback), request);

        }

        void GetResponseCallback(IAsyncResult getNearBusStopResponse)
        {

            try
            {
                HttpWebRequest request = (HttpWebRequest)getNearBusStopResponse.AsyncState;

                // End the operation
                HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(getNearBusStopResponse);//.EndGetResponse(getNearBusStopResponse);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Stream streamResponse = response.GetResponseStream();
                    StreamReader streamRead = new StreamReader(streamResponse);
                    // string Notificationdata = Helper.RemoveNameSpace.RemoveAllNamespaces(streamRead.ReadToEnd()); 
                    string responseString = streamRead.ReadToEnd();

                    //display the web response
                    //  Debug.WriteLine("Response String : " + responseString);
                    String restext = responseString;

                    XDocument xDocx = XDocument.Load(new StringReader(restext));
                    var id = xDocx.Descendants("return").First().Value;
                    string sd = id.ToString();
                    sd = sd.Replace("{\"status\":\"1\",\"data\":", "");
                    sd = sd.Replace("]}", "]");

                    responetext = sd;

                   
                    // Close the stream object
                    streamResponse.Close();
                    streamRead.Close();

                    // Release the HttpWebResponse
                    response.Close();

                } 
            }
            catch (WebException ex)
            {
                using (StreamReader reader = new StreamReader(ex.Response.GetResponseStream()))
                {
                    Debug.WriteLine("Exception output : " + ex);
                }
            }

        }

      //  public List<GeoCoordinate> MyCoordinates = new List<GeoCoordinate>();
        LocationList LocationListobj = new LocationList();
        private void LoadAvalableParks()
        {
            try
            {
                getNearBusStop();
              
                string strJSON = null;
                strJSON = responetext;
                MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(strJSON));
                ObservableCollection<LocationDetail> LocationListobj = new ObservableCollection<LocationDetail>();
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(ObservableCollection<LocationDetail>));
                LocationListobj = (ObservableCollection<LocationDetail>)serializer.ReadObject(ms);
                if (responetext != "")
                {
                    mymap.Layers.Clear();
                    MapLayer mapLayer = new MapLayer();
                    MyCoordinates.Clear();
                    mymap.MapElements.Clear();
                    btBusStop.Visibility = System.Windows.Visibility.Collapsed;

                    for (int i = 0; i < LocationListobj.Count; i++)
                    {
                        MyCoordinates.Add(new GeoCoordinate { Latitude = double.Parse("" + LocationListobj[i].latitude), Longitude = double.Parse("" + LocationListobj[i].longitude) });

                        //DrawMapMarker(MyCoordinates[i], Colors.Red, mapLayer, parklist.parking_details[i].DestinationName);
                        UCCustomToolTip _tooltip = new UCCustomToolTip();
                        _tooltip.Description = LocationListobj[i].stop_name.ToString() + "\n" + LocationListobj[i].busline;
                        _tooltip.DataContext = LocationListobj[i];
                        // _tooltip.Menuitem.Click += Menuitem_Click;
                        _tooltip.imgmarker.Tap += _tooltip_Tapimg;
                        _tooltip.imgborder.Click += _tooltip_url;

                        MapOverlay overlay = new MapOverlay();
                        overlay.Content = _tooltip;
                        overlay.GeoCoordinate = MyCoordinates[i];
                        overlay.PositionOrigin = new Point(0.0, 1.0);
                        mapLayer.Add(overlay);
                    }
                    mymap.Layers.Add(mapLayer);
                    GetLocation();
                    //  DrawMapMarkers();
                    // Thickness ss=new Thickness(10,0,0,10);


                    // LocationRectangle boundingRectangle = new LocationRectangle( );
                    mymap.Center = MyCoordinates[MyCoordinates.Count - 1];
                    //  MapVieMode.ZoomLevel = 14;
                    Dispatcher.BeginInvoke(() =>
                    {
                        mymap.SetView(LocationRectangle.CreateBoundingRectangle(MyCoordinates));
                    });
                    // MapVieMode.SetView(LocationRectangle.CreateBoundingRectangle(from 1 in MyCoordinates);
                    mymap.SetView(MyCoordinates[MyCoordinates.Count - 1], 10, MapAnimationKind.Linear);
                }
                else
                {
                    GetLocation();
                }
            }
            catch
            {
            }

        }

        private void DrawMapMarkers()
        {
            //MapVieMode.Layers.Clear();
            MapLayer mapLayer = new MapLayer();
            // Draw marker for current position       

            // Draw markers for location(s) / destination(s)
            for (int i = 0; i < LocationListobj.Count; i++)
            {

                //DrawMapMarker(MyCoordinates[i], Colors.Red, mapLayer, parklist.parking_details[i].DestinationName);
                UCCustomToolTip _tooltip = new UCCustomToolTip();
                _tooltip.Description = LocationListobj[i].stop_name.ToString() + "\n" + LocationListobj[i].distance;
                _tooltip.DataContext = LocationListobj[i];
               // _tooltip.Menuitem.Click += Menuitem_Click;
                _tooltip.imgmarker.Tap += _tooltip_Tapimg;

                MapOverlay overlay = new MapOverlay();
                overlay.Content = _tooltip;
                overlay.GeoCoordinate = MyCoordinates[i];
                overlay.PositionOrigin = new Point(0.0, 1.0);
                mapLayer.Add(overlay);
            }

            mymap.Layers.Add(mapLayer);
        }


        private async void _tooltip_url(object sender, RoutedEventArgs e)
        {


            btBusStop.Visibility = Visibility;
            mymap.Layers.Clear();
            MyCoordinates.Clear();
            mymap.MapElements.Clear();
           

           Button _button = (Button)sender;
           string bt = _button.Tag.ToString();

            

           string strJSON = null;
           strJSON = responetext;
           MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(strJSON));
           ObservableCollection<LocationDetail> LocationListobj = new ObservableCollection<LocationDetail>();
           DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(ObservableCollection<LocationDetail>));
           LocationListobj = (ObservableCollection<LocationDetail>)serializer.ReadObject(ms);
         //  LocationListobj.Where(r => r.id == bt);
            SelectPin c = new SelectPin();
           if (responetext != "")
           {
               items.Clear();
               foreach (var item in LocationListobj.Where(p => p.id == bt))
               {
                c.id = item.id;
                c.stop_name = item.stop_name;
                c.latitude = item.latitude;
                c.longitude = item.longitude;
                c.distance = item.distance;
                c.busline = item.busline;
                items.Add(c);
               }

               
               MapLayer mapLayer = new MapLayer();


               for(int i =0; i < items.Count; i++)
                   {
                   MyCoordinates.Add(new GeoCoordinate { Latitude = double.Parse("" + items[i].latitude), Longitude = double.Parse("" + items[i].longitude) });

                   //DrawMapMarker(MyCoordinates[i], Colors.Red, mapLayer, parklist.parking_details[i].DestinationName);
                   UCToolTip _stooltip = new UCToolTip();
                   _stooltip.Description = items[i].stop_name.ToString() + "\n" + items[i].busline;
                   _stooltip.DataContext = items[i];
                   // _tooltip.Menuitem.Click += Menuitem_Click;
                   _tooltip.imgmarker.Tap += _tooltip_Tapimg;
                 //  _tooltip.imgborder.Click += _tooltip_url;

                   MapOverlay overlaypoly = new MapOverlay();
                   overlaypoly.Content = _stooltip;
                   overlaypoly.GeoCoordinate = MyCoordinates[i];
                   overlaypoly.PositionOrigin = new Point(0.0, 1.0);
                   mapLayer.Add(overlaypoly);
                   }


               line = new MapPolyline();
               line.StrokeColor = Colors.Blue;
               line.StrokeThickness = 2;

               polylines pl = new polylines();
             
               // Get current location.
               Geolocator myGeolocator = new Geolocator();
               Geoposition myGeoposition = await myGeolocator.GetGeopositionAsync(maximumAge: TimeSpan.FromMinutes(5), timeout: TimeSpan.FromSeconds(10));
               Geocoordinate myGeocoordinate = myGeoposition.Coordinate;
              
               line.Path.Add(new GeoCoordinate(myGeocoordinate.Latitude, myGeocoordinate.Longitude));


               itemspoly.Clear();
               foreach (var itempoly in LocationListobj.Where(p => p.id == bt))
               {
                   pl.id = itempoly.id;
                   pl.latitude = Convert.ToDouble(itempoly.latitude);
                   pl.longitude = Convert.ToDouble(itempoly.longitude);
                   itemspoly.Add(pl);

                   line.Path.Add(new GeoCoordinate(pl.latitude, pl.longitude));
               }

              
               mymap.MapElements.Add(line);

               mymap.Layers.Add(mapLayer);
               GetLocation();
               btBusStop.Content = c.stop_name+"\n"+c.busline;
               btBusStop.Tag = c.busline;
               btBusStop.CommandParameter = c.stop_name;
               btBusStop.FontSize = 20;
               btBusStop.Click +=btBusStop_Click;
               //  DrawMapMarkers();
               // Thickness ss=new Thickness(10,0,0,10);


               // LocationRectangle boundingRectangle = new LocationRectangle( );
               mymap.Center = MyCoordinates[MyCoordinates.Count - 1];
               //  MapVieMode.ZoomLevel = 14;
               Dispatcher.BeginInvoke(() =>
               {
                   mymap.SetView(LocationRectangle.CreateBoundingRectangle(MyCoordinates));
               });
               // MapVieMode.SetView(LocationRectangle.CreateBoundingRectangle(from 1 in MyCoordinates);
               mymap.SetView(MyCoordinates[MyCoordinates.Count - 1], 10, MapAnimationKind.Linear);
           }
          
         //  NavigationService.Navigate(new Uri("/BMTA_BusStop_pin.xaml?key=" + _button.Tag.ToString() + "&stop_name=" + c.stop_name.ToString() + "&busline=" + c.busline.ToString() + "&latitude=" + c.latitude + "&longitude=" + c.longitude, UriKind.Relative));
        }

        private void btBusStop_Click(object sender, RoutedEventArgs e)
        {
           // MessageBox.Show("gooooooooooooo");
            Button _button = (Button)sender;
            string _busline = _button.Tag.ToString();
            string _stop_name = _button.CommandParameter.ToString();

            NavigationService.Navigate(new Uri("/BMTA_BusStop_pin.xaml?_busline=" + _button.Tag.ToString() + "&stop_name=" + _stop_name.ToString(), UriKind.Relative));

        }

        public class SelectPin
        {
            public string id{get;set;}
            public string stop_name{get;set;}
            public string busline{get;set;}
            public string latitude { get; set; }
            public string longitude { get; set; }
            public string distance { get; set; }
        }
        public static List<SelectPin> items = new List<SelectPin>();

        public class polylines
        {
            public string id { get; set; }
            public string stop_name { get; set; }
            public string busline { get; set; }
            public double latitude { get; set; }
            public double longitude { get; set; }
            public string distance { get; set; }
        }
        public static List<polylines> itemspoly = new List<polylines>();

        private void Menuitem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MenuItem item = (MenuItem)sender;
                string selecteditem = item.Tag.ToString();
                var selectedparkdata = LocationListobj.Where(s => s.id == selecteditem).ToList();
                if (selectedparkdata.Count > 0)
                {
                    foreach (var items in selectedparkdata)
                    {

                        //if (Settings.FileExists("LocationDetailItem"))
                        //{
                        //    Settings.DeleteFile("LocationDetailItem");
                        //}
                        //using (IsolatedStorageFileStream fileStream = Settings.OpenFile("LocationDetailItem", FileMode.Create))
                        //{
                        //    DataContractSerializer serializer = new DataContractSerializer(typeof(LocationDetail));
                        //    serializer.WriteObject(fileStream, items);

                        //}
                        NavigationService.Navigate(new Uri("/MapViewDetailsPage.xaml", UriKind.Relative));
                        break;
                    }
                }
            }
            catch
            {
            }
        }

        private void _tooltip_Tapimg(object sender, System.Windows.Input.GestureEventArgs e)
        {
            try
            {
                Image item = (Image)sender;
                string selecteditem = item.Tag.ToString();
                var selectedparkdata = LocationListobj.Where(s => s.id == selecteditem).ToList();


                if (selectedparkdata.Count > 0)
                {
                    foreach (var items in selectedparkdata)
                    {
                        ContextMenu contextMenu =
                    ContextMenuService.GetContextMenu(item);
                        contextMenu.DataContext = items;
                        if (contextMenu.Parent == null)
                        {
                            contextMenu.IsOpen = true;

                        }
                        NavigationService.Navigate(new Uri("/MapViewDetailsPage.xaml", UriKind.Relative));
                        break;

                    }
                }
            }
            catch
            {
            }
        }


        protected async void LoadMap()
        {
           // this.LoadMapCurrent();
            //*** Map
           // Map MyMap = new Map();
           // MyMap.ZoomLevel = 16;
            mymap.Layers.Clear();
            MapLayer mapLayer = new MapLayer();
           // MyCoordinates.Clear(); 
            MapLayer layer = new MapLayer();

         //   responetext = "[{\"id\":\"1635\",\"stop_name\":\"\u0e40\u0e14\u0e2d\u0e27\u0e34\u0e25\u0e40\u0e25\u0e08\u0e2d\u0e34\u0e19\u0e17\u0e32\u0e27\u0e2a\u0e4c\",\"latitude\":\"13.77179585\",\"longitude\":\"100.60690929\",\"distance\":\"0.916916395966528\",\"busline\":\"36\u0e01,154\"}]";//,{"id":"1634","stop_name":"\u0e17\u0e32\u0e27\u0e4c\u0e2d\u0e34\u0e19\u0e17\u0e32\u0e27\u0e4c","latitude":"13.7750791877082","longitude":"100.607224823679","distance":"1.03013728936038","busline":"36\u0e01,154"},{"id":"1192","stop_name":"\u0e15\u0e23\u0e07\u0e02\u0e49\u0e32\u0e21\u0e40\u0e17\u0e1e\u0e25\u0e35\u0e25\u0e32 \u0e0b\u0e2d\u0e22 8","latitude":"13.76218101692503","longitude":"100.6087379003672","distance":"1.11313062039173","busline":"36\u0e01"},{"id":"1193","stop_name":"\u0e15\u0e23\u0e07\u0e02\u0e49\u0e32\u0e21\u0e40\u0e17\u0e1e\u0e25\u0e35\u0e25\u0e32 \u0e0b\u0e2d\u0e22 2","latitude":"13.76046028001318","longitude":"100.6118853550142","distance":"1.11776957041667","busline":""},{"id":"1171","stop_name":"\u0e0a\u0e38\u0e21\u0e0a\u0e19\u0e23\u0e38\u0e48\u0e07\u0e21\u0e13\u0e35\u0e1e\u0e31\u0e12\u0e19\u0e32","latitude":"13.76063212036984","longitude":"100.6113020987868","distance":"1.12172210251276","busline":""},{"id":"1173","stop_name":"\u0e27\u0e34\u0e17\u0e22\u0e32\u0e25\u0e31\u0e22\u0e2d\u0e34\u0e19\u0e17\u0e23\u0e32\u0e0a\u0e31\u0e22","latitude":"13.76275395290122","longitude":"100.6078930245248","distance":"1.12609068831856","busline":""},{"id":"1645","stop_name":"\u0e1b\u0e49\u0e32\u0e22\u0e23\u0e16\u0e1b\u0e23\u0e30\u0e08\u0e33\u0e17\u0e32\u0e07\u0e40\u0e25\u0e35\u0e22\u0e1a\u0e17\u0e32\u0e07\u0e14\u0e48\u0e27\u0e19 3","latitude":"13.7757246956735","longitude":"100.606590634465","distance":"1.12703041992614","busline":"36\u0e01,154"},{"id":"1172","stop_name":"\u0e2b\u0e21\u0e39\u0e01\u0e23\u0e30\u0e17\u0e30","latitude":"13.76132017559768","longitude":"100.6096244208446","distance":"1.13580294658066","busline":""},{"id":"1644","stop_name":"\u0e0b\u0e2d\u0e22\u0e2b\u0e21\u0e39\u0e48\u0e1a\u0e49\u0e32\u0e19\u0e01\u0e25\u0e32\u0e07\u0e40\u0e21\u0e37\u0e2d\u0e07","latitude":"13.77057937","longitude":"100.6046747","distance":"1.13780317156702","busline":"36\u0e01,154"},{"id":"1191","stop_name":"\u0e0a\u0e38\u0e21\u0e0a\u0e19\u0e17\u0e23\u0e31\u0e1e\u0e22\u0e4c\u0e2a\u0e34\u0e19\u0e43\u0e2b\u0e21\u0e48","latitude":"13.76426018263352","longitude":"100.6062342642443","distance":"1.15815017620203","busline":"36\u0e01"},{"id":"1174","stop_name":"\u0e2a\u0e20\u0e32\u0e27\u0e34\u0e28\u0e27\u0e01\u0e23","latitude":"13.7639788756759","longitude":"100.606349401346","distance":"1.16542427717435","busline":""},{"id":"1643","stop_name":"\u0e1b\u0e49\u0e32\u0e22\u0e23\u0e16\u0e1b\u0e23\u0e30\u0e08\u0e33\u0e17\u0e32\u0e07\u0e40\u0e25\u0e35\u0e22\u0e1a\u0e17\u0e32\u0e07\u0e14\u0e48\u0e27\u0e19 2","latitude":"13.76989470501","longitude":"100.60428335765","distance":"1.17819609959488","busline":"154"},{"id":"4790","stop_name":"\u0e21\u0e2b\u0e32\u0e27\u0e34\u0e17\u0e22\u0e32\u0e25\u0e31\u0e22\u0e23\u0e32\u0e21\u0e04\u0e33\u0e41\u0e2b\u0e07","latitude":"13.7597154249846","longitude":"100.618682065543","distance":"1.2025881326274","busline":"93,22,36\u0e01,40,58,60,71,109,207,113,501,115,122,126,137"},{"id":"1190","stop_name":"\u0e1a\u0e23\u0e34\u0e29\u0e31\u0e17\u0e2b\u0e34\u0e19\u0e2d\u0e48\u0e2d\u0e19","latitude":"13.76532608719498","longitude":"100.6049483673126","distance":"1.22165682665223","busline":"36\u0e01"},{"id":"1175","stop_name":"\u0e1a\u0e23\u0e34\u0e29\u0e31\u0e17\u0e2b\u0e34\u0e19\u0e2d\u0e48\u0e2d\u0e19","latitude":"13.76514197731848","longitude":"100.6049766783524","distance":"1.227730994933","busline":""},{"id":"3999","stop_name":"\u0e0b\u0e2d\u0e22\u0e25\u0e32\u0e14\u0e1e\u0e23\u0e49\u0e32\u0e27 114, \u0e42\u0e23\u0e07\u0e40\u0e23\u0e35\u0e22\u0e19\u0e1a\u0e32\u0e07\u0e01\u0e2d\u0e01\u0e28\u0e36\u0e01\u0e29\u0e32","latitude":"13.78002699693741","longitude":"100.6200119420173","distance":"1.23190962702237","busline":"8,502,178,44,73\u0e01,151,514,122,126,137,145,191,156,96,172,27"},{"id":"1188","stop_name":"\u0e23\u0e49\u0e32\u0e19\u0e2d\u0e32\u0e2b\u0e32\u0e23\u0e1a\u0e49\u0e32\u0e19\u0e15\u0e49\u0e19\u0e0b\u0e38\u0e07","latitude":"13.76885944","longitude":"100.60377743","distance":"1.23913950108282","busline":"154"}]";

            if (responetext != "")
            {
                string strJSON = null;
                strJSON = responetext;
                MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(strJSON));
                ObservableCollection<Member> list = new ObservableCollection<Member>();
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(ObservableCollection<Member>));
                list = (ObservableCollection<Member>)serializer.ReadObject(ms);

                List<Member> myMember = new List<Member>();

                foreach (Member cm in list)
                {
                   // Pushpin pushpin = new Pushpin();
                  //  pushpin.GeoCoordinate = new GeoCoordinate(cm.latitude, cm.longitude);
                  //  var uriString = @"Assets/pin_blue.png";
                  //  pushpin.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(uriString, UriKind.Relative)) };
                  //  pushpin.Width = 40;
                  //  pushpin.Height = 31;

                    MapOverlay overlay = new MapOverlay();
                    overlay.Content = _tooltip;
                    _tooltip.Description = cm.stop_name;
                    overlay.GeoCoordinate = new GeoCoordinate(cm.latitude, cm.longitude);
                    layer.Add(overlay);
                }

                // Map Layer
                mymap.Layers.Add(layer);


                MapOverlay mylocationOverlay = new MapOverlay();
                mylocationOverlay.Content = _Pushpin;
                mylocationOverlay.GeoCoordinate = currentLocation;
                MapLayer myLocationLayer = new MapLayer();
                myLocationLayer.Add(mylocationOverlay);
                mymap.Layers.Add(myLocationLayer);
                mymap.Center = currentLocation;

            }
            else
            {
                MapOverlay mylocationOverlay = new MapOverlay();
                mylocationOverlay.Content = _Pushpin;
                mylocationOverlay.GeoCoordinate = currentLocation;
                MapLayer myLocationLayer = new MapLayer();
                myLocationLayer.Add(mylocationOverlay);
                mymap.Layers.Add(myLocationLayer);
                mymap.Center = currentLocation;
            }
            
        }
     
        private void btSearchAd_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_Search_Advance_busstop.xaml", UriKind.Relative));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_BusStop.xaml", UriKind.Relative));
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_bus_line.xaml", UriKind.Relative));
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_BusCoordinates.xaml", UriKind.Relative));
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
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

        private void showbusstop()
        {

        }

        private static XDocument CreateSoapEnvelope(string content)
        {
            XDocument soapEnvelopeXml = XDocument.Parse(content);
            return soapEnvelopeXml;
        }

        [DataContract]
        public class Member
        {
            [DataMember]
            public string dxml { get; set; }
            [DataMember]
            public int id { get; set; }
            [DataMember]
            public string stop_name { get; set; }
            [DataMember]
            public double longitude { get; set; }
            [DataMember]
            public double latitude { get; set; }
            [DataMember]
            public string distance { get; set; }
            [DataMember]
            public string busline { get; set; }



            public Member(string strName)
            {
                this.stop_name = strName;
            }

            public Member()
            {
                // TODO: Complete member initialization
            }


        }

        private async void Button_Click_5(object sender, RoutedEventArgs e)
        {
           ContentPanelp.Visibility = Visibility;
           getNearBusStop();
           LoadAvalableParks();
           ContentPanelp.Visibility = System.Windows.Visibility.Collapsed;

        }

        public class LocationDetail
        {
            public string id { get; set; }
            public string stop_name { get; set; }
            public string latitude { get; set; }
            public string longitude { get; set; }
            public string _number;
            public string busline {get;set;}
            public string distance 
            {
                get
                {
                    return this._number;
                }
                set
                {
                    if (value != null)
                    {

                        this._number = value + " " + "";
                    }
                    else
                    {
                        this._number = value;
                    }
                }
            }
      

        }

        public class LocationList : List<LocationDetail>
        {
        }

        //private async void dataws(object sender, KeyEventArgs e)
        //{
        //    itemx.Clear();s
        //    if (acBox.Text.Length > 3 && acBox.Text != "")
        //    {
        //        try
        //        {
        //            string gq = acBox.Text;
        //            using (HttpClient client = new HttpClient())
        //            {
        //                client.BaseAddress = new Uri("http://202.6.18.31/bmta/webservice/keyword.php");

        //                var url = "?type=busstop&q={0}";
        //                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        //                HttpResponseMessage response = await client.GetAsync(String.Format(url, gq));

        //                if (response.IsSuccessStatusCode)
        //                {
        //                    var data = response.Content.ReadAsStringAsync();

        //                    var weatherdata = JsonConvert.DeserializeObject<WeatherObject>(data.Result.ToString());

        //                    string sd = data.Result.ToString();
        //                    sd = sd.Replace("{\"status\":1,\"data\":", "");
        //                    sd = sd.Replace("]}", "]");


        //                    string strJSON = null;
        //                    strJSON = sd;
        //                    MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(strJSON));
        //                    ObservableCollection<landmarkObject> list = new ObservableCollection<landmarkObject>();
        //                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(ObservableCollection<landmarkObject>));
        //                    list = (ObservableCollection<landmarkObject>)serializer.ReadObject(ms);

        //                    List<landmarkObject> myMember = new List<landmarkObject>();
                            
        //                    foreach (var item in list)
        //                    {

        //                        try
        //                        {
        //                            landmarkObject c = new landmarkObject();
        //                            c.id = item.id;
        //                            c.stop_name = item.stop_name;
        //                            itemx.Add(c);
        //                        }
        //                        catch (Exception ex)
        //                        {

        //                        }
        //                    }
        //                    this.acBox.ItemsSource = itemx;
        //                }
        //            }
        //        }

        //        catch (Exception ex)
        //        {
        //            // MessageBox.Show("ไม่พบข้อมูลที่ค้นหาต้นทาง");
        //        }
        //    }
        //}


        public class WP7Phone
        {

            public string Image
            {
                get;
                set;
            }

            public string Name
            {
                get;
                set;
            }
        }

        public class landmarkObject
        {
            public int id { get; set; }
            public string stop_name { get; set; }
        }
        public static List<landmarkObject> itemx = new List<landmarkObject>();

        public class WeatherObject
        {
            public int id { get; set; }
            public string stop_name { get; set; }
        }
    }
}