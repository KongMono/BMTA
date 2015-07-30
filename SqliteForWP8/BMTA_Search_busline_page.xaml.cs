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

//using XML_Parsing.Resources;
using System.Windows.Resources;
using System.Xml.Linq;
using SQLite;
using Windows.Storage;
using System.Windows.Input;
using BMTA.Item;
using System.Diagnostics;
using System.Xml;
using News;
using BMTA.Usercontrols;
using System.Collections.ObjectModel;


namespace BMTA
{
    public partial class BMTA_Search_busline_page : PhoneApplicationPage
    {
        public String lang = (Application.Current as App).Language;
        List<buslineItem> retrievedTasks = new List<buslineItem>();
        ObservableCollection<buslineItem> buslines = new ObservableCollection<buslineItem>();


        public BMTA_Search_busline_page()
        {
            InitializeComponent();

            loadData();
        }

        private void loadData()
        {
            retrievedTasks =  (Application.Current as App).DataSearchList;
            buslines = new ObservableCollection<buslineItem>(retrievedTasks);
            buslinelistbox.ItemsSource = buslines;
        }
        private void buslinelistbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            (Application.Current as App).DataBuslinehList = retrievedTasks;
            Article item = (sender as ListBox).SelectedItem as Article;
            if (buslinelistbox.SelectedIndex != -1)
            {
                this.NavigationService.Navigate(new Uri("/BMTA_bus_line_details.xaml?Search=false&Index=" + buslinelistbox.SelectedIndex.ToString(), UriKind.Relative));
            }
            buslinelistbox.SelectedIndex = -1;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (lang.Equals("th"))
            {
                titleName.Text = "ระบบค้นหาอย่างละเอียด";
            }
            else
            {
                titleName.Text = "Advance Search";
            }
        }

        private void btback_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

      

    }
}