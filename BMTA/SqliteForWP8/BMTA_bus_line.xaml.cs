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
    public partial class BMTA_bus_line : PhoneApplicationPage
    {
        /// <summary>
        /// The database path.
        /// </summary>
        public static string DB_PATH = Path.Combine(Path.Combine(ApplicationData.Current.LocalFolder.Path, "bmtadatabase.sqlite"));

        /// <summary>
        /// The sqlite connection.
        /// </summary>
        private SQLiteConnection dbConn;

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            dbConn = new SQLiteConnection(DB_PATH);
            dbConn.CreateTable<busline>(); 
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (dbConn != null)
            {
                /// Close the database connection.
                dbConn.Close();
            }
        }

        public BMTA_bus_line()
        {
            InitializeComponent();
          
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            rightmenu.Visibility = System.Windows.Visibility.Collapsed;
            rightmenux.Visibility = System.Windows.Visibility.Collapsed;
            close.Visibility = System.Windows.Visibility.Collapsed;
          //  BuildLocalizedApplicationbar();
            if (!HasNetwork())
            {
                Application.Current.Terminate();
            }
            else if (!HasInternet())
            {
                Application.Current.Terminate();
            }
            else
            {
                //load data 
                SQLiteCommand sqlComm = new SQLiteCommand(dbConn);
                sqlComm.CommandText = "SELECT * FROM busline where bus_type !='' Group by bus_line";
                List<busline> retrievedTasks = retrievedTasks = sqlComm.ExecuteQuery<busline>();
               // listbox1.Items.Clear();
                
                Dispatcher.BeginInvoke(() =>
                {
                    StackPanel panel = new StackPanel();
                    var articles = new List<Article>();
                    panel.Orientation = System.Windows.Controls.Orientation.Vertical;
                    foreach (var t in retrievedTasks)
                    {
                        //Button btn = new Button() { Content = t.bus_line };
                        //Button img = new Button();
                        //btn.Width = 209;
                        //btn.Height = 205;
                        //img.Width = 104;
                        //img.Height = 21;
                        //if (t.bus_type == "ขสมก.รถธรรมดา")
                        //{
                        //    var uriString = @"Assets/bg_nomal.png";
                        //    btn.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(uriString, UriKind.Relative)) };
                        //    var uriArrow = @"Assets/arrow.png";
                        //    img.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(uriArrow, UriKind.Relative)) };
                        //}
                        //else if (t.bus_type == "ขสมก.ปอ.")
                        //{
                        //    var uriString = @"Assets/bg_nomal2.png";
                        //    btn.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(uriString, UriKind.Relative)) };
                        //    var uriArrow = @"Assets/arrow.png";
                        //    img.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(uriArrow, UriKind.Relative)) };
                        //}
                        //else if (t.bus_type == "ร่วมบริการ.รถธรรมดา")
                        //{
                        //    var uriString = @"Assets/bg_nomal1.png";
                        //    btn.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(uriString, UriKind.Relative)) };
                        //    var uriArrow = @"Assets/arrow.png";
                        //    img.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(uriArrow, UriKind.Relative)) };
                        //}
                        //else if (t.bus_type == "ร่วมบริการ.ปอ.")
                        //{
                        //    var uriString = @"Assets/bg_nomal3.png";
                        //    btn.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(uriString, UriKind.Relative)) };
                        //    var uriArrow = @"Assets/arrow.png";
                        //    img.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(uriArrow, UriKind.Relative)) };
                        //}
                        //else if (t.bus_type == "มินิบัส.รถธรรมดา")
                        //{
                        //    var uriString = @"Assets/bg_nomal3.png";
                        //    btn.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(uriString, UriKind.Relative)) };
                        //    var uriArrow = @"Assets/arrow.png";
                        //    img.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(uriArrow, UriKind.Relative)) };
                        //}

                        //btn.Foreground = new SolidColorBrush(Colors.Black);
                        //btn.BorderBrush = new SolidColorBrush(Colors.Transparent);
                        //btn.FontSize = 70;
                        //btn.Tag = t.bus_line;
                        //btn.Click += btn_Click;
                       // listbox1.Items.Add(btn);
                       
                      //  listbox1.ItemsSource = retrievedTasks;
                        if (t.bus_type == "ขสมก.รถธรรมดา") 
                        {
                            Article article = new Article() { btcontent = t.bus_line, bg = @"Assets/bg_nomal.png", Start = t.bus_start, Stop = t.bus_stop, ImagePath = @"Assets/arrow.png" };
                            articles.Add(article);
                        }
                        else if (t.bus_type == "ขสมก.ปอ.")
                        {
                            Article article = new Article() { btcontent = t.bus_line, bg = @"Assets/bg_nomal2.png", Start = t.bus_start, Stop = t.bus_stop, ImagePath = @"Assets/arrow.png" };
                            articles.Add(article);
                        }
                        else if (t.bus_type == "ร่วมบริการ.รถธรรมดา")
                        {
                            Article article = new Article() { btcontent = t.bus_line, bg = @"Assets/bg_nomal1.png", Start = t.bus_start, Stop = t.bus_stop, ImagePath = @"Assets/arrow.png" };
                            articles.Add(article);
                        }
                        else if (t.bus_type == "ร่วมบริการ.ปอ.")
                        {
                            Article article = new Article() { btcontent = t.bus_line, bg = @"Assets/bg_nomal3.png", Start = t.bus_start, Stop = t.bus_stop, ImagePath = @"Assets/arrow.png" };
                            articles.Add(article);
                        }
                        else if (t.bus_type == "มินิบัส.รถธรรมดา")
                        {
                            Article article = new Article() { btcontent = t.bus_line, bg = @"Assets/bg_nomal3.png", Start = t.bus_start, Stop = t.bus_stop, ImagePath = @"Assets/arrow.png" };
                            articles.Add(article);
                        }

                       
                        
                        
                        
                    }
                    listbox1.DataContext = articles;
                });


                //end load data

                string x = BMTA.clGetResolution.Width.ToString();
                string y = BMTA.clGetResolution.Height.ToString();
                string xy = x + "x" + y;
                if (x == "480")
                {
                    ImageBrush brush = new ImageBrush
                    {
                        ImageSource = new System.Windows.Media.Imaging.BitmapImage(new Uri("/Assets/480x852/BMTA_busline_bg.png", UriKind.Relative)),
                        Opacity = 1d
                    };
                    this.LayoutRoot.Background = brush;
                    brush.Stretch = Stretch.Fill;
                }
                else if (x == "720")
                {
                    ImageBrush brush = new ImageBrush
                    {
                        ImageSource = new System.Windows.Media.Imaging.BitmapImage(new Uri("/Assets/720x1280/BMTA_busline_bg.png", UriKind.Relative)),
                        Opacity = 1d
                    };
                    this.LayoutRoot.Background = brush;
                    brush.Stretch = Stretch.Fill;
                }
                else
                {
                    ImageBrush brush = new ImageBrush
                    {
                        ImageSource = new System.Windows.Media.Imaging.BitmapImage(new Uri("/Assets/768x1280/BMTA_busline_bg.png", UriKind.Relative)),
                        Opacity = 1d
                    };
                    this.LayoutRoot.Background = brush;
                    brush.Stretch = Stretch.Fill;
                }
       
            }
        }
        private void BuildLocalizedApplicationbar()
        {
            ApplicationBar = new ApplicationBar();

            ApplicationBar.Mode = ApplicationBarMode.Default;
            ApplicationBar.Opacity = 0.78;
            ApplicationBar.IsVisible = true;
            ApplicationBar.IsMenuEnabled = true;
           // ApplicationBar.BackgroundColor = Color.FromArgb(100, 0, 165, 78);
            ApplicationBar.BackgroundColor = Colors.Green;

            ApplicationBarIconButton button1 = new ApplicationBarIconButton();
            button1.IconUri = new Uri("/Assets/bt_main_th/btf_bus.png", UriKind.Relative);
            button1.Text = "สายรถเมล์";
            ApplicationBar.Buttons.Add(button1);
            button1.Click += new EventHandler(button1_Click);

            ApplicationBarIconButton button2 = new ApplicationBarIconButton();
            button2.IconUri = new Uri("/Assets/bt_main_th/btf_busstop.png", UriKind.Relative);
            button2.Text = "ป้ายรถเมล์";
            ApplicationBar.Buttons.Add(button2);
            button2.Click += new EventHandler(button2_Click);

            ApplicationBarIconButton button3 = new ApplicationBarIconButton();
            button3.IconUri = new Uri("/Assets/bt_main_th/btf_place.png", UriKind.Relative);
            button3.Text = "สถานที่สำคัญ";
            ApplicationBar.Buttons.Add(button3);
            button3.Click += new EventHandler(button3_Click);

            ApplicationBarIconButton button4 = new ApplicationBarIconButton();
            button4.IconUri = new Uri("/Assets/bt_main_th/btf_startstop.png", UriKind.Relative);
            button4.Text = "ต้นทางปลายทาง";
            ApplicationBar.Buttons.Add(button4);
            button4.Click += new EventHandler(button4_Click);


        }
        private void button1_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_bus_line.xaml", UriKind.Relative));
        }
        private void button2_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_BusStop.xaml", UriKind.Relative));
        }
        private void button3_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_BusCoordinates.xaml", UriKind.Relative));
        }
        private void button4_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_BusStartStop.xaml", UriKind.Relative));
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
        private void btSearchAd_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_SearchAdvance.xaml", UriKind.Relative));
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
        private void btn_Click(object sender, RoutedEventArgs e)
        {
            // throw new NotImplementedException();
          //  MessageBox.Show();
            Button _button = (Button)sender;
           // NavigationService.Navigate(new System.Uri(_button.Tag.ToString()));
            NavigationService.Navigate(new Uri("/BMTA_bus_line_details.xaml?key=" + _button.Tag.ToString(), UriKind.Relative));

          
        }
        public class Article
        {
            public string Start { get; set; }

            public string Stop { get; set; }

            public string ImagePath { get; set; }

            public string bg { get; set; }

            public string btcontent { get; set; }
        }
        public sealed class busline
        {
            /// <summary>
            /// You can create an integer primary key and let the SQLite control it.
            /// </summary>
            [PrimaryKey, AutoIncrement]
            public int id { get; set; }

            public string bus_id { get; set; }

            public string bus_line { get; set; }

            public string bus_type { get; set; }

            public string bus_start { get; set; }

            public string bus_stop { get; set; }

            public string ImagePath { get; set; }
             

            public override string ToString()
            {
                return bus_line;
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //load data 
            SQLiteCommand sqlComm = new SQLiteCommand(dbConn);
            sqlComm.CommandText = "SELECT * FROM busline where bus_type !='' and bus_line LIKE '1%' Group by bus_line";
            List<busline> retrievedTasks = retrievedTasks = sqlComm.ExecuteQuery<busline>();
           // listbox1.Items.Clear();
            Dispatcher.BeginInvoke(() =>
            {
                StackPanel panel = new StackPanel();
                var articles = new List<Article>();
                panel.Orientation = System.Windows.Controls.Orientation.Vertical;
                foreach (var t in retrievedTasks)
                {
                    //Button btn = new Button() { Content = t.bus_line };
                    //btn.Width = 209;
                    //btn.Height = 205;
                    //if (t.bus_type == "ขสมก.รถธรรมดา")
                    //{
                    //    var uriString = @"Assets/bg_nomal.png";
                    //    btn.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(uriString, UriKind.Relative)) };
                    //}
                    //else if (t.bus_type == "ขสมก.ปอ.")
                    //{
                    //    var uriString = @"Assets/bg_nomal2.png";
                    //    btn.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(uriString, UriKind.Relative)) };
                    //}
                    //else if (t.bus_type == "ร่วมบริการ.รถธรรมดา")
                    //{
                    //    var uriString = @"Assets/bg_nomal1.png";
                    //    btn.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(uriString, UriKind.Relative)) };
                    //}
                    //else if (t.bus_type == "ร่วมบริการ.ปอ.")
                    //{
                    //    var uriString = @"Assets/bg_nomal3.png";
                    //    btn.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(uriString, UriKind.Relative)) };
                    //}
                    //else if (t.bus_type == "มินิบัส.รถธรรมดา")
                    //{
                    //    var uriString = @"Assets/bg_nomal3.png";
                    //    btn.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(uriString, UriKind.Relative)) };
                    //}
                    //btn.Foreground = new SolidColorBrush(Colors.Black);
                    //btn.BorderBrush = new SolidColorBrush(Colors.Transparent);
                    //btn.FontSize = 70;
                    //btn.Tag = t.bus_line;
                    //btn.Click += btn_Click;
                    //listbox1.Items.Add(btn);

                    if (t.bus_type == "ขสมก.รถธรรมดา")
                    {
                        Article article = new Article() { btcontent = t.bus_line, bg = @"Assets/bg_nomal.png", Start = t.bus_start, Stop = t.bus_stop, ImagePath = @"Assets/arrow.png" };
                        articles.Add(article);
                    }
                    else if (t.bus_type == "ขสมก.ปอ.")
                    {
                        Article article = new Article() { btcontent = t.bus_line, bg = @"Assets/bg_nomal2.png", Start = t.bus_start, Stop = t.bus_stop, ImagePath = @"Assets/arrow.png" };
                        articles.Add(article);
                    }
                    else if (t.bus_type == "ร่วมบริการ.รถธรรมดา")
                    {
                        Article article = new Article() { btcontent = t.bus_line, bg = @"Assets/bg_nomal1.png", Start = t.bus_start, Stop = t.bus_stop, ImagePath = @"Assets/arrow.png" };
                        articles.Add(article);
                    }
                    else if (t.bus_type == "ร่วมบริการ.ปอ.")
                    {
                        Article article = new Article() { btcontent = t.bus_line, bg = @"Assets/bg_nomal3.png", Start = t.bus_start, Stop = t.bus_stop, ImagePath = @"Assets/arrow.png" };
                        articles.Add(article);
                    }
                    else if (t.bus_type == "มินิบัส.รถธรรมดา")
                    {
                        Article article = new Article() { btcontent = t.bus_line, bg = @"Assets/bg_nomal3.png", Start = t.bus_start, Stop = t.bus_stop, ImagePath = @"Assets/arrow.png" };
                        articles.Add(article);
                    }

                }
                listbox1.DataContext = articles;
            });

        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //load data 
            SQLiteCommand sqlComm = new SQLiteCommand(dbConn);
            sqlComm.CommandText = "SELECT * FROM busline where bus_type !='' and bus_line LIKE '2%' Group by bus_line";
            List<busline> retrievedTasks = retrievedTasks = sqlComm.ExecuteQuery<busline>();
          //  listbox1.Items.Clear();
            Dispatcher.BeginInvoke(() =>
            {
                StackPanel panel = new StackPanel();
                var articles = new List<Article>();
                panel.Orientation = System.Windows.Controls.Orientation.Vertical;
                foreach (var t in retrievedTasks)
                {
                   

                    if (t.bus_type == "ขสมก.รถธรรมดา")
                    {
                        Article article = new Article() { btcontent = t.bus_line, bg = @"Assets/bg_nomal.png", Start = t.bus_start, Stop = t.bus_stop, ImagePath = @"Assets/arrow.png" };
                        articles.Add(article);
                    }
                    else if (t.bus_type == "ขสมก.ปอ.")
                    {
                        Article article = new Article() { btcontent = t.bus_line, bg = @"Assets/bg_nomal2.png", Start = t.bus_start, Stop = t.bus_stop, ImagePath = @"Assets/arrow.png" };
                        articles.Add(article);
                    }
                    else if (t.bus_type == "ร่วมบริการ.รถธรรมดา")
                    {
                        Article article = new Article() { btcontent = t.bus_line, bg = @"Assets/bg_nomal1.png", Start = t.bus_start, Stop = t.bus_stop, ImagePath = @"Assets/arrow.png" };
                        articles.Add(article);
                    }
                    else if (t.bus_type == "ร่วมบริการ.ปอ.")
                    {
                        Article article = new Article() { btcontent = t.bus_line, bg = @"Assets/bg_nomal3.png", Start = t.bus_start, Stop = t.bus_stop, ImagePath = @"Assets/arrow.png" };
                        articles.Add(article);
                    }
                    else if (t.bus_type == "มินิบัส.รถธรรมดา")
                    {
                        Article article = new Article() { btcontent = t.bus_line, bg = @"Assets/bg_nomal3.png", Start = t.bus_start, Stop = t.bus_stop, ImagePath = @"Assets/arrow.png" };
                        articles.Add(article);
                    }
                }
                listbox1.DataContext = articles;
            });
        }
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            //load data 
            SQLiteCommand sqlComm = new SQLiteCommand(dbConn);
            sqlComm.CommandText = "SELECT * FROM busline where bus_type !='' and bus_line LIKE '3%' Group by bus_line";
            List<busline> retrievedTasks = retrievedTasks = sqlComm.ExecuteQuery<busline>();
           // listbox1.Items.Clear();
            Dispatcher.BeginInvoke(() =>
            {
                StackPanel panel = new StackPanel();
                var articles = new List<Article>();
                panel.Orientation = System.Windows.Controls.Orientation.Vertical;
                foreach (var t in retrievedTasks)
                {
                    if (t.bus_type == "ขสมก.รถธรรมดา")
                    {
                        Article article = new Article() { btcontent = t.bus_line, bg = @"Assets/bg_nomal.png", Start = t.bus_start, Stop = t.bus_stop, ImagePath = @"Assets/arrow.png" };
                        articles.Add(article);
                    }
                    else if (t.bus_type == "ขสมก.ปอ.")
                    {
                        Article article = new Article() { btcontent = t.bus_line, bg = @"Assets/bg_nomal2.png", Start = t.bus_start, Stop = t.bus_stop, ImagePath = @"Assets/arrow.png" };
                        articles.Add(article);
                    }
                    else if (t.bus_type == "ร่วมบริการ.รถธรรมดา")
                    {
                        Article article = new Article() { btcontent = t.bus_line, bg = @"Assets/bg_nomal1.png", Start = t.bus_start, Stop = t.bus_stop, ImagePath = @"Assets/arrow.png" };
                        articles.Add(article);
                    }
                    else if (t.bus_type == "ร่วมบริการ.ปอ.")
                    {
                        Article article = new Article() { btcontent = t.bus_line, bg = @"Assets/bg_nomal3.png", Start = t.bus_start, Stop = t.bus_stop, ImagePath = @"Assets/arrow.png" };
                        articles.Add(article);
                    }
                    else if (t.bus_type == "มินิบัส.รถธรรมดา")
                    {
                        Article article = new Article() { btcontent = t.bus_line, bg = @"Assets/bg_nomal3.png", Start = t.bus_start, Stop = t.bus_stop, ImagePath = @"Assets/arrow.png" };
                        articles.Add(article);
                    }
                }
                listbox1.DataContext = articles;
            });
        }
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            //load data 
            SQLiteCommand sqlComm = new SQLiteCommand(dbConn);
            sqlComm.CommandText = "SELECT * FROM busline where bus_type !='' and bus_line LIKE '4%' Group by bus_line";
            List<busline> retrievedTasks = retrievedTasks = sqlComm.ExecuteQuery<busline>();
           // listbox1.Items.Clear();
            Dispatcher.BeginInvoke(() =>
            {
                StackPanel panel = new StackPanel();
                var articles = new List<Article>();
                panel.Orientation = System.Windows.Controls.Orientation.Vertical;
                foreach (var t in retrievedTasks)
                {
                    if (t.bus_type == "ขสมก.รถธรรมดา")
                    {
                        Article article = new Article() { btcontent = t.bus_line, bg = @"Assets/bg_nomal.png", Start = t.bus_start, Stop = t.bus_stop, ImagePath = @"Assets/arrow.png" };
                        articles.Add(article);
                    }
                    else if (t.bus_type == "ขสมก.ปอ.")
                    {
                        Article article = new Article() { btcontent = t.bus_line, bg = @"Assets/bg_nomal2.png", Start = t.bus_start, Stop = t.bus_stop, ImagePath = @"Assets/arrow.png" };
                        articles.Add(article);
                    }
                    else if (t.bus_type == "ร่วมบริการ.รถธรรมดา")
                    {
                        Article article = new Article() { btcontent = t.bus_line, bg = @"Assets/bg_nomal1.png", Start = t.bus_start, Stop = t.bus_stop, ImagePath = @"Assets/arrow.png" };
                        articles.Add(article);
                    }
                    else if (t.bus_type == "ร่วมบริการ.ปอ.")
                    {
                        Article article = new Article() { btcontent = t.bus_line, bg = @"Assets/bg_nomal3.png", Start = t.bus_start, Stop = t.bus_stop, ImagePath = @"Assets/arrow.png" };
                        articles.Add(article);
                    }
                    else if (t.bus_type == "มินิบัส.รถธรรมดา")
                    {
                        Article article = new Article() { btcontent = t.bus_line, bg = @"Assets/bg_nomal3.png", Start = t.bus_start, Stop = t.bus_stop, ImagePath = @"Assets/arrow.png" };
                        articles.Add(article);
                    }
                }
                listbox1.DataContext = articles;
            });
        }
        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            //load data 
            SQLiteCommand sqlComm = new SQLiteCommand(dbConn);
            sqlComm.CommandText = "SELECT * FROM busline where bus_type !='' and bus_line LIKE '5%' Group by bus_line";
            List<busline> retrievedTasks = retrievedTasks = sqlComm.ExecuteQuery<busline>();
           // listbox1.Items.Clear();
            Dispatcher.BeginInvoke(() =>
            {
                StackPanel panel = new StackPanel();
                var articles = new List<Article>();
                panel.Orientation = System.Windows.Controls.Orientation.Vertical;
                foreach (var t in retrievedTasks)
                {
                    if (t.bus_type == "ขสมก.รถธรรมดา")
                    {
                        Article article = new Article() { btcontent = t.bus_line, bg = @"Assets/bg_nomal.png", Start = t.bus_start, Stop = t.bus_stop, ImagePath = @"Assets/arrow.png" };
                        articles.Add(article);
                    }
                    else if (t.bus_type == "ขสมก.ปอ.")
                    {
                        Article article = new Article() { btcontent = t.bus_line, bg = @"Assets/bg_nomal2.png", Start = t.bus_start, Stop = t.bus_stop, ImagePath = @"Assets/arrow.png" };
                        articles.Add(article);
                    }
                    else if (t.bus_type == "ร่วมบริการ.รถธรรมดา")
                    {
                        Article article = new Article() { btcontent = t.bus_line, bg = @"Assets/bg_nomal1.png", Start = t.bus_start, Stop = t.bus_stop, ImagePath = @"Assets/arrow.png" };
                        articles.Add(article);
                    }
                    else if (t.bus_type == "ร่วมบริการ.ปอ.")
                    {
                        Article article = new Article() { btcontent = t.bus_line, bg = @"Assets/bg_nomal3.png", Start = t.bus_start, Stop = t.bus_stop, ImagePath = @"Assets/arrow.png" };
                        articles.Add(article);
                    }
                    else if (t.bus_type == "มินิบัส.รถธรรมดา")
                    {
                        Article article = new Article() { btcontent = t.bus_line, bg = @"Assets/bg_nomal3.png", Start = t.bus_start, Stop = t.bus_stop, ImagePath = @"Assets/arrow.png" };
                        articles.Add(article);
                    }
                }
                listbox1.DataContext = articles;
            });
        }
        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            //load data 
            SQLiteCommand sqlComm = new SQLiteCommand(dbConn);
            sqlComm.CommandText = "SELECT * FROM busline where bus_type !='' and bus_line LIKE '6%' Group by bus_line";
            List<busline> retrievedTasks = retrievedTasks = sqlComm.ExecuteQuery<busline>();
           // listbox1.Items.Clear();
            Dispatcher.BeginInvoke(() =>
            {
                StackPanel panel = new StackPanel();
                var articles = new List<Article>();
                panel.Orientation = System.Windows.Controls.Orientation.Vertical;
                foreach (var t in retrievedTasks)
                {
                    if (t.bus_type == "ขสมก.รถธรรมดา")
                    {
                        Article article = new Article() { btcontent = t.bus_line, bg = @"Assets/bg_nomal.png", Start = t.bus_start, Stop = t.bus_stop, ImagePath = @"Assets/arrow.png" };
                        articles.Add(article);
                    }
                    else if (t.bus_type == "ขสมก.ปอ.")
                    {
                        Article article = new Article() { btcontent = t.bus_line, bg = @"Assets/bg_nomal2.png", Start = t.bus_start, Stop = t.bus_stop, ImagePath = @"Assets/arrow.png" };
                        articles.Add(article);
                    }
                    else if (t.bus_type == "ร่วมบริการ.รถธรรมดา")
                    {
                        Article article = new Article() { btcontent = t.bus_line, bg = @"Assets/bg_nomal1.png", Start = t.bus_start, Stop = t.bus_stop, ImagePath = @"Assets/arrow.png" };
                        articles.Add(article);
                    }
                    else if (t.bus_type == "ร่วมบริการ.ปอ.")
                    {
                        Article article = new Article() { btcontent = t.bus_line, bg = @"Assets/bg_nomal3.png", Start = t.bus_start, Stop = t.bus_stop, ImagePath = @"Assets/arrow.png" };
                        articles.Add(article);
                    }
                    else if (t.bus_type == "มินิบัส.รถธรรมดา")
                    {
                        Article article = new Article() { btcontent = t.bus_line, bg = @"Assets/bg_nomal3.png", Start = t.bus_start, Stop = t.bus_stop, ImagePath = @"Assets/arrow.png" };
                        articles.Add(article);
                    }
                }
                listbox1.DataContext = articles;
            });
        }
        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            //load data 
            SQLiteCommand sqlComm = new SQLiteCommand(dbConn);
            sqlComm.CommandText = "SELECT * FROM busline where bus_type !='' and bus_line LIKE '7%' Group by bus_line";
            List<busline> retrievedTasks = retrievedTasks = sqlComm.ExecuteQuery<busline>();
           // listbox1.Items.Clear();
            Dispatcher.BeginInvoke(() =>
            {
                StackPanel panel = new StackPanel();
                var articles = new List<Article>();
                panel.Orientation = System.Windows.Controls.Orientation.Vertical;
                foreach (var t in retrievedTasks)
                {
                    if (t.bus_type == "ขสมก.รถธรรมดา")
                    {
                        Article article = new Article() { btcontent = t.bus_line, bg = @"Assets/bg_nomal.png", Start = t.bus_start, Stop = t.bus_stop, ImagePath = @"Assets/arrow.png" };
                        articles.Add(article);
                    }
                    else if (t.bus_type == "ขสมก.ปอ.")
                    {
                        Article article = new Article() { btcontent = t.bus_line, bg = @"Assets/bg_nomal2.png", Start = t.bus_start, Stop = t.bus_stop, ImagePath = @"Assets/arrow.png" };
                        articles.Add(article);
                    }
                    else if (t.bus_type == "ร่วมบริการ.รถธรรมดา")
                    {
                        Article article = new Article() { btcontent = t.bus_line, bg = @"Assets/bg_nomal1.png", Start = t.bus_start, Stop = t.bus_stop, ImagePath = @"Assets/arrow.png" };
                        articles.Add(article);
                    }
                    else if (t.bus_type == "ร่วมบริการ.ปอ.")
                    {
                        Article article = new Article() { btcontent = t.bus_line, bg = @"Assets/bg_nomal3.png", Start = t.bus_start, Stop = t.bus_stop, ImagePath = @"Assets/arrow.png" };
                        articles.Add(article);
                    }
                    else if (t.bus_type == "มินิบัส.รถธรรมดา")
                    {
                        Article article = new Article() { btcontent = t.bus_line, bg = @"Assets/bg_nomal3.png", Start = t.bus_start, Stop = t.bus_stop, ImagePath = @"Assets/arrow.png" };
                        articles.Add(article);
                    }
                }
                listbox1.DataContext = articles;
            });
        }
        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            //load data 
            SQLiteCommand sqlComm = new SQLiteCommand(dbConn);
            sqlComm.CommandText = "SELECT * FROM busline where bus_type !='' and bus_line LIKE '8%' Group by bus_line";
            List<busline> retrievedTasks = retrievedTasks = sqlComm.ExecuteQuery<busline>();
           // listbox1.Items.Clear();
            Dispatcher.BeginInvoke(() =>
            {
                StackPanel panel = new StackPanel();
                var articles = new List<Article>();
                panel.Orientation = System.Windows.Controls.Orientation.Vertical;
                foreach (var t in retrievedTasks)
                {
                    if (t.bus_type == "ขสมก.รถธรรมดา")
                    {
                        Article article = new Article() { btcontent = t.bus_line, bg = @"Assets/bg_nomal.png", Start = t.bus_start, Stop = t.bus_stop, ImagePath = @"Assets/arrow.png" };
                        articles.Add(article);
                    }
                    else if (t.bus_type == "ขสมก.ปอ.")
                    {
                        Article article = new Article() { btcontent = t.bus_line, bg = @"Assets/bg_nomal2.png", Start = t.bus_start, Stop = t.bus_stop, ImagePath = @"Assets/arrow.png" };
                        articles.Add(article);
                    }
                    else if (t.bus_type == "ร่วมบริการ.รถธรรมดา")
                    {
                        Article article = new Article() { btcontent = t.bus_line, bg = @"Assets/bg_nomal1.png", Start = t.bus_start, Stop = t.bus_stop, ImagePath = @"Assets/arrow.png" };
                        articles.Add(article);
                    }
                    else if (t.bus_type == "ร่วมบริการ.ปอ.")
                    {
                        Article article = new Article() { btcontent = t.bus_line, bg = @"Assets/bg_nomal3.png", Start = t.bus_start, Stop = t.bus_stop, ImagePath = @"Assets/arrow.png" };
                        articles.Add(article);
                    }
                    else if (t.bus_type == "มินิบัส.รถธรรมดา")
                    {
                        Article article = new Article() { btcontent = t.bus_line, bg = @"Assets/bg_nomal3.png", Start = t.bus_start, Stop = t.bus_stop, ImagePath = @"Assets/arrow.png" };
                        articles.Add(article);
                    }
                }
                listbox1.DataContext = articles;
            });
        }
        private void Button_Click_8(object sender, RoutedEventArgs e)
        {
            //load data 
            SQLiteCommand sqlComm = new SQLiteCommand(dbConn);
            sqlComm.CommandText = "SELECT * FROM busline where bus_type !='' and bus_line LIKE '9%' Group by bus_line";
            List<busline> retrievedTasks = retrievedTasks = sqlComm.ExecuteQuery<busline>();
           // listbox1.Items.Clear();
            Dispatcher.BeginInvoke(() =>
            {
                StackPanel panel = new StackPanel();
                var articles = new List<Article>();
                panel.Orientation = System.Windows.Controls.Orientation.Vertical;
                foreach (var t in retrievedTasks)
                {
                    if (t.bus_type == "ขสมก.รถธรรมดา")
                    {
                        Article article = new Article() { btcontent = t.bus_line, bg = @"Assets/bg_nomal.png", Start = t.bus_start, Stop = t.bus_stop, ImagePath = @"Assets/arrow.png" };
                        articles.Add(article);
                    }
                    else if (t.bus_type == "ขสมก.ปอ.")
                    {
                        Article article = new Article() { btcontent = t.bus_line, bg = @"Assets/bg_nomal2.png", Start = t.bus_start, Stop = t.bus_stop, ImagePath = @"Assets/arrow.png" };
                        articles.Add(article);
                    }
                    else if (t.bus_type == "ร่วมบริการ.รถธรรมดา")
                    {
                        Article article = new Article() { btcontent = t.bus_line, bg = @"Assets/bg_nomal1.png", Start = t.bus_start, Stop = t.bus_stop, ImagePath = @"Assets/arrow.png" };
                        articles.Add(article);
                    }
                    else if (t.bus_type == "ร่วมบริการ.ปอ.")
                    {
                        Article article = new Article() { btcontent = t.bus_line, bg = @"Assets/bg_nomal3.png", Start = t.bus_start, Stop = t.bus_stop, ImagePath = @"Assets/arrow.png" };
                        articles.Add(article);
                    }
                    else if (t.bus_type == "มินิบัส.รถธรรมดา")
                    {
                        Article article = new Article() { btcontent = t.bus_line, bg = @"Assets/bg_nomal3.png", Start = t.bus_start, Stop = t.bus_stop, ImagePath = @"Assets/arrow.png" };
                        articles.Add(article);
                    }
                }
                listbox1.DataContext = articles;
            });
        }
        private void Button_Click_9(object sender, RoutedEventArgs e)
        {
            //load data 
            SQLiteCommand sqlComm = new SQLiteCommand(dbConn);
            sqlComm.CommandText = "SELECT * FROM busline where bus_type !='' and bus_line LIKE 'ปอ.%' Group by bus_line";
            List<busline> retrievedTasks = retrievedTasks = sqlComm.ExecuteQuery<busline>();
          //  listbox1.Items.Clear();
            Dispatcher.BeginInvoke(() =>
            {
                StackPanel panel = new StackPanel();
                var articles = new List<Article>();
                panel.Orientation = System.Windows.Controls.Orientation.Vertical;
                foreach (var t in retrievedTasks)
                {
                    if (t.bus_type == "ขสมก.รถธรรมดา")
                    {
                        Article article = new Article() { btcontent = t.bus_line, bg = @"Assets/bg_nomal.png", Start = t.bus_start, Stop = t.bus_stop, ImagePath = @"Assets/arrow.png" };
                        articles.Add(article);
                    }
                    else if (t.bus_type == "ขสมก.ปอ.")
                    {
                        Article article = new Article() { btcontent = t.bus_line, bg = @"Assets/bg_nomal2.png", Start = t.bus_start, Stop = t.bus_stop, ImagePath = @"Assets/arrow.png" };
                        articles.Add(article);
                    }
                    else if (t.bus_type == "ร่วมบริการ.รถธรรมดา")
                    {
                        Article article = new Article() { btcontent = t.bus_line, bg = @"Assets/bg_nomal1.png", Start = t.bus_start, Stop = t.bus_stop, ImagePath = @"Assets/arrow.png" };
                        articles.Add(article);
                    }
                    else if (t.bus_type == "ร่วมบริการ.ปอ.")
                    {
                        Article article = new Article() { btcontent = t.bus_line, bg = @"Assets/bg_nomal3.png", Start = t.bus_start, Stop = t.bus_stop, ImagePath = @"Assets/arrow.png" };
                        articles.Add(article);
                    }
                    else if (t.bus_type == "มินิบัส.รถธรรมดา")
                    {
                        Article article = new Article() { btcontent = t.bus_line, bg = @"Assets/bg_nomal3.png", Start = t.bus_start, Stop = t.bus_stop, ImagePath = @"Assets/arrow.png" };
                        articles.Add(article);
                    }
                }
                listbox1.DataContext = articles;
            });
        }
        private void Button_Click_10(object sender, RoutedEventArgs e)
        {
            //load data 
            SQLiteCommand sqlComm = new SQLiteCommand(dbConn);
            sqlComm.CommandText = "SELECT * FROM busline where bus_type !='' and bus_line LIKE 'ต%' Group by bus_line";
            List<busline> retrievedTasks = retrievedTasks = sqlComm.ExecuteQuery<busline>();
           // listbox1.Items.Clear();
            Dispatcher.BeginInvoke(() =>
            {
                StackPanel panel = new StackPanel();
                var articles = new List<Article>();
                panel.Orientation = System.Windows.Controls.Orientation.Vertical;
                foreach (var t in retrievedTasks)
                {
                    if (t.bus_type == "ขสมก.รถธรรมดา")
                    {
                        Article article = new Article() { btcontent = t.bus_line, bg = @"Assets/bg_nomal.png", Start = t.bus_start, Stop = t.bus_stop, ImagePath = @"Assets/arrow.png" };
                        articles.Add(article);
                    }
                    else if (t.bus_type == "ขสมก.ปอ.")
                    {
                        Article article = new Article() { btcontent = t.bus_line, bg = @"Assets/bg_nomal2.png", Start = t.bus_start, Stop = t.bus_stop, ImagePath = @"Assets/arrow.png" };
                        articles.Add(article);
                    }
                    else if (t.bus_type == "ร่วมบริการ.รถธรรมดา")
                    {
                        Article article = new Article() { btcontent = t.bus_line, bg = @"Assets/bg_nomal1.png", Start = t.bus_start, Stop = t.bus_stop, ImagePath = @"Assets/arrow.png" };
                        articles.Add(article);
                    }
                    else if (t.bus_type == "ร่วมบริการ.ปอ.")
                    {
                        Article article = new Article() { btcontent = t.bus_line, bg = @"Assets/bg_nomal3.png", Start = t.bus_start, Stop = t.bus_stop, ImagePath = @"Assets/arrow.png" };
                        articles.Add(article);
                    }
                    else if (t.bus_type == "มินิบัส.รถธรรมดา")
                    {
                        Article article = new Article() { btcontent = t.bus_line, bg = @"Assets/bg_nomal3.png", Start = t.bus_start, Stop = t.bus_stop, ImagePath = @"Assets/arrow.png" };
                        articles.Add(article);
                    }
                }
                listbox1.DataContext = articles;
            });
        }
        private void Button_Click_11(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_bus_line.xaml", UriKind.Relative));
        }
        private void Button_Click_12(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_BusStop.xaml", UriKind.Relative));
        }
        private void Button_Click_13(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_BusCoordinates.xaml", UriKind.Relative));
        }
        private void Button_Click_14(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_BusStartStop.xaml", UriKind.Relative));
        }
        private void close_Click(object sender, RoutedEventArgs e)
        {
            rightmenu.Visibility = System.Windows.Visibility.Collapsed;
            rightmenux.Visibility = System.Windows.Visibility.Collapsed;
            close.Visibility = System.Windows.Visibility.Collapsed;
        }
        private void btTopMenu_Click(object sender, RoutedEventArgs e)
        {
            rightmenux.Visibility = Visibility;
            rightmenu.Visibility = Visibility;
            close.Visibility = Visibility;
        }
        private void rhome_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_AppTh.xaml", UriKind.Relative));
        }
        private void rbusline_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_bus_line.xaml", UriKind.Relative));
        }
        private void rbusstop_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_BusStop.xaml", UriKind.Relative));
        }
        private void rcoor_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_BusCoordinates.xaml", UriKind.Relative));
        }
        private void rbusstartstop_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_BusStartStop.xaml", UriKind.Relative));
        }
        private void rbusspeed_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_Speed_history.xaml", UriKind.Relative));
        }
        private void rbusnew_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_EventNew.xaml", UriKind.Relative));
        }
    }
}