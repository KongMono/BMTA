﻿using System;
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
    public partial class BMTA_BusLandMarkDetailBus : PhoneApplicationPage
    {
        public String lang = (Application.Current as App).Language;
        static WebClient webClient;
        Boolean alreadyLandMark = false;
        ProgressIndicator progressIndicator = new ProgressIndicator();
        private searchlandmarkAndBusstopdetailItem itemLandMark;
        UCStartStop UCStartStop = new UCStartStop();
        List<new_searchfindRoutingItem_data> mylist;
        public BMTA_BusLandMarkDetailBus()
        {
            InitializeComponent();
            landMarklistbox.Items.Clear();
            UCStartStop = new UCStartStop();

            mylist = (Application.Current as App).DataLandMark.data;

            mylist = mylist.OrderBy(o => Double.Parse(o.distance)).ToList();

            foreach (var item in mylist)
            {
                UCStartStop = new UCStartStop();
                UCStartStop.DataContext = item;
                var d = Convert.ToDouble(item.distance);
                if (d < 1000)
                {
                    if (lang.Equals("th"))
                    {
                        UCStartStop.textKm.Text = Convert.ToString(Math.Round(d, 2)) + " ม.";
                        UCStartStop.textPrice.Text = getPrice(item.price);
                    }
                    else
                    {
                        UCStartStop.textKm.Text = Convert.ToString(Math.Round(d, 2)) + " m.";
                        UCStartStop.textPrice.Text = getPrice(item.price);
                    }
                }
                else
                {
                    d = d / 1000;
                    if (lang.Equals("th"))
                    {
                        UCStartStop.textKm.Text = Convert.ToString(Math.Round(d, 2)) + " กม.";
                        UCStartStop.textPrice.Text = getPrice(item.price);
                    }
                    else
                    {
                        UCStartStop.textKm.Text = Convert.ToString(Math.Round(d, 2)) + " km.";
                        UCStartStop.textPrice.Text = getPrice(item.price);
                    }
                }

                if (item.list.Count == 1)
                {
                    UCStartStop.text_route1.Text = getNameBustype(item.list[0].busline, item.list[0].bustype);
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
                else if (item.list.Count == 2)
                {
                    UCStartStop.text_route1.Text = getNameBustype(item.list[0].busline, item.list[0].bustype);
                    UCStartStop.text_route2.Text = getNameBustype(item.list[1].busline, item.list[1].bustype);

                    UCStartStop.img_route3.Visibility = System.Windows.Visibility.Collapsed;
                    UCStartStop.img_route4.Visibility = System.Windows.Visibility.Collapsed;

                    UCStartStop.img_cen3.Visibility = System.Windows.Visibility.Collapsed;
                    UCStartStop.img_cen4.Visibility = System.Windows.Visibility.Collapsed;

                    UCStartStop.text_route3.Visibility = System.Windows.Visibility.Collapsed;
                    UCStartStop.text_route4.Visibility = System.Windows.Visibility.Collapsed;

                }
                else if (item.list.Count == 3)
                {
                    UCStartStop.text_route1.Text = getNameBustype(item.list[0].busline, item.list[0].bustype);
                    UCStartStop.text_route2.Text = getNameBustype(item.list[1].busline, item.list[1].bustype);
                    UCStartStop.text_route3.Text = getNameBustype(item.list[2].busline, item.list[2].bustype);

                    UCStartStop.img_route4.Visibility = System.Windows.Visibility.Collapsed;
                    UCStartStop.img_cen4.Visibility = System.Windows.Visibility.Collapsed;
                    UCStartStop.text_route4.Visibility = System.Windows.Visibility.Collapsed;
                }
                landMarklistbox.Items.Add(UCStartStop);
            }
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

        public String getPrice(string price)
        {
            string value = "";

            if (price != "0")
            {
                if (lang.Equals("th"))
                {
                    value = "ราคา " + price + " บ.";
                }
                else
                {
                    value = "Price " + price + " ฿";
                }
            }
            else
            {
                value = "";
            }

            return value;
        }

        private void landMark_search_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            searchlandmarkAndBusstopdetailItem item = (sender as AutoCompleteBox).SelectedItem as searchlandmarkAndBusstopdetailItem;
            if (item != null)
            {
                itemLandMark = item;
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

        private void landMarklistbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (landMarklistbox.SelectedIndex != -1)
            {
                UCStartStop item = (sender as ListBox).SelectedItem as UCStartStop;
                (Application.Current as App).RountingDataLandMark = (new_searchfindRoutingItem_data)item.DataContext;

                this.NavigationService.Navigate(new Uri("/BMTA_BusLandMarkDetailMap.xaml?TextFrom=" + landMark_search.Text, UriKind.Relative));
            }
            landMarklistbox.SelectedIndex = -1;
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            landMark_search.Text = this.NavigationContext.QueryString["TextFrom"];

            if (lang.Equals("th"))
            {
                titleName.Text = "ถนนและสถานที่สำคัญ";
                p1.Content = "ระยะทางใกล้ที่สุด";
                p2.Content = "ราคาถูกที่สุด";
                p3.Content = "ต่อรถน้อยที่สุด";
            }
            else
            {
                titleName.Text = "Streets and Landmarks";
                p1.Content = "Sort By Distance";
                p2.Content = "Sort By Price";
                p3.Content = "Fewer Transfers";
            }
        }

        public void callServicegetAutocompleteLandMark()
        {
            ShowProgressIndicator("Loading..");

            webClient = new WebClient();
            String url = "http://128.199.232.94/webservice/keyword.php";
            string myParameters;
            try
            {
                myParameters = url + "?type=" + "place" + "&q=" + landMark_search.Text + "&lang=" + lang;
                Debug.WriteLine("URL callServicegetAutocompleteLandMark = " + myParameters);
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

            landMark_search.ItemsSource = results.data;

            HideProgressIndicator();
            alreadyLandMark = false;
        }


        private void landMark_search_TextChanged(object sender, RoutedEventArgs e)
        {
            if (landMark_search.Text.Length > 2)
            {
                if (!alreadyLandMark)
                {
                    ShowProgressIndicator("Loading..");
                    alreadyLandMark = true;
                    callServicegetAutocompleteLandMark();
                }
            }
        }

        private void landMark_search_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (string.IsNullOrWhiteSpace(landMark_search.Text))
                {
                    MessageBox.Show("กรุณาใส่ป้ายรถเมล์ที่ต้องการ");
                    return;
                }
                callplacecurrentfindRouting();
            }
        }

        public void callplacecurrentfindRouting()
        {
            webClient = new WebClient();
            webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            String url = "http://202.6.18.31:7777/placecurrentfindRoutingv2";
            string myParameters;
            try
            {
                if (itemLandMark == null)
                {
                    myParameters = "lat=" + (Application.Current as App).lat_current + "&long=" + (Application.Current as App).lon_current + "&elatlong=" + "0.0-0.0" + "&bus_type=" + "" + "&running_type=" + "" + "&orderby=" + "";
                }
                else
                {
                    myParameters = "lat=" + (Application.Current as App).lat_current + "&long=" + (Application.Current as App).lon_current + "&elatlong=" + itemLandMark.lattitude + "-" + itemLandMark.longtitude + "&bus_type=" + "" + "&running_type=" + "" + "&orderby=" + "";
                }

                Debug.WriteLine("URL callplacecurrentfindRouting = " + url);
                webClient.UploadStringCompleted += new UploadStringCompletedEventHandler(callplacecurrentfindRouting_Completed);
                webClient.UploadStringAsync(new Uri(url), myParameters);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void setData(List<new_searchfindRoutingItem_data> data)
        {
            landMarklistbox.Items.Clear();
            foreach (var item in data)
            {
                UCStartStop = new UCStartStop();
                UCStartStop.DataContext = item;
                var d = Convert.ToDouble(item.distance);
                if (d < 1000)
                {
                    if (lang.Equals("th"))
                    {
                        UCStartStop.textKm.Text = Convert.ToString(Math.Round(d, 2)) + " ม.";
                        if (item.price.Equals("0"))
                        {
                            UCStartStop.textPrice.Text = "";
                        }
                        else
                        {
                            UCStartStop.textPrice.Text = "ราคา " + item.price + " บ.";
                        }
                    }
                    else
                    {
                        UCStartStop.textKm.Text = Convert.ToString(Math.Round(d, 2)) + " m.";
                        if (item.price.Equals("0"))
                        {
                            UCStartStop.textPrice.Text = "";
                        }
                        else
                        {
                            UCStartStop.textPrice.Text = "Price " + item.price + " ฿";
                        }
                    }
                }
                else
                {
                    d = d / 1000;
                    if (lang.Equals("th"))
                    {
                        UCStartStop.textKm.Text = Convert.ToString(Math.Round(d, 2)) + " กม.";
                        if (item.price.Equals("0"))
                        {
                            UCStartStop.textPrice.Text = "";
                        }
                        else
                        {
                            UCStartStop.textPrice.Text = "ราคา " + item.price + " บ.";
                        }
                    }
                    else
                    {
                        UCStartStop.textKm.Text = Convert.ToString(Math.Round(d, 2)) + " km.";
                        if (item.price.Equals("0"))
                        {
                            UCStartStop.textPrice.Text = "";
                        }
                        else
                        {
                            UCStartStop.textPrice.Text = "Price " + item.price + " ฿";
                        }
                    }
                }

                if (item.list.Count == 1)
                {
                    UCStartStop.text_route1.Text = item.list[0].busline;
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
                else if (item.list.Count == 2)
                {
                    UCStartStop.text_route1.Text = item.list[0].busline;
                    UCStartStop.text_route2.Text = item.list[1].busline;

                    UCStartStop.img_route3.Visibility = System.Windows.Visibility.Collapsed;
                    UCStartStop.img_route4.Visibility = System.Windows.Visibility.Collapsed;

                    UCStartStop.img_cen3.Visibility = System.Windows.Visibility.Collapsed;
                    UCStartStop.img_cen4.Visibility = System.Windows.Visibility.Collapsed;

                    UCStartStop.text_route3.Visibility = System.Windows.Visibility.Collapsed;
                    UCStartStop.text_route4.Visibility = System.Windows.Visibility.Collapsed;

                }
                else if (item.list.Count == 3)
                {
                    UCStartStop.text_route1.Text = item.list[0].busline;
                    UCStartStop.text_route2.Text = item.list[1].busline;
                    UCStartStop.text_route3.Text = item.list[2].busline;

                    UCStartStop.img_route4.Visibility = System.Windows.Visibility.Collapsed;
                    UCStartStop.img_cen4.Visibility = System.Windows.Visibility.Collapsed;
                    UCStartStop.text_route4.Visibility = System.Windows.Visibility.Collapsed;
                }

                landMarklistbox.Items.Add(UCStartStop);
            }
        }

        private void callplacecurrentfindRouting_Completed(object sender, UploadStringCompletedEventArgs e)
        {
            new_searchfindRoutingItem results = JsonConvert.DeserializeObject<new_searchfindRoutingItem>(e.Result);
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

            mylist = mylist.OrderBy(o => Double.Parse(o.distance)).ToList();

            setData(mylist);

        }

        private void busFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListPickerItem lpi = (sender as ListPicker).SelectedItem as ListPickerItem;

            if (lpi.Content != null)
            {
                if (lpi.Content.Equals("ระยะทางใกล้ที่สุด") || lpi.Content.Equals("Sort By Distance"))
                {
                    mylist = mylist.OrderBy(o => Double.Parse(o.distance)).ToList();
                    setData(mylist);
                }
                else if (lpi.Content.Equals("ราคาถูกที่สุด") || lpi.Content.Equals("Sort By Price"))
                {
                    mylist = mylist.OrderBy(o => Double.Parse(o.price)).ToList();
                    setData(mylist);
                }
                else
                {
                    mylist = mylist.OrderBy(o => o.list.Count).ToList();
                    setData(mylist);
                }
            }
        }
    }
}