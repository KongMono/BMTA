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


namespace BMTA
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
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
                        ImageSource = new System.Windows.Media.Imaging.BitmapImage(new Uri("/Assets/480x852/BMTA_intro.png", UriKind.Relative)),
                        Opacity = 1d
                    };
                    this.LayoutRoot.Background = brush;
                }
                else if (x == "720")
                {
                    ImageBrush brush = new ImageBrush
                    {
                        ImageSource = new System.Windows.Media.Imaging.BitmapImage(new Uri("/Assets/720x1280/BMTA_intro.png", UriKind.Relative)),
                        Opacity = 1d
                        
                    };
                    this.LayoutRoot.Background = brush;
                    brush.Stretch = Stretch.Fill;

                }
                else
                {
                    ImageBrush brush = new ImageBrush
                    {
                        ImageSource = new System.Windows.Media.Imaging.BitmapImage(new Uri("/Assets/768x1280/BMTA_intro.png", UriKind.Relative)),
                        Opacity = 1d
                    };
                    this.LayoutRoot.Background = brush;
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

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_intro_details.xaml", UriKind.Relative)); 
           // (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri("/BMTA_intro_details.xaml", UriKind.RelativeOrAbsolute));
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            stackPanel1.Visibility = Visibility.Visible;
            //button5.Visibility = Visibility.Visible;
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_AppTh.xaml", UriKind.Relative)); 
          //  (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri("/BMTA_AppTh.xaml", UriKind.RelativeOrAbsolute));
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_AppTh_en.xaml", UriKind.Relative)); 
          //  (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri("/BMTA_AppEn.xaml", UriKind.RelativeOrAbsolute));
        }

        private void button5_Click(object sender, RoutedEventArgs e)
        {
            stackPanel1.Visibility = Visibility.Collapsed;
           // button5.Visibility = Visibility.Collapsed;
        }

        
    }
}