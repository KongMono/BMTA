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
using System.Windows.Resources;
using System.Xml.Linq;
using BMTA.Usercontrols;

namespace BMTA
{
    public partial class BMTA_Abouts : PhoneApplicationPage
    {
        public String lang = (Application.Current as App).Language;
        UCFeedText text = new UCFeedText();
        public BMTA_Abouts()
        {
            InitializeComponent();
            loadData();
        }
        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (lang.Equals("th"))
            {
                titleName.Text = "เกี่ยวกับ ขสมก.";
            }
            else
            {
                titleName.Text = "About BMTA";
            }
        }

        private void loadData()
        {

            if (lang.Equals("th"))
            {
                text = new UCFeedText();
                text.texttitle_name.Text = "ประวัติ ขสมก.";
                text.texttitle_name.Tag = "1";
                feedlistbox.Items.Add(text);

                text = new UCFeedText();
                text.texttitle_name.Text = "วิสัยทัศน์/พันธกิจ/ค่านิยม";
                text.texttitle_name.Tag = "2";
                feedlistbox.Items.Add(text);

                text = new UCFeedText();
                text.texttitle_name.Text = "ติดต่อ ขสมก.";
                text.texttitle_name.Tag = "3";
                feedlistbox.Items.Add(text);
            }
            else
            {
                text = new UCFeedText();
                text.texttitle_name.Text = "BMTA History";
                text.texttitle_name.Tag = "1";
                feedlistbox.Items.Add(text);

                text = new UCFeedText();
                text.texttitle_name.Text = "Vision/Mission/Value";
                text.texttitle_name.Tag = "2";
                feedlistbox.Items.Add(text);

                text = new UCFeedText();
                text.texttitle_name.Text = "Contact Us";
                text.texttitle_name.Tag = "3";
                feedlistbox.Items.Add(text);
            }
          

        

        }

        private void btback_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
        private void feedlistbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UCFeedText item = (sender as ListBox).SelectedItem as UCFeedText;
            if (feedlistbox.SelectedIndex != -1)
            {
                this.NavigationService.Navigate(new Uri("/BMTA_Abouts_details.xaml?page=" + item.texttitle_name.Tag + "&Header=" + item.texttitle_name.Text, UriKind.Relative));
            }
            feedlistbox.SelectedIndex = -1;
        }
    }
}