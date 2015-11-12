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
using System.Text.RegularExpressions;
using System.Windows.Media.Imaging;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using BMTA.Item;

namespace BMTA
{
    public partial class BMTA_Comment : PhoneApplicationPage
    {
        public String lang = (Application.Current as App).Language;
        UCFeedText text = new UCFeedText();
        ImageBrush background;
        WebClient webClient;
        public BMTA_Comment()
        {
            InitializeComponent();
            loadData();
        }
        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {

            if (lang.Equals("th"))
            {
                titleName.Text = "คำแนะนำ / ติชม";
                header1.Text = "คำแนะนำ/ติชม";
                header2.Text = "อีเมล์";
                header3.Text = "เบอร์โทรศัพท์";
                background = new ImageBrush();
                background.ImageSource = new BitmapImage(new Uri("/Assets/comment_send_th.png", UriKind.Relative));
                btnsend.Background = background;

                background = new ImageBrush();
                background.ImageSource = new BitmapImage(new Uri("/Assets/comment_cancel_th.png", UriKind.Relative));
                btncancel.Background = background;
            }
            else
            {
                titleName.Text = "FEEDBACK / COMMENT";
                header1.Text = "Feedback/Comment";
                header2.Text = "E-mail";
                header3.Text = "Telephone";
                background = new ImageBrush();
                background.ImageSource = new BitmapImage(new Uri("/Assets/comment_send_eng.png", UriKind.Relative));
                btnsend.Background = background;

                background = new ImageBrush();
                background.ImageSource = new BitmapImage(new Uri("/Assets/comment_cancel_eng.png", UriKind.Relative));
                btncancel.Background = background;
            }
        }

        private void loadData()
        {

        }

        private void btback_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void textBox3_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(e.Key.ToString(), "[0-9]"))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        public void callServiceComment()
        {
            webClient = new WebClient();
            webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            String url = "http://202.6.18.31:7777/getComment";
            string myParameters;
            try
            {
                myParameters = "comment=" + textBox1.Text + "&email=" + textBox2.Text + "&telephone=" + textBox3.Text;

                Debug.WriteLine("URL callServiceComment = " + url);
                webClient.UploadStringCompleted += new UploadStringCompletedEventHandler(callServiceComment_Completed);
                webClient.UploadStringAsync(new Uri(url), myParameters);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void callServiceComment_Completed(object sender, UploadStringCompletedEventArgs e)
        {
            commentItem comment = JsonConvert.DeserializeObject<commentItem>(e.Result);
            if (comment.result.Equals("1"))
            {
                MessageBox.Show("success..");
                NavigationService.GoBack();
            }
            else
            {
                MessageBox.Show("error something");
                MessageBox.Show(e.Result);
            }
        }

        private void btnsend_Click(object sender, RoutedEventArgs e)
        {

            Match match = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$").Match(textBox2.Text);
            if (!match.Success)
            {
                MessageBox.Show("Invalid Email");
                return;
            }

            match = new Regex("[0-9]").Match(textBox3.Text);
            if (!match.Success)
            {
                MessageBox.Show("Invalid Phone number");
                return;
            }

            callServiceComment();
        }

        private void btncancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}