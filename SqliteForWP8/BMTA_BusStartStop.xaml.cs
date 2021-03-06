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
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections;
using System.Threading;
using BMTA.Item;
using BMTA.Usercontrols;

namespace BMTA
{
    public partial class BMTA_BusStartStop : PhoneApplicationPage
    {
        public String lang = (Application.Current as App).Language;
        Boolean alreadyStart = false;
        Boolean alreadyEnd = false;
        static WebClient webClient;
        ProgressIndicator progressIndicator = new ProgressIndicator();
        private searchStartStopDetailItem itemstart, itemend;

        public BMTA_BusStartStop()
        {
            InitializeComponent();


            UCStartStop UCStartStop = new UCStartStop();
            foreach (var item in (Application.Current as App).DataStartStop.data)
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

                busStartStoplistbox.Items.Add(UCStartStop);
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

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            busStartStopFrom_search.Text = this.NavigationContext.QueryString["TextFrom"];
            busStartStopTo_search.Text = this.NavigationContext.QueryString["TextTo"];

            if (lang.Equals("th"))
            {
                titleName.Text = "ต้นทางปลายทาง";
            }
            else
            {
                titleName.Text = "Start - End";
            }

        }

        private void btback_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }


        private void busStartStopTo_search_TextChanged(object sender, RoutedEventArgs e)
        {
            if (busStartStopTo_search.Text.Length > 2)
            {
                if (!alreadyEnd)
                {
                    ShowProgressIndicator("Loading..");
                    alreadyEnd = true;
                    callServicegetAutocompleteend();
                }
            }
        }

        private void busStartStopTo_search_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            itemend = (sender as AutoCompleteBox).SelectedItem as searchStartStopDetailItem;
        }

        private void busStartStopFrom_search_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            itemstart = (sender as AutoCompleteBox).SelectedItem as searchStartStopDetailItem;
        }

        private void busStartStopFrom_search_TextChanged(object sender, RoutedEventArgs e)
        {
            if (busStartStopFrom_search.Text.Length > 2)
            {
                if (!alreadyStart)
                {
                    ShowProgressIndicator("Loading..");
                    alreadyStart = true;
                    callServicegetAutocompletestart();
                }
            }
        }

        public void callServicegetAutocompletestart()
        {
            webClient = new WebClient();
            webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            String url = "http://202.6.18.31:7777/getAutocomplete";
            string myParameters;
            try
            {
                myParameters = "type=" + "busstop" + "&keyword=" + busStartStopFrom_search.Text + "&lang=" + lang;
                Debug.WriteLine("URL callServicegetAutocompletestart = " + url);
                webClient.UploadStringCompleted += new UploadStringCompletedEventHandler(callServicegetAutocompletestart_Completed);
                webClient.UploadStringAsync(new Uri(url), myParameters);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void callServicegetAutocompletestart_Completed(object sender, UploadStringCompletedEventArgs e)
        {
            try
            {
                searchStartStopItem results = JsonConvert.DeserializeObject<searchStartStopItem>(e.Result);
                busStartStopFrom_search.ItemsSource = results.data;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                HideProgressIndicator();
                alreadyStart = false;
            }

        }

        public void callServicegetAutocompleteend()
        {
            webClient = new WebClient();
            webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            String url = "http://202.6.18.31:7777/getAutocomplete";
            string myParameters;
            try
            {
                myParameters = "type=" + "busstop" + "&keyword=" + busStartStopTo_search.Text + "&lang=" + lang;
                Debug.WriteLine("URL callServicegetAutocompleteend = " + url);
                webClient.UploadStringCompleted += new UploadStringCompletedEventHandler(callServicegetAutocompleteend_Completed);
                webClient.UploadStringAsync(new Uri(url), myParameters);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void callServicegetAutocompleteend_Completed(object sender, UploadStringCompletedEventArgs e)
        {
            try
            {
                searchStartStopItem results = JsonConvert.DeserializeObject<searchStartStopItem>(e.Result);
                busStartStopTo_search.ItemsSource = results.data;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                HideProgressIndicator();
                alreadyEnd = false;
            }
        }

        private void busStartStopbtn_search_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(busStartStopFrom_search.Text))
            {
                MessageBox.Show("กรุณากรอกต้นทาง");
                return;
            }
            if (string.IsNullOrWhiteSpace(busStartStopTo_search.Text))
            {
                MessageBox.Show("กรุณากรอกปลายทาง");
                return;
            }

            callService_startstop_searchfindRouting();
        }

        public void callService_startstop_searchfindRouting()
        {
            webClient = new WebClient();
            webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            String url = "http://202.6.18.31:7777/searchfindRoutingv2";
            string myParameters;
            try
            {
                if (itemend == null || itemstart == null)
                {
                    myParameters = "busstop_start_id=" + "0" + "&busstop_end_id=" + "0" + "&bus_type=&running_type=&orderby=" + "";
                }
                else
                {
                    myParameters = "busstop_start_id=" + itemstart.id + "&busstop_end_id=" + itemend.id + "&bus_type=&running_type=&orderby=" + "";
                }

                Debug.WriteLine("URL callServicecurrentfindRouting = " + url);

                webClient.UploadStringCompleted += new UploadStringCompletedEventHandler(callService_startstop_searchfindRouting_Completed);
                webClient.UploadStringAsync(new Uri(url), myParameters);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void callService_startstop_searchfindRouting_Completed(object sender, UploadStringCompletedEventArgs e)
        {
            busStartStoplistbox.Items.Clear();
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
            UCStartStop UCStartStop = new UCStartStop();
            foreach (var item in results.data)
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

                busStartStoplistbox.Items.Add(UCStartStop);
            }
        }

        private void busStartStoplistbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (busStartStoplistbox.SelectedIndex != -1)
            {
                UCStartStop item = (sender as ListBox).SelectedItem as UCStartStop;
                (Application.Current as App).RountingDataStartStop = (new_searchfindRoutingItem_data)item.DataContext;

                this.NavigationService.Navigate(new Uri("/BMTA_BusStartStopDetailMap.xaml?TextFrom=" + busStartStopFrom_search.Text + "&TextTo=" + busStartStopTo_search.Text, UriKind.Relative));
            }
            busStartStoplistbox.SelectedIndex = -1;
        }
    }
}