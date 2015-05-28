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

        public BMTA_SearchAdvance()
        {
            InitializeComponent();

        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (lang.Equals("th"))
            {
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
                titleName.Text = "Advance Search";
                t1.Content = "Any";
                t2.Content = "Freeway";
                t3.Content = "Expressways";

                x1.Content = "Any";
                x2.Content = "Regular Bus";
                x3.Content = "Air Condition Bus";
            }
        }
    }
}