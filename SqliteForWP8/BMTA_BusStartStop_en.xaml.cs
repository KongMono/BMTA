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
namespace BMTA
{
    public partial class BMTA_BusStartStop_en : PhoneApplicationPage
    {
        public BMTA_BusStartStop_en()
        {
            InitializeComponent();

        }

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
                        ImageSource = new System.Windows.Media.Imaging.BitmapImage(new Uri("/Assets/480x852/BMTA_BusStart_Stop_bg_en.png", UriKind.Relative)),
                        Opacity = 1d
                    };
                    this.LayoutRoot.Background = brush;
                    brush.Stretch = Stretch.Fill;
                }
                else if (x == "720")
                {
                    ImageBrush brush = new ImageBrush
                    {
                        ImageSource = new System.Windows.Media.Imaging.BitmapImage(new Uri("/Assets/720x1280/BMTA_BusStart_Stop_bg_en.png", UriKind.Relative)),
                        Opacity = 1d
                    };
                    this.LayoutRoot.Background = brush;
                    brush.Stretch = Stretch.Fill;
                }
                else
                {
                    ImageBrush brush = new ImageBrush
                    {
                        ImageSource = new System.Windows.Media.Imaging.BitmapImage(new Uri("/Assets/768x1280/BMTA_BusStart_Stop_bg_en.png", UriKind.Relative)),
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
            NavigationService.Navigate(new Uri("/BMTA_Search_Advance_start_en.xaml", UriKind.Relative));
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_bus_line_en.xaml", UriKind.Relative));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_BusStop_en.xaml", UriKind.Relative));
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

        private void btSearch_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("กดหาพ่องเมิงเหรอ แสดดดดดด");
        }

        private async void txtStart_KeyDown(object sender, KeyEventArgs e)
        {
            if (txtStart.Text.Length >= 10)
            {
                //  HttpWebRequest hwr = WebRequest.Create(new Uri("http://202.6.18.31/bmta/webservice/keyword.php?type=busstop&q=")) as HttpWebRequest;
                //  hwr.Headers[System.Net.HttpRequestHeader.Authorization] = "" + txtStart.Text;
                //  hwr.BeginGetResponse(GetResponseCallback, hwr);

                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        // pbWeather.Visibility = System.Windows.Visibility.Visible;
                        client.BaseAddress = new Uri("http://202.6.18.31/bmta/webservice/keyword.php");

                        var url = "?type=busstop&q={0}";

                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        HttpResponseMessage response = await client.GetAsync(String.Format(url, txtStart.Text));

                        if (response.IsSuccessStatusCode)
                        {
                            var data = response.Content.ReadAsStringAsync();

                            var weatherdata = JsonConvert.DeserializeObject<WeatherObject>(data.Result.ToString());

                            string sd = data.Result.ToString();
                            // sd = sd.Replace("{\"status\":\"1\",\"data\":", "");
                            sd = sd.Replace("{\"status\":1\",\"data\":", "");
                            sd = sd.Replace("]}", "]");
                            txtStart.Text = sd;

                            string strJSON = null;
                            strJSON = txtStart.Text;
                            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(strJSON));
                            ObservableCollection<WeatherObject> list = new ObservableCollection<WeatherObject>();
                            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(ObservableCollection<WeatherObject>));
                            list = (ObservableCollection<WeatherObject>)serializer.ReadObject(ms);

                            List<WeatherObject> myMember = new List<WeatherObject>();

                            foreach (WeatherObject cm in list)
                            {

                                AutoCompleteBox txtbox = new AutoCompleteBox();
                                stack.Children.Add(txtbox);
                                txtbox.ItemsSource = cm.stop_name;
                            }


                        }

                        //  pbWeather.Visibility = System.Windows.Visibility.Collapsed;

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Some Error Occured");
                    // pbWeather.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
        }

        public class WeatherObject
        {
            public int id { get; set; }
            public string stop_name { get; set; }
        }
    }
}