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
    public partial class BMTA_Search_Advance_start : PhoneApplicationPage
    {
        public String lang = (Application.Current as App).Language;

        public BMTA_Search_Advance_start()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {

            if (lang.Equals("th"))
            {
                txtboxstart.Hint = "คำค้นหา";
                txtboxend.Hint = "คำค้นหา";
                titleName.Text = "ระบบค้นหาอย่างละเอียด";
                txtstart.Text = "ต้นทาง";
                txtend.Text = "ปลายทาง";
                txtroute.Text = "เส้นทาง";
                txtrbustype.Text = "ประเภทรถ";
                txtselecttype.Text = "ระบุประเภท";

                head1.Text = "ค้นหาต้นทาง-ปลายทาง";
                head2.Text = "ค้นหาประเภทรถ";
                head3.Text = "เรียงผลการต้นหาตาม";

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
                txtboxstart.Hint = "Keyword";
                txtboxend.Hint = "Keyword";
                titleName.Text = "Advance Search";
                txtstart.Text = "Start";
                txtend.Text = "End";
                txtroute.Text = "Route";
                txtrbustype.Text = "Bus Type";
                txtselecttype.Text = "Select Type";

                head1.Text = "Search Start-End";
                head2.Text = "Search Bus Type";
                head3.Text = "Sort By Results";

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

        private void close_Click(object sender, RoutedEventArgs e)
        {
           
        }

        private void btTopMenu_Click(object sender, RoutedEventArgs e)
        {
          
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
        private void btback_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}