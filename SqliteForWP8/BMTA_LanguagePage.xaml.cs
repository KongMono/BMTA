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
    public partial class BMTA_LanguagePage : PhoneApplicationPage
    {
        ProgressIndicator progressIndicator;
        
        public BMTA_LanguagePage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        private void btn_th_Click(object sender, RoutedEventArgs e)
        {
            (Application.Current as App).Language = "th";
            this.NavigationService.Navigate(new Uri("/BMTA_OrderPage.xaml", UriKind.Relative));
        }

        private void btn_en_Click(object sender, RoutedEventArgs e)
        {
            (Application.Current as App).Language = "en";
            this.NavigationService.Navigate(new Uri("/BMTA_OrderPage.xaml", UriKind.Relative));
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
    }
}