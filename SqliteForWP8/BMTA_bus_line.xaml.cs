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

namespace BMTA
{
    public partial class BMTA_bus_line : PhoneApplicationPage
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
        bool Menu4 = true;
        public String CurrentGroup = "1";
        /// <summary>
        /// busline
        /// </summary>
        ObservableCollection<Article> articles = new ObservableCollection<Article>();

        /// <summary>
        /// map
        /// </summary>
        UCToolTip _stooltip = new UCToolTip();
        MapLayer mymapLayer = new MapLayer();

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            // Create the database connection.  
            dbConn = new SQLiteConnection(App.DB_PATH);
            // Create the table Task, if it doesn't exist.  
            dbConn.CreateTable<busline>();

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

            base.OnNavigatedTo(e);

        }

        private void callServiceBusline()
        {
            //load data 
            ShowProgressIndicator("Loading..");
            List<busline> retrievedTasks = dbConn.Query<busline>("SELECT * FROM busline WHERE bus_line LIKE '" + CurrentGroup + "%' AND (bus_direction LIKE '%เข้าเมือง%' OR bus_direction LIKE '%วนซ้าย%')");

            foreach (var item in retrievedTasks)
            {
                Article article = new Article();
                article.btcontent = item.bus_line;
                article.Start = item.bus_start;
                article.Stop = item.bus_end;
                article.ImagePath = "Assets/arrow.png";

                if (item.bustype == "1")
                {
                    article.bg = "Assets/bg_nomal.png";
                }
                else if (item.bustype == "2")
                {
                    article.bg = "Assets/bg_nomal2.png";
                }
                articles.Add(article);
            }
            buslinelistbox.ItemsSource = articles;
            HideProgressIndicator();
            this.buslinelistbox.Visibility = System.Windows.Visibility.Visible;
        }

        public void callServicegetNearBusStop(String lat, String lon)
        {
            webClient = new WebClient();
            String url = null;
            try
            {
                url = (Application.Current as App).getNearBusStop + "lat=" + lat + "&lon=" + lon + "&distance=" + "12";
                Debug.WriteLine("URL callServicegetNearBusStop = " + url);
                webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(callServicgetNearBusStop_Completed);
                webClient.DownloadStringAsync(new Uri(url));

            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void callServicgetNearBusStop_Completed(object sender, DownloadStringCompletedEventArgs e)
        {
            NearBusStopItem results = JsonConvert.DeserializeObject<NearBusStopItem>(e.Result);
            String status = results.status;

            var datas = results.data;
            foreach (var data in datas)
            {
                String stop_name = data.stop_name;
                Debug.WriteLine(data);

                //add Tooltip
                MapOverlay mapoverlay = new MapOverlay();
                _stooltip = new UCToolTip();
                _stooltip.Description = data.stop_name.ToString() + "\n" + data.busline;
                _stooltip.DataContext = data;
                mapoverlay.Content = _stooltip;
                mapoverlay.GeoCoordinate = new GeoCoordinate(Convert.ToDouble(data.latitude), Convert.ToDouble(data.longitude));
                mymapLayer.Add(mapoverlay);
            }
        }

        private void pushtap_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Pushpin pushpin = sender as Pushpin;
            if (pushpin.Content != null)
            {


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

        public BMTA_bus_line()
        {
            InitializeComponent();
            ShowMyLocationOnTheMap();
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
            this.map.ZoomLevel = 18;

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
            mymapLayer = new MapLayer();
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
            rightmenu.Visibility = System.Windows.Visibility.Collapsed;
            rightmenux.Visibility = System.Windows.Visibility.Collapsed;
            close.Visibility = System.Windows.Visibility.Collapsed;
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
            }
            else
            {
                titleName.Text = "Streets and Landmarks";
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

                ShowProgressIndicator("Loading..");
                Menu2 = false;
            }

            setViewBusstop();
        }
        private void btn_bottom_3(object sender, EventArgs e)
        {
            if (Menu3)
            {
                Menu3 = false;
            }
            setViewStreetAndLandMarks();
        }
        private void btn_bottom_4(object sender, EventArgs e)
        {
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
                NavigationService.Navigate(new Uri("/BMTA_Search_Advance_start.xaml", UriKind.Relative));
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
                this.NavigationService.Navigate(new Uri("/BMTA_bus_line_details.xaml?Group=" + CurrentGroup + "&Index=" + buslinelistbox.SelectedIndex.ToString(), UriKind.Relative));
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
            Button btn = (Button)sender;

            ShowProgressIndicator("Loading..");
            articles.Clear();

            List<busline> retrievedTasks = new List<busline>();
            if (btn.Name == "btn_other")
            {
                retrievedTasks = dbConn.Query<busline>("SELECT * FROM busline WHERE bus_line GLOB '*[A-Za-z]*' AND (bus_direction LIKE '%เข้าเมือง%' OR bus_direction LIKE '%วนซ้าย%')");
            }
            else if (btn.Name == "btn_van")
            {
                retrievedTasks = dbConn.Query<busline>("SELECT * FROM busline WHERE bus_owner = 4 AND (bus_direction LIKE '%เข้าเมือง%'  OR bus_direction LIKE '%วนซ้าย%')");
            }
            else
            {
                retrievedTasks = dbConn.Query<busline>("SELECT * FROM busline WHERE bus_line LIKE '" + btn.Content + "%' AND (bus_direction LIKE '%เข้าเมือง%' OR bus_direction LIKE '%วนซ้าย%')");
            }

            foreach (var item in retrievedTasks)
            {
                Article article = new Article();
                article.btcontent = item.bus_line;
                article.Start = item.bus_start;
                article.Stop = item.bus_end;
                article.ImagePath = @"Assets/arrow.png";

                if (item.bustype == "1")
                {
                    article.bg = @"Assets/bg_nomal.png";
                }
                else if (item.bustype == "2")
                {
                    article.bg = @"Assets/bg_nomal2.png";
                }
                articles.Add(article);
            }
            buslinelistbox.ItemsSource = articles;
            HideProgressIndicator();
            CurrentGroup = btn.Content.ToString();
        }

        private void Button_Click_11(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_bus_line.xaml", UriKind.Relative));
        }
        private void Button_Click_12(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_BusStop.xaml", UriKind.Relative));
            NavigationService.RemoveBackEntry();
        }
        private void Button_Click_13(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_BusCoordinates.xaml", UriKind.Relative));
        }
        private void Button_Click_14(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_BusStartStop.xaml", UriKind.Relative));
        }
        private void close_Click(object sender, RoutedEventArgs e)
        {
            rightmenu.Visibility = System.Windows.Visibility.Collapsed;
            rightmenux.Visibility = System.Windows.Visibility.Collapsed;
            close.Visibility = System.Windows.Visibility.Collapsed;
        }
        private void btTopMenu_Click(object sender, RoutedEventArgs e)
        {
            rightmenux.Visibility = Visibility;
            rightmenu.Visibility = Visibility;
            close.Visibility = Visibility;
        }
        private void rhome_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }
        private void rbusline_Click(object sender, RoutedEventArgs e)
        {
        }
        private void rbusstop_Click(object sender, RoutedEventArgs e)
        {
        }
        private void rcoor_Click(object sender, RoutedEventArgs e)
        {
        }
        private void rbusstartstop_Click(object sender, RoutedEventArgs e)
        {
        }
        private void rbusspeed_Click(object sender, RoutedEventArgs e)
        {
        }
        private void rbusnew_Click(object sender, RoutedEventArgs e)
        {
        }

        private void busline_search_TextChanged(object sender, TextChangedEventArgs e)
        {
            //load data 
            ShowProgressIndicator("Loading..");
            articles.Clear();
            List<busline> retrievedTasks = new List<busline>();
            if (busline_search.Text != null || busline_search.Text != "")
            {
                retrievedTasks = dbConn.Query<busline>("SELECT * FROM busline WHERE bus_line LIKE '" + busline_search.Text + "%' AND (bus_direction LIKE '%เข้าเมือง%' OR bus_direction LIKE '%วนซ้าย%')");
            }
            else
            {
                retrievedTasks = dbConn.Query<busline>("SELECT * FROM busline WHERE bus_line LIKE '" + CurrentGroup + "%' AND (bus_direction LIKE '%เข้าเมือง%' OR bus_direction LIKE '%วนซ้าย%')");
            }

            foreach (var item in retrievedTasks)
            {
                Article article = new Article();
                article.btcontent = item.bus_line;
                article.Start = item.bus_start;
                article.Stop = item.bus_end;
                article.ImagePath = "Assets/arrow.png";

                if (item.bustype == "1")
                {
                    article.bg = "Assets/bg_nomal.png";
                }
                else if (item.bustype == "2")
                {
                    article.bg = "Assets/bg_nomal2.png";
                }
                articles.Add(article);
            }
            buslinelistbox.ItemsSource = articles;
            HideProgressIndicator();
        }

        private void busline_search_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                //load data 
                ShowProgressIndicator("Loading..");
                articles.Clear();
                List<busline> retrievedTasks = new List<busline>();
                if (busline_search.Text != "")
                {
                    retrievedTasks = dbConn.Query<busline>("SELECT * FROM busline WHERE bus_line LIKE '" + busline_search.Text + "%' AND (bus_direction LIKE '%เข้าเมือง%' OR bus_direction LIKE '%วนซ้าย%')");
                }
                else
                {
                    retrievedTasks = dbConn.Query<busline>("SELECT * FROM busline WHERE bus_line LIKE '" + CurrentGroup + "%' AND (bus_direction LIKE '%เข้าเมือง%' OR bus_direction LIKE '%วนซ้าย%')");
                }

                foreach (var item in retrievedTasks)
                {
                    Article article = new Article();
                    article.btcontent = item.bus_line;
                    article.Start = item.bus_start;
                    article.Stop = item.bus_end;
                    article.ImagePath = "Assets/arrow.png";

                    if (item.bustype == "1")
                    {
                        article.bg = "Assets/bg_nomal.png";
                    }
                    else if (item.bustype == "2")
                    {
                        article.bg = "Assets/bg_nomal2.png";
                    }
                    articles.Add(article);
                }
                buslinelistbox.ItemsSource = articles;
                HideProgressIndicator();
            }
        }


    }
}