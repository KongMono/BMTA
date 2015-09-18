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

            new_searchfindRoutingItem_data data = (Application.Current as App).RountingDataBusStop;


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


            for (int i = 0; i < data.route.Count; i++)
            {
                UCRoutingList = new UCRoutingList();

                if (data.route[i].step == "1" || data.route[i].step == "3")
                {

                    UCRoutingList.textbusline_name.Text = getBuslinebyUnit(data.route[i].busline, data.route[i].bustype);
                    if (lang.Equals("th"))
                    {
                        UCRoutingList.textstop_name.Text = data.route[i].busstop.name;

                        if (data.route[i].distance == "nan")
                        {
                            UCRoutingList.textRoute.Text = "";
                        }
                        else
                        {
                            if (data.route[i].step == "3" && data.route[i] == data.route.Last())
                            {
                                UCRoutingList.textRoute.Text = "";
                            }
                            else
                            {
                                UCRoutingList.textRoute.Text = getRoutebyUnit(data.route[i].distance);
                            }
                        }
                    }
                    else
                    {
                        UCRoutingList.textstop_name.Text = data.route[i].busstop.name_en;

                        if (data.route[i].distance == "nan")
                        {
                            UCRoutingList.textRoute.Text = "";
                        }
                        else
                        {
                            if (data.route[i].step == "3" && data.route[i] == data.route.Last())
                            {
                                UCRoutingList.textRoute.Text = "";
                            }
                            else
                            {
                                UCRoutingList.textRoute.Text = getRoutebyUnit(data.route[i].distance);
                            }
                        }
                    }
                }
                else if (data.route[i].step == "2")
                {
                    UCRoutingList.textbusline_name.Text = getBuslinebyUnit(data.route[i].busline, data.route[i].bustype);

                    if (lang.Equals("th"))
                    {
                        UCRoutingList.textstop_name.Text = data.route[i].busstop.name;

                        if (data.route[i].distance == "nan")
                        {
                            UCRoutingList.textRoute.Text = "";
                        }
                        else
                        {
                            UCRoutingList.textRoute.Text = getRoutebyUnit(data.route[i].distance);
                        }
                    }
                    else
                    {
                        UCRoutingList.textstop_name.Text = data.route[i].busstop.name_en;

                        if (data.route[i].distance == "nan")
                        {
                            UCRoutingList.textRoute.Text = "";
                        }
                        else
                        {
                            UCRoutingList.textRoute.Text = getRoutebyUnit(data.route[i].distance);
                        }
                    }
                    //loop change
                    dataExistStatus2 = new List<UCRoutingList>();
                    for (int x = 0; x < data.route[i].change.Count; x++)
                    {
                        UCRoutingList UCRoutingListDetail = new UCRoutingList();
                        UCRoutingListDetail.textbusline_name.Text = "";
                        if (lang.Equals("th"))
                        {
                            UCRoutingListDetail.textstop_name.Text = data.route[i].change[x].name;
                            if (data.route[i].distance == "nan")
                            {
                                UCRoutingListDetail.textRoute.Text = "";
                            }
                            else
                            {
                                UCRoutingListDetail.textRoute.Text = getRoutebyUnit(data.route[i].distance);
                            }
                        }
                        else
                        {
                            UCRoutingListDetail.textstop_name.Text = data.route[i].change[x].name_en;
                            if (data.route[i].distance == "nan")
                            {
                                UCRoutingListDetail.textRoute.Text = "";
                            }
                            else
                            {
                                UCRoutingListDetail.textRoute.Text = getRoutebyUnit(data.route[i].distance);
                            }
                        }

                        UCRoutingListDetail.step = data.route[i].step;
                        dataExistStatus2.Add(UCRoutingListDetail);
                    }

                    //check detail
                    if (dataExistStatus2.Count > 0)
                    {
                        UCRoutingList.btn_collapsed.Click += btn_collapsed_Click;
                        UCRoutingList.hide = false;
                        UCRoutingList.listpopup = dataExistStatus2;
                    }
                    else
                    {
                        UCRoutingList.hide = true;
                    }
                }
                else if (data.route[i].step == "4" || data.route[i].step == "5")
                {
                    if (lang.Equals("th"))
                    {
                        UCRoutingList.textstop_name.Text = data.route[i].busstop.name;
                        UCRoutingList.textbusline_name.Text = "เดินไปยัง";
                        UCRoutingList.textRoute.Text = data.route[i + 1].busstop.name;
                    }
                    else
                    {
                        UCRoutingList.textstop_name.Text = data.route[i].busstop.name_en;
                        UCRoutingList.textbusline_name.Text = "walk to";
                        UCRoutingList.textRoute.Text = data.route[i + 1].busstop.name_en;
                    }
                }
                else if (data.route[i].step == "6")
                {
                    if (lang.Equals("th"))
                    {
                        UCRoutingList.textstop_name.Text = data.route[i].busstop.name;
                        UCRoutingList.textbusline_name.Text = "";
                        UCRoutingList.textRoute.Text = "";
                    }
                    else
                    {
                        UCRoutingList.textstop_name.Text = data.route[i].busstop.name_en;
                        UCRoutingList.textbusline_name.Text = "";
                        UCRoutingList.textRoute.Text = "";
                    }
                }

                UCRoutingList.step = data.route[i].step;
                busStoplistbox.Items.Add(UCRoutingList);
            }
        }

        private String getRoutebyUnit(string distance)
        {
            string value = null;
            var d = Convert.ToDouble(distance);

            if (d < 1000)
            {
                if (lang.Equals("th"))
                {
                    value = "ระยะทาง " + Convert.ToString(Math.Round(d, 2)) + " ม.";
                }
                else
                {
                    value = "Distance " + Convert.ToString(Math.Round(d, 2)) + " m.";
                }
            }
            else
            {
                d = d / 1000;
                if (lang.Equals("th"))
                {
                    value = "ระยะทาง " + Convert.ToString(Math.Round(d, 2)) + " กม.";
                }
                else
                {
                    value = "Distance " + Convert.ToString(Math.Round(d, 2)) + " km.";
                }
            }

            return value;
        }

        private String getBuslinebyUnit(string busline, string bustype)
        {
            string value = null;

            if (lang.Equals("th"))
            {

                if (bustype == "1")
                {
                    value = "สาย-" + busline + " รถธรรมดา";
                }
                else
                {
                    value = "สาย-" + busline + " รถปรับอากาศ";
                }

            }
            else
            {
                if (bustype == "1")
                {
                    value = "line-" + busline + "";
                }
                else
                {
                    value = "line-" + busline + " air";
                }
            }

            return value;
        }

        private void btn_collapsed_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn != null)
            {
                List<UCRoutingList> d = (List<UCRoutingList>)btn.DataContext;
                if (d.Count > 0)
                {
                    try
                    {
                        if (listpopup.Items.Count < 1)
                        {

                            for (int i = 0; i < d.Count; i++)
                            {
                                if (d[i].hide == false)
                                {
                                    d[i].hide = true;
                                }
                                listpopup.Items.Add(d[i]);
                            }

                            popup = new Popup();
                            popup.Height = 780;
                            popup.Width = 480;
                            popup.VerticalOffset = 78;
                            listpopup.Height = 780;
                            popup.Child = listpopup;
                            popup.IsOpen = true;
                            busStartStopLayout.Visibility = System.Windows.Visibility.Collapsed;
                        }
                        else
                        {
                            listpopup.Items.Clear();

                            for (int i = 0; i < d.Count; i++)
                            {
                                if (d[i].hide == false)
                                {
                                    d[i].hide = true;
                                }
                                listpopup.Items.Add(d[i]);
                            }
                            popup.Child = listpopup;
                            popup.IsOpen = true;
                            busStartStopLayout.Visibility = System.Windows.Visibility.Collapsed;
                        }

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
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
                    busStartStopLayout.Visibility = System.Windows.Visibility.Visible;
                }
            }
        }

        private void btback_Click(object sender, RoutedEventArgs e)
        {
            if (popup.IsOpen)
            {
                popup.IsOpen = false;
                busStartStopLayout.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                NavigationService.GoBack();
            }
        }
    }
}