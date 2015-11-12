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
using SQLite;
using Windows.Storage;
using System.Windows.Input;
using BMTA.Item;
using System.Diagnostics;
using System.Xml;
using News;
using BMTA.Usercontrols;


namespace BMTA
{
    public partial class BMTA_News : PhoneApplicationPage
    {
        public String lang = (Application.Current as App).Language;
        public FeedItem items = new FeedItem();
        public List<FeedItemDescription> desclist = new List<FeedItemDescription>();
        static WebClient webClient;
        UCFeedList ls = new UCFeedList();
        private SQLiteConnection dbConn;

        public BMTA_News()
        {
            InitializeComponent();

            dbConn = new SQLiteConnection(App.DB_PATH);

            loadData();
        }
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (dbConn != null)
            {
                dbConn.Close();
            }
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (lang.Equals("th"))
            {
                titleName.Text = "ข่าวสาร";
            }
            else
            {
                titleName.Text = "News";
            }
        }
        private void loadData()
        {
            webClient = new WebClient();
            String url;
            try
            {
                if (!HasNetwork() || !HasInternet())
                {
                    if (lang.Equals("th"))
                    {
                        url = "http://www.bmta.co.th/?q=th/feed/news";
                    }
                    else
                    {
                        url = "http://www.bmta.co.th/?q=en/feed/news";
                    }

                    Debug.WriteLine("URL url_TopChannel = " + url);
                    webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(FeedData_DownloadStringCompleted);
                    webClient.DownloadStringAsync(new Uri(url));
                }
                else
                {
                    List<news> retrievedTasks = new List<news>();

                    retrievedTasks = dbConn.Query<news>("SELECT * FROM news");

                    foreach (var v in retrievedTasks.AsEnumerable().Reverse())
                    {
                        ls = new UCFeedList();

                        ls.path_img = v.image;
                        ls.texttitle_name.Text = HtmlRemoval.StripTagsRegexCompiled(v.title);
                        ls.textDescription.Text = HtmlRemoval.StripTagsRegexCompiled(v.description);

                        ls.texttitle_name.Text = ls.texttitle_name.Text.Replace("&quot;", "");
                        ls.textDescription.Text = ls.textDescription.Text.Replace("&quot;", "");

                        ls.texttitle_name.Text = ls.texttitle_name.Text.Replace("&nbsp;", "");
                        ls.textDescription.Text = ls.textDescription.Text.Replace("&nbsp;", "");

                        feedlistbox.Items.Add(ls);
                    }
                }
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void FeedData_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {

            if (e.Error != null)
            {
                return;
            }
            var o = XDocument.Parse(e.Result);
            try
            {
                items.title = XmlValueParser.ParseString(o.Root.Element("channel").Element("title"));
                items.description = XmlValueParser.ParseString(o.Root.Element("channel").Element("description"));
                FeedItemDescription desc = new FeedItemDescription();
                foreach (var v in o.Descendants("item"))
                {
                    desc = new FeedItemDescription();

                    desc.title = v.Element("title").Value;
                    desc.description = v.Element("description").Value;
                    desc.link = v.Element("link").Value;
                    desc.author = v.Element("author").Value;
                    desc.enclosure = v.Element("enclosure").Value;
                    desc.pubdate = v.Element("pubDate").Value;

                    desclist.Add(desc);
                }
                items.item = desclist;
            }
            catch (XmlException ex)
            {
                MessageBox.Show("limited connectivity or invalid data.\nplease try again");
            }

            foreach (var v in items.item.AsEnumerable().Reverse())
            {
                ls = new UCFeedList();

                ls.path_img = v.enclosure;
                ls.texttitle_name.Text = HtmlRemoval.StripTagsRegexCompiled(v.title);
                ls.textDescription.Text = HtmlRemoval.StripTagsRegexCompiled(v.description);

                ls.texttitle_name.Text = ls.texttitle_name.Text.Replace("&quot;", "");
                ls.textDescription.Text = ls.textDescription.Text.Replace("&quot;", "");

                ls.texttitle_name.Text = ls.texttitle_name.Text.Replace("&nbsp;", "");
                ls.textDescription.Text = ls.textDescription.Text.Replace("&nbsp;", "");

                feedlistbox.Items.Add(ls);
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

        private void btback_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void feedlistbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UCFeedList item = (sender as ListBox).SelectedItem as UCFeedList;
            if (feedlistbox.SelectedIndex != -1)
            {
                this.NavigationService.Navigate(new Uri("/BMTA_News_details.xaml?image=" + item.path_img + "&title=" + item.texttitle_name.Text + "&desc=" + item.textDescription.Text, UriKind.Relative));
            }
            feedlistbox.SelectedIndex = -1;
        }
    }
}