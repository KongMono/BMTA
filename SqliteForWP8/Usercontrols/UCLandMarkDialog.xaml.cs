using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media.Imaging;
using BMTA.Item;
using System.Windows.Controls.Primitives;


namespace BMTA.Usercontrols
{
    public partial class UCLandMarkDialog : UserControl
    {
        public String lang = (Application.Current as App).Language;

        public UCLandMarkDialog()
        {
            InitializeComponent();
            Loaded += UCRoutingList_Loaded;
        }

        private void UCRoutingList_Loaded(object sender, RoutedEventArgs e)
        {
            if (lang.Equals("th"))
            {
                TextHeader1.Text = "โปรดระบุสถานที่ที่ต้องการบันทึก";
                TextHeader2.Text = "กรุณากรอกข้อมูลตัวอย่าง";
            }
            else
            {
                TextHeader1.Text = "Please specify a location to save";
                TextHeader2.Text = " Please enter sample data";
            }
        }     
    }
}
