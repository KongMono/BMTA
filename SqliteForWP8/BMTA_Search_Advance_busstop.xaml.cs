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
using System.Windows.Media.Imaging;
using BMTA.Resources;
using System.Collections.ObjectModel;
using SQLite;
using Windows.Storage;
using System.Windows.Input;
using System.Diagnostics;
using Newtonsoft.Json;
using BMTA.Item;


namespace BMTA
{
    public partial class BMTA_Search_Advance_busstop : PhoneApplicationPage
    {
        ProgressIndicator progressIndicator = new ProgressIndicator();
        public String lang = (Application.Current as App).Language;
        private searchlandmarkAndBusstopdetailItem item;
        Boolean already = false;
        private WebClient webClient;
        public BMTA_Search_Advance_busstop()
        {
            InitializeComponent();
            textbox.MinimumPrefixLength = 2;
            textbox.ItemFilter = SearchText;
            
        }
        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (lang.Equals("th"))
            {

                headbusstop.Text = "ค้นหาป้ายรถเมล์";
                headbustype.Text = "ค้นหาประเภทรถ";
                headbussort.Text = "ประเภทการค้นหา";

                txtbusstop.Text = "ป้ายรถเมล์";
                txtbusroute.Text = "เส้นทาง";
                txtbustype.Text = "ประเภทรถ";
                txtselecttype.Text = "ระบุประเภท";

                titleName.Text = "ระบบค้นหาอย่างละเอียด";
                t1.Content = "ทั้งหมด";
                t2.Content = "ปกติ";
                t3.Content = "ทางด่วน";

                x1.Content = "ทั้งหมด";
                x2.Content = "รถธรรมดา";
                x3.Content = "รถปรับอากาศ";

                z1.Content = "เรียงตามระยะทาง";
                z2.Content = "เรียงตามราคา";
                z3.Content = "ต่อรถน้อยที่สุด";
            }
            else
            {
                headbusstop.Text = "Search Bus Stop";
                headbustype.Text = "Search Bus Type";
                headbussort.Text = "Sort By Results";

                txtbusstop.Text = "Bus Stop";
                txtbusroute.Text = "Route";
                txtbustype.Text = "Bus Type";
                txtselecttype.Text = "Select Type";

                titleName.Text = "Advance Search";
                t1.Content = "All";
                t2.Content = "Freeway";
                t3.Content = "Expressways";

                x1.Content = "All";
                x2.Content = "Regular Bus";
                x3.Content = "Air Condition Bus";

                z1.Content = "Sort By Distance";
                z2.Content = "Sort By Price";
                z3.Content = "Fewer Transfers";
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

        private void btback_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void rhome_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_AppTh.xaml", UriKind.Relative));
        }    

        private void btsubmit_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textbox.Text))
            {
                MessageBox.Show("กรุณากรอกป้ายรถเมล์");
                return;
            }

            var buslinePick = (ListPickerItem)busline.SelectedItem;
            var busRunningPick = (ListPickerItem)bustyperunning.SelectedItem;

            callServicecurrentfindRouting(buslinePick, busRunningPick);
        }
        public void callServicecurrentfindRouting(ListPickerItem buslinePick, ListPickerItem busRunningPick)
        {
            webClient = new WebClient();
            webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            String url = "http://202.6.18.31:7777/currentfindRouting";
            string myParameters;
            try
            {
                if (item == null)
                {
                    myParameters = "lat=" + (Application.Current as App).lat_current + "&long=" + (Application.Current as App).lon_current + "&busstop_end_id=" + "0" + "&bus_type=" + buslinePick.Tag + "&running_type=" + busRunningPick.Tag + "&orderby=" + "";
                }
                else
                {
                    myParameters = "lat=" + (Application.Current as App).lat_current + "&long=" + (Application.Current as App).lon_current + "&busstop_end_id=" + item.id + "&bus_type=" + buslinePick.Tag + "&running_type=" + busRunningPick.Tag + "&orderby=" + "";
                }
              
                Debug.WriteLine("URL callServicecurrentfindRouting = " + url);

                webClient.UploadStringCompleted += new UploadStringCompletedEventHandler(callServicecurrentfindRouting_Completed);
                webClient.UploadStringAsync(new Uri(url), myParameters);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void textbox_TextChanged(object sender, RoutedEventArgs e)
        {
            if (textbox.Text.Length > 2)
            {
                if (!already)
                {
                    ShowProgressIndicator("Loading..");
                    already = true;
                    callServicegetAutocomplete();
                }
            }
        }
        bool SearchText(string search, object value)
        {
            if (value != null)
            {
                return true;
            }
            //... If no match, return false. 
            return false;
        }

        private void callServicecurrentfindRouting_Completed(object sender, UploadStringCompletedEventArgs e)
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
            (Application.Current as App).DataStop = results;

            this.NavigationService.Navigate(new Uri("/BMTA_BusStopDetailBus.xaml?TextFrom=" + textbox.Text, UriKind.Relative));
        }

        public void callServicegetAutocomplete()
        {
            webClient = new WebClient();
            webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            String url = "http://202.6.18.31:7777/getAutocomplete";
            string myParameters;
            try
            {
                myParameters = "type=" + "busstop" + "&keyword=" + textbox.Text + "&lang=" + lang;
                Debug.WriteLine("URL callServicegetAutocomplete = " + url);
                webClient.UploadStringCompleted += new UploadStringCompletedEventHandler(callServicegetAutocomplete_Completed);
                webClient.UploadStringAsync(new Uri(url), myParameters);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void callServicegetAutocomplete_Completed(object sender, UploadStringCompletedEventArgs e)
        {
            searchbusstopItem results = JsonConvert.DeserializeObject<searchbusstopItem>(e.Result);

            textbox.ItemsSource = results.data;

            HideProgressIndicator();
            already = false;
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
            progressIndicator.IsIndeterminate = true;
            SystemTray.SetProgressIndicator(this, progressIndicator);
        }

        private void HideProgressIndicator()
        {
            progressIndicator.IsVisible = false;
            progressIndicator.IsIndeterminate = false;
            SystemTray.SetProgressIndicator(this, progressIndicator);
        }

        private void textbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            item = (sender as AutoCompleteBox).SelectedItem as searchlandmarkAndBusstopdetailItem;
        }

    }
}