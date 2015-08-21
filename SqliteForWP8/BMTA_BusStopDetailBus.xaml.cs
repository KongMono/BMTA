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
    public partial class BMTA_BusStopDetailBus : PhoneApplicationPage
    {
        public String lang = (Application.Current as App).Language;
        static WebClient webClient;
        Boolean alreadyBusStop = false;
        ProgressIndicator progressIndicator = new ProgressIndicator();
        private searchlandmarkAndBusstopdetailItem itemBusStop;
        UCStartStop UCStartStop = new UCStartStop();
        List<searchfindRoutingItem_data> mylist;
        public BMTA_BusStopDetailBus()
        {
            InitializeComponent();
            busStoplistbox.Items.Clear();
            UCStartStop = new UCStartStop();

            mylist = (Application.Current as App).DataStop.data;

            mylist = mylist.OrderBy(o => Double.Parse(o.total.total_distance)).ToList();

            foreach (var item in mylist)
            {

                UCStartStop = new UCStartStop();
                UCStartStop.DataContext = item;
                if (lang.Equals("th"))
                {
                    UCStartStop.textKm.Text = item.total.total_distance + " กม.";
                    UCStartStop.textPrice.Text = "ราคา " + item.total.total_price + " บ.";
                }
                else
                {
                    UCStartStop.textKm.Text = item.total.total_distance + " km.";
                    UCStartStop.textPrice.Text = "Price " + item.total.total_price + " ฿";
                }

                if (item.routing.Count == 1)
                {
                    UCStartStop.text_route1.Text = item.routing[0].bus_line;
                    UCStartStop.img_route2.Visibility = System.Windows.Visibility.Collapsed;
                    UCStartStop.img_route3.Visibility = System.Windows.Visibility.Collapsed;
                    UCStartStop.img_route4.Visibility = System.Windows.Visibility.Collapsed;

                    UCStartStop.img_cen2.Visibility = System.Windows.Visibility.Collapsed;
                    UCStartStop.img_cen3.Visibility = System.Windows.Visibility.Collapsed;
                    UCStartStop.img_cen4.Visibility = System.Windows.Visibility.Collapsed;

                    UCStartStop.text_route2.Visibility = System.Windows.Visibility.Collapsed;
                    UCStartStop.text_route3.Visibility = System.Windows.Visibility.Collapsed;
                    UCStartStop.text_route4.Visibility = System.Windows.Visibility.Collapsed;
                }
                else if (item.routing.Count == 2)
                {
                    UCStartStop.text_route1.Text = item.routing[0].bus_line;
                    UCStartStop.text_route2.Text = item.routing[1].bus_line;

                    UCStartStop.img_route3.Visibility = System.Windows.Visibility.Collapsed;
                    UCStartStop.img_route4.Visibility = System.Windows.Visibility.Collapsed;

                    UCStartStop.img_cen3.Visibility = System.Windows.Visibility.Collapsed;
                    UCStartStop.img_cen4.Visibility = System.Windows.Visibility.Collapsed;

                    UCStartStop.text_route3.Visibility = System.Windows.Visibility.Collapsed;
                    UCStartStop.text_route4.Visibility = System.Windows.Visibility.Collapsed;
                }
                else if (item.routing.Count == 3)
                {
                    UCStartStop.text_route1.Text = item.routing[0].bus_line;
                    UCStartStop.text_route2.Text = item.routing[1].bus_line;
                    UCStartStop.text_route3.Text = item.routing[2].bus_line;

                    UCStartStop.img_route4.Visibility = System.Windows.Visibility.Collapsed;
                    UCStartStop.img_cen4.Visibility = System.Windows.Visibility.Collapsed;
                    UCStartStop.text_route4.Visibility = System.Windows.Visibility.Collapsed;
                }
                busStoplistbox.Items.Add(UCStartStop);
            }
        }
        private void busstop_search_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            searchlandmarkAndBusstopdetailItem item = (sender as AutoCompleteBox).SelectedItem as searchlandmarkAndBusstopdetailItem;
            if (item != null)
            {
                itemBusStop = item;
            }
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

        private void btback_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void busStoplistbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (busStoplistbox.SelectedIndex != -1)
            {
                UCStartStop item = (sender as ListBox).SelectedItem as UCStartStop;
                (Application.Current as App).RountingDataBusStop = (searchfindRoutingItem_data)item.DataContext;

                this.NavigationService.Navigate(new Uri("/BMTA_BusStopDetailMap.xaml?TextFrom=" + busstop_search.Text, UriKind.Relative));
            }
            busStoplistbox.SelectedIndex = -1;
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            busstop_search.Text = this.NavigationContext.QueryString["TextFrom"];

            if (lang.Equals("th"))
            {
                titleName.Text = "ป้ายรถเมล์";

                p1.Content = "ระยะทางใกล้ที่สุด";
                p2.Content = "ราคาถูกที่สุด";
                p3.Content = "ต่อรถน้อยที่สุด";
            }
            else
            {
                titleName.Text = "Bus Stop";
                p1.Content = "Sort By Distance";
                p2.Content = "Sort By Price";
                p3.Content = "Fewer Transfers";
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

        private void busstop_search_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                busstop_search.IsEnabled = false;
                busstop_search.IsEnabled = true;

                if (string.IsNullOrWhiteSpace(busstop_search.Text))
                {
                    MessageBox.Show("กรุณาใส่ป้ายรถเมล์ที่ต้องการ");
                    return;
                }
                callService_busstop_currentfindRouting();
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

            mylist = results.data;
            setData(mylist);
        }

        private void setData(List<searchfindRoutingItem_data> data)
        {
            busStoplistbox.Items.Clear();
            foreach (var item in data)
            {
                UCStartStop = new UCStartStop();
                UCStartStop.DataContext = item;
                if (lang.Equals("th"))
                {
                    UCStartStop.textKm.Text = item.total.total_distance + " กม.";
                    UCStartStop.textPrice.Text = "ราคา " + item.total.total_price + " บ.";
                }
                else
                {
                    UCStartStop.textKm.Text = item.total.total_distance + " km.";
                    UCStartStop.textPrice.Text = "Price " + item.total.total_price + " ฿";
                }

                if (item.routing.Count == 1)
                {
                    UCStartStop.text_route1.Text = item.routing[0].bus_line;
                    UCStartStop.img_route2.Visibility = System.Windows.Visibility.Collapsed;
                    UCStartStop.img_route3.Visibility = System.Windows.Visibility.Collapsed;
                    UCStartStop.img_route4.Visibility = System.Windows.Visibility.Collapsed;

                    UCStartStop.img_cen2.Visibility = System.Windows.Visibility.Collapsed;
                    UCStartStop.img_cen3.Visibility = System.Windows.Visibility.Collapsed;
                    UCStartStop.img_cen4.Visibility = System.Windows.Visibility.Collapsed;

                    UCStartStop.text_route2.Visibility = System.Windows.Visibility.Collapsed;
                    UCStartStop.text_route3.Visibility = System.Windows.Visibility.Collapsed;
                    UCStartStop.text_route4.Visibility = System.Windows.Visibility.Collapsed;
                }
                else if (item.routing.Count == 2)
                {
                    UCStartStop.text_route1.Text = item.routing[0].bus_line;
                    UCStartStop.text_route2.Text = item.routing[1].bus_line;

                    UCStartStop.img_route3.Visibility = System.Windows.Visibility.Collapsed;
                    UCStartStop.img_route4.Visibility = System.Windows.Visibility.Collapsed;

                    UCStartStop.img_cen3.Visibility = System.Windows.Visibility.Collapsed;
                    UCStartStop.img_cen4.Visibility = System.Windows.Visibility.Collapsed;

                    UCStartStop.text_route3.Visibility = System.Windows.Visibility.Collapsed;
                    UCStartStop.text_route4.Visibility = System.Windows.Visibility.Collapsed;

                }
                else if (item.routing.Count == 3)
                {
                    UCStartStop.text_route1.Text = item.routing[0].bus_line;
                    UCStartStop.text_route2.Text = item.routing[1].bus_line;
                    UCStartStop.text_route3.Text = item.routing[2].bus_line;

                    UCStartStop.img_route4.Visibility = System.Windows.Visibility.Collapsed;
                    UCStartStop.img_cen4.Visibility = System.Windows.Visibility.Collapsed;
                    UCStartStop.text_route4.Visibility = System.Windows.Visibility.Collapsed;
                }

                busStoplistbox.Items.Add(UCStartStop);
            }
        }

        private void busFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListPickerItem lpi = (sender as ListPicker).SelectedItem as ListPickerItem;
            //MessageBox.Show("selected item is : " + lpi.Content);

            if (lpi.Content != null)
            {
                if (lpi.Content.Equals("ระยะทางใกล้ที่สุด") || lpi.Content.Equals("Sort By Distance"))
                {
                    mylist = mylist.OrderBy(o => Double.Parse(o.total.total_distance)).ToList();
                    setData(mylist);
                }
                else if (lpi.Content.Equals("ราคาถูกที่สุด") || lpi.Content.Equals("Sort By Price"))
                {
                    mylist = mylist.OrderBy(o => Double.Parse(o.total.total_price)).ToList();
                    setData(mylist);
                }
                else
                {
                    mylist = mylist.OrderBy(o => o.routing.Count).ToList();
                    setData(mylist);
                }
            }
        }
    }
}