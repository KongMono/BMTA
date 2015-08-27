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
using Microsoft.Devices;
namespace BMTA
{
    public partial class BMTA_Speed_Check : PhoneApplicationPage
    {
        private DispatcherTimer dispatcherTimer;
        public enum DistanceType { Miles, Kilometers };
        private GeoCoordinateWatcher _watcher;
        public String lang = (Application.Current as App).Language;
        private TimeSpan _previousPositionChangeTick;
        private double _kilometres;
        private GeoCoordinate previousPoint = null;

        GeoCoordinate previous = new GeoCoordinate();
        DateTime previousTime = DateTime.Now;
        //private static DateTime EndTime { get; set; }
        private DateTime EndTime;
        VibrateController vibrateController;
        public BMTA_Speed_Check()
        {
            InitializeComponent();
            vibrateController = VibrateController.Default;
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
            _watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High);
            _watcher.MovementThreshold = 100;
            _watcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(this.watcher_PositionChanged);
            _watcher.StatusChanged += this.watcher_StatusChanged;
        }

        public void watcher_StatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
        {
            switch (e.Status)
            {
                case GeoPositionStatus.Disabled:
                    MessageBox.Show("the application does not have the right capability or the location master switch is off");

                    break;
                case GeoPositionStatus.Initializing:
                    MessageBox.Show("the geolocator started the tracking operation");

                    break;
                case GeoPositionStatus.NoData:
                    MessageBox.Show("the location service was not able to acquire the location");

                    break;
                case GeoPositionStatus.Ready:
                    MessageBox.Show("the location service is generating geopositions as specified by the tracking parameters");

                    break;
            }
        }

        public void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {

            Double distanceValue = 0;

            if (e.Position.Location.IsUnknown)
            {
                MessageBox.Show(" The Location Service IsUnknown.");
                return;
            }
            var coord = new GeoCoordinate(e.Position.Location.Latitude, e.Position.Location.Longitude);

            if (previousPoint != null)
            {
                distanceValue = Distance(previousPoint.Latitude, previousPoint.Longitude, coord.Latitude, coord.Longitude, DistanceType.Kilometers);
                _kilometres = _kilometres + distanceValue;
                double output = Math.Round(_kilometres, 2);
                sumdistanct.Text = Convert.ToString(output) + " km.";
                previousPoint = coord;

                double distanceInKilometres = distanceValue;

                DateTime dt2 = Convert.ToDateTime(_previousPositionChangeTick.ToString().ToString());
                DateTime dt1 = Convert.ToDateTime(this.timeSpan.Value.ToString());

                TimeSpan span = dt1.Subtract(dt2);

                double timeInHours = span.TotalHours;
                double speedInKilometresPerHour = distanceInKilometres / timeInHours;

                _previousPositionChangeTick = (TimeSpan)this.timeSpan.Value;

                if (Double.IsNaN(speedInKilometresPerHour))
                {
                    sumkm.Text = "0";
                }
                else
                {
                    sumkm.Text = Convert.ToString(speedInKilometresPerHour);
                    if (speedInKilometresPerHour > 60)
                    {
                        vibrateController.Start(TimeSpan.FromSeconds(3));
                    }
                    else
                    {
                        vibrateController.Stop();
                    }
                }
            }
            else
            {
                previousPoint = coord;
                _previousPositionChangeTick = (TimeSpan)this.timeSpan.Value;
                double distanceInKilometres = distanceValue;
                double timeInHours = TimeSpan.Parse(_previousPositionChangeTick.ToString()).TotalHours;
                double speedInKilometresPerHour = distanceInKilometres / timeInHours;

                sumkm.Text = Convert.ToString(speedInKilometresPerHour);

            }

            //if (Double.IsNaN(e.Position.Location.Speed))
            //{
            //    sumkm.Text = "0";
            //}
            //else
            //{
            //    sumkm.Text = e.Position.Location.Speed.ToString();
            //}


        }

        public double Distance(double Latitude1, double Longitude1, double Latitude2, double Longitude2, DistanceType type)
        {
            double R = (type == DistanceType.Miles) ? 3960 : 6371;
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


            this.dispatcherTimer = new DispatcherTimer();
            this.dispatcherTimer.Interval = TimeSpan.FromMilliseconds(1);
            this.dispatcherTimer.Tick += new EventHandler(Timer_Tick);


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