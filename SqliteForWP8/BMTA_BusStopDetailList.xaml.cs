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
    public partial class BMTA_BusStopDetailList : PhoneApplicationPage
    {
        public String lang = (Application.Current as App).Language;
        public int countExist = 0;
        public Boolean isFirstStatus2 = true;
        public Boolean isExist = false;
        public List<UCRoutingList> dataExistStatus2 = new List<UCRoutingList>();
        Popup popup = new Popup();
        ListBox listpopup = new ListBox();

        public BMTA_BusStopDetailList()
        {
            InitializeComponent();
        }


        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {

            searchfindRoutingItem_data data = (Application.Current as App).RountingDataBusStop;


            if (lang.Equals("th"))
            {
                titleName.Text = "ป้ายรถเมล์";
            }
            else
            {
                titleName.Text = "Bus Stop";
            }

            busstop_search.Text = this.NavigationContext.QueryString["TextFrom"];

            UCRoutingList UCRoutingList = new UCRoutingList();

            UCRoutingList.DataContext = data;
            foreach (var item in data.routing)
            {
                foreach (var busstop in item.busstop)
                {
                    if (busstop.status != "0")
                    {
                        UCRoutingList = new UCRoutingList();

                        if (busstop.status != "2")
                        {
                            isExist = false;
                            UCRoutingList = new UCRoutingList();
                            UCRoutingList.textbusline_name.Text = item.busline_name;
                            if (lang.Equals("th"))
                            {
                                UCRoutingList.textstop_name.Text = busstop.stop_name;

                                if (item.distance == "nan")
                                {
                                    UCRoutingList.textRoute.Text = "0.00 กม.";
                                }
                                else
                                {
                                    UCRoutingList.textRoute.Text = item.distance + " กม.";
                                }
                            }
                            else
                            {
                                UCRoutingList.textstop_name.Text = busstop.stop_name_en;

                                if (item.distance == "nan")
                                {
                                    UCRoutingList.textRoute.Text = "0.00 km.";
                                }
                                else
                                {
                                    UCRoutingList.textRoute.Text = item.distance + " km.";
                                }
                              
                            }


                            if (busstop.status == "4")
                            {
                               

                                if (lang.Equals("th"))
                                {
                                    UCRoutingList.textstop_name.Text = busstop.stop_name;
                                    UCRoutingList.textbusline_name.Text = "เดินไปยัง";
                                }
                                else
                                {
                                    UCRoutingList.textstop_name.Text = busstop.stop_name_en;
                                    UCRoutingList.textbusline_name.Text = "walk to";
                                }

                                UCRoutingList.textRoute.Text = busstop.stop_name;
                            }

                            UCRoutingList.status = busstop.status;

                            busStoplistbox.Items.Add(UCRoutingList);

                            countExist = 0;
                        }
                        else
                        {
                            if (countExist < 1)
                            {
                            }

                            if (busstop.status == "2" && isFirstStatus2)
                            {
                                isFirstStatus2 = false;
                                isExist = true;
                                UCRoutingList = new UCRoutingList();
                                UCRoutingList.textbusline_name.Text = item.busline_name;
                                if (lang.Equals("th"))
                                {
                                    UCRoutingList.textstop_name.Text = busstop.stop_name;
                                    if (item.distance == "nan")
                                    {
                                        UCRoutingList.textRoute.Text = "0.00 กม.";
                                    }
                                    else
                                    {
                                        UCRoutingList.textRoute.Text = item.distance + " กม.";
                                    }
                                 
                                }
                                else
                                {
                                    UCRoutingList.textstop_name.Text = busstop.stop_name_en;
                                    if (item.distance == "nan")
                                    {
                                        UCRoutingList.textRoute.Text = "0.00 km.";
                                    }
                                    else
                                    {
                                        UCRoutingList.textRoute.Text = item.distance + " km.";
                                    }
                                }
                                UCRoutingList.status = busstop.status;
                                busStoplistbox.Items.Add(UCRoutingList);
                            }
                            else
                            {
                                if (!isExist)
                                {
                                    UCRoutingList = new UCRoutingList();
                                    UCRoutingList.textbusline_name.Text = item.busline_name;
                                    if (lang.Equals("th"))
                                    {
                                        UCRoutingList.textstop_name.Text = busstop.stop_name;
                                        if (item.distance == "nan")
                                        {
                                            UCRoutingList.textRoute.Text = "0.00 กม.";
                                        }
                                        else
                                        {
                                            UCRoutingList.textRoute.Text = item.distance + " กม.";
                                        }
                                    }
                                    else
                                    {
                                        UCRoutingList.textstop_name.Text = busstop.stop_name_en;
                                        if (item.distance == "nan")
                                        {
                                            UCRoutingList.textRoute.Text = "0.00 km.";
                                        }
                                        else
                                        {
                                            UCRoutingList.textRoute.Text = item.distance + " km.";
                                        }
                                    }
                                    UCRoutingList.status = busstop.status;
                                    busStoplistbox.Items.Add(UCRoutingList);
                                }
                                else
                                {
                                    UCRoutingList = new UCRoutingList();
                                    UCRoutingList.textbusline_name.Text = item.busline_name;
                                    if (lang.Equals("th"))
                                    {
                                        UCRoutingList.textstop_name.Text = busstop.stop_name;
                                        if (item.distance == "nan")
                                        {
                                            UCRoutingList.textRoute.Text = "0.00 กม.";
                                        }
                                        else
                                        {
                                            UCRoutingList.textRoute.Text = item.distance + " กม.";
                                        }
                                    }
                                    else
                                    {
                                        UCRoutingList.textstop_name.Text = busstop.stop_name_en;
                                        if (item.distance == "nan")
                                        {
                                            UCRoutingList.textRoute.Text = "0.00 km.";
                                        }
                                        else
                                        {
                                            UCRoutingList.textRoute.Text = item.distance + " km.";
                                        }
                                    }
                                    UCRoutingList.status = busstop.status;
                                    dataExistStatus2.Add(UCRoutingList);
                                }
                            }
                            countExist++;
                        }
                    }
                }
            }

            if (dataExistStatus2.Count > 0)
            {
                for (int i = 0; i < busStoplistbox.Items.Count; i++)
                {
                    UCRoutingList routingList = (UCRoutingList)busStoplistbox.Items[i];
                    if (routingList.status == "2")
                    {
                        routingList.btn_collapsed.Click += btn_collapsed_Click;

                        routingList.hide = false;
                        routingList.listpopup = dataExistStatus2;
                    }
                }
            }
            else
            {
                for (int i = 0; i < busStoplistbox.Items.Count; i++)
                {
                    UCRoutingList routingList = (UCRoutingList)busStoplistbox.Items[i];
                    if (routingList.status == "2")
                    {
                        routingList.hide = true;
                    }
                }
            }
        }

        private void btn_collapsed_Click(object sender, RoutedEventArgs e)
        {
            if (listpopup.Items.Count < 1)
            {
                popup = new Popup();
                popup.Height = 780;
                popup.Width = 480;
                popup.VerticalOffset = 78;
                listpopup.Height = 780;
                foreach (var itemRoutingList in dataExistStatus2)
                {
                    itemRoutingList.hide = true;
                    listpopup.Items.Add(itemRoutingList);
                }
                popup.Child = listpopup;
                popup.IsOpen = true;
            }
            else
            {
                popup.IsOpen = true;
            }
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if (popup != null)
            {
                if (popup.IsOpen)
                {
                    e.Cancel = true;
                    popup.IsOpen = false;
                }
            }
        }

        private void btback_Click(object sender, RoutedEventArgs e)
        {
            if (popup.IsOpen)
            {
                popup.IsOpen = false;
            }
            else
            {
                NavigationService.GoBack();
            }
        }
    }
}