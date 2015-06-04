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
    public partial class BMTA_BusStop_pin : PhoneApplicationPage
    {
        public String lang = (Application.Current as App).Language;
        string _busline = "";
        string stop_name = "";
        private SQLiteConnection dbConn;
        List<Article> articles = new List<Article>();

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {

            // Create the database connection.  
            dbConn = new SQLiteConnection(App.DB_PATH);
            // Create the table Task, if it doesn't exist.  
            dbConn.CreateTable<busline>();

            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (dbConn != null)
            {
                dbConn.Close();
            }
            /// Close the database connection.
        }


        public BMTA_BusStop_pin()
        {
            InitializeComponent();
        }

        public void busstop(string bus_line, int li)
        {

            //SQLiteCommand sqlComm = new SQLiteCommand(dbConn);
            //sqlComm.CommandText = "SELECT * FROM busline Where bus_line='" + bus_line.ToString() + "' LIMIT " + li;
            //List<busline> retrievedTasks = sqlComm.ExecuteQuery<busline>();
            //foreach (var t in retrievedTasks)
            //{
            //    lblbustype.Text = t.bus_name;
            //    lblbustype1.Text = t.bus_discription;
            //    lbltime.Text = t.bus_startstop_time;
            //}
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (lang.Equals("th"))
            {
                titleName.Text = "ป้ายหยุดรถ";
            }
            else
            {
                titleName.Text = "Bus Stop";

            }

        }


        private void Box_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Enter))
            {
                //Do something
                // MessageBox.Show(busline_search.Text);
                //  NavigationService.Navigate(new Uri("/BMTA_bus_line.xaml?key=" + busline_search.Text, UriKind.Relative));
            }
        }

        private void btback_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void rhome_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_AppTh.xaml", UriKind.Relative));
        }

        private void btrigth_Click(object sender, RoutedEventArgs e)
        {
            if (articles != null)
            {
                // SelectNextCity();
                var next = SelectNextBusline();
                busno_bg.Content = next.btcontent;
                busstop(next.btcontent, 1);
            }
        }

        private int currentPosition = 0;
        public Article SelectNextBusline()
        {
            currentPosition++;
            if (currentPosition == articles.Count)
            {
                currentPosition = 0;
                return articles[0];
            }
            else
            {
                return articles[currentPosition];
            }
            // in this context `list` would be a class-level field.
        }
        public Article SelectPreviousBusline()
        {
            currentPosition--;
            if (currentPosition < 0)
            {
                // int carticle = articles.Count;
                currentPosition = articles.Count - 1;
                return articles[currentPosition];
            }
            else if (currentPosition == 0)
            {
                //  currentPosition = articles.Count;
                return articles[currentPosition];
            }
            else
            {
                // currentPosition = 0;
                return articles[currentPosition];
            }
            // in this context `list` would be a class-level field.
        }

        private void btleft_Click(object sender, RoutedEventArgs e)
        {
            if (articles != null)
            {
                // SelectNextCity();
                var next = SelectPreviousBusline();
                busno_bg.Content = next.btcontent;
                busstop(next.btcontent, 1);
            }
        }

    }
}