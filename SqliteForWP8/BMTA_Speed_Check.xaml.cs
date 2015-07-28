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
using System.Diagnostics;
using System.Text;
using BMTA.Item;
namespace BMTA
{
    public partial class BMTA_Speed_Check : PhoneApplicationPage
    {
        private DispatcherTimer dispatcherTimer;
        public enum DistanceType { Miles, Kilometers };
        private GeoCoordinateWatcher _watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High);
        public String lang = (Application.Current as App).Language;
        private long _previousPositionChangeTick;
        private double _kilometres;
        private GeoCoordinate previousPoint = null;
        GeoCoordinate previous = new GeoCoordinate();
        DateTime previousTime = DateTime.Now;
        private static DateTime EndTime { get; set; }

        public BMTA_Speed_Check()
        {
            InitializeComponent();

            _watcher.MovementThreshold = 1;
            _watcher.StatusChanged += new EventHandler<GeoPositionStatusChangedEventArgs>(watcher_StatusChanged);
            _watcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(watcher_PositionChanged);
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (lang.Equals("th"))
            {
                titleName.Text = "เช็คความเร็ว";
            }
            else
            {
                titleName.Text = "Speed Check";
            }
        }

        public void watcher_StatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
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

        public void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            if (e.Position.Location.IsUnknown)
            {
                MessageBox.Show(" The Location Service IsUnknown.");
                return;
            }
            var coord = new GeoCoordinate(e.Position.Location.Latitude, e.Position.Location.Longitude);

            if (previousPoint != null && previousPoint != coord)
            {
                //var distance = coord.GetDistanceTo(previousPoint);

                //// compute pace
                //var millisPerKilometer = (1000.0 / distance) * (System.Environment.TickCount - _previousPositionChangeTick);

                // compute total distance travelled


                _kilometres += Distance(coord, previousPoint, DistanceType.Kilometers);

                double output = Math.Round(_kilometres, 2);

                sumdistanct.Text = output + " km.";
            }

            previousPoint = coord;

            _previousPositionChangeTick = System.Environment.TickCount;

            if (Double.IsNaN(e.Position.Location.Speed))
            {
                sumkm.Text = "0";
            }
            else
            {
                sumkm.Text = e.Position.Location.Speed.ToString();
            }
        }
        /// <summary>  
        /// Returns the distance in miles or kilometers of any two  
        /// latitude / longitude points.  
        /// </summary>  
        public double Distance(GeoCoordinate pos1, GeoCoordinate pos2, DistanceType type)
        {
            double R = (type == DistanceType.Miles) ? 3960 : 6371;
            double dLat = this.toRadian(pos2.Latitude - pos1.Latitude);
            double dLon = this.toRadian(pos2.Longitude - pos1.Longitude);
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(this.toRadian(pos1.Latitude)) * Math.Cos(this.toRadian(pos2.Latitude)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Asin(Math.Min(1, Math.Sqrt(a)));
            double d = R * c;
            return d;
        }
        /// <summary>  
        /// Convert to Radians.  
        /// </summary>  
        private double toRadian(double val)
        {
            return (Math.PI / 180) * val;
        }

        private void btback_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            layoutCheck1.Visibility = System.Windows.Visibility.Collapsed;
            layoutCheck2.Visibility = System.Windows.Visibility.Collapsed;
            layoutCheck3.Visibility = System.Windows.Visibility.Collapsed;

            layoutResult1.Visibility = System.Windows.Visibility.Visible;
            layoutResult2.Visibility = System.Windows.Visibility.Visible;
            layoutResult3.Visibility = System.Windows.Visibility.Visible;

            if (this.dispatcherTimer == null)
            {
                this.dispatcherTimer = new DispatcherTimer();
                this.dispatcherTimer.Interval = TimeSpan.FromMilliseconds(1);
                this.dispatcherTimer.Tick += new EventHandler(Timer_Tick);
            }

            if (EndTime == DateTime.MinValue)
            {
                EndTime = DateTime.Now + (TimeSpan)this.timeSpan.Value;
            }

            this.dispatcherTimer.Start();
            _watcher.Start();

            if (_watcher.Permission == GeoPositionPermission.Denied)
            {
                MessageBox.Show("Please enable location services and retry");
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            var remaining = DateTime.Now - EndTime;
            int remainingSeconds = (int)remaining.TotalSeconds;
            this.timeSpan.Value = TimeSpan.FromSeconds(remainingSeconds);

            if (remaining.TotalSeconds <= 0)
            {
                this.dispatcherTimer.Stop();
            }
        }

        private void btn_pause_Click(object sender, RoutedEventArgs e)
        {
            if (btn_save.Visibility == System.Windows.Visibility.Collapsed && btn_resume.Visibility == System.Windows.Visibility.Collapsed)
            {
                btn_save.Visibility = System.Windows.Visibility.Visible;
                btn_resume.Visibility = System.Windows.Visibility.Visible;
                btn_pause.Visibility = System.Windows.Visibility.Collapsed;
                _watcher.Stop();
                this.dispatcherTimer.Stop();
            }
            else
            {
                btn_save.Visibility = System.Windows.Visibility.Collapsed;
                btn_resume.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void btn_resume_Click(object sender, RoutedEventArgs e)
        {
            if (btn_save.Visibility == System.Windows.Visibility.Visible && btn_resume.Visibility == System.Windows.Visibility.Visible)
            {
                btn_save.Visibility = System.Windows.Visibility.Collapsed;
                btn_resume.Visibility = System.Windows.Visibility.Collapsed;
                btn_pause.Visibility = System.Windows.Visibility.Visible;

                this.dispatcherTimer.Start();
                _watcher.Start();
            }
        }

        private void btn_save_Click(object sender, RoutedEventArgs e)
        {
            SpeedCheckItem item = new SpeedCheckItem();
            item.line_number = txtbusline.Text;
            item.speed = sumkm.Text;
            item.date = DateTime.Now.ToString("MM/dd/yyyy");
            item.time = this.timeSpan.Value.ToString();
            item.distance = sumdistanct.Text;

            (Application.Current as App).CheckSpeedList.Add(item);

            NavigationService.GoBack();
        }
    }
}