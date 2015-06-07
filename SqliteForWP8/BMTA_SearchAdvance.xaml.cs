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


    public partial class BMTA_SearchAdvance : PhoneApplicationPage
    {
        public String lang = (Application.Current as App).Language;
        private SQLiteConnection dbConn;
        public BMTA_SearchAdvance()
        {
            InitializeComponent();
        }

        private void btback_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (dbConn != null)
            {
                dbConn.Close();
                // Close the database connection.  
            }
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            // Create the database connection.  
            dbConn = new SQLiteConnection(App.DB_PATH);

            if (lang.Equals("th"))
            {
                headbusline.Text = "ค้นหาสายรถเมล์";
                headbustype.Text = "ค้นหาประเภทรถ";
                hintbusline.Hint = "กรุณากรอกตัวเลข";

                textbusline.Text = "สายรถเมล์";
                textbusroute.Text = "เส้นทาง";
                textbustype.Text = "ประเภทรถ";

                titleName.Text = "ระบบค้นหาอย่างละเอียด";
                t1.Content = "ทั้งหมด";
                t2.Content = "ปกติ";
                t3.Content = "ทางด่วน";

                x1.Content = "ทั้งหมด";
                x2.Content = "รถธรรมดา";
                x3.Content = "รถปรับอากาศ";
            }
            else
            {
                headbusline.Text = "Search Bus Line";
                headbustype.Text = "Search Bus Type";
                hintbusline.Hint = "Bus No.";

                textbusline.Text = "Bus Line";
                textbusroute.Text = "Route";
                textbustype.Text = "Bus Type";

                titleName.Text = "Advance Search";
                t1.Content = "All";
                t2.Content = "Freeway";
                t3.Content = "Expressways";

                x1.Content = "All";
                x2.Content = "Regular Bus";
                x3.Content = "Air Condition Bus";
            }
            (Application.Current as App).DataSearchList.Clear();
        }

        private void btsubmit_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(hintbusline.Text))
            {
                MessageBox.Show("กรุณากรอกสายรถเมล์");
                return;
            }

            List<buslineItem> retrievedTasks = new List<buslineItem>();
            var buslinePick = (ListPickerItem)busline.SelectedItem;
            var busRunningPick = (ListPickerItem)bustyperunning.SelectedItem;


            if ((buslinePick.Content == "ทั้งหมด" || buslinePick.Content == "All") && (busRunningPick.Content == "ทั้งหมด" || busRunningPick.Content == "All"))
            {
                retrievedTasks = dbConn.Query<buslineItem>("SELECT * FROM busline WHERE bus_name LIKE '%" + hintbusline.Text + "%' AND (bus_direction LIKE '%เข้าเมือง%'  OR bus_direction LIKE '%วนซ้าย%')");
            }
            else if ((buslinePick.Content == "ทั้งหมด" || buslinePick.Content == "All") && (busRunningPick.Content != "ทั้งหมด" || busRunningPick.Content != "All"))
            {
                retrievedTasks = dbConn.Query<buslineItem>("SELECT * FROM busline WHERE bus_name LIKE '%" + hintbusline.Text + "%' AND bus_running LIKE '%" + busRunningPick.Tag + "%' AND (bus_direction LIKE '%เข้าเมือง%'  OR bus_direction LIKE '%วนซ้าย%')");
            }

            else if ((buslinePick.Content != "ทั้งหมด" || buslinePick.Content != "All") && (busRunningPick.Content == "ทั้งหมด" || busRunningPick.Content == "All"))
            {
                retrievedTasks = dbConn.Query<buslineItem>("SELECT * FROM busline WHERE bus_name LIKE '%" + hintbusline.Text + "%' AND bustype LIKE '%" + buslinePick.Tag + "%' AND (bus_direction LIKE '%เข้าเมือง%'  OR bus_direction LIKE '%วนซ้าย%')");
            }
            else
            {
                retrievedTasks = dbConn.Query<buslineItem>("SELECT * FROM busline WHERE bus_name LIKE '%" + hintbusline.Text + "%' AND bustype LIKE '%" + buslinePick.Tag + "%' AND bus_running LIKE '%" + busRunningPick.Tag + "%' AND (bus_direction LIKE '%เข้าเมือง%'  OR bus_direction LIKE '%วนซ้าย%')");
            }

            if (retrievedTasks.Count > 0)
            {
                MessageBoxResult result = MessageBox.Show("ค้นหาสำเร็จ", (Application.Current as App).AppName, MessageBoxButton.OK);

                if (result == MessageBoxResult.OK)
                {
                    (Application.Current as App).DataSearchList = retrievedTasks;
                    this.NavigationService.Navigate(new Uri("/BMTA_bus_line_details.xaml?Search=true", UriKind.Relative));
                }
            }
            else
            {
                MessageBox.Show("ไม่พบข้อมูล");
                return;
            }

        }
    }
}