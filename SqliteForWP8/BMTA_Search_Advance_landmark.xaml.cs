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

namespace BMTA
{
    public partial class BMTA_Search_Advance_landmark : PhoneApplicationPage
    {
        ProgressIndicator progressIndicator = new ProgressIndicator();
        private searchlandmarkAndBusstopdetailItem item;
        Boolean already = false;
        private WebClient webClient;
        public String lang = (Application.Current as App).Language;
        public BMTA_Search_Advance_landmark()
        {
            InitializeComponent();
            textbox.ItemFilter = SearchText;
        }
        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (lang.Equals("th"))
            {
                head1.Text = "ค้นหาสถานที่สำคัญ";
                head2.Text = "ค้นหาประเภทรถ";
                head3.Text = "เรียงผลการค้นหาตาม";

                textlandmark.Text = "ระบุสถานที่";
                textbusroute.Text = "เส้นทาง";
                textbustype.Text = "ประเภทรถ";
                textsorttype.Text = "ระบุประเภท";

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
                head1.Text = "Search Landmarks";
                head2.Text = "Search Bus Type";
                head3.Text = "Sort By Results";

                textlandmark.Text = "Landmarks";
                textbusroute.Text = "Route";
                textbustype.Text = "Bus Type";
                textsorttype.Text = "Select Type";

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

        private void btback_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }


        private void btsubmit_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textbox.Text))
            {
                MessageBox.Show("กรุณาระบุสถานที่");
                return;
            }

            var buslinePick = (ListPickerItem)busline.SelectedItem;
            var busRunningPick = (ListPickerItem)bustyperunning.SelectedItem;

            callplacecurrentfindRouting(buslinePick, busRunningPick);
        }

        public void callplacecurrentfindRouting(ListPickerItem buslinePick, ListPickerItem busRunningPick)
        {
            webClient = new WebClient();
            webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            String url = "http://202.6.18.31:7777/placecurrentfindRouting";
            string myParameters;
            try
            {
                if (item == null)
                {
                    myParameters = "lat=" + (Application.Current as App).lat_current + "&long=" + (Application.Current as App).lon_current + "&elatlong=" + "0.0-0.0" + "&bus_type=" + buslinePick.Tag + "&running_type=" + busRunningPick.Tag + "&orderby=" + "";
                }
                else
                {
                    myParameters = "lat=" + (Application.Current as App).lat_current + "&long=" + (Application.Current as App).lon_current + "&elatlong=" + item.lattitude + "-" + item.longtitude + "&bus_type=" + buslinePick.Tag + "&running_type=" + busRunningPick.Tag + "&orderby=" + "";
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

        private void callplacecurrentfindRouting_Completed(object sender, UploadStringCompletedEventArgs e)
        {
            MessageBox.Show(e.Result);
        }

        public void callServicegetAutocomplete()
        {
            webClient = new WebClient();
            webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            String url = "http://202.6.18.31:7777/getAutocomplete";
            string myParameters;
            try
            {
                myParameters = "type=" + "place" + "&keyword=" + textbox.Text + "&lang=" + lang;
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
            searchlandmarkItem results = JsonConvert.DeserializeObject<searchlandmarkItem>(e.Result);

            textbox.ItemsSource = results.data;

            HideProgressIndicator();
            already = false;
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