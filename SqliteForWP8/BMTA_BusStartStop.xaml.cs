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
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections; // for 'IEnumerable'
using System.Threading;


namespace BMTA
{
    public partial class BMTA_BusStartStop : PhoneApplicationPage
    {
        string bstartid = "";
        string bstopid = "";
        string responetext = "";
        public BMTA_BusStartStop()
        {
            InitializeComponent();
            // createRandomItemSource();

        }

        bool SearchPhones(string search, object value)
        {
            if (value != null)
            {
                WP7Phone datasourceValue = value as WP7Phone;
                string name = datasourceValue.stop_name;

                if (name.ToLower().StartsWith(search.ToLower()))
                    return true;
            }
            //... If no match, return false. 
            return false;
        }
        public class WP7Phone
        {

            public string id
            {
                get;
                set;
            }

            public string stop_name
            {
                get;
                set;
            }
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            rightmenu.Visibility = System.Windows.Visibility.Collapsed;
            rightmenux.Visibility = System.Windows.Visibility.Collapsed;
            close.Visibility = System.Windows.Visibility.Collapsed;
            ContentPanel.Visibility = System.Windows.Visibility.Collapsed;
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
                        ImageSource = new System.Windows.Media.Imaging.BitmapImage(new Uri("/Assets/480x852/BMTA_BusStart_Stop_bg.png", UriKind.Relative)),
                        Opacity = 1d
                    };
                    this.LayoutRoot.Background = brush;
                    brush.Stretch = Stretch.Fill;
                }
                else if (x == "720")
                {
                    ImageBrush brush = new ImageBrush
                    {
                        ImageSource = new System.Windows.Media.Imaging.BitmapImage(new Uri("/Assets/720x1280/BMTA_BusStart_Stop_bg.png", UriKind.Relative)),
                        Opacity = 1d
                    };
                    this.LayoutRoot.Background = brush;
                    brush.Stretch = Stretch.Fill;
                }
                else
                {
                    ImageBrush brush = new ImageBrush
                    {
                        ImageSource = new System.Windows.Media.Imaging.BitmapImage(new Uri("/Assets/768x1280/BMTA_BusStart_Stop_bg.png", UriKind.Relative)),
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

        private void btSearchAd_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_Search_Advance_start.xaml", UriKind.Relative));
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_bus_line.xaml", UriKind.Relative));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_BusStop.xaml", UriKind.Relative));
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

        private void btSearch_Click(object sender, RoutedEventArgs e)
        {


            ContentPanel.Visibility = Visibility;

            // getsearchfindRouting();
            DoUIThings();

            ContentPanel.Visibility = System.Windows.Visibility.Collapsed;

        }




        public void getsearchfindRouting()
        {
            string url = "http://202.6.18.31/bmta/webservice/bmta_service_bo.php/";
            HttpWebRequest request = WebRequest.CreateHttp(new Uri(url)) as HttpWebRequest;

            string data = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
            "<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\n" +
            "xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"\n" +
            "xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">\n" +
            "<soap:Body>\n" +
            "<searchfindRouting xmlns=\"http://202.6.18.31/bmta/webservice/bmta_service_bo.php\">\n" +
            "<busstop_start_id>" + bstartid + "</busstop_start_id>\n" +
            "<busstop_end_id>" + bstopid + "</busstop_end_id>\n" +
            "<bus_type></bus_type>\n" +
            "<running_type></running_type>\n" +
            "<orderby></orderby>\n" +
            "</searchfindRouting>\n" +
            "</soap:Body>\n" +
            "</soap:Envelope>";

            request.ContentType = "text/xml; charset=utf-8";// "application/x-www-form-urlencoded";
            request.UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0; Touch)";
            request.Headers["SOAPAction"] = "http://202.6.18.31/bmta/webservice/bmta_service.php?wsdl";
            // Set the Method property to 'POST' to post data to the URI.
            request.Method = "POST";
            request.ContentLength = data.Length;
            // start the asynchronous operation
            ContentPanel.Visibility = Visibility;
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
               "<searchfindRouting xmlns=\"http://202.6.18.31/bmta/webservice/bmta_service.php\">\n" +
               "<busstop_start_id>" + bstartid + "</busstop_start_id>\n" +
               "<busstop_end_id>" + bstopid + "</busstop_end_id>\n" +
               "<bus_type></bus_type>\n" +
               "<running_type></running_type>\n" +
               "<orderby></orderby>\n" +
               "</searchfindRouting>\n" +
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
                    //  sd = sd.Replace("{\"status\":\"1\",\"data\":", "");
                    //  sd = sd.Replace("]}", "]");

                    responetext = sd;

                    // Close the stream object
                    streamResponse.Close();
                    streamRead.Close();

                    // Release the HttpWebResponse
                    response.Close();
                    //allDone.Set();

                }
            }
            catch (WebException ex)
            {
                using (StreamReader reader = new StreamReader(ex.Response.GetResponseStream()))
                {
                    Debug.WriteLine("Exception output : " + ex);
                }
            }

            ContinueDoUIThings();
        }

        public void DoUIThings()
        {
            // Do some UI related things.
            getsearchfindRouting();
            // Don't continue doing things here.... Wait for the ContinueDoUIThings() to be called.
        }

        public void ContinueDoUIThings()
        {
            if (responetext == "")
            {
                MessageBox.Show("ไม่พบข้อมูลที่ค้นหา");
            }
            else if (responetext == "{\"routing\":{\"status\":0}}")
            {
                MessageBox.Show("ไม่พบข้อมูลที่ค้นหา");
            }
            else
            {
                string strJSON = null;
                strJSON = responetext;


                string json = strJSON;// @"{ ""names"" : [ {""name"":""bla""} , {""name"":""bla2""} ] }";

                var dict = (JObject)JsonConvert.DeserializeObject(json);
                List<Routings> newsResults = new List<Routings>();
                foreach (var objx in dict["data"])
                {
                    // foreach (var sx in objx["routing"])
                    for (int i = 0; i < objx["routing"].Count(); i++)
                    {
                      //  newsResults.Add(new Routings { bus_line = objx["routing"][i]["bus_line"].ToString() });
                        // string row = sx.ToString();
                    }
                }



                Dish addressMap = JsonConvert.DeserializeObject<Dish>(json);
                //  var j = JsonConvert.DeserializeObject<SmallestDotNetThing>(json);
                // var sa = addressMap.bus_line;

                JObject obj = JObject.Parse(strJSON);
                List<Routings> Dishes_1 = new List<Routings>();
                List<JToken> jdata = obj.SelectTokens("data[*].routing").ToList();
                for (int i = 0; i < jdata.Count(); i++)
                {
                    Dishes_1.Add(new Routings { routing = jdata[i].ToString() });
                }

                var articles = new List<DotNetVersion>();
                for (int x = 0; x < Dishes_1.Count; x++)
                {
                    string strLoop = null;
                    strLoop = Dishes_1[x].routing;
                    MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(strLoop));
                    ObservableCollection<Dish> LocationListobj = new ObservableCollection<Dish>();
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(ObservableCollection<Dish>));
                    LocationListobj = (ObservableCollection<Dish>)serializer.ReadObject(ms);
                    
                    foreach (var t in LocationListobj)
                    {
                        DotNetVersion article = new DotNetVersion() { bus_line = t.bus_line, bus_polyline = t.bus_polyline, busline_coordinates = t.busline_coordinates, busline_id = t.busline_id, busline_name = t.busline_name };
                        articles.Add(article);
                    }
                    Mainpivot.DataContext = articles;
                }
               

                //foreach (JToken result in jdata)
                //{
                    //Console.WriteLine(result.ToString());
                    // newsResults.Add(new Routings { bus_line = result.Children()["bus_line"].Value<string>() });

                    //  Dishes_1.Add(new Dish { busline_id = result.SelectToken("busline_id").ToList() });
              //  }

                //foreach (JToken result in jdata)
                //{
                //    Routings searchResult = JsonConvert.DeserializeObject<Routings>(result.ToString());

                //}

                //for (int i = 0; i < jdata.Count; i++)
                //{
                //   // var tracknames = jdata[i].Children()["bus_line"].Values<string>();

                //    foreach (var pair in jdata[i])
                //    {

                //    }
                //}
            }
        }

        public class SmallestDotNetThing
        {
            public DotNetVersion data { get; set; }
            public List<DotNetVersion> routing { get; set; }
            //   public List<DotNetVersion> routing { get; set; }
        }

        public class DotNetVersion
        {
            public string busline_id { get; set; }
            public string busline_name { get; set; }
            public string bus_line { get; set; }
            public string bustation_id_start { get; set; }
            public string bustation_id_end { get; set; }
            public string busline_coordinates { get; set; }
            public string bus_polyline { get; set; }
            public string price { get; set; }
            public string distance { get; set; }
            public string type { get; set; }
            public string route_type { get; set; }
            public string exchange_point { get; set; }
            public string busstop { get; set; }

        }


        private async void dataws(object sender, KeyEventArgs e)
        {
            items.Clear();
            if (acBox.Text.Length > 3 && acBox.Text != "")
            {
                try
                {
                    string gq = acBox.Text;
                    using (HttpClient client = new HttpClient())
                    {
                        client.BaseAddress = new Uri("http://202.6.18.31/bmta/webservice/keyword.php");

                        var url = "?type=busstop&q={0}";
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        HttpResponseMessage response = await client.GetAsync(String.Format(url, gq));

                        if (response.IsSuccessStatusCode)
                        {
                            var data = response.Content.ReadAsStringAsync();

                            var weatherdata = JsonConvert.DeserializeObject<WeatherObject>(data.Result.ToString());

                            string sd = data.Result.ToString();
                            sd = sd.Replace("{\"status\":1,\"data\":", "");
                            sd = sd.Replace("]}", "]");


                            string strJSON = null;
                            strJSON = sd;
                            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(strJSON));
                            ObservableCollection<landmarkObject> list = new ObservableCollection<landmarkObject>();
                            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(ObservableCollection<landmarkObject>));
                            list = (ObservableCollection<landmarkObject>)serializer.ReadObject(ms);

                            List<landmarkObject> myMember = new List<landmarkObject>();

                            foreach (var item in list)
                            {

                                try
                                {
                                    landmarkObject c = new landmarkObject();
                                    c.id = item.id;
                                    c.stop_name = item.stop_name;
                                    items.Add(c);
                                }
                                catch (Exception ex)
                                {

                                }
                            }
                            this.acBox.ItemsSource = items;
                        }
                    }
                }

                catch (Exception ex)
                {
                    // MessageBox.Show("ไม่พบข้อมูลที่ค้นหาต้นทาง");
                }
            }
        }

        private async void datawse(object sender, KeyEventArgs e)
        {
            itemse.Clear();
            if (acBoxe.Text.Length > 3 && acBoxe.Text != "")
            {
                try
                {
                    string gq = acBoxe.Text;
                    using (HttpClient client = new HttpClient())
                    {
                        client.BaseAddress = new Uri("http://202.6.18.31/bmta/webservice/keyword.php");

                        var url = "?type=busstop&q={0}";
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        HttpResponseMessage response = await client.GetAsync(String.Format(url, gq));

                        if (response.IsSuccessStatusCode)
                        {
                            var data = response.Content.ReadAsStringAsync();

                            var weatherdata = JsonConvert.DeserializeObject<WeatherObject>(data.Result.ToString());

                            string sd = data.Result.ToString();
                            sd = sd.Replace("{\"status\":1,\"data\":", "");
                            sd = sd.Replace("]}", "]");


                            string strJSON = null;
                            strJSON = sd;
                            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(strJSON));
                            ObservableCollection<landmarkObjecte> liste = new ObservableCollection<landmarkObjecte>();
                            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(ObservableCollection<landmarkObjecte>));
                            liste = (ObservableCollection<landmarkObjecte>)serializer.ReadObject(ms);

                            List<landmarkObjecte> myMember = new List<landmarkObjecte>();
                            itemse.Clear();
                            foreach (var iteme in liste)
                            {

                                try
                                {
                                    landmarkObjecte c = new landmarkObjecte();
                                    c.id = iteme.id;
                                    c.stop_name = iteme.stop_name;
                                    itemse.Add(c);
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message);
                                }
                            }
                            this.acBoxe.ItemsSource = itemse;
                        }
                    }
                }

                catch (Exception ex)
                {
                    // MessageBox.Show("ไม่พบข้อมูลที่ค้นหาต้นทาง");
                }
            }
        }

        public class Menu
        {
            public string Name { get; set; }
            public List<Dish> Dishes_1 { get; set; }
            public List<Dish> Dishes_2 { get; set; }
            public List<Dish> Dishes_3 { get; set; }
            public List<Dish> Dishes_4 { get; set; }
            public List<Dish> Dishes_5 { get; set; }
            public List<Dish> Dishes_6 { get; set; }
            public List<Dish> Dishes_7 { get; set; }
            public List<Dish> Dishes_8 { get; set; }
            public List<Dish> Dishes_9 { get; set; }
            public List<Dish> Dishes_10 { get; set; }
        }

        public class Dish
        {
            public string busline_id { get; set; }
            public string busline_name { get; set; }
            public string bus_line { get; set; }
            public string bustation_id_start { get; set; }
            public string bustation_id_end { get; set; }
            public string busline_coordinates { get; set; }
            public string bus_polyline { get; set; }
            public string price { get; set; }
            public string distance { get; set; }
            public string type { get; set; }
            public string route_type { get; set; }
            public string exchange_point { get; set; }
            public string busstop { get; set; }
        }

        public class WeatherObject
        {
            public int id { get; set; }
            public string stop_name { get; set; }
        }

        public class landmarkObject
        {
            public int id { get; set; }
            public string stop_name { get; set; }
        }
        public class landmarkObjecte
        {
            public int id { get; set; }
            public string stop_name { get; set; }
        }
        public static List<landmarkObject> items = new List<landmarkObject>();
        public static List<landmarkObjecte> itemse = new List<landmarkObjecte>();

        public class Routings
        {
         public string routing { get; set; }
        }

        public class LocationDetail
        {
            public string id { get; set; }
            public string stop_name { get; set; }
            public string latitude { get; set; }
            public string longitude { get; set; }
            public string _number;
            public string busline { get; set; }
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

        private void acbstop_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (acb_stop.SelectedItem != null)
            //{
            //    // selectedText.Text = acb.SelectedItem.ToString();
            //    // searchText.Text = acb.SearchText;
            //}


            var selectitemac = items.Where(sp => sp.stop_name == acBox.Text);
            foreach (var item in selectitemac)
            {
                // MessageBox.Show(item.id.ToString());
                bstartid = item.id.ToString();
            }
            var selectitemace = itemse.Where(st => st.stop_name == acBoxe.Text);
            foreach (var iteme in selectitemace)
            {
                // MessageBox.Show(iteme.id.ToString());
                bstopid = iteme.id.ToString();
            }
        }

        private void acb_GotFocus(object sender, RoutedEventArgs e)
        {
            // acb_start.Text = "";
        }


        List<string> textList = new List<string>();

        private void createRandomItemSource()
        {
            for (int i = 0; i < 50; i++)
            {
                // textList.Add(getRandomText());
            }

            // acb_start.ItemsSource = textList;
            // acb_stop.ItemsSource = textList;
        }


        Random random = new Random();
        private async void getRandomText()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://202.6.18.31/bmta/webservice/keyword.php");

                    var url = "?type=busstop&q={0}";
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.GetAsync(String.Format(url, acBox.Text));

                    if (response.IsSuccessStatusCode)
                    {
                        var data = response.Content.ReadAsStringAsync();

                        var weatherdata = JsonConvert.DeserializeObject<WeatherObject>(data.Result.ToString());

                        string sd = data.Result.ToString();
                        sd = sd.Replace("{\"status\":1,\"data\":", "");
                        sd = sd.Replace("]}", "]");


                        string strJSON = null;
                        strJSON = sd;
                        MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(strJSON));
                        ObservableCollection<landmarkObject> list = new ObservableCollection<landmarkObject>();
                        DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(ObservableCollection<landmarkObject>));
                        list = (ObservableCollection<landmarkObject>)serializer.ReadObject(ms);

                        List<landmarkObject> myMember = new List<landmarkObject>();
                        foreach (var item in list)
                        {

                            try
                            {
                                landmarkObject c = new landmarkObject();
                                c.id = item.id;
                                c.stop_name = item.stop_name;
                                items.Add(c);
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                        this.acBox.ItemsSource = items;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ไม่พบข้อมูลที่ค้นหาต้นทาง");
            }


        }


    }
}