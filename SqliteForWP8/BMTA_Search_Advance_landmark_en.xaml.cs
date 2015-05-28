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

namespace BMTA
{
    public partial class BMTA_Search_Advance_landmark_en : PhoneApplicationPage
    {
        public BMTA_Search_Advance_landmark_en()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            rightmenu.Visibility = System.Windows.Visibility.Collapsed;
            rightmenux.Visibility = System.Windows.Visibility.Collapsed;
            close.Visibility = System.Windows.Visibility.Collapsed;

            if (!HasNetwork())
            {
                Application.Current.Terminate();
                // new Microsoft.Xna.Framework.Game().Exit();
            }
            else if (!HasInternet())
            {
                Application.Current.Terminate();
                // new Microsoft.Xna.Framework.Game().Exit();
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
                        ImageSource = new System.Windows.Media.Imaging.BitmapImage(new Uri("/Assets/480x852/BMTA_Search_Advance_landmark_en.png", UriKind.Relative)),
                        Opacity = 1d
                    };
                    this.LayoutRoot.Background = brush;
                    brush.Stretch = Stretch.Fill;
                }
                else if (x == "720")
                {
                    ImageBrush brush = new ImageBrush
                    {
                        ImageSource = new System.Windows.Media.Imaging.BitmapImage(new Uri("/Assets/720x1280/BMTA_Search_Advance_landmark_en.png", UriKind.Relative)),
                        Opacity = 1d
                    };
                    this.LayoutRoot.Background = brush;
                    brush.Stretch = Stretch.Fill;
                }
                else
                {
                    ImageBrush brush = new ImageBrush
                    {
                        ImageSource = new System.Windows.Media.Imaging.BitmapImage(new Uri("/Assets/768x1280/BMTA_Search_Advance_landmark_en.png", UriKind.Relative)),
                        Opacity = 1d
                    };
                    this.LayoutRoot.Background = brush;
                    brush.Stretch = Stretch.Fill;
                }
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

        private void btSearchAd_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_Search_Advance_landmark_en.xaml", UriKind.Relative));
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