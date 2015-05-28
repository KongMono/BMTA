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

namespace BMTA
{
    public partial class BMTA_AppTh : PhoneApplicationPage
    {
        public BMTA_AppTh()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            //if (!HasNetwork())
            //{
                
            //    Application.Current.Terminate();
            //}
            //else if (!HasInternet())
            //{
            //    Application.Current.Terminate();
            //}
            //else
            //{
                string x = BMTA.clGetResolution.Width.ToString();
                string y = BMTA.clGetResolution.Height.ToString();
                string xy = x + "x" + y;
                if (x == "480")
                {
                    ImageBrush brush = new ImageBrush
                    {
                        ImageSource = new System.Windows.Media.Imaging.BitmapImage(new Uri("/Assets/480x852/BMTA_main_bg.png", UriKind.Relative)),
                        Opacity = 1d
                    };
                    this.LayoutRoot.Background = brush;
                    brush.Stretch = Stretch.Fill;
                }
                else if (x == "720")
                {
                    ImageBrush brush = new ImageBrush
                    {
                        ImageSource = new System.Windows.Media.Imaging.BitmapImage(new Uri("/Assets/720x1280/BMTA_menu_bg.png", UriKind.Relative)),
                        Opacity = 1d
                    };
                    this.LayoutRoot.Background = brush;
                    brush.Stretch = Stretch.Fill;
                }
                else
                {
                    ImageBrush brush = new ImageBrush
                    {
                        ImageSource = new System.Windows.Media.Imaging.BitmapImage(new Uri("/Assets/768x1280/BMTA_main_bg.png", UriKind.Relative)),
                        Opacity = 1d
                    };
                    this.LayoutRoot.Background = brush;
                    brush.Stretch = Stretch.Fill;
                }
           // }
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

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_bus_line.xaml", UriKind.Relative)); 
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_BusStop.xaml", UriKind.Relative));
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_BusStartStop.xaml", UriKind.Relative));
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_BusCoordinates.xaml", UriKind.Relative));
        }

        private void button5_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_Speed_history.xaml", UriKind.Relative)); 
        }

        private void button6_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_EventNew.xaml", UriKind.Relative)); 
        }

        
    }
}