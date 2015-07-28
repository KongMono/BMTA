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

namespace BMTA
{
    public partial class BMTA_Speed_Result : PhoneApplicationPage
    {

        public BMTA_Speed_Result()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string parameter = "";
            string timex = "";

            if (NavigationContext.QueryString.TryGetValue("parameter", out parameter))
            {
                this.sumdistanct.Text = parameter;
                this.sumkm.Text = parameter;
            }
            if (NavigationContext.QueryString.TryGetValue("timex", out timex))
            {
                this.sumtime.Text = timex;
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {

        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {      
            
        }
        private void btback_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_bus_line.xaml", UriKind.Relative));
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_BusStop.xaml", UriKind.Relative));
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_BusCoordinates.xaml", UriKind.Relative));
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_BusStartStop.xaml", UriKind.Relative));
        }

        private void close_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btTopMenu_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}