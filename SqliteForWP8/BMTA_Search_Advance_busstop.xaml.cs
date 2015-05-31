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
    public partial class BMTA_Search_Advance_busstop : PhoneApplicationPage
    {
        public String lang = (Application.Current as App).Language;

        public BMTA_Search_Advance_busstop()
        {
            InitializeComponent();
        }
        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (lang.Equals("th"))
            {
                hintbusstop.Hint = "ใส่คำค้นหา";

                headbusstop.Text = "ค้นหาป้ายรถเมล์";
                headbustype.Text = "ค้นหาประเภทรถ";
                headbussort.Text = "ประเภทการค้นหา";

                txtbusstop.Text = "ป้ายรถเมล์";
                txtbusroute.Text = "เส้นทาง";
                txtbustype.Text = "ประเภทรถ"; 
                txtselecttype.Text = "ระบุประเภท"; 

                titleName.Text = "ระบบค้นหาอย่างละเอียด";
                t1.Content = "ทั้งหมด";
                t2.Content = "ปกติ";
                t3.Content = "ทางด่วน";

                x1.Content = "ทั้งหมด";
                x2.Content = "รถธรรมดา";
                x3.Content = "รถปรับอากาศ";

                z1.Content = "เรียงตามระยะทาง";
                z2.Content = "เรียงตามราคา";
                z3.Content = "ต่อรถน้อยที่สุด";
            }
            else
            {
                hintbusstop.Hint = "Keyword";

                headbusstop.Text = "Search Bus Stop";
                headbustype.Text = "Search Bus Type";
                headbussort.Text = "Sort By Results";

                txtbusstop.Text = "Bus Stop";
                txtbusroute.Text = "Route";
                txtbustype.Text = "Bus Type";
                txtselecttype.Text = "Select Type"; 

                titleName.Text = "Advance Search";
                t1.Content = "Any";
                t2.Content = "Freeway";
                t3.Content = "Expressways";

                x1.Content = "Any";
                x2.Content = "Regular Bus";
                x3.Content = "Air Condition Bus";

                z1.Content = "Sort By Distance";
                z2.Content = "Sort By Price";
                z3.Content = "Fewer Transfers";
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
            NavigationService.Navigate(new Uri("/BMTA_Search_Advance_busstop.xaml", UriKind.Relative));
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

        private void btback_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void rhome_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_AppTh.xaml", UriKind.Relative));
        }

        private void rbusline_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_bus_line.xaml", UriKind.Relative));
        }

        private void rbusstop_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_BusStop.xaml", UriKind.Relative));
        }

        private void rcoor_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_BusCoordinates.xaml", UriKind.Relative));
        }

        private void rbusstartstop_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_BusStartStop.xaml", UriKind.Relative));
        }

        private void rbusspeed_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_Speed_history.xaml", UriKind.Relative));
        }

        private void rbusnew_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_EventNew.xaml", UriKind.Relative));
        }
    }
}