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
using BMTA.Usercontrols;
using System.Diagnostics;


namespace BMTA
{
    public partial class BMTA_bus_line_details : PhoneApplicationPage
    {
        public enum DistanceType { Miles, Kilometers };
        public String lang = (Application.Current as App).Language;
        private SQLiteConnection dbConn;
        public List<buslineItem> retrievedTasks = new List<buslineItem>();
        public buslineItem retrievedTask = new buslineItem();
        public string group = "";
        public string search, query;
        public int index;
        private MapPolyline line;
        private Boolean isInTwown, showmap;
        Double lat, lon;
        private List<landmark> dataLandMark;
        private Boolean iscate1, iscate2;
        private ImageBrush img;
        ObservableCollection<listBuslineDetailItem> listBuslineDetailItem = new ObservableCollection<listBuslineDetailItem>();

        ProgressIndicator progressIndicator = new ProgressIndicator();

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            ShowProgressIndicator("Loading..");
            dbConn = new SQLiteConnection(App.DB_PATH);
            search = this.NavigationContext.QueryString["Search"];

            String indexnum = this.NavigationContext.QueryString["Index"];
            index = Convert.ToInt32(indexnum);

            retrievedTasks = (Application.Current as App).DataBuslinehList;

            if (retrievedTasks.Count > 0)
            {
                retrievedTask = retrievedTasks[index];
                lblbusid.Text = retrievedTasks[index].bus_line;

                if (lang.Equals("th"))
                {
                    lblStart.Text = retrievedTasks[index].bus_start;
                    lblStop.Text = retrievedTasks[index].bus_stop;
                }
                else
                {
                    lblStart.Text = retrievedTasks[index].bus_start_en;
                    lblStop.Text = retrievedTasks[index].bus_stop_en;
                }

                lblbusName.Text = getNameBusType(retrievedTasks[index].bustype);

                lbltime.Text = retrievedTasks[index].bus_startstop_time;
                getListDatabusstop(retrievedTasks[index].busstop_list);
            }

            isInTwown = true;
            iscate1 = true;
            iscate2 = true;
            showmap = true;

            dataLandMark = dbConn.Query<landmark>("SELECT * FROM landmark");

            this.Pushpin(retrievedTasks[index].busstop_list, retrievedTasks[index].bus_polyline, iscate1, iscate2);


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

            if (lang.Equals("th"))
            {
                img = new ImageBrush();
                img.ImageSource = new BitmapImage(new Uri("/Assets/btn_in_atvth.png", UriKind.Relative));
                btin.Background = img;


                img = new ImageBrush();
                img.ImageSource = new BitmapImage(new Uri("/Assets/btn_out_th.png", UriKind.Relative));
                btout.Background = img;
            }
            else
            {
                img = new ImageBrush();
                img.ImageSource = new BitmapImage(new Uri("/Assets/btn_in_atven.png", UriKind.Relative));
                btin.Background = img;

                img = new ImageBrush();
                img.ImageSource = new BitmapImage(new Uri("/Assets/btn_out_en.png", UriKind.Relative));
                btout.Background = img;
            }

            LoadMap();
        }
        private string getNameBusType(string bustype){

            String type = null;

            if (lang.Equals("th"))
            {
                if (bustype.Equals("1"))
                {
                    type = "รถธรรมดา";
                }
                else if (bustype.Equals("2"))
                {
                    type = "รถปรับอากาศ";
                }
            }
            else
            {
                if (bustype.Equals("1"))
                {
                    type = "Regular Bus";
                }
                else if (bustype.Equals("2"))
                {
                    type = "Air Condition Bus";
                }
            }

            return type;
        }
        

        private void LoadMap()
        {
            // Map clear
            map.Layers.Clear();
            map.MapElements.Clear();
            map.CartographicMode = MapCartographicMode.Road;

        }

        private void Pushpin(string DataBusStopList, string DataBusPolyline, Boolean showcateBus, Boolean showcatelocate)
        {
            // Map clear
            map.Layers.Clear();
            map.MapElements.Clear();

            MapLayer layer = new MapLayer();

            if (DataBusStopList == null)
            {
                return;
            }
            String[] str = DataBusStopList.Split(',');
            List<string> list = new List<string>(str);


            List<listBuslineDetailItem> results = new List<listBuslineDetailItem>();

            foreach (string item in list)
            {
                if (item != "")
                {
                    listBuslineDetailItem ls = dbConn.Query<listBuslineDetailItem>("SELECT * FROM busstop WHERE id =" + item).LastOrDefault();
                    results.Add(ls);
                }
            }

            string buspolylineJson = DataBusPolyline;
            line = new MapPolyline();
            line.StrokeColor = Colors.Red;
            line.StrokeThickness = 3;

            if (showcateBus && results != null)
            {
                if (results.Count > 0)
                {
                    foreach (listBuslineDetailItem cm in results)
                    {
                        if (cm != null)
                        {
                            UCCustomToolTipDetail _tooltip = new UCCustomToolTipDetail();
                            if (lang.Equals("th"))
                            {
                                _tooltip.Description = cm.stop_name;
                            }
                            else
                            {
                                _tooltip.Description = cm.stop_name_en;
                            }

                            _tooltip.DataContext = cm;
                            MapOverlay overlay = new MapOverlay();
                            overlay.Content = _tooltip;
                            overlay.GeoCoordinate = new GeoCoordinate(cm.latitude, cm.longitude);
                            layer.Add(overlay);

                            lat = cm.latitude;
                            lon = cm.longitude;
                        }
                    }
                }
            }

            if (showcatelocate && results != null)
            {
                if (results.Count > 0)
                {
                    foreach (listBuslineDetailItem cm in results)
                    {
                        if (cm != null)
                        {
                            var coord1 = new GeoCoordinate(cm.latitude, cm.longitude);

                            foreach (var itemLandMark in dataLandMark)
                            {
                                if (itemLandMark.lattitude != null && itemLandMark.longtitude != null)
                                {
                                    try
                                    {
                                        var coord2 = new GeoCoordinate(Double.Parse(itemLandMark.lattitude), Double.Parse(itemLandMark.longtitude));

                                        var distance = Distance(coord1.Latitude, coord1.Longitude, coord2.Latitude, coord2.Longitude, DistanceType.Kilometers);
                                        //convert to mater
                                        distance = distance * 1000;
                                        if (distance < 200)
                                        {

                                            UCCustomToolTipDetail _tooltip = new UCCustomToolTipDetail();
                                            if (lang.Equals("th"))
                                            {
                                                _tooltip.Description = itemLandMark.name;
                                            }
                                            else
                                            {
                                                _tooltip.Description = itemLandMark.name_en;
                                            }

                                            _tooltip.ImagePath = Convert.ToString(itemLandMark.type);

                                            _tooltip.DataContext = itemLandMark;
                                            MapOverlay overlay = new MapOverlay();
                                            overlay.Content = _tooltip;
                                            overlay.GeoCoordinate = new GeoCoordinate(coord2.Latitude, coord2.Longitude);
                                            layer.Add(overlay);


                                            //Pushpin pushpin = new Pushpin();
                                            //pushpin.GeoCoordinate = new GeoCoordinate(coord2.Latitude, coord2.Longitude);

                                            //var uriString = @"Assets/" + itemLandMark.type + ".png";
                                            //pushpin.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(uriString, UriKind.Relative)) };
                                            //pushpin.Width = 40;
                                            //pushpin.Height = 31;
                                            //MapOverlay overlay = new MapOverlay();
                                            //overlay.Content = pushpin;
                                            //overlay.GeoCoordinate = new GeoCoordinate(coord2.Latitude, coord2.Longitude);
                                            //layer.Add(overlay);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Debug.WriteLine("setpin error: " + ex.Message);
                                        //MessageBox.Show(ex.Message);
                                    }
                                    //}
                                }
                            }
                        }
                    }
                }
            }

            map.Center = new GeoCoordinate(lat, lon);
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
            map.ZoomLevel = 13;

            HideProgressIndicator();
        }

        public double Distance(double Latitude1, double Longitude1, double Latitude2, double Longitude2, DistanceType type)
        {
            //1- miles
            double R = (type == DistanceType.Miles) ? 3960 : 6371;          // R is earth radius.
            double dLat = this.toRadian(Latitude2 - Latitude1);
            double dLon = this.toRadian(Longitude2 - Longitude1);

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Cos(this.toRadian(Latitude1)) * Math.Cos(this.toRadian(Latitude2)) * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double c = 2 * Math.Asin(Math.Min(1, Math.Sqrt(a)));
            double d = R * c;

            return d;
        }

        private double toRadian(double val)
        {
            return (Math.PI / 180) * val;
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

        public void getListDatabusstop(string DataBusStopList)
        {
            if (DataBusStopList == null)
            {
                return;
            }
            String[] str = DataBusStopList.Split(',');
            List<string> list = new List<string>(str);

            List<listBuslineDetailItem> results = new List<listBuslineDetailItem>();

            foreach (string item in list)
            {
                if (item != "")
                {
                    listBuslineDetailItem retrievedTasks = dbConn.Query<listBuslineDetailItem>("SELECT * FROM busstop WHERE id =" + item).LastOrDefault();
                    results.Add(retrievedTasks);
                }
            }

            List<listBuslineDetailItem> ls = new List<listBuslineDetailItem>();

            int b = 0;
            int index = 1;
            if (results != null)
            {


                foreach (var item in results)
                {
                    listBuslineDetailItem lsitem = new listBuslineDetailItem();
                    if (item != null)
                    {
                        if (lang.Equals("th"))
                        {
                            lsitem.stop_name = index + ". " + item.stop_name;
                        }
                        else
                        {
                            lsitem.stop_name = index + ". " + item.stop_name_en;
                        }

                        index++;

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
            }
            else
            {
                MessageBox.Show("No data");
            }
            TaskListBox.ItemsSource = ls;
            HideProgressIndicator();
        }

        private void btback_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void btin_Click(object sender, RoutedEventArgs e)
        {
            ShowProgressIndicator("Loading..");
            if (lang.Equals("th"))
            {
                img = new ImageBrush();
                img.ImageSource = new BitmapImage(new Uri("/Assets/btn_in_atvth.png", UriKind.Relative));
                btin.Background = img;


                img = new ImageBrush();
                img.ImageSource = new BitmapImage(new Uri("/Assets/btn_out_th.png", UriKind.Relative));
                btout.Background = img;
            }
            else
            {
                img = new ImageBrush();
                img.ImageSource = new BitmapImage(new Uri("/Assets/btn_in_atven.png", UriKind.Relative));
                btin.Background = img;

                img = new ImageBrush();
                img.ImageSource = new BitmapImage(new Uri("/Assets/btn_out_en.png", UriKind.Relative));
                btout.Background = img;
            }

            List<buslineItem> ls = dbConn.Query<buslineItem>("SELECT bus_line,bus_start,bus_start_en,bus_name,bus_name_en,bus_startstop_time,bus_stop,bus_stop_en,busstop_list,bus_polyline,bustype FROM busline where bus_line = '" + retrievedTasks[index].bus_line + "' and bustype = '" + retrievedTasks[index].bustype + "' and bus_owner = '" + retrievedTasks[index].bus_owner + "' and (bus_direction like '%เข้าเมือง%' or bus_direction like '%วนขวา%')");

            retrievedTask = ls[0];
            lblbusid.Text = ls[0].bus_line;
            if (lang.Equals("th"))
            {
                lblStart.Text = ls[0].bus_start;
                lblStop.Text = ls[0].bus_stop;
            }
            else
            {
                lblStart.Text = ls[0].bus_start_en;
                lblStop.Text = ls[0].bus_stop_en;
            }

            lbltime.Text = ls[0].bus_startstop_time;
            lblbusName.Text = getNameBusType(ls[0].bustype);
            getListDatabusstop(ls[0].busstop_list);
            this.Pushpin(retrievedTasks[index].busstop_list, retrievedTasks[index].bus_polyline, iscate1, iscate2);

            isInTwown = true;
        }

        private void btout_Click(object sender, RoutedEventArgs e)
        {
            ShowProgressIndicator("Loading..");
            if (lang.Equals("th"))
            {
                img = new ImageBrush();
                img.ImageSource = new BitmapImage(new Uri("/Assets/btn_in_th.png", UriKind.Relative));
                btin.Background = img;


                img = new ImageBrush();
                img.ImageSource = new BitmapImage(new Uri("/Assets/btn_out_atvth.png", UriKind.Relative));
                btout.Background = img;
            }
            else
            {
                img = new ImageBrush();
                img.ImageSource = new BitmapImage(new Uri("/Assets/btn_in_en.png", UriKind.Relative));
                btin.Background = img;

                img = new ImageBrush();
                img.ImageSource = new BitmapImage(new Uri("/Assets/btn_out_atven.png", UriKind.Relative));
                btout.Background = img;
            }
            List<buslineItem> ls = dbConn.Query<buslineItem>("SELECT bus_line,bus_start,bus_start_en,bus_name,bus_name_en,bus_startstop_time,bus_stop,bus_stop_en,busstop_list,bus_polyline,bustype FROM busline where bus_line = '" + retrievedTasks[index].bus_line + "' and bustype = '" + retrievedTasks[index].bustype + "' and bus_owner = '" + retrievedTasks[index].bus_owner + "' and (bus_direction not like '%เข้าเมือง%' AND bus_direction not like '%วนซ้าย%' )");

            retrievedTask = ls[0];
            lblbusid.Text = ls[0].bus_line;
            if (lang.Equals("th"))
            {
                lblStart.Text = ls[0].bus_start;
                lblStop.Text = ls[0].bus_stop;
                lblbusName.Text = ls[0].bus_name;
            }
            else
            {
                lblStart.Text = ls[0].bus_start_en;
                lblStop.Text = ls[0].bus_stop_en;
                lblbusName.Text = ls[0].bus_name_en;
            }
            lblbusName.Text = getNameBusType(ls[0].bustype);
            getListDatabusstop(ls[0].busstop_list);
            this.Pushpin(retrievedTasks[index].busstop_list, retrievedTasks[index].bus_polyline, iscate1, iscate2);

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
            ShowProgressIndicator("Loading..");
            List<buslineItem> ls = new List<buslineItem>();

            if (retrievedTasks.Last() == retrievedTasks[index])
            {
                if (isInTwown)
                {
                    ls = dbConn.Query<buslineItem>("SELECT bus_line,bus_start,bus_start_en,bus_name,bus_name_en,bus_startstop_time,bus_stop,bus_stop_en,busstop_list,bus_polyline,bustype FROM busline where bus_line = '" + retrievedTasks[index].bus_line + "' and bustype = '" + retrievedTasks[index].bustype + "' and bus_owner = '" + retrievedTasks[index].bus_owner + "' and (bus_direction like '%เข้าเมือง%' or bus_direction like '%วนขวา%')");
                }
                else
                {
                    ls = dbConn.Query<buslineItem>("SELECT bus_line,bus_start,bus_start_en,bus_name,bus_name_en,bus_startstop_time,bus_stop,bus_stop_en,busstop_list,bus_polyline,bustype FROM busline where bus_line = '" + retrievedTasks[index].bus_line + "' and bustype = '" + retrievedTasks[index].bustype + "' and bus_owner = '" + retrievedTasks[index].bus_owner + "' and (bus_direction not like '%เข้าเมือง%' AND bus_direction not like '%วนซ้าย%' )");
                }

                if (ls.Count > 0)
                {
                    retrievedTask = ls[0];
                    lblbusid.Text = ls[0].bus_line;
                    lblStart.Text = ls[0].bus_start;
                    lblStop.Text = ls[0].bus_stop;
                    lbltime.Text = ls[0].bus_startstop_time;

                    lblbusName.Text = getNameBusType(ls[0].bustype);
                    getListDatabusstop(ls[0].busstop_list);
                    this.Pushpin(retrievedTasks[index].busstop_list, retrievedTasks[index].bus_polyline, iscate1, iscate2);
                }
            }
            else
            {
                index += 1;

                if (isInTwown)
                {
                    ls = dbConn.Query<buslineItem>("SELECT bus_line,bus_start,bus_start_en,bus_name,bus_name_en,bus_startstop_time,bus_stop,bus_stop_en,busstop_list,bus_polyline,bustype FROM busline where bus_line = '" + retrievedTasks[index].bus_line + "' and bustype = '" + retrievedTasks[index].bustype + "' and bus_owner = '" + retrievedTasks[index].bus_owner + "' and (bus_direction like '%เข้าเมือง%' or bus_direction like '%วนขวา%')");
                }
                else
                {
                    ls = dbConn.Query<buslineItem>("SELECT bus_line,bus_start,bus_start_en,bus_name,bus_name_en,bus_startstop_time,bus_stop,bus_stop_en,busstop_list,bus_polyline,bustype FROM busline where bus_line = '" + retrievedTasks[index].bus_line + "' and bustype = '" + retrievedTasks[index].bustype + "' and bus_owner = '" + retrievedTasks[index].bus_owner + "' and (bus_direction not like '%เข้าเมือง%' AND bus_direction not like '%วนซ้าย%' )");
                }

                if (ls.Count > 0)
                {
                    retrievedTask = ls[0];
                    lblbusid.Text = ls[0].bus_line;
                    if (lang.Equals("th"))
                    {
                        lblStart.Text = ls[0].bus_start;
                        lblStop.Text = ls[0].bus_stop;
                    }
                    else
                    {
                        lblStart.Text = ls[0].bus_start_en;
                        lblStop.Text = ls[0].bus_stop_en;
                    }
                    lblbusName.Text = getNameBusType(ls[0].bustype);
                    lbltime.Text = ls[0].bus_startstop_time;

                    getListDatabusstop(ls[0].busstop_list);
                    this.Pushpin(retrievedTasks[index].busstop_list, retrievedTasks[index].bus_polyline, iscate1, iscate2);
                }
            }
        }

        private void btnLeft_Click(object sender, RoutedEventArgs e)
        {
            ShowProgressIndicator("Loading..");
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
                ls = dbConn.Query<buslineItem>("SELECT bus_line,bus_start,bus_name,bus_startstop_time,bus_stop,busstop_list,bus_polyline,bustype FROM busline where bus_line = '" + retrievedTasks[index].bus_line + "' and bustype = '" + retrievedTasks[index].bustype + "' and bus_owner = '" + retrievedTasks[index].bus_owner + "' and (bus_direction like '%เข้าเมือง%' or bus_direction like '%วนขวา%')");
            }
            else
            {
                ls = dbConn.Query<buslineItem>("SELECT bus_line,bus_start,bus_name,bus_startstop_time,bus_stop,busstop_list,bus_polyline,bustype FROM busline where bus_line = '" + retrievedTasks[index].bus_line + "' and bustype = '" + retrievedTasks[index].bustype + "' and bus_owner = '" + retrievedTasks[index].bus_owner + "' and (bus_direction not like '%เข้าเมือง%' AND bus_direction not like '%วนซ้าย%' )");
            }

            if (ls.Count > 0)
            {
                retrievedTask = ls[0];
                lblbusid.Text = ls[0].bus_line;
                if (lang.Equals("th"))
                {
                    lblStart.Text = ls[0].bus_start;
                    lblStop.Text = ls[0].bus_stop;
                }
                else
                {
                    lblStart.Text = ls[0].bus_start_en;
                    lblStop.Text = ls[0].bus_stop_en;
                }
                lbltime.Text = ls[0].bus_startstop_time;
                lblbusName.Text = getNameBusType(ls[0].bustype);
                getListDatabusstop(ls[0].busstop_list);
                this.Pushpin(retrievedTasks[index].busstop_list, retrievedTasks[index].bus_polyline, iscate1, iscate2);
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

            this.Pushpin(retrievedTask.busstop_list, retrievedTask.bus_polyline, iscate1, iscate2);
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
            this.Pushpin(retrievedTask.busstop_list, retrievedTask.bus_polyline, iscate1, iscate2);
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