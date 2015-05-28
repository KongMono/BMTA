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
using BMTA.BMTAws;
using System.Diagnostics;


namespace BMTA
{
    public partial class BMTA_BusStop : PhoneApplicationPage
    {
        public BMTA_BusStop()
        {
            InitializeComponent();
        
        }

        WebClient client;
        ProgressIndicator prog;

        GeoCoordinateWatcher watcher;


        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
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

                LoadMap();
                LoadCurrentLocation();

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

        private void LoadCurrentLocation()
        {
            if (watcher == null)
            {
                watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.Default);
                watcher.MovementThreshold = 20;
                watcher.StatusChanged += new EventHandler<GeoPositionStatusChangedEventArgs>(watcher_StatusChanged);
                watcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(watcher_PositionChanged);

            }
            watcher.Start();
        }

        void watcher_StatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
        {
            switch (e.Status)
            {
                case GeoPositionStatus.Disabled:
                    MessageBox.Show("Location Service is not enabled on the device");
                    break;

                case GeoPositionStatus.NoData:
                    MessageBox.Show(" The Location Service is working, but it cannot get location data.");
                    break;
            }
        }

        void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            if (e.Position.Location.IsUnknown)
            {
                this.notification.Text = "Please wait while your prosition is determined....";
                return;
            }
            lat.Text = e.Position.Location.Latitude.ToString();
            lon.Text = e.Position.Location.Longitude.ToString();
        }

        protected async void LoadMap()
        {

            string url = "http://202.6.18.31/bmta/webservice/server.php/?method=getNearBusStop";


            HttpWebRequest request = WebRequest.CreateHttp(new Uri(url)) as HttpWebRequest;
            request.AllowReadStreamBuffering = true;
            string strsoaprequestbody = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
                "<soap-env:envelope xmlns:soap-env=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:soap-enc=\"http://schemas.xmlsoap.org/soap/encoding/\" xmlns:xsi=\"http://www.w3.org/2001/xmlschema-instance\" xmlns:xsd=\"http://www.w3.org/2001/xmlschema\" soap-env:encodingstyle=\"http://schemas.xmlsoap.org/soap/encoding/\">\n" +
                "<soap-env:body>\n" +
                "<dologin xmlns=\"http://login.mss.uks.com\">\n" +
                "<username xsi:type=\"xsd:string\">userID</username>\n" +
                "<password xsi:type=\"xsd:string\">password</password>\n" +
                "<app_id xsi:type=\"xsd:int\">2</app_id>\n" +
                "</dologin>" +
                "</soap-env:body>\n" +
                "</soap-env:envelope>\n";


            request.ContentType = "application/x-www-form-urlencoded";
            request.UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0; Touch)";
            request.Headers["SOAPAction"] = "http://schemas.xmlsoap.org/soap/encoding/";
            // Set the Method property to 'POST' to post data to the URI.
            request.Method = "POST";
            request.ContentLength = strsoaprequestbody.Length;
            // start the asynchronous operation
            request.BeginGetRequestStream(new AsyncCallback(GetRequestStreamCallback), request);


            //string url = "http://202.6.18.31/bmta/webservice/server.php/getNearBusStop";
            //Uri uri = new Uri(url);
            //client = new WebClient();
            //client.AllowReadStreamBuffering = true;
            //client.DownloadStringCompleted += client_DownloadStringCompleted;
            //client.DownloadProgressChanged += client_DownloadProgressChanged;
            //client.DownloadStringAsync(uri);

            //*** SystemTray ProgressBar ***'
            prog = new ProgressIndicator();
            prog.IsVisible = true;
            prog.IsIndeterminate = true;
            prog.Text = "Downloading....";
            SystemTray.SetProgressIndicator(this, prog);

           // clientProxy.Endpoint.Address = new EndpointAddress(uri);
            //creating the reference to the service
          //  BMTAws.BMTAPortTypeClient ws = new BMTAws.BMTAPortTypeClient();
          //  ws.OpenAsync();


            // this.LoadMapCurrent();
            //*** Map
            Map MyMap = new Map();
            MyMap.ZoomLevel = 18;

            MapLayer layer = new MapLayer();

                //string strJSON = null;
                //strJSON = "";
                //MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(strJSON));
                //ObservableCollection<Member> list = new ObservableCollection<Member>();
                //DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(ObservableCollection<Member>));
                //list = (ObservableCollection<Member>)serializer.ReadObject(ms);

                //List<Member> myMember = new List<Member>();

                //foreach (Member cm in list)
                //{
                //    Pushpin pushpin = new Pushpin();
                //    pushpin.GeoCoordinate = new GeoCoordinate(cm.lat, cm.Long);
                //    var uriString = @"Assets/btn_bus.png";
                //    pushpin.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(uriString, UriKind.Relative)) };
                //    pushpin.Width = 40;
                //    pushpin.Height = 31;
                //    MapOverlay overlay = new MapOverlay();
                //    overlay.Content = pushpin;
                //    overlay.GeoCoordinate = new GeoCoordinate(cm.lat, cm.Long);
                //    layer.Add(overlay);
                //}
           
               // // Map Layer
               // MyMap.Layers.Add(layer);


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


        private void GetRequestStreamCallback(IAsyncResult asynchronousResult)
        {
            {
                Debug.WriteLine("GetRequestStreamCallback method called....");
                HttpWebRequest request = (HttpWebRequest)asynchronousResult.AsyncState;

                // End the operation
                Stream postStream = request.EndGetRequestStream(asynchronousResult);

                string postData = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
                                    "<SOAP-ENV:Envelope xmlns:SOAP-ENV=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:SOAP-ENC=\"http://schemas.xmlsoap.org/soap/encoding/\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" SOAP-ENV:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">\n" +
                                    "<SOAP-ENV:Body>\n" +
                                    "<doLogin xmlns=\"http://login.mss.uks.com\">\n" +
                                    "<username xsi:type=\"xsd:string\">userID</username>\n" +
                                    "<password xsi:type=\"xsd:string\">password</password>\n" +
                                    "<app_id xsi:type=\"xsd:int\">2</app_id>\n" +
                                    "</doLogin>" +
                                    "</SOAP-ENV:Body>\n" +
                                    "</SOAP-ENV:Envelope>\n";

                // Convert the string into a byte array. 
                byte[] byteArray = Encoding.UTF8.GetBytes(postData);

                // Write to the request stream.
                postStream.Write(byteArray, 0, postData.Length);
                postStream.Close();

                // Start the asynchronous operation to get the response
                request.BeginGetResponse(new AsyncCallback(GetResponseCallback), request);
            }

        }

        private static void GetResponseCallback(IAsyncResult asynchronousResult)
        {
            Debug.WriteLine("GetResponseCallback method called....");

            try
            {
                HttpWebRequest request = (HttpWebRequest)asynchronousResult.AsyncState;

                // End the operation
                HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(asynchronousResult);
                Stream streamResponse = response.GetResponseStream();
                StreamReader streamRead = new StreamReader(streamResponse);
                string responseString = streamRead.ReadToEnd();
                //display the web response
                Debug.WriteLine("Response String : " + responseString);
                // Close the stream object
                streamResponse.Close();
                streamRead.Close();

                // Release the HttpWebResponse
                response.Close();
            }
            catch (WebException ex)
            {
                using (StreamReader reader = new StreamReader(ex.Response.GetResponseStream()))
                {
                    Debug.WriteLine("Exception output : " + ex);
                }
            }
        }


        private void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            //*** Map
            Map MyMap = new Map();
            MyMap.ZoomLevel = 18;

            MapLayer layer = new MapLayer();


            if (e.Cancelled == false & e.Error == null)
            {
                MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(e.Result));
                ObservableCollection<Member> list = new ObservableCollection<Member>();
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(ObservableCollection<Member>));
                list = (ObservableCollection<Member>)serializer.ReadObject(ms);

                List<Member> myCustomer = new List<Member>();

                foreach (Member cm in list)
                {
                    Pushpin pushpin = new Pushpin();
                    pushpin.GeoCoordinate = new GeoCoordinate(cm.lat, cm.Long);
                    var uriString = @"Assets/btn_bus.png";
                    pushpin.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(uriString, UriKind.Relative)) };
                    pushpin.Width = 40;
                    pushpin.Height = 31;
                    MapOverlay overlay = new MapOverlay();
                    overlay.Content = pushpin;
                    overlay.GeoCoordinate = new GeoCoordinate(cm.lat, cm.Long);
                    layer.Add(overlay);
                }

                //this.CustomerList.ItemsSource = myCustomer;

                prog.IsVisible = false;
            }

        }

        private void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {

        }

        [DataContract]
        public class Member
        {
            [DataMember]
            public double lat { get; set; }

            [DataMember]
            public double Long { get; set; }

            [DataMember]
            public string distance { get; set; }

        }

        private void btSearchAd_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_BusStop_SearchAdvance.xaml", UriKind.Relative));
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


    }
}