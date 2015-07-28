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
using BMTA.Item;
using BMTA.Usercontrols;


namespace BMTA
{
    public partial class BMTA_Speed_history : PhoneApplicationPage
    {
        public String lang = (Application.Current as App).Language;
        public BMTA_Speed_history()
        {
            InitializeComponent();
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

            List<SpeedCheckItem> Items = (Application.Current as App).CheckSpeedList;

            historylistbox.Items.Clear();

            foreach (SpeedCheckItem item in Items)
            {

                UCCheckspeedList UCCheckspeedList = new UCCheckspeedList();
                UCCheckspeedList.textdate.Text = item.date;
                UCCheckspeedList.texttime.Text = item.time;

                if (lang.Equals("th"))
                {
                    UCCheckspeedList.textRoute.Text = "ความเร็วเฉลี่ย";
                    UCCheckspeedList.texttitle_stop_name.Text = "สายรถเมล์";
                }
                else
                {
                    UCCheckspeedList.textRoute.Text = "speed";
                    UCCheckspeedList.texttitle_stop_name.Text = "busline";
                }

                UCCheckspeedList.textRouteNumber.Text = item.speed;
                UCCheckspeedList.textstop_name.Text = item.line_number;

                historylistbox.Items.Add(UCCheckspeedList);
            }
          
        }

        private void btAdd_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_Speed_Check.xaml", UriKind.Relative)); 
        }
    }
}