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
    public partial class BMTA_Search_Advance_startend : PhoneApplicationPage
    {
        ProgressIndicator progressIndicator = new ProgressIndicator();
        public String lang = (Application.Current as App).Language;
        private searchStartStopDetailItem itemstart, itemend;
        Boolean alreadyStart = false;
        Boolean alreadyEnd = false;
        private WebClient webClient;


        public BMTA_Search_Advance_startend()
        {
            InitializeComponent();
            txtboxstart.ItemFilter = SearchTextstart;
            txtboxend.ItemFilter = SearchTextend;
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {

            if (lang.Equals("th"))
            {

                titleName.Text = "ระบบค้นหาอย่างละเอียด";
                txtstart.Text = "ต้นทาง";
                txtend.Text = "ปลายทาง";
                txtroute.Text = "เส้นทาง";
                txtrbustype.Text = "ประเภทรถ";
                txtselecttype.Text = "ระบุประเภท";

                head1.Text = "ค้นหาต้นทาง-ปลายทาง";
                head2.Text = "ค้นหาประเภทรถ";
                head3.Text = "เรียงผลการต้นหาตาม";

                t1.Content = "ทั้งหมด";
                t2.Content = "ปกติ";
                t3.Content = "ทางด่วน";

                x1.Content = "ทั้งหมด";
                x2.Content = "รถธรรมดา";
                x3.Content = "รถปรับอากาศ";

                z1.Content = "ระยะทางใกล้ที่สุด";
                z2.Content = "ราคาถูกที่สุด";
                z3.Content = "ต่อรถน้อยที่สุด";
            }
            else
            {

                titleName.Text = "Advance Search";
                txtstart.Text = "Start";
                txtend.Text = "End";
                txtroute.Text = "Route";
                txtrbustype.Text = "Bus Type";
                txtselecttype.Text = "Select Type";

                head1.Text = "Search Start-End";
                head2.Text = "Search Bus Type";
                head3.Text = "Sort By Results";

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
        private void btback_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void btsubmit_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtboxstart.Text))
            {
                MessageBox.Show("กรุณากรอกต้นทาง");
                return;
            }
            if (string.IsNullOrWhiteSpace(txtboxend.Text))
            {
                MessageBox.Show("กรุณากรอกปลายทาง");
                return;
            }

            var buslinePick = (ListPickerItem)busline.SelectedItem;
            var busRunningPick = (ListPickerItem)bustyperunning.SelectedItem;

            callServicesearchfindRouting(buslinePick, busRunningPick);
        }
        public void callServicesearchfindRouting(ListPickerItem buslinePick, ListPickerItem busRunningPick)
        {
            webClient = new WebClient();
            webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            String url = "http://202.6.18.31:7777/searchfindRouting";
            string myParameters;
            try
            {
                if (itemend == null || itemstart == null)
                {
                    myParameters = "busstop_start_id=" + "0" + "&busstop_end_id=" + "0" + "&bus_type=" + buslinePick.Tag + "&running_type=" + busRunningPick.Tag + "&orderby=" + "";
                }
                else
                {
                    myParameters = "busstop_start_id=" + itemstart.id + "&busstop_end_id=" + itemend.id + "&bus_type=" + buslinePick.Tag + "&running_type=" + busRunningPick.Tag + "&orderby=" + "";
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

        private void txtboxstart_TextChanged(object sender, RoutedEventArgs e)
        {
            if (txtboxstart.Text.Length > 2)
            {
                if (!alreadyStart)
                {
                    ShowProgressIndicator("Loading..");
                    alreadyStart = true;
                    callServicegetAutocompletestart();
                }
            }
        }
        private void txtboxend_TextChanged(object sender, RoutedEventArgs e)
        {
            if (txtboxend.Text.Length > 2)
            {
                if (!alreadyEnd)
                {
                    ShowProgressIndicator("Loading..");
                    alreadyEnd = true;
                    callServicegetAutocompleteend();
                }
            }
        }
        bool SearchTextstart(string search, object value)
        {
            if (value != null)
            {
                return true;
            }
            //... If no match, return false. 
            return false;
        }
        bool SearchTextend(string search, object value)
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
            (Application.Current as App).DataStartStop = results;
            this.NavigationService.Navigate(new Uri("/BMTA_BusStartStop.xaml?TextFrom=" + txtboxstart.Text + "&TextTo=" + txtboxend.Text, UriKind.Relative));
        }

        public void callServicegetAutocompletestart()
        {
            webClient = new WebClient();
            webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            String url = "http://202.6.18.31:7777/getAutocomplete";
            string myParameters;
            try
            {
                myParameters = "type=" + "busstop" + "&keyword=" + txtboxstart.Text + "&lang=" + lang;
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
            searchStartStopItem results = JsonConvert.DeserializeObject<searchStartStopItem>(e.Result);

            txtboxstart.ItemsSource = results.data;

            HideProgressIndicator();
            alreadyStart = false;
        }


        public void callServicegetAutocompleteend()
        {
            webClient = new WebClient();
            webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            String url = "http://202.6.18.31:7777/getAutocomplete";
            string myParameters;
            try
            {
                myParameters = "type=" + "busstop" + "&keyword=" + txtboxend.Text + "&lang=" + lang;
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
            searchStartStopItem results = JsonConvert.DeserializeObject<searchStartStopItem>(e.Result);

            txtboxend.ItemsSource = results.data;

            HideProgressIndicator();
            alreadyEnd = false;
        }
        //
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

        private void txtboxstart_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            itemstart = (sender as AutoCompleteBox).SelectedItem as searchStartStopDetailItem;
        }

        private void txtboxend_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            itemend = (sender as AutoCompleteBox).SelectedItem as searchStartStopDetailItem;
        }

    }
}