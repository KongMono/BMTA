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
using BMTA.Item;


namespace BMTA
{
    public partial class BMTA_bus_line_details : PhoneApplicationPage
    {
        public String lang = (Application.Current as App).Language;
        private SQLiteConnection dbConn;
        public List<buslineItem> retrievedTasks = new List<buslineItem>();
        public buslineItem retrievedTask = new buslineItem();
        public string group = "";
        public string search, query;
        public int index;
        private MapPolyline line;
        private Boolean isInTwown, showmap;
        private Boolean iscate1, iscate2;
        ObservableCollection<listBuslineDetailItem> listBuslineDetailItem = new ObservableCollection<listBuslineDetailItem>();

        ProgressIndicator progressIndicator = new ProgressIndicator();

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            ShowProgressIndicator("Loading..");
            dbConn = new SQLiteConnection(App.DB_PATH);
            // Create the table Task, if it doesn't exist.  
            dbConn.CreateTable<busline>();
            search = this.NavigationContext.QueryString["Search"];
            if (search == "true")
            {
                retrievedTasks = (Application.Current as App).DataSearchList;

                retrievedTask = retrievedTasks[0];

                lblbusid.Text = retrievedTasks[0].bus_line;
                lblStart.Text = retrievedTasks[0].bus_start;
                lblStop.Text = retrievedTasks[0].bus_end;
                lblbusName.Text = retrievedTasks[0].bus_name;
                lbltime.Text = retrievedTasks[0].bus_startstop_time;
                getListDatabusstop(retrievedTasks[0].bus_stop);

                isInTwown = true;
                iscate1 = true;
                iscate2 = true;
                showmap = true;

                this.Pushpin(retrievedTasks[0].bus_stop, retrievedTasks[0].bus_polyline, retrievedTasks[0].important_location, iscate1, iscate2);
            }
            else
            {
                String indexnum = this.NavigationContext.QueryString["Index"];
                index = Convert.ToInt32(indexnum);

                retrievedTasks = (Application.Current as App).DataBuslinehList;

                if (retrievedTasks.Count > 0)
                {
                    retrievedTask = retrievedTasks[index];

                    lblbusid.Text = retrievedTasks[index].bus_line;
                    lblStart.Text = retrievedTasks[index].bus_start;
                    lblStop.Text = retrievedTasks[index].bus_end;
                    lblbusName.Text = retrievedTasks[index].bus_name;
                    lbltime.Text = retrievedTasks[index].bus_startstop_time;
                    getListDatabusstop(retrievedTasks[index].bus_stop);
                }

                isInTwown = true;
                iscate1 = true;
                iscate2 = true;
                showmap = true;

                this.Pushpin(retrievedTasks[index].bus_stop, retrievedTasks[index].bus_polyline, retrievedTasks[index].important_location, iscate1, iscate2);
            }
            
            base.OnNavigatedTo(e);

            
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (dbConn != null)
            {
                dbConn.Close();
            }
        }

        public BMTA_bus_line_details()
        {
            InitializeComponent();

            LoadMap();
        }

        private void LoadMap()
        {
            // Map clear
            map.Layers.Clear();
            map.MapElements.Clear();
            map.CartographicMode = MapCartographicMode.Road;

        }

        private void Pushpin(string bus_stop, string bus_polyline, string important_location, Boolean showcateBus, Boolean showcatelocate)
        {
            // Map clear
            map.Layers.Clear();
            map.MapElements.Clear();

            MapLayer layer = new MapLayer();

            string busStopJson = bus_stop;
            string buspolylineJson = bus_polyline;
            string busimportant_location = important_location;
            
            line = new MapPolyline();
            line.StrokeColor = Colors.Red;
            line.StrokeThickness = 3;

            List<listBuslineDetailItem> results = new List<listBuslineDetailItem>();

            if (showcatelocate)
            {
                results = JsonConvert.DeserializeObject<List<listBuslineDetailItem>>(busimportant_location);
                if (results != null)
                {
                    if (results.Count > 0)
                    {
                        foreach (listBuslineDetailItem cm in results)
                        {
                            Pushpin pushpin = new Pushpin();
                            pushpin.GeoCoordinate = new GeoCoordinate(cm.lattitude, cm.longtitude);

                            var uriString = @"Assets/" + cm.type + ".png";
                            pushpin.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(uriString, UriKind.Relative)) };
                            pushpin.Width = 40;
                            pushpin.Height = 31;
                            MapOverlay overlay = new MapOverlay();
                            overlay.Content = pushpin;
                            overlay.GeoCoordinate = new GeoCoordinate(cm.lattitude, cm.longtitude);
                            layer.Add(overlay);
                        }

                        map.Center = new GeoCoordinate(results[0].lattitude, results[0].longtitude);
                    }
                }
            }

            if (showcateBus)
            {
                results = JsonConvert.DeserializeObject<List<listBuslineDetailItem>>(busStopJson);

                if (results != null)
                {
                    if (results.Count > 0)
                    {
                        foreach (listBuslineDetailItem cm in results)
                        {
                            
                            Pushpin pushpin = new Pushpin();
                            pushpin.GeoCoordinate = new GeoCoordinate(cm.latitude, cm.longitude);
                            var uriString = @"Assets/btn_bus.png";
                            pushpin.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(uriString, UriKind.Relative)) };
                            pushpin.Width = 40;
                            pushpin.Height = 31;
                            MapOverlay overlay = new MapOverlay();
                            overlay.Content = pushpin;
                            overlay.GeoCoordinate = new GeoCoordinate(cm.latitude, cm.longitude);
                            layer.Add(overlay);
                        }

                        map.Center = new GeoCoordinate(results[0].latitude, results[0].longitude);
                    }
                }
            }    
            // Map Layer
            map.Layers.Add(layer);

            if (buspolylineJson != null)
            {
                JArray lspoly = JArray.Parse(buspolylineJson);
                string[][] Users = lspoly.ToObject<string[][]>();
                foreach (String[] cm in Users)
                {
                    line.Path.Add(new GeoCoordinate(Convert.ToDouble(cm[0]), Convert.ToDouble(cm[1])));
                }
                map.MapElements.Add(line);
            }
            map.ZoomLevel = 15;
        }


        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (lang.Equals("th"))
            {
                titleName.Text = "เส้นทางเดินรถ";
                FixStart.Text = "ต้นทาง";
                FixStop.Text = "ปลายทาง";
            }
            else
            {
                titleName.Text = "Route";
                FixStart.Text = "Start";
                FixStop.Text = "End";
            }
        }

        public void getListDatabusstop(string queryBustop)
        {
            List<listBuslineDetailItem> results = JsonConvert.DeserializeObject<List<listBuslineDetailItem>>(queryBustop);
            List<listBuslineDetailItem> ls = new List<listBuslineDetailItem>();

            int b = 0;
            if (results != null)
            {
                foreach (var item in results)
                {
                    listBuslineDetailItem lsitem = new listBuslineDetailItem();
                    lsitem.stop_name = item.stop_name;

                    if (b < 1)
                    {
                        lsitem.bg = "#D1E1F3";
                        b++;
                    }
                    else
                    {
                        lsitem.bg = "#E8F0F9";
                        b--;
                    }

                    ls.Add(lsitem);
                }
            }
            else
            {
                MessageBox.Show("No data");
            }
            TaskListBox.ItemsSource = ls;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //ContentPanel.Visibility = Visibility;
            //this.LoadMap(lblbusid.Text);
            //btmap.Visibility = System.Windows.Visibility.Collapsed;
            //btmapback.Visibility = Visibility;
        }

        private void btback_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void btin_Click(object sender, RoutedEventArgs e)
        {
            var uriStringin = @"Assets/btn_in_atvth.png";
            btin.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(uriStringin, UriKind.Relative)) };

            var uriStringout = @"Assets/btn_out_th.png";
            btout.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(uriStringout, UriKind.Relative)) };

            List<buslineItem> ls = dbConn.Query<buslineItem>("SELECT bus_line,bus_start,bus_end,bus_name,bus_startstop_time,bus_stop,bus_polyline,important_location FROM busline where bus_line = '" + retrievedTasks[index].bus_line + "' and bustype = '" + retrievedTasks[index].bustype + "' and bus_owner = '" + retrievedTasks[index].bus_owner + "' and (bus_direction like '%เข้าเมือง%' or bus_direction like '%วนขวา%')");

            retrievedTask = ls[0];
            lblStart.Text = ls[0].bus_start;
            lblStop.Text = ls[0].bus_end;
            lblbusName.Text = ls[0].bus_name;
            lbltime.Text = ls[0].bus_startstop_time;

            getListDatabusstop(ls[0].bus_stop);
            this.Pushpin(ls[0].bus_stop, ls[0].bus_polyline, ls[0].important_location, iscate1, iscate2);

            isInTwown = true;
        }

        private void btout_Click(object sender, RoutedEventArgs e)
        {
            var uriStringin = @"Assets/btn_in_th.png";
            btin.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(uriStringin, UriKind.Relative)) };

            var uriStringout = @"Assets/btn_out_atvth.png";
            btout.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(uriStringout, UriKind.Relative)) };

            List<buslineItem> ls = dbConn.Query<buslineItem>("SELECT bus_line,bus_start,bus_end,bus_name,bus_startstop_time,bus_stop,bus_polyline,important_location FROM busline where bus_line = '" + retrievedTasks[index].bus_line + "' and bustype = '" + retrievedTasks[index].bustype + "' and bus_owner = '" + retrievedTasks[index].bus_owner + "' and (bus_direction not like '%เข้าเมือง%' AND bus_direction not like '%วนซ้าย%' )");

            retrievedTask = ls[0];
            lblStart.Text = ls[0].bus_start;
            lblStop.Text = ls[0].bus_end;
            lblbusName.Text = ls[0].bus_name;
            lbltime.Text = ls[0].bus_startstop_time;

            getListDatabusstop(ls[0].bus_stop);
            this.Pushpin(ls[0].bus_stop, ls[0].bus_polyline, ls[0].important_location, iscate1, iscate2);

            isInTwown = false;
        }

        private void btmapback_Click(object sender, RoutedEventArgs e)
        {
            if (map.Visibility == System.Windows.Visibility.Visible)
            {
                map.Visibility = System.Windows.Visibility.Collapsed;
                TaskListBox.Visibility = System.Windows.Visibility.Visible;
                catemap.Visibility = System.Windows.Visibility.Collapsed;
                btnfullmap.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                map.Visibility = System.Windows.Visibility.Visible;
                TaskListBox.Visibility = System.Windows.Visibility.Collapsed;
                catemap.Visibility = System.Windows.Visibility.Visible;
                btnfullmap.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void btnRight_Click(object sender, RoutedEventArgs e)
        {

            List<buslineItem> ls = new List<buslineItem>();

            if (retrievedTasks.Last() == retrievedTasks[index])
            {
                if (isInTwown)
                {
                    ls = dbConn.Query<buslineItem>("SELECT bus_line,bus_start,bus_end,bus_name,bus_startstop_time,bus_stop,bus_polyline,important_location FROM busline where bus_line = '" + retrievedTasks[index].bus_line + "' and bustype = '" + retrievedTasks[index].bustype + "' and bus_owner = '" + retrievedTasks[index].bus_owner + "' and (bus_direction like '%เข้าเมือง%' or bus_direction like '%วนขวา%')");
                }
                else
                {
                    ls = dbConn.Query<buslineItem>("SELECT bus_line,bus_start,bus_end,bus_name,bus_startstop_time,bus_stop,bus_polyline,important_location FROM busline where bus_line = '" + retrievedTasks[index].bus_line + "' and bustype = '" + retrievedTasks[index].bustype + "' and bus_owner = '" + retrievedTasks[index].bus_owner + "' and (bus_direction not like '%เข้าเมือง%' AND bus_direction not like '%วนซ้าย%' )");
                }

                if (ls.Count > 0)
                {
                    retrievedTask = ls[0];
                    lblbusid.Text = ls[0].bus_line;
                    lblStart.Text = ls[0].bus_start;
                    lblStop.Text = ls[0].bus_end;
                    lblbusName.Text = ls[0].bus_name;
                    lbltime.Text = ls[0].bus_startstop_time;

                    getListDatabusstop(ls[0].bus_stop);
                    this.Pushpin(ls[0].bus_stop, ls[0].bus_polyline, ls[0].important_location, iscate1, iscate2);
                }
            }
            else
            {
                index += 1;

                if (isInTwown)
                {
                    ls = dbConn.Query<buslineItem>("SELECT bus_line,bus_start,bus_end,bus_name,bus_startstop_time,bus_stop,bus_polyline,important_location FROM busline where bus_line = '" + retrievedTasks[index].bus_line + "' and bustype = '" + retrievedTasks[index].bustype + "' and bus_owner = '" + retrievedTasks[index].bus_owner + "' and (bus_direction like '%เข้าเมือง%' or bus_direction like '%วนขวา%')");
                }
                else
                {
                    ls = dbConn.Query<buslineItem>("SELECT bus_line,bus_start,bus_end,bus_name,bus_startstop_time,bus_stop,bus_polyline,important_location FROM busline where bus_line = '" + retrievedTasks[index].bus_line + "' and bustype = '" + retrievedTasks[index].bustype + "' and bus_owner = '" + retrievedTasks[index].bus_owner + "' and (bus_direction not like '%เข้าเมือง%' AND bus_direction not like '%วนซ้าย%' )");
                }

                if (ls.Count > 0)
                {
                    retrievedTask = ls[0];
                    lblbusid.Text = ls[0].bus_line;
                    lblStart.Text = ls[0].bus_start;
                    lblStop.Text = ls[0].bus_end;
                    lblbusName.Text = ls[0].bus_name;
                    lbltime.Text = ls[0].bus_startstop_time;

                    getListDatabusstop(ls[0].bus_stop);
                    this.Pushpin(ls[0].bus_stop, ls[0].bus_polyline, ls[0].important_location, iscate1, iscate2);
                }
            }
        }

        private void btnLeft_Click(object sender, RoutedEventArgs e)
        {

            if (index != 0)
            {
                index -= 1;
            }
            else
            {
                index = 0;
            }

            List<buslineItem> ls = new List<buslineItem>();

            if (isInTwown)
            {
                ls = dbConn.Query<buslineItem>("SELECT bus_line,bus_start,bus_end,bus_name,bus_startstop_time,bus_stop,bus_polyline,important_location FROM busline where bus_line = '" + retrievedTasks[index].bus_line + "' and bustype = '" + retrievedTasks[index].bustype + "' and bus_owner = '" + retrievedTasks[index].bus_owner + "' and (bus_direction like '%เข้าเมือง%' or bus_direction like '%วนขวา%')");
            }
            else
            {
                ls = dbConn.Query<buslineItem>("SELECT bus_line,bus_start,bus_end,bus_name,bus_startstop_time,bus_stop,bus_polyline,important_location FROM busline where bus_line = '" + retrievedTasks[index].bus_line + "' and bustype = '" + retrievedTasks[index].bustype + "' and bus_owner = '" + retrievedTasks[index].bus_owner + "' and (bus_direction not like '%เข้าเมือง%' AND bus_direction not like '%วนซ้าย%' )");
            }

            if (ls.Count > 0)
            {
                retrievedTask = ls[0];
                lblbusid.Text = ls[0].bus_line;
                lblStart.Text = ls[0].bus_start;
                lblStop.Text = ls[0].bus_end;
                lblbusName.Text = ls[0].bus_name;
                lbltime.Text = ls[0].bus_startstop_time;

                getListDatabusstop(ls[0].bus_stop);
                this.Pushpin(ls[0].bus_stop, ls[0].bus_polyline, ls[0].important_location, iscate1, iscate2);
            }
        }

        private void close_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btTopMenu_Click(object sender, RoutedEventArgs e)
        {

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

        private void btncate1_Click(object sender, RoutedEventArgs e)
        {
            if (iscate1)
            {
                iscate1 = false;
                var uriStringin = @"Assets/btbus.png";
                btncate1.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(uriStringin, UriKind.Relative)) };
            }
            else
            {
                iscate1 = true;
                var uriStringout = @"Assets/btbus_active.png";
                btncate1.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(uriStringout, UriKind.Relative)) };
            }


            this.Pushpin(retrievedTask.bus_stop, retrievedTask.bus_polyline, retrievedTask.important_location, iscate1, iscate2);
        }

        private void btncate2_Click(object sender, RoutedEventArgs e)
        {
            if (iscate2)
            {
                iscate2 = false;
                var uriStringin = @"Assets/btlandmark.png";
                btncate2.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(uriStringin, UriKind.Relative)) };
            }
            else
            {
                iscate2 = true;
                var uriStringin = @"Assets/btlandmark_active.png";
                btncate2.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(uriStringin, UriKind.Relative)) };
            }

            this.Pushpin(retrievedTask.bus_stop, retrievedTask.bus_polyline, retrievedTask.important_location, iscate1, iscate2);
        }

        private void btnfullmap_Click(object sender, RoutedEventArgs e)
        {
            if (showmap)
            {
                LayoutRoot.RowDefinitions[1].Height = new GridLength(0);
                showmap = false;
            }
            else
            {
                LayoutRoot.RowDefinitions[1].Height = new GridLength(0.8, GridUnitType.Star);
                showmap = true;
            }
        }
    }
}