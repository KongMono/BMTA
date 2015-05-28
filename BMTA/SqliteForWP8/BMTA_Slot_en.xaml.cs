using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ShakeGestures;
using System.IO;
using System.Windows.Threading;
using Microsoft.Phone.Net.NetworkInformation;
//using System.Windows.Markup.XamlParseException;
using Microsoft.Xna.Framework;

namespace BMTA
{
    public partial class BMTA_Slot_en : PhoneApplicationPage
    {
        public BMTA_Slot_en()
        {
            ShakeGesturesHelper.Instance.ShakeGesture += new EventHandler<ShakeGestureEventArgs>(Instance_ShakeGesture);
            ShakeGesturesHelper.Instance.MinimumRequiredMovesForShake = 6;
            ShakeGesturesHelper.Instance.Active = true;
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
                        ImageSource = new System.Windows.Media.Imaging.BitmapImage(new Uri("/Assets/480x852/BMTA_main_bg_en.png", UriKind.Relative)),
                        Opacity = 1d
                    };
                    this.LayoutRoot.Background = brush;
                    brush.Stretch = Stretch.Fill;
                }
                else if (x == "720")
                {
                    ImageBrush brush = new ImageBrush
                    {
                        ImageSource = new System.Windows.Media.Imaging.BitmapImage(new Uri("/Assets/720x1280/BMTA_slot_bg_en.png", UriKind.Relative)),
                        Opacity = 1d
                    };
                    this.LayoutRoot.Background = brush;
                    brush.Stretch = Stretch.Fill;
                }
                else
                {
                    ImageBrush brush = new ImageBrush
                    {
                        ImageSource = new System.Windows.Media.Imaging.BitmapImage(new Uri("/Assets/768x1280/BMTA_main_bg_en.png", UriKind.Relative)),
                        Opacity = 1d
                    };
                    this.LayoutRoot.Background = brush;
                    brush.Stretch = Stretch.Fill;
                }

                txtnumber.Text = "";
                txtnumber.Focus();
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

        // Set the data context of the TextBlock to the answer.
        void Instance_ShakeGesture(object sender, ShakeGestureEventArgs e)
        {
            //if (txtnumber.Text.Length < 7)
            //{
            //    MessageBox.Show("กรุณาใส่จำนวนตัวเลขให้ครบ 7หลัก");
            //    txtnumber.Focus();
            //    return;
            //}
            // Use BeginInvoke to write to the UI thread.
            txtnumber.Dispatcher.BeginInvoke(() =>
            {
                if (txtnumber.Text.Length < 7)
                {
                    MessageBox.Show("กรุณาใส่จำนวนตัวเลขให้ครบ 7หลัก");
                    txtnumber.Focus();
                    return;
                }
                else
                {
                    int len = txtnumber.Text.Length;
                    int sum = 0;
                    string pasteText = txtnumber.Text;
                    for (int i = 0; i < pasteText.Length; i++)
                    {
                        if (char.IsDigit(pasteText[i]))
                            sum += Convert.ToInt32(pasteText[i].ToString());
                    }
                    string spintext = sum.ToString();
                    int result = 0;
                    result = Convert.ToInt32(spintext[1].ToString());
                    NavigationService.Navigate(new Uri("/BMTA_Slot_Result_en.xaml?parameter=" + result, UriKind.Relative));
                }
            });
            textBlock1.Dispatcher.BeginInvoke(() =>
            {
                // textBlock1.DataContext = GetAnswer();
                //(Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri("/BMTA_Slot_Result.xaml", UriKind.RelativeOrAbsolute)); 
            });
        }

        void checkInput(int length)
        {
            if (length < 7)
            {
                return;
            }
            else
            {
                int len = txtnumber.Text.Length;
                int sum = 0;
                string pasteText = txtnumber.Text;
                for (int i = 0; i < pasteText.Length; i++)
                {
                    if (char.IsDigit(pasteText[i]))
                        sum += Convert.ToInt32(pasteText[i].ToString());
                }
                string spintext = sum.ToString();
                int result = 0;
                // for (int x = 0; x < spintext.Length; x++)
                // {
                result = Convert.ToInt32(spintext[1].ToString());
                //  }

                // (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri("/BMTA_Slot_Result.xaml?parameter="+result, UriKind.RelativeOrAbsolute));
                NavigationService.Navigate(new Uri("/BMTA_Slot_Result.xaml?parameter=" + result, UriKind.Relative));

            }
        }

        private string GetAnswer()
        {
            Random random = new Random();
            int randomNumber = random.Next(Answers.Count);
            return Answers[randomNumber];
        }

        // List of answers.
        private List<string> answersValue;
        public List<string> Answers
        {
            get
            {
                if (answersValue == null)
                    LoadAnswers();

                return answersValue;
            }
        }

        // Load the answers from the text file.
        private void LoadAnswers()
        {
            answersValue = new List<string>();

            using (StreamReader reader =
                new StreamReader(Application.GetResourceStream(new Uri("answers.txt", UriKind.Relative)).Stream))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                    answersValue.Add(line);
            }
        }

        private void txtnumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            // checkInput(txtnumber.Text.Length);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_bus_line_en.xaml", UriKind.Relative));
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_BusStop_en.xaml", UriKind.Relative));
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_BusCoordinates_en.xaml", UriKind.Relative));
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_BusStartStop_en.xaml", UriKind.Relative));
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
            NavigationService.Navigate(new Uri("/BMTA_AppTh_en.xaml", UriKind.Relative));
        }

        private void rbusline_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_bus_line_en.xaml", UriKind.Relative));
        }

        private void rbusstop_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_BusStop_en.xaml", UriKind.Relative));
        }

        private void rcoor_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_BusCoordinates_en.xaml", UriKind.Relative));
        }

        private void rbusstartstop_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_BusStartStop_en.xaml", UriKind.Relative));
        }

        private void rbusspeed_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_Speed_history_en.xaml", UriKind.Relative));
        }

        private void rbusnew_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BMTA_EventNew_en.xaml", UriKind.Relative));
        }

    }
}