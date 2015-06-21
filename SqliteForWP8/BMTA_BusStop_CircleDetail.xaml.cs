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
using BMTA.Item;
using Newtonsoft.Json;
using System.Diagnostics;


namespace BMTA
{
    public partial class BMTA_BusStop_CircleDetail : PhoneApplicationPage
    {
        public String lang = (Application.Current as App).Language;
        private SQLiteConnection dbConn;
        Boolean alreadyBusStop = false;
        ProgressIndicator progressIndicator = new ProgressIndicator();
        static WebClient webClient;
        private searchlandmarkAndBusstopdetailItem itemBusStop;
        dataNearBusStopItem data;
        List<buslineItem> retrievedTasks;
        int indexData = 0;
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {

            // Create the database connection.  
            dbConn = new SQLiteConnection(App.DB_PATH);

            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (dbConn != null)
            {
                dbConn.Close();
            }
            /// Close the database connection.
        }


        public BMTA_BusStop_CircleDetail()
        {
            InitializeComponent();
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

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            data = (Application.Current as App).DataBusstopDetail;

            textBuslineGroup.Text = data.busline;
            List<string> buslines = data.busline.Split(',').ToList<string>();

            String q = "SELECT * FROM busline WHERE bus_line = ";
            int count = 0;
            foreach (string line in buslines)
            {
                if (count < 1)
                {
                    q = q + "'" + line + "'";
                }
                else
                {
                    q = q + " OR bus_line = '" + line + "'";
                }
                count++;
            }
            q = q + "AND (bus_direction LIKE '%เข้าเมือง%' OR bus_direction LIKE '%วนซ้าย%') Group By bus_line";

            retrievedTasks = dbConn.Query<buslineItem>(q);

            if (lang.Equals("th"))
            {
                titleName.Text = "ป้ายหยุดรถ";
                textName.Text = data.stop_name;
            }
            else
            {
                titleName.Text = "Bus Stop";
                textName.Text = data.stop_name_en;
            }

            selectData(indexData);
        }

        private void selectData(int index)
        {
            if (lang.Equals("th"))
            {

                lblbustype.Text = retrievedTasks[index].bus_name;
            }
            else
            {
                lblbustype.Text = retrievedTasks[index].bus_name_en;
            }
            lbltime.Text = retrievedTasks[index].bus_startstop_time;
            busno_bg.Content = retrievedTasks[index].bus_line;
        }

        private void btback_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void btleft_Click(object sender, RoutedEventArgs e)
        {
            if (indexData != 0)
            {
                indexData--;
                selectData(indexData);
            }
        }

        private void btrigth_Click(object sender, RoutedEventArgs e)
        {
            if (indexData == retrievedTasks.Count - 1)
            {
                selectData(indexData);
            }
            else
            {
                indexData++;
                selectData(indexData);
            }
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

        public void callServicegetAutocompleteBusStop()
        {
            webClient = new WebClient();
            webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            String url = "http://202.6.18.31:7777/getAutocomplete";
            string myParameters;
            try
            {
                myParameters = "type=" + "busstop" + "&keyword=" + busstop_search.Text + "&lang=" + lang;
                Debug.WriteLine("URL StreetsandLandmarks_search_SelectionChanged = " + url);
                webClient.UploadStringCompleted += new UploadStringCompletedEventHandler(callServicegetAutocompleteBusStop_Completed);
                webClient.UploadStringAsync(new Uri(url), myParameters);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void callServicegetAutocompleteBusStop_Completed(object sender, UploadStringCompletedEventArgs e)
        {
            searchbusstopItem results = JsonConvert.DeserializeObject<searchbusstopItem>(e.Result);

            busstop_search.ItemsSource = results.data;

            HideProgressIndicator();
            alreadyBusStop = false;
        }

        private void busstop_search_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            searchlandmarkAndBusstopdetailItem item = (sender as AutoCompleteBox).SelectedItem as searchlandmarkAndBusstopdetailItem;
            if (item != null)
            {
                itemBusStop = item;
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
                    //myParameters = "lat=" + "13.741709" + "&long=" + "100.420125" + "&busstop_end_id=" + "4101" + "&bus_type=&running_type=&orderby=" + "";
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
            (Application.Current as App).DataStop = results;

            this.NavigationService.Navigate(new Uri("/BMTA_BusStopDetailBus.xaml?Search=false&TextFrom=" + busstop_search.Text, UriKind.Relative));
        }

        private void busstop_search_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (string.IsNullOrWhiteSpace(busstop_search.Text))
                {
                    MessageBox.Show("กรุณาใส่ป้ายรถเมล์ที่ต้องการ");
                    return;
                }
                callService_busstop_currentfindRouting();
            }
        }

    }
}