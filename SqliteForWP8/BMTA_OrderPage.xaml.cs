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
using System.Windows.Media.Imaging;
using System.Reflection;

namespace BMTA
{
    public partial class BMTA_OrderPage : PhoneApplicationPage
    {
        ProgressIndicator progressIndicator;
        ImageBrush background;
        public String lang = (Application.Current as App).Language;

        public BMTA_OrderPage()
        {
            InitializeComponent();

            progressIndicator = new ProgressIndicator();

            var nameHelper = new AssemblyName(Assembly.GetExecutingAssembly().FullName);

            var version = nameHelper.Version;
            text_version.Text = "version " + version;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {

            NavigationService.Navigate(new Uri("/BMTA_bus_mainpage.xaml?busCate=1", UriKind.Relative));
        }
        private void button2_Click(object sender, RoutedEventArgs e)
        {
            if (!HasNetwork() || !HasInternet())
            {
                MessageBox.Show("No internet connection is available. Try again later.");
            }
            else
            {
                NavigationService.Navigate(new Uri("/BMTA_bus_mainpage.xaml?busCate=2", UriKind.Relative));
            }
        }
        private void button3_Click(object sender, RoutedEventArgs e)
        {
            if (!HasNetwork() || !HasInternet())
            {
                MessageBox.Show("No internet connection is available. Try again later.");
            }
            else
            {
                NavigationService.Navigate(new Uri("/BMTA_bus_mainpage.xaml?busCate=3", UriKind.Relative));
            }
        }
        private void button4_Click(object sender, RoutedEventArgs e)
        {
            if (!HasNetwork() || !HasInternet())
            {
                MessageBox.Show("No internet connection is available. Try again later.");
            }
            else
            {
                NavigationService.Navigate(new Uri("/BMTA_bus_mainpage.xaml?busCate=4", UriKind.Relative));
            }
        }
        private void button5_Click(object sender, RoutedEventArgs e)
        {
            if (!HasNetwork() || !HasInternet())
            {
                MessageBox.Show("No internet connection is available. Try again later.");
            }
            else
            {
                NavigationService.Navigate(new Uri("/BMTA_Speed_history.xaml", UriKind.Relative));
            }
        }
        private void button6_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_EventNew.xaml", UriKind.Relative));
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

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            //remove all page in stack
            while (this.NavigationService.BackStack.Count() > 0)
            {
                this.NavigationService.RemoveBackEntry();
            }

            base.OnBackKeyPress(e);
        }

        private void bt_1_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (lang.Equals("th"))
            {
                background = new ImageBrush();
                background.ImageSource = new BitmapImage(new Uri("/Assets/main/menu/btn_1th_atv.png", UriKind.Relative));
                bt_1.Background = background;
            }
            else
            {
                background = new ImageBrush();
                background.ImageSource = new BitmapImage(new Uri("/Assets/main/menu/btn_1en_atv.png", UriKind.Relative));
                bt_1.Background = background;
            }
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            ShowProgressIndicator("Loading..");

            if (!HasNetwork() || !HasInternet())
            {
                if (lang.Equals("th"))
                {
                    background = new ImageBrush();
                    background.ImageSource = new BitmapImage(new Uri("/Assets/main/menu/btn_1th_atv.png", UriKind.Relative));
                    bt_1.Background = background;
                    background = new ImageBrush();
                    background.ImageSource = new BitmapImage(new Uri("/Assets/main/menu/btn_2th_off.png", UriKind.Relative));
                    bt_2.Background = background;
                    background = new ImageBrush();
                    background.ImageSource = new BitmapImage(new Uri("/Assets/main/menu/btn_3th_off.png", UriKind.Relative));
                    bt_3.Background = background;
                    background = new ImageBrush();
                    background.ImageSource = new BitmapImage(new Uri("/Assets/main/menu/btn_4th_off.png", UriKind.Relative));
                    bt_4.Background = background;
                    background = new ImageBrush();
                    background.ImageSource = new BitmapImage(new Uri("/Assets/main/menu/btn_5th_off.png", UriKind.Relative));
                    bt_5.Background = background;
                    background = new ImageBrush();
                    background.ImageSource = new BitmapImage(new Uri("/Assets/main/menu/btn_6th_atv.png", UriKind.Relative));
                    bt_6.Background = background;
                }
                else
                {
                    background = new ImageBrush();
                    background.ImageSource = new BitmapImage(new Uri("/Assets/main/menu/btn_1en_atv.png", UriKind.Relative));
                    bt_1.Background = background;
                    background = new ImageBrush();
                    background.ImageSource = new BitmapImage(new Uri("/Assets/main/menu/btn_2en_off.png", UriKind.Relative));
                    bt_2.Background = background;
                    background = new ImageBrush();
                    background.ImageSource = new BitmapImage(new Uri("/Assets/main/menu/btn_3en_off.png", UriKind.Relative));
                    bt_3.Background = background;
                    background = new ImageBrush();
                    background.ImageSource = new BitmapImage(new Uri("/Assets/main/menu/btn_4en_off.png", UriKind.Relative));
                    bt_4.Background = background;
                    background = new ImageBrush();
                    background.ImageSource = new BitmapImage(new Uri("/Assets/main/menu/btn_5en_off.png", UriKind.Relative));
                    bt_5.Background = background;
                    background = new ImageBrush();
                    background.ImageSource = new BitmapImage(new Uri("/Assets/main/menu/btn_6en_atv.png", UriKind.Relative));
                    bt_6.Background = background;
                }
            }
            else
            {
                if (lang.Equals("th"))
                {
                    background = new ImageBrush();
                    background.ImageSource = new BitmapImage(new Uri("/Assets/main/menu/btn_1th_atv.png", UriKind.Relative));
                    bt_1.Background = background;
                    background = new ImageBrush();
                    background.ImageSource = new BitmapImage(new Uri("/Assets/main/menu/btn_2th_atv.png", UriKind.Relative));
                    bt_2.Background = background;
                    background = new ImageBrush();
                    background.ImageSource = new BitmapImage(new Uri("/Assets/main/menu/btn_3th_atv.png", UriKind.Relative));
                    bt_3.Background = background;
                    background = new ImageBrush();
                    background.ImageSource = new BitmapImage(new Uri("/Assets/main/menu/btn_4th_atv.png", UriKind.Relative));
                    bt_4.Background = background;
                    background = new ImageBrush();
                    background.ImageSource = new BitmapImage(new Uri("/Assets/main/menu/btn_5th_atv.png", UriKind.Relative));
                    bt_5.Background = background;
                    background = new ImageBrush();
                    background.ImageSource = new BitmapImage(new Uri("/Assets/main/menu/btn_6th_atv.png", UriKind.Relative));
                    bt_6.Background = background;
                }
                else
                {
                    background = new ImageBrush();
                    background.ImageSource = new BitmapImage(new Uri("/Assets/main/menu/btn_1en_atv.png", UriKind.Relative));
                    bt_1.Background = background;
                    background = new ImageBrush();
                    background.ImageSource = new BitmapImage(new Uri("/Assets/main/menu/btn_2en_atv.png", UriKind.Relative));
                    bt_2.Background = background;
                    background = new ImageBrush();
                    background.ImageSource = new BitmapImage(new Uri("/Assets/main/menu/btn_3en_atv.png", UriKind.Relative));
                    bt_3.Background = background;
                    background = new ImageBrush();
                    background.ImageSource = new BitmapImage(new Uri("/Assets/main/menu/btn_4en_atv.png", UriKind.Relative));
                    bt_4.Background = background;
                    background = new ImageBrush();
                    background.ImageSource = new BitmapImage(new Uri("/Assets/main/menu/btn_5en_atv.png", UriKind.Relative));
                    bt_5.Background = background;
                    background = new ImageBrush();
                    background.ImageSource = new BitmapImage(new Uri("/Assets/main/menu/btn_6en_atv.png", UriKind.Relative));
                    bt_6.Background = background;
                }
            }
            HideProgressIndicator();
        }

        private void bt_comment_Click(object sender, RoutedEventArgs e)
        {
            if (!HasNetwork() || !HasInternet())
            {
                MessageBox.Show("No internet connection is available. Try again later.");
            }
            else
            {
                NavigationService.Navigate(new Uri("/BMTA_Comment.xaml", UriKind.Relative));
            }
        }

        private void bt_howto_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_Howto.xaml", UriKind.Relative));
        }
    }
}