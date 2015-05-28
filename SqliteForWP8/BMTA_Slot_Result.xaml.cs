using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using BMTA.Resources;
using SQLite;
using Windows.Storage;
using System.IO;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Phone.Tasks;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Media.PhoneExtensions;
using System.Globalization;
using System.Threading.Tasks;

namespace BMTA
{

    public partial class BMTA_Slot_Result : PhoneApplicationPage
    {
        /// <summary>
        /// The database path.
        /// </summary>
        public static string DB_PATH = Path.Combine(Path.Combine(ApplicationData.Current.LocalFolder.Path, "bmtadatabase.sqlite"));

        /// <summary>
        /// The sqlite connection.
        /// </summary>
        private SQLiteConnection dbConn;

        int Selected_ContactId = 0;

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string parameter = "";

            if (NavigationContext.QueryString.TryGetValue("parameter", out parameter))

                this.label.Text = parameter;

            /// Create the database connection.
            dbConn = new SQLiteConnection(DB_PATH);
            /// Create the table Task, if it doesn't exist.
           // dbConn.DropTable<slot>();
           // dbConn.DeleteAll<slot>();
           // dbConn.Query<slot>("delete from slot", "");
           // dbConn.Query<sqlite_sequence>("delete from sqlite_sequence where name = ?", "your_table_name");
           // dbConn.DeleteAll<slot>();
            dbConn.CreateTable<slot>();

            Selected_ContactId = int.Parse(NavigationContext.QueryString["parameter"]);

            SQLiteCommand sqlComm = new SQLiteCommand(dbConn);
            sqlComm.CommandText = "select * from slot where index_slot =" + Selected_ContactId;

            List<slot> retrievedTasks = sqlComm.ExecuteQuery<slot>();
            foreach (var t in retrievedTasks)
            {
                //TaskListBox.Items.Add(t);
                lblDetail.Text = t.ToString();
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (dbConn != null)
            {
                /// Close the database connection.
                dbConn.Close();
            }
        }

        public BMTA_Slot_Result()
        {
            InitializeComponent();
      
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            rightmenu.Visibility = System.Windows.Visibility.Collapsed;
            rightmenux.Visibility = System.Windows.Visibility.Collapsed;
            close.Visibility = System.Windows.Visibility.Collapsed;

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
                string x = BMTA.clGetResolution.Width.ToString();
                string y = BMTA.clGetResolution.Height.ToString();
                string xy = x + "x" + y;
                if (x == "480")
                {
                    ImageBrush brush = new ImageBrush
                    {
                        ImageSource = new System.Windows.Media.Imaging.BitmapImage(new Uri("/Assets/480x852/BMTA_main_bg.png", UriKind.Relative)),
                        Opacity = 1d
                    };
                    this.LayoutRoot.Background = brush;
                    brush.Stretch = Stretch.Fill;
                }
                else if (x == "720")
                {
                    ImageBrush brush = new ImageBrush
                    {
                        ImageSource = new System.Windows.Media.Imaging.BitmapImage(new Uri("/Assets/720x1280/BMTA_slotresult_bg.png", UriKind.Relative)),
                        Opacity = 1d
                    };
                    this.LayoutRoot.Background = brush;
                    brush.Stretch = Stretch.Fill;
                }
                else
                {
                    ImageBrush brush = new ImageBrush
                    {
                        ImageSource = new System.Windows.Media.Imaging.BitmapImage(new Uri("/Assets/768x1280/BMTA_main_bg.png", UriKind.Relative)),
                        Opacity = 1d
                    };
                    this.LayoutRoot.Background = brush;
                    brush.Stretch = Stretch.Fill;
                }

               
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

        private void Button_Click(object sender, RoutedEventArgs e)

        {
            SaveToMediaLibrary(LayoutRoot);
        }

        public static void SaveToMediaLibrary(FrameworkElement element)
        {
            try
            {
                var bmp = new WriteableBitmap(element, null);

                var ms = new MemoryStream();
                bmp.SaveJpeg(ms, bmp.PixelWidth, bmp.PixelHeight, 0, 100);
                ms.Seek(0, SeekOrigin.Begin);

                var lib = new MediaLibrary();
                var filePath = string.Format(DateTime.Now + "_" + "BMTASlotResult.jpg");
               // lib.SavePicture(filePath, ms);

               // WriteableBitmap wb = new WriteableBitmap(element, null);
               // wb = wb.FromStream(ms);
               // wb = wb.Crop((wb.PixelWidth / 2) - 100, (wb.PixelHeight / 2) - 100, wb.PixelWidth, 700);

                var bmpc = new WriteableBitmap(0, 0).FromStream(ms);//.FromContent("Assets/ApplicationIcon.png");//fromfile
                var croppedBmp = bmpc.Crop(0, 0, bmpc.PixelWidth, 600);
               // croppedBmp.SaveToMediaLibrary(DateTime.Now + "_" + "BMTASlotResultX.jpg");

                var picture = croppedBmp.SaveToMediaLibrary(DateTime.Now + "_" + "BMTASlotResult.jpg"); //lib.SavePicture(filePath, ms);

                ShareMediaTask shareMediaTask = new ShareMediaTask();
                shareMediaTask = new ShareMediaTask();
                shareMediaTask.FilePath = picture.GetPath();
                shareMediaTask.Show();

            }
            catch
            {
                MessageBox.Show(
                    "There was an error. Please disconnect your phone from the computer before saving.",
                    "Cannot save",
                    MessageBoxButton.OK);
            }
        }


        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        /// <summary>
        /// Task class representing the Task table. Each attribute in the class become one attribute in the database.
        /// </summary>
        public sealed class slot
        {
            /// <summary>
            /// You can create an integer primary key and let the SQLite control it.
            /// </summary>
            [PrimaryKey, AutoIncrement]
            public int id { get; set; }

            public int index_slot { get; set; }

            public string slot_detail { get; set; }

            public override string ToString()
            {
                return slot_detail;
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_bus_line.xaml", UriKind.Relative));
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_BusStop.xaml", UriKind.Relative));
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_BusCoordinates.xaml", UriKind.Relative));
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_BusStartStop.xaml", UriKind.Relative));
        }

        private void btback_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
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