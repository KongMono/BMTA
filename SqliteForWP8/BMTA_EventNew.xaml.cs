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
    public partial class BMTA_EventNew : PhoneApplicationPage
    {
        public BMTA_EventNew()
        {
            InitializeComponent();
     
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void btback_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

      
        private void close_Click(object sender, RoutedEventArgs e)
        {
           
        }

        private void btTopMenu_Click(object sender, RoutedEventArgs e)
        {
           
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {

        }
    }
}