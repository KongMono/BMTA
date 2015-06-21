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
using System.Diagnostics;
using BMTA.Item;
using System.Xml;
using News;
using System.Text.RegularExpressions;



namespace BMTA
{
    public partial class BMTA_Abouts_details : PhoneApplicationPage
    {
        public String lang = (Application.Current as App).Language;
        WebClient webClient = new WebClient();
        public FeedItemAbout items = new FeedItemAbout();
        public String url, page, header;
        public BMTA_Abouts_details()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            page = this.NavigationContext.QueryString["page"];
            header = this.NavigationContext.QueryString["Header"];


            titleName.Text = header;
           

            loadData(page);
        }
        private void loadData(String page)
        {

            if (page == "1")
            {
                if (lang.Equals("th"))
                {
                    url = "http://www.bmta.co.th/?q=th/feed/about-us";
                }
                else
                {
                    url = "http://www.bmta.co.th/?q=en/feed/about-us";
                }
            }
            else if (page == "2")
            {
                if (lang.Equals("th"))
                {
                    url = "http://www.bmta.co.th/?q=th/feed/vision";
                }
                else
                {
                    url = "http://www.bmta.co.th/?q=en/feed/vision";
                }

            }
            else
            {
                if (lang.Equals("th"))
                {
                    url = "http://www.bmta.co.th/?q=th/feed/contact";
                }
                else
                {
                    url = "http://www.bmta.co.th/?q=en/feed/contact";
                }
            }
            try
            {
                Debug.WriteLine("URL :" + url);
                webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(FeedData_DownloadStringCompleted);
                webClient.DownloadStringAsync(new Uri(url));

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
                foreach (var v in o.Descendants("item"))
                {
                    items.description = v.Element("description").Value;

                }
            }
            catch (XmlException ex)
            {
                MessageBox.Show("limited connectivity or invalid data.\nplease try again");
            }

            descriptionLabel.NavigateToString(items.description);
           
        }

     
        public static string StripTagsRegex(string source)
        {
         
            String resize_text = ResizeImage.Instance.GetResizeAutoHeight(300);
            String pre = @"<html><meta name='viewport' content='initial-scale=0,user-scalable=no,width=device-width'><style>body{color:#EFECE9 !important;background-color:black;font-weight:normal !important; font-size: 18px !important;}a,p,span,strong{font-size: 18px !important; color:#EFECE9 !important;}A:link{color:#EFECE9 !important;}
                         A:visited{color:white !important;}</style><body>";
            String last = "</body></html>";
            String center = source;
         

           
            foreach (Match i in Regex.Matches(center, @"<iframe.*src=\""(.*?)\"""))
            {
                Match match = Regex.Match(i.Groups[1].Value, @"youtube", RegexOptions.IgnoreCase);

                // Here we check the Match instance.
                if (match.Success)
                {
                    var youtube_url = i.Groups[1].Value;
                    //var p = i.Groups[1].Value;
                    var index = youtube_url.IndexOf('?');
                    if (index != -1 && index < youtube_url.Length)
                    {
                        try
                        {
                            var s = Uri.UnescapeDataString(youtube_url.Substring(0, index)).Split('/');
                            youtube_url = "http://youtube.com/embed/" + s[4];
                        }
                        catch { }
                    }

                    // Finally, we get the Group value and display it.
                    center = Regex.Replace(source, "<iframe.*></iframe>", "<a href=\"" + youtube_url + "\">คลิกชมวิดีโอจาก Youtube</a>");
                    //center = Regex.Replace(center, i.Groups[1].Value + "." + i.Groups[2].Value, resize_text + m.Groups[1].Value + "." + m.Groups[2].Value);
                    Debug.WriteLine("Regex.Matches = " + youtube_url);
                }
                else
                {
                    center = Regex.Replace(source, "<iframe.*></iframe>", string.Empty);
                    Debug.WriteLine("Not youtube!!");
                }


                Debug.WriteLine("center = " + center);
            }


            center = Regex.Replace(center, @"width=""[^\s]*""", "");
            center = Regex.Replace(center, @"height=""[^\s]*""", "");
            //center = Regex.Replace(center, @"height=""[^\s]*""", "height=\"auto\"");

            foreach (Match m in Regex.Matches(center, "src=\"(\\S+?)\\.(jpg|png|bmp)"))
            {
                center = Regex.Replace(center, m.Groups[1].Value + "." + m.Groups[2].Value, resize_text + m.Groups[1].Value + "." + m.Groups[2].Value);
                //Debug.WriteLine("Regex.Matches = " + m.Groups[1].Value + "." + m.Groups[2].Value);
            }
            //Debug.WriteLine("center = " + pre + center + last);
            return pre + center + last;

        }


        private void btback_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

    }
}