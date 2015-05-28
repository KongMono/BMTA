using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO;
using System.Windows.Threading;
using Microsoft.Phone.Net.NetworkInformation;
using System.Windows.Media;
using NExtra.Geo;
using System.Device.Location;
using Windows.Devices.Geolocation;
using Microsoft.Phone.Maps.Controls;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace BMTA
{
    public partial class BMTA_Speed_Check_en : PhoneApplicationPage
    {
        private GeoCoordinateWatcher _watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High);
        private DispatcherTimer _timer = new DispatcherTimer();
        private long _startTime;
        private MapPolyline _line;


        GeoCoordinateWatcher watcher;
        Geolocator Locator = new Geolocator();
        List<GeoCoordinate> Locations;

        public BMTA_Speed_Check_en()
        {
            InitializeComponent();

            _line = new MapPolyline();
            _line.StrokeColor = Colors.Red;
            _line.StrokeThickness = 1;
            Map.MapElements.Add(_line);

            _watcher.PositionChanged += Watcher_PositionChanged;

            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;
        }

        //ID_CAP_LOCATION
        private double _kilometres;
        private long _previousPositionChangeTick;

        private void Watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            var coord = new GeoCoordinate(e.Position.Location.Latitude, e.Position.Location.Longitude);

            if (_line.Path.Count > 0)
            {
                var previousPoint = _line.Path.Last();
                var distance = coord.GetDistanceTo(previousPoint);
                var millisPerKilometer = (1000.0 / distance) * (System.Environment.TickCount - _previousPositionChangeTick);
                _kilometres += distance / 1000.0;

                paceLabel.Text = TimeSpan.FromMilliseconds(millisPerKilometer).ToString(@"mm\:ss");
                distanceLabel.Text = string.Format("{0:f2}", _kilometres);
                // caloriesLabel.Text = string.Format("{0:f0}", _kilometres * 65);

                PositionHandler handler = new PositionHandler();
                var heading = handler.CalculateBearing(new Position(previousPoint), new Position(coord));
                Map.SetView(coord, Map.ZoomLevel, heading, MapAnimationKind.Parabolic);

                //  ShellTile.ActiveTiles.First().Update(new IconicTileData()
                //   {
                //       Title = "WP8Runner",
                //      WideContent1 = string.Format("{0:f2} km", _kilometres),
                //      WideContent2 = string.Format("{0:f0} calories", _kilometres * 65),
                //   });
            }
            else
            {
                Map.Center = coord;
            }

            _line.Path.Add(coord);
            _previousPositionChangeTick = System.Environment.TickCount;
        }

        private void Locator_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            var CurrentLocation = args.Position;


            List<GeoCoordinate> locationData = new List<GeoCoordinate>();
            // locationData.Add(CurrentLocation.Coordinate.Latitude.ToString("Latitude:" + "0.000"));
            //  locationData.Add(CurrentLocation.Coordinate.Longitude.ToString("Longitude:" + "0.000"));
            //  locationData.Add(CurrentLocation.Coordinate.Altitude.ToString());
            // locationData.Add(CurrentLocation.Coordinate.Speed.ToString());


            //  if (GetPositionTime >= 1) // Checks to see if 8 seconds has passed
            // {
            Deployment.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                GeoCoordinate cord = new GeoCoordinate(CurrentLocation.Coordinate.Latitude, CurrentLocation.Coordinate.Longitude);
                if (locationData.Count > 0)
                {
                    GeoCoordinate PreviousLocation = locationData.Last();

                    // This part will update the stats on the screen as a textbox is bound to
                    // DistanceMoved
                    var distance = cord.GetDistanceTo(PreviousLocation);
                    //var millisPerKilometer = (1000.0 / distance) * (System.Environment.TickCount - _previousPositionChangeTick);
                    _kilometres += distance / 1000.0;
                    //var distance = cord.GetDistanceTo(PreviousLocation);
                    distanceLabel.Text = string.Format("{0:f2} km", _kilometres);
                }
                locationData.Add(cord);
            }));
            // }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            TimeSpan runTime = TimeSpan.FromMilliseconds(System.Environment.TickCount - _startTime);
            timeLabel.Text = runTime.ToString(@"hh\:mm\:ss");
        }



        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {

            rightmenu.Visibility = System.Windows.Visibility.Collapsed;
            rightmenux.Visibility = System.Windows.Visibility.Collapsed;
            close.Visibility = System.Windows.Visibility.Collapsed;

            if (!HasNetwork())
            {

                Application.Current.Terminate();
            }
            else if (!HasInternet())
            {
                Application.Current.Terminate();
            }
            else
            {
                string x = BMTA.clGetResolution.Width.ToString();
                string y = BMTA.clGetResolution.Height.ToString();
                string xy = x + "x" + y;
                if (x == "480")
                {
                    ImageBrush brush = new ImageBrush
                    {
                        ImageSource = new System.Windows.Media.Imaging.BitmapImage(new Uri("/Assets/480x852/BMTA_speed_bg_en.png", UriKind.Relative)),
                        Opacity = 1d
                    };
                    this.LayoutRoot.Background = brush;
                    brush.Stretch = Stretch.Fill;
                }
                else if (x == "720")
                {
                    ImageBrush brush = new ImageBrush
                    {
                        ImageSource = new System.Windows.Media.Imaging.BitmapImage(new Uri("/Assets/720x1280/BMTA_speed_bg_en.png", UriKind.Relative)),
                        Opacity = 1d
                    };
                    this.LayoutRoot.Background = brush;
                    brush.Stretch = Stretch.Fill;
                }
                else
                {
                    ImageBrush brush = new ImageBrush
                    {
                        ImageSource = new System.Windows.Media.Imaging.BitmapImage(new Uri("/Assets/768x1280/BMTA_speed_bg_en.png", UriKind.Relative)),
                        Opacity = 1d
                    };
                    this.LayoutRoot.Background = brush;
                    brush.Stretch = Stretch.Fill;
                }
                txtbusline.Focus();
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

        private void button5_Click(object sender, RoutedEventArgs e)
        {
            // button5.Content = "STOP";
            if (_timer.IsEnabled)
            {
                _watcher.Stop();
                _timer.Stop();
                StartButton.Content = "START";

                string result = "";
                string timelable = "";
                result = distanceLabel.Text;
                timelable = timeLabel.Text;
                NavigationService.Navigate(new Uri("/BMTA_Speed_Result_en.xaml?parameter=" + result + "&timex=" + timelable, UriKind.Relative));

            }
            else
            {
                _watcher.Start();
                _timer.Start();
                _startTime = System.Environment.TickCount;
                StartButton.Content = "STOP";

            }

           // if (_timer.IsEnabled)
          //  {
           //     Locator.PositionChanged -= Locator_PositionChanged;
                // Locator.StatusChanged -= Locator_StatusChanged;
           //     Locator = null;

                // watcher.Stop();
                // _watcher.Stop();
            //    _timer.Stop();
            //    StartButton.Content = "START";
           // }
           // else
           // {
           //     _timer.Interval = TimeSpan.FromSeconds(1);
           //     _timer.Tick += Timer_Tick;

           //     Locator.DesiredAccuracy = PositionAccuracy.High;
           //     Locator.MovementThreshold = 1;
                //  Locator.StatusChanged += Locator_StatusChanged;
           //     Locator.PositionChanged += Locator_PositionChanged;


                //   myGeoLocator = new Geolocator();
                //   myGeoLocator.DesiredAccuracy = PositionAccuracy.Default;
                //   myGeoLocator.MovementThreshold = 50;
                //   myGeoLocator.StatusChanged += myGeoLocator_StatusChanged;
                //   myGeoLocator.PositionChanged += myGeoLocator_PositionChanged;

                //if (watcher == null)
                //{
                //    watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.Default);
                //    watcher.MovementThreshold = 1;
                //    watcher.StatusChanged += new EventHandler<GeoPositionStatusChangedEventArgs>(watcher_StatusChanged);
                //    watcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(watcher_PositionChanged);

                //}
                // watcher.Start();
                // _watcher.Start();
            //    _timer.Start();
            //    _startTime = System.Environment.TickCount;
            //    StartButton.Content = "STOP";
           // }
        }


        void watcher_StatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
        {
            switch (e.Status)
            {
                case GeoPositionStatus.Disabled:
                    MessageBox.Show("Location Service is not enabled on the device");
                    break;

                case GeoPositionStatus.NoData:
                    MessageBox.Show(" The Location Service is working, but it cannot get location data.");
                    break;
            }
        }

        void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            if (e.Position.Location.IsUnknown)
            {
                // this.notification.Text = "Please wait while your prosition is determined....";
                return;
            }

            distanceLabel.Text = e.Position.Location.Speed.ToString("0.00");

        }

        private void btback_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_bus_line_en.xaml", UriKind.Relative));
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_BusStop_en.xaml", UriKind.Relative));
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_BusCoordinates_en.xaml", UriKind.Relative));
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_BusStartStop_en.xaml", UriKind.Relative));
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
            NavigationService.Navigate(new Uri("/BMTA_AppTh_en.xaml", UriKind.Relative));
        }

        private void rbusline_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_bus_line_en.xaml", UriKind.Relative));
        }

        private void rbusstop_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_BusStop_en.xaml", UriKind.Relative));
        }

        private void rcoor_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_BusCoordinates_en.xaml", UriKind.Relative));
        }

        private void rbusstartstop_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_BusStartStop_en.xaml", UriKind.Relative));
        }

        private void rbusspeed_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_Speed_history_en.xaml", UriKind.Relative));
        }

        private void rbusnew_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_EventNew_en.xaml", UriKind.Relative));
        }

    
    }
}