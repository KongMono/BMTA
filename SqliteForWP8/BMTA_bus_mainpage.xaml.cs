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
using Windows.Devices.Geolocation;
using System.Device.Location;
using Microsoft.Phone.Maps.Services;
using Microsoft.Phone.Maps.Controls;
using BMTA.Usercontrols;
using System.Text;
using System.Windows.Shapes;
using Microsoft.Phone.Maps.Toolkit;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using BMTA.Item;
using Coding4Fun.Toolkit.Controls;

namespace BMTA
{
    public partial class BMTA_bus_mainpage : PhoneApplicationPage
    {
        /// <summary>
        /// other
        /// </summary>
        public String lang = (Application.Current as App).Language;
        ProgressIndicator progressIndicator = new ProgressIndicator();
        private SQLiteConnection dbConn;
        public string CurrentPage;
        static WebClient webClient;
        bool Menu1 = true;
        bool Menu2 = true;
        bool Menu3 = true;
        Boolean alreadyStart = false;
        Boolean alreadyEnd = false;
        Boolean alreadyBusStop = false;
        Boolean alreadyLandMark = false;
        String currentBtn = "btn1";
        private searchStartStopDetailItem itemstart, itemend;
        private searchlandmarkAndBusstopdetailItem itemLandMark, itemBusStop;
        public String CurrentGroup = "1";
        /// <summary>
        /// busline
        /// </summary>
        ObservableCollection<buslineItem> buslines = new ObservableCollection<buslineItem>();

        /// <summary>
        /// map
        /// </summary>
        UCToolTip _stooltip = new UCToolTip();
        MapLayer mymapLayer = new MapLayer();
        NearBusStopItem NearBusStopResults = new NearBusStopItem();
        UCLandMarkDialog dialog = new UCLandMarkDialog();

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            dbConn = new SQLiteConnection(App.DB_PATH);

            if (e.NavigationMode == System.Windows.Navigation.NavigationMode.New)
            {
                CurrentPage = this.NavigationContext.QueryString["busCate"];

                if (CurrentPage == "1")
                {
                    if (Menu1)
                    {
                        this.callServiceBusline();
                        Menu1 = false;
                    }
                    setViewBusline();
                }
                else if (CurrentPage == "2")
                {
                    setViewBusstop();
                }
                else if (CurrentPage == "3")
                {
                    setViewStartEnd();
                }
                else
                {
                    setViewStreetAndLandMarks();
                }
            }

            HideProgressIndicator();

            base.OnNavigatedTo(e);

        }

        private void callServiceBusline()
        {
            //load data 
            ShowProgressIndicator("Loading..");
            Dispatcher.BeginInvoke(() =>
            {
                try
                {
                    List<buslineItem> retrievedTasks = dbConn.Query<buslineItem>("SELECT * FROM busline WHERE bus_line LIKE '" + CurrentGroup + "%' AND (bus_direction LIKE '%เข้าเมือง%' OR bus_direction LIKE '%วนซ้าย%')");
                    buslines = new ObservableCollection<buslineItem>(retrievedTasks);
                    (Application.Current as App).DataBuslinehList = retrievedTasks;

                    buslinelistbox.ItemsSource = buslines;
                    HideProgressIndicator();
                    this.buslinelistbox.Visibility = System.Windows.Visibility.Visible;
                }catch(Exception ex){
                    MessageBox.Show(ex.Message);
                }
            });
        }

        public void callServicegetNearBusStop(String lat, String lon)
        {
            progressBar_busstop.Visibility = System.Windows.Visibility.Visible;

            webClient = new WebClient();
            webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            String url = "http://202.6.18.31:7777/getNearBusstop";
            string myParameters;
            try
            {
                (Application.Current as App).lat_current = lat;
                (Application.Current as App).lon_current = lon;

                myParameters = "lat=" + lat + "&long=" + lon + "&distance=" + "12";
                Debug.WriteLine("URL callServicegetNearBusStop = " + url);

                webClient.UploadStringCompleted += new UploadStringCompletedEventHandler(callServicgetNearBusStop_Completed);
                webClient.UploadStringAsync(new Uri(url), myParameters);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void callServicgetNearBusStop_Completed(object sender, UploadStringCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                HideProgressIndicator();
                progressBar_busstop.Visibility = System.Windows.Visibility.Collapsed;
                return;
            }
            NearBusStopResults = JsonConvert.DeserializeObject<NearBusStopItem>(e.Result);
            if (NearBusStopResults != null)
            {
                String status = NearBusStopResults.status;
                MapLayer layer = new MapLayer();
                if (status != "0")
                {
                    var datas = NearBusStopResults.data;
                    foreach (var data in datas)
                    {
                        String stop_name = data.stop_name;
                        Debug.WriteLine(data);

                        //add Tooltip
                        MapOverlay mapoverlay = new MapOverlay();

                        UCCustomToolTip _tooltip = new UCCustomToolTip();
                        _tooltip.Description = data.stop_name.ToString() + "\n" + data.busline;
                        _tooltip.DataContext = data;
                        _tooltip.Lbltext.Tap += _tooltip_TapLbltext;

                        mapoverlay.Content = _tooltip;
                        mapoverlay.GeoCoordinate = new GeoCoordinate(Convert.ToDouble(data.latitude), Convert.ToDouble(data.longitude));
                        layer.Add(mapoverlay);
                    }
                }
                this.map.Layers.Add(layer);
            }
            HideProgressIndicator();
            progressBar_busstop.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void _tooltip_TapLbltext(object sender, System.Windows.Input.GestureEventArgs e)
        {
            TextBlock item = (TextBlock)sender;
            string selecteditem = item.Tag.ToString();
            var selected = NearBusStopResults.data.Where(s => s.id == selecteditem).ToList();

            if (selected.Count > 0)
            {
                foreach (var items in selected)
                {
                    (Application.Current as App).DataBusstopDetail = items;
                    NavigationService.Navigate(new Uri("/BMTA_BusStop_Map.xaml", UriKind.Relative));
                }
            }

        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (dbConn != null)
            {
                dbConn.Close();
                // Close the database connection.  
            }
        }

        public BMTA_bus_mainpage()
        {
            InitializeComponent();

            ShowMyLocationOnTheMap();
            busStartStopFrom_search.ItemFilter = SearchText;
            busStartStopTo_search.ItemFilter = SearchText;
            busstop_search.ItemFilter = SearchText;
            StreetsandLandmarks_search.ItemFilter = SearchText;
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

        private async void ShowMyLocationOnTheMap()
        {
            // Get my current location.
            Geolocator myGeolocator = new Geolocator();
            Geoposition myGeoposition = await myGeolocator.GetGeopositionAsync(TimeSpan.FromMinutes(5), TimeSpan.FromSeconds(5));
            Geocoordinate myGeocoordinate = myGeoposition.Coordinate;
            GeoCoordinate myGeoCoordinate = CoordinateConverter.ConvertGeocoordinate(myGeocoordinate);

            this.callServicegetNearBusStop(Convert.ToString(myGeoCoordinate.Latitude), Convert.ToString(myGeoCoordinate.Longitude));

            this.map.Center = myGeoCoordinate;
            this.map.ZoomLevel = 13;

            // Create a small circle to mark the current location.
            Ellipse myCircle = new Ellipse();
            myCircle.Fill = new SolidColorBrush(Colors.Blue);
            myCircle.Height = 20;
            myCircle.Width = 20;
            myCircle.Opacity = 50;

            // Create a MapOverlay to contain the circle.
            MapOverlay myLocationOverlay = new MapOverlay();
            myLocationOverlay.Content = myCircle;
            myLocationOverlay.GeoCoordinate = myGeoCoordinate;

            // Create a MapLayer to contain the MapOverlay.
            mymapLayer.Add(myLocationOverlay);

            // Add the MapLayer to the Map.
            this.map.Layers.Add(mymapLayer);

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
            List<datasearchLandMarkByGeoItem> listHistory = (Application.Current as App).MemLandMarkList;

            foreach (datasearchLandMarkByGeoItem item in listHistory)
            {
                UCLandMarkItem i = new UCLandMarkItem();
                i.TextLandMark.Text = item.keyword;
                LandmarksHistorylistbox.Items.Add(item);
            }
        }

        private void setViewBusline()
        {
            CurrentPage = "1";
            this.buslineLayout.Visibility = System.Windows.Visibility.Visible;
            this.busstopLayout.Visibility = System.Windows.Visibility.Collapsed;
            this.busStartStopLayout.Visibility = System.Windows.Visibility.Collapsed;
            this.StreetsandLandmarksLayout.Visibility = System.Windows.Visibility.Collapsed;

            if (lang.Equals("th"))
            {
                titleName.Text = "สายรถเมล์";
            }
            else
            {
                titleName.Text = "Bus line";
            }
        }

        private void setViewBusstop()
        {
            CurrentPage = "2";
            this.buslineLayout.Visibility = System.Windows.Visibility.Collapsed;
            this.busstopLayout.Visibility = System.Windows.Visibility.Visible;
            this.busStartStopLayout.Visibility = System.Windows.Visibility.Collapsed;
            this.StreetsandLandmarksLayout.Visibility = System.Windows.Visibility.Collapsed;

            if (lang.Equals("th"))
            {
                titleName.Text = "ป้ายรถเมล์";
            }
            else
            {
                titleName.Text = "Bus Stop";
            }
        }

        private void setViewStartEnd()
        {
            CurrentPage = "3";
            this.buslineLayout.Visibility = System.Windows.Visibility.Collapsed;
            this.busstopLayout.Visibility = System.Windows.Visibility.Collapsed;
            this.busStartStopLayout.Visibility = System.Windows.Visibility.Visible;
            this.StreetsandLandmarksLayout.Visibility = System.Windows.Visibility.Collapsed;

            if (lang.Equals("th"))
            {
                titleName.Text = "ต้นทางปลายทาง";
            }
            else
            {
                titleName.Text = "Start - End";
            }
        }

        private void setViewStreetAndLandMarks()
        {
            CurrentPage = "4";
            this.buslineLayout.Visibility = System.Windows.Visibility.Collapsed;
            this.busstopLayout.Visibility = System.Windows.Visibility.Collapsed;
            this.busStartStopLayout.Visibility = System.Windows.Visibility.Collapsed;
            this.StreetsandLandmarksLayout.Visibility = System.Windows.Visibility.Visible;
            if (lang.Equals("th"))
            {
                titleName.Text = "ถนนและสถานที่สำคัญ";
                textHeaderLandMark.Text = "ผลการค้นหาโดยใช้พิกัด";
            }
            else
            {
                titleName.Text = "Streets and Landmarks";
                textHeaderLandMark.Text = "Recent";
            }
        }

        private void btn_bottom_1(object sender, EventArgs e)
        {
            if (Menu1)
            {
                this.callServiceBusline();
                ShowProgressIndicator("Loading..");
                Menu1 = false;
            }
            setViewBusline();
        }
        private void btn_bottom_2(object sender, EventArgs e)
        {
            if (Menu2)
            {
                Menu2 = false;
            }

            setViewBusstop();
        }
        private void btn_bottom_3(object sender, EventArgs e)
        {
            if (Menu3)
            {
                HideProgressIndicator();
                Menu3 = false;
            }
            setViewStreetAndLandMarks();
        }
        private void btn_bottom_4(object sender, EventArgs e)
        {
            HideProgressIndicator();
            setViewStartEnd();
        }

        private void btSearch_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPage == "1")
            {
                NavigationService.Navigate(new Uri("/BMTA_SearchAdvance.xaml", UriKind.Relative));
            }
            else if (CurrentPage == "2")
            {
                NavigationService.Navigate(new Uri("/BMTA_Search_Advance_busstop.xaml", UriKind.Relative));
            }
            else if (CurrentPage == "3")
            {
                NavigationService.Navigate(new Uri("/BMTA_Search_Advance_startend.xaml", UriKind.Relative));
            }
            else
            {
                NavigationService.Navigate(new Uri("/BMTA_Search_Advance_landmark.xaml", UriKind.Relative));
            }
        }

        private void buslinelistbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Article item = (sender as ListBox).SelectedItem as Article;
            if (buslinelistbox.SelectedIndex != -1)
            {
                this.NavigationService.Navigate(new Uri("/BMTA_bus_line_details.xaml?Search=false&Index=" + buslinelistbox.SelectedIndex.ToString(), UriKind.Relative));
            }
            buslinelistbox.SelectedIndex = -1;
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            this.NavigationService.GoBack();
            base.OnBackKeyPress(e);
        }

        private void BtnNumber_Click(object sender, RoutedEventArgs e)
        {
            RoundToggleButton btn = (RoundToggleButton)sender;
            progressBar_busline.Visibility = System.Windows.Visibility.Visible;
            progressBar_busline.IsIndeterminate = true;
            ShowProgressIndicator("Loading..");
            //articles.Clear();

            List<buslineItem> retrievedTasks = new List<buslineItem>();
            if (btn.Name == "btn_other")
            {
                retrievedTasks = dbConn.Query<buslineItem>("SELECT * FROM busline WHERE bus_line GLOB '*[A-Za-z]*' AND (bus_direction LIKE '%เข้าเมือง%' OR bus_direction LIKE '%วนซ้าย%')");
            }
            else if (btn.Name == "btn_van")
            {
                retrievedTasks = dbConn.Query<buslineItem>("SELECT * FROM busline WHERE bus_owner = 4 AND (bus_direction LIKE '%เข้าเมือง%'  OR bus_direction LIKE '%วนซ้าย%')");
            }
            else
            {
                retrievedTasks = dbConn.Query<buslineItem>("SELECT * FROM busline WHERE bus_line LIKE '" + btn.Content + "%' AND (bus_direction LIKE '%เข้าเมือง%' OR bus_direction LIKE '%วนซ้าย%')");
            }

            (Application.Current as App).DataBuslinehList = retrievedTasks;
            buslines = new ObservableCollection<buslineItem>(retrievedTasks);

            buslinelistbox.ItemsSource = buslines;
            HideProgressIndicator();
            progressBar_busline.IsIndeterminate = false;
            progressBar_busline.Visibility = System.Windows.Visibility.Collapsed;
            CurrentGroup = btn.Content.ToString();
        }

        private void btTopMenu_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }
        private void rhome_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }

        private void busline_search_TextChanged(object sender, TextChangedEventArgs e)
        {
            //load data 
            progressBar_busline.Visibility = System.Windows.Visibility.Visible;
            //articles.Clear();
            List<buslineItem> retrievedTasks = new List<buslineItem>();
            if (busline_search.Text != null || busline_search.Text != "")
            {
                retrievedTasks = dbConn.Query<buslineItem>("SELECT * FROM busline WHERE bus_line LIKE '" + busline_search.Text + "%' AND (bus_direction LIKE '%เข้าเมือง%' OR bus_direction LIKE '%วนซ้าย%')");
            }
            else
            {
                retrievedTasks = dbConn.Query<buslineItem>("SELECT * FROM busline WHERE bus_line LIKE '" + CurrentGroup + "%' AND (bus_direction LIKE '%เข้าเมือง%' OR bus_direction LIKE '%วนซ้าย%')");
            }

            buslines = new ObservableCollection<buslineItem>(retrievedTasks);
            buslinelistbox.ItemsSource = buslines;
            progressBar_busline.Visibility = System.Windows.Visibility.Collapsed;
            HideProgressIndicator();
        }

        private void busline_search_KeyDown(object sender, KeyEventArgs e)
        {
            progressBar_busline.Visibility = System.Windows.Visibility.Visible;

            if (e.Key == Key.Enter)
            {
                ShowProgressIndicator("Loading..");
                List<buslineItem> retrievedTasks = new List<buslineItem>();
                if (busline_search.Text != "")
                {
                    retrievedTasks = dbConn.Query<buslineItem>("SELECT * FROM busline WHERE bus_line LIKE '" + busline_search.Text + "%' AND (bus_direction LIKE '%เข้าเมือง%' OR bus_direction LIKE '%วนซ้าย%')");
                }
                else
                {
                    retrievedTasks = dbConn.Query<buslineItem>("SELECT * FROM busline WHERE bus_line LIKE '" + CurrentGroup + "%' AND (bus_direction LIKE '%เข้าเมือง%' OR bus_direction LIKE '%วนซ้าย%')");
                }

                (Application.Current as App).DataBuslinehList = retrievedTasks;
                buslines = new ObservableCollection<buslineItem>(retrievedTasks);
                buslinelistbox.ItemsSource = buslines;
                HideProgressIndicator();
                progressBar_busline.Visibility = System.Windows.Visibility.Collapsed;
            }
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
            ShowProgressIndicator("Loading..");
            callService_startstop_searchfindRouting();
        }

        public void callService_startstop_searchfindRouting()
        {
            ShowProgressIndicator("Loading..");
            progressBar_busstartstop.Visibility = System.Windows.Visibility.Visible;

            webClient = new WebClient();
            webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            String url = "http://202.6.18.31:7777/searchfindRouting";
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
          
            searchfindRoutingItem results = JsonConvert.DeserializeObject<searchfindRoutingItem>(e.Result);
            if (results == null)
            {
                MessageBox.Show("ไม่พบข้อมูล");
                progressBar_busstartstop.Visibility = System.Windows.Visibility.Collapsed;
                return;
            }
            if (results.status == "0")
            {
                MessageBox.Show("ไม่พบข้อมูล");
                progressBar_busstartstop.Visibility = System.Windows.Visibility.Collapsed;
                return;
            }
            UCStartStop UCStartStop = new UCStartStop();
            foreach (var item in results.data)
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

               
                busStartStoplistbox.Items.Add(UCStartStop);
            }
            progressBar_busstartstop.Visibility = System.Windows.Visibility.Collapsed;
            HideProgressIndicator();
        }

        public void callServicegetAutocompleteLandMark()
        {
            ShowProgressIndicator("Loading..");
            progressBar_landmark.Visibility = System.Windows.Visibility.Visible;

            webClient = new WebClient();
            String url = "http://128.199.232.94/webservice/keyword.php";
            string myParameters;
            try
            {
                myParameters = url + "?type=" + "place" + "&q=" + StreetsandLandmarks_search.Text + "&lang=" + lang;
                Debug.WriteLine("URL callServicegetAutocompleteLandMark = " + myParameters);
                webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(callServicegetAutocomplete_Completed);
                webClient.DownloadStringAsync(new Uri(myParameters));
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void callServicegetAutocomplete_Completed(object sender, DownloadStringCompletedEventArgs e)
        {
            progressBar_landmark.Visibility = System.Windows.Visibility.Collapsed;

            searchlandmarkItem results = JsonConvert.DeserializeObject<searchlandmarkItem>(e.Result);

            StreetsandLandmarks_search.ItemsSource = results.data;

            HideProgressIndicator();
            alreadyLandMark = false;
        }

        private void StreetsandLandmarks_search_TextChanged(object sender, RoutedEventArgs e)
        {
            if (StreetsandLandmarks_search.Text.Length > 2)
            {
                if (!alreadyLandMark)
                {
                    ShowProgressIndicator("Loading..");
                    alreadyLandMark = true;
                    callServicegetAutocompleteLandMark();
                }
            }
        }

        private void StreetsandLandmarks_search_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            searchlandmarkAndBusstopdetailItem item = (sender as AutoCompleteBox).SelectedItem as searchlandmarkAndBusstopdetailItem;
            if (item != null)
            {
                itemLandMark = item;
            }
        }

        private void StreetsandLandmarks_search_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (string.IsNullOrWhiteSpace(StreetsandLandmarks_search.Text))
                {
                    MessageBox.Show("กรุณาใส่ป้ายรถเมล์ที่ต้องการ");
                    return;
                }
                callplacecurrentfindRouting();
            }
        }

        public void callplacecurrentfindRouting()
        {
            ShowProgressIndicator("Loading..");
            progressBar_landmark.Visibility = System.Windows.Visibility.Visible;

            webClient = new WebClient();
            webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            String url = "http://202.6.18.31:7777/placecurrentfindRouting";
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

        public void callplacecurrentfindRouting_Memo(String lat, String lon)
        {
            webClient = new WebClient();
            webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            String url = "http://202.6.18.31:7777/placecurrentfindRouting";
            string myParameters;
            try
            {
                myParameters = "lat=" + (Application.Current as App).lat_current + "&long=" + (Application.Current as App).lon_current + "&elatlong=" + lat + "-" + lon + "&bus_type=" + "" + "&running_type=" + "" + "&orderby=" + "";

                Debug.WriteLine("URL callplacecurrentfindRouting_Memo = " + url);
                webClient.UploadStringCompleted += new UploadStringCompletedEventHandler(callplacecurrentfindRouting_Memo_Completed);
                webClient.UploadStringAsync(new Uri(url), myParameters);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void callplacecurrentfindRouting_Memo_Completed(object sender, UploadStringCompletedEventArgs e)
        {
            HideProgressIndicator();
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
            datasearchLandMarkByGeoItem dataSearchDialog = new datasearchLandMarkByGeoItem();
            dataSearchDialog.keyword = dialog.Textbox1.Text;
            dataSearchDialog.data = results;
            (Application.Current as App).MemLandMarkList.Add(dataSearchDialog);
            (Application.Current as App).DataLandMark = results;

            this.NavigationService.Navigate(new Uri("/BMTA_BusLandMarkDetailBus.xaml?TextFrom=" + StreetsandLandmarks_search.Text, UriKind.Relative));
        }

        private void callplacecurrentfindRouting_Completed(object sender, UploadStringCompletedEventArgs e)
        {
            searchfindRoutingItem results = JsonConvert.DeserializeObject<searchfindRoutingItem>(e.Result);
            if (results == null)
            {
                MessageBox.Show("ไม่พบข้อมูล");
                progressBar_landmark.Visibility = System.Windows.Visibility.Collapsed;
                HideProgressIndicator();
                return;
            }
            if (results.status == "0")
            {
                MessageBox.Show("ไม่พบข้อมูล");
                progressBar_landmark.Visibility = System.Windows.Visibility.Collapsed;
                HideProgressIndicator();
                return;
            }
            (Application.Current as App).DataLandMark = results;
            HideProgressIndicator();
            progressBar_landmark.Visibility = System.Windows.Visibility.Collapsed;

            this.NavigationService.Navigate(new Uri("/BMTA_BusLandMarkDetailBus.xaml?TextFrom=" + StreetsandLandmarks_search.Text, UriKind.Relative));
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

            if (e.Error != null)
            {
                MessageBox.Show(e.Result);
                return;
            }
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
            progressBar_busstop.Visibility = System.Windows.Visibility.Visible;

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
            HideProgressIndicator();
            progressBar_busstop.Visibility = System.Windows.Visibility.Collapsed;

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

        private void busStartStoplistbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (busStartStoplistbox.SelectedIndex != -1)
            {
                UCStartStop item = (sender as ListBox).SelectedItem as UCStartStop;
                (Application.Current as App).RountingDataStartStop = (searchfindRoutingItem_data)item.DataContext;

                this.NavigationService.Navigate(new Uri("/BMTA_BusStartStopDetailMap.xaml?TextFrom=" + busStartStopFrom_search.Text + "&TextTo=" + busStartStopTo_search.Text, UriKind.Relative));
            }
            busStartStoplistbox.SelectedIndex = -1;
        }

        private void btn_Checked(object sender, RoutedEventArgs e)
        {
            RoundToggleButton item = (RoundToggleButton)sender;
            if (item != null)
            {
                if (currentBtn == "btn1" && btn1 != null)
                {
                    btn1.IsChecked = false;
                }
                else if (currentBtn == "btn2")
                {
                    btn2.IsChecked = false;
                }
                else if (currentBtn == "btn3")
                {
                    btn3.IsChecked = false;
                }
                else if (currentBtn == "btn4")
                {
                    btn4.IsChecked = false;
                }
                else if (currentBtn == "btn5")
                {
                    btn5.IsChecked = false;
                }
                else if (currentBtn == "btn6")
                {
                    btn6.IsChecked = false;
                }
                else if (currentBtn == "btn7")
                {
                    btn7.IsChecked = false;
                }
                else if (currentBtn == "btn8")
                {
                    btn8.IsChecked = false;
                }
                else if (currentBtn == "btn9")
                {
                    btn9.IsChecked = false;
                }
                else if (currentBtn == "btn_other")
                {
                    btn_other.IsChecked = false;
                }
                else if (currentBtn == "btn_van")
                {
                    btn_van.IsChecked = false;
                }
                currentBtn = item.Name;
            }

        }

        private void latlonbtn_search_Click(object sender, RoutedEventArgs e)
        {
            dialog = new UCLandMarkDialog();
            CustomMessageBox cmb = new CustomMessageBox();
            cmb.Content = dialog;
            cmb.Opacity = 0.7;
            if (lang.Equals("th"))
            {
                cmb.LeftButtonContent = "ค้นหา";
                cmb.RightButtonContent = "ยกเลิก";
            }
            else
            {
                cmb.LeftButtonContent = "search";
                cmb.RightButtonContent = "cancel";
            }

            cmb.Show();
            cmb.Dismissed += (dismissSender, dismissedEvent) =>
            {
                switch (dismissedEvent.Result)
                {
                    case CustomMessageBoxResult.LeftButton:
                        
                        String lat = dialog.Textbox2.Text.Split(',').First();
                        String lon = dialog.Textbox2.Text.Split(',').Last();
                        callplacecurrentfindRouting_Memo(lat, lon);
                        ShowProgressIndicator("Loading..");
                        break;
                    case CustomMessageBoxResult.RightButton:
                        
                        break;
                }
            };
        }
    }
}